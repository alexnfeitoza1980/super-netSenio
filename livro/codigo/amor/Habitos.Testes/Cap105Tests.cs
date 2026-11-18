using Jornada.Habitos.Cap105;

namespace Habitos.Testes;

public class Cap105Tests
{
    [Fact]
    public void Cobertura_junta_as_dimensoes_de_todas_as_atividades()
    {
        var plano = new PlanoDeAprendizado()
            .Adicionar("Curso em vídeo", Dimensao.Cabeca)
            .Adicionar("Projeto pessoal", Dimensao.Maos | Dimensao.Coracao);

        Assert.Equal(Dimensao.Todas, plano.Cobertura());
        Assert.Equal(Dimensao.Nenhuma, plano.Faltando());
    }

    [Fact]
    public void Um_plano_so_de_informacao_deixa_coracao_e_maos_de_fora()
    {
        var plano = new PlanoDeAprendizado()
            .Adicionar("Ler o livro", Dimensao.Cabeca)
            .Adicionar("Assistir à palestra", Dimensao.Cabeca);

        Assert.True(plano.EnvolveSo(Dimensao.Cabeca));
        Assert.Equal(Dimensao.Coracao | Dimensao.Maos, plano.Faltando());
        Assert.Equal(2, plano.Sugestoes().Count());
    }

    [Fact]
    public void Plano_vazio_pede_as_tres_dimensoes()
    {
        var sugestoes = new PlanoDeAprendizado().Sugestoes().ToList();
        Assert.Equal(3, sugestoes.Count);
        Assert.Contains(sugestoes, s => s.Contains("prática"));
    }

    private static readonly Opcao[] Noite =
    {
        new("Estudar mais um tutorial", new Dictionary<string, double> { ["ser bom no que faço"] = 0.7, ["descansar"] = 0.1 }),
        new("Ver série", new Dictionary<string, double> { ["descansar"] = 0.9 }),
        new("Treinar um kata", new Dictionary<string, double> { ["ser bom no que faço"] = 0.8, ["descansar"] = 0.2 }),
    };

    [Fact]
    public void O_mesmo_conjunto_de_opcoes_da_resultados_diferentes_conforme_o_que_se_quer()
    {
        var quemQuerCrescer = new[] { new Desejo("ser bom no que faço", 0.9), new Desejo("descansar", 0.2) };
        var quemQuerDescansar = new[] { new Desejo("ser bom no que faço", 0.2), new Desejo("descansar", 0.9) };

        Assert.Equal("Treinar um kata", Decisor.Escolher(quemQuerCrescer, Noite));
        Assert.Equal("Ver série", Decisor.Escolher(quemQuerDescansar, Noite));
    }

    [Fact]
    public void O_ranking_vem_do_maior_para_o_menor_e_desempata_pelo_nome()
    {
        var iguais = new[]
        {
            new Opcao("B", new Dictionary<string, double> { ["x"] = 0.5 }),
            new Opcao("A", new Dictionary<string, double> { ["x"] = 0.5 }),
        };

        var ranking = Decisor.Ranquear(new[] { new Desejo("x", 1) }, iguais);

        Assert.Equal(new[] { "A", "B" }, ranking.Select(p => p.Opcao));
    }

    [Fact]
    public void Atrativo_ausente_vale_zero_e_sem_opcoes_nao_ha_escolha()
    {
        var ranking = Decisor.Ranquear(new[] { new Desejo("y", 1) }, new[] { new Opcao("A", new Dictionary<string, double>()) });
        Assert.Equal(0, ranking[0].Pontos);
        Assert.Throws<InvalidOperationException>(() => Decisor.Escolher(new[] { new Desejo("y", 1) }, Array.Empty<Opcao>()));
    }
}
