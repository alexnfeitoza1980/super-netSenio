namespace Jornada.Emocoes;

public sealed record RegistroEmocional
{
    public EmocaoNomeada Emocao { get; }
    public string SinalObservado { get; }
    public string Contexto { get; }
    public DateTimeOffset Momento { get; }

    public RegistroEmocional(
        EmocaoNomeada emocao,
        string sinalObservado,
        string contexto,
        DateTimeOffset momento)
    {
        if (!Enum.IsDefined(emocao))
            throw new ArgumentOutOfRangeException(nameof(emocao));

        Emocao = emocao;
        SinalObservado = ExigirTexto(sinalObservado, nameof(sinalObservado));
        Contexto = ExigirTexto(contexto, nameof(contexto));
        Momento = momento;
    }

    private static string ExigirTexto(string valor, string parametro)
    {
        if (string.IsNullOrWhiteSpace(valor))
            throw new ArgumentException("Informe uma descrição ou use um exemplo neutro.", parametro);

        return valor.Trim();
    }
}
