namespace Jornada.Joao.Texto;

// Jo 2,6-10 (Bíblia Livre): "E estavam ali postos seis vasos de pedra [...] Enchei estes vasos com água. E encheram-nas até encima.
//  E disse-lhes: Agora tirai, e a levai ao mestre de cerimônia. [...] E quando o mestre de cerimônia experimentou a água feita vinho
//  (sem saber de onde era, porém os serventes que haviam tirado a água sabiam) [...] tu guardaste o bom vinho até agora."
//
// TEXTO     : os vasos são enchidos com água (2,7); o que se leva ao mestre de cerimônia é provado como "a água feita vinho" (2,9).
//             O texto NÃO diz em que momento a água se tornou vinho: só que, na prova, já era vinho.
// IMPLEMENTAÇÃO (escolha nossa, não relato): a mudança acontece no método Tirar, para que o exemplo possa mostrar uma
//             coisa útil de C#: o trabalho só é feito quando alguém pede.

public record Bebida(string Nome, int Qualidade);

// <vaso>
public class Vaso
{
    private Bebida? conteudo;

    public string Conteudo => conteudo?.Nome ?? "vazio";

    // "Enchei estes vasos com água"
    public void Encher(Bebida agua)
    {
        if (agua.Nome != "água")
            throw new ArgumentException("Este vaso é enchido com água.", nameof(agua));

        conteudo = agua;
    }

    // "Agora tirai": o que sai daqui já é "a água feita vinho". ESCOLHA DE IMPLEMENTAÇÃO: o texto não diz quando mudou.
    public Bebida Tirar()
    {
        if (conteudo is null)
            throw new InvalidOperationException("Não há o que tirar: o vaso está vazio.");

        conteudo = new Bebida("vinho", Qualidade: 9);
        return conteudo;
    }
}
// </vaso>

// <mestre>
public class MestreDeCerimonia
{
    private const int QualidadeDoVinhoDeAntes = 5;

    // "o mestre de cerimônia experimentou a água feita vinho": ele só vê o que tem no copo
    public string Provar(Bebida bebida) => bebida switch
    {
        { Nome: "vinho", Qualidade: > QualidadeDoVinhoDeAntes } => "tu guardaste o bom vinho até agora",
        { Nome: "vinho" }                                      => "vinho comum",
        _                                                      => "só água",
    };
}
// </mestre>
