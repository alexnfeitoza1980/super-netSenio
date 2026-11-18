# Código do Livro 7 — Crenças em C#

Requer o .NET 8 SDK. Na pasta `codigo/`, execute:

```bash
dotnet test Crencas.sln
dotnet run --project Crencas.Console
```

- `Crencas.Core`: modelos, validação, perguntas e síntese das observações.
- `Crencas.Console`: cenário fictício de execução, sem entrada pessoal.
- `Crencas.Testes`: testes das combinações de evidências e dos limites dos modelos.

O exemplo mantém os dados somente em memória; não grava arquivos nem envia informações. Use dados fictícios e não trate os resultados como diagnóstico ou avaliação de pessoas.
