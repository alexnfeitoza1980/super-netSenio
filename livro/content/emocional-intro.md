---
id: emocional-intro
order: 150
kind: intro
group: C# & Mentalidade · Parte 17 · Emoções e C#
title: Inteligência Emocional em C# — apresentação
subtitle: Perceber sinais, levar o contexto em conta e escolher respostas possíveis — enquanto aprendemos C#.
minutes: 12
hue: 170
badge: ★
objectives:
- Conhecer a proposta educativa e os limites deste livro
- Distinguir sinal percebido, interpretação e ação escolhida
- Ver o mapa dos oito capítulos e do Laboratório 13
- Saber como executar os exemplos sem registrar informações pessoais
origem: livros/09-inteligencia-emocional-csharp/content/00-intro.md
---

::: ilustracao img/emocional/intro.svg | Perceber, compreender, escolher: entre sentir e agir há espaço para uma pausa e para mais de uma resposta.
:::

## Sobre este livro

Este é o **Livro 9** da coleção *Aprenda C#*. Ele se inspira em temas amplos de *Inteligência Emocional*, de **Daniel Goleman**, e os transforma em uma jornada original ligada ao estudo e ao trabalho em tecnologia. A obra consultada apresenta a inteligência emocional em diferentes contextos; este volume não resume seus capítulos nem substitui sua leitura.

Vamos explorar uma ideia prática: notar uma emoção não obriga alguém a agir de determinada forma. Um sinal pode convidar a uma pausa, a uma pergunta, a um limite ou a uma conversa. O contexto importa; as opções disponíveis também.

::: cuidado Este livro não avalia nem trata pessoas
As atividades são **educativas e opcionais**, não terapia, diagnóstico ou aconselhamento de saúde. Emoções não são defeitos, e C# não consegue inferir com segurança o que uma pessoa sente, medir inteligência emocional ou prever comportamento. Use exemplos fictícios ou neutros; não registre experiências privadas. Se uma situação trouxer sofrimento intenso ou persistente, procure um profissional qualificado ou alguém de confiança.
:::

## A jornada: perceber, compreender, escolher

| Etapa | Pergunta | Ideia de C# |
|---|---|---|
| Perceber | Que sinal notei, sem ainda concluir o motivo? | `record` com observações declaradas |
| Nomear | Que palavra descreve melhor a experiência para mim? | `enum` como vocabulário opcional |
| Contextualizar | O que mais acontecia naquele momento? | campos explícitos e dados relacionados |
| Escolher | Qual pausa, limite ou apoio é possível agora? | validação e opções |
| Conversar | Como perguntar e explicar sem adivinhar ou acusar? | mensagens e roteiros reutilizáveis |

::: figura Uma resposta tem espaço para escolha
<svg viewBox="0 0 720 160" role="img" aria-label="Um sinal percebido leva a uma pausa e à consideração do contexto; depois, a pessoa escolhe uma resposta, uma conversa ou um limite">
  <rect x="10" y="46" width="145" height="62" rx="12" class="f-info"/><text x="82" y="72" text-anchor="middle" font-weight="800">Perceber</text><text x="82" y="92" text-anchor="middle" font-size="12" class="t-dim">um sinal</text>
  <path class="arrow" d="M157 77H190"/>
  <rect x="194" y="46" width="145" height="62" rx="12" class="f-warn"/><text x="266" y="72" text-anchor="middle" font-weight="800">Pausar</text><text x="266" y="92" text-anchor="middle" font-size="12" class="t-dim">se for possível</text>
  <path class="arrow" d="M341 77H374"/>
  <rect x="378" y="46" width="145" height="62" rx="12" class="f-good"/><text x="450" y="72" text-anchor="middle" font-weight="800">Contextualizar</text><text x="450" y="92" text-anchor="middle" font-size="12" class="t-dim">perguntar, não presumir</text>
  <path class="arrow" d="M525 77H558"/>
  <rect x="562" y="46" width="148" height="62" rx="12" class="f-prim"/><text x="636" y="72" text-anchor="middle" font-weight="800">Escolher</text><text x="636" y="92" text-anchor="middle" font-size="12" class="t-dim">agir, conversar ou parar</text>
</svg>
:::

## O mapa

| Capítulo | Tema | C# |
|---|---|---|
| 116 | Emoções trazem informação, não ordens | `enum` e `record` |
| 117 | Dar nome sem virar rótulo | tipos distintos e validação |
| 118 | Criar espaço antes de responder | estado e opções de pausa |
| 119 | Atenção, tarefa e contexto | `DateTimeOffset` e planejamento |
| 120 | Empatia sem adivinhar | perguntas reutilizáveis |
| 121 | Conversas difíceis com respeito | mensagens explícitas e validação |
| 122 | Relações e acordos de equipe | composição de dados |
| 123 | Praticar, rever e pedir apoio | revisão sem pontuar pessoas |
| Lab 13 | Roteiro de Pausa e Conversa | integrar os modelos e testar escolhas |

Cada capítulo propõe uma cena de trabalho ou estudo, uma ponte para C#, um exemplo, um exercício com resposta, quizzes com justificativas, resumo e cartões. O laboratório final junta conceitos em uma demonstração de console com dados fictícios, sem salvar arquivos.

## O que você precisa saber e como rodar

Você deve conhecer variáveis, métodos, classes e testes básicos do Livro 1. Os projetos usam .NET 8 e C# 12; LINQ e `record` são recapitulados quando aparecem.

```bash
dotnet test codigo/emocional/Emocoes.sln
dotnet run --project codigo/emocional/Emocoes.Console
```

O programa mostra um roteiro educativo já preenchido com uma situação fictícia. Não pede relatos pessoais, não grava e não envia dados.

Neste volume, o primeiro comando executa uma demonstração fixa e fictícia. Os tipos da biblioteca também podem ser usados separadamente nos exercícios. Para acompanhar o código, abra `codigo/emocional/Emocoes.sln` e observe os três projetos: biblioteca, console e testes. Os capítulos apontam os arquivos e explicam as decisões antes de integrá-las no laboratório.

## Um jeito de estudar cada capítulo

1. Leia a cena e sublinhe o que é fato, hipótese e escolha.
2. Leia a explicação sem tentar descobrir o que outra pessoa “realmente sente”.
3. Percorra o código linha a linha e identifique qual contrato técnico ele protege.
4. Antes de olhar a resposta do exercício, escreva uma versão com seus próprios dados fictícios.
5. Responda aos quizzes e leia a justificativa, inclusive se acertar.
6. Rode os testes da solução no laboratório para observar o que o código garante — e o que permanece fora dele.

Pular uma pergunta ou não registrar uma reflexão também é válido. As atividades foram escritas para ensinar pensamento cuidadoso e conceitos de C#, não para medir a vida emocional do leitor.

::: pense Antes de começar
Quando surge uma emoção durante uma tarefa, que diferença há entre perceber o sinal e decidir imediatamente o que ele significa?
:::

::: cartas
Uma emoção é uma ordem? :: Não. É uma experiência que pode trazer informação; a resposta ainda depende de contexto e escolha.
O código identifica sentimentos? :: Não. Ele organiza apenas opções e textos que alguém decidiu informar.
É necessário preencher todos os exercícios? :: Não. As atividades são opcionais; usar exemplos neutros ou pular uma pergunta é válido.
Inteligência emocional é nunca se irritar? :: Não. Este livro não exige suprimir emoções nem promete controle perfeito.
:::
