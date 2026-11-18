---
id: agil-cap38
order: 55
kind: capitulo
num: 38
group: C# · Parte 11 · Pacotes, padrões e arquitetura
title: Visitor e sua família
subtitle: Como acrescentar operações novas a uma hierarquia de classes sem mexer nela — Visitor, Acyclic Visitor, Decorator e Extension Object.
minutes: 70
hue: 175
objectives:
- Entender o problema de **adicionar métodos a uma hierarquia** sem violar o OCP e o CCP
- Dominar o **double dispatch** que move o Visitor
- Saber quando usar **Acyclic Visitor** (e o que ele custa)
- Usar **Decorator** para acrescentar comportamento e **Extension Object** para acrescentar *capacidades* inteiras
origem: livros/02-projeto-agil-csharp/content/54-cap38.md
---

::: ilustracao img/agil/cap38.svg | Visitor: uma operação nova percorre uma hierarquia sem alterar as peças.
:::

## O problema: operações novas, hierarquia congelada

Você tem uma hierarquia de classes estável — digamos, dispositivos de uma casa inteligente: `Termostato`, `Lampada`, `Camera`. Chega o requisito: "precisamos **configurar** todos os dispositivos para o protocolo Zigbee". Depois: "agora para Matter". Depois: "gere um **relatório** de consumo".

A reação ingênua é pendurar um método `ConfigurarParaZigbee()` na interface `IDispositivo`. O resultado é desastroso:

- cada novo protocolo/relatório **reabre a interface** e obriga a recompilar e reimplantar **todos** os dispositivos (viola o **OCP**);
- classes que deveriam falar de *dispositivos* acabam cheias de coisas sobre Zigbee, relatórios, XML… (viola o **SRP** e o **CCP**: elas mudam por motivos diferentes).

A **família Visitor** resolve isso: são quatro padrões que permitem **adicionar funcionalidade a uma hierarquia existente sem modificá-la**.

::: figura A família
<svg viewBox="0 0 720 150" role="img" aria-label="Quatro padrões: Visitor, Acyclic Visitor, Decorator e Extension Object">
  <rect class="f-prim" x="10" y="40" width="160" height="70" rx="12"/><text x="90" y="70" text-anchor="middle" font-weight="800">Visitor</text><text x="90" y="92" text-anchor="middle" font-size="11" class="t-dim">nova operação</text>
  <rect class="f-info" x="190" y="40" width="160" height="70" rx="12"/><text x="270" y="70" text-anchor="middle" font-weight="800">Acyclic Visitor</text><text x="270" y="92" text-anchor="middle" font-size="11" class="t-dim">sem ciclo de dependência</text>
  <rect class="f-good" x="370" y="40" width="160" height="70" rx="12"/><text x="450" y="70" text-anchor="middle" font-weight="800">Decorator</text><text x="450" y="92" text-anchor="middle" font-size="11" class="t-dim">comportamento extra</text>
  <rect class="f-warn" x="550" y="40" width="160" height="70" rx="12"/><text x="630" y="70" text-anchor="middle" font-weight="800">Extension Object</text><text x="630" y="92" text-anchor="middle" font-size="11" class="t-dim">capacidades plugáveis</text>
</svg>
:::

## Visitor e o double dispatch

A estrutura: a hierarquia visitada ganha **um único método** `Aceitar(visitante)`. O visitante, por sua vez, tem **um método `Visitar` para cada tipo concreto** da hierarquia. Adicionar uma operação nova = criar **um visitante novo**; a hierarquia não muda.

```cs
public interface IVisitanteDeDispositivo
{
    void Visitar(Termostato t);
    void Visitar(Lampada l);
    void Visitar(Camera c);
}

public interface IDispositivo
{
    string Nome { get; }
    void Aceitar(IVisitanteDeDispositivo v);
}

public class Termostato : IDispositivo
{
    public string Nome { get { return "Termostato"; } }
    public int TemperaturaAlvo { get; set; }
    public string PerfilRadio { get; set; }

    public void Aceitar(IVisitanteDeDispositivo v) { v.Visitar(this); }   // 'this' é Termostato!
}

public class Lampada : IDispositivo
{
    public string Nome { get { return "Lâmpada"; } }
    public int Brilho { get; set; }
    public string CanalRadio { get; set; }

    public void Aceitar(IVisitanteDeDispositivo v) { v.Visitar(this); }
}

public class Camera : IDispositivo
{
    public string Nome { get { return "Câmera"; } }
    public string ModoDeEnlace { get; set; }

    public void Aceitar(IVisitanteDeDispositivo v) { v.Visitar(this); }
}

// Nova operação = nova classe. Nenhum dispositivo é tocado.
public class ConfiguradorZigbee : IVisitanteDeDispositivo
{
    public void Visitar(Termostato t) { t.PerfilRadio = "zb-hvac"; }
    public void Visitar(Lampada l)    { l.CanalRadio = "zb-ch-15"; }
    public void Visitar(Camera c)     { c.ModoDeEnlace = "zb-bridge"; }
}
```

Uso e teste:

```cs
IDispositivo d = new Lampada();
d.Aceitar(new ConfiguradorZigbee());
// agora ((Lampada)d).CanalRadio == "zb-ch-15"
```

### O truque: dois despachos polimórficos

Repare que quem chama `d.Aceitar(...)` só conhece `IDispositivo` e `IVisitanteDeDispositivo` — **dois tipos abstratos**. Mesmo assim, o código certo roda. Como?

1. **Primeiro despacho:** `d.Aceitar(v)` — o runtime escolhe, pelo tipo real de `d`, a implementação de `Aceitar` (a da `Lampada`).
2. **Segundo despacho:** dentro dela, `v.Visitar(this)` — como `this` tem o tipo estático `Lampada`, o compilador escolhe a sobrecarga `Visitar(Lampada)`, e o runtime escolhe a implementação pelo tipo real de `v`.

::: figura Double dispatch
<svg viewBox="0 0 720 190" role="img" aria-label="d.Aceitar(v) chama Lampada.Aceitar que chama v.Visitar(Lampada)">
  <g text-anchor="middle" font-size="12"><rect class="f-prim" x="10" y="10" width="120" height="30" rx="4"/><text x="70" y="30">cliente</text><rect class="f-info" x="230" y="10" width="140" height="30" rx="4"/><text x="300" y="30">d : Lampada</text><rect class="f-good" x="520" y="10" width="190" height="30" rx="4"/><text x="615" y="30">v : ConfiguradorZigbee</text></g>
  <path d="M70 42V180M300 42V180M615 42V180" stroke="var(--ink-3)" stroke-dasharray="5 4"/>
  <path class="arrow" d="M72 70H298"/><text x="185" y="62" text-anchor="middle" font-size="11" class="t-mono">1º despacho: Aceitar(v)</text>
  <path class="arrow" d="M302 118H612"/><text x="457" y="110" text-anchor="middle" font-size="11" class="t-mono">2º despacho: Visitar(this = Lampada)</text>
  <path d="M612 150H302" stroke="var(--ink-2)" stroke-dasharray="5 4" fill="none" marker-end="url(#arr2)"/>
</svg>
:::

O resultado é uma **matriz de funções**: um eixo são os **tipos de dispositivo**; o outro, as **operações** (visitantes). Cada célula é uma função que diz *"como fazer esta operação neste tipo"*. Visitor é **rápido**: sempre exatamente dois despachos, não importa o tamanho da hierarquia.

::: pense Um giro de 90°
O Visitor transforma **derivadas em métodos**: em vez de "cada dispositivo sabe se configurar", temos "cada operação sabe tratar cada dispositivo". É o oposto do polimorfismo comum — e por isso resolve problemas que o polimorfismo comum não resolve (adicionar **operações**) e cria o problema inverso (adicionar **tipos** fica caro).
:::

## Visitor em estruturas compostas: relatórios

O uso mais frequente do Visitor é **percorrer uma estrutura** (árvore) e produzir **relatórios** — sem que a estrutura saiba de relatório algum. Vejamos um sistema de arquivos simples:

```cs
public interface IItemDeDisco
{
    string Nome { get; }
    void Aceitar(IVisitanteDeDisco v);
}

public interface IVisitanteDeDisco
{
    void Visitar(Arquivo a);
    void Visitar(Pasta p);
}

public class Arquivo : IItemDeDisco
{
    public string Nome { get; private set; }
    public long Bytes { get; private set; }
    public Arquivo(string nome, long bytes) { Nome = nome; Bytes = bytes; }

    public void Aceitar(IVisitanteDeDisco v) { v.Visitar(this); }
}

public class Pasta : IItemDeDisco
{
    private readonly List<IItemDeDisco> filhos = new List<IItemDeDisco>();
    public string Nome { get; private set; }
    public IReadOnlyList<IItemDeDisco> Filhos { get { return filhos; } }
    public Pasta(string nome) { Nome = nome; }
    public void Adicionar(IItemDeDisco item) { filhos.Add(item); }

    public void Aceitar(IVisitanteDeDisco v)
    {
        v.Visitar(this);                       // visita a si mesma...
        foreach (var filho in filhos)          // ...e propaga a visita pela árvore
            filho.Aceitar(v);
    }
}
```

Relatórios viram **classes pequenas e independentes**:

```cs
public class TamanhoTotal : IVisitanteDeDisco
{
    public long Bytes { get; private set; }
    public void Visitar(Arquivo a) { Bytes += a.Bytes; }
    public void Visitar(Pasta p)   { }
}

public class Contador : IVisitanteDeDisco
{
    public int Arquivos { get; private set; }
    public int Pastas { get; private set; }
    public void Visitar(Arquivo a) { Arquivos++; }
    public void Visitar(Pasta p)   { Pastas++; }
}
```

```cs
var raiz = new Pasta("raiz");
raiz.Adicionar(new Arquivo("a.txt", 10));
raiz.Adicionar(new Arquivo("b.txt", 20));
var sub = new Pasta("fotos");
sub.Adicionar(new Arquivo("c.png", 5));
raiz.Adicionar(sub);

var total = new TamanhoTotal();
raiz.Aceitar(total);
Console.WriteLine(total.Bytes);                    // 35

var cont = new Contador();
raiz.Aceitar(cont);
Console.WriteLine(cont.Arquivos + " arquivos, " + cont.Pastas + " pastas");
```

```saida
35
3 arquivos, 2 pastas
```

Isso honra o **SRP**: a hierarquia muda quando surgem novos tipos de item; relatórios mudam quando o negócio pede relatórios novos — são motivos diferentes. E cada relatório pode ser **compilado e distribuído sozinho**, só para quem precisa dele.

::: dica Onde mais se usa Visitor
Em tudo que "interpreta" uma estrutura de dados de várias maneiras: **árvores de sintaxe** de compiladores (checar tipos, otimizar, gerar código, imprimir), estruturas de **configuração**, documentos, árvores de UI. O próprio .NET traz `ExpressionVisitor` (em `System.Linq.Expressions`) e `CSharpSyntaxVisitor`/`CSharpSyntaxWalker` (Roslyn) — são Visitors de verdade.
:::

## O preço do Visitor: o ciclo de dependência

Olhe de novo as setas: `IItemDeDisco` depende de `IVisitanteDeDisco` (por causa de `Aceitar`), e o visitante tem **um método para cada derivada** (`Arquivo`, `Pasta`) — logo, depende de todas elas. Fecha-se um **ciclo**. Consequências:

- Adicionar **uma nova derivada** (digamos, `AtalhoDeDisco`) obriga a mudar `IVisitanteDeDisco` **e todos os visitantes** — recompilação em cascata.
- Visitor é ótimo quando a hierarquia **raramente ganha tipos novos** mas **frequentemente ganha operações novas**.

::: regra Matriz de decisão
- Muitas **operações novas**, poucos tipos novos → **Visitor**.
- Muitos **tipos novos**, poucas operações novas → **polimorfismo comum** (método virtual na hierarquia).
- Os dois mudam muito → considere Acyclic Visitor, Extension Object ou outra abordagem.
:::

## Acyclic Visitor: quebrando o ciclo

Quando a hierarquia visitada é **volátil**, usa-se o **Acyclic Visitor**: a interface base do visitante fica **vazia** (um "marcador") e existe **uma interface de visita por tipo**. Cada derivada visitada **testa** se o visitante entende o seu tipo:

```cs
public interface IVisitante { }                              // marcador: sem métodos

public interface IVisitante<T> : IVisitante                  // uma interface por tipo
{
    void Visitar(T elemento);
}

public interface IDispositivo
{
    void Aceitar(IVisitante v);
}

public class Lampada : IDispositivo
{
    public string CanalRadio { get; set; }

    public void Aceitar(IVisitante v)
    {
        if (v is IVisitante<Lampada> especifico)             // só visita quem "entende" Lampada
            especifico.Visitar(this);
    }
}

// Este visitante só sabe tratar lâmpadas e câmeras — ignora termostatos sem erro.
public class ConfiguradorMatter : IVisitante<Lampada>, IVisitante<Camera>
{
    public void Visitar(Lampada l) { l.CanalRadio = "matter-1"; }
    public void Visitar(Camera c)  { c.ModoDeEnlace = "matter-thread"; }
}
```

Vantagens e custos:

| | Visitor | Acyclic Visitor |
|---|---|---|
| Ciclo de dependência | **Sim** | **Não** |
| Novas derivadas visitadas | Caro (altera todos os visitantes) | **Fácil** (nova interface; visitantes antigos não mudam) |
| Matriz de funções | **Completa** (o compilador obriga a tratar todos os tipos) | **Esparsa** (visitante trata só alguns) |
| Complexidade e custo | Menor, 2 despachos fixos | Maior; o teste de tipo (`is`) pode custar e ser imprevisível em hierarquias grandes |
| Segurança | O compilador avisa se você esqueceu um tipo | **Silencioso**: um tipo esquecido simplesmente não é visitado |

::: cuidado Nem sempre vale a complexidade
Em sistemas de **tempo real** o custo do teste de tipo pode ser inaceitável; em outros, a complexidade extra já basta para descartar o padrão. Use-o quando a hierarquia visitada for **volátil** e a **compilação incremental** importar.
:::

### Visitor moderno em C#

Se os tipos da hierarquia ficam **todos no mesmo assembly** e você só quer "uma função com um caso por tipo", o C# moderno oferece alternativas mais leves:

```cs
// switch com pattern matching — "visitor local", sem Aceitar/Visitar
decimal Consumo(IDispositivo d) => d switch
{
    Termostato t => t.TemperaturaAlvo * 0.5m,
    Lampada l    => l.Brilho * 0.1m,
    Camera       => 4m,
    _            => throw new NotSupportedException(d.GetType().Name)
};
```

Isso é conciso, mas **vive num só lugar** e o compilador não obriga a cobrir tipos novos (a menos que use hierarquias seladas/`record`s fechados e o aviso de exaustividade). O Visitor formal ganha quando você precisa de **vários visitantes distribuíveis separadamente**, **estado acumulado** no visitante, ou **extensibilidade por terceiros**.

## Decorator: acrescentando comportamento por embrulho

Outro modo de estender sem alterar: o **Decorator**. Em vez de adicionar *operações novas*, ele **muda ou enriquece o comportamento** de operações existentes, **embrulhando** um objeto que implementa a **mesma interface**.

Problema: um sistema envia notificações. Alguns usuários querem **log** de cada envio; outros querem **retentativa** se falhar; outros os dois. Colocar `if (queroLog)` em cada ponto de envio é um pesadelo de duplicação; colocar flags dentro das classes de envio contamina quem só deveria enviar.

```cs
public interface INotificador
{
    void Enviar(string destino, string mensagem);
}

public class NotificadorEmail : INotificador
{
    public void Enviar(string destino, string mensagem) { /* SMTP... */ }
}

// Base que só delega — evita repetir a delegação em cada decorador
public abstract class NotificadorDecorador : INotificador
{
    protected readonly INotificador interno;
    protected NotificadorDecorador(INotificador interno) { this.interno = interno; }
    public virtual void Enviar(string destino, string mensagem) { interno.Enviar(destino, mensagem); }
}

public class NotificadorComLog : NotificadorDecorador
{
    private readonly ILogger log;
    public NotificadorComLog(INotificador interno, ILogger log) : base(interno) { this.log = log; }

    public override void Enviar(string destino, string mensagem)
    {
        log.Info("Enviando para " + destino);
        base.Enviar(destino, mensagem);
        log.Info("Enviado");
    }
}

public class NotificadorComRetentativa : NotificadorDecorador
{
    private readonly int tentativas;
    public NotificadorComRetentativa(INotificador interno, int tentativas) : base(interno) { this.tentativas = tentativas; }

    public override void Enviar(string destino, string mensagem)
    {
        for (int i = 1; ; i++)
        {
            try { base.Enviar(destino, mensagem); return; }
            catch (Exception) when (i < tentativas) { /* tenta de novo */ }
        }
    }
}
```

A decisão de "quem quer o quê" fica em **um único lugar** (a montagem), e o resto do sistema só enxerga um `INotificador`:

```cs
INotificador n = new NotificadorEmail();
if (usuario.QuerLog)          n = new NotificadorComLog(n, log);
if (usuario.QuerRetentativa)  n = new NotificadorComRetentativa(n, 3);

n.Enviar("ana@exemplo.com", "Sua fatura chegou");
```

::: figura Decoradores empilhados
<svg viewBox="0 0 720 150" role="img" aria-label="Retentativa envolve Log que envolve Email">
  <rect class="f-warn" x="60" y="10" width="600" height="130" rx="14"/><text x="360" y="32" text-anchor="middle" font-weight="800" class="t-warn">NotificadorComRetentativa</text>
  <rect class="f-info" x="130" y="46" width="460" height="86" rx="12"/><text x="360" y="68" text-anchor="middle" font-weight="800" class="t-info">NotificadorComLog</text>
  <rect class="f-good" x="220" y="80" width="280" height="42" rx="10"/><text x="360" y="106" text-anchor="middle" font-weight="800">NotificadorEmail</text>
</svg>
:::

Você já usa decoradores no .NET sem perceber: `BufferedStream`, `GZipStream` e `CryptoStream` **embrulham outro `Stream`**; middlewares do ASP.NET Core formam uma cadeia parecida; `Polly` e `DelegatingHandler` (HttpClient) decoram chamadas com retentativa, logging e autenticação.

::: dica Decorador vs herança
Se fôssemos fazer `NotificadorEmailComLogComRetentativa`, `NotificadorSmsComLog`… teríamos uma **explosão de subclasses**. Decoradores se **combinam em tempo de execução** — e a ordem importa (retentativa *por fora* do log registra cada tentativa; por dentro, registra uma vez).
:::

::: pense Decorator × Proxy
Estruturalmente quase idênticos (mesma interface, delegam ao interno). A **intenção** difere: o Decorator **acrescenta responsabilidades**; o Proxy **controla o acesso** ao objeto real (Capítulo 37).
:::

## Extension Object: capacidades plugáveis

O Visitor concentra **todo o código de uma operação num só objeto** (ex.: todo o código de "gerar XML" para todos os tipos fica no mesmo visitante). E se você quiser o oposto — o código de gerar XML **de cada tipo** em **sua própria classe**, e poder pedir à hierarquia "me dê sua extensão de CSV"?

No **Extension Object**, cada objeto da hierarquia guarda uma coleção de **objetos de extensão**, pesquisáveis por nome (ou por tipo). Cada extensão sabe manipular **aquele** objeto.

```cs
public interface IExtensao { }                                    // marcador

public class ExtensaoInvalida : IExtensao { }                      // Null Object: nunca devolvemos null

public abstract class ElementoDoCatalogo
{
    private readonly Dictionary<string, IExtensao> extensoes = new Dictionary<string, IExtensao>();

    public abstract string Codigo { get; }
    public abstract string Descricao { get; }

    public void AdicionarExtensao(string chave, IExtensao extensao) { extensoes[chave] = extensao; }

    public IExtensao ObterExtensao(string chave)
    {
        IExtensao e;
        return extensoes.TryGetValue(chave, out e) ? e : new ExtensaoInvalida();
    }
}

public class Item : ElementoDoCatalogo
{
    public override string Codigo { get; }
    public override string Descricao { get; }
    public decimal Preco { get; }

    public Item(string codigo, string descricao, decimal preco)
    {
        Codigo = codigo; Descricao = descricao; Preco = preco;
        AdicionarExtensao("CSV", new CsvDeItem(this));
    }
}

public class Kit : ElementoDoCatalogo
{
    private readonly List<ElementoDoCatalogo> partes = new List<ElementoDoCatalogo>();
    public override string Codigo { get; }
    public override string Descricao { get; }
    public IReadOnlyList<ElementoDoCatalogo> Partes { get { return partes; } }

    public Kit(string codigo, string descricao)
    {
        Codigo = codigo; Descricao = descricao;
        AdicionarExtensao("CSV", new CsvDeKit(this));
    }
    public void Adicionar(ElementoDoCatalogo e) { partes.Add(e); }
}

public interface IExtensaoCsv : IExtensao { string Csv { get; } }

public class CsvDeItem : IExtensaoCsv
{
    private readonly Item item;
    public CsvDeItem(Item item) { this.item = item; }
    public string Csv
    {
        get { return "Item," + item.Codigo + "," + item.Descricao + "," + item.Preco.ToString(CultureInfo.InvariantCulture); }
    }
}

public class CsvDeKit : IExtensaoCsv
{
    private readonly Kit kit;
    public CsvDeKit(Kit kit) { this.kit = kit; }
    public string Csv
    {
        get
        {
            var sb = new StringBuilder("Kit," + kit.Codigo + "," + kit.Descricao);
            foreach (var parte in kit.Partes)
            {
                var csv = (IExtensaoCsv)parte.ObterExtensao("CSV");
                sb.Append(",{").Append(csv.Csv).Append("}");
            }
            return sb.ToString();
        }
    }
}
```

```cs
var caneta = new Item("C-1", "Caneta", 3.5m);
var caderno = new Item("C-2", "Caderno", 12m);
var kit = new Kit("K-1", "Escolar");
kit.Adicionar(caneta);
kit.Adicionar(caderno);

var csv = (IExtensaoCsv)kit.ObterExtensao("CSV");
Console.WriteLine(csv.Csv);
```

```saida
Kit,K-1,Escolar,{Item,C-1,Caneta,3.5},{Item,C-2,Caderno,12}
```

O código de CSV **não está** em `Item` nem em `Kit`; cada formato novo (`"XML"`, `"JSON"`) é um **conjunto novo de classes** que se pluga na hierarquia. E como as extensões ficam num dicionário, podem ser **inseridas ou removidas em tempo de execução**, de acordo com o estado do sistema.

::: cuidado Flexibilidade demais
É fácil se empolgar. Observe que os construtores de `Item` e `Kit` ainda **conhecem** as classes CSV — uma dependência tênue. Para eliminá-la, uma **Factory** poderia criar os elementos já carregando suas extensões. Mas na maioria das vezes isso nem é necessário: use o padrão com moderação.
:::

::: dica Extension Object no mundo real
Se você já usou `GetService<T>()` do `IServiceProvider`, `Component.GetComponent<T>()` do Unity ou `IExtensible` do WCF, usou a ideia: *"pergunte ao objeto se ele tem uma capacidade e use-a"*.
:::

## Qual escolher?

| Preciso de… | Use |
|---|---|
| **Operações novas** sobre uma hierarquia estável, com todo o código da operação junto | **Visitor** |
| Operações novas, mas a hierarquia ganha tipos com frequência | **Acyclic Visitor** |
| **Comportamento extra** (log, cache, retentativa, compressão) numa operação **existente**, combinável em tempo de execução | **Decorator** |
| **Capacidades** completas e separadas **por tipo**, plugáveis/destacáveis em tempo de execução | **Extension Object** |
| Uma única função com um caso por tipo, em um lugar só | `switch` com *pattern matching* |

::: exercicio Qual padrão?
Para cada situação, escolha o padrão mais adequado e justifique:

1. Uma árvore de formas geométricas (`Circulo`, `Retangulo`, `Triangulo`) estável há anos; o time precisa de relatórios novos todo mês (área total, perímetro, exportar SVG).
2. Um cliente HTTP que precisa, em alguns ambientes, de autenticação, cache e logging — em combinações diferentes.
3. Um modelo de documentos onde cada tipo (`Parágrafo`, `Tabela`, `Imagem`) deve poder ser exportado para PDF, DOCX e HTML, com cada exportador de cada tipo na sua própria classe, e exportadores novos instalados como *plugins*.

--- resposta
1. **Visitor**: hierarquia estável, operações novas com frequência. Cada relatório é um visitante.
2. **Decorator**: cada preocupação (`AutenticacaoHandler`, `CacheHandler`, `LogHandler`) embrulha o cliente; a composição é decidida na montagem.
3. **Extension Object**: cada tipo guarda extensões `"PDF"`, `"DOCX"`, `"HTML"`; plugins adicionam novas extensões sem alterar os tipos.
:::

## Conclusão

A família Visitor oferece várias maneiras de **modificar o comportamento de uma hierarquia sem alterá-la**. Ela nos ajuda a manter o **OCP** e a **segregar** tipos de funcionalidade (CCP/SRP); o **LSP** e o **DIP** também se aplicam à estrutura. Mas esses padrões são **sedutores**: use-os quando ajudarem, mantenha ceticismo saudável — muitas vezes o que um Visitor resolve também se resolve com algo **mais simples** (um método virtual, um `switch`, um delegate).

::: resumo
- Problema: **acrescentar operações** a uma hierarquia sem reabrir suas classes (OCP, CCP, SRP).
- **Visitor:** `Aceitar(v)` na hierarquia + `Visitar(Tipo)` por tipo no visitante → **double dispatch**; ótimo p/ relatórios sobre estruturas; **ciclo** de dependência (novos tipos são caros).
- **Acyclic Visitor:** interface base vazia + uma interface por tipo (`IVisitante<T>`); quebra o ciclo; matriz **esparsa**; custo do teste de tipo e risco de esquecer tipos em silêncio.
- **Decorator:** mesma interface, embrulha e acrescenta comportamento; combinável em tempo de execução (Streams, middlewares, handlers).
- **Extension Object:** cada objeto guarda extensões pesquisáveis; separa o código **por tipo** e permite plugar/desplugar capacidades.
- Em C# moderno, `switch` com *pattern matching* resolve casos simples; Visitor continua útil para visitantes independentes e com estado.
:::

::: quiz
? O que é o "double dispatch" do Visitor?
+ Dois despachos polimórficos: `Aceitar` pelo tipo do elemento e `Visitar` pelo tipo do visitante
- Chamar o mesmo método duas vezes
- Usar duas threads
- Herdar de duas classes
> O primeiro escolhe a implementação de Aceitar; o segundo escolhe a sobrecarga Visitar do tipo concreto.

? Quando o Visitor é a melhor escolha?
+ Hierarquia estável e operações novas frequentes
- Hierarquia que ganha tipos novos o tempo todo
- Quando não há hierarquia
- Quando se quer evitar interfaces
> Adicionar operações = criar visitantes; adicionar tipos = alterar todos os visitantes.

? Qual é o ponto fraco do Visitor clássico?
+ Cria um ciclo de dependência: novos tipos obrigam a alterar o visitante e todos os seus derivados
- É lento (muitos despachos)
- Só funciona com herança múltipla
- Impede relatórios
> Sempre são só dois despachos, mas o ciclo dificulta compilação incremental.

? Como o Acyclic Visitor quebra o ciclo?
+ A base do visitante é vazia e há uma interface de visita por tipo, testada com is/as
- Removendo o método Aceitar
- Usando variáveis globais
- Tornando todos os métodos estáticos
> A hierarquia visitada deixa de depender de todas as derivadas por meio de uma base única.

? O que o Decorator acrescenta, em comparação com o Visitor?
+ Comportamento adicional a operações existentes, combinável em tempo de execução
- Operações novas com despacho duplo
- Um ciclo de dependências
- Mais tipos concretos
> Embrulha um objeto da mesma interface e delega.

? Por que criar uma classe base de decorador (como NotificadorDecorador)?
+ Para centralizar o código de delegação e evitar duplicá-lo em cada decorador
- Para tornar o código mais lento
- Para impedir combinações
- Para permitir herança de várias classes
> Os decoradores concretos só sobrescrevem o que precisam.

? Qual a principal característica do Extension Object?
+ Cada objeto guarda extensões pesquisáveis que encapsulam capacidades específicas daquele tipo
- Um único visitante com um método por tipo
- Um proxy remoto
- Um singleton global
> Permite separar o código de cada capacidade por tipo e plugar/desplugar em runtime.
:::

::: cartas
Visitor :: Padrão que adiciona operações a uma hierarquia por meio de Aceitar/Visitar (double dispatch).
Double dispatch :: Escolha da função por dois tipos em tempo de execução (elemento e visitante).
Acyclic Visitor :: Visitor com base vazia e interfaces por tipo, sem ciclo de dependência.
Decorator :: Embrulha um objeto de mesma interface para acrescentar comportamento.
Extension Object :: Objetos de extensão pesquisáveis que acrescentam capacidades a cada elemento.
Interface marcadora :: Interface sem métodos usada como tipo-base (ex.: IVisitante, IExtensao).
Matriz de funções :: Eixo de tipos × eixo de operações preenchido por métodos Visitar.
Pattern matching (switch) :: Alternativa leve em C# para "um caso por tipo" num só lugar.
:::
