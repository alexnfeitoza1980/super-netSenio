---
id: aprenda-cap05
order: 16
kind: capitulo
num: 5
group: C# · Parte 3 · Orientação a objetos
title: Encapsulamento
subtitle: Mantenha o que é particular… particular. Campos privados, propriedades e construtores para proteger o estado dos seus objetos.
minutes: 50
hue: 150
objectives:
- Entender **por que** esconder dados dentro da classe (e o que dá errado sem isso)
- Usar `private`, `public` e **propriedades** (`get`/`set`, automáticas)
- Escrever **construtores** para que o objeto já nasça válido
- Reconhecer boas ideias de encapsulamento no mundo real
origem: livros/01-aprenda-csharp/content/06-cap05.md
---

::: ilustracao img/aprenda/cap05.svg | Encapsulamento: o mundo usa os botões públicos; o interior fica protegido.
:::

## Quando os objetos "xeretam" uns aos outros

Assim como você não quer que ninguém leia seu diário ou mexa na sua conta bancária, **um bom objeto não deixa outros objetos mexerem à vontade em seus campos**. O **encapsulamento** é a ideia de proteger os dados de uma classe e expor só o que é necessário, por meio de métodos e propriedades controlados.

::: vila Catarina e o orçamento de festas
Catarina é promotora de eventos. Ela precisa de um **estimador de custo**: dado o número de convidados, comida saudável ou não, e decoração normal ou chique, calcula o custo da festa. Cada opção é calculada separadamente. Você escreve a classe `PartyPlanner` — e aí começa a confusão.
:::

### O teste da Catarina

Suponha uma classe sem proteção alguma:

```cs
class Estimador
{
    public int NumeroDeConvidados;
    public bool Saudavel;
    public bool DecoracaoChique;
    public decimal CustoDecoracao;     // público: qualquer um pode mudar!
    public decimal CustoPorPessoa;
}
```

Catarina testa: 25 convidados, sem decoração chique... e o resultado está errado. Por quê? Porque **outro trecho do código** alterou `NumeroDeConvidados` depois de a decoração ser calculada, e o custo ficou **desatualizado**. É fácil usar o objeto de forma errada quando todos os campos são públicos.

::: analogia O caixa eletrônico
Você pode sacar dinheiro, mas **não pode abrir o cofre** do banco. O caixa expõe poucas operações (sacar, depositar, ver saldo) e esconde o resto. Se qualquer um pudesse mexer direto no cofre, ninguém confiaria no banco.
:::

## Encapsulamento: manter dados privados

**Encapsular** significa manter **privados** alguns dados da classe e dar acesso a eles por meio de membros **públicos**, que garantem as regras. O C# controla isso com **modificadores de acesso**:

| Modificador | Quem enxerga |
|---|---|
| `public` | Qualquer classe, em qualquer lugar |
| `private` | **Só** a própria classe (padrão para membros sem modificador) |
| `protected` | A classe e suas subclasses (capítulo 6) |
| `internal` | Qualquer classe do mesmo projeto (assembly) |

```cs
class Estimador
{
    public int NumeroDeConvidados;           // quem fala com a classe

    private decimal custoDecoracao;          // detalhe interno
    private decimal custoPorPessoa;

    public decimal CalcularCusto(bool saudavel)
    {
        // todas as regras ficam AQUI, num só lugar
        return custoDecoracao + custoPorPessoa * NumeroDeConvidados;
    }
}
```

Agora o código de fora **não consegue** escrever `estimador.custoDecoracao = 0`: o compilador recusa. Campos e métodos `private` só podem ser acessados de **dentro** da própria classe.

::: pense O campo "NomeVerdadeiro" fica REALMENTE protegido?
Um agente secreto guarda `private string nomeVerdadeiro`. Se um método público `public string Revelar(string senha)` devolver o nome só para a senha certa, o segredo está seguro. Mas se a senha for fraca... o encapsulamento só é **tão bom** quanto a lógica dos métodos públicos. **Private** impede o acesso direto — não substitui segurança de verdade (criptografia, autenticação).
:::

### Boas ideias de encapsulamento

- Comece com tudo **`private`** e abra (`public`) só o que for realmente necessário.
- Pergunte: *"o que pode dar errado se alguém mudar este valor?"*
- Valide os valores em **um** lugar (o método/propriedade), não em todos os chamadores.
- Pense na classe como uma **caixa-preta** com botões e luzes; o motor fica escondido.

## Propriedades: encapsulamento sem sofrimento

Criar um método `GetX()` e `SetX()` para cada campo funciona, mas é trabalhoso. O C# tem **propriedades**: parecem campos para quem usa, mas executam código ao serem lidas (`get`) ou escritas (`set`).

```cs
class Farmer
{
    private int bagsOfFeed;                 // campo privado (o "dado real")

    public int BagsOfFeed                   // propriedade pública
    {
        get { return bagsOfFeed; }
        set
        {
            bagsOfFeed = value;             // 'value' é o que foi atribuído
            // recalcula tudo que depende deste valor:
            FeedCost = bagsOfFeed * 30m;
        }
    }

    public decimal FeedCost { get; private set; }   // qualquer um lê; só a classe escreve
}
```

Uso:

```cs
Farmer f = new Farmer();
f.BagsOfFeed = 15;            // chama o set (value = 15)
Console.WriteLine(f.FeedCost); // chama o get
// f.FeedCost = 0;             // ERRO: o set é privado
```

::: regra A propriedade é o "porteiro" do campo
O campo é o morador; a propriedade é o porteiro que decide quem entra, valida e avisa quem precisa saber. Dentro de `set`, `value` é a palavra reservada para **o valor atribuído**.
:::

### Propriedades automáticas

Quando não há lógica extra, o C# gera o campo escondido por você:

```cs
public string Name { get; set; }          // leitura e escrita
public int Id { get; private set; }       // só a classe altera
public bool Ativo { get; }                // só leitura (atribuível no construtor)
```

::: dica Propriedade ou campo público?
Prefira **sempre** propriedades para expor dados. Parecem iguais ao usar, mas permitem **mudar a implementação depois** (validação, cálculo, notificação) **sem alterar quem usa a classe**. Com um campo público essa porta se fecha.
:::

::: exercicio Crie a classe Farmer
Escreva uma classe `Farmer` com:

- uma propriedade `NumberOfCows` (número de vacas), que qualquer um pode ler e alterar;
- uma propriedade `FeedMultiplier` (sacos de ração por vaca) que qualquer um pode ler, mas **só o construtor** define;
- uma propriedade somente leitura `BagsOfFeed`, calculada como `NumberOfCows * FeedMultiplier`.

Depois responda: por que `BagsOfFeed` não deve ser um campo com valor guardado?

--- resposta
```cs
class Farmer
{
    public int FeedMultiplier { get; private set; }
    public int NumberOfCows { get; set; }
    public int BagsOfFeed { get { return NumberOfCows * FeedMultiplier; } }

    public Farmer(int numberOfCows, int feedMultiplier)
    {
        NumberOfCows = numberOfCows;
        FeedMultiplier = feedMultiplier;
    }
}
```
`BagsOfFeed` **sempre** reflete o estado atual (não há valor guardado que fique velho). O multiplicador só pode ser definido no construtor, então ninguém o muda por engano — as regras estão **dentro** da classe.
:::

## Construtores: o objeto já nasce certo

Um **construtor** é um método especial, com o **mesmo nome da classe** e **sem tipo de retorno**, chamado automaticamente quando você usa `new`. Serve para **inicializar** os campos privados e garantir que o objeto nunca exista "pela metade".

```cs
class Farmer
{
    public int NumberOfCows { get; set; }
    public int FeedMultiplier { get; private set; }

    public Farmer(int numberOfCows, int feedMultiplier)   // construtor
    {
        NumberOfCows = numberOfCows;
        FeedMultiplier = feedMultiplier;
    }
}

// uso:
Farmer joao = new Farmer(15, 30);   // obrigatoriamente com os dois valores
// Farmer erro = new Farmer();      // ERRO: não existe construtor sem parâmetros
```

::: passos O que `new Farmer(15, 30)` faz
**1)** O `new` reserva espaço no heap para o objeto `Farmer`.

::: html
<svg viewBox="0 0 560 110"><rect class="f-plain" stroke-dasharray="6 5" x="140" y="10" width="280" height="90" rx="12"/><text x="280" y="62" text-anchor="middle" class="t-dim" font-size="14">objeto Farmer (campos vazios)</text></svg>
:::
===
**2)** O **construtor** é executado: copia os argumentos para os campos.

::: html
<svg viewBox="0 0 560 110"><rect class="f-info" x="140" y="10" width="280" height="90" rx="12"/><text x="280" y="42" text-anchor="middle" font-weight="800" class="t-info" font-size="14">objeto Farmer</text><text x="280" y="66" text-anchor="middle" class="t-mono" font-size="13">NumberOfCows = 15</text><text x="280" y="86" text-anchor="middle" class="t-mono" font-size="13">FeedMultiplier = 30</text></svg>
:::
===
**3)** O `new` devolve a **referência** e ela é guardada na variável `joao`.

::: html
<svg viewBox="0 0 560 110"><rect class="f-prim" x="10" y="30" width="110" height="40" rx="9"/><text x="65" y="56" text-anchor="middle" class="t-mono" font-size="14">joao</text><rect class="f-info" x="300" y="10" width="250" height="90" rx="12"/><text x="425" y="42" text-anchor="middle" font-weight="800" class="t-info" font-size="14">objeto Farmer</text><text x="425" y="66" text-anchor="middle" class="t-mono" font-size="13">NumberOfCows = 15</text><text x="425" y="86" text-anchor="middle" class="t-mono" font-size="13">FeedMultiplier = 30</text><path class="arrow" d="M122 50C200 50 240 55 296 55"/></svg>
:::
:::

::: curiosidade Se você não escrever construtor...
O C# cria um **construtor padrão** vazio (sem parâmetros). Assim que você escrever **qualquer** construtor, o padrão deixa de existir — por isso `new Farmer()` quebra no exemplo acima. Quer ambos? Escreva os dois (**sobrecarga**, vista no capítulo 8).
:::

### Construtores + this

```cs
public Farmer(int numberOfCows) : this(numberOfCows, 30) { }   // reaproveita outro construtor
```

`: this(...)` chama outro construtor da mesma classe: evita repetir código.

## De volta à Catarina: o estimador encapsulado

Com tudo junto, a classe da Catarina fica protegida e simples de usar:

```cs
class PartyPlanner
{
    private const decimal ComidaPorPessoa = 25m;
    private const decimal TaxaDecoracaoChique = 50m;
    private const decimal DecoracaoChiquePorPessoa = 15m;
    private const decimal DecoracaoNormalPorPessoa = 7.5m;
    private const decimal TaxaDecoracaoNormal = 30m;

    private int numeroDeConvidados;
    private bool decoracaoChique;

    public int NumeroDeConvidados
    {
        get { return numeroDeConvidados; }
        set { numeroDeConvidados = value; }          // aqui dá para validar value > 0
    }

    public bool DecoracaoChique { get { return decoracaoChique; } set { decoracaoChique = value; } }

    public decimal CalcularCusto()
    {
        decimal custoDecoracao = decoracaoChique
            ? numeroDeConvidados * DecoracaoChiquePorPessoa + TaxaDecoracaoChique
            : numeroDeConvidados * DecoracaoNormalPorPessoa + TaxaDecoracaoNormal;
        return numeroDeConvidados * ComidaPorPessoa + custoDecoracao;
    }
}
```

Note a vantagem: **o custo é calculado na hora**, a partir dos valores atuais. Não existe campo "custo" que possa ficar desatualizado — o bug original da Catarina desaparece.

::: erro Encapsular de mentira
Declarar `private int x;` e depois criar `public int X { get; set; }` **sem nenhuma regra** protege pouco — é o mesmo que um campo público (embora permita adicionar regras depois). O valor real do encapsulamento vem das **regras** que você coloca nos métodos e propriedades.
:::

::: resumo
- **Encapsulamento** = esconder os dados (`private`) e controlar o acesso por membros públicos.
- Sem ele, é fácil usar o objeto de forma errada e deixar valores inconsistentes.
- **Propriedades** (`get`/`set`, automáticas ou não) são a forma idiomática de expor dados.
- `private set` permite leitura pública e escrita só interna.
- **Construtores** (mesmo nome da classe, sem retorno) garantem que o objeto nasça válido.
- Prefira **calcular** valores derivados a **armazená-los** — assim nunca ficam velhos.
:::

::: quiz
? O que significa `private` em um campo?
- Qualquer classe pode ler, mas não escrever
+ Só a própria classe pode acessar o campo
- Só subclasses podem acessar
- O campo é apagado depois do uso
> `private` restringe o acesso ao interior da classe.

? Qual a principal vantagem do encapsulamento?
+ Proteger o estado interno e manter as regras num único lugar
- Fazer o programa rodar mais rápido
- Evitar o uso de classes
- Eliminar a necessidade de testar
> Evita estados inválidos e facilita mudanças futuras.

? Em `set { bagsOfFeed = value; }`, o que é `value`?
- O valor atual do campo
+ O valor que foi atribuído à propriedade
- Uma variável global
- O valor padrão do tipo
> `value` é o valor do lado direito da atribuição `obj.Prop = valor`.

? O que faz `public decimal Custo { get; private set; }`?
+ Qualquer classe lê `Custo`; só a própria classe pode alterá-lo
- Só a própria classe lê e escreve
- Ninguém pode ler
- Torna `Custo` estático
> O modificador no acessor restringe quem pode escrever.

? Como se reconhece um construtor?
- Tem tipo de retorno `void`
+ Tem o mesmo nome da classe e nenhum tipo de retorno
- É sempre `static`
- Chama-se `Main`
> O construtor é executado automaticamente ao usar `new`.

? Se você escreve um construtor com parâmetros e nenhum sem parâmetros, o que acontece com `new MinhaClasse()`?
- Funciona com valores padrão
+ Não compila, pois o construtor padrão deixou de existir
- Chama o construtor com valores null
- O programa pergunta os valores
> O construtor padrão só é gerado automaticamente quando não há nenhum construtor.

? Por que é melhor **calcular** um valor derivado em vez de guardá-lo em um campo?
+ Porque o valor guardado pode ficar desatualizado se os dados de origem mudarem
- Porque campos consomem mais memória que métodos
- Porque o C# proíbe campos
- Porque é obrigatório usar `static`
> Calcular na hora garante consistência com o estado atual.
:::

::: cartas
Encapsulamento :: Esconder detalhes internos e expor só o necessário, por meio de membros controlados.
private :: Visível apenas dentro da própria classe.
public :: Visível para qualquer código.
Propriedade :: Membro que parece um campo, mas usa get/set com código.
Propriedade automática :: `{ get; set; }` — o compilador cria o campo oculto.
value :: Palavra reservada dentro do set: o valor atribuído.
Construtor :: Método especial, mesmo nome da classe, que inicializa o objeto no `new`.
Construtor padrão :: Sem parâmetros; só existe se você não escrever outro construtor.
const :: Valor fixo definido em tempo de compilação.
Caixa-preta :: Ideia de usar uma classe sem conhecer sua implementação interna.
:::
