namespace Jornada.Habitos.Cap115;

public static class EtapasCondicionais
{
    /// <summary>Roda a etapa só quando a condição vale; senão, passa direto para a próxima, sem interromper nada.</summary>
    public static Etapa Quando(Func<ContextoDeEntrega, bool> condicao, Etapa etapa) =>
        (contexto, proximo) => condicao(contexto) ? etapa(contexto, proximo) : proximo();
}
