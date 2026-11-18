---
id: ang-pratica-cap12
order: 287
kind: capitulo
num: 197
group: Angular · Parte 48 · Dados e integração
title: Estado local, persistência e offline
subtitle: Persistir o mínimo necessário reduz risco, surpresa e complexidade.
minutes: 45
hue: 210
objectives:
- Diferenciar estado efêmero, estado de rota e preferência persistida
- Tratar `localStorage` como dado não confiável
- Entender limites de cache e aplicações offline
origem: angular/livros/01-biblioteca-angular/content/12-cap12.md
---

## Onde cada estado deve morar

Texto temporário de busca pode viver na página/store da sessão. Um filtro compartilhável pode virar query parameter. Favoritos e progresso deste protótipo persistem no `localStorage`. Uma conta sincronizada entre dispositivos exigiria API autenticada e um modelo de conflito.

Não guarde tudo “porque talvez seja útil”. Quanto mais dados persistidos, maior o impacto de XSS, corrupção, versão antiga e dispositivo compartilhado. O protótipo conserva apenas preferências não sensíveis.

## Evolução de esquema

A chave `biblioteca-viva:estado:v1` identifica o formato. Se a estrutura mudar, migração explícita pode ler uma versão antiga e gravar uma nova. Sem migração, descarte o dado inválido com explicação e preserve o funcionamento do catálogo. Nunca trate um JSON arbitrário como objeto seguro.

`localStorage` é síncrono e limitado. Para dados grandes, consultas estruturadas ou offline robusto, avalie IndexedDB e Service Worker, com uma estratégia de sincronização e conflitos. Angular Service Worker não é ativado por mágica: precisa de configuração, assets e testes próprios.

::: pratica Trate a confiança como parte da API
Abra as ferramentas do navegador e altere a chave de estado para um JSON malformado. Recarregue o app. Observe que a Biblioteca Viva apresenta um aviso, mantém o catálogo e não declara que a preferência foi recuperada.
:::

::: quiz
? Favoritos armazenados em `localStorage` sincronizam automaticamente entre dispositivos?
+ Não. O armazenamento pertence a este navegador; sincronização requer um serviço remoto e regras de conta.
- Sim, porque todo navegador compartilha a mesma origem de armazenamento.
- Sim, se o signal for marcado como `readonly`.
:::

::: cartas
O que não deve ir para `localStorage` neste protótipo? :: Senhas, tokens, dados pessoais sensíveis ou qualquer segredo.
Por que nomear a versão da chave? :: Para reconhecer mudanças de formato e planejar migração ou descarte seguro de dados antigos.
:::
