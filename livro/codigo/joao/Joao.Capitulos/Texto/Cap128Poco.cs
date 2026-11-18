namespace Jornada.Joao.Texto;

// Jo 4,11-15 (Bíblia Livre): "[...] tu não tens com que a tirar, e o poço é fundo; de onde pois tens a água viva? [...]
//  Todo aquele que beber desta água voltará a ter sede; Porém aquele que beber da água que eu lhe der, para sempre não terá sede,
//  mas a água que eu lhe der se fará nele fonte de água, que salte para vida eterna. [...] para que eu não tenha mais sede,
//  nem venha aqui para tirar."
//
// TEXTO     : duas águas: a do poço, que obriga a voltar, e a que Jesus dá ("água viva"), que se faz "fonte" em quem a bebe.
//             A mulher entende de modo prático; Jesus fala de "vida eterna".
// ANALOGIA  : só a ESTRUTURA do problema prático: buscar no fundo toda vez custa; ter por dentro evita voltar.
// LIMITE    : guardar um resultado (cache) não é a "água viva" do texto, que não é um dado nem uma economia de esforço.

// <poco>
public class PocoFundo
{
    public int Descidas { get; private set; }

    // "o poço é fundo": cada vez que se quer água, é preciso descer de novo
    public string TirarAgua()
    {
        Descidas++;
        return "água do poço";
    }
}
// </poco>

// <fonte>
public class FonteDentro
{
    private readonly PocoFundo poco;
    private string? guardada;

    public FonteDentro(PocoFundo poco) => this.poco = poco;

    // "se fará nele fonte": depois da primeira vez, a água já está por dentro e não é preciso voltar ao poço
    public string Beber() => guardada ??= poco.TirarAgua();
}
// </fonte>
