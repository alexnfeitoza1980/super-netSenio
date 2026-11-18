using Jornada.Joao.Cap126;

namespace Jornada.Joao.Exercicios;

public static class TransformacaoExtensoes
{
    /// <summary>Aplica a mesma transformação várias vezes em sequência (a saída de uma volta é a entrada da próxima).</summary>
    public static Transformacao<T, T> Repetir<T>(this Transformacao<T, T> transformacao, int vezes)
    {
        if (vezes < 0) throw new ArgumentOutOfRangeException(nameof(vezes));

        var resultado = new Transformacao<T, T>(x => x);              // começa "sem fazer nada"
        for (int i = 0; i < vezes; i++)
            resultado = resultado.Depois(transformacao.Aplicar);      // método passado direto, sem lambda

        return resultado;
    }
}
