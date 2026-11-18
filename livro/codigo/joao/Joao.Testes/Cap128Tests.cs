using Jornada.Joao.Cap128;

namespace Joao.Testes;

public class Cap128Tests
{
    [Fact]
    public void Na_primeira_vez_vai_a_fonte_e_na_segunda_usa_o_poco()
    {
        int idasAFonte = 0;
        var poco = new Poco<string, int>(chave => { idasAFonte++; return chave.Length; }, TimeSpan.FromMinutes(5), new RelogioDeTeste());

        Assert.Equal(4, poco.Obter("água"));
        Assert.Equal(4, poco.Obter("água"));

        Assert.Equal(1, idasAFonte);
        Assert.Equal((1L, 1L, 0L), (poco.Acertos, poco.Falhas, poco.Expiradas));
    }

    [Fact]
    public void Passado_o_prazo_o_poco_busca_de_novo()
    {
        var relogio = new RelogioDeTeste();
        int versao = 0;
        var poco = new Poco<string, string>(_ => $"v{++versao}", TimeSpan.FromMinutes(1), relogio);

        Assert.Equal("v1", poco.Obter("a"));
        relogio.Avancar(TimeSpan.FromSeconds(59));
        Assert.Equal("v1", poco.Obter("a"));                // ainda vale
        relogio.Avancar(TimeSpan.FromSeconds(2));
        Assert.Equal("v2", poco.Obter("a"));                // venceu: nova busca

        Assert.Equal(1, poco.Expiradas);
    }

    [Fact]
    public void Chaves_diferentes_tem_entradas_diferentes()
    {
        var poco = new Poco<int, int>(n => n * n, TimeSpan.FromMinutes(1), new RelogioDeTeste());

        Assert.Equal(new[] { 4, 9, 16 }, new[] { 2, 3, 4 }.Select(poco.Obter));
        Assert.Equal(3, poco.Quantidade);
    }

    [Fact]
    public void Invalidar_obriga_a_ir_a_fonte_na_proxima()
    {
        int idas = 0;
        var poco = new Poco<string, int>(_ => ++idas, TimeSpan.FromMinutes(1), new RelogioDeTeste());

        poco.Obter("x");
        Assert.True(poco.Invalidar("x"));
        Assert.False(poco.Invalidar("x"));
        poco.Obter("x");

        Assert.Equal(2, idas);
    }

    [Fact]
    public void Cem_pedidos_ao_mesmo_tempo_chamam_a_fonte_uma_unica_vez()
    {
        int idas = 0;
        var poco = new Poco<string, int>(_ => { Interlocked.Increment(ref idas); Thread.Sleep(20); return 7; }, TimeSpan.FromMinutes(1));

        Parallel.For(0, 100, _ => Assert.Equal(7, poco.Obter("chave")));

        Assert.Equal(1, idas);                              // o Lazy cuidou de a fonte ser consultada só uma vez
        Assert.Equal(100, poco.Acertos + poco.Falhas);
    }

    [Fact]
    public void Validade_precisa_ser_positiva() =>
        Assert.Throws<ArgumentOutOfRangeException>(() => new Poco<int, int>(n => n, TimeSpan.Zero));

    [Fact]
    public void Esvaziar_limpa_tudo()
    {
        var poco = new Poco<int, int>(n => n, TimeSpan.FromMinutes(1));
        poco.Obter(1); poco.Obter(2);

        poco.Esvaziar();

        Assert.Equal(0, poco.Quantidade);
    }

    [Fact]
    public void Falha_da_fonte_fica_guardada_ate_a_entrada_vencer()
    {
        var relogio = new RelogioDeTeste();
        int tentativas = 0;
        var poco = new Poco<string, int>(_ => { tentativas++; throw new InvalidOperationException("fonte fora do ar"); }, TimeSpan.FromMinutes(1), relogio);

        Assert.Throws<InvalidOperationException>(() => poco.Obter("x"));
        Assert.Throws<InvalidOperationException>(() => poco.Obter("x"));      // a mesma exceção, sem nova tentativa
        Assert.Equal(1, tentativas);

        relogio.Avancar(TimeSpan.FromMinutes(2));
        Assert.Throws<InvalidOperationException>(() => poco.Obter("x"));
        Assert.Equal(2, tentativas);                                          // venceu: tentou de novo
    }
}
