namespace Jornada.Emocoes;

public sealed record PlanoDeRetomada
{
    public string Tarefa { get; }
    public string AjusteDeContexto { get; }
    public string Apoio { get; }
    public DateTimeOffset RevisarEm { get; }

    public PlanoDeRetomada(
        string tarefa,
        string ajusteDeContexto,
        string apoio,
        DateTimeOffset revisarEm)
    {
        Tarefa = ExigirTexto(tarefa, nameof(tarefa));
        AjusteDeContexto = ExigirTexto(ajusteDeContexto, nameof(ajusteDeContexto));
        Apoio = ExigirTexto(apoio, nameof(apoio));
        if (revisarEm == default)
            throw new ArgumentOutOfRangeException(nameof(revisarEm), "Informe um instante de revisão explícito.");

        RevisarEm = revisarEm;
    }

    private static string ExigirTexto(string valor, string parametro)
    {
        if (string.IsNullOrWhiteSpace(valor))
            throw new ArgumentException("Informe o campo ou use um exemplo neutro.", parametro);

        return valor.Trim();
    }
}
