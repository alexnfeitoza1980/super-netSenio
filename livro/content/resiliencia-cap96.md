---
id: resiliencia-cap96
order: 122
kind: capitulo
num: 96
group: C# & Mentalidade · Parte 15 · Resiliência em C#
title: Quando o serviço cai
subtitle: Falhas passageiras, tentar de novo sem se desgastar, um plano B e o descanso do disjuntor. Em C#, é o capítulo do async/await, das retentativas com espera crescente e do circuito que sabe parar de insistir.
minutes: 75
hue: 30
objectives:
- Reconhecer a **resiliência de software** como um espelho (e não uma cópia) da resiliência das pessoas
- Escrever código **assíncrono** com `async/await`, `Task<T>` e `CancellationToken`
- Implementar **retentativas com espera crescente** (*backoff*), **tempo limite** e **plano B**
- Construir um **disjuntor de circuito** com três estados e um relógio injetado
origem: livros/06-resiliencia-csharp/content/133-cap96.md
---

::: vila O serviço que ficou fora do ar
Numa sexta, a API de pagamentos que a equipe usava começou a falhar. O código do Caio tentava de novo **imediatamente**, centenas de vezes por segundo, e acabou ajudando a derrubar de vez o serviço, que já estava em apuros. Lia desligou o laço e propôs três regras: **esperar um pouco mais a cada tentativa**, **desistir depois de poucas** e, se o serviço não voltasse, **mostrar uma resposta simples** em vez de uma tela de erro. Na segunda, o serviço voltou, e o aplicativo nem tinha caído.
:::

::: ilustracao img/resiliencia/cap96.svg | Falhar é inevitável; o que importa é como se reage: esperar mais a cada tentativa, ter um plano B e saber descansar.
:::

## O que o livro diz (e o que ele não diz)

O texto sobre resiliência fala de **pessoas**. Neste capítulo, fazemos uma **ponte**, e com cuidado: a palavra *resiliência* também é usada em engenharia de software, para sistemas que **continuam funcionando apesar de falhas**. Não estamos dizendo que pessoas são máquinas, e sim que **alguns princípios se parecem**, e que olhar um lado pode iluminar o outro:

| Resiliência das pessoas | Resiliência do software |
|---|---|
| **Pausar** antes de agir | **Esperar** entre tentativas (*backoff*) |
| Não insistir sem parar em algo que está dando errado | **Limitar** as tentativas |
| Ter um **plano B** | *Fallback*: uma resposta simples quando a principal falha |
| **Descansar** quando o desgaste é grande | **Disjuntor**: parar de chamar um serviço que falhou várias vezes |
| Saber **o que merece** outra tentativa | Tentar de novo **só falhas passageiras** |

::: cuidado A ponte tem limites
Quando uma **pessoa** falha, ela merece acolhimento, não "retentativa automática". E quando um **sistema** falha, nenhum "acolhimento" substitui testar, monitorar e corrigir a causa. A analogia serve para **aprender** os padrões do software com ideias que já conhecemos; não para tratar gente como componente.
:::

::: ilustracao img/resiliencia/photos/montanhas.jpg | Algumas travessias são longas. Sistemas e pessoas chegam lá com pausas, planos B e a decisão de não insistir sem parar.
:::

## Da vida para o código

| Ideia | Em C# |
|---|---|
| Uma operação que demora ou falha | `Task<T>` e **`async/await`** |
| "Cancele, por favor" | **`CancellationToken`** |
| Esperar mais a cada falha | `PoliticaDeTentativas` com **fator** (1 s, 2 s, 4 s...) |
| Só vale repetir falhas **passageiras** | Exceção própria `FalhaPassageiraException` + `catch` específico |
| Desistir deixando a causa | `TentativasEsgotadasException(…, causa)` com **`InnerException`** |
| Plano B | `ComPlanoBAsync` |
| Limite de tempo | `CancelAfter` + `TimeoutException` |
| Descanso | **`DisjuntorDeCircuito`** com `TimeProvider` |

::: figura Os três estados do disjuntor
<svg viewBox="0 0 740 230" role="img" aria-label="Disjuntor com três estados: fechado deixa passar, aberto bloqueia e meio aberto permite uma chamada de teste">
  <g font-size="13" text-anchor="middle">
    <rect x="30" y="70" width="170" height="80" rx="14" class="f-good"/><text x="115" y="100" font-weight="800">Fechado</text><text x="115" y="124" font-size="11" class="t-dim">tudo passa; conta falhas</text>
    <rect x="285" y="70" width="170" height="80" rx="14" class="f-bad"/><text x="370" y="100" font-weight="800">Aberto</text><text x="370" y="124" font-size="11" class="t-dim">bloqueia; deixa descansar</text>
    <rect x="540" y="70" width="170" height="80" rx="14" class="f-warn"/><text x="625" y="100" font-weight="800">Meio aberto</text><text x="625" y="124" font-size="11" class="t-dim">uma chamada de teste</text>
  </g>
  <path class="arrow" d="M202 100H283"/><text x="242" y="92" text-anchor="middle" font-size="11" class="t-dim">N falhas seguidas</text>
  <path class="arrow" d="M457 100H538"/><text x="497" y="92" text-anchor="middle" font-size="11" class="t-dim">passou o descanso</text>
  <path class="arrow" d="M590 152C560 215 170 215 140 152"/><text x="365" y="205" text-anchor="middle" font-size="11" class="t-dim">o teste deu certo: fecha</text>
  <path class="arrow" d="M660 72C640 20 440 20 400 70"/><text x="530" y="26" text-anchor="middle" font-size="11" class="t-dim">o teste falhou: abre de novo</text>
</svg>
:::

## 1) `async/await` em cinco minutos

Algumas operações **demoram** (rede, disco, banco). Se o programa ficasse **parado** esperando, nada mais responderia. O **`async/await`** resolve isso: um método marcado `async` devolve uma **`Task<T>`** (uma "promessa" de valor futuro), e o **`await`** diz "espere o resultado **sem travar** o resto". Para quem lê, o código continua parecendo sequencial.

Um **`CancellationToken`** é o jeito de dizer a uma operação em andamento "pare, por favor". Quase todo método assíncrono do .NET aceita um.

## 2) Tentar de novo, com juízo

A `PoliticaDeTentativas` define o **máximo** de tentativas e a **espera** que cresce a cada falha. O `Tentador.ExecutarAsync` aplica três regras:

1. **Só repete falhas passageiras** (`FalhaPassageiraException`). Um erro de programação (um argumento inválido, por exemplo) **sobe na hora**: repetir um bug não o conserta.
2. **Espera cada vez mais** antes da próxima tentativa, para não sobrecarregar um serviço que já está mal.
3. **Desiste** depois do máximo, lançando `TentativasEsgotadasException` **com a causa** na `InnerException`, ou seja, sem esconder o que realmente aconteceu.

Note o parâmetro `esperar`: por padrão é `Task.Delay`, mas **nos testes** passamos uma espera falsa que só **anota** quanto tempo seria esperado, a mesma ideia do relógio injetado do capítulo 89.

```cs Jornada/Resiliencia/Cap96/Tentativas.cs
namespace Jornada.Resiliencia.Cap96;

/// <summary>Falha que costuma passar sozinha (rede instável, serviço ocupado). Só essas merecem nova tentativa.</summary>
public sealed class FalhaPassageiraException : Exception
{
    public FalhaPassageiraException(string mensagem) : base(mensagem) { }
}

public sealed class TentativasEsgotadasException : Exception
{
    public int Tentativas { get; }

    public TentativasEsgotadasException(int tentativas, Exception causa)
        : base($"Desisti depois de {tentativas} tentativas.", causa) => Tentativas = tentativas;
}

public sealed record PoliticaDeTentativas(int MaximoDeTentativas, TimeSpan EsperaInicial, double Fator = 2.0)
{
    /// <summary>Espera antes da próxima tentativa: cresce a cada falha (1 s, 2 s, 4 s, ...).</summary>
    public TimeSpan EsperaApos(int tentativaQueFalhou) =>
        TimeSpan.FromTicks((long)(EsperaInicial.Ticks * Math.Pow(Fator, tentativaQueFalhou - 1)));
}

public static class Tentador
{
    public static async Task<T> ExecutarAsync<T>(
        Func<CancellationToken, Task<T>> acao,
        PoliticaDeTentativas politica,
        Func<TimeSpan, CancellationToken, Task>? esperar = null,
        CancellationToken ct = default)
    {
        esperar ??= Task.Delay;                                 // nos testes, trocamos por uma espera falsa

        for (int tentativa = 1; ; tentativa++)
        {
            try
            {
                return await acao(ct);
            }
            catch (FalhaPassageiraException falha)
            {
                if (tentativa >= politica.MaximoDeTentativas)
                    throw new TentativasEsgotadasException(tentativa, falha);

                await esperar(politica.EsperaApos(tentativa), ct);
            }
            // qualquer outra exceção (um erro de programação, por exemplo) sobe na hora: repetir não adianta
        }
    }

    /// <summary>Dá um limite de tempo à ação. Passou do limite, vira TimeoutException.</summary>
    public static async Task<T> ComLimiteAsync<T>(
        Func<CancellationToken, Task<T>> acao, TimeSpan limite, CancellationToken ct = default)
    {
        using var origem = CancellationTokenSource.CreateLinkedTokenSource(ct);
        origem.CancelAfter(limite);

        try
        {
            return await acao(origem.Token);
        }
        catch (OperationCanceledException) when (!ct.IsCancellationRequested)
        {
            throw new TimeoutException($"A operação passou de {limite.TotalSeconds:0.#} s.");
        }
    }

    /// <summary>Plano B: se as tentativas esgotarem, entrega uma resposta mais simples em vez de quebrar.</summary>
    public static async Task<T> ComPlanoBAsync<T>(Func<Task<T>> principal, Func<T> planoB)
    {
        try
        {
            return await principal();
        }
        catch (TentativasEsgotadasException)
        {
            return planoB();
        }
    }
}
```

Dois outros métodos completam o kit: **`ComLimiteAsync`** dá um tempo máximo à operação (usa `CancelAfter` e converte o cancelamento por tempo em `TimeoutException`, **sem** confundir com um cancelamento pedido por quem chamou), e **`ComPlanoBAsync`** devolve uma resposta simples quando as tentativas acabam.

O serviço de mentira abaixo falha um número combinado de vezes, perfeito para testar:

```cs Jornada/Resiliencia/Cap96/ServicoInstavel.cs
namespace Jornada.Resiliencia.Cap96;

/// <summary>Um serviço de mentira que falha um número combinado de vezes e depois passa a funcionar.</summary>
public sealed class ServicoInstavel
{
    private readonly int falhasAntesDeFuncionar;

    public int Chamadas { get; private set; }

    public ServicoInstavel(int falhasAntesDeFuncionar) => this.falhasAntesDeFuncionar = falhasAntesDeFuncionar;

    public Task<string> BuscarFraseAsync(CancellationToken ct = default)
    {
        Chamadas++;

        if (Chamadas <= falhasAntesDeFuncionar)
            throw new FalhaPassageiraException($"Falha {Chamadas}: serviço ocupado.");

        return Task.FromResult("Um passo de cada vez.");
    }
}
```

```cs Uso
var politica = new PoliticaDeTentativas(MaximoDeTentativas: 4, EsperaInicial: TimeSpan.FromSeconds(1));
var servico = new ServicoInstavel(falhasAntesDeFuncionar: 2);

Task Esperar(TimeSpan tempo, CancellationToken ct)
{
    Console.WriteLine($"  esperando {tempo.TotalSeconds} s antes de tentar de novo...");
    return Task.CompletedTask;                       // aqui não esperamos de verdade
}

string frase = await Tentador.ExecutarAsync(servico.BuscarFraseAsync, politica, Esperar);
Console.WriteLine($"Funcionou na chamada {servico.Chamadas}: {frase}");

var fora = new ServicoInstavel(falhasAntesDeFuncionar: 99);
string resposta = await Tentador.ComPlanoBAsync(
    () => Tentador.ExecutarAsync(fora.BuscarFraseAsync, politica, Esperar),
    planoB: () => "Respire fundo e tente de novo mais tarde.");
Console.WriteLine(resposta);
```

```saida
  esperando 1 s antes de tentar de novo...
  esperando 2 s antes de tentar de novo...
Funcionou na chamada 3: Um passo de cada vez.
  esperando 1 s antes de tentar de novo...
  esperando 2 s antes de tentar de novo...
  esperando 4 s antes de tentar de novo...
Respire fundo e tente de novo mais tarde.
```

Leia a saída com calma: a espera **dobrou** (1 s, 2 s) e, na terceira chamada, funcionou. No segundo cenário, o serviço nunca voltou: depois de **4 tentativas** (3 esperas: 1, 2 e 4 s), o **plano B** entregou uma frase simples. O aplicativo **não caiu**.

## 3) O disjuntor de circuito

Retentativas ajudam quando a falha é **curta**. Mas se o serviço está fora do ar **há minutos**, cada tentativa só desperdiça tempo (e ainda sobrecarrega o serviço). O **disjuntor** (*circuit breaker*) resolve: depois de **N falhas seguidas**, ele **abre** e passa a recusar chamadas **sem tentar**, dando descanso ao serviço. Passado um tempo, ele fica **meio aberto** e permite **uma** chamada de teste: se der certo, **fecha**; se falhar, **abre de novo**.

Veja como o estado é **calculado** a partir do relógio: nada de `Timer` nem de `Thread`.

```cs Jornada/Resiliencia/Cap96/Disjuntor.cs
namespace Jornada.Resiliencia.Cap96;

public enum EstadoDoDisjuntor { Fechado, Aberto, MeioAberto }

public sealed class CircuitoAbertoException : Exception
{
    public CircuitoAbertoException() : base("O serviço está descansando. Tente mais tarde ou use o plano B.") { }
}

/// <summary>Depois de falhas seguidas, para de insistir por um tempo e deixa o serviço respirar.</summary>
public sealed class DisjuntorDeCircuito
{
    private readonly int limiteDeFalhas;
    private readonly TimeSpan descanso;
    private readonly TimeProvider relogio;

    private int falhasSeguidas;
    private DateTimeOffset? abertoEm;

    public DisjuntorDeCircuito(int limiteDeFalhas, TimeSpan descanso, TimeProvider relogio)
    {
        this.limiteDeFalhas = limiteDeFalhas;
        this.descanso = descanso;
        this.relogio = relogio;
    }

    public EstadoDoDisjuntor Estado
    {
        get
        {
            if (abertoEm is null) return EstadoDoDisjuntor.Fechado;
            return relogio.GetUtcNow() - abertoEm.Value >= descanso
                ? EstadoDoDisjuntor.MeioAberto          // passou o descanso: deixa UMA chamada de teste
                : EstadoDoDisjuntor.Aberto;
        }
    }

    public async Task<T> ExecutarAsync<T>(Func<CancellationToken, Task<T>> acao, CancellationToken ct = default)
    {
        var estadoAntes = Estado;
        if (estadoAntes == EstadoDoDisjuntor.Aberto)
            throw new CircuitoAbertoException();

        try
        {
            var resultado = await acao(ct);
            falhasSeguidas = 0;                         // deu certo: o circuito fecha de novo
            abertoEm = null;
            return resultado;
        }
        catch (Exception) when (!ct.IsCancellationRequested)
        {
            falhasSeguidas++;
            if (estadoAntes == EstadoDoDisjuntor.MeioAberto || falhasSeguidas >= limiteDeFalhas)
                abertoEm = relogio.GetUtcNow();         // abre (ou reabre) e começa a contar o descanso
            throw;
        }
    }
}
```

```cs Uso
var relogio = new RelogioManual();
var disjuntor = new DisjuntorDeCircuito(limiteDeFalhas: 3, descanso: TimeSpan.FromMinutes(1), relogio);
var fora = new ServicoInstavel(falhasAntesDeFuncionar: 99);

for (int i = 1; i <= 5; i++)
{
    try
    {
        await disjuntor.ExecutarAsync(fora.BuscarFraseAsync);
    }
    catch (FalhaPassageiraException)
    {
        Console.WriteLine($"Chamada {i}: falhou. Estado: {disjuntor.Estado}");
    }
    catch (CircuitoAbertoException e)
    {
        Console.WriteLine($"Chamada {i}: nem tentei. {e.Message}");
    }
}

relogio.Avancar(TimeSpan.FromMinutes(1));
Console.WriteLine($"Depois de 1 minuto: {disjuntor.Estado}");
```

```saida
Chamada 1: falhou. Estado: Fechado
Chamada 2: falhou. Estado: Fechado
Chamada 3: falhou. Estado: Aberto
Chamada 4: nem tentei. O serviço está descansando. Tente mais tarde ou use o plano B.
Chamada 5: nem tentei. O serviço está descansando. Tente mais tarde ou use o plano B.
Depois de 1 minuto: MeioAberto
```

As chamadas 1 e 2 **falharam** (o circuito ainda estava fechado). A 3ª falha **abriu** o circuito. As chamadas 4 e 5 **nem tentaram**: foram recusadas na hora. E, depois de um minuto (com o relógio avançado à mão), ele ficou **meio aberto**, pronto para o teste. Esse é o "descansar" do software: **parar de insistir** por um tempo.

## 4) Testes

Os testes de assíncrono usam `async Task` e `await`; o `Assert.ThrowsAsync` espera uma exceção de uma chamada assíncrona. Veja os principais:

```cs Resiliencia.Testes/Cap96Tests.cs
using Jornada.Resiliencia.Cap96;

namespace Resiliencia.Testes;

public class Cap96Tests
{
    private static readonly PoliticaDeTentativas Politica = new(MaximoDeTentativas: 4, EsperaInicial: TimeSpan.FromSeconds(1));

    /// <summary>Espera falsa: registra quanto seria esperado, sem esperar de verdade.</summary>
    private sealed class EsperaFalsa
    {
        public List<TimeSpan> Esperas { get; } = new();
        public Task Esperar(TimeSpan tempo, CancellationToken ct) { Esperas.Add(tempo); return Task.CompletedTask; }
    }

    [Fact]
    public void Espera_cresce_a_cada_falha()
    {
        Assert.Equal(TimeSpan.FromSeconds(1), Politica.EsperaApos(1));
        Assert.Equal(TimeSpan.FromSeconds(2), Politica.EsperaApos(2));
        Assert.Equal(TimeSpan.FromSeconds(4), Politica.EsperaApos(3));
    }

    [Fact]
    public async Task Tenta_de_novo_ate_funcionar()
    {
        var servico = new ServicoInstavel(falhasAntesDeFuncionar: 2);
        var espera = new EsperaFalsa();

        var frase = await Tentador.ExecutarAsync(servico.BuscarFraseAsync, Politica, espera.Esperar);

        Assert.Equal("Um passo de cada vez.", frase);
        Assert.Equal(3, servico.Chamadas);
        Assert.Equal(new[] { TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(2) }, espera.Esperas);
    }

    [Fact]
    public async Task Desiste_quando_as_tentativas_acabam_e_guarda_a_causa()
    {
        var servico = new ServicoInstavel(falhasAntesDeFuncionar: 99);

        var e = await Assert.ThrowsAsync<TentativasEsgotadasException>(() =>
            Tentador.ExecutarAsync(servico.BuscarFraseAsync, Politica, new EsperaFalsa().Esperar));

        Assert.Equal(4, e.Tentativas);
        Assert.IsType<FalhaPassageiraException>(e.InnerException);
    }

    [Fact]
    public async Task Erro_de_programacao_nao_e_repetido()
    {
        var espera = new EsperaFalsa();
        int chamadas = 0;

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            Tentador.ExecutarAsync<string>(_ => { chamadas++; throw new InvalidOperationException("bug"); },
                                           Politica, espera.Esperar));

        Assert.Equal(1, chamadas);
        Assert.Empty(espera.Esperas);
    }

    [Fact]
    public async Task Plano_B_entrega_algo_simples_quando_tudo_falha()
    {
        var servico = new ServicoInstavel(99);

        var frase = await Tentador.ComPlanoBAsync(
            () => Tentador.ExecutarAsync(servico.BuscarFraseAsync, Politica, new EsperaFalsa().Esperar),
            planoB: () => "Respire fundo e tente de novo mais tarde.");

        Assert.Equal("Respire fundo e tente de novo mais tarde.", frase);
    }
```

E os do disjuntor, usando o `RelogioDeTeste` do capítulo 89:

```cs Resiliencia.Testes/Cap96Tests.cs
    // ---------- disjuntor ----------

    private static Task<string> Falha(CancellationToken _) => throw new FalhaPassageiraException("fora do ar");
    private static Task<string> Funciona(CancellationToken _) => Task.FromResult("ok");

    [Fact]
    public async Task Disjuntor_abre_depois_de_falhas_seguidas_e_nao_chama_mais_o_servico()
    {
        var relogio = new RelogioDeTeste();
        var d = new DisjuntorDeCircuito(limiteDeFalhas: 3, descanso: TimeSpan.FromMinutes(1), relogio);

        for (int i = 0; i < 3; i++)
            await Assert.ThrowsAsync<FalhaPassageiraException>(() => d.ExecutarAsync(Falha));

        Assert.Equal(EstadoDoDisjuntor.Aberto, d.Estado);

        int chamadas = 0;
        await Assert.ThrowsAsync<CircuitoAbertoException>(() =>
            d.ExecutarAsync(_ => { chamadas++; return Funciona(_); }));
        Assert.Equal(0, chamadas);
    }

    [Fact]
    public async Task Depois_do_descanso_uma_chamada_de_teste_fecha_o_circuito()
    {
        var relogio = new RelogioDeTeste();
        var d = new DisjuntorDeCircuito(2, TimeSpan.FromMinutes(1), relogio);
        for (int i = 0; i < 2; i++)
            await Assert.ThrowsAsync<FalhaPassageiraException>(() => d.ExecutarAsync(Falha));

        relogio.Avancar(TimeSpan.FromMinutes(1));
        Assert.Equal(EstadoDoDisjuntor.MeioAberto, d.Estado);

        Assert.Equal("ok", await d.ExecutarAsync(Funciona));
        Assert.Equal(EstadoDoDisjuntor.Fechado, d.Estado);
    }

    [Fact]
    public async Task Se_o_teste_falha_o_circuito_reabre_e_o_descanso_recomeca()
    {
        var relogio = new RelogioDeTeste();
        var d = new DisjuntorDeCircuito(2, TimeSpan.FromMinutes(1), relogio);
        for (int i = 0; i < 2; i++)
            await Assert.ThrowsAsync<FalhaPassageiraException>(() => d.ExecutarAsync(Falha));

        relogio.Avancar(TimeSpan.FromMinutes(1));
        await Assert.ThrowsAsync<FalhaPassageiraException>(() => d.ExecutarAsync(Falha));

        Assert.Equal(EstadoDoDisjuntor.Aberto, d.Estado);
        relogio.Avancar(TimeSpan.FromSeconds(30));
        Assert.Equal(EstadoDoDisjuntor.Aberto, d.Estado);
    }

    [Fact]
    public async Task Um_sucesso_no_meio_zera_a_contagem_de_falhas()
    {
        var d = new DisjuntorDeCircuito(3, TimeSpan.FromMinutes(1), new RelogioDeTeste());

        await Assert.ThrowsAsync<FalhaPassageiraException>(() => d.ExecutarAsync(Falha));
        await Assert.ThrowsAsync<FalhaPassageiraException>(() => d.ExecutarAsync(Falha));
        await d.ExecutarAsync(Funciona);
        await Assert.ThrowsAsync<FalhaPassageiraException>(() => d.ExecutarAsync(Falha));

        Assert.Equal(EstadoDoDisjuntor.Fechado, d.Estado);
    }
}
```

::: dica Por que injetar a espera e o relógio?
Se os testes **esperassem de verdade**, uma suíte com retentativas levaria minutos e ficaria instável. Ao injetar a **espera** (`Func<TimeSpan, CancellationToken, Task>`) e o **relógio** (`TimeProvider`), os testes rodam em **milissegundos** e são **determinísticos**: não dependem da velocidade do computador.
:::

::: exercicio Um teto para a espera
Dobrar a espera para sempre não é razoável: 1, 2, 4, 8... 1024 segundos. Escreva o método de extensão `EsperaComTeto(this PoliticaDeTentativas politica, int tentativaQueFalhou, TimeSpan teto)`, que devolve a espera calculada **ou o teto**, o que for **menor**. Teste com um teto de 10 s: a 3ª tentativa espera 4 s; a 6ª (32 s) é limitada a 10 s.

--- resposta
```cs Jornada/Resiliencia/Exercicios/Cap96Teto.cs
namespace Jornada.Resiliencia.Cap96;

public static class PoliticaExtensoes
{
    /// <summary>Espera crescente, mas com teto: insistir cada vez mais devagar também cansa.</summary>
    public static TimeSpan EsperaComTeto(this PoliticaDeTentativas politica, int tentativaQueFalhou, TimeSpan teto) =>
        TimeSpan.FromTicks(Math.Min(politica.EsperaApos(tentativaQueFalhou).Ticks, teto.Ticks));
}
```

`Math.Min` compara os **ticks** (a menor unidade de tempo do `TimeSpan`) e devolvemos um `TimeSpan` a partir do menor valor. Em sistemas reais, costuma-se também somar um pequeno **sorteio** (*jitter*) à espera, para que muitos clientes não voltem **todos no mesmo instante**.
:::

::: resumo
- **Resiliência de software** é espelho, e não cópia: sistemas esperam, limitam tentativas, têm plano B e descansam. **Pessoas merecem acolhimento**, não retentativa automática.
- `async/await` e `Task<T>` não travam o programa enquanto esperam; `CancellationToken` pede para parar.
- **Retentativas com espera crescente (*backoff*)** valem só para **falhas passageiras**; erro de programação sobe na hora; ao desistir, guarde a **causa**.
- **Tempo limite** (`CancelAfter`) e **plano B** evitam esperas infinitas e telas de erro.
- O **disjuntor** tem três estados (**fechado, aberto, meio aberto**) e é testável porque o **relógio** é **injetado**.
:::

::: quiz
? Por que não repetir imediatamente, centenas de vezes, uma chamada que falhou?
+ Porque isso pode sobrecarregar o serviço e piorar a falha
- Porque o C# não permite laços
- Porque a rede é infinita
- Porque o `await` bloqueia
> Esperar mais a cada tentativa dá folga ao serviço.

? Quais exceções a `Tentador` repete?
+ Apenas as passageiras (`FalhaPassageiraException`)
- Todas, sempre
- Nenhuma
- Só `ArgumentException`
> Repetir um bug não o conserta.

? O que a `InnerException` de `TentativasEsgotadasException` guarda?
+ A causa original da última falha
- O número de tentativas
- O tempo de espera
- O nome do serviço
> Desistir não deve esconder o que aconteceu.

? O que acontece com chamadas feitas quando o disjuntor está aberto?
+ São recusadas na hora, sem tentar o serviço
- São repetidas até funcionar
- São colocadas em fila para sempre
- São executadas em paralelo
> O objetivo é dar descanso ao serviço.

? Por que o `DisjuntorDeCircuito` recebe um `TimeProvider`?
+ Para que os testes avancem o relógio sem esperar de verdade
- Porque o C# exige
- Para economizar memória
- Para medir a nota do usuário
> É a mesma injeção de dependência do capítulo 89.

? A analogia entre resiliência de pessoas e de software significa que pessoas são máquinas?
- Sim, só que mais complexas
+ Não: serve para aprender padrões; pessoas merecem acolhimento, não retentativa automática
- Sim, para fins técnicos
- Só em ambientes de trabalho
> A ponte ajuda a pensar, mas tem limites.
:::

::: cartas
Resiliência de software :: Capacidade de um sistema continuar funcionando apesar de falhas.
async / await :: Palavras-chave para esperar operações demoradas sem travar o programa.
Task<T> :: Promessa de um valor `T` que ainda será produzido.
CancellationToken :: Sinal para pedir a uma operação em andamento que pare.
Backoff :: Esperar mais a cada nova tentativa (1 s, 2 s, 4 s...).
Fallback (plano B) :: Resposta simples usada quando a principal falha.
Disjuntor de circuito :: Para de chamar um serviço após falhas seguidas, deixando-o descansar.
Falha passageira :: Falha que costuma passar sozinha e, por isso, merece nova tentativa.
:::
