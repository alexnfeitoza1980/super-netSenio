using Jornada.Resiliencia.Cap94;

namespace Resiliencia.Testes;

public class Cap94Tests
{
    private static readonly Situacao Atraso = new("Entreguei o trabalho atrasado");

    [Fact]
    public void Cada_lente_gera_suas_perguntas_marcadas_com_o_nome()
    {
        var perguntas = Conversa.Gerar(Atraso, Lentes.Padrao.ToArray());

        Assert.Contains(perguntas, p => p.StartsWith("[Eu]"));
        Assert.Contains(perguntas, p => p.StartsWith("[Relações]"));
        Assert.Contains(perguntas, p => p.StartsWith("[Lugar]"));
        Assert.Contains(perguntas, p => p.StartsWith("[Momento]"));
    }

    [Fact]
    public void Uma_unica_lente_e_recusada()
    {
        var e = Assert.Throws<ArgumentException>(() => Conversa.Gerar(Atraso, Lentes.Padrao[0]));
        Assert.Contains("duas lentes", e.Message);
    }

    [Fact]
    public void Quando_e_onde_opcionais_mudam_as_perguntas()
    {
        var com = new Situacao("Perdi a reunião", Quando: "semana de provas", Onde: "casa");
        var lugar = Conversa.PorLente(com, Lentes.Padrao)["Lugar"].Single();
        var momento = Conversa.PorLente(com, Lentes.Padrao)["Momento"].Single();

        Assert.Contains("casa", lugar);
        Assert.Contains("semana de provas", momento);

        var sem = Conversa.PorLente(Atraso, Lentes.Padrao)["Lugar"].Single();
        Assert.DoesNotContain("casa", sem);
    }

    [Fact]
    public void Funciona_com_qualquer_tipo_porque_e_generico()
    {
        var lentes = new[]
        {
            new Lente<int>("Tamanho", n => new[] { $"{n} cabe em um passo?" }),
            new Lente<int>("Prazo", n => new[] { $"O que muda se forem {n * 2}?" }),
        };

        Assert.Equal(new[] { "[Tamanho] 3 cabe em um passo?", "[Prazo] O que muda se forem 6?" },
                     Conversa.Gerar(3, lentes));
    }

    [Fact]
    public void Nenhuma_pergunta_padrao_aponta_um_culpado()
    {
        var todas = Conversa.Gerar(Atraso, Lentes.Padrao.ToArray());
        Assert.DoesNotContain(todas, p => p.Contains("culpa", StringComparison.OrdinalIgnoreCase));
    }
}
