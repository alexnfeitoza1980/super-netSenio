namespace Jornada.Habitos.Cap112;

/// <summary>A que uma prática serve. É a "história" que ela conta sobre o que o time valoriza.</summary>
public enum Intencao { Alinhar, Aprender, Cuidar, Entregar, Celebrar }

public sealed record Pratica(string Nome, Intencao Intencao, int Minutos, IReadOnlyList<DayOfWeek> Dias);

/// <summary>Uma prática acontecendo num dia. Sabe se comparar com outra: por data e, no mesmo dia, por nome.</summary>
public sealed record Ocorrencia(DateOnly Dia, Pratica Pratica) : IComparable<Ocorrencia>
{
    public int CompareTo(Ocorrencia? outra)
    {
        if (outra is null) return 1;

        int porDia = Dia.CompareTo(outra.Dia);
        return porDia != 0 ? porDia : string.Compare(Pratica.Nome, outra.Pratica.Nome, StringComparison.Ordinal);
    }
}

public sealed class CalendarioDoTime
{
    private readonly List<Pratica> praticas = new();

    public CalendarioDoTime Adicionar(Pratica pratica) { praticas.Add(pratica); return this; }

    public IReadOnlyList<Pratica> Praticas => praticas;

    /// <summary>Gera, dia a dia, o que acontece entre duas datas. "yield return" entrega um de cada vez.</summary>
    public IEnumerable<Ocorrencia> Ocorrencias(DateOnly de, DateOnly ate)
    {
        for (var dia = de; dia <= ate; dia = dia.AddDays(1))
        {
            foreach (var pratica in praticas.Where(p => p.Dias.Contains(dia.DayOfWeek)))
                yield return new Ocorrencia(dia, pratica);
        }
    }

    /// <summary>Agenda ordenada, sem repetição, usando um SortedSet (que se mantém ordenado sozinho).</summary>
    public IReadOnlyCollection<Ocorrencia> Agenda(DateOnly de, DateOnly ate) =>
        new SortedSet<Ocorrencia>(Ocorrencias(de, ate));

    /// <summary>Quantos minutos por semana cada intenção recebe: o "peso" de cada história que o time conta.</summary>
    public IReadOnlyDictionary<Intencao, int> MinutosPorSemana() =>
        praticas
            .GroupBy(p => p.Intencao)
            .ToDictionary(g => g.Key, g => g.Sum(p => p.Minutos * p.Dias.Count));

    /// <summary>Intenções que nenhuma prática atende. É um convite a conversar, não uma falha.</summary>
    public IReadOnlyList<Intencao> Lacunas() =>
        Enum.GetValues<Intencao>().Except(praticas.Select(p => p.Intencao)).ToList();
}
