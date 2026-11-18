using Jornada.Habitos.Cap108;

namespace Habitos.Testes;

public class Cap108Tests
{
    private static readonly DateOnly Dia = new(2026, 11, 2);

    private static readonly Prioridade[] Declaradas =
    {
        new("Família", 1),
        new("Saúde", 2),
        new("Estudo", 3),
        new("Trabalho", 4),
    };

    private static Registro R(string area, int minutos) => new(Dia, area, minutos);

    [Fact]
    public void Compara_a_ordem_declarada_com_a_ordem_do_tempo_registrado()
    {
        var registros = new[] { R("Trabalho", 600), R("Estudo", 200), R("Família", 120), R("Saúde", 30), R("Trabalho", 50) };

        var relatorio = Revelacao.Comparar(Declaradas, registros);

        var familia = relatorio.Declaradas.Single(c => c.Area == "Família");
        Assert.Equal(1, familia.OrdemDeclarada);
        Assert.Equal(3, familia.OrdemObservada);
        Assert.Equal(2, familia.Diferenca);

        var trabalho = relatorio.Declaradas.Single(c => c.Area == "Trabalho");
        Assert.Equal(650, trabalho.Minutos);
        Assert.Equal(1, trabalho.OrdemObservada);
        Assert.Equal(1000, relatorio.TotalDeMinutos);
    }

    [Fact]
    public void Prioridade_sem_nenhum_registro_aparece_com_zero_minutos_em_vez_de_sumir()
    {
        var relatorio = Revelacao.Comparar(Declaradas, new[] { R("Trabalho", 100) });

        var saude = relatorio.Declaradas.Single(c => c.Area == "Saúde");
        Assert.Equal(0, saude.Minutos);
        Assert.Equal(0, saude.Parte);
        Assert.Equal(4, relatorio.Declaradas.Count);
    }

    [Fact]
    public void Areas_com_tempo_mas_sem_prioridade_declarada_sao_listadas_a_parte()
    {
        var registros = new[] { R("Trabalho", 300), R("Redes sociais", 200), R("Jogos", 100) };

        var relatorio = Revelacao.Comparar(Declaradas, registros);

        Assert.Equal(new[] { "Redes sociais", "Jogos" }, relatorio.NaoDeclaradas.Select(a => a.Area));
        Assert.Equal(200.0 / 600, relatorio.NaoDeclaradas[0].Parte, 6);
    }

    [Fact]
    public void Sem_registros_nao_ha_divisao_por_zero()
    {
        var relatorio = Revelacao.Comparar(Declaradas, Array.Empty<Registro>());
        Assert.Equal(0, relatorio.TotalDeMinutos);
        Assert.All(relatorio.Declaradas, c => Assert.Equal(0, c.Parte));
    }

    [Fact]
    public void A_descricao_pergunta_com_curiosidade_e_nao_acusa()
    {
        var descompasso = new Comparacao("Família", 1, 3, 120, 0.12);
        var alinhado = new Comparacao("Trabalho", 2, 2, 400, 0.4);
        var acima = new Comparacao("Estudo", 3, 1, 500, 0.5);

        Assert.Contains("O que está no caminho?", Revelacao.Descrever(descompasso));
        Assert.Contains("caminham juntos", Revelacao.Descrever(alinhado));
        Assert.Contains("escolha ou um acaso", Revelacao.Descrever(acima));

        foreach (var c in new[] { descompasso, alinhado, acima })
        {
            var texto = Revelacao.Descrever(c).ToLowerInvariant();
            Assert.DoesNotContain("culpa", texto);
            Assert.DoesNotContain("fracasso", texto);
        }
    }
}
