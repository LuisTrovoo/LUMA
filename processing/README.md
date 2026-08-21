# Análise de Imagens de Histologia

Aplicação para detecção de núcleos em imagens de histologia (deconvolução de
cor + Otsu + watershed + clustering K-means) com uma interface gráfica em
PySide6 para revisão/edição manual das associações célula grande ↔ células
pequenas.

Este projeto é o mesmo código do script original, apenas reorganizado em
módulos. Nenhuma lógica foi alterada — é uma reorganização 1:1.

## Estrutura de pastas

```
histologia_app/
├── main.py                       # Ponto de entrada (roda a aplicação PySide6)
├── requirements.txt               # Dependências Python
├── README.md
│
├── core/
│   └── pipeline.py                # pipeline_core(image_path) — todo o processamento
│                                   # de imagem (OpenCV/scipy/skimage/sklearn).
│                                   # Não depende de PySide6 — pode ser chamado
│                                   # isoladamente, inclusive via linha de comando.
│
├── workers/
│   └── processing_worker.py       # ProcessingWorker (QThread) — roda o pipeline
│                                   # em segundo plano e emite os sinais
│                                   # finished/error para a UI.
│
└── ui/
    ├── cell_viewer.py              # CellViewer (QGraphicsView) — visualização e
    │                                 edição interativa das células.
    ├── original_image_viewer.py    # OriginalImageViewer — janela de zoom da
    │                                 imagem original.
    └── main_window.py              # MainWindow — janela principal, layout e
                                       conexões de botões/atalhos.
```

## Como rodar

```bash
cd processing
python -m venv .venv
source .venv/bin/activate  # (Windows: .venv\Scripts\activate)
pip install -r requirements.txt
python main.py
```

## O que muda em relação ao script original

Nada na lógica. A única mudança é organizacional:

- `pipeline_core` foi movido para `core/pipeline.py` (sem nenhuma alteração
  no corpo da função).
- `ProcessingWorker` foi movido para `workers/processing_worker.py`.
- `CellViewer`, `OriginalImageViewer` e `MainWindow` foram movidos para
  `ui/`, cada um em seu próprio arquivo.
- `main.py` ficou só com o bloco `if __name__ == "__main__":`.
- Os `imports` foram ajustados para refletir os novos caminhos entre os
  módulos (ex.: `ui/main_window.py` agora importa
  `from workers.processing_worker import ProcessingWorker`), mas nenhuma
  variável, nome de classe/método ou passo do algoritmo foi renomeado ou
  alterado.

## Pensando na futura interface em C#

Como `core/pipeline.py` não depende de PySide6, ele é a peça mais fácil de
reaproveitar a partir de uma aplicação C#/.NET. Duas abordagens comuns:

1. **Processo externo (mais simples de integrar):** a interface em C# chama
   um script Python (via `Process.Start`, passando o caminho da imagem) que
   roda `pipeline_core` e grava os resultados em `results.mat`
   (ou, se preferir, em JSON) na pasta `<nome_da_imagem>_results/`. O C#
   apenas lê esse arquivo de resultado depois que o processo termina.
2. **Servidor local (mais interativo):** expor `pipeline_core` atrás de uma
   API HTTP simples (ex.: com FastAPI/Flask) rodando localmente, e o C#
   chama essa API via HTTP. Isso facilita reaproveitar a lógica de edição
   interativa (associar/excluir células) também no lado do backend, se um
   dia vocês quiserem tirar essa lógica da UI Qt.

Nenhuma dessas mudanças foi feita agora — é só para deixar claro por que a
separação `core/` (sem GUI) vs. `ui/` (PySide6) já ajuda nesse próximo passo.
