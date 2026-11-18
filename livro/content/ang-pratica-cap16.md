---
id: ang-pratica-cap16
order: 291
kind: capitulo
num: 201
group: Angular · Parte 50 · Preparar para crescer
title: Desempenho e carregamento inteligente
subtitle: Meça antes de otimizar; faça a primeira experiência carregar o necessário e não todo o futuro da aplicação.
minutes: 50
hue: 210
objectives:
- Medir carregamento, execução e renderização com ferramentas do navegador
- Usar lazy loading, `@defer`, `track` e derivação eficiente
- Reconhecer imagens, rede e fontes como parte do orçamento de desempenho
origem: angular/livros/01-biblioteca-angular/content/16-cap16.md
---

## Primeiro medir

Abra DevTools, confira tamanho do bundle, tempo de carregamento e tarefas longas. Reproduza num dispositivo ou perfil de rede representativo. Evite “otimizar” uma função pequena sem perfil que indique um problema.

A Biblioteca Viva usa `loadComponent` nas páginas e `track livro.id` na lista. O `computed` evita calcular filtro fora do modelo reativo. Uma lista curta não precisa de virtualização; milhares de registros podem precisar de paginação, API de busca ou CDK virtual scroll.

`@defer` carrega conteúdo não crítico sob condições como viewport ou interação. Agrupe as dependências adiadas corretamente e ofereça `@placeholder`, `@loading` e `@error` quando fizer sentido. O conteúdo principal e os controles de navegação não devem desaparecer atrás de um adiamento mal planejado.

## Perfis diferentes, orçamento explícito

Imagem real deve ter tamanho apropriado, texto alternativo conforme contexto e formato moderno. Fonte externa custa requisição e pode afetar privacidade; fonte local também precisa de orçamento. Defina limites de bundle no build e investigue avisos em vez de aumentar o limite automaticamente.

::: quiz
? O que fazer antes de uma otimização de performance?
+ Reproduzir e medir o gargalo com uma ferramenta e um cenário representativo.
- Reescrever todo o app usando a menor quantidade de linhas.
- Aumentar o limite de bundle até o aviso sumir.
:::

::: cartas
Para que serve `@defer`? :: Adiar dependências e conteúdo não críticos sob gatilhos controlados.
Por que `track` deve usar uma chave estável? :: Para manter identidade de itens entre mudanças e evitar trabalho de DOM desnecessário.
:::
