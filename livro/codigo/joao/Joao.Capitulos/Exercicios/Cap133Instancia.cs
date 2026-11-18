using Jornada.Joao.Cap133;

namespace Jornada.Joao.Exercicios;

public static class RecipienteExtensoes
{
    /// <summary>Registra uma peça que já existe: o recipiente devolve sempre essa mesma instância.</summary>
    public static Recipiente RegistrarInstancia<T>(this Recipiente recipiente, T instancia) where T : notnull =>
        recipiente.Registrar<T>(_ => instancia, Ciclo.Permanente);
}
