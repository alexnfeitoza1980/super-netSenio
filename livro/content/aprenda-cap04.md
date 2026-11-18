---
id: aprenda-cap04
order: 14
kind: capitulo
num: 4
group: C# · Parte 2 · Dados e referências
title: Tipos e referências
subtitle: São 10 horas. Você sabe onde seus dados estão? Tipos de dados, conversões, referências, coleta de lixo e matrizes.
minutes: 60
hue: 205
objectives:
- Escolher o **tipo certo** para cada variável (`int`, `long`, `double`, `decimal`, `char`, `bool`…)
- Entender **conversão** implícita, **coerção** explícita (cast) e estouro de tamanho
- Distinguir **tipos de valor** de **variáveis de referência** (objetos)
- Saber o que é **coleta de lixo**, `null` e como **matrizes** (arrays) funcionam
origem: livros/01-aprenda-csharp/content/04-cap04.md
---

::: ilustracao img/aprenda/cap04.svg | Tipos de valor guardam o dado; referências apontam para um objeto que pode ser compartilhado.
:::

## O tipo da variável define o que ela guarda

Sem dados, programas são inúteis. Dados chegam dos usuários, são processados e devolvidos. Quase tudo que você faz em programação envolve dados — e **cada dado tem um tipo**.

::: analogia Variável = copo
Imagine uma variável como um **copo**: um copo de 300 ml (`int`) comporta certos valores; um balde (`long`) comporta muito mais. Tentar enfiar 5 litros num copo derrama (estouro). E um copo de água não serve para areia: **o tipo** define que espécie de dado entra.
:::

### Os tipos numéricos de valor mais usados

| Tipo | Bits | Guarda | Faixa aproximada |
|---|---|---|---|
| `byte` | 8 | inteiro sem sinal | 0 a 255 |
| `sbyte` | 8 | inteiro | −128 a 127 |
| `short` | 16 | inteiro | −32.768 a 32.767 |
| `ushort` | 16 | inteiro sem sinal | 0 a 65.535 |
| `int` | 32 | inteiro | ≈ ±2,1 bilhões |
| `uint` | 32 | inteiro sem sinal | 0 a ≈ 4,3 bilhões |
| `long` | 64 | inteiro | ≈ ±9,2 × 10¹⁸ |
| `ulong` | 64 | inteiro sem sinal | 0 a ≈ 1,8 × 10¹⁹ |
| `float` | 32 | decimal (≈ 7 dígitos) | ≈ ±3,4 × 10³⁸ |
| `double` | 64 | decimal (≈ 15 dígitos) | ≈ ±1,7 × 10³⁰⁸ |
| `decimal` | 128 | decimal exato (28–29 dígitos) | ≈ ±7,9 × 10²⁸ |
| `char` | 16 | um caractere Unicode | `'A'`, `'ç'` |
| `bool` | — | verdadeiro ou falso | `true`, `false` |

::: dica Qual escolher no dia a dia?
`int` para contagens, `double` para medidas científicas, **`decimal` para dinheiro** (não tem erros de arredondamento binário), `string` para texto, `bool` para sim/não. Os demais, só quando há um motivo.
:::

### Literais: o "sufixo" que dá o tipo ao número

```cs
float f = 3.14f;      // sem o f, 3.14 é double e não cabe em float
decimal preco = 19.90m;
long grande = 5000000000L;
double d = 3.14;      // número com ponto é double por padrão
```

## Conversão: quando o valor "cabe" (e quando não)

Mesmo que o **número** caiba na variável, você nem sempre pode atribuí-lo diretamente: o C# olha o **tipo**, não o valor.

```cs
int meuInt = 10;
long meuLong = meuInt;        // OK: todo int cabe em long (conversão implícita)
int outro = meuLong;          // ERRO: long pode não caber em int
int ok = (int) meuLong;       // OK: você assume o risco com um cast (coerção)
```

::: regra Cast = "eu sei o que estou fazendo"
Um **cast** `(tipo)valor` diz ao compilador: *"converta, e a responsabilidade é minha"*. A conversão só é automática quando é **segura** (`int` → `long`, `float` → `double`).
:::

::: cuidado Estouro silencioso
Quando você converte um valor grande demais, o C# **ajusta** o resultado "dando a volta":

```cs
int grande = 300;
byte pequeno = (byte) grande;   // pequeno = 44  (300 - 256)
```

Nenhuma exceção, nenhum aviso: você só percebe depois, quando o resultado está estranho. Use `checked((byte) grande)` para lançar um erro em vez de dar a volta.
:::

### Coerção automática e outros atalhos

- Em `1.5 * 2`, o `2` vira `double` automaticamente.
- Ao concatenar com `+`, números viram texto: `"x vale " + 5` → `"x vale 5"`.
- Divisão de inteiros **descarta a parte decimal**: `7 / 2` é `3`; use `7 / 2.0` para `3.5`.
- Métodos exigem que os argumentos **combinem com os parâmetros** em tipo: passar `string` onde se espera `int` não compila.
- Operadores compostos: `+=`, `-=`, `*=`, `/=`, `%=` (`x *= 3` é `x = x * 3`).

::: exercicio Qual dessas linhas compila?
Considere `int i = 5; long l = 10; double d = 2.5; float f = 1.5f; byte b = 20;`

1. `long a = i;`
2. `int b2 = l;`
3. `float c = d;`
4. `double e = f;`
5. `byte g = i;`
6. `int h = (int) d;`

--- resposta
1. ✅ `int` → `long` (alarga).
2. ❌ `long` não cabe em `int` sem cast.
3. ❌ `double` → `float` perde precisão: exige `(float) d`.
4. ✅ `float` → `double`.
5. ❌ `int` → `byte` exige cast (mesmo que 5 caiba!).
6. ✅ vira `2` (o cast **trunca**, não arredonda).
:::

## Objetos também são variáveis (por referência)

Até aqui, as variáveis **guardavam o valor** dentro delas: `int x = 5` → a caixa `x` contém o `5`. Com **objetos**, é diferente: a variável guarda uma **referência** — como o controle remoto de uma TV, ou uma **etiqueta amarrada ao objeto**. O objeto em si vive no **heap**.

```cs
Dog fido = new Dog();     // 'fido' é uma referência; o Dog está no heap
```

::: analogia Referência = controle remoto
Dois controles remotos podem comandar **a mesma TV**. Se você mudar o canal por um, o outro "vê" o novo canal — não existem duas TVs. Assim funcionam referências: duas variáveis, **um** objeto.
:::

::: passos O ciclo de vida de uma referência
Vamos acompanhar este código no heap:

`Dog fido = new Dog(); fido.Name = "Fido";`
===
**Agora, uma segunda variável aponta para o mesmo cachorro:**

`Dog rex = fido;` ← **não** cria outro Dog. Só amarra uma segunda etiqueta no mesmo objeto.

::: html
<svg viewBox="0 0 560 170"><rect class="f-prim" x="10" y="20" width="110" height="40" rx="9"/><text x="65" y="46" text-anchor="middle" class="t-mono" font-size="14">fido</text><rect class="f-prim" x="10" y="100" width="110" height="40" rx="9"/><text x="65" y="126" text-anchor="middle" class="t-mono" font-size="14">rex</text><rect class="f-info" x="300" y="45" width="240" height="80" rx="12"/><text x="420" y="75" text-anchor="middle" font-weight="800" class="t-info">objeto Dog</text><text x="420" y="100" text-anchor="middle" class="t-mono" font-size="14">Name = "Fido"</text><path class="arrow" d="M122 40C200 40 230 70 296 80"/><path class="arrow" d="M122 120C200 120 230 100 296 95"/></svg>
:::
===
**Duas referências = DUAS formas de mudar o mesmo objeto:**

`rex.Name = "Rex";` → e `fido.Name` agora também é `"Rex"`!

::: html
<svg viewBox="0 0 560 170"><rect class="f-prim" x="10" y="20" width="110" height="40" rx="9"/><text x="65" y="46" text-anchor="middle" class="t-mono" font-size="14">fido</text><rect class="f-prim" x="10" y="100" width="110" height="40" rx="9"/><text x="65" y="126" text-anchor="middle" class="t-mono" font-size="14">rex</text><rect class="f-warn" x="300" y="45" width="240" height="80" rx="12"/><text x="420" y="75" text-anchor="middle" font-weight="800" class="t-warn">objeto Dog</text><text x="420" y="100" text-anchor="middle" class="t-mono" font-size="14">Name = "Rex"</text><path class="arrow" d="M122 40C200 40 230 70 296 80"/><path class="arrow" d="M122 120C200 120 230 100 296 95"/></svg>
:::
===
**Soltando uma referência:** `fido = null;`

`null` significa "não aponto para nada". O objeto ainda tem uma referência (`rex`), então continua vivo.

::: html
<svg viewBox="0 0 560 170"><rect class="f-plain" x="10" y="20" width="110" height="40" rx="9"/><text x="65" y="46" text-anchor="middle" class="t-mono" font-size="14">fido</text><text x="170" y="46" class="t-mono t-bad" font-size="14">null</text><rect class="f-prim" x="10" y="100" width="110" height="40" rx="9"/><text x="65" y="126" text-anchor="middle" class="t-mono" font-size="14">rex</text><rect class="f-warn" x="300" y="45" width="240" height="80" rx="12"/><text x="420" y="75" text-anchor="middle" font-weight="800" class="t-warn">objeto Dog</text><text x="420" y="100" text-anchor="middle" class="t-mono" font-size="14">Name = "Rex"</text><path class="arrow" d="M122 120C200 120 230 100 296 95"/></svg>
:::
===
**Última referência solta:** `rex = null;`

Nada mais aponta para o objeto. Ele não pode ser alcançado de lugar nenhum: o **coletor de lixo** (*garbage collector*) o recolherá e liberará a memória — **automaticamente**, em momento que o .NET escolhe.

::: html
<svg viewBox="0 0 560 170"><rect class="f-plain" x="10" y="20" width="110" height="40" rx="9"/><text x="65" y="46" text-anchor="middle" class="t-mono" font-size="14">fido = null</text><rect class="f-plain" x="10" y="100" width="110" height="40" rx="9"/><text x="65" y="126" text-anchor="middle" class="t-mono" font-size="14">rex = null</text><rect class="f-bad" stroke-dasharray="6 5" x="300" y="45" width="240" height="80" rx="12"/><text x="420" y="82" text-anchor="middle" font-weight="800" class="t-bad">objeto Dog</text><text x="420" y="104" text-anchor="middle" font-size="13" class="t-dim">🗑 será coletado como lixo</text></svg>
:::
:::

::: regra Sem referências → vira lixo
Se **nenhuma** referência aponta para um objeto, ele é elegível para coleta de lixo. Em C# você nunca libera memória à mão (como em C/C++): o coletor cuida disso.
:::

### Efeitos colaterais de múltiplas referências

Observe a diferença entre **copiar valor** e **copiar referência**:

```cs
int a = 10;
int b = a;      // b recebe uma CÓPIA do valor
b = 99;         // a continua 10

Dog d1 = new Dog() { Name = "Fido" };
Dog d2 = d1;    // d2 recebe a REFERÊNCIA — o mesmo objeto
d2.Name = "Rex";
// d1.Name agora é "Rex" também!
```

::: exercicio Troque duas referências sem perder nada
Você tem `Elefante lucinda` e `Elefante lloyd`, apontando para dois objetos diferentes. Escreva o código que **troca** as referências, de modo que `lucinda` aponte para o que era de `lloyd` e vice-versa. Dica: você precisa de uma terceira variável.

--- resposta
```cs
Elefante temp = lloyd;   // temp passa a apontar para o objeto de lloyd
lloyd = lucinda;         // lloyd aponta para o objeto de lucinda
lucinda = temp;          // lucinda aponta para o antigo objeto de lloyd
```
Sem `temp`, depois de `lloyd = lucinda` ninguém mais apontaria para o primeiro objeto e ele viraria lixo.
:::

## Matrizes (arrays)

Uma **matriz** (*array*) é uma variável que guarda **vários valores do mesmo tipo**, em posições numeradas a partir de **0**.

```cs
int[] numeros = new int[7];        // 7 posições, todas começam com 0
numeros[0] = 10;
numeros[1] = 20;
numeros[6] = 70;                   // a última posição é Length - 1
Console.WriteLine(numeros.Length); // 7

string[] frutas = { "maçã", "pera", "uva" };   // inicializador
foreach (string fruta in frutas)
{
    Console.WriteLine(fruta);
}
```

```saida
7
maçã
pera
uva
```

::: cuidado O índice começa em zero
Em `new int[7]` os índices válidos são **0 a 6**. Acessar `numeros[7]` lança `IndexOutOfRangeException`. É o famoso erro "off by one" (errar por um).
:::

### Matrizes de referências

Um array de objetos guarda **referências**, não os objetos. Ao criar `Dog[] dogs = new Dog[3]`, você tem 3 *vagas* — todas `null`. Cada `Dog` precisa ser criado com `new`:

```cs
Dog[] dogs = new Dog[3];       // três referências null
dogs[0] = new Dog() { Name = "Fido" };
dogs[1] = new Dog() { Name = "Lucky" };
// dogs[2] continua null — usar dogs[2].Name derrubaria o programa
```

::: figura Matriz de referências na memória
<svg viewBox="0 0 640 200" role="img" aria-label="Array dogs com duas referências e uma null">
  <text x="20" y="22" class="t-mono" font-size="14">Dog[] dogs</text>
  <g class="t-mono" font-size="13">
    <rect class="f-prim" x="20" y="40" width="110" height="46" rx="8"/><text x="75" y="68" text-anchor="middle">dogs[0]</text>
    <rect class="f-prim" x="20" y="96" width="110" height="46" rx="8"/><text x="75" y="124" text-anchor="middle">dogs[1]</text>
    <rect class="f-plain" x="20" y="152" width="110" height="40" rx="8"/><text x="75" y="178" text-anchor="middle">dogs[2]</text>
  </g>
  <text x="145" y="178" class="t-mono t-bad" font-size="13">null</text>
  <rect class="f-info" x="360" y="30" width="250" height="52" rx="10"/><text x="485" y="62" text-anchor="middle" class="t-mono" font-size="13">Dog  Name = "Fido"</text>
  <rect class="f-info" x="360" y="100" width="250" height="52" rx="10"/><text x="485" y="132" text-anchor="middle" class="t-mono" font-size="13">Dog  Name = "Lucky"</text>
  <path class="arrow" d="M132 63C240 63 300 56 356 56"/><path class="arrow" d="M132 119C240 119 300 126 356 126"/>
</svg>
:::

## Objetos usam referências para conversar

Quando um método recebe um objeto, ele recebe a **referência** — não uma cópia. Se o método alterar o objeto, quem chamou **enxerga** a mudança.

```cs
void Batizar(Dog d) { d.Name = "Totó"; }

Dog fido = new Dog() { Name = "Fido" };
Batizar(fido);
Console.WriteLine(fido.Name);   // Totó
```

### `null`: onde nenhum objeto jamais esteve

`null` é o valor de uma referência que **não aponta para nada**. Todo campo de referência começa `null` por padrão. Tentar usar um membro de uma referência nula causa **`NullReferenceException`** — um dos erros mais comuns da programação .NET.

```cs
Dog fantasma = null;
fantasma.Name = "X";   // 💥 NullReferenceException
```

::: dica Proteja-se do null
```cs
if (fantasma != null) { fantasma.Name = "X"; }
// ou, no C# moderno:
fantasma?.Latir();
```
:::

## Mini-projeto: a lanchonete de sanduíches

::: vila O Sanduíche Surpresa do Seu João
Na lanchonete de descontos do Seu João, o "sanduíche do dia" é montado **sorteando** um ingrediente de cada lista (carnes, queijos, molhos). Cada lista é uma **matriz de strings**, e o programa usa a classe `Random` para escolher um índice.
:::

```cs Sanduiche.cs
class Lanchonete
{
    string[] Carnes  = { "Rosbife", "Salame", "Peru", "Presunto", "Frango" };
    string[] Queijos = { "Prato", "Suíço", "Cheddar", "Mussarela" };
    string[] Molhos  = { "Maionese", "Mostarda", "Barbecue", "Ranch" };
    Random random = new Random();

    public string MontarSanduiche()
    {
        string carne  = Carnes[random.Next(Carnes.Length)];
        string queijo = Queijos[random.Next(Queijos.Length)];
        string molho  = Molhos[random.Next(Molhos.Length)];
        return carne + " com queijo " + queijo + " e " + molho;
    }
}
```

`random.Next(n)` devolve um inteiro de `0` até `n - 1` — exatamente os índices válidos de uma matriz de tamanho `n`. 

::: pense Por que `Carnes.Length` e não `5`?
Se amanhã o Seu João incluir uma sexta carne, `Carnes.Length` acompanha sozinho. Um `5` fixo no código viraria um bug escondido.
:::

::: resumo
- Cada variável tem **tipo**; escolha o menor que comporta sem sacrificar clareza (e `decimal` para dinheiro).
- Conversões **seguras** são automáticas; as arriscadas exigem **cast** e podem "dar a volta".
- Tipos de **valor** guardam o dado; variáveis de **objeto** guardam uma **referência**.
- Várias referências ao mesmo objeto: mudar por uma muda para todas.
- Sem referências, o objeto vira lixo e o **GC** o coleta.
- **Matrizes** guardam vários itens do mesmo tipo (índices de 0 a `Length - 1`).
- `null` = referência sem objeto; usá-la causa `NullReferenceException`.
:::

::: quiz
? Qual tipo é mais indicado para valores em dinheiro?
- `float`
- `double`
+ `decimal`
- `byte`
> `decimal` tem precisão exata para base 10; float/double acumulam erros de arredondamento.

? O que acontece em `int x = 5L;` (5L é long)?
- Funciona normalmente
+ Erro de compilação: long não converte implicitamente em int
- x passa a ser long
- x vale 0
> A conversão de `long` para `int` pode perder dados, então exige cast.

? Qual o valor de `(byte) 300`?
- 300
- 0
+ 44
- Lança exceção sempre
> Estouro: 300 − 256 = 44 (sem `checked`).

? Em `Dog b = a;` (a é um Dog existente), o que é copiado?
- O objeto inteiro
+ A referência: a e b apontam para o mesmo objeto
- Apenas o nome do Dog
- Nada; é um erro
> Atribuir referências não duplica o objeto.

? Quando um objeto pode ser coletado pelo garbage collector?
+ Quando nenhuma referência aponta mais para ele
- Quando você chama `new` de novo
- Logo depois de qualquer método terminar
- Nunca; é preciso liberar à mão
> O GC recolhe objetos inalcançáveis.

? Em `int[] v = new int[4];` quais são os índices válidos?
- 1 a 4
+ 0 a 3
- 0 a 4
- 1 a 3
> Índices começam em 0 e vão até Length − 1.

? O que é `NullReferenceException`?
+ Erro ao usar um membro de uma referência que vale null
- Erro de divisão por zero
- Erro de digitação do compilador
- Falha de conversão de tipos
> Ocorre ao acessar campo ou método de uma referência que não aponta para objeto.

? Ao passar um objeto para um método, o que o método recebe?
- Uma cópia completa do objeto
+ Uma cópia da referência (apontando para o mesmo objeto)
- Apenas os campos públicos
- Um ponteiro para a stack
> Mudanças no objeto dentro do método são visíveis a quem chamou.
:::

::: cartas
Tipo de dado :: Define qual espécie de valor uma variável guarda e o que se pode fazer com ele.
int / long / double / decimal :: Inteiro 32 bits / inteiro 64 bits / decimal de ponto flutuante / decimal exato (dinheiro).
Cast (coerção) :: `(tipo)valor` — força a conversão, assumindo o risco de perda.
Conversão implícita :: Conversão automática quando é segura (ex.: int → long).
Estouro (overflow) :: Resultado que "dá a volta" quando o valor não cabe no tipo.
Referência :: Variável que aponta para um objeto no heap (em vez de conter o objeto).
null :: Referência que não aponta para objeto algum.
Coleta de lixo (GC) :: Mecanismo do .NET que libera objetos sem referências.
Array (matriz) :: Coleção de tamanho fixo de itens do mesmo tipo, indexada a partir de 0.
Length :: Propriedade que informa o tamanho de um array.
NullReferenceException :: Exceção ao acessar membro de uma referência nula.
Random.Next(n) :: Devolve um inteiro aleatório de 0 até n − 1.
:::
