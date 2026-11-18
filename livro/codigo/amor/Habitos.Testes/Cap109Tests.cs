using Jornada.Habitos.Cap109;

namespace Habitos.Testes;

public class Cap109Tests
{
    private static readonly Fluxo Feed = new("Feed", PadraoDeDesign.RolagemInfinita | PadraoDeDesign.RecompensaVariavel | PadraoDeDesign.NotificacaoPush, true);

    [Fact]
    public void Cada_padrao_ligado_gera_uma_pergunta_de_cuidado()
    {
        var observacoes = AuditoriaDeFluxo.Examinar(Feed);

        Assert.Equal(3, observacoes.Count);
        Assert.Contains(observacoes, o => o.Padrao == PadraoDeDesign.RolagemInfinita && o.Pergunta.Contains("parada"));
        Assert.All(observacoes, o => Assert.EndsWith("?", o.Pergunta));
    }

    [Fact]
    public void Fluxo_sem_padroes_nao_tem_observacoes() =>
        Assert.Empty(AuditoriaDeFluxo.Examinar(new Fluxo("Formulário simples", PadraoDeDesign.Nenhum, true)));

    [Fact]
    public void Flags_se_combinam_com_ou_e_se_testam_com_HasFlag()
    {
        var p = PadraoDeDesign.SequenciaDiaria | PadraoDeDesign.ProvaSocial;

        Assert.True(p.HasFlag(PadraoDeDesign.SequenciaDiaria));
        Assert.False(p.HasFlag(PadraoDeDesign.NotificacaoPush));
        Assert.Equal(2, AuditoriaDeFluxo.Quantidade(p));
        Assert.Equal(7, AuditoriaDeFluxo.Quantidade((PadraoDeDesign)127));
    }

    [Theory]
    [InlineData(PadraoDeDesign.Nenhum, true, Gravidade.Leve)]
    [InlineData(PadraoDeDesign.RolagemInfinita, true, Gravidade.Leve)]
    [InlineData(PadraoDeDesign.RolagemInfinita | PadraoDeDesign.NotificacaoPush, true, Gravidade.Moderada)]
    [InlineData(PadraoDeDesign.RolagemInfinita, false, Gravidade.Alta)]
    public void Avaliacao_considera_quantidade_e_facilidade_de_sair(PadraoDeDesign padroes, bool sai, Gravidade esperada) =>
        Assert.Equal(esperada, AuditoriaDeFluxo.Avaliar(new Fluxo("x", padroes, sai)));

    [Fact]
    public void Muitos_padroes_juntos_sao_alta_mesmo_com_saida_facil()
    {
        var tudo = new Fluxo("x", (PadraoDeDesign)127, PermiteSairFacilmente: true);
        Assert.Equal(Gravidade.Alta, AuditoriaDeFluxo.Avaliar(tudo));
    }

    [Fact]
    public void Acrescentou_mostra_so_o_que_a_nova_versao_ligou()
    {
        var v1 = new Fluxo("v1", PadraoDeDesign.NotificacaoPush, true);
        var v2 = new Fluxo("v2", PadraoDeDesign.NotificacaoPush | PadraoDeDesign.SequenciaDiaria | PadraoDeDesign.EscassezFalsa, true);

        Assert.Equal(PadraoDeDesign.SequenciaDiaria | PadraoDeDesign.EscassezFalsa, AuditoriaDeFluxo.Acrescentou(v1, v2));
    }

    [Fact]
    public void O_padrao_de_notificacoes_cuida_de_quem_usa()
    {
        var prefs = new PreferenciasDeNotificacao { Produto = "App de estudos" };

        Assert.False(prefs.NotificacoesAtivas);
        Assert.Equal(3, prefs.MaximoPorDia);
        Assert.False(prefs.PodeNotificar(TimeSpan.FromHours(15), 0));      // desligado por padrão
    }

    [Theory]
    [InlineData(22, true)]
    [InlineData(3, true)]
    [InlineData(8, false)]
    [InlineData(15, false)]
    public void O_silencio_atravessa_a_meia_noite(int hora, bool esperado)
    {
        var prefs = new PreferenciasDeNotificacao { Produto = "x" };
        Assert.Equal(esperado, prefs.EstaNoSilencio(TimeSpan.FromHours(hora)));
    }

    [Fact]
    public void Ligadas_respeitam_silencio_e_limite_diario()
    {
        var prefs = new PreferenciasDeNotificacao { Produto = "x", NotificacoesAtivas = true };

        Assert.True(prefs.PodeNotificar(TimeSpan.FromHours(10), 2));
        Assert.False(prefs.PodeNotificar(TimeSpan.FromHours(10), 3));
        Assert.False(prefs.PodeNotificar(TimeSpan.FromHours(23), 0));
    }
}
