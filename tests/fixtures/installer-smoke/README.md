# Installer smoke-test app

This is a tiny local APK fixture for preview, install, version verification, and removal checks. It has one launcher activity that displays its package ID. The manifest declares no permissions, services, receivers, providers, or network configuration.

Build it on the Windows workstation with an installed Android SDK API 36, build-tools 36.0.0, and Android Studio's Java runtime:

```powershell
pwsh -File tests/fixtures/installer-smoke/build.ps1
```

The output is `artifacts/DeviceToolkitInstallTest.apk`. The script creates an ephemeral signing key under the system temporary directory and removes the key after signing. Each build therefore has a different signing identity; uninstall the previous test build before installing a rebuilt copy. The APK is only for this local device-management test, not for distribution.

To exercise the product workflow on a connected test phone, run:

```powershell
pwsh -File tests/fixtures/installer-smoke/run-smoke-on-single-device.ps1
```

The runner proceeds only when exactly one authorized ADB device is online and the dedicated package is absent. It prints both operation previews, then uses the app's `DeviceWorkflow` to install, verify, remove, and verify absence of only this package. It attempts to clean up the matching test package after an incomplete run.
