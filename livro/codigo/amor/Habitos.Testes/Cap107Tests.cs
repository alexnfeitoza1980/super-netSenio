using Jornada.Habitos.Cap107;

namespace Habitos.Testes;

public class Cap107Tests
{
    private static DateOnly D(int dia) => new DateOnly(2026, 11, 1).AddDays(dia - 1);

    [Fact]
    public void Praticar_devolve_um_habito_novo_e_o_original_continua_igual()
    {
        var antes = new Habito("Escrever teste primeiro");
        var depois = Formacao.Praticar(antes, D(1));

        Assert.Equal(0, antes.Forca);
        Assert.Null(antes.UltimaPratica);
        Assert.Equal(0.12, depois.Forca, 6);
        Assert.Equal(D(1), depois.UltimaPratica);
    }

    [Fact]
    public void Cada_repeticao_soma_menos_que_a_anterior()
    {
        var h0 = new Habito("x");
        var h1 = Formacao.Praticar(h0, D(1));
        var h2 = Formacao.Praticar(h1, D(2));
        var h3 = Formacao.Praticar(h2, D(3));

        Assert.True(h2.Forca - h1.Forca < h1.Forca - h0.Forca);
        Assert.True(h3.Forca - h2.Forca < h2.Forca - h1.Forca);
    }

    [Fact]
    public void Sem_praticar_a_forca_cai_pela_metade_a_cada_catorze_dias()
    {
        var h = new Habito("x", 0.8, D(1));
        Assert.Equal(0.4, Formacao.ForcaEm(h, D(15)), 6);
        Assert.Equal(0.2, Formacao.ForcaEm(h, D(29)), 6);
        Assert.Equal(0.8, Formacao.ForcaEm(h, D(1)), 6);
    }

    [Fact]
    public void Com_mentor_o_habito_se_forma_mais_depressa()
    {
        var h = new Habito("revisar o próprio código");

        var sozinho = Formacao.DiasAteAutomatico(h, D(1));
        var acompanhado = Formacao.DiasAteAutomatico(h, D(1), comMentor: true);

        Assert.NotNull(sozinho);
        Assert.NotNull(acompanhado);
        Assert.True(acompanhado < sozinho);
    }

    [Fact]
    public void Praticar_todo_dia_chega_ao_automatico_e_parar_desfaz_aos_poucos()
    {
        var h = Formacao.PraticarVarias(new Habito("x"), Enumerable.Range(1, 14).Select(D));
        Assert.True(h.Automatico);

        Assert.True(Formacao.ForcaEm(h, D(14 + 14)) < Habito.Limiar);
    }

    [Fact]
    public void Praticar_de_vez_em_quando_nao_chega_ao_automatico()
    {
        var aCadaDezDias = new[] { 1, 11, 21, 31, 41, 51 }.Select(D);
        Assert.False(Formacao.PraticarVarias(new Habito("x"), aCadaDezDias).Automatico);
    }

    [Fact]
    public void Ordem_dos_dias_informados_nao_importa()
    {
        var a = Formacao.PraticarVarias(new Habito("x"), new[] { D(3), D(1), D(2) });
        var b = Formacao.PraticarVarias(new Habito("x"), new[] { D(1), D(2), D(3) });
        Assert.Equal(a, b);
    }

    [Fact]
    public void Limite_de_dias_curto_demais_devolve_null() =>
        Assert.Null(Formacao.DiasAteAutomatico(new Habito("x"), D(1), limiteDeDias: 3));
}
