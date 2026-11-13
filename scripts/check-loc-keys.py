#!/usr/bin/env python3
"""Check that every localisation key the code uses exists in the English table.

Why this exists: the copied code calls `Loc.T("...")` / `Loc.F("...", args)` with
Chinese source strings, and the English table is keyed by those exact strings.
A key that is missing from the table does not crash - the lookup silently falls
back to Chinese. That is the right runtime behaviour (never throw over a missing
translation) but a terrible failure mode during development: the app just shows
Chinese in one dialog and nobody notices.

CI caught exactly this. The first version of Loc.cs carried 19 hand-picked keys
and a copied test failed on one missing key. This script turns that one-off
discovery into a permanent gate, so the table cannot silently fall behind the
code again.

What it checks:
  1. Every literal Loc.T/Loc.F key in src/DeviceToolkit.Core exists in
     LocTable.g.cs.
  2. LocTable.g.cs is byte-identical to a fresh transcription of upstream
     (delegated to scripts/gen-loctable.py --check when upstream is available).

Keys built by string concatenation are skipped and counted: the check is
conservative by design, since a false negative here is far cheaper than the
false positives that make people ignore a checker.

Usage:
    scripts/check-loc-keys.py
    scripts/check-loc-keys.py --upstream /path/to/logcat-viewer   # also check drift
"""

from __future__ import annotations

import argparse
import re
import subprocess
import sys
from pathlib import Path

SRC = Path("src/DeviceToolkit.Core")
TABLE = Path("src/DeviceToolkit.Core/Localization/LocTable.g.cs")

TABLE_ENTRY = re.compile(r'\["((?:[^"\\]|\\.)*)"\]\s*=')
CALL_LITERAL = re.compile(r'Loc\.[TF]\(\s*"((?:[^"\\]|\\.)*)"')

# A "//" line comment can mention Loc.T("x") as prose - LogParser.cs does exactly
# that when explaining a hash truncation. Anchoring the call to a non-comment
# position is fiddly, so strip line comments first. Block comments are rare in
# this codebase (upstream uses /// doc comments, which are line comments).
LINE_COMMENT = re.compile(r'//.*$', re.MULTILINE)


def table_keys() -> set[str]:
    if not TABLE.exists():
        raise SystemExit(f"FAIL {TABLE} is missing - run scripts/gen-loctable.py")
    return set(TABLE_ENTRY.findall(TABLE.read_text(encoding="utf-8")))


def used_keys() -> dict[str, set[str]]:
    used: dict[str, set[str]] = {}
    for path in sorted(SRC.rglob("*.cs")):
        if path == TABLE:
            continue
        text = path.read_text(encoding="utf-8", errors="replace")
        text = LINE_COMMENT.sub("", text)
        for key in CALL_LITERAL.findall(text):
            used.setdefault(key, set()).add(path.name)
    return used


def main() -> int:
    ap = argparse.ArgumentParser()
    ap.add_argument("--upstream", type=Path,
                    help="also verify the generated table matches upstream")
    args = ap.parse_args()

    keys = table_keys()
    used = used_keys()
    missing = {k: v for k, v in used.items() if k not in keys}

    if missing:
        for key, files in sorted(missing.items()):
            print(f"MISSING {key!r} (used by {', '.join(sorted(files))})", file=sys.stderr)
        print(f"FAIL {len(missing)} of {len(used)} keys are absent from the table",
              file=sys.stderr)
        return 1

    print(f"OK   all {len(used)} literal Loc keys used by the core exist in the "
          f"{len(keys)}-key table")

    if args.upstream:
        result = subprocess.run(
            [sys.executable, "scripts/gen-loctable.py", "--upstream", str(args.upstream),
             "--check"])
        if result.returncode != 0:
            return result.returncode

    return 0


if __name__ == "__main__":
    sys.exit(main())
