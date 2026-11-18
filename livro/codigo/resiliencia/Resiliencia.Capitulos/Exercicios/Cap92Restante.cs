namespace Jornada.Resiliencia.Cap92;

public static class MetaExtensoes
{
    public static int MinutosRestantes(this Meta meta) =>
        meta.Passos.Where(p => !p.Concluido).Sum(p => p.Minutos);

    public static string Estimativa(this Meta meta) => meta.MinutosRestantes() switch
    {
        0       => "Nada pendente.",
        <= 25   => "Cabe em um único passo.",
        var m   => $"Cerca de {m} min no total, em passos de até {Meta.LimiteDeMinutosPorPasso} min."
    };
}
