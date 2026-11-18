---
id: ang-pratica-cap05
order: 280
kind: capitulo
num: 190
group: Angular · Parte 47 · Estado e interação
title: Signals e estado derivado
subtitle: Estado explícito ajuda a explicar por que a tela mudou e a manter cálculos derivados fora de efeitos colaterais.
minutes: 50
hue: 210
objectives:
- Criar e ler signals e atualizar estado sem mutação
- Derivar dados com `computed`
- Distinguir `computed` de `effect` e reconhecer dependências reativas
origem: angular/livros/01-biblioteca-angular/content/05-cap05.md
---

## Estado que a tela acompanha

Um signal é um valor reativo. Chamá-lo sem argumentos lê o valor; `set` substitui; `update` calcula o próximo valor a partir do atual. A store guarda busca, categoria e preferências. `resultados` é um `computed` que depende da busca e da categoria.

::: ilustracao img/ang-pratica/fluxo-interacao.svg | A busca altera um signal; um computed deriva resultados e o template reflete a mudança.
:::

<<< src/app/biblioteca.store.ts

`computed` é apropriado para uma derivação determinística: para o mesmo estado de entrada, os resultados são os mesmos. Não use `effect` para copiar um signal a outro e manter duas fontes de verdade. Efeitos são voltados a sincronizar com sistemas externos quando a derivação não basta.

`linkedSignal` representa estado gravável que começa derivado de outra fonte e pode mudar independentemente — por exemplo, uma categoria selecionada que acompanha o filtro padrão até a pessoa escolher outra. `resource` modela uma leitura assíncrona reativa com estados de carregamento e erro. Use essas APIs quando a semântica combinar; não troque código simples por uma API nova só porque ela existe.

## Atualização zoneless e OnPush

Este workspace usa a configuração moderna do Angular, sem `zone.js`, e declara `OnPush` nos componentes do projeto. Signals, eventos Angular, inputs e outras notificações conhecidas informam ao framework quando a view precisa ser atualizada. Evite atualizar o DOM diretamente: isso ignora a relação declarativa entre estado e tela.

O modelo exato de detecção depende da versão e configuração Angular. Em aplicação legada com Zone.js, mudanças assíncronas podem disparar verificações mais amplas; em zoneless, deixe a origem da notificação explícita. `OnPush` não significa “nunca verificar”: significa reduzir verificações sem necessidade e seguir as notificações corretas.

## Atualização imutável

Para alternar favorito, criamos uma nova lista em vez de modificar a lista anterior. Para registrar progresso, criamos um novo objeto `progresso`. Essa disciplina torna as transições visíveis, preserva o histórico lógico e facilita testes.

O `computed` de resultados é derivado sob demanda e pode ser memoizado. Não mantenha também uma lista “resultados atuais” alterada manualmente: logo ela ficaria diferente dos filtros.

## Um fluxo que cabe na cabeça

Evento de busca → `busca.set(...)` → `resultados()` recalculado → template atualizado. O desenho é simples porque há uma fonte de verdade. Quando uma feature tem dezenas de estados, o princípio permanece: nomeie o estado, derive o que puder, e introduza efeitos externos com limites claros.

::: pratica Faça uma hipótese pequena
Troque “nada funciona” por “ao digitar ‘oceano’, espero um único resultado”. Rode a aplicação, teste essa hipótese e observe o comportamento. Uma hipótese observável é mais útil do que aumentar a tensão ou reescrever sem evidência.
:::

::: quiz
? Para que serve `computed`?
+ Para derivar e memoizar um valor a partir dos signals que ele lê.
- Para fazer uma chamada HTTP em toda mudança de estado.
- Para alterar automaticamente todos os signals que usam o mesmo objeto.
:::

::: cartas
Qual é a diferença entre ler e escrever um signal? :: A leitura chama o signal sem argumento; a escrita usa métodos como `set` ou `update`.
Por que não duplicar o valor filtrado em outro signal? :: Duas fontes de verdade podem divergir; `computed` mantém o resultado ligado aos filtros atuais.
:::
