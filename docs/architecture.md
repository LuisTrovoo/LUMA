# Arquitetura: como uma interface conversa com `processing/`

O `processing/core/pipeline.py` não depende de nenhuma biblioteca de
interface gráfica — recebe o caminho de uma imagem e devolve os resultados
(células detectadas, grupos, mapa de infecção, etc.), além de já salvar um
`results.mat` e algumas imagens intermediárias em
`<nome_da_imagem>_results/`. Isso foi feito de propósito para que qualquer
linguagem consiga reutilizar esse motor sem reescrevê-lo.

Duas formas de integrar, da mais simples para a mais robusta:

## Opção 1 — Processo externo (recomendada para começar)

A interface (C#, web, o que for) chama o Python como um processo separado,
passando o caminho da imagem, e espera ele terminar:

```csharp
var psi = new ProcessStartInfo {
    FileName = "python",
    Arguments = $"processing/main_cli.py \"{caminhoDaImagem}\"",
    RedirectStandardOutput = true,
    UseShellExecute = false
};
using var process = Process.Start(psi);
process.WaitForExit();
```

Depois, a interface só lê os arquivos gerados em
`<nome_da_imagem>_results/` (o `results.mat`, ou um `results.json` — ver
nota abaixo).

**Prós:** simples, não exige manter nada rodando em segundo plano.
**Contras:** cada chamada reprocessa do zero; não é interativo em tempo real.

> Nota: hoje o resultado é salvo em `.mat` (formato MATLAB/SciPy). Para
> facilitar a leitura pelo C#, pode valer a pena adicionar, no
> `pipeline_core`, uma exportação adicional em JSON com os campos que a
> interface realmente precisa (centróides, grupos, ids das células grandes).
> Isso é uma mudança pequena e isolada dentro de `processing/`.

## Opção 2 — API local (mais interativa)

Expor `pipeline_core` (e futuramente as operações de edição — associar,
excluir, desfazer — que hoje vivem em `CellViewer`) atrás de uma API HTTP
local simples, por exemplo com FastAPI:

```python
# processing/api.py (exemplo, ainda não implementado)
from fastapi import FastAPI
from core.pipeline import pipeline_core

app = FastAPI()

@app.post("/process")
def process(image_path: str):
    return pipeline_core(image_path)
```

A interface em C# chamaria essa API via HTTP
(`http://localhost:8000/process`) e receberia o resultado em JSON
diretamente, sem precisar ler arquivos do disco.

**Prós:** mais rápido para iterar, permite mover a lógica de edição
(associar/excluir células) para o backend, reaproveitável por qualquer
interface.
**Contras:** precisa manter o servidor local rodando.

## Recomendação

Comece pela **Opção 1** para validar rápido que a interface consegue disparar
o processamento e ler o resultado. Se o time sentir necessidade de mais
interatividade (edição em tempo real, várias imagens em sequência, etc.),
migrem para a Opção 2 — sem precisar mudar nada do algoritmo em si.
