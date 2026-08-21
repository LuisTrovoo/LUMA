#!/usr/bin/env bash
set -euo pipefail
ROOT="$(cd "$(dirname "$0")/.." && pwd)"
PROJECT="$ROOT/interfaces/AnalisadorAmastigotas/AnalisadorAmastigotas.csproj"
ARTIFACTS="$ROOT/artifacts/linux"
APP_OUT="$ARTIFACTS/app"

echo "=== L.U.M.A. | Linux x64 ==="

"$ROOT/processing/build_linux.sh"
rm -rf "$ARTIFACTS"
mkdir -p "$APP_OUT"

dotnet restore "$PROJECT"
dotnet publish "$PROJECT" -c Release -r linux-x64 --self-contained true -o "$APP_OUT"
chmod +x "$APP_OUT/AnalisadorAmastigotas" || true
chmod +x "$APP_OUT/processing/linux/process_image" || true

tar -C "$APP_OUT" -czf "$ROOT/artifacts/LUMA-Linux-x64.tar.gz" .

echo "Build concluido."
echo "Aplicativo: $APP_OUT"
echo "Portatil:   $ROOT/artifacts/LUMA-Linux-x64.tar.gz"
