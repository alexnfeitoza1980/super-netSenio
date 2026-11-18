---
id: ang-pratica-cap01
order: 276
kind: capitulo
num: 186
group: Angular · Parte 46 · Começar com clareza
title: O projeto e as ferramentas
subtitle: Antes de escrever código, descubra o que a aplicação promete e como verificar que ela cumpre.
minutes: 35
hue: 210
objectives:
- Identificar o problema e os limites da primeira versão
- Instalar e usar Node.js, npm e Angular CLI
- Ler a estrutura de um workspace sem tentar decorar cada arquivo
origem: angular/livros/01-biblioteca-angular/content/01-cap01.md
---

::: ilustracao img/ang-pratica/mapa-do-livro.svg | Um caminho dividido em etapas pequenas torna o projeto menos opaco.
:::

## Primeiro, entender o pedido

“Quero uma biblioteca digital” não define ainda o comportamento. Uma pessoa pode querer encontrar um livro; outra, retomar a leitura; a equipe pode precisar atualizar dados por uma API. Comece separando **necessidade**, **comportamento** e **decisão de implementação**.

| Necessidade observada | Comportamento verificável | Decisão desta versão |
|---|---|---|
| Encontrar um assunto | busca procura título, autoria, categoria e descrição | catálogo local e busca no navegador |
| Guardar um título | pessoa ativa ou remove favorito | estado do app com persistência local |
| Voltar a um livro | página mostra progresso ajustável | percentagem salva no `localStorage` |

Isso não transforma a aplicação em produto de produção. Também não antecipa login, sincronização ou pagamentos sem evidência de que sejam necessários. Decisões explícitas tornam a próxima conversa melhor.

## Node, npm e Angular CLI

Node.js executa as ferramentas JavaScript fora do navegador. npm instala dependências e executa scripts. Angular CLI cria e opera workspaces Angular. Para criar um projeto equivalente do zero, numa pasta vazia:

```bash
npx @angular/cli@22 new biblioteca-viva --style=css --ssr=false
```

O repositório já inclui a aplicação pronta e o lockfile, então você não precisa recriá-la para continuar. Consulte `package.json` para os scripts e `angular.json` para o build, os estilos e o servidor local. `tsconfig.json` configura o compilador; `src/main.ts` inicializa o componente raiz; `src/app/app.config.ts` reúne os provedores.

::: csharp Uma comparação para quem já conhece C#
`package.json` lembra o papel do arquivo de projeto na lista de ferramentas e scripts, mas não é um `.csproj`. npm e Angular CLI fazem parte do ecossistema JavaScript; o compilador TypeScript participa do build, não do runtime do navegador.
:::

## Conhecer o ponto de partida

O primeiro comando a testar é `npm start`. Abra o catálogo e use o navegador para observar o que está realmente na tela. Em outro terminal, `npm run test:ci` deve concluir sem modo de observação; `npm run build` gera a versão de produção em `dist/`.

::: passos Um primeiro ciclo verificável
Leia o script chamado `start` no `package.json`.
===
Execute `npm start` e abra a URL local apresentada no terminal.
===
Mude o texto da chamada do catálogo e confira o resultado no navegador.
===
Rode `npm run test:ci` e `npm run build`; diferencie teste de build.
:::

## Linguagens e ferramentas: escopo honesto

O app deste livro é escrito em Angular e TypeScript, com HTML e CSS para a apresentação. Não adicionamos React, Vue, Bootstrap ou outro framework visual. Node/npm/CLI são ferramentas de construção, não partes da UI. A aplicação final não contém backend: uma API real será uma extensão documentada, não uma capacidade escondida do protótipo.

::: pratica Antes de abrir muitos arquivos
Escolha uma pergunta verificável sobre o app, como “onde são definidos os livros?” Encontre apenas o arquivo necessário e anote o caminho. A habilidade não é memorizar a árvore inteira; é saber localizar a próxima peça.
:::

::: quiz
? Qual é a função principal do Angular CLI neste projeto?
+ Criar e operar o workspace: servir localmente, testar e gerar o build.
- Substituir o navegador e executar a aplicação final no servidor Node.
- Guardar os dados de livros e favoritos como um banco de dados.
:::

::: cartas
O que faz `npm ci`? :: Instala as dependências reproduzindo as versões resolvidas no lockfile.
Por que definir limites do protótipo? :: Para comunicar o que existe hoje e evitar prometer backend, autenticação ou sincronização que não foram construídos.
:::
