---
id: crencas-cap97
order: 126
kind: capitulo
num: 97
group: C# & Mentalidade · Parte 16 · Crenças e C#
title: De onde vêm as crenças
subtitle: Família, sociedade e experiências podem influenciar o que esperamos. Conhecer a origem é contexto, não prova nem destino.
minutes: 45
hue: 280
objectives:
- Reconhecer possíveis origens **familiares, sociais e pessoais** de uma ideia
- Usar `enum` e `record` para representar dados sem transformar uma pessoa em rótulo
- Distinguir origem de evidência: saber de onde veio uma frase não mostra se ela é verdadeira
origem: livros/07-crencas-csharp/content/135-cap97.md
---

::: vila Uma frase no primeiro PR
Caio recebe uma sugestão de mudança no código e pensa: “Se alguém pediu revisão, é porque eu não sirvo para programar”. A frase parece familiar; ele já ouviu comentários parecidos na escola. Mas a sugestão de revisão fala de um caso nulo específico. A origem emocional da frase merece cuidado; ela não muda o conteúdo observável do comentário.
:::

## Fontes possíveis

Uma ideia sobre nós ou sobre o mundo pode ser aprendida em casa, na escola, na mídia, em um grupo ou depois de uma experiência marcante. Podemos investigar origens familiares, sociais e pessoais perguntando “de onde pode ter vindo esta ideia?”. Essas categorias podem se misturar: uma experiência acontece dentro de um contexto social e pode ser interpretada com palavras que aprendemos antes.

**Origem não é diagnóstico.** Não precisamos descobrir uma causa única nem identificar culpados para examinar uma afirmação. Uma lembrança pode ajudar a entender por que uma frase parece convincente, mas a pergunta “isso descreve todos os casos atuais?” continua aberta.

| Origem possível | Exemplo de contexto | O que não conclui |
|---|---|---|
| Familiar | Uma regra repetida durante a infância | Que a família causou todo resultado atual |
| Social | Uma expectativa vista em um grupo ou na mídia | Que todas as pessoas compartilham a mesma expectativa |
| Experiência pessoal | Um erro ou dificuldade em uma tarefa | Que a dificuldade se repetirá em qualquer tarefa |
| Desconhecida | A frase aparece, mas a origem não é clara | Que é preciso escavar uma lembrança para continuar |

::: regra Origem e validade são perguntas diferentes
“Aprendi esta ideia em casa” descreve uma origem possível. “A ideia é verdadeira em todas as situações” é outra afirmação, que precisaria de evidências próprias.
:::

## Modelando a origem com `enum`

Um `enum` representa opções nomeadas. Um `record` agrupa dados de um jeito claro. O modelo descreve **uma frase**, não uma pessoa:

```cs Jornada/Crencas/Hipotese.cs
namespace Jornada.Crencas;

public sealed record Hipotese(string Enunciado, OrigemDaCrenca Origem);
```

Exemplo fictício:

```cs
var hipotese = new Hipotese(
    "Preciso saber tudo antes de abrir um pull request.",
    OrigemDaCrenca.Social);
```

O tipo não diz se o enunciado é verdadeiro, útil ou prejudicial; isso será investigado nos capítulos seguintes. Também não armazena nome, idade, histórico familiar ou qualquer dado de uma pessoa.

::: exercicio Classifique a origem sem adivinhar a verdade
Uma pessoa ouviu muitas vezes “não faça perguntas” em uma equipe antiga e agora hesita em pedir esclarecimentos. Que origem você registraria como hipótese? O que essa classificação permite concluir?
---
**Resposta possível:** origem social ou familiar, dependendo de onde a regra foi aprendida; se não souber, `Desconhecida` é válida. A origem sugere uma pergunta para investigação, mas **não** prova que a pessoa nunca deva perguntar nem explica todo comportamento atual.
:::

::: quiz
? O que informa `OrigemDaCrenca.Familiar` no modelo?
+ Uma origem possível da frase que está sendo examinada.
- Que a frase é verdadeira porque veio da família.
- Que a família deve ser responsabilizada pelo resultado.
> O campo registra contexto, não validade nem culpa.

? Uma pessoa não sabe de onde veio uma ideia. Qual representação é mais honesta?
+ `OrigemDaCrenca.Desconhecida`.
- Escolher a origem mais provável como se fosse fato.
- Excluir a ideia do modelo.
> Desconhecido é um estado válido; investigar a origem não é obrigatório.

? O que uma dificuldade em um exercício de C# demonstra?
+ Que houve dificuldade naquele exercício, com aquele contexto.
- Que a pessoa não consegue aprender programação.
- Que a dificuldade terá sempre a mesma causa.
> Uma observação específica não autoriza uma conclusão universal.
:::

::: resumo
- Origens familiares, sociais e pessoais são **possibilidades de contexto**.
- Origem não prova validade, não determina identidade e não identifica culpados.
- `enum` e `record` deixam os dados explícitos e evitam inferências escondidas.
- É válido marcar a origem como desconhecida ou simplesmente não investigar.
:::

::: cartas
Origem da crença :: Contexto possível em que uma ideia foi aprendida; não é prova de que ela seja verdadeira.
`enum` :: Tipo que nomeia um conjunto definido de opções, como as origens possíveis.
`record` :: Tipo conciso para agrupar dados relacionados.
:::
