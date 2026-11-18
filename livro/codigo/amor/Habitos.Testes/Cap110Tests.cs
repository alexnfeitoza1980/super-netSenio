using Jornada.Habitos.Cap110;

namespace Habitos.Testes;

public class Cap110Tests
{
    [Fact]
    public void Curvas_comecam_e_terminam_onde_esperado()
    {
        var novo = Curvas.Crescimento(0.1);
        var antigo = Curvas.Declinio(0.1);

        Assert.Equal(0, novo(0), 6);
        Assert.Equal(1, antigo(0), 6);
        Assert.True(novo(60) > 0.99);
        Assert.True(antigo(60) < 0.01);
    }

    [Fact]
    public void O_novo_ultrapassa_o_antigo_quando_as_curvas_se_cruzam()
    {
        var r = new Reabituacao(Curvas.Declinio(0.05), Curvas.Crescimento(0.05));

        int? dia = r.DiaDoCruzamento(200);

        Assert.NotNull(dia);
        var ponto = r.Simular(200).First(p => p.Dia == dia);
        Assert.True(ponto.Novo > ponto.Antigo);
        Assert.False(r.Simular(200).First(p => p.Dia == dia - 1).NovoJaPassou);
    }

    [Fact]
    public void Dias_de_desconforto_e_o_tempo_ate_o_cruzamento()
    {
        var r = new Reabituacao(Curvas.Declinio(0.05), Curvas.Crescimento(0.05));
        Assert.Equal(r.DiaDoCruzamento(200), r.DiasDeDesconforto(200));
    }

    [Fact]
    public void Quando_o_novo_cresce_devagar_demais_nao_cruza_no_prazo()
    {
        var r = new Reabituacao(Curvas.Declinio(0.001), Curvas.Crescimento(0.01));
        Assert.Null(r.DiaDoCruzamento(20));
    }

    [Fact]
    public void Com_parceria_o_cruzamento_chega_antes()
    {
        var sozinho = new Reabituacao(Curvas.Declinio(0.04), Curvas.Crescimento(0.04));
        var acompanhado = new Reabituacao(Curvas.ComParceria(Curvas.Declinio(0.04), 1.5), Curvas.ComParceria(Curvas.Crescimento(0.04), 1.5));

        Assert.True(acompanhado.DiaDoCruzamento(200) < sozinho.DiaDoCruzamento(200));
    }

    [Fact]
    public void Comparar_usa_zip_e_mostra_a_vantagem_dia_a_dia()
    {
        var sozinho = new Reabituacao(Curvas.Declinio(0.04), Curvas.Crescimento(0.04));
        var acompanhado = new Reabituacao(Curvas.Declinio(0.04), Curvas.ComParceria(Curvas.Crescimento(0.04), 2));

        var diferencas = Reabituacao.Comparar(sozinho, acompanhado, 30).ToList();

        Assert.Equal(31, diferencas.Count);
        Assert.Equal(0, diferencas[0].Diferenca, 6);
        Assert.True(diferencas[10].Diferenca > 0);
    }

    [Fact]
    public void Simular_e_preguicoso_e_so_calcula_o_que_o_chamador_consome()
    {
        int chamadas = 0;
        var r = new Reabituacao(d => { chamadas++; return 1; }, _ => 0);

        _ = r.Simular(1000).Take(3).ToList();

        Assert.Equal(3, chamadas);
    }
}
