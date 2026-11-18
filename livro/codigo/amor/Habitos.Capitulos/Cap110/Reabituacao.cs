namespace Jornada.Habitos.Cap110;

/// <summary>
/// Uma curva é só uma função: dado o dia, devolve a força de um hábito (de 0 a 1).
/// Func&lt;int, double&gt; deixa passar "comportamento" como se fosse dado.
/// </summary>
public static class Curvas
{
    /// <summary>Hábito novo: começa em 0 e se aproxima de 1 a cada dia de prática.</summary>
    public static Func<int, double> Crescimento(double ganhoPorDia) =>
        dia => 1 - Math.Pow(1 - ganhoPorDia, dia);

    /// <summary>Hábito antigo: começa em 1 e perde força a cada dia em que se pratica o novo.</summary>
    public static Func<int, double> Declinio(double perdaPorDia) =>
        dia => Math.Pow(1 - perdaPorDia, dia);

    /// <summary>Praticar em grupo (ou com alguém que cobra com carinho) "encurta" o caminho: o dia conta mais.</summary>
    public static Func<int, double> ComParceria(Func<int, double> curva, double fator) =>
        dia => curva((int)Math.Round(dia * fator));
}

public sealed record Ponto(int Dia, double Antigo, double Novo)
{
    public bool NovoJaPassou => Novo > Antigo;
}

public sealed class Reabituacao
{
    private readonly Func<int, double> antigo;
    private readonly Func<int, double> novo;

    public Reabituacao(Func<int, double> antigo, Func<int, double> novo)
    {
        this.antigo = antigo;
        this.novo = novo;
    }

    /// <summary>Gera os pontos dia a dia. É "preguiçoso": só calcula o que for pedido.</summary>
    public IEnumerable<Ponto> Simular(int dias) =>
        Enumerable.Range(0, dias + 1).Select(d => new Ponto(d, antigo(d), novo(d)));

    /// <summary>Primeiro dia em que o hábito novo supera o antigo (null se não acontecer no prazo).</summary>
    public int? DiaDoCruzamento(int limiteDeDias) =>
        Simular(limiteDeDias).FirstOrDefault(p => p.NovoJaPassou)?.Dia;

    /// <summary>Quantos dias o novo ainda "dá mais trabalho" que o antigo: o período em que quase todo mundo desiste.</summary>
    public int DiasDeDesconforto(int limiteDeDias) =>
        Simular(limiteDeDias).TakeWhile(p => !p.NovoJaPassou).Count();

    /// <summary>Compara dois caminhos lado a lado, dia a dia, com Zip.</summary>
    public static IEnumerable<(int Dia, double Diferenca)> Comparar(Reabituacao a, Reabituacao b, int dias) =>
        a.Simular(dias).Zip(b.Simular(dias), (pa, pb) => (pa.Dia, pb.Novo - pa.Novo));
}
