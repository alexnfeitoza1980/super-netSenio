---
id: ang-pratica-lab01
order: 296
kind: lab
num: 42
group: Angular · Parte 51 · Laboratórios
title: Laboratório 1 · De catálogo vazio a estante
subtitle: Transforme a estrutura de um projeto standalone numa experiência de catálogo que funciona com teclado.
minutes: 60
hue: 210
objectives:
- Criar modelo e conjunto pequeno de livros com TypeScript
- Montar cards reutilizáveis com input e `@for`
- Verificar estados de lista, foco e tela estreita
origem: angular/livros/01-biblioteca-angular/content/21-lab01.md
---

## Missão

Implemente uma estante com ao menos quatro títulos originais, categorias variadas, autores e descrições. Use um card para cada item e organize o catálogo em uma grade responsiva.

::: passos Critérios de aceite
O build termina sem erros TypeScript ou de template.
===
Cada cartão tem título, autor, categoria e link de detalhe.
===
A grade se ajusta sem rolagem horizontal em viewport de 320 px.
===
Todas as ações podem ser alcançadas com teclado e mostram foco.
===
Estado sem resultados oferece orientação e um próximo passo.
:::

## Uma solução possível

Compare sua solução com `src/app/catalogo.ts`, `src/app/livro-card.ts` e `src/app/catalogo-page.ts`. O resultado do livro adiciona IDs estáveis, `input.required`, `@for` com `track`, semântica HTML e filtro que não depende de biblioteca visual externa.

Não precisa copiar exatamente os estilos. Preserve o contrato e teste com conteúdo comprido: títulos longos revelam problemas que uma tela com “Livro 1” não mostra.

::: quiz
? Qual é um critério melhor que “a tela ficou bonita” para verificar este laboratório?
+ Dados exibidos, rotas acessíveis, layout estreito e estado vazio foram observados e podem ser repetidos.
- Todos os cartões têm a mesma altura em qualquer conteúdo e dispositivo.
- Não há nenhum botão para testar.
:::

::: cartas
Por que começar com poucos livros? :: Um conjunto pequeno permite entender modelo, renderização e estados antes de introduzir infraestrutura.
O que testar em conteúdo longo? :: Quebras de linha, contraste, foco, dimensão do card e ausência de overflow horizontal.
:::
