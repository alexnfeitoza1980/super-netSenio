---
id: leis-lei16
order: 77
kind: capitulo
num: 57
group: C# & Mentalidade · Parte 12 · As 16 leis do sucesso em C#
title: Lei 16 — A Regra de Ouro
subtitle: "Faça aos outros o que gostaria que fizessem a você" em código: contratos honrados (LSP e testes de contrato), APIs honestas, privacidade nos logs, SQL parametrizado e o código que você gostaria de herdar.
minutes: 60
hue: 48
objectives:
- Traduzir a **Regra de Ouro** em princípios de engenharia: **honestidade, respeito, segurança e cuidado com quem vem depois**
- Garantir que implementações **honrem o contrato** (LSP) usando **testes de contrato**
- Projetar **APIs honestas**: sem efeitos colaterais escondidos, com nomes que dizem a verdade
- Proteger **privacidade**: não vazar segredos em logs e `ToString`
- Evitar **injeção de SQL** com consultas parametrizadas
origem: livros/03-leis-do-sucesso-csharp/content/76-lei16.md
---

::: vila O código que Lia herdou
Num dia de manhã, Lia abre um módulo escrito por alguém que já saiu da empresa. Há um método chamado `Calcular` que, escondido, **grava no banco**. Um `ToString` que imprime a senha do usuário no log. Uma consulta montada com concatenação de texto. Nada é "ilegal" — só é **desconsiderado com quem vem depois**. Naquele momento, Lia faz uma promessa a si mesma: *"vou escrever o código que eu gostaria de herdar."*
:::

::: ilustracao img/leis/lei16.svg | Regra de Ouro: trate quem usa e quem mantém seu código como você gostaria de ser tratado.
:::

## A lei, em uma frase

> **"Tudo o que quereis que os outros vos façam, fazei-o também a eles."** A Regra de Ouro não é só um bom discurso: nossos pensamentos e atos moldam o nosso caráter e voltam para nós. Por isso, **pense dos outros só o que gostaria que pensassem de você**, **seja honesto até nos mínimos detalhes**, **perdoe** as injustiças e **ajude** onde e quando puder.

A lição insiste: *conhecer e acreditar na regra não basta — é preciso **praticá-la***; e lembra que **"enquanto a honestidade for objeto de louvor, a desonestidade seguirá sendo o normal"** — isto é, honestidade deve ser **a norma**, não a exceção elogiada.

## Da vida para o código

| O que a lei diz | O que isso significa para quem programa |
|---|---|
| Não faça aos outros o que não quer que façam a você | **Escreva código para a próxima pessoa** (talvez você mesmo) |
| Honestidade nos mínimos detalhes | **APIs honestas**: nomes e contratos que dizem a verdade; sem efeitos escondidos |
| Cumprir o que se promete | **Honrar contratos** (LSP, interfaces, documentação) |
| Respeito pelo outro | **Privacidade**: não vazar dados sensíveis; **segurança** por padrão |
| Perdoar | Tratar bem os **erros do usuário** e de colegas (Lei 8, 10, 15) |
| Ajudar quando puder | Documentar, **deixar o código melhor** do que encontrou (Lei 4) |
| O que fazemos aos outros fazemos a nós | **Dívida técnica**: o próximo a pagar costuma ser você |

::: figura A regra de ouro, do código ao usuário
<svg viewBox="0 0 740 190" role="img" aria-label="Você escreve código para quem vai herdá-lo e para quem o usa; o que você faz volta para você">
  <rect class="f-prim" x="20" y="60" width="150" height="60" rx="12"/><text x="95" y="86" text-anchor="middle" font-weight="800">Você hoje</text><text x="95" y="106" text-anchor="middle" font-size="11" class="t-dim">escreve o código</text>
  <rect class="f-info" x="300" y="20" width="170" height="52" rx="12"/><text x="385" y="44" text-anchor="middle" font-weight="800">Quem usa</text><text x="385" y="62" text-anchor="middle" font-size="11" class="t-dim">pessoas e outros sistemas</text>
  <rect class="f-warn" x="300" y="110" width="170" height="52" rx="12"/><text x="385" y="134" text-anchor="middle" font-weight="800">Quem herda</text><text x="385" y="152" text-anchor="middle" font-size="11" class="t-dim">colegas… e você, no futuro</text>
  <rect class="f-good" x="570" y="60" width="150" height="60" rx="12"/><text x="645" y="86" text-anchor="middle" font-weight="800">Você amanhã</text><text x="645" y="106" text-anchor="middle" font-size="11" class="t-dim">colhe o que plantou</text>
  <path class="arrow" d="M172 80L298 50"/><path class="arrow" d="M172 100L298 132"/><path class="arrow" d="M472 50L568 80"/><path class="arrow" d="M472 132L568 102"/>
</svg>
:::

## 1) Honrar o contrato: o LSP e os testes de contrato

Quando você implementa uma interface, **promete** um comportamento. Se uma implementação devolve `null` para "não achei" e outra lança exceção, quem usa a interface é **traído** — o código "funciona" com uma e quebra com a outra. É a violação do **Princípio da Substituição de Liskov** (cap. 22).

A melhor forma de **cumprir o que se promete** é escrever **testes de contrato**: um único conjunto de testes, escrito contra a **interface**, que **todas** as implementações precisam passar.

```cs Jornada/Lei16/Armazenamento.cs
namespace Jornada.Lei16;

/// <summary>
/// Contrato: Salvar sobrescreve; Obter devolve null se não existir (nunca lança por isso);
/// Remover devolve true se existia e false se não existia. Ids válidos: letras, números, '-' e '_'.
/// </summary>
public interface IArmazenamentoDeNotas
{
    void Salvar(string id, string texto);
    string? Obter(string id);
    bool Remover(string id);
}

internal static class Ids
{
    public static void Validar(string id)
    {
        if (string.IsNullOrWhiteSpace(id) || !id.All(c => char.IsLetterOrDigit(c) || c is '-' or '_'))
            throw new ArgumentException($"Id inválido: '{id}'. Use letras, números, '-' e '_'.", nameof(id));
    }
}

public sealed class ArmazenamentoEmMemoria : IArmazenamentoDeNotas
{
    private readonly Dictionary<string, string> notas = new();

    public void Salvar(string id, string texto) { Ids.Validar(id); notas[id] = texto; }
    public string? Obter(string id) { Ids.Validar(id); return notas.GetValueOrDefault(id); }
    public bool Remover(string id) { Ids.Validar(id); return notas.Remove(id); }
}

public sealed class ArmazenamentoEmArquivo : IArmazenamentoDeNotas
{
    private readonly string pasta;

    public ArmazenamentoEmArquivo(string pasta)
    {
        this.pasta = pasta;
        Directory.CreateDirectory(pasta);
    }

    private string Caminho(string id) { Ids.Validar(id); return Path.Combine(pasta, id + ".txt"); }

    public void Salvar(string id, string texto) => File.WriteAllText(Caminho(id), texto);

    public string? Obter(string id)
    {
        var caminho = Caminho(id);
        return File.Exists(caminho) ? File.ReadAllText(caminho) : null;
    }

    public bool Remover(string id)
    {
        var caminho = Caminho(id);
        if (!File.Exists(caminho)) return false;
        File.Delete(caminho);
        return true;
    }
}
```

Agora o **contrato como teste** — escrito uma vez, herdado por cada implementação:

```cs JornadaTests/Lei16/ContratoDoArmazenamento.cs
using Jornada.Lei16;
using Xunit;

public abstract class ContratoDoArmazenamento
{
    protected abstract IArmazenamentoDeNotas Criar();

    [Fact]
    public void Obter_de_id_inexistente_devolve_null() =>
        Assert.Null(Criar().Obter("nao-existe"));

    [Fact]
    public void Salvar_e_Obter_devolvem_o_mesmo_texto()
    {
        var a = Criar();
        a.Salvar("nota-1", "olá");
        Assert.Equal("olá", a.Obter("nota-1"));
    }

    [Fact]
    public void Salvar_sobrescreve()
    {
        var a = Criar();
        a.Salvar("n", "v1");
        a.Salvar("n", "v2");
        Assert.Equal("v2", a.Obter("n"));
    }

    [Fact]
    public void Remover_informa_se_existia()
    {
        var a = Criar();
        a.Salvar("n", "x");
        Assert.True(a.Remover("n"));
        Assert.False(a.Remover("n"));
    }

    [Theory]
    [InlineData("../fora")]
    [InlineData("a/b")]
    [InlineData("")]
    public void Ids_invalidos_sao_recusados(string id) =>
        Assert.Throws<ArgumentException>(() => Criar().Salvar(id, "x"));
}

public class ArmazenamentoEmMemoriaTests : ContratoDoArmazenamento
{
    protected override IArmazenamentoDeNotas Criar() => new ArmazenamentoEmMemoria();
}

public class ArmazenamentoEmArquivoTests : ContratoDoArmazenamento
{
    protected override IArmazenamentoDeNotas Criar() =>
        new ArmazenamentoEmArquivo(Path.Combine(Path.GetTempPath(), "jornada-" + Guid.NewGuid().ToString("N")));
}
```

Quando alguém criar uma **terceira** implementação (digamos, em banco de dados), basta herdar `ContratoDoArmazenamento` e fornecer `Criar()`: **todas as regras** serão verificadas. Se a nova implementação, por exemplo, lançar `KeyNotFoundException` em `Obter`, o teste `Obter_de_id_inexistente_devolve_null` **falha** — o contrato foi violado **antes** de o código chegar a quem o usaria. É a regra de ouro automatizada: *quem implementa cumpre o que a interface prometeu.*

::: dica Note a validação do id
Repare em `Ids.Validar`: a implementação em arquivo **não confia** no id recebido (poderia ser `../../etc/passwd`). Honestidade com o usuário inclui **proteger** o que ele confiou a você (Lei 15, seção 4).
:::

## 2) APIs honestas: o nome diz a verdade

Uma API desonesta **surpreende** — e a surpresa, em software, vira bug, perda de dados ou vulnerabilidade. Algumas convenções que mantêm a honestidade:

| Se o nome diz… | Então o código deve… | Violação clássica |
|---|---|---|
| `Calcular…`, `Obter…`, `Is…` | **Não alterar** o estado do sistema | `Calcular()` que grava no banco |
| `TryParse`, `TryObter` | **Nunca lançar** por entrada inválida | `TryParse` que lança `FormatException` |
| `…Async` | Retornar `Task` e **não bloquear** | `ObterAsync()` que chama `.Result` por dentro |
| `Dispose` | **Liberar** os recursos e ser seguro chamar duas vezes | `Dispose` que lança se for chamado de novo |
| `Salvar`, `Enviar`, `Excluir` | Dizer **claramente** que tem efeito | `Validar()` que também envia e-mail |
| Documentação/XML docs | Descrever **exceções e efeitos** reais | Documentação que mente (ou desatualizada) |

E há a **honestidade no tratamento de erros** (Lei 8): não devolva `0` ou `""` como se fosse um resultado válido quando houve falha; não diga "sucesso" quando falhou; não esconda informação relevante do chamador.

## 3) Privacidade: não faça aos dados dos outros o que não quer que façam aos seus

Logs e `ToString` são lugares onde dados sensíveis **vazam** com facilidade. Duas defesas simples:

```cs Jornada/Lei16/Privacidade.cs
namespace Jornada.Lei16;

/// <summary>Embrulha um valor sensível: impresso, interpolado ou logado por engano, nunca aparece.</summary>
public readonly record struct Segredo(string Valor)
{
    public override string ToString() => "***";
    public string Revelar() => Valor;                 // uso explícito, consciente
}

public static class Mascara
{
    public static string Cartao(string numero)
    {
        var digitos = new string(numero.Where(char.IsDigit).ToArray());
        return digitos.Length < 4 ? "****" : $"**** **** **** {digitos[^4..]}";
    }

    public static string Email(string email)
    {
        int arroba = email.IndexOf('@');
        return arroba <= 1 ? "*" + email[Math.Max(arroba, 0)..] : email[0] + new string('*', arroba - 1) + email[arroba..];
    }
}
```

```cs Uso
var senha = new Segredo("S3nh@-Mu1to-F0rte");
Console.WriteLine($"Autenticando com a senha {senha}");            // ← o log não vaza

Console.WriteLine($"Cartão: {Mascara.Cartao("4111 1111 1111 1234")}");
Console.WriteLine($"E-mail: {Mascara.Email("ana@exemplo.com")}");
```

```saida
Autenticando com a senha ***
Cartão: **** **** **** 1234
E-mail: a**@exemplo.com
```

::: cuidado `ToString` não é a única porta
`Segredo` protege `ToString` e a interpolação, mas **não** a serialização: `JsonSerializer.Serialize(segredo)` ainda emitiria o valor. Combine com `[JsonIgnore]`/conversores, com **não registrar objetos inteiros** em logs e com ferramentas de **redação** (*redaction*) de logs. Também respeite leis como a **LGPD**: colete **só o necessário**, por **tempo limitado**, com **finalidade clara**.
:::

## 4) Segurança como cortesia: nunca monte SQL com texto do usuário

Concatenar texto do usuário numa consulta é como entregar a chave do cofre a um desconhecido. O **ataque de injeção de SQL** é um dos mais antigos e ainda dos mais comuns. Veja, com um banco SQLite em memória, a diferença entre **desrespeitar** e **respeitar** quem usa o sistema:

```cs Jornada/Lei16/Clientes.cs
using Microsoft.Data.Sqlite;

namespace Jornada.Lei16;

public static class Clientes
{
    public static SqliteConnection CriarBancoDeExemplo()
    {
        var conexao = new SqliteConnection("Data Source=:memory:");
        conexao.Open();

        using var cmd = conexao.CreateCommand();
        cmd.CommandText = "CREATE TABLE clientes(nome TEXT); INSERT INTO clientes VALUES ('Ana'), ('Bia'), ('Caio');";
        cmd.ExecuteNonQuery();
        return conexao;
    }

    // ✘ NUNCA faça isto: o texto do usuário vira parte do comando SQL.
    public static int ContarVulneravel(SqliteConnection conexao, string nome)
    {
        using var cmd = conexao.CreateCommand();
        cmd.CommandText = "SELECT COUNT(*) FROM clientes WHERE nome = '" + nome + "'";
        return Convert.ToInt32(cmd.ExecuteScalar());
    }

    // ✔ O valor viaja como PARÂMETRO: nunca é interpretado como SQL.
    public static int ContarSeguro(SqliteConnection conexao, string nome)
    {
        using var cmd = conexao.CreateCommand();
        cmd.CommandText = "SELECT COUNT(*) FROM clientes WHERE nome = $nome";
        cmd.Parameters.AddWithValue("$nome", nome);
        return Convert.ToInt32(cmd.ExecuteScalar());
    }
}
```

```cs Uso
using var banco = Clientes.CriarBancoDeExemplo();

foreach (var entrada in new[] { "Ana", "x' OR '1'='1" })
{
    Console.WriteLine($"entrada: {entrada}");
    Console.WriteLine($"  vulnerável → {Clientes.ContarVulneravel(banco, entrada)} registro(s)");
    Console.WriteLine($"  seguro     → {Clientes.ContarSeguro(banco, entrada)} registro(s)");
}
```

```saida
entrada: Ana
  vulnerável → 1 registro(s)
  seguro     → 1 registro(s)
entrada: x' OR '1'='1
  vulnerável → 3 registro(s)
  seguro     → 0 registro(s)
```

Com a entrada maliciosa, a versão vulnerável **devolveu todos os clientes** — num cenário real, poderia ser todas as senhas. A versão com parâmetro tratou o texto como **dado**, não como **comando**. O mesmo vale para **Entity Framework Core** (que parametriza por padrão — cuidado com `FromSqlRaw` e interpolação) e **Dapper** (use `@parametros`).

## 5) O código que você gostaria de herdar

Fechando a lição: **"o que quer que façamos aos outros, fazemos também a nós mesmos"**. Em software, o "outro" é, muito concretamente, **você daqui a seis meses**. Uma lista de verificação pessoal para cada *pull request*:

- [ ] Eu entenderia este código se o encontrasse sem contexto?
- [ ] Os **nomes** dizem a verdade? Há **efeitos escondidos**?
- [ ] Há **testes** que protegem o comportamento (e o contrato)?
- [ ] Os **erros** são honestos e claros? Os **dados sensíveis** estão protegidos?
- [ ] A **documentação** (README, XML docs) existe e está correta?
- [ ] Deixei o código **um pouco melhor** do que encontrei?
- [ ] Tratei com **respeito** quem vai revisar, usar e manter?

::: pense Honestidade como padrão, não como mérito
A lição termina com uma frase forte: *enquanto a honestidade for objeto de louvor, a desonestidade seguirá sendo o normal*. A meta é fazer da **integridade técnica** — contratos honrados, erros claros, privacidade e segurança por padrão — **o jeito normal** de trabalhar, não um diferencial raro de quem "é caprichoso". Quando isso é a norma da equipe (definição de pronto, revisão, analisadores), **ninguém precisa ser herói para ser íntegro**.
:::

::: exercicio Mais uma regra de contrato
Acrescente ao contrato (`ContratoDoArmazenamento`) uma regra nova: **`Obter` de um id válido, mas nunca salvo, devolve `null` — mesmo depois de outro id ter sido salvo**. Em seguida, escreva uma terceira implementação **`ArmazenamentoComCache`** que embrulha outra e guarda o último valor lido; verifique que ela **passa no mesmo contrato** (basta herdar o teste).

--- resposta
```cs ContratoDoArmazenamento.cs (trecho)
// 1) Nova regra no contrato (adicione dentro de ContratoDoArmazenamento):
[Fact]
public void Salvar_um_id_nao_afeta_os_outros()
{
    var a = Criar();
    a.Salvar("a", "1");
    Assert.Null(a.Obter("b"));
}
```
```cs
// 2) Terceira implementação: decorator com cache.
public sealed class ArmazenamentoComCache : IArmazenamentoDeNotas
{
    private readonly IArmazenamentoDeNotas interno;
    private readonly Dictionary<string, string?> cache = new();

    public ArmazenamentoComCache(IArmazenamentoDeNotas interno) => this.interno = interno;

    public void Salvar(string id, string texto) { interno.Salvar(id, texto); cache[id] = texto; }

    public string? Obter(string id)
    {
        if (cache.TryGetValue(id, out var valor)) return valor;
        valor = interno.Obter(id);
        cache[id] = valor;
        return valor;
    }

    public bool Remover(string id) { cache.Remove(id); return interno.Remover(id); }
}

// 3) Basta herdar o contrato:
public class ArmazenamentoComCacheTests : ContratoDoArmazenamento
{
    protected override IArmazenamentoDeNotas Criar() => new ArmazenamentoComCache(new ArmazenamentoEmMemoria());
}
```
Reparou? Um **cache** é uma implementação **a mais** do mesmo contrato — e os testes garantem que ele **não quebra** as promessas (por exemplo, `Remover` precisa limpar o cache, ou `Obter` devolveria um valor que não existe mais). É o LSP **verificado**, não apenas desejado.
:::

::: resumo
- **Lei 16:** *faça aos outros o que gostaria que fizessem a você* — honestidade nos detalhes, perdão, ajuda; e **praticar**, não só concordar.
- **Contratos honrados (LSP):** use **testes de contrato** (uma classe abstrata de testes herdada por cada implementação).
- **APIs honestas:** nomes que dizem a verdade (`Calcular` não grava, `TryParse` não lança, `Async` não bloqueia); sem efeitos escondidos.
- **Privacidade:** `Segredo` com `ToString` mascarado, máscaras de cartão/e-mail, logs sem dados sensíveis, LGPD.
- **Segurança:** **SQL parametrizado**, sempre; texto do usuário é **dado**, nunca **comando**.
- **Para quem vem depois:** o código que você escreve hoje é a herança de alguém (talvez a sua); honestidade deve ser a **norma**.
:::

::: quiz
? O que a Regra de Ouro propõe, em essência?
+ Tratar os outros como gostaríamos de ser tratados, com honestidade e ação
- Obedecer sempre ao chefe
- Evitar conflitos a todo custo
- Pensar apenas em si
> E praticá-la, não apenas conhecê-la.

? O que é um teste de contrato?
+ Um conjunto de testes sobre a interface que todas as implementações precisam passar
- Um teste jurídico
- Um teste de performance
- Um teste da interface gráfica
> Garante o LSP na prática.

? Qual destes nomes é desonesto em relação ao que o método faz?
+ Calcular() que também grava no banco
- Salvar() que grava no banco
- TryParse() que devolve false em caso de erro
- Dispose() que libera recursos
> Quem lê "Calcular" não espera efeitos colaterais.

? Por que sobrescrever ToString() em um tipo de segredo?
+ Para que o valor não vaze em logs e interpolações por engano
- Para deixá-lo mais rápido
- Para permitir serialização
- Para criptografá-lo
> Mas lembre que serialização é outro caminho de vazamento.

? Por que parâmetros em SQL evitam injeção?
+ O valor é tratado como dado e nunca interpretado como parte do comando
- Porque deixam a consulta mais rápida
- Porque criptografam o banco
- Porque removem aspas
> Concatenar é entregar a chave do cofre.

? Qual a ligação entre a lei e a dívida técnica?
+ O que deixamos para os outros (o código ruim) costuma voltar para nós quando somos o próximo a mantê-lo
- Nenhuma
- Dívida só afeta clientes
- Só afeta o gerente
> "O que fazemos aos outros, fazemos a nós mesmos."
:::

::: cartas
Regra de Ouro :: Tratar os outros como gostaria de ser tratado.
Teste de contrato :: Testes escritos contra a interface, herdados por todas as implementações.
LSP :: Subtipos devem poder substituir seus tipos base sem quebrar o comportamento.
API honesta :: Nomes e contratos que descrevem exatamente o que o código faz.
Segredo (wrapper) :: Tipo que impede o valor sensível de aparecer em ToString/logs.
Injeção de SQL :: Ataque que insere comandos em consultas montadas por concatenação.
Consulta parametrizada :: Consulta em que os valores viajam separados do comando.
LGPD :: Lei Geral de Proteção de Dados: coleta mínima, finalidade e segurança.
:::
