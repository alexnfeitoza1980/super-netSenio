---
id: resiliencia-encerramento
order: 124
kind: apendice
group: C# & Mentalidade · Parte 15 · Resiliência em C#
title: Encerramento — Resiliência em C#
subtitle: O que levar do livro, para a vida e para o código, antes de fechá-lo.
minutes: 10
hue: 30
badge: ✦
objectives:
- Rever, em uma página, o que cada capítulo deixou **para a vida** e **para o código**
- Escolher **uma** coisa para levar, e só uma
origem: livros/06-resiliencia-csharp/content/150-encerramento.md
---

## Antes de fechar o livro

Percorremos nove ideias sobre resiliência e, junto com elas, nove recursos de C#. Se você chegou até aqui, já escreveu (ou, ao menos, leu) `enum`, `try/catch`, `TimeProvider`, LINQ, tipos anuláveis, iteradores, interfaces, genéricos, `Resultado<T>`, `async/await` e um disjuntor de circuito, **e rodou tudo com testes**.

| Para a vida | Para o código |
|---|---|
| Resiliência não é voltar a ser como antes (88) | `enum`, `record`, `switch`, `try/catch/finally` |
| Emoções se regulam; não se desligam (89) | Padrões em tupla, `TimeProvider`, `GroupBy` |
| Começar pelo que está na mochila e sob meu alcance (90) | LINQ: `Where`, `ToLookup`, `Except`, tuplas |
| Esperança com evidências, não com frases prontas (91) | Tipos anuláveis, `switch` com tupla, `MaxBy` |
| Pequenas provas fazem a confiança crescer (92) | Encapsulamento, `internal`, `yield return` |
| Ninguém atravessa tudo sozinho (93) | `interface`, classe abstrata, polimorfismo |
| Cada história tem muitas raízes (94) | Genéricos, `Func<>`, lambdas, `SelectMany` |
| Não é preciso sorrir por obrigação (95) | `Resultado<T>`, falhas esperadas, métodos de extensão |
| Saber parar de insistir, e ter um plano B (96) | `async/await`, *backoff*, plano B, disjuntor |

::: pense Uma frase para levar
Se você pudesse levar **uma só** frase deste livro para a próxima semana, qual seria? Escolha uma que caiba no bolso: "um passo possível já é um começo", "pedir ajuda também é seguir em frente", "uma história é uma hipótese", "silêncio também é resposta"...
:::

::: ilustracao img/resiliencia/photos/amanhecer.jpg | A travessia não precisa começar com todas as respostas; às vezes, basta enxergar o próximo trecho.
:::

## Uma lista de verificação gentil

Nada disso é tarefa obrigatória. É só um lembrete do que o livro tentou mostrar:

- Eu posso **reconhecer o que sinto** sem ter que resolver tudo agora.
- Eu posso **separar** o que depende de mim do que não depende, **sem minimizar** o que dói.
- Eu posso **pedir ajuda** de forma clara, **e também dizer "hoje não dá"**.
- No código, eu posso **escrever um teste antes de confiar** em uma ideia.
- Eu posso **parar de insistir** (em uma pessoa que se cobra demais, em um serviço que não volta) e **escolher outro caminho**.

::: cuidado Se pesar demais
Se, durante a leitura, algum tema mexeu mais do que o esperado, **vale conversar** com alguém de confiança ou com um profissional de saúde. Isso também é seguir em frente. No Brasil: **CVV 188** (apoio emocional, 24 h, gratuito) e **SAMU 192** (emergências).
:::

## E agora, no código

- **Rode** `dotnet test codigo/resiliencia/Resiliencia.sln` mais uma vez e **leia os nomes dos testes** em voz alta: eles contam as regras do livro.
- Escolha **um capítulo** e **mude uma regra**: o limite de 25 minutos, a pausa para intensidade alta, as perguntas de uma lente. Veja **quais testes falham** e por quê.
- Visite os outros livros da coleção: **Foco em C#** (livro 4) e **Os Quatro Compromissos em C#** (livro 5) usam o mesmo estilo de modelar uma ideia de vida em código.

::: cartas
Passo possível :: Um passo do tamanho certo para hoje, nem mais nem menos.
Pedir ajuda :: Habilidade que também faz parte de seguir em frente.
Teste :: Frase executável que protege o que foi prometido.
Hipótese :: Explicação que merece ser checada, e não adotada como verdade.
Plano B :: Resposta simples preparada para quando a principal falha.
:::
