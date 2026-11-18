---
id: leis-lei08
order: 69
kind: capitulo
num: 49
group: C# & Mentalidade · Parte 12 · As 16 leis do sucesso em C#
title: Lei 8 — Autocontrole
subtitle: Reconhecer, admitir, corrigir e aprender com cada erro. Guard clauses, exceções honestas, retentativas com recuo exponencial e um compilador que não deixa passar o que você preferiria ignorar.
minutes: 55
hue: 220
objectives:
- Entender a lei do **autocontrole**: domínio sobre o que entra na mente — e sobre como reagimos aos erros
- Aplicar os **quatro passos** (reconhecer, admitir, corrigir, aprender) ao tratamento de erros em C#
- Escrever **guard clauses** e exceções com contexto — e evitar `catch {}` e `throw ex;`
- Implementar **retentativas com recuo exponencial** só para falhas passageiras
- Usar **analisadores** e `TreatWarningsAsErrors` como "críticas bem-vindas"
origem: livros/03-leis-do-sucesso-csharp/content/68-lei08.md
---

::: vila O erro que alguém escondeu
Numa sexta, uma importação de pedidos falhou em silêncio. Ninguém soube por quê; um `catch { }` no meio do caminho "resolveu" o problema escondendo-o. Na segunda, o financeiro descobriu que 312 pedidos não existiam. Lia lê o código e percebe o mais difícil: o erro original foi **pequeno**. O que o tornou grande foi **como o código reagiu a ele**.
:::

::: ilustracao img/leis/lei08.svg | Autocontrole: tentar de novo com pausas crescentes, só para falhas passageiras.
:::

## A lei, em uma frase

> O autocontrole é a capacidade de **dominar o próprio pensamento** e canalizá-lo para o propósito; sem ele, criamos coisas por omissão — dizemos, fazemos e permitimos o que não queríamos. Uma de suas provas de fogo é como lidamos com os **erros**: **reconhecer, admitir, corrigir e aprender** (e não esconder, culpar outros ou matar o mensageiro).

A lição pede também outros treinos: **aceitar críticas** sem se irritar (e investigar se têm razão), **defender o ausente** em vez de aderir à maledicência, e permitir que entre na mente **apenas o que for útil** ao propósito.

## Da vida para o código

| O que a lei diz | O que isso significa para quem programa |
|---|---|
| Controlar o que entra na mente | **Validar** na fronteira: só dados válidos entram (*guard clauses*) |
| Reconhecer o erro | **Detectar** a falha cedo e de forma explícita |
| Admitir o erro | **Não engolir** exceções: registrar, expor, dar contexto |
| Corrigir o erro | **Compensar**, repetir, usar *fallback* — de forma explícita |
| Aprender com o erro | **Teste de regressão** + *post-mortem* (Lei 14) |
| Não matar o mensageiro | A exceção/*warning* **é** o mensageiro: respeite-o |
| Agradecer a crítica | **Analisadores** e *warnings* como erro no CI |
| Não criar por omissão | **Imutabilidade** e invariantes: o que não é permitido, não acontece |

::: figura Os quatro passos
<svg viewBox="0 0 740 150" role="img" aria-label="Reconhecer, admitir, corrigir, aprender: ciclo">
  <rect class="f-info" x="10" y="40" width="160" height="64" rx="12"/><text x="90" y="68" text-anchor="middle" font-weight="800">1 · Reconhecer</text><text x="90" y="88" text-anchor="middle" font-size="11" class="t-dim">validar e detectar</text>
  <rect class="f-warn" x="195" y="40" width="160" height="64" rx="12"/><text x="275" y="68" text-anchor="middle" font-weight="800">2 · Admitir</text><text x="275" y="88" text-anchor="middle" font-size="11" class="t-dim">expor com contexto</text>
  <rect class="f-good" x="380" y="40" width="160" height="64" rx="12"/><text x="460" y="68" text-anchor="middle" font-weight="800">3 · Corrigir</text><text x="460" y="88" text-anchor="middle" font-size="11" class="t-dim">retry, fallback, fix</text>
  <rect class="f-prim" x="565" y="40" width="165" height="64" rx="12"/><text x="647" y="68" text-anchor="middle" font-weight="800">4 · Aprender</text><text x="647" y="88" text-anchor="middle" font-size="11" class="t-dim">teste de regressão</text>
  <path class="arrow" d="M172 72H193"/><path class="arrow" d="M357 72H378"/><path class="arrow" d="M542 72H563"/>
  <path d="M647 106Q647 140 370 140Q90 140 90 106" stroke="var(--ink-3)" stroke-dasharray="5 4" fill="none" marker-end="url(#arr2)"/>
</svg>
:::

## O antipadrão: matar o mensageiro

Veja um jeito comum — e perigoso — de "tratar" um erro:

```cs Jornada/Lei08/Configuracao.cs
namespace Jornada.Lei08;

public static class Configuracao
{
    // ✘ Engole TUDO. Ninguém fica sabendo. A porta "0" circula pelo sistema como se fosse válida.
    public static int LerPortaRuim(string? texto)
    {
        try { return int.Parse(texto!); }
        catch { return 0; }
    }

    // ✔ 1) RECONHECER: valida na fronteira.  2) ADMITIR: erros claros, com contexto.
    public static int LerPorta(string? texto)
    {
        if (string.IsNullOrWhiteSpace(texto))
            throw new ArgumentException("A porta não foi informada.", nameof(texto));

        if (!int.TryParse(texto, out int porta))
            throw new FormatException($"A porta '{texto}' não é um número.");

        if (porta is < 1 or > 65535)
            throw new ArgumentOutOfRangeException(nameof(texto), porta, "A porta deve estar entre 1 e 65535.");

        return porta;
    }

    // ✔ 3) CORRIGIR: recupera, mas às claras — o problema é registrado, não escondido.
    public static int LerPortaOuPadrao(string? texto, int padrao, Action<string> registrar)
    {
        try
        {
            return LerPorta(texto);
        }
        catch (Exception e) when (e is ArgumentException or FormatException)   // só o que sabemos tratar
        {
            registrar($"porta inválida ({e.GetType().Name}) → usando {padrao}");
            return padrao;
        }
    }
}
```

```cs Uso
var avisos = new List<string>();

Console.WriteLine($"RUIM: \"80a\" → {Configuracao.LerPortaRuim("80a")}   (ninguém soube que deu erro)");
Console.WriteLine($"BOM : \"80a\" → {Configuracao.LerPortaOuPadrao("80a", 8080, avisos.Add)}");
Console.WriteLine($"BOM : \"9000\" → {Configuracao.LerPortaOuPadrao("9000", 8080, avisos.Add)}");
Console.WriteLine($"BOM : \"\" → {Configuracao.LerPortaOuPadrao("", 8080, avisos.Add)}");

Console.WriteLine("Avisos registrados:");
avisos.ForEach(a => Console.WriteLine("  " + a));
```

```saida
RUIM: "80a" → 0   (ninguém soube que deu erro)
BOM : "80a" → 8080
BOM : "9000" → 9000
BOM : "" → 8080
Avisos registrados:
  porta inválida (FormatException) → usando 8080
  porta inválida (ArgumentException) → usando 8080
```

E o quarto passo — **aprender** — é transformar o erro encontrado em um **teste de regressão**, para que ele nunca mais passe despercebido:

```cs JornadaTests/Lei08/ConfiguracaoTests.cs
using Jornada.Lei08;
using Xunit;

public class ConfiguracaoTests
{
    [Theory]
    [InlineData("80",    80)]
    [InlineData(" 8080 ", 8080)]
    [InlineData("65535", 65535)]
    public void Portas_validas(string texto, int esperado) =>
        Assert.Equal(esperado, Configuracao.LerPorta(texto));

    [Theory]
    [InlineData("")]
    [InlineData("0")]
    [InlineData("70000")]
    public void Portas_vazias_ou_fora_do_intervalo_sao_recusadas(string texto) =>
        Assert.Throws<ArgumentException>(() => Configuracao.LerPorta(texto));    // ArgumentOutOfRangeException herda de ArgumentException

    [Fact]
    public void Texto_nao_numerico_gera_FormatException() =>
        Assert.Throws<FormatException>(() => Configuracao.LerPorta("80a"));      // o caso que quebrou em produção: virou teste
}
```

::: dica Escolha bem o tipo da exceção
`ArgumentOutOfRangeException` **herda** de `ArgumentException`; `FormatException` **não**. Por isso o teste verifica cada contrato separadamente. Um tipo de exceção bem escolhido **comunica** o problema ao chamador — que pode tratar `FormatException` de um jeito e `ArgumentException` de outro (veja o `when` em `LerPortaOuPadrao`).
:::

::: dica Dois erros clássicos com exceções
**`catch { }`** (ou `catch (Exception) { }` vazio): engole o erro — o contrário de "admitir".
**`throw ex;`**: **zera o *stack trace*** e esconde onde o erro nasceu. Use **`throw;`** para repassar preservando a pilha, ou `throw new Falha("contexto", ex)` para **acrescentar contexto** mantendo a causa original em `InnerException`.
:::

## "Corrigir" tentando de novo — só quando faz sentido

Algumas falhas são **passageiras**: um timeout de rede, um banco ocupado. Tentar novamente é uma forma legítima de corrigir. Mas há regras: **limite de tentativas**, **espera crescente** (para não sobrecarregar o serviço doente — recuo exponencial) e **só repetir o que é passageiro** (repetir um erro de validação nunca ajuda).

```cs Jornada/Lei08/Persistencia.cs
namespace Jornada.Lei08;

public static class Persistencia
{
    public static async Task<T> ExecutarAsync<T>(
        Func<int, CancellationToken, Task<T>> operacao,
        int maxTentativas,
        Func<int, TimeSpan> atraso,
        Func<Exception, bool> ehPassageira,
        Action<string>? registrar = null,
        CancellationToken ct = default)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(maxTentativas, 1);

        for (int tentativa = 1; ; tentativa++)
        {
            try
            {
                return await operacao(tentativa, ct);
            }
            catch (Exception e) when (ehPassageira(e) && tentativa < maxTentativas)
            {
                var espera = atraso(tentativa);
                registrar?.Invoke($"tentativa {tentativa} falhou ({e.Message}); nova tentativa em {espera.TotalMilliseconds:F0} ms");
                await Task.Delay(espera, ct);
            }
            // falha não passageira, ou última tentativa: a exceção original sobe, intacta.
        }
    }

    /// <summary>100 ms, 200 ms, 400 ms, 800 ms… (cada tentativa dobra a espera).</summary>
    public static TimeSpan Exponencial(int tentativa, double baseMs = 100) =>
        TimeSpan.FromMilliseconds(baseMs * Math.Pow(2, tentativa - 1));
}
```

```cs Uso
var avisos = new List<string>();

// Uma operação que falha nas 2 primeiras vezes e funciona na 3ª.
var resultado = await Persistencia.ExecutarAsync(
    operacao: (tentativa, ct) => tentativa < 3
        ? Task.FromException<string>(new TimeoutException("tempo esgotado"))
        : Task.FromResult($"ok (na tentativa {tentativa})"),
    maxTentativas: 5,
    atraso: n => Persistencia.Exponencial(n),
    ehPassageira: e => e is TimeoutException,
    registrar: avisos.Add);

avisos.ForEach(Console.WriteLine);
Console.WriteLine("Resultado: " + resultado);

// Uma falha NÃO passageira não é repetida: sobe na primeira tentativa.
try
{
    await Persistencia.ExecutarAsync<string>(
        (_, _) => throw new ArgumentException("dado inválido"),
        maxTentativas: 5, atraso: n => TimeSpan.Zero, ehPassageira: e => e is TimeoutException);
}
catch (ArgumentException e)
{
    Console.WriteLine("não repetiu: " + e.Message);
}
```

```saida
tentativa 1 falhou (tempo esgotado); nova tentativa em 100 ms
tentativa 2 falhou (tempo esgotado); nova tentativa em 200 ms
Resultado: ok (na tentativa 3)
não repetiu: dado inválido
```

```cs JornadaTests/Lei08/PersistenciaTests.cs
using Jornada.Lei08;
using Xunit;

public class PersistenciaTests
{
    [Fact]
    public async Task Desiste_depois_do_limite_e_repassa_a_excecao_original()
    {
        int chamadas = 0;

        var e = await Assert.ThrowsAsync<TimeoutException>(() =>
            Persistencia.ExecutarAsync<int>(
                (_, _) => { chamadas++; throw new TimeoutException("sempre falha"); },
                maxTentativas: 3, atraso: _ => TimeSpan.Zero, ehPassageira: ex => ex is TimeoutException));

        Assert.Equal(3, chamadas);
        Assert.Equal("sempre falha", e.Message);
    }
}
```

::: pense Na prática, use uma biblioteca
Retentativas, **disjuntor** (*circuit breaker*: parar de chamar um serviço que está caído), **timeout** e **bulkhead** são tão comuns que existem bibliotecas maduras: **Polly** e `Microsoft.Extensions.Http.Resilience`. Escrever à mão, como acima, serve para **entender** o princípio; em produção, prefira o que é mantido e testado por outras pessoas (Lei 2). O princípio de autocontrole permanece: *falhar de forma controlada, nunca descontrolada*.
:::

## Autocontrole por construção

A melhor forma de evitar criar coisas "por omissão" é **tornar o erro impossível**. O C# oferece várias ferramentas:

| Ferramenta | O que impede |
|---|---|
| `readonly`, `init`, `record` | Mutação acidental de dados que deveriam ser estáveis |
| `sealed` | Herança que quebraria invariantes |
| `private set` / encapsulamento | Estados inválidos criados de fora |
| `required` (C# 11) | Esquecer de preencher campos essenciais |
| `string?` + nullable | `NullReferenceException` por omissão |
| `switch` *expression* sem `_` e `enum` | Esquecer de tratar um novo caso (aviso do compilador) |
| `ArgumentNullException.ThrowIfNull` & cia. | Argumentos inválidos entrarem no método |

E o compilador também pode ser o **"amigo que critica"**. Habilite os analisadores e trate avisos como erros no projeto:

```xml
<!-- Jornada.csproj -->
<PropertyGroup>
  <Nullable>enable</Nullable>
  <AnalysisLevel>latest-recommended</AnalysisLevel>
  <TreatWarningsAsErrors>true</TreatWarningsAsErrors>
</PropertyGroup>
```

Lembre-se da lição: **"Agradeça a crítica e comprometa-se a corrigir"**. Um *warning* ignorado é um mensageiro morto.

::: exercicio Regressão de verdade
1. Um colega reportou que `"80 80"` (com espaço no meio) foi aceito em outro módulo. Escreva **primeiro** o teste de regressão para `LerPorta("80 80")` (qual exceção você espera?) e confirme que a implementação atual já o recusa.
2. Escreva uma versão `ExecutarComPrazoAsync` de `Persistencia.ExecutarAsync` que abandone tudo (todas as tentativas) depois de um **prazo total** informado.

--- resposta
```cs
public class PortaComEspacoTests
{
    [Fact]
    public void Espaco_no_meio_e_recusado() =>
        Assert.Throws<FormatException>(() => Configuracao.LerPorta("80 80"));   // int.TryParse falha → FormatException
}

public static class PersistenciaComPrazo
{
    public static async Task<T> ExecutarComPrazoAsync<T>(
        Func<int, CancellationToken, Task<T>> operacao, TimeSpan prazoTotal,
        int maxTentativas, Func<int, TimeSpan> atraso, Func<Exception, bool> ehPassageira)
    {
        using var cts = new CancellationTokenSource(prazoTotal);                // o prazo vale para tudo
        return await Persistencia.ExecutarAsync(operacao, maxTentativas, atraso, ehPassageira, ct: cts.Token);
    }
}
```
Quando o prazo estoura, o `Task.Delay` (ou a própria operação) lança `OperationCanceledException`: a decisão de **parar** vem de cima, como vimos na Lei 5.
:::

::: resumo
- **Lei 8:** autocontrole = domínio sobre o que entra na mente e sobre **como reagimos aos erros**: reconhecer, admitir, corrigir e aprender.
- **Reconhecer:** validar na fronteira (*guard clauses*). **Admitir:** exceções com contexto; nunca `catch {}`; `throw;` (não `throw ex;`).
- **Corrigir:** *fallback* explícito e registrado; **retentativas** só para falhas passageiras, com limite e **recuo exponencial**.
- **Aprender:** transformar o erro em **teste de regressão** (e *post-mortem*, Lei 14).
- **Por construção:** `readonly`/`init`/`required`/`sealed`/nullable tornam estados inválidos impossíveis.
- **Analisadores** e `TreatWarningsAsErrors`: a "crítica" que você aceita e agradece.
:::

::: quiz
? O que o `catch { }` vazio representa, à luz da lei?
+ Matar o mensageiro: esconder o erro em vez de admiti-lo
- Boa prática de resiliência
- Uma otimização
- Autocontrole
> O erro continua existindo — só deixou de ser visto.

? Por que usar `throw;` em vez de `throw ex;`?
+ `throw;` preserva o stack trace original
- É mais rápido apenas
- Evita a exceção
- Não há diferença
> `throw ex;` reinicia a pilha e esconde a origem.

? Quando uma retentativa faz sentido?
+ Para falhas passageiras (timeout, indisponibilidade breve), com limite e espera crescente
- Para qualquer exceção
- Para erros de validação
- Sempre infinitamente
> Repetir erro de validação nunca resolve.

? Qual é o "quarto passo" do tratamento de erro da lição?
+ Aprender com ele — por exemplo, escrevendo um teste de regressão
- Esquecer o erro
- Culpar alguém
- Desligar o servidor
> O teste impede que o mesmo erro volte despercebido.

? Para que serve `TreatWarningsAsErrors`?
+ Fazer o build falhar com avisos, impedindo que "críticas" sejam ignoradas
- Acelerar o build
- Desativar analisadores
- Ocultar exceções
> É a crítica bem-vinda, automatizada.

? Qual dessas ferramentas torna um estado inválido impossível "por construção"?
+ required + init + construtor com validação
- catch genérico
- variáveis globais
- dynamic
> Evitar criar coisas por omissão.
:::

::: cartas
Autocontrole :: Domínio sobre o próprio pensamento e a reação aos fatos, canalizado para o propósito.
Guard clause :: Verificação no início do método que recusa entradas inválidas.
throw; :: Repassa a exceção preservando o stack trace.
Recuo exponencial :: Esperas crescentes entre tentativas (100, 200, 400 ms…).
Falha passageira :: Erro temporário, que pode desaparecer ao tentar de novo.
Teste de regressão :: Teste que impede um bug já corrigido de voltar.
Disjuntor (circuit breaker) :: Interrompe chamadas a um serviço que está falhando.
TreatWarningsAsErrors :: Opção que transforma avisos do compilador em erros.
:::
