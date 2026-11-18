using Jornada.Joao.Cap131;

namespace Joao.Testes;

public class Cap131Tests
{
    private static DateTimeOffset T(int minutos) => new DateTimeOffset(2026, 11, 2, 9, 0, 0, TimeSpan.Zero).AddMinutes(minutos);

    private static LivroDeEventos AteOChamado()
    {
        var livro = new LivroDeEventos();
        livro.Registrar(new Faleceu(T(0)));
        livro.Registrar(new PedraTirada(T(10)));
        livro.Registrar(new Chamado(T(20)));
        return livro;
    }

    [Fact]
    public void O_estado_e_o_resultado_de_todos_os_eventos_somados()
    {
        var livro = AteOChamado();
        livro.Registrar(new Saiu(T(21)));

        var estado = livro.Estado();

        Assert.Equal(Situacao.Vivo, estado.Situacao);
        Assert.True(estado.JaSaiu);
        Assert.Equal(4, estado.Versao);
        Assert.Equal(3, estado.Ataduras.Count);
    }

    [Fact]
    public void Quem_sai_continua_atado_e_so_fica_livre_depois_de_desatado()
    {
        var livro = AteOChamado();
        livro.Registrar(new Saiu(T(21)));

        Assert.False(livro.Estado().Livre);

        foreach (var atadura in livro.Estado().Ataduras.ToList())
            livro.Registrar(new Desatado(T(22), atadura));

        Assert.True(livro.Estado().Livre);
    }

    [Fact]
    public void A_ordem_do_relato_e_exigida_nao_se_sai_sem_ser_chamado()
    {
        var livro = new LivroDeEventos();
        livro.Registrar(new Faleceu(T(0)));

        Assert.Throws<InvalidOperationException>(() => livro.Registrar(new Saiu(T(5))));
        Assert.Throws<InvalidOperationException>(() => livro.Registrar(new Chamado(T(5))));
    }

    [Fact]
    public void So_falece_quem_esta_vivo_e_so_se_tira_a_pedra_de_quem_faleceu()
    {
        var livro = new LivroDeEventos();

        Assert.Throws<InvalidOperationException>(() => livro.Registrar(new PedraTirada(T(0))));

        livro.Registrar(new Faleceu(T(1)));
        Assert.Throws<InvalidOperationException>(() => livro.Registrar(new Faleceu(T(2))));
    }

    [Fact]
    public void Viagem_no_tempo_mostra_como_estava_em_cada_momento()
    {
        var livro = AteOChamado();
        livro.Registrar(new Saiu(T(21)));

        Assert.Equal(Situacao.Vivo, livro.EstadoEm(T(-1)).Situacao);
        Assert.Equal(Situacao.Morto, livro.EstadoEm(T(15)).Situacao);
        Assert.True(livro.EstadoEm(T(15)).SepulcroAberto);
        Assert.False(livro.EstadoEm(T(15)).JaSaiu);
        Assert.True(livro.EstadoEm(T(21)).JaSaiu);
    }

    [Fact]
    public void Evento_fora_de_ordem_de_tempo_e_recusado()
    {
        var livro = new LivroDeEventos();
        livro.Registrar(new Faleceu(T(10)));

        Assert.Throws<InvalidOperationException>(() => livro.Registrar(new PedraTirada(T(5))));
    }

    [Fact]
    public void Reconstruir_do_zero_da_o_mesmo_estado_que_o_livro_ja_tinha()
    {
        var livro = AteOChamado();
        livro.Registrar(new Saiu(T(21)));
        livro.Registrar(new Desatado(T(22), "lenço no rosto"));

        var atual = livro.Estado();
        var reconstruido = LivroDeEventos.Reconstruir(livro.Eventos);

        Assert.Equal(atual.Situacao, reconstruido.Situacao);
        Assert.Equal(atual.Versao, reconstruido.Versao);
        Assert.Equal(atual.Ataduras, reconstruido.Ataduras);          // listas se comparam elemento a elemento
    }

    [Fact]
    public void Desatar_o_que_nao_existe_e_erro_e_nao_entra_no_livro()
    {
        var livro = AteOChamado();
        livro.Registrar(new Saiu(T(21)));

        Assert.Throws<InvalidOperationException>(() => livro.Registrar(new Desatado(T(22), "corrente")));
        Assert.Equal(4, livro.Eventos.Count);
    }
}
