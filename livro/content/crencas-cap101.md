---
id: crencas-cap101
order: 130
kind: capitulo
num: 101
group: C# & Mentalidade · Parte 16 · Crenças e C#
title: Como uma hipótese pode influenciar uma ação
subtitle: Uma expectativa pode mudar o que tentamos e o que percebemos. Isso é uma possibilidade de ciclo, não uma lei que explica todo resultado.
minutes: 50
hue: 328
objectives:
- Descrever um ciclo possível entre expectativa, ação, resultado e interpretação
- Distinguir influência de causalidade garantida
- Representar uma sequência observável sem atribuir um resultado à crença como única causa
origem: livros/07-crencas-csharp/content/139-cap101.md
---

::: vila O tutorial pulado
Depois de dois erros num exercício, Caio conclui que “LINQ não é para mim”. Na semana seguinte, pula os exemplos de LINQ. Como praticou menos, tem menos oportunidades de notar progresso. O ciclo é plausível, mas não é a única explicação: talvez o tutorial estivesse confuso, a tarefa fosse longa ou o tempo de estudo fosse insuficiente. Investigar o ciclo abre perguntas; não encerra o caso.
:::

## Um ciclo que pode ser interrompido

Uma expectativa **pode** influenciar o que tentamos, e o que tentamos pode influenciar as observações disponíveis depois. Por exemplo:

1. expectativa: “vou errar”;
2. ação possível: evitar a tentativa;
3. observação: houve menos prática;
4. interpretação: “isso prova que não consigo”.

Essa sequência não acontece sempre e não prova que expectativas causem qualquer resultado. Condições externas e a qualidade do ensino também importam. O valor do modelo é tornar a sequência **perguntável**: “qual ação ocorreu?”, “que outras causas havia?” e “que tipo de apoio mudaria as condições?”.

::: figura Influência possível, não destino
<svg viewBox="0 0 700 190" role="img" aria-label="Uma expectativa pode influenciar uma ação; a ação produz observações, que são interpretadas com contexto; uma pergunta pode abrir outro experimento">
  <rect x="10" y="62" width="145" height="58" rx="12" class="f-warn"/><text x="82" y="86" text-anchor="middle" font-weight="800">Expectativa</text><text x="82" y="105" text-anchor="middle" font-size="12" class="t-dim">hipótese</text>
  <path class="arrow" d="M157 91H187"/>
  <rect x="191" y="62" width="145" height="58" rx="12" class="f-info"/><text x="263" y="86" text-anchor="middle" font-weight="800">Ação</text><text x="263" y="105" text-anchor="middle" font-size="12" class="t-dim">ou não ação</text>
  <path class="arrow" d="M338 91H368"/>
  <rect x="372" y="62" width="145" height="58" rx="12" class="f-good"/><text x="444" y="86" text-anchor="middle" font-weight="800">Observação</text><text x="444" y="105" text-anchor="middle" font-size="12" class="t-dim">com contexto</text>
  <path class="arrow" d="M519 91H549"/>
  <rect x="553" y="62" width="137" height="58" rx="12" class="f-prim"/><text x="621" y="86" text-anchor="middle" font-weight="800">Revisão</text><text x="621" y="105" text-anchor="middle" font-size="12" class="t-dim">ou nova pergunta</text>
  <text x="350" y="159" text-anchor="middle" font-size="13" class="t-dim">O diagrama descreve um ciclo possível; não afirma causa única nem inevitabilidade.</text>
</svg>
:::

## Sequências de eventos em C#

Um registro simples pode descrever uma observação temporal. O campo `Contexto` ajuda a lembrar que o resultado não está isolado:

```cs Jornada/Crencas/EventoDeAprendizagem.cs
namespace Jornada.Crencas;

public sealed record EventoDeAprendizagem(
    string Expectativa,
    string Acao,
    string Observacao,
    string Contexto);
```

Não use esse tipo para classificar colegas. Seu propósito é organizar **um exemplo de aprendizagem fictício** e perguntar o que mais poderia explicar o resultado.

::: exercicio Identifique uma explicação alternativa
Uma pessoa acredita que não consegue apresentar e evita a reunião. Depois diz que “não teve oportunidade de praticar”. Cite uma relação possível entre expectativa e ação, e duas explicações contextuais alternativas.
---
**Resposta possível:** a expectativa pode ter contribuído para evitar a reunião. Alternativas: a reunião mudou de horário e a pessoa não recebeu aviso; não havia tempo de preparação ou acessibilidade adequada. A sequência não prova uma causa única.
:::

::: quiz
? O que o modelo de ciclo permite afirmar?
+ Que uma expectativa pode influenciar uma ação em certo contexto.
- Que crenças determinam todos os resultados.
- Que resultados difíceis demonstram uma crença limitante.
> “Pode influenciar” é uma hipótese contextual, não uma lei universal.

? Para que registrar `Contexto`?
+ Para lembrar que condições adicionais podem ajudar a explicar a observação.
- Para provar que a hipótese original está correta.
- Para atribuir nota ao comportamento de alguém.
> Um campo de contexto abre investigação; não dá certeza automática.

? Se uma pessoa evita uma tarefa, qual inferência é segura?
+ Sabemos que a tarefa foi evitada; o motivo precisa ser perguntado ou investigado.
- Sabemos que ela tem medo de fracassar.
- Sabemos que ela não deseja aprender.
> O motivo não pode ser deduzido de um único comportamento.
:::

::: resumo
- Expectativas podem influenciar ações; ações afetam as oportunidades de observar progresso.
- O ciclo é uma hipótese, não uma explicação universal ou uma acusação.
- Um evento deve registrar ação, observação e contexto.
- Procure causas alternativas e pergunte em vez de inferir motivos.
:::

::: cartas
Ciclo de feedback :: Sequência possível entre expectativa, ação, resultado e interpretação.
Influência :: Relação que pode contribuir para um resultado, sem ser necessariamente a única causa.
Variável contextual :: Condição que pode alterar como uma ação ou resultado ocorre.
:::
