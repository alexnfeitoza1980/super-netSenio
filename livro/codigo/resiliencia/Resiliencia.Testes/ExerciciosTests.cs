using Jornada.Resiliencia.Cap88;
using Jornada.Resiliencia.Cap89;
using Jornada.Resiliencia.Cap90;
using Jornada.Resiliencia.Cap91;
using Jornada.Resiliencia.Cap92;
using Jornada.Resiliencia.Cap93;
using Jornada.Resiliencia.Cap94;
using Jornada.Resiliencia.Cap95;
using Jornada.Resiliencia.Cap96;

namespace Resiliencia.Testes;

public class ExerciciosTests
{
    [Theory]
    [InlineData("7", 7)]
    [InlineData("0", 0)]
    [InlineData("10", 10)]
    [InlineData("11", null)]
    [InlineData("-1", null)]
    [InlineData("sete", null)]
    [InlineData(null, null)]
    public void Cap88_LerNota_nao_lanca_excecao(string? entrada, int? esperado) =>
        Assert.Equal(esperado, Notas.LerNota(entrada));

    [Fact]
    public void Cap89_Texto_da_pausa_acompanha_o_tempo()
    {
        var relogio = new RelogioDeTeste();
        var pausa = new PausaConsciente(relogio, TimeSpan.FromMinutes(2));
        pausa.Iniciar();

        relogio.Avancar(TimeSpan.FromSeconds(30));
        Assert.Equal("Faltam 1 min e 30 s.", TextoDaPausa.Descrever(pausa));

        relogio.Avancar(TimeSpan.FromMinutes(2));
        Assert.StartsWith("Pausa concluída", TextoDaPausa.Descrever(pausa));
    }

    [Fact]
    public void Cap90_Resumo_da_mochila_por_tipo()
    {
        var mochila = new Mochila()
            .Guardar("Marta", TipoRecurso.Pessoas)
            .Guardar("Beto", TipoRecurso.Pessoas)
            .Guardar("Sei pesquisar", TipoRecurso.Pessoal);

        Assert.Equal(new[] { "Pessoal: 1 (Sei pesquisar)", "Pessoas: 2 (Beto, Marta)" }, mochila.Resumo());
    }

    [Fact]
    public void Cap91_Mais_recente_ignora_o_que_nao_tem_data()
    {
        var itens = new[]
        {
            new Evidencia("sem data", "x"),
            new Evidencia("março", "x", new DateOnly(2026, 3, 1)),
            new Evidencia("outubro", "x", new DateOnly(2026, 10, 1)),
        };

        Assert.Equal("outubro", itens.MaisRecente()!.Texto);
        Assert.Null(new[] { new Evidencia("sem data") }.MaisRecente());
    }

    [Fact]
    public void Cap92_Estimativa_conta_so_o_que_falta()
    {
        var meta = new Meta("Estudar").AdicionarDividindo("Lista", 60);
        Assert.Equal(60, meta.MinutosRestantes());
        Assert.StartsWith("Cerca de 60 min", meta.Estimativa());

        meta.ConcluirProximo(new DateOnly(2026, 11, 2));
        meta.ConcluirProximo(new DateOnly(2026, 11, 2));
        Assert.Equal("Cabe em um único passo.", meta.Estimativa());

        meta.ConcluirProximo(new DateOnly(2026, 11, 2));
        Assert.Equal("Nada pendente.", meta.Estimativa());
    }

    [Fact]
    public void Cap93_Um_tipo_novo_entra_no_mapa_sem_mudar_o_mapa()
    {
        var mapa = new MapaDeApoio().Adicionar(new GrupoDeEstudo("Algoritmos"));

        var quem = Assert.Single(mapa.Quem(Necessidade.Estudar));
        Assert.Contains("grupo Algoritmos", quem.ComoPedir(Necessidade.Estudar));
    }

    [Fact]
    public void Cap94_Pergunta_do_dia_e_estavel_para_o_mesmo_dia()
    {
        var situacao = new Situacao("Perdi um prazo");
        var dia = new DateOnly(2026, 11, 2);

        var a = ConversaDoDia.PerguntaDoDia(situacao, Lentes.Padrao, dia);
        var b = ConversaDoDia.PerguntaDoDia(situacao, Lentes.Padrao, dia);
        var outra = ConversaDoDia.PerguntaDoDia(situacao, Lentes.Padrao, dia.AddDays(1));

        Assert.Equal(a, b);
        Assert.NotEqual(a, outra);
    }

    [Fact]
    public void Cap95_Mapear_transforma_o_sucesso_e_repassa_a_falha()
    {
        var ok = CheckIn.Ler("1").Mapear(r => r.ToString().ToUpperInvariant());
        Assert.Equal("BEM", ok.Combinar(v => v, e => e));

        var falha = CheckIn.Ler("talvez").Mapear(r => r.ToString());
        Assert.Contains("1 a 4", falha.Combinar(v => v, e => e));
    }

    [Fact]
    public void Cap96_Espera_com_teto_nunca_passa_do_limite()
    {
        var politica = new PoliticaDeTentativas(10, TimeSpan.FromSeconds(1));
        var teto = TimeSpan.FromSeconds(10);

        Assert.Equal(TimeSpan.FromSeconds(4), politica.EsperaComTeto(3, teto));
        Assert.Equal(teto, politica.EsperaComTeto(6, teto));
    }
}
