# L.U.M.A. — Analisador de Amastigotas

Repositório oficial do projeto L.U.M.A., reunindo o motor de processamento de imagens em Python, a interface desktop em Avalonia/.NET e a documentação necessária para desenvolvimento, validação e distribuição.

> Estado atual: desenvolvimento (`0.1.0-dev`). A base funcional foi preservada; correções de interface devem ser feitas e testadas antes da primeira release `1.0.0`.

## Estrutura

```text
.
├── processing/                  # motor de processamento (Python)
│   ├── core/                    # algoritmo principal
│   ├── workers/
│   ├── ui/                      # interface Python de referencia
│   ├── process_image.py         # entrada usada pela interface Avalonia
│   ├── process_image.spec       # empacotamento PyInstaller
│   ├── build_windows.bat
│   └── build_linux.sh
├── interfaces/
│   └── AnalisadorAmastigotas/   # interface Avalonia/.NET
├── scripts/                     # builds reproduziveis do aplicativo completo
├── docs/                        # arquitetura, desenvolvimento e releases
├── references/                  # bibliografia e notas cientificas
├── .github/workflows/           # validacao Windows/Linux e release futura
├── VERSION
└── README.md
```

## Fonte da verdade

- `main`: versão oficial do código.
- `processing/core/pipeline.py`: algoritmo de processamento.
- `interfaces/AnalisadorAmastigotas/`: interface desktop oficial atual.
- `artifacts/`: somente arquivos gerados localmente; não entra no Git.

## Desenvolvimento no Windows

```powershell
cd processing
.\build_windows.bat
cd ..\interfaces\AnalisadorAmastigotas
dotnet run
```

## Desenvolvimento no Linux

```bash
./processing/build_linux.sh
cd interfaces/AnalisadorAmastigotas
dotnet run
```

## Build portátil

Windows:

```powershell
powershell -ExecutionPolicy Bypass -File .\scripts\build-windows.ps1
```

Linux:

```bash
./scripts/build-linux.sh
```

Veja `docs/DEVELOPMENT.md` e `docs/RELEASES.md` antes de publicar versões.

## Referências científicas

A bibliografia é mantida em `references/`. Em um repositório público, preferimos armazenar DOI/links e notas, e não PDFs protegidos por copyright.
