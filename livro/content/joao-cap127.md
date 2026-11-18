---
id: joao-cap127
order: 165
kind: capitulo
num: 127
group: C# & Mentalidade · Parte 18 · O Evangelho segundo João em C#
title: O vento sopra onde quer
subtitle: A conversa de Jesus com Nicodemos, de noite, e a imagem do vento que se ouve mas não se sabe de onde vem. Em C#, quem emite e quem ouve se desconhecem, e é por isso que funciona.
minutes: 70
hue: 214
objectives:
- Ler a conversa de **Nicodemos** (Jo 3,1-8) e entender o lugar da imagem do vento
- Transformar o vento em C#: quem **emite** (`Soprar`) e quem **ouve** (`Ouvir`) ligados por um **evento**
- Entender por que quem emite **não precisa saber** quem está ouvindo, e o que o código **não** diz sobre o Espírito
origem: livros/10-evangelho-joao-csharp/content/104-cap127.md
---

::: vila O painel que não conhecia ninguém
Bia ligou três telas ao mesmo sensor de temperatura. Quando alguém propôs que o sensor "avisasse cada tela", ela discordou: "se o sensor conhecer as telas, **cada tela nova mexe no sensor**". O sensor só devia **soprar valores**; quem quisesse ouvir **se inscrevia**, e quem enjoasse **cancelava**, sem que o sensor soubesse.
:::

::: ilustracao img/joao/cap127.svg | O vento sopra onde quer: quem emite não conhece quem ouve. Cada ouvinte se inscreve e cancela por conta própria.
:::

## O texto

> **1** E havia um homem dos fariseus, cujo nome era Nicodemos, chefe dos judeus. **2** Este veio a Jesus de noite, e disse-lhe: Rabi, bem sabemos que és Mestre vindo de Deus; porque ninguém pode fazer estes sinais que tu fazes, se Deus não for com ele. **3** Respondeu Jesus e disse-lhe: Em verdade, em verdade te digo, que aquele que não voltar a nascer, não pode ver o Reino de Deus. **4** Nicodemos lhe disse: Como pode o homem nascer, sendo já velho? Pode ele voltar a entrar no ventre de sua mãe, e nascer? **5** Respondeu Jesus: Em verdade, em verdade te digo, que aquele que não nascer de água e do Espírito, não pode entrar no Reino de Deus. **6** O que é nascido de carne, carne é; e o que é nascido do Espírito, espírito é. **7** Não te maravilhes de que te disse: necessário vos é voltar a nascer. **8** O vento sopra onde quer, e ouves o seu som; porém não sabes de onde vem, nem para onde vai; assim é todo aquele que é nascido do Espírito.
>
> *João 3,1-8 (Bíblia Livre)*

## Lendo devagar

- **Nicodemos**, um homem dos fariseus e chefe dos judeus, procura Jesus **de noite** (3,1-2) e reconhece que ele vem de Deus.
- Jesus fala em **voltar a nascer** (3,3.7) e, depois, em nascer "de água e do Espírito" (3,5). O grego tem aí a palavra *anōthen*, que pode ser "de novo" e também "do alto" (Strong G509: "from above... anew"); as traduções precisam escolher.
- **Nicodemos não entende**: "Como pode o homem nascer, sendo já velho?" (3,4).
- Para explicar, Jesus usa uma imagem, o **vento** (3,8): ele sopra **onde quer**, você **ouve o seu som**, mas **não sabe de onde vem, nem para onde vai**, e "assim é todo aquele que é nascido do Espírito".
- Em grego, a palavra *pneuma* é **vento** e **espírito** ao mesmo tempo (Strong G4151: "breath... breeze; ... spirit"); o verbo "soprar" (*pneō*, G4154) é da mesma família. A imagem de 3,8 usa esse duplo sentido.

A imagem do vento tem três partes: ele **sopra**; alguém **ouve**; e quem ouve **não sabe a origem** nem o destino.

## Do versículo ao código

### Passo 1: "O vento sopra onde quer"

O vento **faz uma coisa**: sopra. Isso é um **método**. Mas há um detalhe: ele sopra **sem escolher quem ouve**. Em C#, existe um recurso para isso, o **evento**. Um evento é um "canal" aberto: quem quiser, se **inscreve** nele; quem emite apenas dispara.

```cs
public class VentoSimples
{
    public event Action<string>? Soprou;                          // o canal aberto

    public void Soprar(string som) => Soprou?.Invoke(som);        // "O vento sopra"
}
```

`Soprou?.Invoke(som)` significa: **se houver alguém inscrito**, avise todos com este som; se não houver, não faz nada (e não dá erro).

### Passo 2: "e ouves o seu som"

Quem ouve é outro objeto, com um método que **recebe o som**:

```cs
public class OuvinteDoVento
{
    public List<string> Ouvido { get; } = new();

    public void Ouvir(string som) => Ouvido.Add(som);             // "e ouves o seu som"
}
```

Para ouvir, é preciso **se inscrever** no evento, com `+=`:

```cs
var vento = new VentoSimples();
var ouvinte = new OuvinteDoVento();

vento.Soprou += ouvinte.Ouvir;       // passa a ouvir
vento.Soprar("shhh");                // o ouvinte recebe "shhh"
```

### Passo 3: "não sabes de onde vem, nem para onde vai"

Olhe o que **não existe** no código: o `VentoSimples` **não sabe quem** está ouvindo (só guarda uma lista de inscritos), e o `OuvinteDoVento` **não sabe** quem sopra (só recebe o som). Cada um ignora o outro, e o programa funciona. É a **estrutura** da imagem: ouve-se o som, sem conhecer origem e destino.

Quem quiser **deixar de ouvir**, usa `-=`:

```cs
vento.Soprou -= ouvinte.Ouvir;       // deixou de ouvir
```

### O código completo, rodando

```cs Jornada/Joao/Texto/Cap127Vento.cs
public class VentoSimples
{
    // quem quiser ouvir, se inscreve neste evento
    public event Action<string>? Soprou;

    public int Ouvintes => Soprou?.GetInvocationList().Length ?? 0;

    // "O vento sopra onde quer": ele só sopra; não escolhe quem ouve
    public void Soprar(string som) => Soprou?.Invoke(som);
}

public class OuvinteDoVento
{
    public List<string> Ouvido { get; } = new();

    // "e ouves o seu som": só recebe o som, sem saber quem soprou
    public void Ouvir(string som) => Ouvido.Add(som);
}
```

```bash
dotnet run --project codigo/joao/Joao.Console -- exemplo t127
```

```cs Uso
var vento = new VentoSimples();
var primeiro = new OuvinteDoVento();
var segundo = new OuvinteDoVento();

vento.Soprou += primeiro.Ouvir;
vento.Soprou += segundo.Ouvir;
vento.Soprar("shhh");

vento.Soprou -= segundo.Ouvir;                    // o segundo deixa de ouvir
vento.Soprar("fuuu");

Console.WriteLine("Primeiro ouviu: " + string.Join(", ", primeiro.Ouvido));
Console.WriteLine("Segundo ouviu:  " + string.Join(", ", segundo.Ouvido));
Console.WriteLine("Ouvintes agora: " + vento.Ouvintes);
```

```saida
Primeiro ouviu: shhh, fuuu
Segundo ouviu:  shhh
Ouvintes agora: 1
```

O primeiro ouvinte recebeu os dois sopros; o segundo **saiu depois do primeiro** e só ouviu o "shhh". O vento nem precisou ser avisado: continuou a soprar.

::: cuidado Limite da analogia
- **O código só dá forma à imagem**: emitir e ouvir sem se conhecerem. Ele **não** trata de nascer de novo, do Espírito, nem do diálogo com Nicodemos. O texto usa o vento para falar do Espírito; o código usa o vento só como **estrutura**.
- O duplo sentido de *pneuma* (vento/espírito), que dá força à frase, **não** passa para o código: em C#, `Soprou` é só um evento.
- Esta versão simples **não é segura para várias threads**: os inscritos ficam em uma lista comum, e inscrever ou desinscrever enquanto outra thread sopra pode dar erro. Para um programa de verdade com várias threads, é preciso proteger a lista.
:::

::: dica Por que isso é bom
Se o vento precisasse conhecer cada ouvinte, **toda vez que alguém novo chegasse**, você teria que mexer no vento. Assim, quem emite e quem ouve podem **mudar à vontade**. É o jeito de funcionar de botões, notificações, painéis que se atualizam: um dispara, vários reagem.
:::

::: curiosidade Lupa no grego
- **ἄνωθεν** (*anōthen*, G509: "do alto; ... de novo") aparece **5 vezes** (3,3; 3,7; 3,31; 19,11; 19,23) (`onde anōthen`). Em 3,3.7, as traduções escolhem entre "de novo" (como a Bíblia Livre, "voltar a nascer") e "do alto".
- **πνεῦμα** (*pneuma*, G4151: "sopro, brisa; ... espírito") aparece **24 vezes** (`onde pneuma`); **πνέω** (*pneō*, "soprar", G4154) aparece **só 2 vezes**: 3,8 e 6,18 (`onde pneō`). Em 3,8, a frase usa a palavra *pneuma* e o verbo *pneō* juntos (*to pneuma hopou thelei pnei*).
- **φωνή** (*phōnē*, "voz, som", G5456) aparece **15 vezes**, e em 3,8 é o "som" do vento (`onde phōnē`).
- **Νικόδημος** (*Nikodēmos*) aparece **5 vezes**: 3,1.4.9; 7,50; 19,39 (`onde Nikodēmos`). **νύξ** ("noite") aparece **6 vezes**: 3,2; 9,4; 11,10; 13,30; 19,39; 21,3 (`onde nyx`).

*Fontes: texto grego SBLGNT (CC BY 4.0); contagens por lema do MorphGNT (CC BY-SA 3.0), reproduzíveis com `dotnet run --project codigo/joao/Joao.Console -- onde <palavra>`; sentidos do dicionário de Strong (OpenScriptures), com a entrada indicada.*
:::


## Indo além: de evento a fluxo (`IObservable`)

O evento do C# resolve o básico. Para casos mais ricos (filtrar o que se ouve, transformar os valores, saber quando o vento **parou** ou **falhou**), a plataforma .NET tem duas interfaces: `IObservable<T>` (quem emite) e `IObserver<T>` (quem ouve). As seções abaixo constroem uma versão completa, com operadores como `Onde` e `Transformar`.

::: figura Um vento, muitos ouvintes
<svg viewBox="0 0 740 230" role="img" aria-label="Um emissor envia valores a três ouvintes; um operador filtra os pares; um ouvinte cancela e para de receber">
  <rect x="20" y="86" width="120" height="58" rx="14" class="f-info"/><text x="80" y="112" text-anchor="middle" font-weight="800">Vento&lt;int&gt;</text><text x="80" y="130" text-anchor="middle" font-size="11" class="t-dim">Soprar(1..4)</text>
  <path class="arrow" d="M142 105L260 54"/><path class="arrow" d="M142 115L260 115"/><path class="arrow" d="M142 125L260 176"/>
  <rect x="262" y="30" width="170" height="48" rx="12" class="f-good"/><text x="347" y="52" text-anchor="middle" font-weight="800">Ouvinte A</text><text x="347" y="68" text-anchor="middle" font-size="11" class="t-dim">Onde(par).Transformar</text>
  <rect x="262" y="92" width="170" height="48" rx="12" class="f-good"/><text x="347" y="114" text-anchor="middle" font-weight="800">Ouvinte B</text><text x="347" y="130" text-anchor="middle" font-size="11" class="t-dim">vê tudo</text>
  <rect x="262" y="154" width="170" height="48" rx="12" class="f-bad" opacity=".5"/><text x="347" y="176" text-anchor="middle" font-weight="800">Ouvinte C</text><text x="347" y="192" text-anchor="middle" font-size="11" class="t-dim">Dispose(): saiu</text>
  <rect x="490" y="62" width="230" height="110" rx="14" class="f-warn"/>
  <text x="605" y="92" text-anchor="middle" font-weight="800">Por que isso ajuda</text>
  <text x="605" y="116" text-anchor="middle" font-size="12">O emissor não muda</text><text x="605" y="134" text-anchor="middle" font-size="12">quando alguém chega ou sai.</text>
  <text x="605" y="156" text-anchor="middle" font-size="12" class="t-dim">Baixo acoplamento.</text>
</svg>
:::

### 1) O vento e o ouvinte

O .NET já traz as duas interfaces: `IObservable<T>` (quem emite, com um único método `Subscribe`) e `IObserver<T>` (quem ouve, com `OnNext`, `OnError` e `OnCompleted`). Três regras, quase leis, regem o contrato:

1. Pode haver **muitos** `OnNext`, mas **no máximo um** final: `OnCompleted` **ou** `OnError`.
2. **Depois do final, não há mais nada.**
3. `Subscribe` devolve um **`IDisposable`**: chamar `Dispose()` cancela a inscrição.

Repare em três decisões do `Vento<T>`: ele itera sobre **uma cópia** (`ToArray()`) dos ouvintes, porque um ouvinte pode se desinscrever **durante** o sopro; quem chega **depois do fim** recebe `OnCompleted` na hora; e `Inscricao.Dispose` é **idempotente** (descartar duas vezes é inofensivo).

```cs Jornada/Joao/Cap127/Vento.cs
namespace Jornada.Joao.Cap127;

/// <summary>
/// Um "vento": publica valores para quem estiver ouvindo, sem saber quem são.
/// Implementa IObservable&lt;T&gt;, a interface padrão do .NET para fluxos de eventos.
/// </summary>
public sealed class Vento<T> : IObservable<T>
{
    private readonly List<IObserver<T>> ouvintes = new();
    private bool terminou;

    public int Ouvintes => ouvintes.Count;

    public IDisposable Subscribe(IObserver<T> ouvinte)
    {
        if (terminou)
        {
            ouvinte.OnCompleted();                    // quem chega depois do fim só ouve o silêncio
            return Inscricao.Vazia;
        }

        ouvintes.Add(ouvinte);
        return new Inscricao(() => ouvintes.Remove(ouvinte));
    }

    public void Soprar(T valor)
    {
        if (terminou) return;

        foreach (var ouvinte in ouvintes.ToArray())   // cópia: um ouvinte pode sair durante o sopro
            ouvinte.OnNext(valor);
    }

    public void Completar()
    {
        if (terminou) return;
        terminou = true;
        foreach (var ouvinte in ouvintes.ToArray()) ouvinte.OnCompleted();
        ouvintes.Clear();
    }

    public void Falhar(Exception erro)
    {
        if (terminou) return;
        terminou = true;
        foreach (var ouvinte in ouvintes.ToArray()) ouvinte.OnError(erro);
        ouvintes.Clear();
    }
}

/// <summary>Um vento que lembra os últimos valores: quem "nasce de novo" mais tarde recebe o que já aconteceu.</summary>
public sealed class VentoComMemoria<T> : IObservable<T>
{
    private readonly Vento<T> vento = new();
    private readonly Queue<T> memoria = new();
    private readonly int capacidade;

    public VentoComMemoria(int capacidade)
    {
        if (capacidade < 1) throw new ArgumentOutOfRangeException(nameof(capacidade));
        this.capacidade = capacidade;
    }

    public IDisposable Subscribe(IObserver<T> ouvinte)
    {
        foreach (var antigo in memoria.ToArray())
            ouvinte.OnNext(antigo);                   // primeiro o que já passou...

        return vento.Subscribe(ouvinte);              // ...depois o que vier
    }

    public void Soprar(T valor)
    {
        memoria.Enqueue(valor);
        if (memoria.Count > capacidade) memoria.Dequeue();
        vento.Soprar(valor);
    }

    public void Completar() => vento.Completar();
}

/// <summary>Desinscrever é descartar: Subscribe devolve um IDisposable que desliga a escuta.</summary>
internal sealed class Inscricao : IDisposable
{
    public static readonly Inscricao Vazia = new(() => { });

    private Action? aoDescartar;

    public Inscricao(Action aoDescartar) => this.aoDescartar = aoDescartar;

    public void Dispose()
    {
        aoDescartar?.Invoke();
        aoDescartar = null;                           // descartar duas vezes não faz mal
    }
}

internal sealed class Ouvinte<T> : IObserver<T>
{
    private readonly Action<T> aoReceber;
    private readonly Action? aoCompletar;
    private readonly Action<Exception>? aoFalhar;

    public Ouvinte(Action<T> aoReceber, Action? aoCompletar, Action<Exception>? aoFalhar)
    {
        this.aoReceber = aoReceber;
        this.aoCompletar = aoCompletar;
        this.aoFalhar = aoFalhar;
    }

    public void OnNext(T valor) => aoReceber(valor);
    public void OnCompleted() => aoCompletar?.Invoke();
    public void OnError(Exception erro) => aoFalhar?.Invoke(erro);
}

/// <summary>Operadores: pequenos "filtros de vento" que se encadeiam, como o LINQ, mas para fluxos que ainda vão acontecer.</summary>
public static class Observaveis
{
    public static IDisposable Ouvir<T>(this IObservable<T> fonte, Action<T> aoReceber, Action? aoCompletar = null, Action<Exception>? aoFalhar = null) =>
        fonte.Subscribe(new Ouvinte<T>(aoReceber, aoCompletar, aoFalhar));

    public static IObservable<T> Onde<T>(this IObservable<T> fonte, Func<T, bool> condicao) =>
        new Operador<T, T>(fonte, (valor, adiante) => { if (condicao(valor)) adiante(valor); });

    public static IObservable<TSaida> Transformar<T, TSaida>(this IObservable<T> fonte, Func<T, TSaida> funcao) =>
        new Operador<T, TSaida>(fonte, (valor, adiante) => adiante(funcao(valor)));

    private sealed class Operador<TEntrada, TSaida> : IObservable<TSaida>
    {
        private readonly IObservable<TEntrada> fonte;
        private readonly Action<TEntrada, Action<TSaida>> passo;

        public Operador(IObservable<TEntrada> fonte, Action<TEntrada, Action<TSaida>> passo)
        {
            this.fonte = fonte;
            this.passo = passo;
        }

        public IDisposable Subscribe(IObserver<TSaida> ouvinte) =>
            fonte.Subscribe(new Ouvinte<TEntrada>(
                valor => passo(valor, ouvinte.OnNext),
                ouvinte.OnCompleted,
                ouvinte.OnError));
    }
}
```

O `VentoComMemoria<T>` resolve "o que quem chega tarde perde": guarda os últimos `capacidade` valores numa `Queue<T>` e **reenvia** a cada novo ouvinte antes de passar a ouvir o vivo. O nome é só nosso: não tem relação com o "voltar a nascer" do texto.

Os operadores `Onde` e `Transformar` são pequenos **adaptadores**: cada um recebe uma fonte e devolve **outra** `IObservable`, que filtra ou converte valores antes de passá-los adiante. Como no LINQ, você encadeia: `vento.Onde(par).Transformar(texto).Ouvir(...)`.

```cs Uso
var vento = new Vento<int>();

vento.Onde(n => n % 2 == 0)
     .Transformar(n => $"par {n}")
     .Ouvir(texto => Console.WriteLine($"  ouvinte A ouviu: {texto}"));

var inscricaoB = vento.Ouvir(n => Console.WriteLine($"  ouvinte B viu {n}"));

for (int n = 1; n <= 4; n++)
{
    Console.WriteLine($"o vento sopra {n}");
    vento.Soprar(n);
    if (n == 2) inscricaoB.Dispose();                // B vai embora depois do 2
}

var memoria = new VentoComMemoria<string>(capacidade: 2);
memoria.Soprar("a"); memoria.Soprar("b"); memoria.Soprar("c");
memoria.Ouvir(letra => Console.WriteLine($"quem nasceu de novo recebeu: {letra}"));
```

```saida
o vento sopra 1
  ouvinte B viu 1
o vento sopra 2
  ouvinte A ouviu: par 2
  ouvinte B viu 2
o vento sopra 3
o vento sopra 4
  ouvinte A ouviu: par 4
quem nasceu de novo recebeu: b
quem nasceu de novo recebeu: c
```

Acompanhe a saída. O ouvinte B ouve **tudo**; o A só os pares, já transformados em `par 2` e `par 4`. O sopro 3 não chega ao A (filtrado) e o B já cancelou antes do 3. No fim, quem se inscreveu no `VentoComMemoria` **recebeu `b` e `c`** (os dois últimos guardados), e não o `a`, que já foi esquecido.

::: dica Observável não é evento do C#
Os `event` do C# fazem algo parecido, mas **não compõem** (você não consegue filtrar um `event` com `Onde`), **não sinalizam fim ou erro** e vivem na instância que os declara. `IObservable<T>` é um **valor**: pode ser passado, guardado, filtrado e combinado. A biblioteca *Reactive Extensions* (Rx.NET) traz dezenas de operadores sobre essa mesma interface.
:::

::: cuidado Limites desta implementação
- `Vento<T>` e `VentoComMemoria<T>` **não são seguros para várias threads**: os ouvintes e a memória ficam em coleções comuns. Em um programa com várias threads, seria preciso proteger essas coleções (por exemplo, com `lock`).
- Um ouvinte que lança exceção em `OnNext` interrompe o sopro para os seguintes; as bibliotecas reativas completas tratam disso.
:::

### 2) Testes

```cs Joao.Testes/Cap127Tests.cs
using Jornada.Joao.Cap127;

namespace Joao.Testes;

public class Cap127Tests
{
    [Fact]
    public void Quem_se_inscreve_ouve_o_que_o_vento_trouxer_sem_saber_quem_sopra()
    {
        var vento = new Vento<string>();
        var ouvido = new List<string>();

        vento.Ouvir(ouvido.Add);
        vento.Soprar("um");
        vento.Soprar("dois");

        Assert.Equal(new[] { "um", "dois" }, ouvido);
    }

    [Fact]
    public void Varios_ouvintes_recebem_o_mesmo_sopro()
    {
        var vento = new Vento<int>();
        int a = 0, b = 0;

        vento.Ouvir(x => a += x);
        vento.Ouvir(x => b += x * 10);
        vento.Soprar(3);

        Assert.Equal((3, 30), (a, b));
    }

    [Fact]
    public void Descartar_a_inscricao_para_de_ouvir()
    {
        var vento = new Vento<int>();
        var ouvido = new List<int>();

        var inscricao = vento.Ouvir(ouvido.Add);
        vento.Soprar(1);
        inscricao.Dispose();
        vento.Soprar(2);

        Assert.Equal(new[] { 1 }, ouvido);
        Assert.Equal(0, vento.Ouvintes);
    }

    [Fact]
    public void Operadores_se_encadeiam_como_no_LINQ()
    {
        var vento = new Vento<int>();
        var ouvido = new List<string>();

        vento.Onde(n => n % 2 == 0)
             .Transformar(n => $"par {n}")
             .Ouvir(ouvido.Add);

        foreach (var n in Enumerable.Range(1, 6))
            vento.Soprar(n);

        Assert.Equal(new[] { "par 2", "par 4", "par 6" }, ouvido);
    }

    [Fact]
    public void Depois_de_completar_nada_mais_chega_e_quem_chega_tarde_so_ouve_o_fim()
    {
        var vento = new Vento<int>();
        var ouvido = new List<int>();
        int completou = 0;

        vento.Ouvir(ouvido.Add, () => completou++);
        vento.Soprar(1);
        vento.Completar();
        vento.Soprar(2);                                    // ignorado

        vento.Ouvir(_ => Assert.Fail("não deveria ouvir nada"), () => completou++);

        Assert.Equal(new[] { 1 }, ouvido);
        Assert.Equal(2, completou);
    }

    [Fact]
    public void Um_erro_avisa_todo_mundo_e_encerra()
    {
        var vento = new Vento<int>();
        Exception? recebido = null;

        vento.Ouvir(_ => { }, aoFalhar: e => recebido = e);
        vento.Falhar(new InvalidOperationException("tempestade"));

        Assert.Equal("tempestade", recebido!.Message);
    }

    [Fact]
    public void Com_memoria_quem_nasce_de_novo_recebe_o_que_ja_passou()
    {
        var vento = new VentoComMemoria<string>(capacidade: 2);
        vento.Soprar("a");
        vento.Soprar("b");
        vento.Soprar("c");                                  // "a" foi esquecido

        var recebido = new List<string>();
        vento.Ouvir(recebido.Add);
        vento.Soprar("d");

        Assert.Equal(new[] { "b", "c", "d" }, recebido);
    }

    [Fact]
    public void Ouvinte_que_sai_durante_o_sopro_nao_quebra_os_outros()
    {
        var vento = new Vento<int>();
        var ouvido = new List<int>();
        IDisposable? primeira = null;

        primeira = vento.Ouvir(_ => primeira!.Dispose());   // sai logo no primeiro sopro
        vento.Ouvir(ouvido.Add);

        vento.Soprar(1);
        vento.Soprar(2);

        Assert.Equal(new[] { 1, 2 }, ouvido);
    }
}
```

Dois testes merecem atenção. **`Ouvinte_que_sai_durante_o_sopro_nao_quebra_os_outros`** protege a decisão do `ToArray()`: sem a cópia, remover da lista durante o `foreach` lançaria `InvalidOperationException`. E **`Depois_de_completar_nada_mais_chega_e_quem_chega_tarde_so_ouve_o_fim`** testa as regras 1 e 2 do contrato.

::: exercicio Contar os sopros
Escreva `Contagem<T>(this IObservable<T> fonte)`, que devolve um `IObservable<int>`: **a cada valor emitido pela fonte, emite quantos valores já passaram** (1, 2, 3...). Atenção: **cada ouvinte** deve ter a **sua própria** contagem.

--- resposta
```cs Jornada/Joao/Exercicios/Cap127Contagem.cs
using Jornada.Joao.Cap127;

namespace Jornada.Joao.Exercicios;

public static class ContagemExtensoes
{
    /// <summary>Cada vez que a fonte emite, emite quantas vezes ela já emitiu. Cada ouvinte tem a sua própria conta.</summary>
    public static IObservable<int> Contagem<T>(this IObservable<T> fonte) => new Contador<T>(fonte);

    private sealed class Contador<T> : IObservable<int>
    {
        private readonly IObservable<T> fonte;

        public Contador(IObservable<T> fonte) => this.fonte = fonte;

        public IDisposable Subscribe(IObserver<int> ouvinte)
        {
            int n = 0;                                    // uma variável por inscrição: nada de conta compartilhada
            return fonte.Ouvir(_ => ouvinte.OnNext(++n), ouvinte.OnCompleted, ouvinte.OnError);
        }
    }
}
```

A variável `n` fica **dentro de `Subscribe`**, e por isso é criada **uma vez por inscrição**. Se ela estivesse como campo da classe, dois ouvintes compartilhariam a mesma conta, um erro clássico de operadores reativos. Repare também que `OnCompleted` e `OnError` são repassados: o operador não pode "engolir" o final.
:::

::: resumo
- O vento **sopra** (`Soprar`) e alguém **ouve** (`Ouvir`), ligados por um **evento** (`+=`). Nenhum dos dois conhece o outro.
- `Soprou?.Invoke(...)` avisa **quem estiver inscrito**; sem ninguém, não faz nada e não dá erro. O código não trata do Espírito nem de nascer de novo.
- Em João, **anōthen** ("do alto" ou "de novo") aparece 5 vezes; **pneuma** quer dizer "vento" e "espírito", e o verbo "soprar" (*pneō*) só aparece 2 vezes.
- **Observador**: o emissor não conhece os ouvintes; só o contrato `IObservable<T>`/`IObserver<T>`.
- `Subscribe` devolve um `IDisposable`: **descartar cancela a escuta**.
- O contrato: muitos `OnNext`, no máximo **um** final (`OnCompleted` ou `OnError`), e nada depois.
- **Operadores** devolvem novos observáveis e se encadeiam; `VentoComMemoria` entrega o passado a quem chega depois.
:::

::: quiz
? Qual recurso do C# representa o vento (emite sem saber quem ouve)?
+ Um `event`
- Um `enum`
- Uma herança
- Um `switch`
> Quem quiser, se inscreve com `+=`.

? Para que serve `-=` em `vento.Soprou -= ouvinte.Ouvir`?
+ Para deixar de ouvir
- Para apagar o vento
- Para soprar mais forte
- Para criar um ouvinte
> É o contrário do `+=`.

? O que torna *anōthen* (Jo 3,3) uma palavra especial?
+ Pode ser "do alto" ou "de novo", e as traduções precisam escolher
- Só aparece uma vez
- É uma palavra inventada por João
- Quer dizer "noite"
> Aparece 5 vezes em João; Nicodemos entende "de novo".

? O que a palavra grega *pneuma* significa em Jo 3,8?
+ Vento e espírito: a mesma palavra, o que permite o jogo de sentidos
- Somente "água"
- Somente "voz"
- Somente "luz"
> O verbo "soprar" (*pneō*) aparece só 2 vezes em João.

? No padrão observador, o emissor conhece cada ouvinte?
+ Não: ele só conhece o contrato `IObservable<T>`
- Sim, guarda o nome de cada um
- Só quando há erro
- Só em C#
> Esse desconhecimento mútuo é o baixo acoplamento.

? O que `Subscribe` devolve, e para quê?
+ Um `IDisposable`; chamar `Dispose()` cancela a inscrição
- Um `int` com o número de ouvintes
- O último valor
- Nada
> Descartar a inscrição é a forma de parar de ouvir.

? Por que o `Vento<T>` percorre uma cópia da lista de ouvintes?
+ Porque um ouvinte pode se desinscrever durante o sopro
- Para ser mais rápido
- Para ordenar os ouvintes
- Por exigência de `IObserver`
> Remover de uma lista durante o `foreach` lança exceção.

? Para que serve o `VentoComMemoria<T>`?
+ Para quem se inscreve depois receber os valores recentes que já passaram
- Para guardar o vento em disco
- Para ouvir duas vezes
- Para acelerar o sopro
> Guarda os últimos valores numa fila.
:::

::: cartas
event :: Canal em que quem quiser se inscreve (`+=`) para ser avisado quando algo acontece.
Nicodemos (Jo 3) :: Líder que visita Jesus à noite; reaparece em 7,50 e 19,39.
Anōthen :: "Do alto" ou "de novo": 5 vezes em João; palavra de dois sentidos.
Pneuma :: "Vento" e "espírito": a mesma palavra no grego.
Padrão observador :: Emissor e ouvintes se ignoram; falam só pelo contrato.
IObservable<T> :: Quem emite: tem `Subscribe`.
IObserver<T> :: Quem ouve: `OnNext`, `OnError`, `OnCompleted`.
Inscrição descartável :: `Subscribe` devolve `IDisposable`; descartar = cancelar.
Operador :: Adaptador que recebe um observável e devolve outro: `Onde`, `Transformar`.
:::
