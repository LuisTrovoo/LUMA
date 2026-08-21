# Releases

## Estado atual

Enquanto o fluxo completo da interface ainda estiver sendo validado, use versoes `0.x`.

- `0.1.0-dev`: desenvolvimento inicial organizado
- `1.0.0`: primeira versao considerada pronta para usuario final

## Versionamento

Usaremos versionamento semantico:

- PATCH: `1.0.0 -> 1.0.1` para correcoes
- MINOR: `1.0.0 -> 1.1.0` para funcionalidades compativeis
- MAJOR: `1.x -> 2.0.0` para mudancas grandes/incompativeis

## Importante

Criar uma tag nao altera o codigo. A tag apenas marca um commit testado como uma versao publicada.

Exemplo futuro:

```bash
git tag -a v1.0.0 -m "L.U.M.A. v1.0.0"
git push origin v1.0.0
```

Nao publique `v1.0.0` ate Windows e Linux passarem pelo fluxo completo de teste.
