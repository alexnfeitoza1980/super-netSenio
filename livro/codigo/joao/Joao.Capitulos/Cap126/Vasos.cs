namespace Jornada.Joao.Cap126;

public sealed record Liquido(string Nome, double Qualidade);

/// <summary>
/// Uma transformação de TEntrada em TSaida que se pode encadear com Depois().
/// Dois parâmetros de tipo: o que entra e o que sai (por isso "genérica").
/// </summary>
public sealed class Transformacao<TEntrada, TSaida>
{
    private readonly Func<TEntrada, TSaida> funcao;

    public Transformacao(Func<TEntrada, TSaida> funcao) => this.funcao = funcao;

    public TSaida Aplicar(TEntrada entrada) => funcao(entrada);

    /// <summary>Junta esta transformação com outra: a saída de uma vira a entrada da próxima.</summary>
    public Transformacao<TEntrada, TNova> Depois<TNova>(Func<TSaida, TNova> proxima) =>
        new(entrada => proxima(funcao(entrada)));

    /// <summary>Aplica a vários itens, um de cada vez e só quando alguém pede: execução adiada.</summary>
    public IEnumerable<TSaida> AplicarA(IEnumerable<TEntrada> entradas)
    {
        foreach (var entrada in entradas)
            yield return funcao(entrada);
    }
}

public sealed record Taca(int Vaso, Liquido Conteudo);

public sealed class Banquete
{
    private int transformadas;

    /// <summary>Quantos vasos já foram realmente transformados. Mostra que o trabalho só acontece quando se prova.</summary>
    public int Transformadas => transformadas;

    public Transformacao<Liquido, Liquido> DaAguaAoVinho { get; } =
        new Transformacao<Liquido, Liquido>(agua => agua with { Nome = "vinho" })
            .Depois(vinho => vinho with { Qualidade = vinho.Qualidade + 4 });   // quanto mais adiante, melhor

    /// <summary>Serve os vasos na ordem. Cada um só é transformado quando a taça é pedida.</summary>
    public IEnumerable<Taca> Servir(int vasos)
    {
        for (int i = 1; i <= vasos; i++)
        {
            var agua = new Liquido("água", i);
            transformadas++;
            yield return new Taca(i, DaAguaAoVinho.Aplicar(agua));
        }
    }
}

public static class Provador
{
    /// <summary>O mestre de cerimônia só enxerga o que está na taça: não sabe de onde veio.</summary>
    public static string Avaliar(Liquido liquido) => (liquido.Nome, liquido.Qualidade) switch
    {
        ("vinho", >= 9) => "o melhor ficou para o fim",
        ("vinho", >= 7) => "vinho muito bom",
        ("vinho", _)    => "vinho",
        _               => "só água"
    };
}
