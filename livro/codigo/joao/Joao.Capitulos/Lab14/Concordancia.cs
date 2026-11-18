using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Jornada.Joao.Lab14;

public sealed record Lema(
    string Grego,
    string Translit,
    string Sentido,
    int Total,
    IReadOnlyDictionary<string, int> PorCapitulo)
{
    public int Em(int capitulo) => PorCapitulo.GetValueOrDefault(capitulo.ToString());
}

public sealed record EuSouNoTexto(int Capitulo, int Versiculo, string? Predicado)
{
    public override string ToString() => $"Jo {Capitulo},{Versiculo}" + (Predicado is null ? " (uso absoluto ou outra construção)" : $": {Predicado}");
}

public sealed record DadosDoTexto(
    string Obra,
    string Fonte,
    IReadOnlyDictionary<string, int> PalavrasPorCapitulo,
    IReadOnlyList<Lema> Lemas,
    IReadOnlyList<EuSouNoTexto> EuSou)
{
    public int TotalDePalavras => PalavrasPorCapitulo.Values.Sum();
}

/// <summary>Uma concordância: onde cada palavra do original aparece, e com que "densidade" em cada capítulo.</summary>
public sealed class Concordancia
{
    private static readonly JsonSerializerOptions Opcoes = new() { PropertyNameCaseInsensitive = true };

    private readonly DadosDoTexto dados;

    public Concordancia(DadosDoTexto dados) => this.dados = dados;

    /// <summary>Lê os dados embutidos no programa (recurso incorporado), sem depender de nenhum arquivo externo.</summary>
    public static Concordancia Embutida()
    {
        using var fluxo = Assembly.GetExecutingAssembly().GetManifestResourceStream("lemas.json")
            ?? throw new InvalidOperationException("O recurso lemas.json não foi encontrado no programa.");

        var dados = JsonSerializer.Deserialize<DadosDoTexto>(fluxo, Opcoes)
            ?? throw new InvalidOperationException("Os dados estão vazios.");

        return new Concordancia(dados);
    }

    public DadosDoTexto Dados => dados;

    /// <summary>Procura por qualquer pedaço do original, da transliteração ou do sentido, sem se importar com maiúsculas.</summary>
    public Lema? Buscar(string termo) =>
        dados.Lemas.FirstOrDefault(l =>
            l.Grego == termo ||
            l.Translit.Equals(termo, StringComparison.OrdinalIgnoreCase) ||
            l.Sentido.Contains(termo, StringComparison.OrdinalIgnoreCase));

    public IReadOnlyList<Lema> MaisFrequentes(int quantos) =>
        dados.Lemas.OrderByDescending(l => l.Total).ThenBy(l => l.Translit).Take(quantos).ToList();

    /// <summary>Ocorrências a cada mil palavras do capítulo: permite comparar capítulos de tamanhos diferentes.</summary>
    public double Densidade(Lema lema, int capitulo)
    {
        var palavras = dados.PalavrasPorCapitulo.GetValueOrDefault(capitulo.ToString());
        return palavras == 0 ? 0 : lema.Em(capitulo) * 1000.0 / palavras;
    }

    /// <summary>O capítulo onde a palavra é mais densa. Sem nenhuma ocorrência, não há pico (null).</summary>
    public (int Capitulo, double Densidade)? Pico(Lema lema)
    {
        if (lema.Total == 0) return null;

        return dados.PalavrasPorCapitulo.Keys
            .Select(k => int.Parse(k))
            .Select(c => (Capitulo: c, Densidade: Densidade(lema, c)))
            .MaxBy(x => x.Densidade);
    }

    /// <summary>Que fatia das ocorrências da palavra está concentrada em um único capítulo?</summary>
    public double Concentracao(Lema lema, int capitulo) => lema.Total == 0 ? 0 : (double)lema.Em(capitulo) / lema.Total;

    /// <summary>Um gráfico de barras em texto, um capítulo por linha.</summary>
    public string Barras(Lema lema, int largura = 30)
    {
        int maximo = Math.Max(1, Enumerable.Range(1, 21).Max(lema.Em));
        var texto = new StringBuilder();

        for (int capitulo = 1; capitulo <= 21; capitulo++)
        {
            int n = lema.Em(capitulo);
            texto.AppendLine($"{capitulo,2} {new string('█', n * largura / maximo)}{(n > 0 ? " " + n : "")}");
        }

        return texto.ToString().TrimEnd();
    }
}
