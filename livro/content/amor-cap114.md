---
id: amor-cap114
order: 146
kind: capitulo
num: 114
group: C# & Mentalidade · Parte 17 · Você é aquilo que ama, em C#
title: Restrições que libertam
subtitle: Tradição e inovação não são inimigas: limites bem escolhidos podem despertar a criatividade. Em C#, é o capítulo dos tipos que não deixam o erro existir: objetos de valor, IParsable e o operador implícito.
minutes: 75
hue: 255
objectives:
- Entender a ideia de **restrição libertadora** e de **"tradicionalidade inovadora"**
- Criar **objetos de valor** (value objects) que **só existem se válidos**, na borda do sistema
- Implementar **`IParsable<T>`** e usar **genéricos com membros `static abstract`** (`T.Parse`)
- Definir um **operador implícito** e entender uma pegadinha de sobrecarga de métodos
origem: livros/08-voce-e-aquilo-que-ama-csharp/content/134-cap114.md
---

::: vila A coluna que não aceitava lixo
O time de Beto sofria com um relatório que, uma vez por mês, mostrava "desconto de 140%". Ninguém sabia de onde vinha. Lia propôs uma regra simples: **nunca mais um `double` solto para guardar um percentual**; haveria um **tipo `Percentual`** que simplesmente **não podia existir** fora de 0 a 100. No começo, Beto reclamou ("mais código para a mesma coisa!"). Duas semanas depois, o relatório sumiu dos incidentes, e o código de todo o resto ficou **mais curto**: ninguém mais conferia nada.
:::

::: ilustracao img/amor/cap114.svg | Um gabarito não limita o desenho: guia a mão. Os limites certos deixam o resto do código livre para trabalhar sem medo.
:::

## O que o livro diz

Perto do fim do livro, Smith conta um caso de arquitetura que usa como parábola. Um colecionador de arte, Albert Barnes, deixou determinado que **nenhuma das suas milhares de obras** poderia ser vendida, emprestada ou **retirada da disposição** que ele criou em sua casa. Décadas depois, a fundação que guarda a coleção foi autorizada a mudar para um novo prédio na cidade, mas o tribunal decidiu que **a disposição das galerias devia ser reproduzida exatamente**. Parecia um desastre criativo: o que sobraria aos arquitetos além de **copiar**?

O que aconteceu, relata ele (apoiado em um crítico de arquitetura), foi o contrário: os arquitetos **aceitaram a restrição como catalisador**. Reproduziram as galerias, mas inventaram em volta: uma "caixa de luz" ao longo do prédio e um pátio de luz, que fizeram as mesmas obras parecerem vistas pela primeira vez. O autor chama isso de **"tradicionalidade inovadora"** ou "repetição criativa": o novo prédio é uma **extensão fiel** do original, sem imitá-lo.

A lição que ele tira: **restrições não são o contrário da criatividade; muitas vezes, são o que a libera**. Ele aplica isso à vida: nossas ações diárias poderiam florescer melhor **dentro dos limites** que uma tradição nos entrega, em vez de reinventarmos tudo a cada dia.

::: cuidado Sobre a aplicação do autor
Smith aplica essa ideia às tradições de adoração e à vida cristã, o que não vamos discutir aqui. Também não vale generalizar: **nem toda restrição liberta**, e algumas oprimem. O princípio útil é mais modesto: **bons limites**, escolhidos com cuidado e entendidos por quem convive com eles, podem **abrir espaço** para fazer melhor.
:::

::: analogia O gabarito do pintor de placas
O pintor de letras usa um gabarito: uma régua com furos. Parece uma prisão, e é o que garante que todos os "A" saiam iguais e o pintor possa **pensar na cor, no espaçamento, na mensagem**. A restrição tira um peso da cabeça.
:::

## Da vida para o código

| No livro | Em C# |
|---|---|
| Uma restrição que não se discute | Um tipo que **não consegue existir** inválido (`Percentual`: de 0 a 100) |
| Dentro do limite, liberdade | O resto do código **não confere nada**, porque o tipo já garante |
| Tradição: um formato conhecido | **`IParsable<T>`**: a interface padrão do .NET para "ler de texto" |
| Inovar dentro do contrato | `Leitor.Ler<T>` funciona para **qualquer** `T : IParsable<T>` |
| Parte da limitação vira comodidade | **Operador implícito**: um `Percentual` serve onde se espera `double` |
| Tudo precisa fechar 100% | `Distribuicao.Criar` recusa partes que não somam 100 |

::: figura Validar na borda, confiar no miolo
<svg viewBox="0 0 740 220" role="img" aria-label="Texto bruto entra por uma porta onde é convertido em tipo válido; dentro do sistema, todo valor já é válido e nenhuma conferência é necessária">
  <rect x="20" y="60" width="180" height="90" rx="14" class="f-bad"/><text x="110" y="95" text-anchor="middle" font-weight="800">Texto bruto</text><text x="110" y="118" text-anchor="middle" font-size="12" class="t-dim">"120", "abc", " 7 %"</text>
  <rect x="250" y="40" width="120" height="130" rx="14" class="f-warn"/><text x="310" y="84" text-anchor="middle" font-weight="800">Porta</text><text x="310" y="106" text-anchor="middle" font-size="12" class="t-dim">TryParse</text><text x="310" y="124" text-anchor="middle" font-size="12" class="t-dim">valida aqui</text>
  <rect x="420" y="30" width="300" height="150" rx="16" class="f-good"/><text x="570" y="62" text-anchor="middle" font-weight="800">Dentro do sistema</text>
  <text x="570" y="90" text-anchor="middle" font-size="13">Percentual (0 a 100)</text><text x="570" y="112" text-anchor="middle" font-size="13">Email (formato ok)</text><text x="570" y="134" text-anchor="middle" font-size="13">Distribuição (soma 100%)</text><text x="570" y="162" text-anchor="middle" font-size="12" class="t-dim">tudo já é válido: nada a conferir</text>
  <path class="arrow" d="M202 105H248"/><path class="arrow" d="M372 105H418"/>
  <text x="370" y="206" text-anchor="middle" font-size="13" class="t-dim">Uma só porta rígida deixa todo o resto mais livre.</text>
</svg>
:::

## 1) Um percentual que só existe se for válido

Um **objeto de valor** (*value object*) é um tipo pequeno que representa **um valor do domínio** (um percentual, um e-mail, uma data de vencimento) e **garante** as regras desse valor no construtor. Se o objeto existe, é válido. Isso muda tudo: em vez de espalhar `if (p < 0 || p > 100)` por 30 lugares, **uma única classe** cuida disso.

Veja `Percentual`. Ele é um `readonly record struct`, como o vetor do capítulo 106, mas com **construtor explícito** que valida. E implementa **`IParsable<Percentual>`**, uma interface do .NET 7+ para tipos que sabem se **ler de um texto**: exige dois métodos, `Parse` e `TryParse`.

Duas coisas novas:

- **`public static implicit operator double(Percentual p) => p.Valor;`** é um **operador de conversão implícita**: onde o C# espera um `double`, você pode passar um `Percentual` **sem converter**. Convenhamos: só vale quando a conversão **nunca perde informação nem falha**.
- **`TryParse` + `Parse`**: o primeiro devolve `bool` (para entrada que pode estar errada, como vimos no Livro 6); o segundo **lança** `FormatException` com uma mensagem útil. Quem implementa os dois dá a quem usa a **escolha**.

```cs Jornada/Habitos/Cap114/Tipos.cs
using System.Globalization;

namespace Jornada.Habitos.Cap114;

/// <summary>
/// Um valor que só existe se for válido: de 0 a 100. Quem recebe um Percentual não precisa conferir nada.
/// A restrição vira liberdade: o resto do código fica mais simples.
/// </summary>
public readonly record struct Percentual : IParsable<Percentual>
{
    public double Valor { get; }

    public Percentual(double valor)
    {
        if (double.IsNaN(valor) || valor is < 0 or > 100)
            throw new ArgumentOutOfRangeException(nameof(valor), "Um percentual vai de 0 a 100.");

        Valor = valor;
    }

    /// <summary>Conversão automática: onde se espera um double, um Percentual serve.</summary>
    public static implicit operator double(Percentual p) => p.Valor;

    public static Percentual Parse(string s, IFormatProvider? provider)
    {
        if (!TryParse(s, provider, out var resultado))
            throw new FormatException($"\"{s}\" não é um percentual válido (use um número de 0 a 100).");
        return resultado;
    }

    public static bool TryParse(string? s, IFormatProvider? provider, out Percentual resultado)
    {
        resultado = default;
        if (s is null) return false;

        var texto = s.Trim().TrimEnd('%').Trim();
        if (!double.TryParse(texto, NumberStyles.Float, provider ?? CultureInfo.InvariantCulture, out var numero))
            return false;
        if (double.IsNaN(numero) || numero is < 0 or > 100)
            return false;

        resultado = new Percentual(numero);
        return true;
    }

    public override string ToString() => $"{Valor.ToString("0.##", CultureInfo.InvariantCulture)}%";
}

/// <summary>Um e-mail "bom o bastante": a classe controla a própria criação e nunca existe inválida.</summary>
public sealed class Email
{
    public string Endereco { get; }

    private Email(string endereco) => Endereco = endereco;

    public static bool TryCriar(string? texto, out Email? email)
    {
        email = null;
        if (string.IsNullOrWhiteSpace(texto)) return false;

        var limpo = texto.Trim();
        int arroba = limpo.IndexOf('@');
        bool formatoOk = arroba > 0 && arroba == limpo.LastIndexOf('@') && limpo.IndexOf('.', arroba) > arroba + 1
                         && !limpo.EndsWith('.') && !limpo.Contains(' ');
        if (!formatoOk) return false;

        email = new Email(limpo.ToLowerInvariant());
        return true;
    }

    public override string ToString() => Endereco;
}

/// <summary>Uma divisão do tempo (ou do esforço) que fecha 100%: impossível criar uma que não feche.</summary>
public sealed class Distribuicao
{
    private readonly IReadOnlyList<(string Nome, Percentual Parte)> partes;

    private Distribuicao(IReadOnlyList<(string, Percentual)> partes) => this.partes = partes;

    public IReadOnlyList<(string Nome, Percentual Parte)> Partes => partes;

    public static Distribuicao Criar(params (string Nome, Percentual Parte)[] partes)
    {
        if (partes.Length == 0)
            throw new ArgumentException("Informe ao menos uma parte.", nameof(partes));

        double soma = partes.Sum(p => p.Parte.Valor);
        if (Math.Abs(soma - 100) > 0.001)
            throw new ArgumentException($"As partes somam {soma:0.##}%, e precisam somar 100%.", nameof(partes));

        return new Distribuicao(partes);
    }

    /// <summary>Converte "Foco 50, Reuniões 30, Aprender 20" em partes. Usa o Parse genérico abaixo.</summary>
    public static Distribuicao Ler(string texto) =>
        Criar(texto.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
                   .Select(item =>
                   {
                       int espaco = item.LastIndexOf(' ');
                       if (espaco < 0) throw new FormatException($"Use \"Nome valor\", e não \"{item}\".");
                       return (item[..espaco].Trim(), Leitor.Ler<Percentual>(item[(espaco + 1)..]));
                   })
                   .ToArray());
}

public static class Leitor
{
    /// <summary>Genérico com restrição estática: qualquer T que saiba se "parsear" (IParsable) serve.</summary>
    public static T Ler<T>(string texto) where T : IParsable<T> => T.Parse(texto, null);

    public static bool TentarLer<T>(string? texto, out T? resultado) where T : IParsable<T> =>
        T.TryParse(texto, null, out resultado);
}
```

O `Email` e a `Distribuicao` usam uma variação: o **construtor é privado** e a criação passa por `TryCriar` ou `Criar`, que **validam**. Quem recebeu um `Email` sabe que o texto tem um formato razoável. E a `Distribuicao` **não fecha** se as partes não somam 100% (a mensagem informa **quanto somou**).

O último pedaço é o **`Leitor`**:

```cs Genérico com membro estático abstrato
public static T Ler<T>(string texto) where T : IParsable<T> => T.Parse(texto, null);
```

Isso é novo no C# 11: a restrição **`where T : IParsable<T>`** garante que `T` tem um método **estático** `Parse`, e o código chama **`T.Parse(...)`** diretamente no tipo genérico. É por isso que `Leitor.Ler<int>("42")`, `Leitor.Ler<DateOnly>(...)` e `Leitor.Ler<Percentual>(...)` funcionam com **o mesmo código**.

```cs Uso
var p = Leitor.Ler<Percentual>(" 37.5 %");
Console.WriteLine(p);              // o operador implícito faz o compilador escolher WriteLine(double)
Console.WriteLine($"{p}");          // a interpolação chama ToString()

if (!Leitor.TentarLer<Percentual>("120", out _))
    Console.WriteLine("120 não é um percentual válido");

var distribuicao = Distribuicao.Ler("Foco profundo 50, Reuniões 30, Aprender 20");
foreach (var (nome, parte) in distribuicao.Partes)
    Console.WriteLine($"{nome}: {parte}");

try
{
    Distribuicao.Criar(("Foco", new Percentual(70)), ("Reuniões", new Percentual(40)));
}
catch (ArgumentException e)
{
    Console.WriteLine(e.Message[..e.Message.IndexOf(" (")]);
}

Console.WriteLine(Email.TryCriar("  Lia@Exemplo.com ", out var email) ? $"E-mail guardado como {email}" : "E-mail inválido");
Console.WriteLine(Email.TryCriar("lia@", out _) ? "válido" : "\"lia@\" não vira e-mail");
```

```saida
37,5
37.5%
120 não é um percentual válido
Foco profundo: 50%
Reuniões: 30%
Aprender: 20%
As partes somam 110%, e precisam somar 100%.
E-mail guardado como lia@exemplo.com
"lia@" não vira e-mail
```

Uma **pegadinha** apareceu logo na primeira linha da saída: `Console.WriteLine(p)` imprimiu **`37,5`**, e não `37.5%`! Por quê? Porque, com o **operador implícito**, o compilador acha que `WriteLine(double)` é uma **conversão melhor** que `WriteLine(object)` (que chamaria o `ToString()` do `Percentual`). Já a **interpolação** (`$"{p}"`) chama o `ToString()` e mostra `37.5%`. Moral: operadores implícitos são **cômodos**, mas podem **mudar qual sobrecarga é escolhida**, e vale testar o que é impresso.

::: dica Quando usar operador implícito?
Só quando a conversão for **segura, óbvia e sem perda**. `Percentual → double` cumpre. O contrário (`double → Percentual`) **não** deveria ser implícito: ele pode **falhar** (um 120), e erros não devem acontecer "por baixo dos panos". Para esse sentido, use um método explícito (`new Percentual(...)`, `TryParse`).
:::

## 2) Testes

```cs Habitos.Testes/Cap114Tests.cs
using Jornada.Habitos.Cap114;

namespace Habitos.Testes;

public class Cap114Tests
{
    [Theory]
    [InlineData(-1)]
    [InlineData(100.5)]
    [InlineData(double.NaN)]
    public void Percentual_fora_da_faixa_nao_consegue_existir(double valor) =>
        Assert.Throws<ArgumentOutOfRangeException>(() => new Percentual(valor));

    [Fact]
    public void Percentual_vira_double_sozinho_e_se_escreve_com_sinal()
    {
        double d = new Percentual(37.5);
        Assert.Equal(37.5, d);
        Assert.Equal("37.5%", new Percentual(37.5).ToString());
        Assert.Equal("50%", new Percentual(50).ToString());
    }

    [Theory]
    [InlineData("42", 42)]
    [InlineData(" 7.5 % ", 7.5)]
    [InlineData("100%", 100)]
    public void Parse_aceita_numero_com_ou_sem_sinal(string texto, double esperado) =>
        Assert.Equal(esperado, Leitor.Ler<Percentual>(texto).Valor);

    [Theory]
    [InlineData("abc")]
    [InlineData("101")]
    [InlineData("")]
    [InlineData(null)]
    public void TryParse_recusa_sem_lancar_excecao(string? texto)
    {
        Assert.False(Leitor.TentarLer<Percentual>(texto, out var p));
        Assert.Equal(default, p);
    }

    [Fact]
    public void Parse_invalido_lanca_uma_mensagem_que_ajuda() =>
        Assert.Contains("0 a 100", Assert.Throws<FormatException>(() => Leitor.Ler<Percentual>("200")).Message);

    [Fact]
    public void O_leitor_generico_funciona_com_tipos_da_biblioteca_padrao_tambem()
    {
        Assert.Equal(42, Leitor.Ler<int>("42"));
        Assert.Equal(new DateOnly(2026, 11, 2), Leitor.Ler<DateOnly>("2026-11-02"));
    }

    [Theory]
    [InlineData("Lia@Exemplo.com", "lia@exemplo.com")]
    [InlineData("  caio@time.dev  ", "caio@time.dev")]
    public void Email_valido_e_normalizado(string texto, string esperado)
    {
        Assert.True(Email.TryCriar(texto, out var email));
        Assert.Equal(esperado, email!.ToString());
    }

    [Theory]
    [InlineData("")]
    [InlineData("sem-arroba")]
    [InlineData("a@@b.com")]
    [InlineData("a@b")]
    [InlineData("a b@c.com")]
    [InlineData(null)]
    public void Email_invalido_nao_vira_objeto(string? texto)
    {
        Assert.False(Email.TryCriar(texto, out var email));
        Assert.Null(email);
    }

    [Fact]
    public void Distribuicao_que_fecha_cem_e_aceita()
    {
        var d = Distribuicao.Criar(("Foco", new Percentual(50)), ("Reuniões", new Percentual(30)), ("Aprender", new Percentual(20)));
        Assert.Equal(3, d.Partes.Count);
    }

    [Fact]
    public void Distribuicao_que_nao_fecha_cem_diz_quanto_somou()
    {
        var e = Assert.Throws<ArgumentException>(() =>
            Distribuicao.Criar(("Foco", new Percentual(70)), ("Reuniões", new Percentual(40))));

        Assert.Contains("110%", e.Message);
    }

    [Fact]
    public void Distribuicao_pode_ser_lida_de_texto()
    {
        var d = Distribuicao.Ler("Foco profundo 50, Reuniões 30, Aprender 20");
        Assert.Equal("Foco profundo", d.Partes[0].Nome);
        Assert.Equal(50, d.Partes[0].Parte.Valor);
        Assert.Throws<FormatException>(() => Distribuicao.Ler("SóNome"));
    }
}
```

O teste **`O_leitor_generico_funciona_com_tipos_da_biblioteca_padrao_tambem`** mostra a beleza do `IParsable`: o mesmo `Leitor.Ler<T>` lê `int` e `DateOnly`, tipos **da biblioteca padrão**, e o seu `Percentual`.

::: exercicio Uma faixa que nunca fica ao contrário
Crie o tipo `Faixa` (por exemplo, "de 25 a 50 minutos"): um `readonly record struct` com `Minimo` e `Maximo`, que **recusa** (`ArgumentException`) um mínimo maior que o máximo. Acrescente `Contem(int valor)` e `Sobrepoe(Faixa outra)`. Teste os limites (inclusive!) e a sobreposição.

--- resposta
```cs Jornada/Habitos/Exercicios/Cap114Faixa.cs
namespace Jornada.Habitos.Cap114;

/// <summary>Uma faixa de números (ex.: 25 a 50 minutos). Não existe faixa ao contrário.</summary>
public readonly record struct Faixa
{
    public int Minimo { get; }
    public int Maximo { get; }

    public Faixa(int minimo, int maximo)
    {
        if (minimo > maximo)
            throw new ArgumentException($"O mínimo ({minimo}) não pode passar do máximo ({maximo}).");

        Minimo = minimo;
        Maximo = maximo;
    }

    public bool Contem(int valor) => valor >= Minimo && valor <= Maximo;

    public bool Sobrepoe(Faixa outra) => Minimo <= outra.Maximo && outra.Minimo <= Maximo;

    public override string ToString() => $"{Minimo}–{Maximo}";
}
```

A regra de sobreposição: duas faixas **se tocam** se o início de uma **não passa** do fim da outra, **nos dois sentidos** (`Minimo <= outra.Maximo && outra.Minimo <= Maximo`). Repare em `Contem`: os limites são **inclusivos**, e o teste confere isso. A `Faixa` **não existe** ao contrário, e quem a recebe pode **confiar**.
:::

::: resumo
- Para o autor, **restrições não são o contrário da criatividade**: o caso do museu mostra uma "**tradicionalidade inovadora**" em que limites rígidos **liberam** a invenção.
- Em código, **objetos de valor** (`Percentual`, `Email`, `Distribuicao`) **só existem se válidos**: valida-se **na borda**, e **dentro** do sistema ninguém confere nada.
- **`IParsable<T>`** padroniza "ler de texto" (`Parse`, `TryParse`); o genérico `where T : IParsable<T>` chama **`T.Parse`** por meio de **membros estáticos abstratos** (C# 11).
- Um **operador implícito** é cômodo (`Percentual → double`), mas só vale se **seguro e sem perda**; pode mudar a sobrecarga escolhida (`WriteLine(double)`).
- Nem toda restrição liberta: **escolha limites que protejam** o resto, e explique-os a quem convive com eles.
:::

::: quiz
? O que é "tradicionalidade inovadora", no caso do museu?
+ Aceitar fielmente uma restrição herdada e inovar dentro dela
- Ignorar a tradição para inovar
- Copiar sem alterar nada
- Reformar tudo do zero
> Os arquitetos usaram os limites como catalisador criativo.

? Qual a principal vantagem de um objeto de valor como `Percentual`?
+ Se ele existe, é válido: o resto do código não precisa conferir
- É mais rápido que `double`
- Evita o uso de testes
- Aceita qualquer número
> A validação fica em um lugar só, na criação.

? O que a restrição `where T : IParsable<T>` permite?
+ Chamar `T.Parse(...)` em um tipo genérico
- Converter `T` em texto
- Criar listas de `T`
- Evitar nulos
> É um membro estático abstrato, recurso do C# 11.

? Por que `Console.WriteLine(p)` imprimiu `37,5` em vez de `37.5%`?
+ Porque o operador implícito fez o compilador escolher `WriteLine(double)`
- Porque o `ToString` está errado
- Porque o console arredonda
- Porque `Percentual` é nulo
> A interpolação `$"{p}"` chama o `ToString()` e mostra `37.5%`.

? Quando um operador implícito é uma boa ideia?
+ Quando a conversão é segura, óbvia e nunca falha nem perde informação
- Sempre que possível
- Quando a conversão pode falhar
- Nunca
> `double → Percentual` poderia falhar, por isso não deveria ser implícito.

? Por que a `Distribuicao` informa "As partes somam 110%..." no erro?
+ Porque uma boa mensagem diz o que está errado e quanto
- Para ocupar espaço
- Porque o C# exige
- Porque é obrigatório por lei
> Mensagens claras ajudam a corrigir sem tentativa e erro.
:::

::: cartas
Restrição libertadora :: Limite bem escolhido que libera criatividade e tira peso da cabeça.
Tradicionalidade inovadora :: Inovar dentro e a partir de uma herança que se respeita.
Objeto de valor :: Tipo pequeno que representa um valor do domínio e garante suas regras.
Validar na borda :: Conferir a entrada em um só ponto; dentro do sistema, tudo já é válido.
IParsable<T> :: Interface do .NET para tipos que se leem de texto (`Parse`, `TryParse`).
Membro static abstract :: Membro estático exigido por uma interface (C# 11); permite `T.Parse(...)`.
Operador implícito :: Conversão automática de um tipo para outro, sem escrever `(tipo)`.
Sobrecarga de método :: Várias versões do mesmo método com parâmetros diferentes.
:::
