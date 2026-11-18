---
id: ang-atencao-cap11
order: 333
kind: capitulo
num: 235
group: Angular · Parte 58 · As oito semanas
title: "Semana 7 · O que nutre e o que desgasta"
subtitle: Quando você parou de dançar? Equilibrar o dia, equilibrar o app. Desempenho sem sofrimento com computed, track, pipes puros e @defer.
minutes: 70
hue: 160
objectives:
- Entender o **funil da exaustão** e a proposta de reequilibrar atividades que **nutrem** e que **desgastam**
- Saber **onde** o desempenho de um app Angular costuma se perder
- Usar **`computed`** para não recalcular à toa, e **`track`** para não recriar o DOM à toa
- Escrever um **pipe puro** e entender por que ele é melhor que uma função no template
- Adiar o que pode esperar com **`@defer`** e testar os estados do bloco
- Medir antes de otimizar (**budgets**, **Angular DevTools**)
origem: angular/livros/01-atencao-plena-angular/content/12-cap11.md
---

::: cena A tela que demorava
A lista de atividades da equipe demorava para responder a cada clique. Caio abriu o *profiler*: a cada clique, o Angular recriava a lista inteira, recalculava totais em três lugares e carregava um gráfico pesado que estava fora da tela. "Não é que o app seja lento", disse Lia. "É que ele está gastando energia com o que não precisa agora."
:::

::: ilustracao img/ang-atencao/cap11.svg | Uma balança: de um lado, o que nutre; do outro, o que desgasta. Ninguém zera um dos lados; dá para equilibrar.
:::

## O que o livro diz

A sétima semana começa com uma pergunta: **quando você parou de dançar?** Os autores descrevem o **funil da exaustão**: quando a vida aperta, a gente começa a cortar as atividades que parecem "opcionais" (o hobby, o encontro com amigos, a caminhada) para dar conta do que é "importante". O círculo da vida vai se estreitando, e sobram justamente as atividades que **desgastam**.

A proposta é olhar para um dia comum e separar o que **nutre** do que **desgasta**, e então fazer pequenos ajustes: acrescentar uma atividade que nutre, fazer de outro jeito uma que desgasta. Os autores insistem em ações **específicas e pequenas** (e não planos grandiosos) e em agir **mesmo sem vontade**, porque a motivação muitas vezes vem depois da ação, e não antes.

Nas nossas palavras: **não dá para tirar tudo o que cansa, mas dá para cuidar do equilíbrio, de propósito.**

## Da vida para o código

| Na vida | No Angular |
|---|---|
| O funil da exaustão: gastar energia em tudo, o tempo todo | Recalcular e recriar tudo a cada mudança |
| Separar o que nutre do que desgasta | **Medir**: o que realmente custa (profiler, budgets) |
| Não refazer o que não mudou | **`computed`** (memória) e **`track`** (reaproveitar o DOM) |
| Um jeito mais leve de fazer a mesma coisa | **Pipe puro** em vez de função no template |
| Deixar para depois o que pode esperar | **`@defer`**: carregar só quando aparecer |
| Ações pequenas e específicas | Uma otimização por vez, com teste |

## 1) Os dados e o cálculo

As atividades e o resumo do dia:

<<< src/app/capitulos/cap11/atividades.ts

`resumir` é uma função pura: soma os minutos por efeito e calcula a proporção do que nutre **entre o que nutre e o que desgasta** (o neutro fica fora da conta). Lista vazia devolve proporção 0, sem dividir por zero.

## 2) O componente

<<< src/app/capitulos/cap11/balanco.ts

Quatro decisões de desempenho, uma em cada linha importante:

### `computed`: calcular uma vez

`resumo = computed(() => resumir(this.atividades()))`. O template lê `resumo()` **quatro** vezes (nutre, desgasta, o `@if` e o input do gráfico), e o cálculo roda **uma**, até `atividades` mudar. Sem o `computed`, seria uma chamada por leitura, a cada verificação.

### `track a.id`: reaproveitar o DOM

`classificar` cria uma **lista nova** com **um** item novo (os outros são os mesmos objetos). Com `track a.id`, o Angular reconhece cada item pelo `id` e só mexe no `<li>` que mudou. O teste prova isso, conferindo que o primeiro `<li>` é **o mesmo elemento** antes e depois.

::: erro `track` com um objeto recriado
`track a` (o próprio objeto) funciona enquanto os objetos são os mesmos. Mas quando a lista vem do servidor de novo, todos os objetos são novos, e o Angular recria tudo. Use um **identificador estável** (`a.id`).
:::

### Pipe puro: formatar sem recalcular

<<< src/app/capitulos/cap11/minutos.pipe.ts

`{{ a.minutos | minutos }}` passa o valor pelo pipe. Pipes são **puros** por padrão: o Angular só chama `transform` de novo quando a **entrada** muda. Uma função no template (`{{ formatar(a.minutos) }}`) rodaria em toda verificação daquele componente, mesmo sem mudança nenhuma.

::: dica Pipe ou computed?
Use **pipe** para formatações reutilizáveis em muitos templates (datas, moedas, minutos). Use **`computed`** para valores derivados do estado de **um** componente ou serviço. Os dois guardam memória do último resultado.
:::

### `@defer`: o que pode esperar, espera

```html
@defer (on viewport) {
  <app-grafico-do-dia [resumo]="resumo()" />
} @placeholder {
  <p>O gráfico aparece quando você chegar aqui.</p>
}
```

Tudo o que está dentro do `@defer` (o componente `GraficoDoDia` e o código dele) vai para um **arquivo separado**, baixado só quando o **gatilho** acontece. Até lá, aparece o `@placeholder`.

| Gatilho | Quando carrega |
|---|---|
| `on viewport` | Quando o placeholder entra na área visível da tela |
| `on idle` (padrão) | Quando o navegador fica ocioso |
| `on interaction` | No primeiro clique ou tecla no placeholder |
| `on hover` | Quando o mouse passa por cima |
| `on timer(2s)` | Depois de um tempo |
| `when condicao()` | Quando uma expressão vira verdadeira |

Também existem `@loading` (enquanto baixa) e `@error` (se falhar), e `prefetch on ...` para baixar antes e mostrar depois.

<<< src/app/capitulos/cap11/grafico-do-dia.ts

## 3) Testando, inclusive o `@defer`

<<< src/app/capitulos/cap11/desempenho.spec.ts

- **`deferBlockBehavior: DeferBlockBehavior.Manual`** faz o `@defer` esperar o teste mandar.
- **`fixture.getDeferBlocks()`** devolve os blocos; **`bloco.render(DeferBlockState.Complete)`** mostra o conteúdo final (também dá para testar `Placeholder`, `Loading` e `Error`).
- O teste de `track` confere **identidade** de elementos (`toBe`), e não só o texto.

## 4) Medir antes de otimizar

- **Budgets**: o `angular.json` define limites de tamanho do pacote inicial (aviso em 500 kB, erro em 1 MB) e dos estilos de cada componente. O `ng build` falha se alguém passar do limite.
- **`ng build`** lista os arquivos gerados: o **pacote inicial** e os **lazy chunks** (rotas com `loadComponent` e blocos `@defer`).
- **Angular DevTools → Profiler**: mostra cada passada de detecção de mudanças e o custo de cada componente.

::: regra Uma otimização por vez
Meça, mude **uma** coisa, meça de novo. Otimizações sem medida costumam deixar o código mais difícil sem deixar o app mais rápido. Com signals, OnPush e `track`, a maior parte dos apps Angular já nasce rápida.
:::

::: pratica O balanço do dia
Liste cinco coisas que você fez hoje. Ao lado de cada uma, escreva **N** (nutre), **D** (desgasta) ou nada. Não é para julgar o dia. Depois, escolha **uma** ação pequena e específica para amanhã: cinco minutos de algo que nutre, ou um jeito diferente de fazer algo que desgasta.
:::

::: exercicio Carregar o gráfico só quando pedirem
Troque o gatilho do `@defer` para que o gráfico só seja carregado quando a pessoa clicar num botão **Ver gráfico**, e mostre "Carregando…" durante o download. Como ficaria o teste?

--- resposta
```html
@defer (on interaction(verGrafico)) {
  <app-grafico-do-dia [resumo]="resumo()" />
} @placeholder {
  <button #verGrafico type="button">Ver gráfico</button>
} @loading (minimum 300ms) {
  <p aria-live="polite">Carregando…</p>
}
```

`on interaction(verGrafico)` usa a **variável de template** do botão como gatilho. O `minimum 300ms` evita um piscar rápido do "Carregando…".

No teste, com `DeferBlockBehavior.Manual`, dá para conferir cada estado:

```ts
const [bloco] = await fixture.getDeferBlocks();
await bloco.render(DeferBlockState.Loading);
expect(tela.textContent).toContain('Carregando…');
await bloco.render(DeferBlockState.Complete);
expect(tela.querySelector('svg')).toBeTruthy();
```
:::

::: resumo
- A sétima semana descreve o **funil da exaustão** e propõe reequilibrar o que **nutre** e o que **desgasta**, com ações pequenas e específicas, mesmo sem vontade.
- **`computed`** calcula uma vez e reaproveita até uma fonte mudar.
- **`track`** com um **id estável** faz o `@for` reaproveitar os elementos que não mudaram.
- **Pipes** são puros por padrão: só recalculam quando a entrada muda; funções no template rodam a cada verificação.
- **`@defer`** separa um trecho em outro arquivo e o carrega num **gatilho** (`viewport`, `idle`, `interaction`...), com `@placeholder`, `@loading` e `@error`.
- Nos testes, **`DeferBlockBehavior.Manual`** + **`getDeferBlocks()`** controlam os estados do bloco.
- **Meça antes**: budgets, saída do `ng build` e o profiler do Angular DevTools.
:::

::: quiz
? O que o livro chama de "funil da exaustão"?
+ O estreitamento da vida quando cortamos atividades que nutrem para dar conta das que desgastam
- Um tipo de meditação
- Um exercício físico
- Uma técnica de produtividade
> Sobram justamente as atividades que cansam.

? Por que usar `computed` para o resumo, e não uma função chamada no template?
+ Porque o `computed` calcula uma vez e reaproveita até as atividades mudarem
- Porque funções não podem ser chamadas no template
- Porque `computed` é assíncrono
- Porque o pipe exige
> O template lê o resumo quatro vezes; o cálculo roda uma.

? Para que serve `track a.id` no `@for`?
+ Para o Angular reconhecer cada item e reaproveitar os elementos que não mudaram
- Para ordenar por id
- Para filtrar itens repetidos
- Para numerar a lista
> O teste confere que o `<li>` é o mesmo elemento depois da mudança.

? O que significa um pipe ser "puro"?
- Que ele não tem dependências
+ Que o Angular só o executa de novo quando a entrada muda
- Que ele não pode receber argumentos
- Que ele roda no servidor
> É uma forma de memória automática.

? O que acontece com o conteúdo de um `@defer (on viewport)`?
+ Vai para um arquivo separado e só é carregado quando o placeholder aparece na tela
- É carregado primeiro
- Nunca é carregado
- É carregado a cada rolagem
> Até lá, aparece o `@placeholder`.

? Qual é a primeira coisa a fazer antes de otimizar?
- Trocar tudo por `@defer`
- Desligar o OnPush
+ Medir onde está o custo
- Reescrever em outro framework
> Uma otimização por vez, medindo antes e depois.
:::

::: cartas
Funil da exaustão :: Cortar o que nutre para dar conta do resto, até sobrarem só atividades que desgastam.
computed (desempenho) :: Valor derivado com memória: calcula uma vez e reaproveita até uma fonte mudar.
track por id :: Faz o `@for` reconhecer cada item e reaproveitar os elementos que não mudaram.
Pipe puro :: Transformação no template recalculada só quando a entrada muda (`{{ x | minutos }}`).
@defer :: Bloco carregado depois, num gatilho; gera um arquivo separado.
Gatilhos do @defer :: `on viewport`, `on idle`, `on interaction`, `on hover`, `on timer`, `when`.
DeferBlockBehavior.Manual :: Nos testes, faz o `@defer` esperar `render(estado)`.
Budgets :: Limites de tamanho no `angular.json`; o build avisa ou falha quando são ultrapassados.
:::
