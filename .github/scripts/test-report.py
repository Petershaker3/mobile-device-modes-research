#!/usr/bin/env python3
r"""Turn xUnit trx results into GitHub check-run annotations.

Why this exists: the CI runner uploads trx files as artifacts and writes the
console log, but those two channels are hard to reach from a remote session
(artifact download needs auth plumbing, and log retrieval has been returning
EOF). A check-run annotation, by contrast, is a single small API call and is
also what GitHub shows inline on the diff. So we convert the test outcome into
annotations, one per failing test, plus a summary.

Known platform differences
--------------------------
Upstream is a Windows-only application, so a handful of the copied tests assert
Windows behaviour outright: that `Z:\no-access\x.log` round-trips through
`Path.GetFullPath` unchanged, or that a directory cannot be deleted while
another handle holds a file open. Those expectations are false on Linux, where
`Z:\no-access` is an ordinary relative filename and unlinking an open file is
allowed.

The honest way to run such a suite cross-platform is not to weaken the tests but
to pin the difference down and check it in both directions:

  * a test fails that is not on the allow-list  -> error (new regression)
  * a test passes that IS on the allow-list     -> error (stale allow-list)

That second rule is the important one: it stops the allow-list from silently
rotting into a place where real breakage hides. When a fix lands, the stale
entry fails loudly and someone must delete the line.

Usage:
    test-report.py TestResults --strict
    test-report.py TestResults --allow ci/known-linux-diffs.txt

Exit status
-----------
0 always, unless --strict is given, in which case any unexplained failure exits
non-zero. The default is deliberately 0: this script is a reporter, and a bug in
it must not be mistaken for, or mask, the real test result. --strict is used on
Windows, where "any failure at all" is by definition unexplained.
"""

from __future__ import annotations

import sys
import xml.etree.ElementTree as ET
from pathlib import Path


def localname(tag: str) -> str:
    return tag.rsplit("}", 1)[-1]


def find_all(root: ET.Element, name: str) -> list[ET.Element]:
    """Namespace-agnostic search, so a trx format bump cannot silently yield 0."""
    return [el for el in root.iter() if localname(el.tag) == name]


def emit(kind: str, message: str, file: str | None = None) -> None:
    message = " ".join(message.split())
    if len(message) > 900:
        message = message[:900] + "..."
    if file:
        print(f"::{kind} file={file}::{message}")
    else:
        print(f"::{kind}::{message}")


def load_allow_list(path: Path | None) -> set[str]:
    if path is None:
        return set()
    names: set[str] = set()
    for raw in path.read_text(encoding="utf-8").splitlines():
        line = raw.strip()
        if line and not line.startswith("#"):
            names.add(line)
    return names


def parse_trx(path: Path) -> tuple[int, int, int, list[tuple[str, str]]]:
    root = ET.parse(path).getroot()
    results = find_all(root, "UnitTestResult")
    passed = sum(1 for r in results if r.get("outcome") == "Passed")
    failed = sum(1 for r in results if r.get("outcome") == "Failed")
    skipped = sum(1 for r in results if r.get("outcome") in ("NotExecuted", "Inconclusive"))

    failures: list[tuple[str, str]] = []
    for r in results:
        if r.get("outcome") != "Failed":
            continue
        name = r.get("testName") or "(unnamed test)"
        detail = ""
        for node in r.iter():
            if localname(node.tag) in ("Message", "StackTrace") and node.text:
                detail = node.text.strip()
                break
        failures.append((name, detail or "no message captured"))
    return passed, failed, skipped, failures


def main(argv: list[str]) -> int:
    args = argv[1:]
    strict = "--strict" in args
    if strict:
        args.remove("--strict")
    allow_path: Path | None = None
    if "--allow" in args:
        i = args.index("--allow")
        allow_path = Path(args[i + 1])
        del args[i:i + 2]
    results_dir = Path(args[0] if args else "TestResults")

    allowed = load_allow_list(allow_path)
    trx_files = sorted(results_dir.rglob("*.trx"))

    if not trx_files:
        emit("error", f"no .trx files under {results_dir}; the test step never ran")
        return 0

    passed = failed = skipped = 0
    failures: list[tuple[str, str]] = []
    for trx in trx_files:
        try:
            p, f, s, fails = parse_trx(trx)
        except ET.ParseError as exc:
            emit("warning", f"{trx.name} is not parseable: {exc}")
            continue
        passed += p
        failed += f
        skipped += s
        failures.extend(fails)

    total = passed + failed + skipped

    expected = [(n, d) for n, d in failures if n in allowed]
    unexpected = [(n, d) for n, d in failures if n not in allowed]
    failed_names = {n for n, _ in failures}
    stale = sorted(allowed - failed_names)

    for name, detail in unexpected:
        emit("error", f"{name} - {detail}")
    for name, detail in expected:
        emit("warning", f"known platform difference, not a regression: {name} - {detail}")
    for name in stale:
        emit("error", f"{name} is on the allow-list but passed - delete the stale entry")

    if unexpected or stale:
        emit(
            "error",
            f"{failed} of {total} tests failed: {len(unexpected)} unexpected, "
            f"{len(expected)} known platform differences, {len(stale)} stale entries "
            f"({passed} passed, {skipped} skipped)",
        )
    elif expected:
        emit(
            "notice",
            f"{passed} of {total} tests passed; {len(expected)} known platform "
            f"differences accounted for by {'ci/known-linux-diffs.txt' if allow_path else 'the allow-list'}",
        )
    else:
        emit("notice", f"all {passed} tests passed ({skipped} skipped)")

    if strict and (unexpected or stale):
        return 1
    return 0


if __name__ == "__main__":
    sys.exit(main(sys.argv))
