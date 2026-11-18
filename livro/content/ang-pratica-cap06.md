---
id: ang-pratica-cap06
order: 281
kind: capitulo
num: 191
group: Angular · Parte 47 · Estado e interação
title: Composição e contratos de componentes
subtitle: Componentes se entendem por contratos pequenos, nomeados e testáveis, não por acesso escondido aos detalhes uns dos outros.
minutes: 45
hue: 210
objectives:
- Passar dados para filhos com `input`
- Comunicar eventos com `output`
- Reconhecer `model`, projeção de conteúdo e ControlValueAccessor
origem: angular/livros/01-biblioteca-angular/content/06-cap06.md
---

## Entradas, saídas e estado compartilhado

Um filho recebe dados por `input`. Quando algo acontece dentro dele, comunica intenção por `output`. `model` serve quando o componente realmente precisa de um valor editável sincronizado nos dois sentidos. Não use `model` como atalho para estado global.

No cartão da Biblioteca Viva, `livro` é uma entrada obrigatória. O favorito é estado de produto compartilhado, por isso a ação segue uma store de escopo de aplicação; em um design com componentes mais isolados, o cartão poderia emitir `output` e deixar a página tratar a ação. Ambas as escolhas são válidas se o contrato estiver claro.

## Exemplo de contrato de um controle

```ts
import { Component, input, output } from '@angular/core';

@Component({ selector: 'app-filtro-livros', template: '' })
export class FiltroLivros {
  readonly selecionada = input.required<string>();
  readonly selecionar = output<string>();
}
```

O pai usa `<app-filtro-livros [selecionada]="categoria()" (selecionar)="escolherCategoria($event)" />`. A direção do dado fica visível no template.

`ng-content` projeta conteúdo fornecido pelo pai e é útil para um contêiner reutilizável sem obrigar a API a conhecer o texto interno. `ControlValueAccessor` integra um controle customizado à Forms API. São ferramentas para necessidades específicas, não etapas obrigatórias de toda tela.

## Quando extrair

Um componente compartilhado precisa de propósito, nomes claros, comportamento acessível e uma forma de ser testado. Evite uma API com 20 inputs booleanos. Prefira uma entrada de dados coerente e estados nomeados. Revisores conseguem discutir o contrato sem adivinhar a intenção.

::: quiz
? Qual é a responsabilidade de `output`?
+ Emitir um evento tipado para que o componente pai reaja a uma intenção do filho.
- Guardar automaticamente qualquer objeto no localStorage.
- Substituir o sistema de rotas.
:::

::: cartas
Quando usar `model`? :: Quando o filho precisa expor um valor editável e sincronizado de forma deliberada.
Para que serve `ControlValueAccessor`? :: Para integrar um controle personalizado aos formulários Angular.
:::
