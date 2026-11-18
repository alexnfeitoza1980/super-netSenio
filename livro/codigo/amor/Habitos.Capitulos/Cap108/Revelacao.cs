namespace Jornada.Habitos.Cap108;

/// <summary>O que a pessoa DISSE que importa (1 = mais importante).</summary>
public sealed record Prioridade(string Area, int Ordem);

/// <summary>O que o calendário e os registros MOSTRAM: tempo gasto num dia, numa área.</summary>
public sealed record Registro(DateOnly Dia, string Area, int Minutos);

public sealed record Comparacao(string Area, int OrdemDeclarada, int OrdemObservada, int Minutos, double Parte)
{
    public int Diferenca => OrdemObservada - OrdemDeclarada;
}

public sealed record AreaSemPrioridade(string Area, int Minutos, double Parte);

public sealed record Relatorio(
    IReadOnlyList<Comparacao> Declaradas,
    IReadOnlyList<AreaSemPrioridade> NaoDeclaradas,
    int TotalDeMinutos);

public static class Revelacao
{
    public static Relatorio Comparar(IEnumerable<Prioridade> declaradas, IEnumerable<Registro> registros)
    {
        var prioridades = declaradas.OrderBy(p => p.Ordem).ToList();

        // 1) Soma o tempo por área.
        var porArea = registros
            .GroupBy(r => r.Area)
            .Select(g => new { Area = g.Key, Minutos = g.Sum(r => r.Minutos) })
            .ToList();
        int total = porArea.Sum(a => a.Minutos);

        // 2) GroupJoin: para cada prioridade, o grupo (possivelmente vazio) do tempo registrado nela.
        var comTempo = prioridades
            .GroupJoin(porArea, p => p.Area, a => a.Area,
                       (p, grupo) => new { p.Area, p.Ordem, Minutos = grupo.Sum(a => a.Minutos) })
            .ToList();

        // 3) A ordem observada entre as áreas declaradas: quem recebeu mais tempo vem primeiro.
        var observada = comTempo
            .OrderByDescending(x => x.Minutos)
            .ThenBy(x => x.Ordem)
            .Select((x, indice) => (x.Area, Ordem: indice + 1))
            .ToDictionary(x => x.Area, x => x.Ordem);

        var comparacoes = comTempo
            .Select(x => new Comparacao(x.Area, x.Ordem, observada[x.Area], x.Minutos, Parte(x.Minutos, total)))
            .ToList();

        // 4) Áreas com tempo registrado que ninguém declarou: ExceptBy compara só pela chave.
        var semPrioridade = porArea
            .ExceptBy(prioridades.Select(p => p.Area), a => a.Area)
            .OrderByDescending(a => a.Minutos)
            .Select(a => new AreaSemPrioridade(a.Area, a.Minutos, Parte(a.Minutos, total)))
            .ToList();

        return new Relatorio(comparacoes, semPrioridade, total);
    }

    private static double Parte(int minutos, int total) => total == 0 ? 0 : (double)minutos / total;

    /// <summary>Uma frase curiosa, não acusadora, para cada diferença.</summary>
    public static string Descrever(Comparacao c) => c.Diferenca switch
    {
        0   => $"{c.Area}: o que você disse e o que o calendário mostra caminham juntos ({c.Parte:P0} do tempo).",
        > 0 => $"{c.Area}: você a pôs em {c.OrdemDeclarada}º, mas ela aparece em {c.OrdemObservada}º ({c.Parte:P0} do tempo). O que está no caminho?",
        _   => $"{c.Area}: você a pôs em {c.OrdemDeclarada}º e ela aparece em {c.OrdemObservada}º ({c.Parte:P0} do tempo). Isso é uma escolha ou um acaso?"
    };
}
