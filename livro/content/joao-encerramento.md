---
id: joao-encerramento
order: 173
kind: apendice
group: C# & Mentalidade · Parte 18 · O Evangelho segundo João em C#
title: Encerramento — O Evangelho segundo João em C#
subtitle: O que levar do livro para a leitura e para o código, e uma pergunta para escolher só uma coisa.
minutes: 10
hue: 220
badge: ✦
objectives:
- Rever, em uma página, o que cada capítulo deixou **para a leitura** e **para o código**
- Escolher **uma** prática pequena para repetir esta semana
origem: livros/10-evangelho-joao-csharp/content/150-encerramento.md
---

## Antes de fechar o livro

Passamos por dez cenas de um dos textos mais lidos do mundo e, junto com elas, por dez recursos de C#, do *logging* à injeção de dependência. Entre os primeiros capítulos do Evangelho há um convite curto, repetido com poucas palavras: **"vem e vê"** (1,39 e 1,46). O livro tentou ser isso: **chegar perto do texto**, olhar as palavras de perto e **conferir** o que se diz sobre elas. Em código, o equivalente é o teste: **não confie, rode**.

| Para a leitura | Para o código |
|---|---|
| Luz, trevas e testemunho (124) | Logging com níveis, escopos e `IDisposable` |
| "Eu sou": imagens que se descrevem (125) | Atributos, reflexão e `IComparable<T>` |
| Caná: só quem prova percebe (126) | Genéricos de dois tipos e `yield return` |
| O vento sopra onde quer (127) | `IObservable<T>`, `IObserver<T>` e operadores |
| O poço fundo e a fonte (128) | Cache, `Lazy<T>`, `ConcurrentDictionary`, `TimeProvider` |
| Cinco pães e nada se perde (129) | `Channel<T>`, `IAsyncEnumerable<T>` e invariantes |
| A voz do pastor (130) | HMAC-SHA256, base64url e tempo constante |
| Lázaro: soltai-o (131) | Event sourcing, `abstract record` e `Aggregate` |
| A toalha e a bacia (132) | Decorator e a ordem das camadas |
| A videira e os ramos (133) | Injeção de dependência e reflexão |
| Concordância e roteiro (Lab 14) | LINQ, densidade, BFS e Dijkstra |

::: pense Uma prática para esta semana
Escolha **uma** prática pequena de leitura ou de código para repetir nos próximos sete dias. Por exemplo: "antes de citar um número, **conferir** na fonte", "escrever o teste **antes** de corrigir um erro", "ler **uma** passagem devagar, em voz alta, e anotar uma palavra que me surpreendeu". Se possível, **conte a alguém** que vai tentar.
:::

## Uma lista de verificação gentil

Nada disso é tarefa obrigatória. É só um lembrete do que o livro tentou mostrar:

- **Conferir é um ato de respeito.** Com o texto, com os números e com quem lê o que escrevemos. Os totais deste livro estão em testes.
- **Uma palavra pode ter dois sentidos** (*anōthen*, *katelaben*, *pneuma*). Em código também: um nome ambíguo merece um teste ou uma nota.
- **O que o texto não diz, o livro não inventa.** Uma lista de "sete sinais" não está em João, que numera só dois; e o texto não diz quando a água virou vinho.
- **Metáfora é ponte, e não prova.** Dizer que uma videira "é" um recipiente de dependências ilumina o código, mas não explica a videira.
- **Camadas, eventos, canais e caches** resolvem problemas de **sistemas**, e podem ser recusados quando o problema é pequeno. Não complique o que cabe numa função.

::: cuidado Se algum capítulo pesou
O texto estudado trata de morte, luto e fé, e cada pessoa o recebe de um jeito. Se alguma cena mexeu mais do que o esperado, **vale conversar** com alguém de confiança ou com um profissional de saúde. No Brasil: **CVV 188** (apoio emocional, 24 h, gratuito) e **SAMU 192** (emergências).
:::

## E agora, no código

- Rode `dotnet test codigo/joao/Joao.sln` e **leia os nomes dos testes** em voz alta: eles contam as ideias do livro.
- Use a **concordância** (Lab 14) com **outras palavras** do arquivo e compare as densidades.
- Escolha **um capítulo** e **mude uma regra** (a ordem dos decoradores no 132, a validade do poço no 128, o segredo da voz no 130). Veja **quais testes falham** e por quê.
- Visite os outros livros da coleção: o **Livro 8** (*Você é Aquilo que Ama em C#*) e o **Livro 6** (*Resiliência em C#*) tratam, por outras lentes, de hábito e cuidado.

::: cartas
Vem e vê :: Convite de 1,39 e 1,46: chegar perto e conferir por conta própria.
Conferir :: Verificar contra a fonte (o texto, o teste, o número) antes de afirmar.
Palavra de dois sentidos :: Termo que o original deixa aberto (anōthen, katelaben, pneuma).
Metáfora de estudo :: Ponte que ajuda a pensar, sem ser prova nem doutrina.
Teste como espelho :: Frase executável que lembra o que prometemos a quem usa o código.
:::
