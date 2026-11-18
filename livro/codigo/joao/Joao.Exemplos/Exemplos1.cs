using Jornada.Joao.Cap124;
using Jornada.Joao.Cap125;
using Jornada.Joao.Cap126;
using Jornada.Joao.Cap127;
using Jornada.Joao.Cap128;

namespace Jornada.Joao.Exemplos;

public sealed class RelogioFixo : TimeProvider
{
    public override DateTimeOffset GetUtcNow() => new(2026, 11, 2, 9, 0, 0, TimeSpan.Zero);
}

public sealed class RelogioManual : TimeProvider
{
    private DateTimeOffset agora = new(2026, 11, 2, 9, 0, 0, TimeSpan.Zero);
    public override DateTimeOffset GetUtcNow() => agora;
    public void Avancar(TimeSpan tempo) => agora += tempo;
}

public static class Demo124
{
    public static void Run()
    {
        var relogio = new RelogioFixo();
        // <uso>
        var linhas = new List<Registro>();
        var lanterna = new Lanterna(Nivel.Informacao, linhas.Add, relogio);

        using (lanterna.Escopo("entrega do relatório"))
        {
            lanterna.Escrever(Nivel.Detalhe, "abrindo o arquivo");              // abaixo do mínimo: fica no escuro
            lanterna.Escrever(Nivel.Informacao, "revisão iniciada", ("revisor", "Lia"));

            using (lanterna.Escopo("testes"))
                lanterna.Escrever(Nivel.Aviso, "2 testes lentos", ("segundos", 14));

            lanterna.Escrever(Nivel.Erro, "publicação recusada", ("motivo", "falta aprovação"));
        }

        foreach (var registro in linhas)
            Console.WriteLine(Lanterna.Formatar(registro));

        Console.WriteLine($"{linhas.Count} linhas escritas; a de nível Detalhe não acendeu");
        // </uso>
    }
}

public static class Demo125
{
    public static void Run()
    {
        // <uso>
        var imagens = Catalogo.Descobrir(typeof(IImagem).Assembly);

        foreach (var imagem in imagens)
        {
            var servico = Catalogo.Criar(imagem);
            Console.WriteLine($"{imagem.Passagem,-9} {imagem.Texto,-31} {servico.Promete()}");
        }

        Console.WriteLine($"{imagens.Count} imagens descobertas por reflexão");
        Console.WriteLine($"O serviço sem etiqueta aparece? {imagens.Any(i => i.Tipo == typeof(ImagemSemEtiqueta))}");
        // </uso>
    }
}

public static class Demo126
{
    public static void Run()
    {
        // <uso>
        var banquete = new Banquete();
        var vasos = banquete.Servir(6);                                         // só uma "receita"
        Console.WriteLine($"Vasos transformados até agora: {banquete.Transformadas}");

        foreach (var taca in vasos)
            Console.WriteLine($"vaso {taca.Vaso}: {taca.Conteudo.Nome}, qualidade {taca.Conteudo.Qualidade} → {Provador.Avaliar(taca.Conteudo)}");

        Console.WriteLine($"Transformadas depois de provar tudo: {banquete.Transformadas}");

        var apenasDuas = new Banquete();
        _ = apenasDuas.Servir(6).Take(2).ToList();
        Console.WriteLine($"Quem prova só duas taças causa {apenasDuas.Transformadas} transformações");

        var palavraParaTamanho = new Transformacao<string, int>(texto => texto.Length)
            .Depois(n => n * 2)
            .Depois(n => $"dobro do tamanho: {n}");
        Console.WriteLine(palavraParaTamanho.Aplicar("caná"));
        // </uso>
    }
}

public static class Demo127
{
    public static void Run()
    {
        // <uso>
        var vento = new Vento<int>();

        vento.Onde(n => n % 2 == 0)
             .Transformar(n => $"par {n}")
             .Ouvir(texto => Console.WriteLine($"  ouvinte A ouviu: {texto}"));

        var inscricaoB = vento.Ouvir(n => Console.WriteLine($"  ouvinte B viu {n}"));

        for (int n = 1; n <= 4; n++)
        {
            Console.WriteLine($"o vento sopra {n}");
            vento.Soprar(n);
            if (n == 2) inscricaoB.Dispose();                // B vai embora depois do 2
        }

        var memoria = new VentoComMemoria<string>(capacidade: 2);
        memoria.Soprar("a"); memoria.Soprar("b"); memoria.Soprar("c");
        memoria.Ouvir(letra => Console.WriteLine($"quem nasceu de novo recebeu: {letra}"));
        // </uso>
    }
}

public static class Demo128
{
    public static void Run()
    {
        // <uso>
        var relogio = new RelogioManual();
        int idasAFonte = 0;

        var poco = new Poco<string, string>(
            nome => { idasAFonte++; return $"água do {nome} (busca nº {idasAFonte})"; },
            validade: TimeSpan.FromMinutes(5),
            relogio);

        Console.WriteLine(poco.Obter("poço de Jacó"));
        Console.WriteLine(poco.Obter("poço de Jacó"));          // vem do poço, sem ir à fonte

        relogio.Avancar(TimeSpan.FromMinutes(6));
        Console.WriteLine(poco.Obter("poço de Jacó"));          // venceu: vai à fonte de novo

        Console.WriteLine($"acertos={poco.Acertos} · falhas={poco.Falhas} · expiradas={poco.Expiradas} · idas à fonte={idasAFonte}");
        // </uso>
    }
}
