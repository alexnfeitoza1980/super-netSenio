---
id: joao-intro
order: 161
kind: intro
group: C# & Mentalidade · Parte 18 · O Evangelho segundo João em C#
title: O Evangelho segundo João em C# — apresentação
subtitle: Dez trechos do Evangelho segundo João, lidos versículo por versículo e transformados em C# passo a passo: cada frase do texto vira algumas linhas de código.
minutes: 15
hue: 220
badge: ★
objectives:
- Entender **como o livro funciona**: o texto, a leitura devagar e o código construído frase por frase
- Ver o **mapa** dos dez trechos e do recurso de C# de cada capítulo
- Saber como **rodar o código** e a **concordância com dados reais do texto grego**
origem: livros/10-evangelho-joao-csharp/content/00-intro.md
---

::: ilustracao img/joao/intro.svg | Uma luz que atravessa a noite: o texto grego de um lado, o código do outro, e a lupa entre os dois.
:::

## Sobre este livro

O **Evangelho segundo João** é um dos textos mais lidos da história: poucas cenas, imagens muito fortes (luz, água, pão, porta, pastor, videira) e frases que a gente pode ler uma por uma. Este livro parte dele de um jeito simples:

> **O texto vira código.** Pegamos um trecho, lemos **frase por frase** e transformamos **cada frase em algumas linhas de C#**, em passos pequenos.

Por exemplo, em **Jo 1,1**: *"No princípio era a Palavra, e a Palavra **estava junto de** Deus, e a Palavra **era** Deus"* (Bíblia Livre; os tipos de C# usam o nome tradicional **Verbo**). Há duas expressões que parecem parecidas e não são: **"estava junto de"** e **"era"**. Em C#, elas viram **dois recursos diferentes**: uma *referência* (um objeto ligado a outro) e uma *herança* (um tipo que **é** de outro tipo). Ler o texto assim obriga a ler **com atenção**, palavra por palavra, e a dizer onde a analogia **deixa de valer**.

Outro exemplo, em **Jo 2,7-9**: *"Enchei estes vasos com água [...] Agora tirai, e a levai ao mestre de cerimônia"*. Cada ordem vira um **método** (`Encher`, `Tirar`, `Provar`), e o código **conta a cena na mesma ordem do texto**, marcando o que é **escolha nossa** (o texto não diz quando a água virou vinho).

Cada capítulo segue sempre o mesmo caminho:

1. **O texto.** O trecho, por inteiro, na Bíblia Livre (CC BY 4.0).
2. **Lendo devagar.** O que o trecho diz, em palavras simples.
3. **Do versículo ao código.** Cada frase vira código, passo a passo. O código final é compilado e testado, e a saída que você vê é a de um comando que você mesmo pode rodar (`dotnet run --project codigo/joao/Joao.Console -- cap124`).
4. **Lupa no grego.** Algumas palavras do original, com contagens conferidas no texto grego.
5. **Limite da analogia.** O que o código **não** diz: onde a comparação funciona e onde deixa de valer.
6. **Indo além.** A mesma ideia em versão profissional, com recursos mais avançados de C#, para quem quer ir mais fundo.
7. **Exercício, resumo, quiz e cartões.**

::: cuidado O que este livro é, e o que não é
- A essência do livro é **religiosa**: o texto é o do Evangelho, e o livro o trata com **respeito**. Mas é um livro de **estudo**: usa o texto para ensinar C#. Não é culto, sermão nem aconselhamento espiritual. Você **não precisa compartilhar a fé do texto** para aproveitar o livro.
- O código é um jeito de **ler com cuidado**, e **não** uma explicação do mistério do texto. Quando o C# mostra algo (por exemplo, a diferença entre "estar junto de" e "ser"), ele mostra **a forma da frase**; o que a frase significa para a fé é assunto das tradições que a leem. O livro separa sempre **texto**, **interpretação**, **analogia** e **realidade técnica**.
- Quando o texto **não diz** algo (como o momento em que a água virou vinho, ou uma lista de "sete sinais"), o livro **não inventa**: avisa e marca a escolha como nossa.
- Os **versículos** são da **Bíblia Livre** (© Diego Santos, Mario Sérgio e Marco Teles, licença **CC BY 4.0**, eBible.org), com pequenos ajustes de pontuação. O **texto grego** vem do SBLGNT (CC BY 4.0), com as anotações do MorphGNT (CC BY-SA 3.0), e os sentidos das palavras do dicionário de Strong (OpenScriptures). As fontes e licenças estão em `AUDITORIA/fontes-e-licencas.md` e no README.
:::

## Um olhar sobre o conjunto

João conta pouco e escolhe muito. Os estudiosos costumam ver duas grandes partes, uma forma de organizar a leitura (e não uma divisão escrita pelo autor):

::: figura Como o Evangelho costuma ser lido, em duas partes
<svg viewBox="0 0 760 210" role="img" aria-label="Linha do tempo com o prólogo, o livro dos sinais (capítulos 1 a 12), o livro da hora ou da glória (capítulos 13 a 20) e o epílogo (capítulo 21)">
  <rect x="20" y="70" width="70" height="60" rx="12" class="f-prim"/><text x="55" y="96" text-anchor="middle" font-weight="800" font-size="13">Prólogo</text><text x="55" y="114" text-anchor="middle" font-size="11" class="t-dim">1,1-18</text>
  <rect x="100" y="50" width="330" height="100" rx="14" class="f-info"/><text x="265" y="84" text-anchor="middle" font-weight="800">O livro dos sinais</text><text x="265" y="106" text-anchor="middle" font-size="12" class="t-dim">capítulos 1 a 12: sinais e discursos, em público</text><text x="265" y="126" text-anchor="middle" font-size="12" class="t-dim">água em vinho · poço · pães · luz · Lázaro</text>
  <rect x="440" y="50" width="250" height="100" rx="14" class="f-warn"/><text x="565" y="84" text-anchor="middle" font-weight="800">O livro da hora</text><text x="565" y="106" text-anchor="middle" font-size="12" class="t-dim">capítulos 13 a 20: a despedida, em privado</text><text x="565" y="126" text-anchor="middle" font-size="12" class="t-dim">lava-pés · videira · caminho · paixão</text>
  <rect x="700" y="70" width="46" height="60" rx="12" class="f-good"/><text x="723" y="96" text-anchor="middle" font-weight="800" font-size="12">21</text><text x="723" y="114" text-anchor="middle" font-size="10" class="t-dim">epílogo</text>
  <text x="380" y="186" text-anchor="middle" font-size="13" class="t-dim">João conta só duas vezes os sinais pelo número (2,11 e 4,54): a lista dos "sete" é de quem estuda o texto.</text>
</svg>
:::

## O mapa vivo

Antes de começar, **brinque**: o mapa abaixo mostra as sete imagens "Eu sou", as palavras do grego e os lugares, e liga cada um ao capítulo do livro. Clique nos pontos.

::: html
<iframe src="codigo/joao/mapa.html" title="Mapa vivo do Evangelho segundo João e do livro" loading="lazy" style="width:100%;height:640px;border:1px solid var(--line);border-radius:16px;background:var(--surface)"></iframe>
:::

## O mapa do livro: dez trechos, dez ideias de C#

| Cap. | O texto | Frase por frase, em C# | Indo além |
|---|---|---|---|
| 124 | **Jo 1,1-5**: "No princípio era a Palavra" | tipo, **referência** ("estava junto de") e **herança** ("era"), com os limites | *logging*, `AsyncLocal`, `IDisposable`, objeto nulo |
| 125 | **Os sete "Eu sou"** | `record` com a declaração; **etiqueta** em cada classe | atributos e **reflexão** |
| 126 | **Jo 2,1-11**: água em vinho | métodos `Encher`, `Tirar`, `Provar` (e o que é escolha nossa) | **execução adiada** (`yield`) e genéricos |
| 127 | **Jo 3,1-8**: o vento | **evento** (`+=`): quem sopra e quem ouve | `IObservable<T>` e operadores |
| 128 | **Jo 4,5-15**: o poço de Jacó | "o poço é fundo" e "uma fonte por dentro" (`??=`) | **cache** com validade e `Lazy<T>` |
| 129 | **Jo 6,5-13**: os pães | **fila**, laço e "nada se perde" (invariante) | `Channel<T>` e `IAsyncEnumerable<T>` |
| 130 | **Jo 10,1-5**: a voz do pastor | comparar a voz; o problema da imitação | **assinatura HMAC** |
| 131 | **Jo 11,38-44**: Lázaro | `enum`, `Chamar`, `Desligar`, `Livre` | ***event sourcing*** |
| 132 | **Jo 13,3-15**: o lava-pés | `IPessoa` e a **toalha** (*decorator*) | camadas de serviço (registro, tentativas, cache) |
| 133 | **Jo 15,1-8**: a videira | construtor que **recebe** a videira (injeção) | **recipiente** de dependências |
| Lab 14 | Os dados de todo o livro | concordância e rotas | LINQ, busca em largura, Dijkstra |

## O que você precisa saber antes

- Variáveis, `if`, laços, métodos, classes e listas, do **Livro 1**. Os passos "do versículo ao código" são **simples**; a parte "Indo além" é de nível intermediário a avançado e **pode ser pulada** numa primeira leitura.
- Não é preciso saber grego. Todas as palavras gregas vêm com **transliteração** e **sentido**.

::: dica Como rodar o código do livro
Todo o código está em `codigo/joao/` (uma solução do Visual Studio). Com o **.NET 8 SDK** ou superior:

```bash
dotnet test codigo/joao/Joao.sln
dotnet run --project codigo/joao/Joao.Console -- onde menō
dotnet run --project codigo/joao/Joao.Console -- rota jordao betania
```

O primeiro comando roda **todos os testes**: há um para quase cada frase dos versículos (pasta `Texto/`). Os outros dois usam a **concordância** (Laboratório 14): onde a palavra *menō* ("permanecer") aparece no grego, e uma rota no mapa dos lugares do Evangelho. Os projetos usam `ImplicitUsings`, por isso não aparecem `using System;` nem `using System.Linq;` nos trechos.
:::

::: dica A numeração é a da coleção
Os capítulos e os laboratórios dos dez livros formam **uma só numeração**. Por isso este livro começa no capítulo 124, e as menções como "cap. 55" ou "Lab 7" viram **links** que abrem o livro certo.
:::

## Como aproveitar a leitura

- Leia **um trecho por vez**. O texto original merece pausa: leia o versículo **em voz alta** antes do código.
- **Execute os testes**, mude números, quebre regras, veja o que falha: o nome de cada teste diz **o que o texto afirma**.
- Na **lupa no grego**, confira: o laboratório deixa você **repetir qualquer contagem** do livro com um comando.
- Se algum trecho for pesado demais, **pule**: você pode ler os capítulos em qualquer ordem.

::: pense Uma pergunta para começar
Pense em uma frase que você já ouviu muitas vezes sem parar para **ler palavra por palavra**. O que muda quando você a lê devagar, como se tivesse de explicá-la a um computador?
:::

::: cartas
Este livro é religioso? :: A essência é religiosa (o texto é o do Evangelho, tratado com respeito), mas o livro é de estudo: usa o texto para ensinar C#. Você não precisa compartilhar a fé do texto.
Como o texto vira código? :: Lemos o trecho frase por frase, e cada frase vira algumas linhas de C#: uma classe, um método, uma lista, uma regra.
Que tradução é usada? :: A Bíblia Livre (CC BY 4.0), com os trechos marcados.
O que é a "lupa no grego"? :: Uma seção em cada capítulo com palavras do original, sua transliteração, seu sentido e contagens verificadas no texto grego.
Os "sete sinais" são de João? :: Não. João numera só dois (2,11 e 4,54); uma lista de sete não está no texto.
:::
