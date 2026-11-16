# Samsung Read Info over MTP and ADB

**Research checked:** 16 November 2026. **Scope:** Windows MTP/WPD metadata plus read-only ADB diagnostics. No Download Mode, AT modem, FRP, flashing, or identity lookup is performed.

## What each connection can report

MTP and ADB are separate transports. MTP can report device-declared model, name, firmware version, and battery level when the handset exposes those WPD fields. The existing `MediaDevices` 2.0.0 dependency is reused through the isolated WPD worker, which already connects with `GenericRead` and `Share.Read`. A missing property remains `Not reported`. We do not show the WPD serial number or PnP identity.

MTP does not expose Android's live boot security state. When the user explicitly selects the phone in the ADB device list and chooses Read device info, the existing fixed `getprop` allowlist now also reads Samsung sales CSC, bootloader lock, Verified Boot state, Knox warranty bit, and verity mode, plus `getenforce`. These are reported property values, not a proof of device integrity, warranty status, or account/FRP state. A modified build may omit or change them. No root, token, IMEI, serial, account, or broad property dump is requested.

The MTP screen generates a SamFW link from a reported Samsung `SM-...` model and opens its model listing. The ADB overview link adds the three-character CSC only if the selected phone reported a valid sales code; otherwise it also opens the model listing. The app only opens directory pages. It does not download firmware, query Samsung/SamFW APIs, or send an IMEI/serial. SamFW is a third-party directory; confirm exact model, CSC, binary revision, and build before any manual firmware action.

## GitHub and source review

| Candidate | Review | Decision |
|---|---|---|
| [Yogendra96/samsung-kali-nethunter-rootless](https://github.com/Yogendra96/samsung-kali-nethunter-rootless), GPL-3.0 | Its Knox guide demonstrates `ro.boot.warranty_bit`, `ro.boot.flash.locked`, `ro.boot.verifiedbootstate`, and `ro.boot.veritymode`. The repository is a rootless install project and makes broader warranty claims than these properties can prove. | Reuse the property names as research leads only. No code copied; UI describes them as reported values. |
| [Magisk-Modules-Repo/MagiskHidePropsConf](https://github.com/Magisk-Modules-Repo/MagiskHidePropsConf), archived | It references the same property names, but its purpose includes changing/overriding property values. | Do not reuse its implementation or treat properties as authoritative. It is a reason to disclose spoofing limits. |
| [apeppels/galaxy-at-tool](https://github.com/apeppels/galaxy-at-tool) | A Samsung modem/AT-command project. The repository page has no declared license, and its command surface includes operations beyond safe device information. | No AT commands or code reuse. Reconsider only with a per-command read-only audit, license, device matrix, and protocol fixtures. |
| [ysfchn/SamFetch](https://github.com/ysfchn/SamFetch), AGPL-3.0, archived 7 February 2026 | Firmware lookup/download API; archived and requests identifiers for some Samsung-server operations. | Not used. The UI links to SamFW's public model/CSC listing only and sends no device identifier. |

These projects do not provide a suitable, licensed, narrowly read-only Samsung MTP implementation for this app. The existing Windows WPD layer remains the MTP implementation. No parallel MTP protocol is added.

## SamFW page shape

The site was checked on 16 November 2026. Its model page is `/firmware/{model}` and its CSC-filtered page is `/firmware/{model}/{csc}`. For example, `SM-G991B/BTU` resolves to a firmware listing. The page contains firmware build and security patch listings; this is a third-party index and not an integrity attestation. The application constructs the URL from validated model/CSC text and opens only `https://samfw.com`.

## Implementation and verification status

- MTP metadata reads are explicit-device, read-only, and run through the existing WPD worker. Unsupported fields are individually omitted rather than failing storage browsing.
- ADB queries remain bounded to fixed property names and target the explicitly selected serial.
- SamFW model and CSC path segments are regex-validated; the app has no firmware download action.
- Windows Debug build completed successfully. The initial full compile reported two existing `CS9191` warnings in unchanged `AdbManager.cs`; the final incremental build reported zero warnings and zero errors.
- `DeviceToolkit.Tests` passed 577/577, including fixed-property parsing and safe SamFW URL construction cases.
- The newly built desktop app was opened and its MTP screen was visually inspected. Follow-up physical-device check: WPD enumerated one Samsung phone and the isolated read-only worker returned model `SM-A105F`, device name `SM-A105F`, firmware value `A105FDDS9CWD1`, and a power-level value that varied between reads. It is left uninterpreted because WPD documents it as a percentage or similar device-reported value. The model page `https://samfw.com/firmware/SM-A105F` loaded and listed the reported firmware value `A105FDDS9CWD1` under several CSCs, including EGY. This confirms the model-page and firmware-version match; it does not identify the phone's CSC. No CSC was exposed over MTP, so the model-only link was the correct result.
- The ADB Platform Tools executable was found locally, but `adb devices` returned no devices. Samsung boot-security properties, patch level, and CSC via ADB remain unverified on this handset. The MTP read itself required no debugging prompt and made no device changes.
- Visual gate scripts listed in the repository brief are not present in this checkout. Computer Use could not connect to its native pipe. The row-layout correction compiled and the empty MTP screen was inspected; with the phone selected, the live details card was observed before that correction, so the post-fix populated card still needs a visual check. The latest empty-screen screenshot is at `artifacts/samsung-mtp-after-row-fix.png`.
- Hardware validation still requires a Samsung phone with MTP enabled. MTP and ADB selection are deliberately separate, so the UI does not claim they are the same handset unless the user confirms by selecting each one.
- Follow-up MTP metadata probe on 16 November 2026 against the connected `SM-A105F`: `PowerSource=Battery` and `Protocol=MTP: 1.00` were reported. Device date/time was not reported. `GetDrives()` returned no storage volumes, so capacity/free-space remains unavailable on this handset. Battery value was 18 on this read and is treated as a device-reported reading, not a calibrated estimate. The probe did not enumerate folders or files, and did not print serial/PnP identifiers or storage labels.
