---
id: ang-atencao-lab01
order: 326
kind: lab
num: 47
group: Angular · Parte 57 · Sair do piloto automático
title: "Laboratório: Pausa de um minuto"
subtitle: Um temporizador gentil, do zero ao teste: signals, linkedSignal, DestroyRef, transformação de inputs e um relógio falso.
minutes: 80
hue: 300
objectives:
- **Juntar** o que vimos nos capítulos 1 a 4 num componente completo
- Converter e validar entradas com **`input(..., { transform })`**
- Limpar recursos com **`DestroyRef`**, para não deixar intervalos vivos
- Testar código com tempo usando **relógio falso** do Vitest, sem quebrar o Angular
- Encontrar (e corrigir) um bug real que só aparece com a rota
origem: angular/livros/01-atencao-plena-angular/content/05-lab01.md
---

::: cena Um minuto que virou NaN
Caio terminou o temporizador e abriu o app: a tela mostrava `NaN:NaN`. Nos testes, tudo verde. Beto sorriu: "Os testes passam a duração. A rota não passa nada." O componente funcionava em todo lugar, menos no único lugar em que alguém de verdade ia usá-lo.
:::

::: ilustracao img/ang-atencao/lab01.svg | Um relógio grande, um botão de começar e nenhuma pressa: um minuto é pouco tempo, e é tempo suficiente.
:::

## O que vamos construir

Um componente `app-temporizador` que:

1. Mostra uma contagem regressiva no formato `m:ss` e uma barra de progresso.
2. Tem **Começar**, **Pausar**, **Continuar** e **Reiniciar**.
3. Recebe a duração por **input** (padrão: 60 s) e aceita que ela venha da **URL**: `/pausa?duracao=180`.
4. Avisa por **output** quando termina, uma única vez.
5. Não deixa nenhum intervalo vivo depois que sai da tela.
6. Mostra frases curtas e gentis em cada estado, num `aria-live` para leitores de tela.

No app, ele fica na rota `/pausa` (menu **Pausa de 1 minuto**).

## 1) As funções puras primeiro

Antes de qualquer componente, as regras que não dependem de tela. Funções puras são as mais fáceis de testar, e por isso vêm primeiro:

<<< src/app/labs/lab01/tempo.ts

`formatarTempo` usa `Math.ceil` para que 59,2 s apareça como `1:00` (ainda não passou um segundo inteiro) e `padStart` para os dois dígitos. `duracaoValida` é a resposta ao bug da cena: ela aceita número, texto vindo da URL ou **nada**, e devolve sempre algo utilizável.

## 2) O componente

<<< src/app/labs/lab01/temporizador.ts

Peça por peça:

| Peça | Por quê |
|---|---|
| `input(60, { transform: duracaoValida })` | O **transform** roda sobre todo valor que chega no input, inclusive `undefined` e textos da URL. O resto do componente só vê números válidos. |
| `linkedSignal(() => this.duracao())` | `restantes` é ajustável (o tique diminui) **e** volta ao total quando a duração muda. É o caso clássico de `linkedSignal`. |
| `computed` para `tempo`, `progresso`, `terminou`, `rotuloDuracao` e `mensagem` | Tudo o que a tela mostra é **consequência** de `restantes`, `rodando` e `duracao`. Nada de estado copiado. |
| `inject(DestroyRef).onDestroy(...)` | Quando o componente sai da tela (trocou de rota, por exemplo), o intervalo é limpo. Sem isso, o `setInterval` continuaria rodando para sempre. |
| `@if (rodando()) { ... } @else { ... }` | Um botão só para cada estado, com o texto certo (`Começar` ou `Continuar`). |
| `role="timer"` e `aria-live="polite"` | Leitores de tela entendem que é um temporizador; a **mensagem** é anunciada quando muda, mas o número **não** é anunciado a cada segundo (`aria-live="off"`), o que seria cansativo. |

::: csharp `inject()` em vez de construtor
Em C#, você pediria `DestroyRef` como parâmetro do construtor. No Angular moderno, a função **`inject()`** faz a mesma coisa, chamada dentro do construtor ou na inicialização de um campo. O capítulo 8 explica a injeção de dependência com calma.
:::

### O bug da cena, explicado

No `app.config.ts`, o roteador usa `withComponentInputBinding()`: parâmetros e *query params* da URL viram inputs automaticamente. É ótimo (`/pausa?duracao=180` funciona sem uma linha a mais), mas tem um detalhe: quando a URL **não** traz o valor, o roteador entrega **`undefined`** para o input, **por cima do padrão** `60`. Sem o `transform`, `duracao()` virava `undefined`, `undefined / 60` virava `NaN`, e a barra de progresso ainda lançava um erro ao receber `NaN`.

O teste com `setInput` não pegava o bug porque sempre passava um número. A correção tem duas partes: o `transform` (que trata **qualquer** entrada) e um teste novo que reproduz a rota, passando `undefined`.

::: regra Entradas de fora são boatos
Tudo que chega de fora do componente (inputs, URL, armazenamento, rede) pode vir vazio, com o tipo errado ou fora da faixa. Trate isso **na porta de entrada**, uma vez, e o resto do código fica simples.
:::

## 3) Testando o tempo sem esperar o tempo

Esperar 60 segundos de verdade num teste é inviável. O Vitest oferece **relógio falso**: `vi.useFakeTimers()` substitui `setTimeout`, `setInterval` e companhia por versões que você avança à mão (`vi.advanceTimersByTimeAsync(1000)`).

Mas há uma armadilha: o Angular sem zone.js **também** usa temporizadores internos para agendar a atualização da tela. Se você falsificar todos, `fixture.whenStable()` espera para sempre, e o teste estoura o tempo. A saída é falsificar **só o que o componente usa**:

<<< src/app/labs/lab01/temporizador.spec.ts

- **`toFake: ['setInterval', 'clearInterval']`** deixa `setTimeout` e companhia reais para o Angular, e entrega o `setInterval` para o teste controlar.
- **`it.each`** roda o mesmo teste para uma tabela de casos: ótimo para funções puras como `formatarTempo` e `duracaoValida`.
- O primeiro teste do componente avança **muito** além do fim (`10_000` ms) e confere que o aviso de término aconteceu **uma** vez: se o intervalo tivesse ficado vivo, ele avisaria de novo.
- O teste de `undefined` reproduz exatamente a situação da rota.
- O último confere que `destroy()` diminui a contagem de temporizadores vivos.

::: erro Falsificar todos os temporizadores num app sem zone.js
`vi.useFakeTimers()` sem opções congela também o agendador do Angular, e `await fixture.whenStable()` nunca termina. Use `toFake` com a lista do que **o seu código** usa.
:::

## 4) Experimente no app

Com `npm start`, abra `http://localhost:4200/pausa` e depois `http://localhost:4200/pausa?duracao=180`. A mesma tela, com três minutos. Tente também `?duracao=abc` e `?duracao=0`: as duas voltam para um minuto, sem quebrar.

::: pratica A pausa, de verdade
Use o temporizador que você acabou de construir. Um minuto, olhos abertos ou fechados, atenção na respiração. Quando a mente sair, e ela vai sair, volte. Depois, repare: o código ficou diferente aos seus olhos?
:::

::: exercicio Som opcional ao terminar
Acrescente um input `comSom` (booleano, padrão `false`). Quando for `true`, o componente toca um som curto ao terminar. Como testar isso sem tocar som nenhum?

--- resposta
O som é um efeito colateral, e efeitos colaterais ficam mais fáceis de testar quando são **injetados**. Crie um token com uma função padrão:

```ts
export const TOCAR_SINO = new InjectionToken<() => void>('TOCAR_SINO', {
  providedIn: 'root',
  factory: () => () => void new Audio('sino.mp3').play().catch(() => {}),
});
```

No componente:

```ts
readonly comSom = input(false);
private readonly tocarSino = inject(TOCAR_SINO);

// dentro de tique(), quando terminar:
if (this.comSom()) this.tocarSino();
```

E no teste, troque a função por um espião:

```ts
const sino = vi.fn();
TestBed.configureTestingModule({ providers: [{ provide: TOCAR_SINO, useValue: sino }] });
// ... setInput('comSom', true), começar, avançar o tempo ...
expect(sino).toHaveBeenCalledTimes(1);
```

Repare no `.catch(() => {})`: navegadores bloqueiam áudio sem interação da pessoa, e um som que não toca não deve derrubar o temporizador.
:::

::: resumo
- Comece pelas **funções puras** (`formatarTempo`, `duracaoValida`): são as mais fáceis de testar.
- **`input(padrão, { transform })`** trata toda entrada na porta: número, texto da URL ou `undefined`.
- Com **`withComponentInputBinding`**, a ausência de um valor na URL chega como `undefined`, **por cima** do padrão do input.
- **`linkedSignal`** é perfeito para um valor que muda sozinho (o tique) e volta ao início quando a fonte muda (a duração).
- **`DestroyRef.onDestroy`** limpa intervalos e assinaturas quando o componente sai da tela.
- Com **relógio falso** do Vitest, falsifique só o que o seu código usa (`toFake`), para não congelar o agendador do Angular.
:::

::: quiz
? Por que a tela mostrava `NaN:NaN` na rota `/pausa`?
+ Porque o roteador entregou `undefined` ao input `duracao`, por cima do padrão 60
- Porque o `setInterval` não funciona em rotas
- Porque o `linkedSignal` não aceita números
- Porque faltou importar o componente
> Com `withComponentInputBinding`, um valor ausente na URL vira `undefined` no input.

? O que faz o `transform` de um input?
- Muda o nome do input
+ Converte (e pode validar) todo valor que chega, antes de o componente usá-lo
- Transforma o input em output
- Anima a mudança de valor
> É a porta de entrada: o resto do componente só vê valores válidos.

? Por que `restantes` é um `linkedSignal` e não um `computed`?
+ Porque precisa ser alterado pelo tique e voltar ao total quando a duração muda
- Porque `computed` não aceita números
- Porque `linkedSignal` é mais rápido
- Porque o template não lê `computed`
> Ajustável à mão e reacomodado pela fonte: é o caso de uso do `linkedSignal`.

? O que acontece se o componente não limpar o intervalo no `DestroyRef`?
- Nada; o navegador limpa sozinho
+ O intervalo continua rodando depois que o componente sai da tela
- O componente não compila
- O teste fica mais rápido
> Vazamentos assim se acumulam e geram comportamentos estranhos.

? Por que usar `toFake: ['setInterval', 'clearInterval']` em vez de `vi.useFakeTimers()` puro?
+ Porque o agendador do Angular sem zone.js também usa temporizadores, e congelá-los trava o `whenStable`
- Porque o Vitest não suporta `setTimeout` falso
- Porque fica mais bonito
- Porque o `setInterval` é mais rápido
> Falsifique só o que o seu código usa.

? Como o teste garante que o aviso de término acontece uma única vez?
- Conferindo o texto do botão
+ Avançando o tempo muito além do fim e conferindo que o output foi emitido só uma vez
- Desligando o relógio
- Não garante
> Se o intervalo tivesse ficado vivo, haveria um segundo aviso.
:::

::: cartas
transform (input) :: Função que converte e valida todo valor que chega a um input, inclusive `undefined`.
withComponentInputBinding :: Faz parâmetros e query params da rota virarem inputs; valor ausente chega como `undefined`.
DestroyRef :: Serviço injetável; `onDestroy` registra uma limpeza para quando o componente sair da tela.
Relógio falso (Vitest) :: `vi.useFakeTimers()` substitui temporizadores por versões controladas pelo teste.
toFake :: Lista dos temporizadores a falsificar; deixe os do Angular reais num app sem zone.js.
it.each :: Roda o mesmo teste para cada linha de uma tabela de casos.
aria-live :: Faz leitores de tela anunciarem mudanças de um trecho ("polite" espera uma pausa).
:::
