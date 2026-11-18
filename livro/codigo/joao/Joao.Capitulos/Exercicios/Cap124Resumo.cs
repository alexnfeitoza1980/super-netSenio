using Jornada.Joao.Cap124;

namespace Jornada.Joao.Exercicios;

public static class RegistroExtensoes
{
    /// <summary>Uma linha com quantos registros há em cada nível, do mais grave ao mais leve: "Erro: 1 · Informacao: 2".</summary>
    public static string Resumo(this IEnumerable<Registro> registros) =>
        string.Join(" · ",
            registros
                .GroupBy(r => r.Nivel)
                .OrderByDescending(g => g.Key)
                .Select(g => $"{g.Key}: {g.Count()}"));
}
