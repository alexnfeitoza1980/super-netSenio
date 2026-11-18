---
id: aprenda-cap11
order: 23
kind: capitulo
num: 11
group: C# · Parte 5 · Robustez e eventos
title: Eventos e delegados
subtitle: O que o seu código faz quando você não está olhando — publicar, assinar, reagir; delegados e callbacks.
minutes: 60
hue: 340
objectives:
- Entender **eventos** como "publicar e assinar" entre objetos
- Declarar eventos com **delegados** e `EventHandler`, e assinar com `+=`
- Ver como a **IDE** liga os eventos dos controles aos seus métodos
- Distinguir **evento** de **callback** (delegado simples) e usar `Action`/`Func`
origem: livros/01-aprenda-csharp/content/13-cap11.md
---

::: ilustracao img/aprenda/cap11.svg | Eventos: quem aciona não sabe quem vai reagir; cada ouvinte cuida do seu jeito.
:::

## Seus objetos começam a pensar por si mesmos

Você nem sempre controla o que um objeto está fazendo: algumas coisas **simplesmente acontecem** — o usuário clica, o timer dispara, uma temperatura sobe. Quando acontecem, você quer que seus objetos sejam inteligentes o bastante para **reagir** ao que aparece. É para isso que servem os **eventos**.

Um objeto **publica** um evento; outros objetos **se inscrevem**; quando o evento ocorre, todos os inscritos são avisados. E se muitos objetos precisam reagir ao mesmo evento — ou se só **um** precisa receber uma chamada de volta — entram os **delegados** e os **callbacks**.

## Como um objeto SABE responder?

Imagine uma bola de futebol e um goleiro. Quando a bola é chutada, o goleiro precisa reagir. Mas a bola **não conhece** o goleiro, e o goleiro **não fica perguntando** a cada milissegundo "a bola já saiu?" (isso seria **polling**, ineficiente). O correto:

1. A **bola** declara: *"posso avisar quando for atingida"* (**publica** um evento);
2. O **goleiro** diz: *"me avise quando isso acontecer"* (**se inscreve**);
3. Quando a bola é chutada, ela **dispara** o evento e os inscritos **respondem**.

::: figura Publicar e assinar
<svg viewBox="0 0 720 270" role="img" aria-label="Um publicador dispara o evento para vários assinantes">
  <rect class="f-warn" x="10" y="95" width="190" height="80" rx="14"/><text x="105" y="128" text-anchor="middle" font-weight="800" class="t-warn">Publicador</text><text x="105" y="152" text-anchor="middle" font-size="12" class="t-mono">event BolaAtingida</text>
  <rect class="f-info" x="470" y="15" width="230" height="60" rx="12"/><text x="585" y="50" text-anchor="middle" font-weight="700" class="t-info">Goleiro (assinante)</text>
  <rect class="f-good" x="470" y="105" width="230" height="60" rx="12"/><text x="585" y="140" text-anchor="middle" font-weight="700" class="t-good">Torcida (assinante)</text>
  <rect class="f-prim" x="470" y="195" width="230" height="60" rx="12"/><text x="585" y="230" text-anchor="middle" font-weight="700" class="t-prim">Placar (assinante)</text>
  <path class="arrow" d="M202 120C320 100 380 60 466 48"/><path class="arrow" d="M202 135H466"/><path class="arrow" d="M202 150C320 170 380 210 466 225"/>
  <text x="330" y="115" font-size="13" class="t-dim">dispara o evento</text>
</svg>
:::

::: regra Quem publica não conhece os assinantes
O publicador não sabe **quem** está ouvindo nem **o que** cada um faz. Isso é o que torna o design flexível: dá para ligar e desligar assinantes sem mexer no publicador.
:::

## Um evento na prática

Vamos criar um **sensor de temperatura** que avisa quando a temperatura muda:

```cs
// 1) os dados que vão junto com o evento
class TemperaturaEventArgs : EventArgs
{
    public double Valor { get; private set; }
    public TemperaturaEventArgs(double valor) { Valor = valor; }
}

// 2) o publicador
class Sensor
{
    public event EventHandler<TemperaturaEventArgs> TemperaturaAlterada;   // o evento

    private double temperatura;
    public double Temperatura
    {
        get { return temperatura; }
        set
        {
            temperatura = value;
            if (TemperaturaAlterada != null)                                  // há assinantes?
                TemperaturaAlterada(this, new TemperaturaEventArgs(value));   // dispara!
        }
    }
}

// 3) os assinantes
class Ventilador
{
    public void Reagir(object sender, TemperaturaEventArgs e)
    {
        if (e.Valor > 28) Console.WriteLine("Ventilador LIGADO (" + e.Valor + "°C)");
    }
}

class Alarme
{
    public void Reagir(object sender, TemperaturaEventArgs e)
    {
        if (e.Valor > 40) Console.WriteLine("🚨 ALARME: calor extremo!");
    }
}
```

Uso:

```cs
Sensor sensor = new Sensor();
Ventilador v = new Ventilador();
Alarme a = new Alarme();

sensor.TemperaturaAlterada += v.Reagir;   // inscreve
sensor.TemperaturaAlterada += a.Reagir;   // inscreve

sensor.Temperatura = 25;   // ninguém reage
sensor.Temperatura = 30;   // ventilador liga
sensor.Temperatura = 45;   // ventilador e alarme

sensor.TemperaturaAlterada -= a.Reagir;   // cancela a inscrição
```

```saida
Ventilador LIGADO (30°C)
Ventilador LIGADO (45°C)
🚨 ALARME: calor extremo!
```

::: passos Ligando os pontos
**1.** `sensor.TemperaturaAlterada += v.Reagir;` — o ventilador se **inscreve**. O evento guarda, internamente, "uma lista de métodos a chamar".

::: html
<svg viewBox="0 0 700 120"><rect class="f-warn" x="10" y="20" width="200" height="80" rx="12"/><text x="110" y="50" text-anchor="middle" font-weight="800" class="t-warn">Sensor</text><text x="110" y="76" text-anchor="middle" class="t-mono" font-size="11">lista: [ v.Reagir ]</text><rect class="f-info" x="440" y="30" width="240" height="60" rx="12"/><text x="560" y="66" text-anchor="middle" font-weight="700" class="t-info">Ventilador</text><path class="arrow" d="M212 60H436"/></svg>
:::
===
**2.** `sensor.TemperaturaAlterada += a.Reagir;` — o alarme também. Agora a lista tem **dois** métodos.

::: html
<svg viewBox="0 0 700 150"><rect class="f-warn" x="10" y="30" width="200" height="90" rx="12"/><text x="110" y="60" text-anchor="middle" font-weight="800" class="t-warn">Sensor</text><text x="110" y="86" text-anchor="middle" class="t-mono" font-size="11">[ v.Reagir, a.Reagir ]</text><rect class="f-info" x="440" y="10" width="240" height="50" rx="12"/><text x="560" y="42" text-anchor="middle" font-weight="700" class="t-info">Ventilador</text><rect class="f-bad" x="440" y="90" width="240" height="50" rx="12"/><text x="560" y="122" text-anchor="middle" font-weight="700" class="t-bad">Alarme</text><path class="arrow" d="M212 60C320 60 380 40 436 36"/><path class="arrow" d="M212 90C320 90 380 112 436 116"/></svg>
:::
===
**3.** `sensor.Temperatura = 45;` — o `set` **dispara** o evento: o sensor percorre a lista e chama **cada** método com `(sensor, args)`. O sensor nem sabe quem são os assinantes.
:::

### O padrão `EventHandler`

A convenção do .NET para eventos:

- O delegado tem **dois parâmetros**: `object sender` (quem disparou) e `EventArgs e` (dados do evento).
- Se não há dados, use `EventHandler` simples e `EventArgs.Empty`; se há, herde de `EventArgs` e use `EventHandler<T>`.
- O **nome** do evento descreve algo que **aconteceu** (`Clicado`, `TemperaturaAlterada`), no passado.

::: dica Disparar com segurança
Em C# moderno, em vez do `if (evento != null)`, escreva apenas:
```cs
TemperaturaAlterada?.Invoke(this, new TemperaturaEventArgs(value));
```
:::

::: cuidado Cancele a assinatura!
Um objeto que se inscreve num evento **fica referenciado** pelo publicador. Se você descartar o assinante sem fazer `-=`, o **garbage collector não o libera** (o publicador ainda aponta para ele): é um vazamento de memória clássico em aplicativos de longa duração.
:::

## Os eventos que você já usou

Todo `button1_Click` que você escreveu é um **tratador de evento** (*event handler*). Abra o `Form1.Designer.cs`:

```cs
this.button1.Click += new System.EventHandler(this.button1_Click);
```

É exatamente o que acabamos de fazer: o **botão** (publicador) tem um evento `Click`; o **formulário** (assinante) liga o seu método `button1_Click` a ele. Quando o usuário clica, o Windows avisa o botão, que **dispara** o evento, e seu método roda.

::: ide A IDE cria os tratadores para você
- **Duplo clique** num controle → cria o tratador do evento **padrão** (para o botão, `Click`).
- Na janela **Propriedades**, botão ⚡ (**Eventos**) → lista **todos** os eventos do controle; dê duplo clique ao lado do evento desejado (`MouseEnter`, `KeyDown`, `TextChanged`…).
- Nunca apague o método **à mão** sem remover a linha do `Designer.cs` — o formulário deixa de compilar.
:::

::: exercicio Reaja ao mouse
Num formulário com um `Label` (`label1`) e um `Button` (`button1`), faça:

1. Ao passar o mouse sobre o botão, o texto do rótulo vira "Vai clicar?".
2. Ao tirar o mouse, volta a "…".
3. Ao clicar, aparece "Clicou!".

--- resposta
Pela janela Propriedades → Eventos, crie `MouseEnter`, `MouseLeave` e `Click`:
```cs
private void button1_MouseEnter(object sender, EventArgs e) { label1.Text = "Vai clicar?"; }
private void button1_MouseLeave(object sender, EventArgs e) { label1.Text = "…"; }
private void button1_Click(object sender, EventArgs e)      { label1.Text = "Clicou!"; }
```
Cada método é ligado ao evento no `Designer.cs` com `+=` — a IDE faz isso por você.
:::

## Delegados: um substituto para um método

Até agora usamos eventos como se fossem mágica. O ingrediente por trás deles é o **delegado**: um **tipo que representa um método** com uma **assinatura** (parâmetros + retorno) determinada. Uma variável de delegado guarda **uma referência a um método** — e você pode chamá-lo por ela.

::: analogia Delegado = procuração
Você assina uma **procuração** a alguém: *"esta pessoa age em meu nome"*. Um delegado é uma procuração para um método: a variável aponta para o método, e quem a chama **não precisa saber qual método** será executado.
:::

```cs
// declara o TIPO do delegado: qualquer método que receba um int e devolva int
delegate int Operacao(int x);

class Exemplo
{
    static int Dobrar(int x)   { return x * 2; }
    static int Quadrado(int x) { return x * x; }

    static void Main()
    {
        Operacao op = Dobrar;        // a variável aponta para o método
        Console.WriteLine(op(5));    // 10

        op = Quadrado;               // agora aponta para outro
        Console.WriteLine(op(5));    // 25

        op = Dobrar;
        op += Quadrado;              // delegado MULTICAST: chama os dois, em ordem
    }
}
```

O .NET já traz delegados genéricos prontos, então raramente você declara os seus:

| Tipo | Assinatura | Exemplo |
|---|---|---|
| `Action` | `void()` | `Action oi = () => Console.WriteLine("Oi");` |
| `Action<T>` | `void(T)` | `Action<string> f = s => Console.WriteLine(s);` |
| `Func<T, R>` | `R(T)` | `Func<int,int> dobro = x => x * 2;` |
| `Predicate<T>` | `bool(T)` | `lista.FindAll(n => n > 10)` |
| `EventHandler` | `void(object, EventArgs)` | usado em eventos de controles |

### Lambdas: delegados escritos na hora

```cs
Func<int, int> dobro = x => x * 2;           // expressão lambda
Action<string> log = mensagem => { Console.WriteLine("[LOG] " + mensagem); };

botao.Click += (sender, e) => MessageBox.Show("Clicou!");   // assina com lambda
```

## Evento × callback

Um **evento** tem **vários** assinantes e quem publica **não sabe quem são**. Um **callback** é mais simples: você passa **um único** delegado a um objeto, que o chamará **quando terminar** ou quando precisar de uma resposta.

```cs
class Download
{
    private Action<int> aoProgredir;                       // callback (um só)
    public Download(Action<int> aoProgredir) { this.aoProgredir = aoProgredir; }

    public void Baixar()
    {
        for (int p = 0; p <= 100; p += 25)
            aoProgredir(p);                                // "volta" para quem chamou
    }
}

// uso:
new Download(p => Console.WriteLine(p + "%")).Baixar();
```

| | **Evento** | **Callback** |
|---|---|---|
| Quantos ouvintes? | Vários (inscrição aberta) | Exatamente um |
| Quem decide? | Cada assinante se inscreve sozinho | Quem cria o objeto passa o método |
| Protegido? | Só o dono dispara; fora só `+=`/`-=` | O delegado pode ser chamado por quem o tem |
| Típico de | Interface do usuário, notificações | Algoritmos configuráveis, assíncrono |

::: pense Evento ou callback?
Se **qualquer** parte do programa pode querer saber "quando aconteceu X", use **evento**. Se você quer **injetar** um comportamento específico num objeto (ex.: "como comparar dois itens"), use **callback/delegado**.
:::

::: resumo
- **Evento** = publicar/assinar: o publicador dispara; os assinantes reagem.
- Assine com **`+=`** e cancele com **`-=`**; assinantes são métodos com a assinatura do delegado.
- O padrão do .NET: `EventHandler` / `EventHandler<T>` com `(object sender, EventArgs e)`.
- A IDE liga os eventos dos controles com **`+=`** no `Designer.cs`.
- **Delegado** = tipo que referencia métodos; **multicast** chama vários; `Action` e `Func` cobrem a maioria dos casos.
- **Callback** = um delegado único passado a um objeto; **lambda** = delegado escrito na hora.
:::

::: quiz
? Qual operador inscreve um método em um evento?
- `=`
+ `+=`
- `++`
- `=>`
> `evento += metodo;` adiciona o assinante à lista.

? Quem dispara um evento?
- O assinante
- O compilador
+ O objeto que publica (o dono) do evento
- Qualquer classe
> Fora da classe só se pode inscrever/cancelar.

? Em `void Reagir(object sender, EventArgs e)`, o que é `sender`?
+ O objeto que disparou o evento
- O assinante
- A thread atual
- O nome do evento
> `sender` identifica o publicador.

? O que é um delegado?
+ Um tipo que representa uma referência a métodos com certa assinatura
- Uma classe abstrata
- Um tipo de coleção
- Uma exceção
> É como um "ponteiro" seguro para métodos.

? Qual delegado genérico representa um método `bool(int)`?
- `Action<int>`
- `Func<bool>`
+ `Predicate<int>` (ou `Func<int, bool>`)
- `EventHandler`
> `Predicate<T>` devolve bool.

? Qual a principal diferença entre evento e callback?
+ Evento admite vários assinantes; callback é um único delegado entregue ao objeto
- Callback usa `+=`; evento não
- Evento não pode ter parâmetros
- Não há diferença
> Eventos expõem inscrição; callbacks são injetados.

? O que acontece se um assinante nunca cancelar a inscrição (`-=`)?
+ O publicador continua referenciando-o, impedindo a coleta de lixo
- O evento para de funcionar
- O programa não compila
- Nada
> Vazamento de memória clássico.

? Onde a IDE registra a ligação entre o botão e seu método `button1_Click`?
- Em `Program.cs`
+ Em `Form1.Designer.cs`, com `this.button1.Click += ...`
- No banco de dados
- No arquivo `.resx`
> O designer escreve a assinatura no código gerado.
:::

::: cartas
Evento :: Sinal que um objeto publica para avisar que algo aconteceu.
Publicador / assinante :: Quem dispara o evento / quem se inscreve para reagir.
+= / -= :: Inscrever / cancelar a inscrição em um evento.
EventHandler :: Delegado padrão de eventos: `(object sender, EventArgs e)`.
EventArgs :: Classe base dos dados que acompanham um evento.
Tratador de evento :: Método chamado quando o evento ocorre (ex.: `button1_Click`).
Delegado :: Tipo que referencia métodos de uma assinatura específica.
Multicast :: Delegado que chama vários métodos em sequência.
Callback :: Delegado passado a um objeto para ser chamado de volta.
Lambda :: Método anônimo escrito inline: `x => x * 2`.
Action / Func :: Delegados genéricos prontos: sem retorno / com retorno.
:::
