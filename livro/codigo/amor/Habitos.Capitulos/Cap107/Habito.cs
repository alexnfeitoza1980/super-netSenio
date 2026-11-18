namespace Jornada.Habitos.Cap107;

/// <summary>
/// Um hábito em formação. Imutável: cada prática devolve um hábito novo (com "with"),
/// o que deixa o histórico fácil de testar e de raciocinar.
/// </summary>
public sealed record Habito(string Nome, double Forca = 0, DateOnly? UltimaPratica = null)
{
    /// <summary>A partir daqui o hábito "anda sozinho": pede pouca decisão consciente.</summary>
    public const double Limiar = 0.8;

    public bool Automatico => Forca >= Limiar;
}

public static class Formacao
{
    public const double GanhoSozinho = 0.12;
    public const double GanhoComMentor = 0.18;
    public const double MeiaVidaEmDias = 14;

    /// <summary>A força que o hábito tem num dia: sem prática, ela vai se desfazendo pela metade a cada 14 dias.</summary>
    public static double ForcaEm(Habito habito, DateOnly dia)
    {
        if (habito.UltimaPratica is not { } ultima || dia <= ultima)
            return habito.Forca;

        int dias = dia.DayNumber - ultima.DayNumber;
        return habito.Forca * Math.Pow(0.5, dias / MeiaVidaEmDias);
    }

    /// <summary>
    /// Cada repetição soma menos que a anterior: o que falta para chegar a 1 vai diminuindo.
    /// O desgaste só corre nos dias SEM prática (por isso olhamos a força "de ontem").
    /// </summary>
    public static Habito Praticar(Habito habito, DateOnly dia, bool comMentor = false)
    {
        double atual = ForcaEm(habito, dia.AddDays(-1));
        double ganho = comMentor ? GanhoComMentor : GanhoSozinho;

        return habito with { Forca = atual + (1 - atual) * ganho, UltimaPratica = dia };
    }

    /// <summary>Aplica uma sequência de práticas, em ordem de data, e devolve o hábito resultante.</summary>
    public static Habito PraticarVarias(Habito inicial, IEnumerable<DateOnly> dias) =>
        dias.OrderBy(d => d).Aggregate(inicial, (h, dia) => Praticar(h, dia));

    /// <summary>Quantos dias de prática diária seriam necessários para o hábito ficar automático? (null se passar do limite)</summary>
    public static int? DiasAteAutomatico(Habito inicial, DateOnly comeco, bool comMentor = false, int limiteDeDias = 365)
    {
        var habito = inicial;

        for (int i = 0; i < limiteDeDias; i++)
        {
            habito = Praticar(habito, comeco.AddDays(i), comMentor);
            if (habito.Automatico)
                return i + 1;
        }

        return null;
    }
}
