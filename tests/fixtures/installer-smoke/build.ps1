$ErrorActionPreference = 'Stop'

$sdkRoot = $env:ANDROID_SDK_ROOT
if ([string]::IsNullOrWhiteSpace($sdkRoot)) { $sdkRoot = $env:ANDROID_HOME }
if ([string]::IsNullOrWhiteSpace($sdkRoot)) { throw 'Set ANDROID_SDK_ROOT or ANDROID_HOME to an installed Android SDK.' }

$buildTools = Join-Path $sdkRoot 'build-tools\36.0.0'
$platform = Join-Path $sdkRoot 'platforms\android-36'
$javaHome = Join-Path $env:ProgramFiles 'Android\Android Studio\jbr'
if (-not (Test-Path (Join-Path $javaHome 'bin\java.exe'))) { throw 'Android Studio Java runtime was not found.' }
if (-not (Test-Path (Join-Path $platform 'android.jar'))) { throw 'Android API 36 platform was not found.' }

$repoRoot = (Resolve-Path (Join-Path $PSScriptRoot '..\..\..')).Path
$artifactDirectory = Join-Path $repoRoot 'artifacts'
New-Item -ItemType Directory -Path $artifactDirectory -Force | Out-Null
$artifactPath = Join-Path $artifactDirectory 'DeviceToolkitInstallTest.apk'
$tempRoot = Join-Path $env:TEMP ("device-toolkit-install-test-{0}" -f [Guid]::NewGuid().ToString('N'))
$classesDirectory = Join-Path $tempRoot 'classes'
$dexDirectory = Join-Path $tempRoot 'dex'
$compiledResources = Join-Path $tempRoot 'compiled-resources'
New-Item -ItemType Directory -Path $classesDirectory, $dexDirectory, $compiledResources -Force | Out-Null

function Invoke-BuildTool([string] $FilePath, [string[]] $Arguments) {
    & $FilePath @Arguments
    if ($LASTEXITCODE -ne 0) { throw "Build tool failed with exit code ${LASTEXITCODE}: $FilePath" }
}

try {
    $env:JAVA_HOME = $javaHome
    $manifest = Join-Path $PSScriptRoot 'AndroidManifest.xml'
    $source = Join-Path $PSScriptRoot 'TestActivity.java'
    $androidJar = Join-Path $platform 'android.jar'
    $unsignedApk = Join-Path $tempRoot 'unsigned.apk'
    $alignedApk = Join-Path $tempRoot 'aligned.apk'
    $signedApk = Join-Path $tempRoot 'signed.apk'
    $keystore = Join-Path $tempRoot 'ephemeral-test-key.jks'
    $password = [Guid]::NewGuid().ToString('N')

    Invoke-BuildTool (Join-Path $buildTools 'aapt2.exe') @(
        'compile', '--dir', (Join-Path $PSScriptRoot 'res'), '-o', $compiledResources
    )
    $stringTable = Get-ChildItem -LiteralPath $compiledResources -Filter 'values_strings.arsc.flat' |
        Select-Object -First 1 -ExpandProperty FullName
    if (-not $stringTable) { throw 'AAPT2 did not produce the app label resource.' }
    Invoke-BuildTool (Join-Path $buildTools 'aapt2.exe') @(
        'link', '-o', $unsignedApk, '-I', $androidJar, '--manifest', $manifest,
        '--min-sdk-version', '23', '--target-sdk-version', '35', '-R', $stringTable,
        '--auto-add-overlay'
    )
    Invoke-BuildTool (Join-Path $javaHome 'bin\javac.exe') @(
        '-Xlint:-options', '-source', '8', '-target', '8', '-bootclasspath', $androidJar,
        '-d', $classesDirectory, $source
    )
    Invoke-BuildTool (Join-Path $buildTools 'd8.bat') @(
        '--min-api', '23', '--lib', $androidJar, '--output', $dexDirectory,
        (Join-Path $classesDirectory 'com\petershaker\devicetoolkit\installtest\TestActivity.class')
    )
    Invoke-BuildTool (Join-Path $javaHome 'bin\jar.exe') @(
        'uf', $unsignedApk, '-C', $dexDirectory, 'classes.dex'
    )
    Invoke-BuildTool (Join-Path $buildTools 'zipalign.exe') @('-f', '4', $unsignedApk, $alignedApk)
    Invoke-BuildTool (Join-Path $javaHome 'bin\keytool.exe') @(
        '-genkeypair', '-keystore', $keystore, '-alias', 'install-test', '-keyalg', 'RSA',
        '-keysize', '2048', '-validity', '3650', '-storepass', $password, '-keypass', $password,
        '-dname', 'CN=Device Toolkit Temporary Install Test'
    )
    Invoke-BuildTool (Join-Path $buildTools 'apksigner.bat') @(
        'sign', '--ks', $keystore, '--ks-key-alias', 'install-test',
        '--ks-pass', "pass:$password", '--key-pass', "pass:$password", '--out', $signedApk, $alignedApk
    )
    Invoke-BuildTool (Join-Path $buildTools 'apksigner.bat') @('verify', '--verbose', $signedApk)
    Move-Item -LiteralPath $signedApk -Destination $artifactPath -Force
    Get-FileHash -Algorithm SHA256 -LiteralPath $artifactPath
    Get-Item -LiteralPath $artifactPath | Select-Object FullName, Length
}
finally {
    $tempBase = [IO.Path]::GetFullPath($env:TEMP).TrimEnd('\') + '\'
    $tempFullPath = [IO.Path]::GetFullPath($tempRoot)
    if ($tempFullPath.StartsWith($tempBase, [StringComparison]::OrdinalIgnoreCase) -and
        (Split-Path $tempFullPath -Leaf) -match '^device-toolkit-install-test-[0-9a-f]{32}$') {
        Remove-Item -LiteralPath $tempFullPath -Recurse -Force -ErrorAction SilentlyContinue
    }
}
