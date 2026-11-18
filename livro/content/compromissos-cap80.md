---
id: compromissos-cap80
order: 104
kind: capitulo
num: 80
group: C# & Mentalidade · Oficina · O Caderno dos Quatro Compromissos
title: Não sabemos ainda: uma resposta honesta
subtitle: Uma experiência do tema, uma escolha e uma nova capacidade no mesmo programa.
minutes: 45
hue: 168
objectives:
- Compreender a ideia estudada e seu contexto na obra de Ruiz
- Construir a capacidade deste capítulo no Caderno
- Executar o cenário, variar uma entrada e explicar seu resultado
- Distinguir a proposta do autor das regras do software
origem: livros/05-quatro-compromissos-csharp/content/116-cap80.md
---

::: vila Uma página da nossa história
Caio recebe “Preciso disso amanhã”. Sua cabeça completa: impresso, colorido e em três vias. Nenhum desses detalhes foi pedido. Antes de correr para a impressora, ele escreve uma pergunta. O caderno ganha uma capacidade valiosa: representar o que ainda não sabemos.
:::

::: ilustracao img/compromissos/jornada80.svg | Não sabemos ainda: uma resposta honesta: uma escolha visível e sua consequência no caderno.
:::

::: html
<figure style="max-width:420px;margin:2rem auto"><div style="overflow:hidden;aspect-ratio:3/4;border-radius:24px"><img src="img/compromissos/quatro-jornada.png" alt="Perguntar pode construir uma ponte" style="display:block;width:400%;max-width:none;transform:translateX(-50%)"></div><figcaption>Perguntar pode construir uma ponte. Uma imagem para acompanhar a experiência deste compromisso.</figcaption></figure>
:::

## A ideia que vamos experimentar

O terceiro compromisso aborda a tendência de presumir o que os outros querem e tratar a presunção como realidade. Ruiz relaciona esse movimento a mal-entendidos e recomenda perguntar e expressar os próprios desejos. Perguntar não garante que receberemos resposta nem obriga a outra pessoa a concordar.

**Fonte de leitura:** Don Miguel Ruiz, *Os Quatro Compromissos*, páginas físicas 27–31 do PDF fornecido. Este texto é uma explicação própria, não uma citação literal. A paginação de outras edições pode variar.

## Do tema para uma decisão no programa

Uma pergunta nasce pendente. Sua evidência ainda é nula. Não usamos false para representar desconhecimento: uma hipótese refutada é diferente de uma pergunta não investigada. O programa precisa preservar essa distinção antes de qualquer armazenamento ou interface.

::: pense Antes de executar
Descreva o que você espera ver mudar. Qual informação já existe? Qual ainda não existe? O que o programa deve recusar? Escreva sua previsão antes de olhar a saída.
:::

## Vamos fazer acontecer

Execute **a partir da pasta deste livro**. Todos os comandos cap74 a cap87 existem no mesmo projeto. Os cenários começam com dados novos e fictícios para serem reproduzíveis; o diário pessoal persistente é apresentado no laboratório.

```bash
dotnet run --project codigo/compromissos/Caderno.Console -- cap80
```

O trecho abaixo está dentro do método `Cap80` em `codigo/compromissos/Caderno.Console/Exemplos.cs`. Usa `using Quatro;`. É um corpo de método, não um arquivo independente: não o cole ao lado das declarações das classes.

```cs
var caderno = new Caderno();
var pergunta = caderno.Perguntar("A entrega precisa ser impressa?");
Console.WriteLine(pergunta.Estado);
Console.WriteLine(pergunta.Evidencia is null ? "Ainda não sabemos" : "Há um registro");
```

### Leia o que o código faz

`EstadoDaPergunta` representa três alternativas; o `?` de `Evidencia?` declara que a referência pode estar ausente. O operador `is null` verifica a ausência. Não é a mesma coisa que um texto vazio. A pessoa ainda pode registrar uma pergunta ruim: o construtor valida conteúdo presente, não clareza ou relevância.

Observe os verbos do código: registrar, perguntar, escolher, cumprir, salvar. Cada um deve produzir uma mudança que você consiga explicar. Execute de novo: nos cenários, os dados são recriados. Para conservar sua própria prática, use os comandos do laboratório.

### A saída real desta experiência

```saida
Pendente
Ainda não sabemos
```

Compare sua previsão com a execução. Não basta obter a mesma tela: explique qual instrução produziu cada linha e qual objeto foi consultado. Troque apenas uma entrada por vez para observar a causa da diferença.

## Por dentro da capacidade

O arquivo de referência é `codigo/compromissos/Caderno.Core/Pergunta.cs`. Alguns membros serão usados nos próximos capítulos; hoje concentre-se no caminho percorrido pela experiência. A definição completa evita que o leitor precise adivinhar membros omitidos.

```cs
namespace Quatro;

public enum EstadoDaPergunta { Pendente, Confirmada, Refutada }
public sealed record Evidencia(string Resposta, string Fonte);

public sealed class Pergunta
{
    public string Texto { get; }
    public EstadoDaPergunta Estado { get; private set; }
    public Evidencia? Evidencia { get; private set; }

    public Pergunta(string texto) => Texto = Regras.Texto(texto);

    public void Esclarecer(bool confirmou, string resposta, string fonte)
    {
        if (Estado != EstadoDaPergunta.Pendente)
            throw new InvalidOperationException("Registre outra pergunta para uma nova investigação.");
        var evidencia = new Evidencia(Regras.Texto(resposta), Regras.Texto(fonte));
        Evidencia = evidencia;
        Estado = confirmou ? EstadoDaPergunta.Confirmada : EstadoDaPergunta.Refutada;
    }
}
```



## Agora escolha outro caminho

::: exercicio Experimente e explique
Explique por que começar com EstadoDaPergunta.Refutada produziria uma informação incorreta.

Altere o método Cap80 em uma cópia do projeto, execute o mesmo comando e compare com sua previsão. Quando houver uma exceção esperada, capture-a com `catch (ArgumentException erro)` ou `catch (InvalidOperationException erro)`, conforme a regra estudada, e mostre `erro.Message`.
--- resposta
O programa afirmaria que a hipótese já foi examinada e negada. Pendente preserva a ausência de investigação sem inventar uma resposta.

Procure o teste correspondente em `codigo/compromissos/Caderno.Testes/Program.cs`. Os testes exercitam entradas, mudanças e falhas. Eles não comprovam a filosofia do autor: verificam o contrato que escolhemos implementar.
:::

## Limite da analogia

Um campo preenchido não transforma uma suposição em verdade. A aplicação registra o caminho de esclarecimento. Não extrai pensamentos da outra pessoa nem elimina a necessidade de conversar.

## O que entrou na sua mochila

`EstadoDaPergunta` representa três alternativas; o `?` de `Evidencia?` declara que a referência pode estar ausente. Agora você consegue relacionar uma necessidade humana, uma regra escolhida e um resultado executável. Na próxima etapa reutilizaremos os objetos; não recomeçaremos um curso de sintaxe.

::: quiz Confira sua leitura e sua execução
? O que null significa em Evidencia?
+ Ainda não há evidência registrada.
- O resultado prova uma qualidade moral da pessoa.
- O programa decide sozinho o significado do texto.
- Não há uma regra verificável para esse caso.
> Consulte a experiência e a implementação deste capítulo; o alcance da regra é específico.

? Por que Pendente não é Refutada?
+ Ausência de investigação não é uma resposta negativa.
- O resultado prova uma qualidade moral da pessoa.
- O programa decide sozinho o significado do texto.
- Não há uma regra verificável para esse caso.
> Consulte a experiência e a implementação deste capítulo; o alcance da regra é específico.

? Uma pergunta preenchida já foi esclarecida?
+ Não; registrar a pergunta não registra uma conclusão.
- O resultado prova uma qualidade moral da pessoa.
- O programa decide sozinho o significado do texto.
- Não há uma regra verificável para esse caso.
> Consulte a experiência e a implementação deste capítulo; o alcance da regra é específico.

? O que o símbolo ? declara em Evidencia??
+ Que a referência pode estar ausente.
- O resultado prova uma qualidade moral da pessoa.
- O programa decide sozinho o significado do texto.
- Não há uma regra verificável para esse caso.
> Consulte a experiência e a implementação deste capítulo; o alcance da regra é específico.

? Explique por que começar com EstadoDaPergunta.Refutada produziria uma informação incorreta.?
+ O programa afirmaria que a hipótese já foi examinada e negada.
- O programa avalia a intenção da pessoa
- Nada pode ser observado pela execução
- A linguagem decide o sentido do texto
> Reproduza a variação descrita no exercício e acompanhe o estado.

? Qual limite precisa acompanhar esta experiência?
+ Um campo preenchido não transforma uma suposição em verdade. A aplicação registra o caminho de esclarecimento. Não extrai pensamentos da outra pessoa nem elimina a necessidade de conversar.
- O estado do programa certifica o desenvolvimento pessoal.
- O código define como toda pessoa deve sentir e agir.
- Não precisamos distinguir texto e aplicação.
> A utilidade da comparação depende de manter claro o que o modelo representa e o que não representa.
:::

::: cartas Lembre sem decorar
Nullability :: `EstadoDaPergunta` representa três alternativas; o `?` de `Evidencia?` declara que a referência pode estar ausente.
desconhecido :: Uma pergunta nasce pendente.
estado explícito :: Um campo preenchido não transforma uma suposição em verdade.
pergunta :: O programa afirmaria que a hipótese já foi examinada e negada.
Passagem estudada :: Páginas físicas 27–31 do PDF recebido; explicação e exemplos são próprios.
Experiência executável :: dotnet run --project codigo/compromissos/Caderno.Console -- cap80
Variação para experimentar :: Explique por que começar com EstadoDaPergunta.Refutada produziria uma informação incorreta.
Limite desta comparação :: Um campo preenchido não transforma uma suposição em verdade. A aplicação registra o caminho de esclarecimento. Não extrai pensamentos da outra pessoa nem elimina a necessidade de conversar.
:::
