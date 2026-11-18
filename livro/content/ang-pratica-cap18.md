---
id: ang-pratica-cap18
order: 293
kind: capitulo
num: 203
group: Angular · Parte 50 · Preparar para crescer
title: SSR, hydration e localização
subtitle: Renderizar no servidor e oferecer vários idiomas são decisões de produto e infraestrutura, não caixas que se marcam sem testes.
minutes: 55
hue: 210
objectives:
- Entender SSR, prerender e hydration em nível de aplicação
- Evitar APIs exclusivas do navegador em código executado no servidor
- Planejar i18n, locale, rotas e conteúdo traduzido
origem: angular/livros/01-biblioteca-angular/content/18-cap18.md
---

::: ilustracao img/ang-pratica/ssr-hydration.svg | O HTML inicial do servidor precisa combinar com o estado que o Angular hidrata no navegador.
:::

## Quando renderizar no servidor

Server-Side Rendering produz HTML inicial no servidor; prerender gera HTML durante o build; hydration liga o HTML já entregue ao runtime Angular no navegador. Podem melhorar descoberta, primeira pintura e experiência em redes lentas, mas elevam a complexidade de deploy e de acesso a dados.

O construtor da store deste protótipo acessa `localStorage` dentro de um `try`, o que não é um projeto SSR completo. Para SSR robusto, injete uma abstração de persistência que escolha armazenamento apenas no browser (`isPlatformBrowser`) e defina comportamento explícito no servidor. O HTML prerenderizado não deve depender de estado privado do navegador.

IDs, horários, randomização e conteúdo precisam ser estáveis entre renderização do servidor e hydration. Divergência causa hydration mismatch. Teste o build SSR e a navegação real; não assuma que um build cliente prova SSR.

## Idioma e formatos

Idioma da UI, conteúdo dos livros e locale para datas/números são decisões diferentes. Planeje traduções, pluralização, textos alternativos e URLs localizadas. Evite concatenar frases em ordem fixa; use mensagens com parâmetros. `LOCALE_ID` afeta formatação Angular, mas não traduz automaticamente cada string do produto.

::: passos Decidir sobre SSR com evidência
Identifique o problema de produto que SSR/prerender deve resolver.
===
Confira as opções suportadas pela versão do Angular e pelo hosting escolhido.
===
Implemente a fronteira browser/server e trate cache e dados corretamente.
===
Teste HTML inicial, hydration, navegação, erro de rede e conteúdo traduzido.
:::

## Trade-offs explicados com honestidade

Uma biblioteca interna atrás de login pode não precisar de prerender. Um catálogo público e indexável pode se beneficiar. A escolha depende do público, hospedagem, métricas e capacidade operacional. Informe a equipe sobre custos, não apenas benefícios.

::: quiz
? O que precisa ser considerado para uma store acessar `localStorage` com SSR?
+ O armazenamento precisa ser isolado para rodar somente no browser, e o estado inicial do servidor deve ser definido com coerência.
- Angular replica localStorage automaticamente no servidor.
- É suficiente esconder o erro com `as any`.
:::

::: cartas
Qual diferença entre SSR e prerender? :: SSR renderiza HTML por requisição no servidor; prerender gera páginas durante o build.
O que é hydration mismatch? :: Quando o HTML inicial e a árvore que o Angular espera no browser divergem.
:::
