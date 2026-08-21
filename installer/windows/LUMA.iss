#define MyAppName "L.U.M.A."
#define MyAppVersion "0.1.0"
#define MyAppPublisher "L.U.M.A. Development Team"
#define MyAppExeName "AnalisadorAmastigotas.exe"

[Setup]
AppId={{E1320D12-073E-4CD9-9F3C-0A8EB021A463}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppPublisher={#MyAppPublisher}

DefaultDirName={localappdata}\Programs\LUMA
DefaultGroupName=L.U.M.A.

OutputDir=..\..\artifacts
OutputBaseFilename=LUMA-Windows-x64-Setup

Compression=lzma2
SolidCompression=yes
WizardStyle=modern

PrivilegesRequired=lowest
ArchitecturesAllowed=x64compatible
ArchitecturesInstallIn64BitMode=x64compatible

UninstallDisplayName=L.U.M.A.

[Files]
Source: "..\..\artifacts\windows\app\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs

[Icons]
Name: "{group}\L.U.M.A."; Filename: "{app}\{#MyAppExeName}"
Name: "{autodesktop}\L.U.M.A."; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon

[Tasks]
Name: "desktopicon"; Description: "Criar atalho na área de trabalho"; GroupDescription: "Atalhos:"; Flags: unchecked

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "Abrir L.U.M.A."; Flags: nowait postinstall skipifsilent