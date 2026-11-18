using Jornada.Habitos.Cap105;
using Jornada.Habitos.Cap106;
using Jornada.Habitos.Cap107;
using Jornada.Habitos.Cap108;
using Jornada.Habitos.Cap109;
using Jornada.Habitos.Cap110;
using Jornada.Habitos.Cap111;
using Jornada.Habitos.Cap112;
using Jornada.Habitos.Cap113;
using Jornada.Habitos.Cap114;
using Jornada.Habitos.Cap115;

namespace Habitos.Testes;

public class ExerciciosTests
{
    [Fact]
    public void Cap105_Resumo_marca_o_que_o_plano_cobre()
    {
        var plano = new PlanoDeAprendizado().Adicionar("Ler", Dimensao.Cabeca).Adicionar("Praticar", Dimensao.Maos);
        Assert.Equal("Cabeça ✓ · Coração ✗ · Mãos ✓", plano.Resumo());
        Assert.Equal("Cabeça ✗ · Coração ✗ · Mãos ✗", new PlanoDeAprendizado().Resumo());
    }

    [Fact]
    public void Cap106_As_mais_alinhadas_vem_primeiro()
    {
        var bussola = new Bussola(new Vetor(1, 1));
        var itens = new[]
        {
            new Iniciativa("Só tela nova", new Vetor(1, -0.5)),
            new Iniciativa("Refatorar", new Vetor(0.5, 1)),
            new Iniciativa("Testar e entregar", new Vetor(1, 1)),
        };

        var melhores = bussola.MaisAlinhadas(itens, 2);

        Assert.Equal(new[] { "Testar e entregar", "Refatorar" }, melhores.Select(i => i.Nome));
    }

    [Theory]
    [InlineData(1.0, 5)]
    [InlineData(0.8, 1)]
    public void Cap107_Dias_ate_deixar_de_ser_automatico(double forca, int esperado)
    {
        var habito = new Habito("x", forca, new DateOnly(2026, 11, 1));
        int dias = habito.DiasAteDeixarDeSerAutomatico()!.Value;

        Assert.Equal(esperado, dias);

        var dia = new DateOnly(2026, 11, 1);
        Assert.True(Formacao.ForcaEm(habito, dia.AddDays(dias)) < Habito.Limiar);
        if (dias > 1)
            Assert.True(Formacao.ForcaEm(habito, dia.AddDays(dias - 1)) >= Habito.Limiar);
    }

    [Fact]
    public void Cap107_Habito_que_nao_e_automatico_devolve_null() =>
        Assert.Null(new Habito("x", 0.5).DiasAteDeixarDeSerAutomatico());

    [Fact]
    public void Cap108_Maior_descompasso_ou_null_se_tudo_caminha_junto()
    {
        var dia = new DateOnly(2026, 11, 2);
        var declaradas = new[] { new Prioridade("Família", 1), new Prioridade("Trabalho", 2) };

        var descompasso = Revelacao.Comparar(declaradas, new[] { new Registro(dia, "Trabalho", 500), new Registro(dia, "Família", 50) });
        Assert.Equal("Família", descompasso.MaiorDescompasso()!.Area);

        var alinhado = Revelacao.Comparar(declaradas, new[] { new Registro(dia, "Família", 500), new Registro(dia, "Trabalho", 50) });
        Assert.Null(alinhado.MaiorDescompasso());
    }

    [Fact]
    public void Cap109_Descrever_lista_os_padroes_ligados()
    {
        Assert.Equal("RolagemInfinita + NotificacaoPush", (PadraoDeDesign.RolagemInfinita | PadraoDeDesign.NotificacaoPush).Descrever());
        Assert.Equal("nenhum", PadraoDeDesign.Nenhum.Descrever());
    }

    [Fact]
    public void Cap110_Uma_pausa_atrasa_o_cruzamento_pelo_mesmo_numero_de_dias()
    {
        var antigo = Curvas.Declinio(0.05);
        var semPausa = new Reabituacao(antigo, Curvas.Crescimento(0.05));
        var comPausa = new Reabituacao(antigo, Curvas.Crescimento(0.05).ComPausa(5, 7));

        var parado = Curvas.Crescimento(0.05).ComPausa(5, 7);
        Assert.Equal(parado(5), parado(11), 9);                 // durante a pausa, nada muda

        Assert.True(comPausa.DiaDoCruzamento(200) > semPausa.DiaDoCruzamento(200));
    }

    [Fact]
    public void Cap111_Quem_se_desinscreve_deixa_de_receber_avisos()
    {
        var ritual = new RitualDeReuniao();
        var contador = new ContadorDeMovimentos(ritual);

        ritual.Concluir("a");
        Assert.Equal(1, contador.Total);

        contador.Dispose();
        ritual.Concluir("b");
        Assert.Equal(1, contador.Total);
    }

    [Fact]
    public void Cap112_Proxima_ocorrencia_olha_uma_semana_a_frente()
    {
        var calendario = new CalendarioDoTime().Adicionar(new Pratica("Demo", Intencao.Entregar, 45, new[] { DayOfWeek.Friday }));
        var segunda = new DateOnly(2026, 11, 2);

        Assert.Equal(new DateOnly(2026, 11, 6), calendario.ProximaOcorrencia("Demo", segunda)!.Dia);
        Assert.Equal(new DateOnly(2026, 11, 13), calendario.ProximaOcorrencia("Demo", new DateOnly(2026, 11, 6))!.Dia);
        Assert.Null(calendario.ProximaOcorrencia("Retro", segunda));
    }

    [Fact]
    public void Cap113_Proxima_revisao_e_dominados()
    {
        var caixa = new CaixaDeLeitner();
        Assert.Null(caixa.ProximaRevisao());

        var d = new DateOnly(2026, 11, 1);
        caixa.Adicionar("a", "A", d);
        caixa.Adicionar("b", "B", d);
        for (int i = 0; i < 5; i++) caixa.Responder("a", true, d.AddDays(i * 20));

        Assert.Equal(d, caixa.ProximaRevisao());                 // o cartão "b" ainda vence em d
        Assert.Equal("a", caixa.Dominados().Single().Frente);
    }

    [Fact]
    public void Cap114_Faixa_nao_existe_ao_contrario_e_sabe_se_sobrepor()
    {
        Assert.Throws<ArgumentException>(() => new Faixa(50, 25));

        var foco = new Faixa(25, 50);
        Assert.True(foco.Contem(25) && foco.Contem(50) && !foco.Contem(51));
        Assert.True(foco.Sobrepoe(new Faixa(50, 90)));
        Assert.False(foco.Sobrepoe(new Faixa(51, 90)));
        Assert.Equal("25–50", foco.ToString());
    }

    [Fact]
    public async Task Cap115_Etapa_condicional_pula_sem_interromper()
    {
        var ctx = new ContextoDeEntrega("x");
        var pipeline = new Pipeline()
            .Usar(EtapasCondicionais.Quando(c => false, Etapas.Verificar("só em release", _ => Task.FromResult(true), "x")))
            .Usar(Etapas.Verificar("publicar", _ => Task.FromResult(true), "x"));

        await pipeline.ExecutarAsync(ctx);

        Assert.Equal(new[] { "✓ publicar" }, ctx.Registro);
        Assert.False(ctx.Interrompido);
    }
}
