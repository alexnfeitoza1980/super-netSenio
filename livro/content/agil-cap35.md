---
id: agil-cap35
order: 52
kind: capitulo
num: 35
group: C# · Parte 11 · Pacotes, padrões e arquitetura
title: Observer — evoluindo até um padrão
subtitle: Em vez de "inserir" um padrão pronto, deixe o código evoluir em passos pequenos até ele — e conheça os modelos pull e push, os eventos do C# e `IObservable`.
minutes: 55
hue: 165
objectives:
- Ver como um projeto **evolui por pequenos passos** até o padrão **Observer**
- Entender **Subject/Observer**, os modelos **pull** e **push** e o papel de **OCP, LSP, DIP e ISP**
- Relacionar Observer com **eventos** e delegados do C#, `INotifyPropertyChanged` e `IObservable<T>`
- Reconhecer o **abuso** de Observer (fluxo difícil de rastrear)
origem: livros/02-projeto-agil-csharp/content/51-cap35.md
---

::: ilustracao img/agil/cap35.svg | Observer: quem muda avisa quem se inscreveu, sem conhecer os detalhes de cada um.
:::

## O objetivo deste capítulo

O Observer é um padrão importante — mas este capítulo tem um objetivo **maior**: mostrar **como um projeto e um código podem evoluir para usar um padrão**. Nos capítulos anteriores muitos padrões apareceram "prontos", o que passa a ideia de que padrões são algo que se **insere** no código. Não é o que o autor aconselha. Em vez disso, **evolui-se o código em direção a um padrão**: pode-se chegar nele ou não, dependendo de os problemas **serem resolvidos**. Às vezes você começa com um padrão em mente e termina em outro lugar.

::: regra Evolua em direção ao padrão
Se você conhece padrões, um deles vai surgir na sua cabeça diante de um problema. A pergunta é: **implemento direto** ou **evoluo por pequenos passos**? Aqui fazemos a segunda — e a **qualquer momento** poderíamos parar (problema resolvido) ou mudar de rumo.
:::

## O problema: um termostato digital

Temos um objeto `Termometro` que sabe **medir** a temperatura do ambiente (recebe leituras do sensor e converte em graus). Queremos um **mostrador digital** na área de trabalho que exiba a temperatura **continuamente**.

### Tentativa 1 — Laço de sondagem (polling)

```cs
public void ExibirTemperatura(Termometro t)
{
    while (true)
    {
        double graus = t.Graus;
        Mostrar(graus);
    }
}
```

Isso consome **todos os ciclos de CPU** para redesenhar valores que **quase sempre não mudaram**. Para um relógio de parede talvez sirva; num computador de mesa, não. Precisamos de um **mecanismo** para os dados irem do termômetro ao mostrador — e de **como testar** que ele funciona.

### Passo 1 — Pensar no teste cria interfaces

O problema fundamental é **como conectar** `Termometro` e `Mostrador`. Para testar essa conexão em **isolamento**, criamos duas interfaces que *fingem* ser cada lado: `IFonteDeTemperatura` e `IDestinoDeTemperatura`, e um objeto **Driver** que as liga. O teste usa **falsos** (mocks) para as duas pontas.

::: figura Isolando para testar
<svg viewBox="0 0 740 190" role="img" aria-label="Teste usa fonte falsa e destino falso ligados por um driver">
  <g text-anchor="middle" font-size="12"><rect class="f-plain" x="10" y="70" width="130" height="46" rx="8"/><text x="75" y="98">Teste</text>
  <rect class="f-info" x="190" y="20" width="170" height="44" rx="8"/><text x="275" y="40" font-size="10" class="t-dim">«interface»</text><text x="275" y="56" font-weight="700">IFonte</text>
  <rect class="f-info" x="190" y="125" width="170" height="44" rx="8"/><text x="275" y="145" font-size="10" class="t-dim">«interface»</text><text x="275" y="161" font-weight="700">IDestino</text>
  <rect class="f-prim" x="420" y="70" width="140" height="46" rx="8"/><text x="490" y="98" font-weight="700">Driver</text>
  <rect class="f-good" x="610" y="20" width="120" height="44" rx="8"/><text x="670" y="46">FonteFalsa</text>
  <rect class="f-good" x="610" y="125" width="120" height="44" rx="8"/><text x="670" y="151">DestinoFalso</text></g>
  <path class="arrow" d="M420 82L362 48"/><path class="arrow" d="M420 106L362 142"/><path d="M608 42L362 42" stroke="var(--ink-3)" stroke-dasharray="4 3"/><path d="M608 148L362 148" stroke="var(--ink-3)" stroke-dasharray="4 3"/>
</svg>
:::

Observe: as **interfaces surgiram por causa do teste**. Para testar um módulo é preciso **isolá-lo** dos demais; pensar nos testes primeiro **minimiza o acoplamento**.

### Passo 2 — Como o Driver sabe que a temperatura mudou?

Sondar recriaria o desperdício de CPU. O mais simples: **a fonte avisa o driver** quando muda. Passamos o driver à fonte:

```cs
public interface IFonte { void DefinirDriver(Driver driver); }
public interface IDestino { void DefinirTemperatura(double graus); }

public class Driver
{
    private readonly IDestino destino;
    public Driver(IFonte fonte, IDestino destino) { fonte.DefinirDriver(this); this.destino = destino; }
    public void Atualizar(double graus) { destino.DefinirTemperatura(graus); }
}
```

O teste: cria o driver ligando **fonte falsa** e **destino falso**, muda a temperatura **na fonte** e espera que **chegue ao destino**. Verde.

### Passo 3 — Quebrando a dependência da fonte no driver

`IFonte` depende de `Driver` (o parâmetro de `DefinirDriver`): só um `Driver` pode usar uma fonte. Não é isso que queremos. Criamos uma interface que a fonte possa usar **e** que o driver implemente — `IObservador`:

```cs
public interface IObservador { void Atualizar(double graus); }
public interface IFonte { void DefinirObservador(IObservador o); }
public class Driver : IObservador { /* ... */ }
```

Agora **qualquer um** pode usar a fonte implementando `IObservador` e passando-se a si mesmo.

### Passo 4 — Vários destinos

Queremos que **uma** fonte alimente **vários** destinos: o mostrador digital, um **alarme de calor**, um **registrador histórico**. O `Driver` teria de receber uma **lista de destinos** (`AdicionarDestino`). Mas olhe: `IObservador` e `IDestino` têm o **mesmo método**! O destino pode **implementar** `IObservador` — e então **não precisamos do `Driver` nem de `IDestino`**:

::: passos Simplificando
**Antes:** Fonte → Driver (observador) → Destino. Duas indireções.
===
**Observação:** `IObservador.Atualizar(graus)` e `IDestino.DefinirTemperatura(graus)` são o mesmo conceito.
===
**Depois:** o próprio **destino implementa `IObservador`** e se registra direto na fonte. `Driver` e `IDestino` desaparecem — por que pensamos que precisávamos deles?
===
**Vários observadores:** `DefinirObservador` vira `RegistrarObservador` e a fonte guarda uma **lista**.
:::

```cs
public interface IFonte { void RegistrarObservador(IObservador o); }

public class TermometroFalso : IFonte
{
    private readonly List<IObservador> observadores = new List<IObservador>();
    public void RegistrarObservador(IObservador o) { observadores.Add(o); }
    public void DefinirTemperatura(double graus)
    {
        foreach (IObservador o in observadores) o.Atualizar(graus);
    }
}
```

Teste: registrar **dois** destinos e ver ambos receberem `22,5`.

### Passo 5 — Tirar o registro e a notificação de cada fonte

O registro/atualização está dentro de `TermometroFalso`. Mas o **termômetro de verdade** (e qualquer outra fonte) teria de **duplicar** esse código. Não gostamos de **duplicar**. Movemos tudo para uma **classe base** `Sujeito` (*Subject*), com `Registrar` e `Notificar`:

```cs
public abstract class Sujeito
{
    private readonly List<IObservador> observadores = new List<IObservador>();
    public void Registrar(IObservador o) { observadores.Add(o); }
    protected void Notificar(double graus) { observadores.ForEach(o => o.Atualizar(graus)); }
}

public class Termometro : Sujeito
{
    public void Medir(double graus) { /* ... */ Notificar(graus); }
}
```

Mas agora **`Termometro` precisa herdar de `Sujeito`** — e em C# só temos **herança simples de classe**. E `Termometro` "só devia saber sobre temperatura"; por que depender de registro e notificação? Há uma saída por **composição** (*delegação*): `Termometro` tem um objeto `Sujeito` interno e repassa `Registrar`. É desajeitado — e o autor decide **voltar atrás** e **conviver** com a dependência: às vezes o compromisso vale a pena.

### Passo 6 — O nome e o modelo pull

`IFonte` virou uma classe com registro e notificação — o nome já não serve. O padrão chama isso de **Subject**. Mas nosso observador recebe a **temperatura** como argumento (*modelo push*), o que torna a interface **específica de temperatura**. Se passarmos a **não enviar dado** e deixarmos o observador **consultar** a fonte quando for avisado (*modelo pull*), `Sujeito`/`IObservador` ficam **genéricos** e reutilizáveis:

```cs
public interface IObservador { void Atualizar(); }

public class Sujeito
{
    private readonly List<IObservador> observadores = new List<IObservador>();
    public void Registrar(IObservador o) { observadores.Add(o); }
    protected void Notificar() { observadores.ForEach(o => o.Atualizar()); }
}

public interface IFonteDeTemperatura { double Graus { get; } }       // o observador consulta por aqui

public class Termometro : Sujeito, IFonteDeTemperatura
{
    public double Graus { get; private set; }
    public void Medir(double graus) { Graus = graus; Notificar(); }
}

public class Mostrador : IObservador
{
    private readonly IFonteDeTemperatura fonte;
    public Mostrador(IFonteDeTemperatura fonte) { this.fonte = fonte; }
    public void Atualizar() { Console.WriteLine($"{fonte.Graus:F1} °C"); }     // PUXA o valor
}
```

Chegamos ao **Observer canônico (modelo pull)**.

## O padrão Observer

::: figura Observer (pull)
<svg viewBox="0 0 740 230" role="img" aria-label="Subject com Registrar e Notificar, Observer com Atualizar, Termometro e Mostrador">
  <rect class="f-prim" x="40" y="20" width="220" height="86" rx="10"/><text x="150" y="42" text-anchor="middle" font-weight="800" class="t-prim">Sujeito (Subject)</text><g class="t-mono" font-size="12"><text x="54" y="66">+ Registrar(IObservador)</text><text x="54" y="86"># Notificar()</text></g>
  <rect class="f-info" x="480" y="20" width="220" height="66" rx="10"/><text x="590" y="40" text-anchor="middle" font-size="10" class="t-dim">«interface»</text><text x="590" y="58" text-anchor="middle" font-weight="800" class="t-info">IObservador</text><text x="590" y="78" text-anchor="middle" font-size="12" class="t-mono">+ Atualizar()</text>
  <path class="arrow" d="M262 60H478"/><text x="370" y="52" text-anchor="middle" font-size="12" class="t-mono">0..*  observadores</text>
  <path class="line" d="M150 108V150" style="marker-end:none"/><rect class="f-good" x="70" y="150" width="160" height="42" rx="8"/><text x="150" y="176" text-anchor="middle" font-weight="700">Termometro</text>
  <path class="line" d="M590 88V150" style="marker-end:none"/><rect class="f-good" x="510" y="150" width="160" height="42" rx="8"/><text x="590" y="176" text-anchor="middle" font-weight="700">Mostrador</text>
  <path class="arrow" d="M510 172H232" stroke-dasharray="5 4"/><text x="370" y="164" text-anchor="middle" font-size="11" class="t-dim">consulta (pull)</text>
</svg>
:::

- **Sujeito (Subject):** guarda uma **lista de observadores**; oferece **registrar** e **notificar**.
- **Observador (Observer):** interface com `Atualizar()`.
- Quando o estado do termômetro muda, ele chama `Notificar()`, que chama `Atualizar()` de **cada** observador registrado. O `Mostrador` então **pergunta** ao termômetro a nova temperatura e a exibe.

Depois que você entende o padrão, **vê-o em todo lugar**: com a indireção, dá para registrar observadores em qualquer objeto, em vez de escrever o objeto para chamar você. Mas a indireção **pode ser levada a extremos**: o **abuso** de Observer torna os sistemas **difíceis de entender e rastrear** ("quem reage a isso? em que ordem?").

### Modelo pull × modelo push

| | **Pull** | **Push** |
|---|---|---|
| O que `Atualizar` recebe | Nada — o observador **consulta** o sujeito | Um **argumento** (dado ou "dica" do que mudou) |
| Vantagem | **Simples**; `Sujeito`/`IObservador` reutilizáveis em biblioteca | O observador sabe **o que mudou** sem perguntar |
| Desvantagem | E se o objeto tem **mil campos**? *O que* mudou? | Interface mais específica/acoplada ao dado |
| Quando | Objeto observado **simples** | Objeto **complexo** e o observador precisa de uma **pista** |

Exemplo de push com **dica**: observar um `Funcionario` com muitos campos — `Notificar(DicaDeMudanca.Salario)`; `ObservadorDeSalario.Atualizar(dica)` só reage se a dica for `Salario`. A dica pode ser um `enum`, uma string ou um objeto com **valor antigo e novo**.

## Observer e os princípios

- **OCP:** o motivo central do padrão — **novos observadores** entram **sem alterar** o sujeito (que permanece fechado).
- **LSP:** `Termometro` é substituível por `Sujeito`; `Mostrador`, por `IObservador`.
- **DIP:** o `Mostrador` concreto depende da abstração `IObservador`; o código concreto do `Sujeito` também. (O `Sujeito`, mesmo sem métodos abstratos, é **logicamente abstrato** — só faz sentido derivado; pode ter construtor `protected`.)
- **ISP:** `Sujeito` e `IFonteDeTemperatura` **segregam** os clientes do termômetro: quem observa vê uma interface; quem consulta vê outra.

## E no C#? Eventos já são Observer

Em C#, o padrão vem **pronto na linguagem**: o **evento** (capítulo 11) é um Subject embutido; os **assinantes** (`+=`) são os observadores.

```cs
public class Termometro
{
    public event EventHandler<double> TemperaturaMudou;          // Subject embutido
    private double graus;
    public double Graus
    {
        get { return graus; }
        private set { graus = value; TemperaturaMudou?.Invoke(this, value); }   // push (e o sender permite pull)
    }
    public void Medir(double g) { Graus = g; }
}

var t = new Termometro();
t.TemperaturaMudou += (s, graus) => Console.WriteLine($"Mostrador: {graus:F1} °C");
t.TemperaturaMudou += (s, graus) => { if (graus > 35) Console.WriteLine("ALERTA de calor!"); };
t.Medir(36);
```

Outras formas no .NET:

| Mecanismo | Para quê |
|---|---|
| `event` + `EventHandler<T>` | Observer clássico, em memória, com `+=`/`-=` |
| **`INotifyPropertyChanged`** | Observer da **interface gráfica**: a tela observa o modelo (data binding WPF/MAUI/WinForms) |
| **`IObservable<T>` / `IObserver<T>`** | Observer **formal** (push) do .NET; base do **Rx** (Reactive Extensions) |
| **`ObservableCollection<T>`** | Coleção que avisa quando itens entram/saem |
| **Mensageria / Pub-Sub** (MediatR, filas, SignalR) | Observer **entre processos/serviços** |

```cs
// INotifyPropertyChanged: a UI observa o modelo
public class Funcionario : INotifyPropertyChanged
{
    private decimal salario;
    public event PropertyChangedEventHandler PropertyChanged;
    public decimal Salario
    {
        get { return salario; }
        set { if (salario != value) { salario = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Salario))); } }
        // a "dica" é o nome da propriedade: push com pista
    }
}
```

::: cuidado Armadilhas do Observer
- **Vazamento de memória:** o sujeito **segura referências** aos observadores; se um observador de vida curta não faz `-=`/`Desregistrar`, **nunca é coletado**.
- **Ordem e reentrância:** observadores que **alteram** o sujeito durante a notificação podem causar laços; a ordem de notificação nem sempre é garantida.
- **Exceções:** se um observador lança exceção, os seguintes podem **não ser avisados** — decida a política.
- **Threads:** notificações podem chegar em outra thread (cuidado com a interface gráfica).
- **Rastreabilidade:** muita indireção dificulta responder "o que acontece quando X muda?".
:::

## Sobre diagramas durante a evolução

O autor menciona um detalhe revelador: os diagramas deste capítulo seriam, na prática, **rabiscos em papel ou quadro**, usados para decidir o **próximo passo** e depois **descartados** — "não conheço circunstância em que uma ferramenta de desenho seja mais rápida que um guardanapo". Nesse nível de detalhe, o **código** costuma bastar como documentação; diagramas de alto nível, nem sempre.

::: exercicio Evolua você mesmo
Você tem uma classe `Pedido` e três partes do sistema que precisam reagir quando o status muda: enviar e-mail, atualizar o estoque, registrar auditoria. Hoje `Pedido.MudarStatus()` chama as três diretamente.

1. Que princípio isso viola?
2. Refatore para Observer com **eventos C#** (modelo push com `StatusMudouEventArgs`).
3. Dê um motivo para preferir **pull** nesse caso.

--- resposta
1. **OCP** (cada nova reação exige editar `Pedido`) e **DIP/SRP** (`Pedido` conhece e depende de e-mail, estoque e auditoria).
2. ```cs
   public class StatusMudouEventArgs : EventArgs
   {
       public StatusPedido Anterior { get; }
       public StatusPedido Novo { get; }
       public StatusMudouEventArgs(StatusPedido anterior, StatusPedido novo) { Anterior = anterior; Novo = novo; }
   }

   public class Pedido
   {
       public event EventHandler<StatusMudouEventArgs> StatusMudou;
       private StatusPedido status;
       public void MudarStatus(StatusPedido novo)
       {
           var anterior = status; status = novo;
           StatusMudou?.Invoke(this, new StatusMudouEventArgs(anterior, novo));
       }
   }

   pedido.StatusMudou += (s, e) => email.Enviar(...);
   pedido.StatusMudou += (s, e) => estoque.Reservar(...);
   pedido.StatusMudou += (s, e) => auditoria.Registrar(e.Anterior, e.Novo);
   ```
3. Se o `Pedido` tem **muitos campos** e cada observador só quer o **estado atual** de alguns, o pull (`Atualizar()` + consulta ao pedido) evita inflar o `EventArgs` — porém push com "dica" é melhor quando é preciso saber **o que mudou** (antes/depois).
:::

::: resumo
- Prefira **evoluir o código em direção a um padrão**, em passos pequenos, a "inseri-lo" pronto; pare quando o problema estiver resolvido.
- **Pensar em testes** gerou as interfaces; simplificações removeram intermediários (Driver, Destino).
- **Observer:** `Sujeito` (lista, `Registrar`, `Notificar`) + `IObservador.Atualizar()`; novos observadores **não alteram o sujeito (OCP)**.
- **Pull** (observador consulta) é simples; **push** (com dados/dica) é melhor para objetos complexos.
- No C#: **eventos**, `INotifyPropertyChanged`, `IObservable<T>`/Rx e `ObservableCollection<T>` implementam o padrão.
- Cuide de **vazamentos** (`-=`), ordem, exceções, threads e **abuso** (sistema difícil de rastrear).
:::

::: quiz
? Qual era o objetivo principal do capítulo, além de apresentar o Observer?
+ Mostrar como um projeto pode evoluir passo a passo até um padrão
- Provar que o Observer é o melhor padrão
- Ensinar a usar herança múltipla
- Mostrar que testes são opcionais
> Padrões não devem ser inseridos prontos; evolui-se na direção deles.

? Por que as interfaces `IFonte`/`IDestino` surgiram no primeiro passo?
+ Para isolar o módulo e testá-lo com falsos (pensar nos testes reduz o acoplamento)
- Porque o C# exige interfaces
- Para economizar memória
- Para evitar eventos
> Testar exige isolar o que se testa.

? Qual a vantagem do Observer em termos do OCP?
+ Novos observadores podem ser adicionados sem alterar o sujeito
- O sujeito pode ser alterado livremente
- Elimina a necessidade de interfaces
- Impede mais de um observador
> O sujeito permanece fechado para modificação.

? Qual a diferença entre os modelos pull e push?
+ No pull o observador consulta o sujeito ao ser avisado; no push o aviso já traz dados/dica
- No pull o sujeito nunca notifica
- No push não há interface
- São idênticos
> Push é melhor quando o sujeito é complexo e o observador precisa saber o que mudou.

? Qual recurso do C# é, na prática, o padrão Observer embutido na linguagem?
+ Eventos (`event` com `+=`)
- `goto`
- `static`
- `unsafe`
> Assinantes são os observadores; o evento é o sujeito.

? Um risco comum do Observer é…
+ Vazamento de memória quando observadores não cancelam a assinatura
- Impossibilidade de ter mais de um observador
- Falta de interfaces
- Perda do polimorfismo
> O sujeito mantém referências aos observadores.

? Qual interface do .NET é usada pela UI para observar mudanças em propriedades do modelo?
+ `INotifyPropertyChanged`
- `IDisposable`
- `IComparable`
- `ICloneable`
> Ela dispara `PropertyChanged` com o nome da propriedade alterada.
:::

::: cartas
Observer :: Padrão em que observadores registrados são notificados quando o sujeito muda.
Subject (sujeito) :: Objeto observado: guarda os observadores e os notifica.
Modelo pull :: O observador consulta o sujeito depois de notificado.
Modelo push :: A notificação carrega o dado ou uma dica do que mudou.
Evoluir para um padrão :: Refatorar em passos pequenos até a estrutura do padrão.
INotifyPropertyChanged :: Interface .NET para a UI observar mudanças de propriedades.
IObservable<T> / IObserver<T> :: Contratos formais de Observer (push) no .NET; base do Rx.
Vazamento de assinatura :: Observador que não se desinscreve e impede o GC.
:::
