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
