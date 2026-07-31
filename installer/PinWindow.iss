#define MyAppName "PinWindow"
#ifndef MyAppVersion
  #define MyAppVersion "3.3.2"
#endif
#define MyAppPublisher "helliong"
#define MyAppExeName "PinWindow.exe"

[Setup]
AppId={{A3F0B093-07D4-41C8-A4A9-BCE41A15DF47}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL=https://github.com/helliong/pinWindow
AppSupportURL=https://github.com/helliong/pinWindow/issues
AppUpdatesURL=https://github.com/helliong/pinWindow/releases

DefaultDirName={localappdata}\Programs\{#MyAppName}
DefaultGroupName={#MyAppName}
DisableProgramGroupPage=yes

OutputDir=..\artifacts\installer
OutputBaseFilename=PinWindowSetup-v{#MyAppVersion}-win-x64
SetupIconFile=..\PinWindow.ico
UninstallDisplayIcon={app}\{#MyAppExeName}

Compression=lzma2/ultra64
SolidCompression=yes
WizardStyle=modern

PrivilegesRequired=lowest
ArchitecturesAllowed=x64compatible
ArchitecturesInstallIn64BitMode=x64compatible
MinVersion=10.0

CloseApplications=yes
RestartApplications=no
AppMutex=PinWindow.SingleInstance.1499A0DA-7CD9-43C8-88E1-67E4DBCEAD43

VersionInfoVersion={#MyAppVersion}.0
VersionInfoCompany={#MyAppPublisher}
VersionInfoDescription=PinWindow Installer
VersionInfoProductName={#MyAppName}
VersionInfoProductVersion={#MyAppVersion}

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"
Name: "russian"; MessagesFile: "compiler:Languages\Russian.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked

[Files]
Source: "..\artifacts\app\win-x64\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs

[Icons]
Name: "{group}\PinWindow"; Filename: "{app}\{#MyAppExeName}"
Name: "{autodesktop}\PinWindow"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchProgram,PinWindow}"; Flags: nowait postinstall skipifsilent

[Code]

const
  DotNetRuntimeUrl =
    'https://aka.ms/dotnet/8.0/windowsdesktop-runtime-win-x64.exe';

  DotNetRuntimeFileName =
    'windowsdesktop-runtime-8.0-win-x64.exe';

var
  DownloadPage: TDownloadWizardPage;
  NeedDotNetRuntime: Boolean;
  RuntimeDownloaded: Boolean;

function IsDotNet8DesktopInstalled: Boolean;
var
  DotNetPath: String;
  ResultCode: Integer;
  RuntimeOutput: TExecOutput;
  LineIndex: Integer;
begin
  Result := False;

  DotNetPath := ExpandConstant(
    '{pf64}\dotnet\dotnet.exe');

  if not FileExists(DotNetPath) then
  begin
    Exit;
  end;

  try
    if ExecAndCaptureOutput(
      DotNetPath,
      '--list-runtimes',
      '',
      SW_SHOWNORMAL,
      ewWaitUntilTerminated,
      ResultCode,
      RuntimeOutput)
    then
    begin
      if ResultCode = 0 then
      begin
        for LineIndex :=
          0 to GetArrayLength(RuntimeOutput.StdOut) - 1 do
        begin
          if Pos(
            'Microsoft.WindowsDesktop.App 8.',
            RuntimeOutput.StdOut[LineIndex]) = 1
          then
          begin
            Result := True;
            Exit;
          end;
        end;
      end;
    end;
  except
    Log(
      'Unable to check installed .NET runtimes: ' +
      GetExceptionMessage);
  end;
end;

procedure InitializeWizard;
begin
  NeedDotNetRuntime := not IsDotNet8DesktopInstalled;
  RuntimeDownloaded := False;

  DownloadPage := CreateDownloadPage(
    SetupMessage(msgWizardPreparing),
    SetupMessage(msgPreparingDesc),
    nil);

  DownloadPage.ShowBaseNameInsteadOfUrl := True;
end;

function NextButtonClick(CurPageID: Integer): Boolean;
var
  ErrorMessage: String;
begin
  Result := True;

  if
    (CurPageID = wpReady) and
    NeedDotNetRuntime and
    (not IsDotNet8DesktopInstalled) and
    (not RuntimeDownloaded)
  then
  begin
    DownloadPage.Clear;

    DownloadPage.Add(
      DotNetRuntimeUrl,
      DotNetRuntimeFileName,
      '');

    DownloadPage.Show;

    try
      try
        DownloadPage.Download;
        RuntimeDownloaded := True;
      except
        if DownloadPage.AbortedByUser then
          Log('The runtime download was cancelled.')
        else
        begin
          ErrorMessage := Format(
            '%s: %s', [DownloadPage.LastBaseNameOrUrl, GetExceptionMessage]);

          SuppressibleMsgBox(
            AddPeriod(ErrorMessage),
            mbCriticalError,
            MB_OK,
            IDOK);
        end;

        Result := False;
      end;
    finally
      DownloadPage.Hide;
    end;
  end;
end;

function PrepareToInstall(
  var NeedsRestart: Boolean): String;
var
  RuntimeInstallerPath: String;
  ResultCode: Integer;
begin
  Result := '';

  if
    NeedDotNetRuntime and
    (not IsDotNet8DesktopInstalled)
  then
  begin
    if not RuntimeDownloaded then
    begin
      Result := '.NET 8 Desktop Runtime was not downloaded.';
      Exit;
    end;

    RuntimeInstallerPath :=
      ExpandConstant('{tmp}\') +
      DotNetRuntimeFileName;

    if not ShellExec(
      'runas',
      RuntimeInstallerPath,
      '/install /quiet /norestart',
      '',
      SW_SHOWNORMAL,
      ewWaitUntilTerminated,
      ResultCode)
    then
    begin
      Result :=
        'Unable to start the .NET 8 Desktop Runtime installer.';
      Exit;
    end;

    if ResultCode = 3010 then
    begin
      NeedsRestart := True;
      Exit;
    end;

    if ResultCode <> 0 then
    begin
      Result := Format(
        '.NET 8 Desktop Runtime installation failed with code %d.', [ResultCode]);
      Exit;
    end;

    if not IsDotNet8DesktopInstalled then
    begin
      Result :=
        '.NET 8 Desktop Runtime was not detected after installation.';
    end;
  end;
end;