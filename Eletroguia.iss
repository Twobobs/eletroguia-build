#define AppName "Eletroguia"
#define AppVer "1.0.0"

[Setup]
AppId={{B7D1C0E2-5A3F-4C57-9E21-6F0A7D4A91C3}
AppName={#AppName}
AppVersion={#AppVer}
DefaultDirName={autopf}\{#AppName}
DefaultGroupName={#AppName}
ArchitecturesAllowed=x64compatible
ArchitecturesInstallIn64BitMode=x64compatible
OutputDir=dist
OutputBaseFilename=Eletroguia-Setup
SetupIconFile=Resources\Eletroguia.ico
Compression=lzma2/max
SolidCompression=yes
WizardStyle=modern
UninstallDisplayIcon={app}\Eletroguia.exe
PrivilegesRequired=admin

[Languages]
Name: "brazilianportuguese"; MessagesFile: "compiler:Languages\BrazilianPortuguese.isl"

[Tasks]
Name: "desktopicon"; Description: "Criar atalho na Área de Trabalho"; Flags: checkedonce

[Files]
Source: "publish\Eletroguia.exe"; DestDir: "{app}"; Flags: ignoreversion

[Icons]
Name: "{group}\{#AppName}"; Filename: "{app}\Eletroguia.exe"
Name: "{autodesktop}\Eletroguia Cálculo de Motores"; Filename: "{app}\Eletroguia.exe"; Comment: "Sistema de Cálculo para Motores de Indução Monofásicos e Trifásicos"; Tasks: desktopicon

[Run]
Filename: "{app}\Eletroguia.exe"; Description: "Abrir o Eletroguia"; Flags: nowait postinstall skipifsilent

[Code]
function WebView2Instalado: Boolean;
var v: String;
begin
  Result := (RegQueryStringValue(HKLM, 'SOFTWARE\WOW6432Node\Microsoft\EdgeUpdate\Clients\{F3017226-FE2A-4295-8BDF-00C3A9A7E4C5}', 'pv', v)
         or RegQueryStringValue(HKCU, 'Software\Microsoft\EdgeUpdate\Clients\{F3017226-FE2A-4295-8BDF-00C3A9A7E4C5}', 'pv', v))
         and (v <> '') and (v <> '0.0.0.0');
end;

var DlPage: TDownloadWizardPage;

procedure InitializeWizard;
begin
  DlPage := CreateDownloadPage(SetupMessage(msgWizardPreparing), SetupMessage(msgPreparingDesc), nil);
end;

function NextButtonClick(CurPageID: Integer): Boolean;
var rc: Integer;
begin
  Result := True;
  if (CurPageID = wpReady) and (not WebView2Instalado) then
  begin
    DlPage.Clear;
    DlPage.Add('https://go.microsoft.com/fwlink/p/?LinkId=2124703', 'MicrosoftEdgeWebview2Setup.exe', '');
    DlPage.Show;
    try
      try
        DlPage.Download;
        Exec(ExpandConstant('{tmp}\MicrosoftEdgeWebview2Setup.exe'), '/silent /install', '', SW_HIDE, ewWaitUntilTerminated, rc);
      except
        if not DlPage.AbortedByUser then
          SuppressibleMsgBox('Não foi possível baixar o WebView2. Instale o "Microsoft Edge WebView2 Runtime" e execute este instalador de novo.', mbError, MB_OK, IDOK);
        Result := False;
      end;
    finally
      DlPage.Hide;
    end;
  end;
end;
