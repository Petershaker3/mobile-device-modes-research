#!/usr/bin/env python3
"""Turn xUnit trx results into GitHub check-run annotations.

Why this exists: the CI runner uploads trx files as artifacts and writes the
console log, but those two channels are hard to reach from a remote session
(artifact download needs auth plumbing, and log retrieval has been returning
EOF). A check-run annotation, by contrast, is a single small API call and is
also what GitHub shows inline on the diff. So we convert the test outcome into
annotations, one per failing test, plus a summary.

Emits:
  ::error file=...,line=...::message   for each failed test
  ::notice::summary                    always (so a green matrix is legible too)

Exit code is 0 unless the report itself cannot be produced: the annotations are
a diagnostic aid, and a script bug must not mask the real test result.
"""

from __future__ import annotations

import sys
import xml.etree.ElementTree as ET
from pathlib import Path

NS = {"t": "http://microsoft.com/schemas/VisualStudio/TeamTest/2010"}


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


def parse_trx(path: Path) -> tuple[int, int, int, list[tuple[str, str, str]]]:
    root = ET.parse(path).getroot()
    results = find_all(root, "UnitTestResult")
    passed = sum(1 for r in results if r.get("outcome") == "Passed")
    failed = sum(1 for r in results if r.get("outcome") == "Failed")
    skipped = sum(1 for r in results if r.get("outcome") in ("NotExecuted", "Inconclusive"))

    failures: list[tuple[str, str, str]] = []
    for r in results:
        if r.get("outcome") != "Failed":
            continue
        name = r.get("testName") or "(unnamed test)"
        detail = ""
        for node in r.iter():
            ln = localname(node.tag)
            if ln in ("Message", "StackTrace") and node.text:
                detail += node.text.strip() + "\n"
                break
        failures.append((name, detail.strip() or "no message captured", ""))
    return passed, failed, skipped, failures


def main(argv: list[str]) -> int:
    results_dir = Path(argv[1] if len(argv) > 1 else "TestResults")
    trx_files = sorted(results_dir.rglob("*.trx"))

    if not trx_files:
        emit("error", f"no .trx files under {results_dir}; the test step never ran")
        return 0

    total = [0, 0, 0]
    all_failures: list[tuple[str, str, str]] = []
    for trx in trx_files:
        try:
            p, f, s, failures = parse_trx(trx)
        except ET.ParseError as exc:
            emit("warning", f"{trx.name} is not parseable: {exc}")
            continue
        total[0] += p
        total[1] += f
        total[2] += s
        all_failures.extend(failures)

    passed, failed, skipped = total
    if failed:
        for name, detail, _ in all_failures:
            emit("error", f"{name} - {detail}")
        emit(
            "error",
            f"{failed} of {passed + failed + skipped} tests failed "
            f"({passed} passed, {skipped} skipped)",
        )
    else:
        emit("notice", f"all {passed} tests passed ({skipped} skipped)")

    return 0


if __name__ == "__main__":
    sys.exit(main(sys.argv))
