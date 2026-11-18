---
id: ang-pratica-cap17
order: 292
kind: capitulo
num: 202
group: Angular · Parte 50 · Preparar para crescer
title: Segurança no cliente e limites de confiança
subtitle: Angular ajuda a reduzir classes de risco, mas nenhuma UI substitui validação, autorização e proteção no servidor.
minutes: 55
hue: 210
objectives:
- Entender escaping, sanitização e risco de XSS
- Tratar parâmetros, localStorage e respostas de API como entrada não confiável
- Reconhecer limites de autenticação e autorização no navegador
origem: angular/livros/01-biblioteca-angular/content/17-cap17.md
---

## O que Angular protege — e o que não promete

Interpolação mostra texto escapado por padrão. Isso reduz risco de injeção ao renderizar texto normal. Não use `bypassSecurityTrustHtml` para “consertar” conteúdo e nunca interprete conteúdo não confiável como template Angular.

Não monte HTML de usuário e não injete conteúdo com DOM APIs sem sanitização apropriada. Sanitização não é validação de regra de negócio, nem impede phishing ou conteúdo enganoso. Trate URL, nomes e JSON externo como dados.

## Dados locais não são autorização

Uma pessoa pode abrir DevTools, mudar localStorage e chamar endpoints diretamente. Guards e botões desabilitados ajudam a navegação, mas não autorizam operações. A API precisa conferir a sessão e a permissão em cada requisição. Tokens persistidos em armazenamento acessível a JavaScript aumentam o impacto de XSS; escolha arquitetura de autenticação com análise de risco.

Não inclua segredos em arquivos do frontend, variáveis `environment` ou bundles. O navegador entrega esses dados a qualquer pessoa que carregue o app.

## Dependências e Content Security Policy

Fixe e revise dependências, mantenha lockfile e atualize Angular com estratégia de migração. Uma Content Security Policy bem planejada reduz caminhos de execução de scripts injetados; teste a política com o build real, sem copiar uma configuração permissiva às pressas.

::: cuidado Segurança exige defesa em camadas
Um aplicativo público com autenticação, perfil e dados pessoais precisa de revisão especializada, backend seguro, logs com privacidade e tratamento de abuso. O app deste livro é um protótipo educativo, não um produto pronto para tratar contas reais.
:::

::: quiz
? Um usuário altera o localStorage e marca como favorito um ID inventado. Qual regra continua válida?
+ O cliente deve validar para evitar estado quebrado, e qualquer operação protegida também precisa ser validada no servidor.
- O botão escondido garante que o servidor nunca receba a operação.
- Tipos TypeScript impedem alteração pelo DevTools.
:::

::: cartas
Interpolação Angular transforma texto em HTML executável? :: Não; texto interpolado é escapado por padrão. Ainda assim, evite interpretar HTML não confiável.
Onde deve ocorrer a autorização real? :: No servidor, em cada operação protegida.
:::
