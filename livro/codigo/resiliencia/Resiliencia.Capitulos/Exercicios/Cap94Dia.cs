namespace Jornada.Resiliencia.Cap94;

public static class ConversaDoDia
{
    /// <summary>Escolhe uma pergunta para hoje. O mesmo dia sempre dá a mesma pergunta.</summary>
    public static string PerguntaDoDia<T>(T situacao, IReadOnlyList<Lente<T>> lentes, DateOnly dia) where T : notnull
    {
        var todas = Conversa.Gerar(situacao, lentes.ToArray());
        return todas[dia.DayNumber % todas.Count];
    }
}
