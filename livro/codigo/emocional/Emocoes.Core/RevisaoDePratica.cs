namespace Jornada.Emocoes;

public sealed record RevisaoDePratica
{
    public string OQueNotei { get; }
    public string EscolhaFeita { get; }
    public string ProximaTentativa { get; }
    public string? ApoioOuLimite { get; }

    public RevisaoDePratica(
        string oQueNotei,
        string escolhaFeita,
        string proximaTentativa,
        string? apoioOuLimite = null)
    {
        OQueNotei = ExigirTexto(oQueNotei, nameof(oQueNotei));
        EscolhaFeita = ExigirTexto(escolhaFeita, nameof(escolhaFeita));
        ProximaTentativa = ExigirTexto(proximaTentativa, nameof(proximaTentativa));
        ApoioOuLimite = apoioOuLimite is null
            ? null
            : ExigirTexto(apoioOuLimite, nameof(apoioOuLimite));
    }

    private static string ExigirTexto(string valor, string parametro)
    {
        if (string.IsNullOrWhiteSpace(valor))
            throw new ArgumentException("Informe um campo ou escolha não registrar uma revisão.", parametro);

        return valor.Trim();
    }
}
