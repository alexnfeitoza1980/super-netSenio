---
id: ang-pratica-lab02
order: 297
kind: lab
num: 43
group: Angular · Parte 51 · Laboratórios
title: Laboratório 2 · Busca, filtros e favoritos
subtitle: Conecte sinais, resultados derivados e persistência com comportamento explícito.
minutes: 60
hue: 210
objectives:
- Criar estado de busca e categoria em signals
- Derivar resultados com `computed`
- Alternar e recuperar favoritos sem confiar cegamente no JSON salvo
origem: angular/livros/01-biblioteca-angular/content/22-lab02.md
---

## Missão

Adicione busca por título, autora e descrição; filtro por categoria; botão de favorito; e persistência local versionada. Combine busca e categoria. Quando não houver resultados, explique como recuperar. Não guarde dados pessoais.

::: ilustracao img/ang-pratica/fluxo-interacao.svg | O estado reativo flui de evento a derivação e de volta à interface.
:::

## Critérios e investigação

- Busca não diferencia maiúscula/minúscula e, de preferência, acentos.
- Mudar categoria não apaga o texto da busca.
- Favorito pode ser adicionado e removido; estado tem nome acessível.
- Estado inválido não derruba o catálogo e não aparece como salvo.
- Há testes para busca, filtro, toggle e dados inválidos.

Compare `biblioteca.store.ts` e `biblioteca.store.spec.ts`. Observe `computed`, atualização imutável, versão da chave e o aviso explícito quando leitura ou gravação falha.

::: pratica Feedback que ajuda
Peça a alguém para fazer uma busca que não encontra resultados. Em vez de explicar antes, observe a tela e pergunte o que esperava acontecer. Anote o comportamento percebido; não transforme a resposta em avaliação da pessoa.
:::

::: quiz
? Ao filtrar a lista, qual é a melhor fonte de `resultados`?
+ Um `computed` derivado da busca, categoria e catálogo atual.
- Um array mutável atualizado em vários eventos sem relação.
- O HTML concatenado a partir do texto digitado.
:::

::: cartas
Qual estado precisa de persistência neste laboratório? :: Favoritos; no projeto final também o progresso de leitura.
Como tratar uma gravação inválida? :: Validar, apresentar aviso útil, manter o catálogo utilizável e evitar declarar sucesso falso.
:::
