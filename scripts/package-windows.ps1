$ErrorActionPreference = "Stop"

# ============================================================
# Localiza a raiz do projeto
# ============================================================

$Root = (Resolve-Path (Join-Path $PSScriptRoot "..")).Path

$AppDir = Join-Path $Root "artifacts\windows\app"
$InstallerScript = Join-Path $Root "installer\windows\LUMA.iss"
$OutputFile = Join-Path $Root "artifacts\LUMA-Windows-x64-Setup.exe"

Write-Host ""
Write-Host "==========================================" -ForegroundColor Cyan
Write-Host "       L.U.M.A. - Windows Package" -ForegroundColor Cyan
Write-Host "==========================================" -ForegroundColor Cyan
Write-Host ""

# ============================================================
# Verifica build
# ============================================================

if (-not (Test-Path $AppDir)) {
    Write-Host "ERRO: build Windows nao encontrado." -ForegroundColor Red
    Write-Host ""
    Write-Host "Execute primeiro:"
    Write-Host ".\scripts\build-windows.ps1"
    exit 1
}

# ============================================================
# Procura Inno Setup
# ============================================================

$PossiblePaths = @(
    "$env:ProgramFiles\Inno Setup 7\ISCC.exe",
    "${env:ProgramFiles(x86)}\Inno Setup 7\ISCC.exe",
    "$env:LOCALAPPDATA\Programs\Inno Setup 7\ISCC.exe",
    "$env:ProgramFiles\Inno Setup 6\ISCC.exe",
    "${env:ProgramFiles(x86)}\Inno Setup 6\ISCC.exe"
)

$ISCC = $null

foreach ($Path in $PossiblePaths) {
    if ($Path -and (Test-Path $Path)) {
        $ISCC = $Path
        break
    }
}

if (-not $ISCC) {
    Write-Host "ERRO: Inno Setup nao encontrado." -ForegroundColor Red
    exit 1
}

Write-Host "[OK] Inno Setup:"
Write-Host $ISCC
Write-Host ""

# ============================================================
# Gera instalador
# ============================================================

Write-Host "Gerando instalador..." -ForegroundColor Yellow

& $ISCC $InstallerScript

if ($LASTEXITCODE -ne 0) {
    Write-Host "ERRO ao gerar instalador." -ForegroundColor Red
    exit 1
}

if (-not (Test-Path $OutputFile)) {
    Write-Host "ERRO: Setup.exe nao foi encontrado." -ForegroundColor Red
    exit 1
}

Write-Host ""
Write-Host "==========================================" -ForegroundColor Green
Write-Host "INSTALADOR GERADO COM SUCESSO" -ForegroundColor Green
Write-Host "==========================================" -ForegroundColor Green
Write-Host ""
Write-Host $OutputFile