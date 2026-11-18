namespace Jornada.Emocoes;

public sealed record PedidoDeConversa
{
    public string Observacao { get; }
    public string Impacto { get; }
    public string Pedido { get; }
    public string Formatar() =>
        $"Observação: {Observacao} | Impacto: {Impacto} | Pedido: {Pedido}";

    public PedidoDeConversa(string observacao, string impacto, string pedido)
    {
        Observacao = ExigirTexto(observacao, nameof(observacao));
        Impacto = ExigirTexto(impacto, nameof(impacto));
        Pedido = ExigirTexto(pedido, nameof(pedido));
    }

    private static string ExigirTexto(string valor, string parametro)
    {
        if (string.IsNullOrWhiteSpace(valor))
            throw new ArgumentException("Descreva o item ou use um exemplo fictício.", parametro);

        return valor.Trim();
    }
}
