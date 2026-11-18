using System.Text;
using Jornada.Joao.Cap124;
using Jornada.Joao.Cap129;
using Jornada.Joao.Cap130;
using Jornada.Joao.Cap131;
using Jornada.Joao.Cap132;
using Jornada.Joao.Cap133;
using Jornada.Joao.Exercicios;

namespace Jornada.Joao.Exemplos;

public static class Demo129
{
    public static async Task Run()
    {
        // <uso>
        var fomes = new[] { 2, 2, 2, 2, 2 };                                    // cinco pessoas, duas porções cada

        var resultado = await Distribuicao.DistribuirAsync(
            Distribuicao.PartirAsync(paes: 5, peixes: 2, pedacosPorAlimento: 4),   // 28 porções no total
            fomes,
            capacidadeDoCesto: 3);

        Console.WriteLine($"servidos: {string.Join(", ", resultado.ServidosPorPessoa)} (total {resultado.TotalServido})");

        var cestos = Distribuicao.EmCestos(resultado.Sobras, porCesto: 6);
        Console.WriteLine($"sobras: {resultado.Sobras.Count} porções, juntadas em {cestos.Count} cestos");
        foreach (var (cesto, numero) in cestos.Select((c, i) => (c, i + 1)))
            Console.WriteLine($"  cesto {numero}: {cesto.Count} porções");

        Console.WriteLine($"nada se perdeu? {resultado.TotalServido + resultado.Sobras.Count == 28}");
        // </uso>
    }
}

public static class Demo130
{
    public static void Run()
    {
        var relogio = new RelogioManual();
        // <uso>
        var voz = new Voz(Encoding.UTF8.GetBytes("um-segredo-longo-de-estudo-123"), relogio);
        var porta = new Porta(voz);

        var token = voz.Emitir("Ana", "cuidadora", TimeSpan.FromMinutes(30));
        Console.WriteLine($"a voz começa assim: {token[..20]}...");

        Console.WriteLine($"voz reconhecida: {porta.Entrar(token)}");
        Console.WriteLine($"política exige perfil admin: {porta.Entrar(token, quem => quem.Perfil == "admin")}");

        var deEstranho = new Voz(Encoding.UTF8.GetBytes("outro-segredo-qualquer-0000000"), relogio)
            .Emitir("Ana", "cuidadora", TimeSpan.FromMinutes(30));
        Console.WriteLine($"voz de um estranho: {porta.Entrar(deEstranho)}");
        Console.WriteLine($"texto qualquer: {porta.Entrar("não-sou-uma-voz")}");

        relogio.Avancar(TimeSpan.FromMinutes(31));
        Console.WriteLine($"31 minutos depois: {porta.Entrar(token)}");
        // </uso>
    }
}

public static class Demo131
{
    public static void Run()
    {
        // <uso>
        var livro = new LivroDeEventos();
        var inicio = new DateTimeOffset(2026, 11, 2, 9, 0, 0, TimeSpan.Zero);

        livro.Registrar(new Faleceu(inicio));
        livro.Registrar(new PedraTirada(inicio.AddMinutes(10)));
        livro.Registrar(new Chamado(inicio.AddMinutes(20)));
        livro.Registrar(new Saiu(inicio.AddMinutes(21)));

        var estado = livro.Estado();
        Console.WriteLine($"depois de sair: {estado.Situacao}; ataduras: {string.Join(" + ", estado.Ataduras)}; livre? {estado.Livre}");

        foreach (var atadura in estado.Ataduras.ToList())
            livro.Registrar(new Desatado(inicio.AddMinutes(22), atadura));

        Console.WriteLine($"depois de desatado: livre? {livro.Estado().Livre}");

        var aos15 = livro.EstadoEm(inicio.AddMinutes(15));
        Console.WriteLine($"como estava aos 15 minutos? {aos15.Situacao}; sepulcro aberto? {aos15.SepulcroAberto}; já saiu? {aos15.JaSaiu}");
        Console.WriteLine($"{livro.Eventos.Count} eventos guardados; nenhum foi apagado");
        // </uso>
    }
}

public static class Demo132
{
    public static async Task Run()
    {
        var relogio = new RelogioFixo();
        // <uso>
        var chao = new RepositorioEmMemoria();
        var linhas = new List<Registro>();

        IRepositorioDeTarefas repositorio = chao
            .ComCache()
            .ComTentativas(3)
            .ComRegistro(new Lanterna(Nivel.Informacao, linhas.Add, relogio));

        await chao.SalvarAsync(new Tarefa(1, "Lavar os pés dos discípulos", false));   // direto no "chão"
        chao.FalharProximas(2);                                                         // o serviço vai falhar duas vezes

        var primeira = await repositorio.ObterAsync(1);     // falha, falha e, na terceira, funciona
        var segunda = await repositorio.ObterAsync(1);      // agora vem do cache

        Console.WriteLine($"tarefa: {primeira!.Titulo}");
        Console.WriteLine($"leituras que chegaram ao serviço de verdade: {chao.Leituras}");
        foreach (var registro in linhas)
            Console.WriteLine(Lanterna.Formatar(registro));
        // </uso>
    }
}

public static class Demo133
{
    public static void Run()
    {
        // <uso>
        // As peças do exemplo (cada uma declara, no construtor, do que precisa):
        //   interface ISeiva;             class SeivaDaTerra : ISeiva
        //   interface IRamo;              class Ramo(ISeiva seiva) : IRamo
        //   interface IFruto;             class Fruto(IRamo ramo, ISeiva seiva) : IFruto
        // </uso>
        // <uso>
        using var videira = new Recipiente()
            .Registrar<ISeiva, SeivaDaTerra>(Ciclo.Permanente)
            .Registrar<IRamo, Ramo>()
            .Registrar<IFruto, Fruto>();

        var fruto = videira.Obter<IFruto>();
        Console.WriteLine($"o fruto recebeu o ramo e a seiva '{fruto.Seiva.Nome}'");
        Console.WriteLine($"ramo e fruto bebem da mesma seiva? {ReferenceEquals(fruto.Seiva, fruto.Ramo.Seiva)}");
        Console.WriteLine($"dois pedidos de ramo dão o mesmo ramo? {ReferenceEquals(videira.Obter<IRamo>(), videira.Obter<IRamo>())}");

        try { new Recipiente().Registrar<IRamo, Ramo>().Obter<IRamo>(); }
        catch (InvalidOperationException e) { Console.WriteLine($"sem seiva registrada: {e.Message}"); }
        // </uso>
    }

    public interface ISeiva { string Nome { get; } }
    public sealed class SeivaDaTerra : ISeiva { public string Nome => "da terra"; }
    public interface IRamo { ISeiva Seiva { get; } }
    public sealed class Ramo : IRamo { public Ramo(ISeiva seiva) => Seiva = seiva; public ISeiva Seiva { get; } }
    public interface IFruto { IRamo Ramo { get; } ISeiva Seiva { get; } }
    public sealed class Fruto : IFruto
    {
        public Fruto(IRamo ramo, ISeiva seiva) { Ramo = ramo; Seiva = seiva; }
        public IRamo Ramo { get; }
        public ISeiva Seiva { get; }
    }
}
