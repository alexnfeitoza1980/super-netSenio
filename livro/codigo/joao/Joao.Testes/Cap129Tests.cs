using Jornada.Joao.Cap129;

namespace Joao.Testes;

public class Cap129Tests
{
    [Fact]
    public async Task Partir_entrega_pedacos_numerados_em_ordem_com_await_foreach()
    {
        var porcoes = new List<Porcao>();

        await foreach (var p in Distribuicao.PartirAsync(paes: 2, peixes: 1, pedacosPorAlimento: 2))
            porcoes.Add(p);

        Assert.Equal(6, porcoes.Count);
        Assert.Equal(Enumerable.Range(1, 6), porcoes.Select(p => p.Numero));
        Assert.Equal(new[] { "pão 1", "pão 1", "pão 2", "pão 2", "peixe 1", "peixe 1" }, porcoes.Select(p => p.Origem));
    }

    [Fact]
    public async Task Com_fartura_todos_se_saciam_e_o_resto_vira_sobras()
    {
        var fomes = new[] { 2, 2, 2, 2, 2 };                                        // 10 porções necessárias

        var resultado = await Distribuicao.DistribuirAsync(
            Distribuicao.PartirAsync(5, 2, 4), fomes, capacidadeDoCesto: 3);          // 28 porções existem

        Assert.Equal(fomes, resultado.ServidosPorPessoa);
        Assert.Equal(10, resultado.TotalServido);
        Assert.Equal(18, resultado.Sobras.Count);
    }

    [Fact]
    public async Task Nada_se_perde_servidos_mais_sobras_sao_sempre_o_total()
    {
        var resultado = await Distribuicao.DistribuirAsync(
            Distribuicao.PartirAsync(5, 2, 4), new[] { 3, 3, 3 }, capacidadeDoCesto: 2);

        Assert.Equal(28, resultado.TotalServido + resultado.Sobras.Count);

        var numeros = resultado.Sobras.Select(p => p.Numero).Distinct().Count();
        Assert.Equal(resultado.Sobras.Count, numeros);                              // nenhuma porção aparece duas vezes
    }

    [Fact]
    public async Task Na_escassez_servem_so_o_que_existe_e_ninguem_passa_do_que_precisa()
    {
        var fomes = new[] { 5, 5, 5 };                                              // 15 necessárias, 6 existem

        var resultado = await Distribuicao.DistribuirAsync(Distribuicao.PartirAsync(3, 0, 2), fomes, 2);

        Assert.Equal(6, resultado.TotalServido);
        Assert.Empty(resultado.Sobras);
        Assert.All(resultado.ServidosPorPessoa.Zip(fomes), par => Assert.True(par.First <= par.Second));
    }

    [Fact]
    public async Task Cesto_pequeno_nao_trava_nada_o_produtor_apenas_espera()
    {
        // capacidade 1: o produtor só consegue colocar uma porção por vez (contrapressão)
        var resultado = await Distribuicao.DistribuirAsync(Distribuicao.PartirAsync(2, 2, 5), new[] { 4, 4 }, capacidadeDoCesto: 1)
            .WaitAsync(TimeSpan.FromSeconds(10));

        Assert.Equal(8, resultado.TotalServido);
        Assert.Equal(12, resultado.Sobras.Count);
    }

    [Fact]
    public void Sobras_vao_para_cestos_de_tamanho_fixo()
    {
        var sobras = Enumerable.Range(1, 7).Select(n => new Porcao(n, "pão 1"));

        var cestos = Distribuicao.EmCestos(sobras, porCesto: 3);

        Assert.Equal(new[] { 3, 3, 1 }, cestos.Select(c => c.Count));
    }

    [Fact]
    public async Task Cancelar_interrompe_a_distribuicao()
    {
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(() =>
            Distribuicao.DistribuirAsync(Distribuicao.PartirAsync(5, 2, 4), new[] { 2 }, 3, cts.Token));
    }
}
