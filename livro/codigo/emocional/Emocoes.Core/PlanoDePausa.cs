namespace Jornada.Emocoes;

public sealed record PlanoDePausa
{
    public OpcaoDePausa Opcao { get; }
    public string ProximoPasso { get; }
    public string LimiteOuApoio { get; }

    public PlanoDePausa(OpcaoDePausa opcao, string proximoPasso, string limiteOuApoio)
    {
        if (!Enum.IsDefined(opcao))
            throw new ArgumentOutOfRangeException(nameof(opcao));

        Opcao = opcao;
        ProximoPasso = ExigirTexto(proximoPasso, nameof(proximoPasso));
        LimiteOuApoio = ExigirTexto(limiteOuApoio, nameof(limiteOuApoio));
    }

    private static string ExigirTexto(string valor, string parametro)
    {
        if (string.IsNullOrWhiteSpace(valor))
            throw new ArgumentException("Informe um próximo passo ou use um exemplo neutro.", parametro);

        return valor.Trim();
    }
}
