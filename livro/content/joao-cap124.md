---
id: joao-cap124
order: 162
kind: capitulo
num: 124
group: C# & Mentalidade · Parte 18 · O Evangelho segundo João em C#
title: No princípio era a Palavra
subtitle: João 1,1-5 lido devagar e transformado em C#. "Estava junto de" vira referência, "era" vira herança, e a luz que brilha nas trevas vira uma lanterna.
minutes: 70
hue: 220
objectives:
- Ler **Jo 1,1-5** frase por frase e separar o que o **texto** diz do que é **interpretação**
- Transformar "estava junto de" em uma **referência** e "era" em uma **herança** (`:`), e saber o que `is` e `==` realmente perguntam
- Entender por que `new` (criar) **não** representa o "era" do texto, e quais são os **limites** dessa analogia
- Construir uma **lanterna** (registro de eventos) e ver por que ela precisa ser segura para tarefas simultâneas
origem: livros/10-evangelho-joao-csharp/content/101-cap124.md
---

::: vila A madrugada em que ninguém sabia nada
Às três da manhã, o sistema de pagamentos parou. Pela manhã, a equipe tinha 40 mensagens de chat e nenhuma resposta: **o que** falhou? **Quando**? **Depois de qual ação?** Lia disse: "o código fez tudo certo, ou tudo errado, e **não deixou rastro**. Estamos no escuro". Caio perguntou: "e se cada passo importante acendesse uma luz pequena?".
:::

::: ilustracao img/joao/cap124.svg | Três frases de Jo 1,1-5 em C#: "estava junto de" é uma referência, "era" é herança, e a luz brilha nas trevas. É uma analogia: o código não define a natureza de Deus.
:::

## O texto

> **1** No princípio era a Palavra, e a Palavra estava junto de Deus, e a Palavra era Deus. **2** Esta estava no princípio junto de Deus. **3** Por esta foram feitas todas as coisas, e sem ela não se fez coisa nenhuma do que foi feito. **4** Nela estava a vida, e a vida era a luz dos seres humanos. **5** E a luz brilha nas trevas; e as trevas não a compreenderam.
>
> *João 1,1-5 (Bíblia Livre)*

Esta edição traduz *Palavra* e anota, em 1,1, "equiv. Verbo". Nos tipos de C# deste capítulo usamos **`Verbo`**, o nome tradicional em português para a mesma palavra grega (*logos*).

## Lendo devagar

João começa de um jeito diferente dos outros evangelhos: não conta uma cena, volta ao **princípio**. Frase por frase:

- **"No princípio era a Palavra"**: já **existia** no princípio. O grego usa o verbo *ēn* ("era, existia"), no passado contínuo.
- **"e a Palavra estava junto de Deus"**: a Palavra está **em companhia de** Deus. São duas realidades que o texto põe lado a lado.
- **"e a Palavra era Deus"**: e, ao mesmo tempo, a Palavra **é** Deus. A mesma frase afirma duas coisas diferentes, **"estar junto de"** e **"ser"**.
- **"Nela estava a vida, e a vida era a luz dos seres humanos"**: a vida está na Palavra, e essa vida é a **luz** das pessoas.
- **"E a luz brilha nas trevas; e as trevas não a compreenderam"**: a luz brilha onde está escuro, e a escuridão **não a alcançou**. O verbo grego (*katelaben*) também pode ser lido como "apanhar, alcançar" ou "dominar"; por isso há traduções em português que dizem "compreenderam" e outras que dizem "não conseguiram dominá-la".

::: dica Texto, interpretação, analogia e técnica
Este livro mantém quatro coisas separadas. Neste capítulo:

| O que é | Neste capítulo |
|---|---|
| **Texto** | "a Palavra estava junto de Deus" e "a Palavra era Deus" (Jo 1,1) |
| **Interpretação** | A tradição cristã identifica a Palavra com Jesus, a partir de Jo 1,14 ("a Palavra se fez carne"). **Não é modelada aqui.** |
| **Analogia** | "estava junto de" = **referência**; "era" = **herança** |
| **Realidade técnica** | `:` declara herança; `is` testa o tipo de um objeto; `ReferenceEquals` compara referências |
:::

## Do versículo ao código

Vamos construir, passo a passo, **uma analogia**: cada frase do texto escolhe um recurso de C#. O objetivo é aprender os recursos; o código **não** prova nem explica nada sobre o que o texto afirma. Na seção "Limite da analogia", mais adiante, dizemos exatamente onde ela deixa de valer.

### Passo 1: "No princípio era a Palavra"

Em C#, `class Verbo { }` **declara um tipo**: diz que existe algo chamado Verbo. Essa declaração vale **antes** de existir qualquer objeto do programa.

```cs
public class Verbo
{
}
```

Repare no que **não** fizemos: nenhum `new`. Isso é de propósito.

::: cuidado Por que não usamos `new` aqui
Em C#, `new Verbo()` **cria** um objeto: ele passa a existir num instante, durante a execução. O texto diz o contrário do Verbo: ele **"era"** (*ēn*, "já existia") no princípio. João reserva outro verbo, *egeneto* ("veio a ser", "foi feito"), para o que foi criado (1,3: "todas as coisas foram feitas por ela") e para quando "a Palavra se fez carne" (1,14). Por isso `new` **não** representa o "era". Nos exemplos que rodam, o programa cria objetos (com `new`) **só para ter algo sobre o que falar**: isso é do programa, e não do texto.
:::

### Passo 2: "e a Palavra estava junto de Deus"

"Estar junto de" liga **dois**: o Verbo de um lado, Deus do outro. Em C#, ligar um objeto a outro é uma **propriedade que guarda uma referência**:

```cs
public class Deus { }

public class Verbo
{
    public Deus? EstavaCom { get; set; }     // "estava junto de Deus"
}

var deus = new Deus();                       // o programa precisa de um objeto para falar dele
var verbo = new Verbo { EstavaCom = deus };  // o Verbo aponta para Deus
```

`deus` e `verbo` continuam sendo **dois objetos**; um está **ligado** ao outro.

### Passo 3: "e a Palavra era Deus"

"Era" não é estar **junto**: é **ser**. Em C#, dizer que um tipo **é** um tipo de outro é **herança**, escrita com dois-pontos:

```cs
public class Verbo : Deus      // "a Palavra era Deus": Verbo é do tipo Deus
{
    public Deus? EstavaCom { get; set; }
}
```

O `:` significa "**é do tipo**": tudo o que vale para `Deus` passa a valer para `Verbo` (inclusive as propriedades que `Deus` tiver). Isso é uma relação **entre tipos**, e vale pela declaração. Por isso dá para perguntar sem criar objeto nenhum: `typeof(Deus).IsAssignableFrom(typeof(Verbo))` é `true`.

Quando já existe um **objeto**, o operador `is` pergunta se o tipo dele é compatível com outro: `verbo is Deus` é `true`. Atenção ao que **não** perguntam: nenhum deles compara se dois objetos são o mesmo. Essa é outra pergunta, `ReferenceEquals(verbo, deus)`, que olha a **memória** do programa. Resumindo, C# tem **três perguntas diferentes**:

| Pergunta | Em C# | O que responde |
|---|---|---|
| "Um tipo é do tipo do outro?" | `typeof(Deus).IsAssignableFrom(typeof(Verbo))` | relação entre **tipos** |
| "Este objeto é de tal tipo?" | `verbo is Deus` | tipo do **objeto** |
| "São o mesmo objeto na memória?" | `ReferenceEquals(verbo, deus)` | **identidade** na memória |

### Passo 4: "Nela estava a vida, e a vida era a luz"

"Nela estava a vida" é a Vida **dentro** do Verbo: uma propriedade. "A vida era a luz" é de novo **ser**: a Vida é do tipo Luz.

```cs
public class Luz { }
public class Vida : Luz { }            // "a vida era a luz"

public class Verbo : Deus
{
    public Deus? EstavaCom { get; set; }
    public Vida? Vida { get; set; }    // "Nela estava a vida"
}
```

### Passo 5: "a luz brilha nas trevas; e as trevas não a compreenderam"

Duas ações: a luz **brilha** e as trevas **não compreendem**. Cada uma vira um método:

```cs
public class Luz
{
    public void Resplandecer(Trevas trevas) => trevas.Receber(this);   // "a luz brilha nas trevas"
}

public class Trevas
{
    public bool Iluminadas { get; private set; }
    public void Receber(Luz luz) => Iluminadas = true;
    public bool Compreendeu(Luz luz) => false;                         // "as trevas não a compreenderam"
}
```

Uma observação honesta: `Compreendeu` devolve `false` **por decreto**, porque o texto assim afirma. Ele não calcula nada: o resultado foi **definido** por nós a partir do versículo. O teste confirma que o código **registra** a afirmação do texto, e não explica o motivo.

### O código completo, rodando

Este é o arquivo inteiro (compilado e testado):

```cs Jornada/Joao/Texto/Cap124Verbo.cs
namespace Jornada.Joao.Texto;

// Jo 1,1-5 (Bíblia Livre): "No princípio era a Palavra, e a Palavra estava junto de Deus, e a Palavra era Deus. [...]
// Nela estava a vida, e a vida era a luz dos seres humanos. E a luz brilha nas trevas; e as trevas não a compreenderam."
// A própria edição anota, em 1,1: "Palavra, equiv. Verbo". Os tipos abaixo se chamam Verbo, o nome tradicional.
//
// TEXTO     : duas expressões diferentes na mesma frase: "estava junto de" e "era".
// ANALOGIA  : em C#, "estar junto de" é uma REFERÊNCIA (um objeto ligado a outro) e "ser" é HERANÇA (um tipo é do tipo de outro).
// LIMITE    : o sistema de tipos de C# não define o que o texto afirma; o código só dá forma às duas relações.

/// <summary>
/// O tipo que representa "Deus" nesta analogia. Ser uma classe que se pode instanciar é um detalhe de C#:
/// o código NÃO afirma que se possa criar Deus, nem que existam vários.
/// </summary>
public class Deus
{
    public string Nome => "Deus";
}

public class Luz
{
    // "E a luz brilha nas trevas"
    public void Resplandecer(Trevas trevas) => trevas.Receber(this);
}

public class Vida : Luz
{
}

// <verbo>
// "e a Palavra era Deus": o ":" diz que Verbo é do tipo Deus. É uma relação entre TIPOS,
// que vale pela declaração, antes de existir qualquer objeto (nenhum "new" é necessário).
public class Verbo : Deus
{
    public Deus? EstavaCom { get; set; }       // "e a Palavra estava junto de Deus": referência a outro objeto
    public Vida? Vida { get; set; }            // "Nela estava a vida"
}
// </verbo>

public class Trevas
{
    public bool Iluminadas { get; private set; }

    public void Receber(Luz luz) => Iluminadas = true;

    // "e as trevas não a compreenderam": o resultado é DECLARADO pelo texto, não calculado por um mecanismo.
    // O teste confirma que o código registra a declaração; não explica o motivo.
    public bool Compreendeu(Luz luz) => false;
}
```

Para rodar na sua máquina (a saída abaixo é a deste comando):

```bash
dotnet run --project codigo/joao/Joao.Console -- exemplo t124
```

```cs Uso
// "e a Palavra era Deus": uma relação entre TIPOS, verdadeira sem criar nenhum objeto
Console.WriteLine($"Pelos tipos, o Verbo era Deus? {typeof(Deus).IsAssignableFrom(typeof(Verbo))}");

// Para rodar o exemplo, o programa precisa de objetos para falar deles. O "new" é do programa, e não do texto.
var deus = new Deus();
var verbo = new Verbo { EstavaCom = deus };

Console.WriteLine($"O Verbo estava junto de Deus (referência)? {ReferenceEquals(verbo.EstavaCom, deus)}");
Console.WriteLine($"O objeto verbo é do tipo Deus (teste `is`)? {verbo is Deus}");
Console.WriteLine($"São a mesma referência na memória (outra pergunta)? {ReferenceEquals(verbo, deus)}");

var luz = new Luz();
var trevas = new Trevas();
luz.Resplandecer(trevas);
Console.WriteLine($"A luz brilha nas trevas: {trevas.Iluminadas}");
Console.WriteLine($"As trevas a compreenderam? {trevas.Compreendeu(luz)} (declarado pelo texto, não calculado)");
```

```saida
Pelos tipos, o Verbo era Deus? True
O Verbo estava junto de Deus (referência)? True
O objeto verbo é do tipo Deus (teste `is`)? True
São a mesma referência na memória (outra pergunta)? False
A luz brilha nas trevas: True
As trevas a compreenderam? False (declarado pelo texto, não calculado)
```

Linha por linha:
1. **Pelos tipos**, o Verbo era Deus (`True`): vale sem criar objeto algum.
2. O Verbo estava junto de Deus (`True`): é uma **referência**.
3. O objeto `verbo` é do tipo Deus (`True`): é o teste `is`.
4. Mas **não são a mesma referência** na memória (`False`). Esta é a pergunta de C# sobre **objetos** na memória do programa, e não sobre o que o texto afirma de Deus e da Palavra.
5. A luz brilha nas trevas, e 6. as trevas não a compreenderam: este resultado é **declarado**, como vimos no passo 5.

::: cuidado Limite da analogia
- **Herança diz "é do tipo", e só isso.** Em C#, a classe derivada costuma *especializar* a base (ser uma versão mais específica dela). O texto **não** diz que a Palavra seja uma "versão" menor ou derivada de Deus. O que o código aproveita é só a ideia de "ser do tipo"; C# não tem como expressar "mesma natureza, sem ser derivado".
- **`Deus` é uma classe que se pode instanciar.** O `new Deus()` existe no exemplo **só por conveniência**. O código não afirma que se possa criar Deus, nem que existam vários.
- **A linha 4 é um detalhe de C#**, não uma afirmação sobre a natureza da Palavra e de Deus.
- **O artigo grego.** Em 1,1, "estava junto de Deus" tem o artigo (*ton theon*) e "era Deus" não (*theos ēn ho logos*), e essa diferença existe no grego. O código usa o **mesmo tipo** `Deus` nas duas frases e **não representa** essa diferença.
- **A Palavra e Jesus.** A tradição cristã lê a Palavra como Jesus (Jo 1,14). Isso **não** é modelado: no capítulo 132, Jesus aparece apenas como uma pessoa à mesa (`IPessoa`), e o Pai como o papel de quem cuida da videira (cap. 133). O livro **não** tenta unir os modelos em herança.
- **Em resumo:** o sistema de tipos de C# não define nem representa a natureza divina. Ele só dá **forma** às duas relações que a frase distingue.
:::

::: curiosidade Lupa no grego
- **φῶς** (*phōs*, "luz") aparece **23 vezes** em João e **σκοτία** (*skotia*, "trevas"), **8** (`onde phōs`, `onde skotia`).
- **λόγος** (*logos*, "palavra") aparece **40 vezes**; **4** delas estão no capítulo 1 (1,1 três vezes e 1,14).
- Em 1,1, o verbo é **ἦν** (*ēn*, "era, existia") três vezes. Para o que foi feito, João usa **ἐγένετο** (*egeneto*, "veio a ser"): em 1,3 ("todas as coisas") e em 1,14 ("a Palavra se fez carne").
- **κατέλαβεν** (*katelaben*, 1,5), de **καταλαμβάνω** (G2638), abrange "apanhar, alcançar, compreender". É por isso que há traduções em português que dizem "compreenderam" (como a Bíblia Livre) e outras que dizem "não conseguiram dominá-la".
- **φαίνει** ("brilha", 1,5) está no **presente**: a luz brilha agora, e não apenas "brilhou" uma vez (SBLGNT, 3ª pessoa do presente).

*Fontes: texto grego SBLGNT (CC BY 4.0); contagens por lema do MorphGNT (CC BY-SA 3.0), reproduzíveis com `dotnet run --project codigo/joao/Joao.Console -- onde <palavra>`; sentidos do dicionário de Strong (OpenScriptures), com a entrada indicada.*
:::


## Indo além: a luz de um programa

O versículo 5 fala de **luz que brilha onde está escuro**. Um programa tem um "escuro" parecido: o que acontece **por dentro** dele (que passos deu? o que falhou? em quê?) fica invisível se ninguém acender uma luz. Essa luz, em programação, chama-se **registro** (ou *log*), e a capacidade de ver o que se passa por dentro, **observabilidade**.

::: cuidado Limite da analogia: luz e registro
No Evangelho, a **luz** é a vida que vem da Palavra (1,4), e as trevas são o que **não a acolhe**. Um *log* não é isso: é uma ferramenta de engenharia. A única ponte é a **estrutura**: onde não há luz, não se vê; onde há, vê-se. Por isso a lanterna abaixo **não** tem um objeto para as "trevas": uma lanterna apagada (o objeto nulo) só obedece e se cala, e as trevas do texto são o contrário de obedientes.
:::

As próximas seções constroem uma "lanterna" de verdade, com **níveis** de luz, dados organizados e **escopos** que abrem e fecham sozinhos.

::: ilustracao img/joao/cap124-log.svg | Uma lanterna que acende em camadas: cada nível de registro deixa mais luz no caminho. O nível "Detalhe" fica abaixo do mínimo e não acende.
:::

::: figura Camadas de luz: quanto mais grave, mais certo de aparecer
<svg viewBox="0 0 740 220" role="img" aria-label="Quatro faixas de luz crescente: detalhe, informação, aviso e erro; um nível mínimo corta as faixas abaixo dele">
  <defs><linearGradient id="g124" x1="0" x2="1"><stop offset="0" stop-color="var(--ink-3)" stop-opacity=".25"/><stop offset="1" stop-color="#f2c14e"/></linearGradient></defs>
  <rect x="30" y="40" width="680" height="34" rx="8" fill="url(#g124)"/>
  <g font-size="14" font-weight="800" text-anchor="middle">
    <text x="115" y="63">Detalhe</text><text x="285" y="63">Informação</text><text x="455" y="63">Aviso</text><text x="625" y="63">Erro</text>
  </g>
  <line x1="370" y1="28" x2="370" y2="170" stroke="var(--primary)" stroke-width="3" stroke-dasharray="8 6"/>
  <text x="370" y="22" text-anchor="middle" font-size="12" class="t-dim">nível mínimo = Aviso</text>
  <rect x="30" y="96" width="336" height="60" rx="10" class="f-bad" opacity=".35"/><text x="198" y="132" text-anchor="middle" font-size="13">fica no escuro: nem é escrito</text>
  <rect x="374" y="96" width="336" height="60" rx="10" class="f-good"/><text x="542" y="122" text-anchor="middle" font-size="13">chega ao destino</text><text x="542" y="142" text-anchor="middle" font-size="11" class="t-dim">com hora, dados e escopos</text>
  <text x="370" y="196" text-anchor="middle" font-size="13" class="t-dim">Luz demais cega; luz de menos deixa no escuro. O nível mínimo é o botão de ajuste.</text>
</svg>
:::

### 1) A lanterna: níveis, dados e escopos

O primeiro recurso é o **registro estruturado**. Em vez de escrever só texto (`"pedido 42 pago"`), guardamos **a mensagem e os dados separados** (`pedido=42`, `valor=19,90`). Texto livre é bom para humanos; dados em campos são bons para **buscas, filtros e gráficos**.

O segundo é o **escopo**. Uma operação tem partes ("entrega", dentro dela "revisão", dentro dela "testes"), e queremos que **todo registro escrito ali dentro carregue esse caminho**. Para isso, `Escopo("nome")` devolve um `IDisposable`: quando o bloco `using` termina, o `Dispose()` roda **sozinho** e fecha o escopo, mesmo que haja uma exceção no meio. Esse é o jeito do C# de dizer "o que abri, eu fecho".

Três ideias de projeto aparecem no código:

- **Uma lista imutável por fluxo de execução (`AsyncLocal`)** guarda os escopos abertos: abrir um escopo cria uma lista nova, e fechar volta à anterior. Cada tarefa enxerga **a sua própria** lista, e por isso duas tarefas ao mesmo tempo **não misturam** os escopos (uma `Stack` comum e compartilhada misturaria). Fechar fora de ordem é **erro de programação**, e o código o recusa.
- **`params (string Nome, object? Valor)[] campos`** aceita qualquer número de pares nome/valor, na chamada: `Escrever(nivel, "texto", ("pedido", 42), ("valor", 19.9m))`.
- **Relógio injetado** (`TimeProvider`): a hora vem de fora, e os testes controlam o tempo (você viu isso no Livro 6).

```cs Jornada/Joao/Cap124/Lanterna.cs
using System.Collections.Immutable;

namespace Jornada.Joao.Cap124;

/// <summary>Quanto de luz queremos: do detalhe miúdo ao erro que não pode passar despercebido.</summary>
public enum Nivel { Detalhe, Informacao, Aviso, Erro }

/// <summary>Uma linha do registro: quando, quão grave, o quê, com quais dados e dentro de quais "cenas" (escopos).</summary>
public sealed record Registro(
    DateTimeOffset Quando,
    Nivel Nivel,
    string Mensagem,
    IReadOnlyDictionary<string, object?> Campos,
    IReadOnlyList<string> Escopos);

public interface ILanterna
{
    void Escrever(Nivel nivel, string mensagem, params (string Nome, object? Valor)[] campos);

    /// <summary>Abre uma "cena": tudo escrito até o Dispose carrega o nome dela. Use com <c>using</c>.</summary>
    IDisposable Escopo(string nome);
}

/// <summary>
/// A lanterna apagada aceita tudo e não mostra nada. É o padrão "objeto nulo": quem usa não precisa testar se há lanterna antes de escrever.
/// Atenção: isto NÃO representa as "trevas" de Jo 1,5. As trevas do texto não acolhem a luz; o objeto nulo só obedece e se cala.
/// </summary>
public sealed class LanternaApagada : ILanterna
{
    public static readonly LanternaApagada Instancia = new();

    private LanternaApagada() { }

    public void Escrever(Nivel nivel, string mensagem, params (string Nome, object? Valor)[] campos) { }

    public IDisposable Escopo(string nome) => Vazio.Instancia;

    private sealed class Vazio : IDisposable
    {
        public static readonly Vazio Instancia = new();
        public void Dispose() { }
    }
}

public sealed class Lanterna : ILanterna
{
    private readonly Nivel minimo;
    private readonly Action<Registro> destino;
    private readonly TimeProvider relogio;

    // Cada fluxo de execução (cada tarefa) enxerga a sua própria lista de escopos abertos. A lista é imutável: abrir um escopo cria
    // uma lista nova, e fechar volta à anterior. Com uma pilha comum e compartilhada, duas tarefas misturariam os escopos.
    private readonly AsyncLocal<ImmutableList<string>?> escopos = new();

    private ImmutableList<string> Abertos => escopos.Value ?? ImmutableList<string>.Empty;

    public Lanterna(Nivel minimo, Action<Registro> destino, TimeProvider? relogio = null)
    {
        this.minimo = minimo;
        this.destino = destino;
        this.relogio = relogio ?? TimeProvider.System;
    }

    public void Escrever(Nivel nivel, string mensagem, params (string Nome, object? Valor)[] campos)
    {
        if (nivel < minimo)
            return;                                   // abaixo do nível mínimo, a luz nem acende

        var dados = new Dictionary<string, object?>();
        foreach (var (nome, valor) in campos)
            dados[nome] = valor;

        destino(new Registro(relogio.GetUtcNow(), nivel, mensagem, dados, Abertos.ToList()));
    }

    public IDisposable Escopo(string nome)
    {
        var anteriores = Abertos;
        escopos.Value = anteriores.Add(nome);
        return new SaidaDoEscopo(this, anteriores);
    }

    private sealed class SaidaDoEscopo : IDisposable
    {
        private readonly Lanterna lanterna;
        private readonly ImmutableList<string> anteriores;
        private bool fechado;

        public SaidaDoEscopo(Lanterna lanterna, ImmutableList<string> anteriores)
        {
            this.lanterna = lanterna;
            this.anteriores = anteriores;
        }

        public void Dispose()
        {
            if (fechado)
                return;                               // Dispose pode ser chamado duas vezes sem estrago

            if (lanterna.Abertos.Count != anteriores.Count + 1)
                throw new InvalidOperationException("Os escopos precisam ser fechados na ordem inversa da abertura.");

            lanterna.escopos.Value = anteriores;
            fechado = true;
        }
    }

    /// <summary>Transforma um registro em uma linha legível: [Nivel] hora > cena > cena mensagem {campo=valor}.</summary>
    public static string Formatar(Registro r)
    {
        var caminho = r.Escopos.Count > 0 ? " > " + string.Join(" > ", r.Escopos) : "";
        var campos = r.Campos.Count > 0
            ? " {" + string.Join(", ", r.Campos.Select(c => $"{c.Key}={c.Value}")) + "}"
            : "";

        return $"[{r.Nivel}] {r.Quando:HH:mm:ss}{caminho}: {r.Mensagem}{campos}";
    }
}
```

Repare no **objeto nulo**, `LanternaApagada`: ela cumpre o mesmo contrato (`ILanterna`) e **não faz nada**. Quem recebe uma lanterna **nunca precisa perguntar** "será que há uma?": se não quiser luz, entrega a apagada. É um jeito elegante de evitar `if (lanterna != null)` por todo o código, e uma boa lição sobre **fazer o "nada" ser um valor**.

```cs Uso
var linhas = new List<Registro>();
var lanterna = new Lanterna(Nivel.Informacao, linhas.Add, relogio);

using (lanterna.Escopo("entrega do relatório"))
{
    lanterna.Escrever(Nivel.Detalhe, "abrindo o arquivo");              // abaixo do mínimo: fica no escuro
    lanterna.Escrever(Nivel.Informacao, "revisão iniciada", ("revisor", "Lia"));

    using (lanterna.Escopo("testes"))
        lanterna.Escrever(Nivel.Aviso, "2 testes lentos", ("segundos", 14));

    lanterna.Escrever(Nivel.Erro, "publicação recusada", ("motivo", "falta aprovação"));
}

foreach (var registro in linhas)
    Console.WriteLine(Lanterna.Formatar(registro));

Console.WriteLine($"{linhas.Count} linhas escritas; a de nível Detalhe não acendeu");
```

```saida
[Informacao] 09:00:00 > entrega do relatório: revisão iniciada {revisor=Lia}
[Aviso] 09:00:00 > entrega do relatório > testes: 2 testes lentos {segundos=14}
[Erro] 09:00:00 > entrega do relatório: publicação recusada {motivo=falta aprovação}
3 linhas escritas; a de nível Detalhe não acendeu
```

Leia a saída com atenção: a linha de nível `Detalhe` **não acendeu** (abaixo do mínimo `Informacao`); a de `Aviso` aparece **dentro de dois escopos** (`entrega do relatório > testes`); a de `Erro`, dentro de um só, porque o escopo `testes` já havia fechado. É o `using` trabalhando.

::: dica Nível mínimo é o botão de ajuste
Em produção, costuma-se deixar `Informacao` ou `Aviso`; para **investigar um problema**, desce-se para `Detalhe` por um tempo. Se tudo for `Erro`, ninguém olha mais. Se nada for, ninguém vê o problema. A arte é **escolher o que merece luz**.
:::

::: cuidado Limites desta implementação
- O **destino** dos registros (`Action<Registro>`) é chamado na thread de quem escreve. Se ele guarda em uma lista comum (como nos exemplos de uma tarefa só), **só é seguro com uma tarefa por vez**; para várias tarefas, use um destino seguro (por exemplo, `ConcurrentBag`, como no teste).
- Os escopos acompanham o fluxo assíncrono (`AsyncLocal`): uma tarefa disparada sem `await` **herda** o escopo do momento em que nasceu.
- É uma lanterna **didática**: não há arquivo, formato JSON, envio para fora, nem filtro por origem.
:::

### 2) Testes

```cs Joao.Testes/Cap124Tests.cs
using Jornada.Joao.Cap124;

namespace Joao.Testes;

public class Cap124Tests
{
    private static (Lanterna Lanterna, List<Registro> Linhas) Nova(Nivel minimo = Nivel.Detalhe)
    {
        var linhas = new List<Registro>();
        return (new Lanterna(minimo, linhas.Add, new RelogioDeTeste()), linhas);
    }

    [Fact]
    public void Abaixo_do_nivel_minimo_a_luz_nem_acende()
    {
        var (lanterna, linhas) = Nova(Nivel.Aviso);

        lanterna.Escrever(Nivel.Detalhe, "miudeza");
        lanterna.Escrever(Nivel.Informacao, "rotina");
        lanterna.Escrever(Nivel.Aviso, "atenção");
        lanterna.Escrever(Nivel.Erro, "problema");

        Assert.Equal(new[] { "atenção", "problema" }, linhas.Select(l => l.Mensagem));
    }

    [Fact]
    public void Os_campos_chegam_como_dados_e_nao_so_como_texto()
    {
        var (lanterna, linhas) = Nova();

        lanterna.Escrever(Nivel.Informacao, "pedido pago", ("pedido", 42), ("valor", 19.9m));

        Assert.Equal(42, linhas[0].Campos["pedido"]);
        Assert.Equal(19.9m, linhas[0].Campos["valor"]);
    }

    [Fact]
    public void Escopos_se_acumulam_e_desaparecem_ao_fechar()
    {
        var (lanterna, linhas) = Nova();

        using (lanterna.Escopo("entrega"))
        {
            using (lanterna.Escopo("revisão"))
                lanterna.Escrever(Nivel.Informacao, "dentro");

            lanterna.Escrever(Nivel.Informacao, "no meio");
        }

        lanterna.Escrever(Nivel.Informacao, "fora");

        Assert.Equal(new[] { "entrega", "revisão" }, linhas[0].Escopos);
        Assert.Equal(new[] { "entrega" }, linhas[1].Escopos);
        Assert.Empty(linhas[2].Escopos);
    }

    [Fact]
    public void Fechar_escopos_fora_de_ordem_e_um_erro_de_programacao()
    {
        var (lanterna, _) = Nova();

        var externo = lanterna.Escopo("externo");
        var interno = lanterna.Escopo("interno");

        Assert.Throws<InvalidOperationException>(() => externo.Dispose());
        interno.Dispose();
        externo.Dispose();                                  // agora, na ordem certa, funciona
    }

    [Fact]
    public void Descartar_duas_vezes_nao_faz_mal()
    {
        var (lanterna, linhas) = Nova();

        var escopo = lanterna.Escopo("a");
        escopo.Dispose();
        escopo.Dispose();

        lanterna.Escrever(Nivel.Informacao, "livre");
        Assert.Empty(linhas[0].Escopos);
    }

    [Fact]
    public void A_hora_vem_do_relogio_injetado_e_o_formato_e_legivel()
    {
        var (lanterna, linhas) = Nova();

        using (lanterna.Escopo("entrega"))
            lanterna.Escrever(Nivel.Aviso, "revisão pendente", ("dias", 2));

        Assert.Equal("[Aviso] 09:00:00 > entrega: revisão pendente {dias=2}", Lanterna.Formatar(linhas[0]));
    }

    [Fact]
    public void A_lanterna_apagada_aceita_tudo_e_nao_mostra_nada()
    {
        ILanterna semRegistro = LanternaApagada.Instancia;

        using (semRegistro.Escopo("qualquer"))
            semRegistro.Escrever(Nivel.Erro, "ninguém vê");        // não lança, não registra, não exige teste de nulo
    }

    [Fact]
    public async Task Cada_fluxo_de_execucao_tem_os_seus_proprios_escopos()
    {
        var registros = new System.Collections.Concurrent.ConcurrentBag<Registro>();
        var lanterna = new Lanterna(Nivel.Detalhe, registros.Add, new RelogioDeTeste());

        async Task Trabalhar(string nome)
        {
            using (lanterna.Escopo(nome))
            {
                await Task.Delay(5);
                lanterna.Escrever(Nivel.Informacao, "dentro");
                await Task.Delay(5);
            }
        }

        await Task.WhenAll(Enumerable.Range(1, 20).Select(i => Trabalhar($"tarefa {i}")));

        Assert.Equal(20, registros.Count);
        Assert.All(registros, r => Assert.Single(r.Escopos));        // nenhuma tarefa enxerga o escopo de outra
    }
}
```

Três testes contam a história do capítulo. **`Fechar_escopos_fora_de_ordem_e_um_erro_de_programacao`** protege a ordem dos escopos. **`Cada_fluxo_de_execucao_tem_os_seus_proprios_escopos`** abre 20 tarefas ao mesmo tempo e confere que nenhuma enxerga o escopo de outra. **`A_hora_vem_do_relogio_injetado_e_o_formato_e_legivel`** mostra o formato `[Aviso] 09:00:00 > entrega: revisão pendente {dias=2}`. E **`A_lanterna_apagada_aceita_tudo_e_nao_mostra_nada`** prova que o "nada" também é um comportamento testável.

::: exercicio Um resumo da noite
Escreva o método de extensão `Resumo(this IEnumerable<Registro> registros)`, que devolve uma linha com **quantos registros há em cada nível**, do mais grave ao mais leve, separados por " · ": por exemplo, `"Erro: 1 · Informacao: 2"`. Uma lista vazia dá texto vazio.

--- resposta
```cs Jornada/Joao/Exercicios/Cap124Resumo.cs
using Jornada.Joao.Cap124;

namespace Jornada.Joao.Exercicios;

public static class RegistroExtensoes
{
    /// <summary>Uma linha com quantos registros há em cada nível, do mais grave ao mais leve: "Erro: 1 · Informacao: 2".</summary>
    public static string Resumo(this IEnumerable<Registro> registros) =>
        string.Join(" · ",
            registros
                .GroupBy(r => r.Nivel)
                .OrderByDescending(g => g.Key)
                .Select(g => $"{g.Key}: {g.Count()}"));
}
```

`GroupBy(r => r.Nivel)` agrupa; `OrderByDescending(g => g.Key)` põe o **mais grave primeiro** (o `enum` tem valores 0 a 3, então maior = mais grave); e `string.Join` junta os pedaços. Uma lista vazia gera **zero grupos**, e `Join` de nada é o texto vazio, sem tratamento especial.
:::

::: resumo
- Em **Jo 1,1**, o texto distingue **"estava junto de"** de **"era"**. Em C#: **referência** (`EstavaCom`) e **herança** (`: Deus`). O `:` diz "é do tipo", e `is` testa o tipo de um objeto.
- `new` **cria**; o texto diz que a Palavra **era** (já existia). Por isso o tipo é **declarado**, e o `new` dos exemplos é só do programa.
- A **luz** brilha nas trevas (1,5). Num programa, a "luz" é o **registro** (*log*), uma ferramenta de engenharia: a analogia é só de **estrutura**.
- **Escopos** aninhados abrem e fecham sozinhos (`IDisposable` + `using`) e, com `AsyncLocal`, valem por tarefa.
- Um **registro estruturado** guarda mensagem e **campos separados**; **níveis** decidem o que merece luz.
- O **objeto nulo** (`LanternaApagada`) torna o "nada" um valor seguro, sem `if` espalhado.
:::

::: quiz
? Em C#, qual recurso expressa "a Palavra estava junto de Deus"?
+ Uma propriedade que guarda uma referência a outro objeto (`EstavaCom`)
- Herança (`class Verbo : Deus`)
- Um `enum`
- Um laço `for`
> "Estar junto de" liga dois objetos: os dois continuam sendo dois.

? E "a Palavra era Deus"?
+ Herança: `class Verbo : Deus`, que diz que Verbo é do tipo Deus
- Uma lista
- Uma exceção
- Um método estático
> O `:` declara "é do tipo"; é uma relação entre tipos.

? Por que o capítulo evita criar o Verbo com `new` ao representar "era"?
+ Porque `new` cria um objeto num instante, e o texto diz que a Palavra já existia no princípio
- Porque `new` é lento
- Porque `new` não funciona com herança
- Porque o C# proíbe
> João usa *ēn* ("era") para a Palavra e *egeneto* ("veio a ser") para o que foi feito.

? O que `ReferenceEquals(verbo, deus)` responde?
+ Se são o mesmo objeto na memória; não diz nada sobre a natureza de Deus ou da Palavra
- Se um herda do outro
- Se os dois têm o mesmo nome
- Se Deus foi criado
> Herança, `is` e `ReferenceEquals` são perguntas diferentes de C#.

? Por que a `Lanterna` usa uma lista imutável por fluxo de execução (`AsyncLocal`) em vez de uma `Stack` compartilhada?
+ Para que tarefas simultâneas não misturem os seus escopos
- Para economizar memória
- Para escrever mais rápido
- Para evitar o `using`
> Cada tarefa enxerga a sua própria lista de escopos.

? Quantas vezes aparece a palavra grega para "luz" (*phōs*) em João?
+ 23 vezes
- 2 vezes
- 230 vezes
- Só no prólogo
> A contagem vem do texto grego SBLGNT; a luz atravessa o Evangelho.

? Para que serve `IDisposable` em `Escopo()`?
+ Para que o bloco `using` feche o escopo sozinho, mesmo se houver exceção
- Para gerar mensagens em maiúsculas
- Para apagar os registros
- Para trocar o nível mínimo
> O `Dispose()` roda ao final do `using`.

? Por que existe a classe `LanternaApagada`?
+ Para quem não quer registro entregar um objeto que aceita tudo e não faz nada, sem testar `null`
- Para economizar memória
- Para gerar erros
- Para registrar em arquivo
> É o padrão "objeto nulo".

? O que acontece se um registro de nível `Detalhe` for escrito com nível mínimo `Aviso`?
+ Nada: ele nem chega ao destino
- Ele vira `Aviso`
- Lança exceção
- É guardado para depois
> Abaixo do mínimo, a luz nem acende.
:::

::: cartas
Estava junto de (Jo 1,1) :: Em C#, uma referência: um objeto ligado a outro.
Era (Jo 1,1) :: Em C#, herança: um tipo é do tipo de outro (`: Deus`). Não representa criação.
AsyncLocal :: Valor que cada fluxo de execução (tarefa) enxerga como seu.
Log estruturado :: Mensagem mais campos separados (nome e valor), fáceis de buscar.
Nível de log :: Grau de gravidade: detalhe, informação, aviso, erro.
Escopo :: "Cena" que carrega seu nome em todos os registros escritos dentro dela.
IDisposable :: Contrato "o que abri, eu fecho": `Dispose()` roda ao fim do `using`.
Objeto nulo :: Objeto que cumpre o contrato e não faz nada, no lugar de `null`.
:::
