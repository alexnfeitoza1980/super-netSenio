---
id: aprenda-cap15
order: 27
kind: capitulo
num: 15
group: C# · Parte 6 · Gráficos, memória e LINQ
title: LINQ
subtitle: Controle seus dados — consultas, ordenação, agrupamento, junção e conexão com bancos SQL, tudo com uma sintaxe só.
minutes: 70
hue: 180
objectives:
- Escrever consultas LINQ com **`from … where … select`** e com **métodos** (`Where`, `Select`)
- **Ordenar** (`orderby`), **agrupar** (`group by`) e **juntar** (`join`) coleções
- Usar **tipos anônimos** e **`var`**, e entender a **execução adiada**
- Perceber como o LINQ se conecta a **bancos de dados** e a outras fontes
origem: livros/01-aprenda-csharp/content/17-cap15.md
---

::: ilustracao img/aprenda/cap15.svg | LINQ: descreva o que quer (filtrar, ordenar, transformar) e deixe a consulta fazer o trabalho.
:::

## É um mundo voltado para dados

Acabaram os tempos em que você programava por semanas **sem lidar com montes de dados**. Hoje tudo é dado — e quase sempre vindo de **mais de uma fonte**, em **mais de um formato**: bancos SQL, XML, JSON, coleções do seu programa... É para isso que existe o **LINQ** (*Language Integrated Query*): consultar dados de forma **simples e intuitiva**, agrupar e **unir** fontes diferentes — tudo com a mesma sintaxe, dentro do C#.

::: vila O problema do Jaime: dados espalhados
Jaime vende **revistas em quadrinhos raras**. Seus dados estão em vários lugares: uma lista de quadrinhos com preço e título, uma planilha com o que cada **cliente** comprou, e uma tabela de **avaliações** de colecionadores. Ele quer responder perguntas: *"quais são os mais valiosos?"*, *"quanto cada cliente gastou?"*, *"quais títulos o Fulano ainda não tem?"*
:::

## Coleções do .NET já nascem preparadas

As coleções (`List<T>`, matrizes, `Dictionary`…) implementam **`IEnumerable<T>`** — e é nessa interface que o LINQ opera. Basta ter **`using System.Linq;`**.

```cs
using System.Linq;

class Quadrinho
{
    public string Titulo { get; set; }
    public int Ano { get; set; }
    public decimal Preco { get; set; }
    public string Editora { get; set; }
}

List<Quadrinho> catalogo = new List<Quadrinho>()
{
    new Quadrinho { Titulo = "Aventuras Espaciais #1", Ano = 1962, Preco = 4500m, Editora = "Orion" },
    new Quadrinho { Titulo = "Heróis do Mar #7",       Ano = 1975, Preco = 320m,  Editora = "Delta" },
    new Quadrinho { Titulo = "Mistério na Selva #3",   Ano = 1988, Preco = 90m,   Editora = "Orion" },
    new Quadrinho { Titulo = "Cidade Sombria #12",     Ano = 1962, Preco = 1850m, Editora = "Delta" },
    new Quadrinho { Titulo = "Pequenos Robôs #5",      Ano = 1999, Preco = 45m,   Editora = "Orion" },
};
```

## LINQ torna as consultas fáceis

Uma consulta tem três partes naturais: **de onde** (`from`), **o quê** (`where`) e **o que devolver** (`select`).

```cs
var caros = from q in catalogo
            where q.Preco > 1000m
            select q;

foreach (var q in caros)
    Console.WriteLine(q.Titulo + " — R$ " + q.Preco);
```

```saida
Aventuras Espaciais #1 — R$ 4500
Cidade Sombria #12 — R$ 1850
```

- **`from q in catalogo`** declara uma variável de intervalo `q` que percorre cada item.
- **`where`** mantém só os itens que satisfazem a condição.
- **`select`** define o que vai no resultado (pode ser o próprio item, um campo, ou algo novo).
- **`var`** deixa o compilador **inferir** o tipo (útil quando ele é longo ou **anônimo**).

::: figura O caminho de uma consulta
<svg viewBox="0 0 740 150" role="img" aria-label="Fonte, filtro, ordenação, projeção, resultado">
  <g text-anchor="middle" font-size="13"><rect class="f-prim" x="5" y="40" width="120" height="64" rx="12"/><text x="65" y="68" font-weight="800" class="t-prim">from</text><text x="65" y="88" font-size="11" class="t-dim">a fonte de dados</text>
  <rect class="f-info" x="160" y="40" width="120" height="64" rx="12"/><text x="220" y="68" font-weight="800" class="t-info">where</text><text x="220" y="88" font-size="11" class="t-dim">filtra</text>
  <rect class="f-good" x="315" y="40" width="120" height="64" rx="12"/><text x="375" y="68" font-weight="800" class="t-good">orderby</text><text x="375" y="88" font-size="11" class="t-dim">ordena</text>
  <rect class="f-warn" x="470" y="40" width="120" height="64" rx="12"/><text x="530" y="68" font-weight="800" class="t-warn">select</text><text x="530" y="88" font-size="11" class="t-dim">projeta</text>
  <rect class="f-plain" x="625" y="40" width="110" height="64" rx="12"/><text x="680" y="68" font-weight="800">resultado</text><text x="680" y="88" font-size="11" class="t-dim">IEnumerable&lt;T&gt;</text></g>
  <path class="arrow" d="M127 72H156"/><path class="arrow" d="M282 72H311"/><path class="arrow" d="M437 72H466"/><path class="arrow" d="M592 72H621"/>
</svg>
:::

### As consultas são simples… mas podem ser sofisticadas

Combine condições, ordene e transforme:

```cs
var lista = from q in catalogo
            where q.Editora == "Orion" && q.Ano < 1990
            orderby q.Preco descending
            select q.Titulo + " (" + q.Ano + ")";

// resultado: IEnumerable<string>
```

```cs
// sintaxe de MÉTODOS (equivalente, e mais comum no dia a dia)
var lista2 = catalogo
    .Where(q => q.Editora == "Orion" && q.Ano < 1990)
    .OrderByDescending(q => q.Preco)
    .Select(q => q.Titulo + " (" + q.Ano + ")");
```

::: dica Sintaxe de consulta × sintaxe de métodos
São **intercambiáveis**: o compilador converte a primeira na segunda. Use a que for mais legível. Agrupamentos e junções costumam ficar **mais claros** na sintaxe de consulta; encadeamentos curtos, nos métodos.
:::

## A LINQ é versátil: operadores úteis

| Operador | O que faz | Exemplo |
|---|---|---|
| `Where` | Filtra | `catalogo.Where(q => q.Ano > 1980)` |
| `Select` | Transforma cada item | `catalogo.Select(q => q.Titulo)` |
| `OrderBy` / `ThenBy` | Ordena crescente / desempate | `.OrderBy(q => q.Ano).ThenBy(q => q.Titulo)` |
| `OrderByDescending` | Ordena decrescente | `.OrderByDescending(q => q.Preco)` |
| `First` / `FirstOrDefault` | Primeiro item (ou o padrão se vazio) | `catalogo.FirstOrDefault(q => q.Ano == 1962)` |
| `Count` / `Any` / `All` | Conta / existe algum / todos satisfazem | `catalogo.Any(q => q.Preco > 4000m)` |
| `Sum` / `Average` / `Min` / `Max` | Agregações | `catalogo.Average(q => q.Preco)` |
| `Take` / `Skip` | Pega / pula N itens | `.OrderBy(...).Take(3)` |
| `Distinct` | Remove repetidos | `catalogo.Select(q => q.Editora).Distinct()` |
| `ToList` / `ToArray` | Executa e materializa o resultado | `... .ToList()` |

```cs
decimal media = catalogo.Average(q => q.Preco);
decimal total1962 = catalogo.Where(q => q.Ano == 1962).Sum(q => q.Preco);
Quadrinho maisBarato = catalogo.OrderBy(q => q.Preco).First();
bool temRaro = catalogo.Any(q => q.Preco > 4000m);
```

### Tipos anônimos: projetando formatos novos

`select new { … }` cria um objeto **sem classe declarada**, só com os campos que você pede:

```cs
var resumo = from q in catalogo
             select new { q.Titulo, Decada = (q.Ano / 10) * 10 };

foreach (var r in resumo)
    Console.WriteLine(r.Titulo + " — década de " + r.Decada);
```

::: pense Por que `var` aqui?
Como o tipo **não tem nome**, não dá para escrever `IEnumerable<???>`. O `var` resolve — o compilador sabe o tipo exato e o IntelliSense mostra `r.Titulo` e `r.Decada`.
:::

## Execução adiada (lazy)

Uma consulta LINQ **não executa** quando você a escreve. Ela só roda quando você **percorre o resultado** (`foreach`) ou o **materializa** (`ToList`, `Count`, `First`…).

```cs
var caros = catalogo.Where(q => q.Preco > 1000m);   // NADA executado ainda

catalogo.Add(new Quadrinho { Titulo = "Novo", Preco = 9999m });

foreach (var q in caros) { }        // AGORA executa — e inclui o "Novo"!
```

::: cuidado Cada `foreach` reexecuta a consulta
Se você vai usar o resultado várias vezes (ou quer um "retrato" fixo dos dados), chame `.ToList()` **uma vez** e trabalhe com a lista. Senão, cada percurso refaz todo o trabalho (e pode dar resultados diferentes se a fonte mudar).
:::

::: exercicio Responda com LINQ
Usando o `catalogo` acima, escreva consultas para:

1. Os títulos dos quadrinhos da editora **Delta**, em ordem alfabética.
2. O **preço médio** por editora (dica: a próxima seção).
3. Quantos quadrinhos são **anteriores a 1980**?

--- resposta
```cs
// 1
var delta = catalogo.Where(q => q.Editora == "Delta").OrderBy(q => q.Titulo).Select(q => q.Titulo);

// 2 (usando agrupamento)
var medias = from q in catalogo
             group q by q.Editora into g
             select new { Editora = g.Key, Media = g.Average(x => x.Preco) };

// 3
int antigos = catalogo.Count(q => q.Ano < 1980);
```
:::

## A LINQ pode combinar resultados em grupos

O **`group by`** agrupa itens com a mesma **chave**. Cada grupo tem a **chave** (`Key`) e os **itens** do grupo.

```cs
var porEditora = from q in catalogo
                 group q by q.Editora into grupo
                 orderby grupo.Key
                 select grupo;

foreach (var grupo in porEditora)
{
    Console.WriteLine(grupo.Key + " (" + grupo.Count() + " títulos)");
    foreach (var q in grupo)
        Console.WriteLine("   - " + q.Titulo);
}
```

```saida
Delta (2 títulos)
   - Heróis do Mar #7
   - Cidade Sombria #12
Orion (3 títulos)
   - Aventuras Espaciais #1
   - Mistério na Selva #3
   - Pequenos Robôs #5
```

::: figura Agrupando por editora
<svg viewBox="0 0 700 200" role="img" aria-label="Itens são separados em grupos por chave">
  <g class="t-mono" font-size="11"><rect class="f-plain" x="5" y="15" width="200" height="30" rx="6"/><text x="12" y="35">Aventuras Espaciais · Orion</text><rect class="f-plain" x="5" y="52" width="200" height="30" rx="6"/><text x="12" y="72">Heróis do Mar · Delta</text><rect class="f-plain" x="5" y="89" width="200" height="30" rx="6"/><text x="12" y="109">Mistério na Selva · Orion</text><rect class="f-plain" x="5" y="126" width="200" height="30" rx="6"/><text x="12" y="146">Cidade Sombria · Delta</text><rect class="f-plain" x="5" y="163" width="200" height="30" rx="6"/><text x="12" y="183">Pequenos Robôs · Orion</text></g>
  <path class="arrow" d="M210 100H290"/><text x="250" y="90" text-anchor="middle" font-size="11" class="t-dim">group by Editora</text>
  <rect class="f-warn" x="300" y="15" width="190" height="85" rx="10"/><text x="395" y="38" text-anchor="middle" font-weight="800" class="t-warn">Key = "Delta"</text><text x="395" y="60" text-anchor="middle" font-size="11" class="t-mono">Heróis do Mar</text><text x="395" y="78" text-anchor="middle" font-size="11" class="t-mono">Cidade Sombria</text>
  <rect class="f-info" x="300" y="110" width="190" height="85" rx="10"/><text x="395" y="133" text-anchor="middle" font-weight="800" class="t-info">Key = "Orion"</text><text x="395" y="155" text-anchor="middle" font-size="11" class="t-mono">Aventuras Espaciais</text><text x="395" y="173" text-anchor="middle" font-size="11" class="t-mono">Mistério · Pequenos Robôs</text>
</svg>
:::

## `join`: combinar duas coleções em uma consulta

Quando os dados estão em **duas coleções** que se relacionam por um campo em comum (como uma **chave estrangeira**), use **`join`**:

```cs
class Compra { public string Cliente { get; set; } public string TituloComprado { get; set; } }

List<Compra> compras = new List<Compra>()
{
    new Compra { Cliente = "Ana",   TituloComprado = "Aventuras Espaciais #1" },
    new Compra { Cliente = "Bruno", TituloComprado = "Pequenos Robôs #5" },
    new Compra { Cliente = "Ana",   TituloComprado = "Cidade Sombria #12" },
};

var gastos = from c in compras
             join q in catalogo on c.TituloComprado equals q.Titulo
             select new { c.Cliente, q.Titulo, q.Preco };

var totalPorCliente = from g in gastos
                      group g by g.Cliente into grupo
                      select new { Cliente = grupo.Key, Total = grupo.Sum(x => x.Preco) };

foreach (var t in totalPorCliente)
    Console.WriteLine(t.Cliente + " gastou R$ " + t.Total);
```

```saida
Ana gastou R$ 6350
Bruno gastou R$ 45
```

::: analogia O join é um "cruzamento de listas"
Pegue a lista de compras e, para cada linha, **procure no catálogo** o título correspondente; cole os dados lado a lado. Se o título não existir no catálogo, a linha some do resultado (é um *inner join*).
:::

::: figura Join por um campo em comum
<svg viewBox="0 0 700 190" role="img" aria-label="Compras unidas ao catálogo pelo título">
  <rect class="f-prim" x="10" y="30" width="230" height="120" rx="10"/><text x="125" y="54" text-anchor="middle" font-weight="800" class="t-prim">compras</text><g class="t-mono" font-size="11"><text x="22" y="82">Ana · Aventuras Espaciais #1</text><text x="22" y="104">Bruno · Pequenos Robôs #5</text><text x="22" y="126">Ana · Cidade Sombria #12</text></g>
  <rect class="f-info" x="460" y="30" width="230" height="120" rx="10"/><text x="575" y="54" text-anchor="middle" font-weight="800" class="t-info">catálogo</text><g class="t-mono" font-size="11"><text x="472" y="82">Aventuras Espaciais #1 · 4500</text><text x="472" y="104">Pequenos Robôs #5 · 45</text><text x="472" y="126">Cidade Sombria #12 · 1850</text></g>
  <path class="line" d="M242 78H458M242 100H458M242 122H458" stroke-dasharray="5 4"/><text x="350" y="68" text-anchor="middle" font-size="12" font-weight="700" class="t-prim">equals (Título)</text>
</svg>
:::

## Conecte a LINQ a um banco de dados SQL

Como o LINQ é só uma **sintaxe sobre `IQueryable`/`IEnumerable`**, a mesma consulta pode rodar em **memória**, em **XML**, ou em um **banco SQL**: nesse caso, o provedor **traduz** a consulta C# em comandos `SELECT`.

No livro, isso é feito com **LINQ to SQL** (arraste tabelas para o designer `.dbml` e use um `DataContext`). Hoje a ferramenta equivalente e mantida é o **Entity Framework Core**:

```cs
using (var db = new LojaContext())
{
    var caras = from p in db.Quadrinhos
                where p.Preco > 1000m
                orderby p.Preco descending
                select new { p.Titulo, p.Preco };

    foreach (var c in caras)          // aqui o EF envia um SELECT ... WHERE ... ORDER BY
        Console.WriteLine(c.Titulo);
}
```

O **mesmo** raciocínio (`from/where/select`, `join`, `group by`) funciona. O que muda é a **fonte** (`db.Quadrinhos` em vez de uma lista) e o fato de a consulta virar SQL no servidor — **muito mais eficiente** do que trazer todos os dados para filtrar na memória.

::: curiosidade Um join entre tabelas e coleções
Dá para juntar **dados de fontes diferentes**: uma tabela SQL com uma lista em memória (com cuidado: o provedor precisa saber traduzir). Esse poder de **unir fontes** com uma sintaxe só é o grande trunfo do LINQ.
:::

::: resumo
- **LINQ** consulta qualquer `IEnumerable<T>` (e bancos, XML…) com sintaxe integrada ao C#.
- Consulta: **`from` … `where` … `orderby` … `select`**; ou métodos: `Where`, `OrderBy`, `Select`.
- **Agregações**: `Count`, `Sum`, `Average`, `Min`, `Max`, `Any`, `All`.
- **Tipos anônimos** (`select new { … }`) e **`var`** formatam o resultado.
- **Execução adiada**: a consulta roda quando você a percorre; use **`ToList()`** para fixar o resultado.
- **`group by`** agrupa por chave; **`join`** une coleções por campo comum.
- Com **EF Core/LINQ to SQL**, a consulta é traduzida em SQL.
:::

::: quiz
? Qual namespace é necessário para usar LINQ?
- `System.Data`
+ `System.Linq`
- `System.IO`
- `System.Query`
> `using System.Linq;` traz os métodos de extensão.

? O que faz `where q.Preco > 100` em uma consulta?
+ Mantém apenas os itens que satisfazem a condição
- Ordena os itens por preço
- Soma os preços
- Cria uma nova classe
> `where` é o filtro.

? Quando uma consulta LINQ é realmente executada?
- Quando é declarada
+ Quando seu resultado é percorrido ou materializado (execução adiada)
- Ao compilar
- Apenas com `Execute()`
> `foreach`, `ToList`, `Count`, `First`… disparam a execução.

? Para que serve `select new { q.Titulo, q.Preco }`?
+ Criar um objeto de tipo anônimo só com esses campos
- Alterar a classe Quadrinho
- Salvar no banco
- Ordenar por título
> Tipos anônimos são úteis para projeções.

? O que `group q by q.Editora into g` produz?
+ Grupos, cada um com uma chave (`g.Key`) e os itens correspondentes
- Uma única lista ordenada
- Um erro
- Um dicionário vazio
> Cada grupo é um `IGrouping<TKey, T>`.

? Qual cláusula combina duas coleções por um campo comum?
- `union`
- `merge`
+ `join … on … equals …`
- `link`
> `join` implementa o inner join.

? Qual método materializa o resultado em uma lista para reutilizá-lo sem reexecutar a consulta?
- `Skip()`
+ `ToList()`
- `Where()`
- `Select()`
> `ToList()` executa e guarda os itens em memória.

? Qual a vantagem de usar LINQ com um banco (EF/LINQ to SQL)?
+ A consulta C# é traduzida em SQL e executa no servidor
- Elimina a necessidade de banco
- Faz o banco virar JSON
- Evita índices
> A filtragem acontece no banco, trazendo menos dados.
:::

::: cartas
LINQ :: Language Integrated Query: sintaxe de consulta integrada ao C#.
IEnumerable<T> :: Interface das coleções percorríveis; base do LINQ.
from / where / select :: Fonte / filtro / projeção de uma consulta.
orderby :: Ordena o resultado (ascending/descending).
group by :: Agrupa itens por chave; cada grupo tem `Key`.
join :: Une duas coleções por um campo em comum.
Tipo anônimo :: Objeto criado com `new { }` sem declarar uma classe.
var :: Palavra-chave para tipo inferido pelo compilador.
Execução adiada :: A consulta só roda ao ser percorrida ou materializada.
ToList() :: Executa a consulta e guarda o resultado em uma lista.
Método de extensão (LINQ) :: Where, Select, OrderBy… são extensões de IEnumerable<T>.
LINQ to SQL / EF Core :: Provedores que traduzem consultas LINQ em SQL.
:::
