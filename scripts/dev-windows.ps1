param(
    [switch]$RebuildProcessing
)

$ErrorActionPreference = "Stop"

# ============================================================
# Localiza automaticamente a raiz do projeto
# ============================================================

$Root = (Resolve-Path (Join-Path $PSScriptRoot "..")).Path

$ProcessingDir = Join-Path $Root "processing"
$ProcessingExe = Join-Path $ProcessingDir "windows\process_image.exe"

$ProjectFile = Join-Path $Root `
    "interfaces\AnalisadorAmastigotas\AnalisadorAmastigotas.csproj"

Write-Host ""
Write-Host "==========================================" -ForegroundColor Cyan
Write-Host "       L.U.M.A. - Development Mode" -ForegroundColor Cyan
Write-Host "==========================================" -ForegroundColor Cyan
Write-Host ""

Write-Host "Projeto:"
Write-Host $Root
Write-Host ""

# ============================================================
# Verifica .NET
# ============================================================

if (-not (Get-Command dotnet -ErrorAction SilentlyContinue)) {
    Write-Host "ERRO: .NET SDK nao encontrado." -ForegroundColor Red
    exit 1
}

Write-Host "[OK] .NET encontrado." -ForegroundColor Green

# ============================================================
# Verifica motor Python compilado
# ============================================================

if ($RebuildProcessing -or -not (Test-Path $ProcessingExe)) {

    if ($RebuildProcessing) {
        Write-Host ""
        Write-Host "Recompilando motor de processamento..." -ForegroundColor Yellow
    }
    else {
        Write-Host ""
        Write-Host "process_image.exe nao encontrado." -ForegroundColor Yellow
        Write-Host "Gerando motor de processamento..." -ForegroundColor Yellow
    }

    $BuildProcessing = Join-Path $ProcessingDir "build_windows.bat"

    & $BuildProcessing

    if ($LASTEXITCODE -ne 0) {
        Write-Host ""
        Write-Host "ERRO ao gerar process_image.exe." -ForegroundColor Red
        exit 1
    }
}

if (-not (Test-Path $ProcessingExe)) {
    Write-Host "ERRO: process_image.exe continua ausente." -ForegroundColor Red
    exit 1
}

Write-Host "[OK] Motor de processamento encontrado." -ForegroundColor Green

# ============================================================
# Abre o projeto no VS Code
# ============================================================

if (Get-Command code -ErrorAction SilentlyContinue) {

    Write-Host "[OK] Abrindo projeto no VS Code..." -ForegroundColor Green

    Start-Process `
        -FilePath "code" `
        -ArgumentList "`"$Root`""
}
else {
    Write-Host ""
    Write-Host "AVISO: comando 'code' nao foi encontrado." -ForegroundColor Yellow
    Write-Host "Abra o VS Code manualmente na pasta:"
    Write-Host $Root
}

# ============================================================
# Executa o aplicativo
# ============================================================

Write-Host ""
Write-Host "Iniciando L.U.M.A..." -ForegroundColor Cyan
Write-Host ""
Write-Host "Para encerrar pelo terminal: Ctrl+C"
Write-Host ""

dotnet run --project $ProjectFile