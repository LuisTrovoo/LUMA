# Como contribuir

Este guia descreve o fluxo de trabalho para o time (processamento +
múltiplas interfaces sendo testadas em paralelo).

## 1. Acesso ao repositório

- Peça para ser adicionado como **colaborador** (Settings → Collaborators
  and teams, no GitHub) ou, se o time crescer, migrem para uma
  **Organização** do GitHub e criem times (`@time-processing`,
  `@time-interfaces`, etc.) com permissões por pasta via CODEOWNERS.
- Todo mundo clona o **mesmo repositório** (não é necessário fork, já que é
  um time fechado trabalhando junto):

```bash
git clone https://github.com/<sua-org-ou-usuario>/<nome-do-repo>.git
cd <nome-do-repo>
```

## 2. Branches

- `main` é protegida: ninguém dá push direto nela, só via Pull Request
  (configurar em Settings → Branches → Branch protection rules → Require a
  pull request before merging).
- Crie uma branch por tarefa/pessoa/interface, com prefixo indicando o tipo:

| Prefixo       | Quando usar                                      | Exemplo                        |
|---------------|---------------------------------------------------|---------------------------------|
| `feature/`    | Nova funcionalidade                                | `feature/csharp-wpf-upload`     |
| `fix/`        | Correção de bug                                    | `fix/pipeline-otsu-threshold`   |
| `docs/`       | Documentação                                       | `docs/architecture-diagram`     |
| `experiment/` | Protótipo/teste que pode não virar produto final   | `experiment/interface-web`      |

```bash
git checkout -b feature/csharp-wpf-upload
```

## 3. Trabalhando no `processing/`

- Qualquer mudança em `processing/` afeta todo mundo — abra um PR e peça
  revisão antes de mesclar, especialmente para mudanças no `core/pipeline.py`.
- Não renomeie funções/parâmetros usados pela integração (veja
  `docs/architecture.md`) sem avisar o time, pois isso quebra as interfaces
  que já estiverem consumindo o motor.

## 4. Trabalhando em uma interface (`interfaces/<nome>/`)

- Copie `interfaces/_template/` para começar.
- Fique livre para organizar sua pasta como quiser internamente (é o seu
  projeto C#/.NET, web, etc.) — só não mexa em `processing/` a não ser que
  seja necessário, e nesse caso avise/discuta com o time antes.
- Cada interface tem seu próprio `.gitignore` específico se precisar (ex.:
  `bin/`, `obj/`, `.vs/` para projetos .NET) — já deixamos um `.gitignore`
  raiz cobrindo os casos mais comuns.

## 5. Commits

- Mensagens curtas e no imperativo: `Adiciona validação de threshold no Otsu`,
  não `Adicionado`/`Adicionando`.
- Prefira commits pequenos e frequentes a um commit gigante no final.

## 6. Pull Requests

1. Suba sua branch: `git push origin feature/csharp-wpf-upload`.
2. Abra o PR no GitHub apontando para `main`.
3. Descreva o que mudou e, se for uma interface nova, inclua prints/gif de
   como está funcionando.
4. Peça revisão de pelo menos 1 pessoa do time antes de mesclar.
5. Depois de aprovado, use "Squash and merge" para manter o histórico da
   `main` limpo (configurável em Settings → General → Pull Requests).

## 7. Issues e organização das tarefas

- Use a aba **Issues** para tarefas e bugs, e o **Projects** (quadro Kanban)
  do GitHub para acompanhar quem está testando qual interface. Sugestão de
  colunas: `Backlog` → `Em andamento` → `Em revisão (PR aberto)` → `Concluído`.
- Marque cada issue com uma label indicando a área: `processing`,
  `interface:csharp-wpf`, `docs`, etc.
