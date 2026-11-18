using Jornada.Joao.Cap124;
using Jornada.Joao.Cap132;

namespace Joao.Testes;

public class Cap132Tests
{
    private static readonly Tarefa Lavar = new(1, "Lavar os pés dos discípulos", false);

    [Fact]
    public async Task Quem_usa_nao_sabe_se_fala_com_o_servico_ou_com_uma_pilha_de_decoradores()
    {
        IRepositorioDeTarefas simples = new RepositorioEmMemoria();
        IRepositorioDeTarefas empilhado = new RepositorioEmMemoria().ComCache().ComTentativas(3).ComRegistro(LanternaApagada.Instancia);

        foreach (var repositorio in new[] { simples, empilhado })
        {
            await repositorio.SalvarAsync(Lavar);
            Assert.Equal(Lavar, await repositorio.ObterAsync(1));
            Assert.Null(await repositorio.ObterAsync(99));
        }
    }

    [Fact]
    public async Task Com_tentativas_uma_falha_passageira_e_superada_sem_que_ninguem_perceba()
    {
        var chao = new RepositorioEmMemoria();
        await chao.SalvarAsync(Lavar);
        chao.FalharProximas(2);

        var repositorio = chao.ComTentativas(3);

        Assert.Equal(Lavar, await repositorio.ObterAsync(1));
        Assert.Equal(3, chao.Leituras);                     // duas falhas + uma que deu certo
    }

    [Fact]
    public async Task Esgotadas_as_tentativas_a_falha_aparece()
    {
        var chao = new RepositorioEmMemoria();
        chao.FalharProximas(5);

        await Assert.ThrowsAsync<FalhaPassageiraException>(() => chao.ComTentativas(3).ObterAsync(1));
        Assert.Equal(3, chao.Leituras);
    }

    [Fact]
    public async Task Com_cache_a_segunda_leitura_nem_chega_ao_servico()
    {
        var chao = new RepositorioEmMemoria();
        await chao.SalvarAsync(Lavar);
        var repositorio = chao.ComCache();

        await repositorio.ObterAsync(1);
        await repositorio.ObterAsync(1);

        Assert.Equal(1, chao.Leituras);
    }

    [Fact]
    public async Task Salvar_atualiza_o_cache_para_nunca_servir_dado_velho()
    {
        var repositorio = new RepositorioEmMemoria().ComCache();
        await repositorio.SalvarAsync(Lavar);
        await repositorio.ObterAsync(1);

        await repositorio.SalvarAsync(Lavar with { Feita = true });

        Assert.True((await repositorio.ObterAsync(1))!.Feita);
    }

    [Fact]
    public async Task A_ordem_dos_decoradores_muda_o_comportamento()
    {
        // Registro POR FORA do cache: registra todas as leituras. Registro POR DENTRO: só as que chegam ao serviço.
        var porFora = new List<Registro>();
        var porDentro = new List<Registro>();

        var a = new RepositorioEmMemoria().ComCache().ComRegistro(new Lanterna(Nivel.Detalhe, porFora.Add));
        var b = new RepositorioEmMemoria().ComRegistro(new Lanterna(Nivel.Detalhe, porDentro.Add)).ComCache();
        foreach (var r in new[] { a, b }) await r.SalvarAsync(Lavar);
        porFora.Clear(); porDentro.Clear();

        for (int i = 0; i < 3; i++) { await a.ObterAsync(1); await b.ObterAsync(1); }

        Assert.Equal(3, porFora.Count);                     // viu os três pedidos
        Assert.Empty(porDentro);                            // o cache respondeu antes de chegar lá
    }

    [Fact]
    public async Task O_registro_mostra_escopo_e_erro_quando_o_servico_falha()
    {
        var linhas = new List<Registro>();
        var chao = new RepositorioEmMemoria();
        chao.FalharProximas(1);
        var repositorio = chao.ComRegistro(new Lanterna(Nivel.Detalhe, linhas.Add));

        await Assert.ThrowsAsync<FalhaPassageiraException>(() => repositorio.ObterAsync(7));

        var erro = Assert.Single(linhas);
        Assert.Equal(Nivel.Erro, erro.Nivel);
        Assert.Equal(new[] { "obter 7" }, erro.Escopos);
        Assert.Equal("o serviço está ocupado", erro.Campos["erro"]);
    }
}
