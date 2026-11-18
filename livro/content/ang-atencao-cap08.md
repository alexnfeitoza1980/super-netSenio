---
id: ang-atencao-cap08
order: 330
kind: capitulo
num: 232
group: Angular · Parte 58 · As oito semanas
title: "Semana 4 · Pensamentos são eventos"
subtitle: Sons chegam e passam; pensamentos também. Serviços, injeção de dependência, RxJS e a ponte entre Observables e signals.
minutes: 80
hue: 250
objectives:
- Entender a ideia da quarta semana: **pensamentos são eventos mentais**, não fatos
- Criar **serviços** com `@Injectable` e pedir dependências com **`inject()`**
- Injetar o que não é classe com **`InjectionToken`**, e trocar dependências nos testes
- Pensar em **fluxos de eventos** com **RxJS**: `Subject`, `map`, `scan`, `debounceTime`, `distinctUntilChanged`, `switchMap`
- Converter entre Observables e signals com **`toSignal`** e **`toObservable`**
- Testar fluxos com **diagramas de bolinhas** (`TestScheduler`)
origem: angular/livros/01-atencao-plena-angular/content/09-cap08.md
---

::: cena A fila frustrante
Caio fez uma busca que consultava o servidor a cada tecla. Beto digitou "respiração" rápido: dez requisições saíram, e as respostas chegaram fora de ordem. A tela mostrou o resultado de "res" **depois** do de "respiração". "É como mudar de fila no mercado a cada segundo", disse Lia, "e ficar com a fila errada no fim."
:::

::: ilustracao img/ang-atencao/cap08.svg | Pensamentos passando como nuvens com etiquetas: planejando, lembrando, preocupando. Quem observa não precisa embarcar em nenhuma.
:::

## O que o livro diz

A quarta semana propõe uma prática com **sons e pensamentos**: primeiro, ouvir os sons ao redor como sons (altura, duração, começo e fim), sem dar nome nem julgar; depois, olhar para os pensamentos **do mesmo jeito**, como eventos que surgem, ficam um pouco e vão embora. Os autores usam imagens como a de estar num cinema, vendo o filme na tela em vez de estar dentro dele.

A ideia central é que **pensamentos não são fatos**. São eventos da mente, às vezes úteis, às vezes **boatos**. O livro traz ainda a meditação da **fila frustrante** (perceber o que acontece na mente e no corpo numa fila demorada) e um hábito pequeno: ir ao cinema e **escolher o filme só ao chegar**.

Nas nossas palavras: **um pensamento é algo que acontece, e não uma ordem que precisamos obedecer.**

## Da vida para o código

| Na vida | No Angular |
|---|---|
| Pensamentos surgem e passam | Um **Observable**: valores que chegam ao longo do tempo |
| Dar um nome ao tipo de pensamento | **`map`**: transformar cada evento ao passar |
| Esperar o pensamento assentar | **`debounceTime`**: esperar a pessoa parar de digitar |
| Não se agarrar ao que já mudou | **`switchMap`**: cancelar a busca antiga quando chega uma nova |
| Um observador que está sempre ali | Um **serviço** com uma instância para o app todo |
| Boatos que parecem fatos | Dependências trocadas nos testes: **injeção de dependência** |

## 1) Serviços e `inject()`

Um **serviço** é uma classe com uma responsabilidade que não é desenhar tela: guardar estado compartilhado, falar com o servidor, calcular regras. O decorador **`@Injectable({ providedIn: 'root' })`** diz ao Angular: "crie **uma** instância desta classe para o app inteiro, na primeira vez que alguém pedir".

<<< src/app/capitulos/cap08/fluxo-de-pensamentos.ts

Quem precisa do serviço **pede** com **`inject(FluxoDePensamentos)`**, num campo ou no construtor. Ninguém faz `new FluxoDePensamentos()`: o Angular cria, guarda e entrega. Isso é **injeção de dependência**: as peças recebem o que precisam, em vez de construí-lo.

::: csharp É o mesmo DI do ASP.NET Core
`providedIn: 'root'` equivale a `services.AddSingleton<FluxoDePensamentos>()`, e `inject(X)` faz o papel do parâmetro de construtor. O Angular também tem escopos menores: um serviço listado em `providers` de um componente ganha uma instância por componente, parecido com um `Scoped`.
:::

### `InjectionToken`: injetando o que não é classe

Às vezes a dependência é um número, uma função, uma configuração. Para isso existe o **`InjectionToken`**, um "nome" injetável com uma fábrica de valor padrão:

<<< src/app/capitulos/cap08/tokens.ts

`RELOGIO` é uma função que diz que horas são. Parece exagero, mas é o que permite testar código que depende do tempo (o capítulo 12 volta a isso). O serviço acima pede `inject(RELOGIO)` em vez de chamar `new Date()` direto.

## 2) Observables: valores ao longo do tempo

Um **signal** tem sempre **um** valor atual. Um **Observable** (da biblioteca **RxJS**) é um **fluxo**: zero, um ou muitos valores chegando ao longo do tempo, e talvez um erro ou um fim. Cliques, teclas e respostas de rede são fluxos naturais.

No serviço:

- **`Subject`** é um Observable em que **você** empurra valores com `next()`. `notar(texto)` faz isso.
- **`pipe(...)`** encadeia **operadores**: funções que transformam o fluxo.
- **`map`** transforma cada valor (aqui: tira espaços e dá um rótulo).
- **`scan`** acumula, como um `reduce` que emite a cada passo (aqui: os cinco mais recentes).
- **`shareReplay({ bufferSize: 1 })`** compartilha o fluxo entre todos que o escutam e entrega o **último** valor a quem chegar depois.

O rótulo vem de uma função pura simples, de propósito:

<<< src/app/capitulos/cap08/rotulos.ts

::: cuidado Um brinquedo, não um diagnóstico
`classificar` procura palavras-chave. Ela serve para treinar RxJS, e só. Programas não leem mentes, e nenhum código deste livro rotula pessoas.
:::

## 3) A fila frustrante, resolvida

A busca da cena tinha dois problemas: uma requisição por tecla, e respostas antigas sobrescrevendo as novas. A solução é um fluxo de quatro operadores:

<<< src/app/capitulos/cap08/sugestoes.ts#busca

| Operador | O que faz | Na cena |
|---|---|---|
| `debounceTime(calma)` | Só deixa passar um valor depois de `calma` ms **sem** valor novo | Espera a pessoa parar de digitar |
| `distinctUntilChanged()` | Ignora um valor igual ao anterior | "resp" duas vezes não busca duas vezes |
| `switchMap(...)` | Para cada termo, começa uma busca **e cancela a anterior** | A resposta de "res" nunca chega depois da de "respiração" |
| `of([])` | Um Observable que emite um valor e termina | Termo curto demais: lista vazia, sem ir ao servidor |

O `agendador` opcional existe para os testes, como você vai ver.

::: dica Qual "map" escolher?
`switchMap` cancela o anterior (buscas, navegação). `concatMap` enfileira (gravações que precisam de ordem). `mergeMap` roda tudo em paralelo (independentes). `exhaustMap` ignora os novos enquanto um está em andamento (o clique repetido no botão "Enviar").
:::

O "servidor" das sugestões é uma **classe abstrata** usada como token, com uma implementação local padrão:

<<< src/app/capitulos/cap08/sugestoes.ts#fonte

`@Injectable({ providedIn: 'root', useFactory: ... })` numa classe abstrata é um truque útil: o **tipo** e o **token** são a mesma coisa, e o padrão já vem configurado. Nos testes, basta `{ provide: FonteDeSugestoes, useValue: falso }`.

## 4) Observables ↔ signals

O componente junta tudo:

<<< src/app/capitulos/cap08/observador.ts

- **`toSignal(observable, { initialValue })`** assina o Observable e expõe o último valor como signal. A assinatura é desfeita sozinha quando o componente sai da tela.
- **`toObservable(signal)`** faz o caminho inverso: emite sempre que o signal muda. Aqui, o termo digitado (um signal) vira o fluxo de entrada da `buscaCalma`.
- O template lê só signals (`recentes()`, `sugestoes()`), como em todo o livro.

::: regra Signals para estado, RxJS para eventos no tempo
Use **signals** para o que **é** (o termo atual, a lista atual). Use **RxJS** quando o **tempo** importa (esperar, cancelar, combinar fluxos). E converta na fronteira com `toSignal`/`toObservable`, para que o template fale só com signals.
:::

## 5) Testando fluxos com bolinhas

<<< src/app/capitulos/cap08/pensamentos.spec.ts

Destaques:

- **`TestBed.configureTestingModule({ providers: [...] })`** troca dependências: um relógio fixo, uma fonte de sugestões falsa, uma calma de 0 ms.
- **`TestScheduler`** roda o tempo de mentira e entende **diagramas de bolinhas** (*marble diagrams*): em `'a 100ms b 100ms b'`, cada letra é um valor, e `100ms` é tempo passando. O teste diz, num desenho, "o termo curto vira lista vazia aos 50 ms; o termo bom vira resultado 100 ms depois; a repetição é ignorada".
- O segundo teste de bolinhas prova o `switchMap`: a busca lenta por "sons" é **cancelada** quando "corpo" chega, e só "rápida" aparece.

::: pratica Sons, depois pensamentos
Por um minuto, só ouça: sons perto, sons longe, o começo e o fim de cada um, sem dar nome. No minuto seguinte, faça o mesmo com os pensamentos: note quando um chega, quanto tempo fica, quando vai. Se der, dê só um rótulo curto: "planejando", "lembrando", "preocupando". Não precisa seguir nenhum.
:::

::: exercicio Botão que não aceita clique repetido
Um botão **Salvar** dispara uma gravação que leva um segundo. Pessoas impacientes clicam três vezes. Usando um `Subject` de cliques e o operador certo, garanta que só **uma** gravação aconteça enquanto a anterior não terminar. Qual operador? Escreva o fluxo.

--- resposta
O operador é o **`exhaustMap`**: enquanto a gravação atual não termina, ele **ignora** os novos cliques.

```ts
private readonly cliques = new Subject<void>();

readonly gravacoes$ = this.cliques.pipe(
  exhaustMap(() => this.servico.gravar(this.dados())),
);

salvar(): void {
  this.cliques.next();
}
```

Com `switchMap`, o segundo clique **cancelaria** a primeira gravação (ruim: pode deixar dado pela metade). Com `concatMap`, os três cliques virariam três gravações em fila. Com `mergeMap`, três gravações em paralelo. Só o `exhaustMap` faz "uma de cada vez, e ignore a insistência".
:::

::: resumo
- A quarta semana trata **pensamentos como eventos**: surgem, ficam, passam. Não são fatos.
- **Serviços** (`@Injectable({ providedIn: 'root' })`) guardam regras e estado compartilhado; quem precisa usa **`inject()`**.
- **`InjectionToken`** injeta o que não é classe (números, funções, configurações) e permite trocar dependências nos testes.
- **Observables** são fluxos de valores no tempo; **operadores** (`map`, `scan`, `debounceTime`, `distinctUntilChanged`, `switchMap`) transformam o fluxo.
- **`switchMap`** cancela a operação anterior; **`exhaustMap`** ignora novas enquanto uma roda; **`concatMap`** enfileira; **`mergeMap`** paraleliza.
- **`toSignal`** e **`toObservable`** fazem a ponte; o template fala só com signals.
- **`TestScheduler`** testa o tempo com diagramas de bolinhas, sem esperar de verdade.
:::

::: quiz
? Qual é a ideia central da quarta semana?
+ Pensamentos são eventos mentais que surgem e passam, e não fatos
- Pensamentos devem ser eliminados
- Só os pensamentos positivos são verdadeiros
- Sons atrapalham a meditação
> Observar sons e pensamentos do mesmo jeito ajuda a não embarcar em cada um.

? O que `providedIn: 'root'` faz?
+ O Angular cria uma única instância do serviço para o app inteiro, quando alguém a pede
- O serviço é criado a cada uso
- O serviço só funciona no componente raiz
- O serviço vira um componente
> É o equivalente a um singleton no DI do ASP.NET Core.

? Para que serve `debounceTime(300)`?
- Atrasar todos os valores em 300 ms
+ Deixar passar um valor só depois de 300 ms sem valor novo
- Limitar a 300 valores
- Cancelar o fluxo depois de 300 ms
> É o "esperar a pessoa parar de digitar".

? Na busca, por que `switchMap` e não `mergeMap`?
+ Porque `switchMap` cancela a busca anterior, evitando que uma resposta antiga sobrescreva a nova
- Porque `mergeMap` não existe
- Porque `switchMap` é mais rápido
- Porque `mergeMap` não aceita Observables
> Com `mergeMap`, as respostas chegariam fora de ordem, como na cena.

? O que `toSignal` faz?
+ Assina um Observable e expõe o último valor como signal, desfazendo a assinatura quando o componente sai da tela
- Converte um signal em Observable
- Cria um Subject
- Testa um Observable
> É a ponte do mundo dos fluxos para o mundo dos signals.

? Em `cold('a 100ms b')`, o que `100ms` representa?
- Uma letra
+ Tempo passando no diagrama, entre um valor e outro
- O valor do evento
- Um erro
> Diagramas de bolinhas desenham o tempo; o `TestScheduler` o simula.
:::

::: cartas
Serviço :: Classe com uma responsabilidade que não é tela; `@Injectable({ providedIn: 'root' })` cria uma instância para o app.
inject() :: Pede uma dependência ao Angular, num campo ou no construtor.
InjectionToken :: Token para injetar o que não é classe, com uma fábrica de valor padrão.
Observable :: Fluxo de valores ao longo do tempo (RxJS); pode emitir muitos valores, um erro ou terminar.
Subject :: Observable em que você empurra valores com `next()`.
switchMap :: Para cada valor, inicia um Observable novo e cancela o anterior.
exhaustMap :: Ignora valores novos enquanto o Observable atual não termina.
toSignal / toObservable :: Pontes entre Observables e signals.
:::
