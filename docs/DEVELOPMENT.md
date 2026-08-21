# Desenvolvimento

## Regra principal

A branch `main` representa sempre a base oficial do projeto. Alteracoes devem ser feitas em uma branch curta, testadas e depois mescladas na `main`.

Exemplos de branches:

- `fix/resultado-window`
- `feature/exportacao-pdf`
- `docs/referencias`

Nao mantenha duas copias diferentes do codigo como se fossem versoes oficiais.

## Windows

Requisitos para desenvolvimento:

- Windows 10/11 x64
- Python 3.11
- .NET 10 SDK
- Git/GitHub Desktop

Para executar em modo de desenvolvimento:

1. Gere o motor Python: `processing\\build_windows.bat`
2. Entre em `interfaces\\AnalisadorAmastigotas`
3. Execute `dotnet run`

Para gerar uma versao portatil completa:

`powershell -ExecutionPolicy Bypass -File .\\scripts\\build-windows.ps1`

A saida fica em `artifacts/` e nao e versionada no Git.

## Linux

Requisitos para desenvolvimento:

- Python 3.11 ou compativel
- python3-venv
- .NET 10 SDK
- dependencias nativas do Avalonia/X11

Build completo:

`./scripts/build-linux.sh`

## Codigo-fonte vs artefatos

O GitHub guarda codigo-fonte e scripts de reproducao. Binarios gerados (`process_image.exe`, `process_image`, `bin/`, `obj/`, `artifacts/`) nao sao fonte e ficam fora do versionamento.
