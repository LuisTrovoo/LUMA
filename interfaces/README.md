# interfaces/

Cada abordagem de interface testada pelo time vive em sua própria subpasta
aqui dentro. Isso permite testar mais de uma opção em paralelo (ex.: C# WPF,
C# MAUI, uma interface web) sem que uma atrapalhe a outra.

## Convenção de nomes

`interfaces/<stack>-<variante opcional>/`, por exemplo:

- `interfaces/csharp-wpf/`
- `interfaces/csharp-maui/`
- `interfaces/web-react/`

## Para começar uma interface nova

1. Copie a pasta `_template/` e renomeie para a sua interface:
   ```bash
   cp -r interfaces/_template interfaces/csharp-wpf
   ```
2. Edite o `README.md` de dentro da sua pasta explicando como rodar o
   projeto e como ele se comunica com `processing/` (veja
   `docs/architecture.md` para as opções recomendadas de integração).
3. Abra um Pull Request assim que tiver algo funcional, mesmo que
   incompleto — isso facilita o time acompanhar o progresso.

## Quando uma interface for descontinuada

Não precisa apagar na hora — pode marcar no `README.md` da pasta como
"descontinuada" e mover para `interfaces/_archive/` depois que o time
decidir qual interface seguir.
