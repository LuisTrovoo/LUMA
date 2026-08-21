#!/usr/bin/env bash

set -e

# ============================================================
# Localiza automaticamente a raiz do projeto
# ============================================================

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
ROOT="$(cd "$SCRIPT_DIR/.." && pwd)"

PROCESSING_DIR="$ROOT/processing"
PROCESSING_EXE="$PROCESSING_DIR/linux/process_image"

PROJECT_FILE="$ROOT/interfaces/AnalisadorAmastigotas/AnalisadorAmastigotas.csproj"

echo
echo "=========================================="
echo "       L.U.M.A. - Development Mode"
echo "                  Linux"
echo "=========================================="
echo

echo "Projeto:"
echo "$ROOT"
echo

# ============================================================
# Verifica .NET
# ============================================================

if ! command -v dotnet >/dev/null 2>&1; then
    echo "ERRO: .NET SDK não encontrado."
    echo "Instale o .NET SDK antes de continuar."
    exit 1
fi

echo "[OK] .NET encontrado."

# ============================================================
# Verifica motor Python compilado
# ============================================================

REBUILD_PROCESSING=false

if [[ "${1:-}" == "--rebuild-processing" ]]; then
    REBUILD_PROCESSING=true
fi

if [[ "$REBUILD_PROCESSING" == true || ! -f "$PROCESSING_EXE" ]]; then

    echo

    if [[ "$REBUILD_PROCESSING" == true ]]; then
        echo "Recompilando motor de processamento..."
    else
        echo "process_image não encontrado."
        echo "Gerando motor de processamento..."
    fi

    chmod +x "$PROCESSING_DIR/build_linux.sh"
    "$PROCESSING_DIR/build_linux.sh"
fi

if [[ ! -f "$PROCESSING_EXE" ]]; then
    echo "ERRO: processing/linux/process_image não foi gerado."
    exit 1
fi

chmod +x "$PROCESSING_EXE"

echo "[OK] Motor de processamento encontrado."

# ============================================================
# Abre o VS Code
# ============================================================

if command -v code >/dev/null 2>&1; then

    echo "[OK] Abrindo projeto no VS Code..."
    code "$ROOT" >/dev/null 2>&1 &

else

    echo
    echo "AVISO: comando 'code' não encontrado."
    echo "Abra manualmente no VS Code:"
    echo "$ROOT"

fi

# ============================================================
# Executa o aplicativo
# ============================================================

echo
echo "Iniciando L.U.M.A..."
echo
echo "Para encerrar pelo terminal: Ctrl+C"
echo

dotnet run --project "$PROJECT_FILE"