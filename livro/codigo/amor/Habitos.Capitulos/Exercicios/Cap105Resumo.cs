namespace Jornada.Habitos.Cap105;

public static class PlanoExtensoes
{
    /// <summary>Uma linha com as três dimensões e um ✓ ou ✗ em cada: "Cabeça ✓ · Coração ✗ · Mãos ✓".</summary>
    public static string Resumo(this PlanoDeAprendizado plano)
    {
        var coberto = plano.Cobertura();

        string Marca(Dimensao dimensao, string nome) => $"{nome} {(coberto.HasFlag(dimensao) ? "✓" : "✗")}";

        return string.Join(" · ", Marca(Dimensao.Cabeca, "Cabeça"), Marca(Dimensao.Coracao, "Coração"), Marca(Dimensao.Maos, "Mãos"));
    }
}
