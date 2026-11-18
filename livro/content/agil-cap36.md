---
id: agil-cap36
order: 53
kind: capitulo
num: 36
group: C# · Parte 11 · Pacotes, padrões e arquitetura
title: Abstract Server, Adapter e Bridge
subtitle: Três respostas à pergunta "como ligo um cliente a algo que não nasceu para ele?" — e por que a solução mais simples que resolve o problema costuma ser a melhor.
minutes: 50
hue: 165
objectives:
- Aplicar **Abstract Server** (interface pertencente ao cliente) para satisfazer **DIP** e **OCP**
- Usar **Adapter** (de objeto e de classe) para integrar código que você não pode alterar
- Reconhecer como **adaptadores evitam violações do LSP** e isolam "gambiarras"
- Aplicar **Bridge** quando uma hierarquia tem **mais de um grau de liberdade**
origem: livros/02-projeto-agil-csharp/content/52-cap36.md
---

::: ilustracao img/agil/cap36.svg | Adapter liga formatos incompatíveis; Bridge separa a abstração da implementação.
:::

## Um problema simples que rendeu meses de discussão

Nos anos 1990, num fórum de discussão sobre orientação a objetos, os participantes decidiram usar um **problema minúsculo** para comparar suas filosofias de projeto: o software de uma **luminária de mesa**, com um **interruptor** e uma **lâmpada**. O interruptor responde se está ligado ou desligado; a lâmpada acende e apaga. A discussão durou **meses**: alguns queriam só dois objetos; outros um objeto "Luminária" que contivesse ambos; outros a "eletricidade" como objeto; alguém sugeriu o "cabo elétrico".

Apesar do absurdo, é um ótimo laboratório de projeto. Vamos usar o mesmo espírito, com nomes nossos: um `Interruptor` que controla uma `Lampada`.

```cs
public class Lampada
{
    public void Acender() { Console.WriteLine("💡 acesa"); }
    public void Apagar()  { Console.WriteLine("⚫ apagada"); }
}

public class Interruptor
{
    private readonly Lampada lampada;                    // classe concreta!
    public Interruptor(Lampada lampada) { this.lampada = lampada; }

    public void Pressionar(bool ligado)
    {
        if (ligado) lampada.Acender(); else lampada.Apagar();
    }
}
```

Funciona. Mas viola dois princípios:

- **DIP:** `Interruptor` (política) depende de `Lampada` (classe concreta).
- **OCP** (menos óbvio, mais relevante): **onde levarmos um `Interruptor`, teremos que levar uma `Lampada`**; ele **não** pode controlar facilmente algo que **não** seja lâmpada (um ventilador, uma porta).

Se herdarmos `InterruptorDeVentilador : Interruptor` para controlar um `Ventilador`, **não resolve**: a subclasse **herda a dependência de `Lampada`**.

## Abstract Server (Servidor Abstrato)

A solução é um dos padrões mais simples: introduzir uma **interface entre o cliente e o servidor**.

```cs
public interface ILigavel             // nome do CLIENTE, não do servidor
{
    void Ligar();
    void Desligar();
}

public class Interruptor
{
    private readonly ILigavel alvo;
    public Interruptor(ILigavel alvo) { this.alvo = alvo; }
    public void Pressionar(bool ligado) { if (ligado) alvo.Ligar(); else alvo.Desligar(); }
}

public class Lampada : ILigavel
{
    public void Ligar() { Console.WriteLine("💡 acesa"); }
    public void Desligar() { Console.WriteLine("⚫ apagada"); }
}
public class Ventilador : ILigavel { public void Ligar() { /*...*/ } public void Desligar() { /*...*/ } }
```

Agora o `Interruptor` controla **qualquer coisa** que implemente `ILigavel`: **DIP e OCP satisfeitos de uma vez**.

::: figura Abstract Server
<svg viewBox="0 0 700 190" role="img" aria-label="Interruptor depende de ILigavel; Lampada e Ventilador implementam">
  <rect class="f-prim" x="10" y="60" width="150" height="56" rx="10"/><text x="85" y="94" text-anchor="middle" font-weight="800">Interruptor</text>
  <path class="arrow" d="M162 88H260"/>
  <rect class="f-info" x="262" y="62" width="170" height="52" rx="10"/><text x="347" y="82" text-anchor="middle" font-size="10" class="t-dim">«interface»</text><text x="347" y="102" text-anchor="middle" font-weight="800" class="t-info">ILigavel</text>
  <path class="line" d="M347 116V140M270 140H470M270 140V156M470 140V156" style="marker-end:none"/>
  <rect class="f-good" x="200" y="156" width="140" height="32" rx="8"/><text x="270" y="177" text-anchor="middle">Lampada</text>
  <rect class="f-good" x="400" y="156" width="140" height="32" rx="8"/><text x="470" y="177" text-anchor="middle">Ventilador</text>
</svg>
:::

::: regra A interface pertence ao cliente
Repare que a interface se chama **`ILigavel`** (o que o **cliente** precisa), não `ILampada`. **Interfaces pertencem aos clientes**, não às classes derivadas. O vínculo **lógico** entre `Interruptor` e `ILigavel` é **mais forte** que o vínculo entre `ILigavel` e `Lampada`: não faz sentido entregar `Interruptor` sem `ILigavel`, mas é perfeitamente lógico entregar `ILigavel` **sem** `Lampada`.
:::

### O desencontro entre vínculo lógico e físico

Nos anos 1990 acreditava-se que o vínculo **físico** (herança) mandava: livros respeitáveis recomendavam colocar **hierarquias de herança no mesmo pacote**. A experiência mostrou que isso **engana**: as hierarquias normalmente **não** devem ser empacotadas juntas; em vez disso, **os clientes tendem a ser empacotados com as interfaces que controlam** (`Interruptor` + `ILigavel` no mesmo assembly; `Lampada` em outro, referenciando-o). Esse desalinhamento é um **artefato das linguagens estaticamente tipadas** como C#: linguagens dinâmicas não usam herança para obter polimorfismo e não têm o problema.

## Adapter (Adaptador)

O Abstract Server tem uma limitação: ele **acopla** `Lampada` a `ILigavel` (a lâmpada **implementa** a interface). Isso pode violar o **SRP** — duas coisas que talvez **não mudem pelos mesmos motivos**. E se:

- você **não pode** alterar `Lampada` (veio de um **fornecedor**, sem código-fonte)?
- quer que o `Interruptor` controle uma classe que **não pode** implementar `ILigavel`?
- a classe tem **métodos com nomes diferentes** (`Acionar`/`Parar` em vez de `Ligar`/`Desligar`)?

Entra o **Adapter**: um objeto que **implementa a interface do cliente** e **delega** à classe adaptada.

```cs
// classe de terceiros, que NÃO podemos alterar:
public class LampadaInteligenteDeFabricante
{
    public void Brilhar(int intensidade) { /* ... */ }
    public void Escurecer() { /* ... */ }
}

public class AdaptadorDeLampada : ILigavel
{
    private readonly LampadaInteligenteDeFabricante lampada;
    public AdaptadorDeLampada(LampadaInteligenteDeFabricante lampada) { this.lampada = lampada; }

    public void Ligar()    { lampada.Brilhar(100); }       // adapta a assinatura e a semântica
    public void Desligar() { lampada.Escurecer(); }
}

var interruptor = new Interruptor(new AdaptadorDeLampada(new LampadaInteligenteDeFabricante()));
```

::: figura Adapter
<svg viewBox="0 0 740 180" role="img" aria-label="Interruptor usa ILigavel; AdaptadorDeLampada implementa e delega à Lampada de terceiros">
  <rect class="f-prim" x="10" y="60" width="140" height="54" rx="10"/><text x="80" y="92" text-anchor="middle" font-weight="800">Interruptor</text>
  <path class="arrow" d="M152 87H220"/>
  <rect class="f-info" x="222" y="62" width="140" height="50" rx="10"/><text x="292" y="82" text-anchor="middle" font-size="10" class="t-dim">«interface»</text><text x="292" y="100" text-anchor="middle" font-weight="800" class="t-info">ILigavel</text>
  <path class="line" d="M292 114V140" style="marker-end:none"/>
  <rect class="f-warn" x="222" y="140" width="140" height="34" rx="8"/><text x="292" y="162" text-anchor="middle" font-weight="700">AdaptadorDeLampada</text>
  <path class="arrow" d="M364 157H470" style="stroke:var(--warn);marker-end:url(#arr)"/><text x="417" y="148" text-anchor="middle" font-size="11" class="t-dim">delega</text>
  <rect class="f-plain" x="472" y="136" width="250" height="42" rx="8" stroke-dasharray="5 4"/><text x="597" y="162" text-anchor="middle">LampadaInteligenteDeFabricante (terceiros)</text>
</svg>
:::

Resolve com elegância: **qualquer** coisa que possa ser ligada/desligada pode ser controlada, basta criar o adaptador. O objeto adaptado **nem precisa** ter métodos com os mesmos nomes.

### O custo dos adaptadores

Adaptadores **custam**: escrever a classe, instanciar, ligar ao adaptado, e pagar **tempo e espaço de delegação** a cada chamada. Portanto, **não use o tempo todo**. O **Abstract Server** serve bem a quase todos os casos — e até a solução inicial (cliente → concreta) **é ótima**, a não ser que você saiba que existirão outros objetos para controlar. Padrões são **remédios**: só tome quando houver a doença.

### Adapter de objeto × Adapter de classe

- **De objeto** (o que fizemos): o adaptador **contém** o adaptado e **delega** — composição.
- **De classe:** o adaptador **herda** da interface **e** da classe adaptada (no C#, herda da classe e implementa a interface). Um pouco mais eficiente e fácil, **ao custo do forte acoplamento da herança**:

```cs
public class AdaptadorDeClasse : LampadaInteligenteDeFabricante, ILigavel
{
    public void Ligar()    { Brilhar(100); }
    public void Desligar() { Escurecer(); }
}
```

Prefira, em geral, o **de objeto**.

## O problema do modem dedicado: Adapter, LSP e dependências em cadeia

Uma situação muito comum: centenas de clientes usam uma interface de comunicação por meio de várias implementações — tudo respeita **OCP**, **LSP** e **DIP**. Vamos nos apoiar num exemplo nosso, equivalente: **enviar pagamentos** por vários **provedores**.

```cs
public interface IProvedorDePagamento
{
    string Autorizar(decimal valor, string cartao);    // devolve um código de autorização
    void Capturar(string autorizacao);
    void Estornar(string autorizacao);
}
```

Centenas de módulos usam `IProvedorDePagamento`; há implementações `ProvedorA`, `ProvedorB`, `ProvedorC`. Ótimo, **por anos**.

Chega um novo requisito: uma **carteira digital** em que o pagamento é **instantâneo**: **não há autorização separada de captura** (nem estorno, só devolução). E os usuários **proíbem alterar** os módulos clientes.

O ideal seria usar o **ISP**, separando "autorizar/capturar" de "pagar instantâneo" — mas isso exigiria **alterar todos os clientes**. O que fazer?

### Gambiarra por herança

Derivar `CarteiraDigital` de `IProvedorDePagamento` e implementar `Autorizar`/`Capturar`/`Estornar` de forma **degenerada** (vazia, ou lançando `NotSupportedException`). **Sinal de violação do LSP**: os clientes esperam que `Autorizar` mude o estado (reserve o dinheiro) e que `Capturar` o efetive; uma implementação degenerada **frustra** essa expectativa — por exemplo, pode **debitar duas vezes**.

Você pode "consertar" fazendo a carteira **simular** um estado de autorização e exigindo que quem usa a carteira chame `Autorizar`/`Capturar` "só para dar certo". Funciona… por ora. Meses depois há centenas de aplicativos da carteira **chamando métodos que não precisam**, só porque mandamos. E aí chega **uma nova mudança** (autorizações com **parcelas e códigos longos**): todos os clientes do contrato antigo mudam (inevitável), e **os aplicativos da carteira também precisam mudar**, mesmo sem querer — porque **fizeram o que nós pedimos**. Uma **correção temporária** gerou uma **teia de dependências** que vaza para uma parte "não relacionada" do sistema.

### Adapter como salvação

Em vez de herdar, **adapte**: a carteira **não** implementa `IProvedorDePagamento`; os clientes antigos a usam **por meio de um adaptador** que **simula** o estado de autorização por dentro e **delega** o essencial; os aplicativos da carteira usam a carteira **diretamente**, sem chamar `Autorizar`/`Capturar`.

```cs
public interface ICarteiraDigital { string PagarAgora(decimal valor, string token); void Devolver(string recibo); }

public class AdaptadorDeCarteira : IProvedorDePagamento
{
    private readonly ICarteiraDigital carteira;
    private readonly Dictionary<string, (decimal Valor, string Token)> pendentes = new Dictionary<string, (decimal, string)>();

    public AdaptadorDeCarteira(ICarteiraDigital carteira) { this.carteira = carteira; }

    public string Autorizar(decimal valor, string cartao)
    {
        string codigo = Guid.NewGuid().ToString("N");
        pendentes[codigo] = (valor, cartao);          // SIMULA a autorização (nada é debitado ainda)
        return codigo;
    }

    public void Capturar(string autorizacao)
    {
        var (valor, token) = pendentes[autorizacao];
        carteira.PagarAgora(valor, token);            // agora sim: pagamento instantâneo
        pendentes.Remove(autorizacao);
    }

    public void Estornar(string autorizacao) { pendentes.Remove(autorizacao); }   // só descarta se ainda não capturou
}
```

Resultado: os **clientes do provedor** veem o comportamento que esperam; os **aplicativos da carteira** não mexem em `Autorizar`/`Capturar`; quando o requisito dos códigos longos chegar, **os aplicativos da carteira não são afetados**. O adaptador **corrigiu as violações de LSP e OCP**.

::: figura O adaptador isolando a gambiarra
<svg viewBox="0 0 760 230" role="img" aria-label="Clientes antigos usam IProvedor; o adaptador simula estado e delega à carteira; apps da carteira usam a carteira direto">
  <rect class="f-plain" x="10" y="20" width="150" height="40" rx="6"/><text x="85" y="45" text-anchor="middle">Clientes antigos</text>
  <path class="arrow" d="M162 40H250"/>
  <rect class="f-info" x="252" y="20" width="170" height="40" rx="8"/><text x="337" y="45" text-anchor="middle" font-weight="700">IProvedorDePagamento</text>
  <path class="line" d="M337 62V100M150 100H520M150 100V120M337 100V120M520 100V120" style="marker-end:none"/>
  <rect class="f-plain" x="80" y="120" width="140" height="36" rx="8"/><text x="150" y="143" text-anchor="middle" font-size="12">ProvedorA</text>
  <rect class="f-plain" x="267" y="120" width="140" height="36" rx="8"/><text x="337" y="143" text-anchor="middle" font-size="12">ProvedorB</text>
  <rect class="f-warn" x="440" y="120" width="160" height="36" rx="8"/><text x="520" y="143" text-anchor="middle" font-size="12" font-weight="700">AdaptadorDeCarteira</text>
  <path class="arrow" d="M602 138H640" style="stroke:var(--warn)"/>
  <rect class="f-good" x="642" y="120" width="108" height="36" rx="8"/><text x="696" y="143" text-anchor="middle" font-size="12">Carteira</text>
  <rect class="f-plain" x="600" y="190" width="150" height="34" rx="6"/><text x="675" y="212" text-anchor="middle" font-size="12">Apps da carteira</text>
  <path class="arrow" d="M690 190V158"/>
  <text x="250" y="195" font-size="12" class="t-dim">A gambiarra existe — mas fica dentro do adaptador</text>
</svg>
:::

Note que **a gambiarra ainda existe** (o adaptador simula estado). Pode parecer horrível — e é. A diferença: **todas as dependências apontam para fora do adaptador**. A correção provisória fica **isolada**, escondida num adaptador que **quase ninguém conhece**; a única dependência incondicional nele será, provavelmente, **uma fábrica** em algum lugar (capítulo 34).

## Bridge (Ponte)

Há outro modo de olhar o problema. A chegada da carteira acrescentou **um novo grau de liberdade** à hierarquia: antes só variava **o provedor** (A, B, C); agora varia também **o modo de pagamento** (com autorização × instantâneo). Se fundirmos as duas variações na herança, teremos **explosão de classes**:

::: figura Duas dimensões fundidas → muitas classes
<svg viewBox="0 0 700 200" role="img" aria-label="Hierarquia com combinações ProvedorAAutorizacao, ProvedorAInstantaneo etc.">
  <rect class="f-prim" x="280" y="8" width="140" height="34" rx="6"/><text x="350" y="30" text-anchor="middle" font-weight="700">Pagamento</text>
  <path class="line" d="M350 44V64M180 64H520M180 64V80M520 64V80" style="marker-end:none"/>
  <rect class="f-info" x="100" y="80" width="160" height="34" rx="6"/><text x="180" y="102" text-anchor="middle">PagtoComAutorizacao</text>
  <rect class="f-info" x="440" y="80" width="160" height="34" rx="6"/><text x="520" y="102" text-anchor="middle">PagtoInstantaneo</text>
  <g text-anchor="middle" font-size="11"><rect class="f-plain" x="20" y="150" width="110" height="30" rx="5"/><text x="75" y="169">…ProvedorA</text><rect class="f-plain" x="135" y="150" width="110" height="30" rx="5"/><text x="190" y="169">…ProvedorB</text><rect class="f-plain" x="250" y="150" width="110" height="30" rx="5"/><text x="305" y="169">…ProvedorC</text><rect class="f-plain" x="380" y="150" width="110" height="30" rx="5"/><text x="435" y="169">…ProvedorA</text><rect class="f-plain" x="495" y="150" width="110" height="30" rx="5"/><text x="550" y="169">…ProvedorB</text><rect class="f-plain" x="610" y="150" width="80" height="30" rx="5"/><text x="650" y="169">…C</text></g>
  <path class="line" d="M180 116V150M520 116V150" style="marker-end:none"/>
  <text x="350" y="196" text-anchor="middle" font-size="12" class="t-bad">novo provedor = 2 classes; novo modo = 3 classes…</text>
</svg>
:::

O **Bridge** ajuda quando uma hierarquia tem **mais de um grau de liberdade**: em vez de mesclar as dimensões por herança, **separe-as em duas hierarquias** e **ligue-as por uma ponte** (composição).

```cs
// Hierarquia 1: "o que fazer" — a abstração (modo de pagamento)
public abstract class Pagamento : IProvedorDePagamento
{
    protected readonly IGateway gateway;                  // a PONTE para a outra hierarquia
    protected Pagamento(IGateway gateway) { this.gateway = gateway; }
    public abstract string Autorizar(decimal valor, string cartao);
    public abstract void Capturar(string autorizacao);
    public abstract void Estornar(string autorizacao);
}

public class PagamentoComAutorizacao : Pagamento
{
    public PagamentoComAutorizacao(IGateway g) : base(g) { }
    public override string Autorizar(decimal v, string c) { return gateway.Reservar(v, c); }
    public override void Capturar(string a) { gateway.Efetivar(a); }
    public override void Estornar(string a) { gateway.Cancelar(a); }
}

public class PagamentoInstantaneo : Pagamento
{
    private readonly Dictionary<string, (decimal, string)> pendentes = new Dictionary<string, (decimal, string)>();
    public PagamentoInstantaneo(IGateway g) : base(g) { }
    public override string Autorizar(decimal v, string c) { var id = Guid.NewGuid().ToString("N"); pendentes[id] = (v, c); return id; }
    public override void Capturar(string a) { var (v, c) = pendentes[a]; gateway.CobrarNaHora(v, c); pendentes.Remove(a); }
    public override void Estornar(string a) { pendentes.Remove(a); }
}

// Hierarquia 2: "como conversar com o fornecedor" — a implementação
public interface IGateway
{
    string Reservar(decimal v, string c);  void Efetivar(string a);  void Cancelar(string a);  void CobrarNaHora(decimal v, string c);
}
public class GatewayA : IGateway { /* ... */ public string Reservar(decimal v, string c){return "";} public void Efetivar(string a){} public void Cancelar(string a){} public void CobrarNaHora(decimal v,string c){} }
public class GatewayB : IGateway { /* ... */ public string Reservar(decimal v, string c){return "";} public void Efetivar(string a){} public void Cancelar(string a){} public void CobrarNaHora(decimal v,string c){} }

// combinações em tempo de execução:
IProvedorDePagamento p1 = new PagamentoComAutorizacao(new GatewayA());
IProvedorDePagamento p2 = new PagamentoInstantaneo(new GatewayB());
```

::: figura Bridge: duas hierarquias ligadas por composição
<svg viewBox="0 0 740 230" role="img" aria-label="Pagamento (abstração) tem um IGateway (implementação); cada lado tem suas subclasses">
  <rect class="f-prim" x="40" y="20" width="200" height="56" rx="10"/><text x="140" y="44" text-anchor="middle" font-weight="800" class="t-prim">Pagamento {abstract}</text><text x="140" y="64" text-anchor="middle" font-size="11" class="t-mono"># gateway : IGateway</text>
  <path class="arrow" d="M242 48H480"/><text x="360" y="40" text-anchor="middle" font-size="12" class="t-dim">ponte (composição)</text>
  <rect class="f-info" x="482" y="20" width="220" height="56" rx="10"/><text x="592" y="42" text-anchor="middle" font-size="10" class="t-dim">«interface»</text><text x="592" y="62" text-anchor="middle" font-weight="800" class="t-info">IGateway</text>
  <path class="line" d="M140 78V104M70 104H210M70 104V124M210 104V124" style="marker-end:none"/>
  <rect class="f-plain" x="10" y="124" width="120" height="44" rx="6"/><text x="70" y="150" text-anchor="middle" font-size="12">ComAutorizacao</text>
  <rect class="f-plain" x="150" y="124" width="120" height="44" rx="6"/><text x="210" y="150" text-anchor="middle" font-size="12">Instantaneo</text>
  <path class="line" d="M592 78V104M522 104H662M522 104V124M662 104V124" style="marker-end:none"/>
  <rect class="f-plain" x="462" y="124" width="120" height="44" rx="6"/><text x="522" y="150" text-anchor="middle" font-size="12">GatewayA</text>
  <rect class="f-plain" x="602" y="124" width="120" height="44" rx="6"/><text x="662" y="150" text-anchor="middle" font-size="12">GatewayB</text>
  <text x="370" y="205" text-anchor="middle" font-size="13" class="t-good">2 + 2 classes em vez de 2 × 2 (e cresce somando, não multiplicando)</text>
</svg>
:::

Os clientes continuam usando **a mesma interface**; os dois "graus de liberdade" variam **independentemente**: nova política de pagamento = **uma** classe na hierarquia 1; novo fornecedor = **uma** classe na 2. A estrutura é complexa — mas separa **completamente** as diretivas de uso da **implementação** do fornecedor, permite criar **novas funções de implementação** sem afetar usuários e abre **um caminho de migração** (via ISP) para uma API de mais alto nível.

## Adapter × Bridge × Abstract Server

| | **Abstract Server** | **Adapter** | **Bridge** |
|---|---|---|---|
| Problema | Cliente preso a um concreto | Integrar uma classe que **não pode** (ou não deve) ser alterada | Hierarquia com **duas dimensões** variando |
| Como | Interface do cliente + servidores a implementam | Objeto que **implementa a interface** e **delega** | **Duas hierarquias** ligadas por composição |
| Complexidade | Baixa | Média | Alta |
| Quando | **Quase sempre** que precisar de substituição | Integração, código legado, gambiarras a isolar | Só com **evidência forte** de duas dimensões voláteis |

## Conclusão: não existe estrutura perfeita

Poderiam dizer: *"o problema é que os projetistas originais entenderam mal o domínio; deveriam ter separado autorização e captura desde o início"*. **Bobagem.** **Não existe análise suficiente.** Por mais tempo que se gaste procurando a estrutura perfeita, o usuário sempre trará uma mudança que a viole. **Não há estruturas perfeitas — só estruturas que equilibram custos e benefícios atuais**, e que **precisam mudar** com os requisitos. O truque é **manter o sistema o mais simples e flexível possível**.

::: dica Qual escolher?
O **Adapter** é simples e direto: mantém as dependências apontando na direção certa. O **Bridge** é bem mais complexo: só vale quando há evidência forte de que as duas dimensões **realmente** variarão. **Cada padrão tem vantagens e desvantagens: use o mais adequado ao problema que você tem hoje.**
:::

::: exercicio Qual padrão?
1. Seu sistema usa `IRelogio` em 200 lugares; precisa usar um relógio atômico de um fornecedor, com métodos `ObterHoraUtc()` e `Sincronizar()`, cujo código você não pode mudar.
2. Você tem `Relatorio` com formatos (PDF, Excel) **e** destinos (e-mail, pasta, nuvem) combináveis livremente, e novos formatos e destinos surgem sempre.
3. Um `Botao` precisa controlar qualquer coisa que se possa acionar, e você controla todas as classes.

--- resposta
1. **Adapter**: `AdaptadorDeRelogioAtomico : IRelogio` delega ao fornecedor.
2. **Bridge**: hierarquia de **formatos** (abstração) com uma referência à hierarquia de **destinos** (implementação); `ExportadorPdf(new DestinoEmail())`.
3. **Abstract Server**: interface `IAcionavel` pertencente ao `Botao`, implementada pelas classes que você controla.
:::

::: resumo
- **Abstract Server:** o cliente depende de uma **interface que lhe pertence**; os servidores a implementam (DIP + OCP). O vínculo lógico cliente↔interface é mais forte que interface↔servidor.
- **Adapter:** implementa a interface do cliente e **delega** à classe adaptada (código de terceiros/legado). **De objeto** (composição) é preferível ao **de classe** (herança).
- Adaptadores **isolam gambiarras** e evitam violações do LSP/OCP; custam classes e delegação — **não os use sempre**.
- **Bridge:** separa **duas hierarquias** (abstração e implementação) ligadas por composição, quando há **mais de um grau de liberdade**; evita explosão de subclasses; é **complexo** — use com forte evidência.
- **Não existem estruturas perfeitas**: mantenha o sistema simples e flexível e aceite que ele mudará.
:::

::: quiz
? Qual problema o Abstract Server resolve?
+ O cliente depender de uma classe concreta, violando DIP e OCP
- A criação de objetos
- A destruição de objetos
- A serialização
> O cliente passa a depender de uma interface que lhe pertence.

? Por que a interface se chama `ILigavel` e não `ILampada`?
+ Porque interfaces pertencem aos clientes e devem ter o nome do que o cliente precisa
- Porque lâmpadas não existem
- Porque o C# proíbe o nome
- Porque é mais curto
> O vínculo lógico é com o cliente.

? Quando usar um Adapter?
+ Quando é preciso usar uma classe que não se pode (ou não se deve) alterar através da interface esperada
- Quando se quer criar objetos sem new
- Quando só há uma instância
- Quando se quer observar eventos
> O adaptador implementa a interface e delega.

? Qual a diferença entre Adapter de objeto e de classe?
+ O de objeto usa composição/delegação; o de classe usa herança (mais acoplado)
- O de classe é mais flexível
- O de objeto não pode delegar
- Não há diferença
> Prefira, em geral, o de objeto.

? Por que o adaptador isolou a "gambiarra" sem espalhar dependências?
+ Porque todas as dependências apontam para fora do adaptador; ninguém depende dos detalhes da simulação
- Porque removeu a gambiarra
- Porque usa herança múltipla
- Porque é estático
> A correção provisória fica escondida.

? Quando o Bridge é indicado?
+ Quando uma hierarquia tem mais de um grau de liberdade e as combinações gerariam explosão de classes
- Quando há um único tipo
- Quando se quer garantir uma única instância
- Quando se quer desfazer comandos
> Duas hierarquias independentes ligadas por composição.

? Qual a principal conclusão sobre estruturas de software no capítulo?
+ Não existem estruturas perfeitas; mantenha o sistema simples e flexível e aceite as mudanças
- Basta analisar mais para acertar
- Padrões devem ser sempre usados
- Evite qualquer abstração
> Estruturas equilibram custos e benefícios atuais e mudam com o tempo.
:::

::: cartas
Abstract Server :: Interface pertencente ao cliente, implementada pelos servidores.
Adapter :: Objeto que implementa a interface esperada e delega a uma classe incompatível.
Adaptador de objeto × de classe :: Composição/delegação × herança.
Bridge :: Separa abstração e implementação em hierarquias independentes ligadas por composição.
Grau de liberdade :: Dimensão independente de variação numa hierarquia.
Implementação degenerada :: Método vazio/que lança exceção só para cumprir a interface; risco de LSP.
Vínculo lógico × físico :: Relação conceitual × relação por herança/empacotamento.
:::
