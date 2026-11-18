---
id: joao-cap133
order: 171
kind: capitulo
num: 133
group: C# & Mentalidade · Parte 18 · O Evangelho segundo João em C#
title: A videira e os ramos
subtitle: "Estai em mim, e eu em vós": o ramo que só dá fruto ligado à videira. Em C#, é uma classe que não fabrica o que precisa: recebe.
minutes: 80
hue: 202
objectives:
- Ler o discurso da **videira** (Jo 15,1-8) frase por frase
- Transformar "**estai em mim**" em um **construtor** que recebe a videira e "**sem mim nada podeis fazer**" em uma **verificação**
- Entender a **injeção de dependência** e, depois, um **recipiente** que liga as peças sozinho
- Saber onde o código **para**: o que "permanecer" e "dar fruto" significam no texto não é tema do código
origem: livros/10-evangelho-joao-csharp/content/110-cap133.md
---

::: vila A classe que construía o mundo
No sistema de pedidos, a classe `ServicoDePedidos` fazia `new Banco()`, `new Email()`, `new Relogio()` dentro do construtor. Para testar, era preciso um banco de verdade. Caio sugeriu virar o jogo: "a classe **declara do que precisa** e **recebe** de fora". No dia seguinte, o teste usava um banco de mentira, e a produção, o de verdade, **sem mudar uma linha da classe**.
:::

::: ilustracao img/joao/cap133.svg | Os ramos recebem a seiva: cada peça declara do que precisa, e o recipiente liga tudo. A seiva é permanente; ramo e fruto, transientes.
:::

## O texto

> **1** Eu sou a videira verdadeira, e meu Pai é o lavrador. **2** Todo ramo que em mim não dá fruto, ele o tira; e todo o que da fruto, ele o limpa, para que dê mais fruto. **3** Vós já estais limpos pela palavra que vos tenho falado. **4** Estai em mim, e eu em vós; como o ramo de si mesmo não pode dar fruto, se não estiver na videira, assim vós também não, se não estiverdes em mim. **5** Eu sou a videira, vós sois os ramos; quem está em mim, e eu nele, esse dá muito fruto; porque sem mim nada podeis fazer. **6** Se alguém não estiver em mim, é lançado fora, como o ramo, e seca-se; e os colhem, e os lançam no fogo, e ardem. **7** Se vós estiverdes em mim, e minhas palavras estiverem em vós, tudo o que quiserdes pedireis, e será feito para vós. **8** Nisto é glorificado meu Pai, em que deis muito fruto; e assim sereis meus discípulos.
>
> *João 15,1-8 (Bíblia Livre)*

## Lendo devagar

- **"Eu sou a videira verdadeira, e meu Pai é o lavrador"** (15,1). A imagem tem três partes: a **videira** (o tronco), os **ramos** e o **lavrador** (quem cuida).
- O lavrador **tira** o ramo que não dá fruto e **limpa** o que dá, para que dê **mais** fruto (15,2).
- O convite central: **"Estai em mim, e eu em vós"** (15,4). O ramo **não pode dar fruto de si mesmo**: só dá se estiver **na videira**.
- **"Sem mim nada podeis fazer"** (15,5).
- Em 15,6, o texto fala de quem não permanece, "lançado fora, como o ramo", e em 15,8 de dar muito fruto e ser discípulo.

O ponto que interessa ao código: o ramo **não fabrica a seiva**. Ele **recebe** da videira. Sem a ligação, não produz nada. (A palavra "seiva" é **nossa**: o texto não a usa.)

## Do versículo ao código

### Passo 1: "Eu sou a videira"

A videira é a **fonte** de que os ramos dependem. Em código, uma classe:

```cs
public class Videira
{
    public string Seiva => "seiva da videira";      // a palavra "seiva" é nossa, não do texto
}
```

### Passo 2: "Estai em mim": o ramo recebe a videira

Aqui está o coração do capítulo. Há **dois jeitos** de um ramo ter o que precisa. O primeiro é **fabricar o seu próprio**, criando dentro de si uma videira (`new Videira()`): é "dar fruto **de si mesmo**". O segundo é **receber** a videira de fora, no construtor:

```cs
public class Ramo
{
    private readonly Videira videira;

    public Ramo(Videira videira) => this.videira = videira;      // "Estai em mim": ligado à videira
}
```

O segundo é o do texto: o ramo **está na** videira. Em programação, receber de fora o que se precisa, em vez de criar por conta própria, chama-se **injeção de dependência**: o ramo **depende** da videira, e a videira é **injetada** nele.

### Passo 3: "sem mim nada podeis fazer"

Se o ramo é criado **sem** videira, ele **não deve nem existir**. O construtor recusa:

```cs
public Ramo(Videira videira) =>
    this.videira = videira ?? throw new ArgumentNullException(nameof(videira), "Sem mim nada podeis fazer.");
```

O `??` significa "se for vazio, faça isto": se a videira for `null` (nada), lança uma exceção com a própria frase do versículo.

### Passo 4: "como o ramo de si mesmo não pode dar fruto"

O fruto **vem da seiva** que o ramo recebeu:

```cs
public string DarFruto() => DaFruto ? $"fruto feito da {videira.Seiva}" : "nenhum fruto";
```

### Passo 5: "Todo ramo [...] que não dá fruto, ele o tira; e todo o que dá fruto, ele o limpa"

O lavrador olha os ramos: **tira** os que não dão fruto e **limpa** os que dão. Em C#, separar uma lista em dois grupos por uma condição é um trabalho de `Where`:

```cs
var tirados = ramos.Where(r => !r.DaFruto).ToList();    // "ele o tira"
var limpos  = ramos.Where(r => r.DaFruto).ToList();     // "ele o limpa"
ramos.RemoveAll(tirados.Contains);                       // os tirados saem da videira
```

### O código completo, rodando

```cs Jornada/Joao/Texto/Cap133Videira.cs
namespace Jornada.Joao.Texto;

// Jo 15,1-5 (Bíblia Livre): "Eu sou a videira verdadeira, e meu Pai é o lavrador. Todo ramo que em mim não dá fruto, ele o tira;
//  e todo o que dá fruto, ele o limpa, para que dê mais fruto. [...] Estai em mim, e eu em vós; como o ramo de si mesmo não pode
//  dar fruto, se não estiver na videira [...] porque sem mim nada podeis fazer."
//
// TEXTO     : a imagem da videira, dos ramos e do lavrador para falar de permanecer em Jesus e dar fruto.
// ANALOGIA  : o ramo RECEBE o que precisa da videira (injeção de dependência) e não o fabrica.
// LIMITE    : "permanecer" e "dar fruto" são relações espirituais que o texto não define em termos de código. O código só
//             aproveita a ESTRUTURA da imagem (quem recebe, de quem, e quem cuida). Em 15,6 o texto fala do ramo lançado fora
//             e queimado; o código não modela isso nem qualquer juízo.

public class Videira
{
    public string Seiva => "seiva da videira";
}

// <ramo>
public class Ramo
{
    private readonly Videira videira;

    // "Estai em mim": o ramo só existe ligado à videira; sem ela, nem pode ser criado
    public Ramo(Videira videira) =>
        this.videira = videira ?? throw new ArgumentNullException(nameof(videira), "Sem mim nada podeis fazer.");

    public bool DaFruto { get; set; } = true;

    // "como o ramo de si mesmo não pode dar fruto": o fruto vem da seiva que recebe
    public string DarFruto() => DaFruto ? $"fruto feito da {videira.Seiva}" : "nenhum fruto";
}
// </ramo>

// <lavrador>
// "meu Pai é o lavrador": aqui o lavrador é só o papel de quem cuida da videira.
public class Lavrador
{
    // "Todo ramo [...] que não dá fruto, ele o tira; e todo o que dá fruto, ele o limpa, para que dê mais fruto."
    public (List<Ramo> Tirados, List<Ramo> Limpos) Podar(List<Ramo> ramos)
    {
        var tirados = ramos.Where(r => !r.DaFruto).ToList();
        var limpos = ramos.Where(r => r.DaFruto).ToList();

        ramos.RemoveAll(tirados.Contains);
        return (tirados, limpos);
    }
}
// </lavrador>
```

```bash
dotnet run --project codigo/joao/Joao.Console -- exemplo t133
```

```cs Uso
var videira = new Videira();
var ramos = new List<Ramo> { new(videira), new(videira) { DaFruto = false }, new(videira) };

Console.WriteLine("Ligado à videira: " + ramos[0].DarFruto());

try { _ = new Ramo(null!); }
catch (ArgumentNullException e) { Console.WriteLine("Sem a videira: " + e.Message.Split('(')[0].Trim()); }

var (tirados, limpos) = new Lavrador().Podar(ramos);
Console.WriteLine($"O lavrador tirou {tirados.Count} e limpou {limpos.Count}; ficam {ramos.Count} ramos");
```

```saida
Ligado à videira: fruto feito da seiva da videira
Sem a videira: Sem mim nada podeis fazer.
O lavrador tirou 1 e limpou 2; ficam 2 ramos
```

Ligado à videira, o ramo dá fruto feito da seiva **dela**. Tentar criar um ramo **sem videira** dá a mensagem do versículo 5. E o lavrador, de três ramos, **tirou um** (o que não dava fruto) e **limpou dois**.

::: cuidado Limite da analogia
- **O código usa a imagem só como estrutura**: quem recebe, de quem recebe, e quem cuida. O que "permanecer" e "dar fruto" significam no texto (relação com Jesus, discipulado, amor mútuo em 15,9-17) **não** é objeto do código.
- **`Lavrador` é só um papel** ("meu Pai é o lavrador", 15,1). O Pai **não** é modelado como classe neste livro; assim, não há conflito com o capítulo 124 (onde só `Deus` e `Verbo` aparecem) nem com o 132 (onde Jesus é uma pessoa à mesa).
- **"Tirar" e "limpar"** no código são `Where` e `RemoveAll`. O texto não fala de listas, e o versículo 15,6 (o ramo lançado fora e queimado) **não** foi modelado: não há "juízo" no código.
- **"Seiva"** é palavra nossa: o texto diz "videira", "ramos" e "fruto".
:::

::: curiosidade Lupa no grego
- **μένω** (*menō*, "permanecer, ficar", G3306) aparece **40 vezes** em João; **11** no capítulo 15, que tem cerca de 500 palavras: são **22 ocorrências por mil palavras**, a maior densidade entre os capítulos (`onde menō`).
- **ἄμπελος** (*ampelos*, "videira") aparece **3 vezes** (15,1.4.5); **κλῆμα** (*klēma*, "ramo", G2814) **4** (15,2.4.5.6); **γεωργός** (*geōrgos*, "lavrador", G1092) **1** (15,1); **καρπός** (*karpos*, "fruto") **10**, das quais **8** em Jo 15 (`onde klēma`, `onde karpos`).
- Em 15,2, os dois verbos têm sons parecidos: **αἴρει** (*airei*, "tira, levanta", G142) e **καθαίρει** (*kathairei*, "limpa" e, em especial, "podar", G2508) (`onde airō`, `onde kathairō`). **καθαίρω** aparece **só aqui** em João.

*Fontes: texto grego SBLGNT (CC BY 4.0); contagens por lema do MorphGNT (CC BY-SA 3.0), reproduzíveis com `dotnet run --project codigo/joao/Joao.Console -- onde <palavra>`; sentidos do dicionário de Strong (OpenScriptures), com a entrada indicada.*
:::


## Indo além: um recipiente que liga as peças sozinho

Com poucas peças (uma videira, alguns ramos), dá para ligar tudo **à mão**. Em programas com **dezenas** de peças que dependem umas das outras (o ramo depende da videira, o fruto depende do ramo e da seiva...), alguém precisa ligar tudo. Esse "alguém" é um **recipiente**, e as seções abaixo constroem um, usando reflexão.

::: figura Quem monta quem
<svg viewBox="0 0 740 240" role="img" aria-label="O recipiente monta Fruto, que precisa de Ramo e de Seiva; Ramo precisa de Seiva; a Seiva é permanente e compartilhada; Ramo e Fruto são transientes">
  <rect x="20" y="92" width="150" height="56" rx="14" class="f-prim"/><text x="95" y="116" text-anchor="middle" font-weight="800">Recipiente</text><text x="95" y="134" text-anchor="middle" font-size="11" class="t-dim">Obter&lt;IFruto&gt;()</text>
  <path class="arrow" d="M172 120H248"/>
  <rect x="250" y="40" width="150" height="56" rx="14" class="f-warn"/><text x="325" y="64" text-anchor="middle" font-weight="800">Fruto</text><text x="325" y="82" text-anchor="middle" font-size="11" class="t-dim">transiente</text>
  <rect x="250" y="144" width="150" height="56" rx="14" class="f-warn"/><text x="325" y="168" text-anchor="middle" font-weight="800">Ramo</text><text x="325" y="186" text-anchor="middle" font-size="11" class="t-dim">transiente</text>
  <path class="arrow" d="M325 98V142"/>
  <rect x="520" y="92" width="190" height="56" rx="14" class="f-good"/><text x="615" y="116" text-anchor="middle" font-weight="800">Seiva</text><text x="615" y="134" text-anchor="middle" font-size="11" class="t-dim">permanente: a mesma para todos</text>
  <path class="arrow" d="M402 68L518 108"/><path class="arrow" d="M402 172L518 132"/>
  <text x="370" y="226" text-anchor="middle" font-size="13" class="t-dim">Fruto precisa de Ramo e de Seiva; Ramo precisa de Seiva. O recipiente resolve a ordem.</text>
</svg>
:::

### 1) Receber, e não construir

Uma classe que escreve `new Banco()` dentro de si **decide** qual banco usa. Isso a prende a ele: para testá-la, é preciso o banco de verdade. A **injeção de dependência** inverte a situação: a classe **declara no construtor** do que precisa (em geral, **interfaces**) e **recebe** de fora. Chama-se **inversão de controle** porque quem controla a montagem passa a ser **outro** (o recipiente), e não a própria classe.

As peças do exemplo usam as palavras da imagem (videira, ramo, fruto) e a "seiva", que é nossa: uma `ISeiva` (implementada por `SeivaDaTerra`), um `Ramo` que precisa de uma `ISeiva` e um `Fruto` que precisa de um `IRamo` e de uma `ISeiva`.

### 2) O recipiente, com reflexão

O `Recipiente` guarda um **registro** por tipo: qual implementação usar e qual **ciclo de vida**. Quando alguém pede `Obter<IFruto>()`, ele:

1. procura o registro; se não houver, lança um erro que **diz o que falta**;
2. verifica no **caminho** (uma `Stack<Type>`) se o tipo já está em construção: se estiver, achou um **ciclo** e mostra a cadeia (`IA → IB → IA`);
3. se o ciclo de vida é **Permanente** e já existe uma instância, devolve-a;
4. senão, **constrói**: por **reflexão**, escolhe o construtor com **mais parâmetros** que o recipiente consegue satisfazer, **resolve cada parâmetro recursivamente** e chama `construtor.Invoke(...)`;
5. se a peça é `IDisposable`, **anota** para descartar depois; se é Permanente, **guarda**.

O `try`/`finally` com `caminho.Pop()` garante que o caminho esteja sempre limpo, mesmo quando algo falha no meio.

```cs Jornada/Joao/Cap133/Recipiente.cs
using System.Reflection;

namespace Jornada.Joao.Cap133;

/// <summary>Quanto tempo uma peça "permanece": criada de novo a cada pedido, ou uma só, que fica.</summary>
public enum Ciclo { Transiente, Permanente }

/// <summary>
/// Um recipiente de dependências bem pequeno: quem precisa de uma peça pede pelo TIPO,
/// e o recipiente descobre, por reflexão, de que a peça precisa e liga tudo (o ramo na videira).
/// </summary>
public sealed class Recipiente : IDisposable
{
    private sealed record Registro(Type Implementacao, Ciclo Ciclo, Func<Recipiente, object>? Fabrica);

    private readonly Dictionary<Type, Registro> registros = new();
    private readonly Dictionary<Type, object> permanentes = new();
    private readonly List<IDisposable> descartaveis = new();
    private bool descartado;

    public Recipiente Registrar<TServico, TImplementacao>(Ciclo ciclo = Ciclo.Transiente)
        where TImplementacao : TServico
    {
        registros[typeof(TServico)] = new Registro(typeof(TImplementacao), ciclo, null);
        return this;
    }

    public Recipiente Registrar<TServico>(Func<Recipiente, TServico> fabrica, Ciclo ciclo = Ciclo.Transiente)
        where TServico : notnull
    {
        registros[typeof(TServico)] = new Registro(typeof(TServico), ciclo, r => fabrica(r)!);
        return this;
    }

    public T Obter<T>() where T : notnull => (T)Obter(typeof(T), new Stack<Type>());

    private object Obter(Type tipo, Stack<Type> caminho)
    {
        if (descartado)
            throw new ObjectDisposedException(nameof(Recipiente));

        if (!registros.TryGetValue(tipo, out var registro))
            throw new InvalidOperationException($"Não sei criar {tipo.Name}: ninguém a registrou.");

        if (caminho.Contains(tipo))
        {
            var ciclo = string.Join(" → ", caminho.Reverse().Append(tipo).Select(t => t.Name));
            throw new InvalidOperationException($"Dependência circular: {ciclo}.");
        }

        if (registro.Ciclo == Ciclo.Permanente && permanentes.TryGetValue(tipo, out var existente))
            return existente;

        caminho.Push(tipo);
        try
        {
            var criado = registro.Fabrica?.Invoke(this) ?? Construir(registro.Implementacao, caminho);

            if (criado is IDisposable descartavel)
                descartaveis.Add(descartavel);
            if (registro.Ciclo == Ciclo.Permanente)
                permanentes[tipo] = criado;

            return criado;
        }
        finally
        {
            caminho.Pop();
        }
    }

    /// <summary>Escolhe o construtor com mais parâmetros que o recipiente consiga satisfazer e resolve cada um.</summary>
    private object Construir(Type implementacao, Stack<Type> caminho)
    {
        var construtor = implementacao
            .GetConstructors(BindingFlags.Public | BindingFlags.Instance)
            .OrderByDescending(c => c.GetParameters().Length)
            .FirstOrDefault(c => c.GetParameters().All(p => registros.ContainsKey(p.ParameterType)))
            ?? throw new InvalidOperationException(
                $"Não sei criar {implementacao.Name}: nenhum construtor tem todas as peças registradas.");

        var argumentos = construtor.GetParameters()
            .Select(p => Obter(p.ParameterType, caminho))
            .ToArray();

        return construtor.Invoke(argumentos);
    }

    /// <summary>"Podar": descarta, na ordem inversa da criação, tudo o que o recipiente criou e é descartável.</summary>
    public void Dispose()
    {
        if (descartado) return;
        descartado = true;

        for (int i = descartaveis.Count - 1; i >= 0; i--)
            descartaveis[i].Dispose();

        descartaveis.Clear();
        permanentes.Clear();
    }
}
```

Veja as peças do exemplo e como ele monta tudo:

```cs Uso
// As peças do exemplo (cada uma declara, no construtor, do que precisa):
//   interface ISeiva;             class SeivaDaTerra : ISeiva
//   interface IRamo;              class Ramo(ISeiva seiva) : IRamo
//   interface IFruto;             class Fruto(IRamo ramo, ISeiva seiva) : IFruto
```

```cs Uso
using var videira = new Recipiente()
    .Registrar<ISeiva, SeivaDaTerra>(Ciclo.Permanente)
    .Registrar<IRamo, Ramo>()
    .Registrar<IFruto, Fruto>();

var fruto = videira.Obter<IFruto>();
Console.WriteLine($"o fruto recebeu o ramo e a seiva '{fruto.Seiva.Nome}'");
Console.WriteLine($"ramo e fruto bebem da mesma seiva? {ReferenceEquals(fruto.Seiva, fruto.Ramo.Seiva)}");
Console.WriteLine($"dois pedidos de ramo dão o mesmo ramo? {ReferenceEquals(videira.Obter<IRamo>(), videira.Obter<IRamo>())}");

try { new Recipiente().Registrar<IRamo, Ramo>().Obter<IRamo>(); }
catch (InvalidOperationException e) { Console.WriteLine($"sem seiva registrada: {e.Message}"); }
```

```saida
o fruto recebeu o ramo e a seiva 'da terra'
ramo e fruto bebem da mesma seiva? True
dois pedidos de ramo dão o mesmo ramo? False
sem seiva registrada: Não sei criar Ramo: nenhum construtor tem todas as peças registradas.
```

Quatro respostas. O fruto recebeu o ramo e a seiva `'da terra'`. **Ramo e fruto bebem da mesma seiva**, porque a seiva é **Permanente**. Dois pedidos de ramo **não** dão o mesmo ramo, porque ele é **Transiente**. E, sem a seiva registrada, o erro **diz o que falta**: *"nenhum construtor tem todas as peças registradas"*.

::: erro Service locator: o primo mal-comportado
É tentador passar o `Recipiente` inteiro para dentro das classes e chamar `recipiente.Obter<X>()` quando precisar. Isso se chama **service locator** e **esconde** as dependências: ao ler o construtor, você não vê mais do que a classe precisa. Prefira **declarar no construtor**; o recipiente só deve aparecer na **raiz** do programa (onde tudo é montado).
:::

::: dica Cuidado com o ciclo de vida
Uma peça **Permanente** que depende de uma **Transiente** "prende" aquela instância para sempre. Em recipientes reais isso se chama *captive dependency* e é fonte de erros difíceis. Regra prática: **uma peça só deve depender de peças que vivam tanto quanto ela, ou mais**.
:::

::: cuidado Limites desta implementação
- O `Recipiente` **não é seguro para várias threads** (usa dicionários comuns) e só conhece os ciclos **Transiente** e **Permanente**.
- Escolhe sempre o construtor com mais parâmetros resolvíveis, e só resolve por **tipo exato** (não há nomes, nem peças opcionais).
- Em bibliotecas reais (como a injeção de dependência da própria plataforma .NET) há ciclo de vida por escopo, validação e outras proteções que este exemplo não tem.
:::

### 3) Testes

```cs Joao.Testes/Cap133Tests.cs
using Jornada.Joao.Cap133;

namespace Joao.Testes;

public class Cap133Tests
{
    // Peças de teste: cada uma declara no construtor do que precisa.
    public interface ISeiva { string Nome { get; } }
    public sealed class SeivaDaTerra : ISeiva { public string Nome => "seiva"; }

    public interface IRamo { ISeiva Seiva { get; } }
    public sealed class Ramo : IRamo
    {
        public Ramo(ISeiva seiva) => Seiva = seiva;
        public ISeiva Seiva { get; }
    }

    public interface IFruto { IRamo Ramo { get; } ISeiva Seiva { get; } }
    public sealed class Fruto : IFruto
    {
        public Fruto(IRamo ramo, ISeiva seiva) { Ramo = ramo; Seiva = seiva; }
        public IRamo Ramo { get; }
        public ISeiva Seiva { get; }
    }

    public sealed class Raiz : IDisposable
    {
        public static List<string> Descartes { get; } = new();
        public string Id { get; } = Guid.NewGuid().ToString("N")[..4];
        public void Dispose() => Descartes.Add(Id);
    }

    [Fact]
    public void O_recipiente_liga_a_cadeia_inteira_de_dependencias()
    {
        using var videira = new Recipiente()
            .Registrar<ISeiva, SeivaDaTerra>()
            .Registrar<IRamo, Ramo>()
            .Registrar<IFruto, Fruto>();

        var fruto = videira.Obter<IFruto>();

        Assert.Equal("seiva", fruto.Ramo.Seiva.Nome);
    }

    [Fact]
    public void Transiente_cria_uma_peça_nova_a_cada_pedido()
    {
        using var videira = new Recipiente().Registrar<ISeiva, SeivaDaTerra>(Ciclo.Transiente);

        Assert.NotSame(videira.Obter<ISeiva>(), videira.Obter<ISeiva>());
    }

    [Fact]
    public void Permanente_permanece_e_e_a_mesma_para_todos()
    {
        using var videira = new Recipiente()
            .Registrar<ISeiva, SeivaDaTerra>(Ciclo.Permanente)
            .Registrar<IRamo, Ramo>()
            .Registrar<IFruto, Fruto>();

        var fruto = videira.Obter<IFruto>();

        Assert.Same(fruto.Seiva, fruto.Ramo.Seiva);          // ramo e fruto bebem da MESMA seiva
        Assert.Same(fruto.Seiva, videira.Obter<ISeiva>());
    }

    [Fact]
    public void Peça_nao_registrada_gera_erro_que_diz_o_que_falta()
    {
        using var videira = new Recipiente().Registrar<IRamo, Ramo>();   // falta a seiva

        var e = Assert.Throws<InvalidOperationException>(() => videira.Obter<IRamo>());
        Assert.Contains("nenhum construtor tem todas as peças registradas", e.Message);
        Assert.Throws<InvalidOperationException>(() => videira.Obter<IFruto>());
    }

    public interface IA { }
    public interface IB { }
    public sealed class A : IA { public A(IB b) { } }
    public sealed class B : IB { public B(IA a) { } }

    [Fact]
    public void Dependencia_circular_e_detectada_com_o_caminho_do_ciclo()
    {
        using var videira = new Recipiente().Registrar<IA, A>().Registrar<IB, B>();

        var e = Assert.Throws<InvalidOperationException>(() => videira.Obter<IA>());

        Assert.Equal("Dependência circular: IA → IB → IA.", e.Message);
    }

    [Fact]
    public void Fabrica_permite_registrar_com_logica_propria()
    {
        using var videira = new Recipiente()
            .Registrar<ISeiva>(_ => new SeivaDaTerra(), Ciclo.Permanente);

        Assert.Equal("seiva", videira.Obter<ISeiva>().Nome);
    }

    [Fact]
    public void Podar_descarta_na_ordem_inversa_da_criacao_e_so_uma_vez()
    {
        Raiz.Descartes.Clear();
        Raiz? primeira = null, segunda = null;

        var videira = new Recipiente().Registrar<Raiz>(_ => new Raiz(), Ciclo.Transiente);
        primeira = videira.Obter<Raiz>();
        segunda = videira.Obter<Raiz>();

        videira.Dispose();
        videira.Dispose();                                   // segunda vez: nada acontece

        Assert.Equal(new[] { segunda.Id, primeira.Id }, Raiz.Descartes);
    }

    [Fact]
    public void Depois_de_podado_o_recipiente_nao_serve_mais()
    {
        var videira = new Recipiente().Registrar<ISeiva, SeivaDaTerra>();
        videira.Dispose();

        Assert.Throws<ObjectDisposedException>(() => videira.Obter<ISeiva>());
    }
}
```

O teste **`Dependencia_circular_e_detectada_com_o_caminho_do_ciclo`** valida a mensagem *"Dependência circular: IA → IB → IA."* **`Podar_descarta_na_ordem_inversa_da_criacao_e_so_uma_vez`** confirma que a última peça criada é a primeira a ser descartada, como se desfaz uma pilha de camadas, e que um segundo `Dispose()` não faz nada.

::: exercicio Registrar uma instância pronta
Escreva `RegistrarInstancia<T>(this Recipiente recipiente, T instancia)`, que registra um objeto que **já existe**: o recipiente devolve sempre essa mesma instância. Use o que o recipiente já oferece, sem alterar a classe.

--- resposta
```cs Jornada/Joao/Exercicios/Cap133Instancia.cs
using Jornada.Joao.Cap133;

namespace Jornada.Joao.Exercicios;

public static class RecipienteExtensoes
{
    /// <summary>Registra uma peça que já existe: o recipiente devolve sempre essa mesma instância.</summary>
    public static Recipiente RegistrarInstancia<T>(this Recipiente recipiente, T instancia) where T : notnull =>
        recipiente.Registrar<T>(_ => instancia, Ciclo.Permanente);
}
```

`Registrar<T>(fabrica, ciclo)` aceita uma **fábrica**, uma função que devolve a peça. Passamos `_ => instancia` (ignorando o parâmetro) e marcamos **Permanente**: assim, a fábrica é chamada uma vez e o resultado é guardado. Repare também que o recipiente descartará a instância ao ser descartado, se ela for `IDisposable`, o que talvez não seja o que você quer para um objeto emprestado: uma boa discussão para os testes.
:::

::: resumo
- "Estai em mim": o `Ramo` **recebe** a `Videira` no construtor (injeção de dependência); "sem mim nada podeis fazer": sem ela, nem existe. É só **estrutura**: "permanecer" e "dar fruto" não são tema do código.
- O lavrador **tira** (`!DaFruto`) e **limpa** (`DaFruto`) com `Where`. Em 15,6, o texto fala do ramo lançado fora; isso **não** foi modelado.
- Em João, **menō** (permanecer) aparece 40 vezes, **11 no cap. 15** (22 por mil palavras, a maior densidade); **ampelos** 3, **klēma** 4, **karpos** 10 (8 em Jo 15); em 15,2 o jogo **airei/kathairei**.
- **Injeção de dependência**: a classe **declara** o que precisa no construtor e **recebe**; **inversão de controle**.
- O **recipiente** usa reflexão: escolhe o construtor, resolve a cadeia recursivamente e detecta **ciclos**.
- **Transiente** (novo a cada pedido) e **Permanente** (um só, que permanece); cuidado com *captive dependency*.
- `Dispose()` do recipiente descarta na **ordem inversa** da criação; evite o *service locator*.
:::

::: quiz
? O que o construtor `Ramo(Videira videira)` expressa?
+ "Estai em mim": o ramo recebe a videira, em vez de criá-la
- Que o ramo cria a sua seiva
- Que o ramo é uma videira
- Nada
> Receber de fora o que se precisa é injeção de dependência.

? O que acontece com `new Ramo(null)`?
+ Lança `ArgumentNullException` com "Sem mim nada podeis fazer."
- Cria um ramo sem fruto
- Cria uma videira
- Nada
> O construtor recusa o ramo sem videira.

? A palavra "seiva" está em João 15?
+ Não: é palavra nossa, para nomear o que o ramo recebe
- Sim, em 15,1
- Sim, em 15,4
- Sim, em 15,5
> O texto diz "videira", "ramos" e "fruto".

? Quantas das 40 ocorrências de *menō* ("permanecer") estão no capítulo 15?
+ 11
- 4
- 40
- Nenhuma
> O capítulo 15 é o de maior densidade da palavra.

? O que o jogo *airei/kathairei* de Jo 15,2 mostra?
+ Dois verbos parecidos no som: "tirar/erguer" e "podar/limpar"
- Dois nomes de ramos
- Um erro de cópia
- Uma citação de outro livro
> *Kathairō* só aparece uma vez em João.

? O que é injeção de dependência?
+ A classe declara do que precisa e recebe de fora, em vez de construir
- Usar `new` em todo lugar
- Uma biblioteca de banco de dados
- Um tipo de exceção
> A montagem passa a ser feita por outro (inversão de controle).

? Qual a diferença entre ciclo Transiente e Permanente?
+ Transiente cria nova instância a cada pedido; Permanente reaproveita uma só
- Nenhuma
- Permanente é mais rápido
- Transiente nunca é descartado
> A seiva é permanente; o ramo, transiente.

? Como o recipiente detecta uma dependência circular?
+ Mantém uma pilha do caminho em construção e vê se o tipo já está nela
- Com um contador
- Esperando a exceção do .NET
- Não detecta
> A mensagem mostra a cadeia: IA → IB → IA.

? Por que evitar o service locator?
+ Porque esconde as dependências: o construtor deixa de mostrar o que a classe precisa
- Porque é lento demais
- Porque não existe em C#
- Porque exige reflexão
> Declarar no construtor torna tudo visível.
:::

::: cartas
Estai em mim (Jo 15,4) :: Em código: a classe recebe de fora o que precisa (injeção de dependência).
Videira (Jo 15) :: Discurso: "Eu sou a videira verdadeira", ramos, fruto e permanecer.
Menō :: "Permanecer, ficar": 40 vezes em João, 11 no cap. 15.
Airei / kathairei :: Jogo de sons de 15,2: "tira/ergue" e "poda/limpa".
Injeção de dependência :: A classe declara o que precisa e recebe de fora.
Inversão de controle :: Quem monta as peças deixa de ser a própria classe.
Transiente / Permanente :: Nova a cada pedido / uma só que permanece.
Dependência circular :: A precisa de B e B precisa de A; o recipiente detecta.
Service locator :: Pedir peças ao recipiente de dentro das classes: esconde dependências.
:::
