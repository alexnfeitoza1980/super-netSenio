namespace Jornada.Resiliencia.Cap92;

public sealed class Passo
{
    public string Descricao { get; }
    public int Minutos { get; }
    public bool Concluido { get; private set; }

    public Passo(string descricao, int minutos)
    {
        Descricao = descricao;
        Minutos = minutos;
    }

    internal void Marcar() => Concluido = true;      // só a Meta pode concluir um passo
}

public sealed record Prova(DateOnly Dia, string Descricao);

public sealed class Meta
{
    public const int LimiteDeMinutosPorPasso = 25;

    private readonly List<Passo> passos = new();
    private readonly List<Prova> provas = new();

    public string Titulo { get; }

    public Meta(string titulo)
    {
        if (string.IsNullOrWhiteSpace(titulo))
            throw new ArgumentException("Dê um nome à meta.", nameof(titulo));
        Titulo = titulo.Trim();
    }

    public IReadOnlyList<Passo> Passos => passos;
    public IReadOnlyList<Prova> Provas => provas;

    public Passo? ProximoPasso => passos.FirstOrDefault(p => !p.Concluido);

    public (int Feitos, int Total) Andamento => (passos.Count(p => p.Concluido), passos.Count);

    public Meta AdicionarPasso(string descricao, int minutos)
    {
        if (string.IsNullOrWhiteSpace(descricao))
            throw new ArgumentException("Descreva o passo.", nameof(descricao));
        if (minutos <= 0)
            throw new ArgumentOutOfRangeException(nameof(minutos), "O passo precisa de um tempo maior que zero.");
        if (minutos > LimiteDeMinutosPorPasso)
            throw new ArgumentException(
                $"Um passo precisa caber em {LimiteDeMinutosPorPasso} min. Divida: use AdicionarDividindo.", nameof(minutos));

        passos.Add(new Passo(descricao.Trim(), minutos));
        return this;
    }

    /// <summary>Recebe uma tarefa grande e a quebra em passos que cabem no limite.</summary>
    public Meta AdicionarDividindo(string tarefa, int minutosTotais)
    {
        foreach (var (descricao, minutos) in Dividir(tarefa, minutosTotais))
            AdicionarPasso(descricao, minutos);
        return this;
    }

    public void ConcluirProximo(DateOnly hoje)
    {
        var passo = ProximoPasso ?? throw new InvalidOperationException("Não há passo pendente.");
        passo.Marcar();
        provas.Add(new Prova(hoje, $"Concluí: {passo.Descricao}"));    // cada passo vira uma pequena prova
    }

    /// <summary>Iterador: devolve um passo de cada vez, sem montar a lista inteira antes.</summary>
    public static IEnumerable<(string Descricao, int Minutos)> Dividir(string tarefa, int minutosTotais)
    {
        int partes = (int)Math.Ceiling(minutosTotais / (double)LimiteDeMinutosPorPasso);
        int restante = minutosTotais;

        for (int parte = 1; restante > 0; parte++)
        {
            int minutos = Math.Min(LimiteDeMinutosPorPasso, restante);
            yield return ($"{tarefa} (parte {parte} de {partes})", minutos);
            restante -= minutos;
        }
    }
}
