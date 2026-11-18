namespace Jornada.Crencas;

public sealed record PlanoDeExperimento
{
    public string Acao { get; }
    public string OQueObservar { get; }
    public string LimiteOuApoio { get; }

    public PlanoDeExperimento(string acao, string oQueObservar, string limiteOuApoio)
    {
        Acao = Validar(acao, nameof(acao));
        OQueObservar = Validar(oQueObservar, nameof(oQueObservar));
        LimiteOuApoio = Validar(limiteOuApoio, nameof(limiteOuApoio));
    }

    private static string Validar(string valor, string parametro)
    {
        if (string.IsNullOrWhiteSpace(valor))
            throw new ArgumentException("Preencha este campo do plano.", parametro);

        return valor.Trim();
    }
}
