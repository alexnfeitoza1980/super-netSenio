---
id: agil-cap39
order: 56
kind: capitulo
num: 39
group: C# · Parte 11 · Pacotes, padrões e arquitetura
title: State e máquinas de estados
subtitle: Três jeitos de implementar uma máquina de estados finitos — switch aninhado, tabela de transição e o padrão State — e onde essas máquinas valem ouro.
minutes: 60
hue: 185
objectives:
- Revisar o conceito de **FSM** (estados, eventos, transições, ações) e traduzi-lo em código
- Comparar **switch aninhado**, **tabela de transição** e o **padrão State** (custos e benefícios)
- Distinguir **State** de **Strategy**
- Reconhecer onde FSMs brilham: **políticas de UI**, **interações de GUI**, **protocolos**
origem: livros/02-projeto-agil-csharp/content/55-cap39.md
---

::: ilustracao img/agil/cap39.svg | State: o comportamento depende do estado; a catraca bloqueada vira livre ao receber a moeda.
:::

## Uma lembrança: máquinas de estados finitos

Uma **máquina de estados finitos (FSM)** descreve um sistema que, a cada instante, está em **um** de **poucos estados**. **Eventos** provocam **transições**; cada transição pode disparar uma **ação**. É a abstração perfeita para tudo que "se comporta de um jeito *dependendo do que aconteceu antes*": players, catracas, pedidos, conexões, wizards, telas de login.

Vamos usar um exemplo de bolso: um **tocador de música** com três estados e três eventos.

::: figura FSM do player
<svg viewBox="0 0 720 230" role="img" aria-label="Parado, Tocando e Pausado com transições Play, Pause e Stop">
  <rect class="f-plain" x="20" y="85" width="140" height="56" rx="28"/><text x="90" y="119" text-anchor="middle" font-weight="800">Parado</text>
  <rect class="f-good" x="290" y="20" width="140" height="56" rx="28"/><text x="360" y="54" text-anchor="middle" font-weight="800">Tocando</text>
  <rect class="f-warn" x="290" y="160" width="140" height="56" rx="28"/><text x="360" y="194" text-anchor="middle" font-weight="800">Pausado</text>
  <path class="arrow" d="M150 92Q215 40 288 46"/><text x="185" y="52" text-anchor="middle" font-size="11" class="t-mono">Play / Iniciar</text>
  <path class="arrow" d="M300 78Q250 118 330 160" style="marker-end:url(#arr)"/><text x="262" y="124" text-anchor="middle" font-size="11" class="t-mono">Pause / Pausar</text>
  <path class="arrow" d="M395 160Q430 118 395 78"/><text x="480" y="124" text-anchor="middle" font-size="11" class="t-mono">Play / Retomar</text>
  <path class="arrow" d="M288 200Q200 210 120 142"/><text x="170" y="200" text-anchor="middle" font-size="11" class="t-mono">Stop / Parar</text>
  <path class="arrow" d="M290 36Q80 -10 50 83"/><text x="130" y="14" text-anchor="middle" font-size="11" class="t-mono">Stop / Parar</text>
</svg>
:::

As ações que o player precisa executar ficam atrás de uma interface — assim a **lógica da FSM** não se mistura com **como** tocar áudio:

```cs
public enum EstadoDoPlayer { Parado, Tocando, Pausado }
public enum Evento { Play, Pause, Stop }

public interface IAcoesDoPlayer
{
    void Iniciar();
    void Pausar();
    void Retomar();
    void Parar();
}

public class TransicaoInvalidaException : Exception
{
    public TransicaoInvalidaException(EstadoDoPlayer estado, Evento evento)
        : base("Transição indefinida: " + estado + " com " + evento) { }
}
```

::: dica O teste molda o projeto
Observe o `IAcoesDoPlayer`: foi criado **para testar** (um falso/*spy* registra o que foi chamado). Resultado colateral feliz: a lógica da FSM ficou **desacoplada** das ações — outra FSM pode reaproveitar a interface. A necessidade de testar cada unidade isoladamente nos força a desacoplar de formas que não imaginaríamos. **Testabilidade empurra o projeto para baixo acoplamento.**
:::

## Estratégia 1 — `switch` aninhado

A mais direta: um `switch` no estado, dentro dele um `switch` no evento. Cada "célula" é uma das transições do diagrama.

```cs
public class PlayerComSwitch
{
    private readonly IAcoesDoPlayer acoes;
    public EstadoDoPlayer Estado { get; private set; } = EstadoDoPlayer.Parado;

    public PlayerComSwitch(IAcoesDoPlayer acoes) { this.acoes = acoes; }

    public void Tratar(Evento e)
    {
        switch (Estado)
        {
            case EstadoDoPlayer.Parado:
                switch (e)
                {
                    case Evento.Play: Estado = EstadoDoPlayer.Tocando; acoes.Iniciar(); break;
                    case Evento.Stop: break;
                    default: throw new TransicaoInvalidaException(Estado, e);
                }
                break;

            case EstadoDoPlayer.Tocando:
                switch (e)
                {
                    case Evento.Pause: Estado = EstadoDoPlayer.Pausado; acoes.Pausar(); break;
                    case Evento.Stop:  Estado = EstadoDoPlayer.Parado;  acoes.Parar();  break;
                    case Evento.Play:  break;
                }
                break;

            case EstadoDoPlayer.Pausado:
                switch (e)
                {
                    case Evento.Play:  Estado = EstadoDoPlayer.Tocando; acoes.Retomar(); break;
                    case Evento.Stop:  Estado = EstadoDoPlayer.Parado;  acoes.Parar();   break;
                    case Evento.Pause: throw new TransicaoInvalidaException(Estado, e);
                }
                break;
        }
    }
}
```

**Custos e benefícios**

- ✅ Para FSMs **pequenas** é elegante e rápida: tudo cabe em uma ou duas telas.
- ❌ Em FSMs com **dezenas** de estados/eventos viram páginas e páginas de `case`, sem delimitadores que ajudem a saber *onde você está*: difícil de manter e **propensa a erros**.
- ❌ **Sem separação** garantida entre a lógica da FSM e o código das ações: é comum o corpo das ações acabar enterrado nos `case`s (aqui evitamos com `IAcoesDoPlayer`, mas nada obriga).

## Estratégia 2 — Tabela de transição

Em vez de **código**, descrevemos as transições como **dados**; um pequeno **motor** as interpreta.

```cs
public class PlayerComTabela
{
    private readonly Dictionary<(EstadoDoPlayer, Evento), (EstadoDoPlayer, Action)> tabela
        = new Dictionary<(EstadoDoPlayer, Evento), (EstadoDoPlayer, Action)>();

    public EstadoDoPlayer Estado { get; private set; } = EstadoDoPlayer.Parado;

    public PlayerComTabela(IAcoesDoPlayer a)
    {
        Adicionar(EstadoDoPlayer.Parado,  Evento.Play,  EstadoDoPlayer.Tocando, a.Iniciar);
        Adicionar(EstadoDoPlayer.Parado,  Evento.Stop,  EstadoDoPlayer.Parado,  () => { });
        Adicionar(EstadoDoPlayer.Tocando, Evento.Pause, EstadoDoPlayer.Pausado, a.Pausar);
        Adicionar(EstadoDoPlayer.Tocando, Evento.Stop,  EstadoDoPlayer.Parado,  a.Parar);
        Adicionar(EstadoDoPlayer.Tocando, Evento.Play,  EstadoDoPlayer.Tocando, () => { });
        Adicionar(EstadoDoPlayer.Pausado, Evento.Play,  EstadoDoPlayer.Tocando, a.Retomar);
        Adicionar(EstadoDoPlayer.Pausado, Evento.Stop,  EstadoDoPlayer.Parado,  a.Parar);
    }

    private void Adicionar(EstadoDoPlayer de, Evento ev, EstadoDoPlayer para, Action acao)
    {
        tabela[(de, ev)] = (para, acao);
    }

    public void Tratar(Evento e)
    {
        (EstadoDoPlayer destino, Action acao) transicao;
        if (!tabela.TryGetValue((Estado, e), out transicao))
            throw new TransicaoInvalidaException(Estado, e);

        Estado = transicao.destino;
        transicao.acao();
    }
}
```

**Custos e benefícios**

- ✅ O código que monta a tabela **se lê como a tabela canônica** de estados do diagrama; a lógica está **toda num só lugar**, separada das ações.
- ✅ **Manutenção fácil**: nova transição = **uma linha** `Adicionar(...)`.
- ✅ A tabela é **dado**: pode ser **alterada em tempo de execução**, ter várias versões escolhidas conforme o contexto, ou até ser **carregada de um arquivo** (JSON/XML).
- ❌ Custo principal: **velocidade** (pesquisa na tabela a cada evento). Com `Dictionary` o custo é pequeno; numa lista linear em FSM grande pode pesar.

## Estratégia 3 — O padrão State

Une **muito da eficiência** do `switch` com **muito da flexibilidade** da tabela. Cada **estado vira uma classe**, que sabe responder aos eventos; o **contexto** (o player) guarda uma referência ao estado atual e **delega** os eventos a ele.

::: figura Estrutura do State
<svg viewBox="0 0 720 220" role="img" aria-label="Player delega Play, Pause e Stop a IEstado, implementado por Parado, Tocando e Pausado">
  <rect class="f-prim" x="10" y="60" width="190" height="100" rx="10"/><text x="105" y="84" text-anchor="middle" font-weight="800">Player (contexto)</text><text x="105" y="108" text-anchor="middle" font-size="12" class="t-mono">+Play() +Pause() +Stop()</text><text x="105" y="130" text-anchor="middle" font-size="12" class="t-mono">~Iniciar() ~Pausar()…</text>
  <path class="arrow" d="M202 110H290"/><text x="246" y="102" text-anchor="middle" font-size="10" class="t-dim">estado</text>
  <rect class="f-info" x="292" y="80" width="170" height="60" rx="10"/><text x="377" y="102" text-anchor="middle" font-size="10" class="t-dim">«interface»</text><text x="377" y="124" text-anchor="middle" font-weight="800" class="t-info">IEstado</text>
  <path class="line" d="M377 142V166M200 166H560M200 166V180M377 166V180M560 166V180" style="marker-end:none"/>
  <rect class="f-plain" x="130" y="180" width="140" height="34" rx="8"/><text x="200" y="202" text-anchor="middle">Parado</text>
  <rect class="f-good" x="307" y="180" width="140" height="34" rx="8"/><text x="377" y="202" text-anchor="middle">Tocando</text>
  <rect class="f-warn" x="490" y="180" width="140" height="34" rx="8"/><text x="560" y="202" text-anchor="middle">Pausado</text>
  <path d="M200 178Q150 120 106 162" stroke="var(--ink-3)" stroke-dasharray="4 4" fill="none" marker-end="url(#arr2)"/><text x="4" y="194" font-size="10" class="t-dim">estados chamam</text><text x="4" y="207" font-size="10" class="t-dim">métodos do contexto</text>
</svg>
:::

```cs
public interface IEstado
{
    void Play(Player p);
    void Pause(Player p);
    void Stop(Player p);
}

// Base: qualquer evento não tratado é inválido (como o código gerado por ferramentas de FSM faz)
public abstract class EstadoBase : IEstado
{
    protected abstract EstadoDoPlayer Nome { get; }
    public virtual void Play(Player p)  { throw new TransicaoInvalidaException(Nome, Evento.Play); }
    public virtual void Pause(Player p) { throw new TransicaoInvalidaException(Nome, Evento.Pause); }
    public virtual void Stop(Player p)  { throw new TransicaoInvalidaException(Nome, Evento.Stop); }
}

public class Parado : EstadoBase
{
    protected override EstadoDoPlayer Nome { get { return EstadoDoPlayer.Parado; } }
    public override void Play(Player p) { p.MudarPara(Player.Tocando); p.Iniciar(); }
    public override void Stop(Player p) { /* nada a fazer */ }
}

public class Tocando : EstadoBase
{
    protected override EstadoDoPlayer Nome { get { return EstadoDoPlayer.Tocando; } }
    public override void Play(Player p)  { /* já está tocando */ }
    public override void Pause(Player p) { p.MudarPara(Player.Pausado); p.Pausar(); }
    public override void Stop(Player p)  { p.MudarPara(Player.Parado);  p.Parar(); }
}

public class Pausado : EstadoBase
{
    protected override EstadoDoPlayer Nome { get { return EstadoDoPlayer.Pausado; } }
    public override void Play(Player p) { p.MudarPara(Player.Tocando); p.Retomar(); }
    public override void Stop(Player p) { p.MudarPara(Player.Parado);  p.Parar(); }
}

public class Player
{
    // Os estados não têm campos: uma única instância de cada basta (e é compartilhada)
    internal static readonly IEstado Parado   = new Parado();
    internal static readonly IEstado Tocando  = new Tocando();
    internal static readonly IEstado Pausado  = new Pausado();

    private readonly IAcoesDoPlayer acoes;
    private IEstado estado = Parado;

    public Player(IAcoesDoPlayer acoes) { this.acoes = acoes; }

    // Eventos: apenas delegam ao estado atual
    public void Play()  { estado.Play(this); }
    public void Pause() { estado.Pause(this); }
    public void Stop()  { estado.Stop(this); }

    // Ações e mudança de estado: usadas pelos estados
    internal void MudarPara(IEstado novo) { estado = novo; }
    internal void Iniciar() { acoes.Iniciar(); }
    internal void Pausar()  { acoes.Pausar(); }
    internal void Retomar() { acoes.Retomar(); }
    internal void Parar()   { acoes.Parar(); }
}
```

Uso:

```cs
var player = new Player(new AcoesDeAudio());
player.Play();     // Parado  → Tocando  (Iniciar)
player.Pause();    // Tocando → Pausado  (Pausar)
player.Play();     // Pausado → Tocando  (Retomar)
player.Stop();     // Tocando → Parado   (Parar)
player.Pause();    // lança TransicaoInvalidaException: "Parado com Pause"
```

**Custos e benefícios**

- ✅ **Forte separação** entre ações (no contexto) e lógica (nas classes de estado): é fácil reusar as ações com outra lógica de estados, ou herdar do contexto para trocar as ações.
- ✅ **Eficiente**: tão rápido quanto `switch`; não há pesquisa.
- ❌ Escrever as classes de estado é **maçante** (uma FSM com 20 estados = 20 classes).
- ❌ A lógica fica **distribuída**: não existe um lugar para ver a máquina inteira.

## State × Strategy

As estruturas são quase iguais (contexto + interface polimórfica + várias derivadas). A diferença:

| | **State** | **Strategy** |
|---|---|---|
| Derivadas conhecem o contexto? | **Sim**: recebem o contexto e **chamam métodos dele** (inclusive para trocar o estado) | Não precisam |
| Quem troca a variante? | Os **próprios estados** (e o contexto) conforme os eventos | Em geral **quem monta/configura** o contexto |
| Intenção | Comportamento que **muda com o estado interno** | Algoritmo **intercambiável** |

Toda instância de State é também uma instância de Strategy, mas **nem toda Strategy é State**.

## FSM de três camadas e geradores

Escrever classes de estado à mão cansa e a lógica se espalha. A solução clássica: descrever a máquina **em texto** (estado → evento → novo estado → ação) e **gerar** o código. A estrutura resultante tem **três camadas**:

1. **Ações** (interface/classe abstrata com os métodos de ação);
2. **FSM gerada** (o contexto + as classes de estado, que ninguém precisa ler);
3. **Implementação das ações** (derivada que fala com o mundo real).

Assim se pode criar muitas FSMs diferentes sobre as mesmas ações, e muitas implementações de ações sobre a mesma FSM. Hoje, em .NET, você tem opções prontas: bibliotecas como **Stateless** (FSM fluente), **Appccelerate**, ou **geradores de código** (source generators).

```cs
// Exemplo ilustrativo com a biblioteca Stateless
var m = new StateMachine<EstadoDoPlayer, Evento>(EstadoDoPlayer.Parado);

m.Configure(EstadoDoPlayer.Parado)
    .Permit(Evento.Play, EstadoDoPlayer.Tocando)
    .Ignore(Evento.Stop);

m.Configure(EstadoDoPlayer.Tocando)
    .OnEntry(() => audio.Iniciar())
    .Permit(Evento.Pause, EstadoDoPlayer.Pausado)
    .Permit(Evento.Stop, EstadoDoPlayer.Parado)
    .Ignore(Evento.Play);

m.Configure(EstadoDoPlayer.Pausado)
    .Permit(Evento.Play, EstadoDoPlayer.Tocando)
    .Permit(Evento.Stop, EstadoDoPlayer.Parado);

m.Fire(Evento.Play);
```

## Quando usar cada estratégia

| FSM… | Melhor opção |
|---|---|
| **Pequena**, estável, 2-5 estados | `switch` (ou `switch` expression) |
| **Média/grande**, que você quer **ler como tabela**, alterar em runtime | **Tabela de transição** |
| Média, quer eficiência e **ações trocáveis** | **Padrão State** |
| **Grande** e com muitas mudanças | **Gerador/DSL** (Stateless, SMC-like) — descrição em um só lugar |

## Onde FSMs aparecem de verdade

### 1. Política de alto nível de interfaces gráficas

Curiosamente, interfaces "sem estado" para o usuário são **fortemente orientadas a estados** por dentro: qual item de menu está desabilitado? qual aba está ativa? qual tela aparece? Organizar essas decisões numa **única estrutura de controle** (uma FSM) evita o pesadelo. Veja um fluxo de **login** descrito como tabela de transição:

| Estado | Evento | Próximo estado | Ação |
|---|---|---|---|
| `Inicio` | `Iniciar` | `Digitando` | MostrarTelaDeLogin |
| `Digitando` | `Enter` | `Verificando` | VerificarSenha |
| `Digitando` | `Cancelar` | `Inicio` | LimparTela |
| `Verificando` | `SenhaBoa` | `Logado` | IniciarProcessoDoUsuario |
| `Verificando` | `SenhaRuim` | `AvisandoErro` | MostrarErro |
| `Verificando` | `TerceiraSenhaRuim` | `Bloqueado` | MostrarTelaDeBloqueio |
| `AvisandoErro` | `OK` | `Verificando`* | MostrarTelaDeLogin |
| `AvisandoErro` | `Cancelar` | `Inicio` | LimparTela |
| `Bloqueado` | `Enter` | `VerificandoAdmin` | VerificarSenhaDeAdmin |
| `VerificandoAdmin` | `SenhaBoa` | `Inicio` | LimparTela |
| `VerificandoAdmin` | `SenhaRuim` | `Bloqueado` | MostrarTelaDeBloqueio |

*(Num fluxo real o "OK" voltaria para `Digitando`; adapte conforme o seu desenho.)*

A **política** do aplicativo (o que acontece depois de quê) fica **num só lugar**, fácil de manter — e o resto do código **não se mistura** com ela. Isso vale para GUIs, interfaces de texto e interfaces máquina-máquina.

### 2. Controladores de interação

Desenhar um retângulo com o mouse: *esperando clique → (mouse down) arrastando → (mouse up) desenha e volta a esperar*; se o mouse sai da janela, a animação **pausa**; se volta, **retoma**; `Esc` **aborta**. São exatamente estados e eventos — sem FSM, vira uma pilha de `bool`s (`estaArrastando`, `estaForaDaJanela`, …) cheia de combinações impossíveis.

```cs
public enum Interacao { EsperandoClique, Arrastando, ForaDaJanela }

public class DesenhoDeRetangulo
{
    public Interacao Estado { get; private set; } = Interacao.EsperandoClique;
    private Point primeiroPonto;

    public void MouseDown(Point p)
    {
        if (Estado != Interacao.EsperandoClique) return;
        primeiroPonto = p; IniciarAnimacao(); Estado = Interacao.Arrastando;
    }
    public void MouseMove(Point p)  { if (Estado == Interacao.Arrastando) Animar(primeiroPonto, p); }
    public void MouseLeave()        { if (Estado == Interacao.Arrastando) { PausarAnimacao(); Estado = Interacao.ForaDaJanela; } }
    public void MouseEnter()        { if (Estado == Interacao.ForaDaJanela) { RetomarAnimacao(); Estado = Interacao.Arrastando; } }
    public void MouseUp(Point p)    { if (Estado == Interacao.Arrastando) { PararAnimacao(); Desenhar(primeiroPonto, p); Estado = Interacao.EsperandoClique; } }
    public void Abortar()           { if (Estado != Interacao.EsperandoClique) { PararAnimacao(); Estado = Interacao.EsperandoClique; } }

    private void IniciarAnimacao() { } private void Animar(Point a, Point b) { } private void PausarAnimacao() { }
    private void RetomarAnimacao() { } private void PararAnimacao() { } private void Desenhar(Point a, Point b) { }
}
```

### 3. Processamento distribuído e protocolos

Enviar um bloco grande em vários pacotes entre dois nós: *Estabelecendo sessão → Enviando pacotes (cada ACK dispara o próximo) → Finalizando sessão*; recusas e abortos fazem **retentar** ou **desistir**. Protocolos de rede (TCP!), *sagas* de pedidos e *workflows* de aprovação são FSMs.

::: figura FSM de transferência
<svg viewBox="0 0 720 150" role="img" aria-label="Estabelecendo, enviando pacotes e finalizando">
  <rect class="f-plain" x="10" y="45" width="170" height="60" rx="30"/><text x="95" y="80" text-anchor="middle" font-weight="800">Estabelecendo</text>
  <rect class="f-good" x="270" y="45" width="170" height="60" rx="30"/><text x="355" y="80" text-anchor="middle" font-weight="800">Enviando pacotes</text>
  <rect class="f-warn" x="530" y="45" width="170" height="60" rx="30"/><text x="615" y="80" text-anchor="middle" font-weight="800">Finalizando</text>
  <path class="arrow" d="M182 75H268"/><text x="225" y="66" text-anchor="middle" font-size="10" class="t-mono">sessaoAceita</text>
  <path class="arrow" d="M442 75H528"/><text x="485" y="66" text-anchor="middle" font-size="10" class="t-mono">ultimoAck</text>
  <path class="arrow" d="M300 45Q355 5 410 45"/><text x="355" y="14" text-anchor="middle" font-size="10" class="t-mono">ack / enviarProximo</text>
  <path class="arrow" d="M95 107Q95 140 60 140M60 140Q20 140 40 107" style="marker-end:none"/><text x="95" y="146" text-anchor="middle" font-size="10" class="t-dim">recusada → reenviar pedido</text>
</svg>
:::

::: cuidado FSM não é desculpa para tudo
Se o seu "estado" é só **um campo que varia um valor**, uma FSM completa é exagero. Elas brilham quando existem **regras sobre quais eventos são válidos em cada estado** e **ações atreladas às transições**.
:::

::: exercicio Modele um pedido
Um pedido de e-commerce tem os estados `Novo`, `Pago`, `Enviado`, `Entregue` e `Cancelado`. Eventos: `Pagar`, `Enviar`, `Entregar`, `Cancelar`.
Regras: só se paga um pedido `Novo`; só se envia um `Pago`; só se entrega um `Enviado`; pode-se cancelar em `Novo` ou `Pago`, nunca depois de enviado. Evento inválido deve lançar exceção.

1. Desenhe a tabela de transição.
2. Implemente com uma tabela `Dictionary<(Estado, Evento), Estado>`.

--- resposta
| Estado | Pagar | Enviar | Entregar | Cancelar |
|---|---|---|---|---|
| Novo | Pago | ✗ | ✗ | Cancelado |
| Pago | ✗ | Enviado | ✗ | Cancelado |
| Enviado | ✗ | ✗ | Entregue | ✗ |
| Entregue | ✗ | ✗ | ✗ | ✗ |
| Cancelado | ✗ | ✗ | ✗ | ✗ |

```cs
public enum EstadoPedido { Novo, Pago, Enviado, Entregue, Cancelado }
public enum EventoPedido { Pagar, Enviar, Entregar, Cancelar }

public class Pedido
{
    private static readonly Dictionary<(EstadoPedido, EventoPedido), EstadoPedido> Tabela =
        new Dictionary<(EstadoPedido, EventoPedido), EstadoPedido>
        {
            { (EstadoPedido.Novo,    EventoPedido.Pagar),    EstadoPedido.Pago },
            { (EstadoPedido.Novo,    EventoPedido.Cancelar), EstadoPedido.Cancelado },
            { (EstadoPedido.Pago,    EventoPedido.Enviar),   EstadoPedido.Enviado },
            { (EstadoPedido.Pago,    EventoPedido.Cancelar), EstadoPedido.Cancelado },
            { (EstadoPedido.Enviado, EventoPedido.Entregar), EstadoPedido.Entregue },
        };

    public EstadoPedido Estado { get; private set; } = EstadoPedido.Novo;

    public void Tratar(EventoPedido e)
    {
        EstadoPedido proximo;
        if (!Tabela.TryGetValue((Estado, e), out proximo))
            throw new InvalidOperationException("Evento " + e + " inválido no estado " + Estado);
        Estado = proximo;
    }
}
```
:::

## Conclusão

Máquinas de estados finitos são **subutilizadas**. Em muitos cenários — interfaces, protocolos, fluxos de negócio — usá-las produz código **mais claro, simples, flexível e preciso**. Combine o **padrão State** (ou uma tabela) com **ferramentas simples de geração** a partir da descrição das transições e você terá a lógica da máquina num só lugar, isolada das ações.

::: resumo
- **FSM:** estados + eventos + transições + ações; ótima para comportamento que depende do histórico.
- **Switch aninhado:** simples e rápido para FSMs pequenas; difícil de manter em grandes; ações podem se misturar à lógica.
- **Tabela de transição:** lógica **em dados**, lê-se como a tabela; fácil de alterar (inclusive em runtime); custo de pesquisa.
- **Padrão State:** uma classe por estado; delega eventos; eficiente, ações separadas; classes maçantes e lógica distribuída.
- **State ≠ Strategy:** no State as derivadas conhecem o contexto e o manipulam (inclusive o estado).
- **Três camadas + gerador/biblioteca** (Stateless etc.) para FSMs grandes.
- Usos: **políticas de UI**, **controladores de interação**, **protocolos/processamento distribuído**, workflows.
- Separar ações da lógica via interface também **facilita testar**.
:::

::: quiz
? Qual a grande vantagem da tabela de transição?
+ A lógica da FSM fica em um só lugar, lê-se como a tabela e pode ser alterada em runtime
- É sempre a mais rápida
- Elimina a necessidade de ações
- Dispensa testes
> O custo é a pesquisa na tabela a cada evento.

? Qual o maior problema do switch aninhado em FSMs grandes?
+ Páginas de cases difíceis de navegar e manter, com ações que tendem a se misturar à lógica
- Não compila em C#
- É lento demais sempre
- Não aceita enums
> Para FSMs pequenas, porém, é elegante.

? No padrão State, o que o contexto faz ao receber um evento?
+ Delega ao objeto do estado atual
- Executa um switch gigante
- Cria uma nova thread
- Consulta o banco
> O estado escolhe a ação e pode mudar o estado do contexto.

? Qual a principal diferença entre State e Strategy?
+ No State as derivadas conhecem e manipulam o contexto (inclusive trocando o estado)
- State usa herança; Strategy não
- Strategy é mais eficiente
- Não há diferença estrutural nem de intenção
> Todo State é um Strategy, mas nem todo Strategy é State.

? Por que as classes de estado podem ser instâncias únicas e estáticas?
+ Porque não têm campos (estado próprio), então uma instância compartilhada basta
- Porque C# obriga
- Porque ficam mais rápidas ao serem criadas várias vezes
- Para permitir herança múltipla
> Evita criar um objeto novo a cada transição.

? Qual um bom uso de FSM em interfaces gráficas?
+ Concentrar a política de alto nível: o que aparece/habilita em cada estado da interação
- Desenhar pixels
- Compilar XAML
- Gerenciar memória
> Evita um emaranhado de flags booleanas.

? Por que a interface de ações (IAcoesDoPlayer) ajuda?
+ Desacopla a lógica da FSM das ações e permite testar com um falso
- Torna o código mais lento
- Impede mudar de estado
- Dispensa a FSM
> A testabilidade empurra o projeto para baixo acoplamento.
:::

::: cartas
FSM :: Máquina de estados finitos: estados, eventos, transições e ações.
Transição :: Mudança de um estado para outro disparada por um evento (com ação opcional).
Switch aninhado :: Implementação simples: switch por estado e, dentro, switch por evento.
Tabela de transição :: Dados que descrevem (estado, evento) → (novo estado, ação), interpretados por um motor.
Padrão State :: Cada estado é uma classe; o contexto delega os eventos ao estado atual.
Contexto :: Classe que guarda o estado atual e as ações; recebe os eventos.
FSM de três camadas :: Ações + FSM gerada + implementação das ações.
Stateless :: Biblioteca .NET para declarar máquinas de estado de forma fluente.
:::
