param(
    [Parameter(Mandatory = $true)]
    [ValidatePattern('^[A-Za-z0-9._:-]{1,128}$')]
    [string]$Serial
)

$ErrorActionPreference = 'Stop'
$repoRoot = (Resolve-Path (Join-Path $PSScriptRoot '..\..\..')).Path
$adbCommand = Get-Command adb -ErrorAction SilentlyContinue
$adb = if ($adbCommand) { $adbCommand.Source } else { Join-Path $env:LOCALAPPDATA 'Android\Sdk\platform-tools\adb.exe' }
if (-not (Test-Path $adb)) { throw 'Android Platform Tools were not found.' }

$deviceRows = @(& $adb devices -l | Select-Object -Skip 1 | Where-Object { $_ -match '^\S+\s+(device|unauthorized|offline)\b' })
if ($LASTEXITCODE -ne 0) { throw 'ADB device discovery failed.' }
$targetRows = @($deviceRows | Where-Object { ($_ -split '\s+')[0] -ceq $Serial -and $_ -match '^\S+\s+device\b' })
if ($targetRows.Count -ne 1) { throw 'The requested serial is not uniquely online. No diagnostic was run.' }

$env:DEVICE_TOOLKIT_TEST_SERIAL = $Serial
try {
    & dotnet run --project (Join-Path $PSScriptRoot 'WorkflowSmoke.csproj') --configuration Release
    if ($LASTEXITCODE -ne 0) { throw "The selected-device diagnostic smoke test failed with exit code ${LASTEXITCODE}." }
}
finally {
    Remove-Item Env:\DEVICE_TOOLKIT_TEST_SERIAL -ErrorAction SilentlyContinue
}
