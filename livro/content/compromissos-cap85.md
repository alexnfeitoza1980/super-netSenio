---
id: compromissos-cap85
order: 109
kind: capitulo
num: 85
group: C# & Mentalidade · Oficina · O Caderno dos Quatro Compromissos
title: Rever pendências sem transformar tudo em culpa
subtitle: Uma experiência do tema, uma escolha e uma nova capacidade no mesmo programa.
minutes: 45
hue: 168
objectives:
- Compreender a ideia estudada e seu contexto na obra de Ruiz
- Construir a capacidade deste capítulo no Caderno
- Executar o cenário, variar uma entrada e explicar seu resultado
- Distinguir a proposta do autor das regras do software
origem: livros/05-quatro-compromissos-csharp/content/121-cap85.md
---

::: vila Uma página da nossa história
Lia abre sua lista e vê uma tarefa já cumprida misturada com duas promessas abertas. Se o programa repetir tudo como cobrança, não ajudará a escolher o próximo passo. Ela quer uma consulta simples: mostrar somente o que está aberto, na ordem dos prazos.
:::

::: ilustracao img/compromissos/jornada85.svg | Rever pendências sem transformar tudo em culpa: uma escolha visível e sua consequência no caderno.
:::

## A ideia que vamos experimentar

O livro usa a imagem de pagar repetidamente pelo mesmo erro para criticar a condenação interna recorrente. Isso não elimina a responsabilidade de reparar danos ou atender obrigações. Nosso recorte separa histórico e pendência: lembrar uma entrega não é tratá-la como se nunca tivesse acontecido.

**Fonte de leitura:** Don Miguel Ruiz, *Os Quatro Compromissos*, páginas físicas 8–9; 34 do PDF fornecido. Este texto é uma explicação própria, não uma citação literal. A paginação de outras edições pode variar.

## Do tema para uma decisão no programa

Consultamos o mesmo Caderno. Os registros completos permanecem no histórico; Pendentes devolve somente promessas abertas. Não chamamos isso de perdão automático. O programa responde a uma pergunta operacional: que compromissos ainda exigem uma decisão?

::: pense Antes de executar
Descreva o que você espera ver mudar. Qual informação já existe? Qual ainda não existe? O que o programa deve recusar? Escreva sua previsão antes de olhar a saída.
:::

## Vamos fazer acontecer

Execute **a partir da pasta deste livro**. Todos os comandos cap74 a cap87 existem no mesmo projeto. Os cenários começam com dados novos e fictícios para serem reproduzíveis; o diário pessoal persistente é apresentado no laboratório.

```bash
dotnet run --project codigo/compromissos/Caderno.Console -- cap85
```

O trecho abaixo está dentro do método `Cap85` em `codigo/compromissos/Caderno.Console/Exemplos.cs`. Usa `using Quatro;`. É um corpo de método, não um arquivo independente: não o cole ao lado das declarações das classes.

```cs
var caderno = new Caderno();
caderno.Prometer("Rever a conversa", new DateOnly(2026, 11, 7));
caderno.Prometer("Fazer uma pergunta", new DateOnly(2026, 11, 5));
var concluida = caderno.Prometer("Enviar resposta", new DateOnly(2026, 11, 4));
concluida.Cumprir(new DateOnly(2026, 11, 4));
foreach (var p in caderno.Pendentes()) Console.WriteLine(p.Descricao);
```

### Leia o que o código faz

`Where` filtra o estado, `OrderBy` ordena pela data e `ThenBy` resolve empates pela descrição. `ToArray` materializa o resultado naquele momento. Se uma promessa mudar depois, outra consulta pode produzir outro conjunto. Não apagamos as cumpridas: elas continuam em Exportar.

Observe os verbos do código: registrar, perguntar, escolher, cumprir, salvar. Cada um deve produzir uma mudança que você consiga explicar. Execute de novo: nos cenários, os dados são recriados. Para conservar sua própria prática, use os comandos do laboratório.

### A saída real desta experiência

```saida
Fazer uma pergunta
Rever a conversa
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
Cancele a primeira promessa com um motivo e faça outra consulta. Ela ainda aparece no retrato exportado?

Altere o método Cap85 em uma cópia do projeto, execute o mesmo comando e compare com sua previsão. Quando houver uma exceção esperada, capture-a com `catch (ArgumentException erro)` ou `catch (InvalidOperationException erro)`, conforme a regra estudada, e mostre `erro.Message`.
--- resposta
A promessa cancelada sai de Pendentes, mas permanece no retrato com seu estado e motivo. Filtro é uma visão; não é exclusão de dados.

Procure o teste correspondente em `codigo/compromissos/Caderno.Testes/Program.cs`. Os testes exercitam entradas, mudanças e falhas. Eles não comprovam a filosofia do autor: verificam o contrato que escolhemos implementar.
:::

## Limite da analogia

Um item sair da consulta não desfaz consequências no mundo. Cancelar uma promessa no sistema não cancela uma obrigação legal ou um acordo com outra pessoa. O histórico e a conversa continuam importantes.

## O que entrou na sua mochila

`Where` filtra o estado, `OrderBy` ordena pela data e `ThenBy` resolve empates pela descrição. Agora você consegue relacionar uma necessidade humana, uma regra escolhida e um resultado executável. Na próxima etapa reutilizaremos os objetos; não recomeçaremos um curso de sintaxe.

::: quiz Confira sua leitura e sua execução
? Pendentes remove os itens cumpridos do histórico?
+ Não; apenas filtra a visão devolvida.
- O resultado prova uma qualidade moral da pessoa.
- O programa decide sozinho o significado do texto.
- Não há uma regra verificável para esse caso.
> Consulte a experiência e a implementação deste capítulo; o alcance da regra é específico.

? Qual estado entra na consulta?
+ Somente Aberta.
- O resultado prova uma qualidade moral da pessoa.
- O programa decide sozinho o significado do texto.
- Não há uma regra verificável para esse caso.
> Consulte a experiência e a implementação deste capítulo; o alcance da regra é específico.

? Como a consulta resolve prazos iguais?
+ ThenBy ordena pela descrição.
- O resultado prova uma qualidade moral da pessoa.
- O programa decide sozinho o significado do texto.
- Não há uma regra verificável para esse caso.
> Consulte a experiência e a implementação deste capítulo; o alcance da regra é específico.

? Cancelar no código encerra qualquer obrigação real?
+ Não; o estado do registro não substitui acordos ou responsabilidades fora do programa.
- O resultado prova uma qualidade moral da pessoa.
- O programa decide sozinho o significado do texto.
- Não há uma regra verificável para esse caso.
> Consulte a experiência e a implementação deste capítulo; o alcance da regra é específico.

? Cancele a primeira promessa com um motivo e faça outra consulta?
+ A promessa cancelada sai de Pendentes, mas permanece no retrato com seu estado e motivo.
- O programa avalia a intenção da pessoa
- Nada pode ser observado pela execução
- A linguagem decide o sentido do texto
> Reproduza a variação descrita no exercício e acompanhe o estado.

? Qual limite precisa acompanhar esta experiência?
+ Um item sair da consulta não desfaz consequências no mundo. Cancelar uma promessa no sistema não cancela uma obrigação legal ou um acordo com outra pessoa. O histórico e a conversa continuam importantes.
- O estado do programa certifica o desenvolvimento pessoal.
- O código define como toda pessoa deve sentir e agir.
- Não precisamos distinguir texto e aplicação.
> A utilidade da comparação depende de manter claro o que o modelo representa e o que não representa.
:::

::: cartas Lembre sem decorar
Consulta :: `Where` filtra o estado, `OrderBy` ordena pela data e `ThenBy` resolve empates pela descrição.
ordenação :: Consultamos o mesmo Caderno.
histórico :: Um item sair da consulta não desfaz consequências no mundo.
visão :: A promessa cancelada sai de Pendentes, mas permanece no retrato com seu estado e motivo.
Passagem estudada :: Páginas físicas 8–9; 34 do PDF recebido; explicação e exemplos são próprios.
Experiência executável :: dotnet run --project codigo/compromissos/Caderno.Console -- cap85
Variação para experimentar :: Cancele a primeira promessa com um motivo e faça outra consulta. Ela ainda aparece no retrato exportado?
Limite desta comparação :: Um item sair da consulta não desfaz consequências no mundo. Cancelar uma promessa no sistema não cancela uma obrigação legal ou um acordo com outra pessoa. O histórico e a conversa continuam importantes.
:::
