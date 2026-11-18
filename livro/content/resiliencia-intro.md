---
id: resiliencia-intro
order: 113
kind: intro
group: C# & Mentalidade · Parte 15 · Resiliência em C#
title: Resiliência em C# — apresentação
subtitle: Resiliência como lente de cuidado, e C# para transformar cada ideia em um modelo que você consegue rodar, testar e conversar sobre.
minutes: 12
hue: 262
badge: ★
objectives:
- Entender como o tema **resiliência** é usado aqui, e o que **fica de fora**
- Ver o **mapa** dos 9 capítulos e do laboratório, com o **recurso de C#** de cada um
- Saber o que já precisa conhecer e como **rodar o código** do livro
origem: livros/06-resiliencia-csharp/content/00-intro.md
---

::: ilustracao img/resiliencia/intro.svg | De um lado, só ler; do outro, pensar, programar e testar uma ideia. Este livro é o segundo caminho.
:::

## Sobre este livro

Este é o **Livro 6** da coleção *Aprenda C#*. Ele parte de um pequeno texto sobre **resiliência**, *Resiliência: Superando suas dificuldades*, de **Kaique Mathias da Silva** e **Wendel Juan Oliveira Reolon**, que conversa sobre o significado da palavra e reúne temas como emoções, controle de impulsos, otimismo, análise do ambiente, empatia, autoeficácia e apoio de outras pessoas.

Aqui, cada tema vira **uma pergunta de vida** e **um modelo em C#** que você escreve, roda e testa. O objetivo duplo é:

1. **Pensar com mais calma** sobre como atravessamos dificuldades, sem receita pronta e sem rótulos.
2. **Aprender C# de verdade**, passo a passo: de `enum` e `try/catch` até `async/await`, retentativas e disjuntor de circuito.

As cenas, as explicações, os exercícios e todo o código são **originais**. O livro é **inspirado** nos temas da obra: não a substitui, e nada aqui é terapia, diagnóstico ou avaliação de pessoas.

::: ilustracao img/resiliencia/photos/referencia-placa-resiliencia.png | A palavra no caminho: resiliência é seguir, e seguir pode incluir parar, pedir ajuda e mudar de rota.
:::

::: cuidado O que este livro não faz
Programas **não medem** quem você é. Nenhum código deste livro dá nota, compara pessoas, prevê comportamento ou "detecta" emoções em textos. Quando um modelo organiza ideias (um diário, um mapa de apoio), ele é uma **ferramenta de reflexão voluntária**, e tudo fica **só na memória**, sem gravar nem enviar nada. Em sofrimento intenso ou persistente, converse com um profissional de saúde. No Brasil, o **CVV atende pelo 188** (24 h, gratuito) e o **SAMU, pelo 192**, em emergências.
:::

## A ideia central: duas resiliências que se espelham

Resiliência é a palavra que usamos para **pessoas** e também para **sistemas de software**. Não é coincidência, e vamos explorar o espelho com cuidado (**sem** dizer que pessoas são máquinas):

| Para pessoas | Para software |
|---|---|
| Dificuldades **acontecem** | Falhas **acontecem**: rede cai, entrada vem errada |
| Reconhecer o que sente, sem se cobrar | Tratar a falha em vez de fingir que não existe (`try/catch`) |
| Pausar antes de decidir | Esperar entre tentativas (*backoff*) |
| Pedir apoio | Plano B (*fallback*) e serviços que ajudam |
| Saber quando descansar | Disjuntor de circuito: parar de insistir por um tempo |
| Um passo possível por vez | Funções pequenas, testadas, compostas |

::: figura A jornada do livro: da vida para o código e de volta
<svg viewBox="0 0 760 220" role="img" aria-label="Uma jornada de quatro etapas: perceber, apoiar-se, agir e rever, cada uma com um recurso de C#">
  <path d="M92 112 C210 35, 310 185, 425 105 S620 45, 668 108" fill="none" stroke="var(--primary)" stroke-width="4" stroke-dasharray="8 7"/>
  <g text-anchor="middle">
    <circle cx="92" cy="112" r="46" class="f-info"/><text x="92" y="106" font-weight="800">Perceber</text><text x="92" y="126" font-size="12" class="t-dim">enum · record</text>
    <circle cx="284" cy="112" r="46" class="f-warn"/><text x="284" y="106" font-weight="800">Apoiar-se</text><text x="284" y="126" font-size="12" class="t-dim">LINQ · interface</text>
    <circle cx="475" cy="112" r="46" class="f-good"/><text x="475" y="106" font-weight="800">Agir</text><text x="475" y="126" font-size="12" class="t-dim">classes · genéricos</text>
    <circle cx="668" cy="112" r="46" class="f-prim"/><text x="668" y="106" font-weight="800">Seguir</text><text x="668" y="126" font-size="12" class="t-dim">async · retry</text>
  </g>
  <text x="380" y="207" text-anchor="middle" font-size="13" class="t-dim">É um caminho com voltas, pausas e companhia, não uma escada que só sobe.</text>
</svg>
:::

## O mapa: 9 capítulos e 1 laboratório

| Cap. | Tema (resiliência) | Recurso de C# que você aprende |
|---|---|---|
| 88 | A vida não vem com botão de desfazer | `enum`, `record`, `switch`, **`try / catch / finally`**, `TryParse` |
| 89 | Emoções não são botões de desligar | Padrões em tupla, `TimeSpan`, **`TimeProvider`** (relógio injetável), `GroupBy` |
| 90 | O que está na sua mochila? | **LINQ** (`ToLookup`, `Except`, `Where`), tuplas, *fluent API* |
| 91 | Otimismo com os pés no chão | **Nullable** (`string?`), *relational/property patterns*, `switch` com tuplas |
| 92 | Pequenas provas de que você consegue | **Encapsulamento**, `internal`, **iteradores** (`yield return`), `IReadOnlyList` |
| 93 | Ninguém precisa ser uma ilha | **Interfaces**, classe abstrata, **polimorfismo**, `HashSet` |
| 94 | Cada história tem muitas raízes | **Genéricos**, `Func<>`, `params`, lambdas, `where T : notnull` |
| 95 | Resiliência não é sorrir por obrigação | **`Resultado<T>`**, tratamento de erro sem exceção, métodos de extensão |
| 96 | Quando o serviço cai | **`async/await`**, `CancellationToken`, *retry* com *backoff*, **disjuntor** |
| Lab 10 | Caderno de um próximo passo | Tudo junto: `TextReader/TextWriter`, `init`, *property patterns*, testes |

Cada capítulo segue o mesmo ritmo: **uma cena do dia a dia**, **uma imagem**, **a ideia do livro** (em palavras nossas), a **ponte para o código**, o **modelo em C#** com teste, um **exercício com resposta**, um **resumo**, um **quiz** e **cartões de revisão**.

## O que você precisa saber antes

- Variáveis, `if`, `for`/`foreach`, métodos e classes simples, do **Livro 1** da coleção (*Aprenda C#*).
- Não precisa dominar LINQ, genéricos nem `async`: cada um é **apresentado no capítulo em que aparece**, com um passo de cada vez.

::: dica Como rodar o código do livro
Todo o código está em `codigo/resiliencia/` (uma solução do Visual Studio). Com o **.NET 8 SDK** ou superior:

```bash
dotnet test codigo/resiliencia/Resiliencia.sln
dotnet run --project codigo/resiliencia/Resiliencia.Console
```

O primeiro comando roda **todos os testes** dos capítulos. O segundo abre o **Caderno de um próximo passo** (Laboratório 10). Os arquivos têm `ImplicitUsings` ligado, por isso não aparecem `using System;` nem `using System.Linq;` nos trechos do livro.
:::

## Como aproveitar a leitura

- Leia **um pouco por vez**. Não há medalha por terminar rápido.
- Você pode **pular qualquer exercício** que mexa demais com você. Os de código usam situações leves (uma tarefa atrasada, uma dúvida na prova).
- **Execute os testes**: eles são a melhor documentação. Quando um teste falha, é uma conversa, não um julgamento.
- Volte aos capítulos quantas vezes quiser. Resiliência também é **rever o caminho sem se cobrar perfeição**.

::: pense Uma pergunta para começar
Quando um dia difícil termina, o que costuma ajudar você a dar o próximo passo, mesmo que pequeno?
:::

::: cartas
Resiliência é nunca ficar triste? :: Não. É possível sentir emoções difíceis e, ainda assim, buscar apoio, adaptação ou um próximo passo.
Pedir ajuda é sinal de fraqueza? :: Não. Relações e apoio fazem parte dos recursos que ajudam pessoas a atravessar dificuldades.
Um programa pode medir a resiliência de alguém? :: Não. Os modelos deste livro organizam ideias e perguntas; não avaliam pessoas.
Por que usar C# para falar de resiliência? :: Porque modelar uma ideia em código obriga a separar suas partes, e testá-la mostra onde ela é frágil.
O que é um teste neste livro? :: Uma frase executável sobre o comportamento do código: "dado isto, espero aquilo".
:::
