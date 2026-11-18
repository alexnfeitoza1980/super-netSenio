namespace Jornada.Joao.Texto;

// Jo 13,4-5.13-15 (Bíblia Livre): "Levantou-se da ceia, e tirou as roupas, e tomando uma toalha, envolveu-a em si; Depois pôs água em uma bacia,
//  e começou a lavar os pés dos discípulos, e limpá-los com a toalha [...] Vós me chamais Mestre, e Senhor, e bem dizeis; que eu o sou;
//  Pois se eu, o Senhor, e o Mestre, tenho lavado vossos pés, também vós deveis lavar vossos pés uns aos outros. Porque vos tenho dado exemplo [...]"
//
// TEXTO     : Jesus serve e continua sendo Mestre e Senhor (13,13-14); o gesto é dado como exemplo (13,15).
// ANALOGIA  : o decorator: uma camada que acrescenta o serviço SEM mudar quem a veste (mesmo nome, mesmo contrato).
// LIMITE    : o código não explica o sentido do gesto. A relação entre o Verbo (cap. 124) e Jesus (Jo 1,14) é uma leitura
//             cristã do próprio Evangelho que NÃO é modelada aqui: neste capítulo, Jesus é só uma pessoa à mesa (IPessoa).

public interface IPessoa
{
    string Nome { get; }
    IReadOnlyList<string> Atos();
}

public class Jesus : IPessoa
{
    public string Nome => "Jesus (Mestre e Senhor)";
    public IReadOnlyList<string> Atos() => new[] { "está à mesa" };
}

public class Discipulo : IPessoa
{
    public Discipulo(string nome) => Nome = nome;
    public string Nome { get; }
    public IReadOnlyList<string> Atos() => new[] { "está à mesa" };
}

// <toalha>
// A toalha não muda quem a veste: continua sendo uma IPessoa, com o mesmo nome. Só acrescenta o serviço.
public class ComToalha : IPessoa
{
    private readonly IPessoa pessoa;

    public ComToalha(IPessoa pessoa) => this.pessoa = pessoa;

    public string Nome => pessoa.Nome;

    public IReadOnlyList<string> Atos() => pessoa.Atos().Concat(new[]
    {
        "levanta-se da ceia",
        "tira as roupas",
        "envolve-se com uma toalha",
        "põe água em uma bacia",
        "lava os pés dos discípulos",
        "limpa-os com a toalha",
    }).ToList();
}
// </toalha>
