namespace Jornada.Joao.Texto;

// Jo 10,3-5 (Bíblia Livre): "[...] as ovelhas ouvem sua voz, e a suas ovelhas chama nome por nome, e as leva fora. [...]
//  Mas ao estranho em maneira nenhuma seguirão, ao invés disso dele fugirão; porque não conhecem a voz dos estranhos."
//
// TEXTO     : as ovelhas seguem a voz que conhecem e fogem da voz dos estranhos.
// ANALOGIA  : "conhecer a voz" vira "conferir se a voz é a que se conhece".
// LIMITE    : o texto NÃO fala de alguém imitar a voz do pastor. Essa pergunta (e a assinatura que a resolve) é uma
//             EXTENSÃO nossa, para ensinar um problema real de programação; não está em João 10.

// <ovelha>
public class Ovelha
{
    private readonly string vozQueConhece;

    public Ovelha(string vozQueConhece) => this.vozQueConhece = vozQueConhece;

    // "não conhecem a voz dos estranhos": só segue a voz que conhece
    public bool Segue(string voz) => voz == vozQueConhece;
}
// </ovelha>
