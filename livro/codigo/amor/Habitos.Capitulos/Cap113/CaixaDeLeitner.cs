namespace Jornada.Habitos.Cap113;

/// <summary>Um cartão de estudo: uma pergunta, a resposta e em que "caixa" ele está (1 a 5).</summary>
public sealed record Cartao(string Frente, string Verso, int Caixa = 1, DateOnly Proxima = default);

/// <summary>
/// Repetição espaçada: o que se acerta volta cada vez mais tarde; o que se erra volta logo.
/// É uma forma de transformar "saber" em "saber de cor", aquilo que a mão faz sem pedir licença.
/// </summary>
public sealed class CaixaDeLeitner
{
    /// <summary>Dias até rever um cartão, conforme a caixa em que ele está (caixa 1 = índice 0).</summary>
    public static readonly IReadOnlyList<int> Intervalos = new[] { 1, 2, 4, 8, 16 };

    private readonly List<Cartao> cartoes = new();

    public IReadOnlyList<Cartao> Todos => cartoes;

    public void Adicionar(string frente, string verso, DateOnly hoje) =>
        cartoes.Add(new Cartao(frente, verso, 1, hoje));

    /// <summary>Os cartões que já "venceram" hoje: primeiro os mais frágeis (caixa baixa).</summary>
    public IReadOnlyList<Cartao> Devidos(DateOnly hoje) =>
        cartoes.Where(c => c.Proxima <= hoje).OrderBy(c => c.Caixa).ThenBy(c => c.Frente).ToList();

    /// <summary>A sessão do dia, embaralhada com um Random recebido de fora (assim o teste controla o sorteio).</summary>
    public IReadOnlyList<Cartao> SessaoDoDia(DateOnly hoje, Random sorteio) =>
        Devidos(hoje).OrderBy(_ => sorteio.Next()).ToList();

    public void Responder(string frente, bool acertou, DateOnly hoje)
    {
        int indice = cartoes.FindIndex(c => c.Frente == frente);
        if (indice < 0)
            throw new KeyNotFoundException($"Não há cartão com a frente \"{frente}\".");

        var cartao = cartoes[indice];
        int nova = acertou ? Math.Min(cartao.Caixa + 1, Intervalos.Count) : 1;

        cartoes[indice] = cartao with { Caixa = nova, Proxima = hoje.AddDays(Intervalos[nova - 1]) };
    }

    /// <summary>Quantos cartões há em cada caixa (as caixas vazias aparecem com 0).</summary>
    public IReadOnlyDictionary<int, int> PorCaixa() =>
        Enumerable.Range(1, Intervalos.Count)
                  .ToDictionary(caixa => caixa, caixa => cartoes.Count(c => c.Caixa == caixa));
}
