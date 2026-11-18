namespace Jornada.Habitos.Cap113;

public static class CaixaExtensoes
{
    /// <summary>O dia da próxima revisão de qualquer cartão. Sem cartões, não há próxima revisão (null).</summary>
    public static DateOnly? ProximaRevisao(this CaixaDeLeitner caixa) =>
        caixa.Todos.Count == 0 ? null : caixa.Todos.Min(c => c.Proxima);

    /// <summary>Os cartões já na última caixa: o que a mão já sabe de cor.</summary>
    public static IEnumerable<Cartao> Dominados(this CaixaDeLeitner caixa) =>
        caixa.Todos.Where(c => c.Caixa == CaixaDeLeitner.Intervalos.Count);
}
