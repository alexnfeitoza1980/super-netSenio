namespace Jornada.Resiliencia.Cap93;

public sealed class MapaDeApoio
{
    private readonly List<IApoio> apoios = new();

    public MapaDeApoio Adicionar(IApoio apoio) { apoios.Add(apoio); return this; }

    /// <summary>Quem pode ajudar com isso? Um "foreach" sobre a interface chama o código de cada tipo.</summary>
    public IReadOnlyList<IApoio> Quem(Necessidade necessidade)
    {
        var lista = apoios.Where(a => a.Ajuda(necessidade)).ToList();

        // Em urgência, os serviços públicos sempre entram na lista, mesmo que o mapa esteja vazio.
        if (necessidade == Necessidade.Urgencia)
            lista.AddRange(ServicosPublicos.Brasil.Where(s => s.Ajuda(necessidade) && !lista.Contains(s)));

        return lista;
    }

    /// <summary>Necessidades que ninguém do mapa cobre ainda. É um convite para ampliar a rede, não uma falha.</summary>
    public IReadOnlyList<Necessidade> Lacunas() =>
        Enum.GetValues<Necessidade>()
            .Where(n => n != Necessidade.Urgencia && !apoios.Any(a => a.Ajuda(n)))
            .ToList();
}

/// <summary>Um pedido com começo (quem), meio (o quê, quando) e fim (um limite gentil).</summary>
public sealed record Pedido(string Para, string OQue, string Quando, string Limite)
{
    public string Montar()
    {
        foreach (var (campo, valor) in new[] { (nameof(Para), Para), (nameof(OQue), OQue), (nameof(Quando), Quando), (nameof(Limite), Limite) })
        {
            if (string.IsNullOrWhiteSpace(valor))
                throw new ArgumentException($"Preencha \"{campo}\" para o pedido ficar claro.");
        }

        return $"{Para}, {OQue} {Quando}. {Limite}";
    }
}
