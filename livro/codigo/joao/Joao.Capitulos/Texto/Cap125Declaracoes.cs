namespace Jornada.Joao.Texto;

// Cada "Eu sou" do Evangelho vem seguido de uma promessa. Um registro guarda as três coisas:
// onde está escrito, o que é dito ("Eu sou ...") e o que se promete logo depois.
// Textos: Bíblia Livre (CC BY 4.0). Só entram as declarações que têm uma imagem (predicado); o "Eu sou" sem predicado não é modelado.

// <registro>
public record Declaracao(string Passagem, string EuSou, string Promessa);
// </registro>

public static class Declaracoes
{
    public static IReadOnlyList<Declaracao> Todas { get; } = new[]
    {
        new Declaracao("Jo 6,35",  "o pão da vida",                    "quem vem a mim de maneira nenhuma terá fome, e quem crê em mim nunca terá sede"),
        new Declaracao("Jo 8,12",  "a luz do mundo",                   "quem me seguir não andará em trevas, mas terá luz de vida"),
        new Declaracao("Jo 10,9",  "a porta",                          "se alguém entrar por mim, será salvo; e entrará, e sairá, e achará pasto"),
        new Declaracao("Jo 10,11", "o bom Pastor",                     "o bom Pastor dá sua vida pelas ovelhas"),
        new Declaracao("Jo 11,25", "a ressurreição, e a vida",         "quem crê em mim, ainda que esteja morto, viverá"),
        new Declaracao("Jo 14,6",  "o caminho, e a verdade, e a vida", "Ninguém vem ao Pai, a não ser por mim"),
        new Declaracao("Jo 15,5",  "a videira",                        "quem está em mim, e eu nele, esse dá muito fruto"),
    };

    public static string Frase(this Declaracao d) => $"{d.Passagem}: Eu sou {d.EuSou}; {d.Promessa}.";
}
