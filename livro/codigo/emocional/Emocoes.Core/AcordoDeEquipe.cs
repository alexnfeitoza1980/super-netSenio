namespace Jornada.Emocoes;

public sealed record AcordoDeEquipe
{
    public string Rotina { get; }
    public string OpcaoDePausa { get; }
    public string ApoioDisponivel { get; }
    public string Revisao { get; }

    public AcordoDeEquipe(
        string rotina,
        string opcaoDePausa,
        string apoioDisponivel,
        string revisao)
    {
        Rotina = ExigirTexto(rotina, nameof(rotina));
        OpcaoDePausa = ExigirTexto(opcaoDePausa, nameof(opcaoDePausa));
        ApoioDisponivel = ExigirTexto(apoioDisponivel, nameof(apoioDisponivel));
        Revisao = ExigirTexto(revisao, nameof(revisao));
    }

    private static string ExigirTexto(string valor, string parametro)
    {
        if (string.IsNullOrWhiteSpace(valor))
            throw new ArgumentException("Descreva o acordo ou use um exemplo fictício.", parametro);

        return valor.Trim();
    }
}
