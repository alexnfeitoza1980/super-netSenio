namespace Jornada.Habitos.Cap105;

/// <summary>Algo que a pessoa quer, e o quanto (peso de 0 a 1).</summary>
public sealed record Desejo(string Nome, double Peso);

/// <summary>Uma opção na mesa e o quanto ela "atrai" para cada desejo (0 a 1).</summary>
public sealed record Opcao(string Nome, IReadOnlyDictionary<string, double> Atrativos);

public sealed record Pontuacao(string Opcao, double Pontos);

public static class Decisor
{
    /// <summary>
    /// Pontua cada opção pela soma de (peso do desejo × atrativo da opção para aquele desejo).
    /// Repare: não existe aqui nenhum "conhecimento". O comportamento sai do que se quer.
    /// </summary>
    public static IReadOnlyList<Pontuacao> Ranquear(IEnumerable<Desejo> desejos, IEnumerable<Opcao> opcoes)
    {
        var lista = desejos.ToList();

        return opcoes
            .Select(o => new Pontuacao(o.Nome, lista.Sum(d => d.Peso * o.Atrativos.GetValueOrDefault(d.Nome))))
            .OrderByDescending(p => p.Pontos)
            .ThenBy(p => p.Opcao)
            .ToList();
    }

    public static string Escolher(IEnumerable<Desejo> desejos, IEnumerable<Opcao> opcoes) =>
        Ranquear(desejos, opcoes).FirstOrDefault()?.Opcao
        ?? throw new InvalidOperationException("Sem opções para escolher.");
}
