---
id: agil-cap40
order: 58
kind: capitulo
num: 40
group: C# · Parte 11 · Pacotes, padrões e arquitetura
title: A interface do usuário — Model View Presenter
subtitle: Como construir telas finas, testáveis e trocáveis — o padrão MVP (Humble Dialog), com testes de comportamento, e como ele se relaciona com MVC e MVVM.
minutes: 70
hue: 200
objectives:
- Separar a **lógica de negócio da UI** para proteger as regras da volatilidade da interface
- Aplicar o padrão **Model–View–Presenter** com uma **view-interface** (DIP) e uma **view falsa** nos testes
- Escrever **testes de comportamento** (o que o presenter *diz* à view) em vez de testes de estado
- Entender a **janela "humilde"** (Humble Dialog) e a relação com **MVC/MVVM** e com outras tecnologias (WPF, MAUI, Blazor, web)
origem: livros/02-projeto-agil-csharp/content/57-cap40.md
---

::: ilustracao img/agil/cap40.svg | MVP: a tela só mostra; o apresentador decide; o modelo guarda os dados e as regras.
:::

## Do requisito à decisão de interface

O sistema de folha de pagamentos já faz tudo para o cliente: adiciona funcionários de vários tipos, calcula pagamentos por várias agendas, paga de várias formas e **persiste em SQL** (Laboratório 6). Foi para produção. Quem usa? **Joe**, do RH: ele recebe pedidos de toda a empresa e os digita num **arquivo de texto de transações** que é processado todas as noites. Joe anda irritado — e adoraria que cada pessoa **inserisse a própria transação**.

A decisão sobre **que tipo de interface construir** merece conversa com o cliente:

| Opção | Vantagens | Desvantagens |
|---|---|---|
| **Texto** (menus, teclado) | Fácil de construir | Menos fácil de usar; parece "deselegante" |
| **Web** | Sem instalação; acessível em qualquer máquina da intranet | Infraestrutura (servidores, camadas), HTML/CSS/JS, modelo de usuário "de tela" |
| **Desktop** (Windows Forms, WPF, MAUI) | Funcionalidade rica, simples de construir e implantar quando todos estão no mesmo escritório | Pouco portátil, menos fácil de distribuir |

Os usuários trabalham no mesmo escritório, com máquinas da empresa: o **desktop** vence. E, como a UI é difícil de acertar, **limitamos a primeira versão** à ação **Adicionar funcionário** — assim aprendemos rápido quão complicada é a UI e o que Joe acha dela. (Se o retorno disser que texto ou web seria melhor, melhor descobrir **antes** de investir no aplicativo inteiro.)

::: regra A forma da UI importa menos que a arquitetura interna
Interfaces são **voláteis**: mudam bem mais que as regras de negócio sob elas. Por isso, **separe cuidadosamente** a lógica de negócio da interface — escreva o **mínimo possível de código** no formulário e coloque o resto em **classes C# puras**. Se um dia a UI virar web, as regras já estarão separadas.
:::

## A interface imaginada

A janela principal tem:

- Uma lista de **transações pendentes** (o sistema é **em lote**: as transações se acumulam durante o dia e rodam à noite);
- Uma lista de **funcionários** existentes;
- Um botão **Executar transações** (que dispara o lote);
- Um menu **Ação → Adicionar funcionário**, que abre um formulário.

O formulário de adicionar funcionário tem: **id**, **nome**, **endereço**, uma escolha de tipo (**horista / assalariado / comissionado**) e os campos de valores correspondentes (valor/hora; salário; salário-base + comissão). O botão **Enviar** só habilita quando **todos** os dados necessários estão preenchidos; e cada campo de valor só habilita quando seu tipo está selecionado.

::: figura O formulário
<svg viewBox="0 0 560 330" role="img" aria-label="Formulário com id, nome, endereço, três opções de tipo com campos e botão Enviar desabilitado">
  <rect class="f-plain" x="10" y="10" width="540" height="310" rx="10"/>
  <text x="30" y="40" font-weight="800">Adicionar funcionário</text>
  <text x="30" y="76" class="t-dim" font-size="13">Id</text><rect class="f-prim" x="130" y="58" width="120" height="26" rx="4"/>
  <text x="30" y="114" class="t-dim" font-size="13">Nome</text><rect class="f-prim" x="130" y="96" width="360" height="26" rx="4"/>
  <text x="30" y="152" class="t-dim" font-size="13">Endereço</text><rect class="f-prim" x="130" y="134" width="360" height="26" rx="4"/>
  <circle cx="40" cy="192" r="7" class="f-info"/><text x="56" y="197" font-size="13">Horista</text><text x="200" y="197" class="t-dim" font-size="13">Valor/hora</text><rect class="f-plain" x="290" y="179" width="100" height="24" rx="4" style="opacity:.55"/>
  <circle cx="40" cy="226" r="7" class="f-info"/><text x="56" y="231" font-size="13">Assalariado</text><text x="200" y="231" class="t-dim" font-size="13">Salário</text><rect class="f-plain" x="290" y="213" width="100" height="24" rx="4" style="opacity:.55"/>
  <circle cx="40" cy="260" r="7" class="f-info"/><text x="56" y="265" font-size="13">Comissionado</text><text x="200" y="265" class="t-dim" font-size="13">Salário-base / Comissão</text><rect class="f-plain" x="370" y="247" width="70" height="24" rx="4" style="opacity:.55"/><rect class="f-plain" x="450" y="247" width="70" height="24" rx="4" style="opacity:.55"/>
  <rect class="f-plain" x="440" y="282" width="100" height="28" rx="6" style="opacity:.55"/><text x="490" y="301" text-anchor="middle" class="t-dim" font-size="13">Enviar</text>
</svg>
:::

## O padrão Model–View–Presenter

Quais regras essa janela precisa obter? Reunir as informações; descobrir **qual** `AdicionarFuncionario…` criar; colocar a transação na lista pendente; habilitar/desabilitar o botão e campos. Nada disso deve ficar **dentro do formulário**.

O padrão **MVP** divide a responsabilidade em três:

- **Model** — as regras de negócio (aqui, as transações `AdicionarFuncionario…` e o domínio);
- **View** — a janela: **só** exibe e captura entradas; **nenhuma decisão de negócio**;
- **Presenter** — a **lógica da tela**: recebe os dados da view, decide, cria transações, e **manda a view se atualizar**.

::: figura MVP com view-interface
<svg viewBox="0 0 740 230" role="img" aria-label="Presenter usa IView; Janela e ViewFalsa implementam; Presenter usa Model">
  <rect class="f-prim" x="270" y="20" width="200" height="56" rx="10"/><text x="370" y="46" text-anchor="middle" font-weight="800">Presenter</text><text x="370" y="64" text-anchor="middle" font-size="11" class="t-dim">lógica da tela</text>
  <rect class="f-info" x="270" y="120" width="200" height="46" rx="10"/><text x="370" y="140" text-anchor="middle" font-size="10" class="t-dim">«interface»</text><text x="370" y="158" text-anchor="middle" font-weight="800" class="t-info">IView</text>
  <path class="arrow" d="M370 78V118"/>
  <path class="line" d="M370 168V186M150 186H590M150 186V196M590 186V196" style="marker-end:none"/>
  <rect class="f-good" x="10" y="196" width="280" height="30" rx="8"/><text x="150" y="216" text-anchor="middle" font-size="13">Janela (Windows Forms / WPF / …)</text>
  <rect class="f-warn" x="500" y="196" width="180" height="30" rx="8"/><text x="590" y="216" text-anchor="middle" font-size="13">ViewFalsa (testes)</text>
  <rect class="f-plain" x="560" y="20" width="170" height="56" rx="10"/><text x="645" y="54" text-anchor="middle" font-weight="800">Model (transações)</text>
  <path class="arrow" d="M472 48H558"/>
  <path d="M60 192Q10 100 268 50" stroke="var(--ink-3)" stroke-dasharray="5 4" fill="none" marker-end="url(#arr2)"/><text x="30" y="110" font-size="11" class="t-dim">eventos</text>
</svg>
:::

Repare no detalhe que faz toda a diferença: o presenter **não depende da janela concreta**, e sim de uma **interface** `IView` — a **inversão de dependência**. Por quê? **Para testar.** Colocar uma UI real "sob teste" é um desafio; com a interface, o teste do presenter usa uma **view falsa** (um *spy* simples) e nunca abre uma janela.

::: dica O que acontece se a lógica ficar no formulário
É **muito comum** colocar tudo no formulário — e é problemático: (1) viola o **SRP**; (2) as regras ficam **difíceis de testar** (você precisa clicar em botões, ler *labels*, escolher itens em combos: testes **frágeis**, e a UI no *harness* de testes já é um desafio por si só); (3) se amanhã precisar de uma **versão web**, a lógica terá de ser **duplicada**.
:::

## O presenter, guiado por testes

Primeiro, a **view** que o presenter enxerga — uma interface mínima:

```cs
public interface IAdicionarFuncionarioView
{
    bool EnviarHabilitado { set; }
}
```

E a **view falsa**, que só anota o que o presenter lhe disse:

```cs
public class AdicionarFuncionarioViewFalsa : IAdicionarFuncionarioView
{
    public bool EnviarHabilitado { get; private set; }     // último valor recebido
    public int VezesHabilitadoAtualizado { get; private set; }

    bool IAdicionarFuncionarioView.EnviarHabilitado
    {
        set { EnviarHabilitado = value; VezesHabilitadoAtualizado++; }
    }
}
```

> Em testes reais você pode usar **Moq/NSubstitute** para gerar essas views falsas. Escrevê-la à mão (como aqui) mostra o conceito sem mágica.

Uma pequena **fila de transações** (o lote do dia) e o **tipo de pagamento** como enum — **estados impossíveis não deveriam ser representáveis** (em vez de três `bool` independentes, onde se poderia marcar "horista" e "assalariado" juntos):

```cs
public enum TipoDeFuncionario { Nenhum, Horista, Assalariado, Comissionado }

public class FilaDeTransacoes
{
    private readonly List<ITransacao> transacoes = new List<ITransacao>();
    public IReadOnlyList<ITransacao> Transacoes { get { return transacoes; } }
    public event Action Alterada;                              // avisa quando muda

    public void Adicionar(ITransacao t) { transacoes.Add(t); Alterada?.Invoke(); }
    public void Limpar()                { transacoes.Clear(); Alterada?.Invoke(); }
}
```

### Teste 1 — o mais simples possível

Quando não se sabe por onde começar, **teste a coisa mais simples**. Os testes seguintes vêm naturalmente:

```cs
[TestFixture]
public class AdicionarFuncionarioPresenterTests
{
    private AdicionarFuncionarioPresenter presenter;
    private FilaDeTransacoes fila;
    private BancoEmMemoria banco;
    private AdicionarFuncionarioViewFalsa view;

    [SetUp]
    public void SetUp()
    {
        view = new AdicionarFuncionarioViewFalsa();
        fila = new FilaDeTransacoes();
        banco = new BancoEmMemoria();
        presenter = new AdicionarFuncionarioPresenter(view, fila, banco);
    }

    [Test]
    public void ComecaSemInformacaoCompleta()
    {
        Assert.IsFalse(presenter.TemTodasAsInformacoes());
    }
}
```

### Teste 2 — reunir as informações

Uma responsabilidade do presenter é **reunir todos os dados** necessários. Dados parciais não servem — ele precisa saber **quando** tem tudo. O teste alimenta os dados **um a um**, perguntando a cada passo:

```cs
[Test]
public void TemTodasAsInformacoes_PassoAPasso()
{
    Assert.IsFalse(presenter.TemTodasAsInformacoes());
    presenter.Id = 1;                  Assert.IsFalse(presenter.TemTodasAsInformacoes());
    presenter.Nome = "Bia";            Assert.IsFalse(presenter.TemTodasAsInformacoes());
    presenter.Endereco = "Rua A, 1";   Assert.IsFalse(presenter.TemTodasAsInformacoes());
    presenter.Tipo = TipoDeFuncionario.Horista;
                                       Assert.IsFalse(presenter.TemTodasAsInformacoes());
    presenter.ValorHora = 12.5m;       Assert.IsTrue(presenter.TemTodasAsInformacoes());

    presenter.Tipo = TipoDeFuncionario.Assalariado;
                                       Assert.IsFalse(presenter.TemTodasAsInformacoes());
    presenter.Salario = 3000m;         Assert.IsTrue(presenter.TemTodasAsInformacoes());

    presenter.Tipo = TipoDeFuncionario.Comissionado;
                                       Assert.IsFalse(presenter.TemTodasAsInformacoes());
    presenter.SalarioBase = 1000m;     Assert.IsFalse(presenter.TemTodasAsInformacoes());
    presenter.Comissao = 0.12m;        Assert.IsTrue(presenter.TemTodasAsInformacoes());
}
```

### Teste 3 — avisar a view (comportamento, não estado)

O usuário **não deve poder enviar** até o presenter estar satisfeito. Logo, é do presenter a responsabilidade de **dizer à view** quando enviar está liberado:

```cs
[Test]
public void AvisaAViewQuandoEnviarFicaPossivel()
{
    presenter.Id = 1;                                  Verificar(false, 1);
    presenter.Nome = "Bia";                            Verificar(false, 2);
    presenter.Endereco = "Rua A, 1";                   Verificar(false, 3);
    presenter.Tipo = TipoDeFuncionario.Horista;        Verificar(false, 4);
    presenter.ValorHora = 12.5m;                       Verificar(true, 5);
}

private void Verificar(bool esperado, int chamadas)
{
    Assert.AreEqual(esperado, view.EnviarHabilitado);
    Assert.AreEqual(chamadas, view.VezesHabilitadoAtualizado);
}
```

::: pense Testamos o comportamento, não só o estado
Repare: **não testamos** "o botão fica habilitado" (isso seria um fato da janela). Testamos que, quando todos os dados chegam, **o presenter envia a mensagem certa para a view**. Esse estilo é o **desenvolvimento orientado a comportamento (BDD)**: asserções sobre estado e resultado são lidas como **especificações do comportamento** — *"quando X, o presenter manda a view fazer Y"*. Imagine o teste se tivéssemos de escavar controles de janela!
:::

### Teste 4 e 5 — criar e enfileirar a transação

```cs
[Test]
public void CriaATransacaoCerta()
{
    presenter.Id = 123; presenter.Nome = "Joe"; presenter.Endereco = "Rua Olmo, 314";

    presenter.Tipo = TipoDeFuncionario.Horista;      presenter.ValorHora = 10m;
    Assert.IsInstanceOf<AdicionarHorista>(presenter.CriarTransacao());

    presenter.Tipo = TipoDeFuncionario.Assalariado;  presenter.Salario = 3000m;
    Assert.IsInstanceOf<AdicionarAssalariado>(presenter.CriarTransacao());

    presenter.Tipo = TipoDeFuncionario.Comissionado; presenter.SalarioBase = 1000m; presenter.Comissao = 0.25m;
    Assert.IsInstanceOf<AdicionarComissionado>(presenter.CriarTransacao());
}

[Test]
public void AdicionarColocaATransacaoNaFila()
{
    presenter.Id = 123; presenter.Nome = "Joe"; presenter.Endereco = "Rua Olmo, 314";
    presenter.Tipo = TipoDeFuncionario.Horista; presenter.ValorHora = 25m;

    presenter.Adicionar();

    Assert.AreEqual(1, fila.Transacoes.Count);
    Assert.IsInstanceOf<AdicionarHorista>(fila.Transacoes[0]);
}
```

### O presenter

```cs
public class AdicionarFuncionarioPresenter
{
    private readonly IAdicionarFuncionarioView view;
    private readonly FilaDeTransacoes fila;
    private readonly IBancoDeFuncionarios banco;

    public AdicionarFuncionarioPresenter(IAdicionarFuncionarioView view, FilaDeTransacoes fila, IBancoDeFuncionarios banco)
    {
        this.view = view; this.fila = fila; this.banco = banco;
    }

    // Cada propriedade guarda o valor e reavalia a view
    private int id;                       public int Id { get { return id; } set { id = value; AtualizarView(); } }
    private string nome;                  public string Nome { get { return nome; } set { nome = value; AtualizarView(); } }
    private string endereco;              public string Endereco { get { return endereco; } set { endereco = value; AtualizarView(); } }
    private TipoDeFuncionario tipo;       public TipoDeFuncionario Tipo { get { return tipo; } set { tipo = value; AtualizarView(); } }
    private decimal valorHora;            public decimal ValorHora { get { return valorHora; } set { valorHora = value; AtualizarView(); } }
    private decimal salario;              public decimal Salario { get { return salario; } set { salario = value; AtualizarView(); } }
    private decimal salarioBase;          public decimal SalarioBase { get { return salarioBase; } set { salarioBase = value; AtualizarView(); } }
    private decimal comissao;             public decimal Comissao { get { return comissao; } set { comissao = value; AtualizarView(); } }

    public bool TemTodasAsInformacoes()
    {
        if (id <= 0 || string.IsNullOrEmpty(nome) || string.IsNullOrEmpty(endereco)) return false;

        switch (tipo)
        {
            case TipoDeFuncionario.Horista:      return valorHora > 0;
            case TipoDeFuncionario.Assalariado:  return salario > 0;
            case TipoDeFuncionario.Comissionado: return salarioBase > 0 && comissao > 0;
            default:                             return false;
        }
    }

    public ITransacao CriarTransacao()
    {
        switch (tipo)
        {
            case TipoDeFuncionario.Horista:      return new AdicionarHorista(id, nome, endereco, valorHora, banco);
            case TipoDeFuncionario.Assalariado:  return new AdicionarAssalariado(id, nome, endereco, salario, banco);
            case TipoDeFuncionario.Comissionado: return new AdicionarComissionado(id, nome, endereco, salarioBase, comissao, banco);
            default: throw new InvalidOperationException("Tipo de funcionário não informado.");
        }
    }

    public virtual void Adicionar() { fila.Adicionar(CriarTransacao()); }

    private void AtualizarView() { view.EnviarHabilitado = TemTodasAsInformacoes(); }
}
```

Note o `switch` que escolhe a transação pelo tipo: um ponto **conhecido e isolado** de decisão. Se os tipos de funcionário se multiplicarem muito, troque-o por um **Factory** (Capítulo 34).

## A janela "humilde"

Com o presenter pronto, falta a **janela**. O visual vem do *designer* (arrastar controles). O que escrevemos à mão é **comportamento de UI**: ligar eventos ao presenter e habilitar/desabilitar campos conforme o tipo escolhido.

```cs
public partial class JanelaAdicionarFuncionario : Form, IAdicionarFuncionarioView
{
    public AdicionarFuncionarioPresenter Presenter { get; set; }

    public bool EnviarHabilitado { set { botaoEnviar.Enabled = value; } }

    // --- repassa o que o usuário digitou ---
    private void IdAlterado(object s, EventArgs e)       { Presenter.Id = LerInt(caixaId.Text); }
    private void NomeAlterado(object s, EventArgs e)     { Presenter.Nome = caixaNome.Text; }
    private void EnderecoAlterado(object s, EventArgs e) { Presenter.Endereco = caixaEndereco.Text; }
    private void ValorHoraAlterado(object s, EventArgs e){ Presenter.ValorHora = LerDecimal(caixaValorHora.Text); }
    // (salário, salário-base e comissão: idem)

    // --- comportamento puramente visual ---
    private void HoristaMarcado(object s, EventArgs e)
    {
        caixaValorHora.Enabled = radioHorista.Checked;          // habilitar campo = coisa da UI
        if (radioHorista.Checked) Presenter.Tipo = TipoDeFuncionario.Horista;
    }
    // AssalariadoMarcado / ComissionadoMarcado: idem

    private void EnviarClicado(object s, EventArgs e)
    {
        Presenter.Adicionar();
        Close();
    }

    private static int LerInt(string texto)
    { int v; return int.TryParse(texto, out v) ? v : 0; }

    private static decimal LerDecimal(string texto)
    { decimal v; return decimal.TryParse(texto, NumberStyles.Number, CultureInfo.CurrentCulture, out v) ? v : 0m; }
}
```

Esse é o conceito do **Humble Dialog / Humble Object** ("caixa de diálogo humilde"): o código **difícil de testar** (UI) é reduzido ao **mínimo burro** — só repassa eventos e liga/desliga controles — e todo o **raciocínio** vai para o que é fácil de testar (o presenter).

Ainda assim vale um **punhado de testes de UI** (rodar o formulário numa suíte automatizada ou com ferramentas de UI), cobrindo só o que ela faz:

- estado inicial (botão e campos de valor desabilitados);
- digitar nos campos **chega** ao presenter com os valores convertidos;
- marcar "Horista" **habilita** só o campo de valor/hora;
- `EnviarHabilitado = true` habilita o botão;
- clicar em Enviar **fecha** a janela e coloca 1 transação na fila.

::: cuidado Conversão ≠ regra de negócio
Converter texto em número (e lidar com cultura/vírgula decimal) é **responsabilidade da view** — o presenter já recebe `decimal`. Mas **validar** que o valor é positivo **é regra** — fica no presenter (`valorHora > 0`).
:::

## A janela principal

A janela principal segue o mesmo desenho: um presenter com sua view-interface.

```cs
public interface IJanelaPrincipalView
{
    string TextoDasTransacoes { set; }
    string TextoDosFuncionarios { set; }
}

public interface IAbridorDeJanelas
{
    void AbrirAdicionarFuncionario();
}

public class JanelaPrincipalPresenter
{
    private readonly IJanelaPrincipalView view;
    private readonly FilaDeTransacoes fila;
    private readonly IBancoDeFuncionarios banco;
    private readonly IAbridorDeJanelas janelas;

    public JanelaPrincipalPresenter(IJanelaPrincipalView view, FilaDeTransacoes fila,
                                    IBancoDeFuncionarios banco, IAbridorDeJanelas janelas)
    {
        this.view = view; this.fila = fila; this.banco = banco; this.janelas = janelas;
        fila.Alterada += Atualizar;                    // a lista de pendentes se atualiza sozinha
        Atualizar();
    }

    public void AcaoAdicionarFuncionario() { janelas.AbrirAdicionarFuncionario(); }

    public void ExecutarTransacoes()
    {
        foreach (var t in fila.Transacoes) t.Executar();   // roda o lote
        fila.Limpar();                                     // dispara Atualizar via evento
    }

    private void Atualizar()
    {
        view.TextoDasTransacoes   = string.Join(Environment.NewLine, fila.Transacoes.Select(t => t.ToString()));
        view.TextoDosFuncionarios = string.Join(Environment.NewLine, banco.Todos().Select(f => f.Id + " - " + f.Nome + " (" + f.Endereco + ")"));
    }
}
```

Teste (sem janela alguma):

```cs
[Test]
public void ExecutarTransacoesAtualizaALista()
{
    var banco = new BancoEmMemoria();
    var fila = new FilaDeTransacoes();
    var view = new JanelaPrincipalViewFalsa();
    var presenter = new JanelaPrincipalPresenter(view, fila, banco, new AbridorFalso());

    fila.Adicionar(new AdicionarAssalariado(1, "Ana", "Rua B, 2", 2000m, banco));
    StringAssert.Contains("AdicionarAssalariado", view.TextoDasTransacoes);   // pendente aparece
    Assert.AreEqual("", view.TextoDosFuncionarios);                           // ainda não executou

    presenter.ExecutarTransacoes();

    Assert.AreEqual("", view.TextoDasTransacoes);                             // lote esvaziado
    StringAssert.Contains("Ana", view.TextoDosFuncionarios);                  // funcionário apareceu
}
```

E a **inauguração** — o ponto de entrada monta tudo (é a *composition root*, o único lugar que conhece as classes concretas):

```cs
static class Program
{
    [STAThread]
    static void Main()
    {
        IBancoDeFuncionarios banco = new BancoEmMemoria();     // em produção: new BancoSql(conexao)
        var fila = new FilaDeTransacoes();

        var janela = new JanelaPrincipal();
        var abridor = new AbridorDeJanelas(fila, banco);        // cria JanelaAdicionarFuncionario + seu presenter
        janela.Presenter = new JanelaPrincipalPresenter(janela, fila, banco, abridor);

        Application.Run(janela);
    }
}
```

Troque `BancoEmMemoria` por `BancoSql` e **nada mais muda** — o DIP e a separação em pacotes (Laboratório 6) pagando juros.

## E hoje? MVP, MVC e MVVM

| Padrão | Quem faz a ponte | Onde brilha |
|---|---|---|
| **MVP** (aqui) | O **presenter** manda na view através de uma interface; a view é *passiva* | Windows Forms, formulários simples, qualquer UI com testes por view falsa |
| **MVC** | O **controller** recebe a entrada, atualiza o **model** e escolhe a **view** | **ASP.NET Core MVC**, APIs web |
| **MVVM** | O **view-model** expõe propriedades/comandos; a **view** se liga por *data binding* | **WPF**, **MAUI**, **Avalonia**, **Blazor** (variações) |

A ideia central é a mesma em todos: **a lógica de apresentação vive em uma classe C# testável, sem dependência de controles**; a view é fina. Em MVVM, o view-model faz o papel do presenter; em vez de o presenter chamar `view.EnviarHabilitado = …`, o view-model expõe `PodeEnviar` (com `INotifyPropertyChanged`/`ICommand`) e o *binding* atualiza o botão.

```cs
// Mesma lógica, versão MVVM: sem interface de view, o binding faz a ponte
public class AdicionarFuncionarioViewModel : INotifyPropertyChanged
{
    private string nome;
    public string Nome { get { return nome; } set { nome = value; Mudou(nameof(Nome)); Mudou(nameof(PodeEnviar)); } }
    public bool PodeEnviar { get { return !string.IsNullOrEmpty(Nome) /* && ...demais regras... */; } }

    public event PropertyChangedEventHandler PropertyChanged;
    private void Mudou(string propriedade) { PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propriedade)); }
}
```

E a **portabilidade** que o capítulo prometia: se um dia o cliente pedir uma **versão web**, a **mesma** lógica (`TemTodasAsInformacoes`, `CriarTransacao`) é reaproveitada por um controller ASP.NET Core ou componente Blazor — a regra não precisa ser reescrita.

::: dica Humble Object em qualquer lugar
O truque vale muito além de telas: o que é **difícil de testar** (UI, banco, relógio, rede, sistema de arquivos) deve ser **tão fino quanto possível**, escondido atrás de uma interface, e o **raciocínio** concentrado em classes simples. É o mesmo princípio dos gateways do Capítulo 37.
:::

::: exercicio Excluir funcionário
Planeje a ação **Excluir funcionário** (só o id é necessário):

1. Que **view-interface** e **presenter** você criaria?
2. Que **teste de comportamento** escreveria primeiro?
3. O que ficaria na **janela**?

--- resposta
1. `IExcluirFuncionarioView { bool ExcluirHabilitado { set; } }` e `ExcluirFuncionarioPresenter` com a propriedade `Id`, o método `TemTodasAsInformacoes()` (`id > 0`), `CriarTransacao()` → `ExcluirFuncionario(id, banco)` e `Excluir()` que enfileira.
2. "Enquanto o id não for informado, a view recebe `ExcluirHabilitado = false`; ao informar `Id = 5`, recebe `true`" (verificando também a contagem de chamadas).
3. Apenas: repassar o texto digitado do id (com conversão), `ExcluirHabilitado` → `botao.Enabled`, e o clique que chama `Presenter.Excluir()` e fecha a janela.
:::

## Conclusão

Joe vai ficar contente — e, com certeza, vai comentar que a interface está **rudimentar**. Interfaces são difíceis de acertar; a arquitetura em MVP nos deixa **iterar rápido**: ouvir Joe, ajustar o presenter e a janela, e acrescentar as próximas ações (alterar funcionário, cartões de ponto, recibos de venda, dia de pagamento) **sem tocar nas regras de negócio**.

::: resumo
- Escolha a interface **com o cliente** (texto, web, desktop) e comece com uma **fatia pequena** para aprender rápido.
- A UI é **volátil**: proteja as regras de negócio separando-as dela (SRP, DIP).
- **MVP:** *Model* (regras), *View* (fina; interface), *Presenter* (lógica da tela; decide e diz à view o que mostrar).
- O presenter depende de uma **IView**, não da janela: assim os testes usam uma **view falsa** — sem UI.
- Teste **comportamento** (mensagens do presenter para a view), não só estado → BDD/especificação executável.
- **Humble Dialog:** a janela só repassa eventos e liga/desliga controles; conversão de texto fica na view, **validação** no presenter.
- Prefira **tipos que impeçam estados impossíveis** (enum em vez de vários `bool`).
- **MVC/MVVM** compartilham a ideia; a lógica de apresentação testável é reaproveitável em outras tecnologias (web, MAUI).
:::

::: quiz
? Por que separar a lógica de negócio da interface gráfica?
+ Porque a UI muda mais que as regras; a separação protege as regras e facilita os testes
- Porque formulários não permitem código
- Para deixar o aplicativo mais lento
- Porque o C# proíbe
> Se a UI mudar (ou virar web), as regras continuam intactas.

? No MVP, de que o presenter depende para falar com a janela?
+ De uma interface de view (IView), invertendo a dependência
- Da classe Form concreta
- De variáveis globais
- De arquivos de configuração
> Isso permite testá-lo com uma view falsa.

? O que é uma "view falsa" nos testes?
+ Uma implementação simples da IView que apenas registra o que o presenter lhe disse
- Uma janela com visual diferente
- Um mock do banco de dados
- Um componente de terceiros
> Ela guarda o último valor e quantas vezes foi chamada.

? Por que o teste AvisaAViewQuandoEnviarFicaPossivel é um teste de comportamento?
+ Verifica a mensagem que o presenter manda à view, não um estado interno da janela
- Verifica a cor do botão
- Mede o tempo de resposta
- Executa a UI real
> Ele especifica "quando todos os dados chegam, o presenter manda habilitar o envio".

? O que fica na janela "humilde" (Humble Dialog)?
+ Repassar eventos ao presenter e controlar elementos visuais (habilitar campos, conversões de texto)
- Regras de negócio e validação
- Acesso ao banco
- Criação de transações
> O raciocínio vive no presenter, fácil de testar.

? Por que usar um enum TipoDeFuncionario em vez de três bools?
+ Impede estados impossíveis (ex.: horista e assalariado ao mesmo tempo)
- Porque enums são mais rápidos
- Porque bools não existem em C#
- Porque o designer exige
> Modele os dados para que o estado inválido não seja representável.

? Qual a relação entre MVVM e MVP?
+ O view-model faz o papel do presenter, mas a view se liga a ele por data binding
- São opostos e incompatíveis
- MVVM não tem model
- MVP só funciona na web
> Em ambos, a lógica de apresentação fica numa classe testável.
:::

::: cartas
MVP :: Model–View–Presenter: lógica da tela no presenter; a view é passiva e fina.
View-interface :: Interface que o presenter usa para falar com a view (permite fakes).
View falsa :: Implementação de teste que registra as mensagens recebidas do presenter.
Humble Dialog :: Janela reduzida ao mínimo (só eventos e controles); a lógica vai para um objeto testável.
BDD :: Desenvolvimento orientado a comportamento: testes como especificações de como o código se comporta.
Composition root :: Ponto de entrada onde as classes concretas são montadas e injetadas.
MVVM :: Padrão em que o view-model expõe propriedades/comandos e a view usa data binding.
Fila de transações :: Lote de transações pendentes executado de uma vez (modelo batch).
:::
