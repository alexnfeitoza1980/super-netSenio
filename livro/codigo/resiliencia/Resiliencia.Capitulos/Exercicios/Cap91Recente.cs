namespace Jornada.Resiliencia.Cap91;

public static class EvidenciasExtensoes
{
    /// <summary>A evidência mais recente entre as que têm data. Sem nenhuma data, devolve null.</summary>
    public static Evidencia? MaisRecente(this IEnumerable<Evidencia> evidencias) =>
        evidencias
            .Where(e => e.Data is not null)
            .MaxBy(e => e.Data);
}
