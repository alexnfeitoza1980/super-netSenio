namespace Jornada.Joao.Texto;

// Jo 15,1-5 (Bíblia Livre): "Eu sou a videira verdadeira, e meu Pai é o lavrador. Todo ramo que em mim não dá fruto, ele o tira;
//  e todo o que dá fruto, ele o limpa, para que dê mais fruto. [...] Estai em mim, e eu em vós; como o ramo de si mesmo não pode
//  dar fruto, se não estiver na videira [...] porque sem mim nada podeis fazer."
//
// TEXTO     : a imagem da videira, dos ramos e do lavrador para falar de permanecer em Jesus e dar fruto.
// ANALOGIA  : o ramo RECEBE o que precisa da videira (injeção de dependência) e não o fabrica.
// LIMITE    : "permanecer" e "dar fruto" são relações espirituais que o texto não define em termos de código. O código só
//             aproveita a ESTRUTURA da imagem (quem recebe, de quem, e quem cuida). Em 15,6 o texto fala do ramo lançado fora
//             e queimado; o código não modela isso nem qualquer juízo.

public class Videira
{
    public string Seiva => "seiva da videira";
}

// <ramo>
public class Ramo
{
    private readonly Videira videira;

    // "Estai em mim": o ramo só existe ligado à videira; sem ela, nem pode ser criado
    public Ramo(Videira videira) =>
        this.videira = videira ?? throw new ArgumentNullException(nameof(videira), "Sem mim nada podeis fazer.");

    public bool DaFruto { get; set; } = true;

    // "como o ramo de si mesmo não pode dar fruto": o fruto vem da seiva que recebe
    public string DarFruto() => DaFruto ? $"fruto feito da {videira.Seiva}" : "nenhum fruto";
}
// </ramo>

// <lavrador>
// "meu Pai é o lavrador": aqui o lavrador é só o papel de quem cuida da videira.
public class Lavrador
{
    // "Todo ramo [...] que não dá fruto, ele o tira; e todo o que dá fruto, ele o limpa, para que dê mais fruto."
    public (List<Ramo> Tirados, List<Ramo> Limpos) Podar(List<Ramo> ramos)
    {
        var tirados = ramos.Where(r => !r.DaFruto).ToList();
        var limpos = ramos.Where(r => r.DaFruto).ToList();

        ramos.RemoveAll(tirados.Contains);
        return (tirados, limpos);
    }
}
// </lavrador>
