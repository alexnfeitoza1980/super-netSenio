namespace Jornada.Emocoes;

public static class RotulosEmocionais
{
    public static string Descrever(EmocaoNomeada emocao) => emocao switch
    {
        EmocaoNomeada.Alegria => "Alegria",
        EmocaoNomeada.Tristeza => "Tristeza",
        EmocaoNomeada.Raiva => "Raiva",
        EmocaoNomeada.Medo => "Medo",
        EmocaoNomeada.Ansiedade => "Ansiedade",
        EmocaoNomeada.Frustracao => "Frustração",
        EmocaoNomeada.Cansaco => "Cansaço",
        EmocaoNomeada.AindaNaoSei => "Ainda não sei",
        EmocaoNomeada.PrefiroNaoNomear => "Prefiro não nomear",
        EmocaoNomeada.Outra => "Outra",
        _ => throw new ArgumentOutOfRangeException(nameof(emocao))
    };
}
