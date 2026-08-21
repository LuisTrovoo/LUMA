@echo off
setlocal
cd /d "%~dp0"

echo ==========================================
echo L.U.M.A. - Build do motor Windows
echo ==========================================

where py >nul 2>nul
if errorlevel 1 (
    echo ERRO: Python Launcher ^(py^) nao foi encontrado.
    exit /b 1
)

if not exist ".venv\Scripts\python.exe" (
    echo Criando ambiente virtual...
    py -3.11 -m venv .venv
    if errorlevel 1 exit /b 1
)

call .venv\Scripts\activate.bat
python -m pip install --upgrade pip
if errorlevel 1 exit /b 1
python -m pip install -r requirements.txt
if errorlevel 1 exit /b 1
python -m pip install pyinstaller
if errorlevel 1 exit /b 1

if exist "build" rmdir /s /q "build"
if exist "dist" rmdir /s /q "dist"

python -m PyInstaller --clean process_image.spec
if errorlevel 1 exit /b 1

if not exist "windows" mkdir "windows"
copy /Y "dist\process_image.exe" "windows\process_image.exe" >nul
if errorlevel 1 exit /b 1

echo.
echo OK: %CD%\windows\process_image.exe
exit /b 0
