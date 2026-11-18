using Jornada.Resiliencia.Cap89;

namespace Resiliencia.Testes;

public class RelogioDeTeste : TimeProvider
{
    private DateTimeOffset agora = new(2026, 11, 2, 9, 0, 0, TimeSpan.Zero);
    public override DateTimeOffset GetUtcNow() => agora;
    public void Avancar(TimeSpan tempo) => agora += tempo;
}

public class Cap89Tests
{
    [Fact]
    public void Intensidade_fora_da_escala_e_recusada() =>
        Assert.Throws<ArgumentOutOfRangeException>(() => new Sentimento(Emocao.Raiva, 11));

    [Theory]
    [InlineData(Emocao.Raiva, 2, "irritação")]
    [InlineData(Emocao.Raiva, 9, "raiva muito forte")]
    [InlineData(Emocao.Medo, 5, "medo")]
    [InlineData(Emocao.Alegria, 8, "alegria")]
    [InlineData(Emocao.Cansaco, 9, "exaustão")]
    public void Nomear_usa_palavras_mais_precisas(Emocao e, int i, string esperado) =>
        Assert.Equal(esperado, Vocabulario.Nomear(new Sentimento(e, i)));

    [Theory]
    [InlineData(2, 0)]
    [InlineData(5, 2)]
    [InlineData(9, 10)]
    public void Pausa_cresce_com_a_intensidade(int intensidade, int minutos) =>
        Assert.Equal(TimeSpan.FromMinutes(minutos), SugestaoDePausa.Para(new Sentimento(Emocao.Raiva, intensidade)));

    [Fact]
    public void Pausa_termina_quando_o_tempo_passa()
    {
        var relogio = new RelogioDeTeste();
        var pausa = new PausaConsciente(relogio, TimeSpan.FromMinutes(2));

        Assert.False(pausa.Terminou);          // ainda nem começou
        pausa.Iniciar();
        relogio.Avancar(TimeSpan.FromSeconds(90));
        Assert.False(pausa.Terminou);
        Assert.Equal(TimeSpan.FromSeconds(30), pausa.Restante);

        relogio.Avancar(TimeSpan.FromSeconds(30));
        Assert.True(pausa.Terminou);
    }

    [Fact]
    public void Diario_conta_sem_julgar_e_mostra_a_linha_do_tempo()
    {
        var diario = new DiarioDeEmocoes();
        diario.Registrar(new DateOnly(2026, 11, 3), new Sentimento(Emocao.Medo, 4));
        diario.Registrar(new DateOnly(2026, 11, 2), new Sentimento(Emocao.Raiva, 2));
        diario.Registrar(new DateOnly(2026, 11, 4), new Sentimento(Emocao.Medo, 6));

        Assert.Equal(2, diario.Contagem()[Emocao.Medo]);
        Assert.Equal("02/11: irritação (2/10)", diario.Linha()[0]);
    }
}
