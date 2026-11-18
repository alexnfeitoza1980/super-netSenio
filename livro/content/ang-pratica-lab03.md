---
id: ang-pratica-lab03
order: 298
kind: lab
num: 44
group: Angular · Parte 51 · Laboratórios
title: Laboratório 3 · Rotas, progresso e testes
subtitle: Leve a estante a uma experiência navegável e proteja as regras com testes.
minutes: 70
hue: 210
objectives:
- Criar rotas para catálogo, detalhes, favoritos e caminho desconhecido
- Guardar e ajustar progresso sem aceitar valores fora da faixa
- Testar store, navegação e interação essencial
origem: angular/livros/01-biblioteca-angular/content/23-lab03.md
---

## Missão

Crie URLs compartilháveis para catálogo, detalhe e favoritos. A rota de detalhe deve tratar IDs inexistentes. Permita alterar progresso e atualizar a interface sem recarregar. Proteja filtros e estado de forma apropriada.

::: passos O que validar
Abra diretamente uma rota de livro válido e depois um ID desconhecido.
===
Marque e desmarque favorito; atualize; confira persistência esperada.
===
Use o slider por teclado e confira rótulo, valor e barra de progresso.
===
Rode testes de unidade e um fluxo de navegação no navegador.
:::

## Conversa de revisão

Descreva uma regra em forma de teste: “um ID desconhecido não cria um favorito invisível”. Peça a outra pessoa que encontre uma borda do fluxo. Responda com curiosidade; um questionamento é uma oportunidade de descobrir um requisito, não uma prova de incapacidade.

Compare a solução com `app.routes.ts`, `detalhe-page.ts` e `biblioteca.store.spec.ts`.

::: quiz
? Onde deve ser validado o intervalo de 0 a 100 do progresso?
+ Na regra da store e também no controle da interface, sem confiar apenas no slider.
- Só no CSS.
- Apenas na URL.
:::

::: cartas
Por que a rota coringa vem ao final? :: Ela deve capturar caminhos que não foram atendidos pelas rotas mais específicas.
Por que validar progresso também na store? :: A store pode ser chamada por qualquer componente ou evento, não apenas pelo slider.
:::
