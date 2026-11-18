namespace Jornada.Resiliencia.Cap94;

public sealed record Situacao(string Resumo, string? Quando = null, string? Onde = null);

/// <summary>Uma lente é um nome e uma função que, dada uma situação, gera perguntas.</summary>
public sealed record Lente<T>(string Nome, Func<T, IEnumerable<string>> Perguntas);

public static class Lentes
{
    public static IReadOnlyList<Lente<Situacao>> Padrao { get; } = new[]
    {
        new Lente<Situacao>("Eu", s => new[]
        {
            $"Como eu estava (sono, alimentação, carga de trabalho) quando \"{s.Resumo}\" aconteceu?",
            "Que experiências anteriores eu poderia reaproveitar?"
        }),

        new Lente<Situacao>("Relações", s => new[]
        {
            "Quem estava envolvido ou poderia ter ajudado?",
            "Alguém combinou algo comigo que não ficou claro?"
        }),

        new Lente<Situacao>("Lugar", s => s.Onde is null
            ? new[] { "Em que ambiente isso aconteceu? Ele facilitou ou atrapalhou?" }
            : new[] { $"Como era estar em \"{s.Onde}\"? Havia barulho, pressa, falta de recurso?" }),

        new Lente<Situacao>("Momento", s => s.Quando is null
            ? new[] { "Em que fase da vida ou da semana isso caiu?" }
            : new[] { $"O que mais estava acontecendo em \"{s.Quando}\"?" }),
    };
}

public static class Conversa
{
    /// <summary>Gera as perguntas de todas as lentes. Exige duas ou mais: uma só vira "a explicação".</summary>
    public static IReadOnlyList<string> Gerar<T>(T situacao, params Lente<T>[] lentes) where T : notnull
    {
        if (lentes.Length < 2)
            throw new ArgumentException("Use pelo menos duas lentes: uma só vira a explicação total.", nameof(lentes));

        return lentes
            .SelectMany(lente => lente.Perguntas(situacao).Select(p => $"[{lente.Nome}] {p}"))
            .ToList();
    }

    public static ILookup<string, string> PorLente<T>(T situacao, IEnumerable<Lente<T>> lentes) where T : notnull =>
        lentes
            .SelectMany(l => l.Perguntas(situacao).Select(p => (l.Nome, Pergunta: p)))
            .ToLookup(x => x.Nome, x => x.Pergunta);
}
