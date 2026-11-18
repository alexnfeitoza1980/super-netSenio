---
id: compromissos-cap83
order: 107
kind: capitulo
num: 83
group: C# & Mentalidade · Oficina · O Caderno dos Quatro Compromissos
title: Praticar outra vez não apaga o que você aprendeu
subtitle: Uma experiência do tema, uma escolha e uma nova capacidade no mesmo programa.
minutes: 45
hue: 168
objectives:
- Compreender a ideia estudada e seu contexto na obra de Ruiz
- Construir a capacidade deste capítulo no Caderno
- Executar o cenário, variar uma entrada e explicar seu resultado
- Distinguir a proposta do autor das regras do software
origem: livros/05-quatro-compromissos-csharp/content/119-cap83.md
---

::: vila Uma página da nossa história
Lia praticou na segunda e voltou na quarta. Um contador de sequência poderia anunciar que ela voltou a zero. Seu caderno conta outra história: há dois dias de prática e dois aprendizados. O intervalo existe, mas não apaga o primeiro registro.
:::

::: ilustracao img/compromissos/jornada83.svg | Praticar outra vez não apaga o que você aprendeu: uma escolha visível e sua consequência no caderno.
:::

## A ideia que vamos experimentar

Ruiz conecta o quarto compromisso à repetição dos demais. Na discussão da mudança de acordos, descreve um processo paciente. O convite à prática não exige um placar de perfeição. Nosso diário transforma essa ideia em registro de ações e aprendizados, preservando inclusive o retorno depois de uma pausa.

**Fonte de leitura:** Don Miguel Ruiz, *Os Quatro Compromissos*, páginas físicas 34–37; 43–44 do PDF fornecido. Este texto é uma explicação própria, não uma citação literal. A paginação de outras edições pode variar.

## Do tema para uma decisão no programa

Cada prática reúne dia, ação e aprendizado. Várias práticas no mesmo dia são permitidas. A quantidade de dias é calculada sobre datas distintas. Assim, distinguimos volume de registros e número de dias sem confundir os dois. Não inventamos uma fórmula de progresso espiritual.

::: pense Antes de executar
Descreva o que você espera ver mudar. Qual informação já existe? Qual ainda não existe? O que o programa deve recusar? Escreva sua previsão antes de olhar a saída.
:::

## Vamos fazer acontecer

Execute **a partir da pasta deste livro**. Todos os comandos cap74 a cap87 existem no mesmo projeto. Os cenários começam com dados novos e fictícios para serem reproduzíveis; o diário pessoal persistente é apresentado no laboratório.

```bash
dotnet run --project codigo/compromissos/Caderno.Console -- cap83
```

O trecho abaixo está dentro do método `Cap83` em `codigo/compromissos/Caderno.Console/Exemplos.cs`. Usa `using Quatro;`. É um corpo de método, não um arquivo independente: não o cole ao lado das declarações das classes.

```cs
var caderno = new Caderno();
caderno.Diario.Registrar(new DateOnly(2026, 11, 1), "Perguntei antes de supor", "Entendi o pedido");
caderno.Diario.Registrar(new DateOnly(2026, 11, 3), "Voltei a praticar", "Uma pausa não apaga o aprendizado");
Console.WriteLine($"Dias praticados: {caderno.Diario.DiasPraticados}");
Console.WriteLine($"Registros preservados: {caderno.Diario.Registros.Count}");
```

### Leia o que o código faz

`Select` projeta os registros nas datas, `Distinct` remove repetições e `Count` conta. Essa cadeia responde uma pergunta concreta. As datas são passadas ao método para permitir exemplos reproduzíveis. O console pessoal, apresentado no laboratório, usa a data local da máquina e informa essa diferença.

Observe os verbos do código: registrar, perguntar, escolher, cumprir, salvar. Cada um deve produzir uma mudança que você consiga explicar. Execute de novo: nos cenários, os dados são recriados. Para conservar sua própria prática, use os comandos do laboratório.

### A saída real desta experiência

```saida
Dias praticados: 2
Registros preservados: 2
```

Compare sua previsão com a execução. Não basta obter a mesma tela: explique qual instrução produziu cada linha e qual objeto foi consultado. Troque apenas uma entrada por vez para observar a causa da diferença.

## Por dentro da capacidade

O arquivo de referência é `codigo/compromissos/Caderno.Core/Diario.cs`. Alguns membros serão usados nos próximos capítulos; hoje concentre-se no caminho percorrido pela experiência. A definição completa evita que o leitor precise adivinhar membros omitidos.

```cs
namespace Quatro;

public sealed record Pratica(DateOnly Dia, string Acao, string Aprendizado);

public sealed class DiarioDePratica
{
    private readonly List<Pratica> registros = [];
    public IReadOnlyList<Pratica> Registros => registros.ToArray();
    public int DiasPraticados => registros.Select(r => r.Dia).Distinct().Count();

    public void Registrar(DateOnly dia, string acao, string aprendizado) =>
        registros.Add(new(dia, Regras.Texto(acao), Regras.Texto(aprendizado)));
}
```



## Agora escolha outro caminho

::: exercicio Experimente e explique
Registre duas ações na mesma data e uma terceira dois dias depois. Preveja o número de registros e de dias praticados antes de executar.

Altere o método Cap83 em uma cópia do projeto, execute o mesmo comando e compare com sua previsão. Quando houver uma exceção esperada, capture-a com `catch (ArgumentException erro)` ou `catch (InvalidOperationException erro)`, conforme a regra estudada, e mostre `erro.Message`.
--- resposta
São três registros e dois dias distintos. Uma pausa não remove nenhuma entrada. Os testes cobrem tanto repetição de datas quanto retorno após intervalo.

Procure o teste correspondente em `codigo/compromissos/Caderno.Testes/Program.cs`. Os testes exercitam entradas, mudanças e falhas. Eles não comprovam a filosofia do autor: verificam o contrato que escolhemos implementar.
:::

## Limite da analogia

Uma contagem não comprova a qualidade da prática. Duas pessoas não devem ser classificadas por esse número. Falta de registro também não prova ausência de esforço ou aprendizagem.

## O que entrou na sua mochila

`Select` projeta os registros nas datas, `Distinct` remove repetições e `Count` conta. Agora você consegue relacionar uma necessidade humana, uma regra escolhida e um resultado executável. Na próxima etapa reutilizaremos os objetos; não recomeçaremos um curso de sintaxe.

::: quiz Confira sua leitura e sua execução
? Duas práticas no mesmo dia contam quantos dias?
+ Um dia distinto e dois registros.
- O resultado prova uma qualidade moral da pessoa.
- O programa decide sozinho o significado do texto.
- Não há uma regra verificável para esse caso.
> Consulte a experiência e a implementação deste capítulo; o alcance da regra é específico.

? Uma pausa apaga as práticas anteriores?
+ Não; o diário preserva os registros.
- O resultado prova uma qualidade moral da pessoa.
- O programa decide sozinho o significado do texto.
- Não há uma regra verificável para esse caso.
> Consulte a experiência e a implementação deste capítulo; o alcance da regra é específico.

? O que Select projeta na consulta?
+ A data de cada prática.
- O resultado prova uma qualidade moral da pessoa.
- O programa decide sozinho o significado do texto.
- Não há uma regra verificável para esse caso.
> Consulte a experiência e a implementação deste capítulo; o alcance da regra é específico.

? DiasPraticados mede felicidade?
+ Não; apenas conta datas distintas registradas.
- O resultado prova uma qualidade moral da pessoa.
- O programa decide sozinho o significado do texto.
- Não há uma regra verificável para esse caso.
> Consulte a experiência e a implementação deste capítulo; o alcance da regra é específico.

? Registre duas ações na mesma data e uma terceira dois dias depois?
+ São três registros e dois dias distintos.
- O programa avalia a intenção da pessoa
- Nada pode ser observado pela execução
- A linguagem decide o sentido do texto
> Reproduza a variação descrita no exercício e acompanhe o estado.

? Qual limite precisa acompanhar esta experiência?
+ Uma contagem não comprova a qualidade da prática. Duas pessoas não devem ser classificadas por esse número. Falta de registro também não prova ausência de esforço ou aprendizagem.
- O estado do programa certifica o desenvolvimento pessoal.
- O código define como toda pessoa deve sentir e agir.
- Não precisamos distinguir texto e aplicação.
> A utilidade da comparação depende de manter claro o que o modelo representa e o que não representa.
:::

::: cartas Lembre sem decorar
Projeção :: `Select` projeta os registros nas datas, `Distinct` remove repetições e `Count` conta.
Distinct :: Cada prática reúne dia, ação e aprendizado.
diário :: Uma contagem não comprova a qualidade da prática.
recomeço :: São três registros e dois dias distintos.
Passagem estudada :: Páginas físicas 34–37; 43–44 do PDF recebido; explicação e exemplos são próprios.
Experiência executável :: dotnet run --project codigo/compromissos/Caderno.Console -- cap83
Variação para experimentar :: Registre duas ações na mesma data e uma terceira dois dias depois. Preveja o número de registros e de dias praticados antes de executar.
Limite desta comparação :: Uma contagem não comprova a qualidade da prática. Duas pessoas não devem ser classificadas por esse número. Falta de registro também não prova ausência de esforço ou aprendizagem.
:::
