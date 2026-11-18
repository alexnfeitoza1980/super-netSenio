namespace Jornada.Joao.Cap125;

// As sete imagens "Eu sou" do Evangelho (com predicado) viram sete serviços de um sistema.
// Cada classe leva o texto do versículo: a etiqueta diz quem fala ("Eu sou ...") e Promete() repete a promessa dita logo em seguida.
// Os textos são da Bíblia Livre (CC BY 4.0).

[EuSou("o pão da vida", Referencia = "Jo 6,35", Promessa = "de maneira nenhuma terá fome")]
public sealed class Pao : IImagem
{
    public string Promete() => "quem vem a mim de maneira nenhuma terá fome, e quem crê em mim nunca terá sede";
}

[EuSou("a luz do mundo", Referencia = "Jo 8,12", Promessa = "terá luz de vida")]
public sealed class Luz : IImagem
{
    public string Promete() => "quem me seguir não andará em trevas, mas terá luz de vida";
}

[EuSou("a porta", Referencia = "Jo 10,9", Promessa = "será salvo")]
public sealed class Porta : IImagem
{
    public string Promete() => "se alguém entrar por mim, será salvo; e entrará, e sairá, e achará pasto";
}

[EuSou("o bom pastor", Referencia = "Jo 10,11", Promessa = "dá sua vida pelas ovelhas")]
public sealed class Pastor : IImagem
{
    public string Promete() => "o bom Pastor dá sua vida pelas ovelhas";
}

[EuSou("a ressurreição, e a vida", Referencia = "Jo 11,25", Promessa = "viverá")]
public sealed class Ressurreicao : IImagem
{
    public string Promete() => "quem crê em mim, ainda que esteja morto, viverá";
}

[EuSou("o caminho, e a verdade, e a vida", Referencia = "Jo 14,6", Promessa = "ninguém vem ao Pai, a não ser por mim")]
public sealed class Caminho : IImagem
{
    public string Promete() => "Ninguém vem ao Pai, a não ser por mim";
}

[EuSou("a videira", Referencia = "Jo 15,5", Promessa = "dá muito fruto")]
public sealed class Videira : IImagem
{
    public string Promete() => "quem está em mim, e eu nele, esse dá muito fruto";
}

/// <summary>Este NÃO é marcado: o catálogo o ignora. Reflexão enxerga só o que está etiquetado.</summary>
public sealed class ImagemSemEtiqueta : IImagem
{
    public string Promete() => "existe, mas não se apresenta";
}
