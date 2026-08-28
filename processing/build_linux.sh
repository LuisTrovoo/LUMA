#!/usr/bin/env bash
set -euo pipefail
cd "$(dirname "$0")"

echo "=========================================="
echo "L.U.M.A. - Build do motor Linux"
echo "=========================================="

PYTHON_BIN="${PYTHON_BIN:-python3}"

if [ ! -x .venv/bin/python ]; then
  "$PYTHON_BIN" -m venv .venv
fi

source .venv/bin/activate
python -m pip install --upgrade pip
python -m pip install -r requirements.txt
python -m pip install pyinstaller

rm -rf build dist
python -m PyInstaller --clean process_image.spec

mkdir -p linux
cp dist/process_image linux/process_image
chmod +x linux/process_image

echo "OK: $(pwd)/linux/process_image"
