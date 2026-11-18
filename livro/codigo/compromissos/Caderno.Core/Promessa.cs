namespace Quatro;

public enum EstadoDaPromessa { Aberta, Cumprida, Cancelada }
public sealed record RevisaoDePrazo(DateOnly Antes, DateOnly Depois, string Motivo);

public sealed class Promessa
{
    private readonly List<RevisaoDePrazo> revisoes = [];
    public Guid Id { get; } = Guid.NewGuid();
    public string Descricao { get; }
    public DateOnly Prazo { get; private set; }
    public EstadoDaPromessa Estado { get; private set; }
    public DateOnly? ConcluidaEm { get; private set; }
    public string? MotivoDoCancelamento { get; private set; }
    public IReadOnlyList<RevisaoDePrazo> Revisoes => revisoes.ToArray();

    public Promessa(string descricao, DateOnly prazo)
    {
        Descricao = Regras.Texto(descricao);
        Prazo = prazo;
    }

    private void ExigirAberta()
    {
        if (Estado != EstadoDaPromessa.Aberta)
            throw new InvalidOperationException("A promessa já foi encerrada.");
    }

    public void Cumprir(DateOnly hoje)
    {
        ExigirAberta();
        ConcluidaEm = hoje;
        Estado = EstadoDaPromessa.Cumprida;
    }

    public bool EstaAtrasada(DateOnly hoje) =>
        Estado == EstadoDaPromessa.Aberta && hoje > Prazo;

    public bool FoiCumpridaComAtraso => ConcluidaEm is DateOnly dia && dia > Prazo;

    public void Renegociar(DateOnly novoPrazo, DateOnly hoje, string motivo)
    {
        ExigirAberta();
        string justificativa = Regras.Texto(motivo);
        if (novoPrazo <= hoje || novoPrazo == Prazo)
            throw new ArgumentException("Escolha outro prazo, posterior a hoje.");
        revisoes.Add(new(Prazo, novoPrazo, justificativa));
        Prazo = novoPrazo;
    }

    public void Cancelar(string motivo)
    {
        ExigirAberta();
        MotivoDoCancelamento = Regras.Texto(motivo);
        Estado = EstadoDaPromessa.Cancelada;
    }
}
