---
id: agil-lab5
order: 49
kind: lab
num: 5
group: C# · Parte 10 · Padrões de projeto (I)
title: Laboratório 5 — Folha de pagamentos (iteração 1)
subtitle: Do requisito ao código: histórias → casos de uso → abstrações (Strategy, Command, Null Object) → TDD. Um estudo de caso completo, em pequenos passos.
minutes: 120
hue: 320
badge: L5
objectives:
- Transformar **histórias de usuário** em **casos de uso** e extrair **abstrações** deles
- Aplicar **Strategy**, **Command** e **Null Object** em um sistema real
- Praticar **TDD** na implementação de transações e do cálculo de pagamentos
- Adiar a decisão do **banco de dados** (detalhe de implementação)
origem: livros/02-projeto-agil-csharp/content/48-lab5.md
---

::: vila A empresa de Hilda
Hilda é dona de uma pequena empresa e quer um sistema de **folha de pagamento em lote**. Os requisitos chegam em **histórias** — e, como na vida real, a primeira iteração cobre só uma fatia pequena do valor: impostos, por exemplo, **nem foram mencionados**. Começar pequeno é normal e saudável.
:::

::: ilustracao img/agil/lab5.svg | A folha de pagamentos: cada tipo de funcionário calcula o pagamento do seu jeito.
:::

Este laboratório recria a primeira iteração de um clássico estudo de caso do desenvolvimento ágil, **com texto e código próprios**: a sessão rápida de análise/projeto no quadro branco (parte 1) e a implementação guiada por testes (parte 2).

## O que Hilda pediu

Anotações da conversa com a cliente:

- **Horistas** recebem por hora; entregam **cartões de ponto** diários (data + horas). Horas **além de 8 por dia** valem **1,5×**. Recebem **toda sexta-feira**.
- **Assalariados** têm salário mensal fixo e recebem no **último dia útil do mês**.
- Alguns assalariados também ganham **comissão** sobre **recibos de venda** (data + valor); recebem **a cada duas sextas**.
- Cada funcionário escolhe como receber: **cheque enviado pelo correio**, **cheque retido** na tesouraria, ou **depósito direto** em conta.
- Alguns pertencem ao **sindicato**: pagam uma **mensalidade semanal** descontada do pagamento, e o sindicato pode lançar **taxas de serviço** avulsas, também descontadas.
- O sistema roda **a cada dia útil** e paga quem tem pagamento **naquela data**, cobrindo o período desde o último pagamento.

As histórias escolhidas para a iteração:

1. Adicionar funcionário · 2. Excluir funcionário · 3. Lançar cartão de ponto · 4. Lançar recibo de venda · 5. Lançar taxa de serviço do sindicato · 6. Alterar detalhes do funcionário · 7. Rodar a folha de hoje.

::: regra O banco de dados é um detalhe
É tentador começar pelo **esquema de tabelas** — os requisitos praticamente o desenham. Mas assim o aplicativo nasce **centrado no banco**, inseparável dele. **Abstração é "ampliar o essencial e eliminar o irrelevante"**, e neste estágio o banco é **irrelevante**: apenas uma técnica de guardar e buscar dados. **Adie-o o máximo possível.** Comece pelo **comportamento**, que é o que somos pagos para construir.
:::

## Parte 1 — Análise rápida pelos casos de uso

Um caso de uso é uma história de usuário um pouco mais elaborada — vale quando a história foi **escolhida para a iteração atual**. Faremos só o necessário para pensar o projeto. (Formato dos comandos: um por linha, como um arquivo de transações.)

### Caso 1 — Adicionar funcionário

> Chega `AdicionarFuncionario <id> "<nome>" "<endereço>" H <valorHora>` (ou `S <salárioMensal>`, ou `C <salárioMensal> <taxaComissão>`). Cria-se o funcionário com esses campos. **Alternativa:** transação mal formada → mensagem de erro e nenhuma ação.

Há uma abstração à vista: **três formas de uma mesma transação** que compartilham `id`, `nome` e `endereço`. Um **Command** abstrato `AdicionarFuncionarioTransacao` com **três derivadas** (horista, assalariado, comissionado) separa as tarefas — **SRP** em ação (jogar tudo num módulo só concentraria o processamento num lugar grande e propenso a erros).

### Caso 2 — Excluir funcionário

> `ExcluirFuncionario <id>`. **Alternativa:** id desconhecido → erro, nenhuma ação.

Nada de novo: só a transação `ExcluirFuncionarioTransacao`.

### Casos 3 e 4 — Cartão de ponto e recibo de venda

> `CartaoDePonto <id> <data> <horas>` — associa o cartão ao funcionário **horista**. **Alternativa:** o funcionário não é horista → erro.
> `ReciboDeVenda <id> <data> <valor>` — associa ao **comissionado**. **Alternativa:** não é comissionado → erro.

Descobrimos que **algumas transações só se aplicam a certos tipos** de funcionário, o que reforça a ideia de **tipos distintos** — e implica **associações**: horista ↔ N cartões; comissionado ↔ N recibos.

### Caso 5 — Taxa de serviço

> `TaxaDeServico <idMembro> <valor>` — associa a taxa ao **membro do sindicato**.

O sindicato tem **sua própria numeração** de membros; precisaremos associar membros a funcionários. Há várias formas — **adiamos a decisão** até alguma restrição nos empurrar numa direção.

### Caso 6 — Alterar detalhes

> `AlterarFuncionario <id> Nome <nome>` · `Endereco <end>` · `Horista <valorHora>` · `Assalariado <salário>` · `Comissionado <salário> <taxa>` · `Reter` · `Deposito <banco> <conta>` · `Correio <end>` · `Membro <idMembro> Mensalidade <valor>` · `SemSindicato`

Este caso de uso é **revelador**: mostra **tudo que pode mudar**. Se um funcionário pode passar de horista a assalariado, a **hierarquia de subclasses** `Horista/Assalariado/Comissionado` **não serve** (um objeto não troca de classe!). A solução: **Strategy** — o funcionário **contém** uma `ClassificacaoDePagamento`, que pode ser **trocada** por outra.

### Caso 7 — Rodar a folha

> `DiaDePagamento <data>` — encontra todos os funcionários cujo pagamento é nessa data, calcula quanto devem receber e os paga pelo método escolhido, imprimindo um relatório de auditoria.

Perguntas: **quem calcula** o pagamento? As classificações já guardam cartões/recibos — logo, **elas** calculam. **Quem sabe** se hoje é dia de pagar? E o desconto do sindicato?

## O modelo central: as abstrações subjacentes

Relendo os requisitos, três generalizações saltam — nenhuma estava **dita** nos casos de uso:

::: figura O modelo central do sistema
<svg viewBox="0 0 780 360" role="img" aria-label="Funcionario contém ClassificacaoDePagamento, AgendaDePagamento, MetodoDePagamento e uma lista de Afiliacao">
  <rect class="f-prim" x="290" y="150" width="200" height="60" rx="12"/><text x="390" y="176" text-anchor="middle" font-weight="800" font-size="17">Funcionario</text><text x="390" y="196" text-anchor="middle" font-size="11" class="t-mono">Id, Nome, Endereco</text>
  <g text-anchor="middle" font-size="12">
    <rect class="f-info" x="20" y="20" width="200" height="46" rx="8"/><text x="120" y="38" font-size="10" class="t-dim">«interface»</text><text x="120" y="56" font-weight="700">ClassificacaoDePagamento</text>
    <rect class="f-plain" x="10" y="90" width="80" height="36" rx="6"/><text x="50" y="113">Horista</text><rect class="f-plain" x="95" y="90" width="90" height="36" rx="6"/><text x="140" y="113">Assalariado</text><rect class="f-plain" x="190" y="90" width="90" height="36" rx="6"/><text x="235" y="113">Comissionado</text>
    <rect class="f-good" x="560" y="20" width="200" height="46" rx="8"/><text x="660" y="38" font-size="10" class="t-dim">«interface»</text><text x="660" y="56" font-weight="700">AgendaDePagamento</text>
    <rect class="f-plain" x="540" y="90" width="70" height="36" rx="6"/><text x="575" y="113">Semanal</text><rect class="f-plain" x="615" y="90" width="70" height="36" rx="6"/><text x="650" y="113">Mensal</text><rect class="f-plain" x="690" y="90" width="80" height="36" rx="6"/><text x="730" y="113">Quinzenal</text>
    <rect class="f-warn" x="20" y="270" width="200" height="46" rx="8"/><text x="120" y="288" font-size="10" class="t-dim">«interface»</text><text x="120" y="306" font-weight="700">MetodoDePagamento</text>
    <rect class="f-plain" x="10" y="330" width="70" height="28" rx="6"/><text x="45" y="349" font-size="11">Reter</text><rect class="f-plain" x="85" y="330" width="90" height="28" rx="6"/><text x="130" y="349" font-size="11">Deposito</text><rect class="f-plain" x="180" y="330" width="80" height="28" rx="6"/><text x="220" y="349" font-size="11">Correio</text>
    <rect class="f-bad" x="560" y="270" width="200" height="46" rx="8"/><text x="660" y="288" font-size="10" class="t-dim">«interface»  0..*</text><text x="660" y="306" font-weight="700">Afiliacao</text>
    <rect class="f-plain" x="560" y="330" width="120" height="28" rx="6"/><text x="620" y="349" font-size="11">AfiliacaoSindicato</text><rect class="f-plain" x="685" y="330" width="85" height="28" rx="6"/><text x="728" y="349" font-size="11">Caridade…</text>
  </g>
  <path class="arrow" d="M290 165L222 55"/><path class="arrow" d="M490 165L560 55"/><path class="arrow" d="M300 210L222 285"/><path class="arrow" d="M480 210L560 285"/>
  <text x="390" y="260" text-anchor="middle" font-size="12" class="t-dim">cada "contém" é uma Strategy — trocável sem tocar em Funcionario</text>
</svg>
:::

1. **Todo funcionário é pago, mas por esquemas diferentes** → **`ClassificacaoDePagamento`** (Strategy): `Horista`, `Assalariado`, `Comissionado`.
2. **Todo funcionário é pago segundo uma agenda** → **`AgendaDePagamento`** (Strategy): `Semanal`, `Mensal`, `Quinzenal`. Os requisitos **associam** agenda e classificação ("horistas recebem semanalmente"), mas essa ligação é **essencial**? E se amanhã cada departamento tiver uma agenda? **Política de agenda e política de pagamento podem mudar independentemente.** Se a agenda ficasse dentro da classificação, mudar uma obrigaria a **retestar** a outra (violando **OCP** e **SRP**) e um erro numa poderia **desagendar** funcionários — algo que programadores entendem mas que **gerentes temem**, pois mina a confiança ("qualquer mudança pode quebrar qualquer coisa").
3. **Todo funcionário recebe por algum método** → **`MetodoDePagamento`** (Strategy): `Reter`, `Deposito`, `Correio`.
4. **O funcionário pode ser afiliado a várias organizações** (sindicato, caridade, associação profissional) que descontam do pagamento → uma **lista de `Afiliacao`**. Repare: com uma **lista**, o funcionário sem afiliações simplesmente tem a **lista vazia** — **Null Object natural**, sem precisar de uma classe `SemAfiliacao`.

::: dica Abstrações que os requisitos não dizem
A agenda e as afiliações não aparecem como conceitos nos casos de uso: vieram de **ler os requisitos com atenção**, observar "os truques da comunidade de usuários" e **perguntar "isso precisa mesmo ser assim?"**. Ferramentas e procedimentos não substituem inteligência e experiência.
:::

::: pense Pouco, mas suficiente
Em uma tarde (de verdade) chegamos a um **molde** de arquitetura analisando **só as primeiras histórias**. Não fizemos análise abrangente de todos os requisitos, nem diagramas de sequência para cada detalhe. **Pensar o projeto em pequenos passos incrementais é fundamental; fazer demais é pior do que fazer de menos.**
:::

## Parte 2 — Implementando com testes

Convenções: C# moderno, **NUnit**, tudo em memória (o "banco" é uma interface + um dicionário). Começamos **pelos testes**.

### Passo 1 — O banco como detalhe

```cs
public interface IBancoDeFuncionarios
{
    Funcionario Obter(int id);
    void Adicionar(Funcionario f);
    void Remover(int id);
    IEnumerable<Funcionario> Todos();
}

public class BancoEmMemoria : IBancoDeFuncionarios
{
    private readonly Dictionary<int, Funcionario> dados = new Dictionary<int, Funcionario>();
    public Funcionario Obter(int id) { return dados.TryGetValue(id, out var f) ? f : null; }
    public void Adicionar(Funcionario f) { dados[f.Id] = f; }
    public void Remover(int id) { dados.Remove(id); }
    public IEnumerable<Funcionario> Todos() { return dados.Values; }
}
```

Mais tarde (Laboratório 6) trocaremos por um banco **real** sem tocar no resto — graças ao **DIP**.

### Passo 2 — Primeiro teste: adicionar um assalariado

```cs
[Test]
public void AdicionarFuncionarioAssalariado()
{
    var banco = new BancoEmMemoria();
    new AdicionarAssalariado(1, "Bruna", "Rua A, 10", 3000m, banco).Executar();

    Funcionario f = banco.Obter(1);
    Assert.AreEqual("Bruna", f.Nome);
    Assert.IsInstanceOf<ClassificacaoAssalariado>(f.Classificacao);
    Assert.AreEqual(3000m, ((ClassificacaoAssalariado)f.Classificacao).Salario);
    Assert.IsInstanceOf<AgendaMensal>(f.Agenda);
    Assert.IsInstanceOf<MetodoReter>(f.Metodo);
}
```

O teste **expressa a intenção**: mostra quais classes precisamos. Agora o domínio e o **Command**:

```cs
public interface ITransacao { void Executar(); }

public abstract class AdicionarFuncionario : ITransacao
{
    private readonly int id; private readonly string nome, endereco; private readonly IBancoDeFuncionarios banco;
    protected AdicionarFuncionario(int id, string nome, string endereco, IBancoDeFuncionarios banco)
    { this.id = id; this.nome = nome; this.endereco = endereco; this.banco = banco; }

    protected abstract ClassificacaoDePagamento CriarClassificacao();
    protected abstract AgendaDePagamento CriarAgenda();

    public void Executar()                      // Template Method!
    {
        var f = new Funcionario(id, nome, endereco)
        {
            Classificacao = CriarClassificacao(),
            Agenda = CriarAgenda(),
            Metodo = new MetodoReter()          // padrão: reter o cheque
        };
        banco.Adicionar(f);
    }
}

public class AdicionarAssalariado : AdicionarFuncionario
{
    private readonly decimal salario;
    public AdicionarAssalariado(int id, string n, string e, decimal salario, IBancoDeFuncionarios b) : base(id, n, e, b) { this.salario = salario; }
    protected override ClassificacaoDePagamento CriarClassificacao() { return new ClassificacaoAssalariado(salario); }
    protected override AgendaDePagamento CriarAgenda() { return new AgendaMensal(); }
}
// AdicionarHorista (+ AgendaSemanal) e AdicionarComissionado (+ AgendaQuinzenal): idênticos na forma
```

Veja **três padrões juntos**: **Command** (`ITransacao`), **Template Method** (`Executar` na base, `Criar…` nas derivadas) e **Strategy** (classificação/agenda/método).

### Passo 3 — O funcionário e as estratégias

```cs
public class Funcionario
{
    public int Id { get; }
    public string Nome { get; set; }
    public string Endereco { get; set; }
    public ClassificacaoDePagamento Classificacao { get; set; }
    public AgendaDePagamento Agenda { get; set; }
    public MetodoDePagamento Metodo { get; set; }
    public List<Afiliacao> Afiliacoes { get; } = new List<Afiliacao>();   // lista vazia = sem afiliação (Null Object natural)

    public Funcionario(int id, string nome, string endereco) { Id = id; Nome = nome; Endereco = endereco; }

    public bool EhDiaDePagamento(DateTime dia) { return Agenda.EhDiaDePagamento(dia); }

    public void ProcessarPagamento(Contracheque cc)
    {
        cc.Bruto = Classificacao.CalcularPagamento(cc);
        cc.Descontos = Afiliacoes.Sum(a => a.CalcularDesconto(cc));
        cc.Liquido = cc.Bruto - cc.Descontos;
        Metodo.Pagar(cc);
    }
}

public class Contracheque
{
    public DateTime Inicio { get; }
    public DateTime Fim { get; }
    public decimal Bruto, Descontos, Liquido;
    public Contracheque(DateTime inicio, DateTime fim) { Inicio = inicio; Fim = fim; }
}
```

### Passo 4 — Testes de cálculo: o horista

O teste diz o que o cliente espera, em **números simples** (R$ 15/hora; 8 horas normais; extras 1,5×):

```cs
[Test]
public void Horista_PagaHorasExtrasComAdicional()
{
    var classificacao = new ClassificacaoHorista(15m);
    classificacao.AdicionarCartao(new CartaoDePonto(new DateTime(2025, 11, 3), 10));   // 8×15 + 2×15×1.5 = 165
    classificacao.AdicionarCartao(new CartaoDePonto(new DateTime(2025, 11, 4), 5));    // 5×15           =  75
    classificacao.AdicionarCartao(new CartaoDePonto(new DateTime(2025, 10, 1), 8));    // fora do período
    var cc = new Contracheque(new DateTime(2025, 11, 1), new DateTime(2025, 11, 7));

    Assert.AreEqual(240m, classificacao.CalcularPagamento(cc));
}
```

```cs
public abstract class ClassificacaoDePagamento
{
    public abstract decimal CalcularPagamento(Contracheque cc);
    protected static bool NoPeriodo(DateTime d, Contracheque cc) { return d >= cc.Inicio && d <= cc.Fim; }
}

public class ClassificacaoHorista : ClassificacaoDePagamento
{
    private readonly decimal valorHora;
    private readonly List<CartaoDePonto> cartoes = new List<CartaoDePonto>();
    public ClassificacaoHorista(decimal valorHora) { this.valorHora = valorHora; }
    public void AdicionarCartao(CartaoDePonto c) { cartoes.Add(c); }

    public override decimal CalcularPagamento(Contracheque cc)
    {
        decimal total = 0;
        foreach (var c in cartoes.Where(c => NoPeriodo(c.Data, cc)))
        {
            decimal normais = Math.Min(8m, c.Horas);
            decimal extras = Math.Max(0m, c.Horas - 8m);
            total += normais * valorHora + extras * valorHora * 1.5m;
        }
        return total;
    }
}

public class ClassificacaoAssalariado : ClassificacaoDePagamento
{
    public decimal Salario { get; }
    public ClassificacaoAssalariado(decimal salario) { Salario = salario; }
    public override decimal CalcularPagamento(Contracheque cc) { return Salario; }
}

public class ClassificacaoComissionado : ClassificacaoDePagamento
{
    private readonly decimal salario, taxa;
    private readonly List<ReciboDeVenda> recibos = new List<ReciboDeVenda>();
    public ClassificacaoComissionado(decimal salario, decimal taxa) { this.salario = salario; this.taxa = taxa; }
    public void AdicionarRecibo(ReciboDeVenda r) { recibos.Add(r); }
    public override decimal CalcularPagamento(Contracheque cc)
    {
        return salario + recibos.Where(r => NoPeriodo(r.Data, cc)).Sum(r => r.Valor * taxa);
    }
}
```

### Passo 5 — Agendas

```cs
public interface AgendaDePagamento
{
    bool EhDiaDePagamento(DateTime dia);
    DateTime InicioDoPeriodo(DateTime fim);
}

public class AgendaSemanal : AgendaDePagamento
{
    public bool EhDiaDePagamento(DateTime dia) { return dia.DayOfWeek == DayOfWeek.Friday; }
    public DateTime InicioDoPeriodo(DateTime fim) { return fim.AddDays(-6); }
}

public class AgendaMensal : AgendaDePagamento
{
    public bool EhDiaDePagamento(DateTime dia) { return dia.Date == UltimoDiaUtil(dia); }
    public DateTime InicioDoPeriodo(DateTime fim) { return new DateTime(fim.Year, fim.Month, 1); }

    private static DateTime UltimoDiaUtil(DateTime d)
    {
        var ultimo = new DateTime(d.Year, d.Month, DateTime.DaysInMonth(d.Year, d.Month));
        while (ultimo.DayOfWeek == DayOfWeek.Saturday || ultimo.DayOfWeek == DayOfWeek.Sunday) ultimo = ultimo.AddDays(-1);
        return ultimo;
    }
}

public class AgendaQuinzenal : AgendaDePagamento
{
    private static readonly DateTime PrimeiraSexta = new DateTime(2025, 1, 3);   // âncora conhecida
    public bool EhDiaDePagamento(DateTime dia)
    {
        return dia.DayOfWeek == DayOfWeek.Friday && ((dia.Date - PrimeiraSexta).Days / 7) % 2 == 0;
    }
    public DateTime InicioDoPeriodo(DateTime fim) { return fim.AddDays(-13); }
}
```

E os testes de agenda mostram a **independência**: `AgendaMensal` paga em 28/11/2025 (sexta, último dia útil), **não** em 29 ou 30; `AgendaSemanal` paga em qualquer sexta.

### Passo 6 — Afiliações (sindicato)

```cs
public interface Afiliacao { decimal CalcularDesconto(Contracheque cc); }

public class AfiliacaoSindicato : Afiliacao
{
    public int IdMembro { get; }
    private readonly decimal mensalidadeSemanal;
    private readonly List<TaxaDeServico> taxas = new List<TaxaDeServico>();
    public AfiliacaoSindicato(int idMembro, decimal mensalidadeSemanal) { IdMembro = idMembro; this.mensalidadeSemanal = mensalidadeSemanal; }
    public void AdicionarTaxa(TaxaDeServico t) { taxas.Add(t); }

    public decimal CalcularDesconto(Contracheque cc)
    {
        int sextas = 0;
        for (var d = cc.Inicio; d <= cc.Fim; d = d.AddDays(1))
            if (d.DayOfWeek == DayOfWeek.Friday) sextas++;
        decimal avulsas = taxas.Where(t => t.Data >= cc.Inicio && t.Data <= cc.Fim).Sum(t => t.Valor);
        return sextas * mensalidadeSemanal + avulsas;
    }
}
```

### Passo 7 — Métodos de pagamento (só registram, por enquanto)

```cs
public interface MetodoDePagamento { void Pagar(Contracheque cc); }
public class MetodoReter : MetodoDePagamento { public void Pagar(Contracheque cc) { /* guarda para retirada */ } }
public class MetodoDeposito : MetodoDePagamento { /* banco, conta */ public void Pagar(Contracheque cc) { } }
public class MetodoCorreio : MetodoDePagamento { /* endereço */ public void Pagar(Contracheque cc) { } }
```

### Passo 8 — A transação mais importante: o dia de pagamento

```cs
public class DiaDePagamento : ITransacao
{
    private readonly DateTime dia;
    private readonly IBancoDeFuncionarios banco;
    public Dictionary<int, Contracheque> Contracheques { get; } = new Dictionary<int, Contracheque>();

    public DiaDePagamento(DateTime dia, IBancoDeFuncionarios banco) { this.dia = dia; this.banco = banco; }

    public void Executar()
    {
        foreach (Funcionario f in banco.Todos())
        {
            if (!f.EhDiaDePagamento(dia)) continue;
            var cc = new Contracheque(f.Agenda.InicioDoPeriodo(dia), dia);
            f.ProcessarPagamento(cc);
            Contracheques[f.Id] = cc;
        }
    }
}
```

O teste de aceitação do cliente (em linguagem de **exemplo**):

```cs
[Test]
public void AssalariadoNoUltimoDiaUtil_RecebeSalarioMenosMensalidade()
{
    var banco = new BancoEmMemoria();
    new AdicionarAssalariado(1, "Bruna", "Rua A", 3000m, banco).Executar();
    new AlterarMembro(1, idMembro: 77, mensalidadeSemanal: 20m, banco).Executar();    // 4 sextas em nov/2025 até dia 28

    var folha = new DiaDePagamento(new DateTime(2025, 11, 28), banco);
    folha.Executar();

    Contracheque cc = folha.Contracheques[1];
    Assert.AreEqual(3000m, cc.Bruto);
    Assert.AreEqual(80m, cc.Descontos);       // 4 sextas × 20
    Assert.AreEqual(2920m, cc.Liquido);
}
```

Todos os passos seguiram o **ciclo do TDD** (teste vermelho → código mínimo → verde → refatorar). Repare no que **não** fizemos: não projetamos banco, não fizemos interface gráfica, não implementamos impostos — **fatiamos** e entregamos **valor funcionando**.

::: passos Como o Dia de Pagamento colabora
**1.** `DiaDePagamento` percorre os funcionários do banco.
===
**2.** Pergunta a cada um: `EhDiaDePagamento(dia)?` — quem responde é a **Agenda** (Strategy).
===
**3.** Para quem é dia: cria o `Contracheque` com o **período** (também dado pela Agenda) e chama `ProcessarPagamento`.
===
**4.** O `Funcionario` pede o **bruto** à **Classificação** e os **descontos** às **Afiliações**; calcula o líquido e entrega ao **Método de Pagamento**.
:::

## Vendo o conjunto

| Padrão | Onde apareceu | Benefício |
|---|---|---|
| **Command** | `ITransacao` (`AdicionarX`, `Excluir`, `CartaoDePonto`, `DiaDePagamento`…) | Cada pedido do usuário é um objeto que valida e executa |
| **Template Method** | `AdicionarFuncionario.Executar()` | O esqueleto da criação fica numa vez só |
| **Strategy** | Classificação, Agenda, Método de pagamento | Trocar o comportamento sem tocar em `Funcionario` (OCP) |
| **Null Object** | Lista de afiliações vazia | Sem `if (afiliacao != null)` |
| **DIP** | `IBancoDeFuncionarios` | O banco é plugável (adiado) |

::: exercicio Estenda o sistema
1. Crie a transação `AlterarParaHorista(id, valorHora)`: troca a **classificação** e a **agenda**, mantendo o resto. O que isso prova sobre a escolha do Strategy?
2. Acrescente uma `AfiliacaoCaridade` (desconto fixo de R$ 10 por pagamento).
3. Escreva um teste em que um comissionado recebe seu salário **mais 10% de duas vendas** (R$ 1.000 e R$ 2.000) feitas no período.

--- resposta
1. `var f = banco.Obter(id); f.Classificacao = new ClassificacaoHorista(valorHora); f.Agenda = new AgendaSemanal();` — **nada em `Funcionario` mudou** e os dados do funcionário (nome, método, afiliações) permanecem. Com subclasses `Horista : Funcionario` isso seria impossível (um objeto não troca de classe): é a vitória do Strategy sobre a herança.
2. `public class AfiliacaoCaridade : Afiliacao { public decimal CalcularDesconto(Contracheque cc) { return 10m; } }` e `f.Afiliacoes.Add(new AfiliacaoCaridade());` — o funcionário pode ter **várias** afiliações ao mesmo tempo.
3. `var c = new ClassificacaoComissionado(2000m, 0.10m); c.AdicionarRecibo(new ReciboDeVenda(data, 1000m)); c.AdicionarRecibo(new ReciboDeVenda(data, 2000m)); Assert.AreEqual(2300m, c.CalcularPagamento(cc));` (2000 + 0,10 × 3000).
:::

::: resumo
- Comece pelo **comportamento** (casos de uso), não pelo banco; o banco é **detalhe**.
- Os casos de uso revelam o que **pode mudar** → use **Strategy** onde há troca em tempo de execução.
- **Abstrações escondidas** (agenda, afiliações) vêm de reler os requisitos e perguntar "isso precisa ser assim?".
- Cada transação é um **Command**; a criação de funcionários usa **Template Method**.
- **TDD** guia cada passo: o teste mostra a intenção e a classe necessária.
- Projetar um pouco por vez: o necessário para a iteração, **nada além**.
:::

::: quiz
? Por que o projeto da folha de pagamentos adia a escolha do banco de dados?
+ Porque o banco é um detalhe de implementação; o essencial é o comportamento
- Porque bancos de dados são proibidos
- Porque não há dados
- Porque o C# não acessa bancos
> Começar pelo banco gera sistemas inseparáveis dele.

? Por que Horista/Assalariado/Comissionado são estratégias em vez de subclasses de Funcionario?
+ Porque um funcionário pode mudar de tipo e um objeto não troca de classe
- Porque subclasses são mais lentas
- Porque Funcionario é abstrata
- Porque C# não tem herança
> Strategy permite trocar o comportamento em tempo de execução.

? Qual abstração não aparecia explicitamente nos casos de uso, mas foi descoberta nos requisitos?
+ A agenda de pagamento (e a lista de afiliações)
- O botão Salvar
- O formulário
- O banco de dados
> Exige ler os requisitos com atenção e questionar associações aparentes.

? Por que separar agenda de classificação de pagamento?
+ Porque as políticas podem mudar independentemente; juntá-las violaria OCP e SRP
- Porque o compilador exige
- Porque agendas são sempre iguais
- Porque classificações não calculam nada
> Mudar uma não deve obrigar a retestar a outra.

? Que padrão representa cada pedido do usuário (AdicionarFuncionario, DiaDePagamento…)?
+ Command
- Singleton
- Observer
- Façade
> Cada transação valida e executa a si mesma.

? O que o uso de uma lista de afiliações elimina?
+ A necessidade de uma classe "SemAfiliação" e de testes de null (Null Object natural)
- A necessidade de testes
- O cálculo do pagamento
- As agendas
> Lista vazia = sem afiliações.

? Qual das práticas ágeis guiou a implementação?
+ TDD: escrever o teste que expressa a intenção antes do código
- Documentação completa antecipada
- Projeto de todo o banco antes de codificar
- Evitar testes de aceitação
> O teste mostra quais classes e métodos são necessários.
:::

::: cartas
Caso de uso :: História elaborada com mais detalhe, usada na análise da iteração atual.
Modelo central (core model) :: Estrutura fundamental (abstrações principais) que guia o restante do sistema.
ClassificacaoDePagamento :: Strategy: como o bruto é calculado (horista, assalariado, comissionado).
AgendaDePagamento :: Strategy: quando e por qual período o funcionário é pago.
MetodoDePagamento :: Strategy: como o contracheque é entregue (reter, depósito, correio).
Afiliacao :: Organização que desconta valores do pagamento (sindicato, caridade…).
Contracheque :: Resultado do cálculo: período, bruto, descontos e líquido.
Transação (Command) :: Objeto que valida e executa uma operação do usuário.
:::
