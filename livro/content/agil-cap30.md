---
id: agil-cap30
order: 46
kind: capitulo
num: 30
group: C# · Parte 10 · Padrões de projeto (I)
title: Template Method e Strategy
subtitle: Herança versus delegação — dois jeitos de separar um algoritmo genérico dos detalhes, com seus custos, benefícios e o perigo do abuso de padrões.
minutes: 50
hue: 320
objectives:
- Explicar **Template Method** (herança) e **Strategy** (delegação) e quando usar cada um
- Ver como ambos **separam o algoritmo genérico do contexto detalhado** (OCP/DIP)
- Comparar **custo × flexibilidade** e reconhecer o **abuso de padrão**
- Reconhecer os dois padrões em C#: `abstract`/`virtual`, `IComparer`, delegados e LINQ
origem: livros/02-projeto-agil-csharp/content/45-cap30.md
---

::: ilustracao img/agil/cap30.svg | Template Method fixa o esqueleto; Strategy troca o passo variável.
:::

## Herança × delegação

No início dos anos 1990, a herança empolgou todo mundo: podíamos **programar pela diferença** — dada uma classe quase útil, criar uma subclasse e mudar só o que interessava. Reutilização de código "de graça". Por volta de 1995 ficou claro que era **fácil e caro** abusar disso; os autores de *Design Patterns* resumiram: **favoreça a composição de objetos em detrimento da herança de classe**.

Este capítulo conta a história de **dois padrões que simbolizam essa diferença**. Ambos resolvem **o mesmo problema** — separar um **algoritmo genérico** de um **contexto detalhado** — e muitas vezes são intercambiáveis:

| | **Template Method** | **Strategy** |
|---|---|---|
| Mecanismo | **Herança** | **Delegação** (composição) |
| O algoritmo genérico fica… | Num **método** da **classe base abstrata** | Numa **classe concreta** que usa uma **interface** |
| Os detalhes ficam… | Em **métodos abstratos**, preenchidos pelas **subclasses** | Em **implementações da interface**, passadas ao algoritmo |
| Acoplamento | Subclasse **presa** à base | Detalhe e algoritmo **independentes** |

Ambos servem ao **DIP**: queremos que o algoritmo genérico **não dependa** da implementação detalhada; os dois dependem de **abstrações**.

## Template Method

Muitos programas têm a mesma estrutura fundamental:

```cs
Inicializar();
while (!Terminou())
    FazerAlgoUtil();
Finalizar();
```

O **Template Method** coloca **todo o código genérico** num método **concreto** de uma **classe base abstrata** e **transfere os detalhes** para **métodos abstratos**:

```cs
public abstract class Aplicacao
{
    private bool terminou;

    protected abstract void Iniciar();
    protected abstract void Repetir();
    protected abstract void Encerrar();
    protected void MarcarComoTerminada() { terminou = true; }

    public void Executar()            // ← o método-gabarito (template method)
    {
        Iniciar();
        while (!terminou) Repetir();
        Encerrar();
    }
}

public class ConversorDeTemperatura : Aplicacao
{
    protected override void Iniciar()   { Console.WriteLine("Digite temperaturas em °F (vazio para sair):"); }
    protected override void Repetir()
    {
        string linha = Console.ReadLine();
        if (string.IsNullOrEmpty(linha)) { MarcarComoTerminada(); return; }
        double f = double.Parse(linha);
        Console.WriteLine($"{f} °F = {(f - 32) * 5 / 9:F1} °C");
    }
    protected override void Encerrar()  { Console.WriteLine("Até logo!"); }
}

// uso:
new ConversorDeTemperatura().Executar();
```

::: figura Template Method
<svg viewBox="0 0 700 220" role="img" aria-label="Classe base com método Executar e passos abstratos; subclasses implementam">
  <rect class="f-prim" x="190" y="10" width="320" height="104" rx="10"/><text x="350" y="32" text-anchor="middle" font-weight="800" class="t-prim">Aplicacao {abstract}</text>
  <path class="line" d="M190 40H510"/><g class="t-mono" font-size="12"><text x="204" y="62" font-weight="700">+ Executar()  ← algoritmo genérico</text><text x="204" y="82"># Iniciar() {A}</text><text x="204" y="98"># Repetir() {A}   # Encerrar() {A}</text></g>
  <path class="line" d="M350 116V140M170 140H530M170 140V160M530 140V160" style="marker-end:none"/>
  <rect class="f-good" x="70" y="160" width="200" height="46" rx="8"/><text x="170" y="188" text-anchor="middle" font-weight="700">ConversorDeTemperatura</text>
  <rect class="f-good" x="430" y="160" width="200" height="46" rx="8"/><text x="530" y="188" text-anchor="middle" font-weight="700">ProcessadorDeArquivos</text>
</svg>
:::

### Abuso de padrão: quando o custo supera o benefício

Pare e pergunte-se: *"Alguém realmente usaria uma classe `Aplicacao` em todo programa novo?"* Para o conversor acima, usar Template Method é **ridículo**: complica e **aumenta** um programa de 12 linhas. O exemplo existe para mostrar a **mecânica**. A lição:

::: cuidado Padrões não são obrigatórios
Os padrões são maravilhosos para muitos problemas — mas o fato de **existirem** não significa que **sempre** devem ser usados. Aqui o padrão era **aplicável**, não **aconselhável**: seu **custo** superava seu benefício. Aplique um padrão **quando houver dor** que ele resolva.
:::

### Um exemplo mais útil: relatórios

Gerar relatórios tem **sempre** a mesma ordem — cabeçalho, corpo, rodapé — mas o conteúdo muda:

```cs
public abstract class Relatorio
{
    public string Gerar()
    {
        var sb = new StringBuilder();
        sb.AppendLine(Cabecalho());
        foreach (string linha in Linhas()) sb.AppendLine(FormatarLinha(linha));
        sb.AppendLine(Rodape());
        return sb.ToString();
    }

    protected abstract string Cabecalho();
    protected abstract IEnumerable<string> Linhas();
    protected virtual string FormatarLinha(string l) { return l; }     // gancho (hook) com padrão
    protected virtual string Rodape() { return $"Gerado em {DateTime.Now:dd/MM/yyyy}"; }
}

public class RelatorioDeVendas : Relatorio
{
    protected override string Cabecalho() { return "== VENDAS DO MÊS =="; }
    protected override IEnumerable<string> Linhas() { yield return "Notebook — R$ 4.500"; yield return "Mouse — R$ 90"; }
}
```

O **algoritmo** (a ordem dos passos) está **uma vez só**, na base; cada relatório fornece **apenas o que varia**. Métodos `virtual` com implementação padrão são **ganchos** (*hooks*) opcionais.

## Ordenação por bolhas com Template Method

Outro exemplo clássico: separar o **algoritmo** de ordenar do **tipo de dado**. O algoritmo conhece apenas duas perguntas abstratas — *"o par no índice i está fora de ordem?"* e *"troque o par"* —, sem saber o que há no array:

```cs
public abstract class OrdenadorBolha
{
    protected int tamanho;

    protected int Ordenar()                         // algoritmo genérico (template method)
    {
        int operacoes = 0;
        for (int fim = tamanho - 2; fim >= 0; fim--)
            for (int i = 0; i <= fim; i++)
            {
                if (ForaDeOrdem(i)) Trocar(i);
                operacoes++;
            }
        return operacoes;
    }

    protected abstract bool ForaDeOrdem(int indice);
    protected abstract void Trocar(int indice);
}

public class OrdenadorDeInteiros : OrdenadorBolha
{
    private int[] vetor;
    public int Ordenar(int[] v) { vetor = v; tamanho = v.Length; return Ordenar(); }
    protected override bool ForaDeOrdem(int i) { return vetor[i] > vetor[i + 1]; }
    protected override void Trocar(int i) { (vetor[i], vetor[i + 1]) = (vetor[i + 1], vetor[i]); }
}
```

Dá para criar `OrdenadorDeDoubles`, `OrdenadorDeNomes`… cada um preenche os dois métodos. É uma das **formas clássicas de reutilização** da OO: algoritmos genéricos na base, herdados por vários contextos.

### O custo da herança

Mas **herança é um relacionamento muito forte**: as derivadas ficam **inextricavelmente ligadas** à base. Os métodos `ForaDeOrdem` e `Trocar` de `OrdenadorDeInteiros` são **exatamente o que outros algoritmos de ordenação** (quicksort, seleção…) precisariam — porém **não dá para reaproveitá-los**: herdando de `OrdenadorBolha`, `OrdenadorDeInteiros` está **condenado** a ser bolha para sempre. O Strategy oferece outro caminho.

## Strategy

O **Strategy** resolve o mesmo problema — inverter as dependências entre o algoritmo genérico e a implementação detalhada — **de outra forma**:

1. O **algoritmo genérico** vai para uma **classe concreta** (um "executor").
2. Os **passos de que ele precisa** são declarados numa **interface**.
3. As **implementações** da interface são **passadas** ao executor, que **delega** para elas.

```cs
public interface IAplicacao
{
    void Iniciar();
    void Repetir();
    void Encerrar();
    bool Terminou { get; }
}

public class ExecutorDeAplicacao
{
    private readonly IAplicacao app;
    public ExecutorDeAplicacao(IAplicacao app) { this.app = app; }

    public void Executar()
    {
        app.Iniciar();
        while (!app.Terminou) app.Repetir();
        app.Encerrar();
    }
}

public class ConversorStrategy : IAplicacao { /* implementa os quatro membros */ }

new ExecutorDeAplicacao(new ConversorStrategy()).Executar();
```

### A ordenação com Strategy

```cs
public interface IManipuladorDeOrdenacao
{
    void DefinirVetor(object vetor);
    int Tamanho { get; }
    bool ForaDeOrdem(int indice);
    void Trocar(int indice);
}

public class OrdenadorBolha
{
    private readonly IManipuladorDeOrdenacao m;
    public OrdenadorBolha(IManipuladorDeOrdenacao m) { this.m = m; }

    public int Ordenar(object vetor)
    {
        m.DefinirVetor(vetor);
        int operacoes = 0;
        for (int fim = m.Tamanho - 2; fim >= 0; fim--)
            for (int i = 0; i <= fim; i++)
            {
                if (m.ForaDeOrdem(i)) m.Trocar(i);
                operacoes++;
            }
        return operacoes;
    }
}

// outro algoritmo, a MESMA interface:
public class OrdenadorBolhaRapido { /* para cedo se uma passada não troca nada */ }
public class ManipuladorDeInteiros : IManipuladorDeOrdenacao { /* ... */ }
```

`ManipuladorDeInteiros` **nada sabe** sobre `OrdenadorBolha`: pode ser usado com **qualquer** algoritmo que dependa da interface (como `OrdenadorBolhaRapido`).

::: figura Strategy
<svg viewBox="0 0 700 220" role="img" aria-label="Executor usa interface; várias estratégias a implementam">
  <rect class="f-prim" x="20" y="80" width="200" height="60" rx="10"/><text x="120" y="108" text-anchor="middle" font-weight="800">OrdenadorBolha</text><text x="120" y="126" text-anchor="middle" font-size="12" class="t-mono">Ordenar(vetor)</text>
  <path class="arrow" d="M222 110H300"/>
  <rect class="f-info" x="302" y="85" width="200" height="50" rx="10"/><text x="402" y="104" text-anchor="middle" font-size="11" class="t-dim">«interface»</text><text x="402" y="124" text-anchor="middle" font-weight="800" class="t-info">IManipulador…</text>
  <path class="line" d="M402 137V160M330 160H560M330 160V178M560 160V178" style="marker-end:none"/>
  <rect class="f-good" x="250" y="178" width="160" height="36" rx="8"/><text x="330" y="201" text-anchor="middle" font-size="13">ManipuladorInteiros</text>
  <rect class="f-good" x="480" y="178" width="160" height="36" rx="8"/><text x="560" y="201" text-anchor="middle" font-size="13">ManipuladorTextos</text>
  <rect class="f-prim" x="480" y="20" width="200" height="50" rx="10" stroke-dasharray="5 4"/><text x="580" y="50" text-anchor="middle" font-size="13">OrdenadorBolhaRapido</text>
  <path class="arrow" d="M540 72L470 100" stroke-dasharray="4 3"/>
</svg>
:::

## Comparando: custos e benefícios

**Strategy** tem **mais classes** e **mais indireção**; o ponteiro de delegação custa um pouco mais de espaço e tempo que a herança. Por outro lado, se há muitos aplicativos a executar, **uma única instância** do executor pode ser reutilizada com **várias** implementações. Nenhum dos custos domina; no caso típico o que mais pesa é a **classe extra**. Mas há algo mais importante:

::: regra Strategy cumpre o DIP por inteiro
**Template Method** permite que **um algoritmo genérico** manipule **muitas implementações** detalhadas. **Strategy** permite isso **e também** que **cada implementação detalhada** seja manipulada por **muitos algoritmos genéricos**. No Template Method, `ForaDeOrdem`/`Trocar` dependem **diretamente** da classe base (o algoritmo de bolha) — violação **parcial** do DIP.
:::

| Critério | Template Method | Strategy |
|---|---|---|
| Simplicidade | ✅ Mais fácil de escrever e usar | ❌ Classe extra, objeto extra, ligação |
| Flexibilidade em tempo de execução | ❌ Escolhido ao **criar** a subclasse | ✅ Pode **trocar** a estratégia ao longo da execução |
| Reuso dos detalhes | ❌ Presos à base | ✅ Reutilizáveis com outros algoritmos |
| Testabilidade | Boa (subclasse de teste) | Excelente (basta um falso da interface) |
| Risco de herança frágil | Maior | Menor |

::: dica Regra prática
Muitas vezes o autor opta pelo **Template Method** simplesmente porque é **mais fácil de implementar e usar**, a não ser que tenha **certeza** de que precisará de várias implementações do algoritmo (aí, **Strategy**). Comece simples; migre para Strategy quando a rigidez **doer**.
:::

## Onde você já os usa em C#

**Template Method:**

- `Stream`, `Controller`, `Form` — os métodos `virtual`/`abstract` que você sobrescreve (`OnPaint`, `Dispose(bool)`, `Setup`/`TearDown` em testes) são **ganchos** de um algoritmo definido pelo framework.
- Testes NUnit/xUnit: o framework **orquestra** (`SetUp` → teste → `TearDown`).

**Strategy:**

- `IComparer<T>` / `Comparison<T>` em `List<T>.Sort(...)` — o algoritmo de ordenação (genérico) usa **sua** regra de comparação.
- LINQ: `OrderBy(x => x.Nome)`, `Where(x => ...)` — **lambdas** são estratégias.
- Injeção de dependência: qualquer `ILogger`, `IDescontoStrategy`, `IFormatadorDeMoeda`.

```cs
// Strategy "leve": delegados
public class Pedido
{
    private readonly Func<decimal, decimal> calcularDesconto;
    public Pedido(Func<decimal, decimal> calcularDesconto) { this.calcularDesconto = calcularDesconto; }
    public decimal Total(decimal bruto) { return bruto - calcularDesconto(bruto); }
}

var promocional = new Pedido(v => v * 0.10m);
var fidelidade  = new Pedido(v => v > 500 ? v * 0.15m : 0m);
```

::: exercicio Qual padrão?
Escolha **Template Method** ou **Strategy** e justifique:

1. Um framework de importação de arquivos: abrir → ler registros → validar → gravar → fechar, mas cada formato (CSV, XML) lê e valida de um jeito. Você controla o framework e os formatos serão poucos e fixos.
2. Um motor de preços que precisa **trocar a regra de desconto em tempo de execução**, conforme o cliente.
3. Um algoritmo de ordenação que deve funcionar com vários critérios e vários tipos de dado, escolhidos em tempo de execução.

--- resposta
1. **Template Method**: o fluxo é fixo, poucas variações conhecidas, e a simplicidade pesa (subclasses `ImportadorCsv`, `ImportadorXml`).
2. **Strategy** (interface `IRegraDeDesconto` ou `Func<decimal,decimal>`): precisa **trocar em execução**, o que a herança não permite bem.
3. **Strategy** (`IComparer<T>`/lambda): critérios e tipos variam independentemente do algoritmo; os comparadores são reutilizáveis com outros algoritmos.
:::

::: resumo
- **Template Method** (herança): o algoritmo genérico fica num método da **base abstrata**; subclasses preenchem **métodos abstratos/ganchos**. Simples, mas **rígido** (detalhes presos à base).
- **Strategy** (delegação): o algoritmo usa uma **interface**; as implementações são **injetadas**. Mais **flexível** e cumpre o **DIP** por completo, ao custo de mais classes e indireção.
- Ambos separam **política genérica** de **detalhe** e favorecem o **OCP**.
- **Abuso de padrão**: aplicável ≠ aconselhável; só use quando o benefício superar o custo.
- Em C#: `virtual/abstract` (Template Method); `IComparer`, delegados, lambdas e DI (Strategy).
:::

::: quiz
? Qual mecanismo o Template Method usa?
+ Herança
- Delegação
- Reflexão
- Classes estáticas
> A classe base define o algoritmo; subclasses completam os passos.

? Qual mecanismo o Strategy usa?
+ Delegação para uma interface injetada
- Herança de classe concreta
- Variáveis globais
- Enumerações
> O algoritmo delega a objetos que implementam a interface.

? Qual é a principal vantagem do Strategy sobre o Template Method?
+ A implementação detalhada fica independente do algoritmo e pode ser usada com vários algoritmos
- Menos classes
- Menos indireção
- Não precisa de interfaces
> Cumpre o DIP por inteiro.

? Por que usar Template Method para um programa de 12 linhas é "abuso de padrão"?
+ O custo (classes, complexidade) supera o benefício
- Porque o padrão não funciona em C#
- Porque herança é proibida
- Porque é inseguro
> Aplicável não significa aconselhável.

? O que é um "gancho" (hook) em um Template Method?
+ Um método virtual com implementação padrão que subclasses podem sobrescrever opcionalmente
- Uma exceção
- Um evento
- Um delegado estático
> Os abstratos são obrigatórios; os ganchos, opcionais.

? Qual das opções é uma aplicação de Strategy em C#?
+ `list.Sort(new MeuComparador())`
- `class B : A`
- `static class Util`
- `switch` com enum
> O algoritmo de ordenação recebe a regra de comparação.

? Quando preferir Strategy?
+ Quando é preciso trocar a variação em tempo de execução ou reutilizar os detalhes com outros algoritmos
- Quando o fluxo é fixo e simples
- Quando não há testes
- Quando se quer evitar interfaces
> Flexibilidade tem preço: classes e objetos extras.
:::

::: cartas
Template Method :: Algoritmo genérico num método da classe base; passos variáveis são métodos abstratos/virtuais.
Strategy :: Algoritmo que delega passos a uma interface; implementações intercambiáveis.
Gancho (hook) :: Método virtual opcional num Template Method.
Programar pela diferença :: Criar subclasse que altera só o que difere da base.
Composição sobre herança :: Preferir combinar objetos a herdar classes.
Abuso de padrão :: Usar um padrão quando seu custo supera o benefício.
DIP (via Strategy) :: Algoritmo e detalhe dependem da mesma abstração.
:::
