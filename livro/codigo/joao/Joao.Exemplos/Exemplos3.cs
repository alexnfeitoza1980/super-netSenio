using Jornada.Joao.Texto;

namespace Jornada.Joao.Exemplos;

// Exemplos do "passo a passo" de cada capítulo: cada classe imprime o que o capítulo mostra, a partir do versículo.

public static class DemoT124
{
    public static void Run()
    {
        // <uso>
        // "e a Palavra era Deus": uma relação entre TIPOS, verdadeira sem criar nenhum objeto
        Console.WriteLine($"Pelos tipos, o Verbo era Deus? {typeof(Deus).IsAssignableFrom(typeof(Verbo))}");

        // Para rodar o exemplo, o programa precisa de objetos para falar deles. O "new" é do programa, e não do texto.
        var deus = new Deus();
        var verbo = new Verbo { EstavaCom = deus };

        Console.WriteLine($"O Verbo estava junto de Deus (referência)? {ReferenceEquals(verbo.EstavaCom, deus)}");
        Console.WriteLine($"O objeto verbo é do tipo Deus (teste `is`)? {verbo is Deus}");
        Console.WriteLine($"São a mesma referência na memória (outra pergunta)? {ReferenceEquals(verbo, deus)}");

        var luz = new Luz();
        var trevas = new Trevas();
        luz.Resplandecer(trevas);
        Console.WriteLine($"A luz brilha nas trevas: {trevas.Iluminadas}");
        Console.WriteLine($"As trevas a compreenderam? {trevas.Compreendeu(luz)} (declarado pelo texto, não calculado)");
        // </uso>
    }
}

public static class DemoT125
{
    public static void Run()
    {
        // <uso>
        foreach (var declaracao in Declaracoes.Todas)
            Console.WriteLine(declaracao.Frase());
        // </uso>
    }
}

public static class DemoT126
{
    public static void Run()
    {
        // <uso>
        var vasos = Enumerable.Range(1, 6).Select(_ => new Vaso()).ToList();

        foreach (var vaso in vasos)
            vaso.Encher(new Bebida("água", Qualidade: 3));                      // "Enchei estes vasos com água"

        Console.WriteLine("Depois de encher:          " + string.Join(", ", vasos.Select(v => v.Conteudo)));

        var amostra = vasos[0].Tirar();                                           // "Agora tirai"
        Console.WriteLine("O mestre de cerimônia diz: " + new MestreDeCerimonia().Provar(amostra));

        Console.WriteLine("Depois de tirar do 1º:     " + string.Join(", ", vasos.Select(v => v.Conteudo)));
        // </uso>
    }
}

public static class DemoT127
{
    public static void Run()
    {
        // <uso>
        var vento = new VentoSimples();
        var primeiro = new OuvinteDoVento();
        var segundo = new OuvinteDoVento();

        vento.Soprou += primeiro.Ouvir;
        vento.Soprou += segundo.Ouvir;
        vento.Soprar("shhh");

        vento.Soprou -= segundo.Ouvir;                    // o segundo deixa de ouvir
        vento.Soprar("fuuu");

        Console.WriteLine("Primeiro ouviu: " + string.Join(", ", primeiro.Ouvido));
        Console.WriteLine("Segundo ouviu:  " + string.Join(", ", segundo.Ouvido));
        Console.WriteLine("Ouvintes agora: " + vento.Ouvintes);
        // </uso>
    }
}

public static class DemoT128
{
    public static void Run()
    {
        // <uso>
        var semFonte = new PocoFundo();
        for (int i = 0; i < 3; i++) semFonte.TirarAgua();                       // "voltará a ter sede"
        Console.WriteLine($"Sem fonte dentro: 3 goles, {semFonte.Descidas} descidas ao poço");

        var poco = new PocoFundo();
        var fonte = new FonteDentro(poco);
        for (int i = 0; i < 3; i++) fonte.Beber();                              // "se fará nele fonte"
        Console.WriteLine($"Com fonte dentro: 3 goles, {poco.Descidas} descida ao poço");
        // </uso>
    }
}

public static class DemoT129
{
    public static void Run()
    {
        // <uso>
        var pedacos = Enumerable.Range(1, 12).Select(i => $"pedaço {i}");      // o que havia para repartir (a multiplicação NÃO é modelada)
        var distribuicao = new Distribuicao(pedacos);

        var sentados = new[] { new Pessoa("André", 3), new Pessoa("Filipe", 4), new Pessoa("Tomé", 2) };
        distribuicao.Repartir(sentados);

        foreach (var (nome, recebeu) in distribuicao.Servido)
            Console.WriteLine($"{nome} recebeu {recebeu}");

        var recolhidos = distribuicao.Recolher();
        Console.WriteLine($"Sobraram e foram recolhidos: {recolhidos.Count}");
        Console.WriteLine($"Nada se perdeu? {distribuicao.Servido.Values.Sum() + recolhidos.Count == 12}");
        // </uso>
    }
}

public static class DemoT130
{
    public static void Run()
    {
        // <uso>
        var ovelha = new Ovelha("voz do pastor");

        Console.WriteLine($"Segue a voz do pastor? {ovelha.Segue("voz do pastor")}");
        Console.WriteLine($"Segue a voz de um estranho? {ovelha.Segue("voz de um estranho")}");

        // EXTENSÃO NOSSA (não está em João 10): e se o estranho repetir a voz que ouviu?
        string vozOuvidaPeloEstranho = "voz do pastor";
        Console.WriteLine($"Segue um estranho que repete a voz? {ovelha.Segue(vozOuvidaPeloEstranho)}");
        // </uso>
    }
}

public static class DemoT131
{
    public static void Run()
    {
        // <uso>
        var lazaro = new Lazaro();
        Console.WriteLine($"Começa: {lazaro.Condicao}, ataduras: {lazaro.Ataduras.Count}");

        lazaro.Chamar();                                   // "Lázaro, sai fora."
        Console.WriteLine($"Chamado: {lazaro.Condicao}, livre? {lazaro.Livre}");

        foreach (var atadura in lazaro.Ataduras.ToList())
        {
            lazaro.Desatar(atadura);                       // "Desatai-o"
            Console.WriteLine($"Desatou: {atadura} (faltam {lazaro.Ataduras.Count})");
        }

        Console.WriteLine($"Livre? {lazaro.Livre}");        // "e deixai-o ir"
        // </uso>
    }
}

public static class DemoT132
{
    public static void Run()
    {
        // <uso>
        IPessoa jesus = new Jesus();
        IPessoa servindo = new ComToalha(jesus);

        Console.WriteLine($"{servindo.Nome} (o mesmo nome, antes e depois da toalha):");
        foreach (var ato in servindo.Atos())
            Console.WriteLine("  - " + ato);

        // "também vós deveis": qualquer pessoa pode vestir a toalha
        var pedro = new ComToalha(new Discipulo("Pedro"));
        Console.WriteLine($"{pedro.Nome} lava os pés dos discípulos? {pedro.Atos().Contains("lava os pés dos discípulos")}");
        // </uso>
    }
}

public static class DemoT133
{
    public static void Run()
    {
        // <uso>
        var videira = new Videira();
        var ramos = new List<Ramo> { new(videira), new(videira) { DaFruto = false }, new(videira) };

        Console.WriteLine("Ligado à videira: " + ramos[0].DarFruto());

        try { _ = new Ramo(null!); }
        catch (ArgumentNullException e) { Console.WriteLine("Sem a videira: " + e.Message.Split('(')[0].Trim()); }

        var (tirados, limpos) = new Lavrador().Podar(ramos);
        Console.WriteLine($"O lavrador tirou {tirados.Count} e limpou {limpos.Count}; ficam {ramos.Count} ramos");
        // </uso>
    }
}
