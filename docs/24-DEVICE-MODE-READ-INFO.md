# Device information by connection mode

**Research updated:** 14 November 2026. **Host scope:** Windows 10/11, Android 7.0 / API 24 and newer. **Default:** explicit target, read-only, bounded output, no automatic mode changes.

This document turns the mode research in [01](01-connectivity-protocols.md) and
the field catalogue in [06](06-android-system-information.md) into an
implementation and verification plan. “All modes” means that the application
reports what each currently available protocol can and cannot read. It does
not mean that the application reboots the phone between modes.

## Evidence and reuse decisions

| Source | Current evidence | Decision |
|---|---|---|
| Android Debug Bridge | Android Developers documents `device` and `offline` states and warns that `device` does not prove Android has finished booting. | Keep the existing `AdvancedSharpAdbClient` discovery and `AdbShell` runner. ADB facts are a fixed property allowlist against the explicitly selected serial. |
| Fastboot / Fastbootd | AOSP documents userspace Fastboot, the `is-userspace` variable, and that `getvar` variables can be missing. It also lists destructive commands in the same protocol. GitHub candidate review is recorded in [25](25-FASTBOOT-SOURCE-REVALIDATION.md). | Implemented as a separate adapter that locates the installed `fastboot.exe`, requires an explicitly selected serial, and requests only `is-userspace`, `product`, `current-slot`, and `unlocked`. Never issue `getvar all`, OEM commands, reboot, fetch, flash, erase, or slot changes from Read Info. |
| Recovery ADB sideload | AOSP says sideload mode supports only `adb sideload`; shell, logcat, push, pull, and reboot fail. | Report the transport as sideload-limited when positively identified. Do not retry with shell or interpret failure as a broken phone. |
| Windows Portable Devices | Microsoft documents read-only WPD device properties such as manufacturer, model, firmware version, serial number, and transport. The device controls which properties it supplies. | Reuse the current `DeviceToolkit.Wpd` session and isolated worker. Report available model/name/firmware/battery metadata; do not label it Android system information. Exclude serial from routine display/logs. Samsung boot/security properties require the separately selected ADB target. See [Samsung MTP research](26-SAMSUNG-MTP-READ-INFO.md). |
| Xiaomi MiAssistantTool | The upstream repository is Apache-2.0 and its current release notes say Read Info exists. The same tool also exposes flash, format, wipe, and reboot operations; its read commands include device-specific identifiers such as serial and Mi Token. | Candidate for an isolated, read-only adapter after review of the exact transport, source revision, output redaction, fixture coverage, and separation from write entry points. Do not transplant the application or invoke `getmitoken`. |
| SharpOdinClient | The current repository is GPL-3.0, targets .NET Framework 4.5.1, and documents older-device limitations. It combines reading with PIT writing and firmware flashing. | Reference only while license compatibility, framework compatibility, and strict read-only API separation remain unresolved. Do not add as a dependency now. |
| galaxy-at-tool | Repository source exposes `AT+DEVCONINFO`, but also includes SIM identity/phonebook reads and commands that change call forwarding, SMSC, or Download Mode. No license is declared on the repository page. | Do not reuse or send AT commands. Consider Samsung modem metadata only after per-model command review, licensing review, and tests proving the selected command is read-only. |
| Huawei/Honor Fastboot OEM | Huawei examples show `oem get-product-model`; community tools also query product and vendor/country. The responses are model-specific. | In the explicit experimental Huawei profile only, allow one fixed product-model OEM read plus individual getvar reads. No arbitrary OEM input or batch dump. Exact model validation is pending. |
| Qualcomm EDL and MediaTek BROM/Preloader | Explicitly excluded by project scope. | Unsupported; no detection beyond passive Windows device inventory and no protocol I/O. |

Primary references: [ADB](https://developer.android.com/tools/adb),
[AOSP Fastbootd](https://source.android.com/docs/core/architecture/bootloader/fastbootd),
[AOSP Fastboot variable constants](https://android.googlesource.com/platform/system/core/+/refs/tags/android-15.0.0_r29/fastboot/constants.h),
[AOSP Recovery sideload](https://source.android.com/docs/core/ota/nonab/device_code),
[Microsoft WPD device properties](https://learn.microsoft.com/en-us/windows/win32/wpd_sdk/device-properties),
[MiAssistantTool](https://github.com/MiForge/MiAssistantTool),
[SharpOdinClient](https://github.com/Alephgsm/SharpOdinClient), and
[galaxy-at-tool](https://github.com/apeppels/galaxy-at-tool).

## Mode capability matrix

| Detected connection | Safe information in this phase | Explicitly unavailable or deferred |
|---|---|---|
| Android ADB, normal boot | Allowlisted product/build properties, Android release/API/security patch, ABI, boot properties, selected non-identifying settings | IMEI/IMSI, phone number, MAC, ordinary device serial, all-properties dumps, broad `dumpsys`, root probing |
| Recovery with normal ADB shell | Same allowlisted reads only when the shell answers; label recovery only from explicit recovery evidence | Assumption that every recovery exposes identical properties or storage |
| Recovery sideload | Transport/state only if detectable | Android properties, shell, logcat, files, reboot |
| Bootloader Fastboot | Individually requested standard `getvar` values that the bootloader reports | `getvar all`, OEM extensions without per-model evidence, partition reads, transitions and all writes |
| Fastbootd | Same allowlisted `getvar` reads; identify userspace only when `is-userspace=yes` | Inferring Fastbootd when the variable is absent; partition mutation or fetch |
| Huawei Fastboot / FastBoot&Rescue | Separate experimental profile; common variables, `rescue_version` and `vendorcountry` via individual `getvar`, and fixed `oem get-product-model` | No `getvar all`, arbitrary OEM, phone-info/serial/IMEI/PSID query, transition, write, or repair claim. Community examples only; Huawei hardware/model validation is pending. |
| MTP/WPD | Device-declared manufacturer/model/firmware/name/battery level when provided by WPD | Android API/build/runtime facts and bootloader/security state; WPD serial is not displayed by default |
| Samsung AT Modem | Deferred | No AT command sent in this phase; no SIM, subscriber, phonebook or modem state reads |
| Samsung Download Mode / Odin-Loke | Deferred | No Odin session, PIT, flash, reboot or model-agnostic claim |
| Xiaomi Mi Assistant | Deferred pending isolated adapter review | No Mi Token, write/format/wipe/flash/reboot commands |
| Huawei/Honor Fastboot | A separate read-only experimental profile exists; two Huawei `getvar` names and one fixed `oem get-product-model` read are added | No generic OEM probe, PSID/IMEI query, repair, unlock, relock, FRP, or identity query; exact model/revision compatibility remains unverified |
| Motorola and OPPO/vivo/OnePlus/Realme | Standard ADB/MTP/Fastboot paths only | No brand-specific commands absent exact model evidence |
| Qualcomm EDL / MediaTek BROM/Preloader | Unsupported by project scope | All protocol communication |

An empty or absent field is represented as `NotReported` or `Unsupported`,
not as a guessed value. Permission, timeout, disconnect, and command failure
remain distinct outcomes. A transport's visible name, USB VID/PID, or generic
“online” label is not sufficient proof of the operating mode.

## Implementation sequence

1. Expand the current ADB read-only report with a bounded allowlist and provenance.
2. Surface WPD `FriendlyName`, `Manufacturer`, and `Description` through the
   existing long-lived worker session, keeping Android facts and MTP metadata
   in separate groups. This surface is implemented; physical-device metadata
   remains unverified when WPD enumeration returns no device.
3. Add mode selection/status without mode transitions. Recovery sideload must
   have an explicit limited-capability result.
4. Revalidate each vendor adapter against its exact revision, license, platform,
   source code, and tests before any reuse. Add sanitized fixtures before
   hardware trials.
5. Test on Windows and an explicitly selected physical phone in each supported
   mode. No claim of cross-OEM support follows from a single device test.

ADB and Fastboot read-only slices are implemented and unit-tested. The WPF
Fastboot page is build-verified, but a Fastboot phone smoke test and visual
click-through remain open. WPD metadata uses a separate UI group and the
existing worker, but no WPD device was present during the latest metadata-only
smoke. Protocol-level mode status remains a follow-up; proprietary protocols
are intentionally deferred rather than silently represented as supported.
Huawei devices use the common Fastboot base with a separate experimental
profile that adds `rescue_version` and `vendorcountry` as individual `getvar`
reads and one fixed `oem get-product-model` query. These names and the response
shape come from community tool examples, not a vendor compatibility contract;
no Huawei device is available for validation. No general OEM or service
commands are enabled. The next step is a model-specific hardware check, not an
expanded command list.
