---
id: ang-pratica-cap02
order: 277
kind: capitulo
num: 187
group: Angular · Parte 46 · Começar com clareza
title: TypeScript para modelar o domínio
subtitle: Tipos úteis descrevem as regras dos dados e fazem erros aparecerem antes da tela chegar a uma pessoa.
minutes: 45
hue: 210
objectives:
- Criar interfaces e propriedades somente de leitura
- Usar tipos primitivos, união, narrowing, funções e genéricos
- Entender strict mode e validar dados vindos do armazenamento
origem: angular/livros/01-biblioteca-angular/content/02-cap02.md
---

## O formato de um livro

Antes de desenhar a página, modele o que é um livro no nosso catálogo. Propriedades `readonly` deixam claro que um cartão não deve alterar o catálogo por conta própria:

<<< src/app/livro.ts

`readonly` protege contra reatribuição no código TypeScript, mas não congela objetos em runtime e não valida JSON. Tipos são uma ferramenta de desenvolvimento; dados externos ainda precisam de validação.

## Tipos que ajudam a pensar

- `string`, `number`, `boolean` e `null` representam valores básicos.
- `Livro | undefined` diz que a busca pode não encontrar uma correspondência.
- `unknown` é adequado para JSON não validado. Converta após checar o formato.
- Funções tipadas documentam entradas e saídas.
- `readonly Livro[]` comunica que a lista é uma visão, não uma coleção para mutação casual.

Um identificador controlado pode ser modelado como união literal: `type Categoria = 'Ficção' | 'Infantil' | 'Tecnologia'`. Uma lista vinda de um servidor, porém, pode ganhar novas categorias. A validação deve considerar isso.

::: csharp TypeScript e C#
Interfaces ajudam a declarar a forma de um valor como interfaces e records ajudam no C#. O sistema de tipos TypeScript é estrutural: duas formas compatíveis podem ser usadas juntas, mesmo sem declarar a mesma interface nominal.
:::

## Um limite de confiança no armazenamento

O `localStorage` é persistência simples, não uma fronteira confiável. Pode conter conteúdo antigo, inválido ou manipulado. A store do projeto lê JSON como `unknown`, valida campos, confere IDs existentes e limita progresso. Os erros de leitura/gravação viram um aviso visível em vez de um falso “salvo com sucesso”:

<<< src/app/biblioteca.store.ts

O narrowing ocorre quando o código testa uma condição: depois de verificar `evento.target instanceof HTMLInputElement`, o TypeScript permite acessar `value`. Evite `any`: ele desliga essas proteções e espalha incerteza pelo app.

::: dica Ver um erro de tipo é progresso
Quando o compilador aponta uma propriedade inexistente, ele não está dizendo que você não sabe programar. Está mostrando que o modelo e o uso ainda não combinam. Compare os dois, escolha qual expressa a regra correta e rode a verificação de novo.
:::

## Exercício

Inclua uma propriedade `idioma` no tipo `Livro` e em cada item do catálogo. Depois mostre-a nos detalhes. Não resolva o primeiro erro com `as any`; siga a mensagem até o dado de origem.

::: quiz
? Por que o JSON lido do navegador começa como `unknown`?
+ Porque a origem pode conter qualquer valor e precisa ser validada antes de ser tratada como um estado confiável.
- Porque `unknown` permite acessar qualquer propriedade sem checagem.
- Porque o TypeScript executa validação automática de JSON em runtime.
:::

::: cartas
`Livro | undefined` comunica o quê? :: A busca pode não encontrar um livro; o consumidor precisa tratar esse caso.
`readonly` valida um objeto em runtime? :: Não. Ele restringe reatribuições verificadas pelo TypeScript, mas não valida dados executados.
:::
