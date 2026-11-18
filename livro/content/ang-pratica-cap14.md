---
id: ang-pratica-cap14
order: 289
kind: capitulo
num: 199
group: Angular · Parte 49 · Qualidade e cuidado
title: Acessibilidade e design inclusivo
subtitle: Acessibilidade não é acabamento; é qualidade funcional para pessoas com diferentes formas de navegar, perceber e compreender.
minutes: 50
hue: 210
objectives:
- Usar HTML semântico, labels, foco e nomes acessíveis
- Oferecer estado e feedback além da cor
- Testar teclado, zoom, contraste e tecnologias assistivas
origem: angular/livros/01-biblioteca-angular/content/14-cap14.md
---

## Comece pelo HTML

Use `<button>` para ações e `<a>` para navegação. Associe `label` a campos, mantenha hierarquia de títulos, ofereça link para pular ao conteúdo e não remova o indicador de foco. O cartão favorito tem nome acessível e expõe `aria-pressed`.

ARIA complementa HTML; não substitui controles nativos. Um `<div role="button">` exige que a equipe recrie teclado, foco e semântica que um botão já possui.

## Feedback que não depende de um canal

Estado selecionado não pode ser indicado só por cor. O filtro também usa texto e `aria-pressed`. Progresso aparece em texto e no elemento `<progress>`. Mensagens de resultado usam anúncio educado. Animações respeitam `prefers-reduced-motion`.

Zoom a 200%, navegação apenas por teclado e contraste são testes manuais simples. Leitor de tela precisa de nome, função, estado e ordem de foco coerentes. Ferramentas automatizadas encontram alguns problemas, não substituem pessoas e avaliação manual.

::: passos Inspeção rápida com teclado
Use Tab e Shift+Tab para percorrer a tela.
===
Confira se cada foco visível acompanha a ordem lógica.
===
Ative links com Enter e botões com Espaço ou Enter.
===
Teste busca vazia e mensagens anunciadas; amplie o zoom sem perder ações.
:::

## Inclusão também é conteúdo

Texto simples, instruções específicas, uma ação de recuperação e a possibilidade de interromper ajudam pessoas com contextos variados. Uma metáfora não é universal; se uma analogia confundir, explique o conceito diretamente. A interface deve respeitar idioma, preferência e capacidade.

::: quiz
? Qual é a melhor base para um controle de ação?
+ Um elemento `<button>` com texto ou nome acessível e foco visível.
- Um `<div>` clicável sem tabindex.
- Um ícone cuja função só pode ser percebida pela cor.
:::

::: cartas
Por que `aria-pressed` é útil em um filtro? :: Expõe programaticamente se a opção está ativa, não só a cor visual.
O que uma auditoria automática não garante? :: Usabilidade completa para leitor de tela, teclado, baixa visão ou diferentes necessidades; teste manual continua necessário.
:::
