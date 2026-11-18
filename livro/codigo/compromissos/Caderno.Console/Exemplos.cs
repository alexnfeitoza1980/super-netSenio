using Quatro;
namespace CadernoApp;
public static class Exemplos
{
    public static void Cap74()
    {
        var caderno = new Caderno();
        var acordo = caderno.Inventario.Registrar("Responder imediatamente", "Hábito do grupo");
        Console.WriteLine($"Acordos registrados: {caderno.Inventario.Todos.Count}");
        Console.WriteLine($"Escolhido por mim: {acordo.Escolhido}");
    }
    public static void Cap75()
    {
        var caderno = new Caderno();
        var reflexao = new Reflexao("Não respondi hoje", "Sou um péssimo amigo", "Perguntar quando podemos conversar");
        caderno.Refletir(reflexao);
        Console.WriteLine($"Observação: {reflexao.Observacao}");
        Console.WriteLine($"Próximo passo: {reflexao.ProximoPasso}");
    }
    public static void Cap76()
    {
        var caderno = new Caderno();
        var promessa = caderno.Prometer("Enviar a resposta", new DateOnly(2026, 11, 6));
        Console.WriteLine(promessa.Estado);
        promessa.Cumprir(new DateOnly(2026, 11, 5));
        Console.WriteLine(promessa.Estado);
    }
    public static void Cap77()
    {
        var caderno = new Caderno();
        var promessa = caderno.Prometer("Revisar o texto", new DateOnly(2026, 11, 6));
        promessa.Renegociar(new DateOnly(2026, 11, 9), new DateOnly(2026, 11, 5), "Combinamos outro dia");
        Console.WriteLine($"Revisões: {promessa.Revisoes.Count}");
        Console.WriteLine($"Motivo: {promessa.Revisoes[0].Motivo}");
    }
    public static void Cap78()
    {
        var caderno = new Caderno();
        var resposta = new RespostaEscolhida("Não gostei do texto", "Acho que não gostam de mim",
            "Que trecho precisa melhorar?", "Posso ouvir críticas sem aceitar insultos");
        caderno.Responder(resposta);
        Console.WriteLine($"Recebido: {resposta.MensagemRecebida}");
        Console.WriteLine($"Responderei: {resposta.Resposta}");
    }
    public static void Cap79()
    {
        var caderno = new Caderno();
        caderno.Inventario.Registrar("Responder imediatamente", "Hábito antigo");
        var novo = caderno.Inventario.Registrar("Combinar um horário para responder", "Escolha consciente");
        caderno.Inventario.Escolher(novo.Id);
        foreach (var a in caderno.Inventario.Buscar("responder"))
            Console.WriteLine($"{a.Texto}: escolhido={a.Escolhido}");
    }
    public static void Cap80()
    {
        var caderno = new Caderno();
        var pergunta = caderno.Perguntar("A entrega precisa ser impressa?");
        Console.WriteLine(pergunta.Estado);
        Console.WriteLine(pergunta.Evidencia is null ? "Ainda não sabemos" : "Há um registro");
    }
    public static void Cap81()
    {
        var caderno = new Caderno();
        var pergunta = caderno.Perguntar("A entrega precisa ser impressa?");
        pergunta.Esclarecer(false, "Pode ser em PDF", "Conversa com Lia em 05/11");
        Console.WriteLine(pergunta.Estado);
        Console.WriteLine($"Fonte: {pergunta.Evidencia?.Fonte}");
    }
    public static void Cap82()
    {
        var plano = new PlanoDoDia(40);
        plano.Incluir("Esclarecer uma dúvida", 10);
        plano.Incluir("Revisar minha promessa", 20);
        Console.WriteLine($"Planejados: {plano.Planejados} de {plano.Disponiveis} minutos");
        Console.WriteLine("Tempo disponível é informado, não diagnosticado.");
    }
    public static void Cap83()
    {
        var caderno = new Caderno();
        caderno.Diario.Registrar(new DateOnly(2026, 11, 1), "Perguntei antes de supor", "Entendi o pedido");
        caderno.Diario.Registrar(new DateOnly(2026, 11, 3), "Voltei a praticar", "Uma pausa não apaga o aprendizado");
        Console.WriteLine($"Dias praticados: {caderno.Diario.DiasPraticados}");
        Console.WriteLine($"Registros preservados: {caderno.Diario.Registros.Count}");
    }
    public static void Cap84()
    {
        var caderno = new Caderno();
        var promessa = caderno.Prometer("Conversar na sexta", new DateOnly(2026, 11, 6));
        IArquivoDoCaderno arquivo = new ArquivoEmMemoria();
        arquivo.Salvar(caderno.Exportar());
        promessa.Cumprir(new DateOnly(2026, 11, 5));
        Console.WriteLine($"Objeto de trabalho: {promessa.Estado}");
        Console.WriteLine($"Retrato salvo antes: {arquivo.Ler()!.Promessas[0].Estado}");
    }
    public static void Cap85()
    {
        var caderno = new Caderno();
        caderno.Prometer("Rever a conversa", new DateOnly(2026, 11, 7));
        caderno.Prometer("Fazer uma pergunta", new DateOnly(2026, 11, 5));
        var concluida = caderno.Prometer("Enviar resposta", new DateOnly(2026, 11, 4));
        concluida.Cumprir(new DateOnly(2026, 11, 4));
        foreach (var p in caderno.Pendentes()) Console.WriteLine(p.Descricao);
    }
    public static void Cap86()
    {
        var caderno = new Caderno();
        caderno.Diario.Registrar(new DateOnly(2026, 11, 5), "Pedi esclarecimento", "Não precisava adivinhar");
        string pasta = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(pasta);
        try
        {
            IArquivoDoCaderno arquivo = new ArquivoJson(Path.Combine(pasta, "caderno.json"));
            arquivo.Salvar(caderno.Exportar());
            var reaberto = new ArquivoJson(Path.Combine(pasta, "caderno.json")).Ler()!;
            Console.WriteLine($"Registros recuperados: {reaberto.Praticas.Length}");
            Console.WriteLine(reaberto.Praticas[0].Aprendizado);
        }
        finally { Directory.Delete(pasta, true); }
    }
    public static void Cap87()
    {
        var caderno = new Caderno();
        var acordo = caderno.Inventario.Registrar("Perguntar antes de adivinhar", "Prática consciente");
        caderno.Inventario.Escolher(acordo.Id);
        var pergunta = caderno.Perguntar("Qual formato você prefere?");
        pergunta.Esclarecer(true, "Um PDF", "Resposta direta da pessoa");
        var promessa = caderno.Prometer("Preparar o PDF", new DateOnly(2026, 11, 6));
        promessa.Cumprir(new DateOnly(2026, 11, 5));
        caderno.Diario.Registrar(new DateOnly(2026, 11, 5), "Perguntei e cumpri o combinado", "Ouvir mudou meu plano");
        var r = caderno.Exportar();
        Console.WriteLine($"Acordos: {r.Acordos.Length}; perguntas: {r.Perguntas.Length}");
        Console.WriteLine($"Promessa: {r.Promessas[0].Estado}; práticas: {r.Praticas.Length}");
    }
}
