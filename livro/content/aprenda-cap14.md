---
id: aprenda-cap14
order: 26
kind: capitulo
num: 14
group: C# · Parte 6 · Gráficos, memória e LINQ
title: A morte do objeto
subtitle: Finalizadores, `Dispose` e o coletor de lixo; `struct` (tipos de valor), stack × heap e métodos de extensão.
minutes: 60
hue: 180
objectives:
- Entender o **ciclo de vida** de um objeto e o papel do **coletor de lixo**
- Saber a diferença entre **finalizador** (`~Classe`) e **`Dispose`**, e quando usar cada um
- Usar **`struct`** e distinguir **tipos de valor** de **tipos de referência**
- Compreender **stack × heap** e criar **métodos de extensão**
origem: livros/01-aprenda-csharp/content/16-cap14.md
---

::: ilustracao img/aprenda/cap14.svg | Memória: quando ninguém mais aponta para um objeto, o coletor de lixo o recolhe.
:::

## O fim de um objeto também importa

Você já sabe **criar** objetos (`new`) e que, sem referências, eles viram **lixo**. Neste capítulo olhamos o que acontece no **último suspiro** do objeto: como o .NET o recolhe, se você pode "executar algo" antes de ele ir embora e quais são as armadilhas dessa ideia.

::: vila Um super-herói e seu arqui-inimigo
Nosso super-herói, o mais impressionante objeto da Vila Objeto, persegue seu arqui-inimigo… e é derrotado, deixando de existir. Quando um objeto "morre", dá para ele tentar uma **última ação**? E se esse herói segurava um **arquivo aberto**? Quem o fecha?
:::

## O ciclo de vida de um objeto

::: figura Nascimento, vida e morte
<svg viewBox="0 0 760 200" role="img" aria-label="Ciclo de vida do objeto">
  <g text-anchor="middle" font-size="13">
    <rect class="f-good" x="5" y="50" width="130" height="80" rx="12"/><text x="70" y="82" font-weight="800" class="t-good">new</text><text x="70" y="104" font-size="11" class="t-dim">construtor roda</text>
    <rect class="f-info" x="165" y="50" width="130" height="80" rx="12"/><text x="230" y="82" font-weight="800" class="t-info">em uso</text><text x="230" y="104" font-size="11" class="t-dim">há referências</text>
    <rect class="f-warn" x="325" y="50" width="130" height="80" rx="12"/><text x="390" y="82" font-weight="800" class="t-warn">inalcançável</text><text x="390" y="104" font-size="11" class="t-dim">sem referências</text>
    <rect class="f-bad" x="485" y="50" width="130" height="80" rx="12"/><text x="550" y="82" font-weight="800" class="t-bad">finalizador</text><text x="550" y="104" font-size="11" class="t-dim">(se existir)</text>
    <rect class="f-plain" x="645" y="50" width="110" height="80" rx="12"/><text x="700" y="82" font-weight="800">memória</text><text x="700" y="104" font-size="11" class="t-dim">liberada pelo GC</text>
  </g>
  <path class="arrow" d="M137 90H161"/><path class="arrow" d="M297 90H321"/><path class="arrow" d="M457 90H481"/><path class="arrow" d="M617 90H641"/>
  <text x="380" y="170" text-anchor="middle" font-size="13" class="t-dim">Você controla o “new”… mas o .NET decide QUANDO o último passo acontece.</text>
</svg>
:::

## Seu último chance de fazer algo: o finalizador

Um **finalizador** (*destructor* em C#) é um método especial, com o nome da classe precedido de **`~`**, que o coletor de lixo chama **pouco antes** de liberar o objeto:

```cs
class Heroi
{
    public string Nome { get; set; }

    public Heroi(string nome) { Nome = nome; }

    ~Heroi()                       // finalizador: sem modificador, sem parâmetros
    {
        Console.WriteLine(Nome + " está sendo coletado!");
    }
}

Heroi h = new Heroi("Alfa");
h = null;                          // sem referência: elegível para coleta
GC.Collect();                      // (só para demonstração!) força uma coleta
GC.WaitForPendingFinalizers();
```

```saida
Alfa está sendo coletado!
```

### QUANDO exatamente o finalizador executa?

**Você não sabe.** O coletor de lixo decide **quando** rodar (geralmente com a memória apertada ou em momentos ociosos). O finalizador pode executar **milissegundos** ou **minutos** depois — ou nem executar se o programa terminar antes.

::: cuidado Finalizadores não são destrutores do C++
Em C++ o destrutor roda **exatamente** quando o objeto sai de escopo. Em C#, o finalizador é **não determinístico**. Nunca dependa dele para algo que precisa acontecer **num momento certo** (como fechar um arquivo, soltar um bloqueio, liberar uma conexão).
:::

### Finalizadores não podem depender de estabilidade

Quando o finalizador roda, **o mundo ao redor pode já ter sido desmontado**:

- Outros objetos a que o seu faz referência **podem já ter sido finalizados** (e estar inválidos).
- Streams podem já estar fechados.
- Não se pode confiar na **ordem** em que os finalizadores rodam.

Por isso **não tente salvar dados complexos** (por exemplo, serializar o objeto) no finalizador: os objetos de que a serialização precisa podem já ter desaparecido. Para tarefas de limpeza importantes use **`Dispose`**.

## `Dispose()` funciona com `using`; finalizador, com o GC

| | `Dispose()` | Finalizador (`~Classe`) |
|---|---|---|
| Quem chama | **Você** (ou `using`) | O **coletor de lixo** |
| Quando | **Determinístico**: no momento que você escolhe | **Imprevisível** |
| Para quê | Liberar recursos (arquivos, conexões, handles) | **Rede de segurança** caso esqueçam o `Dispose` |
| Garantia | Só se alguém o chamar | Pode nem rodar ao fechar o programa |

O padrão completo, em classes que seguram recursos:

```cs
class Relatorio : IDisposable
{
    private StreamWriter arquivo = new StreamWriter("relatorio.txt");
    private bool descartado = false;

    public void Escrever(string linha) { arquivo.WriteLine(linha); }

    public void Dispose()
    {
        Liberar();
        GC.SuppressFinalize(this);     // já limpei; o GC não precisa chamar o finalizador
    }

    ~Relatorio() { Liberar(); }         // rede de segurança

    private void Liberar()
    {
        if (!descartado)
        {
            arquivo.Close();
            descartado = true;
        }
    }
}

// uso correto: o using chama Dispose, determinístico
using (Relatorio r = new Relatorio())
{
    r.Escrever("Linha 1");
}
```

::: regra Limpeza = `Dispose` + `using`
Para objetos que seguram recursos externos, **implemente `IDisposable`** e **use `using`**. O finalizador é só o plano B. Na maioria dos casos você **nem precisa** de finalizador: só quando segura recursos **não gerenciados** diretamente.
:::

### Um objeto pode serializar a si mesmo no `Dispose`

Como o `Dispose` roda quando **você** manda (com tudo ainda intacto), ele **é** um bom lugar para um objeto **salvar seu estado** (por exemplo, gravar o progresso do herói num arquivo) antes de ser descartado.

```cs
class Jogo : IDisposable
{
    public int Pontos { get; set; }
    public void Dispose() { File.WriteAllText("save.json", JsonSerializer.Serialize(this)); }
}
```

## Mais sobre a memória: stack × heap

O .NET tem **duas áreas** principais de memória para dados:

::: figura Stack e heap
<svg viewBox="0 0 720 280" role="img" aria-label="Stack com variáveis e referências; heap com objetos">
  <rect class="f-prim" x="10" y="15" width="250" height="250" rx="12"/><text x="135" y="40" text-anchor="middle" font-weight="800" class="t-prim">STACK (pilha)</text>
  <g class="t-mono" font-size="13"><rect class="f-plain" x="30" y="55" width="210" height="40" rx="6"/><text x="42" y="80">int idade = 30</text><rect class="f-plain" x="30" y="105" width="210" height="40" rx="6"/><text x="42" y="130">double preco = 9.9</text><rect class="f-plain" x="30" y="155" width="210" height="40" rx="6"/><text x="42" y="180">Ponto p (struct)  x=1,y=2</text><rect class="f-plain" x="30" y="205" width="210" height="40" rx="6"/><text x="42" y="230">Heroi h  ───┐</text></g>
  <rect class="f-info" x="420" y="15" width="290" height="250" rx="12"/><text x="565" y="40" text-anchor="middle" font-weight="800" class="t-info">HEAP (pilha dinâmica)</text>
  <g class="t-mono" font-size="13"><rect class="f-warn" x="445" y="75" width="240" height="70" rx="8"/><text x="460" y="102">objeto Heroi</text><text x="460" y="124">Nome = "Alfa"</text><rect class="f-warn" x="445" y="165" width="240" height="60" rx="8"/><text x="460" y="195">objeto List&lt;int&gt;</text></g>
  <path class="arrow" d="M242 225C330 225 380 130 441 118"/>
  <text x="270" y="262" font-size="11" class="t-dim">a stack guarda valores simples e referências</text>
</svg>
:::

| | **Stack** | **Heap** |
|---|---|---|
| Guarda | Variáveis locais de **tipo de valor** e as **referências** | Os **objetos** (instâncias de classes) |
| Liberação | Automática ao sair do método | Pelo **coletor de lixo** |
| Velocidade | Muito rápida | Mais lenta (alocação e GC) |
| Tamanho | Pequeno | Grande |

## Estruturas (`struct`): parecem objetos, mas são valores

Uma **estrutura** parece uma classe — tem campos, propriedades, métodos, construtores — mas é um **tipo de valor**: o valor fica **dentro** da variável (na stack, ou embutido no objeto que a contém) e **não** no heap.

```cs
struct Ponto
{
    public int X;
    public int Y;
    public Ponto(int x, int y) { X = x; Y = y; }
    public double DistanciaDaOrigem() { return Math.Sqrt(X * X + Y * Y); }
}

Ponto a = new Ponto(1, 2);
Ponto b = a;       // CÓPIA do valor
b.X = 99;
Console.WriteLine(a.X);   // 1  ← a não mudou!
```

Se `Ponto` fosse uma **classe**, `b = a` faria as duas variáveis apontarem **para o mesmo objeto** e `a.X` valeria 99.

::: regra Valores são copiados; referências são atribuídas
- **Tipo de valor** (`int`, `double`, `bool`, `struct`, `enum`): cada variável tem **sua própria cópia**.
- **Tipo de referência** (`class`, `string`, matrizes, delegados): variáveis **apontam para o mesmo objeto**.
:::

::: figura Copiar um struct × copiar uma referência
<svg viewBox="0 0 720 210" role="img" aria-label="Struct duplica o valor; classe duplica só a referência">
  <text x="170" y="22" text-anchor="middle" font-weight="800" class="t-good">struct Ponto: b = a</text>
  <g class="t-mono" font-size="13"><rect class="f-good" x="40" y="40" width="120" height="50" rx="8"/><text x="100" y="62" text-anchor="middle">a</text><text x="100" y="80" text-anchor="middle" font-size="11">X=1  Y=2</text><rect class="f-good" x="190" y="40" width="120" height="50" rx="8"/><text x="250" y="62" text-anchor="middle">b</text><text x="250" y="80" text-anchor="middle" font-size="11">X=99 Y=2</text></g>
  <text x="170" y="120" text-anchor="middle" font-size="12" class="t-dim">duas cópias independentes</text>
  <text x="540" y="22" text-anchor="middle" font-weight="800" class="t-warn">class Ponto: b = a</text>
  <g class="t-mono" font-size="13"><rect class="f-prim" x="420" y="40" width="70" height="36" rx="8"/><text x="455" y="63" text-anchor="middle">a</text><rect class="f-prim" x="420" y="100" width="70" height="36" rx="8"/><text x="455" y="123" text-anchor="middle">b</text><rect class="f-warn" x="590" y="55" width="120" height="70" rx="8"/><text x="650" y="82" text-anchor="middle">objeto</text><text x="650" y="104" text-anchor="middle" font-size="11">X=99 Y=2</text></g>
  <path class="arrow" d="M492 58C540 58 560 70 586 80"/><path class="arrow" d="M492 118C540 118 560 108 586 100"/>
  <text x="540" y="170" text-anchor="middle" font-size="12" class="t-dim">um único objeto, duas referências</text>
</svg>
:::

### Quando usar struct?

- Valores **pequenos e imutáveis** (≲ 16 bytes): pontos, cores, coordenadas, medidas.
- **Muitos** valores pequenos (evita pressão no heap).
- **Não** use para entidades com identidade (Cliente, Pedido) nem quando precisa de **herança** (`struct` não herda de outra struct/classe; só implementa interfaces).

::: erro Struct mutável é fonte de bugs
Como é copiado a cada atribuição e a cada chamada de método, alterar uma **cópia** sem querer é fácil. Prefira structs **imutáveis** (`readonly struct`).
:::

### Boxing: quando um valor vira objeto

Ao tratar um valor como `object`, o C# **embrulha** (*boxing*) o valor em um objeto no heap; ao voltar, **desembrulha** (*unboxing*). Funciona, mas custa memória e tempo:

```cs
int n = 42;
object caixa = n;          // boxing: cria um objeto no heap com o 42
int de_volta = (int) caixa;   // unboxing
```

## Métodos de extensão: acrescentando comportamento a classes EXISTENTES

E se você quer um método a mais em **`string`** — e não pode alterar a classe `string` (ela é do .NET e é *sealed*)? Um **método de extensão** adiciona um método a **qualquer tipo**, **sem herdar** nem modificá-lo.

Regras: uma **classe estática**, um **método estático**, e o primeiro parâmetro com **`this`** antes do tipo:

```cs
static class ExtensoesDeTexto
{
    public static string Truncar(this string texto, int tamanho)
    {
        if (texto == null) return null;
        return texto.Length <= tamanho ? texto : texto.Substring(0, tamanho) + "…";
    }

    public static bool EhEmail(this string texto)
    {
        return texto != null && texto.Contains("@") && texto.Contains(".");
    }
}

// uso: parece método do próprio string!
string frase = "Aprender C# é muito divertido";
Console.WriteLine(frase.Truncar(10));         // Aprender C…
Console.WriteLine("ana@site.com".EhEmail());  // True
```

::: dica Estenda o que você não controla
Dá para estender `string`, `int`, `List<T>`, uma classe do .NET ou de terceiros. A IDE mostra o método no IntelliSense com um ícone de seta. **É assim que o LINQ funciona**: `Where`, `Select`, `OrderBy` são **métodos de extensão** de `IEnumerable<T>` (próximo capítulo).
:::

::: exercicio Qual vai ser a saída?
```cs
struct S { public int V; }
class C { public int V; }

S s1 = new S { V = 1 };
S s2 = s1;
s2.V = 2;

C c1 = new C { V = 1 };
C c2 = c1;
c2.V = 2;

Console.WriteLine(s1.V + " " + c1.V);
```

--- resposta
Saída: **`1 2`**.

`S` é um **struct** (tipo de valor): `s2 = s1` **copia**; alterar `s2.V` não afeta `s1`. `C` é uma **classe**: `c2 = c1` copia a **referência**; ambas apontam para o mesmo objeto, então `c1.V` também vira 2.
:::

::: resumo
- Quando nenhuma referência aponta para um objeto, ele fica **inalcançável** e o GC o recolhe em momento **imprevisível**.
- **Finalizador** (`~Classe`) é uma rede de segurança; não dependa dele para limpeza importante.
- **`Dispose`** + **`using`** = limpeza **determinística** (e `GC.SuppressFinalize`).
- Na **stack** ficam valores locais e referências; no **heap**, os objetos.
- **`struct`** é um tipo de **valor** (copiado); **`class`** é de **referência** (apontada).
- **Boxing** transforma valor em objeto (com custo).
- **Métodos de extensão** (`this` no 1º parâmetro) acrescentam métodos a tipos existentes.
:::

::: quiz
? Quando o finalizador de um objeto é executado?
- Imediatamente ao fazer `obj = null`
+ Em um momento que o coletor de lixo decide (imprevisível)
- Ao chamar `new`
- Sempre ao fechar o método
> Finalização é não determinística.

? Qual é a forma recomendada de garantir a liberação de um arquivo aberto?
+ `IDisposable` + `using`
- Esperar o finalizador
- Chamar `GC.Collect()` sempre
- Deixar o arquivo aberto
> `using` chama `Dispose` ao sair do bloco.

? Em `struct P {..}; P a = new P{X=1}; P b = a; b.X = 9;` qual é o valor de `a.X`?
+ 1
- 9
- 0
- Erro de compilação
> Structs são copiados na atribuição.

? Onde ficam os objetos criados com `new` de uma classe?
- Na stack
+ No heap
- No registro do Windows
- No disco
> O heap é gerenciado pelo GC.

? O que um método de extensão exige?
+ Classe estática, método estático e `this` antes do primeiro parâmetro
- Herdar da classe estendida
- Ser `virtual`
- Modificar o código-fonte do tipo
> `public static string Truncar(this string s, ...)`.

? O que é *boxing*?
+ Embrulhar um valor em um objeto (heap)
- Dividir um objeto em partes
- Converter texto em número
- Um tipo de coleção
> Tratar um valor como `object` cria um objeto no heap.

? Por que não se deve salvar o estado complexo do objeto em um finalizador?
+ Porque outros objetos de que ele depende podem já ter sido finalizados
- Porque o finalizador não compila
- Porque `File` é estático
- Porque salvar é proibido
> Não há garantia de ordem nem de estabilidade durante a finalização.
:::

::: cartas
Coletor de lixo (GC) :: Libera automaticamente a memória de objetos sem referências.
Finalizador (`~Classe`) :: Método chamado pelo GC antes de liberar o objeto; momento imprevisível.
Dispose() :: Método de IDisposable para liberar recursos de forma determinística.
GC.SuppressFinalize :: Informa ao GC que o finalizador não é mais necessário.
Stack :: Área rápida: valores locais, referências, structs locais.
Heap :: Área de objetos instanciados com `new` (classes).
struct :: Tipo de valor: copiado na atribuição; não herda de classes.
Tipo de valor × referência :: Valor é copiado; referência aponta para o mesmo objeto.
Boxing / unboxing :: Embrulhar valor em object / extrair o valor de volta.
Método de extensão :: Método estático com `this` que parece pertencer ao tipo estendido.
:::
