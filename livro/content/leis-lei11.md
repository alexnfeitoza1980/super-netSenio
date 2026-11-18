---
id: leis-lei11
order: 72
kind: capitulo
num: 52
group: C# & Mentalidade · Parte 12 · As 16 leis do sucesso em C#
title: Lei 11 — Pense com precisão
subtitle: Foco no que importa e clareza nos fatos: double × decimal, value objects (Dinheiro, Email), datas com DateTimeOffset, enums no lugar de strings e "torne o estado inválido impossível".
minutes: 55
hue: 55
objectives:
- Entender a lei do **pensamento preciso**: separar o que é fato e relevante do que é ruído
- Escolher **tipos numéricos corretos** (`decimal`, `checked`) e entender os erros de ponto flutuante
- Criar **value objects** (`Dinheiro`, `Email`) que carregam regras e impedem erros
- Representar **tempo** corretamente (`DateOnly`, `DateTimeOffset`) e **estados** com `enum`
- Aplicar "**parse, don't validate**": depois da fronteira, só existem dados válidos
origem: livros/03-leis-do-sucesso-csharp/content/71-lei11.md
---

::: vila Os centavos que sumiram
O financeiro reclamou: *"o relatório diz R$ 99,99 e o banco diz R$ 100,00"*. Lia investiga e encontra três culpados. Um `double` onde deveria haver `decimal`. Um "rateio" que dividia R$ 100 em três partes de R$ 33,33 — **perdendo um centavo**. E uma data lida como "meia-noite local" por um sistema e "meia-noite UTC" por outro. Nenhum bug era complicado; todos nasceram de **pensamento impreciso**.
:::

::: ilustracao img/leis/lei11.svg | Precisão: para dinheiro e dados que importam, o tipo certo evita erros invisíveis.
:::

## A lei, em uma frase

> Pensamentos criam coisas, então **pense com precisão**: escolha **deliberadamente** onde colocar seu foco, mantenha a atenção no que **ajuda a alcançar o propósito** e trate como irrelevante o resto; e use suas emoções como **sinal** — quando algo incomoda, há algo que você quer e está ignorando.

Dois ensinamentos úteis: *qualquer fato que ajude a alcançar o propósito é importante; os demais são distração* — e **pensar com precisão** é **definir com exatidão o que se quer** e manter o pensamento ali. Para o trabalho técnico, a precisão aparece nos **dados**, nos **tipos** e nas **regras**.

## Da vida para o código

| O que a lei diz | O que isso significa para quem programa |
|---|---|
| Pensar com precisão | **Tipos precisos** que dizem exatamente o que o dado é |
| Só os fatos relevantes importam | **Modelar só o essencial** do domínio (abstração, cap. 7) |
| Definir exatamente o que se quer | **Regras explícitas** (arredondamento, moeda, fuso) |
| Emoção negativa = sinal | **Falha de teste / *warning*** = sinal de que algo está impreciso |
| Foco no que se quer, não no que se teme | **"Torne o estado inválido impossível"** em vez de checar depois |
| Atenção de águia: ver o ponto crucial | **Value objects** concentram a regra num único lugar |

::: figura Da imprecisão à precisão
<svg viewBox="0 0 740 160" role="img" aria-label="Dados brutos, tipos precisos, decisões corretas">
  <rect class="f-bad" x="10" y="40" width="200" height="70" rx="12"/><text x="110" y="68" text-anchor="middle" font-weight="800">Dados soltos</text><text x="110" y="90" text-anchor="middle" font-size="11" class="t-dim">double, string, DateTime</text>
  <rect class="f-info" x="270" y="40" width="200" height="70" rx="12"/><text x="370" y="68" text-anchor="middle" font-weight="800">Tipos precisos</text><text x="370" y="90" text-anchor="middle" font-size="11" class="t-dim">decimal, Dinheiro, Email, enum</text>
  <rect class="f-good" x="530" y="40" width="200" height="70" rx="12"/><text x="630" y="68" text-anchor="middle" font-weight="800">Decisões corretas</text><text x="630" y="90" text-anchor="middle" font-size="11" class="t-dim">regras num só lugar</text>
  <path class="arrow" d="M212 75H268"/><path class="arrow" d="M472 75H528"/>
  <text x="370" y="140" text-anchor="middle" class="t-dim" font-size="12">A precisão entra pelos tipos, na fronteira — não nos <tspan font-style="italic">if</tspan>s espalhados.</text>
</svg>
:::

## 1) Números: quando "quase certo" está errado

Computadores representam `double` em **binário**: `0,1` e `0,2` não têm representação exata. O resultado é conhecido:

```cs Uso
using System.Globalization;

Console.WriteLine(0.1 + 0.2 == 0.3);                                        // double
Console.WriteLine(0.1m + 0.2m == 0.3m);                                     // decimal
Console.WriteLine((0.1 + 0.2).ToString("R", CultureInfo.InvariantCulture)); // o valor "de verdade"

int x = int.MaxValue;
Console.WriteLine(unchecked(x + 1));                                        // dá a volta, em silêncio
try { Console.WriteLine(checked(x + 1)); }
catch (OverflowException) { Console.WriteLine("estouro detectado"); }
```

```saida
False
True
0.30000000000000004
-2147483648
estouro detectado
```

Regras práticas:

| Situação | Use |
|---|---|
| **Dinheiro** e quantidades decimais exatas | `decimal` (e uma regra de arredondamento explícita) |
| Medidas científicas, gráficos, estatística | `double` (e compare com **tolerância**) |
| Contadores, índices | `int`/`long` — e pense em **estouro** (`checked`) |
| Identificadores | `Guid`, `long` ou *value objects*; nunca `double` |

## 2) *Value objects*: o tipo que carrega a regra

Dinheiro **não é** um `decimal`: é um valor **com moeda** e com **regras** (não se soma BRL com USD; rateio não pode perder centavos). Um *value object* concentra tudo isso:

```cs Jornada/Lei11/Dinheiro.cs
using System.Globalization;

namespace Jornada.Lei11;

public enum Moeda { BRL, USD, EUR }

public readonly record struct Dinheiro(decimal Valor, Moeda Moeda)
{
    private static readonly CultureInfo Br = new("pt-BR");

    public static Dinheiro Reais(decimal valor) =>
        new(Math.Round(valor, 2, MidpointRounding.AwayFromZero), Moeda.BRL);

    public static Dinheiro operator +(Dinheiro a, Dinheiro b)
    {
        ExigirMesmaMoeda(a, b);
        return new Dinheiro(a.Valor + b.Valor, a.Moeda);
    }

    public static Dinheiro operator -(Dinheiro a, Dinheiro b)
    {
        ExigirMesmaMoeda(a, b);
        return new Dinheiro(a.Valor - b.Valor, a.Moeda);
    }

    public static Dinheiro operator *(Dinheiro a, decimal fator) =>
        new(Math.Round(a.Valor * fator, 2, MidpointRounding.AwayFromZero), a.Moeda);

    /// <summary>Divide em partes iguais sem perder (nem criar) centavos: R$ 100,00 / 3 → 33,34 + 33,33 + 33,33.</summary>
    public IReadOnlyList<Dinheiro> Ratear(int partes)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(partes);
        if (Valor < 0) throw new InvalidOperationException("Rateio de valores negativos não é suportado.");

        long centavos = (long)Math.Round(Valor * 100m, MidpointRounding.AwayFromZero);
        long basico = centavos / partes;
        long resto  = centavos % partes;                 // centavos que sobram: um para cada uma das primeiras partes
        var moeda   = Moeda;                             // em structs, lambdas não acessam 'this': copie para uma local

        return Enumerable.Range(0, partes)
                         .Select(i => new Dinheiro((basico + (i < resto ? 1 : 0)) / 100m, moeda))
                         .ToList();
    }

    private static void ExigirMesmaMoeda(Dinheiro a, Dinheiro b)
    {
        if (a.Moeda != b.Moeda)
            throw new InvalidOperationException($"Não é possível combinar {a.Moeda} com {b.Moeda} sem uma conversão explícita.");
    }

    public override string ToString() => $"{Moeda} {Valor.ToString("N2", Br)}";
}
```

```cs Uso
var conta = Dinheiro.Reais(100);
var partes = conta.Ratear(3);

Console.WriteLine(string.Join(" · ", partes));
Console.WriteLine("Soma das partes: " + partes.Aggregate((a, b) => a + b));

var naive = 100m / 3m;                                              // o jeito impreciso
Console.WriteLine($"Ingênuo: 3 × {Math.Round(naive, 2)} = {Math.Round(naive, 2) * 3}");

try { _ = Dinheiro.Reais(10) + new Dinheiro(5, Moeda.USD); }
catch (InvalidOperationException e) { Console.WriteLine(e.Message); }
```

```saida
BRL 33,34 · BRL 33,33 · BRL 33,33
Soma das partes: BRL 100,00
Ingênuo: 3 × 33,33 = 99,99
Não é possível combinar BRL com USD sem uma conversão explícita.
```

Repare como o erro do começo da história ("R$ 99,99") aparece no cálculo **ingênuo** — e desaparece quando a regra vive **dentro do tipo**. Toda vez que alguém usar `Dinheiro`, a regra vem junto, sem precisar lembrar dela.

```cs JornadaTests/Lei11/DinheiroTests.cs
using Jornada.Lei11;
using Xunit;

public class DinheiroTests
{
    [Theory]
    [InlineData(100, 3)]
    [InlineData(10, 4)]
    [InlineData(0.05, 2)]
    [InlineData(1, 7)]
    public void Ratear_nunca_perde_nem_cria_centavos(double valor, int partes)
    {
        var total = Dinheiro.Reais((decimal)valor);
        var rateio = total.Ratear(partes);

        Assert.Equal(partes, rateio.Count);
        Assert.Equal(total.Valor, rateio.Sum(p => p.Valor));
    }

    [Fact]
    public void Moedas_diferentes_nao_se_misturam() =>
        Assert.Throws<InvalidOperationException>(() => Dinheiro.Reais(1) + new Dinheiro(1, Moeda.EUR));
}
```

## 3) "Parse, don't validate": só entra o que é válido

Em vez de validar o mesmo e-mail em dez lugares, **transforme-o uma vez** em um tipo que **só existe se for válido**. Depois da fronteira (formulário, API, arquivo), o resto do código **não precisa pensar nisso** — a precisão ficou no tipo:

```cs Jornada/Lei11/Email.cs
using System.Net.Mail;

namespace Jornada.Lei11;

public sealed record Email
{
    public string Valor { get; }

    private Email(string valor) => Valor = valor;

    public static bool TryCriar(string? texto, out Email? email)
    {
        email = null;
        if (string.IsNullOrWhiteSpace(texto)) return false;

        var limpo = texto.Trim();
        if (!MailAddress.TryCreate(limpo, out var endereco) || endereco.Address != limpo) return false;

        email = new Email(limpo.ToLowerInvariant());      // normaliza: um único jeito de escrever
        return true;
    }

    public static Email Criar(string? texto) =>
        TryCriar(texto, out var e) ? e! : throw new FormatException($"'{texto}' não é um e-mail válido.");

    public override string ToString() => Valor;
}
```

```cs Uso
foreach (var entrada in new[] { "Ana@Exemplo.com ", "ana@", "sem-arroba", "" })
{
    var ok = Email.TryCriar(entrada, out var email);
    Console.WriteLine($"'{entrada}' → {(ok ? email!.ToString() : "inválido")}");
}

// A partir daqui, um método que recebe 'Email' nunca precisa revalidar.
static string Boas_vindas(Email e) => $"Enviando boas-vindas para {e}";
Console.WriteLine(Boas_vindas(Email.Criar("bia@exemplo.com")));
```

```saida
'Ana@Exemplo.com ' → ana@exemplo.com
'ana@' → inválido
'sem-arroba' → inválido
'' → inválido
Enviando boas-vindas para bia@exemplo.com
```

## 4) Tempo e estados: dois lugares onde a imprecisão adora morar

### Datas e horas

`DateTime` mistura "data", "hora" e uma noção vaga de fuso (`Kind`). Prefira tipos que digam **exatamente** o que significam:

| Preciso de… | Tipo | Exemplo |
|---|---|---|
| Um **instante** no tempo (reunião, log, evento) | `DateTimeOffset` | `2026-10-26 09:00 -03:00` |
| Só uma **data** (aniversário, vencimento) | `DateOnly` | `2026-10-26` |
| Só um **horário** do dia (abertura da loja) | `TimeOnly` | `09:00` |
| Uma **duração** | `TimeSpan` | `01:30:00` |

```cs Uso
var reuniao = new DateTimeOffset(2026, 10, 26, 9, 0, 0, TimeSpan.FromHours(-3));   // 9h em São Paulo

Console.WriteLine($"Local: {reuniao:yyyy-MM-dd HH:mm zzz}   UTC: {reuniao.UtcDateTime:yyyy-MM-dd HH:mm}");
Console.WriteLine("Mesmo instante em Lisboa (+00:00): " + reuniao.ToOffset(TimeSpan.Zero).ToString("HH:mm"));
```

```saida
Local: 2026-10-26 09:00 -03:00   UTC: 2026-10-26 12:00
Mesmo instante em Lisboa (+00:00): 12:00
```

Grave **instantes em UTC** (ou com *offset*) e converta para o fuso do usuário **só na tela**. E, para fusos com horário de verão, use `TimeZoneInfo` com identificadores IANA (`America/Sao_Paulo`).

### Estados com `enum`, não com `string`

```cs Jornada/Lei11/StatusDoPedido.cs
namespace Jornada.Lei11;

public enum StatusDoPedido { Novo, Pago, Enviado, Entregue, Cancelado }

public static class Regras
{
    public static bool PodeCancelar(StatusDoPedido status) => status switch
    {
        StatusDoPedido.Novo or StatusDoPedido.Pago                                      => true,
        StatusDoPedido.Enviado or StatusDoPedido.Entregue or StatusDoPedido.Cancelado   => false,
        _ => throw new ArgumentOutOfRangeException(nameof(status), status, "Status desconhecido.")
    };
}
```

```cs Uso
// Com strings: dois sistemas escreveram "Pago" e "pago"…
string statusA = "Pago", statusB = "pago";
Console.WriteLine($"\"{statusA}\" == \"{statusB}\"? {statusA == statusB}");

// Com enum: só existem os valores que você definiu — e o switch avisa se esquecer algum.
Console.WriteLine($"Pode cancelar um pedido Enviado? {Regras.PodeCancelar(StatusDoPedido.Enviado)}");
Console.WriteLine($"Pode cancelar um pedido Pago?    {Regras.PodeCancelar(StatusDoPedido.Pago)}");
```

```saida
"Pago" == "pago"? False
Pode cancelar um pedido Enviado? False
Pode cancelar um pedido Pago?    True
```

::: pense Emoção como sinal
A lei diz que uma emoção negativa **revela** que você quer algo e o seu foco está desviado. Em programação, o equivalente é o **incômodo** (um teste vermelho, um *warning*, um `if` esquisito): ele aponta onde a modelagem está **imprecisa**. Pergunte-se: *"O que eu quero que este código garanta e ainda estou deixando ao acaso?"* — e a resposta costuma ser um **tipo novo**.
:::

::: cuidado Precisão não é complexidade
Criar um *value object* para cada `string` do sistema vira cerimônia. Use a regra: **crie um tipo quando houver regra a proteger** (validação, unidade, formato, invariante) **ou confusão provável** (dois `int` que significam coisas diferentes). Para o resto, tipos primitivos bem nomeados bastam.
:::

::: exercicio Percentual preciso
Crie um *value object* `Percentual` que:
1. só aceita valores de **0 a 100** (lance `ArgumentOutOfRangeException` caso contrário);
2. tenha o método `Aplicar(Dinheiro)` que devolve o **desconto** (arredondado como `Dinheiro`);
3. tenha `ToString()` como `"15%"`.
Escreva um teste: 10% de R$ 10,05 = R$ 1,01.

--- resposta
```cs
using Jornada.Lei11;

public readonly record struct Percentual
{
    public decimal Valor { get; }

    public Percentual(decimal valor)
    {
        if (valor is < 0 or > 100)
            throw new ArgumentOutOfRangeException(nameof(valor), valor, "O percentual deve estar entre 0 e 100.");
        Valor = valor;
    }

    public Dinheiro Aplicar(Dinheiro dinheiro) => dinheiro * (Valor / 100m);

    public override string ToString() => $"{Valor:0.##}%";
}

public class PercentualTests
{
    [Fact]
    public void Dez_por_cento_de_10_05_e_1_01() =>
        Assert.Equal(1.01m, new Percentual(10).Aplicar(Dinheiro.Reais(10.05m)).Valor);

    [Fact]
    public void Fora_do_intervalo_e_recusado() =>
        Assert.Throws<ArgumentOutOfRangeException>(() => new Percentual(101));
}
```
Observe: com `readonly record struct` e construtor explícito, o `default(Percentual)` ainda existe (valor 0) — uma pegadinha de *structs*. Para garantir que **nenhuma** instância inválida exista, use `class`/`record class` ou valide em `Aplicar`.
:::

::: resumo
- **Lei 11:** pense com precisão — escolha o foco, trate o resto como ruído, use o incômodo como sinal.
- **Números:** `decimal` para dinheiro; `double` tem erro binário; `checked` para estouros; defina **arredondamento** explicitamente.
- **Value objects** (`Dinheiro`, `Email`, `Percentual`) concentram regras e impedem combinações inválidas (BRL + USD).
- **Rateio** sem perder centavos; **normalização** na entrada (e-mail em minúsculas).
- **"Parse, don't validate":** valide na fronteira e carregue um tipo que só existe se for válido.
- **Tempo:** `DateTimeOffset` para instantes, `DateOnly`/`TimeOnly`/`TimeSpan` para o que realmente é; grave em UTC.
- **Estados:** `enum` + `switch` expression no lugar de `string`.
- Precisão **sem exagero**: um tipo novo onde há regra ou risco de confusão.
:::

::: quiz
? Por que `0.1 + 0.2 == 0.3` é `false` com double?
+ Porque double usa representação binária e não representa 0,1 e 0,2 exatamente
- Porque o C# tem um bug
- Porque 0,3 é irracional
- Porque o compilador arredonda para baixo
> Para dinheiro, use decimal.

? O que `checked(x + 1)` faz com `int.MaxValue`?
+ Lança OverflowException em vez de "dar a volta" silenciosamente
- Retorna zero
- Converte para long
- Compila mas ignora
> Evita erros silenciosos de estouro.

? Qual a principal vantagem de um value object como Dinheiro?
+ A regra (moeda, arredondamento, rateio) fica no tipo, em um só lugar
- Ser mais rápido que decimal
- Evitar testes
- Ocupar menos memória
> Quem usa o tipo recebe a regra "de brinde".

? O que significa "parse, don't validate"?
+ Converter a entrada em um tipo que só existe se for válido, em vez de revalidar em todo lugar
- Nunca validar entradas
- Validar só no banco de dados
- Usar regex em tudo
> A fronteira transforma; o interior confia no tipo.

? Qual tipo representa melhor um instante (uma reunião) com fuso?
+ DateTimeOffset
- DateOnly
- TimeSpan
- string
> DateTimeOffset carrega o offset em relação ao UTC.

? Por que preferir enum a string para estados?
+ Só existem os valores definidos, evita erros de digitação e o switch avisa casos esquecidos
- Porque enum é mais lento
- Porque string não permite igualdade
- Porque enum aceita qualquer texto
> "Pago" e "pago" são strings diferentes.
:::

::: cartas
Pensamento preciso :: Foco deliberado no que importa, com clareza sobre o que se quer.
decimal :: Tipo decimal exato, apropriado para dinheiro.
Value object :: Tipo imutável definido pelo valor, que carrega suas regras.
Parse, don't validate :: Transformar a entrada em um tipo válido na fronteira.
Rateio :: Divisão de um valor em partes sem perder centavos.
DateTimeOffset :: Instante com deslocamento (offset) em relação ao UTC.
Estado inválido impossível :: Modelagem que não permite construir dados inconsistentes.
checked :: Contexto que lança exceção em estouro aritmético.
:::
