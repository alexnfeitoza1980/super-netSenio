---
id: crencas-cap98
order: 127
kind: capitulo
num: 98
group: C# & Mentalidade · Parte 16 · Crenças e C#
title: Fato, interpretação e rótulo
subtitle: Uma observação descreve o que aconteceu; uma interpretação tenta explicar. Separá-las torna a conversa mais precisa, sem invalidar sentimentos.
minutes: 50
hue: 292
objectives:
- Separar uma observação verificável de uma interpretação ampla
- Perceber palavras absolutas como “sempre” e “nunca” sem tratá-las como prova de um problema
- Representar observação e interpretação como campos distintos em C#
origem: livros/07-crencas-csharp/content/136-cap98.md
---

::: vila O teste que falhou
O teste `CalculaTotal_ComCarrinhoVazio` falha. Lia pensa: “Sou péssima programadora”. A falha do teste é observável; a frase sobre a identidade dela não é um resultado do compilador. Ao ler o teste, ela encontra o cenário exato: faltava decidir se carrinho vazio deve retornar zero ou uma validação. Agora há um problema de domínio para discutir, não uma sentença sobre a pessoa.
:::

## Uma lente pode ampliar demais

Uma frase pode misturar três coisas: **o que ocorreu**, **o que concluímos** e **o que sentimos**. Separar não é dizer que a interpretação é “falsa” nem que a emoção não importa. É apenas mostrar quais partes podemos conferir e quais pedem mais contexto.

| Camada | Pergunta útil | Exemplo |
|---|---|---|
| Observação | O que uma pessoa poderia verificar? | “O teste falhou em dois cenários de entrada vazia.” |
| Interpretação | Que explicação estou dando? | “Não sei escrever testes.” |
| Sentimento | Como isso me afetou? | “Fiquei frustrada e com receio da revisão.” |
| Próxima pergunta | O que falta saber? | “Qual comportamento o requisito espera?” |

Palavras como **sempre**, **nunca**, **tudo** e **nada** podem indicar uma generalização. Elas não são automaticamente erradas: um programa pode realmente “sempre” validar um campo por contrato. O cuidado é conferir se a frase descreve uma regra testada ou se expandiu um caso isolado para todos.

::: cuidado Sentimento não é um bug a corrigir
Uma observação verificável pode ajudar a revisar uma conclusão, mas não apaga o sentimento. Se uma conversa ou atividade for desconfortável, faça uma pausa ou escolha um exemplo neutro.
:::

## Tipos explícitos em C#

Ao dar nomes diferentes a observação e interpretação, reduzimos o risco de tratá-las como a mesma coisa:

```cs Jornada/Crencas/Observacao.cs
namespace Jornada.Crencas;

public sealed record Observacao(string Fato, string Interpretacao, string Sentimento);
```

Os campos são texto para manter o exemplo simples. Em um sistema real, isso **não** significa que o programa possa validar a verdade de um relato ou inferir sentimentos. O modelo apenas preserva o que foi informado.

```cs Uso
var caso = new Observacao(
    Fato: "O teste falhou ao receber uma lista vazia.",
    Interpretacao: "Não sei programar.",
    Sentimento: "Frustração");

Console.WriteLine(caso.Fato);
Console.WriteLine("Interpretação a investigar: " + caso.Interpretacao);
```

::: exercicio Reescreva sem apagar a emoção
Diante da frase “Eu nunca vou entender LINQ”, escreva uma observação verificável, uma interpretação mais específica e um sentimento possível. Não precisa revelar uma experiência pessoal.
---
**Resposta possível:** observação: “Ainda não entendi a consulta `GroupBy` do exemplo”; interpretação: “Preciso de mais prática com agrupamentos”; sentimento: “Estou impaciente”. A observação é conferível; a interpretação é uma hipótese; o sentimento não precisa ser provado.
:::

::: quiz
? “A compilação falhou por falta de `using`” é principalmente o quê?
+ Uma observação verificável sobre aquele build.
- Uma conclusão sobre a inteligência de quem escreveu o código.
- Uma prova de que o projeto todo é impossível.
> A frase descreve uma falha concreta e pode ser conferida no diagnóstico.

? “Sempre erro em tudo” deveria ser tratada como evidência literal?
+ Não; é uma generalização que pede exemplos e contexto.
- Sim; a palavra “sempre” torna a frase mais precisa.
- Sim, se foi dita com muita emoção.
> Intensidade de sentimento merece cuidado, mas não transforma uma generalização em dado universal.

? Separar fato e interpretação significa invalidar sentimentos?
+ Não. As três dimensões podem ser reconhecidas sem confundi-las.
- Sim, porque só fatos importam.
- Sim, porque interpretações são sempre falsas.
> O objetivo é clareza, não julgamento da pessoa.
:::

::: resumo
- Fato, interpretação e sentimento respondem a perguntas diferentes.
- Uma falha concreta não define a capacidade ou o valor de alguém.
- Generalizações pedem exemplos e contexto, não uma contra-afirmação automática.
- Em C#, `record` torna explícitos campos que não devem ser confundidos.
:::

::: cartas
Observação :: Descrição específica de algo que pode ser conferido no contexto dado.
Interpretação :: Explicação que atribuímos à observação; pode ser investigada.
Generalização :: Extensão de uma conclusão para vários casos; precisa de evidências proporcionais.
:::
