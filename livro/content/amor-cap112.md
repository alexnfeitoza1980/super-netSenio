---
id: amor-cap112
order: 144
kind: capitulo
num: 112
group: C# & Mentalidade · Parte 17 · Você é aquilo que ama, em C#
title: Ritmos que formam o time
subtitle: O éthos de um lugar nasce de pequenas práticas repetidas, mais do que de seus discursos. Em C#, é o capítulo da ordenação: IComparable, SortedSet, iteradores sobre datas e um calendário que mostra que história o time conta.
minutes: 70
hue: 275
objectives:
- Entender o **éthos** (a "energia" implícita) de uma casa ou equipe como efeito de **rotinas repetidas**
- Distinguir **práticas estruturantes** grandiosas e triviais, e por que as pequenas pesam desproporcionalmente
- Implementar **`IComparable<T>`** e usar **`SortedSet<T>`** para uma agenda ordenada sem repetição
- Escrever um **iterador sobre datas** (`DateOnly`, `yield return`) e somar minutos por intenção com LINQ
origem: livros/08-voce-e-aquilo-que-ama-csharp/content/132-cap112.md
---

::: vila O que o time aprendeu sem ninguém dizer
Bia entrou em uma equipe nova. Na primeira semana, ninguém lhe falou da "cultura" do time. Mas ela viu: o **daily** começava sempre às 9h em ponto, mesmo que faltassem pessoas; as **revisões de código** eram feitas em dupla, com café; e toda sexta alguém trazia um **bolo** para a demonstração. Em um mês, Bia sabia, sem que ninguém explicasse: *aqui, pontualidade importa; ninguém revisa sozinho; entrega é motivo de comemoração*.
:::

::: ilustracao img/amor/cap112.svg | Um calendário compartilhado mostra, em horas, que histórias o time conta: o que se alinha, o que se aprende, o que se entrega, o que se cuida.
:::

## O que o livro diz

Em dois momentos diferentes, Smith fala do mesmo fenômeno, em casa e na escola:

**Em casa.** Ele chama de **éthos** (do grego, o "caráter" de um lugar) o **zumbido de fundo** que as rotinas de uma casa produzem: um "papel de parede imaginário" que influencia como as pessoas imaginam o mundo. Toda casa tem esse cantarolar, e ele é **afinado por algum fim**. Por isso sugere uma **auditoria** (ele usa a palavra "litúrgica") das rotinas da casa: *que história é contada nesses ritmos? Que visão de boa vida? Que tipo de gente é formada por eles?* Alerta que isso é **contextual**: uma república de estudantes e uma casa com bebês têm rotinas e tentações completamente diferentes. E dá ao **jantar em família** um papel de destaque: uma "liturgia em desuso" e um lugar poderoso de formação, mesmo quando a conversa vira bate-boca.

**Em comunidades e escolas.** Ele propõe o conceito de **práticas estruturantes**: rituais de **orientação** e **repetição** que organizam o que se faz em conjunto e reforçam a missão e o éthos de uma instituição, seja uma equipe esportiva, uma empresa ou um museu. Há as **macro**, "grandiosas" (as reuniões de entrada, a Semana do Calouro) e as **micro**, "triviais" (as rotinas diárias, os gritos de guerra, os cartazes no vestiário). As micro ocupam pouco tempo, mas têm **influência desproporcional**. E as melhores atuam sobre a **imaginação**, e não apenas informam: ele ironiza que as piores diretrizes corporativas são "slides sucessivos" derramando regras que nunca tocam a imaginação.

Ele também conta, a partir de um teórico da educação, a história de **dois pedreiros**. Perguntados sobre o que fazem, um diz: "estou cortando esta pedra em um quadrado perfeito". O outro: "**estou construindo uma catedral**". As tarefas pequenas importam justamente porque **servem a algo maior**.

::: cuidado Auditar não é vigiar
Uma auditoria de rotinas é **autoexame**, feita **por quem vive** nelas, e **não** instrumento de controle de uma pessoa sobre outra. Ninguém aqui está sendo medido: o modelo só **organiza uma conversa** sobre o que as práticas do time comunicam. Compartilhe só o que quiser, e use dados **do time**, não de indivíduos.
:::

::: analogia O zumbido da geladeira
Você só percebe o zumbido da geladeira quando ele para. Rotinas fazem algo parecido: viram tão "normais" que **ninguém as enxerga**, e é justamente por isso que formam sem pedir licença. A auditoria é o exercício de **ouvir o zumbido**.
:::

## Da vida para o código

| No livro | Em C# |
|---|---|
| Uma prática estruturante | `Pratica(Nome, Intencao, Minutos, Dias)` |
| A história que ela conta | `enum Intencao` (alinhar, aprender, cuidar, entregar, celebrar) |
| Ritmos que se repetem pela semana | **`yield return`** gerando ocorrências por dia (`DateOnly`) |
| Uma agenda ordenada | **`IComparable<Ocorrencia>`** + **`SortedSet<T>`** |
| Quanto tempo cada história recebe | `MinutosPorSemana` (`GroupBy`, `Sum`) |
| "O que não tem nenhum ritmo?" | `Lacunas` (`Enum.GetValues` + `Except`) |

::: figura Mesma duração, histórias diferentes
<svg viewBox="0 0 740 230" role="img" aria-label="Barras horizontais mostrando minutos por semana de cada intenção: alinhar 75, aprender 60, entregar 45, cuidar 0 e celebrar 0">
  <text x="30" y="26" font-weight="800">Minutos por semana, por intenção (exemplo)</text>
  <g font-size="13">
    <text x="30" y="62">Alinhar</text><rect x="130" y="46" width="300" height="24" rx="6" class="f-info"/><text x="440" y="63" class="t-dim" font-size="12">75 min · Daily</text>
    <text x="30" y="102">Aprender</text><rect x="130" y="86" width="240" height="24" rx="6" class="f-good"/><text x="380" y="103" class="t-dim" font-size="12">60 min · Revisão de PR</text>
    <text x="30" y="142">Entregar</text><rect x="130" y="126" width="180" height="24" rx="6" class="f-prim"/><text x="320" y="143" class="t-dim" font-size="12">45 min · Demo</text>
    <text x="30" y="182">Cuidar</text><rect x="130" y="166" width="6" height="24" rx="3" class="f-bad"/><text x="146" y="183" class="t-dim" font-size="12">nenhum ritmo · convite a conversar</text>
    <text x="30" y="212">Celebrar</text><rect x="130" y="196" width="6" height="24" rx="3" class="f-bad"/><text x="146" y="213" class="t-dim" font-size="12">nenhum ritmo · convite a conversar</text>
  </g>
</svg>
:::

## 1) Ocorrências ordenadas por data e nome

Uma `Pratica` acontece em certos dias da semana. Para montar a **agenda** de duas semanas, geramos uma `Ocorrencia` (a prática em um dia) para cada dia que casa.

O primeiro recurso é um **iterador sobre datas**: um `for` que anda de `de` até `ate`, de dia em dia (`dia.AddDays(1)`), entregando cada ocorrência com `yield return`. Ele é **preguiçoso**: se você só pedir a primeira, só a primeira é calculada.

O segundo é a **ordem**. Para que o C# saiba ordenar objetos seus, o tipo implementa **`IComparable<T>`**, com o método `CompareTo`: devolve **negativo** se este vem antes, **zero** se são iguais e **positivo** se vem depois. Com isso, `SortedSet<Ocorrencia>` mantém **tudo ordenado sozinho** e **sem repetição**.

```cs Jornada/Habitos/Cap112/Calendario.cs
namespace Jornada.Habitos.Cap112;

/// <summary>A que uma prática serve. É a "história" que ela conta sobre o que o time valoriza.</summary>
public enum Intencao { Alinhar, Aprender, Cuidar, Entregar, Celebrar }

public sealed record Pratica(string Nome, Intencao Intencao, int Minutos, IReadOnlyList<DayOfWeek> Dias);

/// <summary>Uma prática acontecendo num dia. Sabe se comparar com outra: por data e, no mesmo dia, por nome.</summary>
public sealed record Ocorrencia(DateOnly Dia, Pratica Pratica) : IComparable<Ocorrencia>
{
    public int CompareTo(Ocorrencia? outra)
    {
        if (outra is null) return 1;

        int porDia = Dia.CompareTo(outra.Dia);
        return porDia != 0 ? porDia : string.Compare(Pratica.Nome, outra.Pratica.Nome, StringComparison.Ordinal);
    }
}

public sealed class CalendarioDoTime
{
    private readonly List<Pratica> praticas = new();

    public CalendarioDoTime Adicionar(Pratica pratica) { praticas.Add(pratica); return this; }

    public IReadOnlyList<Pratica> Praticas => praticas;

    /// <summary>Gera, dia a dia, o que acontece entre duas datas. "yield return" entrega um de cada vez.</summary>
    public IEnumerable<Ocorrencia> Ocorrencias(DateOnly de, DateOnly ate)
    {
        for (var dia = de; dia <= ate; dia = dia.AddDays(1))
        {
            foreach (var pratica in praticas.Where(p => p.Dias.Contains(dia.DayOfWeek)))
                yield return new Ocorrencia(dia, pratica);
        }
    }

    /// <summary>Agenda ordenada, sem repetição, usando um SortedSet (que se mantém ordenado sozinho).</summary>
    public IReadOnlyCollection<Ocorrencia> Agenda(DateOnly de, DateOnly ate) =>
        new SortedSet<Ocorrencia>(Ocorrencias(de, ate));

    /// <summary>Quantos minutos por semana cada intenção recebe: o "peso" de cada história que o time conta.</summary>
    public IReadOnlyDictionary<Intencao, int> MinutosPorSemana() =>
        praticas
            .GroupBy(p => p.Intencao)
            .ToDictionary(g => g.Key, g => g.Sum(p => p.Minutos * p.Dias.Count));

    /// <summary>Intenções que nenhuma prática atende. É um convite a conversar, não uma falha.</summary>
    public IReadOnlyList<Intencao> Lacunas() =>
        Enum.GetValues<Intencao>().Except(praticas.Select(p => p.Intencao)).ToList();
}
```

Repare em quatro detalhes:

- **`CompareTo` trata o `null`** (`if (outra is null) return 1`): por convenção, qualquer objeto é maior que `null`.
- **Compara por dia e, no mesmo dia, pelo nome**, com `StringComparison.Ordinal` (comparação previsível, sem depender da cultura).
- **`Enum.GetValues<Intencao>().Except(...)`** acha as intenções **sem nenhuma prática**: não é falha, é **convite** a conversar ("e o cuidado com o time?").
- **`MinutosPorSemana`** usa `GroupBy` + `ToDictionary`: para cada intenção, soma `minutos × número de dias`.

```cs Uso
var uteis = new[] { DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday };

var calendario = new CalendarioDoTime()
    .Adicionar(new Pratica("Daily", Intencao.Alinhar, 15, uteis))
    .Adicionar(new Pratica("Revisão de PR", Intencao.Aprender, 30, new[] { DayOfWeek.Tuesday, DayOfWeek.Thursday }))
    .Adicionar(new Pratica("Demo", Intencao.Entregar, 45, new[] { DayOfWeek.Friday }));

foreach (var (intencao, minutos) in calendario.MinutosPorSemana().OrderByDescending(p => p.Value))
    Console.WriteLine($"{intencao}: {minutos} min por semana");

Console.WriteLine("Sem nenhuma prática para: " + string.Join(", ", calendario.Lacunas()));

var segunda = new DateOnly(2026, 11, 2);
foreach (var o in calendario.Agenda(segunda, segunda.AddDays(1)))
    Console.WriteLine($"{o.Dia:dd/MM}  {o.Pratica.Nome} ({o.Pratica.Minutos} min)");
```

```saida
Alinhar: 75 min por semana
Aprender: 60 min por semana
Entregar: 45 min por semana
Sem nenhuma prática para: Cuidar, Celebrar
02/11  Daily (15 min)
03/11  Daily (15 min)
03/11  Revisão de PR (30 min)
```

O calendário **conta uma história sem que ninguém a tenha escrito**: 75 minutos por semana vão para "alinhar" e 0 para "cuidar" e "celebrar". Isso **não** quer dizer que o time seja frio, e talvez haja cuidado em outros lugares. Mas é uma excelente pergunta para a próxima retrospectiva: "**o que o nosso calendário diz que importa?**".

::: dica Pequeno também forma
Se o **daily** de 15 minutos é o ritmo que mais se repete, ele pesa mais na formação do time do que a grande reunião trimestral de visão. Cuide primeiro das **micropráticas**: são as que a equipe vive todos os dias.
:::

## 2) Testes

```cs Habitos.Testes/Cap112Tests.cs
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
```

O teste `A_agenda_vem_ordenada_por_dia_e_depois_por_nome` fecha o ciclo da ordenação: `agenda.OrderBy(o => o).ToList()` usa o mesmo `IComparable`, e **precisa** devolver a mesma lista. E `O_iterador_e_preguicoso...` pede a primeira ocorrência de um intervalo **gigantesco** e termina na hora.

::: exercicio A próxima vez
Escreva o método de extensão `ProximaOcorrencia(this CalendarioDoTime calendario, string nomeDaPratica, DateOnly depoisDe)`, que devolve a **próxima ocorrência** da prática **depois** de uma data, olhando até **uma semana à frente**, ou `null` se não houver. Dica: reaproveite `Ocorrencias`.

--- resposta
```cs Jornada/Habitos/Exercicios/Cap112Proxima.cs
namespace Jornada.Habitos.Cap112;

public static class CalendarioExtensoes
{
    /// <summary>A próxima vez, depois de uma data, em que a prática acontece (olhando uma semana à frente).</summary>
    public static Ocorrencia? ProximaOcorrencia(this CalendarioDoTime calendario, string nomeDaPratica, DateOnly depoisDe) =>
        calendario
            .Ocorrencias(depoisDe.AddDays(1), depoisDe.AddDays(7))
            .FirstOrDefault(o => o.Pratica.Nome == nomeDaPratica);
}
```

Em vez de reescrever a lógica de datas, a solução **reaproveita** o iterador: `Ocorrencias(depoisDe.AddDays(1), depoisDe.AddDays(7))` gera só uma semana, e `FirstOrDefault(o => o.Pratica.Nome == nomeDaPratica)` acha a primeira. Por ser preguiçoso, o iterador **para** assim que a achar. Reaproveitar peças pequenas e bem testadas é o jeito de construir sem complicar.
:::

::: resumo
- O **éthos** de uma casa ou equipe nasce de **rotinas repetidas**, o "zumbido de fundo" que ninguém percebe; **auditá-las** (por quem vive nelas) revela a história que contam.
- **Práticas estruturantes** grandiosas (orientações) e **triviais** (rotinas diárias) formam; as pequenas têm **influência desproporcional**.
- **`IComparable<T>`** ensina o C# a ordenar seu tipo; **`SortedSet<T>`** mantém a coleção ordenada e sem repetição.
- Um **iterador sobre datas** (`for` + `yield return`) gera ocorrências sob demanda; **`GroupBy`/`Sum`** somam minutos por intenção; **`Except`** acha lacunas.
- A auditoria é **autoexame** e **convite a conversar**, nunca vigilância nem avaliação de pessoas.
:::

::: quiz
? O que é o "éthos" de uma casa ou equipe, no capítulo?
+ A "energia" implícita criada por suas rotinas diárias, como um zumbido de fundo
- O regulamento escrito
- O nome da equipe
- A decoração do ambiente
> Rotinas repetidas formam o caráter do lugar sem que se perceba.

? Por que as "micropráticas" pesam tanto, segundo o autor?
+ Porque ocupam pouco tempo, mas são repetidas, e por isso formam desproporcionalmente
- Porque são obrigatórias por lei
- Porque duram mais
- Porque são mais caras
> A repetição diária faz o efeito.

? O que a história dos dois pedreiros ensina?
+ Que tarefas pequenas ganham sentido ao serem vistas a serviço de algo maior
- Que construir é cansativo
- Que catedrais são antigas
- Que pedreiros são felizes
> "Estou construindo uma catedral" muda o significado do trabalho.

? O que `CompareTo` devolve quando "este objeto vem antes do outro"?
+ Um número negativo
- Zero
- Um número positivo
- Verdadeiro
> Negativo = antes; zero = igual; positivo = depois.

? Por que usar `SortedSet<Ocorrencia>` na agenda?
+ Porque mantém os itens ordenados e sem repetição automaticamente
- Porque é o único que aceita datas
- Porque apaga o passado
- Porque converte para texto
> Ele usa o `IComparable` do tipo para ordenar.

? Como a auditoria do capítulo deve ser usada?
+ Como autoexame e conversa do próprio time, sem vigiar nem avaliar pessoas
- Como ferramenta de controle individual
- Como ranking entre times
- Como medidor de produtividade
> O modelo organiza uma conversa; não mede quem trabalha.
:::

::: cartas
Éthos :: Caráter ou "energia" implícita de um lugar, formada por suas rotinas.
Prática estruturante :: Ritual de orientação ou repetição que organiza a vida de uma comunidade.
Macroprática / micro :: Grandiosa (orientação inicial) × trivial (rotina diária).
Auditoria de rotinas :: Autoexame: que história nossos ritmos contam?
IComparable<T> :: Interface que ensina o C# a ordenar objetos do tipo.
SortedSet<T> :: Conjunto que se mantém ordenado e sem repetição.
Iterador sobre datas :: `for` com `yield return` que gera um item por dia.
Lacuna :: Intenção sem nenhuma prática; convite a conversar, não falha.
:::
