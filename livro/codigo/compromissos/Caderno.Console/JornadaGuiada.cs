using Quatro;
namespace CadernoApp;

public static class JornadaGuiada
{
    private static string Pedir(string pergunta)
    {
        Console.WriteLine(pergunta);
        return Regras.Texto(Console.ReadLine());
    }

    public static void Executar(string caminho)
    {
        IArquivoDoCaderno arquivo = new ArquivoJson(caminho);
        var anterior = arquivo.Ler();
        var caderno = new Caderno();
        Console.WriteLine("UM ACORDO QUE EU POSSO ESCOLHER");
        var acordo = caderno.Inventario.Registrar(
            Pedir("Que regra você percebe na sua vida?"),
            Pedir("De onde você acha que essa regra veio?"));
        Console.WriteLine("Você escolhe praticar esse acordo hoje? Digite sim ou não.");
        string escolha = Console.ReadLine()?.Trim().ToLowerInvariant() ?? "";
        if (escolha != "sim" && escolha != "não" && escolha != "nao")
            throw new ArgumentException("Escolha sim ou não; nada foi salvo.");
        if (escolha == "sim") caderno.Inventario.Escolher(acordo.Id);

        Console.WriteLine("1. CUIDAR DA PALAVRA");
        string descricao = Pedir("Que ação você quer combinar?");
        string data = Pedir("Qual o prazo? Use AAAA-MM-DD.");
        if (!DateOnly.TryParseExact(data, "yyyy-MM-dd", out var prazo))
            throw new ArgumentException("Data inválida; nada foi salvo.");
        caderno.Prometer(descricao, prazo);

        Console.WriteLine("2. SEPARAR A MENSAGEM DA MINHA IDENTIDADE");
        string mensagem = Pedir("Que mensagem você recebeu? Use um exemplo sem dados íntimos.");
        string leitura = Pedir("Que interpretação você fez?");
        string resposta = Pedir("Que resposta você escolhe dar?");
        string limite = Pedir("Que limite você deseja preservar?");
        caderno.Responder(new(mensagem, leitura, resposta, limite));
        caderno.Refletir(new(mensagem, leitura, resposta));

        Console.WriteLine("3. PERGUNTAR ANTES DE CONCLUIR");
        var pergunta = caderno.Perguntar(Pedir("O que você precisa esclarecer?"));
        Console.WriteLine("A hipótese foi confirmada, refutada ou ainda está pendente?");
        string estado = Console.ReadLine()?.Trim().ToLowerInvariant() ?? "";
        if (estado == "confirmada" || estado == "refutada")
            pergunta.Esclarecer(estado == "confirmada", Pedir("Qual resposta você recebeu?"),
                Pedir("Qual a fonte dessa resposta?"));
        else if (estado != "pendente")
            throw new ArgumentException("Estado inválido; nada foi salvo.");

        Console.WriteLine("4. UMA PRÁTICA POSSÍVEL HOJE");
        if (!int.TryParse(Pedir("Quantos minutos você tem disponíveis?"), out int disponiveis))
            throw new ArgumentException("Informe minutos inteiros; nada foi salvo.");
        var plano = new PlanoDoDia(disponiveis);
        string acao = Pedir("Qual pequeno passo você escolhe?");
        if (!int.TryParse(Pedir("Quantos minutos esse passo exige?"), out int minutos))
            throw new ArgumentException("Informe minutos inteiros; nada foi salvo.");
        plano.Incluir(acao, minutos);
        caderno.Diario.Registrar(DateOnly.FromDateTime(DateTime.Today), acao,
            Pedir("O que esta reflexão ensinou a você?"));

        var atual = caderno.Exportar();
        var completo = anterior is null ? atual : new Retrato(1,
            [..anterior.Acordos, ..atual.Acordos], [..anterior.Promessas, ..atual.Promessas],
            [..anterior.Reflexoes, ..atual.Reflexoes], [..anterior.Respostas, ..atual.Respostas],
            [..anterior.Perguntas, ..atual.Perguntas], [..anterior.Praticas, ..atual.Praticas]);
        arquivo.Salvar(completo);
        Console.WriteLine($"Jornada registrada: {completo.Acordos.Length} acordos; {completo.Praticas.Length} práticas.");
        Console.WriteLine("Você pode fechar o programa. O arquivo guarda os registros.");
    }
}
