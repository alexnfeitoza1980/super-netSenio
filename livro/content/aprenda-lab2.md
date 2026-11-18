---
id: aprenda-lab2
order: 20
kind: lab
num: 2
group: C# · Parte 4 · Coleções e arquivos
title: Laboratório 2 — A Missão
subtitle: Um jogo de aventura por turnos, com sete níveis, inimigos, armas e poções — herança, interfaces, coleções e polimorfismo em ação.
minutes: 120
hue: 30
badge: L2
objectives:
- Planejar um projeto grande com **hierarquia de classes** e **classe abstrata**
- Aplicar **polimorfismo** (inimigos e armas diferentes tratados do mesmo jeito)
- Usar **`List<T>`** para inimigos e armas, e `enum` para direções
- Separar o **motor do jogo** (lógica) do **formulário** (interface)
origem: livros/01-aprenda-csharp/content/10-lab2.md
---

::: vila O aventureiro
Um aventureiro importante está numa missão: derrotar inimigos perigosíssimos, **nível após nível**. O jogo é **por turnos**: o jogador faz uma ação (mover ou atacar) e **em seguida cada inimigo** faz a sua. Continua até o jogador derrotar todos os inimigos dos **sete níveis**… ou morrer.
:::

::: ilustracao img/aprenda/lab2.svg | A missão: um jogo de aventura montado com classes, coleções e arquivos.
:::

Este laboratório junta herança, interfaces, coleções e polimorfismo. Ele é grande de propósito: a melhor forma de aprender **design** é ter que organizar muitas peças. Faça com calma — e **desenhe antes de codificar**.

## A especificação

- O jogo é uma **masmorra** (um formulário). A cada nível, o jogador começa num canto e há de **0 a 3 inimigos**, **uma arma** no chão e, às vezes, uma **poção** para curar.
- O jogador **se move** (cima, baixo, esquerda, direita) e **ataca** (4 direções) usando a **arma equipada**.
- **Cada inimigo** se move em direção ao jogador depois de cada turno do jogador, **atacando** se chegar perto.
- O jogador **recolhe** as armas ao passar por cima delas e as **troca** pelos botões da tela.
- Cada nível tem inimigos diferentes (morcego, fantasma, carniçal), com **vida** e **comportamento** próprios.
- Ao matar todos os inimigos do nível, o jogador avança. Ao completar o sétimo, **vence**.

::: figura Visão do jogo
<svg viewBox="0 0 720 300" role="img" aria-label="Esboço da masmorra">
  <rect class="f-plain" x="10" y="10" width="440" height="280" rx="12"/>
  <g stroke="var(--line)"><path d="M10 80H450M10 150H450M10 220H450M80 10V290M150 10V290M220 10V290M290 10V290M360 10V290"/></g>
  <text x="40" y="52" font-size="30">🧙</text><text x="250" y="120" font-size="30">🦇</text><text x="380" y="190" font-size="30">👻</text><text x="110" y="260" font-size="30">🗡️</text><text x="320" y="262" font-size="30">🧪</text>
  <rect class="f-prim" x="470" y="10" width="240" height="280" rx="12"/>
  <g font-size="14"><text x="490" y="42" font-weight="800" class="t-prim">Jogador</text><text x="490" y="66" class="t-mono" font-size="13">Vida: 10   Nível: 3</text><text x="490" y="106" font-weight="800">Inimigos</text><text x="490" y="128" class="t-mono" font-size="12">Morcego 5   Fantasma 8</text><text x="490" y="168" font-weight="800">Armas</text><text x="490" y="190" class="t-mono" font-size="12">[Espada]  [Arco]  [Maça]</text><text x="490" y="230" font-weight="800">Mover / Atacar</text><text x="490" y="254" class="t-mono" font-size="12">↑ ↓ ← →   |   ⚔ ⚔ ⚔ ⚔</text></g>
</svg>
:::

## O design: peças e relações

A primeira pergunta de design é **o que é comum**. O jogador e os inimigos **se movem num mapa**, têm **posição** e **vida**. As armas têm **alcance** e **dano**. Logo, duas hierarquias:

::: figura Duas hierarquias de classes
<svg viewBox="0 0 740 330" role="img" aria-label="Mover com Player e Enemy; Weapon com Sword, Bow, Mace e Potion">
  <g text-anchor="middle">
    <rect class="f-prim" x="40" y="10" width="190" height="46" rx="12"/><text x="135" y="40" font-weight="800" class="t-prim">Mover (abstrata)</text>
    <text x="135" y="74" font-size="11" class="t-dim">Location, Move(), Nearby()</text>
    <path class="line" d="M135 82V104M65 104H255M65 104V122M255 104V122"/>
    <rect class="f-info" x="10" y="122" width="110" height="42" rx="10"/><text x="65" y="148" font-weight="700" class="t-info">Player</text>
    <rect class="f-warn" x="190" y="122" width="130" height="42" rx="10"/><text x="255" y="148" font-weight="700" class="t-warn">Enemy (abstr.)</text>
    <path class="line" d="M255 164V186M175 186H335M175 186V204M255 186V204M335 186V204"/>
    <rect class="f-plain" x="130" y="204" width="90" height="38" rx="9"/><text x="175" y="228">Bat</text>
    <rect class="f-plain" x="210" y="204" width="90" height="38" rx="9"/><text x="255" y="228">Ghost</text>
    <rect class="f-plain" x="290" y="204" width="90" height="38" rx="9"/><text x="335" y="228">Ghoul</text>
  </g>
  <g text-anchor="middle">
    <rect class="f-good" x="490" y="10" width="200" height="46" rx="12"/><text x="590" y="40" font-weight="800" class="t-good">Weapon (abstrata)</text>
    <text x="590" y="74" font-size="11" class="t-dim">Attack(direction, random)</text>
    <path class="line" d="M590 82V104M530 104H680M530 104V122M590 104V122M680 104V122"/>
    <rect class="f-plain" x="485" y="122" width="90" height="38" rx="9"/><text x="530" y="146">Sword</text>
    <rect class="f-plain" x="555" y="170" width="70" height="38" rx="9" opacity="0"/>
    <rect class="f-plain" x="575" y="122" width="70" height="38" rx="9"/><text x="610" y="146">Bow</text>
    <rect class="f-plain" x="650" y="122" width="80" height="38" rx="9"/><text x="690" y="146">Mace</text>
    <rect class="f-prim" x="520" y="200" width="140" height="40" rx="10"/><text x="590" y="225" font-size="13" class="t-prim">IPotion (interface)</text>
  </g>
  <text x="20" y="290" font-size="13" class="t-dim">Armas e poções coletáveis: uma poção "cura"; uma arma "ataca". As duas coisas aparecem na tela como itens no chão.</text>
</svg>
:::

::: pense Por que `Weapon` é abstrata?
Não existe "arma genérica" no mundo: existe espada, arco, maça. A classe abstrata guarda o que é **comum** (posição, se foi pega, como sinalizar) e **exige** que cada arma defina o ataque (`abstract void Attack(...)`). Já a poção cura, não ataca — uma **interface** `IPotion` com `Used` e `Use()` descreve esse papel.
:::

## Peça 1 — Direções com `enum`

```cs
enum Direction { Up, Down, Left, Right }
```

Qualquer método que pergunta "para onde?" recebe `Direction`: impossível passar `"cima"` escrito errado.

## Peça 2 — A base de quem se move

```cs
abstract class Mover
{
    protected Game game;
    public Point Location { get; protected set; }
    public const int MoveInterval = 10;     // distância de cada passo, em pixels

    public Mover(Game game, Point location)
    {
        this.game = game;
        Location = location;
    }

    public bool Nearby(Point other, int distance)
    {
        return Math.Abs(Location.X - other.X) < distance &&
               Math.Abs(Location.Y - other.Y) < distance;
    }

    // devolve o ponto resultante, SEM sair dos limites da arena
    public Point Move(Direction direction, Rectangle boundaries)
    {
        Point novo = Location;
        switch (direction)
        {
            case Direction.Up:    if (novo.Y - MoveInterval >= boundaries.Top)    novo.Y -= MoveInterval; break;
            case Direction.Down:  if (novo.Y + MoveInterval <= boundaries.Bottom) novo.Y += MoveInterval; break;
            case Direction.Left:  if (novo.X - MoveInterval >= boundaries.Left)   novo.X -= MoveInterval; break;
            case Direction.Right: if (novo.X + MoveInterval <= boundaries.Right)  novo.X += MoveInterval; break;
        }
        return novo;
    }
}
```

`protected` deixa `Player` e `Enemy` usarem `game` e alterarem `Location`, mas ninguém de fora.

## Peça 3 — Inimigos: um comportamento, muitas formas

```cs
abstract class Enemy : Mover
{
    private const int NearPlayerDistance = 25;
    public int HitPoints { get; private set; }
    public bool Dead { get { return HitPoints <= 0; } }

    public Enemy(Game game, Point location, int hitPoints) : base(game, location)
    {
        HitPoints = hitPoints;
    }

    public abstract void Move(Random random);      // cada inimigo se move do SEU jeito

    public void Hit(int maxDamage, Random random) { HitPoints -= random.Next(1, maxDamage + 1); }

    protected bool NearPlayer() { return Nearby(game.PlayerLocation, NearPlayerDistance); }

    protected Direction FindPlayerDirection(Point playerLocation)
    {
        Direction d;
        if (playerLocation.X > Location.X + MoveInterval) d = Direction.Right;
        else if (playerLocation.X < Location.X - MoveInterval) d = Direction.Left;
        else if (playerLocation.Y < Location.Y - MoveInterval) d = Direction.Up;
        else d = Direction.Down;
        return d;
    }
}

class Bat : Enemy
{
    public Bat(Game game, Point location) : base(game, location, 6) { }

    public override void Move(Random random)
    {
        if (Dead) return;
        if (random.Next(2) == 0)                         // 50%: caça o jogador
            Location = Move(FindPlayerDirection(game.PlayerLocation), game.Boundaries);
        else                                             // 50%: voa à toa
            Location = Move((Direction) random.Next(4), game.Boundaries);
        if (NearPlayer()) game.HitPlayer(2, random);     // morcego fere pouco
    }
}
```

Os outros inimigos seguem a mesma ideia, mudando só `Move`:

| Inimigo | Vida | Como se move | Dano ao tocar |
|---|---|---|---|
| `Bat` | 6 | 50% caça, 50% aleatório | até 2 |
| `Ghost` | 8 | 1 em 3 turnos persegue; senão fica parado | até 3 |
| `Ghoul` | 10 | 2 em 3 turnos persegue; senão fica parado | até 4 |

## Peça 4 — Armas: abstrata + polimorfismo

```cs
abstract class Weapon : Mover
{
    public bool PickedUp { get; private set; }
    public Weapon(Game game, Point location) : base(game, location) { }
    public void PickUpWeapon() { PickedUp = true; }

    public abstract string Name { get; }
    public abstract void Attack(Direction direction, Random random);

    // utilitário comum: acerta o primeiro inimigo que estiver na direção do ataque, dentro do alcance
    protected bool DamageEnemy(Direction direction, int radius, int damage, Random random)
    {
        Point p = game.PlayerLocation;
        foreach (Enemy inimigo in game.Enemies)
        {
            int dx = inimigo.Location.X - p.X;
            int dy = inimigo.Location.Y - p.Y;
            bool acerta =
                (direction == Direction.Right && dx > 0 && dx <= radius && Math.Abs(dy) < radius / 2) ||
                (direction == Direction.Left  && dx < 0 && -dx <= radius && Math.Abs(dy) < radius / 2) ||
                (direction == Direction.Down  && dy > 0 && dy <= radius && Math.Abs(dx) < radius / 2) ||
                (direction == Direction.Up    && dy < 0 && -dy <= radius && Math.Abs(dx) < radius / 2);
            if (acerta) { inimigo.Hit(damage, random); return true; }
        }
        return false;
    }
}

class Sword : Weapon
{
    public Sword(Game game, Point location) : base(game, location) { }
    public override string Name { get { return "Espada"; } }
    public override void Attack(Direction direction, Random random)
    {
        // alcance curto (40 px) e dano alto (até 3)
        DamageEnemy(direction, 40, 3, random);
    }
}
```

Em `Bow` o raio é **maior** e o dano **menor**; a `Mace` (maça) acerta **todas** as direções em volta. Tudo que o jogo precisa é de `weapon.Attack(direction, random)` — **qual** arma é, ele não precisa saber. É o polimorfismo trabalhando para você.

### Poções com interface

```cs
interface IPotion
{
    bool Used { get; }
    void Use(Game game);
}

class BluePotion : Mover, IPotion
{
    public BluePotion(Game game, Point location) : base(game, location) { }
    public bool Used { get; private set; }
    public void Use(Game game) { game.IncreasePlayerHealth(5, game.Random); Used = true; }
}
```

## Peça 5 — O motor do jogo

```cs
class Game
{
    public List<Enemy> Enemies { get; private set; }
    public Weapon WeaponInRoom { get; private set; }
    public Player Player { get; private set; }
    public Point PlayerLocation { get { return Player.Location; } }
    public Rectangle Boundaries { get; private set; }
    public int Level { get; private set; }
    public Random Random { get; private set; }

    public Game(Rectangle boundaries)
    {
        Boundaries = boundaries;
        Random = new Random();
        Player = new Player(this, new Point(boundaries.Left + 10, boundaries.Top + 70));
        NewLevel(Random);
    }

    public void Move(Direction direction, Random random)
    {
        Player.Move(direction);
        foreach (Enemy e in Enemies) e.Move(random);     // POLIMORFISMO: cada um do seu jeito
    }

    public void Attack(Direction direction, Random random)
    {
        Player.Attack(direction, random);
        foreach (Enemy e in Enemies) e.Move(random);
    }

    private void NewLevel(Random random) { /* cria inimigos e itens conforme o nível */ }
    public void HitPlayer(int maxDamage, Random random) { Player.Hit(maxDamage, random); }
    public void IncreasePlayerHealth(int health, Random random) { Player.IncreaseHealth(health, random); }
}
```

Observe a divisão de responsabilidades: `Game` **coordena**, os objetos **sabem fazer o próprio trabalho**, e o `Form1` só desenha o resultado e repassa os cliques.

::: dica O formulário só repassa
```cs
private void moveUp_Click(object sender, EventArgs e) { game.Move(Direction.Up, random); UpdateCharacters(); }
```
`UpdateCharacters()` posiciona os `PictureBox` conforme as `Location` dos objetos e mostra/esconde os que morreram. A **lógica** nunca fica no botão.
:::

## Níveis: uma tabela de balanceamento

| Nível | Inimigos | Item no chão |
|---|---|---|
| 1 | 1 morcego | Espada |
| 2 | 1 fantasma, 1 morcego | Poção azul |
| 3 | 1 morcego, 1 carniçal | Arco |
| 4 | 2 morcegos, 1 fantasma | Poção azul |
| 5 | 1 fantasma, 1 carniçal | Maça |
| 6 | 3 morcegos, 1 carniçal | Poção vermelha |
| 7 | 2 fantasmas, 2 carniçais | — |

::: exercicio Teste seu raciocínio
1. Por que `Enemy.Move(Random)` é `abstract` e `Enemy.Hit` não?
2. Se você criar um `Dragon : Enemy`, o que precisa mudar no `Game`?
3. Por que `Game` guarda uma `List<Enemy>` e não uma lista separada para cada tipo de inimigo?

--- resposta
1. Cada inimigo **se move de forma diferente** (obrigatoriamente) mas **todos** perdem vida do mesmo jeito: comportamento comum fica na base, o que varia é abstrato.
2. **Nada**: o `Game` trata todos como `Enemy`. Basta criar o `Dragon` e adicioná-lo à lista ao montar o nível.
3. Com `List<Enemy>` o laço `foreach (Enemy e in Enemies) e.Move(...)` funciona para qualquer tipo, atual ou futuro. Listas separadas obrigariam a mudar o laço sempre que surgisse um tipo novo.
:::

## Desafios extras

- Faça o **jogador** perder vida por inimigos que o tocam e exiba "Game over" ao chegar a zero.
- Adicione um inimigo **`Dragon`** que se move duas vezes por turno.
- Crie um **`IWeapon`** além da classe `Weapon` e compare os dois designs.
- Guarde o **recorde** de níveis (capítulo 9) em arquivo.

::: resumo
- Projetos grandes começam pelo **design**: o que é comum vira **classe base/abstrata**; o que varia vira **membro abstrato/virtual**.
- **Polimorfismo** deixa o motor do jogo tratar `Enemy` e `Weapon` sem saber o tipo concreto.
- **Interfaces** (`IPotion`) descrevem papéis que não cabem na hierarquia.
- **Coleções** (`List<Enemy>`) e **enums** (`Direction`) dão clareza e segurança.
- Separe **lógica** (`Game` e objetos) da **interface** (`Form1`).
:::

::: quiz
? Por que `Mover` é declarada `abstract`?
+ Porque nunca existe um "mover genérico": só jogador, inimigos e itens concretos
- Para impedir que `Player` herde dela
- Porque classes abstratas são mais rápidas
- Porque não tem métodos
> A base reúne o que é comum e não faz sentido instanciar.

? Qual a vantagem de `foreach (Enemy e in Enemies) e.Move(random)` no motor?
+ Funciona para qualquer subtipo de `Enemy` sem alterações
- Executa mais rápido que um `for`
- Evita o uso de `List`
- Impede inimigos de se moverem simultaneamente
> Polimorfismo: a versão correta de `Move` é escolhida em tempo de execução.

? Por que usar `enum Direction` em vez de strings?
+ Evita erros de digitação e documenta os valores válidos
- Permite mais direções
- É obrigatório em jogos
- Consome menos memória que qualquer outra coisa
> O compilador valida o enum.

? Por que `IPotion` é uma interface e não uma classe base como `Weapon`?
+ Porque é um papel (algo que pode ser usado) que um objeto de outra hierarquia pode assumir
- Porque interfaces são mais seguras
- Porque poções não existem no jogo
- Porque não pode haver mais de uma classe base
> Interfaces definem capacidades independentes da hierarquia.

? Onde deve ficar a lógica do jogo (movimentos, ataques)?
- Nos eventos dos botões do formulário
+ Nas classes de lógica (`Game`, `Player`, `Enemy`...), com o formulário apenas desenhando
- No arquivo `Program.cs`
- Em variáveis globais estáticas
> Separar lógica e interface facilita testes e mudanças.
:::

::: cartas
Classe abstrata :: Base não instanciável que reúne o que é comum e exige membros abstratos.
Polimorfismo :: Tratar subtipos diferentes pela base, executando o comportamento de cada um.
Motor de jogo (Game) :: Classe que coordena o estado e as regras, sem lidar com a tela.
Turnos :: O jogador age e, em seguida, cada inimigo age.
Separação lógica × interface :: A interface só mostra e repassa eventos; as regras ficam nas classes.
:::
