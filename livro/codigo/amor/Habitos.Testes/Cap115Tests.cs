using Jornada.Habitos.Cap115;

namespace Habitos.Testes;

public class Cap115Tests
{
    private static Etapa Passa(string nome) => Etapas.Verificar(nome, _ => Task.FromResult(true), "falhou");
    private static Etapa Falha(string nome) => Etapas.Verificar(nome, _ => Task.FromResult(false), "não passou");

    [Fact]
    public async Task As_etapas_rodam_na_ordem_em_que_foram_registradas()
    {
        var contexto = new ContextoDeEntrega("corrige cálculo de juros");
        var pipeline = new Pipeline().Usar(Passa("compilar")).Usar(Passa("testar")).Usar(Passa("revisar")).Usar(Passa("publicar"));

        await pipeline.ExecutarAsync(contexto);

        Assert.Equal(new[] { "✓ compilar", "✓ testar", "✓ revisar", "✓ publicar" }, contexto.Registro);
        Assert.False(contexto.Interrompido);
    }

    [Fact]
    public async Task Quando_uma_etapa_falha_as_seguintes_nem_comecam()
    {
        var contexto = new ContextoDeEntrega("x");
        var pipeline = new Pipeline().Usar(Passa("compilar")).Usar(Falha("testar")).Usar(Passa("publicar"));

        await pipeline.ExecutarAsync(contexto);

        Assert.True(contexto.Interrompido);
        Assert.Equal("testar: não passou", contexto.Motivo);
        Assert.Equal(new[] { "✓ compilar", "✗ testar: não passou" }, contexto.Registro);
    }

    [Fact]
    public async Task Pipeline_vazio_nao_faz_nada_e_nao_quebra()
    {
        var contexto = new ContextoDeEntrega("x");
        await new Pipeline().ExecutarAsync(contexto);
        Assert.Empty(contexto.Registro);
    }

    [Fact]
    public async Task Uma_etapa_em_volta_roda_antes_e_depois_do_resto()
    {
        var ordem = new List<string>();
        var pipeline = new Pipeline()
            .Usar(async (ctx, proximo) => { ordem.Add("antes"); await proximo(); ordem.Add("depois"); })
            .Usar(async (ctx, proximo) => { ordem.Add("meio"); await proximo(); });

        await pipeline.ExecutarAsync(new ContextoDeEntrega("x"));

        Assert.Equal(new[] { "antes", "meio", "depois" }, ordem);
    }

    [Fact]
    public async Task A_etapa_de_medicao_envolve_todas_as_outras()
    {
        string? relatorio = null;
        var pipeline = new Pipeline()
            .Usar(Etapas.Medir(texto => relatorio = texto))
            .Usar(async (ctx, proximo) => { await Task.Delay(20); await proximo(); });

        await pipeline.ExecutarAsync(new ContextoDeEntrega("ajuste de layout"));

        Assert.NotNull(relatorio);
        Assert.StartsWith("Entrega de \"ajuste de layout\" levou", relatorio);
    }

    [Fact]
    public async Task Cada_execucao_tem_o_seu_proprio_contexto()
    {
        var pipeline = new Pipeline().Usar(Passa("a"));
        var c1 = new ContextoDeEntrega("um");
        var c2 = new ContextoDeEntrega("dois");

        await pipeline.ExecutarAsync(c1);
        await pipeline.ExecutarAsync(c2);

        Assert.Single(c1.Registro);
        Assert.Single(c2.Registro);
    }

    [Fact]
    public async Task A_verificacao_pode_olhar_o_contexto()
    {
        var etapa = Etapas.Verificar("mensagem clara",
            ctx => Task.FromResult(ctx.Mudanca.Length >= 10), "descreva melhor a mudança");

        var curta = new ContextoDeEntrega("fix");
        await new Pipeline().Usar(etapa).ExecutarAsync(curta);
        Assert.True(curta.Interrompido);

        var boa = new ContextoDeEntrega("corrige arredondamento do frete");
        await new Pipeline().Usar(etapa).ExecutarAsync(boa);
        Assert.False(boa.Interrompido);
    }
}
