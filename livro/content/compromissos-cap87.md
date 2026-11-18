---
id: compromissos-cap87
order: 111
kind: capitulo
num: 87
group: C# & Mentalidade · Oficina · O Caderno dos Quatro Compromissos
title: O novo sonho vira uma jornada executável
subtitle: Uma experiência do tema, uma escolha e uma nova capacidade no mesmo programa.
minutes: 45
hue: 168
objectives:
- Compreender a ideia estudada e seu contexto na obra de Ruiz
- Construir a capacidade deste capítulo no Caderno
- Executar o cenário, variar uma entrada e explicar seu resultado
- Distinguir a proposta do autor das regras do software
origem: livros/05-quatro-compromissos-csharp/content/123-cap87.md
---

::: vila Uma página da nossa história
Agora Lia não abre quatorze programinhas. Ela abre um caderno: escolhe um acordo, faz uma pergunta, ouve a resposta, promete algo possível e registra o que aprendeu. Nenhuma dessas ações garante uma vida perfeita. Juntas, elas tornam visível uma maneira de praticar e revisar suas escolhas.
:::

::: ilustracao img/compromissos/jornada87.svg | O novo sonho vira uma jornada executável: uma escolha visível e sua consequência no caderno.
:::

## A ideia que vamos experimentar

O encerramento de Ruiz apresenta o “novo sonho” como uma visão espiritual de viver com amor e liberdade e inclui orações. Não reduzimos esse encerramento a uma técnica de produtividade. O que programamos é um instrumento de registro inspirado na passagem, não a experiência espiritual nem uma promessa de felicidade.

**Fonte de leitura:** Don Miguel Ruiz, *Os Quatro Compromissos*, páginas físicas 49–54 do PDF fornecido. Este texto é uma explicação própria, não uma citação literal. A paginação de outras edições pode variar.

## Do tema para uma decisão no programa

O cenário final percorre os quatro compromissos: escolha consciente de como usar a palavra, espaço para ouvir, pergunta antes da conclusão e prática em uma ação concreta. Todos os objetos entram no mesmo Caderno. A exportação permite guardar o conjunto sem depender de uma tela específica.

::: pense Antes de executar
Descreva o que você espera ver mudar. Qual informação já existe? Qual ainda não existe? O que o programa deve recusar? Escreva sua previsão antes de olhar a saída.
:::

## Vamos fazer acontecer

Execute **a partir da pasta deste livro**. Todos os comandos cap74 a cap87 existem no mesmo projeto. Os cenários começam com dados novos e fictícios para serem reproduzíveis; o diário pessoal persistente é apresentado no laboratório.

```bash
dotnet run --project codigo/compromissos/Caderno.Console -- cap87
```

O trecho abaixo está dentro do método `Cap87` em `codigo/compromissos/Caderno.Console/Exemplos.cs`. Usa `using Quatro;`. É um corpo de método, não um arquivo independente: não o cole ao lado das declarações das classes.

```cs
var caderno = new Caderno();
var acordo = caderno.Inventario.Registrar("Perguntar antes de adivinhar", "Prática consciente");
caderno.Inventario.Escolher(acordo.Id);
var pergunta = caderno.Perguntar("Qual formato você prefere?");
pergunta.Esclarecer(true, "Um PDF", "Resposta direta da pessoa");
var promessa = caderno.Prometer("Preparar o PDF", new DateOnly(2026, 11, 6));
promessa.Cumprir(new DateOnly(2026, 11, 5));
caderno.Diario.Registrar(new DateOnly(2026, 11, 5), "Perguntei e cumpri o combinado", "Ouvir mudou meu plano");
var r = caderno.Exportar();
Console.WriteLine($"Acordos: {r.Acordos.Length}; perguntas: {r.Perguntas.Length}");
Console.WriteLine($"Promessa: {r.Promessas[0].Estado}; práticas: {r.Praticas.Length}");
```

### Leia o que o código faz

A composição concentra o fluxo sem colocar todas as regras em Program.cs. Promessa controla seus estados, Pergunta exige evidência e Diario preserva práticas. O console coordena os objetos. Não criamos um padrão de projeto por capítulo: interfaces e DTOs apareceram quando surgiram necessidades de troca e persistência.

Observe os verbos do código: registrar, perguntar, escolher, cumprir, salvar. Cada um deve produzir uma mudança que você consiga explicar. Execute de novo: nos cenários, os dados são recriados. Para conservar sua própria prática, use os comandos do laboratório.

### A saída real desta experiência

```saida
Acordos: 1; perguntas: 1
Promessa: Cumprida; práticas: 1
```

Compare sua previsão com a execução. Não basta obter a mesma tela: explique qual instrução produziu cada linha e qual objeto foi consultado. Troque apenas uma entrada por vez para observar a causa da diferença.

## Por dentro da capacidade

O arquivo de referência é `codigo/compromissos/Caderno.Core/Caderno.cs`. Alguns membros serão usados nos próximos capítulos; hoje concentre-se no caminho percorrido pela experiência. A definição completa evita que o leitor precise adivinhar membros omitidos.

```cs
namespace Quatro;

// DTOs: retratos transportáveis, separados dos objetos de trabalho.
public sealed record PromessaSalva(Guid Id, string Descricao, DateOnly Prazo,
    EstadoDaPromessa Estado, DateOnly? ConcluidaEm, string? MotivoDoCancelamento,
    RevisaoDePrazo[] Revisoes);
public sealed record PerguntaSalva(string Texto, EstadoDaPergunta Estado, Evidencia? Evidencia);
public sealed record Retrato(int Versao, Acordo[] Acordos, PromessaSalva[] Promessas,
    Reflexao[] Reflexoes, RespostaEscolhida[] Respostas, PerguntaSalva[] Perguntas, Pratica[] Praticas);

public sealed class Caderno
{
    public Inventario Inventario { get; } = new();
    public DiarioDePratica Diario { get; } = new();
    private readonly List<Promessa> promessas = [];
    private readonly List<Reflexao> reflexoes = [];
    private readonly List<RespostaEscolhida> respostas = [];
    private readonly List<Pergunta> perguntas = [];
    public IReadOnlyList<Promessa> Promessas => promessas.ToArray();
    public IReadOnlyList<Pergunta> Perguntas => perguntas.ToArray();

    public Promessa Prometer(string descricao, DateOnly prazo)
    {
        var p = new Promessa(descricao, prazo);
        promessas.Add(p);
        return p;
    }
    public Pergunta Perguntar(string texto)
    {
        var p = new Pergunta(texto);
        perguntas.Add(p);
        return p;
    }
    public void Refletir(Reflexao reflexao) => reflexoes.Add(reflexao ?? throw new ArgumentNullException(nameof(reflexao)));
    public void Responder(RespostaEscolhida resposta) => respostas.Add(resposta ?? throw new ArgumentNullException(nameof(resposta)));
    public IReadOnlyList<Promessa> Pendentes() => promessas
        .Where(p => p.Estado == EstadoDaPromessa.Aberta).OrderBy(p => p.Prazo)
        .ThenBy(p => p.Descricao, StringComparer.Ordinal).ToArray();

    public Retrato Exportar() => new(1, Inventario.Todos.ToArray(), promessas.Select(p =>
        new PromessaSalva(p.Id, p.Descricao, p.Prazo, p.Estado, p.ConcluidaEm,
            p.MotivoDoCancelamento, p.Revisoes.ToArray())).ToArray(),
        reflexoes.ToArray(), respostas.ToArray(), perguntas.Select(p =>
            new PerguntaSalva(p.Texto, p.Estado, p.Evidencia)).ToArray(), Diario.Registros.ToArray());
}
```



## Agora escolha outro caminho

::: exercicio Experimente e explique
Antes de olhar a saída, liste quais dados devem existir depois do cenário: acordo escolhido, pergunta esclarecida, promessa cumprida e prática registrada. Acrescente uma segunda pergunta pendente.

Altere o método Cap87 em uma cópia do projeto, execute o mesmo comando e compare com sua previsão. Quando houver uma exceção esperada, capture-a com `catch (ArgumentException erro)` ou `catch (InvalidOperationException erro)`, conforme a regra estudada, e mostre `erro.Message`.
--- resposta
O retrato passa a ter duas perguntas com estados diferentes. A nova dúvida não desfaz a conclusão da anterior. Esse é um bom teste de que aprendemos a representar desconhecimento sem apagar o histórico.

Procure o teste correspondente em `codigo/compromissos/Caderno.Testes/Program.cs`. Os testes exercitam entradas, mudanças e falhas. Eles não comprovam a filosofia do autor: verificam o contrato que escolhemos implementar.
:::

## Limite da analogia

O resultado não mede amor, liberdade, fé ou felicidade. Ele registra declarações e ações informadas por uma pessoa. A obra de Ruiz permanece uma fonte a ser lida em sua própria amplitude; este livro é uma experiência original de programação, não sua reprodução integral.

## O que entrou na sua mochila

A composição concentra o fluxo sem colocar todas as regras em Program.cs. Agora você consegue relacionar uma necessidade humana, uma regra escolhida e um resultado executável. Na próxima etapa reutilizaremos os objetos; não recomeçaremos um curso de sintaxe.

::: quiz Confira sua leitura e sua execução
? Quantos cadernos o cenário final usa?
+ Um, reunindo os objetos dos quatro temas.
- O resultado prova uma qualidade moral da pessoa.
- O programa decide sozinho o significado do texto.
- Não há uma regra verificável para esse caso.
> Consulte a experiência e a implementação deste capítulo; o alcance da regra é específico.

? Quem controla as transições de uma promessa?
+ A classe Promessa, não uma tela que altera o estado livremente.
- O resultado prova uma qualidade moral da pessoa.
- O programa decide sozinho o significado do texto.
- Não há uma regra verificável para esse caso.
> Consulte a experiência e a implementação deste capítulo; o alcance da regra é específico.

? O resultado comprova liberdade espiritual?
+ Não; mostra registros e comportamentos do programa.
- O resultado prova uma qualidade moral da pessoa.
- O programa decide sozinho o significado do texto.
- Não há uma regra verificável para esse caso.
> Consulte a experiência e a implementação deste capítulo; o alcance da regra é específico.

? O que a segunda pergunta pendente preserva?
+ A dúvida nova sem desfazer a conclusão da pergunta anterior.
- O resultado prova uma qualidade moral da pessoa.
- O programa decide sozinho o significado do texto.
- Não há uma regra verificável para esse caso.
> Consulte a experiência e a implementação deste capítulo; o alcance da regra é específico.

? Antes de olhar a saída, liste quais dados devem existir depois do cenário: acordo escolhido, pergunta esclarecida, promessa cumprida e prática registrada?
+ O retrato passa a ter duas perguntas com estados diferentes.
- O programa avalia a intenção da pessoa
- Nada pode ser observado pela execução
- A linguagem decide o sentido do texto
> Reproduza a variação descrita no exercício e acompanhe o estado.

? Qual limite precisa acompanhar esta experiência?
+ O resultado não mede amor, liberdade, fé ou felicidade. Ele registra declarações e ações informadas por uma pessoa. A obra de Ruiz permanece uma fonte a ser lida em sua própria amplitude; este livro é uma experiência original de programação, não sua reprodução integral.
- O estado do programa certifica o desenvolvimento pessoal.
- O código define como toda pessoa deve sentir e agir.
- Não precisamos distinguir texto e aplicação.
> A utilidade da comparação depende de manter claro o que o modelo representa e o que não representa.
:::

::: cartas Lembre sem decorar
Integração :: A composição concentra o fluxo sem colocar todas as regras em Program.cs.
responsabilidades :: O cenário final percorre os quatro compromissos: escolha consciente de como usar a palavra, espaço para ouvir, pergunta antes da conclusão e prática em uma ação concreta.
cenário :: O resultado não mede amor, liberdade, fé ou felicidade.
limites do modelo :: O retrato passa a ter duas perguntas com estados diferentes.
Passagem estudada :: Páginas físicas 49–54 do PDF recebido; explicação e exemplos são próprios.
Experiência executável :: dotnet run --project codigo/compromissos/Caderno.Console -- cap87
Variação para experimentar :: Antes de olhar a saída, liste quais dados devem existir depois do cenário: acordo escolhido, pergunta esclarecida, promessa cumprida e prática registrada. Acrescente uma segunda pergunta pendente.
Limite desta comparação :: O resultado não mede amor, liberdade, fé ou felicidade. Ele registra declarações e ações informadas por uma pessoa. A obra de Ruiz permanece uma fonte a ser lida em sua própria amplitude; este livro é uma experiência original de programação, não sua reprodução integral.
:::
