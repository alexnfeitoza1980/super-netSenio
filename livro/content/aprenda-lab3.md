---
id: aprenda-lab3
order: 28
kind: lab
num: 3
group: C# · Parte 6 · Gráficos, memória e LINQ
title: Laboratório 3 — Invasores
subtitle: Seu próprio jogo de arcade no estilo clássico: nave, tiros, ondas de invasores, pontuação — gráficos, eventos, coleções e LINQ juntos.
minutes: 150
hue: 180
badge: L3
objectives:
- Construir um **jogo de arcade completo** com laço de jogo, `Timer` e `Paint`
- Tratar **teclado** (eventos `KeyDown`/`KeyUp`) e **colisões**
- Organizar **invasores, tiros e a nave** em classes e coleções
- Usar **LINQ**, **enum** e **encapsulamento** em um projeto maior
origem: livros/01-aprenda-csharp/content/18-lab3.md
---

::: vila O pai de todos os videogames
Nesta homenagem ao gênero de arcade mais popular da história, você escreve um jogo de **naves contra invasores alienígenas**: uma nave embaixo, ondas de invasores descendo em formação, tiros para todos os lados. É hora de colocar em prática **tudo** que você aprendeu.
:::

::: ilustracao img/aprenda/lab3.svg | Invasores: um jogo clássico com objetos, eventos e gráficos juntos.
:::

Este é o projeto final. Ele **não** tem passos de "arraste isto aqui": só um **plano**, as **peças** (com código de referência) e **desafios**. Tente cada peça sozinho antes de abrir a resposta. Desenhamos formas simples (retângulos/polígonos) — você pode trocá-las por imagens depois.

## O que o jogo faz

- Uma **nave** na parte inferior se move **esquerda/direita** (setas) e **atira** (barra de espaço).
- Uma **formação** de invasores (fileiras × colunas) anda de lado; quando toca a borda, **desce** e **inverte** o sentido.
- Invasores **atiram** de vez em quando; se um tiro acerta a nave, perde-se **uma vida**.
- Um tiro da nave que acerta um invasor o **destrói** e dá **pontos** (fileiras de cima valem mais).
- Se todos os invasores morrem, começa a **próxima onda**, mais rápida. Se eles chegam à base ou as vidas acabam: **fim de jogo**.
- Na tela: **pontos**, **vidas**, **onda**, e uma **estrelinha** de fundo para dar clima.

::: figura Esboço da tela
<svg viewBox="0 0 560 360" role="img" aria-label="Tela do jogo: invasores, nave e tiros">
  <rect x="10" y="10" width="540" height="340" rx="10" fill="#0b0d1a"/>
  <g fill="#fff" opacity=".7"><circle cx="40" cy="40" r="1.5"/><circle cx="120" cy="150" r="1.5"/><circle cx="260" cy="70" r="1.5"/><circle cx="410" cy="190" r="1.5"/><circle cx="500" cy="60" r="1.5"/><circle cx="330" cy="290" r="1.5"/><circle cx="60" cy="300" r="1.5"/></g>
  <g fill="#7fdbca"><rect x="70" y="50" width="30" height="20" rx="4"/><rect x="130" y="50" width="30" height="20" rx="4"/><rect x="190" y="50" width="30" height="20" rx="4"/><rect x="250" y="50" width="30" height="20" rx="4"/><rect x="310" y="50" width="30" height="20" rx="4"/></g>
  <g fill="#f6c177"><rect x="70" y="90" width="30" height="20" rx="4"/><rect x="130" y="90" width="30" height="20" rx="4"/><rect x="190" y="90" width="30" height="20" rx="4"/><rect x="250" y="90" width="30" height="20" rx="4"/><rect x="310" y="90" width="30" height="20" rx="4"/></g>
  <g fill="#ff7b9c"><rect x="70" y="130" width="30" height="20" rx="4"/><rect x="130" y="130" width="30" height="20" rx="4"/><rect x="190" y="130" width="30" height="20" rx="4"/><rect x="250" y="130" width="30" height="20" rx="4"/><rect x="310" y="130" width="30" height="20" rx="4"/></g>
  <rect x="268" y="190" width="3" height="14" fill="#82aaff"/><rect x="132" y="170" width="3" height="12" fill="#ff7b9c"/>
  <polygon points="255,320 280,300 305,320" fill="#a58bff"/><rect x="262" y="320" width="36" height="8" fill="#a58bff"/>
  <text x="22" y="34" fill="#dfe3f5" font-size="14" font-family="monospace">PONTOS 0420</text><text x="420" y="34" fill="#dfe3f5" font-size="14" font-family="monospace">VIDAS ♥♥♥</text><text x="250" y="344" fill="#94a0c9" font-size="12" font-family="monospace">Onda 2</text>
</svg>
:::

## Planejamento: as classes

::: figura O design do jogo
<svg viewBox="0 0 740 330" role="img" aria-label="Game contém Ship, Invaders, Shots e Stars">
  <rect class="f-prim" x="270" y="10" width="200" height="60" rx="12"/><text x="370" y="38" text-anchor="middle" font-weight="800" class="t-prim">Game</text><text x="370" y="58" text-anchor="middle" font-size="12" class="t-mono">Go()  Draw()  OnKey()</text>
  <path class="line" d="M370 70V100M90 100H650M90 100V120M250 100V120M410 100V120M570 100V120"/>
  <rect class="f-info" x="10" y="120" width="160" height="100" rx="12"/><text x="90" y="148" text-anchor="middle" font-weight="800" class="t-info">PlayerShip</text><g class="t-mono" font-size="11"><text x="22" y="172">Location, Lives</text><text x="22" y="190">Move(dir), Shoot()</text></g>
  <rect class="f-good" x="180" y="120" width="160" height="100" rx="12"/><text x="260" y="148" text-anchor="middle" font-weight="800" class="t-good">Invader</text><g class="t-mono" font-size="11"><text x="192" y="172">Type, Location, Score</text><text x="192" y="190">Move(dir), Draw()</text></g>
  <rect class="f-warn" x="350" y="120" width="160" height="100" rx="12"/><text x="430" y="148" text-anchor="middle" font-weight="800" class="t-warn">Shot</text><g class="t-mono" font-size="11"><text x="362" y="172">Location, Direction</text><text x="362" y="190">Move(), Area</text></g>
  <rect class="f-plain" x="520" y="120" width="160" height="100" rx="12"/><text x="600" y="148" text-anchor="middle" font-weight="800">Star</text><g class="t-mono" font-size="11"><text x="532" y="172">Point, Pen</text><text x="532" y="190">Draw()</text></g>
  <text x="20" y="260" font-size="13" class="t-dim">Game guarda: List&lt;Invader&gt;, List&lt;Shot&gt; (da nave), List&lt;Shot&gt; (dos invasores), List&lt;Star&gt;.</text>
  <text x="20" y="282" font-size="13" class="t-dim">Direction e InvaderType são enums.</text>
</svg>
:::

```cs
enum Direction { Left, Right, Up, Down }
enum InvaderType { Bug, Satellite, Saucer }
```

::: pense Que objeto "sabe" fazer o quê?
`Invader` sabe se **desenhar** e se **mover**; `Game` sabe que, **quando algum invasor chega à borda**, a **formação inteira** desce. Esse conhecimento "de grupo" é do `Game`, não de cada invasor. Pensar em **quem tem a informação** ajuda a distribuir responsabilidades.
:::

## Peça 1 — Os invasores

```cs
class Invader
{
    public const int Width = 30, Height = 20;
    public Point Location { get; private set; }
    public InvaderType Type { get; private set; }
    public int Score { get; private set; }

    public Rectangle Area { get { return new Rectangle(Location, new Size(Width, Height)); } }

    public Invader(InvaderType type, Point location)
    {
        Type = type; Location = location;
        Score = type == InvaderType.Bug ? 10 : type == InvaderType.Satellite ? 20 : 40;
    }

    public void Move(Direction direction, int step)
    {
        switch (direction)
        {
            case Direction.Left:  Location = new Point(Location.X - step, Location.Y); break;
            case Direction.Right: Location = new Point(Location.X + step, Location.Y); break;
            case Direction.Down:  Location = new Point(Location.X, Location.Y + step); break;
        }
    }

    public void Draw(Graphics g)
    {
        Brush cor = Type == InvaderType.Bug ? Brushes.MediumAquamarine
                  : Type == InvaderType.Satellite ? Brushes.Gold : Brushes.HotPink;
        g.FillRectangle(cor, Area);
    }
}
```

`Area` devolve um `Rectangle` — ele será usado para detectar **colisões**.

## Peça 2 — Tiros e colisão

```cs
class Shot
{
    private const int Speed = 8;
    public Point Location { get; private set; }
    public Direction Direction { get; private set; }
    public Rectangle Area { get { return new Rectangle(Location.X, Location.Y, 3, 12); } }

    public Shot(Point location, Direction direction) { Location = location; Direction = direction; }

    public void Move()
    {
        Location = new Point(Location.X, Location.Y + (Direction == Direction.Up ? -Speed : Speed));
    }

    public void Draw(Graphics g, Brush b) { g.FillRectangle(b, Area); }
}
```

**Colisão** é só **interseção de retângulos**: `shot.Area.IntersectsWith(invader.Area)`.

## Peça 3 — A nave do jogador

```cs
class PlayerShip
{
    public const int Width = 36, Height = 24;
    public Point Location { get; private set; }
    public int Lives { get; set; }
    public Rectangle Area { get { return new Rectangle(Location, new Size(Width, Height)); } }

    private Rectangle bounds;
    public PlayerShip(Rectangle bounds)
    {
        this.bounds = bounds;
        Lives = 3;
        Location = new Point(bounds.Width / 2, bounds.Bottom - 40);
    }

    public void Move(Direction direction)
    {
        int step = 10;
        int x = Location.X + (direction == Direction.Left ? -step : step);
        x = Math.Max(bounds.Left, Math.Min(bounds.Right - Width, x));   // limita nas bordas
        Location = new Point(x, Location.Y);
    }

    public Shot Shoot() { return new Shot(new Point(Location.X + Width / 2, Location.Y - 10), Direction.Up); }

    public void Draw(Graphics g)
    {
        Point[] triangulo = { new Point(Location.X, Location.Y + Height),
                              new Point(Location.X + Width / 2, Location.Y),
                              new Point(Location.X + Width, Location.Y + Height) };
        g.FillPolygon(Brushes.MediumPurple, triangulo);
    }
}
```

## Peça 4 — O motor do jogo

```cs
class Game
{
    public int Score { get; private set; }
    public int Wave { get; private set; }
    public bool GameOver { get; private set; }

    public PlayerShip Ship { get; private set; }
    private List<Invader> invaders = new List<Invader>();
    private List<Shot> playerShots = new List<Shot>();
    private List<Shot> invaderShots = new List<Shot>();
    private Direction invaderDirection = Direction.Right;
    private Random random = new Random();
    private Rectangle bounds;
    private int frame;

    public Game(Rectangle bounds)
    {
        this.bounds = bounds;
        Ship = new PlayerShip(bounds);
        NextWave();
    }

    private void NextWave()
    {
        Wave++;
        invaders.Clear(); invaderShots.Clear();
        for (int row = 0; row < 5; row++)
            for (int col = 0; col < 8; col++)
            {
                InvaderType tipo = row == 0 ? InvaderType.Saucer : row < 3 ? InvaderType.Satellite : InvaderType.Bug;
                invaders.Add(new Invader(tipo, new Point(40 + col * 50, 40 + row * 36)));
            }
    }

    // um "quadro" do jogo, chamado pelo Timer
    public void Go()
    {
        if (GameOver) return;
        frame++;

        MoveShots();
        if (frame % Math.Max(3, 12 - Wave) == 0) MoveInvaders();     // ondas mais rápidas
        InvaderFire();
        CheckCollisions();

        if (invaders.Count == 0) NextWave();
        if (Ship.Lives <= 0 || invaders.Any(i => i.Area.Bottom >= Ship.Area.Top)) GameOver = true;
    }

    private void MoveShots()
    {
        foreach (Shot s in playerShots) s.Move();
        foreach (Shot s in invaderShots) s.Move();
        playerShots.RemoveAll(s => s.Area.Bottom < 0);
        invaderShots.RemoveAll(s => s.Area.Top > bounds.Bottom);
    }

    private void MoveInvaders()
    {
        bool tocouNaBorda =
            invaders.Any(i => i.Area.Left <= bounds.Left + 5 && invaderDirection == Direction.Left) ||
            invaders.Any(i => i.Area.Right >= bounds.Right - 5 && invaderDirection == Direction.Right);

        if (tocouNaBorda)
        {
            foreach (Invader i in invaders) i.Move(Direction.Down, 14);
            invaderDirection = invaderDirection == Direction.Left ? Direction.Right : Direction.Left;
        }
        else
            foreach (Invader i in invaders) i.Move(invaderDirection, 8);
    }

    private void InvaderFire()
    {
        if (invaders.Count == 0 || random.Next(40) != 0) return;
        // só quem está na frente da coluna pode atirar (o mais baixo de cada coluna)
        var atiradores = invaders.GroupBy(i => i.Location.X)
                                 .Select(g => g.OrderByDescending(i => i.Location.Y).First())
                                 .ToList();
        Invader escolhido = atiradores[random.Next(atiradores.Count)];
        invaderShots.Add(new Shot(new Point(escolhido.Area.Left + Invader.Width / 2, escolhido.Area.Bottom), Direction.Down));
    }

    private void CheckCollisions()
    {
        foreach (Shot s in playerShots.ToList())
        {
            Invader atingido = invaders.FirstOrDefault(i => i.Area.IntersectsWith(s.Area));
            if (atingido != null)
            {
                Score += atingido.Score;
                invaders.Remove(atingido);
                playerShots.Remove(s);
            }
        }
        foreach (Shot s in invaderShots.ToList())
            if (s.Area.IntersectsWith(Ship.Area)) { Ship.Lives--; invaderShots.Remove(s); }
    }

    public void FireShot()
    {
        if (playerShots.Count < 3) playerShots.Add(Ship.Shoot());     // no máximo 3 tiros na tela
    }

    public void Draw(Graphics g)
    {
        foreach (Invader i in invaders) i.Draw(g);
        foreach (Shot s in playerShots) s.Draw(g, Brushes.DeepSkyBlue);
        foreach (Shot s in invaderShots) s.Draw(g, Brushes.HotPink);
        Ship.Draw(g);
    }
}
```

::: dica Olhe como LINQ simplifica
`invaders.Any(...)`, `FirstOrDefault`, `GroupBy` + `OrderByDescending` (o invasor **mais baixo de cada coluna**), `RemoveAll`: sem LINQ cada um deles seria um laço aninhado. O jogo vira **declarativo**: *"há algum invasor que passou da borda?"*.
:::

## Peça 5 — O formulário: teclado, timer e `Paint`

```cs
public partial class Form1 : Form
{
    private Game game;
    private bool left, right;

    public Form1()
    {
        InitializeComponent();
        DoubleBuffered = true;                         // sem tremulação
        BackColor = Color.FromArgb(11, 13, 26);
        game = new Game(ClientRectangle);
        timer1.Interval = 20;                          // ~50 quadros por segundo
        timer1.Tick += gameTick;
        timer1.Start();
    }

    private void gameTick(object sender, EventArgs e)
    {
        if (left)  game.Ship.Move(Direction.Left);
        if (right) game.Ship.Move(Direction.Right);
        game.Go();
        Invalidate();                                  // pede o Paint
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Left)  left = true;
        if (e.KeyCode == Keys.Right) right = true;
        if (e.KeyCode == Keys.Space) game.FireShot();
        base.OnKeyDown(e);
    }

    protected override void OnKeyUp(KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Left)  left = false;
        if (e.KeyCode == Keys.Right) right = false;
        base.OnKeyUp(e);
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        game.Draw(e.Graphics);
        e.Graphics.DrawString("PONTOS " + game.Score + "   VIDAS " + game.Ship.Lives + "   ONDA " + game.Wave,
                              Font, Brushes.White, 10, 8);
        if (game.GameOver)
            e.Graphics.DrawString("FIM DE JOGO", new Font("Consolas", 32, FontStyle.Bold), Brushes.OrangeRed, ClientSize.Width / 2 - 120, ClientSize.Height / 2);
        base.OnPaint(e);
    }
}
```

::: ide Guardar o estado das teclas
Em vez de mover a nave dentro do `KeyDown` (que repete com atraso inicial), guardamos **se a tecla está pressionada** (`left`, `right`) e movemos no `Tick`. O movimento fica suave e consistente, independente do atraso de repetição do teclado.
:::

::: passos O que acontece em cada quadro (20 ms)
**1.** O `Timer` dispara `Tick`: `gameTick` é chamado.
===
**2.** O formulário lê o **estado das teclas** e move a nave.
===
**3.** `game.Go()`: move tiros → move invasores → invasores atiram → colisões → próxima onda / fim de jogo.
===
**4.** `Invalidate()` pede o repintar; o Windows dispara `OnPaint`, que **desenha tudo do zero** (buffer duplo evita tremor).
:::

## Desafios para ir além

- **Estrelas de fundo:** crie uma classe `Star` (posição, cor) e anime um *parallax* (estrelas descendo devagar).
- **Barreiras:** blocos que se desgastam quando recebem tiros.
- **OVNI bônus:** cruza o topo de vez em quando valendo 100–300 pontos.
- **Recorde:** guarde a maior pontuação em arquivo (capítulo 9) e mostre no início.
- **Sons e imagens:** troque retângulos por `Bitmap`s dos recursos; toque sons com `System.Media.SoundPlayer`.
- **Refatorar:** crie uma classe base `Sprite` com `Location`, `Area` e `Draw()` e herde `Invader`, `Shot` e `PlayerShip` dela (herança!).

::: exercicio Refatore com herança
`Invader`, `Shot` e `PlayerShip` têm `Location`, `Area` e `Draw`. Como você eliminaria essa duplicação?

--- resposta
Crie uma **classe abstrata** `Sprite`:
```cs
abstract class Sprite
{
    public Point Location { get; protected set; }
    public abstract Size Size { get; }
    public Rectangle Area { get { return new Rectangle(Location, Size); } }
    public abstract void Draw(Graphics g);
}
class Invader : Sprite { /* só o que é específico */ }
```
Com isso, `Game` pode ter uma `List<Sprite>` para desenhar tudo num único `foreach` — **polimorfismo** ajudando de novo.
:::

::: resumo
- O jogo é um **laço**: `Timer.Tick` → atualizar (`Go`) → pedir repintar (`Invalidate`) → `OnPaint`.
- **Colisão** = `Rectangle.IntersectsWith`.
- **Teclado**: guarde o estado das teclas em `KeyDown`/`KeyUp` e aja no `Tick`.
- **Coleções + LINQ** reduzem laços aninhados (`Any`, `FirstOrDefault`, `GroupBy`, `RemoveAll`).
- **Enums** (`Direction`, `InvaderType`), **encapsulamento** (`private set`) e **separação** entre lógica e tela organizam um projeto grande.
:::

::: quiz
? Qual a sequência típica de um laço de jogo em Windows Forms?
+ Timer.Tick → atualizar o estado → Invalidate() → OnPaint desenha
- Paint → Tick → Load
- Load → Close → Tick
- Click → Dispose → Paint
> O Timer move o jogo; o Paint apenas desenha o estado atual.

? Como detectar colisão entre um tiro e um invasor com retângulos?
+ `tiro.Area.IntersectsWith(invasor.Area)`
- Comparando os nomes dos objetos
- Com `==` entre as referências
- Usando `Random`
> `IntersectsWith` verifica se dois retângulos se sobrepõem.

? Por que guardar o estado das teclas (`left`, `right`) em vez de mover direto no `KeyDown`?
+ Para o movimento ficar suave e independente da repetição do teclado
- Porque `KeyDown` não funciona em jogos
- Para economizar memória
- Porque a nave é `static`
> O `Tick` aplica o movimento em intervalos regulares.

? Qual a função de `DoubleBuffered = true` no formulário do jogo?
+ Evitar a tremulação ao redesenhar rapidamente
- Dobrar a velocidade do jogo
- Salvar o jogo duas vezes
- Habilitar o teclado
> O quadro é montado fora da tela e copiado de uma vez.

? Por que `Game` (e não cada `Invader`) decide descer a formação ao tocar a borda?
+ Porque a decisão depende do grupo inteiro, e é o `Game` quem conhece todos
- Porque `Invader` não pode ter métodos
- Porque o LINQ exige isso
- Por acaso
> Responsabilidades devem ficar com quem tem a informação.
:::

::: cartas
Laço de jogo :: Ciclo contínuo: atualizar estado → desenhar → repetir.
Colisão :: Detectada com `Rectangle.IntersectsWith` entre as áreas dos objetos.
KeyDown / KeyUp :: Eventos de teclado: tecla pressionada / liberada.
Sprite :: Objeto gráfico do jogo com posição, área e desenho.
OnPaint :: Método que desenha o quadro atual do formulário.
Refatorar :: Reorganizar o código (ex.: extrair classe base) sem mudar o comportamento.
:::
