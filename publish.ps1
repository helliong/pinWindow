$ErrorActionPreference = "Stop"

Write-Host "Publishing PinWindow for Windows x64..." -ForegroundColor Cyan

dotnet publish .\PinWindow.csproj `
  -c Release `
  -r win-x64 `
  --self-contained true `
  -p:PublishSingleFile=true `
  -p:IncludeNativeLibrariesForSelfExtract=true

Write-Host "" 
Write-Host "Ready:" -ForegroundColor Green
Write-Host ".\bin\Release\net8.0-windows\win-x64\publish\PinWindow.exe"
