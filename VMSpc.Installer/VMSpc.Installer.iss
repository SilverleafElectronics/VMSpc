; Script generated by the Inno Setup Script Wizard.
; SEE THE DOCUMENTATION FOR DETAILS ON CREATING INNO SETUP SCRIPT FILES!

#define MyAppName "VMSpc"
#define MyAppVersion "5.0.13"
#define MyAppPublisher "SilverLeaf Electronics, Inc"
#define MyAppURL "https://silverleafelectronics.com"
#define MyAppExeName "VMSpc.exe"

[Setup]
; NOTE: The value of AppId uniquely identifies this application. Do not use the same AppId value in installers for other applications.
; (To generate a new GUID, click Tools | Generate GUID inside the IDE.)
AppId={{AAB45884-91E8-4F53-B9DD-D1FBB2BCE575}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
;AppVerName={#MyAppName} {#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}
AppUpdatesURL={#MyAppURL}
DefaultDirName=C:\VMSpc\5.0.13
DisableProgramGroupPage=yes
; Uncomment the following line to run in non administrative install mode (install for current user only.)
;PrivilegesRequired=lowest
OutputBaseFilename=VMSpcInstaller
Compression=lzma
SolidCompression=yes
WizardStyle=modern

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked

[Files]
Source: "C:\VMSpc\VMSpcPackageStager\VMSpc.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\VMSpc\VMSpcPackageStager\rawlogs\*"; DestDir: "{app}\rawlogs"; Flags: ignoreversion recursesubdirs createallsubdirs
Source: "C:\VMSpc\VMSpcPackageStager\logs\*"; DestDir: "{app}\logs"; Flags: ignoreversion recursesubdirs createallsubdirs
Source: "C:\VMSpc\VMSpcPackageStager\history_files"; DestDir: "{app}\history_files"; Flags: ignoreversion recursesubdirs createallsubdirs
Source: "C:\VMSpc\VMSpcPackageStager\engines\*"; DestDir: "{app}\engines"; Flags: ignoreversion recursesubdirs createallsubdirs
Source: "C:\VMSpc\VMSpcPackageStager\configuration\*"; DestDir: "{app}\configuration"; Flags: ignoreversion recursesubdirs createallsubdirs
; NOTE: Don't use "Flags: ignoreversion" on any shared system files

[Code]
function FrameworkIsNotInstalled: Boolean;
begin
  Result := not RegKeyExists(HKEY_LOCAL_MACHINE, 'Software\Microsoft\.NETFramework\policy\v4.0');
end;

[Icons]
Name: "{autoprograms}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{autodesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#StringChange(MyAppName, '&', '&&')}}"; Flags: nowait postinstall skipifsilent

