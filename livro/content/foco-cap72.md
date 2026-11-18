---
id: foco-cap72
order: 94
kind: capitulo
num: 72
group: C# & Mentalidade · Parte 13 · Foco em C#
title: O líder bem focado: estilos e repertório
subtitle: Liderar bem é dirigir os três focos, e escolher o estilo certo para cada situação. Seis estilos como Strategy em C#, um seletor de estilo e um alerta de repertório estreito.
minutes: 55
hue: 70
objectives:
- Ver o **líder** como alguém que dirige a atenção de si, dos outros e do contexto
- Conhecer **seis estilos de liderança** e quando cada um funciona
- Implementar os estilos com o padrão **Strategy** e escolher por **situação**
- Reconhecer o risco do **líder de um estilo só** (o "pioneiro" que cobra sem parar)
- Tratar a classificação como **lente**, não como rótulo científico definitivo
origem: livros/04-foco-em-csharp/content/104-cap72.md
---

::: vila A líder que só tinha uma marcha
Dona Marta é uma ótima engenheira e foi promovida. Seu estilo: **dar o exemplo** e esperar que todos acompanhem seu ritmo. Funcionou até a equipe crescer. Hoje, a equipe cansada e confusa não entende o **porquê** das tarefas. Lia sugere: "Você tem **uma** marcha. Líderes precisam de **várias**, e de saber **qual usar quando**".
:::

::: ilustracao img/foco/cap72.svg | Líderes precisam de mais de uma marcha: escolher o estilo conforme a situação, não por hábito.
:::

## O que o livro diz

> O líder eficaz **dirige a atenção**: a **sua** (autoconsciência e autocontrole), a **dos outros** (empatia e relacionamento) e a **do contexto maior** (sistemas e estratégia). Os três focos, juntos, formam a base da **inteligência emocional** na liderança.

Pontos do capítulo original:

- O livro retoma estudos de consultoria que identificaram **seis estilos** de liderança: **visionário**, **treinador** (*coaching*), **agregador** (*afiliativo*), **democrático**, **pioneiro** (*pacesetting*) e **comandante** (*coercitivo*). Cada um cria um **clima** diferente.
- Líderes de melhor desempenho tendem a dominar **vários** estilos e a **trocar** conforme a situação.
- O **pioneiro**, que define altos padrões e dá o exemplo, funciona com equipes muito competentes e motivadas, mas **desgasta** quando usado o tempo todo.
- O **comandante** serve em **crises** (ou com equipes em colapso), mas, usado sempre, destrói o clima.
- Os líderes também tratam do **futuro**: pensar em prazos longos, no impacto sobre quem **ainda não está na sala** e no **sistema** em que a organização vive.
- Os **líderes informais** (quem as pessoas procuram, mesmo sem cargo) também dirigem atenção e clima.

::: cuidado Uma lente, não uma lei
Essa classificação em seis estilos vem de **estudos de consultoria** (com métodos e dados nem sempre abertos à revisão independente). É uma lente **útil** para pensar, mas **não** uma ciência definitiva. Não rotule pessoas ("você é um comandante"); descreva **comportamentos** e **efeitos** ("nessa reunião, deu ordens e a equipe ficou calada").
:::

## Da vida para o código

| No livro | Em engenharia de software |
|---|---|
| Líder dirige a atenção | **Tech lead** define o que a equipe olha |
| Seis estilos | Seis **estratégias** intercambiáveis (padrão *Strategy*) |
| Escolher por situação | **Seletor** com regras explícitas |
| Repertório amplo | **Mais de um** estilo à disposição |
| Líder informal | **Quem a equipe procura**, mesmo sem cargo |
| Pioneiro em excesso | Líder que **só dá exemplo** e cobra o mesmo ritmo de todos |

::: figura Três focos do líder
<svg viewBox="0 0 740 220" role="img" aria-label="O líder dirige três focos: interno, nos outros e externo, e escolhe entre seis estilos">
  <g font-size="13" font-weight="800" text-anchor="middle">
    <rect class="f-info" x="30" y="30" width="190" height="50" rx="10"/><text x="125" y="60">Foco em si</text>
    <rect class="f-warn" x="30" y="95" width="190" height="50" rx="10"/><text x="125" y="125">Foco nos outros</text>
    <rect class="f-good" x="30" y="160" width="190" height="50" rx="10"/><text x="125" y="190">Foco no contexto</text>
  </g>
  <path class="arrow" d="M222 55L300 110"/><path class="arrow" d="M222 120L300 120"/><path class="arrow" d="M222 185L300 130"/>
  <rect class="f-plain" x="302" y="85" width="140" height="70" rx="12"/>
  <text x="372" y="115" text-anchor="middle" font-weight="800">Seletor</text>
  <text x="372" y="135" text-anchor="middle" font-size="11" class="t-dim">situação → estilo</text>
  <path class="arrow" d="M444 120L520 120"/>
  <g font-size="12" text-anchor="start">
    <text x="530" y="40">• Visionário</text><text x="530" y="66">• Treinador</text><text x="530" y="92">• Agregador</text>
    <text x="530" y="118">• Democrático</text><text x="530" y="144">• Pioneiro</text><text x="530" y="170">• Comandante</text>
  </g>
</svg>
:::

## 1) Seis estilos como Strategy

Cada estilo é uma **estratégia** com: nome, uma frase que o resume e o **cuidado** principal. O restante do sistema só conhece a **interface**:

```cs Jornada/Foco/Cap72/Estilos.cs
namespace Jornada.Foco.Cap72;

public interface IEstiloDeLideranca
{
    string Nome { get; }
    string Frase { get; }
    string Cuidado { get; }
}

public sealed record Visionario : IEstiloDeLideranca
{
    public string Nome => "Visionário";
    public string Frase => "Vamos para lá, e este é o motivo.";
    public string Cuidado => "Sem o passo a passo, a visão vira slogan.";
}

public sealed record Treinador : IEstiloDeLideranca
{
    public string Nome => "Treinador";
    public string Frase => "Vamos desenvolver isso em você.";
    public string Cuidado => "Exige tempo; não serve para quem não quer aprender.";
}

public sealed record Agregador : IEstiloDeLideranca
{
    public string Nome => "Agregador";
    public string Frase => "As pessoas vêm primeiro.";
    public string Cuidado => "Só elogiar deixa o desempenho ruim sem resposta.";
}

public sealed record Democratico : IEstiloDeLideranca
{
    public string Nome => "Democrático";
    public string Frase => "O que vocês pensam?";
    public string Cuidado => "Reuniões sem fim quando falta decisão.";
}

public sealed record Pioneiro : IEstiloDeLideranca
{
    public string Nome => "Pioneiro";
    public string Frase => "Faça como eu, e agora.";
    public string Cuidado => "Com uso constante, esgota a equipe e mata a iniciativa.";
}

public sealed record Comandante : IEstiloDeLideranca
{
    public string Nome => "Comandante";
    public string Frase => "Faça isso porque eu mandei.";
    public string Cuidado => "Só em crise; fora dela destrói a confiança.";
}
```

## 2) O seletor de estilo

O seletor **explicita** as regras que a maioria dos líderes aplica sem perceber. As regras têm **ordem de prioridade**: a crise vem antes de tudo.

```cs Jornada/Foco/Cap72/SeletorDeEstilo.cs
namespace Jornada.Foco.Cap72;

public sealed record Situacao(
    bool Crise = false,
    bool EquipeSemRumo = false,
    bool EquipeTensa = false,
    bool PrecisaDeAdesao = false,
    bool PrecisaDeDesenvolvimento = false,
    bool EquipeCompetenteEMotivada = false);

public static class SeletorDeEstilo
{
    public static IEstiloDeLideranca Escolher(Situacao s) =>
        s.Crise                    ? new Comandante() :
        s.EquipeSemRumo            ? new Visionario() :
        s.EquipeTensa              ? new Agregador() :
        s.PrecisaDeAdesao          ? new Democratico() :
        s.PrecisaDeDesenvolvimento ? new Treinador() :
        s.EquipeCompetenteEMotivada ? new Pioneiro() :
                                      new Democratico();   // padrão: ouvir antes de decidir
}
```

```cs Uso
var casos = new (string Descricao, Situacao S)[]
{
    ("Produção fora do ar",             new Situacao(Crise: true)),
    ("Equipe sem rumo após reorganização", new Situacao(EquipeSemRumo: true)),
    ("Conflito entre duas pessoas",     new Situacao(EquipeTensa: true)),
    ("Júnior com vontade de crescer",   new Situacao(PrecisaDeDesenvolvimento: true)),
    ("Equipe sênior e motivada",        new Situacao(EquipeCompetenteEMotivada: true)),
};

foreach (var (descricao, situacao) in casos)
{
    var estilo = SeletorDeEstilo.Escolher(situacao);
    Console.WriteLine($"{descricao}");
    Console.WriteLine($"  → {estilo.Nome}: \"{estilo.Frase}\"");
    Console.WriteLine($"  ! {estilo.Cuidado}");
}
```

```saida
Produção fora do ar
  → Comandante: "Faça isso porque eu mandei."
  ! Só em crise; fora dela destrói a confiança.
Equipe sem rumo após reorganização
  → Visionário: "Vamos para lá, e este é o motivo."
  ! Sem o passo a passo, a visão vira slogan.
Conflito entre duas pessoas
  → Agregador: "As pessoas vêm primeiro."
  ! Só elogiar deixa o desempenho ruim sem resposta.
Júnior com vontade de crescer
  → Treinador: "Vamos desenvolver isso em você."
  ! Exige tempo; não serve para quem não quer aprender.
Equipe sênior e motivada
  → Pioneiro: "Faça como eu, e agora."
  ! Com uso constante, esgota a equipe e mata a iniciativa.
```

Observe o `?:` encadeado: ele **documenta** a prioridade. Se uma situação marcar crise **e** equipe tensa, a crise vence. Cada `Situacao` é **uma simplificação** (uma situação real mistura vários fatores), e a decisão final continua sendo **do líder**, não do código.

## 3) O alerta de repertório estreito

O risco de Dona Marta: **usar um estilo só**. Um pequeno diário de **estilos usados** ajuda a perceber:

```cs Jornada/Foco/Cap72/RepertorioDeEstilos.cs
namespace Jornada.Foco.Cap72;

public sealed class RepertorioDeEstilos
{
    public const int MinimoDeEstilos = 4;
    public const double LimiteDoDominante = 0.6;

    private readonly List<string> usos = new();

    public void Registrar(IEstiloDeLideranca estilo) => usos.Add(estilo.Nome);

    public int EstilosDistintos => usos.Distinct().Count();

    public (string Nome, double Fracao)? Dominante()
    {
        if (usos.Count == 0) return null;
        var topo = usos.GroupBy(n => n).OrderByDescending(g => g.Count()).ThenBy(g => g.Key, StringComparer.Ordinal).First();
        return (topo.Key, (double)topo.Count() / usos.Count);
    }

    public string? Aviso()
    {
        var dominante = Dominante();
        if (dominante is null) return null;

        if (EstilosDistintos < MinimoDeEstilos || dominante.Value.Fracao > LimiteDoDominante)
            return $"Repertório estreito: {EstilosDistintos} estilo(s) em uso; " +
                   $"'{dominante.Value.Nome}' domina com {dominante.Value.Fracao * 100:F0}%.";

        return null;
    }
}
```

```cs Uso
var repertorio = new RepertorioDeEstilos();
for (int i = 0; i < 8; i++) repertorio.Registrar(new Pioneiro());
repertorio.Registrar(new Comandante());
repertorio.Registrar(new Treinador());

Console.WriteLine(repertorio.Aviso() ?? "Repertório saudável.");
```

```saida
Repertório estreito: 3 estilo(s) em uso; 'Pioneiro' domina com 80%.
```

```cs JornadaTests/Foco/Cap72/Cap72Tests.cs
using Jornada.Foco.Cap72;
using Xunit;

public class Cap72Tests
{
    [Fact]
    public void Crise_vence_qualquer_outra_regra()
    {
        var estilo = SeletorDeEstilo.Escolher(new Situacao(Crise: true, EquipeTensa: true, EquipeSemRumo: true));
        Assert.IsType<Comandante>(estilo);
    }

    [Fact]
    public void Sem_nenhum_fator_o_padrao_e_ouvir() =>
        Assert.IsType<Democratico>(SeletorDeEstilo.Escolher(new Situacao()));

    [Fact]
    public void Equipe_sem_rumo_pede_visao() =>
        Assert.IsType<Visionario>(SeletorDeEstilo.Escolher(new Situacao(EquipeSemRumo: true)));

    [Fact]
    public void Equipe_competente_e_motivada_pede_pioneiro() =>
        Assert.IsType<Pioneiro>(SeletorDeEstilo.Escolher(new Situacao(EquipeCompetenteEMotivada: true)));

    [Fact]
    public void Sem_usos_nao_ha_aviso() =>
        Assert.Null(new RepertorioDeEstilos().Aviso());

    [Fact]
    public void Quatro_estilos_equilibrados_nao_geram_aviso()
    {
        var r = new RepertorioDeEstilos();
        foreach (IEstiloDeLideranca e in new IEstiloDeLideranca[]
                 { new Visionario(), new Treinador(), new Agregador(), new Democratico() })
            r.Registrar(e);

        Assert.Null(r.Aviso());
    }

    [Fact]
    public void Um_estilo_so_gera_aviso()
    {
        var r = new RepertorioDeEstilos();
        r.Registrar(new Pioneiro());
        Assert.NotNull(r.Aviso());
    }
}
```

## 4) O líder informal e a atenção

Nem toda liderança tem cargo. Em muitas equipes há alguém que **todos procuram** quando algo trava: o líder **informal**. O que o distingue: **dá atenção** (escuta, lembra do que foi dito), **explica o contexto** e **faz as pessoas se sentirem capazes**. Qualquer pessoa pode cultivar esse papel. Pergunta útil: **quem a equipe procura, e por quê?** A resposta aponta **comportamentos a copiar**.

::: exercicio Novo estilo, sem mexer no seletor?
O seletor usa `?:` encadeado: acrescentar uma regra exige **editar** a função. Refatore para uma **lista de regras** (`(Func<Situacao, bool>, Func<IEstiloDeLideranca>)`), avaliada **na ordem**, para que seja possível **registrar** novas regras sem mexer no código existente. Escreva um teste em que uma regra personalizada **antecede** as demais.

--- resposta
```cs Jornada/Foco/Cap72/SeletorExtensivel.cs
namespace Jornada.Foco.Cap72;

public sealed class SeletorExtensivel
{
    private readonly List<(Func<Situacao, bool> Condicao, Func<IEstiloDeLideranca> Estilo)> regras = new();

    public SeletorExtensivel Quando(Func<Situacao, bool> condicao, Func<IEstiloDeLideranca> estilo)
    {
        regras.Add((condicao, estilo));
        return this;
    }

    public IEstiloDeLideranca Escolher(Situacao s)
    {
        foreach (var (condicao, estilo) in regras)
            if (condicao(s))
                return estilo();

        return new Democratico();
    }
}
```
```cs Jornada/Foco/Cap72/SeletorExtensivelTests.cs
using Jornada.Foco.Cap72;
using Xunit;

public class SeletorExtensivelTests
{
    [Fact]
    public void Regra_registrada_primeiro_tem_prioridade()
    {
        var seletor = new SeletorExtensivel()
            .Quando(s => s.EquipeTensa, () => new Agregador())
            .Quando(s => s.Crise,       () => new Comandante());

        // Tensa e em crise: aqui a regra "tensa" foi registrada primeiro, então vence.
        Assert.IsType<Agregador>(seletor.Escolher(new Situacao(Crise: true, EquipeTensa: true)));
    }

    [Fact]
    public void Sem_regra_aplicavel_o_padrao_e_democratico() =>
        Assert.IsType<Democratico>(new SeletorExtensivel().Escolher(new Situacao(Crise: true)));
}
```
É o princípio **aberto/fechado** na prática: novas regras entram **por registro**, sem alterar o que já funciona. A **ordem** de registro vira uma decisão explícita, e isso é bom: prioridades escondidas em `?:` são fáceis de errar.
:::

::: resumo
- O líder dirige **três focos**: em si, nos outros e no contexto maior.
- Seis estilos (**visionário, treinador, agregador, democrático, pioneiro, comandante**) são **estratégias**; cada uma serve a situações diferentes e tem um **cuidado**.
- O **Strategy** em C# separa o **estilo** (o que fazer) da **escolha** (quando); o seletor torna as **prioridades** explícitas.
- O líder de **um estilo só** desgasta a equipe; observe o **repertório**.
- É uma **lente de consultoria**, não ciência definitiva: descreva **comportamentos e efeitos**, não rótulos.
:::

::: quiz
? Qual dos estilos serve melhor em uma crise?
+ Comandante, e só durante a crise
- Pioneiro
- Democrático
- Agregador
> Usado sempre, destrói a confiança.

? Qual o risco do estilo pioneiro usado o tempo todo?
+ Esgota a equipe e mata a iniciativa
- Nenhum
- Gera reuniões longas
- Dá pouco feedback
> Funciona com equipes competentes e motivadas, em doses.

? Que padrão de projeto aparece nos estilos de liderança?
+ Strategy: algoritmos (estilos) intercambiáveis atrás de uma interface
- Singleton
- Factory Method apenas
- Observer
> O seletor escolhe a estratégia.

? Por que a ordem das regras do seletor importa?
+ A primeira regra aplicável vence, então a prioridade fica explícita
- Porque o C# exige
- Porque a última sempre vence
- Não importa
> Crise antes de tudo.

? Como tratar a classificação em seis estilos?
+ Como lente útil de consultoria, e não como ciência definitiva
- Como lei científica
- Como rótulo para pessoas
- Como inútil
> Descreva comportamentos e efeitos.

? O que é o líder informal?
+ Alguém que as pessoas procuram e que dirige atenção e clima, mesmo sem cargo
- Quem não segue regras
- O gerente
- O dono da empresa
> Pergunte: quem a equipe procura, e por quê?
:::

::: cartas
Líder bem focado :: Quem dirige a atenção de si, dos outros e do contexto.
Visionário :: Estilo que mostra rumo e motivo.
Treinador :: Estilo que desenvolve pessoas a longo prazo.
Agregador :: Estilo que cria harmonia e vínculos.
Democrático :: Estilo que busca participação e adesão.
Pioneiro :: Estilo que dá exemplo e cobra padrão alto; cansa se constante.
Comandante :: Estilo de ordens diretas; útil em crises.
Repertório de estilos :: Conjunto de estilos que o líder usa conforme a situação.
:::
