namespace Jornada.Crencas;

public sealed class Investigacao
{
    private readonly List<Evidencia> evidencias = new();

    public Hipotese Hipotese { get; }
    public IReadOnlyList<Evidencia> Evidencias => evidencias.AsReadOnly();

    public Investigacao(Hipotese hipotese)
    {
        ArgumentNullException.ThrowIfNull(hipotese);
        if (string.IsNullOrWhiteSpace(hipotese.Enunciado))
            throw new ArgumentException("Descreva a hipótese.", nameof(hipotese));
        if (!Enum.IsDefined(hipotese.Origem))
            throw new ArgumentOutOfRangeException(nameof(hipotese), "A origem da hipótese não é válida.");

        Hipotese = hipotese;
    }

    public void Registrar(Evidencia evidencia)
    {
        ArgumentNullException.ThrowIfNull(evidencia);
        if (string.IsNullOrWhiteSpace(evidencia.Observacao))
            throw new ArgumentException("Descreva a observação.", nameof(evidencia));
        if (!Enum.IsDefined(evidencia.Relacao))
            throw new ArgumentOutOfRangeException(nameof(evidencia), "A relação da evidência não é válida.");

        evidencias.Add(evidencia);
    }

    public SinteseDaInvestigacao Sintetizar()
    {
        bool apoia = evidencias.Any(e => e.Relacao == RelacaoDaEvidencia.Apoia);
        bool contradiz = evidencias.Any(e => e.Relacao == RelacaoDaEvidencia.Contradiz);
        bool contextualiza = evidencias.Any(e => e.Relacao == RelacaoDaEvidencia.Contextualiza);

        return (apoia, contradiz, contextualiza) switch
        {
            (false, false, false) => SinteseDaInvestigacao.SemObservacoes,
            (true, false, false) => SinteseDaInvestigacao.SoApoio,
            (false, true, false) => SinteseDaInvestigacao.SoContradicao,
            (false, false, true) => SinteseDaInvestigacao.SoContexto,
            (true, true, false) => SinteseDaInvestigacao.ApoioEContradicao,
            (true, false, true) => SinteseDaInvestigacao.ApoioEContexto,
            (false, true, true) => SinteseDaInvestigacao.ContradicaoEContexto,
            _ => SinteseDaInvestigacao.ApoioContradicaoEContexto
        };
    }
}
