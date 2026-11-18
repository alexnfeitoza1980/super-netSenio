---
id: agil-cap31
order: 47
kind: capitulo
num: 31
group: C# · Parte 10 · Padrões de projeto (I)
title: Façade, Mediator e Null Object
subtitle: Impor diretrizes por cima (visível) ou por baixo (invisível) — e eliminar a eterna checagem de null com um objeto que "não faz nada".
minutes: 40
hue: 320
objectives:
- Usar **Façade** para esconder uma API complexa atrás de uma interface simples
- Usar **Mediator** para coordenar objetos sem que eles saibam
- Aplicar **Null Object** e saber quando ele substitui `if (x != null)`
- Comparar os três padrões com **Proxy**, **Adapter** e `?.`/`null` do C# moderno
origem: livros/02-projeto-agil-csharp/content/46-cap31.md
---

::: ilustracao img/agil/cap31.svg | Façade simplifica a entrada, Mediator organiza a conversa e Null Object evita o 'se for nulo'.
:::

## Façade e Mediator: impor uma política, de cima ou de baixo

Os dois primeiros padrões têm um objetivo comum: **impor uma política** (*policy*) a um grupo de objetos.

- **Façade** impõe a política **a partir de cima** — de forma **visível e restritiva**.
- **Mediator** impõe a política **a partir de baixo** — de forma **invisível e permissiva**.

## Façade (Fachada)

Use **Façade** quando quiser dar uma **interface simples e específica** a um grupo de objetos com **interface complexa e genérica**.

Exemplo: o acesso a banco de dados com ADO.NET exige conexões, comandos, parâmetros, leitores, transações… Uma **fachada** de produtos esconde tudo isso:

```cs
public class RepositorioDeProdutos            // a fachada
{
    private readonly string conexao;
    public RepositorioDeProdutos(string conexao) { this.conexao = conexao; }

    public void Salvar(Produto p)
    {
        using (var cn = new SqlConnection(conexao))
        using (var cmd = new SqlCommand("INSERT INTO Produtos(Sku, Nome, Preco) VALUES (@sku,@nome,@preco)", cn))
        {
            cmd.Parameters.AddWithValue("@sku", p.Sku);
            cmd.Parameters.AddWithValue("@nome", p.Nome);
            cmd.Parameters.AddWithValue("@preco", p.Preco);
            cn.Open();
            cmd.ExecuteNonQuery();
        }
    }

    public Produto Obter(string sku) { /* abre, consulta, mapeia colunas para Produto, fecha */ return null; }
    public void Excluir(string sku)  { /* ... */ }
}

// quem usa nem sabe que existe SqlConnection:
repositorio.Salvar(new Produto("A-10", "Caneta", 3.5m));
```

::: figura Façade
<svg viewBox="0 0 720 210" role="img" aria-label="Aplicação usa a fachada, que usa várias classes de System.Data">
  <rect class="f-prim" x="10" y="75" width="150" height="56" rx="10"/><text x="85" y="108" text-anchor="middle" font-weight="800">Aplicação</text>
  <path class="arrow" d="M162 103H250"/>
  <rect class="f-good" x="252" y="55" width="190" height="96" rx="12"/><text x="347" y="82" text-anchor="middle" font-weight="800" class="t-good">RepositorioDeProdutos</text><g class="t-mono" font-size="11"><text x="266" y="104">+ Salvar(Produto)</text><text x="266" y="120">+ Obter(sku)</text><text x="266" y="136">+ Excluir(sku)</text></g>
  <rect x="500" y="20" width="210" height="170" rx="12" fill="none" stroke="var(--ink-3)" stroke-dasharray="6 4"/><text x="605" y="42" text-anchor="middle" font-size="12" class="t-dim">System.Data (complexo, genérico)</text>
  <g text-anchor="middle" font-size="12"><rect class="f-plain" x="520" y="55" width="170" height="34" rx="6"/><text x="605" y="77">SqlConnection</text><rect class="f-plain" x="520" y="100" width="170" height="34" rx="6"/><text x="605" y="122">SqlCommand</text><rect class="f-plain" x="520" y="145" width="170" height="34" rx="6"/><text x="605" y="167">SqlDataReader</text></g>
  <path class="arrow" d="M444 80L518 72"/><path class="arrow" d="M444 103L518 117"/><path class="arrow" d="M444 128L518 160"/>
</svg>
:::

A fachada **impõe** várias políticas: sabe **abrir e fechar** a conexão, **mapear** o objeto para colunas e de volta, **montar** as consultas certas. Do ponto de vista da aplicação, `System.Data` **não existe**.

Usar uma fachada significa adotar uma **convenção**: *todas as chamadas ao banco passam por `RepositorioDeProdutos`*. Se alguém no código da aplicação chamar `SqlCommand` diretamente, **viola** a convenção — por isso o Façade é **visível e restritivo**. Como esconder o banco ficou muito comum, o padrão também é conhecido como **Table Data Gateway** (e, hoje, o **Repository** cumpre papel semelhante).

::: dica Façade em outros lugares
Esconder uma biblioteca de e-mail, um SDK de pagamento, uma série de chamadas a serviços internos num único "serviço de aplicação" (`FinalizarCompraService.Executar(pedido)`) — tudo isso é fachada: **uma porta simples** para um **subsistema complexo**.
:::

## Mediator (Mediador)

O **Mediator** também impõe uma política, mas de modo **oculto e não restritivo**. Um objeto fica "nos bastidores" ligando outros, que **nem sabem que ele existe**.

Exemplo clássico: uma **caixa de busca rápida** ligada a uma **lista**. Conforme o usuário digita, o primeiro item da lista que começa com o texto é **selecionado**. Quem usa o `TextBox` e o `ListBox` **não sabe** que há um mediador.

```cs
public class MediadorDeBuscaRapida
{
    private readonly TextBox caixa;
    private readonly ListBox lista;

    public MediadorDeBuscaRapida(TextBox caixa, ListBox lista)
    {
        this.caixa = caixa;
        this.lista = lista;
        caixa.TextChanged += AoMudarTexto;        // se inscreve no evento — e só
    }

    private void AoMudarTexto(object sender, EventArgs e)
    {
        string prefixo = caixa.Text;
        lista.ClearSelected();
        if (prefixo.Length == 0) return;

        for (int i = 0; i < lista.Items.Count; i++)
        {
            if (lista.Items[i].ToString().StartsWith(prefixo, StringComparison.CurrentCultureIgnoreCase))
            {
                lista.SetSelected(i, true);
                return;
            }
        }
    }
}

// uso: crie e esqueça (mas guarde uma referência para o GC não coletá-lo!)
var mediador = new MediadorDeBuscaRapida(txtBusca, lstPaises);
```

Nenhum método a chamar: **cria-se e esquece-se**. O mediador **assina eventos** dos componentes e coordena a reação. A política é um **fato consumado**, não uma questão de convenção.

::: figura Mediator
<svg viewBox="0 0 700 170" role="img" aria-label="TextBox e ListBox não se conhecem; o mediador escuta o TextBox e atualiza o ListBox">
  <rect class="f-info" x="20" y="60" width="150" height="50" rx="10"/><text x="95" y="90" text-anchor="middle" font-weight="700">TextBox</text>
  <rect class="f-good" x="530" y="60" width="150" height="50" rx="10"/><text x="605" y="90" text-anchor="middle" font-weight="700">ListBox</text>
  <rect class="f-warn" x="250" y="10" width="200" height="56" rx="12" stroke-dasharray="6 4"/><text x="350" y="34" text-anchor="middle" font-weight="800" class="t-warn">Mediador</text><text x="350" y="52" text-anchor="middle" font-size="11" class="t-dim">invisível para os dois</text>
  <path class="arrow" d="M172 72L248 44"/><text x="190" y="40" font-size="11">TextChanged</text>
  <path class="arrow" d="M452 44L528 72"/><text x="470" y="40" font-size="11">SetSelected</text>
  <path d="M172 90H528" stroke="var(--line)" stroke-dasharray="3 4" stroke-width="2"/><text x="350" y="108" text-anchor="middle" font-size="11" class="t-dim">(sem ligação direta)</text>
</svg>
:::

### Façade × Mediator

| | **Façade** | **Mediator** |
|---|---|---|
| Direção | De **cima** para o grupo | De **baixo**, no meio do grupo |
| Visibilidade | **Visível**: os clientes a usam | **Invisível**: ninguém sabe |
| Natureza | **Restritiva** (convenção: use-a em vez dos objetos) | **Permissiva** (os objetos continuam normais) |
| Conhece quem? | Conhece os objetos de baixo | Conhece **todos** os colegas que coordena |
| Tipicamente | Subsistema complexo → API simples | Componentes de UI/mensageria que precisam cooperar |

::: pense Cuidado com o mediador "deus"
Um mediador que vai crescendo vira o ponto onde **toda lógica** mora — uma classe deusa disfarçada. Mantenha-o pequeno e com **uma** política clara; senão, divida.
:::

## Null Object (Objeto Nulo)

Quase todos os programadores já escreveram isto:

```cs
Funcionario f = repositorio.Buscar("Bob");
if (f != null && f.EhDiaDePagar(hoje))
    f.Pagar();
```

O `Buscar` devolve `null` se não achar. O idioma é comum — mas é **feio e propenso a erros**: quem nunca se esqueceu de testar? Lançar exceção em vez de retornar `null` ameniza, mas **blocos `try/catch` podem ser ainda mais feios** que um teste de nulo.

### A solução: um objeto que "não faz nada"

O **Null Object** faz `Buscar` **sempre devolver um objeto válido**. Quando não há funcionário, devolve um **funcionário nulo** que implementa **todos os métodos fazendo "nada"** — e o que significa "nada" depende do método.

```cs
public abstract class Funcionario
{
    public abstract bool EhDiaDePagar(DateTime dia);
    public abstract void Pagar();

    public static readonly Funcionario Nulo = new FuncionarioNulo();

    private sealed class FuncionarioNulo : Funcionario
    {
        public override bool EhDiaDePagar(DateTime dia) { return false; }   // nunca é dia de pagar um fantasma
        public override void Pagar() { }                                     // nada a fazer
    }
}

public class FuncionarioReal : Funcionario { /* implementação normal */
    public override bool EhDiaDePagar(DateTime dia) { return true; }
    public override void Pagar() { Console.WriteLine("Pago!"); } }

public class Repositorio
{
    public Funcionario Buscar(string nome) { /* ... */ return Funcionario.Nulo; }   // jamais null
}
```

Agora o código cliente fica **coerente e simples**:

```cs
Funcionario f = repositorio.Buscar("Bob");
if (f.EhDiaDePagar(hoje))      // funciona sempre — sem teste de nulo
    f.Pagar();
```

::: figura Null Object
<svg viewBox="0 0 680 200" role="img" aria-label="Funcionario abstrato com FuncionarioReal e FuncionarioNulo; o repositório cria ambos">
  <rect class="f-info" x="230" y="10" width="220" height="50" rx="10"/><text x="340" y="42" text-anchor="middle" font-weight="800" class="t-info">Funcionario {abstract}</text>
  <path class="line" d="M340 62V90M150 90H530M150 90V112M530 90V112" style="marker-end:none"/>
  <rect class="f-good" x="40" y="112" width="220" height="46" rx="10"/><text x="150" y="140" text-anchor="middle" font-weight="700">FuncionarioReal</text>
  <rect class="f-plain" x="420" y="112" width="220" height="46" rx="10" stroke-dasharray="5 4"/><text x="530" y="140" text-anchor="middle" font-weight="700">FuncionarioNulo (único)</text>
  <text x="530" y="176" text-anchor="middle" font-size="12" class="t-dim">métodos "neutros": false, vazio, 0…</text>
</svg>
:::

### Quando ainda é preciso saber se foi achado

Às vezes você **quer** saber se o funcionário existe. Mantenha **uma única instância** estática do nulo e compare:

```cs
if (f == Funcionario.Nulo) Console.WriteLine("Não encontrado");
```

Tornar `FuncionarioNulo` uma **classe privada aninhada** garante que **só existe uma instância**; senão, o `==` deixaria de ser confiável. Alternativamente, ofereça uma propriedade `EhNulo`.

### Exemplos muito comuns

```cs
// 1) Logger que não registra nada — evita "if (logger != null)" por toda parte
public interface ILogger { void Info(string msg); }
public class LoggerNulo : ILogger { public static readonly LoggerNulo Instancia = new LoggerNulo(); public void Info(string msg) { } }

public class Servico
{
    private readonly ILogger log;
    public Servico(ILogger log = null) { this.log = log ?? LoggerNulo.Instancia; }
    public void Fazer() { log.Info("Fazendo..."); }      // sempre seguro
}

// 2) Coleção vazia em vez de null
public IEnumerable<Pedido> PedidosDe(string cliente) { /* ... */ return Enumerable.Empty<Pedido>(); }

// 3) Estratégia de desconto "sem desconto"
public class SemDesconto : IRegraDeDesconto { public decimal Calcular(decimal v) { return 0m; } }
```

O .NET traz vários: `NullLogger` (Microsoft.Extensions.Logging), `Stream.Null`, `TextWriter.Null`, `Enumerable.Empty<T>()`, `string.Empty`.

::: cuidado Quando NÃO usar
Se **faltar** o objeto é um **erro real** que não pode ser ignorado (por exemplo, "usuário autenticado ausente"), um Null Object **mascara** o problema; prefira exceção. Use o padrão quando **"não há nada a fazer"** é um comportamento **válido**.
:::

### E o C# moderno?

- **Operadores `?.` e `??`**: `f?.Pagar();` e `var nome = cliente?.Nome ?? "(sem nome)";` reduzem muito o ruído.
- **Tipos de referência anuláveis** (`string?`, `#nullable enable`): o compilador **avisa** onde `null` pode aparecer — mudando o jeito de projetar APIs.
- **Padrão `Option`/`Maybe`/`Result`** (bibliotecas): outra forma de tornar a ausência explícita.

Eles **complementam** o Null Object: use `?.` para casos isolados; Null Object quando o **mesmo comportamento neutro** é necessário em muitos lugares.

::: exercicio Aposente um null
Refatore usando Null Object:

```cs
Desconto d = promocoes.Buscar(codigo);
decimal valor = bruto;
if (d != null)
    valor = d.Aplicar(bruto);
```

--- resposta
```cs
public abstract class Desconto
{
    public abstract decimal Aplicar(decimal bruto);
    public static readonly Desconto Nenhum = new DescontoNulo();
    private sealed class DescontoNulo : Desconto { public override decimal Aplicar(decimal bruto) { return bruto; } }
}

// Buscar devolve Desconto.Nenhum quando não existe:
decimal valor = promocoes.Buscar(codigo).Aplicar(bruto);   // sem if
```
O "nada" aqui é **devolver o valor sem alteração** — repare como o comportamento neutro depende do método.
:::

::: resumo
- **Façade:** interface **simples e específica** sobre um subsistema complexo; impõe a política **por cima**, **visível** e restritiva (convenção: use a fachada).
- **Mediator:** coordena objetos **por baixo**, **invisível** e permissivo; "crie e esqueça".
- **Null Object:** retorna sempre um objeto válido cujo comportamento "nada" é neutro; elimina `if (x != null)`.
- Garanta **instância única** do nulo se precisar comparar; evite quando a ausência é um erro que não pode ser ignorado.
- No C# moderno, combine com `?.`, `??` e *nullable reference types*.
:::

::: quiz
? Qual a principal finalidade do Façade?
+ Oferecer uma interface simples e específica para um subsistema complexo
- Coordenar eventos entre componentes sem que saibam
- Criar objetos
- Substituir null
> Esconde a complexidade atrás de uma porta única.

? Qual é a diferença de visibilidade entre Façade e Mediator?
+ Façade é visível e restritiva; Mediator é invisível e permissiva
- Façade é invisível; Mediator é visível
- Ambos são invisíveis
- Ambos são restritivos
> Todos usam a fachada; ninguém precisa saber do mediador.

? No exemplo da busca rápida, como o mediador sabe que o texto mudou?
+ Assina o evento TextChanged do TextBox
- O TextBox chama o mediador diretamente
- Usa um Timer
- Lê o disco
> Eventos permitem coordenar sem acoplar os componentes.

? O que o Null Object garante?
+ Que a função sempre retorna um objeto válido, que "não faz nada" quando há falha
- Que o objeto nunca é criado
- Que exceções são lançadas
- Que o código fica mais rápido sempre
> O cliente não precisa testar null.

? Por que o objeto nulo costuma ser uma única instância?
+ Para poder compará-lo de forma confiável (`x == Tipo.Nulo`)
- Porque o C# proíbe várias instâncias
- Porque economiza disco
- Porque evita herança
> Várias instâncias quebrariam a comparação.

? Em qual caso um Null Object NÃO é apropriado?
+ Quando a ausência do objeto é um erro que deve ser tratado
- Quando "nada a fazer" é válido
- Quando há muitos clientes
- Quando há testes
> Ele pode mascarar problemas reais.

? Qual recurso do C# moderno reduz a necessidade de checar null de forma explícita em casos isolados?
+ Os operadores `?.` e `??`
- `goto`
- `static`
- `unsafe`
> Complementam o padrão Null Object.
:::

::: cartas
Façade :: Interface simples sobre um subsistema complexo; impõe política visível, por cima.
Mediator :: Objeto que coordena outros por baixo, sem que eles saibam.
Null Object :: Objeto neutro que implementa a interface "fazendo nada", evitando checagens de null.
Table Data Gateway :: Façade que esconde o acesso a banco de dados de uma tabela.
Política (policy) :: Regra ou convenção imposta a um grupo de objetos.
`?.` / `??` :: Operadores C# de navegação segura e de valor padrão para null.
:::
