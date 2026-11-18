---
id: crencas-intro
order: 125
kind: intro
group: C# & Mentalidade · Parte 16 · Crenças e C#
title: Crenças em C# — apresentação
subtitle: Investigar uma crença com curiosidade, contexto e evidências — e aprender C# enquanto fazemos isso.
minutes: 12
hue: 280
badge: ★
objectives:
- Entender a proposta do livro e seus limites
- Distinguir uma **hipótese sobre uma situação** de uma identidade ou diagnóstico
- Ver o mapa dos oito capítulos e do Laboratório 11
- Saber como executar os exemplos e preservar a própria privacidade
origem: livros/07-crencas-csharp/content/00-intro.md
---

## Sobre este livro

Este é o **Livro 7** da coleção *Aprenda C#*. Ele adapta, com autorização, temas da obra *Como Identificar e Eliminar Crenças Limitantes*. A autoria da edição consultada não está identificada. As cenas, explicações, exercícios e exemplos de código deste volume foram escritos para esta coleção.

Vamos explorar uma ideia útil para aprender e trabalhar: às vezes uma frase que parece fato é, na verdade, uma **interpretação**. Em vez de tratá-la como verdade absoluta ou tentar apagá-la com uma frase positiva, podemos perguntar de onde veio, que evidências existem, que contexto falta e qual experimento pequeno seria seguro.

::: cuidado Este livro não é terapia
Uma crença difícil não é falha de caráter, e um programa não consegue avaliar sua personalidade, saúde mental ou potencial. As atividades são **educativas e opcionais**: use exemplos leves, como aprender uma ferramenta ou organizar uma tarefa, e não escreva nada que prefira manter privado. Não há escala clínica, diagnóstico ou promessa de que pensar diferente resolverá problemas externos. Se o tema trouxer sofrimento intenso ou persistente, procure um profissional qualificado ou alguém de confiança.
:::

## Uma lente, não uma explicação total

Ideias sobre crenças podem considerar influências familiares, sociais e pessoais. Neste percurso, fazemos uma distinção importante: crenças podem influenciar atenção, escolhas e comportamento, mas **não explicam tudo o que acontece**. Renda, saúde, discriminação, tempo, acesso, segurança e decisões de outras pessoas também importam. Uma dificuldade não prova que alguém “pensou errado” ou deixou de se esforçar.

| Para investigar uma ideia | Para aprender C# |
|---|---|
| Uma frase que parece verdade | `record` com dados explícitos |
| De onde a frase pode ter vindo | `enum` de origem |
| O que foi observado e o que foi interpretado | tipos separados |
| Evidências diferentes e contexto | coleções e LINQ |
| Próximo passo pequeno e verificável | plano validado e testes |

::: figura O percurso: da frase à pergunta
<svg viewBox="0 0 720 160" role="img" aria-label="Uma frase é separada em fato e interpretação, investigada com evidências e contexto, e pode levar a um experimento pequeno ou a uma pausa">
  <rect x="10" y="46" width="145" height="62" rx="12" class="f-warn"/><text x="82" y="72" text-anchor="middle" font-weight="800">Frase</text><text x="82" y="92" text-anchor="middle" font-size="12" class="t-dim">“não consigo”</text>
  <path class="arrow" d="M157 77H190"/>
  <rect x="194" y="46" width="145" height="62" rx="12" class="f-info"/><text x="266" y="72" text-anchor="middle" font-weight="800">Separar</text><text x="266" y="92" text-anchor="middle" font-size="12" class="t-dim">fato ≠ interpretação</text>
  <path class="arrow" d="M341 77H374"/>
  <rect x="378" y="46" width="145" height="62" rx="12" class="f-good"/><text x="450" y="72" text-anchor="middle" font-weight="800">Investigar</text><text x="450" y="92" text-anchor="middle" font-size="12" class="t-dim">evidência + contexto</text>
  <path class="arrow" d="M525 77H558"/>
  <rect x="562" y="46" width="148" height="62" rx="12" class="f-prim"/><text x="636" y="72" text-anchor="middle" font-weight="800">Escolher</text><text x="636" y="92" text-anchor="middle" font-size="12" class="t-dim">agir, pausar ou pedir apoio</text>
</svg>
:::

## O mapa

| Capítulo | Tema | C# |
|---|---|---|
| 97 | De onde vêm as crenças | `enum`, `record` |
| 98 | Fato, interpretação e rótulo | tipos explícitos e validação |
| 99 | Evidências e vieses de confirmação | coleções e LINQ |
| 100 | Circunstâncias, limites e escolhas possíveis | composição de dados |
| 101 | Como uma hipótese pode influenciar uma ação | modelos e testes de comportamento |
| 102 | Perguntas que ajudam a investigar | `IReadOnlyList` e funções |
| 103 | Experimentos pequenos e seguros | validação e resultados observáveis |
| 104 | Revisar sem exigir certeza | `switch`, estados e síntese |
| Lab 11 | Caderno de Hipóteses | integrar os modelos e testar cenários |

Cada capítulo oferece uma cena, uma ponte para C#, exemplos, exercício com resposta, quiz e cartões. Os capítulos são cumulativos; o laboratório final junta os conceitos sem guardar relatos pessoais em arquivo.

## O que você precisa saber

Você deve conhecer variáveis, métodos, classes e testes básicos do Livro 1. Os exemplos usam .NET 8 e C# 12, como o projeto do Livro 6. LINQ e `record` aparecem com explicações e pequenos exemplos.

```bash
dotnet test codigo/crencas/Crencas.sln
dotnet run --project codigo/crencas/Crencas.Console
```

::: pense Antes de começar
Pense em uma habilidade técnica que você já aprendeu aos poucos. Que evidência mostra que o aprendizado aconteceu por etapas, e não de uma vez?
:::

::: cartas
Uma crença é uma identidade? :: Não. Neste livro ela é tratada como uma ideia sobre uma situação, que pode ser investigada sem rotular a pessoa.
Uma crença explica todo resultado? :: Não. Contexto, recursos, oportunidades e decisões de outras pessoas também influenciam.
O objetivo é trocar cada pensamento negativo por um positivo? :: Não. O objetivo é construir uma visão mais específica, justa e verificável.
Um experimento precisa expor algo pessoal? :: Não. Os exemplos usam situações técnicas leves e podem ser feitos sem registrar dados privados.
:::
