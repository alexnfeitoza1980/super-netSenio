---
id: ang-pratica-cap08
order: 283
kind: capitulo
num: 193
group: Angular · Parte 47 · Estado e interação
title: Rotas, páginas e navegação
subtitle: O Router transforma URLs em estados navegáveis, compartilháveis e carregados apenas quando necessário.
minutes: 50
hue: 210
objectives:
- Declarar rotas e parâmetros
- Usar lazy loading, titles, guards e resolvers com responsabilidade
- Tratar rotas desconhecidas e navegação por teclado
origem: angular/livros/01-biblioteca-angular/content/08-cap08.md
---

::: ilustracao img/ang-pratica/mapa-rotas.svg | O catálogo encaminha para detalhe, favoritos ou uma recuperação para caminhos inválidos.
:::

## URL como estado navegável

A Biblioteca Viva tem catálogo em `/`, livro em `/livros/:id` e favoritos em `/favoritos`. O `:id` alimenta um input do componente de detalhe porque usamos `withComponentInputBinding()`. `RouterLink` muda a rota sem recarregar a página. O título da rota ajuda a identificar o conteúdo na aba do navegador.

<<< src/app/app.routes.ts

Cada `loadComponent` carrega a página quando necessário. Lazy loading reduz o JavaScript inicial se a aplicação crescer. A rota coringa vem ao final e oferece uma página de recuperação, em vez de uma tela em branco.

## Guardas não são autorização

`CanActivate` pode melhorar navegação, por exemplo impedir acesso a uma etapa que não faz sentido sem parâmetros. Porém, uma guarda no navegador nunca protege dados do servidor: a API precisa validar identidade e autorização do lado do servidor. Não confie no cliente para segurança.

`Resolve` carrega dados antes da ativação da rota, o que pode simplificar estados, mas também pode deixar a navegação esperando. Escolha conscientemente entre resolver antes, exibir loading na página ou usar um recurso reativo.

## URL, histórico e acessibilidade

Use query parameters para filtros que devem ser compartilhados ou restaurados ao voltar. Não coloque dados pessoais na URL. Garanta foco previsível quando muda a página, título atualizado e links com texto compreensível. Botões são para ações; links são para navegação.

::: quiz
? Uma guarda de rota no navegador protege uma API de acesso não autorizado?
+ Não. O servidor da API precisa autenticar e autorizar cada operação.
- Sim, porque uma rota oculta não pode ser aberta diretamente.
- Sim, se o componente for carregado com `loadComponent`.
:::

::: cartas
Por que carregar páginas com `loadComponent`? :: Para dividir o código por rota e carregar partes quando forem necessárias.
Qual é o papel da rota `**`? :: Capturar caminhos que não correspondem a outras rotas e apresentar uma recuperação útil.
:::
