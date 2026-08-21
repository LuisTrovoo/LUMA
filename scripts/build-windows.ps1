$ErrorActionPreference = "Stop"
$Root = (Resolve-Path (Join-Path $PSScriptRoot "..")).Path
$Project = Join-Path $Root "interfaces\AnalisadorAmastigotas\AnalisadorAmastigotas.csproj"
$Artifacts = Join-Path $Root "artifacts\windows"
$AppOut = Join-Path $Artifacts "app"

Write-Host "=== L.U.M.A. | Windows x64 ==="

& (Join-Path $Root "processing\build_windows.bat")
if ($LASTEXITCODE -ne 0) { throw "Falha ao gerar o motor Python para Windows." }

if (Test-Path $Artifacts) { Remove-Item $Artifacts -Recurse -Force }
New-Item -ItemType Directory -Path $AppOut -Force | Out-Null

dotnet restore $Project
dotnet publish $Project -c Release -r win-x64 --self-contained true -o $AppOut
if ($LASTEXITCODE -ne 0) { throw "Falha no dotnet publish." }

$Portable = Join-Path $Root "artifacts\LUMA-Windows-x64-Portable.zip"
if (Test-Path $Portable) { Remove-Item $Portable -Force }
Compress-Archive -Path (Join-Path $AppOut "*") -DestinationPath $Portable

Write-Host ""
Write-Host "Build concluido."
Write-Host "Aplicativo: $AppOut"
Write-Host "Portatil:   $Portable"
