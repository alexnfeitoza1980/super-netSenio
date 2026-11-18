using Jornada.Joao.Cap124;
using Jornada.Joao.Cap125;
using Jornada.Joao.Cap126;
using Jornada.Joao.Cap127;
using Jornada.Joao.Cap128;
using Jornada.Joao.Cap129;
using Jornada.Joao.Cap130;
using Jornada.Joao.Cap131;
using Jornada.Joao.Cap132;
using Jornada.Joao.Cap133;
using Jornada.Joao.Exercicios;

namespace Joao.Testes;

public class ExerciciosTests
{
    private static DateTimeOffset T(int m) => new DateTimeOffset(2026, 11, 2, 9, 0, 0, TimeSpan.Zero).AddMinutes(m);

    [Fact]
    public void Cap124_Resumo_conta_por_nivel_do_mais_grave_ao_mais_leve()
    {
        var linhas = new List<Registro>();
        var lanterna = new Lanterna(Nivel.Detalhe, linhas.Add);
        lanterna.Escrever(Nivel.Informacao, "a");
        lanterna.Escrever(Nivel.Erro, "b");
        lanterna.Escrever(Nivel.Informacao, "c");

        Assert.Equal("Erro: 1 · Informacao: 2", linhas.Resumo());
        Assert.Equal("", new List<Registro>().Resumo());
    }

    [Fact]
    public void Cap125_Descrever_mostra_imagem_passagem_e_promessa()
    {
        var imagens = Catalogo.Descobrir(typeof(IImagem).Assembly);
        var pastor = imagens.Single(i => i.Texto == "o bom pastor");

        Assert.Equal("o bom pastor (Jo 10,11): dá sua vida pelas ovelhas", pastor.Descrever());
        Assert.Equal("x (sem passagem): y", new Imagem(typeof(Pastor), "x", null, "y").Descrever());
    }

    [Fact]
    public void Cap126_Repetir_aplica_a_transformacao_varias_vezes()
    {
        var mais1 = new Transformacao<Liquido, Liquido>(l => l with { Qualidade = l.Qualidade + 1 });

        Assert.Equal(3, mais1.Repetir(3).Aplicar(new Liquido("x", 0)).Qualidade);
        Assert.Equal(0, mais1.Repetir(0).Aplicar(new Liquido("x", 0)).Qualidade);
        Assert.Throws<ArgumentOutOfRangeException>(() => mais1.Repetir(-1));
    }

    [Fact]
    public void Cap127_Cada_ouvinte_tem_a_sua_propria_contagem()
    {
        var vento = new Vento<string>();
        var contagem = vento.Contagem();
        var a = new List<int>();
        var b = new List<int>();

        contagem.Ouvir(a.Add);
        vento.Soprar("x");
        vento.Soprar("y");
        contagem.Ouvir(b.Add);                               // chegou depois
        vento.Soprar("z");

        Assert.Equal(new[] { 1, 2, 3 }, a);
        Assert.Equal(new[] { 1 }, b);                        // começa do 1: a conta não é compartilhada
    }

    [Fact]
    public void Cap128_Taxa_de_acerto_e_zero_sem_pedidos_e_calcula_a_fracao()
    {
        var poco = new Poco<int, int>(n => n, TimeSpan.FromMinutes(1), new RelogioDeTeste());
        Assert.Equal(0, poco.TaxaDeAcerto());

        poco.Obter(1); poco.Obter(1); poco.Obter(1); poco.Obter(2);     // 2 acertos em 4 pedidos

        Assert.Equal(0.5, poco.TaxaDeAcerto(), 6);
    }

    [Fact]
    public async Task Cap129_Pessoas_saciadas_conta_quem_recebeu_tudo()
    {
        var fomes = new[] { 2, 2, 2 };
        var resultado = await Distribuicao.DistribuirAsync(Distribuicao.PartirAsync(2, 0, 2), fomes, 2);   // só 4 porções

        Assert.Equal(4, resultado.TotalServido);
        Assert.Equal(2, resultado.PessoasSaciadas(fomes));
    }

    [Fact]
    public void Cap130_Politicas_combinam_com_todas()
    {
        var exp = DateTimeOffset.UtcNow.AddHours(1);
        var marta = new Identidade("Marta", "cuidadora", exp);
        var ana = new Identidade("Ana", "dev", exp);

        var soCuidadores = Politicas.Perfil("cuidadora", "pastora");
        var nomeComM = (Func<Identidade, bool>)(i => i.Nome.StartsWith('M'));

        Assert.True(Politicas.Todas(soCuidadores, nomeComM)(marta));
        Assert.False(Politicas.Todas(soCuidadores, nomeComM)(ana));
        Assert.True(Politicas.Todas()(ana));                 // nenhuma exigência: todos passam
    }

    [Fact]
    public void Cap131_Historia_conta_o_que_aconteceu_em_linguagem_simples()
    {
        var livro = new LivroDeEventos();
        livro.Registrar(new Faleceu(T(0)));
        livro.Registrar(new PedraTirada(T(5)));
        livro.Registrar(new Chamado(T(9)));
        livro.Registrar(new Saiu(T(10)));
        livro.Registrar(new Desatado(T(11), "lenço no rosto"));

        Assert.Equal(new[]
        {
            "1. faleceu", "2. a pedra foi tirada", "3. foi chamado", "4. saiu", "5. desatado: lenço no rosto"
        }, livro.Historia());
    }

    [Fact]
    public async Task Cap132_Somente_leitura_deixa_ler_e_recusa_escrever()
    {
        var chao = new RepositorioEmMemoria();
        await chao.SalvarAsync(new Tarefa(1, "x", false));
        IRepositorioDeTarefas leitura = new SomenteLeitura(chao);

        Assert.NotNull(await leitura.ObterAsync(1));
        await Assert.ThrowsAsync<InvalidOperationException>(() => leitura.SalvarAsync(new Tarefa(2, "y", false)));
    }

    public sealed class Configuracao { public string Nome { get; init; } = ""; }

    [Fact]
    public void Cap133_RegistrarInstancia_devolve_sempre_a_mesma_peca()
    {
        var minha = new Configuracao { Nome = "produção" };
        using var videira = new Recipiente().RegistrarInstancia(minha);

        Assert.Same(minha, videira.Obter<Configuracao>());
        Assert.Same(minha, videira.Obter<Configuracao>());
    }
}
