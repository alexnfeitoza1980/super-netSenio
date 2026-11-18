using Jornada.Resiliencia.Cap89;
using Jornada.Resiliencia.Lab10;

namespace Resiliencia.Testes;

public class Lab10Tests
{
    private static readonly DateOnly Hoje = new(2026, 11, 2);

    private static (CadernoDeProximoPasso? Caderno, string Saida) Rodar(params string[] respostas)
    {
        var entrada = new StringReader(string.Join(Environment.NewLine, respostas));
        var saida = new StringWriter();
        var caderno = new Roteiro(entrada, saida, Hoje).Executar();
        return (caderno, saida.ToString());
    }

    [Fact]
    public void Uma_sessao_completa_monta_o_resumo_inteiro()
    {
        var (caderno, saida) = Rodar(
            "2",                                        // mais ou menos
            "Tenho uma tarefa atrasada do curso",
            "medo", "5",
            "Ainda não comecei a primeira parte",       // fato
            "Vou reprovar no curso inteiro",            // história
            "Começar o primeiro exercício", "1",        // depende de mim
            "Beto",
            "Resolver a lista de exercícios", "50");

        Assert.NotNull(caderno);
        Assert.Contains("Como me sinto: medo (5/10).", saida);
        Assert.Contains("Uma pausa de 2 min", saida);
        Assert.Contains("Fatos: 1 · Histórias que preciso checar: 1.", saida);
        Assert.Contains("Onde pôr energia primeiro: Começar o primeiro exercício.", saida);
        Assert.Contains("Próximo passo possível: Resolver a lista de exercícios (parte 1 de 2) (25 min).", saida);
        Assert.Contains("Quem pode apoiar: Beto.", saida);
    }

    [Fact]
    public void Pular_tudo_funciona_e_nao_quebra()
    {
        var (caderno, saida) = Rodar("", "", "", "", "", "", "", "", "", "");

        Assert.NotNull(caderno);
        Assert.Contains("Situação: uma tarefa que quero organizar", saida);
        Assert.Null(caderno!.Sentimento);
        Assert.Null(caderno.Meta);
    }

    [Fact]
    public void Quem_nao_esta_bem_recebe_os_servicos_e_pode_parar_sem_cobranca()
    {
        var (caderno, saida) = Rodar("3", "n");

        Assert.Null(caderno);
        Assert.Contains("188", saida);
        Assert.Contains("192", saida);
        Assert.Contains("quando você quiser", saida);
        Assert.DoesNotContain("Situação:", saida);
    }

    [Fact]
    public void Quem_nao_esta_bem_pode_escolher_continuar()
    {
        var (caderno, _) = Rodar("3", "s", "Organizar a mesa", "", "", "", "", "", "", "");
        Assert.NotNull(caderno);
        Assert.Equal("Organizar a mesa", caderno!.Situacao);
    }

    [Fact]
    public void Intensidade_fora_da_escala_e_ajustada_em_vez_de_quebrar()
    {
        var (caderno, _) = Rodar("1", "x", "raiva", "99");
        Assert.Equal(10, caderno!.Sentimento!.Intensidade);
        Assert.Equal(Emocao.Raiva, caderno.Sentimento.Emocao);
    }
}
