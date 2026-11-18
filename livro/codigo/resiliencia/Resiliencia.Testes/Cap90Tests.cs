using Jornada.Resiliencia.Cap90;

namespace Resiliencia.Testes;

public class Cap90Tests
{
    private static readonly Preocupacao[] Lista =
    {
        new("Começar o primeiro exercício", Controle.DependeDeMim),
        new("Tirar uma dúvida com a monitora", Controle.PossoInfluenciar),
        new("A prova ser difícil", Controle.NaoDepende),
        new("Organizar a mesa de estudo", Controle.DependeDeMim),
    };

    [Fact]
    public void Separar_cria_tres_zonas()
    {
        var zonas = Organizador.Separar(Lista);

        Assert.Equal(2, zonas[Controle.DependeDeMim].Count());
        Assert.Single(zonas[Controle.PossoInfluenciar]);
        Assert.Single(zonas[Controle.NaoDepende]);
    }

    [Fact]
    public void Foco_comeca_pelo_que_depende_de_mim()
    {
        Assert.Equal("Começar o primeiro exercício", Organizador.ProximoFoco(Lista)!.Texto);
    }

    [Fact]
    public void Sem_nada_que_dependa_de_mim_o_foco_vai_para_o_que_posso_influenciar()
    {
        var so = Lista.Where(p => p.Controle != Controle.DependeDeMim);
        Assert.Equal(Controle.PossoInfluenciar, Organizador.ProximoFoco(so)!.Controle);
    }

    [Fact]
    public void Se_so_ha_o_que_nao_depende_o_foco_e_nulo()
    {
        var so = new[] { new Preocupacao("O tempo amanhã", Controle.NaoDepende) };
        Assert.Null(Organizador.ProximoFoco(so));
    }

    [Fact]
    public void Contar_separa_fatos_de_historias()
    {
        var notas = new[]
        {
            new Anotacao("Entreguei o trabalho dois dias depois", NaturezaDaFrase.Fato),
            new Anotacao("Sou um fracasso", NaturezaDaFrase.Historia),
            new Anotacao("A professora não respondeu a mensagem", NaturezaDaFrase.Fato),
        };

        var (fatos, historias) = Organizador.Contar(notas);

        Assert.Equal(2, fatos);
        Assert.Equal(1, historias);
        Assert.Single(Organizador.PerguntasParaAsHistorias(notas));
    }

    [Fact]
    public void Mochila_aponta_lacunas_com_perguntas()
    {
        var mochila = new Mochila()
            .Guardar("Sei pedir ajuda por mensagem", TipoRecurso.Pessoal)
            .Guardar("Colega de turma", TipoRecurso.Pessoas);

        Assert.Equal(new[] { TipoRecurso.Material, TipoRecurso.Informacao }, mochila.TiposVazios());
        Assert.Equal(2, mochila.Perguntas().Count());
        Assert.Equal(2, mochila.PorTipo().Count());
    }

    [Fact]
    public void Recurso_sem_nome_e_recusado() =>
        Assert.Throws<ArgumentException>(() => new Mochila().Guardar("  ", TipoRecurso.Material));
}
