$ErrorActionPreference = "Stop"

dotnet publish `
  -c Release `
  -r win-x64 `
  --self-contained true `
  -p:PublishSingleFile=true `
  -p:IncludeNativeLibrariesForSelfExtract=true

$exe = Join-Path $PSScriptRoot "bin\Release\net8.0-windows\win-x64\publish\PinWindow.exe"

Write-Host ""
Write-Host "Готово:" -ForegroundColor Green
Write-Host $exe
