---
id: leis-lei10
order: 71
kind: capitulo
num: 51
group: C# & Mentalidade · Parte 12 · As 16 leis do sucesso em C#
title: Lei 10 — Desenvolva uma personalidade agradável
subtitle: Código também tem "personalidade": nomes que se explicam, APIs que não surpreendem, mensagens de erro que ajudam, TryParse, ToString e revisões de código gentis — porque quem usa merece ser bem tratado.
minutes: 50
hue: 320
objectives:
- Entender a lei da **personalidade agradável** como **empatia, clareza e respeito**
- Projetar **APIs que não surpreendem** (princípio da menor surpresa): nomes, opções e valores padrão
- Evitar a **armadilha do `bool`** com `enum` e *options records*
- Escrever **mensagens de erro** que dizem o quê, por quê e o que fazer
- Usar o padrão **`TryParse`** e `ToString` para tipos amigáveis; escrever **comentários de revisão** gentis
origem: livros/03-leis-do-sucesso-csharp/content/70-lei10.md
---

::: vila Duas mensagens de erro
Duas bibliotecas falham com o mesmo problema. A primeira diz: `NullReferenceException: Object reference not set to an instance of an object`. A segunda: *"Não foi possível entregar o pedido 8812: o cliente 42 não tem endereço de entrega. Cadastre um endereço em Perfil › Endereços e tente de novo."* Lia pode escolher qual das duas **gostaria de manter** — e qual **gostaria de usar**. Personalidade agradável, em software, é isso.
:::

::: ilustracao img/leis/lei10.svg | Personalidade agradável: mensagens que dizem o que fazer e tratam bem quem as lê.
:::

## A lei, em uma frase

> Ninguém obtém sucesso duradouro sendo arrogante, ganancioso, invejoso ou egocêntrico: é preciso ter uma **personalidade agradável**, baseada em **empatia, bondade, integridade e honestidade**. Para ser agradável: **ouça de verdade** (sem interromper, sem concluir a frase do outro, perguntando em vez de supor), **evite reclamar, criticar e comparar**, e procure **trazer uma mensagem que faça bem** a quem a recebe.

A lição lista oito atitudes de escuta empática, entre elas: **ouvir com integridade**, **não interromper**, **não concluir a frase do outro**, **perguntar** em vez de supor e **não tentar impressionar**. E reforça: *"quem constrói uma casa é um artista; quem demole é apenas um operário"*.

## Da vida para o código

| O que a lei diz | O que isso significa para quem programa |
|---|---|
| Empatia e bondade | **Projetar pela perspectiva de quem usa** (a API é uma conversa) |
| Não surpreender, ser previsível | **Princípio da menor surpresa**: nomes e comportamentos coerentes |
| Ouvir e **perguntar**, não supor | **Parâmetros explícitos**, `TryParse`, requisitos esclarecidos antes de codar |
| Não interromper | Não "engolir" a intenção do chamador: respeitar `CancellationToken`, valores informados |
| Não criticar nem comparar | **Code review gentil**: descreva o código, não a pessoa |
| Integridade e honestidade | Mensagens de erro **verdadeiras** e úteis |
| Não tentar impressionar | **Simplicidade** em vez de código "esperto" |
| Mensagem que faz bem | Documentação, exemplos e erros que **ajudam** |

::: figura Duas respostas ao mesmo erro
<svg viewBox="0 0 740 190" role="img" aria-label="Mensagem hostil à esquerda e mensagem amigável à direita">
  <rect class="f-bad" x="10" y="20" width="340" height="120" rx="14"/>
  <text x="180" y="46" text-anchor="middle" font-weight="800" class="t-bad">Hostil</text>
  <text x="180" y="80" text-anchor="middle" font-size="12" class="t-mono">NullReferenceException:</text>
  <text x="180" y="98" text-anchor="middle" font-size="12" class="t-mono">Object reference not set to</text>
  <text x="180" y="116" text-anchor="middle" font-size="12" class="t-mono">an instance of an object</text>
  <rect class="f-good" x="390" y="20" width="340" height="140" rx="14"/>
  <text x="560" y="46" text-anchor="middle" font-weight="800" class="t-good">Agradável</text>
  <text x="560" y="76" text-anchor="middle" font-size="12">Não foi possível entregar o pedido 8812:</text>
  <text x="560" y="94" text-anchor="middle" font-size="12">o cliente 42 não tem endereço de entrega.</text>
  <text x="560" y="116" text-anchor="middle" font-size="12" font-weight="700">Cadastre um endereço em Perfil › Endereços</text>
  <text x="560" y="134" text-anchor="middle" font-size="12" font-weight="700">e tente de novo.</text>
  <text x="370" y="180" text-anchor="middle" class="t-dim" font-size="11">o quê · por quê · o que fazer</text>
</svg>
:::

## 1) Não deixe o chamador adivinhar: a armadilha do `bool`

Um método com vários `bool` é uma **conversa confusa**: quem lê `Exportar(dados, true, false)` precisa abrir a documentação para saber o que cada valor significa. Compare:

```cs Jornada/Lei10/Exportador.cs
using System.Text.Json;

namespace Jornada.Lei10;

public sealed record Pessoa(string Nome, int Idade);

public enum Formato { Csv, Json }

/// <summary>Opções com valores padrão sensatos: quem não souber o que escolher, acerta (o "poço do sucesso").</summary>
public sealed record OpcoesDeExportacao(Formato Formato = Formato.Csv, bool IncluirCabecalho = true);

public static class Exportador
{
    private static readonly JsonSerializerOptions JsonOpcoes = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

    public static string Exportar(IEnumerable<Pessoa> dados, OpcoesDeExportacao? opcoes = null)
    {
        opcoes ??= new OpcoesDeExportacao();

        if (opcoes.Formato == Formato.Json)
            return JsonSerializer.Serialize(dados, JsonOpcoes);

        var linhas = new List<string>();
        if (opcoes.IncluirCabecalho) linhas.Add("Nome;Idade");
        linhas.AddRange(dados.Select(p => $"{p.Nome};{p.Idade}"));
        return string.Join("\n", linhas);
    }
}
```

```cs Uso
var pessoas = new[] { new Pessoa("Ana", 30), new Pessoa("Bruno", 25) };

// Ruim (hipotético): Exportar(pessoas, true, false)   ← true, false... de quê?
// Bom: a intenção está escrita.
Console.WriteLine(Exportador.Exportar(pessoas));
Console.WriteLine("---");
Console.WriteLine(Exportador.Exportar(pessoas, new OpcoesDeExportacao(Formato.Json)));
Console.WriteLine("---");
Console.WriteLine(Exportador.Exportar(pessoas, new OpcoesDeExportacao(IncluirCabecalho: false)));
```

```saida
Nome;Idade
Ana;30
Bruno;25
---
[{"nome":"Ana","idade":30},{"nome":"Bruno","idade":25}]
---
Ana;30
Bruno;25
```

O código lê-se como uma frase: *"exportar pessoas, em JSON"*; *"exportar pessoas, sem cabeçalho"*. E o valor padrão faz o caminho mais comum **funcionar sem esforço**.

## 2) Mensagens que ajudam: o quê, por quê, o que fazer

A lição pede **honestidade e bondade**. Na prática, uma boa mensagem de erro:

1. diz **o que** aconteceu (sem jargão desnecessário);
2. diz **por que** (com os valores relevantes);
3. diz **o que fazer** a seguir (quando houver caminho);
4. **não culpa** o usuário.

```cs Jornada/Lei10/ValidadorDeSenha.cs
namespace Jornada.Lei10;

public sealed record ProblemaDeSenha(string OQueHouve, string Dica);

public static class ValidadorDeSenha
{
    public const int Minimo = 8;

    public static IReadOnlyList<ProblemaDeSenha> Validar(string? senha)
    {
        senha ??= "";
        var problemas = new List<ProblemaDeSenha>();

        if (senha.Length < Minimo)
            problemas.Add(new($"A senha tem {senha.Length} caractere(s); o mínimo é {Minimo}.",
                              "Que tal juntar duas palavras que só você conhece?"));

        if (!senha.Any(char.IsDigit))
            problemas.Add(new("Falta pelo menos um número.", "Acrescente, por exemplo, o ano de um dia especial."));

        if (!senha.Any(char.IsUpper))
            problemas.Add(new("Falta pelo menos uma letra maiúscula.", "Basta transformar a primeira letra."));

        return problemas;
    }
}
```

```cs Uso
foreach (var p in ValidadorDeSenha.Validar("abc12"))
    Console.WriteLine($"• {p.OQueHouve}\n    {p.Dica}");

Console.WriteLine(ValidadorDeSenha.Validar("Casa&Mar2026").Count == 0 ? "Senha aceita!" : "Ainda há ajustes.");
```

```saida
• A senha tem 5 caractere(s); o mínimo é 8.
    Que tal juntar duas palavras que só você conhece?
• Falta pelo menos uma letra maiúscula.
    Basta transformar a primeira letra.
Senha aceita!
```

Repare que o validador devolve **todos** os problemas de uma vez (em vez de lançar na primeira falha): o usuário não precisa descobrir um erro por tentativa. É o equivalente a **ouvir tudo antes de responder**.

::: dica Para exceções, o mesmo vale
Ao lançar uma exceção, inclua contexto: `throw new InvalidOperationException($"Não foi possível entregar o pedido {pedido.Id}: o cliente {cliente.Id} não tem endereço de entrega.")`. Quem estiver lendo o log às 3h da manhã agradece. E use o **tipo certo** de exceção (Lei 8).
:::

## 3) "Pergunte, não suponha": o padrão `TryParse`

Quem parseia texto **não deve supor** que ele é válido. O C# resolve isso com o par `Parse`/`TryParse`: `Parse` assume que está tudo certo (e lança se não estiver); `TryParse` **pergunta** e devolve `true`/`false`. Vamos criar um tipo que segue essa convenção — e tem uma boa `ToString`:

```cs Jornada/Lei10/Telefone.cs
namespace Jornada.Lei10;

public readonly record struct Telefone(string Ddd, string Numero)
{
    /// <summary>Aceita "11987654321", "(11) 98765-4321", "11 3456-7890"… Nunca lança por entrada inválida.</summary>
    public static bool TryParse(string? texto, out Telefone telefone)
    {
        telefone = default;
        if (string.IsNullOrWhiteSpace(texto)) return false;

        var digitos = new string(texto.Where(char.IsDigit).ToArray());
        if (digitos.Length is not (10 or 11)) return false;

        telefone = new Telefone(digitos[..2], digitos[2..]);
        return true;
    }

    public override string ToString() => Numero.Length == 9
        ? $"({Ddd}) {Numero[..5]}-{Numero[5..]}"
        : $"({Ddd}) {Numero[..4]}-{Numero[4..]}";
}
```

```cs Uso
foreach (var entrada in new[] { "11987654321", "(11) 98765-4321", "11 3456-7890", "123", "", null })
{
    var ok = Telefone.TryParse(entrada, out var fone);
    Console.WriteLine($"{(entrada ?? "null"),-18} → {(ok ? fone.ToString() : "não é um telefone")}");
}
```

```saida
11987654321        → (11) 98765-4321
(11) 98765-4321    → (11) 98765-4321
11 3456-7890       → (11) 3456-7890
123                → não é um telefone
                   → não é um telefone
null               → não é um telefone
```

Quem usa `Telefone` recebe **flexibilidade na entrada** (várias formas de escrever) e **uniformidade na saída** (sempre `(DD) NNNNN-NNNN`) — uma gentileza dupla (e uma amostra de **tolerância**, tema da Lei 15).

```cs JornadaTests/Lei10/TelefoneTests.cs
using Jornada.Lei10;
using Xunit;

public class TelefoneTests
{
    [Theory]
    [InlineData("11987654321", "(11) 98765-4321")]
    [InlineData("(11) 98765-4321", "(11) 98765-4321")]
    [InlineData("1134567890", "(11) 3456-7890")]
    public void Entradas_validas_sao_normalizadas(string entrada, string esperado)
    {
        Assert.True(Telefone.TryParse(entrada, out var t));
        Assert.Equal(esperado, t.ToString());
    }

    [Theory]
    [InlineData("123")]
    [InlineData("")]
    [InlineData(null)]
    public void Entradas_invalidas_nao_lancam_excecao(string? entrada) =>
        Assert.False(Telefone.TryParse(entrada, out _));
}
```

## 4) Revisão de código gentil: critique o código, não a pessoa

A lição proíbe **reclamar, criticar e comparar**. Em engenharia, isso não significa **não revisar** — significa revisar de um jeito que **constrói** (o artista que edifica, não o operário que demole). Uma estrutura simples para comentários:

```cs Jornada/Lei10/ComentarioDeRevisao.cs
namespace Jornada.Lei10;

public sealed record ComentarioDeRevisao(string Observacao, string Motivo, string Sugestao, string? Pergunta = null)
{
    public override string ToString() =>
        $"Observação: {Observacao}\nPor quê: {Motivo}\nSugestão: {Sugestao}" +
        (Pergunta is null ? "" : $"\nO que você acha? {Pergunta}");
}
```

```cs Uso
// ✘ Hostil
Console.WriteLine("\"Isso está horrível, ninguém entende o que você quis fazer.\"");
Console.WriteLine();

// ✔ Agradável
Console.WriteLine(new ComentarioDeRevisao(
    Observacao: "O método Processar() tem 80 linhas e faz validação, cálculo e envio de e-mail.",
    Motivo:     "Fica difícil testar cada parte separadamente (SRP).",
    Sugestao:   "Extrair CalcularTotal() e EnviarConfirmacao() em métodos privados, ou em classes.",
    Pergunta:   "Topa fazermos isso juntos em um par rápido?"));
```

```saida
"Isso está horrível, ninguém entende o que você quis fazer."

Observação: O método Processar() tem 80 linhas e faz validação, cálculo e envio de e-mail.
Por quê: Fica difícil testar cada parte separadamente (SRP).
Sugestão: Extrair CalcularTotal() e EnviarConfirmacao() em métodos privados, ou em classes.
O que você acha? Topa fazermos isso juntos em um par rápido?
```

::: pense A escuta empática no dia a dia
As oito atitudes da lição têm paralelos diretos no trabalho técnico:

| Atitude de escuta | No trabalho com software |
|---|---|
| **Ouvir com integridade** | Ler o requisito inteiro (e o *ticket* inteiro) antes de responder |
| **Não interromper** | Deixar quem explica o problema terminar |
| **Não concluir a frase do outro** | Não presumir a causa do *bug* antes de reproduzi-lo |
| **Perguntar** | "O que acontece se o cliente não tiver endereço?" — antes de codar |
| **Não tentar impressionar** | Preferir código simples a código "esperto" |
| **Não se distrair** | Foco na reunião e na revisão (Lei 12) |
:::

::: cuidado Agradável não é bajulador
Ser agradável **não** é concordar com tudo, omitir problemas ou elogiar código ruim: é **honestidade com respeito**. Uma API "agradável" que esconde erros (retornando `null` ou um valor padrão silenciosamente) é **desonesta** — viola também a Lei 8 e a Lei 16. Seja claro **e** gentil.
:::

::: exercicio Humanize esta exceção
O código abaixo lança uma exceção pouco amigável. Reescreva-o para (1) dizer **o quê/por quê/o que fazer** e (2) oferecer uma versão `TryObterPorId` que **não lança**.

```cs RepositorioDeClientes.cs (trecho)
public Cliente ObterPorId(int id)
{
    var c = clientes.FirstOrDefault(x => x.Id == id);
    if (c == null) throw new Exception("erro");
    return c;
}
```

--- resposta
```cs RepositorioDeClientes.cs (trecho)
public Cliente ObterPorId(int id)
{
    return TryObterPorId(id, out var c)
        ? c!
        : throw new KeyNotFoundException(
            $"Não existe cliente com id {id}. Verifique se o id foi digitado corretamente ou cadastre o cliente antes de continuar.");
}

public bool TryObterPorId(int id, out Cliente? cliente)
{
    cliente = clientes.FirstOrDefault(x => x.Id == id);
    return cliente is not null;
}
```
Ganhos: tipo de exceção correto (`KeyNotFoundException`), mensagem com o valor e a ação sugerida, e uma alternativa que **pergunta** (`Try…`) em vez de **supor** — para quem prefere tratar a ausência sem exceção.
:::

::: resumo
- **Lei 10:** sucesso duradouro vem de uma personalidade agradável — **empatia, integridade, escuta** — e não de arrogância, crítica e comparação.
- No código: **APIs que não surpreendem** (nomes claros, valores padrão sensatos, `enum`/*options* no lugar de `bool`s soltos).
- **Mensagens de erro** dizem o quê, por quê e o que fazer, sem culpar; devolva **todos** os problemas de validação de uma vez.
- **`TryParse`** é o "pergunte, não suponha": aceite entradas flexíveis, normalize a saída (`ToString`).
- **Revisão de código gentil:** descreva o código, motive, sugira, pergunte — construir, não demolir.
- Agradável ≠ bajulador: **honestidade com respeito**.
:::

::: quiz
? O que é o "poço do sucesso" no projeto de uma API?
+ Valores padrão e design que fazem o caminho mais comum funcionar sem esforço
- Um tipo de exceção
- Um padrão de projeto GoF
- Uma ferramenta de build
> Quem não sabe o que escolher, acerta.

? Por que evitar vários `bool` como parâmetros?
+ Porque quem lê a chamada não sabe o que cada valor significa
- Porque bool consome muita memória
- Porque o compilador proíbe
- Porque dificulta o uso de LINQ
> Prefira enums, nomes e options records.

? Qual a vantagem do padrão `TryParse`?
+ Perguntar se a entrada é válida sem lançar exceção, deixando o chamador decidir
- Ser mais lento que Parse
- Ocultar o erro
- Evitar a validação
> "Pergunte, não suponha."

? O que uma boa mensagem de erro deve conter?
+ O que aconteceu, por quê (com valores) e o que fazer a seguir, sem culpar
- Apenas um código numérico
- O stack trace completo para o usuário final
- Humor e ironia
> Honestidade com bondade.

? Como deve ser um comentário de code review gentil?
+ Descrever o código (não a pessoa), explicar o motivo, sugerir e perguntar
- Apontar quem errou
- Comparar com outros desenvolvedores
- Evitar sugestões
> Criticar o código, não a pessoa.

? Por que "agradável" não é o mesmo que "bajulador"?
+ Porque honestidade e clareza continuam necessárias — com respeito
- Porque bajular é mais eficiente
- Porque não se deve apontar erros
- Porque agradável dispensa testes
> Uma API que esconde erros é desonesta.
:::

::: cartas
Personalidade agradável :: Empatia, integridade e respeito nas relações — e nas APIs.
Menor surpresa :: O comportamento de uma API deve ser o que o usuário espera.
Armadilha do bool :: Parâmetros booleanos soltos que não dizem o que significam.
Options record :: Objeto imutável com valores padrão para configurar uma operação.
TryParse :: Convenção: tenta converter e devolve bool, sem lançar exceção.
Mensagem de erro útil :: Diz o quê, por quê e o que fazer.
Code review gentil :: Observação, motivo, sugestão e pergunta — sobre o código, não a pessoa.
Poço do sucesso :: Design que conduz o usuário ao uso correto por padrão.
:::
