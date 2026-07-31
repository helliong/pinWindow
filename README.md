<p align="center">
  <a href="./README.md">
    <img src="https://img.shields.io/badge/English-0078D4?style=for-the-badge" alt="English">
  </a>
  <a href="./README.ru.md">
    <img src="https://img.shields.io/badge/Русский-D52B1E?style=for-the-badge" alt="Русский">
  </a>
</p>

# PinWindow — Always on Top

[![Latest Release](https://img.shields.io/github/v/release/helliong/pinWindow?style=flat-square&label=release)](https://github.com/helliong/pinWindow/releases/latest) [![Windows](https://img.shields.io/badge/Windows-10%20%7C%2011-0078D4?style=flat-square&logo=windows)](https://github.com/helliong/pinWindow) [![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?style=flat-square&logo=dotnet)](https://dotnet.microsoft.com/)

<!-- [![SourceForge Downloads](https://img.shields.io/sourceforge/dm/pinwindow?style=flat-square&label=downloads)](https://sourceforge.net/projects/pinwindow/files/) -->

> Keep any application window above all other windows with one click.

[![Download PinWindow](https://a.fsdn.com/con/app/sf-download-button)](https://sourceforge.net/projects/pinwindow/files/latest/download)

PinWindow is a lightweight Windows utility that lets you keep any application window always on top.

You can pin or unpin the active window using a small button near its title bar, a configurable global keyboard shortcut, or the system tray menu.

## Features

- Keep any supported application window always on top
- Pin button near the active window title bar
- Configurable global keyboard shortcut
- Pin or unpin the active window from the system tray
- Customizable pin button size
- Customizable active pin color
- Adjustable horizontal and vertical button position
- Optional startup with Windows
- Desktop notifications
- Lightweight background operation
- Portable single-file application
- Support for Windows 10 and Windows 11
- No separate .NET installation required

## Download

Download the latest version of PinWindow from:

- [SourceForge](https://sourceforge.net/projects/pinwindow/)
- [GitHub Releases](https://github.com/helliong/pinWindow/releases/latest)

For most users, the SourceForge download is recommended:

[Download the latest version](https://sourceforge.net/projects/pinwindow/files/latest/download)

## Installation

PinWindow does not require installation.

1. Download the latest `PinWindow.exe` release.
2. Move the file to any convenient folder.
3. Run the application.
4. The PinWindow icon will appear in the Windows system tray.

Windows may display a Microsoft Defender SmartScreen warning because the application is not digitally signed.

Only run the application when it was downloaded from the official GitHub repository or SourceForge project page.

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

- Global keyboard shortcut
- Pin button visibility
- Pin button size
- Active pin color
- Horizontal button offset
- Vertical button offset
- Desktop notifications
- Automatic startup with Windows

Settings are saved for the current Windows user in:

```text
%AppData%\PinWindow\settings.json
```

## Portable Application

PinWindow is distributed as a single executable file.

It does not require an installer and can be launched from:

- Any folder on your computer
- A USB drive
- A portable applications directory

The self-contained release includes all required .NET components.

## System Requirements

- Windows 10 or Windows 11
- 64-bit Windows
- Approximately 160 MB of free disk space
- No separate .NET installation required for the self-contained release

## Current Release

### PinWindow v3.3.1

Changes in this release:

- Updated the system tray icon
- Updated the executable file icon
- Improved icon visibility at small sizes
- Added multiple icon resolutions for Windows
- Improved the visual appearance of the tray icon

## SHA-256 Checksum

The SHA-256 checksum for the current release is:

```text
43f83bd4c69dce7321956f88eaab4bb418513ad6bd1dff51b483f79ec6cb4955
```

The checksum applies to:

```text
PinWindow-v3.3.1-win-x64.exe
```

## Verify the Download

Open PowerShell in the folder containing the downloaded file and run:

```powershell
Get-FileHash ".\PinWindow-v3.3.1-win-x64.exe" -Algorithm SHA256
```

The returned hash should match:

```text
43f83bd4c69dce7321956f88eaab4bb418513ad6bd1dff51b483f79ec6cb4955
```

You can also use the Windows `certutil` command:

```powershell
certutil -hashfile "PinWindow-v3.3.1-win-x64.exe" SHA256
```

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

Clone the repository:

```powershell
git clone https://github.com/helliong/pinWindow.git
cd pinWindow
```

Run the application in development mode:

```powershell
dotnet run
```

Create a self-contained release build:

```powershell
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -p:EnableCompressionInSingleFile=true
```

The compiled executable will be located in:

```text
bin\Release\net8.0-windows\win-x64\publish\
```

You can also use the included publishing script:

```powershell
powershell -ExecutionPolicy Bypass -File .\publish.ps1
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
