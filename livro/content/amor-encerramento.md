---
id: amor-encerramento
order: 149
kind: apendice
group: C# & Mentalidade · Parte 17 · Você é aquilo que ama, em C#
title: Encerramento — Você é Aquilo que Ama em C#
subtitle: O que levar do livro para a vida e para o código, e uma pergunta para escolher só uma coisa.
minutes: 10
hue: 25
badge: ✦
objectives:
- Rever, em uma página, o que cada capítulo deixou **para a vida** e **para o código**
- Escolher **uma** prática pequena para repetir esta semana
origem: livros/08-voce-e-aquilo-que-ama-csharp/content/150-encerramento.md
---

## Antes de fechar o livro

Percorremos onze ideias sobre **desejo, hábito e formação**, e, junto com elas, doze recursos de C# (de `[Flags]` e `record struct` a `System.Text.Json`, passando por LINQ, eventos, `IParsable<T>` e pipelines). Smith termina o livro com uma ideia que combina com tudo o que vimos: fomos formados **para sermos enviados**, e não para ficarmos guardados dentro do que praticamos. Em nossa linguagem: **o que repetimos aqui deveria se traduzir no que fazemos lá fora**.

| Para a vida | Para o código |
|---|---|
| Você é mais o que deseja do que o que sabe (105) | `[Flags]`, `Aggregate`, decisões por pesos |
| A bússola e o mapa: some o rumo, não só os passos (106) | `readonly record struct`, operadores, `double?` |
| Amar é hábito: imitação e prática (107) | Imutabilidade, `with`, parâmetros opcionais |
| O que se diz × o que se faz, com curiosidade (108) | `GroupJoin`, `ExceptBy`, `Select` com índice |
| A forma ensina; o padrão cuida (109) | `[Flags]` com `1 << n`, `required`, `init` |
| Reabituar: prática, companhia e paciência (110) | `Func<int,double>`, `Zip`, `TakeWhile` |
| Todo encontro tem um enredo (111) | Máquina de estados, `event`, `IDisposable` |
| Os ritmos pequenos formam o time (112) | `IComparable<T>`, `SortedSet`, iteradores |
| Aprender de cor: rever em intervalos (113) | Leitner, `record`+`with`, `Random` injetado |
| Limites bem escolhidos libertam (114) | Objetos de valor, `IParsable<T>`, operador implícito |
| Tudo importa: o trabalho forma (115) | Pipeline `async`, closures, funções locais |

::: pense Uma prática para esta semana
Escolha **uma** (só uma) prática pequena para repetir nos próximos sete dias, e responda: *o que ela vai formar em mim, se eu a repetir por um ano?* Algo como: "abrir o repositório antes do e-mail", "escrever o teste antes do código numa tarefa por dia", "ligar para uma pessoa em vez de mandar mensagem". Se possível, **conte a alguém** que vai tentar. Companhia faz parte do caminho.
:::

## Uma lista de verificação gentil

Nada disso é tarefa obrigatória. É só um lembrete do que o livro tentou mostrar:

- **Saber não é fazer**, e isso é humano. Mais um curso raramente fecha esse hiato; **prática, ambiente e companhia** ajudam.
- Minha **agenda** é um extrato do que eu desejo hoje. Lê-la com **curiosidade** (e não com culpa) me orienta.
- A **forma** (o formato do encontro, o padrão da configuração) ensina mais que o discurso. Posso **mudar uma forma pequena**.
- **Hábitos se desfazem devagar e se formam devagar.** Uma pausa atrasa; não apaga.
- **Limites bem escolhidos** (um tipo que não aceita lixo, uma etapa que não se pula) tiram peso da cabeça.
- **Ninguém se forma sozinho:** posso pedir que alguém pratique junto.

::: cuidado Se pesar demais
Se algum capítulo mexeu mais do que o esperado (principalmente os que olham para o que fazemos com o tempo e com os desejos), **vale conversar** com alguém de confiança ou com um profissional de saúde. Isso também é uma prática que forma. No Brasil: **CVV 188** (apoio emocional, 24 h, gratuito) e **SAMU 192** (emergências).
:::

## E agora, no código

- Rode `dotnet test codigo/amor/Habitos.sln` e **leia os nomes dos testes** em voz alta: eles contam as ideias do livro.
- Escolha **um capítulo** e **mude uma regra** (o ganho do hábito, os intervalos do Leitner, as perguntas do `Examinar`). Veja **quais testes falham** e por quê.
- Faça a **Auditoria litúrgica** (Lab 12) com **seus** dados, no seu ritmo, e **só** o que quiser.
- Visite os outros livros da coleção: o **Livro 5** (*Os Quatro Compromissos em C#*) e o **Livro 6** (*Resiliência em C#*) tratam, por outras lentes, de palavra, hábito e cuidado.

::: cartas
Enviados :: Formados para voltar ao mundo e agir, e não para guardar o que se aprendeu.
Prática pequena :: Algo do tamanho certo para repetir esta semana.
Companhia :: Parte do caminho: quem pratica junto ajuda a continuar.
Forma pequena :: Detalhe de formato que pode ser mudado com pouco esforço e grande efeito.
Teste como espelho :: Frase executável que lembra o que prometemos a quem usa nosso código.
:::
