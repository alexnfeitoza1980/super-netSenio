namespace Jornada.Resiliencia.Cap95;

/// <summary>Em vez de lançar exceção para uma falha esperada, devolvemos o que aconteceu.</summary>
public sealed class Resultado<T>
{
    private readonly T? valor;
    private readonly string? erro;

    private Resultado(bool deu, T? valor, string? erro)
    {
        Deu = deu;
        this.valor = valor;
        this.erro = erro;
    }

    public bool Deu { get; }

    public static Resultado<T> Ok(T valor) => new(true, valor, null);
    public static Resultado<T> Falha(string erro) => new(false, default, erro);

    /// <summary>Obriga quem usa a decidir os dois caminhos: o que fazer se deu e o que fazer se não deu.</summary>
    public TSaida Combinar<TSaida>(Func<T, TSaida> seDeu, Func<string, TSaida> seFalhou) =>
        Deu ? seDeu(valor!) : seFalhou(erro!);
}
