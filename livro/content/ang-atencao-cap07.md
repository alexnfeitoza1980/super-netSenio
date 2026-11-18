---
id: ang-atencao-cap07
order: 329
kind: capitulo
num: 231
group: Angular · Parte 58 · As oito semanas
title: "Semana 3 · O rato no labirinto"
subtitle: Procurar o queijo ou fugir da coruja muda o caminho. Rotas, parâmetros como input, carregamento sob demanda, guardas que redirecionam e o resource.
minutes: 70
hue: 270
objectives:
- Entender a diferença entre **aproximar-se** e **evitar**, como o livro descreve na terceira semana
- Configurar **rotas** com `provideRouter`, `redirectTo`, rota coringa e **títulos**
- Carregar telas **sob demanda** com `loadComponent` e `loadChildren`
- Receber **parâmetros** e **query params** como `input` (`withComponentInputBinding`)
- Escrever uma **guarda** que redireciona com gentileza, em vez de só barrar
- Carregar algo assíncrono a partir de signals com **`resource`**
origem: angular/livros/01-atencao-plena-angular/content/08-cap07.md
---

::: cena O link quebrado
Um colega mandou para Caio o link `/capitulos/sessoa`, com um erro de digitação. A tela abriu em branco, sem nenhuma mensagem. "Se eu pedisse uma sala que não existe na recepção", disse Marta, "ninguém me deixaria parada no corredor. Me diriam onde estão as salas."
:::

::: ilustracao img/ang-atencao/cap07.svg | Dois ratinhos no mesmo labirinto: um procura o queijo, o outro foge da coruja. O caminho é o mesmo; o estado de espírito, não.
:::

## O que o livro diz

Os autores contam um experimento conhecido: pessoas resolvem um labirinto de papel para levar um ratinho até a toca. Num grupo, o desenho tem um **queijo** na saída; no outro, uma **coruja** ameaçando o ratinho. O labirinto é igual. Depois, os dois grupos fazem testes de criatividade, e quem fugia da coruja vai pior. A ideia: quando o cérebro está no modo de **evitar**, ele se fecha; no modo de **aproximar-se**, ele se abre.

A terceira semana traz práticas de **movimento atento** (alongar devagar, notando os limites sem forçar) e uma prática curta, o **espaço de respiração de três minutos**, com o formato de uma ampulheta: perceber o que está acontecendo, **estreitar** a atenção para a respiração e **ampliar** de novo para o corpo inteiro. Ela é feita a qualquer hora, sem preparação. O hábito da semana é **assistir a algo escolhido de propósito**, e não o que aparecer.

Nas nossas palavras: **o mesmo caminho, feito para chegar a algum lugar, e não só para fugir de outro, fica diferente.**

## Da vida para o código

| Na vida | No Angular |
|---|---|
| O labirinto | O **mapa de rotas** do app |
| Uma sala que não existe | A **rota coringa** `**`: sempre há um caminho de volta |
| Barrar a porta (evitar) | Uma **guarda** que só devolve `false` |
| Mostrar onde fica a sala certa (aproximar) | Uma guarda que devolve uma **`UrlTree`**: redireciona com um aviso |
| Só abrir a porta quando alguém chega | **`loadComponent`**: a tela é baixada quando alguém a visita |
| Escolher de propósito o que ver | **`resource`**: carregar algo derivado de um signal, com estados claros |

## 1) O mapa do app

<<< src/app/app.routes.ts

Cada objeto é uma rota:

- **`path`**: o pedaço da URL (sem a barra inicial). `''` é a raiz; `capitulos/:id` tem um **parâmetro** `id`.
- **`redirectTo`** com **`pathMatch: 'full'`**: só redireciona se a URL for **exatamente** vazia.
- **`title`**: o título da aba do navegador. Leitores de tela anunciam a troca de título, e isso ajuda a saber que a página mudou.
- **`loadComponent`**: uma função que **importa** o componente quando a rota é visitada. O build gera um arquivo separado para cada um (você viu a lista de *lazy chunks* no `ng build`).
- **`loadChildren`**: o mesmo, para um **grupo** de rotas (o Diário inteiro, no laboratório 2).
- **`**`**: a rota coringa, que pega qualquer URL que não bateu antes. **A ordem importa**: ela fica por último.

E no `app.config.ts` (capítulo 1), o roteador é ligado com duas opções: `withComponentInputBinding()` (parâmetros viram inputs) e `withInMemoryScrolling(...)` (voltar ao topo ao trocar de página).

::: csharp Rotas do ASP.NET × rotas do Angular
`capitulos/:id` é o `"capitulos/{id}"` de um controller. A grande diferença é que tudo acontece **no navegador**: trocar de rota não faz uma requisição de página ao servidor. Por isso, em produção, o servidor precisa devolver o `index.html` para qualquer caminho (a configuração de *fallback* de SPA).
:::

## 2) Um catálogo de demonstrações, carregado sob demanda

As demonstrações dos capítulos ficam num registro: um título, o número do capítulo e uma função que carrega o componente.

<<< src/app/capitulos/cap07/demos.ts

`acharDemo` existe por um motivo sutil: um `id` vem da URL, e qualquer pessoa pode digitar qualquer coisa. `DEMOS['toString']` **existe** (todo objeto JavaScript herda um `toString`), e trataria uma URL maliciosa como uma demonstração. `Object.hasOwn` confere só as chaves do próprio objeto.

## 3) O parâmetro vira input, e o `resource` carrega

<<< src/app/capitulos/cap07/demo-do-capitulo.ts

- **`readonly id = input.required<string>()`**: com `withComponentInputBinding`, o `:id` da rota chega aqui. Mudou a URL, mudou o input.
- **`resource({ params, loader })`** carrega algo **assíncrono** a partir de signals. Sempre que `params` muda, o `loader` roda de novo (e o carregamento anterior é abandonado). Se `params` devolve `undefined`, o resource fica **ocioso**.
- O resource expõe signals de **estado**: `isLoading()`, `error()`, `value()`, além de `reload()`. O template trata **cada** estado, inclusive o erro, com um botão para tentar de novo.
- **`*ngComponentOutlet`** desenha um componente cujo tipo só é conhecido em tempo de execução.

::: novo `resource` é estável no Angular 22
`resource` (e `httpResource`, do capítulo 9) saíram da fase experimental no Angular 22. São a forma recomendada de ligar dados assíncronos a signals sem escrever `effect` nem gerenciar assinaturas.
:::

## 4) A guarda que aproxima

<<< src/app/capitulos/cap07/guardas.ts

Uma guarda é uma **função**. Ela recebe a rota que está sendo ativada e devolve:

- **`true`**: pode entrar;
- **`false`**: barra, e a pessoa fica onde estava (às vezes, numa tela em branco, como na cena);
- uma **`UrlTree`**: **redireciona** para outro lugar.

A nossa redireciona para o catálogo **com um aviso** na *query string* (`?naoAchei=sessoa`). O catálogo recebe esse aviso como input e mostra uma mensagem:

<<< src/app/capitulos/cap07/catalogo.ts

E para qualquer outro caminho perdido, a página coringa sempre oferece um caminho de volta:

<<< src/app/capitulos/cap07/nao-encontrada.ts

::: regra Guardas não são segurança
Uma guarda decide **para onde a pessoa vai na interface**. Ela roda no navegador, e qualquer pessoa pode contorná-la. Dados protegidos precisam ser protegidos **no servidor**, sempre.
:::

## 5) Testando rotas

<<< src/app/capitulos/cap07/rotas.spec.ts

- **`provideRouter([...], withComponentInputBinding())`** monta um roteador de verdade, com as rotas que o teste precisa.
- **`RouterTestingHarness`** navega e dá acesso ao componente da rota (`routeNativeElement`).
- **`vi.waitFor`** repete a conferência até ela passar (ou estourar o tempo): útil quando algo é carregado de forma assíncrona, como o `import()` do resource.
- O teste da guarda confere a **URL final** com `Router.url`: a navegação para `/capitulos/inexistente` termina em `/capitulos?naoAchei=inexistente`.

::: pratica O espaço de respiração, em três passos
Em qualquer lugar, de olhos abertos ou fechados. **Primeiro**, note o que está acontecendo agora: pensamentos, sensações, humor, sem mudar nada. **Depois**, estreite a atenção para a respiração, por algumas respirações. **Por fim**, amplie de novo, para o corpo inteiro, sentado aí. Cerca de um minuto em cada passo, ou menos. Ele é a base do interceptor do capítulo 9.
:::

::: exercicio Um parâmetro de query com padrão
Faça a página coringa mostrar o caminho que a pessoa tentou acessar (por exemplo, *Você tentou abrir /um/lugar/qualquer*). Dica: injete o `Router` e use `router.url`.

--- resposta
```ts
import { Component, inject } from '@angular/core';
import { Router, RouterLink } from '@angular/router';

@Component({
  selector: 'app-nao-encontrada',
  imports: [RouterLink],
  template: `
    <h1>Esta página não existe</h1>
    <p>Você tentou abrir <code>{{ caminho }}</code>.</p>
    <p>Respire. Dá para voltar ao <a routerLink="/diario">Diário</a> ou ao <a routerLink="/capitulos">catálogo</a>.</p>
  `,
})
export class NaoEncontrada {
  protected readonly caminho = inject(Router).url;
}
```

`caminho` é lido uma vez, na criação: a página coringa é recriada a cada navegação, então não precisa ser um signal. A interpolação `{{ }}` escapa o texto, e por isso mostrar uma URL digitada por alguém não abre brecha para injetar HTML.
:::

::: resumo
- O experimento do labirinto sugere que **evitar** fecha a mente e **aproximar-se** a abre. A terceira semana traz movimento atento e o **espaço de respiração de três minutos** (perceber, estreitar, ampliar).
- Rotas são objetos com **`path`**, **`title`**, **`redirectTo`**, **`loadComponent`** / **`loadChildren`**; a coringa **`**`** fica por último.
- **`withComponentInputBinding`** transforma parâmetros e query params em **inputs**.
- **`loadComponent`** gera um arquivo separado por tela: só é baixado quando alguém visita.
- **Guardas** são funções; devolver uma **`UrlTree`** redireciona com gentileza. Guardas **não** são segurança.
- **`resource`** liga um carregamento assíncrono a signals, com estados (`isLoading`, `error`, `value`) e `reload()`.
- Use **`Object.hasOwn`** ao buscar por chaves que vêm da URL.
:::

::: quiz
? O que o experimento do labirinto sugere?
+ Que estar no modo de evitar (fugir da coruja) prejudica a criatividade em comparação com aproximar-se (buscar o queijo)
- Que ratos são mais criativos que pessoas
- Que labirintos difíceis melhoram a memória
- Que o medo sempre ajuda a resolver problemas
> O mesmo labirinto, com estados de espírito diferentes, levou a resultados diferentes.

? Para que serve `pathMatch: 'full'` no redirecionamento da raiz?
+ Para só redirecionar quando a URL for exatamente vazia
- Para redirecionar qualquer URL
- Para carregar a rota mais rápido
- Para exigir login
> Sem ele, `''` bateria como prefixo de todas as URLs.

? O que `loadComponent` muda no build?
- Nada
+ O componente vai para um arquivo separado, baixado só quando a rota é visitada
- O componente passa a ser carregado duas vezes
- O componente deixa de ser testável
> É o carregamento sob demanda (*lazy loading*).

? O que uma guarda deve devolver para redirecionar?
- `false`
- `null`
+ Uma `UrlTree`, como `router.createUrlTree([...])`
- Uma string com a URL
> `false` só barra; a `UrlTree` leva a outro lugar.

? Por que `acharDemo` usa `Object.hasOwn`?
+ Porque `DEMOS['toString']` existiria pela herança de todo objeto, e o id vem da URL
- Porque é mais rápido que `in`
- Porque o TypeScript exige
- Porque `DEMOS` é um `Map`
> Chaves vindas de fora só devem bater com as próprias chaves do objeto.

? O que acontece com um `resource` quando `params` devolve `undefined`?
- Ele lança um erro
- Ele carrega mesmo assim
+ Ele fica ocioso e não chama o `loader`
- Ele recarrega a página
> É o jeito de dizer "ainda não há o que carregar".
:::

::: cartas
provideRouter :: Liga o roteador com a lista de rotas e opções como `withComponentInputBinding()`.
Rota coringa (**) :: Pega qualquer URL que não bateu antes; fica por último.
loadComponent / loadChildren :: Carregam uma tela (ou um grupo de rotas) só quando alguém visita.
withComponentInputBinding :: Faz `:parametros` e query params chegarem como inputs do componente.
Guarda (CanActivateFn) :: Função que decide se a rota abre: `true`, `false` ou uma `UrlTree` (redireciona).
UrlTree :: Representa uma URL do app; devolvida por uma guarda, causa um redirecionamento.
resource :: Carrega algo assíncrono a partir de signals; expõe `value`, `isLoading`, `error` e `reload`.
RouterTestingHarness :: Ferramenta de teste para navegar e inspecionar o componente da rota.
:::
