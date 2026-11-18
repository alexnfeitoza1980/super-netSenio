using Jornada.Habitos.Cap113;

namespace Habitos.Testes;

public class Cap113Tests
{
    private static DateOnly D(int dia) => new DateOnly(2026, 11, 1).AddDays(dia - 1);

    private static CaixaDeLeitner Caixa()
    {
        var c = new CaixaDeLeitner();
        c.Adicionar("O que faz o switch expression?", "Devolve um valor para cada caso", D(1));
        c.Adicionar("Para que serve o yield return?", "Entrega itens um a um", D(1));
        c.Adicionar("O que é um record?", "Tipo para dados imutáveis", D(1));
        return c;
    }

    [Fact]
    public void Cartao_novo_comeca_na_caixa_um_e_vence_no_mesmo_dia()
    {
        var c = Caixa();
        Assert.All(c.Todos, x => Assert.Equal(1, x.Caixa));
        Assert.Equal(3, c.Devidos(D(1)).Count);
    }

    [Fact]
    public void Acertar_sobe_de_caixa_e_adia_a_proxima_revisao()
    {
        var c = Caixa();
        c.Responder("O que é um record?", acertou: true, D(1));

        var cartao = c.Todos.Single(x => x.Frente == "O que é um record?");
        Assert.Equal(2, cartao.Caixa);
        Assert.Equal(D(3), cartao.Proxima);                       // caixa 2 → 2 dias
        Assert.Equal(2, c.Devidos(D(2)).Count);                   // os outros dois ainda vencem
    }

    [Fact]
    public void Errar_volta_para_a_caixa_um_e_revisa_no_dia_seguinte()
    {
        var c = Caixa();
        c.Responder("O que é um record?", true, D(1));
        c.Responder("O que é um record?", true, D(3));
        c.Responder("O que é um record?", false, D(7));

        var cartao = c.Todos.Single(x => x.Frente == "O que é um record?");
        Assert.Equal(1, cartao.Caixa);
        Assert.Equal(D(8), cartao.Proxima);
    }

    [Fact]
    public void Quem_acerta_sempre_chega_a_ultima_caixa_e_la_fica()
    {
        var c = Caixa();
        var dia = 1;
        for (int i = 0; i < 8; i++)
        {
            c.Responder("O que é um record?", true, D(dia));
            dia += 20;
        }

        Assert.Equal(5, c.Todos.Single(x => x.Frente == "O que é um record?").Caixa);
        Assert.Equal(16, CaixaDeLeitner.Intervalos[^1]);
    }

    [Fact]
    public void Devidos_mostram_primeiro_os_mais_frageis()
    {
        var c = Caixa();
        c.Responder("O que é um record?", true, D(1));                  // sobe para a caixa 2
        var ordem = c.Devidos(D(10)).Select(x => x.Caixa).ToList();

        Assert.Equal(new[] { 1, 1, 2 }, ordem);
    }

    [Fact]
    public void Mesmo_sorteio_mesma_sessao_e_e_so_uma_reordenacao()
    {
        var c = Caixa();

        var a = c.SessaoDoDia(D(1), new Random(7)).Select(x => x.Frente).ToList();
        var b = c.SessaoDoDia(D(1), new Random(7)).Select(x => x.Frente).ToList();

        Assert.Equal(a, b);
        Assert.Equal(c.Devidos(D(1)).Select(x => x.Frente).OrderBy(x => x), a.OrderBy(x => x));
    }

    [Fact]
    public void Contagem_por_caixa_inclui_as_caixas_vazias()
    {
        var c = Caixa();
        c.Responder("O que é um record?", true, D(1));

        var porCaixa = c.PorCaixa();

        Assert.Equal(2, porCaixa[1]);
        Assert.Equal(1, porCaixa[2]);
        Assert.Equal(0, porCaixa[5]);
        Assert.Equal(5, porCaixa.Count);
    }

    [Fact]
    public void Responder_um_cartao_que_nao_existe_e_um_erro_claro() =>
        Assert.Throws<KeyNotFoundException>(() => Caixa().Responder("inexistente", true, D(1)));
}
