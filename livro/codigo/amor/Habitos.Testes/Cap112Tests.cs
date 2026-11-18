using Jornada.Habitos.Cap112;

namespace Habitos.Testes;

public class Cap112Tests
{
    private static readonly DayOfWeek[] DiasUteis =
        { DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday };

    private static CalendarioDoTime TimeTipico() => new CalendarioDoTime()
        .Adicionar(new Pratica("Daily", Intencao.Alinhar, 15, DiasUteis))
        .Adicionar(new Pratica("Revisão de PR", Intencao.Aprender, 30, new[] { DayOfWeek.Tuesday, DayOfWeek.Thursday }))
        .Adicionar(new Pratica("Demo", Intencao.Entregar, 45, new[] { DayOfWeek.Friday }));

    private static readonly DateOnly Segunda = new(2026, 11, 2);          // uma segunda-feira

    [Fact]
    public void Ocorrencias_geram_so_os_dias_em_que_a_pratica_acontece()
    {
        var uma = TimeTipico().Ocorrencias(Segunda, Segunda.AddDays(6)).ToList();

        Assert.Equal(5 + 2 + 1, uma.Count);
        Assert.DoesNotContain(uma, o => o.Dia.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday);
    }

    [Fact]
    public void A_agenda_vem_ordenada_por_dia_e_depois_por_nome()
    {
        var agenda = TimeTipico().Agenda(Segunda, Segunda.AddDays(4)).ToList();

        Assert.Equal(8, agenda.Count);
        Assert.Equal(Segunda, agenda[0].Dia);
        Assert.Equal(new[] { "Daily", "Revisão de PR" }, agenda.Where(o => o.Dia == Segunda.AddDays(1)).Select(o => o.Pratica.Nome));
        Assert.Equal(agenda.OrderBy(o => o).ToList(), agenda);
    }

    [Fact]
    public void Comparar_trata_o_nulo_e_prioriza_a_data()
    {
        var p = new Pratica("A", Intencao.Alinhar, 10, DiasUteis);
        var cedo = new Ocorrencia(Segunda, p);
        var tarde = new Ocorrencia(Segunda.AddDays(1), p);

        Assert.True(cedo.CompareTo(tarde) < 0);
        Assert.True(tarde.CompareTo(cedo) > 0);
        Assert.Equal(1, cedo.CompareTo(null));
    }

    [Fact]
    public void Minutos_por_semana_mostram_que_historia_o_calendario_conta()
    {
        var minutos = TimeTipico().MinutosPorSemana();

        Assert.Equal(75, minutos[Intencao.Alinhar]);          // 15 x 5
        Assert.Equal(60, minutos[Intencao.Aprender]);         // 30 x 2
        Assert.Equal(45, minutos[Intencao.Entregar]);
        Assert.False(minutos.ContainsKey(Intencao.Cuidar));
    }

    [Fact]
    public void Lacunas_apontam_intencoes_sem_nenhuma_pratica()
    {
        Assert.Equal(new[] { Intencao.Cuidar, Intencao.Celebrar }, TimeTipico().Lacunas());

        var completo = TimeTipico()
            .Adicionar(new Pratica("Retro", Intencao.Cuidar, 60, new[] { DayOfWeek.Friday }))
            .Adicionar(new Pratica("Brinde da sexta", Intencao.Celebrar, 20, new[] { DayOfWeek.Friday }));
        Assert.Empty(completo.Lacunas());
    }

    [Fact]
    public void O_iterador_e_preguicoso_e_nao_gera_alem_do_que_se_pede()
    {
        var primeira = TimeTipico().Ocorrencias(Segunda, DateOnly.MaxValue.AddDays(-1)).First();
        Assert.Equal(Segunda, primeira.Dia);
    }
}
