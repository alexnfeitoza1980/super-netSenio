namespace Quatro;

public sealed record Passo(string Descricao, int Minutos);

public sealed class PlanoDoDia
{
    private readonly List<Passo> passos = [];
    public int Disponiveis { get; }
    public int Planejados => passos.Sum(p => p.Minutos);
    public IReadOnlyList<Passo> Passos => passos.ToArray();

    public PlanoDoDia(int disponiveis)
    {
        if (disponiveis < 0) throw new ArgumentOutOfRangeException(nameof(disponiveis));
        Disponiveis = disponiveis;
    }

    public void Incluir(string descricao, int minutos)
    {
        string texto = Regras.Texto(descricao);
        if (minutos <= 0 || minutos > Disponiveis - Planejados)
            throw new ArgumentOutOfRangeException(nameof(minutos), "O passo não cabe no tempo informado.");
        passos.Add(new(texto, minutos));
    }
}
