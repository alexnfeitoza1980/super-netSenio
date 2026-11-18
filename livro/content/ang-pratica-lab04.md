---
id: ang-pratica-lab04
order: 299
kind: lab
num: 45
group: Angular · Parte 51 · Laboratórios
title: Laboratório 4 · Revisão e entrega
subtitle: Prepare uma entrega que outra pessoa possa executar, testar, revisar e publicar sem adivinhar passos.
minutes: 60
hue: 210
objectives:
- Executar validações limpas de build e testes
- Revisar acessibilidade, segurança e estados de erro
- Escrever uma descrição de mudança útil e respeitosa
origem: angular/livros/01-biblioteca-angular/content/24-lab04.md
---

## Missão final

Escolha uma melhoria pequena que faça sentido: ordenar por título, adicionar paginação, criar estado “continuar lendo” ou permitir uma categoria nova. Declare a necessidade, os critérios de aceite e os arquivos que espera alterar antes de começar.

::: passos Checklist de revisão
Rode `npm ci`, `npm run test:ci` e `npm run build`.
===
Teste teclado, zoom, foco, textos longos e tela estreita.
===
Verifique se nenhum token, dado sensível ou stack trace aparece na UI ou no bundle.
===
Escreva no README como executar e quais limites ainda existem.
===
Peça revisão com uma pergunta concreta e acolha feedback que venha com evidência.
:::

Uma boa entrega informa o que mudou, por que mudou, como foi verificado e o que ainda não faz. Não afirme que usa backend se os dados continuam no navegador. Honestidade técnica cria confiança.

::: quiz
? O que torna uma mudança pronta para revisão?
+ Contexto e critério explícitos, diff focado, validações executadas e limites conhecidos.
- Um grande conjunto de alterações sem descrição.
- Apenas uma captura de tela sem código ou forma de reproduzir.
:::

::: cartas
O que é um critério de aceite? :: Um comportamento observável que indica se a necessidade atendida funciona.
Como pedir feedback útil? :: Compartilhe contexto e peça revisão sobre um ponto específico, incluindo o comportamento ou decisão a avaliar.
:::
