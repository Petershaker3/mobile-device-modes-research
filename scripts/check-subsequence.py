#!/usr/bin/env python3
"""Assert that file A consists of contiguous blocks of file B, in order.

In other words: A must be B with zero or more whole blocks deleted, and nothing
else. No edited line, no reordered line, no inserted line.

Why this exists
---------------
One vendored file (AdbManager.cs) cannot be copied byte-for-byte: it carries a
screenshot feature that needs System.Drawing, which would drag a Windows-only
dependency into a library that must build on Linux. So one contiguous region is
removed and the rest is kept verbatim.

A naive check ("60 lines were removed, that is about right") would happily accept
a file where someone also *changed* a line - the removed-line count would still
match if they removed 61 and added 1. That is exactly the kind of quiet drift
this repository exists to prevent.

So the check is structural: after stripping our provenance header, our file must
be reconstructible from upstream by deleting whole blocks (one block in practice:
the screenshot region). Editing, reordering, inserting or reflowing anything
fails. This is a stronger statement than "the same number of lines came out" -
it pins the content, not the arithmetic.

Usage:
    check-subsequence.py <mine> <upstream>
Exit status 0 when A is a contiguous sub-sequence of B, 1 otherwise.
"""

from __future__ import annotations

import sys
from pathlib import Path


def read_lines(path: Path) -> list[str]:
    return path.read_text(encoding="utf-8", errors="replace").splitlines()


def split_into_runs(mine: list[str], up: list[str]) -> list[tuple[int, int]] | None:
    """Split `mine` into maximal runs that each appear contiguously in `up`.

    Returns a list of (start_in_up, length) in order, or None if some line of
    `mine` does not appear in `up` at all in a contiguous run.
    """
    runs: list[tuple[int, int]] = []
    i = 0
    while i < len(mine):
        best: tuple[int, int] | None = None
        for start in range(len(up)):
            if up[start] != mine[i]:
                continue
            run = 0
            while (start + run < len(up) and i + run < len(mine)
                   and up[start + run] == mine[i + run]):
                run += 1
            if best is None or run > best[1]:
                best = (start, run)
        if best is None or best[1] == 0:
            return None
        runs.append(best)
        i += best[1]
    return runs


def describe_first_divergence(mine: list[str], up: list[str]) -> str:
    """Best-effort explanation so a CI annotation says something useful."""
    for idx, (a, b) in enumerate(zip(mine, up)):
        if a != b:
            return (f"first divergence at line {idx + 1}: "
                    f"ours={a.strip()[:100]!r} upstream={b.strip()[:100]!r}")
    return f"line counts differ: ours={len(mine)} upstream={len(up)}"


def main(argv: list[str]) -> int:
    if len(argv) != 3:
        print(__doc__.strip().splitlines()[0], file=sys.stderr)
        return 2

    mine = read_lines(Path(argv[1]))
    up = read_lines(Path(argv[2]))

    if not mine:
        print("FAIL: our file is empty", file=sys.stderr)
        return 1

    runs = split_into_runs(mine, up)
    if runs is None:
        print("FAIL: some line of ours does not appear verbatim in upstream.", file=sys.stderr)
        print("  " + describe_first_divergence(mine, up), file=sys.stderr)
        print(f"  ours={len(mine)} lines, upstream={len(up)} lines", file=sys.stderr)
        return 1

    # Runs must appear in increasing order in upstream: a reordering would still
    # split into runs, so ordering has to be checked separately.
    starts = [s for s, _ in runs]
    if starts != sorted(starts):
        print("FAIL: blocks of our file appear out of upstream order.", file=sys.stderr)
        print(f"  run starts in upstream: {starts}", file=sys.stderr)
        return 1

    # Report what was deleted, so the verifier can print it for a human.
    covered = sum(length for _, length in runs)
    deleted = len(up) - covered
    print(f"OK: {len(runs)} contiguous block(s), {covered} lines kept, "
          f"{deleted} upstream lines deleted")
    return 0


if __name__ == "__main__":
    sys.exit(main(sys.argv))
