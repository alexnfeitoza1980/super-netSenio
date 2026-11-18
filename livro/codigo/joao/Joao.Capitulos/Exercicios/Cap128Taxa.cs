using Jornada.Joao.Cap128;

namespace Jornada.Joao.Exercicios;

public static class PocoExtensoes
{
    /// <summary>De cada pedido, que fração foi atendida sem ir à fonte? Sem pedidos, zero (e não uma divisão por zero).</summary>
    public static double TaxaDeAcerto<TChave, TValor>(this Poco<TChave, TValor> poco) where TChave : notnull
    {
        long pedidos = poco.Acertos + poco.Falhas + poco.Expiradas;
        return pedidos == 0 ? 0 : (double)poco.Acertos / pedidos;
    }
}
