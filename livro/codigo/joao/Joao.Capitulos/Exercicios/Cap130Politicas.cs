using Jornada.Joao.Cap130;

namespace Jornada.Joao.Exercicios;

public static class Politicas
{
    public static Func<Identidade, bool> Perfil(params string[] perfisPermitidos) =>
        identidade => perfisPermitidos.Contains(identidade.Perfil);

    /// <summary>Junta políticas: só passa quem passar em todas.</summary>
    public static Func<Identidade, bool> Todas(params Func<Identidade, bool>[] politicas) =>
        identidade => politicas.All(p => p(identidade));
}
