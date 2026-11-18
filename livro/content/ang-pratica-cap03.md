---
id: ang-pratica-cap03
order: 278
kind: capitulo
num: 188
group: Angular · Parte 46 · Começar com clareza
title: Componentes standalone
subtitle: Um componente Angular junta comportamento, template, dependências e estilo de uma parte reconhecível da interface.
minutes: 40
hue: 210
objectives:
- Entender `@Component`, seletor, imports e change detection
- Separar a casca, uma página e um cartão reutilizável
- Usar `input.required` para declarar dados recebidos
origem: angular/livros/01-biblioteca-angular/content/03-cap03.md
---

## A peça de interface

Um componente standalone não precisa ser declarado num `NgModule`. O decorador `@Component` descreve o seletor, os imports usados pelo template, o HTML, os estilos e a estratégia de detecção. No Angular atual, componentes standalone são a forma normal de organizar a aplicação.

A casca `App` mantém navegação global e `RouterOutlet`; a página de catálogo organiza busca e filtros; `LivroCard` desenha um livro. Esse limite evita tanto o componente monolítico quanto a criação de um componente por cada elemento sem significado próprio.

<<< src/app/livro-card.ts

## Dados de entrada

`input.required<Livro>()` deixa explícito que o cartão precisa receber um livro. O componente não tem que descobrir o dado por conta própria nem acessar a rota. Ele exibe o item e encaminha a intenção de alternar favorito à store compartilhada.

O `@for (...; track livro.id)` usa uma chave estável. Se a lista for filtrada, Angular associa cada item pelo identificador em vez de tratar a posição como identidade. Isso melhora a previsibilidade e evita recriar DOM desnecessariamente.

## Como decidir se algo merece ser componente

Extraia uma unidade quando ela tem responsabilidade reconhecível, regras próprias, reutilização real ou precisa ser testada isoladamente. Não extraia só para reduzir linhas. O cartão tem visual e interação consistentes, por isso é uma boa fronteira. Um `span` que só mostra uma palavra provavelmente não é.

::: pratica Faça o caminho da tela
Comece em `App`, siga o `RouterOutlet` até `CatalogoPage` e depois veja onde `LivroCard` é criado. Desenhe três caixas e uma seta entre elas. Esse é um mapa de dependências muito mais útil que abrir a pasta inteira sem propósito.
:::

::: quiz
? O que significa dizer que este componente é standalone?
+ Ele declara as próprias dependências no componente e não precisa ser declarado num `NgModule`.
- Ele não pode importar outras diretivas ou componentes.
- Ele só pode aparecer uma vez na aplicação.
:::

::: cartas
Por que usar `track livro.id`? :: Para dar ao Angular uma identidade estável para cada item da coleção.
Quando vale extrair um componente? :: Quando há uma responsabilidade de interface clara, reutilização significativa ou necessidade de isolamento e teste.
:::
