using Jornada.Resiliencia.Cap96;

namespace Resiliencia.Testes;

public class Cap96Tests
{
    private static readonly PoliticaDeTentativas Politica = new(MaximoDeTentativas: 4, EsperaInicial: TimeSpan.FromSeconds(1));

    /// <summary>Espera falsa: registra quanto seria esperado, sem esperar de verdade.</summary>
    private sealed class EsperaFalsa
    {
        public List<TimeSpan> Esperas { get; } = new();
        public Task Esperar(TimeSpan tempo, CancellationToken ct) { Esperas.Add(tempo); return Task.CompletedTask; }
    }

    [Fact]
    public void Espera_cresce_a_cada_falha()
    {
        Assert.Equal(TimeSpan.FromSeconds(1), Politica.EsperaApos(1));
        Assert.Equal(TimeSpan.FromSeconds(2), Politica.EsperaApos(2));
        Assert.Equal(TimeSpan.FromSeconds(4), Politica.EsperaApos(3));
    }

    [Fact]
    public async Task Tenta_de_novo_ate_funcionar()
    {
        var servico = new ServicoInstavel(falhasAntesDeFuncionar: 2);
        var espera = new EsperaFalsa();

        var frase = await Tentador.ExecutarAsync(servico.BuscarFraseAsync, Politica, espera.Esperar);

        Assert.Equal("Um passo de cada vez.", frase);
        Assert.Equal(3, servico.Chamadas);
        Assert.Equal(new[] { TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(2) }, espera.Esperas);
    }

    [Fact]
    public async Task Desiste_quando_as_tentativas_acabam_e_guarda_a_causa()
    {
        var servico = new ServicoInstavel(falhasAntesDeFuncionar: 99);

        var e = await Assert.ThrowsAsync<TentativasEsgotadasException>(() =>
            Tentador.ExecutarAsync(servico.BuscarFraseAsync, Politica, new EsperaFalsa().Esperar));

        Assert.Equal(4, e.Tentativas);
        Assert.IsType<FalhaPassageiraException>(e.InnerException);
    }

    [Fact]
    public async Task Erro_de_programacao_nao_e_repetido()
    {
        var espera = new EsperaFalsa();
        int chamadas = 0;

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            Tentador.ExecutarAsync<string>(_ => { chamadas++; throw new InvalidOperationException("bug"); },
                                           Politica, espera.Esperar));

        Assert.Equal(1, chamadas);
        Assert.Empty(espera.Esperas);
    }

    [Fact]
    public async Task Plano_B_entrega_algo_simples_quando_tudo_falha()
    {
        var servico = new ServicoInstavel(99);

        var frase = await Tentador.ComPlanoBAsync(
            () => Tentador.ExecutarAsync(servico.BuscarFraseAsync, Politica, new EsperaFalsa().Esperar),
            planoB: () => "Respire fundo e tente de novo mais tarde.");

        Assert.Equal("Respire fundo e tente de novo mais tarde.", frase);
    }

    [Fact]
    public async Task Limite_de_tempo_vira_timeout()
    {
        await Assert.ThrowsAsync<TimeoutException>(() =>
            Tentador.ComLimiteAsync<int>(async ct => { await Task.Delay(TimeSpan.FromSeconds(5), ct); return 1; },
                                         TimeSpan.FromMilliseconds(30)));
    }

    [Fact]
    public async Task Cancelamento_de_quem_chamou_continua_sendo_cancelamento()
    {
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(() =>
            Tentador.ComLimiteAsync<int>(async ct => { await Task.Delay(TimeSpan.FromSeconds(5), ct); return 1; },
                                         TimeSpan.FromSeconds(10), cts.Token));
    }

    // ---------- disjuntor ----------

    private static Task<string> Falha(CancellationToken _) => throw new FalhaPassageiraException("fora do ar");
    private static Task<string> Funciona(CancellationToken _) => Task.FromResult("ok");

    [Fact]
    public async Task Disjuntor_abre_depois_de_falhas_seguidas_e_nao_chama_mais_o_servico()
    {
        var relogio = new RelogioDeTeste();
        var d = new DisjuntorDeCircuito(limiteDeFalhas: 3, descanso: TimeSpan.FromMinutes(1), relogio);

        for (int i = 0; i < 3; i++)
            await Assert.ThrowsAsync<FalhaPassageiraException>(() => d.ExecutarAsync(Falha));

        Assert.Equal(EstadoDoDisjuntor.Aberto, d.Estado);

        int chamadas = 0;
        await Assert.ThrowsAsync<CircuitoAbertoException>(() =>
            d.ExecutarAsync(_ => { chamadas++; return Funciona(_); }));
        Assert.Equal(0, chamadas);
    }

    [Fact]
    public async Task Depois_do_descanso_uma_chamada_de_teste_fecha_o_circuito()
    {
        var relogio = new RelogioDeTeste();
        var d = new DisjuntorDeCircuito(2, TimeSpan.FromMinutes(1), relogio);
        for (int i = 0; i < 2; i++)
            await Assert.ThrowsAsync<FalhaPassageiraException>(() => d.ExecutarAsync(Falha));

        relogio.Avancar(TimeSpan.FromMinutes(1));
        Assert.Equal(EstadoDoDisjuntor.MeioAberto, d.Estado);

        Assert.Equal("ok", await d.ExecutarAsync(Funciona));
        Assert.Equal(EstadoDoDisjuntor.Fechado, d.Estado);
    }

    [Fact]
    public async Task Se_o_teste_falha_o_circuito_reabre_e_o_descanso_recomeca()
    {
        var relogio = new RelogioDeTeste();
        var d = new DisjuntorDeCircuito(2, TimeSpan.FromMinutes(1), relogio);
        for (int i = 0; i < 2; i++)
            await Assert.ThrowsAsync<FalhaPassageiraException>(() => d.ExecutarAsync(Falha));

        relogio.Avancar(TimeSpan.FromMinutes(1));
        await Assert.ThrowsAsync<FalhaPassageiraException>(() => d.ExecutarAsync(Falha));

        Assert.Equal(EstadoDoDisjuntor.Aberto, d.Estado);
        relogio.Avancar(TimeSpan.FromSeconds(30));
        Assert.Equal(EstadoDoDisjuntor.Aberto, d.Estado);
    }

    [Fact]
    public async Task Um_sucesso_no_meio_zera_a_contagem_de_falhas()
    {
        var d = new DisjuntorDeCircuito(3, TimeSpan.FromMinutes(1), new RelogioDeTeste());

        await Assert.ThrowsAsync<FalhaPassageiraException>(() => d.ExecutarAsync(Falha));
        await Assert.ThrowsAsync<FalhaPassageiraException>(() => d.ExecutarAsync(Falha));
        await d.ExecutarAsync(Funciona);
        await Assert.ThrowsAsync<FalhaPassageiraException>(() => d.ExecutarAsync(Falha));

        Assert.Equal(EstadoDoDisjuntor.Fechado, d.Estado);
    }
}
