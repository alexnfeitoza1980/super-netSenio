---
id: agil-cap29
order: 45
kind: capitulo
num: 29
group: C# · Parte 10 · Padrões de projeto (I)
title: Command e Active Object
subtitle: Uma interface com um método e uma versatilidade enorme — controle de dispositivos, transações, desfazer/refazer e um núcleo multitarefa cooperativo.
minutes: 55
hue: 320
objectives:
- Entender o padrão **Command** e por que "uma função vira objeto" é útil
- Usar Command para **ligar eventos a ações**, criar **transações** e implementar **Undo**
- Perceber o **desacoplamento físico e temporal** que o padrão traz
- Construir um **Active Object** simples (tarefas cooperativas *run-to-completion*)
origem: livros/02-projeto-agil-csharp/content/44-cap29.md
---

::: ilustracao img/agil/cap29.svg | Command: o pedido vira objeto (a comanda) que pode esperar, ser repetido ou desfeito.
:::

## Um padrão quase risível de tão simples

Entre todos os padrões de projeto, **Command** é um dos mais simples e elegantes. E a simplicidade engana: a variedade de usos é praticamente ilimitada.

::: figura O padrão Command
<svg viewBox="0 0 640 170" role="img" aria-label="Interface Command com Execute; várias implementações">
  <rect class="f-info" x="220" y="10" width="200" height="56" rx="10"/><text x="320" y="34" text-anchor="middle" font-size="11" class="t-dim">«interface»</text><text x="320" y="52" text-anchor="middle" font-weight="800" class="t-info">ICommand</text>
  <path class="line" d="M320 68V92M90 92H550M90 92V112M230 92V112M410 92V112M550 92V112" style="marker-end:none"/>
  <g text-anchor="middle" font-size="12"><rect class="f-plain" x="20" y="112" width="140" height="42" rx="8"/><text x="90" y="138">LigarReleCommand</text><rect class="f-plain" x="165" y="112" width="130" height="42" rx="8"/><text x="230" y="138">DesligarMotor</text><rect class="f-plain" x="340" y="112" width="140" height="42" rx="8"/><text x="410" y="138">CadastrarCliente</text><rect class="f-plain" x="490" y="112" width="130" height="42" rx="8"/><text x="555" y="138">DesenharCirculo</text></g>
</svg>
:::

```cs
public interface ICommand
{
    void Execute();
}
```

Repare no que isso tem de curioso: **a maioria das classes associa um conjunto de métodos a um conjunto de variáveis**. `ICommand` **encapsula uma única função** — e as implementações guardam apenas o necessário para executá-la. É quase uma "decomposição funcional" elevada ao nível de classe (uma heresia para puristas!). Mas nessa fronteira entre dois paradigmas, **coisas interessantes acontecem**.

::: regra A ideia central
Command **transforma uma ação em um objeto**. Um objeto pode ser **guardado, passado, adiado, enfileirado, registrado em log, desfeito** — e **executado por quem não sabe** o que ele faz.
:::

*(Observação do livro: o autor não prefixa interfaces com `I` — argumenta que o nome não deve ser poluído por um detalhe que pode mudar. A convenção .NET usa `I`; aqui usamos `ICommand` por consistência com o restante do livro.)*

## Uso 1 — Comandos simples: ligando sensores a atuadores

Imagine o software embarcado de uma máquina com **relés, motores e engates** controlados por **sensores**. Crie uma hierarquia de comandos:

```cs
public class LigarRele : ICommand
{
    private readonly int endereco;
    public LigarRele(int endereco) { this.endereco = endereco; }
    public void Execute() { Hardware.Rele(endereco, true); }
}

public class DesligarMotor : ICommand
{
    private readonly int endereco;
    public DesligarMotor(int endereco) { this.endereco = endereco; }
    public void Execute() { Hardware.Motor(endereco, false); }
}
```

O **endereço** do relé/motor vai no **construtor**. Agora o **sensor** não precisa saber nada de relés:

```cs
public class SensorOptico
{
    private readonly ICommand aoDetectar;
    public SensorOptico(ICommand aoDetectar) { this.aoDetectar = aoDetectar; }

    public void Evento() { aoDetectar.Execute(); }     // só "dispara" — não sabe o que isso faz
}

// a "fiação" fica concentrada num único lugar de inicialização:
var sensor = new SensorOptico(new LigarRele(endereco: 7));
```

Vantagens:

- O **sensor ignora** a mecânica do equipamento: sua função fica muito mais simples.
- A complexidade de **quem liga o quê** vai para a **inicialização** — toda a "fiação" lógica num lugar só.
- Dá até para ler essa fiação de um **arquivo de configuração** e montar o sistema sem recompilar.

Ou seja: **desacoplamos as interligações lógicas do sistema dos dispositivos conectados**.

::: dica Em C# moderno: delegados
Quando o comando é só "executar uma ação", um `Action` (ou lambda) faz o mesmo papel sem criar classes: `new SensorOptico(() => Hardware.Rele(7, true))`. Use **classes** de comando quando quiser **dados** (para validar, registrar, serializar) e **comportamento extra** (desfazer, descrever).
:::

## Uso 2 — Transações

O segundo uso comum: **criar e executar transações**. Imagine um sistema que mantém um banco de **funcionários**. O usuário pode **adicionar**, **excluir** ou **alterar** funcionários. Antes de atuar nos dados, o sistema precisa **validá-los** (sintática e semanticamente).

O comando serve como **repositório dos dados ainda não validados**, implementa **`Validar()`** e implementa **`Execute()`**:

```cs
public interface ITransacao
{
    void Validar();      // lança exceção se algo estiver errado
    void Executar();     // aplica no banco (só depois de validar)
}

public class AdicionarFuncionario : ITransacao
{
    private readonly string nome;
    private readonly string endereco;
    private readonly decimal salario;
    private readonly IRepositorio repositorio;

    public AdicionarFuncionario(string nome, string endereco, decimal salario, IRepositorio repositorio)
    { this.nome = nome; this.endereco = endereco; this.salario = salario; this.repositorio = repositorio; }

    public void Validar()
    {
        if (string.IsNullOrWhiteSpace(nome)) throw new ArgumentException("Nome é obrigatório");
        if (salario <= 0) throw new ArgumentException("Salário inválido");
        if (repositorio.Existe(nome)) throw new InvalidOperationException("Funcionário já existe");
    }

    public void Executar() { repositorio.Adicionar(new Funcionario(nome, endereco, salario)); }
}
```

### Desacoplamento físico

O código que **coleta os dados** (uma caixa de diálogo, uma API REST, um arquivo) fica **separado** do que **valida e executa**. Se a validação estivesse na tela, ninguém mais a reutilizaria (na API, no processamento em lote). Ao colocá-la na transação, ela serve a **qualquer interface**. Também separamos a **logística do banco** das **próprias entidades de negócio**.

### Desacoplamento temporal

Uma vez coletados os dados, **não há motivo** para validar/executar **imediatamente**. As transações podem ficar numa **lista** e rodar **mais tarde**.

::: analogia A manutenção de meia-noite
Um banco de dados só pode ser alterado entre meia-noite e 1h. Em vez de acordar e digitar todos os comandos às pressas, o operador digita e **valida durante o dia**; as transações validadas ficam numa fila e **executam à meia-noite**. Command permite exatamente isso.
:::

```cs
var fila = new List<ITransacao>();
fila.Add(new AdicionarFuncionario("Ana", "Rua A, 1", 4200m, repo));
fila.Add(new AdicionarFuncionario("Beto", "Rua B, 2", 3900m, repo));

fila.ForEach(t => t.Validar());      // durante o dia: valida e rejeita erros cedo

// à meia-noite:
foreach (var t in fila) t.Executar();
```

## Uso 3 — Desfazer (Undo)

Acrescente um segundo método à interface:

```cs
public interface IComandoDesfazivel
{
    void Execute();
    void Undo();
}
```

Se `Execute()` **lembra os detalhes** do que fez, `Undo()` pode **reverter**. Num editor de desenho:

```cs
public class DesenharCirculo : IComandoDesfazivel
{
    private readonly Tela tela;
    private readonly Point centro;
    private readonly int raio;
    private Circulo desenhado;                         // lembrado para o Undo

    public DesenharCirculo(Tela tela, Point centro, int raio) { this.tela = tela; this.centro = centro; this.raio = raio; }

    public void Execute() { desenhado = new Circulo(centro, raio); tela.Adicionar(desenhado); }
    public void Undo()    { tela.Remover(desenhado); }
}

public class Historico
{
    private readonly Stack<IComandoDesfazivel> feitos = new Stack<IComandoDesfazivel>();
    private readonly Stack<IComandoDesfazivel> desfeitos = new Stack<IComandoDesfazivel>();

    public void Executar(IComandoDesfazivel c) { c.Execute(); feitos.Push(c); desfeitos.Clear(); }
    public void Desfazer() { if (feitos.Count == 0) return; var c = feitos.Pop(); c.Undo(); desfeitos.Push(c); }
    public void Refazer()  { if (desfeitos.Count == 0) return; var c = desfeitos.Pop(); c.Execute(); feitos.Push(c); }
}
```

::: passos O ciclo Executar / Desfazer / Refazer
**Clicar em "Círculo"**: o sistema cria um `DesenharCirculo` e o **executa**; depois o coloca na **pilha de feitos**.
===
**Outro comando**: `DesenharQuadrado` é executado e empilhado. A pilha agora tem [Círculo, Quadrado].
===
**Desfazer (Ctrl+Z)**: o sistema **desempilha** o último comando (Quadrado), chama `Undo()` e o move para a pilha de **desfeitos**.
===
**Refazer (Ctrl+Y)**: o comando volta da pilha de desfeitos, `Execute()` roda de novo e é reempilhado em feitos.
:::

O código que sabe **como desfazer** fica **ao lado** do que sabe **como executar**. Com essa técnica, implementa-se Undo em praticamente qualquer aplicativo.

## Uso 4 — Active Object: um núcleo multitarefa de brinquedo

Um dos usos prediletos do autor para Command é o padrão **Active Object**, uma técnica antiga de obter **múltiplas linhas de controle** com um **núcleo multitarefa simplíssimo** — usada em milhares de sistemas industriais.

A ideia é minúscula: um **motor** mantém uma **fila de comandos**. Dá para **adicionar** comandos e chamar `Rodar()`, que **executa e remove** cada comando da fila até esvaziá-la.

```cs
public class MotorDeTarefas
{
    private readonly Queue<ICommand> fila = new Queue<ICommand>();

    public void Adicionar(ICommand c) { fila.Enqueue(c); }

    public void Rodar()
    {
        while (fila.Count > 0)
        {
            ICommand c = fila.Dequeue();
            c.Execute();                   // roda ATÉ O FIM; depois vem o próximo
        }
    }
}
```

Parece pouco. Mas e se **um comando se recolocasse na fila**? Então a fila nunca esvaziaria e `Rodar()` nunca retornaria — e os comandos ficariam **se revezando**: **multitarefa**.

### O comando que "dorme" sem bloquear

```cs
public class Aguardar : ICommand
{
    private readonly TimeSpan duracao;
    private readonly MotorDeTarefas motor;
    private readonly ICommand depois;
    private DateTime? inicio;

    public Aguardar(TimeSpan duracao, MotorDeTarefas motor, ICommand depois)
    { this.duracao = duracao; this.motor = motor; this.depois = depois; }

    public void Execute()
    {
        if (inicio == null) { inicio = DateTime.Now; motor.Adicionar(this); return; }      // primeira vez: marca e se reagenda
        if (DateTime.Now - inicio.Value < duracao) motor.Adicionar(this);                  // ainda não deu: volta para a fila
        else motor.Adicionar(depois);                                                      // deu o tempo: agenda o que vem depois
    }
}
```

Compare com uma thread tradicional: ao esperar um evento, ela faz uma chamada ao sistema operacional que **bloqueia** a thread até o evento. `Aguardar` **não bloqueia**: se o evento (o tempo passar) ainda não ocorreu, **ele se coloca de volta na fila**.

### Digitadores concorrentes

```cs
public class Digitador : ICommand
{
    private static bool parar;
    private readonly char letra;
    private readonly TimeSpan intervalo;
    private readonly MotorDeTarefas motor;

    public Digitador(char letra, TimeSpan intervalo, MotorDeTarefas motor)
    { this.letra = letra; this.intervalo = intervalo; this.motor = motor; }

    public void Execute()
    {
        Console.Write(letra);
        if (!parar) motor.Adicionar(new Aguardar(intervalo, motor, this));      // "dorme" e depois volta a ser executado
    }

    public static void Parar() { parar = true; }
}

public class Parador : ICommand { public void Execute() { Digitador.Parar(); } }

public static void Main()
{
    var motor = new MotorDeTarefas();
    motor.Adicionar(new Digitador('1', TimeSpan.FromMilliseconds(100), motor));
    motor.Adicionar(new Digitador('3', TimeSpan.FromMilliseconds(300), motor));
    motor.Adicionar(new Digitador('5', TimeSpan.FromMilliseconds(500), motor));
    motor.Adicionar(new Aguardar(TimeSpan.FromSeconds(5), motor, new Parador()));   // depois de 5 s, manda parar
    motor.Rodar();
}
```

```saida
1351131151351131...
```

Cada `Digitador` digita sua letra e **dorme** (sem bloquear) por seu intervalo; o `1` aparece mais que o `3` e o `3` mais que o `5`. Rodando de novo, a sequência **varia ligeiramente** (o relógio do computador e o da CPU não estão em perfeito sincronismo) — o **comportamento não determinístico** que é a marca registrada de sistemas multitarefa… e fonte de boa parte da angústia de depurá-los.

::: figura O motor em ação
<svg viewBox="0 0 700 190" role="img" aria-label="Fila de comandos circulando: Digitador1, Aguardar, Digitador3...">
  <rect class="f-prim" x="10" y="60" width="130" height="60" rx="12"/><text x="75" y="95" text-anchor="middle" font-weight="800">Rodar()</text>
  <g text-anchor="middle" font-size="12"><rect class="f-good" x="200" y="20" width="110" height="36" rx="6"/><text x="255" y="43">Digitador '1'</text><rect class="f-warn" x="320" y="20" width="110" height="36" rx="6"/><text x="375" y="43">Aguardar(100)</text><rect class="f-good" x="440" y="20" width="110" height="36" rx="6"/><text x="495" y="43">Digitador '3'</text><rect class="f-warn" x="560" y="20" width="110" height="36" rx="6"/><text x="615" y="43">Aguardar(300)</text></g>
  <text x="435" y="80" text-anchor="middle" font-size="12" class="t-dim">fila de comandos (cada um roda até o fim e, se precisar, se reinsere)</text>
  <path class="arrow" d="M142 80L198 44"/><path class="arrow" d="M672 40C690 100 400 150 140 110" stroke-dasharray="5 4"/><text x="420" y="150" text-anchor="middle" font-size="12" class="t-dim">quem não terminou volta ao fim da fila</text>
</svg>
:::

### Run-To-Completion

Tarefas assim são chamadas **RTC (*run-to-completion*)**: cada comando **roda até o fim** antes do próximo começar e **nunca bloqueia**. Há uma vantagem sutil: todas compartilham a **mesma pilha de execução** — não é preciso alocar uma pilha por "thread". Em sistemas de **memória restrita** com muitas tarefas, é uma vantagem poderosa.

::: cuidado Cooperação, não preempção
Um comando que **demora muito** (ou entra em laço infinito) **trava todo o sistema**, pois ninguém o interrompe. Em troca, não há **condições de corrida** entre comandos (só um roda por vez). É o modelo dos antigos "loops de eventos" e, em essência, do que `async`/`await` faz com um *scheduler* cooperativo.
:::

::: dica Hoje, sem reinventar
Em C# atual, as ideias do Active Object aparecem em `Task`, `Channel<T>`, `BlockingCollection<T>` (fila de comandos consumida por uma thread), `System.Threading.Timer` e na fila de mensagens da UI (`Dispatcher`/`SynchronizationContext`). O padrão ajuda a **entender** o que essas peças fazem.
:::

::: exercicio Mini-editor com desfazer
Implemente um editor de texto de **uma linha** com comandos `Inserir(texto)` e `Apagar(quantidade)`, ambos **desfazíveis**, e um `Historico` com `Desfazer()`/`Refazer()`.

--- resposta
```cs
public class Documento { public string Texto = ""; }

public class Inserir : IComandoDesfazivel
{
    private readonly Documento doc; private readonly string texto;
    public Inserir(Documento doc, string texto) { this.doc = doc; this.texto = texto; }
    public void Execute() { doc.Texto += texto; }
    public void Undo()    { doc.Texto = doc.Texto.Substring(0, doc.Texto.Length - texto.Length); }
}

public class Apagar : IComandoDesfazivel
{
    private readonly Documento doc; private readonly int quantidade; private string removido = "";
    public Apagar(Documento doc, int quantidade) { this.doc = doc; this.quantidade = quantidade; }
    public void Execute()
    {
        int n = Math.Min(quantidade, doc.Texto.Length);
        removido = doc.Texto.Substring(doc.Texto.Length - n);      // lembra o que apagou
        doc.Texto = doc.Texto.Substring(0, doc.Texto.Length - n);
    }
    public void Undo() { doc.Texto += removido; }
}

var h = new Historico(); var d = new Documento();
h.Executar(new Inserir(d, "Olá, mundo"));   // "Olá, mundo"
h.Executar(new Apagar(d, 5));               // "Olá, "
h.Desfazer();                                // "Olá, mundo"
h.Refazer();                                 // "Olá, "
```
Cada comando **guarda o que precisa para reverter** (o `Apagar` lembra o trecho removido).
:::

::: resumo
- **Command:** interface com um método (`Execute`) — a ação vira **objeto**.
- Usos: **ligar eventos a ações** (desacoplando sensor e atuador, com a "fiação" na inicialização), **transações** (validar/executar), **fila/adiamento**, **desfazer/refazer**.
- Traz **desacoplamento físico** (coleta × validação × execução) e **temporal** (executar depois).
- **Active Object:** fila de comandos + comandos que **se reinserem** = multitarefa cooperativa **sem bloqueio** (*run-to-completion*, pilha compartilhada).
- Em C# atual: `Action`/lambda, `Task`, filas e canais fazem o mesmo papel.
:::

::: quiz
? O que o padrão Command encapsula?
+ Uma ação (função) como objeto
- Um conjunto de variáveis
- Uma conexão de banco
- Uma thread
> Uma interface com um único método (Execute).

? Qual a vantagem de ligar um sensor a um Command em vez de a um relé específico?
+ O sensor não precisa conhecer o hardware; a fiação fica concentrada na inicialização
- O sensor fica mais lento
- O relé deixa de existir
- Evita testes
> Desacopla as interligações lógicas dos dispositivos.

? O que é "desacoplamento temporal" no uso de Command como transação?
+ Validar/executar mais tarde, independentemente de quando os dados foram coletados
- Executar sempre imediatamente
- Executar em outro computador
- Ignorar a validação
> Transações podem ficar numa fila e rodar quando for conveniente.

? O que um comando precisa fazer para suportar Undo?
+ Lembrar, ao executar, os detalhes que permitem reverter a operação
- Chamar o garbage collector
- Rodar em outra thread
- Ser estático
> O Undo usa o estado guardado pelo Execute.

? O que é uma tarefa RTC (run-to-completion)?
+ Um comando que roda até o fim sem bloquear antes de o próximo começar
- Uma thread preemptiva
- Uma tarefa que nunca termina
- Uma tarefa de banco de dados
> Compartilham a mesma pilha de execução.

? No Active Object, como um comando "dorme" sem bloquear?
+ Reinsere-se na fila até o tempo passar
- Chama Thread.Sleep
- Cria uma nova thread
- Fecha o motor
> Se o evento ainda não ocorreu, volta à fila.

? Qual é uma desvantagem do modelo cooperativo?
+ Um comando demorado trava todos os outros
- Há muitas condições de corrida
- Exige muita memória por tarefa
- Não funciona em C#
> Não há preempção; cada comando deve terminar logo.
:::

::: cartas
Command :: Padrão que encapsula uma ação em objeto com `Execute()`.
Transação (Command) :: Comando que guarda dados, valida e executa depois.
Undo/Redo :: Desfazer e refazer usando pilhas de comandos com `Undo()`.
Desacoplamento temporal :: Separar o momento de coletar do momento de executar.
Active Object :: Motor com fila de comandos que se reinserem, dando multitarefa cooperativa.
Run-To-Completion (RTC) :: Cada tarefa roda até o fim sem bloquear.
Não determinismo :: Resultados variam entre execuções em sistemas concorrentes.
:::
