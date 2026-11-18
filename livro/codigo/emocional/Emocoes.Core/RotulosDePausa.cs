namespace Jornada.Emocoes;

public static class RotulosDePausa
{
    public static string Descrever(OpcaoDePausa opcao) => opcao switch
    {
        OpcaoDePausa.ContinuarComCalma => "Continuar no ritmo possível",
        OpcaoDePausa.PedirUmIntervalo => "Pedir um intervalo",
        OpcaoDePausa.PedirApoio => "Pedir apoio",
        OpcaoDePausa.EncerrarPorAgora => "Encerrar por agora",
        _ => throw new ArgumentOutOfRangeException(nameof(opcao))
    };
}
