namespace Jornada.Resiliencia.Cap96;

public static class PoliticaExtensoes
{
    /// <summary>Espera crescente, mas com teto: insistir cada vez mais devagar também cansa.</summary>
    public static TimeSpan EsperaComTeto(this PoliticaDeTentativas politica, int tentativaQueFalhou, TimeSpan teto) =>
        TimeSpan.FromTicks(Math.Min(politica.EsperaApos(tentativaQueFalhou).Ticks, teto.Ticks));
}
