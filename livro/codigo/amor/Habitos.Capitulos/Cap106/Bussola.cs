namespace Jornada.Habitos.Cap106;

/// <summary>
/// Uma direção no plano (X: valor entregue às pessoas; Y: saúde do que construímos).
/// É um <c>struct</c> imutável: pequeno, sem identidade, copiado por valor.
/// </summary>
public readonly record struct Vetor(double X, double Y)
{
    public static Vetor Zero => new(0, 0);

    public static Vetor operator +(Vetor a, Vetor b) => new(a.X + b.X, a.Y + b.Y);
    public static Vetor operator *(double k, Vetor v) => new(k * v.X, k * v.Y);

    public double Norma => Math.Sqrt(X * X + Y * Y);

    public double ProdutoEscalar(Vetor outro) => X * outro.X + Y * outro.Y;

    /// <summary>Cosseno do ângulo entre dois vetores: 1 = mesma direção, 0 = perpendiculares, -1 = opostas.</summary>
    public double Cosseno(Vetor outro) =>
        Norma == 0 || outro.Norma == 0 ? 0 : ProdutoEscalar(outro) / (Norma * outro.Norma);

    public double AnguloEmGraus(Vetor outro) =>
        Math.Acos(Math.Clamp(Cosseno(outro), -1, 1)) * 180 / Math.PI;
}

public sealed record Iniciativa(string Nome, Vetor Efeito);

public enum Leitura { NoRumo, Desviando, RumoErrado }

public sealed class Bussola
{
    public Vetor Telos { get; }

    public Bussola(Vetor telos)
    {
        if (telos.Norma == 0)
            throw new ArgumentException("Um rumo precisa apontar para algum lugar.", nameof(telos));
        Telos = telos;
    }

    /// <summary>Recalibrar não muda a bússola: devolve outra, apontando para o novo rumo.</summary>
    public Bussola Recalibrar(Vetor novoTelos) => new(novoTelos);

    /// <summary>Quanto uma iniciativa, sozinha, aponta para o rumo (de -1 a 1).</summary>
    public double Alinhamento(Iniciativa iniciativa) => iniciativa.Efeito.Cosseno(Telos);

    /// <summary>Para onde o conjunto de tudo que foi feito está nos levando.</summary>
    public Vetor Deriva(IEnumerable<Iniciativa> feitas) =>
        feitas.Aggregate(Vetor.Zero, (soma, i) => soma + i.Efeito);

    /// <summary>Desvio total em graus. Sem nenhum movimento, não há desvio a medir (null).</summary>
    public double? DesvioEmGraus(IEnumerable<Iniciativa> feitas)
    {
        var deriva = Deriva(feitas);
        return deriva.Norma == 0 ? null : deriva.AnguloEmGraus(Telos);
    }

    public static Leitura Ler(double graus) => graus switch
    {
        < 25 => Leitura.NoRumo,
        < 70 => Leitura.Desviando,
        _    => Leitura.RumoErrado
    };
}
