---
id: joao-cap131
order: 169
kind: capitulo
num: 131
group: C# & Mentalidade · Parte 18 · O Evangelho segundo João em C#
title: Lázaro, sai fora
subtitle: O chamado diante do sepulcro e a ordem "Desatai-o, e deixai-o ir". Em C#, um estado que muda em passos, e depois um registro dos fatos na ordem em que o texto os conta.
minutes: 75
hue: 206
objectives:
- Ler a cena de **Lázaro** (Jo 11,38-44) frase por frase
- Transformar o que acontece em um **estado** (`Morto` e `Vivo`) e em **ações** (`Chamar`, `Desatar`)
- Entender por que, em sistemas, vale **guardar os fatos** (o que aconteceu) e não só o estado final
- Saber o que o código **não** representa nesta cena
origem: livros/10-evangelho-joao-csharp/content/108-cap131.md
---

::: vila O relatório que mudou sem ninguém saber como
Um relatório amanheceu diferente, e ninguém sabia **quando, por quem ou por quê**. O banco guardava só o **estado de agora**: cada alteração sobrescrevia a anterior. Caio sugeriu o contrário: "guarde **só os fatos** (`criado`, `editado`, `arquivado`...) e **calcule** o estado a partir deles". Assim dava para ver o relatório como estava em qualquer minuto.
:::

::: ilustracao img/joao/cap131.svg | O estado é a soma dos eventos que o texto narra: o cursor percorre a história e o estado muda, mas nada do que aconteceu é apagado. O código não modela a ressurreição.
:::

## O texto

> **38** Comovendo-se pois Jesus outra vez em si mesmo, veio à sepultura; e era esta uma caverna, e estava uma pedra posta sobre ela. **39** Disse Jesus: Tirai a pedra. Marta, a irmã do morto, disse-lhe: Senhor, já cheira mal, porque já é de quatro dias. **40** Jesus disse-lhe: Não te disse, que se creres, verás a glória de Deus? **41** Tiraram, pois, a pedra de onde o morto jazia. E Jesus levantou os olhos para cima, e disse: Pai, graças te dou, porque me tens ouvido. **42** Porém eu bem sabia que sempre me ouves; mas por causa da multidão, que está ao redor, assim disse; para que creiam que tu me enviaste. **43** E havendo dito isto, clamou com grande voz: Lázaro, sai fora. **44** E o que estava morto saiu, com as mãos e os pés atados, e seu rosto envolto em um lenço. Disse-lhes Jesus: Desatai-o, e deixai-o ir.
>
> *João 11,38-44 (Bíblia Livre)*

## Lendo devagar

Esta é uma das cenas mais fortes do Evangelho. Lázaro, amigo de Jesus, morreu; "já havia quatro dias" que estava na sepultura (11,17.39). Em ordem:

- Jesus vai ao sepulcro e manda **tirar a pedra** (11,38-41).
- Ele **ora em voz alta** ao Pai (11,41-42).
- Então **chama com grande voz**: **"Lázaro, sai fora"** (11,43).
- "O que estava morto **saiu**" (11,44), **ainda envolto**: mãos e pés atados e um **lenço** no rosto.
- A última ordem é para os presentes: **"Desatai-o, e deixai-o ir"** (11,44).

A cena tem **dois momentos**: Lázaro **sai** (e ainda está atado), e **depois** é desatado. Só no fim ele está **livre**.

## Do versículo ao código

### Passo 1: "o que estava morto"

Lázaro está em uma de **duas condições**: morto ou vivo. Em C#, quando algo só pode estar em um número pequeno de situações, usamos um **`enum`**:

```cs
public enum Condicao { Morto, Vivo }
```

### Passo 2: "Lázaro, sai fora"

Um chamado **muda a condição**. É um **método**:

```cs
public Condicao Condicao { get; private set; } = Condicao.Morto;

public void Chamar() => Condicao = Condicao.Vivo;      // "Lázaro, sai fora."
```

O `private set` garante que **só a própria classe** muda a condição; quem está de fora só consegue **chamar**.

### Passo 3: "com as mãos e os pés atados, e seu rosto envolto em um lenço"

Lázaro sai com **três ataduras**. São uma **lista de textos**:

```cs
public List<string> Ataduras { get; } = new() { "atadura nas mãos", "atadura nos pés", "lenço no rosto" };
```

### Passo 4: "Desatai-o, e deixai-o ir"

Desatar é **tirar uma atadura da lista**; "deixai-o ir" é ele estar **vivo e sem nenhuma atadura**:

```cs
public bool Livre => Condicao == Condicao.Vivo && Ataduras.Count == 0;

public bool Desatar(string atadura) => Ataduras.Remove(atadura);   // "Desatai-o"
```

`Remove` devolve `true` se havia aquela atadura e `false` se não havia.

### O código completo, rodando

```cs Jornada/Joao/Texto/Cap131Lazaro.cs
namespace Jornada.Joao.Texto;

// Jo 11,43-44 (Bíblia Livre): "[...] clamou com grande voz: Lázaro, sai fora. E o que estava morto saiu, com as mãos e os pés atados,
//  e seu rosto envolto em um lenço. Disse-lhes Jesus: Desatai-o, e deixai-o ir."
//
// TEXTO     : Lázaro sai; ainda está atado; manda-se desatá-lo e deixá-lo ir.
// ANALOGIA  : a SEQUÊNCIA de estados (morto, vivo e atado, livre) e as ações que os mudam (chamar, desatar).
// LIMITE    : o código não modela a ressurreição, a fé nem o poder que o texto narra. Ele só dá forma à ordem dos acontecimentos.

public enum Condicao { Morto, Vivo }

// <lazaro>
public class Lazaro
{
    public Condicao Condicao { get; private set; } = Condicao.Morto;

    // "com as mãos e os pés atados, e seu rosto envolto em um lenço"
    public List<string> Ataduras { get; } = new() { "atadura nas mãos", "atadura nos pés", "lenço no rosto" };

    public bool Livre => Condicao == Condicao.Vivo && Ataduras.Count == 0;

    // "Lázaro, sai fora."
    public void Chamar() => Condicao = Condicao.Vivo;

    // "Desatai-o, e deixai-o ir."
    public bool Desatar(string atadura) => Ataduras.Remove(atadura);
}
// </lazaro>
```

```bash
dotnet run --project codigo/joao/Joao.Console -- exemplo t131
```

```cs Uso
var lazaro = new Lazaro();
Console.WriteLine($"Começa: {lazaro.Condicao}, ataduras: {lazaro.Ataduras.Count}");

lazaro.Chamar();                                   // "Lázaro, sai fora."
Console.WriteLine($"Chamado: {lazaro.Condicao}, livre? {lazaro.Livre}");

foreach (var atadura in lazaro.Ataduras.ToList())
{
    lazaro.Desatar(atadura);                       // "Desatai-o"
    Console.WriteLine($"Desatou: {atadura} (faltam {lazaro.Ataduras.Count})");
}

Console.WriteLine($"Livre? {lazaro.Livre}");        // "e deixai-o ir"
```

```saida
Começa: Morto, ataduras: 3
Chamado: Vivo, livre? False
Desatou: atadura nas mãos (faltam 2)
Desatou: atadura nos pés (faltam 1)
Desatou: lenço no rosto (faltam 0)
Livre? True
```

A saída conta a cena na ordem do texto: começa morto e atado; depois do chamado, **vivo, mas ainda não livre**; cada "desatar" tira uma atadura; no fim, **livre**. Ser livre exige as **duas coisas**: estar vivo **e** sem ataduras.

### Passo 5: o que esse código não guarda

Depois do último passo, só sobra o **resultado** ("livre"). Não dá para saber **em que ordem** as ataduras foram tiradas, nem **quando** ele foi chamado. Em muitos sistemas (contas, pedidos, prontuários), isso é pouco: é preciso saber **o que aconteceu**, e **quando**. A solução é guardar **cada fato** em uma lista que nunca se apaga e **calcular o estado** a partir deles. É o que as seções seguintes mostram, com os fatos **que o próprio texto narra**.

::: cuidado Limite da analogia
- **O código não representa a ressurreição**, nem a fé, nem o poder de que o texto fala. Ele só dá forma à **ordem** dos acontecimentos (morto, chamado, vivo e atado, livre).
- **Morto e vivo** são dois valores de um `enum`; o texto trata da morte e da vida com uma seriedade que um `enum` não tem. O cuidado aqui é **não confundir** o modelo com o relato.
- As **regras de ordem** do código (por exemplo, "só sai depois de chamado") são a ordem do **relato**; o código **não** afirma que um ato cause o outro.
- Se esta cena pesar, **pule o capítulo**: dá para ler os outros sem ele.
:::

::: curiosidade Lupa no grego
- **δακρύω** (*dakryō*, "chorar", G1145) aparece **uma única vez**, em 11,35: *edakrysen ho Iēsous*, "Jesus chorou" (SBLGNT), na mesma cena (`onde dakryō`).
- **Λάζαρος** (*Lazaros*) aparece **11 vezes**, em Jo 11 e 12; **Βηθανία** (*Bēthania*) aparece **4 vezes**: 1,28; 11,1.18; 12,1.
- **λύω** (*lyō*, "soltar, desatar", G3089) aparece **6 vezes** em João, com usos diferentes (1,27 "desatar a correia"; 2,19 "destruir"; 5,18 "quebrar"; 11,44 "desatar") (`onde lyō`). **ἀφίημι** (*aphiēmi*, "deixar, soltar, perdoar", G863) aparece **15 vezes** (4,28; 11,44; 20,23...) (`onde aphiēmi`).
- **κειρία** (*keiria*, "atadura, faixa", G2750) aparece **uma só vez** (11,44); **σουδάριον** (*soudarion*, "lenço", G4676) aparece **2 vezes**: 11,44 e 20,7 (`onde keiria`, `onde soudarion`).

*Fontes: texto grego SBLGNT (CC BY 4.0); contagens por lema do MorphGNT (CC BY-SA 3.0), reproduzíveis com `dotnet run --project codigo/joao/Joao.Console -- onde <palavra>`; sentidos do dicionário de Strong (OpenScriptures), com a entrada indicada.*
:::


## Indo além: guardar os fatos (*event sourcing*)

Em vez de guardar só o estado atual ("livre"), guardamos a **sequência de fatos**, que aqui são os que o texto narra: `Faleceu`, `PedraTirada`, `Chamado`, `Saiu`, `Desatado("lenço no rosto")`. O estado de qualquer momento é **calculado** reaplicando os fatos, e dá até para **voltar no tempo**. As seções abaixo constroem isso.

::: figura Estado = soma dos eventos
<svg viewBox="0 0 740 230" role="img" aria-label="Uma sequência de cinco eventos ao longo do tempo; o estado em cada instante é a soma dos eventos até ali">
  <line x1="40" y1="100" x2="700" y2="100" stroke="var(--ink-3)" stroke-width="3"/>
  <g font-size="12" text-anchor="middle">
    <circle cx="80" cy="100" r="13" class="f-info"/><text x="80" y="76" font-weight="800">Criado</text><text x="80" y="136" class="t-dim">09:00</text>
    <circle cx="210" cy="100" r="13" class="f-info"/><text x="210" y="76" font-weight="800">Editado</text><text x="210" y="136" class="t-dim">09:10</text>
    <circle cx="340" cy="100" r="13" class="f-bad"/><text x="340" y="76" font-weight="800">Arquivado</text><text x="340" y="136" class="t-dim">09:20</text>
    <circle cx="470" cy="100" r="13" class="f-warn"/><text x="470" y="76" font-weight="800">Chamado</text><text x="470" y="136" class="t-dim">09:40</text>
    <circle cx="600" cy="100" r="13" class="f-good"/><text x="600" y="76" font-weight="800">Desamarrado ×2</text><text x="600" y="136" class="t-dim">09:41</text>
  </g>
  <path d="M340 170V118" stroke="var(--primary)" stroke-width="2" stroke-dasharray="6 5"/>
  <rect x="236" y="170" width="208" height="46" rx="12" class="f-prim"/><text x="340" y="190" text-anchor="middle" font-size="13" font-weight="800">EstadoEm(09:25)</text><text x="340" y="207" text-anchor="middle" font-size="12">Arquivado, amarrado</text>
  <rect x="520" y="170" width="190" height="46" rx="12" class="f-good"/><text x="615" y="190" text-anchor="middle" font-size="13" font-weight="800">Estado()</text><text x="615" y="207" text-anchor="middle" font-size="12">Ativo e livre</text>
</svg>
:::

### 1) Eventos e estado

Em *event sourcing*, **o que se guarda são os fatos**, e não o estado. Para saber "como está agora", você **recalcula**: parte de um estado inicial e **aplica cada evento, em ordem**. Esse "dobrar uma lista em um valor" é exatamente o que `Aggregate` faz.

Os eventos são `record`s que herdam de um `abstract record Evento`, e todos têm o instante em que ocorreram. Eles são **imutáveis**: a história não se reescreve. Aqui, **os eventos são os que o texto narra**: `Faleceu` (11,14), `PedraTirada` (11,39.41), `Chamado` (11,43), `Saiu` (11,44) e `Desatado` (11,44). O `EstadoDeLazaro` tem um método `Aplicar(evento)` que usa uma **expressão `switch` sobre o tipo** do evento, com cláusulas **`when`** que decidem se a transição é possível naquele estado. Por exemplo, `Saiu` só vale depois de `Chamado`, e `Desatado` só vale para uma atadura que existe.

A propriedade `Livre` é "vivo, fora do sepulcro e sem ataduras": o final do relato.

O `LivroDeEventos` guarda a lista. O `Registrar` **valida antes de aceitar**: ele simula `Estado().Aplicar(evento)` e, se a ordem for impossível, o evento **nem entra** no livro. Também exige ordem cronológica.

```cs Jornada/Joao/Cap131/LivroDeEventos.cs
namespace Jornada.Joao.Cap131;

// Jo 11,14.17.39-44 (Bíblia Livre). Os eventos abaixo são os acontecimentos que o texto narra, na ordem em que ele os narra:
//  "Lázaro está morto" (14) · "Tirai a pedra" (39, 41) · "clamou com grande voz: Lázaro, sai fora" (43) ·
//  "o que estava morto saiu, com as mãos e os pés atados, e seu rosto envolto em um lenço" (44) · "Desatai-o, e deixai-o ir" (44).
//
// TEXTO       : narra uma sequência. Não explica COMO nem POR QUÊ; o poder que age ali não é objeto do código.
// ANALOGIA    : guardar os FATOS na ordem e calcular o estado a partir deles (event sourcing).
// LIMITE      : cada regra abaixo (por exemplo, "só sai depois de chamado") é a ORDEM do relato, e não uma explicação do que o texto narra.

/// <summary>Algo que aconteceu. Eventos nunca são apagados nem alterados: o estado é o que resulta deles.</summary>
public abstract record Evento(DateTimeOffset Quando);

public sealed record Faleceu(DateTimeOffset Quando) : Evento(Quando);                         // 11,14: "Lázaro está morto"
public sealed record PedraTirada(DateTimeOffset Quando) : Evento(Quando);                     // 11,39.41: "Tirai a pedra"
public sealed record Chamado(DateTimeOffset Quando) : Evento(Quando);                         // 11,43: "Lázaro, sai fora"
public sealed record Saiu(DateTimeOffset Quando) : Evento(Quando);                            // 11,44: "o que estava morto saiu"
public sealed record Desatado(DateTimeOffset Quando, string Atadura) : Evento(Quando);        // 11,44: "Desatai-o"

public enum Situacao { Vivo, Morto }

public sealed record EstadoDeLazaro(
    Situacao Situacao,
    bool SepulcroAberto,
    bool FoiChamado,
    bool JaSaiu,
    IReadOnlyList<string> Ataduras,
    int Versao)
{
    public static readonly EstadoDeLazaro Inicial = new(Situacao.Vivo, false, false, false, Array.Empty<string>(), 0);

    /// <summary>Livre = vivo, fora do sepulcro e sem nenhuma atadura: o que o final do relato descreve.</summary>
    public bool Livre => Situacao == Situacao.Vivo && JaSaiu && Ataduras.Count == 0;

    /// <summary>Calcula o estado seguinte. Um "switch" sobre o TIPO do evento decide o que muda; ordem impossível é recusada.</summary>
    public EstadoDeLazaro Aplicar(Evento evento)
    {
        var proximo = evento switch
        {
            Faleceu when Situacao == Situacao.Vivo && !JaSaiu =>
                this with { Situacao = Situacao.Morto },

            Faleceu => throw new InvalidOperationException("Só falece quem está vivo."),

            PedraTirada when Situacao == Situacao.Morto && !SepulcroAberto =>
                this with { SepulcroAberto = true },

            PedraTirada => throw new InvalidOperationException("Só se tira a pedra do sepulcro de quem faleceu, e uma vez."),

            Chamado when SepulcroAberto && !FoiChamado =>
                this with { FoiChamado = true },

            Chamado => throw new InvalidOperationException("O chamado vem depois de tirada a pedra, e uma vez."),

            Saiu when FoiChamado && !JaSaiu =>
                this with { Situacao = Situacao.Vivo, JaSaiu = true, Ataduras = new[] { "atadura nas mãos", "atadura nos pés", "lenço no rosto" } },

            Saiu => throw new InvalidOperationException("Só sai depois de chamado, e uma vez."),

            Desatado d when JaSaiu && Ataduras.Contains(d.Atadura) =>
                this with { Ataduras = Ataduras.Where(a => a != d.Atadura).ToList() },

            Desatado d => throw new InvalidOperationException($"Não há a atadura \"{d.Atadura}\" para desatar."),

            _ => throw new NotSupportedException($"Evento desconhecido: {evento.GetType().Name}.")
        };

        return proximo with { Versao = Versao + 1 };
    }
}

/// <summary>
/// O livro dos fatos. Esta versão guarda tudo na memória e foi pensada para uso em uma única thread:
/// serve para entender a ideia, não é um armazenamento de eventos para produção.
/// </summary>
public sealed class LivroDeEventos
{
    private readonly List<Evento> eventos = new();

    public IReadOnlyList<Evento> Eventos => eventos;

    public void Registrar(Evento evento)
    {
        if (eventos.Count > 0 && evento.Quando < eventos[^1].Quando)
            throw new InvalidOperationException("Os eventos precisam ser registrados em ordem de tempo.");

        // Valida aplicando sobre o estado atual: um evento fora de ordem nem chega a entrar no livro.
        _ = Estado().Aplicar(evento);
        eventos.Add(evento);
    }

    /// <summary>O estado de hoje: é só dobrar (Aggregate) todos os eventos, do início até agora.</summary>
    public EstadoDeLazaro Estado() => Reconstruir(eventos);

    /// <summary>Viagem no tempo: como estava em determinado momento?</summary>
    public EstadoDeLazaro EstadoEm(DateTimeOffset momento) =>
        Reconstruir(eventos.TakeWhile(e => e.Quando <= momento));

    public static EstadoDeLazaro Reconstruir(IEnumerable<Evento> eventos) =>
        eventos.Aggregate(EstadoDeLazaro.Inicial, (estado, evento) => estado.Aplicar(evento));
}
```

```cs Uso
var livro = new LivroDeEventos();
var inicio = new DateTimeOffset(2026, 11, 2, 9, 0, 0, TimeSpan.Zero);

livro.Registrar(new Faleceu(inicio));
livro.Registrar(new PedraTirada(inicio.AddMinutes(10)));
livro.Registrar(new Chamado(inicio.AddMinutes(20)));
livro.Registrar(new Saiu(inicio.AddMinutes(21)));

var estado = livro.Estado();
Console.WriteLine($"depois de sair: {estado.Situacao}; ataduras: {string.Join(" + ", estado.Ataduras)}; livre? {estado.Livre}");

foreach (var atadura in estado.Ataduras.ToList())
    livro.Registrar(new Desatado(inicio.AddMinutes(22), atadura));

Console.WriteLine($"depois de desatado: livre? {livro.Estado().Livre}");

var aos15 = livro.EstadoEm(inicio.AddMinutes(15));
Console.WriteLine($"como estava aos 15 minutos? {aos15.Situacao}; sepulcro aberto? {aos15.SepulcroAberto}; já saiu? {aos15.JaSaiu}");
Console.WriteLine($"{livro.Eventos.Count} eventos guardados; nenhum foi apagado");
```

```saida
depois de sair: Vivo; ataduras: atadura nas mãos + atadura nos pés + lenço no rosto; livre? False
depois de desatado: livre? True
como estava aos 15 minutos? Morto; sepulcro aberto? True; já saiu? False
7 eventos guardados; nenhum foi apagado
```

Veja os momentos. Depois de sair, Lázaro está **vivo, mas ainda atado** (as três ataduras, e `livre? False`); depois de desatadas todas, **livre**. Aos 15 minutos, o estado era `Morto`, com o sepulcro **aberto** e sem ter saído: a **viagem no tempo** é só reconstruir com os eventos até aquele instante (`TakeWhile`). No fim, os sete eventos continuam lá: **nenhum foi apagado**.

::: dica Quando event sourcing vale a pena
Vale quando a **história importa** (auditoria, finanças, saúde), quando é preciso **explicar como chegou ali** ou **desfazer**. Custa: mais espaço, consultas por reconstrução (por isso sistemas reais guardam "fotos" periódicas do estado) e a disciplina de **nunca alterar um evento passado**. Para um cadastro simples, um `UPDATE` comum basta.
:::

::: cuidado Limites desta implementação
- O `LivroDeEventos` guarda tudo **na memória** e foi pensado para **uma thread**: serve para entender a ideia, não é um armazenamento de eventos para produção.
- Cada regra de ordem (por exemplo, "só sai depois de chamado") é a **ordem do relato**, e não uma explicação do que o texto narra.
- Os eventos não têm um autor nem uma causa: o texto não os atribui a uma regra do sistema.
:::

### 2) Testes

```cs Joao.Testes/Cap131Tests.cs
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
```

O teste **`Reconstruir_do_zero_da_o_mesmo_estado_que_o_livro_ja_tinha`** é a promessa central: reaplicar os eventos sempre dá o mesmo estado. **`A_ordem_do_relato_e_exigida_nao_se_sai_sem_ser_chamado`** garante que a ordem do relato é respeitada. E **`Viagem_no_tempo_mostra_como_estava_em_cada_momento`** compara vários instantes.

Como `EstadoDeLazaro` contém uma lista, comparar dois estados com `Assert.Equal` compararia **referências** da lista; por isso o teste compara os campos e a lista elemento a elemento.

::: exercicio Contar a história
Escreva `Historia(this LivroDeEventos livro)`, que devolve uma linha por evento, numerada: `"1. faleceu"`, `"2. a pedra foi tirada"`, `"3. foi chamado"`... Use um `switch` sobre o tipo do evento.

--- resposta
```cs Jornada/Joao/Exercicios/Cap131Historia.cs
using Jornada.Joao.Cap131;

namespace Jornada.Joao.Exercicios;

public static class LivroExtensoes
{
    /// <summary>Conta o que aconteceu, um evento por linha: "1. faleceu", "2. a pedra foi tirada"...</summary>
    public static IReadOnlyList<string> Historia(this LivroDeEventos livro) =>
        livro.Eventos
            .Select((evento, i) => $"{i + 1}. " + evento switch
            {
                Faleceu     => "faleceu",
                PedraTirada => "a pedra foi tirada",
                Chamado     => "foi chamado",
                Saiu        => "saiu",
                Desatado d  => $"desatado: {d.Atadura}",
                _           => evento.GetType().Name
            })
            .ToList();
}
```

O `Select` com índice numera as linhas (`i + 1`), e o `switch` com **padrão de tipo** (`Desatado d`...) extrai o dado de cada evento. O caso `_` cobre eventos que ainda não existem, evitando uma exceção em tempo de execução quando alguém acrescentar um novo tipo.
:::

::: resumo
- Cada frase virou código: `Chamar()` ("Lázaro, sai fora"), uma lista de **ataduras** e `Desatar()` ("Desatai-o"). O código **não** modela a ressurreição.
- Ser **livre** exige **duas coisas**: estar vivo **e** sem ataduras (`Livre`).
- Em João, **edakrysen** (11,35) aparece **uma única vez**; **lyō** (6 vezes) tem usos diferentes e é o verbo de "desatai-o" em 11,44; **aphiēmi** (15 vezes) vai de "deixar" a "perdoar".
- **Event sourcing**: guarde os **fatos** (imutáveis, em ordem) e **calcule** o estado a partir deles.
- `abstract record` + `switch` por tipo + cláusulas `when` formam um bom modelo de transições; os eventos aqui são os do relato.
- **`Aggregate`** dobra uma lista de eventos em um estado; `TakeWhile` + `Aggregate` = **viagem no tempo**.
- Valide **antes** de gravar: um evento impossível nem chega ao livro.
:::

::: quiz
? Por que Lázaro "ainda não é livre" logo depois de `Chamar()`?
+ Porque está vivo, mas ainda tem as três ataduras
- Porque o código tem erro
- Porque `Chamar` não faz nada
- Porque é um `enum`
> `Livre` exige condição viva e lista de ataduras vazia (11,44).

? O que o código representa da cena de Lázaro?
+ A ordem dos acontecimentos e os estados; não a ressurreição nem o poder de que o texto fala
- A ressurreição inteira
- A fé de Marta
- O poder de Jesus
> O texto narra uma sequência; o código só dá forma a ela.

? Quantas vezes aparece o verbo *dakryō* ("chorar") em João?
+ Uma só, em 11,35
- 11
- Nenhuma
- Em todo capítulo 11
> A frase tem só dois vocábulos no grego.

? O que há de especial no verbo *aphiēmi*?
+ Aparece 15 vezes e vai de "deixar" a "soltar" e "perdoar"
- É usado só em 11,44
- Significa "chorar"
- Não existe em João
> Em 4,28 é "deixar" o jarro; em 11,44, "deixar ir".

? O que o event sourcing guarda?
+ Os fatos ocorridos, em ordem e imutáveis
- Só o estado atual
- Apenas os erros
- Só os últimos dez eventos
> O estado é calculado a partir deles.

? Qual método do LINQ dobra uma sequência de eventos em um estado?
+ `Aggregate`
- `Where`
- `Select`
- `Count`
> Parte do estado vazio e aplica cada evento.

? O que faz `EstadoEm(momento)`?
+ Reconstrói o estado só com os eventos até aquele momento
- Apaga os eventos mais novos
- Muda a hora do sistema
- Cria um evento novo
> Viagem no tempo com `TakeWhile`.

? Por que `Registrar` aplica o evento antes de guardá-lo?
+ Para recusar eventos impossíveis e manter a história coerente
- Para deixar o código mais lento
- Para duplicar o evento
- Para trocar o tipo
> `Estado().Aplicar(evento)` lança exceção se a transição for inválida.
:::

::: cartas
Desatai-o (Jo 11,44) :: Método `Desatar`: tira uma atadura da lista.
Lázaro (Jo 11) :: Narrativa: a morte, a pedra tirada, o chamado, a saída e o "Desatai-o".
Edakrysen :: "Chorou" (11,35): única ocorrência do verbo no Evangelho.
Lyō :: "Soltar, desatar": 6 vezes em João, com sentidos distintos.
Aphiēmi :: "Deixar, soltar, perdoar": 15 vezes em João.
Event sourcing :: Guardar os fatos e calcular o estado a partir deles.
abstract record :: Registro-base imutável de que os eventos herdam.
Aggregate :: Dobra uma sequência em um único valor, aplicando um passo de cada vez.
Viagem no tempo :: Reconstruir o estado até um instante, usando só os eventos anteriores.
:::
