---
id: ang-atencao-cap05
order: 327
kind: capitulo
num: 229
group: Angular · Parte 58 · As oito semanas
title: "Semana 1 · Acordar para o piloto automático"
subtitle: O que o Angular faz sem que você perceba. Detecção de mudanças sem zone.js, OnPush por padrão, Eager, markForCheck, e por que signals mudam tudo.
minutes: 60
hue: 330
objectives:
- Entender a proposta da **primeira semana**: perceber quanto do dia acontece no piloto automático
- Saber **o que é detecção de mudanças** e o que ela faz a cada passada
- Entender o que mudou com o fim do **zone.js** (zoneless) e com o **OnPush por padrão** no Angular 22
- Saber **o que avisa** o Angular: signals lidos no template, eventos do template, `markForCheck`
- Comparar **OnPush** e **Eager** num experimento que roda nos testes
origem: angular/livros/01-atencao-plena-angular/content/06-cap05.md
---

::: cena A tela atrasada
Caio portou uma função do C#: depois de um `setTimeout`, ela incrementava um contador. No console, o número subia. Na tela, ficava parado no zero. "O Angular está quebrado?" Lia respondeu com uma pergunta: "Quem avisou ao Angular que o número mudou?"
:::

::: ilustracao img/ang-atencao/cap05.svg | Um piloto automático que cuida de tudo em silêncio. Acordar é perceber o que ele está fazendo, e quando ele não está olhando.
:::

## O que o livro diz

A primeira semana do programa é sobre **acordar para o piloto automático**. Os autores convidam a fazer algo comum, como comer um pedaço de chocolate, **devagar e com atenção total**: olhar, cheirar, sentir a textura, notar o gosto mudando. A surpresa costuma ser grande: algo que fazemos o tempo todo tem muito mais detalhes do que percebemos quando fazemos no automático.

A semana inclui uma prática diária com o **corpo e a respiração** e um hábito pequeno: **mudar de lugar**, sentar em outra cadeira numa refeição ou reunião, para notar o quanto escolhemos sem escolher.

Nas nossas palavras: **o piloto automático é útil, mas vale saber o que ele está fazendo, e o que ele deixa de ver.**

## Da vida para o código

| Na vida | No Angular |
|---|---|
| O piloto automático que cuida do dia | A **detecção de mudanças**: o Angular confere os templates e atualiza a tela |
| Acontecer algo e ninguém perceber | Mudar um **campo comum** num `setTimeout`: a tela não fica sabendo |
| Prestar atenção de propósito | Um **signal** lido no template: o Angular sabe exatamente o que mudou |
| Avisar alguém à mão | **`markForCheck()`**: funciona, mas depende de lembrar |
| Comer o chocolate devagar | Ver, num experimento, o que **OnPush** e **Eager** fazem de diferente |

## 1) O que é detecção de mudanças

Um template é uma **descrição**: "aqui vai o valor de `respiracoes`". A **detecção de mudanças** é o processo em que o Angular percorre a árvore de componentes, reavalia as expressões dos templates e, se um valor mudou, atualiza aquele pedacinho do DOM.

A pergunta importante é: **quando** essa passada acontece? Durante anos, a resposta foi "o tempo todo": uma biblioteca chamada **zone.js** interceptava todo evento, todo `setTimeout`, toda resposta de rede, e mandava o Angular conferir **tudo** depois de cada um. Funcionava como um piloto automático muito diligente, e muito desperdiçador.

::: novo Sem zone.js e com OnPush por padrão
Desde o Angular 21, projetos novos **não usam zone.js** (*zoneless*). No Angular 22, todo componente é **OnPush por padrão**. Juntas, as duas mudanças querem dizer: o Angular só confere um componente quando **alguém avisa** que algo mudou nele. A estratégia antiga, que confere sempre, continua existindo com o nome **`Eager`** (o nome `Default` está obsoleto).
:::

## 2) O que avisa o Angular

Num app sem zone.js, uma passada de detecção é agendada quando:

1. Um **signal lido num template** muda. O Angular marca **só** os componentes que leem aquele signal.
2. Um **evento ligado no template** acontece (`(click)`, `(input)`...). O componente do evento e seus ancestrais são marcados.
3. Alguém chama **`markForCheck()`** (ou usa o pipe `async`, que chama por você).
4. Um **input** do componente recebe um valor novo (com OnPush, comparado por referência).

Fora disso, **nada acontece**. Mudar um campo comum dentro de um `setTimeout`, de uma promessa ou de um callback de biblioteca não avisa ninguém.

## 3) Três contadores

O experimento da cena, em três versões:

<<< src/app/capitulos/cap05/contadores.ts

E os testes, que esperam um pouco e conferem a tela:

<<< src/app/capitulos/cap05/deteccao.spec.ts#contadores

- **`ContadorDistraido`**: o número muda, mas ninguém avisa. A tela fica em 0. É o piloto automático sem ninguém olhando.
- **`ContadorAtento`**: `respiracoes` é um signal lido no template. Mudou, a tela acompanha, sem nenhuma linha extra.
- **`ContadorAvisado`**: funciona porque chama `markForCheck()`. Funciona **desde que** alguém lembre de chamar em todo lugar que muda o campo. É o modo Atuante do capítulo 3 de novo.

::: regra A regra prática
Se um valor aparece no template e pode mudar, faça dele um **signal** (ou um `computed`). Assim você não precisa pensar em detecção de mudanças, e isso é exatamente o objetivo.
:::

## 4) OnPush × Eager, lado a lado

Uma última experiência, a "do chocolate": olhar devagar para o que as duas estratégias fazem. Dois componentes idênticos, com um campo **comum** (sem signal), um OnPush e outro Eager, dentro de um pai com um botão que muda os dois:

<<< src/app/capitulos/cap05/vizinhos.ts

<<< src/app/capitulos/cap05/deteccao.spec.ts#vizinhos

O clique acontece no **pai**: o evento marca o pai para verificação, e a passada desce pela árvore.

- O filho **Eager** é conferido sempre que a passada chega nele, então mostra `depois`.
- O filho **OnPush** só é conferido se **ele** foi marcado (um input novo, um evento dele, um signal que ele lê). Ninguém o marcou, então continua mostrando `antes`, mesmo com o campo valendo `depois`.

Por isso o OnPush é o padrão: ele **não confere o que não mudou**. Num app grande, isso é a diferença entre conferir 5 componentes e conferir 500. E com signals, você nunca precisa pensar nisso: o signal marca exatamente quem o lê.

::: cuidado Quando usar Eager
Quase nunca. Ele existe para código antigo que muda campos comuns por fora (bibliotecas antigas, por exemplo), enquanto você migra para signals. Se precisar dele, prefira usar num componente pequeno e isolado, e deixe um comentário dizendo por quê.
:::

::: csharp Do WPF ao Angular
No WPF, a tela só muda quando uma propriedade dispara `PropertyChanged`: quem esquece, vê a tela parada. O Angular antigo com zone.js era o oposto, conferia tudo sempre. O Angular moderno com signals fica no melhor dos dois: avisa sozinho, e avisa só quem precisa.
:::

## 5) Como enxergar isso no navegador

O **Angular DevTools** (extensão para Chrome e Firefox) tem um *profiler* que mostra cada passada de detecção de mudanças e quanto tempo cada componente levou. Abra a página **Demonstrações → OnPush e Eager lado a lado**, grave um perfil, clique no botão e observe quais componentes aparecem na passada.

::: pratica Mudar de cadeira
Hoje, sente em um lugar diferente em alguma situação de rotina: outra cadeira à mesa, outro lado da sala de reunião. Repare no pequeno desconforto e no que você nota de novo ao redor. Depois, no código, faça o equivalente: abra um arquivo que você usa todo dia e leia **devagar**, como se fosse a primeira vez.
:::

::: exercicio Consertando o distraído
Reescreva `ContadorDistraido` para que a tela acompanhe a mudança **sem** usar `markForCheck`. Depois, explique em uma frase por que a sua versão é mais segura que a do `ContadorAvisado`.

--- resposta
Basta fazer do campo um signal, como no `ContadorAtento`:

```ts
readonly respiracoes = signal(0);

contarDepois(ms: number): void {
  setTimeout(() => this.respiracoes.update((n) => n + 1), ms);
}
```

E no template, `{{ respiracoes() }}`. É mais segura porque o aviso faz parte do **valor**: qualquer código que mudar `respiracoes`, em qualquer lugar e a qualquer momento, avisa a tela. Com `markForCheck`, cada lugar que muda o campo precisa lembrar de avisar, e um dia alguém esquece.
:::

::: resumo
- A primeira semana do livro é sobre **perceber o piloto automático**, com práticas como comer devagar e mudar de lugar.
- **Detecção de mudanças** é a passada em que o Angular reavalia os templates e atualiza o DOM.
- Projetos novos são **zoneless** (sem zone.js) e, no Angular 22, componentes são **OnPush** por padrão: só são conferidos quando **alguém avisa**.
- Avisam o Angular: um **signal lido no template**, um **evento do template**, um **input novo** e **`markForCheck()`**.
- Mudar um **campo comum** num `setTimeout` não avisa ninguém; a tela fica para trás.
- **Eager** confere o componente em toda passada que chega nele; **OnPush** só se ele foi marcado. Use Eager só para código legado.
- **Signals** resolvem a questão: marcam exatamente quem os lê.
:::

::: quiz
? Qual é o convite da primeira semana do programa?
+ Perceber quanto do dia acontece no piloto automático, com práticas como comer devagar e mudar de lugar
- Meditar uma hora por dia
- Eliminar todos os hábitos
- Anotar todos os pensamentos
> Acordar para o automático, sem precisar eliminá-lo.

? O que é a detecção de mudanças?
- A compilação do TypeScript
+ A passada em que o Angular reavalia os templates e atualiza o que mudou no DOM
- O carregamento das rotas
- A execução dos testes
> É o "piloto automático" que mantém a tela em dia.

? Num app sem zone.js, por que o `ContadorDistraido` não atualiza a tela?
+ Porque mudar um campo comum num `setTimeout` não avisa o Angular
- Porque `setTimeout` não funciona no Angular
- Porque falta importar o componente
- Porque o template tem um erro
> Nenhum dos gatilhos (signal, evento, input, markForCheck) aconteceu.

? Qual das opções NÃO agenda uma verificação do componente?
- Um signal lido no template mudar
- Um `(click)` no template do componente
- Uma chamada a `markForCheck()`
+ Uma promessa resolvida que muda um campo comum
> Sem zone.js, promessas e temporizadores não avisam o Angular.

? No experimento dos vizinhos, por que o filho OnPush continua mostrando "antes"?
+ Porque ninguém marcou aquele filho; o evento aconteceu no pai e o campo do filho é comum
- Porque OnPush nunca atualiza a tela
- Porque o pai é Eager
- Porque o clique falhou
> OnPush só é conferido quando é marcado (input, evento próprio, signal lido).

? No Angular 22, qual é o nome atual da estratégia que confere sempre?
- Default
- Always
+ Eager
- CheckAlways
> `Default` ainda existe, mas está obsoleto e equivale a `Eager`.
:::

::: cartas
Detecção de mudanças :: Passada em que o Angular reavalia templates e atualiza o DOM onde algo mudou.
zone.js :: Biblioteca que interceptava eventos e temporizadores para disparar detecção em tudo; ausente em projetos novos.
Zoneless :: App sem zone.js; a detecção só é agendada quando algo avisa (signals, eventos, markForCheck).
OnPush :: Estratégia padrão no Angular 22: o componente só é conferido quando é marcado.
Eager :: Estratégia que confere o componente em toda passada que chega nele (antigo `Default`).
markForCheck :: Marca o componente (e ancestrais) para a próxima verificação; aviso manual.
O que avisa o Angular :: Signal lido no template, evento do template, input novo, markForCheck.
:::
