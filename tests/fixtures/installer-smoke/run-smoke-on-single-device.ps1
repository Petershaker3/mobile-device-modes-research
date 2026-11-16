$ErrorActionPreference = 'Stop'
$repoRoot = (Resolve-Path (Join-Path $PSScriptRoot '..\..\..')).Path
$adb = Join-Path $env:ANDROID_SDK_ROOT 'platform-tools\adb.exe'
$apk = Join-Path $repoRoot 'artifacts\DeviceToolkitInstallTest.apk'
$runner = Join-Path $PSScriptRoot 'WorkflowSmoke.csproj'
if (-not (Test-Path $adb)) { throw 'Android Platform Tools were not found.' }
if (-not (Test-Path $apk)) { throw 'Build the test APK first with build.ps1.' }

$deviceRows = @(& $adb devices -l | Select-Object -Skip 1 | Where-Object { $_ -match '^\S+\s+(device|unauthorized|offline)\b' })
if ($LASTEXITCODE -ne 0) { throw 'ADB device discovery failed.' }
$onlineRows = @($deviceRows | Where-Object { $_ -match '^\S+\s+device\b' })
if ($deviceRows.Count -ne 1 -or $onlineRows.Count -ne 1) {
    throw 'Connect exactly one authorized Android device before running this smoke test. No install or removal was attempted.'
}

$serial = ($onlineRows[0] -split '\s+')[0]
$packageId = 'com.petershaker.devicetoolkit.installtest'
$existingPackage = @(& $adb -s $serial shell pm list packages $packageId 2>$null)
if ($LASTEXITCODE -ne 0) { throw 'Could not check whether the dedicated test package is already installed.' }
if ($existingPackage -match "^package:$([Regex]::Escape($packageId))$") {
    throw 'The dedicated test package is already installed. No package was changed.'
}

$env:DEVICE_TOOLKIT_TEST_SERIAL = $serial
$env:DEVICE_TOOLKIT_TEST_APK = $apk
try {
    & dotnet run --project $runner --configuration Release
    if ($LASTEXITCODE -ne 0) { throw "The app-management smoke test failed with exit code ${LASTEXITCODE}." }
}
finally {
    Remove-Item Env:\DEVICE_TOOLKIT_TEST_SERIAL -ErrorAction SilentlyContinue
    Remove-Item Env:\DEVICE_TOOLKIT_TEST_APK -ErrorAction SilentlyContinue
}
