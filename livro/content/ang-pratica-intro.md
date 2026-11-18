---
id: ang-pratica-intro
order: 275
kind: intro
group: Angular · Parte 46 · Começar com clareza
title: Angular na Prática — apresentação
subtitle: Angular moderno do zero ao avançado, aprendido pela construção de um produto inteiro: a Biblioteca Viva.
minutes: 25
hue: 210
badge: ★
objectives:
- Entender o caminho completo do livro e o produto que vamos construir
- Preparar Node.js, Angular CLI e o workspace executável
- Reconhecer como técnica, atenção, comunicação e colaboração se encontram no trabalho
origem: angular/livros/01-biblioteca-angular/content/00-intro.md
---

::: ilustracao img/ang-pratica/mapa-do-livro.svg | O roteiro alterna fundamentos, construção da aplicação, qualidade e entrega.
:::

## Uma biblioteca, não uma pilha de exemplos

Este livro acompanha a construção da **Biblioteca Viva**, uma aplicação Angular para explorar livros, pesquisar por título, autor e descrição, filtrar categorias, abrir detalhes, guardar favoritos e registrar o progresso de leitura. O projeto é pequeno o bastante para compreender e real o bastante para ensinar decisões que você encontrará em produtos de verdade.

Você não vai aprender Angular decorando diretivas. Vai observar uma necessidade, fazer uma pequena escolha, implementá-la e conferir o resultado. O catálogo da primeira versão é local e os dados de preferência ficam no navegador. Isso é intencional: o livro não finge que existe autenticação, API ou banco de dados. Mais adiante vamos desenhar a fronteira para substituir o protótipo por serviços reais.

## O produto ao longo do percurso

| Etapa | O que o produto ganha | Angular que aparece |
|---|---|---|
| 1 · Fundamentos | primeira tela e catálogo | CLI, TypeScript, componentes e templates |
| 2 · Interação | busca, filtros e favoritos | signals, `computed`, eventos e composição |
| 3 · Navegação e dados | páginas de detalhe e progresso | Router, DI, serviços, formulários e persistência |
| 4 · Confiabilidade | testes e estados compreensíveis | Vitest, acessibilidade, erros e segurança |
| 5 · Produção | aplicação rápida e publicável | lazy loading, build, SSR/hydration e observabilidade |

## Mapa capítulo a capítulo

| Capítulo | Conteúdo técnico e de produto |
|---|---|
| 1 | Necessidades do catálogo, Node/npm, CLI e estrutura do workspace |
| 2 | TypeScript, modelos, strict mode e validação de dados desconhecidos |
| 3 | Componentes standalone, imports, inputs e limites de responsabilidade |
| 4 | Templates, bindings, controle de fluxo, estados vazios e eventos tipados |
| 5 | Signals, `computed`, efeitos colaterais e atualização imutável |
| 6 | Composição: `input`, `output`, `model`, projeção e CVA |
| 7 | Injeção de dependência, providers, escopos e contratos de dados |
| 8 | Router, parâmetros, navegação, lazy loading, guards e resolvers |
| 9 | Forms, validação, mensagens acessíveis e controles personalizados |
| 10 | HttpClient, recursos remotos, interceptors e estados de rede |
| 11 | RxJS, operadores de busca, cancelamento e interoperabilidade |
| 12 | Estado local, `localStorage`, migração, IndexedDB e offline |
| 13 | Erros claros, colaboração, revisão de código e comunicação respeitosa |
| 14 | HTML semântico, teclado, leitores de tela, zoom e movimento reduzido |
| 15 | Testes unitários, integração, Router, HTTP e E2E |
| 16 | Medição, lazy loading, `@defer`, imagens, renderização e orçamento |
| 17 | XSS, entradas não confiáveis, autorização e segurança de dependências |
| 18 | SSR, prerender, hydration, browser APIs e localização |
| 19 | Arquitetura por domínio, pipes, diretivas, estado e migrações |
| 20 | CI, build de produção, hosting, observabilidade e revisão final |
| Laboratórios 1–4 | Catálogo, busca/favoritos, rotas/testes e entrega da feature |

O mapa cobre os grandes blocos do Angular atual: CLI e workspace; TypeScript e compilação; componentes standalone; templates e controle de fluxo; signals e detecção de mudanças; inputs, outputs, models e projeção; injeção de dependência; Router, lazy loading, guards e resolvers; formulários reativos e Signal Forms; HttpClient, interceptors e recursos; RxJS e interoperabilidade com signals; testes unitários, integração e E2E; acessibilidade; estado e persistência; performance, SSR/hydration, segurança, internacionalização, builds e deploy. Recursos experimentais ou que mudam entre versões serão identificados como tais, e não tratados como contratos estáveis.

## A parte humana é parte do projeto

O leitor que chega a um assunto difícil não precisa de vergonha; precisa de um próximo passo compreensível. Uma mensagem de erro útil descreve o que aconteceu, o que a pessoa pode fazer e como obter ajuda. Uma revisão de código fala do comportamento observável, pergunta sobre a intenção e propõe uma mudança específica — sem transformar divergência técnica em julgamento pessoal. Uma interface acessível reduz barreiras para quem usa teclado, leitor de tela, zoom ou conexão limitada.

Vamos praticar três hábitos técnicos:

1. **Perceber antes de reagir:** reproduzir um erro, registrar o contexto e separar observação de suposição.
2. **Comunicar para resolver:** explicar impacto, necessidade e pedido concreto, sem culpar quem escreveu o código.
3. **Construir junto:** dividir a mudança em partes revisáveis, reconhecer contribuições e incorporar feedback.

Esses hábitos são práticas de trabalho e convivência, não terapia nem substituto de apoio profissional. As reflexões são opcionais.

## Preparar o ambiente

Instale uma versão LTS do Node.js que satisfaça `>=22.12`. O projeto incluído já tem suas dependências fixadas no `package-lock.json`. Abra um terminal nesta pasta do projeto:

::: terminal Executar e conferir o projeto final
```bash
cd codigo
npm ci
npm start
npm run test:ci
npm run build
```
:::

O servidor local abre o aplicativo em `http://localhost:4200`. O primeiro comando instala versões registradas no lockfile; os próximos executam testes e build. Use `npm ci` para reproduzir o ambiente, não `npm install` quando quiser validar exatamente o que foi testado.

::: dica Ler e executar em paralelo
O projeto `codigo/ang-pratica/` mostra a aplicação completa, não uma série de snapshots por capítulo. Leia cada etapa, rode a aplicação, altere uma pequena parte e use os testes para confirmar o comportamento. Os trechos marcados com caminho são incorporados a partir dos arquivos reais quando o livro é gerado.
:::

::: cuidado Protótipo não é serviço de produção
Favoritos e progresso ficam no `localStorage` deste navegador e podem ser apagados por quem usa o dispositivo. Não guarde ali senhas, tokens, dados de saúde nem informações que precisam de confidencialidade. Um aplicativo comercial precisa de backend, autenticação, autorização, backup e política de privacidade apropriados.
:::

::: pratica Antes de começar
Escreva em uma frase o que gostaria de aprender fazendo este projeto. Depois, escolha um pequeno bloco de tempo para o primeiro capítulo. Se a prática não ajudar hoje, pule-a sem prejuízo.
:::

::: quiz
? O que representa a aplicação `codigo/ang-pratica/` deste livro?
+ Um protótipo completo de front-end que usa catálogo local e persistência no navegador, com limites explicitados.
- Um serviço de livros pronto para produção, com banco de dados, autenticação e backup.
- Um conjunto de telas sem relação entre si.
:::

::: cartas
Qual é o produto que vamos ampliar? :: A Biblioteca Viva, uma aplicação Angular de catálogo, busca, detalhes, favoritos e progresso.
Que tipo de dado este protótipo guarda no navegador? :: Favoritos e progresso de leitura; não é lugar para segredos ou dados confidenciais.
:::
