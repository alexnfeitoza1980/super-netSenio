---
id: aprenda-cap12
order: 24
kind: capitulo
num: 12
group: C# · Parte 5 · Robustez e eventos
title: Revisão e prévia
subtitle: Conhecimento, poder e construir coisas legais — o simulador de colmeia (fase 1), `Timer` e um gostinho de LINQ.
minutes: 80
hue: 340
objectives:
- **Revisar** classes, herança, coleções e eventos construindo um sistema maior
- Modelar um sistema **por turnos** (`Go()`) com `World`, `Hive`, `Bee` e `Flower`
- Usar **máquina de estados** (`enum`) e **`Timer`** para animar
- Dar o primeiro passo com **LINQ** para consultar coleções
origem: livros/01-aprenda-csharp/content/14-cap12.md
---

::: ilustracao img/aprenda/cap12.svg | Revisão e prévia: olhar o que você já domina e o que vem a seguir.
:::

## Aprender não serve de muito até que você CRIE algo

Enquanto você não escreve código que **funciona**, é difícil saber se entendeu mesmo os conceitos mais complexos. Este capítulo é uma **revisão ativa** e também uma **prévia**: amarramos pontas soltas (timers, coleções com LINQ) e construímos a **fase 1** de um aplicativo realmente complexo.

::: vila Já fomos apicultores
Lembra das abelhas dos capítulos 6 e 7? Agora vamos **simulá-las** de verdade: uma colmeia, um campo de flores e abelhas que **saem para coletar néctar**, voltam, **produzem mel** e se reproduzem. Tudo funcionando em **turnos** que o computador executa sozinho.
:::

## Planejando o simulador

Antes de digitar, **desenhe** as peças (você já sabe: diagrama de classes).

::: figura Estrutura do simulador de colmeia
<svg viewBox="0 0 740 330" role="img" aria-label="World contém Hive, Flowers e Bees">
  <rect class="f-prim" x="250" y="10" width="240" height="70" rx="12"/><text x="370" y="38" text-anchor="middle" font-weight="800" class="t-prim">World</text><text x="370" y="60" text-anchor="middle" font-size="12" class="t-mono">Go()  •  Hive  •  Bees  •  Flowers</text>
  <path class="line" d="M370 80V110M110 110H630M110 110V130M370 110V130M630 110V130"/>
  <rect class="f-warn" x="10" y="130" width="200" height="110" rx="12"/><text x="110" y="158" text-anchor="middle" font-weight="800" class="t-warn">Hive (colmeia)</text><g class="t-mono" font-size="11"><text x="22" y="182">Honey, Honey</text><text x="22" y="200">AddHoney(), ConsumeHoney()</text><text x="22" y="218">AddBee(), Go()</text></g>
  <rect class="f-info" x="270" y="130" width="200" height="110" rx="12"/><text x="370" y="158" text-anchor="middle" font-weight="800" class="t-info">Bee (abelha)</text><g class="t-mono" font-size="11"><text x="282" y="182">Age, InsideHive, State</text><text x="282" y="200">Location, NectarCollected</text><text x="282" y="218">Go() — máquina de estados</text></g>
  <rect class="f-good" x="530" y="130" width="200" height="110" rx="12"/><text x="630" y="158" text-anchor="middle" font-weight="800" class="t-good">Flower (flor)</text><g class="t-mono" font-size="11"><text x="542" y="182">Age, Alive, Nectar</text><text x="542" y="200">Location, LifeSpan</text><text x="542" y="218">Go(), HarvestNectar()</text></g>
  <text x="20" y="285" font-size="13" class="t-dim">Um "turno" = World.Go() chama Go() em cada flor, em cada abelha e na colmeia.</text>
  <text x="20" y="305" font-size="13" class="t-dim">Cada classe cuida de si; o World só coordena.</text>
</svg>
:::

## Vida e morte de uma flor

Flores **nascem**, **envelhecem**, **produzem néctar** enquanto vivas e **morrem**. Em cada turno:

```cs
class Flower
{
    private const int LifeSpan = 15000;          // quanto tempo vive
    private const double NectarFactor = 0.001;   // quanto néctar produz por turno
    private const double InitialNectar = 1.5;

    public Point Location { get; private set; }
    public int Age { get; private set; }
    public bool Alive { get; private set; }
    public double Nectar { get; private set; }

    public Flower(Point location, Random random)
    {
        Location = location;
        Age = 0;
        Alive = true;
        Nectar = InitialNectar;
    }

    public double HarvestNectar(double amount)
    {
        if (amount > Nectar) amount = Nectar;     // só entrega o que tem
        Nectar -= amount;
        return amount;
    }

    public void Go()
    {
        Age++;
        if (Age > LifeSpan) { Alive = false; return; }
        if (Alive) Nectar += NectarFactor * Math.Min(1, Age / 5000.0);   // produz mais com a idade
    }
}
```

::: pense Um turno = um "tique" do relógio
Cada chamada de `Go()` representa **um instante** da simulação. Em vez de pensar em tempo real, você pensa em passos discretos: *"o que acontece neste instante?"*. Esse modelo (**baseado em turnos**) aparece em jogos, simulações e animações.
:::

## A abelha: uma máquina de estados

Uma abelha está sempre em **um** de vários **estados**: no ninho, saindo para o campo, coletando, voltando. O comportamento muda com o estado — e as **transições** entre eles seguem regras.

::: figura Estados da abelha
<svg viewBox="0 0 720 270" role="img" aria-label="Máquina de estados da abelha">
  <g text-anchor="middle" font-size="13">
    <rect class="f-warn" x="10" y="100" width="140" height="58" rx="29"/><text x="80" y="134" font-weight="800" class="t-warn">Idle</text>
    <rect class="f-info" x="200" y="20" width="150" height="58" rx="29"/><text x="275" y="54" font-weight="800" class="t-info">FlyingToFlower</text>
    <rect class="f-good" x="400" y="100" width="140" height="58" rx="29"/><text x="470" y="134" font-weight="800" class="t-good">Gathering</text>
    <rect class="f-prim" x="200" y="190" width="150" height="58" rx="29"/><text x="275" y="224" font-weight="800" class="t-prim">ReturningToHive</text>
    <rect class="f-plain" x="580" y="100" width="130" height="58" rx="29"/><text x="645" y="134" font-weight="800">Retired</text>
  </g>
  <path class="arrow" d="M120 100C150 70 170 55 198 50"/><text x="110" y="62" font-size="11" class="t-dim">flor viva existe</text>
  <path class="arrow" d="M352 50C400 60 440 85 460 98"/><text x="410" y="52" font-size="11" class="t-dim">chegou</text>
  <path class="arrow" d="M440 160C410 190 380 205 352 215"/><text x="395" y="205" font-size="11" class="t-dim">encheu</text>
  <path class="arrow" d="M200 218C150 205 110 185 90 160"/><text x="82" y="205" font-size="11" class="t-dim">entregou</text>
  <path class="arrow" d="M542 129H578"/><text x="560" y="120" font-size="11" class="t-dim">velha</text>
</svg>
:::

```cs
enum BeeState { Idle, FlyingToFlower, Gathering, ReturningToHive, Retired }

class Bee
{
    private const double HoneyConsumed = 0.5;
    private const int MoveRate = 3;
    private const double MinimumFlowerNectar = 1.5;
    private const int CareerSpan = 1000;

    public int Age { get; private set; }
    public bool InsideHive { get; private set; }
    public double NectarCollected { get; private set; }
    public Point Location { get; private set; }
    public BeeState CurrentState { get; private set; }

    private Hive hive;
    private World world;
    private Flower destinationFlower;

    public Bee(int id, Point location, Hive hive, World world)
    {
        this.hive = hive; this.world = world;
        Location = location; InsideHive = true; CurrentState = BeeState.Idle;
    }

    public void Go(Random random)
    {
        Age++;
        switch (CurrentState)
        {
            case BeeState.Idle:
                if (Age > CareerSpan) { CurrentState = BeeState.Retired; break; }
                if (world.Flowers.Count > 0 && hive.ConsumeHoney(HoneyConsumed))
                {
                    // escolhe uma flor viva com néctar suficiente
                    foreach (Flower f in world.Flowers)
                        if (f.Nectar >= MinimumFlowerNectar && f.Alive)
                        { destinationFlower = f; CurrentState = BeeState.FlyingToFlower; break; }
                }
                break;

            case BeeState.FlyingToFlower:
                if (!world.Flowers.Contains(destinationFlower)) { CurrentState = BeeState.ReturningToHive; break; }
                if (MoveToward(destinationFlower.Location)) CurrentState = BeeState.Gathering;
                break;

            case BeeState.Gathering:
                if (destinationFlower.Nectar >= 0.3 && destinationFlower.Alive)
                    NectarCollected += destinationFlower.HarvestNectar(0.3);
                else
                    CurrentState = BeeState.ReturningToHive;
                break;

            case BeeState.ReturningToHive:
                if (!InsideHive)
                {
                    if (MoveToward(hive.Location)) { InsideHive = true; }
                }
                else
                {
                    hive.AddHoney(NectarCollected);   // entrega o néctar (vira mel)
                    NectarCollected = 0;
                    CurrentState = BeeState.Idle;
                }
                break;
        }
    }

    // anda MoveRate pixels em direção ao destino; devolve true ao chegar
    private bool MoveToward(Point destination)
    {
        InsideHive = false;
        if (Math.Abs(destination.X - Location.X) <= MoveRate && Math.Abs(destination.Y - Location.Y) <= MoveRate)
            return true;
        int dx = Math.Sign(destination.X - Location.X) * Math.Min(MoveRate, Math.Abs(destination.X - Location.X));
        int dy = Math.Sign(destination.Y - Location.Y) * Math.Min(MoveRate, Math.Abs(destination.Y - Location.Y));
        Location = new Point(Location.X + dx, Location.Y + dy);
        return false;
    }
}
```

::: dica `switch` + `enum` = máquina de estados
É um padrão muito comum: guardar o estado atual num `enum` e, em cada turno, executar o `case` correspondente. Toda decisão de "o que fazer agora" fica **num lugar só**, e é fácil acrescentar um novo estado (basta um novo `case`).
:::

## A colmeia e o mundo

```cs
class Hive
{
    public Point Location { get; private set; }
    public double Honey { get; private set; }
    private List<Bee> bees = new List<Bee>();
    private World world;
    private Random random = new Random();

    public Hive(World world) { this.world = world; Honey = 3.2; Location = new Point(100, 100); }

    public void AddHoney(double nectar) { Honey += nectar * 0.25; }            // néctar → mel
    public bool ConsumeHoney(double amount)
    {
        if (Honey >= amount) { Honey -= amount; return true; }
        return false;                                                             // sem mel: abelha fica parada
    }

    public void AddBee(Bee b) { bees.Add(b); }

    public void Go()
    {
        foreach (Bee b in bees) b.Go(random);        // cada abelha faz o seu turno
        if (Honey > 4 && bees.Count < 8)              // com mel de sobra, nasce uma abelha
        {
            Honey -= 1;
            bees.Add(new Bee(bees.Count, Location, this, world));
        }
    }
}

class World
{
    public Hive Hive { get; private set; }
    public List<Flower> Flowers { get; private set; }
    private Random random = new Random();

    public World()
    {
        Flowers = new List<Flower>();
        Hive = new Hive(this);
        for (int i = 0; i < 10; i++) Flowers.Add(new Flower(new Point(random.Next(20, 400), random.Next(20, 300)), random));
    }

    public void Go()                         // UM TURNO DO MUNDO
    {
        Hive.Go();
        foreach (Flower f in Flowers) f.Go();
        // remove as flores mortas SEM alterar a lista durante o foreach
        Flowers.RemoveAll(f => !f.Alive);
        if (random.Next(500) == 0)           // de vez em quando brota uma flor nova
            Flowers.Add(new Flower(new Point(random.Next(20, 400), random.Next(20, 300)), random));
    }
}
```

::: cuidado Alterando a lista durante o laço
Se você remover flores **dentro** do `foreach` que percorre `Flowers`, lança `InvalidOperationException`. Aqui usamos `RemoveAll` com uma expressão lambda **depois** do laço — que é seguro e limpo.
:::

## Fazendo o mundo andar: o `Timer`

Um **`Timer`** do Windows Forms dispara o evento `Tick` periodicamente (você aprendeu eventos no capítulo anterior!). Ele **usa um delegado** nos bastidores: a assinatura de `timer1.Tick += ...` é a mesma que você já conhece.

```cs
public partial class Form1 : Form
{
    private World world = new World();
    private int framesRun = 0;

    public Form1()
    {
        InitializeComponent();
        timer1.Interval = 50;       // a cada 50 ms (~20 turnos por segundo)
        timer1.Tick += RunFrame;    // equivalente ao que o designer faz
        timer1.Start();
    }

    private void RunFrame(object sender, EventArgs e)
    {
        framesRun++;
        world.Go();                                       // um turno do mundo
        labelEstatisticas.Text =
            "Quadros: " + framesRun +
            "\nMel: " + world.Hive.Honey.ToString("0.0") +
            "\nFlores: " + world.Flowers.Count;
    }

    private void buttonPausar_Click(object sender, EventArgs e)
    {
        if (timer1.Enabled) timer1.Stop(); else timer1.Start();
    }
}
```

::: figura Quem chama quem, a cada Tick
<svg viewBox="0 0 720 120" role="img" aria-label="Timer, Form1, World, Hive, Bees">
  <g text-anchor="middle" font-size="13"><rect class="f-warn" x="5" y="30" width="110" height="52" rx="10"/><text x="60" y="62" font-weight="700" class="t-warn">Timer.Tick</text><rect class="f-prim" x="150" y="30" width="110" height="52" rx="10"/><text x="205" y="62" font-weight="700" class="t-prim">Form1</text><rect class="f-info" x="295" y="30" width="110" height="52" rx="10"/><text x="350" y="62" font-weight="700" class="t-info">World.Go()</text><rect class="f-good" x="440" y="30" width="110" height="52" rx="10"/><text x="495" y="62" font-weight="700" class="t-good">Hive.Go()</text><rect class="f-plain" x="585" y="30" width="125" height="52" rx="10"/><text x="647" y="62" font-weight="700">Bee.Go() ×N</text></g>
  <path class="arrow" d="M117 56H146"/><path class="arrow" d="M262 56H291"/><path class="arrow" d="M407 56H436"/><path class="arrow" d="M552 56H581"/>
</svg>
:::

## Trabalhando com grupos: uma prévia de LINQ

Já temos coleções (`List<Bee>`, `List<Flower>`). Quais perguntas fazer a elas?

- Quantas flores têm néctar acima de 2?
- Qual a flor mais velha?
- Qual a soma de néctar de todas as flores?

Com laços `foreach` isso dá **muito** código. O **LINQ** (*Language Integrated Query*) permite fazer essas consultas em **uma linha**:

```cs
using System.Linq;

int abundantes = world.Flowers.Count(f => f.Nectar > 2);
double totalNectar = world.Flowers.Sum(f => f.Nectar);
Flower maisVelha = world.Flowers.OrderByDescending(f => f.Age).FirstOrDefault();

var ricas = world.Flowers
                 .Where(f => f.Alive && f.Nectar > 1.5)
                 .OrderBy(f => f.Age)
                 .ToList();
```

::: regra Pense em "perguntas" sobre coleções
`Where` filtra, `OrderBy` ordena, `Select` transforma, `Count`/`Sum`/`Max` agregam. Você vai estudar tudo isso no **capítulo 15**; por enquanto, apenas note como código declarativo é mais claro que um `foreach` com `if` e acumuladores.
:::

::: exercicio Estenda o simulador
1. Mostre no rótulo também **quantas abelhas** estão fora da colmeia (use `Count` com LINQ).
2. Faça as flores **nascerem** com uma cor (`enum`).
3. Adicione uma classe `Queen : Bee` que **nunca** sai da colmeia mas **põe ovos** a cada N turnos (nasce uma `Bee`).

--- resposta
1. Exponha `IReadOnlyList<Bee> Bees` na colmeia e use `world.Hive.Bees.Count(b => !b.InsideHive)`.
2. `enum CorFlor { Vermelha, Amarela, Azul }` e uma propriedade `Cor` sorteada no construtor: `Cor = (CorFlor)random.Next(3);`.
3. `class Queen : Bee { override Go(...) }`: torne `Bee.Go` `virtual`; em `Queen`, `public override void Go(Random r) { Age++; if (Age % 200 == 0) hive.AddBee(new Bee(...)); }`.
:::

::: resumo
- Um sistema grande é feito de **classes pequenas e coesas** que cooperam (`World`, `Hive`, `Bee`, `Flower`).
- O modelo **por turnos**: cada objeto tem `Go()`; um coordenador chama todos.
- **`enum` + `switch`** implementam **máquinas de estados**.
- **`Timer`** dispara `Tick` (um evento) para animar a simulação.
- Não altere a coleção durante o `foreach`: use `RemoveAll` ou percorra uma cópia.
- **LINQ** consulta coleções de forma declarativa (`Where`, `OrderBy`, `Count`, `Sum`).
:::

::: quiz
? Em uma simulação baseada em turnos, o que representa o método `Go()` de cada objeto?
+ O que o objeto faz em um único instante (um turno)
- A inicialização do programa
- O encerramento do objeto
- A serialização
> O coordenador chama `Go()` em cada objeto a cada turno.

? Por que usar um `enum BeeState` e `switch`?
+ Para implementar uma máquina de estados clara e fácil de estender
- Porque `if` não existe no C#
- Para evitar usar classes
- Porque `switch` é o único que aceita `enum`
> Cada estado vira um `case`.

? Qual evento do `Timer` executa a cada intervalo?
- `Click`
+ `Tick`
- `Load`
- `Paint`
> `Tick` é disparado a cada `Interval` ms.

? Como remover itens de uma lista sem lançar exceção durante o percurso?
+ Usar `RemoveAll` (ou percorrer uma cópia)
- Remover dentro do `foreach` mesmo
- Reiniciar o programa
- Trocar a lista por uma matriz
> Alterar a coleção durante `foreach` causa `InvalidOperationException`.

? O que LINQ permite fazer com coleções?
+ Consultá-las (filtrar, ordenar, agregar) de forma declarativa
- Apenas ordená-las
- Salvá-las em disco
- Transformá-las em arquivos
> `Where`, `OrderBy`, `Select`, `Count`, `Sum`...

? Qual é a vantagem de o `World` apenas coordenar em vez de fazer tudo?
+ Cada classe cuida do próprio comportamento (coesão), simplificando a manutenção
- O programa fica mais rápido por magia
- Não precisa de classes
- Para esconder bugs
> Responsabilidades claras = código fácil de entender e mudar.
:::

::: cartas
Simulação por turnos :: Modelo em que o tempo avança em passos discretos; cada objeto executa `Go()` a cada passo.
Máquina de estados :: Objeto que está em um estado por vez e muda por transições (enum + switch).
Timer :: Componente que dispara `Tick` periodicamente.
RemoveAll :: Remove da lista todos os itens que satisfazem um predicado.
LINQ :: Linguagem integrada de consulta: `Where`, `OrderBy`, `Select`, `Count`, `Sum`...
Coesão :: Cada classe tem uma responsabilidade bem definida.
:::
