---
id: ang-pratica-cap20
order: 295
kind: capitulo
num: 205
group: Angular · Parte 50 · Preparar para crescer
title: Build, deploy e colaboração de equipe
subtitle: Uma feature só está pronta quando pode ser construída, verificada, publicada e mantida por outras pessoas.
minutes: 50
hue: 210
objectives:
- Gerar e inspecionar build de produção
- Planejar CI, ambiente, hosting e monitoramento
- Entregar uma mudança pequena com documentação e revisão respeitosa
origem: angular/livros/01-biblioteca-angular/content/20-cap20.md
---

::: ilustracao img/ang-pratica/entrega-angular.svg | A feature atravessa revisão, testes, build, publicação e observação.
:::

## Do workspace ao artefato

`npm run build` executa o build configurado para produção. Publique o conteúdo de `dist/biblioteca-viva/browser` no hosting escolhido, conforme a saída da versão instalada. Configure fallback de SPA para rotas como `/livros/jardim-das-ideias`; sem fallback, atualização direta pode retornar 404 do servidor estático.

Não ponha credenciais em `src/environments` e não trate configuração pública como segredo. Defina configuração por ambiente, origens permitidas e política CSP no servidor/hosting. Faça deploy com cache correto: assets com hash podem ter cache longo; HTML inicial costuma precisar de atualização rápida.

## CI como proteção compartilhada

Um pipeline básico instala com lockfile, executa lint quando configurado, testes, build e análise de dependências. Preserve logs úteis sem incluir dados pessoais ou segredos. Em aplicação real, adicione verificações acessíveis e E2E para fluxos críticos. Um status verde não substitui revisão humana.

Erros de produção precisam ser observáveis: métricas agregadas, taxa de falha e contexto técnico mínimo. Planeje retenção e anonimização. Não grave busca pessoal ou conteúdo do usuário em logs por conveniência.

## Pronto inclui comunicação

Na descrição da entrega, explique problema, comportamento esperado, mudança, testes executados e limites conhecidos. Peça revisão com contexto e agradeça questionamentos específicos. Quando houver divergência, respondam ao critério do produto e ao comportamento medido, não a suposições sobre a pessoa.

::: passos Checklist de saída
`npm ci`, testes e build terminam sem erro.
===
Busca, categoria, detalhe, favoritos e progresso funcionam em tela pequena e teclado.
===
Erros e dados persistidos têm comportamento compreensível; nenhum segredo vai no bundle.
===
O README informa requisitos, comandos, limites e como continuar.
===
O deploy resolve rotas diretas e serve os assets com cache e origem configurados.
:::

::: quiz
? O que precisa ser configurado para uma rota Angular abrir após recarregar numa hospedagem SPA?
+ A hospedagem deve encaminhar caminhos da aplicação ao documento inicial, mantendo assets e endpoints separados.
- Angular precisa fazer todas as páginas sem endereço.
- Basta adicionar um guard a cada rota.
:::

::: cartas
O que deve entrar na descrição de uma entrega? :: Contexto, comportamento esperado, mudança realizada, verificações executadas e limites conhecidos.
O que significa observabilidade com privacidade? :: Acompanhar saúde do sistema com o mínimo de dados necessário, retenção e acesso controlados.
:::
