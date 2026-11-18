---
id: resiliencia-cap89
order: 115
kind: capitulo
num: 89
group: C# & Mentalidade · Parte 15 · Resiliência em C#
title: Emoções não são botões de desligar
subtitle: Perceber, nomear e dar tempo ao que sentimos antes de agir. Em C#, vamos modelar sentimentos com padrões de tupla, um relógio que podemos controlar nos testes e um diário que conta sem julgar.
minutes: 60
hue: 275
objectives:
- Entender que **regular emoções** é diferente de **reprimi-las**
- Usar **padrões em tupla** e **relacionais** no `switch` para nomear sentimentos com mais precisão
- Modelar uma **pausa** com `TimeSpan` e um relógio injetável (**`TimeProvider`**)
- Agrupar registros com **`GroupBy`** sem criar notas nem rankings
origem: livros/06-resiliencia-csharp/content/126-cap89.md
---

::: vila A mensagem escrita no calor do momento
Quando o colega alterou o código da Marta sem avisar, ela escreveu uma resposta enorme, cheia de maiúsculas, e já ia enviar. Parou, levantou, tomou água e voltou dez minutos depois. A mensagem enviada tinha três linhas: o fato, o efeito no trabalho e um pedido. Marta **continuava brava**. Só não precisou mandar a primeira versão da raiva.
:::

::: ilustracao img/resiliencia/cap89.svg | Entre o impulso e a ação existe um espaço. A pausa não apaga a emoção: abre tempo para escolher o que fazer com ela.
:::

## O que o livro diz

O texto sobre resiliência dedica atenção às **emoções** e ao **controle de impulsos**: sentir não é o problema; o que importa é como a pessoa **lida** com o que sente. A ideia central, dita em palavras nossas: **regular** uma emoção é perceber que ela está ali, **dar nome**, **dar tempo** e só então escolher o que fazer, sem exigir que ela desapareça.

::: cuidado Regular não é reprimir
**Reprimir** é fingir que nada está acontecendo: "não estou com raiva", "não é nada". **Regular** é reconhecer ("estou com raiva") e escolher a resposta. Tristeza, medo, raiva e cansaço **não são defeitos**. Quando duram muito ou pesam demais, conversar com alguém de confiança ou com um profissional de saúde é um passo válido.
:::

Palavras mais precisas abrem mais caminhos: "estou **irritada**" pede uma conversa; "estou **exausta**" pede descanso. É por isso que vamos construir um pequeno **vocabulário**.

## Da vida para o código

| Na vida | Em C# |
|---|---|
| Uma emoção com intensidade de 0 a 10 | `Sentimento(Emocao, Intensidade)` com **validação** |
| Palavras mais precisas conforme a intensidade | `switch` com **padrão em tupla** e **relacional** (`<= 3`) |
| Dar um tempo antes de decidir | `TimeSpan` e `PausaConsciente` |
| "Quanto tempo já passou?" | Relógio **injetado** (`TimeProvider`), que nos testes é falso e obediente |
| Perceber o que aparece mais, sem julgar | `GroupBy` + `ToDictionary` |

::: figura O espaço entre estímulo e resposta
<svg viewBox="0 0 740 200" role="img" aria-label="Um estímulo gera uma emoção; entre ela e a ação existe uma pausa em que se nomeia o que se sente e se escolhe a resposta">
  <rect x="20" y="70" width="130" height="60" rx="12" class="f-warn"/><text x="85" y="97" text-anchor="middle" font-weight="800">Algo acontece</text><text x="85" y="116" text-anchor="middle" font-size="12" class="t-dim">estímulo</text>
  <rect x="190" y="70" width="130" height="60" rx="12" class="f-bad"/><text x="255" y="97" text-anchor="middle" font-weight="800">Emoção surge</text><text x="255" y="116" text-anchor="middle" font-size="12" class="t-dim">raiva, medo...</text>
  <rect x="360" y="40" width="190" height="120" rx="14" class="f-info"/><text x="455" y="70" text-anchor="middle" font-weight="800">Pausa</text>
  <text x="455" y="96" text-anchor="middle" font-size="12" class="t-dim">respirar · nomear</text><text x="455" y="116" text-anchor="middle" font-size="12" class="t-dim">decidir quem ajudaria</text><text x="455" y="136" text-anchor="middle" font-size="12" class="t-dim">escolher a resposta</text>
  <rect x="590" y="70" width="130" height="60" rx="12" class="f-good"/><text x="655" y="97" text-anchor="middle" font-weight="800">Ação escolhida</text><text x="655" y="116" text-anchor="middle" font-size="12" class="t-dim">no seu tempo</text>
  <path class="arrow" d="M152 100H188"/><path class="arrow" d="M322 100H358"/><path class="arrow" d="M552 100H588"/>
  <text x="370" y="188" text-anchor="middle" font-size="13" class="t-dim">A emoção não some na pausa: o que muda é a pressa de agir.</text>
</svg>
:::

## 1) Sentimentos com nome e escala

Primeiro, o vocabulário. O `Sentimento` **valida** a intensidade no construtor: um valor fora de 0 a 10 não consegue existir. Isso se chama **invariante**: uma regra que o objeto garante durante toda a vida dele.

O método `Nomear` usa o recurso mais bonito do `switch` moderno: ele casa uma **tupla** `(emoção, intensidade)` com padrões, inclusive **relacionais** como `<= 3`. O `_` significa "qualquer valor".

```cs Jornada/Resiliencia/Cap89/Emocoes.cs
namespace Jornada.Resiliencia.Cap89;

public enum Emocao { Calma, Alegria, Tristeza, Raiva, Medo, Cansaco }

public sealed record Sentimento
{
    public Emocao Emocao { get; }
    public int Intensidade { get; }

    public Sentimento(Emocao emocao, int intensidade)
    {
        if (intensidade is < 0 or > 10)
            throw new ArgumentOutOfRangeException(nameof(intensidade), "Use um número de 0 a 10.");

        Emocao = emocao;
        Intensidade = intensidade;
    }
}

public static class Vocabulario
{
    /// <summary>Palavras mais precisas ajudam a perceber o que se sente. Não é diagnóstico.</summary>
    public static string Nomear(Sentimento s) => (s.Emocao, s.Intensidade) switch
    {
        (Emocao.Raiva, <= 3)    => "irritação",
        (Emocao.Raiva, <= 7)    => "raiva",
        (Emocao.Raiva, _)       => "raiva muito forte",
        (Emocao.Medo, <= 3)     => "receio",
        (Emocao.Medo, <= 7)     => "medo",
        (Emocao.Medo, _)        => "pavor",
        (Emocao.Tristeza, <= 3) => "desânimo",
        (Emocao.Tristeza, <= 7) => "tristeza",
        (Emocao.Tristeza, _)    => "tristeza profunda",
        (Emocao.Cansaco, <= 3)  => "cansaço leve",
        (Emocao.Cansaco, _)     => "exaustão",
        (Emocao.Alegria, _)     => "alegria",
        _                       => "calma"
    };
}

public static class SugestaoDePausa
{
    /// <summary>Quanto mais forte a emoção, mais folga antes de decidir. É uma sugestão, não uma regra.</summary>
    public static TimeSpan Para(Sentimento s) => s.Intensidade switch
    {
        <= 3 => TimeSpan.Zero,
        <= 6 => TimeSpan.FromMinutes(2),
        _    => TimeSpan.FromMinutes(10)
    };
}
```

Duas escolhas de cuidado aparecem aqui. A primeira: `Nomear` **não diagnostica**: é só uma palavra mais precisa para uma sensação (a documentação do método diz isso). A segunda: `SugestaoDePausa` devolve uma **sugestão**, não uma regra. O programa **sugere** e a pessoa **decide**.

::: dica Padrões de `switch` em ordem
O `switch` avalia os padrões **de cima para baixo** e para no primeiro que casa. Por isso `(Emocao.Raiva, <= 3)` vem antes de `(Emocao.Raiva, <= 7)`: um valor 2 casa com o primeiro e nem chega ao segundo. Se você inverter a ordem, o compilador reclama que o padrão de baixo ficou inalcançável.
:::

## 2) Uma pausa que o teste consegue controlar

Como testar "passaram dois minutos" sem esperar dois minutos de verdade? A resposta é uma técnica poderosa chamada **injeção de dependência**: em vez de a classe chamar `DateTime.Now` por conta própria, ela **recebe** um relógio pronto. No .NET 8, esse relógio é o **`TimeProvider`**. Em produção, usamos `TimeProvider.System`; nos testes, um relógio nosso, que avança quando mandamos.

```cs Jornada/Resiliencia/Cap89/PausaConsciente.cs
namespace Jornada.Resiliencia.Cap89;

public sealed class PausaConsciente
{
    private readonly TimeProvider relogio;
    private readonly TimeSpan duracao;
    private DateTimeOffset? inicio;

    public PausaConsciente(TimeProvider relogio, TimeSpan duracao)
    {
        this.relogio = relogio;
        this.duracao = duracao;
    }

    public void Iniciar() => inicio = relogio.GetUtcNow();

    public TimeSpan Restante
    {
        get
        {
            if (inicio is null) return duracao;
            var passou = relogio.GetUtcNow() - inicio.Value;
            return passou >= duracao ? TimeSpan.Zero : duracao - passou;
        }
    }

    public bool Terminou => inicio is not null && Restante == TimeSpan.Zero;
}
```

Veja o campo `DateTimeOffset? inicio`: o `?` indica que ele **pode ser nulo** (a pausa ainda não começou). O `Restante` trata os dois casos. Nada de `Thread.Sleep`: a classe só **consulta o relógio**.

E aqui está o relógio de teste, o ator coadjuvante que faz o tempo andar em milissegundos:

```cs Resiliencia.Testes/Cap89Tests.cs
using Jornada.Resiliencia.Cap89;

namespace Resiliencia.Testes;

public class RelogioDeTeste : TimeProvider
{
    private DateTimeOffset agora = new(2026, 11, 2, 9, 0, 0, TimeSpan.Zero);
    public override DateTimeOffset GetUtcNow() => agora;
    public void Avancar(TimeSpan tempo) => agora += tempo;
}
```

## 3) Um diário que conta, mas não julga

O diário guarda registros `(dia, sentimento)` e oferece duas leituras: a **linha do tempo** (ordenada por dia) e a **contagem** por emoção. Repare no que **não** existe aqui: nota, média, "emoção boa" ou "ruim", ranking de dias. Contar é um jeito de **perceber padrões**, nunca de pontuar uma pessoa.

```cs Jornada/Resiliencia/Cap89/DiarioDeEmocoes.cs
namespace Jornada.Resiliencia.Cap89;

public sealed class DiarioDeEmocoes
{
    private readonly List<(DateOnly Dia, Sentimento Sentimento)> registros = new();

    public void Registrar(DateOnly dia, Sentimento sentimento) => registros.Add((dia, sentimento));

    /// <summary>Quantas vezes cada emoção apareceu. Sem nota, sem "emoção boa" ou "ruim".</summary>
    public IReadOnlyDictionary<Emocao, int> Contagem() =>
        registros
            .GroupBy(r => r.Sentimento.Emocao)
            .ToDictionary(g => g.Key, g => g.Count());

    public IReadOnlyList<string> Linha() =>
        registros
            .OrderBy(r => r.Dia)
            .Select(r => $"{r.Dia:dd/MM}: {Vocabulario.Nomear(r.Sentimento)} ({r.Sentimento.Intensidade}/10)")
            .ToList();
}
```

Três recursos de LINQ trabalham juntos: `GroupBy` agrupa os registros por emoção; `ToDictionary` transforma cada grupo em um par *emoção → quantidade*; e `OrderBy` + `Select` montam a linha do tempo. Voltaremos ao LINQ com mais calma no próximo capítulo.

```cs Uso
var sentimento = new Sentimento(Emocao.Raiva, 8);
Console.WriteLine($"{sentimento.Emocao} {sentimento.Intensidade}/10 => {Vocabulario.Nomear(sentimento)}");

var pausa = new PausaConsciente(TimeProvider.System, SugestaoDePausa.Para(sentimento));
pausa.Iniciar();
Console.WriteLine($"Pausa sugerida: {pausa.Restante.TotalMinutes:0} min. Terminou? {pausa.Terminou}");

var diario = new DiarioDeEmocoes();
diario.Registrar(new DateOnly(2026, 11, 2), new Sentimento(Emocao.Raiva, 2));
diario.Registrar(new DateOnly(2026, 11, 3), new Sentimento(Emocao.Medo, 4));
diario.Registrar(new DateOnly(2026, 11, 4), new Sentimento(Emocao.Medo, 6));

foreach (var linha in diario.Linha())
    Console.WriteLine(linha);

foreach (var (emocao, vezes) in diario.Contagem().OrderBy(par => par.Key))
    Console.WriteLine($"{emocao}: {vezes}x");
```

```saida
Raiva 8/10 => raiva muito forte
Pausa sugerida: 10 min. Terminou? False
02/11: irritação (2/10)
03/11: medo (4/10)
04/11: medo (6/10)
Raiva: 1x
Medo: 2x
```

## 4) Testes

```cs Resiliencia.Testes/Cap89Tests.cs
public class Cap89Tests
{
    [Fact]
    public void Intensidade_fora_da_escala_e_recusada() =>
        Assert.Throws<ArgumentOutOfRangeException>(() => new Sentimento(Emocao.Raiva, 11));

    [Theory]
    [InlineData(Emocao.Raiva, 2, "irritação")]
    [InlineData(Emocao.Raiva, 9, "raiva muito forte")]
    [InlineData(Emocao.Medo, 5, "medo")]
    [InlineData(Emocao.Alegria, 8, "alegria")]
    [InlineData(Emocao.Cansaco, 9, "exaustão")]
    public void Nomear_usa_palavras_mais_precisas(Emocao e, int i, string esperado) =>
        Assert.Equal(esperado, Vocabulario.Nomear(new Sentimento(e, i)));

    [Theory]
    [InlineData(2, 0)]
    [InlineData(5, 2)]
    [InlineData(9, 10)]
    public void Pausa_cresce_com_a_intensidade(int intensidade, int minutos) =>
        Assert.Equal(TimeSpan.FromMinutes(minutos), SugestaoDePausa.Para(new Sentimento(Emocao.Raiva, intensidade)));

    [Fact]
    public void Pausa_termina_quando_o_tempo_passa()
    {
        var relogio = new RelogioDeTeste();
        var pausa = new PausaConsciente(relogio, TimeSpan.FromMinutes(2));

        Assert.False(pausa.Terminou);          // ainda nem começou
        pausa.Iniciar();
        relogio.Avancar(TimeSpan.FromSeconds(90));
        Assert.False(pausa.Terminou);
        Assert.Equal(TimeSpan.FromSeconds(30), pausa.Restante);

        relogio.Avancar(TimeSpan.FromSeconds(30));
        Assert.True(pausa.Terminou);
    }

    [Fact]
    public void Diario_conta_sem_julgar_e_mostra_a_linha_do_tempo()
    {
        var diario = new DiarioDeEmocoes();
        diario.Registrar(new DateOnly(2026, 11, 3), new Sentimento(Emocao.Medo, 4));
        diario.Registrar(new DateOnly(2026, 11, 2), new Sentimento(Emocao.Raiva, 2));
        diario.Registrar(new DateOnly(2026, 11, 4), new Sentimento(Emocao.Medo, 6));

        Assert.Equal(2, diario.Contagem()[Emocao.Medo]);
        Assert.Equal("02/11: irritação (2/10)", diario.Linha()[0]);
    }
}
```

O teste **`Pausa_termina_quando_o_tempo_passa`** é o coração do capítulo: avançamos o relógio de teste 90 segundos e verificamos que faltam 30; avançamos mais 30 e a pausa termina. **Zero espera real.** É isso que a injeção de dependência compra: tempo sob controle.

::: exercicio Um texto para a pausa
Escreva `TextoDaPausa.Descrever(PausaConsciente pausa)`: se a pausa terminou, devolve `"Pausa concluída. Você decide o que fazer agora."`; senão, `"Faltam 1 min e 30 s."` (com os minutos e segundos que faltam de verdade). Teste usando o `RelogioDeTeste`.

--- resposta
```cs Jornada/Resiliencia/Exercicios/Cap89Texto.cs
namespace Jornada.Resiliencia.Cap89;

public static class TextoDaPausa
{
    public static string Descrever(PausaConsciente pausa)
    {
        if (pausa.Terminou)
            return "Pausa concluída. Você decide o que fazer agora.";

        var restante = pausa.Restante;
        return $"Faltam {(int)restante.TotalMinutes} min e {restante.Seconds} s.";
    }
}
```

`(int)restante.TotalMinutes` pega só os **minutos inteiros** e `restante.Seconds` é a parte dos segundos (0 a 59), por isso 90 s viram "1 min e 30 s". Repare que a frase de fim **devolve o controle** à pessoa ("você decide"): o texto não manda voltar ao trabalho.
:::

::: resumo
- **Regular** emoções é perceber, nomear e dar tempo, **não** reprimir nem exigir que desapareçam.
- `Sentimento` **valida** a intensidade no construtor (invariante); `Nomear` usa **padrões em tupla e relacionais** no `switch`, avaliados de cima para baixo.
- Um relógio **injetado** (`TimeProvider`) deixa o tempo **testável**: nos testes, avançamos o relógio em vez de esperar.
- `GroupBy` + `ToDictionary` contam registros por categoria; o diário **não nota, não compara, não ranqueia**.
- Sugestões de pausa são **sugestões**: o programa informa, a pessoa decide.
:::

::: quiz
? Qual a diferença entre regular e reprimir uma emoção?
+ Regular é reconhecer e escolher a resposta; reprimir é fingir que ela não existe
- Regular é sentir menos; reprimir é sentir mais
- São a mesma coisa
- Reprimir é mais saudável quando há pressa
> Regular não exige que a emoção desapareça.

? Por que a classe `PausaConsciente` recebe um `TimeProvider` no construtor?
+ Para que os testes controlem o tempo sem esperar de verdade
- Porque `DateTime.Now` foi removido do C#
- Para gastar menos memória
- Porque o relógio do computador é impreciso
> Injetar a dependência deixa o tempo sob controle do teste.

? O que `(Emocao.Raiva, <= 3)` representa em um `switch`?
+ Um padrão em tupla com condição relacional: raiva com intensidade até 3
- Uma comparação entre dois objetos
- Um erro de sintaxe
- Um laço `for`
> O `switch` casa a tupla `(emoção, intensidade)` com cada padrão.

? Por que o `Sentimento` lança exceção para intensidade 11?
+ Para garantir uma invariante: nenhum objeto inválido consegue existir
- Para punir quem digita errado
- Porque o 11 é um número proibido
- Porque `record` não aceita números
> Validar no construtor protege o resto do código.

? O que o diário de emoções deliberadamente NÃO faz?
+ Dar nota, comparar dias ou classificar emoções como boas ou ruins
- Contar registros
- Ordenar por dia
- Nomear a emoção
> Contar ajuda a perceber padrões; pontuar pessoas é outra coisa.

? A pausa sugerida de 10 minutos para intensidade alta é...
+ Uma sugestão que a pessoa pode aceitar ou ignorar
- Uma regra obrigatória do programa
- Um bloqueio do sistema
- Uma punição
> O programa informa; a decisão continua sendo da pessoa.
:::

::: cartas
Regulação emocional :: Perceber e lidar com emoções sem exigir que desapareçam nem agir no automático.
Invariante :: Regra que um objeto garante durante toda a sua vida (ex.: intensidade de 0 a 10).
Padrão em tupla :: `switch` que casa vários valores de uma vez, como `(emoção, intensidade)`.
Padrão relacional :: Condição como `<= 3` dentro de um `switch` ou `is`.
TimeProvider :: Abstração de relógio do .NET 8; permite um relógio falso nos testes.
Injeção de dependência :: Receber de fora o que a classe precisa, em vez de criá-lo lá dentro.
GroupBy :: LINQ que agrupa elementos por uma chave.
ToDictionary :: LINQ que monta um dicionário chave → valor.
:::
