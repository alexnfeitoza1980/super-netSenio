namespace Quatro;

public sealed record Reflexao
{
    public string Observacao { get; }
    public string Interpretacao { get; }
    public string ProximoPasso { get; }

    public Reflexao(string observacao, string interpretacao, string proximoPasso)
    {
        Observacao = Regras.Texto(observacao);
        Interpretacao = Regras.Texto(interpretacao);
        ProximoPasso = Regras.Texto(proximoPasso);
    }
}
