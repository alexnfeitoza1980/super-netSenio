using Jornada.Resiliencia.Cap93;

namespace Resiliencia.Testes;

public class Cap93Tests
{
    private static MapaDeApoio MapaDaBia() => new MapaDeApoio()
        .Adicionar(new Pessoa("Marta", Necessidade.Ouvir, Necessidade.TarefaPratica))
        .Adicionar(new Pessoa("Beto", Necessidade.Estudar))
        .Adicionar(new Profissional("orientadora Ana", Necessidade.Orientacao));

    [Fact]
    public void Cada_tipo_de_apoio_pede_do_seu_jeito()
    {
        Assert.StartsWith("Oi, Marta", new Pessoa("Marta", Necessidade.Ouvir).ComoPedir(Necessidade.Ouvir));
        Assert.StartsWith("Olá.", new Profissional("Dra. Lia", Necessidade.Orientacao).ComoPedir(Necessidade.Orientacao));
        Assert.Contains("ligue 188", new Servico("CVV", "188", Necessidade.Ouvir).ComoPedir(Necessidade.Ouvir));
    }

    [Fact]
    public void Quem_filtra_pelas_necessidades_atendidas()
    {
        var quem = MapaDaBia().Quem(Necessidade.Ouvir);
        Assert.Equal("Marta", Assert.Single(quem).Nome);
    }

    [Fact]
    public void Em_urgencia_os_servicos_publicos_sempre_aparecem()
    {
        var vazio = new MapaDeApoio();
        var quem = vazio.Quem(Necessidade.Urgencia);

        Assert.Contains(quem, a => a.Nome.StartsWith("CVV"));
        Assert.Contains(quem, a => a.Nome.StartsWith("SAMU"));
    }

    [Fact]
    public void Lacunas_mostram_o_que_ninguem_cobre_ainda()
    {
        var mapa = new MapaDeApoio().Adicionar(new Pessoa("Marta", Necessidade.Ouvir));
        Assert.Equal(new[] { Necessidade.Estudar, Necessidade.TarefaPratica, Necessidade.Orientacao }, mapa.Lacunas());
    }

    [Fact]
    public void Pedido_completo_tem_comeco_meio_e_fim()
    {
        var pedido = new Pedido("Beto", "você poderia estudar comigo", "amanhã às 19h por 30 minutos",
                                "Se não der, tudo bem, procuro outro horário.");

        Assert.Equal(
            "Beto, você poderia estudar comigo amanhã às 19h por 30 minutos. Se não der, tudo bem, procuro outro horário.",
            pedido.Montar());
    }

    [Fact]
    public void Pedido_vago_e_recusado_com_o_nome_do_campo()
    {
        var e = Assert.Throws<ArgumentException>(() => new Pedido("Beto", "ajuda", "", "ok").Montar());
        Assert.Contains("Quando", e.Message);
    }
}
