---
id: amor-intro
order: 136
kind: intro
group: C# & Mentalidade · Parte 17 · Você é aquilo que ama, em C#
title: Você é Aquilo que Ama em C# — apresentação
subtitle: Você é mais o que deseja e pratica do que o que sabe. Onze ideias sobre desejo, hábito e formação, cada uma virando um modelo em C# que você roda e testa.
minutes: 15
hue: 345
badge: ★
objectives:
- Entender como o livro *Você é aquilo que ama* (James K. A. Smith) é usado aqui, e **o que fica de fora**
- Ver o **mapa** dos 11 capítulos e do laboratório, com o **recurso de C#** de cada um
- Saber o que já precisa conhecer e como **rodar o código** do livro
origem: livros/08-voce-e-aquilo-que-ama-csharp/content/00-intro.md
---

::: ilustracao img/amor/intro.svg | Cabeça, coração e mãos: saber, querer e praticar. Este livro olha sobretudo para os dois últimos.
:::

## Sobre este livro

Este é o **Livro 8** da coleção *Aprenda C#*. Ele parte de uma ideia simples e incômoda, defendida por **James K. A. Smith** em *Você é aquilo que ama: o poder espiritual do hábito*: nós nos comportamos **menos** pela soma do que sabemos e **mais** pela direção do que desejamos, e o que desejamos é moldado, em grande parte sem que percebamos, **pelo que repetimos**.

Quem programa conhece isso de perto. Sabemos que testes são importantes, e entregamos sem eles na sexta à noite. Sabemos que reuniões longas cansam, e marcamos mais uma. Sabemos que o celular rouba atenção, e o pegamos de novo. O hiato entre **o que sei** e **o que faço** não se fecha com mais informação. Fecha-se (às vezes) com **prática, ambiente, companhia e tempo**.

Aqui, cada capítulo pega uma ideia do livro, traduz para a vida de quem constrói software e a transforma em **um modelo executável em C#**, com testes. As cenas, as explicações, os exercícios e todo o código são **originais**: este livro é **inspirado** na obra, não a substitui nem a resume.

::: cuidado O que este livro é, e o que não é
*Você é aquilo que ama* é uma obra **cristã**, escrita por um filósofo e teólogo para leitores de fé, e muitos de seus argumentos se apoiam em convicções religiosas do autor. Esta coleção a usa como usou *Os Quatro Compromissos* (Livro 5): como **lente de sabedoria prática** sobre desejo, hábito e formação. Quando uma ideia depende da fé do autor, o texto diz que é **dele**; você **não precisa** compartilhá-la para aproveitar o resto. Também **não é ciência**: o autor cita pesquisas de psicologia, e os modelos em C# aqui são **ilustrações**, não medidas de ninguém. Nenhum programa deste livro **avalia, classifica ou prevê pessoas**.
:::

## A ideia central, em uma imagem

A imagem favorita do autor para criticar é a de uma pessoa como "cérebro num palito": uma cabeça enorme (o que sabemos) equilibrada em um corpo minúsculo, como se a vida fosse só decidir bem. A proposta dele é mais **inteira**: somos **cabeça** (saber), **coração** (querer) e **mãos** (praticar), e é o coração (aquilo que nos atrai, o que amamos) que dirige as mãos com mais frequência do que a cabeça.

::: figura Três regiões, uma pessoa
<svg viewBox="0 0 740 230" role="img" aria-label="Três círculos: cabeça (saber), coração (querer) e mãos (praticar), com setas mostrando que o querer dirige a prática e que a prática forma o querer">
  <g text-anchor="middle">
    <circle cx="130" cy="105" r="62" class="f-info"/><text x="130" y="100" font-weight="800">Cabeça</text><text x="130" y="120" font-size="12" class="t-dim">o que sei</text>
    <circle cx="370" cy="105" r="72" class="f-bad"/><text x="370" y="100" font-weight="800">Coração</text><text x="370" y="120" font-size="12" class="t-dim">o que quero, o que amo</text>
    <circle cx="610" cy="105" r="62" class="f-good"/><text x="610" y="100" font-weight="800">Mãos</text><text x="610" y="120" font-size="12" class="t-dim">o que pratico</text>
  </g>
  <path class="arrow" d="M446 90C500 60 510 60 548 80"/><text x="500" y="52" text-anchor="middle" font-size="12" class="t-dim">o querer dirige a prática</text>
  <path class="arrow" d="M548 135C510 165 440 165 442 130"/><text x="495" y="186" text-anchor="middle" font-size="12" class="t-dim">a prática forma o querer</text>
  <path class="arrow" d="M196 105H296"/><text x="246" y="96" text-anchor="middle" font-size="12" class="t-dim">ajuda</text>
  <text x="370" y="222" text-anchor="middle" font-size="13" class="t-dim">A cabeça ilumina o caminho, mas não é ela quem costuma dar o primeiro passo.</text>
</svg>
:::

## O mapa: 11 capítulos e 1 laboratório

| Cap. | Ideia do livro | Recurso de C# que você aprende |
|---|---|---|
| 105 | Você é o que ama (e não só o que pensa) | `[Flags] enum`, operadores bit a bit, `Aggregate`, pesos e `GetValueOrDefault` |
| 106 | O coração é uma bússola; o mapa pode estar errado | `readonly record struct`, **operadores sobrecarregados**, `Math`, `double?` |
| 107 | Amar é um hábito (virtude, imitação e prática) | **Imutabilidade** com `with`, parâmetros opcionais, `Aggregate` como "dobra" |
| 108 | Você pode não amar o que acredita amar | **LINQ avançado**: `GroupJoin`, `ExceptBy`, `Select` com índice |
| 109 | As liturgias "seculares": a forma ensina | **`[Flags]` em profundidade**, `BitOperations`, `required` e `init` |
| 110 | Reabituar o apetite: querer muda com a prática | **Delegates** `Func<int,double>`, `Zip`, `TakeWhile`, iteradores preguiçosos |
| 111 | Em que história você está inserido? | **Máquina de estados**, `event`, `EventHandler<T>`, `EventArgs` |
| 112 | Ritmos que formam o time | `IComparable<T>`, `SortedSet<T>`, `yield return`, `DateOnly` |
| 113 | Aprender "de cor" | Repetição espaçada, `Random` injetado, `record` com `with`, `Dictionary` |
| 114 | Restrições que libertam | **Objetos de valor**, `IParsable<T>`, **operador implícito**, genéricos com `static abstract` |
| 115 | Tudo importa: o trabalho como liturgia | **Pipeline** (middleware): delegates `async`, closures, funções locais |
| Lab 12 | Auditoria litúrgica pessoal | `System.Text.Json`, *raw string literals*, `JsonStringEnumConverter`, tudo junto |

Cada capítulo tem o mesmo ritmo: **uma cena**, **uma imagem**, **o que o livro diz** (em palavras nossas), a **ponte para a vida de quem programa**, o **modelo em C#** com testes, um **exercício com resposta**, **resumo**, **quiz** e **cartões de revisão**.

## O que você precisa saber antes

- Variáveis, `if`, laços, métodos, classes, listas e um pouco de LINQ, do **Livro 1** da coleção. Quem leu o **Livro 6** (*Resiliência em C#*) já viu `enum`, `record`, `async` e testes, mas **não é obrigatório**.
- Cada recurso novo é **apresentado no capítulo em que aparece**, um de cada vez.

::: dica Como rodar o código do livro
Todo o código está em `codigo/amor/` (uma solução do Visual Studio). Com o **.NET 8 SDK** ou superior:

```bash
dotnet test codigo/amor/Habitos.sln
dotnet run --project codigo/amor/Habitos.Console
```

O primeiro comando roda **todos os testes** dos capítulos. O segundo abre a **Auditoria litúrgica** (Laboratório 12), que lê um arquivo JSON **seu**, local, e **não grava nem envia nada**. Os projetos usam `ImplicitUsings`, por isso não aparecem `using System;` nem `using System.Linq;` nos trechos.
:::

::: dica A numeração é a da coleção
Os capítulos e os laboratórios dos dez livros formam **uma só numeração (1 a 133)**. Por isso este livro começa no capítulo 105, e as menções como "cap. 55" ou "Lab 10" viram **links** que abrem o livro certo.
:::

## Como aproveitar a leitura

- Leia **um pouco por vez**. As ideias do livro são lentas por natureza: hábito não se forma em uma tarde.
- **Execute os testes** e **mexa no código**: troque um número, quebre uma regra, veja o teste falhar. É a prática que o livro defende, aplicada ao próprio estudo.
- Use os modelos como **espelho curioso**, nunca como régua. Se um deles lhe der vontade de se cobrar, feche-o e converse com alguém.

::: pense Uma pergunta para começar
Pense em algo que você **sabe** que deveria fazer (testar mais, dormir melhor, escrever uma documentação) e não faz. O que, na sua rotina, **ensina o contrário** todos os dias?
:::

::: cartas
Você é o que pensa? :: Para o autor, não só: somos antes de tudo seres que desejam, e o desejo é moldado por hábitos.
O que é "liturgia", neste livro? :: Uma prática repetida que carrega uma história sobre o que vale a pena desejar; pode ser religiosa ou do dia a dia.
Este livro é ciência? :: Não. É sabedoria prática, com modelos ilustrativos em C#; as ideias do autor têm base filosófica e religiosa.
Preciso concordar com a fé do autor? :: Não. O que depende dela é identificado como convicção dele, e o resto serve como lente de hábito e formação.
Por que testar o código? :: Porque um teste é uma frase executável sobre o que foi prometido, e conferi-la repetidamente forma o hábito de cuidar.
:::
