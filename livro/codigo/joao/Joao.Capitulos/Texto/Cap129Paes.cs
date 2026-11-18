namespace Jornada.Joao.Texto;

// Jo 6,9-13 (Bíblia Livre): "Um menino está aqui que tem cinco pães de cevada e dois peixinhos [...]
//  [Jesus] repartiu-os aos discípulos, e os discípulos aos que estavam sentados, semelhantemente também dos peixes, quanto queriam.
//  [...] Recolhei os pedaços que sobraram, para que nada se perca. [...] encheram doze cestos."
//
// TEXTO     : cinco pães e dois peixes bastam para cerca de cinco mil; cada um recebe quanto quer; recolhe-se o que sobra.
// ANALOGIA  : só a DISTRIBUIÇÃO ("quanto queriam") e a CONTA que fecha ("para que nada se perca").
// LIMITE    : o código NÃO modela a multiplicação, que é o centro do relato. Ele começa com os pedaços já existentes.
//             "Fome" aqui é um número de pedaços, escolha nossa: o texto fala em "quanto queriam" e em "fartos", sem contar.

public record Pessoa(string Nome, int Fome);

// <repartir>
public class Distribuicao
{
    public Dictionary<string, int> Servido { get; } = new();
    public Queue<string> Sobra { get; }

    public Distribuicao(IEnumerable<string> pedacos) => Sobra = new Queue<string>(pedacos);

    // "repartiu-os [...] quanto queriam": cada um recebe até a sua fome, enquanto houver
    public void Repartir(IEnumerable<Pessoa> sentados)
    {
        foreach (var pessoa in sentados)
        {
            int recebeu = 0;
            while (recebeu < pessoa.Fome && Sobra.Count > 0)
            {
                Sobra.Dequeue();
                recebeu++;
            }
            Servido[pessoa.Nome] = recebeu;
        }
    }

    // "Recolhei os pedaços que sobraram, para que nada se perca"
    public List<string> Recolher()
    {
        var recolhidos = Sobra.ToList();
        Sobra.Clear();
        return recolhidos;
    }
}
// </repartir>
