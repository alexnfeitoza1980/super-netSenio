---
id: resiliencia-cap93
order: 119
kind: capitulo
num: 93
group: C# & Mentalidade · Parte 15 · Resiliência em C#
title: Ninguém precisa ser uma ilha
subtitle: Redes de apoio, empatia e o jeito de pedir ajuda com começo, meio e fim. Em C#, é o capítulo das interfaces: tipos diferentes que respondem à mesma pergunta, cada um do seu jeito.
minutes: 65
hue: 140
objectives:
- Reconhecer **empatia** como curiosidade respeitosa e **apoio** como parte da resiliência
- Definir um **contrato** com `interface` e compartilhar código com **classe abstrata**
- Aplicar **polimorfismo**: o mesmo método, comportamentos diferentes por tipo
- Estender o modelo com um **tipo novo** sem alterar o código que já existia
origem: livros/06-resiliencia-csharp/content/130-cap93.md
---

::: vila O lanche que ficou na geladeira
Bia passou a semana sem almoçar direito, entre a faculdade e o estágio. Marta percebeu, mas não disse "você precisa comer". Perguntou: "quer que eu deixe um lanche aqui, ou prefere que a gente almoce junto na quinta?". Bia escolheu a quinta. Marta **não adivinhou** do que Bia precisava: ofereceu duas opções e aceitou a resposta. Na semana seguinte, foi Bia quem trouxe o lanche.
:::

::: ilustracao img/resiliencia/cap93.svg | Uma rede de apoio tem pessoas, profissionais e serviços. Cada um ajuda de um jeito, e a pessoa escolhe a quem pedir.
:::

## O que o livro diz

O texto sobre resiliência trata das **relações**: **empatia** e **apoio** de outras pessoas estão entre os fatores que ajudam a atravessar dificuldades. Em palavras nossas:

- **Empatia** é a **curiosidade respeitosa** de entender como a situação é para o outro, **perguntando** em vez de presumir ("quer que eu...?", "como é isso para você?").
- **Apoio** vem de pessoas **e** de serviços, e **muda conforme a necessidade**: para desabafar, ajuda uma pessoa que escuta; para uma dúvida técnica, alguém que entende do assunto; para uma urgência, um serviço preparado para isso.
- **Pedir ajuda** é uma habilidade, e funciona melhor quando o pedido é **claro**: a quem, o quê, quando e qual é o limite. Isso dá à outra pessoa a chance de dizer "sim" ou "hoje não dá" sem culpa.

::: cuidado Apoio respeita a autonomia
Ajudar **não é** decidir pelo outro, nem resolver tudo, nem exigir gratidão. E **receber** ajuda não é dívida. Também vale lembrar que **nem toda ajuda é boa**: relações que desrespeitam, humilham ou machucam **não** são apoio. Se houver risco imediato, procure um serviço de emergência (**SAMU 192**) ou apoio emocional (**CVV 188**, 24 h, gratuito).
:::

::: ilustracao img/resiliencia/photos/lago-e-companhia.jpg | Atravessar com companhia: remar a dois não tira o esforço, mas divide o peso e o caminho.
:::

## Da vida para o código

| Na vida | Em C# |
|---|---|
| "O que cada um da minha rede pode oferecer?" | **`interface IApoio`**: um contrato (`Ajuda`, `ComoPedir`) |
| Amiga, profissional e serviço pedem de jeitos diferentes | Classes `Pessoa`, `Profissional`, `Servico` com **`override`** |
| O que há em comum entre eles | **Classe abstrata** `ApoioBase` com o código compartilhado |
| "Quem pode me ajudar com isto?" | `Quem(necessidade)`: LINQ sobre `IApoio` |
| "O que ninguém na minha rede cobre ainda?" | `Lacunas()` como convite, não como falha |
| Um pedido com começo, meio e fim | `record Pedido` com validação |
| Em urgência, serviços públicos **sempre** aparecem | Regra fixa em `Quem`, que não depende do mapa |

::: figura Uma interface, vários jeitos de pedir
<svg viewBox="0 0 740 250" role="img" aria-label="A interface IApoio no topo e quatro tipos abaixo, cada um com sua forma de pedir ajuda">
  <rect x="250" y="14" width="240" height="56" rx="12" class="f-prim"/><text x="370" y="38" text-anchor="middle" font-weight="800">«interface» IApoio</text><text x="370" y="58" text-anchor="middle" font-size="12" class="t-dim">Ajuda(necessidade) · ComoPedir(necessidade)</text>
  <g font-size="12" text-anchor="middle">
    <rect x="20" y="150" width="160" height="80" rx="12" class="f-good"/><text x="100" y="176" font-weight="800">Pessoa</text><text x="100" y="196" class="t-dim">"Oi, Marta! Teria um tempinho?"</text>
    <rect x="200" y="150" width="160" height="80" rx="12" class="f-info"/><text x="280" y="176" font-weight="800">Profissional</text><text x="280" y="196" class="t-dim">"Gostaria de marcar um horário"</text>
    <rect x="380" y="150" width="160" height="80" rx="12" class="f-warn"/><text x="460" y="176" font-weight="800">Servico</text><text x="460" y="196" class="t-dim">"Ligue 188. Sem explicar tudo"</text>
    <rect x="560" y="150" width="160" height="80" rx="12" class="f-prim"/><text x="640" y="176" font-weight="800">GrupoDeEstudo (novo!)</text><text x="640" y="196" class="t-dim">"Posso participar do encontro?"</text>
  </g>
  <path class="arrow" d="M100 148L330 72"/><path class="arrow" d="M280 148L350 72"/><path class="arrow" d="M460 148L390 72"/><path class="arrow" d="M640 148L430 72"/>
</svg>
:::

## 1) O contrato e o código em comum

Uma **`interface`** é um **contrato**: lista o que um tipo promete saber fazer, sem dizer **como**. `IApoio` promete duas coisas: responder se **ajuda** com uma necessidade e dizer **como pedir**. Qualquer tipo que cumpra o contrato pode entrar no mesmo mapa.

A classe **abstrata** `ApoioBase` guarda o que é **comum** (o nome e o conjunto de necessidades atendidas, em um `HashSet`, que testa "contém?" muito rápido) e deixa o **jeito de pedir** para cada filho decidir: `abstract string ComoPedir`. Cada filho **sobrescreve** (`override`) o método do seu jeito. Esse é o **polimorfismo**: a mesma chamada, comportamentos diferentes, conforme o tipo real do objeto.

```cs Jornada/Resiliencia/Cap93/Apoios.cs
namespace Jornada.Resiliencia.Cap93;

public enum Necessidade { Ouvir, Estudar, TarefaPratica, Orientacao, Urgencia }

public interface IApoio
{
    string Nome { get; }
    bool Ajuda(Necessidade necessidade);
    string ComoPedir(Necessidade necessidade);
}

public abstract class ApoioBase : IApoio
{
    private readonly HashSet<Necessidade> atende;

    protected ApoioBase(string nome, params Necessidade[] atende)
    {
        Nome = nome;
        this.atende = new HashSet<Necessidade>(atende);
    }

    public string Nome { get; }

    public bool Ajuda(Necessidade necessidade) => atende.Contains(necessidade);

    public abstract string ComoPedir(Necessidade necessidade);

    protected static string Assunto(Necessidade n) => n switch
    {
        Necessidade.Ouvir         => "conversar um pouco",
        Necessidade.Estudar       => "estudar junto",
        Necessidade.TarefaPratica => "dar uma mão numa tarefa",
        Necessidade.Orientacao    => "pedir uma orientação",
        _                         => "falar com urgência"
    };
}

public sealed class Pessoa : ApoioBase
{
    public Pessoa(string nome, params Necessidade[] atende) : base(nome, atende) { }

    public override string ComoPedir(Necessidade n) =>
        $"Oi, {Nome}. Você teria um tempinho para {Assunto(n)}? Pode ser rápido.";
}

public sealed class Profissional : ApoioBase
{
    public Profissional(string nome, params Necessidade[] atende) : base(nome, atende) { }

    public override string ComoPedir(Necessidade n) =>
        $"Olá. Gostaria de marcar um horário com {Nome} para {Assunto(n)}.";
}

public sealed class Servico : ApoioBase
{
    public string Telefone { get; }

    public Servico(string nome, string telefone, params Necessidade[] atende) : base(nome, atende) =>
        Telefone = telefone;

    public override string ComoPedir(Necessidade n) =>
        $"{Nome}: ligue {Telefone}. Você não precisa explicar tudo de uma vez.";
}

public static class ServicosPublicos
{
    /// <summary>Serviços do Brasil que existem para urgências. Valem para qualquer pessoa, a qualquer hora.</summary>
    public static IReadOnlyList<Servico> Brasil { get; } = new[]
    {
        new Servico("CVV (apoio emocional)", "188", Necessidade.Ouvir, Necessidade.Urgencia),
        new Servico("SAMU (emergência médica)", "192", Necessidade.Urgencia),
    };
}
```

Um detalhe de cuidado: `ServicosPublicos.Brasil` lista o **CVV (188)** e o **SAMU (192)**, e o `Servico` ensina que **"você não precisa explicar tudo de uma vez"**. O texto também faz parte do código, e **a forma de pedir ajuda importa**.

## 2) O mapa e o pedido

O `MapaDeApoio` guarda `IApoio` (**nunca** os tipos concretos). Para saber quem pode ajudar, ele só chama `Ajuda`; por isso, **qualquer tipo novo funciona** sem mexer no mapa. Em **urgência**, os serviços públicos **entram sempre**, mesmo que o mapa esteja vazio.

O `record Pedido` valida os quatro campos e monta a frase com **começo** (para quem), **meio** (o quê e quando) e **fim** (um limite gentil, que deixa a outra pessoa livre para dizer não).

```cs Jornada/Resiliencia/Cap93/MapaDeApoio.cs
namespace Jornada.Resiliencia.Cap93;

public sealed class MapaDeApoio
{
    private readonly List<IApoio> apoios = new();

    public MapaDeApoio Adicionar(IApoio apoio) { apoios.Add(apoio); return this; }

    /// <summary>Quem pode ajudar com isso? Um "foreach" sobre a interface chama o código de cada tipo.</summary>
    public IReadOnlyList<IApoio> Quem(Necessidade necessidade)
    {
        var lista = apoios.Where(a => a.Ajuda(necessidade)).ToList();

        // Em urgência, os serviços públicos sempre entram na lista, mesmo que o mapa esteja vazio.
        if (necessidade == Necessidade.Urgencia)
            lista.AddRange(ServicosPublicos.Brasil.Where(s => s.Ajuda(necessidade) && !lista.Contains(s)));

        return lista;
    }

    /// <summary>Necessidades que ninguém do mapa cobre ainda. É um convite para ampliar a rede, não uma falha.</summary>
    public IReadOnlyList<Necessidade> Lacunas() =>
        Enum.GetValues<Necessidade>()
            .Where(n => n != Necessidade.Urgencia && !apoios.Any(a => a.Ajuda(n)))
            .ToList();
}

/// <summary>Um pedido com começo (quem), meio (o quê, quando) e fim (um limite gentil).</summary>
public sealed record Pedido(string Para, string OQue, string Quando, string Limite)
{
    public string Montar()
    {
        foreach (var (campo, valor) in new[] { (nameof(Para), Para), (nameof(OQue), OQue), (nameof(Quando), Quando), (nameof(Limite), Limite) })
        {
            if (string.IsNullOrWhiteSpace(valor))
                throw new ArgumentException($"Preencha \"{campo}\" para o pedido ficar claro.");
        }

        return $"{Para}, {OQue} {Quando}. {Limite}";
    }
}
```

```cs Uso
var mapa = new MapaDeApoio()
    .Adicionar(new Pessoa("Marta", Necessidade.Ouvir, Necessidade.TarefaPratica))
    .Adicionar(new Pessoa("Beto", Necessidade.Estudar));

foreach (var apoio in mapa.Quem(Necessidade.Estudar))
    Console.WriteLine(apoio.ComoPedir(Necessidade.Estudar));

Console.WriteLine("Lacunas: " + string.Join(", ", mapa.Lacunas()));

// a lacuna é um convite: quem poderia orientar?
mapa.Adicionar(new Profissional("a orientadora Ana", Necessidade.Orientacao));
Console.WriteLine("Depois de incluir a orientadora: " + (mapa.Lacunas().Count == 0 ? "sem lacunas" : "ainda há lacunas"));
Console.WriteLine(mapa.Quem(Necessidade.Orientacao)[0].ComoPedir(Necessidade.Orientacao));

foreach (var apoio in mapa.Quem(Necessidade.Urgencia))
    Console.WriteLine(apoio.ComoPedir(Necessidade.Urgencia));

var pedido = new Pedido("Beto", "você poderia estudar comigo", "amanhã às 19h por 30 minutos",
                        "Se não der, tudo bem, procuro outro horário.");
Console.WriteLine(pedido.Montar());
```

```saida
Oi, Beto. Você teria um tempinho para estudar junto? Pode ser rápido.
Lacunas: Orientacao
Depois de incluir a orientadora: sem lacunas
Olá. Gostaria de marcar um horário com a orientadora Ana para pedir uma orientação.
CVV (apoio emocional): ligue 188. Você não precisa explicar tudo de uma vez.
SAMU (emergência médica): ligue 192. Você não precisa explicar tudo de uma vez.
Beto, você poderia estudar comigo amanhã às 19h por 30 minutos. Se não der, tudo bem, procuro outro horário.
```

Note que `Lacunas` listou `Orientacao`, e isso é um **convite** ("quem poderia orientar?"), não uma falha. E que, em urgência, os dois serviços aparecem **mesmo sem terem sido cadastrados**.

## 3) Testes

```cs Resiliencia.Testes/Cap93Tests.cs
using Jornada.Resiliencia.Cap93;

namespace Resiliencia.Testes;

public class Cap93Tests
{
    private static MapaDeApoio MapaDaBia() => new MapaDeApoio()
        .Adicionar(new Pessoa("Marta", Necessidade.Ouvir, Necessidade.TarefaPratica))
        .Adicionar(new Pessoa("Beto", Necessidade.Estudar))
        .Adicionar(new Profissional("orientadora Ana", Necessidade.Orientacao));

    [Fact]
    public void Cada_tipo_de_apoio_pede_do_seu_jeito()
    {
        Assert.StartsWith("Oi, Marta", new Pessoa("Marta", Necessidade.Ouvir).ComoPedir(Necessidade.Ouvir));
        Assert.StartsWith("Olá.", new Profissional("Dra. Lia", Necessidade.Orientacao).ComoPedir(Necessidade.Orientacao));
        Assert.Contains("ligue 188", new Servico("CVV", "188", Necessidade.Ouvir).ComoPedir(Necessidade.Ouvir));
    }

    [Fact]
    public void Quem_filtra_pelas_necessidades_atendidas()
    {
        var quem = MapaDaBia().Quem(Necessidade.Ouvir);
        Assert.Equal("Marta", Assert.Single(quem).Nome);
    }

    [Fact]
    public void Em_urgencia_os_servicos_publicos_sempre_aparecem()
    {
        var vazio = new MapaDeApoio();
        var quem = vazio.Quem(Necessidade.Urgencia);

        Assert.Contains(quem, a => a.Nome.StartsWith("CVV"));
        Assert.Contains(quem, a => a.Nome.StartsWith("SAMU"));
    }

    [Fact]
    public void Lacunas_mostram_o_que_ninguem_cobre_ainda()
    {
        var mapa = new MapaDeApoio().Adicionar(new Pessoa("Marta", Necessidade.Ouvir));
        Assert.Equal(new[] { Necessidade.Estudar, Necessidade.TarefaPratica, Necessidade.Orientacao }, mapa.Lacunas());
    }

    [Fact]
    public void Pedido_completo_tem_comeco_meio_e_fim()
    {
        var pedido = new Pedido("Beto", "você poderia estudar comigo", "amanhã às 19h por 30 minutos",
                                "Se não der, tudo bem, procuro outro horário.");

        Assert.Equal(
            "Beto, você poderia estudar comigo amanhã às 19h por 30 minutos. Se não der, tudo bem, procuro outro horário.",
            pedido.Montar());
    }

    [Fact]
    public void Pedido_vago_e_recusado_com_o_nome_do_campo()
    {
        var e = Assert.Throws<ArgumentException>(() => new Pedido("Beto", "ajuda", "", "ok").Montar());
        Assert.Contains("Quando", e.Message);
    }
}
```

::: dica Programe para a interface, não para a classe
Quem usa `MapaDeApoio` só conhece `IApoio`. Isso permite **trocar** ou **acrescentar** implementações sem quebrar quem já usa. É um dos pilares do design orientado a objetos e a razão de o `foreach` sobre uma lista de `IApoio` poder chamar `ComoPedir` de cada tipo sem `if` por tipo.
:::

::: exercicio Um tipo novo de apoio
Crie `GrupoDeEstudo`, um novo tipo de `IApoio` que atende **estudar** e **ouvir** e pede assim: `"Posso participar do encontro do grupo X desta semana? Levo as minhas dúvidas."`. **Não altere** `MapaDeApoio`. Teste que um `MapaDeApoio` com um `GrupoDeEstudo` responde a `Quem(Necessidade.Estudar)`.

--- resposta
```cs Jornada/Resiliencia/Exercicios/Cap93Grupo.cs
namespace Jornada.Resiliencia.Cap93;

/// <summary>Um tipo novo de apoio. O MapaDeApoio não precisa mudar nada para aceitá-lo.</summary>
public sealed class GrupoDeEstudo : ApoioBase
{
    public GrupoDeEstudo(string nome) : base(nome, Necessidade.Estudar, Necessidade.Ouvir) { }

    public override string ComoPedir(Necessidade n) =>
        $"Posso participar do encontro do grupo {Nome} desta semana? Levo as minhas dúvidas.";
}
```

Basta herdar de `ApoioBase`, passar as necessidades ao construtor e escrever o `override`. O `MapaDeApoio` **continua intacto**, a prova de que o contrato funciona: **aberto para extensão, fechado para modificação**.
:::

::: resumo
- **Empatia** é curiosidade respeitosa: pergunte, não presuma. **Apoio** vem de pessoas **e** de serviços, e muda conforme a necessidade.
- Um **pedido claro** tem começo (quem), meio (o quê, quando) e fim (um limite que permite dizer não).
- Uma **`interface`** é um contrato; uma **classe abstrata** reúne o código comum; `override` dá a cada tipo o seu jeito: **polimorfismo**.
- Programar para `IApoio` permite **incluir tipos novos** (como `GrupoDeEstudo`) sem alterar o mapa.
- Em **urgência**, os serviços públicos (CVV 188, SAMU 192) sempre aparecem, e as **lacunas** da rede são um convite, não uma falha.
:::

::: quiz
? O que é empatia, nas palavras deste capítulo?
+ Curiosidade respeitosa para entender como a situação é para o outro
- Sentir exatamente o mesmo que a outra pessoa
- Resolver o problema do outro
- Concordar com tudo
> Perguntar, em vez de presumir, é parte da empatia.

? Para que serve uma `interface` em C#?
+ Para definir um contrato que vários tipos diferentes podem cumprir
- Para guardar dados em disco
- Para criar janelas
- Para repetir código
> O contrato diz o que o tipo promete fazer, não como.

? O que é polimorfismo, na prática?
+ A mesma chamada de método produz comportamentos diferentes conforme o tipo real do objeto
- Ter muitas variáveis com o mesmo nome
- Converter números em texto
- Criar várias cópias de uma classe
> O `foreach` sobre `IApoio` chama o `ComoPedir` de cada tipo.

? Por que o `MapaDeApoio` aceitou `GrupoDeEstudo` sem nenhuma mudança?
+ Porque ele só conhece `IApoio`, e `GrupoDeEstudo` cumpre o contrato
- Porque o C# adivinha o tipo
- Porque o mapa foi reescrito
- Porque `GrupoDeEstudo` não é uma classe
> Programar para a interface permite estender sem modificar.

? Por que `Quem(Necessidade.Urgencia)` inclui sempre o CVV e o SAMU?
+ Porque, em urgência, os serviços públicos precisam estar à mão mesmo que a pessoa não tenha cadastrado ninguém
- Porque o código sempre falha
- Porque eles são os únicos apoios válidos
- Porque a lista estava vazia por engano
> É uma regra de cuidado do modelo.

? O que é um bom pedido de ajuda, segundo o capítulo?
+ Claro (a quem, o quê, quando) e com um limite gentil que permite dizer não
- Vago, para não incomodar
- Exigente, para ser levado a sério
- Feito só por escrito
> Clareza ajuda; o limite gentil protege a liberdade de quem recebe.
:::

::: cartas
Empatia :: Esforço de compreender a perspectiva do outro e perguntar do que ele precisa.
Rede de apoio :: Pessoas e serviços que podem ajudar de formas diferentes.
interface :: Contrato que lista o que um tipo promete fazer.
Classe abstrata :: Classe que reúne código comum e deixa partes para os filhos implementarem.
override :: Sobrescrever, no filho, um método abstrato ou virtual da classe base.
Polimorfismo :: Mesma chamada, comportamentos diferentes conforme o tipo real.
HashSet<T> :: Conjunto sem repetição, ótimo para perguntar "contém?".
Aberto/fechado :: Estender comportamento sem modificar o código que já funciona.
:::
