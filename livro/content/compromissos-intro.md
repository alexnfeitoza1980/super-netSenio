---
id: compromissos-intro
order: 97
kind: intro
group: C# & Mentalidade · Oficina · O Caderno dos Quatro Compromissos
title: Os Quatro Compromissos em C# — apresentação
subtitle: Uma história, um programa e escolhas que você pode executar.
minutes: 15
hue: 168
badge: ★
objectives:
- Entender o tema antes de construir o modelo
- Percorrer os 14 capítulos no mesmo programa
- Executar, variar e observar sem confundir metáfora com mecanismo
origem: livros/05-quatro-compromissos-csharp/content/00-intro.md
---

## Você não vai apenas ler uma explicação

Imagine receber uma opinião e perceber que a transformou numa definição de quem você é. Imagine supor um pedido, fazer perguntas e descobrir que estava construindo outra coisa. Vamos viver essas situações com Lia e Caio e dar forma a elas em C#.

O programa se chama **Caderno dos Quatro Compromissos**. Nele você registra acordos recebidos, escolhe quais quer praticar, cuida da palavra, distingue observação de interpretação, pergunta antes de concluir e registra ações e aprendizados. A cada capítulo o mesmo projeto ganha uma capacidade. O laboratório permite guardar sua própria prática entre execuções.

::: ilustracao img/compromissos/quatro-jornada.png | Quatro movimentos para experimentar: cuidar da palavra, preservar a identidade, perguntar e praticar.
:::

## A fonte tem uma dimensão espiritual

Don Miguel Ruiz apresenta uma proposta de liberdade pessoal em linguagem espiritual: sonho coletivo, domesticação, Juiz, Vítima, palavra, transformação, novo sonho e orações. Explicamos essas imagens no contexto do autor. Não as tratamos como mecanismos comprovados da mente, nem as apagamos para transformar a obra num manual corporativo.

O código começa nas perguntas provocadas pela leitura. Não mede felicidade, fé ou caráter. Um acordo escolhido no programa é um registro da escolha, não a prova de que um hábito desapareceu. A experiência é original e não substitui nem reproduz integralmente a obra de Ruiz.

## O que você já precisa saber

Este é um volume de aplicação com progressão própria, não uma reorganização já concluída da coleção inteira. É útil conhecer variáveis, condições, métodos e noções de classe. Há links para o [Livro 1](#/c/aprenda-intro) quando precisar revisar. Aqui aprofundamos estados, coleções, consultas, validação, composição, contratos e persistência. Não ensinamos outra vez todo o vocabulário básico.

Use **.NET SDK 10**, sem pacotes externos. As verificações são um executável próprio, não xUnit; use o comando indicado, pois `dotnet test` não executa essa suíte.

```bash
dotnet build codigo/compromissos/Caderno.slnx -m:1
dotnet run --project codigo/compromissos/Caderno.Console -- cap74
dotnet run --project codigo/compromissos/Caderno.Testes
```

## O mapa da experiência

| Capítulo | Experiência | Peça do programa | Avanço técnico |
|---|---|---|---|
| 74 | Os acordos que chegaram antes de nós | `Inventario` | Identidade, coleção privada, record, busca |
| 75 | O juiz interno encontra uma página em branco | `Reflexao` | Validação, construtor, observação, interpretação |
| 76 | Sua palavra começa a ter estado | `Promessa` | Estado, encapsulamento, enum, método |
| 77 | Mudar o combinado sem apagar o que foi dito | `Promessa` | Histórico, pré-condição, imutabilidade, falha sem mutação |
| 78 | Uma opinião não precisa virar sua identidade | `Resposta` | Composição, dados pessoais, limites, resposta escolhida |
| 79 | Escolher o que fica, sem apagar o passado | `Inventario` | LINQ, materialização, busca, decisão consciente |
| 80 | Não sabemos ainda: uma resposta honesta | `Pergunta` | Nullability, desconhecido, estado explícito, pergunta |
| 81 | Perguntar, ouvir e guardar a fonte | `Pergunta` | Evidência, rastreabilidade, validação antes da mudança, acesso condicional |
| 82 | O melhor de hoje cabe em quanto tempo? | `Plano` | Agregação, unidades, limite declarado, regra de negócio |
| 83 | Praticar outra vez não apaga o que você aprendeu | `Diario` | Projeção, Distinct, diário, recomeço |
| 84 | O que fica apenas na memória pode se perder | `Arquivo` | Interface, DTO, serialização, memória volátil |
| 85 | Rever pendências sem transformar tudo em culpa | `Caderno` | Consulta, ordenação, histórico, visão |
| 86 | Fechar o programa e encontrar o caminho de volta | `Arquivo` | Persistência, JSON, versão, tratamento de erros |
| 87 | O novo sonho vira uma jornada executável | `Caderno` | Integração, responsabilidades, cenário, limites do modelo |

Os números 74 a 87 e Lab 9 foram preservados para manter os links da coleção. Este piloto não altera a ordem dos demais livros.

## Quatro passos para estudar cada capítulo

1. Leia a cena e a explicação da ideia de Ruiz. Localize o que é relato do autor e o que é nossa aplicação.
2. Preveja uma mudança no programa antes de executá-lo.
3. Rode o comando, acompanhe o código e compare a saída real.
4. Mude uma entrada, explique o efeito e experimente a pergunta na vida, se fizer sentido para você.

As imagens mostram escolhas; o código torna algumas consequências observáveis. Não existe pontuação moral nem ranking de pessoas. Os arquivos pessoais ficam locais, mas são texto simples: comece com exemplos fictícios.

## E o Angular?

Consultamos a documentação oficial de componentes, signals e HTTP para preparar uma fronteira futura: o núcleo pode continuar em .NET e uma interface Angular pode conversar com uma API. **Esta entrega não contém aplicativo Angular, banco de dados nem microserviços.** Primeiro vamos avaliar se a experiência de aprendizagem funciona. Veja o mapa de evolução em `ESTUDO/decisoes.md` e as fontes em `ESTUDO/fontes.md`.
