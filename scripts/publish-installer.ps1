param(
    [ValidatePattern('^\d+\.\d+\.\d+$')]
    [string]$Version = "3.3.1"
)

$ErrorActionPreference = "Stop"

$projectRoot = Split-Path -Parent $PSScriptRoot

$projectFile = Join-Path $projectRoot "PinWindow.csproj"
$installerScript = Join-Path $projectRoot "installer\PinWindow.iss"
$publishDirectory = Join-Path $projectRoot "artifacts\app\win-x64"
$installerDirectory = Join-Path $projectRoot "artifacts\installer"

if (-not (Test-Path $projectFile)) {
    throw "Project file not found: $projectFile"
}

if (-not (Test-Path $installerScript)) {
    throw "Installer script not found: $installerScript"
}

if (Test-Path $publishDirectory) {
    Remove-Item $publishDirectory -Recurse -Force
}

New-Item -ItemType Directory -Force $publishDirectory | Out-Null
New-Item -ItemType Directory -Force $installerDirectory | Out-Null

Write-Host "Publishing PinWindow..." -ForegroundColor Cyan

dotnet publish $projectFile `
    -c Release `
    -r win-x64 `
    --self-contained false `
    "-p:Version=$Version" `
    "-p:AssemblyVersion=$Version.0" `
    "-p:FileVersion=$Version.0" `
    -p:PublishSingleFile=true `
    -p:DebugType=None `
    -p:DebugSymbols=false `
    -o $publishDirectory

if ($LASTEXITCODE -ne 0) {
    throw "PinWindow publishing failed."
}

$programFiles = [Environment]::GetFolderPath("ProgramFiles")

$innoCandidates = @(
    (Join-Path $programFiles "Inno Setup 7\ISCC.exe"),
    (Join-Path $programFiles "Inno Setup 6\ISCC.exe")
)

$innoCompiler = $innoCandidates |
    Where-Object { Test-Path $_ } |
    Select-Object -First 1

if (-not $innoCompiler) {
    throw "Inno Setup Compiler was not found."
}

Write-Host "Building installer..." -ForegroundColor Cyan

& $innoCompiler "/DMyAppVersion=$Version" $installerScript

if ($LASTEXITCODE -ne 0) {
    throw "Installer compilation failed."
}

Write-Host ""
Write-Host "Installer built successfully:" -ForegroundColor Green
Write-Host $installerDirectory