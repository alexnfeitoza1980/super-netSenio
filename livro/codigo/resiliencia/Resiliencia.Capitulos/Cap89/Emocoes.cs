namespace Jornada.Resiliencia.Cap89;

public enum Emocao { Calma, Alegria, Tristeza, Raiva, Medo, Cansaco }

public sealed record Sentimento
{
    public Emocao Emocao { get; }
    public int Intensidade { get; }

    public Sentimento(Emocao emocao, int intensidade)
    {
        if (intensidade is < 0 or > 10)
            throw new ArgumentOutOfRangeException(nameof(intensidade), "Use um número de 0 a 10.");

        Emocao = emocao;
        Intensidade = intensidade;
    }
}

public static class Vocabulario
{
    /// <summary>Palavras mais precisas ajudam a perceber o que se sente. Não é diagnóstico.</summary>
    public static string Nomear(Sentimento s) => (s.Emocao, s.Intensidade) switch
    {
        (Emocao.Raiva, <= 3)    => "irritação",
        (Emocao.Raiva, <= 7)    => "raiva",
        (Emocao.Raiva, _)       => "raiva muito forte",
        (Emocao.Medo, <= 3)     => "receio",
        (Emocao.Medo, <= 7)     => "medo",
        (Emocao.Medo, _)        => "pavor",
        (Emocao.Tristeza, <= 3) => "desânimo",
        (Emocao.Tristeza, <= 7) => "tristeza",
        (Emocao.Tristeza, _)    => "tristeza profunda",
        (Emocao.Cansaco, <= 3)  => "cansaço leve",
        (Emocao.Cansaco, _)     => "exaustão",
        (Emocao.Alegria, _)     => "alegria",
        _                       => "calma"
    };
}

public static class SugestaoDePausa
{
    /// <summary>Quanto mais forte a emoção, mais folga antes de decidir. É uma sugestão, não uma regra.</summary>
    public static TimeSpan Para(Sentimento s) => s.Intensidade switch
    {
        <= 3 => TimeSpan.Zero,
        <= 6 => TimeSpan.FromMinutes(2),
        _    => TimeSpan.FromMinutes(10)
    };
}
