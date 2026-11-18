---
id: ang-pratica-cap13
order: 288
kind: capitulo
num: 198
group: Angular · Parte 49 · Qualidade e cuidado
title: Erros claros e comunicação respeitosa
subtitle: Um erro bem tratado informa sem culpar, e uma revisão útil abre espaço para corrigir o problema em conjunto.
minutes: 40
hue: 210
objectives:
- Diferenciar erro técnico, mensagem de interface e diagnóstico
- Descrever impacto e próximo passo sem expor dados internos
- Fazer pedidos de revisão específicos e verificáveis
origem: angular/livros/01-biblioteca-angular/content/13-cap13.md
---

## A mensagem para quem usa o produto

“Falha 500” diz pouco a quem queria abrir um livro. Uma mensagem melhor: “Não foi possível carregar o catálogo agora. Sua busca foi mantida. Tente novamente.” Uma ação de retry precisa realmente repetir a consulta, e a tela deve comunicar estados de carregamento e conclusão.

A mensagem para uma pessoa não deve revelar stack trace, URL interna, token, dado de outro usuário ou query de banco. Diagnóstico técnico vai a logs controlados, com acesso restrito, retenção definida e remoção de dados pessoais.

## A conversa dentro da equipe

Um roteiro de feedback concreto:

1. **Observação:** “A rota de detalhe grava o progresso direto no navegador.”
2. **Impacto/necessidade:** “Isso torna a regra difícil de testar e de trocar por uma API.”
3. **Pedido:** “Podemos extrair a gravação para a store antes de adicionar sincronização?”

Não é um script para vencer discussões. É uma maneira de separar fato observável de interpretação e propor uma ação que a outra pessoa pode aceitar, recusar ou ajustar. Pergunte primeiro: qual restrição este código está atendendo?

::: ilustracao img/ang-pratica/ciclo-revisao.svg | Uma feature melhora por meio de escuta, desenho, construção e validação.
:::

## Revisão de código sem julgamento

Comente a linha ou comportamento, não a capacidade ou caráter de quem a escreveu. Substitua “isso é óbvio” por “podemos tornar a condição explícita?”. Diferencie bloqueio de preferência. Se uma sugestão for opcional, diga que é opcional. Se feedback ficar confuso, uma conversa curta pode esclarecer o tom antes de escalar conflito.

Erro e desacordo são informação. A pessoa continua digna de respeito enquanto o código é corrigido. Isso não elimina responsabilidade: pede clareza, critérios e acompanhamento.

::: quiz
? Qual texto ajuda mais quem enfrenta uma falha temporária no catálogo?
+ “Não foi possível carregar os livros. Sua busca foi mantida; tente novamente.”
- “Erro 500. Problema do usuário.”
- Uma página vazia para não mostrar detalhes técnicos.
:::

::: cartas
O que um feedback técnico deve descrever? :: Comportamento observável, impacto ou necessidade e um pedido específico.
O que não mostrar numa mensagem pública de erro? :: Segredos, dados de outras pessoas, stack trace e detalhes internos da infraestrutura.
:::
