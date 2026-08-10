<p align="center">
  <a href="./README.md">
    <img src="https://img.shields.io/badge/English-0078D4?style=for-the-badge" alt="English">
  </a>
  <a href="./README.ru.md">
    <img src="https://img.shields.io/badge/Русский-D52B1E?style=for-the-badge" alt="Русский">
  </a>
</p>

# PinWindow — Always on Top

[![Latest Release](https://img.shields.io/github/v/release/helliong/pinWindow?style=flat-square&label=release)](https://github.com/helliong/pinWindow/releases/latest) [![Windows](https://img.shields.io/badge/Windows-10%20%7C%2011-0078D4?style=flat-square&logo=windows)](https://github.com/helliong/pinWindow) [![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?style=flat-square&logo=dotnet)](https://dotnet.microsoft.com/) [![Author](https://img.shields.io/badge/Author-helliong-181717?style=flat-square&logo=data%3Aimage%2Fsvg%2Bxml%3Bbase64%2CPHN2ZyB4bWxucz0iaHR0cDovL3d3dy53My5vcmcvMjAwMC9zdmciIHZpZXdCb3g9IjAgMCA5MSA2NSI%2BPHBhdGggZmlsbD0iI2ZmZiIgZmlsbC1ydWxlPSJldmVub2RkIiBkPSJNMCwwIDQ2LDAgNTIsMTMgNTQsMTQgNTQsMTYgNTYsMTcgNTYsMTkgNTgsMjAgNjEsMjYgNjMsMjUgNjQsMjEgNjYsMjAgNjcsMTYgNjksMTUgNzEsOSA3Myw4IDc3LDAgOTEsMCA5MSwxIDg4LDQgODcsOCA4NSw5IDgzLDE1IDgxLDE2IDgwLDIwIDc4LDIxIDc3LDI1IDc1LDI2IDczLDMyIDcxLDMzIDcwLDM3IDY4LDM4IDY2LDQ0IDY0LDQ1IDYyLDUxIDYwLDUyIDU5LDU2IDU3LDU3IDUzLDY1IDAsNjVaTTEzLDEyIDEzLDI1IDM4LDI1IDM4LDM3IDEzLDM3IDEzLDUzIDQ2LDUzIDUwLDQ1IDUyLDQ0IDU0LDM3IDQ5LDMxIDQzLDE5IDQxLDE4IDQwLDE0IDM4LDEyWiIvPjwvc3ZnPg%3D%3D)](https://helliong.space)

<!-- [![SourceForge Downloads](https://img.shields.io/sourceforge/dm/pinwindow?style=flat-square&label=downloads)](https://sourceforge.net/projects/pinwindow/files/) -->

> Keep any application window above all other windows with one click.

[![Download PinWindow](https://a.fsdn.com/con/app/sf-download-button)](https://sourceforge.net/projects/pinwindow/files/latest/download)

PinWindow is a lightweight Windows utility that lets you keep any application window always on top.

You can pin or unpin the active window using a small button near its title bar, a configurable global keyboard shortcut, or the system tray menu.

## Features

- Keep supported application windows always on top
- Pin button near the active window title bar
- Configurable global keyboard shortcut
- Pin or unpin the active window from the system tray
- Customizable pin button size and active color
- Adjustable horizontal and vertical button position
- Optional startup with Windows
- Russian and English interface languages
- Automatic update checks
- Custom update notification window
- GitHub and Bug Report links in Settings
- Desktop notifications
- Lightweight background operation
- Installer and portable release options
- Support for Windows 10 and Windows 11
- No separate .NET installation required for the portable version

## Download

Download the latest PinWindow release from:

- [GitHub Releases](https://github.com/helliong/pinWindow/releases/latest)
- [SourceForge](https://sourceforge.net/projects/pinwindow/files/)

Available downloads:

| File                                    | Description                     |      Size |
| --------------------------------------- | ------------------------------- | --------: |
| `PinWindowSetup-v3.3.3-win-x64.exe`     | Lightweight installer           |  ~2.07 MB |
| `PinWindow-v3.3.3-portable-win-x64.exe` | Self-contained portable version | ~68.33 MB |
| `SHA256SUMS.txt`                        | SHA-256 checksums               |         — |

The installer is recommended for most users.

## Installation

### Installer

1. Download `PinWindowSetup-v3.3.3-win-x64.exe`.
2. Run the installer.
3. Select the installation folder and additional options.
4. Complete the installation and launch PinWindow.

If .NET 8 Desktop Runtime is missing, the installer will download it from Microsoft. An internet connection may be required.

### Portable Version

1. Download `PinWindow-v3.3.3-portable-win-x64.exe`.
2. Move the file to any convenient folder.
3. Run the application.

The portable version includes all required .NET components and does not require installation.

Microsoft Defender SmartScreen may display a warning because the application is not digitally signed. Only run files downloaded from the official GitHub repository or SourceForge project page.

## How to Use

### Title Bar Button

1. Open the application window you want to keep on top.
2. Make that window active.
3. Click the pin button near the window title bar.
4. Click the button again to unpin the window.

A pinned window remains visible above other application windows.

### Keyboard Shortcut

Activate the window you want to pin and press the configured global keyboard shortcut.

The default shortcut is:

```text
Ctrl + Alt + T
```

The shortcut can be changed in the PinWindow settings.

### System Tray

Right-click the PinWindow icon in the Windows system tray to:

- Pin or unpin the active window
- Open the settings window
- Show or hide the title bar pin button
- View the current keyboard shortcut
- Exit PinWindow

Double-click the tray icon to open the settings window.

## Settings

PinWindow allows you to configure:

- Interface language: Russian or English
- Global keyboard shortcut
- Pin button visibility
- Pin button size
- Active pin color
- Horizontal and vertical button offsets
- Desktop notifications
- Automatic startup with Windows

The Settings window also displays the installed PinWindow version and provides links to the GitHub repository and bug report page.

Settings are saved for the current Windows user in:

```text
%AppData%\PinWindow\settings.json
```

## Update Notifications

PinWindow automatically checks whether a newer GitHub release is available.

When an update is found, the application displays a dedicated update window with the installed and latest versions. The user can open the release download page or postpone the update.

PinWindow does not install updates automatically.

## Portable Application

The portable release is distributed as a single self-contained executable file. It can be launched from any folder or USB drive without installation.

Settings are still stored separately in the current Windows user profile.

## System Requirements

- Windows 10 or Windows 11
- 64-bit Windows
- Internet connection for update checks
- The installer may require internet access to download .NET 8 Desktop Runtime
- No separate .NET installation is required for the portable version

## Current Release

### PinWindow v3.3.3

Changes in this release:

- Fixed the pin button disappearing after switching between applications or File Explorer windows
- Fixed overlay recovery after using Snipping Tool or closing Telegram
- Fixed the pin icon remaining on the desktop after closing the target window
- Fixed incorrect pin icon positioning when switching or moving windows
- Added Russian and English interface languages
- Added the application version, GitHub, and Bug Report links to Settings
- Added automatic update checks and a custom update notification window
- Improved overlay stability and rendering performance

## Release Files

### Installer

```text
PinWindowSetup-v3.3.3-win-x64.exe
```

A lightweight installer that downloads the .NET 8 Desktop Runtime from Microsoft if it is not already installed.

### Portable Version

```text
PinWindow-v3.3.3-portable-win-x64.exe
```

A self-contained portable version that does not require installation or a separate .NET runtime.

## SHA-256 Checksums

### Installer

```text
D5596EC14E42AB24E26E151D23035F56C792747D2AD426444738AF106BAAACB2
```

### Portable Version

```text
4022EC6323DCC6748B4A6FF35ECDBB2FA76F511AA8E124893598DBA3836AD14B
```

## Verify the Download

Open PowerShell in the folder containing the downloaded files.

Verify the installer:

```powershell
Get-FileHash ".\PinWindowSetup-v3.3.3-win-x64.exe" -Algorithm SHA256
```

Verify the portable version:

```powershell
Get-FileHash ".\PinWindow-v3.3.3-portable-win-x64.exe" -Algorithm SHA256
```

The returned hashes must match the checksums published above and in `SHA256SUMS.txt`.

## Source Code

The complete source code is available on GitHub:

[https://github.com/helliong/pinWindow](https://github.com/helliong/pinWindow)

## Bug Reports and Suggestions

Report bugs or suggest new features through GitHub Issues:

[https://github.com/helliong/pinWindow/issues](https://github.com/helliong/pinWindow/issues)

When reporting a problem, please include:

- Your Windows version
- Your PinWindow version
- The application where the problem occurred
- Steps required to reproduce the issue
- The full error message, when available
- A screenshot or screen recording, when useful

## Build From Source

### Requirements

- Windows 10 or Windows 11
- Git
- .NET 8 SDK
- Inno Setup 7 for building the installer

Clone the repository:

```powershell
git clone https://github.com/helliong/pinWindow.git
cd pinWindow
```

Run PinWindow in development mode:

```powershell
dotnet run
```

Create a Release build:

```powershell
dotnet build -c Release
```

### Build the Installer

```powershell
powershell -ExecutionPolicy Bypass `
    -File ".\scripts\publish-installer.ps1" `
    -Version 3.3.3
```

The installer will be created in:

```text
artifacts\installer\
```

### Build the Portable Version

```powershell
dotnet publish `
    -c Release `
    -r win-x64 `
    --self-contained true `
    -p:PublishSingleFile=true `
    -p:IncludeNativeLibrariesForSelfExtract=true `
    -p:EnableCompressionInSingleFile=true `
    -p:DebugType=None `
    -p:DebugSymbols=false `
    -o ".\artifacts\portable\publish"
```

The compiled executable will be created in:

```text
artifacts\portable\publish\
```

## Project Links

- [SourceForge Project](https://sourceforge.net/projects/pinwindow/)
- [GitHub Repository](https://github.com/helliong/pinWindow)
- [Latest GitHub Release](https://github.com/helliong/pinWindow/releases/latest)
- [Bug Reports](https://github.com/helliong/pinWindow/issues)
- [Source Code](https://github.com/helliong/pinWindow)

## Contributing

Contributions are welcome.

You can contribute by:

- Reporting bugs
- Suggesting new features
- Improving documentation
- Submitting a pull request

Before submitting a pull request, make sure the project builds successfully:

```powershell
dotnet build
```

## Author

Developed by [helliong](https://github.com/helliong).

---

<p align="center">
  <a href="./README.md">English</a>
  ·
  <a href="./README.ru.md">Русский</a>
</p>
