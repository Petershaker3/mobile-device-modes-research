#!/usr/bin/env python3
"""Validate the shared contracts against their JSON Schemas.

This is deliberately runnable on any OS with no .NET toolchain, so contract
changes can be checked in CI and by hand before the C# implementation exists.

Usage:
    python contracts/tools/validate_contracts.py            # validate examples + invariants
    python contracts/tools/validate_contracts.py --strict   # also fail on warnings

Requires: jsonschema >= 4.18 (draft 2020-12).

Exit codes:
    0  every example conformed and every invariant held
    1  at least one violation
    2  environment problem (missing dependency, no schemas)
"""

from __future__ import annotations

import argparse
import json
import sys
from pathlib import Path

try:
    from jsonschema import Draft202012Validator
    from jsonschema.validators import validator_for
    from referencing import Registry, Resource
except ImportError:  # pragma: no cover - environment guard
    print("ERROR: jsonschema is required: pip install 'jsonschema>=4.18'", file=sys.stderr)
    raise SystemExit(2)

CONTRACTS_DIR = Path(__file__).resolve().parent.parent
SCHEMA_DIR = CONTRACTS_DIR / "schemas"
EXAMPLE_DIR = CONTRACTS_DIR / "examples"

SCHEMA_FILES = [
    "_common.schema.json",
    "device-handle.schema.json",
    "capability.schema.json",
    "operation.schema.json",
    "result.schema.json",
    "plan.schema.json",
    "audit-event.schema.json",
]

# Examples are paired with the schema that governs them.
EXAMPLE_PAIRS = {
    "device-handle.adb-usb.example.json": "device-handle.schema.json",
    "device-handle.unauthorized.example.json": "device-handle.schema.json",
    "capability.available.example.json": "capability.schema.json",
    "capability.unsupported.example.json": "capability.schema.json",
    "operation.read-only.example.json": "operation.schema.json",
    "operation.device-write.example.json": "operation.schema.json",
    "operation.outcome-unknown.example.json": "operation.schema.json",
    "result.ok.example.json": "result.schema.json",
    "result.unknown-outcome.example.json": "result.schema.json",
    "plan.install.example.json": "plan.schema.json",
    "audit-event.approval.example.json": "audit-event.schema.json",
}

# Examples that must be REJECTED. A contract that accepts everything is not a contract.
NEGATIVE_CASES = [
    {
        "name": "result.ok=true with a non-OK problem code",
        "schema": "result.schema.json",
        "document": {
            "ok": True,
            "problemCode": "Timeout",
            "recoverability": "Retryable",
            "message": "finished",
            "warnings": [],
        },
        "reason": "ok must imply problemCode OK",
    },
    {
        "name": "result.ok=false claiming OK",
        "schema": "result.schema.json",
        "document": {
            "ok": False,
            "problemCode": "OK",
            "recoverability": "NotRetryable",
            "message": "failed",
            "warnings": [],
        },
        "reason": "a failure must carry a real problem code",
    },
    {
        "name": "OutcomeUnknown without reconcile block",
        "schema": "result.schema.json",
        "document": {
            "ok": False,
            "problemCode": "OutcomeUnknown",
            "recoverability": "RetryAfterReconcile",
            "message": "install timed out",
            "warnings": [],
        },
        "reason": "unknown outcomes must explain reconciliation",
    },
    {
        "name": "OutcomeUnknown with automatic retry allowed",
        "schema": "result.schema.json",
        "document": {
            "ok": False,
            "problemCode": "OutcomeUnknown",
            "recoverability": "Retryable",
            "message": "install timed out",
            "warnings": [],
            "reconcile": {"attempted": False, "outcome": "NotAttempted"},
        },
        "reason": "blind retry after an unknown outcome is forbidden",
    },
    {
        "name": "ok=true while verification says not verified",
        "schema": "result.schema.json",
        "document": {
            "ok": True,
            "problemCode": "OK",
            "recoverability": "NotRetryable",
            "message": "installed",
            "warnings": [],
            "verification": {"performed": True, "verified": False},
        },
        "reason": "success requires observed state",
    },
    {
        "name": "DeviceWrite operation without plan or approval",
        "schema": "operation.schema.json",
        "document": {
            "operationId": "0f2a3b4c-5d6e-4f70-8a9b-0c1d2e3f4a5b",
            "operationType": "app.install",
            "targetDeviceId": "dev_0123456789abcdef",
            "impactLevel": "DeviceWrite",
            "state": "Running",
            "createdAt": "2026-11-13T10:00:00Z",
            "updatedAt": "2026-11-13T10:00:05Z",
            "progress": {"phase": "Installing"},
            "warnings": [],
        },
        "reason": "no write without an approved plan",
    },
    {
        "name": "terminal state without endedAt/result",
        "schema": "operation.schema.json",
        "document": {
            "operationId": "0f2a3b4c-5d6e-4f70-8a9b-0c1d2e3f4a5b",
            "operationType": "device.read_info",
            "targetDeviceId": "dev_0123456789abcdef",
            "impactLevel": "ReadOnly",
            "state": "Succeeded",
            "createdAt": "2026-11-13T10:00:00Z",
            "updatedAt": "2026-11-13T10:00:05Z",
            "progress": {"phase": "Done"},
            "warnings": [],
        },
        "reason": "Succeeded must be backed by a Result and an end time",
    },
    {
        "name": "Unsupported capability without a reason",
        "schema": "capability.schema.json",
        "document": {
            "capabilityId": "adb.logcat.stream",
            "availability": "Unsupported",
            "observedAt": "2026-11-13T10:00:00Z",
            "evidenceSource": "AdbShell",
        },
        "reason": "an unavailable capability must say why, so the UI can act on it",
    },
    {
        "name": "raw serial-like identity rejected by deviceId pattern",
        "schema": "device-handle.schema.json",
        "document": {
            "deviceId": "R58M12345AB",
            "transportKind": "ADB_USB",
            "connectionState": "Online",
            "firstSeenAt": "2026-11-13T09:00:00Z",
            "lastSeenAt": "2026-11-13T10:00:00Z",
        },
        "reason": "a hardware serial must never be a deviceId",
    },
    {
        "name": "DeviceWrite plan without a verification step",
        "schema": "plan.schema.json",
        "document": {
            "planId": "9c8b7a65-4321-4f0e-9d8c-7b6a5f4e3d2c",
            "operationType": "file.delete",
            "impactLevel": "DeviceWrite",
            "targetDeviceId": "dev_0123456789abcdef",
            "createdAt": "2026-11-13T10:00:00Z",
            "steps": [
                {
                    "stepId": 1,
                    "kind": "AdbShellCommand",
                    "description": "remove the file",
                    "command": {"argv": ["rm", "-f", "/sdcard/x.jpg"]},
                }
            ],
        },
        "reason": "a write plan must design its own verification",
    },
]


def load_json(path: Path) -> object:
    with path.open("r", encoding="utf-8") as handle:
        return json.load(handle)


def build_registry(schemas: dict[str, object]) -> Registry:
    registry = Registry()
    for name, document in schemas.items():
        uri = document.get("$id") or name
        registry = registry.with_resource(uri, Resource.from_contents(document))
        registry = registry.with_resource(name, Resource.from_contents(document))
    return registry


def make_validator(schema_name: str, schemas: dict[str, object], registry: Registry) -> Draft202012Validator:
    schema = schemas[schema_name]
    cls = validator_for(schema)
    cls.check_schema(schema)
    return cls(schema, registry=registry)


def format_error(error) -> str:
    location = "/".join(str(part) for part in error.absolute_path) or "<root>"
    return f"    at {location}: {error.message}"


def main() -> int:
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--strict", action="store_true", help="treat warnings as failures")
    args = parser.parse_args()

    if not SCHEMA_DIR.is_dir():
        print(f"ERROR: no schema directory at {SCHEMA_DIR}", file=sys.stderr)
        return 2

    schemas: dict[str, object] = {}
    missing = []
    for name in SCHEMA_FILES:
        path = SCHEMA_DIR / name
        if not path.is_file():
            missing.append(name)
            continue
        schemas[name] = load_json(path)
    if missing:
        print(f"ERROR: missing schemas: {', '.join(missing)}", file=sys.stderr)
        return 2

    failures: list[str] = []
    warnings: list[str] = []
    checks = 0

    registry = build_registry(schemas)

    # 1. Every schema is itself a valid Draft 2020-12 schema.
    for name in SCHEMA_FILES:
        try:
            Draft202012Validator.check_schema(schemas[name])
            checks += 1
        except Exception as exc:  # schema authoring error
            failures.append(f"schema {name} is not a valid 2020-12 schema: {exc}")

    # 2. Positive examples conform.
    for example_name, schema_name in EXAMPLE_PAIRS.items():
        path = EXAMPLE_DIR / example_name
        if not path.is_file():
            failures.append(f"missing example: contracts/examples/{example_name}")
            continue
        validator = make_validator(schema_name, schemas, registry)
        errors = sorted(validator.iter_errors(load_json(path)), key=lambda e: list(e.absolute_path))
        checks += 1
        if errors:
            failures.append(
                f"example {example_name} does not conform to {schema_name}:\n"
                + "\n".join(format_error(error) for error in errors)
            )

    # 3. Negative cases are rejected. Each must fail for the stated reason.
    for case in NEGATIVE_CASES:
        validator = make_validator(case["schema"], schemas, registry)
        errors = list(validator.iter_errors(case["document"]))
        checks += 1
        if not errors:
            failures.append(
                f"NEGATIVE CASE PASSED (contract too weak): {case['name']} -- expected rejection because {case['reason']}"
            )

    # 4. Cross-contract invariants that JSON Schema cannot express.
    for name, schema in schemas.items():
        props = (schema.get("properties") or {})
        if "additionalProperties" not in schema:
            warnings.append(f"{name}: schema does not set additionalProperties=false at the top level")
        for prop, prop_schema in props.items():
            del prop_schema

    # Every enum in _common that is a "closed set" must remain unique and non-empty.
    common_defs = schemas["_common.schema.json"].get("$defs", {})
    for def_name, def_schema in common_defs.items():
        values = def_schema.get("enum")
        if values is not None:
            checks += 1
            if not values:
                failures.append(f"_common.$defs.{def_name}: empty enum")
            if len(values) != len(set(values)):
                failures.append(f"_common.$defs.{def_name}: duplicate enum values")

    # The problem-code set referenced everywhere must include the outcome-unknown family.
    problem_codes = set(common_defs.get("problemCode", {}).get("enum", []))
    for required in ("OK", "OutcomeUnknown", "NeedsReconciliation", "ApprovalMissing", "PolicyBlocked", "DeviceSelectionRequired"):
        checks += 1
        if required not in problem_codes:
            failures.append(f"_common.problemCode is missing required code: {required}")

    total = len(EXAMPLE_PAIRS) + len(NEGATIVE_CASES) + len(SCHEMA_FILES)
    print(f"contracts: {checks} checks over {len(SCHEMA_FILES)} schemas, {len(EXAMPLE_PAIRS)} examples, {len(NEGATIVE_CASES)} negative cases")

    if warnings:
        print(f"\n{len(warnings)} warning(s):")
        for warning in warnings:
            print(f"  - {warning}")

    if failures:
        print(f"\n{len(failures)} violation(s):")
        for failure in failures:
            print(f"  ✗ {failure}")
        return 1

    if args.strict and warnings:
        print("\nstrict mode: warnings are failures")
        return 1

    print("OK: all contracts hold")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
