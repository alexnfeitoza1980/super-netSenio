namespace Jornada.Emocoes;

public sealed record ObservacaoEmocional
{
    public string FatoObservado { get; }
    public string Interpretacao { get; }
    public string? EmocaoNomeada { get; }

    public ObservacaoEmocional(string fatoObservado, string interpretacao, string? emocaoNomeada = null)
    {
        FatoObservado = ExigirTexto(fatoObservado, nameof(fatoObservado));
        Interpretacao = ExigirTexto(interpretacao, nameof(interpretacao));
        EmocaoNomeada = emocaoNomeada is null
            ? null
            : ExigirTexto(emocaoNomeada, nameof(emocaoNomeada));
    }

    private static string ExigirTexto(string valor, string parametro)
    {
        if (string.IsNullOrWhiteSpace(valor))
            throw new ArgumentException("Informe uma descrição ou use um exemplo neutro.", parametro);

        return valor.Trim();
    }
}
