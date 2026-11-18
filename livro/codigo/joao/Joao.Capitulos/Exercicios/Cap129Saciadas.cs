using Jornada.Joao.Cap129;

namespace Jornada.Joao.Exercicios;

public static class ResultadoExtensoes
{
    /// <summary>Quantas pessoas receberam tudo o que precisavam.</summary>
    public static int PessoasSaciadas(this Resultado resultado, IReadOnlyList<int> fomes) =>
        resultado.ServidosPorPessoa.Zip(fomes, (servido, fome) => servido >= fome).Count(saciada => saciada);
}
