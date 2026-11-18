namespace Quatro;

// DTOs: retratos transportáveis, separados dos objetos de trabalho.
public sealed record PromessaSalva(Guid Id, string Descricao, DateOnly Prazo,
    EstadoDaPromessa Estado, DateOnly? ConcluidaEm, string? MotivoDoCancelamento,
    RevisaoDePrazo[] Revisoes);
public sealed record PerguntaSalva(string Texto, EstadoDaPergunta Estado, Evidencia? Evidencia);
public sealed record Retrato(int Versao, Acordo[] Acordos, PromessaSalva[] Promessas,
    Reflexao[] Reflexoes, RespostaEscolhida[] Respostas, PerguntaSalva[] Perguntas, Pratica[] Praticas);

public sealed class Caderno
{
    public Inventario Inventario { get; } = new();
    public DiarioDePratica Diario { get; } = new();
    private readonly List<Promessa> promessas = [];
    private readonly List<Reflexao> reflexoes = [];
    private readonly List<RespostaEscolhida> respostas = [];
    private readonly List<Pergunta> perguntas = [];
    public IReadOnlyList<Promessa> Promessas => promessas.ToArray();
    public IReadOnlyList<Pergunta> Perguntas => perguntas.ToArray();

    public Promessa Prometer(string descricao, DateOnly prazo)
    {
        var p = new Promessa(descricao, prazo);
        promessas.Add(p);
        return p;
    }
    public Pergunta Perguntar(string texto)
    {
        var p = new Pergunta(texto);
        perguntas.Add(p);
        return p;
    }
    public void Refletir(Reflexao reflexao) => reflexoes.Add(reflexao ?? throw new ArgumentNullException(nameof(reflexao)));
    public void Responder(RespostaEscolhida resposta) => respostas.Add(resposta ?? throw new ArgumentNullException(nameof(resposta)));
    public IReadOnlyList<Promessa> Pendentes() => promessas
        .Where(p => p.Estado == EstadoDaPromessa.Aberta).OrderBy(p => p.Prazo)
        .ThenBy(p => p.Descricao, StringComparer.Ordinal).ToArray();

    public Retrato Exportar() => new(1, Inventario.Todos.ToArray(), promessas.Select(p =>
        new PromessaSalva(p.Id, p.Descricao, p.Prazo, p.Estado, p.ConcluidaEm,
            p.MotivoDoCancelamento, p.Revisoes.ToArray())).ToArray(),
        reflexoes.ToArray(), respostas.ToArray(), perguntas.Select(p =>
            new PerguntaSalva(p.Texto, p.Estado, p.Evidencia)).ToArray(), Diario.Registros.ToArray());
}
