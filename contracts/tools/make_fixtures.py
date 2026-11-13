#!/usr/bin/env python3
"""Generate the synthetic, non-sensitive test corpus described in corpus/README.md.

Everything produced here is fake: package names use org.example.*, device models
are generic, and no real identifier (IMEI, serial, MAC, phone number) appears.
Values are internally consistent so parsers can be checked for correctness, not
just for "did not crash".

Categories generated:
    A  Android shell output (getprop, df, pm, dumpsys, settings, fastboot)
    C  Hostile archives (zip slip, absolute path, bomb, many entries, symlink,
       NUL in name, deep nesting)

Categories B (logcat) and D (real packages) are out of scope here: B needs
careful format coverage, D needs real APKs that must not live in git.

Usage:
    python contracts/tools/make_fixtures.py            # write the corpus
    python contracts/tools/make_fixtures.py --check    # verify what is on disk
    python contracts/tools/make_fixtures.py --clean    # remove generated files

Exit codes: 0 ok, 1 verification failure, 2 environment problem.
"""

from __future__ import annotations

import argparse
import hashlib
import json
import re
import shutil
import stat
import sys
import zipfile
from pathlib import Path

CORPUS = Path(__file__).resolve().parent.parent / "corpus"
ANDROID = CORPUS / "android"
MALICIOUS = CORPUS / "malicious"

GENERATOR_VERSION = "1.0.0"
CREATED = "2026-11-13"

# --------------------------------------------------------------------------- #
# Category A: Android shell output
# --------------------------------------------------------------------------- #

GETPROP_PIXEL7 = {
    "ro.product.model": "Pixel 7",
    "ro.product.brand": "google",
    "ro.product.manufacturer": "Google",
    "ro.product.device": "panther",
    "ro.product.name": "panther",
    "ro.product.cpu.abi": "arm64-v8a",
    "ro.product.cpu.abilist": "arm64-v8a,armeabi-v7a,armeabi",
    "ro.build.version.release": "14",
    "ro.build.version.sdk": "34",
    "ro.build.version.security_patch": "2026-09-05",
    "ro.build.version.incremental": "12345678",
    "ro.build.id": "AP2A.240905.003",
    "ro.build.display.id": "AP2A.240905.003",
    "ro.build.fingerprint": (
        "google/panther/panther:14/AP2A.240905.003/12345678:user/release-keys"
    ),
    "ro.build.type": "user",
    "ro.build.tags": "release-keys",
    "ro.build.date.utc": "1725552000",
    "ro.board.platform": "gs201",
    "ro.hardware": "panther",
    "ro.boot.hardware": "panther",
    "ro.boot.bootloader": "slider-1.0-11398904",
    "ro.boot.flash.locked": "1",
    "ro.boot.verifiedbootstate": "green",
    "ro.boot.veritymode": "enforcing",
    "ro.boot.slot_suffix": "_a",
    "sys.oem_unlock_allowed": "0",
    "ro.debuggable": "0",
    "ro.secure": "1",
    "ro.adb.secure": "1",
    "ro.crypto.state": "encrypted",
    "ro.crypto.type": "file",
    "persist.sys.timezone": "Africa/Cairo",
    "ro.product.locale": "en-US",
    "gsm.sim.state": "READY",
    "gsm.network.type": "LTE",
    "ro.opengles.version": "196610",
    "ro.sf.lcd_density": "420",
    "ro.zygote": "zygote64_32",
}

GETPROP_SAMSUNG = {
    "ro.product.model": "SM-A146U",
    "ro.product.brand": "samsung",
    "ro.product.manufacturer": "samsung",
    "ro.product.device": "a14xm",
    "ro.product.cpu.abi": "arm64-v8a",
    "ro.product.cpu.abilist": "arm64-v8a,armeabi-v7a,armeabi",
    "ro.build.version.release": "11",
    "ro.build.version.sdk": "30",
    "ro.build.version.security_patch": "2023-08-01",
    "ro.build.fingerprint": (
        "samsung/a14xmq/a14xm:11/RP1A.200720.012/A146USQS5CWG3:user/release-keys"
    ),
    # Samsung exposes duplicated vendor keys; a parser must not assume uniqueness.
    "ro.csc.country_code": "EGY",
    "ro.csc.sales_code": "EGY",
    "ro.csc.sales_code.vendor": "EGY",
    "ro.build.PDA": "A146USQS5CWG3",
    "ro.boot.bootloader": "A146USQS5CWG3",
    "ro.boot.flash.locked": "1",
    "ro.config.knox": "v40",
    "ro.omc.build.version": "A146UOYM5CWG3",
    "dalvik.vm.heapsize": "512m",
    "ro.sf.lcd_density": "450",
    # A property value that itself contains ':' and '=' to test splitting.
    "ro.vendor.build.signature": "aGVsbG86d29ybGQ9MQ==",
}

GETPROP_MINIMAL = {
    "ro.product.model": "Generic Android",
    "ro.product.brand": "generic",
    "ro.product.cpu.abi": "armeabi-v7a",
    "ro.build.version.release": "7.0",
    "ro.build.version.sdk": "24",
    "ro.build.fingerprint": "generic/generic/generic:7.0/NRD90M/1:user/test-keys",
    "ro.debuggable": "1",
    "ro.secure": "0",
}


def render_getprop(props: dict[str, str]) -> str:
    """Render `getprop` output faithfully: [key]: [value]."""
    return "".join(f"[{key}]: [{value}]\n" for key, value in props.items())


DF_API34 = """Filesystem      1K-blocks      Used Available Use% Mounted on
/dev/block/dm-7  116078592  74518032  41560560  65% /data
/dev/block/dm-2    4194304   4100112     94192  98% /system
/dev/block/dm-3    1048576    912344    136232  88% /vendor
tmpfs              3832840      1024   3831816   1% /dev
/dev/block/dm-8      65536      4096     61440   7% /metadata
tmpfs              3832840         0   3832840   0% /mnt/ramdisk
/dev/fuse        116078592  74518032  41560560  65% /storage/emulated
"""

# Mount points whose names contain spaces and a wrapped line: a naive
# split-on-whitespace parser gets Use%/Mounted-on wrong here.
DF_OVERFLOW = """Filesystem        1K-blocks     Used Available Use% Mounted on
/dev/block/dm-7     116078592 74518032  41560560  65% /data
/dev/block/sda1        524288     1024    523264   1% /mnt/media_rw/My SD Card
/dev/block/dm-2       4194304  4100112     94192  98% /system
tmpfs                 3832840        0   3832840   0% /storage/emulated/0/Android/data
/dev/block/dm-9       2097152   524288   1572864  25%
/mnt/vendor/persist
"""


def render_pm_list(count: int, users: int = 1) -> str:
    """Render `pm list packages`. Names are synthetic and stable per invocation."""
    lines: list[str] = []
    base = [
        "com.android.settings",
        "com.android.systemui",
        "com.android.phone",
        "com.android.shell",
        "com.android.bluetooth",
        "org.example.notes",
        "org.example.reader",
        "org.example.camera",
    ]
    lines.extend(base)
    for index in range(max(0, count - len(base))):
        lines.append(f"org.example.sample{index:04d}")
    rendered = [f"package:{name}" for name in lines]
    if users > 1:
        for name in lines[:3]:
            rendered.append(f"package:{name}")
    return "\n".join(rendered) + "\n"


DUMPSYS_BATTERY = """Current Battery Service state:
  AC powered: false
  USB powered: true
  Wireless powered: false
  Max charging current: 1500000
  Max charging voltage: 5000000
  Charge counter: 3200000
  status: 2
  health: 2
  present: true
  level: 67
  scale: 100
  voltage: 4123
  temperature: 312
  technology: Li-ion
  Charging status: Charging
"""

SETTINGS_PRIVATE_DNS = {
    "mode-off": "off\n",
    "mode-opportunistic": "opportunistic\n",
    "mode-hostname": "hostname\n",
    "specifier-set": "dns.example.invalid\n",
    "specifier-null": "null\n",
}

FASTBOOT_GETVAR_ALL = """(bootloader) product:panther
(bootloader) serialno:REDACTED0001
(bootloader) slot-count:2
(bootloader) current-slot:a
(bootloader) slot-successful:a:yes
(bootloader) slot-unbootable:a:no
(bootloader) secure:yes
(bootloader) unlocked:no
(bootloader) anti:2
(bootloader) hw-revision:MP1.0
(bootloader) battery-soc-ok:yes
(bootloader) battery-voltage:4210
(bootloader) version-baseband:g5300g-00052-240101-B-11830207
(bootloader) version-bootloader:slider-1.0-11398904
(bootloader) off-mode-charge:1
(bootloader) is-userspace:no
(bootloader) partition-type:boot_a:raw
(bootloader) partition-size:boot_a:0x4000000
finished. total time: 0.052s
"""

FASTBOOT_GETVAR_XIAOMI = """(bootloader) product:veux
(bootloader) crc:1
(bootloader) board:veux
(bootloader) secure:yes
(bootloader) hw-revision:0
(bootloader) unlocked:yes
(bootloader) anti:1
(bootloader) off-mode-charge:0
(bootloader) current-slot:a
(bootloader) is-userspace:no
finished. total time: 0.041s
"""

FASTBOOT_GETVAR_HUAWEI_OEM = """...
(bootloader) Device unlocked: false
(bootloader) Device critical unlocked: false
(bootloader) Charger screen enabled: false
(bootloader) FBENFORCE: true
OKAY [  0.011s]
finished. total time: 0.011s
"""

GETPROP_MALFORMED = (
    "[ro.product.model]: [Test]\n"
    "[ro.product.brand]: [test]\n"
    "[ro.build.version.sdk]: [34\n"            # missing closing bracket
    "this line has no property syntax at all\n"
    "[]:\n"                                     # empty key
    "[ro.weird.key]: [value with ] bracket]\n"  # bracket inside value
    "[ro.bin.key]: [\x01\x02]\n"                # control bytes
    "\n"
    "[ro.utf.invalid]: [\udcff]\n"              # surrogate escape -> invalid UTF-8 on write
)


# --------------------------------------------------------------------------- #
# Category C: hostile archives
# --------------------------------------------------------------------------- #

def write_zip(path: Path, entries: list[tuple[str, bytes]], *, symlink: bool = False) -> None:
    path.parent.mkdir(parents=True, exist_ok=True)
    with zipfile.ZipFile(path, "w", compression=zipfile.ZIP_DEFLATED) as archive:
        for name, data in entries:
            info = zipfile.ZipInfo(name)
            if symlink:
                info.external_attr = (stat.S_IFLNK | 0o777) << 16
            else:
                info.external_attr = (stat.S_IFREG | 0o644) << 16
            archive.writestr(info, data)


def build_malicious() -> dict[str, list[tuple[str, bytes]]]:
    """Build each hostile archive and return a manifest of what it contains."""
    MALICIOUS.mkdir(parents=True, exist_ok=True)
    manifest: dict[str, list[tuple[str, bytes]]] = {}

    cases: dict[str, list[tuple[str, bytes]]] = {
        # C1: entry escapes the extraction root.
        "malicious/zip-slip.apks": [
            ("toc.pb", b"synthetic-toc"),
            ("base.apk", b"synthetic-base"),
            ("../../evil.so", b"synthetic-escape"),
        ],
        # C2: absolute path entry.
        "malicious/zip-absolute.apks": [
            ("/etc/passwd", b"root:x:0:0:synthetic"),
            ("base.apk", b"synthetic-base"),
        ],
        # C4: too many entries (cap is 100,000 per the limits table).
        "malicious/zip-many-entries.apks": [
            (f"splits/split_{i:06d}.apk", b"x") for i in range(150_000)
        ],
        # C6 is built separately below: Python's zipfile silently truncates a name
        # at NUL, so the archive is written with a placeholder and then patched.
        # C7: nesting deeper than the 32-level cap.
        "malicious/zip-deep-nesting.apks": [
            ("/".join(["d"] * 40) + "/payload.bin", b"deep"),
        ],
    }
    for name, entries in cases.items():
        write_zip(CORPUS / name, entries)
        manifest[name] = entries

    # C3: zip bomb. A 1 MB run of zeros compresses to ~1 KB and expands to 1 MB;
    # repeated 9000 times in a single entry gives a ratio far past 100:1.
    bomb_payload = b"\0" * (1024 * 1024)
    write_zip(CORPUS / "malicious/zip-bomb.apks", [("base.apk", bomb_payload * 8)])
    manifest["malicious/zip-bomb.apks"] = [("base.apk", bomb_payload * 8)]

    # C6: NUL inside the entry name. zipfile cannot write such a name, so build
    # the archive with an equal-length placeholder and patch the raw bytes in
    # both the local file header and the central directory.
    placeholder = b"base.apkVso"
    nul_path = CORPUS / "malicious/zip-nul-in-name.apks"
    write_zip(CORPUS / "_tmp_nul.apks", [("base.apkVso", b"synthetic"), ("base.apk", b"synthetic-base")])
    raw = (CORPUS / "_tmp_nul.apks").read_bytes()
    (CORPUS / "_tmp_nul.apks").unlink()
    patched = raw.replace(placeholder, b"base.apk\x00.so")
    assert patched != raw, "placeholder not found while building the NUL-name archive"
    nul_path.write_bytes(patched)
    manifest["malicious/zip-nul-in-name.apks"] = [("base.apk\x00.so", b"synthetic")]

    # C5: symlink entry pointing at the filesystem root.
    write_zip(
        CORPUS / "malicious/zip-symlink.apks",
        [("link_to_root", b"/"), ("base.apk", b"synthetic-base")],
        symlink=True,
    )
    manifest["malicious/zip-symlink.apks"] = [("link_to_root", b"/")]

    return manifest


# --------------------------------------------------------------------------- #
# Writing and verification
# --------------------------------------------------------------------------- #

def meta(path: Path, *, category: str, purpose: str, proves: str, cannot: str) -> None:
    document = {
        "name": path.name,
        "category": category,
        "origin": "synthetic",
        "generator": f"make_fixtures.py {GENERATOR_VERSION}",
        "createdAt": CREATED,
        "device": None,
        "redacted": True,
        "purpose": purpose,
        "proves": proves,
        "doesNotProve": cannot,
        "sha256": hashlib.sha256(path.read_bytes()).hexdigest(),
    }
    path.with_suffix(path.suffix + ".meta.json").write_text(
        json.dumps(document, ensure_ascii=False, indent=2) + "\n", encoding="utf-8"
    )


def generate() -> list[Path]:
    ANDROID.mkdir(parents=True, exist_ok=True)
    written: list[Path] = []

    def text(name: str, content: str, **kwargs) -> None:
        path = ANDROID / name
        path.write_text(content, encoding="utf-8", errors="surrogatepass", newline="\n")
        meta(path, **kwargs)
        written.append(path)

    text(
        "getprop-pixel7-api34.txt",
        render_getprop(GETPROP_PIXEL7),
        category="android-output",
        purpose="Full getprop dump from a stock Android 14 device profile.",
        proves="The parser reads every documented identity/build/security property.",
        cannot="Prove these properties exist on every Android 14 device or OEM build.",
    )
    text(
        "getprop-samsung-api30.txt",
        render_getprop(GETPROP_SAMSUNG),
        category="android-output",
        purpose="OEM-flavoured getprop with duplicated and unusual keys.",
        proves="Duplicate keys and ':'/'=' inside values do not break parsing.",
        cannot="Prove the Samsung property set is identical across models or One UI versions.",
    )
    text(
        "getprop-minimal-api24.txt",
        render_getprop(GETPROP_MINIMAL),
        category="android-output",
        purpose="Lowest supported API level with a sparse property set.",
        proves="Missing fields become NotReported instead of throwing.",
        cannot="Prove that a real API 24 device exposes exactly this set.",
    )
    text(
        "getprop-malformed.txt",
        GETPROP_MALFORMED,
        category="android-output",
        purpose="Deliberately broken getprop output.",
        proves="Unbalanced brackets, empty keys, control bytes and invalid UTF-8 yield MalformedResponse, not a crash.",
        cannot="Enumerate every malformed shape a device or proxy could emit.",
    )
    text(
        "df-api34.txt",
        DF_API34,
        category="android-output",
        purpose="Normal df -k output across several mount points.",
        proves="Block-to-byte conversion and per-mount arithmetic are correct.",
        cannot="Prove df reports the same columns on every toybox version.",
    )
    text(
        "df-overflow.txt",
        DF_OVERFLOW,
        category="android-output",
        purpose="df output with a mount point containing spaces and a wrapped line.",
        proves="A parser that splits on the first whitespace run mis-assigns the mount point; the correct parser does not.",
        cannot="Cover every possible wrapping position toybox may choose.",
    )
    text(
        "pm-list-packages-api34.txt",
        render_pm_list(214, users=2),
        category="android-output",
        purpose="Package inventory with more than one user profile.",
        proves="Listing, de-duplication and --user handling are correct.",
        cannot="Prove that any real device installs exactly these packages.",
    )
    text(
        "pm-list-packages-huge.txt",
        render_pm_list(20_000),
        category="android-output",
        purpose="Oversized package listing used with the byte/line caps.",
        proves="LimitExceeded is returned explicitly rather than truncating silently.",
        cannot="Represent a real-world maximum; devices vary.",
    )
    text(
        "dumpsys-battery-api34.txt",
        DUMPSYS_BATTERY,
        category="android-output",
        purpose="dumpsys battery key: value block.",
        proves="Numeric fields (level, temperature in tenths C, voltage in mV) are read with the right units.",
        cannot="Prove the field set is stable across OEMs.",
    )
    for name, content in SETTINGS_PRIVATE_DNS.items():
        text(
            f"settings-private-dns-{name}.txt",
            content,
            category="android-output",
            purpose=f"Private DNS setting readback: {name}.",
            proves="The mode/specifier distinction is preserved; 'null' is treated as NotReported, not as the string 'null'.",
            cannot="Prove that writing the setting is permitted on any given device.",
        )
    text(
        "fastboot-getvar-all.txt",
        FASTBOOT_GETVAR_ALL,
        category="android-output",
        purpose="Full fastboot getvar all output from a stock bootloader.",
        proves="(bootloader) prefixes, duplicate keys such as partition-size twice, and the trailing 'finished.' line are handled.",
        cannot="Prove variable names are consistent across OEM bootloaders.",
    )
    text(
        "fastboot-getvar-xiaomi.txt",
        FASTBOOT_GETVAR_XIAOMI,
        category="android-output",
        purpose="Xiaomi-style getvar output with an 'anti' rollback counter.",
        proves="unlocked/anti are parsed as their own fields, not merged into a generic state.",
        cannot="Prove every Xiaomi bootloader reports 'anti'.",
    )
    text(
        "fastboot-oem-get-bootinfo-huawei.txt",
        FASTBOOT_GETVAR_HUAWEI_OEM,
        category="android-output",
        purpose="Huawei OEM free-text boot info.",
        proves="A 'Device unlocked: false' line is parsed without treating free text as structured.",
        cannot="Prove this exact wording across Huawei/Honor firmware.",
    )

    manifest = build_malicious()
    for name, entries in manifest.items():
        path = CORPUS / name
        meta(
            path,
            category="hostile-archive",
            purpose=f"Attack fixture for {name.split('/')[-1]}.",
            proves="The safe extractor refuses (or caps) this archive before any bytes land outside the operation folder.",
            cannot="Represent every archive attack; add cases as new techniques appear.",
        )
        written.append(path)
    del entries

    (CORPUS / "GENERATED.json").write_text(
        json.dumps(
            {
                "generator": f"make_fixtures.py {GENERATOR_VERSION}",
                "createdAt": CREATED,
                "files": sorted(str(p.relative_to(CORPUS)) for p in written),
                "note": "Category B (logcat) and D (real packages) are intentionally not generated here.",
            },
            indent=2,
        )
        + "\n",
        encoding="utf-8",
    )
    return written


def check() -> int:
    problems: list[str] = []

    required = [
        "android/getprop-pixel7-api34.txt",
        "android/getprop-samsung-api30.txt",
        "android/getprop-minimal-api24.txt",
        "android/getprop-malformed.txt",
        "android/df-api34.txt",
        "android/df-overflow.txt",
        "android/pm-list-packages-api34.txt",
        "android/pm-list-packages-huge.txt",
        "android/dumpsys-battery-api34.txt",
        "android/settings-private-dns-mode-off.txt",
        "android/fastboot-getvar-all.txt",
        "malicious/zip-slip.apks",
        "malicious/zip-absolute.apks",
        "malicious/zip-bomb.apks",
        "malicious/zip-many-entries.apks",
        "malicious/zip-symlink.apks",
        "malicious/zip-nul-in-name.apks",
        "malicious/zip-deep-nesting.apks",
    ]
    for relative in required:
        path = CORPUS / relative
        if not path.is_file():
            problems.append(f"missing fixture: {relative}")
            continue
        meta_path = path.with_suffix(path.suffix + ".meta.json")
        if not meta_path.is_file():
            problems.append(f"missing card: {meta_path.relative_to(CORPUS)}")
            continue
        card = json.loads(meta_path.read_text(encoding="utf-8"))
        actual = hashlib.sha256(path.read_bytes()).hexdigest()
        if card.get("sha256") != actual:
            problems.append(f"sha256 mismatch (fixture changed without a new card): {relative}")
        if card.get("origin") != "synthetic":
            problems.append(f"unexpected origin in {relative}: {card.get('origin')}")

    # The hostile archives must actually contain the attack they claim.
    def names(relative: str) -> list[str]:
        """Raw central-directory names.

        A NUL-containing name cannot survive zipfile's decoding, so for that one
        fixture the raw bytes are used instead.
        """
        path = CORPUS / relative
        if not path.is_file():
            return []
        if path.name == "zip-nul-in-name.apks":
            # Every zip contains NUL bytes in its binary headers, so this single
            # fixture is checked against the raw bytes instead of decoded names.
            return [path.read_bytes().decode("utf-8", errors="replace")]
        with zipfile.ZipFile(path) as archive:
            return archive.namelist()

    if not any("../" in name for name in names("malicious/zip-slip.apks")):
        problems.append("zip-slip.apks contains no traversal entry")
    if not any(name.startswith("/") for name in names("malicious/zip-absolute.apks")):
        problems.append("zip-absolute.apks contains no absolute-path entry")
    if len(names("malicious/zip-many-entries.apks")) < 100_000:
        problems.append("zip-many-entries.apks is below the 100k entry cap used for the test")
    if not any("\x00" in name for name in names("malicious/zip-nul-in-name.apks")):
        problems.append("zip-nul-in-name.apks contains no NUL byte in an entry name")
    if not any(name.count("/") > 32 for name in names("malicious/zip-deep-nesting.apks")):
        problems.append("zip-deep-nesting.apks is not deeper than 32 levels")

    # PII guard: no fixture text may contain a 15-digit Luhn-valid run, which is
    # what a real IMEI looks like. Digit runs are taken maximally, so "1234567890"
    # inside a 20-digit number is not considered.
    for path in sorted(ANDROID.glob("*.txt")):
        text = path.read_text(encoding="utf-8", errors="replace")
        for run in re.findall(r"[0-9]{15,}", text):
            if _luhn_valid(run):
                problems.append(f"possible IMEI-shaped digit run in {path.name}: {run}")

    if problems:
        print(f"{len(problems)} corpus problem(s):")
        for problem in problems:
            print(f"  x {problem}")
        return 1

    print(f"corpus: {len(required)} required fixtures present, cards valid, hostile archives hold their attack")
    return 0


def _luhn_valid(digits: str) -> bool:
    """Luhn check. A 15-digit run that passes is indistinguishable from an IMEI."""
    if not digits or not digits.isdigit():
        return False
    total = 0
    for index, char in enumerate(reversed(digits)):
        value = int(char)
        if index % 2 == 1:
            value *= 2
            if value > 9:
                value -= 9
        total += value
    return total % 10 == 0


def clean() -> None:
    for directory in (ANDROID, MALICIOUS):
        if directory.is_dir():
            shutil.rmtree(directory)
    generated = CORPUS / "GENERATED.json"
    if generated.is_file():
        generated.unlink()
    print("removed generated corpus")


def main() -> int:
    parser = argparse.ArgumentParser(description=__doc__)
    group = parser.add_mutually_exclusive_group()
    group.add_argument("--check", action="store_true", help="verify existing fixtures")
    group.add_argument("--clean", action="store_true", help="delete generated fixtures")
    args = parser.parse_args()

    if not CORPUS.is_dir():
        print(f"ERROR: corpus directory not found at {CORPUS}", file=sys.stderr)
        return 2

    if args.clean:
        clean()
        return 0
    if args.check:
        return check()

    written = generate()
    print(f"generated {len(written)} fixtures under {CORPUS.relative_to(CORPUS.parent)}/")
    return check()


if __name__ == "__main__":
    raise SystemExit(main())
