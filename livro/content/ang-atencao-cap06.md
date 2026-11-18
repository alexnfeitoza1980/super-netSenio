---
id: ang-atencao-cap06
order: 328
kind: capitulo
num: 230
group: Angular · Parte 58 · As oito semanas
title: "Semana 2 · Voltar ao corpo"
subtitle: Percorrer o corpo parte por parte, percorrer o template bloco por bloco. Controle de fluxo, bindings, variáveis de template e acessibilidade.
minutes: 65
hue: 290
objectives:
- Entender a **exploração do corpo** como prática de atenção (e não de relaxamento a qualquer custo)
- Dominar os blocos de **controle de fluxo**: `@if`/`@else`, `@for` com `track` e `@empty`, `@switch`, `@let`
- Usar bindings de **atributo**, **classe** e **evento com tecla**, e **variáveis de template** (`#nota`)
- Construir templates **acessíveis**: rótulos, `fieldset`/`legend`, `aria-pressed`, `aria-live`
- Testar o template, incluindo uma checagem automática de acessibilidade
origem: angular/livros/01-atencao-plena-angular/content/07-cap06.md
---

::: cena A tela que ninguém conseguia usar
Marta testou a tela nova de Caio só com o teclado e um leitor de tela. Botões anunciados como "botão", sem nome; um campo sem rótulo; uma mensagem que mudava sem ninguém ouvir. "Visualmente está lindo", ela disse, "mas tem gente que usa este sistema de outro jeito." Caio percebeu que nunca tinha **percorrido** a tela parte por parte.
:::

::: ilustracao img/ang-atencao/cap06.svg | Uma pessoa deitada, com a atenção passando dos pés à cabeça, como uma lanterna. Cada parte recebe um pouco de atenção, sem pressa.
:::

## O que o livro diz

A prática principal da segunda semana é a **exploração do corpo**: deitado ou sentado, você leva a atenção, devagar, a cada região do corpo, notando o que está ali (calor, peso, formigamento, tensão, ou nada) **sem tentar mudar**. Os autores comentam algo que muita gente percebe logo: o modo Atuante aparece, querendo que a prática "dê certo", que a tensão vá embora, que o relaxamento venha. A proposta é notar isso também e voltar para a região seguinte.

O hábito pequeno da semana é **caminhar**: sair para uma caminhada curta, sem objetivo, prestando atenção no corpo em movimento.

Nas nossas palavras: **percorrer com atenção, parte por parte, inclusive as partes que costumamos ignorar.**

::: cuidado Sobre o corpo
Se alguma região do corpo traz dor, desconforto forte ou lembranças difíceis, você pode simplesmente passar por ela mais rápido, ou abrir os olhos e parar. Ninguém é obrigado a ficar com o que não quer.
:::

## Da vida para o código

| Na vida | No template |
|---|---|
| Percorrer o corpo, região por região | **`@for`** percorre a lista, item por item, com **`track`** |
| "Não sinto nada aqui" também é uma observação | **`@empty`**: o que mostrar quando a lista está vazia |
| Dar atenção à região escolhida | **`@if`/`@else`** e **`@let`** para a região atual |
| Cada sensação tem sua resposta | **`@switch`/`@case`/`@default`** |
| As partes que costumamos ignorar | **Acessibilidade**: rótulos, nomes, anúncios para quem usa leitor de tela |

## 1) O componente

<<< src/app/capitulos/cap06/exploracao-do-corpo.ts

Parece muito, mas cada pedaço é simples. Vamos percorrê-lo, como numa exploração do corpo: de cima para baixo.

## 2) `@for`, `track`, variáveis e `@empty`

```html
@for (r of regioes(); track r.id; let i = $index) {
  ...
} @empty {
  <li>Nenhuma região para explorar agora. Tudo bem.</li>
}
```

- **`track r.id`**: cada região é reconhecida pelo `id`. Quando a lista muda, o Angular reaproveita os elementos dos itens que continuam lá. Use sempre um identificador estável; `track $index` só para listas que nunca mudam de ordem.
- **`let i = $index`** dá outro nome a uma variável implícita. As disponíveis são `$index`, `$first`, `$last`, `$even`, `$odd` e `$count`.
- **`@empty`** aparece quando a lista está vazia. É bem melhor que uma tela em branco.

## 3) `@if ... as`, `@let` e `@else`

```html
@if (sensacoes()[r.id]; as s) { ... }
```

O `as s` guarda o resultado da condição numa variável **já estreitada**: dentro do bloco, `s` é uma `Sensacao`, nunca `undefined`.

```html
@let atual = regiaoAtual();
@if (atual) { ... } @else { <p>Escolha uma região para começar.</p> }
```

**`@let`** declara uma variável **no template**. Aqui, ela evita chamar `regiaoAtual()` várias vezes e deixa o template mais legível. As variáveis de `@let` são somente leitura e valem até o fim do bloco em que foram declaradas.

## 4) `@switch`

```html
@switch (sensacoes()[atual.id]) {
  @case ('tensa') { Só note a tensão. Não precisa soltar à força. }
  @case ('relaxada') { Note como é estar assim. }
  @case ('nao-sei') { "Não sei" também é uma observação válida. }
  @default { Perceba sem pressa. }
}
```

Compara com `===` e mostra o primeiro caso que bater. O `@default` cobre o resto, inclusive o "ainda não anotado" (`undefined`).

## 5) Bindings de atributo, classe e teclado

| Escrita | Para quê |
|---|---|
| `[attr.aria-pressed]="r.id === selecionada()"` | Atributos que **não** são propriedades do elemento (como os `aria-*`) usam `attr.` |
| `class="marca" [class]="s"` | A classe fixa `marca` mais a classe dinâmica (`tensa`, `relaxada`...) |
| `[class.atual]="..."` (capítulo 1) | Liga ou desliga **uma** classe |
| `(keyup.enter)="..."` | Evento de tecla com filtro: só dispara no Enter |
| `#nota` | **Variável de template**: dá nome ao elemento para usá-lo no próprio template (`nota.value`) |

::: dica Variáveis de template são locais
`#nota` só existe no template. Para ler o valor na classe, passe-o como argumento, como em `guardarPalavra(atual.id, nota.value)`. Formulários de verdade, com validação, são assunto do capítulo 10.
:::

## 6) As partes que costumamos ignorar: acessibilidade

O componente aplica o básico que resolveria a cena de Marta:

- **Todo campo tem rótulo**: o `<input id="nota">` tem um `<label for="nota">`, e os rádios estão **dentro** de `<label>`. Leitores de tela leem o rótulo ao focar o campo.
- **Grupos de opções** ficam num **`<fieldset>`** com **`<legend>`**: "Como estão ombros?" é lido junto com as opções.
- **Botões de alternância** usam **`aria-pressed`**: o leitor de tela anuncia "pressionado".
- **Mensagens que mudam** ficam num **`aria-live="polite"`**: são anunciadas quando mudam, sem interromper.
- **Botões são `<button type="button">`**, e não `<div (click)>`: assim funcionam com teclado (Tab, Enter, Espaço) sem nenhum código extra.
- O título `<h2 id="titulo-corpo">` dá nome à lista via **`aria-labelledby`**.

::: erro `<div (click)="...">`
Um `div` clicável não recebe foco pelo teclado nem é anunciado como botão. Se uma coisa é clicável, ela é um `<button>` (ação) ou um `<a>` (navegação). Isso resolve a maior parte dos problemas de acessibilidade antes que eles existam.
:::

## 7) Testes, incluindo acessibilidade

<<< src/app/capitulos/cap06/exploracao-do-corpo.spec.ts

O último teste é uma **checagem automática simples**: todo botão tem texto, todo campo tem rótulo. Não substitui testar com leitor de tela de verdade, mas impede regressões bobas. Para auditorias completas, ferramentas como o **axe** e o painel **Lighthouse** do navegador ajudam.

::: pratica Uma exploração curta
Sentado, leve a atenção aos pés por algumas respirações. Depois às pernas, à barriga, aos ombros, à mandíbula. Em cada lugar, só note o que está ali, inclusive "não sinto nada". Se a mente sair, volte à região onde estava. Três minutos bastam.
:::

::: exercicio Um resumo da exploração
Acrescente ao componente um `computed` `resumo`, que conta quantas regiões foram anotadas como **tensas**, e mostre, depois da lista, algo como *2 regiões tensas*. Use `@switch` ou `@if` para escrever "nenhuma região tensa", "1 região tensa" e "N regiões tensas".

--- resposta
Na classe:

```ts
protected readonly tensas = computed(
  () => Object.values(this.sensacoes()).filter((s) => s === 'tensa').length,
);
```

No template:

```html
<p>
  @switch (tensas()) {
    @case (0) { Nenhuma região tensa. }
    @case (1) { 1 região tensa. }
    @default { {{ tensas() }} regiões tensas. }
  }
</p>
```

`@switch` compara com `===`, então `@case (0)` bate com o número zero. A frase não julga: só conta. A decisão do que fazer com a tensão é de quem a sente.
:::

::: resumo
- A exploração do corpo percorre região por região, **notando sem tentar mudar**; o modo Atuante, querendo resultado, também é algo a notar.
- **`@for`** sempre tem **`track`** (de preferência um id); `$index` e companhia estão disponíveis; **`@empty`** cobre a lista vazia.
- **`@if ... as x`** guarda o valor estreitado; **`@let`** declara variáveis no template; **`@switch`** escolhe entre casos com `===`.
- Use **`[attr.x]`** para atributos (como `aria-*`), **`[class]`** e **`[class.x]`** para classes, **`(keyup.enter)`** para teclas, **`#ref`** para referências no template.
- **Acessibilidade** começa no HTML certo: `<button>`, `<label>`, `<fieldset>`/`<legend>`, `aria-pressed`, `aria-live`.
- Uma checagem automática nos testes (rótulos e nomes) evita regressões.
:::

::: quiz
? O que o livro propõe na exploração do corpo?
- Relaxar todas as tensões a qualquer custo
+ Levar a atenção a cada região e notar o que está ali, sem tentar mudar
- Alongar o corpo até doer
- Contar as respirações até cem
> Inclusive notar o desejo de que a prática "dê certo".

? Para que serve o bloco `@empty`?
+ Mostrar algo quando a lista do `@for` está vazia
- Esvaziar a lista
- Esconder o `@for`
- Marcar itens vazios
> Uma lista vazia merece uma mensagem, e não uma tela em branco.

? Como se liga um atributo como `aria-pressed`, que não é uma propriedade do elemento?
- `[aria-pressed]="x"`
+ `[attr.aria-pressed]="x"`
- `(aria-pressed)="x"`
- `{{ aria-pressed }}`
> Atributos que não são propriedades do DOM usam o prefixo `attr.`.

? O que faz `@let atual = regiaoAtual();`?
- Cria um signal novo
+ Declara uma variável somente leitura no template, válida até o fim do bloco
- Muda o valor de `regiaoAtual`
- Exporta a variável para a classe
> Evita repetir a expressão e deixa o template mais legível.

? Por que usar `<button type="button">` em vez de `<div (click)>`?
+ Porque o botão recebe foco pelo teclado e é anunciado como botão sem código extra
- Porque `div` não aceita eventos
- Porque o `button` é mais bonito
- Porque o Angular proíbe `div` clicável
> HTML certo resolve a maior parte da acessibilidade.

? Para que serve `aria-live="polite"`?
- Para deixar o texto em negrito
+ Para leitores de tela anunciarem o trecho quando ele muda, sem interromper
- Para o Angular atualizar o trecho
- Para impedir mudanças
> Mensagens que mudam precisam ser anunciadas a quem não as vê.
:::

::: cartas
@for ... track :: Repete um bloco por item; `track` diz como reconhecer cada item entre atualizações.
@empty :: Bloco do `@for` mostrado quando a lista está vazia.
@if ... as :: Guarda o resultado da condição, já estreitado, numa variável do bloco.
@let :: Declara uma variável somente leitura no template.
@switch / @case / @default :: Escolhe um bloco comparando com `===`.
[attr.x] :: Binding de atributo, para o que não é propriedade do elemento (como `aria-*`).
#ref :: Variável de template: nomeia um elemento para usá-lo no próprio template.
aria-live :: Faz leitores de tela anunciarem mudanças daquele trecho.
:::
