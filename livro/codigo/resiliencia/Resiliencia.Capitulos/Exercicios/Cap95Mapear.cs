namespace Jornada.Resiliencia.Cap95;

public static class ResultadoExtensoes
{
    /// <summary>Transforma o valor se deu certo; se falhou, repassa a mesma falha.</summary>
    public static Resultado<TNovo> Mapear<T, TNovo>(this Resultado<T> resultado, Func<T, TNovo> transformar) =>
        resultado.Combinar(
            valor => Resultado<TNovo>.Ok(transformar(valor)),
            erro => Resultado<TNovo>.Falha(erro));
}
