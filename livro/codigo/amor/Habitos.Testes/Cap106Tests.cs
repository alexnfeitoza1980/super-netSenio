using Jornada.Habitos.Cap106;

namespace Habitos.Testes;

public class Cap106Tests
{
    [Fact]
    public void Vetores_somam_e_multiplicam_por_numero()
    {
        var soma = new Vetor(1, 2) + new Vetor(3, -1);
        Assert.Equal(new Vetor(4, 1), soma);
        Assert.Equal(new Vetor(2, 4), 2 * new Vetor(1, 2));
    }

    [Fact]
    public void Struct_se_compara_pelo_valor_e_copia_nao_altera_o_original()
    {
        var a = new Vetor(1, 1);
        var b = a;                              // cópia
        b = b with { X = 9 };

        Assert.Equal(new Vetor(1, 1), a);
        Assert.NotEqual(a, b);
        Assert.Equal(new Vetor(1, 1), new Vetor(1, 1));
    }

    [Theory]
    [InlineData(1, 0, 1, 0, 1)]
    [InlineData(1, 0, 0, 1, 0)]
    [InlineData(1, 0, -1, 0, -1)]
    public void Cosseno_mede_o_alinhamento(double x1, double y1, double x2, double y2, double esperado) =>
        Assert.Equal(esperado, new Vetor(x1, y1).Cosseno(new Vetor(x2, y2)), 6);

    [Fact]
    public void Angulo_de_perpendiculares_e_noventa_graus() =>
        Assert.Equal(90, new Vetor(1, 0).AnguloEmGraus(new Vetor(0, 1)), 6);

    [Fact]
    public void Rumo_nulo_e_recusado() =>
        Assert.Throws<ArgumentException>(() => new Bussola(Vetor.Zero));

    private static readonly Bussola Rumo = new(new Vetor(1, 1));       // valor E saúde, juntos

    [Fact]
    public void Iniciativas_que_apontam_para_o_rumo_tem_alinhamento_alto()
    {
        Assert.True(Rumo.Alinhamento(new Iniciativa("Refatorar o que mais dói", new Vetor(1, 1))) > 0.99);
        Assert.True(Rumo.Alinhamento(new Iniciativa("Só novas telas", new Vetor(1, -0.2))) < 0.7);
    }

    [Fact]
    public void Varias_pequenas_entregas_so_de_valor_levam_a_um_desvio_acumulado()
    {
        var feitas = Enumerable.Range(1, 6).Select(i => new Iniciativa($"Funcionalidade {i}", new Vetor(1, -0.3))).ToList();

        double graus = Rumo.DesvioEmGraus(feitas)!.Value;

        Assert.InRange(graus, 55, 70);
        Assert.Equal(Leitura.Desviando, Bussola.Ler(graus));
    }

    [Fact]
    public void Misturar_valor_e_saude_mantem_o_rumo()
    {
        var feitas = new[]
        {
            new Iniciativa("Funcionalidade", new Vetor(1, -0.3)),
            new Iniciativa("Pagar dívida técnica", new Vetor(0.2, 1)),
            new Iniciativa("Funcionalidade", new Vetor(1, -0.3)),
            new Iniciativa("Testes e documentação", new Vetor(0, 1)),
        };

        Assert.Equal(Leitura.NoRumo, Bussola.Ler(Rumo.DesvioEmGraus(feitas)!.Value));
    }

    [Fact]
    public void Sem_nada_feito_nao_ha_desvio_para_medir() =>
        Assert.Null(Rumo.DesvioEmGraus(Array.Empty<Iniciativa>()));

    [Fact]
    public void Recalibrar_devolve_outra_bussola_e_a_primeira_continua_igual()
    {
        var nova = Rumo.Recalibrar(new Vetor(0, 1));
        Assert.Equal(new Vetor(1, 1), Rumo.Telos);
        Assert.Equal(new Vetor(0, 1), nova.Telos);
    }

    [Theory]
    [InlineData(10, Leitura.NoRumo)]
    [InlineData(40, Leitura.Desviando)]
    [InlineData(120, Leitura.RumoErrado)]
    public void Leitura_por_faixas(double graus, Leitura esperado) => Assert.Equal(esperado, Bussola.Ler(graus));
}
