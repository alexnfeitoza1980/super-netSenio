---
id: amor-cap115
order: 147
kind: capitulo
num: 115
group: C# & Mentalidade · Parte 17 · Você é aquilo que ama, em C#
title: Tudo importa: o trabalho como liturgia
subtitle: O que fazemos todos os dias (inclusive o trabalho) nasce do que amamos e, ao mesmo tempo, o forma. Em C#, é o capítulo do pipeline: etapas como funções assíncronas encadeadas, com closures e funções locais, como um ritual de entrega.
minutes: 80
hue: 245
objectives:
- Entender a ideia de que **todo trabalho** expressa e forma o que amamos (a "vocação" como prática)
- Perceber que o que **criamos** é movido por **desejos inconscientes**, e por que isso pede atenção aos nossos rituais de trabalho
- Montar um **pipeline** (middleware) com **delegates assíncronos**, **closures** e **funções locais**
- Usar **fábricas de etapas** e **etapas "em volta"** (antes e depois do que vem pela frente)
origem: livros/08-voce-e-aquilo-que-ama-csharp/content/135-cap115.md
---

::: vila A checagem das nove horas
Todo dia, Caio chegava ao escritório e fazia, sem pensar, o mesmo: abria o e-mail, depois três redes, depois o chat. Quando enfim abria o código, já eram 9h40 e a cabeça estava em outro lugar. Lia sugeriu um experimento: trocar a **primeira ação do dia** por outra (abrir o repositório, ler a lista do que ficou pendente) e só então olhar o resto. Em uma semana, Caio notou algo estranho: o resto do dia **começava diferente**. "Não é que eu tenha ficado mais disciplinado", disse, "é que a primeira coisa **me puxa** para as seguintes".
:::

::: ilustracao img/amor/cap115.svg | Uma esteira de etapas: cada ritual de entrega passa por portas que só abrem se a anterior aprovou. A ordem é o hábito.
:::

## O que o livro diz

No último capítulo, Smith amplia o foco de **onde** praticamos para **o que fazemos com a vida**: o trabalho, a criação, a **vocação**. Três ideias se destacam.

1. **"Tudo importa."** Para ele, uma visão que leva a sério o mundo **inteiro** (da física ao direito, da agricultura às artes) vê qualquer trabalho como parte do que importa, e **não** como um "território secular" separado da vida espiritual. Ninguém deveria ser "um profissional que por acaso tem fé"; o objetivo é que a visão de vida **atravesse** o ofício.
2. **O que criamos nasce do que amamos, em grande parte sem perceber.** Ele conta uma conversa de bastidores entre o criador de uma célebre saga de cinema e um colaborador: o colaborador propõe que um personagem importante morra; o criador **recusa, quase sem conseguir explicar**. O autor comenta que as decisões criativas são movidas por **crenças e desejos que operam abaixo da consciência**: a obra pode acabar dizendo algo **mais complicado** do que a intenção declarada. E conclui: por isso precisamos **cuidar da formação do que está abaixo da superfície**.
3. **Bom design diz a verdade.** Citando um teórico do design, ele afirma que "uma enxada bem projetada fala a verdade ao solo que rompe e fala-nos a verdade sobre o solo": o **bom projeto** respeita a natureza do material e do problema. Fazer cultura (um software, uma lei, um objeto) é **desenhar** o mundo, e todo desenho, por sua vez, **forma** quem convive com ele.

Por fim, ele propõe pensar nos **rituais do trabalho** como **liturgias vocacionais**. Com humor, observa que, se antropólogos de outro planeta visitassem nossos escritórios, veriam nossa **postura curvada sobre o celular** pela manhã como uma espécie de devoção a um "talismã eletrônico". E pergunta: *e se esses rituais, que "apenas fazemos", estiverem **nos fazendo**?*

::: cuidado Entre a fé do autor e o seu ofício
Para Smith, essas práticas vocacionais se ancoram em uma fé específica; não é o que nos cabe discutir aqui. Também **não queremos dizer** que quem pensa diferente trabalha sem sentido ou que a produtividade seja um dever moral. O que aproveitamos é a **pergunta**: *que hábitos o meu trabalho está me formando, e eu escolhi isso?*
:::

::: analogia O primeiro tijolo
O primeiro tijolo de uma parede **não é só mais um tijolo**: é ele que define o prumo de todos os outros. O primeiro hábito do dia faz o mesmo: não é o mais importante, e sim o que **orienta** os que vêm depois.
:::

## Da vida para o código

| No livro | Em C# |
|---|---|
| Um ritual de trabalho com etapas em ordem | **`Pipeline`**: lista de `Etapa` executadas em sequência |
| Cada etapa decide se o ritual continua | A etapa chama (ou não) **`await proximo()`** |
| Uma etapa pode falhar e interromper | `Interromper(motivo)`, sem chamar `proximo` |
| Uma etapa "em volta" (antes e depois) | `Etapas.Medir`: roda código **antes e depois** de `await proximo()` |
| Os rituais que repetimos nos formam | A **ordem** das etapas é o hábito da equipe |
| Fabricar rituais a partir de regras | **Fábrica de etapas**: `Etapas.Verificar(nome, regra, motivo)` |
| Cada entrega é um caso | `ContextoDeEntrega` por execução |

::: figura O pipeline como esteira
<svg viewBox="0 0 740 200" role="img" aria-label="Uma mudança de código passa por cinco etapas em sequência: mensagem clara, compilar, testar, revisar, publicar; se uma falhar, as seguintes não rodam">
  <g font-size="12" text-anchor="middle" font-weight="800">
    <rect x="20" y="60" width="120" height="60" rx="12" class="f-good"/><text x="80" y="95">mensagem clara</text>
    <rect x="160" y="60" width="120" height="60" rx="12" class="f-good"/><text x="220" y="95">compilar</text>
    <rect x="300" y="60" width="120" height="60" rx="12" class="f-bad"/><text x="360" y="88">testar</text><text x="360" y="106" font-size="10" font-weight="400" class="t-dim">falhou aqui</text>
    <rect x="440" y="60" width="120" height="60" rx="12" class="f-info" opacity=".45"/><text x="500" y="95">revisar</text>
    <rect x="580" y="60" width="120" height="60" rx="12" class="f-info" opacity=".45"/><text x="640" y="95">publicar</text>
  </g>
  <path class="arrow" d="M142 90H158"/><path class="arrow" d="M282 90H298"/>
  <path d="M422 90H438" stroke="var(--ink-3)" stroke-width="2" stroke-dasharray="3 4"/><path d="M562 90H578" stroke="var(--ink-3)" stroke-width="2" stroke-dasharray="3 4"/>
  <text x="500" y="146" text-anchor="middle" font-size="12" class="t-dim">não chegam a rodar: quem não chamou "próximo" interrompe a esteira</text>
  <text x="370" y="184" text-anchor="middle" font-size="13" class="t-dim">A ordem é o hábito; o cuidado é não pular nada quando for difícil.</text>
</svg>
:::

## 1) Funções que chamam a próxima função

Você já usou delegates (`Func<...>`). Aqui vamos mais longe: **funções que recebem a próxima função como argumento**. Cada etapa do ritual de entrega recebe duas coisas: o **contexto** (a mudança em análise, o registro do que foi feito) e um **`Proximo`**, uma função que, se chamada, **continua a esteira**.

```cs As duas assinaturas (delegates nomeados)
public delegate Task Proximo();
public delegate Task Etapa(ContextoDeEntrega contexto, Proximo proximo);
```

Isso é o **padrão de pipeline** (ou "middleware"), o mesmo desenho que você verá quando estudar **ASP.NET**, em que cada peça decide **se**, **quando** e **como** a próxima roda. Uma etapa pode:

- **seguir em frente** (`await proximo()`), depois de fazer sua parte;
- **parar a esteira**, simplesmente **não chamando** `proximo`;
- **ficar "em volta"**: fazer algo **antes**, chamar `proximo`, e fazer algo **depois** (como um cronômetro).

```cs Jornada/Habitos/Cap115/Pipeline.cs
namespace Jornada.Habitos.Cap115;

/// <summary>"Chame a próxima etapa". Uma etapa decide se, quando e como a cadeia continua.</summary>
public delegate Task Proximo();

/// <summary>Uma etapa do ritual de entrega: recebe o contexto e a próxima etapa.</summary>
public delegate Task Etapa(ContextoDeEntrega contexto, Proximo proximo);

public sealed class ContextoDeEntrega
{
    private readonly List<string> registro = new();

    public ContextoDeEntrega(string mudanca) => Mudanca = mudanca;

    public string Mudanca { get; }

    public IReadOnlyList<string> Registro => registro;

    public bool Interrompido { get; private set; }

    public string? Motivo { get; private set; }

    public void Anotar(string linha) => registro.Add(linha);

    public void Interromper(string motivo)
    {
        Interrompido = true;
        Motivo = motivo;
        registro.Add($"✗ {motivo}");
    }
}

/// <summary>
/// Uma esteira de etapas (o mesmo desenho dos "middlewares" que você verá em ASP.NET). Cada etapa é uma função.
/// A ordem forma o hábito da equipe: o que sempre vem antes, o que nunca se pula.
/// </summary>
public sealed class Pipeline
{
    private readonly List<Etapa> etapas = new();

    public Pipeline Usar(Etapa etapa)
    {
        etapas.Add(etapa);
        return this;
    }

    public async Task ExecutarAsync(ContextoDeEntrega contexto)
    {
        // Monta a cadeia de trás para a frente: cada "Proximo" chama a etapa seguinte.
        Proximo Montar(int indice) =>
            indice == etapas.Count
                ? () => Task.CompletedTask
                : () => etapas[indice](contexto, Montar(indice + 1));

        await Montar(0)();
    }
}

public static class Etapas
{
    /// <summary>Fábrica de etapas: cria uma etapa que roda uma verificação e interrompe se ela falhar.</summary>
    public static Etapa Verificar(string nome, Func<ContextoDeEntrega, Task<bool>> verificacao, string motivoSeFalhar) =>
        async (contexto, proximo) =>
        {
            if (await verificacao(contexto))
            {
                contexto.Anotar($"✓ {nome}");
                await proximo();                        // só continua se esta etapa aprovou
            }
            else
            {
                contexto.Interromper($"{nome}: {motivoSeFalhar}");
            }
        };

    /// <summary>Uma etapa "em volta": roda antes e depois do que vem pela frente (como um cronômetro).</summary>
    public static Etapa Medir(Action<string> aoFinal) =>
        async (contexto, proximo) =>
        {
            var inicio = System.Diagnostics.Stopwatch.GetTimestamp();
            await proximo();
            var gasto = System.Diagnostics.Stopwatch.GetElapsedTime(inicio);
            aoFinal($"Entrega de \"{contexto.Mudanca}\" levou {gasto.TotalMilliseconds:0} ms");
        };
}
```

O coração é o **`Montar`**, uma **função local** recursiva que constrói a cadeia **de trás para frente**:

```cs Só o essencial
Proximo Montar(int indice) =>
    indice == etapas.Count
        ? () => Task.CompletedTask                          // fim da esteira: nada a fazer
        : () => etapas[indice](contexto, Montar(indice + 1)); // roda a etapa, entregando a próxima
```

Cada `Proximo` é uma **closure**: uma função que **"fecha" em torno** de variáveis que existiam quando foi criada (aqui, `indice`, `contexto` e `etapas`) e as leva consigo. É isso que permite a cada etapa chamar `proximo()` **sem saber** quem vem depois.

E duas **fábricas de etapas**:

- **`Verificar(nome, verificacao, motivoSeFalhar)`** devolve uma `Etapa` pronta: roda uma checagem assíncrona; se passou, **anota** e chama `proximo`; se não, **interrompe**, com o motivo. É uma função que **devolve uma lambda `async`**.
- **`Medir(aoFinal)`** devolve uma etapa que **cronometra tudo o que vem depois**, usando `Stopwatch.GetTimestamp` antes e `GetElapsedTime` depois de `await proximo()`.

```cs Uso
async Task Entregar(string mudanca, bool testesPassam)
{
    var contexto = new ContextoDeEntrega(mudanca);

    var pipeline = new Pipeline()
        .Usar(Etapas.Verificar("mensagem clara", c => Task.FromResult(c.Mudanca.Length >= 10), "descreva melhor a mudança"))
        .Usar(Etapas.Verificar("compilar", _ => Task.FromResult(true), "não compilou"))
        .Usar(Etapas.Verificar("testar", _ => Task.FromResult(testesPassam), "há testes falhando"))
        .Usar(Etapas.Verificar("revisar por um par", _ => Task.FromResult(true), "sem revisão"))
        .Usar(Etapas.Verificar("publicar", _ => Task.FromResult(true), "falha ao publicar"));

    await pipeline.ExecutarAsync(contexto);

    Console.WriteLine($"{mudanca}:");
    foreach (var linha in contexto.Registro)
        Console.WriteLine("  " + linha);
}

await Entregar("corrige arredondamento do frete", testesPassam: true);
await Entregar("ajuste rápido de juros", testesPassam: false);
```

```saida
corrige arredondamento do frete:
  ✓ mensagem clara
  ✓ compilar
  ✓ testar
  ✓ revisar por um par
  ✓ publicar
ajuste rápido de juros:
  ✓ mensagem clara
  ✓ compilar
  ✗ testar: há testes falhando
```

A primeira entrega passou por **todas** as etapas. A segunda foi **interrompida em "testar"**: "revisar" e "publicar" **nem começaram**. Note o que o ritual **ensina**: ninguém precisa decidir, na sexta à tarde, se vale a pena pular os testes. A esteira **já decidiu**, de antemão, quando a cabeça estava fria.

::: dica O ritual protege a gente de si mesmo
Esse é, talvez, o melhor uso das práticas formadoras: **decidir antes**, em um momento calmo, o que **não** faremos sob pressão. Um pipeline bem desenhado é uma pequena liturgia da qualidade: repete o que **queremos** que o time faça, mesmo nos dias em que ninguém está com vontade.
:::

## 2) Testes

```cs Habitos.Testes/Cap115Tests.cs
using Jornada.Habitos.Cap115;

namespace Habitos.Testes;

public class Cap115Tests
{
    private static Etapa Passa(string nome) => Etapas.Verificar(nome, _ => Task.FromResult(true), "falhou");
    private static Etapa Falha(string nome) => Etapas.Verificar(nome, _ => Task.FromResult(false), "não passou");

    [Fact]
    public async Task As_etapas_rodam_na_ordem_em_que_foram_registradas()
    {
        var contexto = new ContextoDeEntrega("corrige cálculo de juros");
        var pipeline = new Pipeline().Usar(Passa("compilar")).Usar(Passa("testar")).Usar(Passa("revisar")).Usar(Passa("publicar"));

        await pipeline.ExecutarAsync(contexto);

        Assert.Equal(new[] { "✓ compilar", "✓ testar", "✓ revisar", "✓ publicar" }, contexto.Registro);
        Assert.False(contexto.Interrompido);
    }

    [Fact]
    public async Task Quando_uma_etapa_falha_as_seguintes_nem_comecam()
    {
        var contexto = new ContextoDeEntrega("x");
        var pipeline = new Pipeline().Usar(Passa("compilar")).Usar(Falha("testar")).Usar(Passa("publicar"));

        await pipeline.ExecutarAsync(contexto);

        Assert.True(contexto.Interrompido);
        Assert.Equal("testar: não passou", contexto.Motivo);
        Assert.Equal(new[] { "✓ compilar", "✗ testar: não passou" }, contexto.Registro);
    }

    [Fact]
    public async Task Pipeline_vazio_nao_faz_nada_e_nao_quebra()
    {
        var contexto = new ContextoDeEntrega("x");
        await new Pipeline().ExecutarAsync(contexto);
        Assert.Empty(contexto.Registro);
    }

    [Fact]
    public async Task Uma_etapa_em_volta_roda_antes_e_depois_do_resto()
    {
        var ordem = new List<string>();
        var pipeline = new Pipeline()
            .Usar(async (ctx, proximo) => { ordem.Add("antes"); await proximo(); ordem.Add("depois"); })
            .Usar(async (ctx, proximo) => { ordem.Add("meio"); await proximo(); });

        await pipeline.ExecutarAsync(new ContextoDeEntrega("x"));

        Assert.Equal(new[] { "antes", "meio", "depois" }, ordem);
    }

    [Fact]
    public async Task A_etapa_de_medicao_envolve_todas_as_outras()
    {
        string? relatorio = null;
        var pipeline = new Pipeline()
            .Usar(Etapas.Medir(texto => relatorio = texto))
            .Usar(async (ctx, proximo) => { await Task.Delay(20); await proximo(); });

        await pipeline.ExecutarAsync(new ContextoDeEntrega("ajuste de layout"));

        Assert.NotNull(relatorio);
        Assert.StartsWith("Entrega de \"ajuste de layout\" levou", relatorio);
    }

    [Fact]
    public async Task Cada_execucao_tem_o_seu_proprio_contexto()
    {
        var pipeline = new Pipeline().Usar(Passa("a"));
        var c1 = new ContextoDeEntrega("um");
        var c2 = new ContextoDeEntrega("dois");

        await pipeline.ExecutarAsync(c1);
        await pipeline.ExecutarAsync(c2);

        Assert.Single(c1.Registro);
        Assert.Single(c2.Registro);
    }

    [Fact]
    public async Task A_verificacao_pode_olhar_o_contexto()
    {
        var etapa = Etapas.Verificar("mensagem clara",
            ctx => Task.FromResult(ctx.Mudanca.Length >= 10), "descreva melhor a mudança");

        var curta = new ContextoDeEntrega("fix");
        await new Pipeline().Usar(etapa).ExecutarAsync(curta);
        Assert.True(curta.Interrompido);

        var boa = new ContextoDeEntrega("corrige arredondamento do frete");
        await new Pipeline().Usar(etapa).ExecutarAsync(boa);
        Assert.False(boa.Interrompido);
    }
}
```

Veja `Uma_etapa_em_volta_roda_antes_e_depois_do_resto`: a ordem registrada (`antes`, `meio`, `depois`) mostra que `await proximo()` **passa a vez** e **volta**. É assim que cronômetros, *logs* e tratamento de erros costumam ser feitos.

::: exercicio Uma etapa condicional
Nem toda etapa deve rodar sempre (por exemplo, "publicar" só em *release*). Escreva `EtapasCondicionais.Quando(Func<ContextoDeEntrega, bool> condicao, Etapa etapa)`, que devolve uma `Etapa` que **só roda a etapa dada** quando a condição for verdadeira; senão, **passa direto** para a próxima, **sem interromper** nada. Teste: com a condição falsa, a etapa pulada não anota nada e a seguinte roda.

--- resposta
```cs Jornada/Habitos/Exercicios/Cap115Quando.cs
namespace Jornada.Habitos.Cap115;

public static class EtapasCondicionais
{
    /// <summary>Roda a etapa só quando a condição vale; senão, passa direto para a próxima, sem interromper nada.</summary>
    public static Etapa Quando(Func<ContextoDeEntrega, bool> condicao, Etapa etapa) =>
        (contexto, proximo) => condicao(contexto) ? etapa(contexto, proximo) : proximo();
}
```

A solução é uma **função que devolve uma função**: recebe uma condição e uma etapa, e devolve uma **nova etapa** que decide, em cada execução, entre rodá-la ou chamar `proximo()` direto. Uma etapa também é um **valor**: pode ser guardada, passada, composta e **embrulhada** por outra. É esse o poder dos delegates, e a razão de o mesmo desenho servir tanto para um pipeline de entrega quanto para um servidor web.
:::

::: resumo
- Para o autor, **tudo importa**: o trabalho não é um território à parte, e o que **criamos** nasce do que **amamos**, muitas vezes sem percebermos, por isso vale cuidar do que nos forma por baixo.
- **Bom design diz a verdade** ao material e ao problema; fazer cultura (software, leis, objetos) **forma** quem convive com ela.
- Os rituais do trabalho (a primeira ação do dia, o fluxo de entrega) são **liturgias vocacionais**: perguntar o que formam é o primeiro passo para escolhê-los.
- Um **pipeline** encadeia **etapas** (delegates `async`); cada etapa decide se chama `proximo`; **closures** e **funções locais** montam a cadeia.
- **Decidir antes**, com a cabeça fria, o que não se fará sob pressão é um uso sábio de rituais.
:::

::: quiz
? O que quer dizer "tudo importa", no último capítulo?
+ Que qualquer trabalho faz parte de uma visão de vida inteira, e não de um território à parte
- Que tudo é igualmente urgente
- Que nada pode ser descartado
- Que só o trabalho importa
> O ofício é parte da vida, e não uma área separada.

? O que a conversa de bastidores sobre o roteiro do filme ilustra?
+ Que as decisões criativas são movidas por crenças e desejos que operam abaixo da consciência
- Que filmes devem ter finais felizes
- Que roteiristas não entendem de histórias
- Que a criatividade é aleatória
> A obra pode dizer algo mais complicado do que a intenção declarada.

? O que faz uma `Etapa` do pipeline quando NÃO chama `proximo()`?
+ Interrompe a esteira: as etapas seguintes não rodam
- Repete a etapa atual
- Pula apenas a etapa seguinte
- Gera um erro automaticamente
> Chamar (ou não) `proximo` é o controle da esteira.

? O que é uma closure, no `Montar`?
+ Uma função que carrega consigo as variáveis do contexto em que foi criada
- Uma função que fecha o programa
- Um tipo de loop
- Uma classe selada
> Cada `Proximo` "lembra" de `indice`, `contexto` e `etapas`.

? O que a etapa `Medir` faz, e por que isso só é possível em um pipeline?
+ Roda código antes e depois de `await proximo()`, envolvendo o resto da esteira
- Mede apenas a primeira etapa
- Substitui as outras
- Roda em paralelo
> `await proximo()` passa a vez e volta, permitindo "envolver".

? Qual é o benefício de "decidir antes" no pipeline?
+ Quando a cabeça está fria, define-se o que não se fará sob pressão
- Evitar escrever testes
- Aumentar o número de reuniões
- Dispensar revisões
> Rituais protegem a gente dos dias em que ninguém está com vontade.
:::

::: cartas
Tudo importa :: Visão de que todo trabalho faz parte de uma vida inteira, e não de um território à parte.
Liturgia vocacional :: Ritual do trabalho que, repetido, forma o que amamos e como fazemos.
Pipeline (middleware) :: Cadeia de etapas, cada uma decidindo se chama a próxima.
Etapa :: Função que recebe o contexto e a próxima etapa.
Closure :: Função que carrega as variáveis do contexto em que foi criada.
Função local :: Função declarada dentro de outro método.
Fábrica de etapas :: Método que devolve uma etapa pronta a partir de parâmetros.
Decidir antes :: Escolher com calma o que se fará (ou não) sob pressão.
:::
