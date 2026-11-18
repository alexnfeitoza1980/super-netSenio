---
id: aprenda-cap08
order: 19
kind: capitulo
num: 8
group: C# · Parte 4 · Coleções e arquivos
title: Enumerações e coleções
subtitle: Guardando um monte de dados — `enum`, `List<T>`, ordenação com `IComparer`, `Dictionary`, sobrecarga, `Queue` e `Stack`.
minutes: 70
hue: 30
objectives:
- Usar **`enum`** para representar categorias com nomes legíveis
- Trocar **matrizes** por **`List<T>`** e saber ordenar com `IComparable`/`IComparer`
- Armazenar pares chave/valor com **`Dictionary<TKey, TValue>`**
- Conhecer **`Queue<T>`** (fila) e **`Stack<T>`** (pilha) e a **sobrecarga** de métodos
origem: livros/01-aprenda-csharp/content/09-cap08.md
---

::: ilustracao img/aprenda/cap08.svg | Listas guardam itens em ordem; dicionários encontram o valor pela chave.
:::

## Quando chove, pode virar tempestade

No mundo real, os dados raramente vêm em porções pequenas: vêm **aos montes**. Você precisa de ferramentas para armazenar, ordenar e gerenciar muitos itens — são as **coleções**. A ideia: deixar que a coleção cuide do trabalho braçal enquanto você pensa nas regras do programa.

## Enumerações: nomes em vez de números mágicos

Quando uma variável só pode ter **um conjunto pequeno de valores válidos**, uma `string` é perigosa ("Copas", "copas", "COPAS", "Copaz"…). Uma **enumeração** (`enum`) limita os valores a uma lista fixa.

```cs
enum Naipe
{
    Espadas,
    Copas,
    Paus,
    Ouros
}

Naipe meuNaipe = Naipe.Copas;
Console.WriteLine(meuNaipe);          // Copas
Console.WriteLine((int) meuNaipe);    // 1   (por baixo dos panos, cada nome é um número)
```

- Os valores são numerados a partir de **0**, mas você pode escolher: `enum Prioridade { Baixa = 1, Media = 5, Alta = 10 }`.
- É possível converter: `(Naipe) 2` → `Paus`.
- Digitou o nome errado? **O compilador reclama** (uma `string` só reclamaria em execução).

::: regra Enum = lista fechada de opções
Use `enum` para tipos de categoria: dias da semana, estados de um pedido, naipes, níveis de log. O IntelliSense até sugere os valores ao digitar `Naipe.`.
:::

### Uma carta e um baralho

```cs
enum Valor { As = 1, Dois, Tres, Quatro, Cinco, Seis, Sete, Oito, Nove, Dez, Valete, Dama, Rei }

class Carta
{
    public Naipe Naipe { get; private set; }
    public Valor Valor { get; private set; }
    public Carta(Valor valor, Naipe naipe) { Valor = valor; Naipe = naipe; }
    public string Nome { get { return Valor + " de " + Naipe; } }
}
```

Quando o `As` vale 1, os demais seguem a ordem: `Dois = 2`, ... `Rei = 13`. Um baralho é uma **coleção de cartas** — e é aí que as matrizes começam a doer.

## Matrizes são difíceis de trabalhar

Com matrizes, o tamanho é **fixo**. Se você precisa de um baralho que **perde cartas ao distribuir** ou de uma mão que **ganha cartas**, vai ter que:

1. criar outro array maior/menor;
2. copiar os itens à mão;
3. controlar sozinho quantas posições estão realmente em uso.

```cs
Carta[] baralho = new Carta[52];
// Tirar a primeira carta?  Precisaria deslocar as outras 51 posições à mão.
```

## Listas facilitam guardar coleções de… qualquer coisa

A classe **`List<T>`** (do namespace `System.Collections.Generic`) é uma **lista redimensionável** de objetos do tipo `T`. Ela **cresce e encolhe sozinha**.

```cs
using System.Collections.Generic;

List<string> nomes = new List<string>();
nomes.Add("Ana");
nomes.Add("Bruno");
nomes.Add("Carla");
Console.WriteLine(nomes.Count);       // 3
Console.WriteLine(nomes[1]);          // Bruno
nomes.Remove("Bruno");                // encolhe
nomes.Insert(0, "Zeca");              // insere no início
Console.WriteLine(nomes.Contains("Ana"));   // True
Console.WriteLine(nomes.IndexOf("Carla"));  // 2
```

| Membro | Faz |
|---|---|
| `Add(item)` | Acrescenta ao final |
| `Insert(i, item)` | Insere na posição `i` |
| `Remove(item)` / `RemoveAt(i)` | Remove um item |
| `Count` | Quantos itens há (em array é `Length`) |
| `Contains(item)` | Está na lista? |
| `IndexOf(item)` | Posição do item (ou −1) |
| `Clear()` | Esvazia a lista |
| `Sort()` | Ordena (se os itens forem comparáveis) |

::: figura Lista × matriz
<svg viewBox="0 0 700 220" role="img" aria-label="Matriz de tamanho fixo e lista que cresce">
  <text x="10" y="22" font-weight="800" class="t-bad">Matriz — tamanho fixo (3)</text>
  <g class="t-mono" font-size="13" text-anchor="middle"><rect class="f-plain" x="10" y="35" width="80" height="40" rx="6"/><text x="50" y="60">"Ana"</text><rect class="f-plain" x="90" y="35" width="80" height="40" rx="6"/><text x="130" y="60">"Bia"</text><rect class="f-plain" x="170" y="35" width="80" height="40" rx="6"/><text x="210" y="60">"Caio"</text><rect class="f-bad" stroke-dasharray="4 4" x="250" y="35" width="80" height="40" rx="6"/><text x="290" y="60" class="t-bad">✗ não cabe</text></g>
  <text x="10" y="130" font-weight="800" class="t-good">List&lt;string&gt; — cresce sozinha</text>
  <g class="t-mono" font-size="13" text-anchor="middle"><rect class="f-good" x="10" y="145" width="80" height="40" rx="6"/><text x="50" y="170">"Ana"</text><rect class="f-good" x="90" y="145" width="80" height="40" rx="6"/><text x="130" y="170">"Bia"</text><rect class="f-good" x="170" y="145" width="80" height="40" rx="6"/><text x="210" y="170">"Caio"</text><rect class="f-good" x="250" y="145" width="80" height="40" rx="6"/><text x="290" y="170">"Duda"</text><rect class="f-good" x="330" y="145" width="80" height="40" rx="6"/><text x="370" y="170">"Eva"</text><text x="480" y="170" class="t-dim" font-size="13">… e o que vier</text></g>
</svg>
:::

### Os `<T>`: coleções genéricas

O `<T>` é um **parâmetro de tipo**: você diz *do quê* é a lista. `List<int>`, `List<Carta>`, `List<List<string>>`… A lista **só aceita** aquele tipo — o compilador impede colocar um texto numa `List<int>`.

### `foreach`: percorrendo coleções

```cs
foreach (string nome in nomes)
{
    Console.WriteLine(nome);
}
```

::: cuidado Não altere a lista enquanto percorre
Dentro de um `foreach` **não** se deve `Add` ou `Remove` itens da lista que está sendo percorrida: lança `InvalidOperationException`. Para remover, percorra uma **cópia** (`new List<string>(nomes)`) ou use um `for` de trás para frente.
:::

### Inicializadores de coleção

```cs
List<string> frutas = new List<string>() { "maçã", "pera", "uva" };
List<Carta> mao = new List<Carta>()
{
    new Carta(Valor.As, Naipe.Espadas),
    new Carta(Valor.Rei, Naipe.Copas)
};
```

Funcionam como os inicializadores de objeto do capítulo 3.

::: exercicio Um baralho com List
Escreva a classe `Baralho` com uma `List<Carta>` que:

1. se preenche com as 52 cartas no construtor (todos os valores × todos os naipes);
2. tem `Carta Distribuir()` que **remove e devolve** a primeira carta;
3. tem `void Embaralhar()` que troca cartas de lugar aleatoriamente.

--- resposta
```cs
class Baralho
{
    private List<Carta> cartas = new List<Carta>();
    private Random random = new Random();

    public Baralho()
    {
        foreach (Naipe n in Enum.GetValues(typeof(Naipe)))
            foreach (Valor v in Enum.GetValues(typeof(Valor)))
                cartas.Add(new Carta(v, n));
    }

    public int Count { get { return cartas.Count; } }

    public Carta Distribuir()
    {
        Carta c = cartas[0];
        cartas.RemoveAt(0);
        return c;
    }

    public void Embaralhar()
    {
        List<Carta> nova = new List<Carta>();
        while (cartas.Count > 0)
        {
            int i = random.Next(cartas.Count);
            nova.Add(cartas[i]);
            cartas.RemoveAt(i);
        }
        cartas = nova;
    }
}
```
`Enum.GetValues(typeof(Naipe))` percorre todos os valores do enum. Note como a `List` dispensa o controle manual do tamanho que a matriz exigiria.
:::

## Ordenando: pode ser complicado

Ordenar números ou textos simples é fácil: `lista.Sort()`. Mas **como o C# sabe ordenar patos, cartas ou contatos**? Você precisa ensinar a **regra de comparação**. Há duas maneiras.

::: vila A lista de patos
Um programa guarda patos com `Tamanho` (em cm) e `Tipo` (um `enum`: Marreco, Pato-de-borracha, Mandarim). Para exibir patos "do menor para o maior", precisamos ensinar a lista a compará-los.
:::

### 1. `IComparable<T>` — a própria classe sabe se comparar

```cs
class Pato : IComparable<Pato>
{
    public int Tamanho;
    public TipoPato Tipo;

    public int CompareTo(Pato outro)
    {
        if (Tamanho > outro.Tamanho) return 1;     // depois
        if (Tamanho < outro.Tamanho) return -1;    // antes
        return 0;                                  // iguais
    }
}

patos.Sort();     // usa o CompareTo
```

### 2. `IComparer<T>` — um objeto separado decide

Quando você quer **vários critérios** (por tamanho, por tipo...), crie classes **comparadoras**:

```cs
class PorTipoDepoisTamanho : IComparer<Pato>
{
    public int Compare(Pato x, Pato y)
    {
        if (x.Tipo < y.Tipo) return -1;
        if (x.Tipo > y.Tipo) return 1;
        return x.Tamanho.CompareTo(y.Tamanho);   // desempate
    }
}

patos.Sort(new PorTipoDepoisTamanho());
```

::: passos Como o Sort usa o seu comparador
`patos.Sort(new PorTipoDepoisTamanho());`
===
O `Sort` pega **dois patos** da lista e pergunta ao comparador: `Compare(x, y)`.
===
A resposta é um número: **negativo** (x vem antes), **zero** (empatados) ou **positivo** (x vem depois).
===
O algoritmo repete a pergunta quantas vezes precisar, rearranjando a lista, até que todos os pares estejam na ordem correta.
:::

::: dica Nos dias de hoje
Você também pode ordenar com uma **expressão lambda**: `patos.Sort((a, b) => a.Tamanho.CompareTo(b.Tamanho));` ou, com LINQ (capítulo 15), `patos.OrderBy(p => p.Tamanho)`.
:::

## Dicionários: chave → valor

Uma lista é **numerada** (0, 1, 2…). Às vezes você quer encontrar algo **pelo nome**: o telefone de "Ana", o preço do "café". O **`Dictionary<TKey, TValue>`** associa **chaves** a **valores**.

```cs
Dictionary<string, string> comidas = new Dictionary<string, string>();
comidas["Pão"] = "Pão francês, R$ 0,80";
comidas.Add("Café", "Café coado, R$ 4,50");

Console.WriteLine(comidas["Café"]);              // Café coado, R$ 4,50
Console.WriteLine(comidas.ContainsKey("Suco"));  // False
comidas.Remove("Pão");

foreach (KeyValuePair<string, string> par in comidas)
    Console.WriteLine(par.Key + " => " + par.Value);
```

::: figura Um dicionário
<svg viewBox="0 0 640 190" role="img" aria-label="Chaves apontando para valores">
  <text x="10" y="20" font-weight="800">Dictionary&lt;string, string&gt;</text>
  <g class="t-mono" font-size="13">
    <rect class="f-prim" x="10" y="35" width="140" height="38" rx="8"/><text x="22" y="59">"Pão"</text>
    <rect class="f-prim" x="10" y="82" width="140" height="38" rx="8"/><text x="22" y="106">"Café"</text>
    <rect class="f-prim" x="10" y="129" width="140" height="38" rx="8"/><text x="22" y="153">"Suco"</text>
    <rect class="f-info" x="330" y="35" width="300" height="38" rx="8"/><text x="342" y="59">"Pão francês, R$ 0,80"</text>
    <rect class="f-info" x="330" y="82" width="300" height="38" rx="8"/><text x="342" y="106">"Café coado, R$ 4,50"</text>
    <rect class="f-info" x="330" y="129" width="300" height="38" rx="8"/><text x="342" y="153">"Suco natural, R$ 7,00"</text>
  </g>
  <path class="arrow" d="M152 54H326"/><path class="arrow" d="M152 101H326"/><path class="arrow" d="M152 148H326"/>
  <text x="160" y="36" font-size="12" class="t-dim">chave → valor</text>
</svg>
:::

- **Chaves são únicas**: `Add` de uma chave repetida lança exceção; `dic[chave] = novo` simplesmente substitui.
- Procurar uma chave é **muito rápido** (independe do tamanho).
- Chaves e valores podem ter **tipos diferentes**: `Dictionary<int, Carta>`, `Dictionary<string, List<string>>`.
- Para ler sem exceção quando a chave pode não existir: `if (dic.TryGetValue("Suco", out string v)) { ... }`.

::: pense Lista ou dicionário?
**Lista**: quando a **ordem/posição** importa e você percorre todos os itens. **Dicionário**: quando você precisa **encontrar um item pela identidade** ("o cliente 4812") sem varrer tudo.
:::

## Sobrecarga de métodos

Você pode ter vários métodos **com o mesmo nome**, desde que os **parâmetros** sejam diferentes em número ou tipo. É a **sobrecarga** (*overloading*) — o IntelliSense do `MessageBox.Show` mostra 21 versões!

```cs
class Calculadora
{
    public int Somar(int a, int b) { return a + b; }
    public double Somar(double a, double b) { return a + b; }
    public int Somar(int a, int b, int c) { return a + b + c; }
}
```

O compilador escolhe pela **assinatura** (nome + lista de parâmetros). O **tipo de retorno sozinho** não diferencia sobrecargas. Os **construtores** também podem ser sobrecarregados — foi o que fizemos no capítulo 5 e usaremos para o baralho (construtor vazio vs. construtor que lê cartas de um arquivo).

## Outras coleções: fila e pilha

### Fila (`Queue<T>`): primeiro a entrar, primeiro a sair (PEPS / FIFO)

```cs
Queue<string> fila = new Queue<string>();
fila.Enqueue("Ana");     // entra no fim
fila.Enqueue("Bruno");
fila.Enqueue("Carla");
Console.WriteLine(fila.Dequeue());   // Ana — quem chegou primeiro sai primeiro
Console.WriteLine(fila.Peek());      // Bruno — espia sem remover
```

### Pilha (`Stack<T>`): último a entrar, primeiro a sair (UEPS / LIFO)

```cs
Stack<string> pilha = new Stack<string>();
pilha.Push("prato 1");
pilha.Push("prato 2");
pilha.Push("prato 3");
Console.WriteLine(pilha.Pop());      // prato 3 — o último empilhado sai primeiro
```

::: analogia Fila do banco × pilha de pratos
Na **fila** quem chegou antes é atendido antes. Numa **pilha** de pratos você pega o **de cima**, o último colocado. Aplicações: fila de impressão (Queue); botão **Desfazer** de um editor (Stack).
:::

::: figura Fila × Pilha
<svg viewBox="0 0 720 200" role="img" aria-label="Fila e pilha">
  <text x="10" y="20" font-weight="800" class="t-info">Queue (fila) — PEPS</text>
  <g class="t-mono" font-size="13" text-anchor="middle"><rect class="f-info" x="60" y="40" width="90" height="46" rx="8"/><text x="105" y="68">Ana</text><rect class="f-info" x="160" y="40" width="90" height="46" rx="8"/><text x="205" y="68">Bruno</text><rect class="f-info" x="260" y="40" width="90" height="46" rx="8"/><text x="305" y="68">Carla</text></g>
  <path class="arrow" d="M58 63H12"/><text x="12" y="105" font-size="12" class="t-dim">sai (Dequeue)</text><path class="arrow" d="M410 63H356"/><text x="360" y="105" font-size="12" class="t-dim">entra (Enqueue)</text>
  <text x="470" y="20" font-weight="800" class="t-warn">Stack (pilha) — UEPS</text>
  <g class="t-mono" font-size="13" text-anchor="middle"><rect class="f-warn" x="520" y="130" width="140" height="40" rx="8"/><text x="590" y="156">prato 1</text><rect class="f-warn" x="520" y="86" width="140" height="40" rx="8"/><text x="590" y="112">prato 2</text><rect class="f-warn" x="520" y="42" width="140" height="40" rx="8"/><text x="590" y="68">prato 3</text></g>
  <path class="arrow" d="M590 20V38" style="stroke:var(--warn)"/><text x="680" y="68" font-size="12" class="t-dim">Push/Pop</text>
</svg>
:::

::: resumo
- **`enum`**: lista fechada de nomes (numerados por baixo dos panos).
- **Matriz** tem tamanho fixo; **`List<T>`** cresce e encolhe (`Add`, `Remove`, `Count`).
- Para ordenar objetos, implemente **`IComparable<T>`** (ordem natural) ou **`IComparer<T>`** (critérios extras).
- **`Dictionary<K,V>`** encontra valores pela chave de forma muito eficiente.
- **Sobrecarga**: mesmo nome, parâmetros diferentes.
- **`Queue<T>`** é PEPS; **`Stack<T>`** é UEPS.
:::

::: quiz
? Qual é o principal benefício de usar `enum` em vez de `string` para categorias?
+ O compilador valida os valores e evita erros de digitação
- É mais rápido de escrever, apenas
- Permite valores ilimitados
- Dispensa o uso de classes
> Um enum limita o conjunto de valores válidos.

? Qual é a principal vantagem da `List<T>` sobre uma matriz?
- Ocupa menos memória sempre
+ Cresce e encolhe automaticamente
- É a única que aceita `foreach`
- Permite índices negativos
> `Add`/`Remove` ajustam o tamanho sem trabalho manual.

? Para uma lista de `Pato`s ser ordenada por `Sort()` sem argumentos, `Pato` deve…
+ Implementar `IComparable<Pato>`
- Ser `static`
- Herdar de `List`
- Ter apenas campos públicos
> `Sort` usa `CompareTo` da própria classe.

? O que um método `Compare(x, y)` retorna quando `x` deve vir antes de `y`?
- Um número positivo
- Zero
+ Um número negativo
- `true`
> Negativo: x antes; positivo: x depois; zero: iguais.

? Quando usar `Dictionary` em vez de `List`?
+ Quando você precisa localizar rapidamente um valor por uma chave
- Quando a ordem de inserção é o mais importante
- Quando há menos de 3 itens
- Nunca; a lista é sempre melhor
> O dicionário associa chaves (únicas) a valores.

? Qual estrutura devolve primeiro o último item inserido?
- `Queue<T>`
- `List<T>`
+ `Stack<T>`
- `Dictionary<K,V>`
> A pilha segue UEPS (LIFO).

? O que é **sobrecarga** de método?
+ Ter métodos com o mesmo nome mas parâmetros diferentes
- Um método que chama a si mesmo
- Um método com muitos comandos
- Substituir um método da classe base
> O compilador escolhe pela assinatura.

? O que acontece ao dar `Add` de uma chave repetida em um `Dictionary`?
- A chave é sobrescrita silenciosamente
+ Lança uma exceção (use o indexador `dic[chave] = ...` para substituir)
- É ignorado
- O dicionário duplica a chave
> Chaves devem ser únicas.
:::

::: cartas
enum :: Tipo com um conjunto fixo de valores nomeados.
List<T> :: Lista genérica redimensionável de itens do tipo T.
Genérico <T> :: Parâmetro de tipo: define o tipo dos itens da coleção.
foreach :: Laço que percorre cada item de uma coleção.
IComparable<T> :: A classe define sua ordem natural com `CompareTo`.
IComparer<T> :: Objeto externo que compara dois itens (`Compare`) — permite vários critérios.
Dictionary<K,V> :: Coleção de pares chave → valor, com chaves únicas.
Sobrecarga :: Vários métodos com mesmo nome e assinaturas diferentes.
Queue<T> :: Fila: primeiro a entrar, primeiro a sair (Enqueue/Dequeue).
Stack<T> :: Pilha: último a entrar, primeiro a sair (Push/Pop).
Inicializador de coleção :: `new List<int>() { 1, 2, 3 }`.
:::
