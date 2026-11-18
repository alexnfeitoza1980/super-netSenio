---
id: agil-lab6
order: 57
kind: lab
num: 6
group: C# · Parte 11 · Pacotes, padrões e arquitetura
title: Laboratório 6 — Folha de pagamentos: pacotes e banco de dados
subtitle: Organizar o sistema em componentes com os princípios de pacotes (e medir com I, A e D′), e então trocar o dicionário por um banco SQL real — guiado por testes, com transações.
minutes: 120
hue: 345
badge: L6
objectives:
- **Dividir** um sistema em componentes aplicando **CCP**, **REP/CRP**, **ADP**, **SDP** e **SAP**
- **Medir** estabilidade (I), abstração (A) e distância da sequência principal (D′)
- Trocar o armazenamento em memória por **SQL** sem quebrar nada (**DIP** + **Repository**)
- Salvar com **transações**, carregar **sem tocar no banco** nos testes, e matar os **if/else** que ferem o OCP
origem: livros/02-projeto-agil-csharp/content/56-lab6.md
---

::: vila A empresa de Hilda cresce
Na iteração 1 (Laboratório 5), dois programadores bastavam: todas as classes num diretório só. Agora a empresa de Hilda contratou mais gente — e precisa de **persistência de verdade**. Duas decisões batem à porta ao mesmo tempo: **como organizar o código** para vários desenvolvedores e **como guardar os dados** sem contaminar as regras de negócio.
:::

::: ilustracao img/agil/lab6.svg | Pacotes e banco de dados: o domínio fala com o banco só por uma porta bem definida.
:::

## Parte 1 — Análise dos pacotes

### O ponto de partida

O sistema tem cerca de **60 classes** em **80 arquivos**, tudo numa pasta. Não é muito, mas já é uma **carga organizacional**: quem edita o quê? Como entregar partes **independentemente**? Precisamos de **componentes** (assemblies/DLLs/projetos) que possam ser trabalhados, testados e versionados **separadamente**.

Uma primeira tentativa — agrupar "o que parece caber junto":

::: figura Estrutura ingênua (agrupada por intuição)
<svg viewBox="0 0 720 300" role="img" aria-label="Aplicação depende de Transações, que depende de Banco, que depende de Métodos, Agendas, Classificações e Afiliações">
  <rect class="f-prim" x="270" y="10" width="180" height="40" rx="8"/><text x="360" y="35" text-anchor="middle" font-weight="800">PayrollApplication</text>
  <rect class="f-info" x="270" y="80" width="180" height="40" rx="8"/><text x="360" y="105" text-anchor="middle" font-weight="800">Transacoes</text>
  <rect class="f-warn" x="270" y="150" width="180" height="40" rx="8"/><text x="360" y="175" text-anchor="middle" font-weight="800">Banco + Funcionario</text>
  <rect class="f-plain" x="20" y="230" width="150" height="40" rx="8"/><text x="95" y="255" text-anchor="middle">Metodos</text>
  <rect class="f-plain" x="200" y="230" width="150" height="40" rx="8"/><text x="275" y="255" text-anchor="middle">Agendas</text>
  <rect class="f-plain" x="380" y="230" width="150" height="40" rx="8"/><text x="455" y="255" text-anchor="middle">Classificacoes</text>
  <rect class="f-plain" x="560" y="230" width="150" height="40" rx="8"/><text x="635" y="255" text-anchor="middle">Afiliacoes</text>
  <path class="arrow" d="M360 52V78"/><path class="arrow" d="M360 122V148"/>
  <path class="arrow" d="M320 192L110 228"/><path class="arrow" d="M340 192L260 228"/><path class="arrow" d="M380 192L440 228"/><path class="arrow" d="M400 192L610 228"/>
</svg>
:::

O que está errado? Mude **uma só** classificação (digamos, `ClassificacaoHorista`):

- O componente **Classificacoes** muda — correto.
- **Banco** recompila e retesta — correto, depende dele.
- **Transacoes** inteiro recompila e retesta — *mas só um punhado de transações* dependia disso.
- **PayrollApplication** também.

As classes de `Transacoes` **não compartilham o mesmo fechamento**: `TransacaoDeTaxaDeServico` reage a mudanças de taxa de serviço; `TransacaoDeCartao` reage a cartão de ponto. Uma parte do componente depende de **quase tudo** — então ele terá **ritmo de entregas altíssimo**. E os "detalhes" (métodos, agendas…) estão **embaixo**, de que todos dependem: o lugar errado para detalhes.

### Refazendo com o CCP: agrupar pelo que muda junto

Aplique o **Princípio do Fechamento Comum**: classes que mudam pelos mesmos motivos ficam **juntas**.

::: figura Estrutura fechada (CCP + DIP)
<svg viewBox="0 0 740 330" role="img" aria-label="Detalhes no topo dependem do domínio abstrato; a implementação do banco depende da interface do banco">
  <rect class="f-good" x="10" y="10" width="112" height="54" rx="8"/><text x="66" y="34" text-anchor="middle" font-size="13" font-weight="800">Parser de texto</text>
  <text x="66" y="52" text-anchor="middle" font-size="9" class="t-dim">fonte de transações</text>
  <path d="M66 64V96" stroke="var(--ink-2)" fill="none"/>
  <rect class="f-good" x="130" y="10" width="112" height="54" rx="8"/><text x="186" y="34" text-anchor="middle" font-size="13" font-weight="800">Aplicacao</text>
  <text x="186" y="52" text-anchor="middle" font-size="9" class="t-dim">main + liga tudo</text>
  <path d="M186 64V96" stroke="var(--ink-2)" fill="none"/>
  <rect class="f-good" x="250" y="10" width="112" height="54" rx="8"/><text x="306" y="34" text-anchor="middle" font-size="13" font-weight="800">Classificacoes</text>
  <text x="306" y="52" text-anchor="middle" font-size="9" class="t-dim">+ cartões, recibos</text>
  <path d="M306 64V96" stroke="var(--ink-2)" fill="none"/>
  <rect class="f-good" x="370" y="10" width="112" height="54" rx="8"/><text x="426" y="42" text-anchor="middle" font-size="13" font-weight="800">Metodos</text>
  <path d="M426 64V96" stroke="var(--ink-2)" fill="none"/>
  <rect class="f-good" x="490" y="10" width="112" height="54" rx="8"/><text x="546" y="42" text-anchor="middle" font-size="13" font-weight="800">Agendas</text>
  <path d="M546 64V96" stroke="var(--ink-2)" fill="none"/>
  <rect class="f-good" x="610" y="10" width="112" height="54" rx="8"/><text x="666" y="42" text-anchor="middle" font-size="13" font-weight="800">Afiliacoes</text>
  <path d="M666 64V96" stroke="var(--ink-2)" fill="none"/>
  <path d="M66 96H666" stroke="var(--ink-2)" fill="none"/>
  <path class="arrow" d="M300 96V138"/>
  <rect class="f-info" x="190" y="140" width="220" height="62" rx="8"/><text x="300" y="166" text-anchor="middle" font-weight="800">DominioDaFolha</text><text x="300" y="186" text-anchor="middle" font-size="10" class="t-dim">Funcionario, abstrações</text>
  <rect class="f-info" x="470" y="140" width="220" height="62" rx="8"/><text x="580" y="166" text-anchor="middle" font-weight="800">BancoDaFolha</text><text x="580" y="186" text-anchor="middle" font-size="10" class="t-dim">interface IBancoDeFuncionarios</text>
  <path class="arrow" d="M468 171H412"/>
  <rect class="f-warn" x="470" y="250" width="220" height="50" rx="8"/><text x="580" y="281" text-anchor="middle" font-size="14" font-weight="800">ImplementacaoDoBanco</text>
  <path class="arrow" d="M580 250V204"/>
  <text x="150" y="250" font-size="11" class="t-dim">Setas = "depende de".</text><text x="150" y="268" font-size="11" class="t-dim">Detalhes (topo) dependem de abstrações.</text>
</svg>
:::

Observe a **inversão** — a marca de um bom projeto OO:

- Os **detalhes** (classificações, métodos, agendas, afiliações, o parser, a implementação do banco) ficam **em cima**, **dependendo** das abstrações. **Ninguém depende deles**: são *irresponsáveis* (mudar lá não afeta ninguém), logo **flexíveis**.
- As **abstrações** (`Funcionario`, `ClassificacaoDePagamento`, `AgendaDePagamento`, `ITransacao`, `IBancoDeFuncionarios`) ficam **embaixo**, **dependendo de nada** (*independentes*) e com muita gente dependendo delas (*responsáveis*). Contêm **pouco código**, o que é bom: a parte difícil de mudar é pequena.
- O que o componente `DominioDaFolha` tem de extraordinário: **a essência do sistema, e depende de nada** — porque só contém classes **abstratas/interfaces**.

### REP e CRP: reutilizar = entregar junto

Quem poderia reutilizar o quê?

- Outro departamento com **regras de pagamento diferentes** reaproveita `DominioDaFolha`, `Aplicacao`, `BancoDaFolha` — mas **não** classificações/agendas/afiliações.
- Outro time querendo **analisar** a base de funcionários reaproveita domínio, classificações, métodos, agendas, afiliações e banco.

Em todos os casos, **o grão da reutilização é o componente**. Na estrutura ingênua isso seria impossível: `Transacoes` e `PayrollApplication` arrastam *tudo*. Quem pegasse "só um pedaço" quebraria a estrutura de versões e acabaria **copiando código** (que não é reutilização).

Mas olhe de novo: em `DominioDaFolha` há `ITransacao`, que **não precisa** viajar com `Funcionario` — alguém pode querer acessar funcionários **sem** nenhuma transação. Pelo **CRP** (reutilizadas juntas), separamos:

::: figura Refinando com o CRP
<svg viewBox="0 0 720 190" role="img" aria-label="ITransacao sai do domínio para um componente AplicacaoDeTransacoes reutilizável">
  <rect class="f-info" x="10" y="50" width="210" height="70" rx="8"/><text x="115" y="78" text-anchor="middle" font-size="15" font-weight="800">AplicacaoDeTransacoes</text><text x="115" y="100" text-anchor="middle" font-size="10" class="t-dim">ITransacao, fonte, laço principal</text>
  <rect class="f-good" x="255" y="50" width="210" height="70" rx="8"/><text x="360" y="78" text-anchor="middle" font-size="15" font-weight="800">AplicacaoDaFolha</text><text x="360" y="100" text-anchor="middle" font-size="10" class="t-dim">main + liga o parser</text>
  <rect class="f-info" x="500" y="50" width="210" height="70" rx="8"/><text x="605" y="78" text-anchor="middle" font-size="15" font-weight="800">DominioDaFolha</text><text x="605" y="100" text-anchor="middle" font-size="10" class="t-dim">Funcionario e abstrações</text>
  <path class="arrow" d="M253 85H222"/><path class="arrow" d="M467 85H498" style="opacity:.6"/>
  <text x="360" y="165" text-anchor="middle" class="t-dim" font-size="12">Mais camadas = mais reuso e manutenção… ao custo de mais componentes.</text>
</svg>
:::

::: pense Vale a pena?
A separação aumenta a reutilização e a manutenibilidade, mas custa **mais componentes** e **dependências mais complexas**. Se o app é estável e pouca gente o reaproveita, é exagero. **Comece simples** e refine quando **dados** (não especulação) mostrarem a necessidade: estruturas de componentes sempre podem ficar mais elaboradas depois.
:::

### Encapsulamento entre componentes

Em C# controlamos o acoplamento entre componentes com **visibilidade**: o que outros componentes usam é `public`; o que é **privativo do componente** deve ser **`internal`**. Em `Classificacoes`, por exemplo, `CartaoDePonto` e `ReciboDeVenda` são **detalhes** de como pagamentos são calculados — bons candidatos a `internal`, limitando o **acoplamento aferente** do componente.

```cs
namespace Folha.Classificacoes
{
    public class ClassificacaoHorista : ClassificacaoDePagamento { /* ... */ }  // usada por outros
    internal class CartaoDePonto { /* ... */ }                                  // só aqui dentro
}
```

Mas se as transações que lançam cartões vivem **em outro componente**, elas precisam enxergá-los. Duas saídas: **mover** essas transações para o mesmo componente (CCP!) ou usar `InternalsVisibleTo`. A primeira é, em geral, mais honesta.

### Medindo: I, A e D′

Calcule as métricas para os componentes (valores ilustrativos de um cenário plausível):

| Componente | Ca | Ce | **I** = Ce/(Ca+Ce) | Classes (Na/Nc) | **A** | **D′** = \|A + I − 1\| |
|---|---|---|---|---|---|---|
| `DominioDaFolha` | 6 | 0 | **0,00** | 6 / 7 | **0,86** | \|0,86 + 0 − 1\| = **0,14** |
| `BancoDaFolha` | 3 | 1 | 0,25 | 1 / 1 | **1,00** | \|1 + 0,25 − 1\| = 0,25 |
| `Classificacoes` | 1 | 2 | 0,67 | 0 / 8 | **0,00** | \|0 + 0,67 − 1\| = 0,33 |
| `Agendas` | 1 | 1 | 0,50 | 0 / 4 | **0,00** | \|0 + 0,50 − 1\| = 0,50 |
| `ImplementacaoDoBanco` | 1 | 3 | 0,75 | 0 / 4 | **0,00** | \|0 + 0,75 − 1\| = 0,25 |
| `Aplicacao` | 0 | 6 | **1,00** | 0 / 3 | **0,00** | \|0 + 1 − 1\| = **0,00** |

Leitura:

- `DominioDaFolha` (estável, abstrato) e `Aplicacao` (instável, concreta) estão **sobre a sequência principal** (D′ ≈ 0): exatamente onde querem estar.
- Componentes **concretos e muito estáveis** (alto Ca, A≈0) cairiam na **zona de dor**; **abstratos e ninguém usa**, na **zona de inutilidade**. Aqui **ninguém** caiu em nenhuma.
- A **regra de dependência estável (SDP)** é satisfeita: toda seta aponta de **mais instável** para **mais estável** (de cima para baixo na figura). O **ADP** (sem ciclos) também: não há ciclos no grafo.

::: dica Automatize a medição
Não calcule isso à mão em projetos reais: ferramentas como **NDepend**, os **analisadores do Visual Studio** (métricas de código) ou *scripts* sobre o `dotnet` + Roslyn fornecem acoplamento e abstração por assembly. O importante é **olhar os números como alarme**, não como fim.
:::

::: exercicio Exercício A — onde fica o quê?
Em qual componente colocar cada classe/interface? Justifique pelo CCP/REP/DIP:
`TransacaoAlterarClassificacao`, `ClassificacaoComissionado`, `IBancoDeFuncionarios`, `BancoSql`, `ParserDeTransacoes`, `Funcionario`.

--- resposta
- `Funcionario` → **DominioDaFolha** (abstração central).
- `IBancoDeFuncionarios` → **BancoDaFolha** (interface da persistência, independente de implementação).
- `BancoSql` → **ImplementacaoDoBanco** (detalhe; depende da interface).
- `ClassificacaoComissionado` e `TransacaoAlterarClassificacao` → **Classificacoes** (mudam juntas: mexer em classificação mexe na transação que as troca — CCP).
- `ParserDeTransacoes` → componente próprio (`ParserDeTexto`): detalhe de entrada que depende de abstrações; trocá-lo por JSON/HTTP não afeta ninguém.
:::

## Parte 2 — Persistência: do dicionário ao SQL

No Laboratório 5 deixamos **de propósito** o armazenamento atrás de `IBancoDeFuncionarios`, com um `BancoEmMemoria`. Pagamos o adiamento do banco agora — e vamos descobrir o quanto o **DIP** nos economiza.

```cs
public interface IBancoDeFuncionarios
{
    Funcionario Obter(int id);
    void Adicionar(Funcionario f);
    void Remover(int id);
    IEnumerable<Funcionario> Todos();
}
```

> **Se o seu banco fosse estático?** Muitos projetos começam com uma classe de banco **estática**. Aí o primeiro passo é uma refatoração segura: criar uma instância, extrair cada método estático para um método de instância, trocar as chamadas uma a uma rodando os testes a cada passo, e depois **extrair a interface**. As transações recebem o banco **pelo construtor** (injeção de dependência). Como aqui já partimos de uma interface, pulamos esse passo.

### O esquema

```sql
CREATE TABLE Funcionario (
    Id              INT           NOT NULL PRIMARY KEY,
    Nome            NVARCHAR(100) NOT NULL,
    Endereco        NVARCHAR(200) NOT NULL,
    TipoAgenda      VARCHAR(20)   NOT NULL,
    TipoMetodo      VARCHAR(20)   NOT NULL,
    TipoClassific   VARCHAR(20)   NOT NULL
);

CREATE TABLE ContaDeposito (
    IdFuncionario INT NOT NULL REFERENCES Funcionario(Id),
    Banco         NVARCHAR(60) NOT NULL,
    Conta         NVARCHAR(30) NOT NULL
);

CREATE TABLE EnderecoCheque (
    IdFuncionario INT NOT NULL REFERENCES Funcionario(Id),
    Endereco      NVARCHAR(200) NOT NULL
);

-- Uma tabela por classificação (dados próprios de cada uma):
CREATE TABLE ClassificacaoHorista      (IdFuncionario INT NOT NULL REFERENCES Funcionario(Id), ValorHora DECIMAL(10,2) NOT NULL);
CREATE TABLE ClassificacaoAssalariado  (IdFuncionario INT NOT NULL REFERENCES Funcionario(Id), Salario   DECIMAL(10,2) NOT NULL);
CREATE TABLE ClassificacaoComissionado (IdFuncionario INT NOT NULL REFERENCES Funcionario(Id), Salario   DECIMAL(10,2) NOT NULL, Taxa DECIMAL(5,4) NOT NULL);
```

`Funcionario` guarda os dados imediatos e **códigos curtos** (`'mensal'`, `'deposito'`…) para agenda, método e classificação; cada variante tem seus dados em sua própria tabela, ligada por `IdFuncionario` (chave estrangeira).

### Passo 1 — Um primeiro teste de integração

Estes testes falam com um **banco real** (um SQL Server local ou contêiner de teste). São **testes de integração**: poucos, focados em **mapeamento**.

```cs
[TestFixture]
public class BancoSqlTests
{
    private BancoSql banco;
    private SqlConnection conexao;
    private Funcionario f;

    [SetUp]
    public void SetUp()
    {
        conexao = new SqlConnection(ConfiguracaoDeTeste.StringDeConexao);   // vem de variável de ambiente
        conexao.Open();
        banco = new BancoSql(conexao);
        LimparTabelas();
        f = new Funcionario(123, "Jorge", "Rua das Flores, 1")
        {
            Agenda = new AgendaMensal(),
            Metodo = new MetodoDeposito("Banco 1", "123890"),
            Classificacao = new ClassificacaoAssalariado(1000m)
        };
    }

    [TearDown] public void TearDown() { conexao.Close(); }

    [Test]
    public void AdicionaFuncionario()
    {
        banco.Adicionar(f);
        var linha = CarregarTabela("Funcionario").Rows[0];
        Assert.AreEqual(123, linha["Id"]);
        Assert.AreEqual("Jorge", linha["Nome"]);
        Assert.AreEqual("Rua das Flores, 1", linha["Endereco"]);
    }
}
```

::: cuidado Nunca coloque credenciais no código
Em produção ou em repositório, **nunca** escreva usuário/senha na *string* de conexão dentro do código — use **variáveis de ambiente**, *User Secrets* ou um cofre (Key Vault). Os exemplos do livro original embutiam credenciais; aqui o valor vem da configuração.
:::

O primeiro código "força bruta" para passar (sempre **parametrizado** — nunca concatene valores em SQL):

```cs
public class BancoSql : IBancoDeFuncionarios
{
    private readonly SqlConnection conexao;
    public BancoSql(SqlConnection conexao) { this.conexao = conexao; }

    public void Adicionar(Funcionario f)
    {
        var cmd = new SqlCommand(
            "INSERT INTO Funcionario (Id, Nome, Endereco, TipoAgenda, TipoMetodo, TipoClassific) " +
            "VALUES (@Id, @Nome, @Endereco, @Agenda, @Metodo, @Classific)", conexao);
        cmd.Parameters.AddWithValue("@Id", f.Id);
        cmd.Parameters.AddWithValue("@Nome", f.Nome);
        cmd.Parameters.AddWithValue("@Endereco", f.Endereco);
        cmd.Parameters.AddWithValue("@Agenda", f.Agenda.GetType().Name);
        cmd.Parameters.AddWithValue("@Metodo", f.Metodo.GetType().Name);
        cmd.Parameters.AddWithValue("@Classific", f.Classificacao.GetType().Name);
        cmd.ExecuteNonQuery();
    }
    // Obter/Remover/Todos ainda lançam NotImplementedException
}
```

Roda uma vez, falha na segunda: *chave duplicada*. Falta **limpar as tabelas** antes de cada teste (`LimparTabelas()` no `SetUp`) — e, por causa das chaves estrangeiras, **na ordem certa**: filhas primeiro, `Funcionario` por último.

```cs
private void LimparTabelas()
{
    foreach (var t in new[] { "ContaDeposito", "EnderecoCheque", "ClassificacaoHorista",
                              "ClassificacaoAssalariado", "ClassificacaoComissionado", "Funcionario" })
        new SqlCommand("DELETE FROM " + t, conexao).ExecuteNonQuery();
}
```

> Tudo que é *nome de tabela constante e controlado por você* pode ser concatenado; **valores vindos de fora**, nunca.

### Passo 2 — Códigos curtos em vez de nomes de classe

`"AgendaMensal"` na coluna é longo e vaza detalhes de implementação. Troque por códigos (`"mensal"`). O teste:

```cs
[Test]
public void AgendaEhSalvaComCodigo()
{
    VerificarCodigoDaAgenda(new AgendaMensal(),     "mensal");   LimparTabelas();
    VerificarCodigoDaAgenda(new AgendaSemanal(),    "semanal");  LimparTabelas();
    VerificarCodigoDaAgenda(new AgendaQuinzenal(),  "quinzenal");
}
```

O primeiro código que o faz passar:

```cs
private static string CodigoDaAgenda(AgendaDePagamento a)
{
    if (a is AgendaMensal)    return "mensal";
    if (a is AgendaSemanal)   return "semanal";
    if (a is AgendaQuinzenal) return "quinzenal";
    return "desconhecida";
}
```

::: pense Uma violação do OCP... tolerada
Aí está o começo de um problema: cada **nova agenda** exige **mais um `if`**. A alternativa seria uma propriedade polimórfica `CodigoNoBanco` na própria hierarquia `AgendaDePagamento`. Mas isso **violaria o SRP** e **acoplaria o domínio ao banco** (e convidaria outros módulos a usar o código!). Comparando: a violação do OCP fica **encapsulada dentro do `BancoSql`** e provavelmente não vaza; a do SRP vazaria por todo o domínio. **Conviva com a primeira por enquanto.** Quando o número de `if`s crescer, uma tabela `Dictionary<Type, string>` ou *delegates* resolve (veja o Passo 5).
:::

### Passo 3 — Dados próprios de cada método de pagamento

`MetodoDeposito` e `MetodoCorreio` têm **dados próprios**, que vão para suas **tabelas**. O teste:

```cs
[Test]
public void DepositoEhSalvoNaTabelaDeConta()
{
    f.Metodo = new MetodoDeposito("Banco -1", "0987654321");
    banco.Adicionar(f);

    var tabela = CarregarTabela("ContaDeposito");
    Assert.AreEqual(1, tabela.Rows.Count);
    Assert.AreEqual("Banco -1", tabela.Rows[0]["Banco"]);
    Assert.AreEqual("0987654321", tabela.Rows[0]["Conta"]);
    Assert.AreEqual(123, tabela.Rows[0]["IdFuncionario"]);
}
```

Ao implementar aparece uma armadilha: a linha de `ContaDeposito` **referencia** `Funcionario` — então só pode ser inserida **depois** do funcionário. Isso cria um dilema:

> E se o `INSERT` do funcionário der certo e o do método de pagamento **falhar**? O banco ficaria com um funcionário **sem método de pagamento** — um estado **impossível** no domínio: **dados corrompidos**.

### Passo 4 — Transações

A solução clássica: **transações**. Se qualquer parte falha, **tudo é desfeito**. Como testar? Forçando uma falha **depois** do primeiro comando:

```cs
[Test]
public void AdicionarEhTransacional()
{
    // Valores nulos violam NOT NULL em ContaDeposito → o 2º INSERT falha
    f.Metodo = new MetodoDeposito(null, null);

    Assert.Throws<SqlException>(() => banco.Adicionar(f));

    Assert.AreEqual(0, CarregarTabela("Funcionario").Rows.Count);   // nada ficou para trás
}
```

O teste falha (o funcionário foi salvo). Agora, a correção:

```cs
public void Adicionar(Funcionario f)
{
    using (var tx = conexao.BeginTransaction())
    {
        try
        {
            ExecutarInsertDoFuncionario(f, tx);
            ExecutarInsertDoMetodo(f, tx);
            ExecutarInsertDaClassificacao(f, tx);
            tx.Commit();
        }
        catch
        {
            tx.Rollback();
            throw;                  // 'throw;' preserva a pilha (nunca 'throw e;')
        }
    }
}
```

::: dica Alternativa moderna
`using var scope = new TransactionScope(); ...; scope.Complete();` (System.Transactions) também funciona e se compõe com várias operações; **EF Core** faz tudo isso por você em `SaveChanges()` (uma transação por chamada).
:::

### Passo 5 — Uma operação por classe

Conforme o `Adicionar` cresceu, a classe `BancoSql` se encheu de métodos privados auxiliares — e foi aí que surgiu um **defeito sutil**: guardávamos o comando do método de pagamento num **campo de instância**. Se um funcionário com **correio** fosse salvo e, depois, outro com **retenção** (que não tem comando próprio), o comando antigo **ficava lá** e era executado de novo. O teste que expõe isso:

```cs
[Test]
public void SalvaCorreioDepoisReter()
{
    f.Metodo = new MetodoCorreio("Rua A, 1");
    banco.Adicionar(f);

    var f2 = new Funcionario(321, "Edu", "Rua B, 2")
    {
        Agenda = new AgendaMensal(), Classificacao = new ClassificacaoAssalariado(500m),
        Metodo = new MetodoReter()
    };
    banco.Adicionar(f2);

    Assert.AreEqual(1, CarregarTabela("EnderecoCheque").Rows.Count);   // falha: 2
}
```

**Estado compartilhado entre chamadas** num objeto de vida longa — é o tipo de bug que mora em campos mutáveis. A cura é a refatoração que o **SRP** pede: **uma classe por operação**, nova a cada chamada.

```cs
public void Adicionar(Funcionario f)
{
    new SalvarFuncionarioOperacao(f, conexao).Executar();     // instância nova: sem estado herdado
}

internal class SalvarFuncionarioOperacao
{
    private readonly Funcionario f;
    private readonly SqlConnection conexao;
    private SqlCommand insereFuncionario, insereMetodo, insereClassificacao;   // campos seguros: vivem 1 operação

    public SalvarFuncionarioOperacao(Funcionario f, SqlConnection conexao) { this.f = f; this.conexao = conexao; }

    public void Executar()
    {
        Preparar();
        using (var tx = conexao.BeginTransaction())
        {
            try
            {
                Rodar(insereFuncionario, tx);
                Rodar(insereMetodo, tx);          // pode ser null (reter): Rodar ignora
                Rodar(insereClassificacao, tx);
                tx.Commit();
            }
            catch { tx.Rollback(); throw; }
        }
    }

    private void Rodar(SqlCommand cmd, SqlTransaction tx)
    {
        if (cmd == null) return;
        cmd.Connection = conexao; cmd.Transaction = tx;
        cmd.ExecuteNonQuery();
    }

    private void Preparar() { /* monta os três comandos conforme os tipos */ }
}
```

E se os `if/else` de tipo voltarem a se duplicar (um para o código, outro para a tabela)? Use **uma única decisão** que escolha *ambos* de uma vez — por exemplo, um dicionário de **estratégias** por tipo:

```cs
private static readonly Dictionary<Type, Func<Funcionario, SqlCommand>> ComandosDoMetodo =
    new Dictionary<Type, Func<Funcionario, SqlCommand>>
    {
        { typeof(MetodoReter),    f => null },
        { typeof(MetodoDeposito), f => ComandoDeposito(f, (MetodoDeposito)f.Metodo) },
        { typeof(MetodoCorreio),  f => ComandoCorreio(f, (MetodoCorreio)f.Metodo) },
    };
```

Agora **um novo método de pagamento** só adiciona **uma linha** ao dicionário (e uma classe de domínio): o OCP voltou a ficar quase intacto.

### Passo 6 — Carregar funcionário (e testar **sem** banco)

O teste de integração principal grava e relê tudo:

```cs
[Test]
public void CarregaFuncionario()
{
    f.Agenda = new AgendaQuinzenal();
    f.Metodo = new MetodoDeposito("1º Banco", "0123456");
    f.Classificacao = new ClassificacaoAssalariado(5432.10m);
    banco.Adicionar(f);

    var lido = banco.Obter(123);

    Assert.AreEqual("Jorge", lido.Nome);
    Assert.IsInstanceOf<AgendaQuinzenal>(lido.Agenda);
    var dep = (MetodoDeposito)lido.Metodo;
    Assert.AreEqual("1º Banco", dep.Banco);
    Assert.AreEqual("0123456", dep.Conta);
    Assert.AreEqual(5432.10m, ((ClassificacaoAssalariado)lido.Classificacao).Salario);
}
```

Mas o **resto** do código de carregamento pode ser testado **sem tocar no banco**: separamos "montar o comando" e "transformar a linha em objeto". Em vez de acessar o `SqlDataReader`, a operação recebe **linhas já lidas** (um `DataRow`, um `IDataRecord` ou um simples dicionário/`record`):

```cs
// Teste unitário PURO (rápido, sem SQL Server)
[Test]
public void CriaMetodoDeDepositoAPartirDaLinha()
{
    var linha = new Dictionary<string, object> { ["Banco"] = "1º Banco", ["Conta"] = "0123456" };

    var metodo = new CarregarMetodoOperacao(idFuncionario: 567, codigo: "deposito").CriarMetodo(linha);

    var dep = (MetodoDeposito)metodo;
    Assert.AreEqual("1º Banco", dep.Banco);
    Assert.AreEqual("0123456", dep.Conta);
}

[Test]
public void ComandoDoDepositoSelecionaATabelaCerta()
{
    var op = new CarregarMetodoOperacao(567, "deposito");
    var cmd = op.ComandoDeConsulta;
    Assert.AreEqual("SELECT * FROM ContaDeposito WHERE IdFuncionario = @Id", cmd.CommandText);
    Assert.AreEqual(567, cmd.Parameters["@Id"].Value);
}
```

A classe `CarregarMetodoOperacao` decide **uma vez** (num único `if`/dicionário) tabela *e* criador de objeto:

```cs
internal class CarregarMetodoOperacao
{
    private readonly int id;
    private readonly string codigo;
    private string tabela;
    private Func<IReadOnlyDictionary<string, object>, MetodoDePagamento> criador;

    public CarregarMetodoOperacao(int idFuncionario, string codigo) { id = idFuncionario; this.codigo = codigo; Preparar(); }

    private void Preparar()
    {
        switch (codigo)
        {
            case "reter":    criador = _ => new MetodoReter(); break;
            case "deposito": tabela = "ContaDeposito";  criador = l => new MetodoDeposito((string)l["Banco"], (string)l["Conta"]); break;
            case "correio":  tabela = "EnderecoCheque"; criador = l => new MetodoCorreio((string)l["Endereco"]); break;
            default: throw new ArgumentException("Método desconhecido: " + codigo);
        }
    }

    public SqlCommand ComandoDeConsulta
    {
        get
        {
            var cmd = new SqlCommand("SELECT * FROM " + tabela + " WHERE IdFuncionario = @Id");
            cmd.Parameters.AddWithValue("@Id", id);
            return cmd;
        }
    }

    public MetodoDePagamento CriarMetodo(IReadOnlyDictionary<string, object> linha) { return criador(linha); }
}
```

O mesmo padrão vale para **classificações**: `CarregarClassificacaoOperacao`. E uma `CarregarFuncionarioOperacao` orquestra: lê a linha de `Funcionario`, cria o objeto, e delega **agenda**, **método** e **classificação** às operações especializadas.

::: dica Muitas operações parecidas? Extraia a base
`SalvarFuncionarioOperacao`, `CarregarFuncionarioOperacao`, `CarregarMetodoOperacao`… repetem trechos (abrir comando, carregar tabela, executar). Uma **classe base** `OperacaoDeBanco` (ou métodos utilitários) é o lugar natural para o código compartilhado — **Template Method** de novo!
:::

### O que falta?

O `BancoSql` já **salva** e **carrega** funcionários, mas não está completo. Fica como exercício:

1. **Salvar um funcionário que já existe** (atualizar em vez de falhar). Como detectar? `MERGE`/`UPDATE ... WHERE`, ou `Obter` antes.
2. **Cartões de ponto, recibos de venda e afiliações ao sindicato** (e suas tabelas), com os mesmos passos: teste de integração, transação, operação dedicada.
3. **`Remover`**: apagar nas tabelas filhas **antes** do `Funcionario` (ou `ON DELETE CASCADE`).
4. Trocar `AddWithValue` por **tipos explícitos** (`SqlDbType`) para evitar conversões implícitas surpreendentes.

::: exercicio Exercício B — Proxy ou Repository?
Neste laboratório usamos o **Repository (gateway)** `IBancoDeFuncionarios`. Aplique o raciocínio do capítulo sobre Proxy:

1. Por que o `Proxy` **não** compensou aqui?
2. Em que cenário você migraria para Proxy/ORM com *lazy loading*?
3. Que ganho o `BancoEmMemoria` continua dando?

--- resposta
1. O esquema é **estável** e só há **um** banco; o acoplamento de `BancoSql` ao esquema fica **contido numa classe** — separação suficiente sem o custo de delegação adiada, 1:N e cache do Proxy.
2. Quando o esquema/serviços forem **instáveis**, houver **vários bancos/middlewares** ou o domínio precisar de **separação absoluta** (ou quando um ORM como EF Core, com proxies de lazy loading, já resolver o mapeamento).
3. **Testes rápidos** de toda a lógica (transações, folha, agendas) **sem banco**, mantendo poucos testes de integração para o mapeamento SQL.
:::

::: resumo
- **Pacotes:** agrupe por **fechamento comum** (CCP); inverta dependências para que **detalhes** (irresponsáveis) dependam de **abstrações** (independentes e pequenas); separe o que **não é reutilizado junto** (CRP).
- Meça **I, A, D′**; verifique **SDP** (setas rumo ao estável) e **ADP** (sem ciclos). Use `internal` para esconder detalhes de um componente.
- Comece simples e refine a estrutura **por dados**, não por especulação.
- **Persistência:** o banco é detalhe atrás de `IBancoDeFuncionarios`; troque a implementação sem tocar no domínio (**DIP**).
- **Testes de integração** para o mapeamento (poucos); **testes unitários** para montar comandos e criar objetos a partir de linhas (muitos, sem banco).
- **Transações** evitam estados corrompidos; `throw;` preserva a pilha; SQL **sempre parametrizado**; credenciais **fora** do código.
- Uma **operação por classe** elimina estado compartilhado; uma **única decisão** de tipo (dicionário/switch) evita `if`s duplicados.
:::

::: quiz
? O que o CCP recomenda ao dividir o sistema em componentes?
+ Agrupar classes que mudam juntas, pelos mesmos motivos
- Agrupar por ordem alfabética
- Colocar todas as classes concretas juntas
- Evitar interfaces
> Assim uma mudança afeta o mínimo de componentes.

? Por que "detalhes embaixo, abstrações em cima" é o lugar errado?
+ Detalhes devem depender das abstrações (DIP); nada deve depender deles
- Abstrações são mais lentas
- Detalhes são mais estáveis
- É indiferente
> Detalhes flexíveis e irresponsáveis em cima; abstrações estáveis embaixo.

? Qual o grão de reutilização?
+ O componente (reutilizar = entregar junto)
- A classe isolada
- O método
- A linha de código
> Pegar só um pedaço quebra a estrutura de versões (REP/CRP).

? Um componente com A=0 e I=0 (muito estável e concreto) está…
+ Na zona de dor
- Sobre a sequência principal
- Na zona de inutilidade
- Fora de qualquer zona
> D′ = |A + I − 1| = 1: máxima distância da sequência principal.

? Por que a persistência de funcionário + método de pagamento deve ser transacional?
+ Para não deixar funcionário sem método de pagamento se o segundo insert falhar
- Para o SQL ficar mais rápido
- Para evitar interfaces
- Porque o SQL Server exige
> Rollback desfaz tudo; nada é salvo em caso de falha.

? Qual o bug causado pelo campo de instância com o comando do método de pagamento?
+ O comando de um salvamento anterior era executado novamente no seguinte
- Memória insuficiente
- Falha de compilação
- Deadlock
> A solução foi criar uma operação nova (sem estado herdado) a cada chamada.

? Como testar o carregamento sem tocar no banco?
+ Passar linhas já criadas (dicionário/DataRow) às operações e testar comando e criação de objetos separadamente
- Usar sempre o SQL Server de produção
- Não testar
- Mockar o compilador
> Deixe apenas um teste de integração para validar a conexão e o mapeamento completo.
:::

::: cartas
Componente :: Unidade entregável (assembly/DLL/pacote) com classes coesas.
CCP :: Classes que mudam juntas ficam juntas.
CRP :: Classes reutilizadas juntas ficam juntas.
SDP :: Dependa na direção da estabilidade.
Sequência principal :: Linha A + I = 1 onde estabilidade e abstração se equilibram.
Teste de integração :: Teste que usa recursos reais (banco) para validar o mapeamento.
Transação :: Conjunto de operações que ou todas têm sucesso ou nenhuma é aplicada.
Operação por classe :: Uma classe por tarefa de banco, instanciada a cada chamada (sem estado compartilhado).
internal :: Visibilidade restrita ao assembly: esconde detalhes do componente.
:::
