using Jornada.Habitos.Cap111;

namespace Habitos.Testes;

public class Cap111Tests
{
    private static void ConcluirTudo(RitualDeReuniao r)
    {
        r.Concluir("Todos presentes; a Bia trouxe um bloqueio");
        r.Concluir("O erro de pagamento subiu 5%");
        r.Concluir("Decidimos pausar a funcionalidade nova; agradecer ao Caio");
        r.Concluir("Lia investiga o erro até quinta");
    }

    [Fact]
    public void O_ritual_percorre_os_quatro_movimentos_em_ordem_e_termina()
    {
        var r = new RitualDeReuniao();
        Assert.Equal(Movimento.Reunir, r.Atual);

        r.Concluir("a");
        Assert.Equal(Movimento.Escutar, r.Atual);
        r.Concluir("b");
        Assert.Equal(Movimento.Partilhar, r.Atual);
        r.Concluir("c");
        Assert.Equal(Movimento.Enviar, r.Atual);
        Assert.False(r.Completo);
        r.Concluir("d");

        Assert.Equal(Movimento.Encerrado, r.Atual);
        Assert.True(r.Completo);
    }

    [Fact]
    public void Nao_da_para_pular_um_movimento_sem_registrar_nada()
    {
        var r = new RitualDeReuniao();
        var e = Assert.Throws<ArgumentException>(() => r.Concluir("   "));

        Assert.Contains("Quem está aqui", e.Message);
        Assert.Equal(Movimento.Reunir, r.Atual);
    }

    [Fact]
    public void Depois_de_encerrado_nao_aceita_mais_registros()
    {
        var r = new RitualDeReuniao();
        ConcluirTudo(r);
        Assert.Throws<InvalidOperationException>(() => r.Concluir("mais uma coisa"));
    }

    [Fact]
    public void O_evento_avisa_quem_se_inscreveu_a_cada_movimento()
    {
        var r = new RitualDeReuniao();
        var avisos = new List<(Movimento, Movimento)>();
        r.MovimentoConcluido += (origem, e) => avisos.Add((e.Concluido, e.Proximo));

        r.Concluir("a");
        r.Concluir("b");

        Assert.Equal(new[] { (Movimento.Reunir, Movimento.Escutar), (Movimento.Escutar, Movimento.Partilhar) }, avisos);
    }

    [Fact]
    public void Varios_ouvintes_recebem_o_mesmo_aviso_e_o_ritual_nao_conhece_nenhum()
    {
        var r = new RitualDeReuniao();
        var ata = new Ata(r);
        int contagem = 0;
        r.MovimentoConcluido += (_, _) => contagem++;

        ConcluirTudo(r);

        Assert.Equal(4, contagem);
        Assert.Equal(4, ata.Linhas.Count);
        Assert.StartsWith("1. Reunir:", ata.Linhas[0]);
        Assert.StartsWith("4. Enviar: Lia investiga", ata.Linhas[3]);
    }

    [Fact]
    public void Funciona_sem_nenhum_inscrito()
    {
        var r = new RitualDeReuniao();
        r.Concluir("a");                       // MovimentoConcluido é null: o "?." evita o erro
        Assert.Single(r.Registros);
    }

    [Theory]
    [InlineData(Movimento.Reunir)]
    [InlineData(Movimento.Escutar)]
    [InlineData(Movimento.Partilhar)]
    [InlineData(Movimento.Enviar)]
    public void Cada_movimento_tem_a_sua_pergunta(Movimento m) =>
        Assert.EndsWith("?", RitualDeReuniao.PerguntaDe(m));
}
