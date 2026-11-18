---
id: aprenda-cap07
order: 18
kind: capitulo
num: 7
group: C# · Parte 3 · Orientação a objetos
title: Interfaces e classes abstratas
subtitle: Fazendo as classes cumprirem suas promessas — contratos, `is`/`as`, upcast e downcast, modificadores de acesso e polimorfismo.
minutes: 65
hue: 150
objectives:
- Definir e implementar **interfaces** (contratos) e usar **referências de interface**
- Usar `is` e `as` e entender **upcast** × **downcast**
- Escolher o **modificador de acesso** certo (`public`, `private`, `protected`, `internal`)
- Criar **classes e métodos abstratos** e reconhecer os quatro pilares da POO
origem: livros/01-aprenda-csharp/content/08-cap07.md
---

::: ilustracao img/aprenda/cap07.svg | Interface: um contrato comum; qualquer aparelho que o cumpra pode ser ligado.
:::

## Ações importam mais que "quem você é"

Às vezes você quer agrupar objetos pelo que eles **sabem fazer**, e não pela família de onde vêm. Um pássaro, um avião e um drone **voam** — mas não herdam da mesma classe. É aí que entram as **interfaces**.

Uma interface é um **contrato**: *"quem se diz capaz de X **tem** de oferecer estes métodos e propriedades"*. E com grande poder vem grande responsabilidade: se a classe prometeu, o compilador a obriga a cumprir **tudo** — senão não compila.

::: vila De volta à colmeia
Nem toda abelha faz o mesmo: algumas coletam **néctar**, outras **picam** invasores, e algumas fazem as duas coisas. Herança de uma única classe base não dá conta: uma abelha precisa ser, ao mesmo tempo, "coletora de néctar" **e** "picadora". Com interfaces, cada abelha **declara** quais habilidades possui.
:::

## O que é uma interface

Uma **interface** diz à classe quais métodos e propriedades ela precisa **implementar**, mas **não** diz *como*. Por convenção, o nome começa com **`I`**.

```cs
interface IStingPatrol            // quem implementa PRECISA oferecer tudo isto
{
    int AlertLevel { get; }
    int StingerLength { get; set; }
    bool LookForEnemies();
    int SharpenStinger(int length);
}

interface INectarCollector
{
    void FindFlowers();
    void GatherNectar();
    void ReturnToHive();
}
```

Note: sem corpo `{ ... }`, sem `public`; cada membro termina com `;`.

### Implementando a interface

```cs
class NectarStinger : Worker, INectarCollector, IStingPatrol    // herda de UMA classe, implementa VÁRIAS interfaces
{
    public int AlertLevel { get; private set; }
    public int StingerLength { get; set; }

    public bool LookForEnemies() { /* ... */ return false; }
    public int SharpenStinger(int length) { StingerLength += length; return StingerLength; }

    public void FindFlowers() { /* ... */ }
    public void GatherNectar() { /* ... */ }
    public void ReturnToHive() { /* ... */ }
}
```

::: regra Uma classe base, muitas interfaces
Em C# uma classe pode herdar de **apenas uma** classe base, mas implementar **quantas interfaces** quiser — é assim que uma abelha pode ser "coletora de néctar" **e** "patrulheira" ao mesmo tempo. Se você esquecer **um** membro, o compilador acusa erro.
:::

### Interfaces não são instanciadas — mas têm referências

Você **não** pode fazer `new INectarCollector()`; uma interface não é um objeto, é apenas a descrição de um comportamento. Mas pode ter **variáveis** do tipo da interface, que apontam para **qualquer objeto** que a implemente:

```cs
INectarCollector coletora = new NectarStinger();   // a variável só "enxerga" o que a interface promete
coletora.FindFlowers();
coletora.GatherNectar();
// coletora.LookForEnemies();   // ERRO: não faz parte de INectarCollector
```

::: analogia O crachá
Você pode ter uma **variável de interface** como se fosse um **crachá**: "este objeto é *Coletor de Néctar*". Quem lê o crachá só sabe que ele **faz** aquilo — não se é abelha, robô ou qualquer outra coisa. Para o código que usa o crachá, isso é suficiente.
:::

Isso permite uma lista de "coisas que coletam néctar", misturando tipos diferentes:

```cs
List<INectarCollector> coletores = new List<INectarCollector>();
coletores.Add(new NectarStinger());
coletores.Add(new RoboBee4000());     // um ROBÔ que também implementa INectarCollector
foreach (INectarCollector c in coletores) c.GatherNectar();
```

::: curiosidade O RoboBee 4000
O robô de 4000 créditos faz o trabalho de uma operária **sem gastar mel**. Ele nem herda de `Bee` — só implementa `INectarCollector`. O código que dirige as coletoras **não se importa**: se promete cumprir o contrato, serve.
:::

### Interfaces podem herdar de outras

```cs
interface IPatrolStinger : IStingPatrol, INectarCollector { /* ... */ }
```

Quem implementa a interface "filha" precisa cumprir os membros de **todas** as "mães".

## `is` e `as`: descobrindo e tratando o tipo

Com uma variável de tipo genérico (a base ou uma interface), às vezes você precisa **perguntar** o que o objeto realmente é.

```cs
if (coletora is IStingPatrol)           // o objeto implementa IStingPatrol?
{
    IStingPatrol patrulha = (IStingPatrol) coletora;   // cast explícito
    patrulha.LookForEnemies();
}

IStingPatrol p2 = coletora as IStingPatrol;   // devolve null se NÃO for (sem exceção)
if (p2 != null) p2.SharpenStinger(2);
```

- **`is`** diz **se** o objeto implementa a interface ou herda da classe (verdadeiro/falso).
- **`as`** tenta **tratar** o objeto como aquele tipo e devolve **`null`** se não der.
- Um **cast** `(Tipo)obj` explode com `InvalidCastException` se for incompatível.

::: dica Padrão moderno
O C# atual combina teste e conversão: `if (coletora is IStingPatrol patrulha) { patrulha.LookForEnemies(); }`.
:::

### Upcast e downcast

Uma cafeteira **é um** eletrodoméstico. Isso dá dois movimentos:

::: figura Subir e descer na hierarquia
<svg viewBox="0 0 720 260" role="img" aria-label="Upcast e downcast">
  <g text-anchor="middle">
    <rect class="f-prim" x="260" y="15" width="200" height="50" rx="12"/><text x="360" y="46" font-weight="800" class="t-prim">Eletrodoméstico</text>
    <rect class="f-good" x="260" y="135" width="200" height="50" rx="12"/><text x="360" y="166" font-weight="800" class="t-good">Cafeteira</text>
    <path class="line" d="M360 135V68" marker-end="url(#arr2)"/>
  </g>
  <path class="arrow" d="M200 150C140 130 140 70 255 42"/><text x="90" y="105" font-size="13" font-weight="700" class="t-good">UPCAST</text><text x="40" y="123" font-size="12" class="t-dim">(automático e seguro)</text>
  <path class="arrow" d="M520 42C640 70 640 130 462 158" style="stroke:var(--warn)"/><text x="570" y="105" font-size="13" font-weight="700" class="t-warn">DOWNCAST</text><text x="545" y="123" font-size="12" class="t-dim">(exige cast / as)</text>
  <text x="360" y="235" text-anchor="middle" font-size="13" class="t-dim">toda cafeteira é um eletrodoméstico, mas nem todo eletrodoméstico é cafeteira</text>
</svg>
:::

```cs
Cafeteira c = new Cafeteira();
Eletrodomestico e = c;               // UPCAST: automático — Cafeteira É UM Eletrodoméstico
Cafeteira de_volta = (Cafeteira) e;   // DOWNCAST: tem de pedir; é válido porque o objeto real é Cafeteira
```

Funciona igual para **interfaces**: um objeto pode ser tratado como qualquer interface que implemente (upcast), e de volta ao tipo da classe (downcast) quando você precisar de membros que a interface não tem.

::: erro Downcast errado
`Torradeira t = (Torradeira) e;` com `e` apontando para uma `Cafeteira` lança `InvalidCastException` — o objeto real **não é** torradeira. Teste antes com `is` (ou use `as` e verifique `null`).
:::

## Modificadores de acesso (em detalhe)

O **escopo** de um membro define quem pode usá-lo. Você já viu `public` e `private`; há mais dois:

| Modificador | Visível para |
|---|---|
| `public` | Qualquer classe |
| `private` | Só a própria classe |
| `protected` | A própria classe **e suas subclasses** |
| `internal` | Qualquer classe **do mesmo projeto** (assembly) |
| `protected internal` | Subclasses **ou** mesmo projeto |

::: analogia Portas de uma casa
Pense em uma casa: o **quintal da frente** é `public` (qualquer visita entra), a **sala de estar** é `internal` (só quem foi convidado ao imóvel), o **quarto de família** é `protected` (só parentes) e o **cofre** do escritório é `private` (só você).
:::

## Classes abstratas: um meio-termo entre classe e interface

Algumas classes **nunca deveriam ser instanciadas**. Faz sentido criar um "animal" qualquer ou um "veículo" genérico? Não — só tipos concretos (lobo, carro).

Uma **classe abstrata** pode ter campos, propriedades e métodos **normais** (como uma classe), mas também **métodos abstratos** — sem corpo — que as subclasses são **obrigadas** a implementar (como uma interface).

```cs
abstract class Animal
{
    public bool EstaVivo { get; set; }
    public void Dormir() { /* implementação normal, herdada por todos */ }
    public abstract void FazerBarulho();    // sem corpo: cada subclasse DEVE implementar
}

class Cachorro : Animal
{
    public override void FazerBarulho() { Console.WriteLine("Au au!"); }
}

// Animal a = new Animal();   // ERRO: classe abstrata não pode ser instanciada
Animal b = new Cachorro();    // OK: a referência é do tipo abstrato, o objeto é concreto
```

::: pense Interface ou classe abstrata?
**Interface**: só contrato, nenhuma implementação (nem campos); uma classe pode implementar várias. **Classe abstrata**: contrato **e** código compartilhado; só se herda de uma. Pergunte: *"preciso compartilhar código ou só garantir que a classe cumpre um papel?"*.
:::

### Exemplo do mundo real: uma casa

Imagine um jogo de aventura com uma casa cheia de cômodos: sala de estar, cozinha, jardim, quintal. Todos são `Room` (**abstrata**: `Description` e `Exits` comuns), mas só os **concretos** (`RoomWithDoor`, `Outside`) podem ser criados. Alguns têm uma **decoração** (`Decoration`), outros não. Esse tipo de modelagem aparecerá no Laboratório 2.

## Polimorfismo: muitas formas, um só tipo

**Polimorfismo** significa que um objeto pode assumir **muitas formas diferentes**. Na prática: você escreve código para o tipo **geral** (classe base, abstrata ou interface) e ele funciona com **qualquer** subtipo.

```cs
Animal[] zoo = { new Cachorro(), new Lobo(), new Hipopotamo() };
foreach (Animal a in zoo)
    a.FazerBarulho();      // cada um faz o SEU barulho
```

Se amanhã você criar um `Gato`, esse laço continuará funcionando **sem nenhuma alteração**. Esse é o ganho: **adicionar tipos novos sem reescrever o que já existe**.

::: figura Os quatro pilares da orientação a objetos
<svg viewBox="0 0 720 200" role="img" aria-label="Herança, abstração, encapsulamento e polimorfismo">
  <g text-anchor="middle" font-size="14">
    <rect class="f-prim" x="10" y="20" width="165" height="150" rx="14"/><text x="92" y="52" font-weight="800" class="t-prim">Herança</text><text x="92" y="82" font-size="12" class="t-dim">reaproveitar e</text><text x="92" y="100" font-size="12" class="t-dim">estender uma</text><text x="92" y="118" font-size="12" class="t-dim">classe existente</text>
    <rect class="f-info" x="190" y="20" width="165" height="150" rx="14"/><text x="272" y="52" font-weight="800" class="t-info">Abstração</text><text x="272" y="82" font-size="12" class="t-dim">modelar só o</text><text x="272" y="100" font-size="12" class="t-dim">essencial (classes</text><text x="272" y="118" font-size="12" class="t-dim">abstratas, interfaces)</text>
    <rect class="f-good" x="370" y="20" width="165" height="150" rx="14"/><text x="452" y="52" font-weight="800" class="t-good">Encapsulamento</text><text x="452" y="82" font-size="12" class="t-dim">esconder detalhes</text><text x="452" y="100" font-size="12" class="t-dim">e proteger o</text><text x="452" y="118" font-size="12" class="t-dim">estado interno</text>
    <rect class="f-warn" x="550" y="20" width="160" height="150" rx="14"/><text x="630" y="52" font-weight="800" class="t-warn">Polimorfismo</text><text x="630" y="82" font-size="12" class="t-dim">um tipo, várias</text><text x="630" y="100" font-size="12" class="t-dim">formas de se</text><text x="630" y="118" font-size="12" class="t-dim">comportar</text>
  </g>
</svg>
:::

::: exercicio Interface, classe abstrata ou herança comum?
Para cada situação, diga o que usar e por quê:

1. `Pato`, `Aviao` e `Drone` precisam de `Voar()`, mas não são parentes.
2. `Circulo`, `Quadrado` e `Triangulo` calculam `Area()`, e todos têm uma `Cor`.
3. `Gerente` é um tipo especial de `Funcionario` com bônus.

--- resposta
1. **Interface** `IVoador`: capacidade comum entre tipos sem parentesco.
2. **Classe abstrata** `Forma`: compartilha `Cor` (código) e obriga cada subclasse a implementar `Area()` (método abstrato).
3. **Herança comum**: `Gerente : Funcionario` — relação "é um" simples, com `override` no cálculo do salário se necessário.
:::

::: resumo
- **Interface** = contrato sem implementação; a classe implementadora precisa cumprir **todos** os membros.
- Uma classe herda de **uma** classe base, mas implementa **várias** interfaces.
- Variáveis de **interface** referenciam qualquer objeto que a implemente.
- `is` testa o tipo; `as` converte ou devolve `null`; cast explícito pode lançar exceção.
- **Upcast** é automático; **downcast** exige cuidado.
- `public`, `private`, `protected`, `internal` definem o **escopo**.
- **Classe abstrata** não é instanciada e pode ter **métodos abstratos** obrigatórios.
- **Polimorfismo**: código escrito para o tipo geral funciona com todos os subtipos.
:::

::: quiz
? Qual é a convenção de nome para interfaces em C#?
- Terminar com `Interface`
+ Começar com `I` (ex.: `IStingPatrol`)
- Começar com `Abstract`
- Ser escritas em minúsculas
> É uma convenção universal no .NET.

? O que acontece se uma classe implementa uma interface mas esquece um dos membros?
+ Erro de compilação
- Funciona; o membro fica vazio
- Apenas um aviso em tempo de execução
- O membro é gerado automaticamente
> A classe precisa cumprir todo o contrato.

? É possível criar uma instância de uma interface com `new`?
- Sim, sempre
+ Não; só se pode criar instâncias de classes concretas que a implementam
- Sim, se for pública
- Só dentro de um laço
> Interfaces não têm implementação, então não podem ser instanciadas.

? O que `obj as IStingPatrol` devolve se `obj` não implementa a interface?
- Lança exceção
+ `null`
- `false`
- O próprio objeto
> `as` não lança exceção: devolve null quando a conversão não é possível.

? Qual modificador permite acesso à própria classe e às subclasses?
- `private`
+ `protected`
- `internal`
- `static`
> `protected` abre o membro para a hierarquia de herança.

? O que é um método `abstract`?
- Um método sem parâmetros
+ Um método sem corpo que as subclasses concretas devem implementar
- Um método estático da classe base
- Um método que não pode ser chamado
> Só existe em classes abstratas e obriga a implementação nas derivadas.

? Qual é a vantagem do polimorfismo?
+ Escrever código para o tipo geral e incluir tipos novos sem alterar o que existe
- Evitar o uso de classes
- Tornar todos os membros públicos
- Reduzir o número de variáveis
> Novos subtipos funcionam com o código antigo automaticamente.

? Uma classe pode herdar de quantas classes base em C#?
- Quantas quiser
+ Apenas uma (mas pode implementar várias interfaces)
- Nenhuma
- Duas
> C# tem herança simples de classes e múltipla implementação de interfaces.
:::

::: cartas
Interface :: Contrato: lista de métodos/propriedades que uma classe se compromete a implementar.
Implementar :: Fornecer o corpo de todos os membros exigidos pela interface.
Referência de interface :: Variável do tipo da interface; aponta para qualquer objeto que a implemente.
is :: Operador que testa se um objeto é de determinado tipo/implementa uma interface.
as :: Converte para o tipo ou devolve null, sem lançar exceção.
Upcast :: Tratar um objeto como tipo mais geral (automático).
Downcast :: Tratar um objeto de tipo geral como específico (exige cast/as).
protected :: Acessível pela classe e suas subclasses.
internal :: Acessível em todo o mesmo projeto (assembly).
Classe abstrata :: Classe que não pode ser instanciada; pode ter membros abstratos.
Método abstrato :: Declaração sem corpo que as subclasses concretas devem implementar.
Polimorfismo :: Capacidade de objetos de tipos diferentes responderem à mesma chamada à sua maneira.
Quatro pilares da POO :: Herança, abstração, encapsulamento e polimorfismo.
:::
