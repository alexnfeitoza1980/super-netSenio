---
id: ang-pratica-cap04
order: 279
kind: capitulo
num: 189
group: Angular · Parte 46 · Começar com clareza
title: Templates e fluxo de controle
subtitle: Templates declarativos conectam dados e eventos sem esconder a lógica em manipulação manual do DOM.
minutes: 50
hue: 210
objectives:
- Reconhecer interpolação, property binding, event binding e attribute binding
- Usar `@if`, `@for`, `@switch` e `@empty`
- Criar conteúdo dinâmico que também faça sentido com teclado e leitor de tela
origem: angular/livros/01-biblioteca-angular/content/04-cap04.md
---

## Quatro formas de ligação

Interpolação (`{{ livro.titulo }}`) escreve texto. Property binding (`[value]="busca()"`) passa um valor a uma propriedade DOM ou input. Event binding (`(click)="salvar()"`) recebe uma ação. Attribute binding (`[attr.aria-label]="rotulo"`) configura um atributo HTML quando a propriedade correspondente não representa a necessidade.

Angular escapa interpolação como texto. Não use `[innerHTML]` para conteúdo sem sanitização e sem uma necessidade real. Para títulos e descrições do catálogo, interpolação é exatamente o que queremos.

## Controle de fluxo atual

`@if` escolhe estados alternativos; `@for` percorre listas e exige `track`; `@switch` seleciona um estado discriminado. O `@empty` pode exibir uma alternativa para lista vazia. No catálogo, o bloco de vazio dá orientação e uma ação de recuperação, em vez de deixar um espaço em branco.

<<< src/app/catalogo-page.ts

Uma busca sem resultados é um estado normal, não um erro. A mensagem informa o que aconteceu e oferece um próximo passo. Para um erro real de servidor, a página precisaria diferenciar falha de rede de “não há livros nesta categoria”.

## Eventos com tipos

O método `buscar(evento: Event)` verifica se `target` é um `HTMLInputElement` antes de ler o valor. Isso é narrowing do TypeScript. O cast genérico `$any($event.target)` esconderia a ausência de verificação.

Uma lista filtrada deve anunciar uma mudança importante sem falar a cada tecla de modo intrusivo. `aria-live="polite"` no contador permite que tecnologia assistiva perceba o novo total. Labels visíveis dão nome ao campo.

::: csharp Atributo e propriedade
Em HTML, `disabled` é um atributo booleano; no template, `[disabled]="ocupado()"` liga a propriedade ao estado atual. Entender a ligação evita construir atributos por concatenação de texto ou escrever diretamente no DOM.
:::

::: quiz
? Qual binding é o mais adequado para atender ao clique em um botão?
+ `(click)="alternarFavorito()"`
- `{{ click = alternarFavorito() }}`
- `innerHTML="alternarFavorito()"`
:::

::: cartas
Para que serve `@for (...; track livro.id)`? :: Percorrer itens e preservar sua identidade entre atualizações.
Quando usar `[attr.aria-label]`? :: Quando é necessário configurar um atributo ARIA a partir do estado, por exemplo dar nome acessível a um botão de ícone.
:::
