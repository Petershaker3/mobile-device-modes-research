#!/usr/bin/env bash
#
# Proves the transplanted code is intact and actually builds.
#
# Runs three independent checks:
#   1. every copied file is byte-identical to upstream at the pinned commit
#      (TestHelpers.cs is the single documented exception: two hunks removed)
#   2. the core library has no Windows-only dependency
#   3. it compiles and the copied tests pass
#
# Requires: git, dotnet SDK 10, network access to github.com and nuget.org.
#
# Usage:
#   scripts/verify-transplant.sh             # all checks incl. build+test
#   scripts/verify-transplant.sh --no-build  # integrity + portability only
#   scripts/verify-transplant.sh --fetch-only # clone upstream and stop
#
# --fetch-only exists for CI steps that need an upstream checkout for a separate
# purpose (regenerating the translation table) but not the diff itself.
#
set -uo pipefail

REPO_ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
UPSTREAM_REPO="https://github.com/daitouniao/logcat-viewer.git"
UPSTREAM_SHA="a985616224a342d6064ed962eec28ace3f38365a"
UPSTREAM_DIR="${TMPDIR:-/tmp}/logcat-viewer-upstream"

RUN_BUILD=1
FETCH_ONLY=0
for arg in "$@"; do
  case "$arg" in
    --no-build)   RUN_BUILD=0 ;;
    --fetch-only) FETCH_ONLY=1; RUN_BUILD=0 ;;
  esac
done

fail=0
ok()   { printf '  \033[32mOK\033[0m   %s\n' "$1"; }
bad()  { printf '  \033[31mFAIL\033[0m %s\n' "$1"; fail=1; }
head2() { printf '\n\033[1m%s\033[0m\n' "$1"; }

# --------------------------------------------------------------------------
if [ "$FETCH_ONLY" = "1" ]; then
  head2 "fetching upstream @ ${UPSTREAM_SHA:0:8}"
else
  head2 "1/3  integrity: copied files vs upstream @ ${UPSTREAM_SHA:0:8}"
fi

if [ ! -d "$UPSTREAM_DIR/.git" ]; then
  echo "  fetching upstream (one time)..."
  git clone --quiet --filter=blob:none --no-checkout "$UPSTREAM_REPO" "$UPSTREAM_DIR" || {
    bad "cannot clone upstream (no network?)"; exit 1; }
fi
git -C "$UPSTREAM_DIR" checkout --quiet "$UPSTREAM_SHA" 2>/dev/null || {
  bad "cannot check out $UPSTREAM_SHA"; exit 1; }

if [ "$FETCH_ONLY" = "1" ]; then
  ok "upstream ready at $UPSTREAM_DIR"
  exit 0
fi

check_identical() {  # $1 = upstream relative path, $2 = local path
  if [ ! -f "$REPO_ROOT/$2" ]; then bad "missing locally: $2"; return; fi
  if diff -q "$UPSTREAM_DIR/$1" "$REPO_ROOT/$2" >/dev/null 2>&1; then
    ok "$2"
  else
    bad "$2 differs from upstream $1"
  fi
}

for f in LogParser LogDocument FilterEngine; do
  check_identical "logcat-viewer/Services/$f.cs" "src/DeviceToolkit.Core/Logcat/$f.cs"
done
for f in CrashExtractor CrashAggregator CrashTime TextProbe FileProxyService; do
  check_identical "logcat-viewer/Services/$f.cs" "src/DeviceToolkit.Core/Diagnostics/$f.cs"
done
for f in SidecarCache SidecarCacheStore; do
  check_identical "logcat-viewer/Services/$f.cs" "src/DeviceToolkit.Core/Logcat/$f.cs"
done
for f in FileProxyClient FileProxyMode FileProxyException ProxyChannel; do
  check_identical "logcat-viewer/Services/$f.cs" "src/DeviceToolkit.Core/Diagnostics/$f.cs"
done
check_identical "logcat-viewer/Services/LogcatSettings.cs" "src/DeviceToolkit.Core/Infrastructure/LogcatSettings.cs"
for f in DeviceLastDir FavoriteDir DeviceInfo FileEntry TryOpenOutcome AdbServerSelfHealResult CommandEntry; do
  check_identical "logcat-uicore/Models/$f.cs" "src/DeviceToolkit.Core/Models/$f.cs"
done
for f in SettingsIO ConfigMigration LegacySettings LanguageSettings AndToolsSettings; do
  check_identical "logcat-uicore/Services/$f.cs" "src/DeviceToolkit.Core/Infrastructure/$f.cs"
done
check_identical "logcat-uicore/Services/StartupLog.cs" "src/DeviceToolkit.Core/Infrastructure/StartupLog.cs"
for f in CrashEvent FilterSpec; do
  check_identical "logcat-viewer/Models/$f.cs" "src/DeviceToolkit.Core/Diagnostics/$f.cs"
done
for f in LogParserTests LogDocumentTests LogDocumentZeroCopyTests LogDocumentProxiedTests \
         FilterEngineTests FilterEngineBlockScanTests ChunkedEquivalenceTests CrashTests \
         TextProbeTests TestCultureBootstrap SidecarCacheTests FileProxyProtocolTests \
         FileProxyLoopbackTests FileProxyChannelStateTests FileProxyModeTests \
         LegacySettingsDtoTests FakeProxyServer; do
  check_identical "tests/logcat.Tests/$f.cs" "src/DeviceToolkit.Tests/$f.cs"
done

# TestHelpers.cs: exactly the WinFormsCollection block may be gone.
if diff -q "$UPSTREAM_DIR/tests/logcat.Tests/TestHelpers.cs" \
           "$REPO_ROOT/src/DeviceToolkit.Tests/TestHelpers.cs" >/dev/null 2>&1; then
  ok "TestHelpers.cs (identical — no shim needed after all)"
else
  removed_lines=$(diff "$UPSTREAM_DIR/tests/logcat.Tests/TestHelpers.cs" \
                       "$REPO_ROOT/src/DeviceToolkit.Tests/TestHelpers.cs" \
                  | grep -c '^<' || true)
  if grep -q "WinFormsCollection" "$UPSTREAM_DIR/tests/logcat.Tests/TestHelpers.cs" \
     && ! grep -q "class WinFormsCollection" "$REPO_ROOT/src/DeviceToolkit.Tests/TestHelpers.cs" \
     && [ "$removed_lines" -le 12 ]; then
    ok "TestHelpers.cs (documented shim: WinFormsCollection removed, $removed_lines lines)"
  else
    bad "TestHelpers.cs changed beyond the documented shim ($removed_lines removed lines)"
  fi
fi

# --------------------------------------------------------------------------
head2 "2/3  portability: no Windows-only dependency in the core"

# What actually breaks portability: WinForms, WPF, Win32 P-Invoke, and WinRT.
# `using System.Drawing;` alone does NOT break it: System.Drawing.Primitives
# (Point, Size, Rectangle, Color) ships in the shared framework on every OS.
# LogcatSettings uses exactly Point and Size for window geometry, so banning the
# whole namespace would be a false positive. Anything beyond the primitives
# (Bitmap, Graphics, Icon) is caught by the second pattern.
FORBIDDEN='System\.Windows\.Forms|System\.Windows\.Media|System\.Windows\.Controls|DllImport|LibraryImport|WinRT|Microsoft\.Win32'
DRAWING_IMPL='new Bitmap|new Graphics|Graphics\.FromImage|System\.Drawing\.Imaging|new Icon\(|SystemIcons\.'

if grep -rEn "$FORBIDDEN" "$REPO_ROOT/src/DeviceToolkit.Core" --include=*.cs >/dev/null 2>&1; then
  bad "core references a Windows-only framework or P-Invoke:"
  grep -rEn "$FORBIDDEN" "$REPO_ROOT/src/DeviceToolkit.Core" --include=*.cs | sed 's/^/       /'
elif grep -rEn "$DRAWING_IMPL" "$REPO_ROOT/src/DeviceToolkit.Core" --include=*.cs >/dev/null 2>&1; then
  bad "core uses System.Drawing beyond the portable primitives:"
  grep -rEn "$DRAWING_IMPL" "$REPO_ROOT/src/DeviceToolkit.Core" --include=*.cs | sed 's/^/       /'
else
  ok "no WinForms / WPF / P-Invoke / non-portable Drawing in src/DeviceToolkit.Core"
fi

# --------------------------------------------------------------------------
if [ "$RUN_BUILD" -eq 0 ]; then
  head2 "3/3  build: skipped (--no-build)"
else
  head2 "3/3  build + test"
  if ! command -v dotnet >/dev/null 2>&1; then
    bad "dotnet SDK not found on PATH"
  else
    echo "  dotnet $(dotnet --version)"
    if ( cd "$REPO_ROOT" && dotnet build DeviceToolkit.slnx -c Release --nologo -v quiet ); then
      ok "build succeeds"
      if ( cd "$REPO_ROOT" && dotnet test DeviceToolkit.slnx -c Release --nologo --no-build ); then
        ok "copied tests pass"
      else
        bad "tests fail"
      fi
    else
      bad "build fails"
    fi
  fi
fi

# --------------------------------------------------------------------------
if [ "$fail" -ne 0 ]; then
  printf '\n\033[31mone or more checks failed\033[0m\n'
  exit 1
fi
printf '\n\033[32mall checks passed\033[0m\n'
