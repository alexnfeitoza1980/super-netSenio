namespace Jornada.Habitos.Cap107;

public static class HabitoExtensoes
{
    /// <summary>
    /// Quantos dias sem praticar até o hábito deixar de ser automático?
    /// Resolve força × 0,5^(d/14) &lt; limiar. Se o hábito já não é automático, devolve null.
    /// </summary>
    public static int? DiasAteDeixarDeSerAutomatico(this Habito habito)
    {
        if (!habito.Automatico)
            return null;

        double dias = Formacao.MeiaVidaEmDias * Math.Log2(habito.Forca / Habito.Limiar);
        return (int)Math.Floor(dias) + 1;
    }
}
