@echo off
REM Rode este arquivo no Windows (clique duas vezes, ou "cmd /c build.bat").
REM Requisitos: .NET 8 SDK (https://dotnet.microsoft.com/download/dotnet/8.0)
REM             Inno Setup 6.3+ com "iscc" no PATH (https://jrsoftware.org/isinfo.php)

if not exist "Resources\app.bin" (
  echo [erro] Resources\app.bin nao existe. Rode "node tools\encrypt.mjs" antes.
  exit /b 1
)

dotnet publish Eletroguia.csproj -c Release -o publish
if errorlevel 1 goto erro

iscc Eletroguia.iss
if errorlevel 1 goto erro

echo.
echo Pronto: dist\Eletroguia-Setup.exe
exit /b 0

:erro
echo.
echo Build falhou. Veja a mensagem acima.
exit /b 1
