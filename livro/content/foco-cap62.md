---
id: foco-cap62
order: 84
kind: capitulo
num: 62
group: C# & Mentalidade · Parte 13 · Foco em C#
title: Fadiga de atenção e pausas que restauram
subtitle: Atenção focada cansa. Um modelo simples de energia mostra por que pausas importam, e por que nem toda pausa restaura o mesmo tanto, com todas as ressalvas que um modelo exige.
minutes: 50
hue: 245
objectives:
- Entender a **fadiga de atenção** como esgotamento do esforço de **inibir distrações**
- Distinguir **pausas restauradoras** (natureza, silêncio) de **pausas "de tela"**
- Construir um **modelo de energia** de atenção em C# e simular um dia de trabalho
- Calcular **quando parar** a partir de um limite, em vez de esperar o esgotamento
- Lembrar que o modelo é **ilustrativo**: seus números vêm de **medir você**
origem: livros/04-foco-em-csharp/content/94-cap62.md
---

::: vila Quinta, 16h
Às 16h, Caio relê a mesma linha de código pela quarta vez sem entender o que ela faz. Ele culpa o café, depois o servidor, depois a própria inteligência. Lia pergunta só: "Há quanto tempo você não para?". Ele responde: "Desde as 13h". **Ele não estava burro; estava esgotado.**
:::

::: ilustracao img/foco/cap62.svg | Atenção cansa. Nem toda pausa restaura o mesmo tanto: a caminhada ganha da pausa na tela.
:::

## O que o livro diz

> **Prestar atenção** exige um esforço de **inibição**: segurar o foco e afastar o resto. Esse esforço se **esgota** com o uso. Quando cansamos, ficamos mais **irritáveis**, **impulsivos** e **distraídos**.

Pontos do capítulo original:

- A atenção dirigida (a de **esforço**) é um recurso que **fatiga**; a atenção **sem esforço**, atraída por algo fascinante, **restaura**.
- A teoria da **restauração da atenção** (dos Kaplan) diz que **ambientes naturais** (parques, jardins, árvores) recuperam o foco melhor que ambientes urbanos barulhentos.
- Por isso, uma **caminhada** costuma ajudar mais que uma pausa olhando o celular — pelo menos segundo parte dos estudos.
- A **meditação** (atenção plena) funciona como **treino**: cada vez que você percebe que divagou e volta, repete uma "repetição" de atenção (voltaremos a isso).
- Cansaço também afeta o **autocontrole**: quem está esgotado cede mais fácil à distração.

::: cuidado Calma com o "esgotamento do ego"
A ideia de que o autocontrole é um "tanque" que esvazia foi muito popular, mas **algumas réplicas não confirmaram** o efeito com a mesma força. O que é mais seguro afirmar: **cansaço prejudica o desempenho** e **pausas bem escolhidas ajudam** — sem apostar em números exatos.
:::

## Da vida para o código

| No livro | Em engenharia de software |
|---|---|
| Atenção dirigida esgota | **Sessão longa** sem pausa → mais erros |
| Fascinação sem esforço restaura | **Caminhada**, janela, silêncio |
| Ambiente natural > urbano | Sair do ambiente barulhento |
| Cansaço → impulsividade | **Commits apressados**, *review* superficial |
| Pausa preventiva | **Parar antes** do esgotamento |

::: figura Energia ao longo do dia
<svg viewBox="0 0 740 230" role="img" aria-label="Gráfico de energia de atenção: cai no trabalho focado, sobe em pausas boas e cai de novo">
  <path class="arrow" d="M40 190L40 20"/><path class="arrow" d="M40 190L720 190"/>
  <text x="30" y="30" text-anchor="end" font-size="11">alta</text>
  <text x="30" y="188" text-anchor="end" font-size="11">baixa</text>
  <line x1="40" y1="130" x2="720" y2="130" class="f-bad" stroke-dasharray="5 5" style="fill:none;stroke-width:1.5"/>
  <text x="716" y="124" text-anchor="end" font-size="11">limite de parada</text>
  <polyline points="40,40 200,100 240,98 320,150 380,85 540,150 600,152 660,90" fill="none" stroke-width="3" style="stroke:var(--accent,#4f7cff)"/>
  <g font-size="11" text-anchor="middle">
    <text x="120" y="212">foco</text>
    <text x="220" y="212">tela</text>
    <text x="350" y="212">caminhada</text>
    <text x="460" y="212">foco</text>
    <text x="570" y="212">tela</text>
    <text x="640" y="212">silêncio</text>
  </g>
</svg>
:::

## 1) Um modelo simples de energia

Vamos modelar a atenção como uma **energia de 0 a 100** que cai com algumas atividades e sobe com outras. As **taxas** abaixo são **ilustrativas**: o que importa é o formato do raciocínio.

```cs Jornada/Foco/Cap62/ModeloDeEnergia.cs
namespace Jornada.Foco.Cap62;

public enum Atividade { TrabalhoFocado, ReuniaoDensa, PausaNaTela, Silencio, Caminhada }

public static class Taxas
{
    /// <summary>Variação de energia por minuto. Valores ILUSTRATIVOS: meça os seus.</summary>
    public static double PorMinuto(Atividade a) => a switch
    {
        Atividade.TrabalhoFocado => -0.5,
        Atividade.ReuniaoDensa   => -0.6,
        Atividade.PausaNaTela    => -0.1,   // quase não restaura: a atenção continua sendo puxada
        Atividade.Silencio       => 0.5,
        Atividade.Caminhada      => 0.8,
        _ => throw new ArgumentOutOfRangeException(nameof(a)),
    };
}

public sealed record Energia(double Valor)
{
    public static Energia Cheia { get; } = new(100);

    public Energia Aplicar(Atividade atividade, int minutos)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(minutos);
        return new Energia(Math.Clamp(Valor + Taxas.PorMinuto(atividade) * minutos, 0, 100));
    }

    public string Estado => Valor switch
    {
        >= 60 => "boa",
        >= 30 => "cansando",
        _     => "fadiga: risco de erro",
    };

    /// <summary>Quantos minutos de esforço restam antes de cruzar o limite (0 se já cruzou).</summary>
    public int MinutosAteOLimite(Atividade atividade, double limite)
    {
        double taxa = Taxas.PorMinuto(atividade);
        if (taxa >= 0) throw new ArgumentException("A atividade precisa gastar energia.", nameof(atividade));
        if (Valor <= limite) return 0;
        return (int)Math.Floor((Valor - limite) / -taxa);
    }
}
```

```cs Uso
var dia = new (Atividade, int)[]
{
    (Atividade.TrabalhoFocado, 90),
    (Atividade.PausaNaTela,    10),
    (Atividade.TrabalhoFocado, 60),
    (Atividade.Caminhada,      20),
    (Atividade.TrabalhoFocado, 90),
    (Atividade.PausaNaTela,    10),
    (Atividade.ReuniaoDensa,   60),
};

var energia = Energia.Cheia;
foreach (var (atividade, minutos) in dia)
{
    energia = energia.Aplicar(atividade, minutos);
    Console.WriteLine($"{atividade,-15} {minutos,3} min → {energia.Valor,3:F0} ({energia.Estado})");
}

Console.WriteLine($"Com 100 de energia, dá para focar {Energia.Cheia.MinutosAteOLimite(Atividade.TrabalhoFocado, 40)} min até o limite 40.");
```

```saida
TrabalhoFocado   90 min →  55 (cansando)
PausaNaTela      10 min →  54 (cansando)
TrabalhoFocado   60 min →  24 (fadiga: risco de erro)
Caminhada        20 min →  40 (cansando)
TrabalhoFocado   90 min →   0 (fadiga: risco de erro)
PausaNaTela      10 min →   0 (fadiga: risco de erro)
ReuniaoDensa     60 min →   0 (fadiga: risco de erro)
Com 100 de energia, dá para focar 120 min até o limite 40.
```

Repare no que o modelo *mostra* (e não só calcula): depois do primeiro bloco, a "pausa na tela" quase não devolve nada; a caminhada recupera bastante; e o segundo bloco longo **zera** a energia. **Parar antes** do limite é mais barato que tentar trabalhar na fadiga.

::: dica Como medir os seus números
Durante duas semanas, anote **a cada bloco**: o que fez, por quanto tempo e, ao final, uma nota de 0 a 10 para "quão claro está o raciocínio". As taxas do modelo saem de **ajustar esses dados**, não de um livro. O modelo da vida real é mais irregular (e a sua curva muda com sono, humor e tarefa); use-o como **conversa**, não como medida.
:::

## 2) Um planejador de paradas

Em vez de esperar o cansaço, o código pode **sugerir o próximo ponto de parada**:

```cs Jornada/Foco/Cap62/PlanejadorDePausas.cs
namespace Jornada.Foco.Cap62;

public static class PlanejadorDePausas
{
    /// <summary>
    /// Divide 'minutosDeFoco' em blocos que não cruzem o limite, intercalando caminhadas de 'minutosDePausa'.
    /// </summary>
    public static IReadOnlyList<(Atividade Atividade, int Minutos)> Planejar(
        int minutosDeFoco, double limite = 40, int minutosDePausa = 30)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(minutosDeFoco);
        var plano = new List<(Atividade, int)>();
        var energia = Energia.Cheia;
        int restante = minutosDeFoco;

        while (restante > 0)
        {
            int ate = energia.MinutosAteOLimite(Atividade.TrabalhoFocado, limite);
            if (ate == 0)
            {
                plano.Add((Atividade.Caminhada, minutosDePausa));
                energia = energia.Aplicar(Atividade.Caminhada, minutosDePausa);
                continue;
            }

            int bloco = Math.Min(ate, restante);
            plano.Add((Atividade.TrabalhoFocado, bloco));
            energia = energia.Aplicar(Atividade.TrabalhoFocado, bloco);
            restante -= bloco;
        }

        return plano;
    }
}
```

```cs Uso
foreach (var (atividade, minutos) in PlanejadorDePausas.Planejar(minutosDeFoco: 300))
    Console.WriteLine($"{atividade,-15} {minutos,3} min");
```

```saida
TrabalhoFocado  120 min
Caminhada        30 min
TrabalhoFocado   48 min
Caminhada        30 min
TrabalhoFocado   48 min
Caminhada        30 min
TrabalhoFocado   48 min
Caminhada        30 min
TrabalhoFocado   36 min
```

::: cuidado O plano "perfeito" é frágil
Veja o resultado: o primeiro bloco consome 60 pontos de energia, mas cada caminhada de 30 min devolve só 24, então os blocos seguintes encolhem para **48 minutos**, e o dia vira uma sequência de **4 caminhadas**. Um modelo realista teria mais nuance (recuperação que depende da qualidade do descanso, horário do dia, tipo de tarefa). **Reparar nessa fragilidade é o objetivo:** o código não decide sua agenda, ele ajuda a **discutir suposições**.
:::

```cs JornadaTests/Foco/Cap62/Cap62Tests.cs
using Jornada.Foco.Cap62;
using Xunit;

public class Cap62Tests
{
    [Fact]
    public void Energia_nunca_sai_de_0_a_100()
    {
        Assert.Equal(0, Energia.Cheia.Aplicar(Atividade.TrabalhoFocado, 1000).Valor);
        Assert.Equal(100, Energia.Cheia.Aplicar(Atividade.Caminhada, 1000).Valor);
    }

    [Fact]
    public void Caminhada_recupera_mais_que_pausa_na_tela()
    {
        var baixa = new Energia(30);
        Assert.True(baixa.Aplicar(Atividade.Caminhada, 10).Valor > baixa.Aplicar(Atividade.PausaNaTela, 10).Valor);
    }

    [Fact]
    public void Minutos_ate_o_limite()
    {
        Assert.Equal(120, Energia.Cheia.MinutosAteOLimite(Atividade.TrabalhoFocado, 40));
        Assert.Equal(0, new Energia(30).MinutosAteOLimite(Atividade.TrabalhoFocado, 40));
    }

    [Fact]
    public void Atividade_que_nao_gasta_energia_e_rejeitada() =>
        Assert.Throws<ArgumentException>(() => Energia.Cheia.MinutosAteOLimite(Atividade.Caminhada, 40));

    [Fact]
    public void Plano_soma_exatamente_o_foco_pedido()
    {
        var plano = PlanejadorDePausas.Planejar(300);
        Assert.Equal(300, plano.Where(p => p.Atividade == Atividade.TrabalhoFocado).Sum(p => p.Minutos));
    }
}
```

::: exercicio Pausa de qualidade
Hoje o planejador usa sempre **caminhada**. Crie uma versão que receba o tipo de pausa (`Atividade pausa`) e escreva um teste que mostre o que acontece com **`PausaNaTela`**: o plano **não consegue cumprir** os 300 minutos de foco.

--- resposta
```cs Jornada/Foco/Cap62/PlanejadorComPausa.cs
namespace Jornada.Foco.Cap62;

public static class PlanejadorComPausa
{
    public static IReadOnlyList<(Atividade Atividade, int Minutos)> Planejar(
        int minutosDeFoco, Atividade pausa, double limite = 40, int minutosDePausa = 30, int maximoDeEtapas = 50)
    {
        var plano = new List<(Atividade, int)>();
        var energia = Energia.Cheia;
        int restante = minutosDeFoco;

        while (restante > 0 && plano.Count < maximoDeEtapas)
        {
            int ate = energia.MinutosAteOLimite(Atividade.TrabalhoFocado, limite);
            if (ate == 0)
            {
                plano.Add((pausa, minutosDePausa));
                energia = energia.Aplicar(pausa, minutosDePausa);
                continue;
            }

            int bloco = Math.Min(ate, restante);
            plano.Add((Atividade.TrabalhoFocado, bloco));
            energia = energia.Aplicar(Atividade.TrabalhoFocado, bloco);
            restante -= bloco;
        }

        return plano;
    }
}
```
```cs Jornada/Foco/Cap62/PlanejadorComPausaTests.cs
using Jornada.Foco.Cap62;
using Xunit;

public class PlanejadorComPausaTests
{
    private static int Foco(IEnumerable<(Atividade Atividade, int Minutos)> plano) =>
        plano.Where(p => p.Atividade == Atividade.TrabalhoFocado).Sum(p => p.Minutos);

    [Fact]
    public void Caminhada_cumpre_o_foco_pedido() =>
        Assert.Equal(300, Foco(PlanejadorComPausa.Planejar(300, Atividade.Caminhada)));

    [Fact]
    public void Pausa_na_tela_nao_cumpre_o_foco_pedido() =>
        Assert.True(Foco(PlanejadorComPausa.Planejar(300, Atividade.PausaNaTela)) < 300);
}
```
O parâmetro `maximoDeEtapas` é a **guarda contra laço infinito**: com uma pausa que não restaura energia, o plano nunca avançaria. Quando a simulação "trava", o resultado é a informação: **essa pausa não sustenta esse ritmo**.
:::

::: resumo
- **Atenção dirigida cansa**; cansaço traz irritação, impulsividade e erros — **parar antes** é mais barato que insistir.
- Pausas diferem: **natureza, silêncio e caminhada** costumam restaurar mais que **olhar a tela** (segundo parte dos estudos).
- O modelo de energia do capítulo é **ilustrativo**: as taxas devem vir da **sua medição**; as réplicas sobre "esgotamento do ego" mostram que cabe cautela.
- O planejador de pausas serve para **discutir suposições**, não para ditar a agenda; observe sua fragilidade.
- Simulações precisam de **guardas** contra laços infinitos.
:::

::: quiz
? O que a fadiga de atenção significa no livro?
+ O esforço de inibir distrações se esgota com o uso, prejudicando o foco
- Falta de inteligência
- Vontade de dormir
- Sinal de que é hora de mais cafeína
> E traz mais impulsividade e erros.

? Qual pausa tende a restaurar mais a atenção, segundo a teoria dos Kaplan?
+ Contato com natureza ou ambiente tranquilo, que fascina sem esforço
- Navegar em redes sociais
- Outra reunião
- Trabalhar em outra tela
> A atenção sem esforço restaura.

? Por que o modelo de energia do capítulo deve ser usado com cuidado?
+ As taxas são ilustrativas; a realidade é mais irregular e varia de pessoa a pessoa
- Porque contém erros de sintaxe
- Porque a ciência provou que é falso
- Porque só vale para atletas
> Meça seus próprios dados.

? Por que os blocos do planejador encolhem depois da primeira pausa?
+ Porque cada caminhada devolve menos energia do que o bloco anterior gastou; o modelo revela essa fragilidade
- Porque o foco foi cancelado
- Porque a caminhada atrapalha
- Por erro de arredondamento
> O objetivo é discutir suposições.

? O que a réplica das pesquisas sobre "esgotamento do ego" sugere?
+ Cautela: o efeito pode ser menor do que se pensava, embora o cansaço ainda atrapalhe
- Que o autocontrole não existe
- Que cansaço não importa
- Que pausas são inúteis
> Menos certeza, mais observação.

? Para que serve a guarda no laço do planejador?
+ Evitar laço infinito se a pausa quase não restaurar energia
- Acelerar a simulação
- Substituir os testes
- Calcular a energia
> Um cuidado clássico em simulações.
:::

::: cartas
Atenção dirigida :: Foco mantido com esforço, que se esgota com o uso.
Atenção sem esforço :: Atenção atraída por algo fascinante, que restaura.
Restauração da atenção :: Teoria (dos Kaplan) de que ambientes naturais recuperam o foco.
Fadiga de atenção :: Queda de desempenho após esforço prolongado de inibir distrações.
Pausa restauradora :: Descanso que realmente recupera (caminhada, silêncio).
Limite de parada :: Nível de energia a partir do qual é hora de parar.
Modelo ilustrativo :: Simplificação cujos números devem ser medidos, não copiados.
Guarda de laço :: Contador que impede repetição infinita.
:::
