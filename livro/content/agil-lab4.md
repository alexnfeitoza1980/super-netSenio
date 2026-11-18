---
id: agil-lab4
order: 34
kind: lab
num: 4
group: C# · Parte 7 · Desenvolvimento ágil
title: Um episódio de programação — o placar do boliche
subtitle: Uma sessão de TDD de ponta a ponta: o design aparece aos poucos, classes "óbvias" desaparecem e o código fica limpo graças aos testes.
minutes: 70
hue: 95
badge: L4
objectives:
- Acompanhar uma **sessão de TDD** completa, com erros, dúvidas e correções
- Ver como o **design emerge** dos testes em vez de ser decidido de antemão
- Perceber quando uma classe "óbvia" **não precisa existir**
- Praticar o ciclo **vermelho → verde → refatorar** num problema com regras de verdade
origem: livros/02-projeto-agil-csharp/content/33-lab4.md
---

::: vila Dois programadores, um teclado
No livro, dois desenvolvedores se sentam lado a lado num quarto de hotel e constroem, em dupla, um aplicativo que **calcula o placar de um jogo de boliche**. O processo é desordenado, como todo processo humano: eles erram, discordam, mudam de ideia — e a ordem que resulta da bagunça é espantosa. Aqui reconstruímos uma sessão equivalente, com **nosso próprio código**, para você fazer junto.
:::

::: ilustracao img/agil/lab4.svg | O episódio do boliche: um placar simples que cresce com testes e refatoração.
:::

## As regras do boliche (resumo)

Um jogo tem **10 quadros** (*frames*). Em cada quadro o jogador tem até **2 arremessos** para derrubar **10 pinos**.

| Situação | Como se chama | Pontuação do quadro |
|---|---|---|
| Derrubou **menos de 10** pinos nos 2 arremessos | quadro aberto | soma dos pinos derrubados |
| Derrubou **10** pinos em **2** arremessos | **spare** (meia) | 10 + **a próxima bola** (bônus) |
| Derrubou **10 pinos no 1º arremesso** | **strike** (pleno) | 10 + **as próximas duas bolas** (bônus); o quadro tem 1 arremesso só |
| **10º quadro** com spare ou strike | | ganha 1 ou 2 arremessos extras, que só contam como bônus |

O **placar de cada quadro é cumulativo**: soma o que veio antes. Um jogo perfeito (12 strikes seguidos) vale **300**.

::: figura Um cartão de pontuação
<svg viewBox="0 0 760 130" role="img" aria-label="Quatro primeiros quadros de um cartão de boliche">
  <g text-anchor="middle" font-size="13">
    <rect class="f-plain" x="10" y="15" width="170" height="95" rx="6"/><path class="line" d="M10 45H180M120 15V45"/><text x="65" y="35">3</text><text x="150" y="35">/</text><text x="95" y="85" font-weight="800" font-size="17">13</text><text x="95" y="105" font-size="11" class="t-dim">spare + 3</text>
    <rect class="f-plain" x="190" y="15" width="170" height="95" rx="6"/><path class="line" d="M190 45H360M300 15V45"/><text x="245" y="35">3</text><text x="330" y="35">4</text><text x="275" y="85" font-weight="800" font-size="17">20</text><text x="275" y="105" font-size="11" class="t-dim">13 + 7</text>
    <rect class="f-plain" x="370" y="15" width="170" height="95" rx="6"/><path class="line" d="M370 45H540M480 15V45"/><text x="510" y="35" font-weight="800">X</text><text x="455" y="85" font-weight="800" font-size="17">40</text><text x="455" y="105" font-size="11" class="t-dim">20 + 10 + 5 + 5</text>
    <rect class="f-plain" x="550" y="15" width="170" height="95" rx="6"/><path class="line" d="M550 45H720M660 15V45"/><text x="605" y="35">5</text><text x="690" y="35">/</text><text x="635" y="85" font-weight="800" font-size="17">…</text><text x="635" y="105" font-size="11" class="t-dim">depende da próxima bola</text>
  </g>
</svg>
:::

## O primeiro impulso (e por que adiá-lo)

A reação natural a um problema assim é **desenhar classes**: um `Jogo` tem 10 `Quadro`s; cada `Quadro` tem 1 a 3 `Arremesso`s.

::: figura O "óbvio" diagrama de objetos
<svg viewBox="0 0 600 130" role="img" aria-label="Jogo tem 10 Quadros; Quadro tem de 1 a 3 Arremessos">
  <g text-anchor="middle"><rect class="f-prim" x="20" y="35" width="130" height="50" rx="10"/><text x="85" y="66" font-weight="800">Jogo</text>
  <rect class="f-info" x="235" y="35" width="130" height="50" rx="10"/><text x="300" y="66" font-weight="800">Quadro</text>
  <rect class="f-good" x="450" y="35" width="130" height="50" rx="10"/><text x="515" y="66" font-weight="800">Arremesso</text></g>
  <path class="line" d="M152 60H233"/><path class="line" d="M367 60H448"/><text x="192" y="52" font-size="12">10</text><text x="408" y="52" font-size="12">1..3</text>
</svg>
:::

Mas uma pergunta simples muda tudo: **qual é o comportamento de um `Arremesso`?** A resposta honesta é *"guardar quantos pinos caíram"* — ou seja, **nenhum**. Uma classe sem comportamento é só um `int` com fantasia. Em vez de criá-la "porque o diagrama manda", deixamos que **os testes peçam** as classes de que realmente precisamos.

::: regra Projete a partir do comportamento
Comece por quem **faz** algo. Classes que só guardam dados talvez nem precisem existir — ou só apareçam depois, quando um teste as exigir. Quando algo parecer "sofisticado demais", faça o **mais simples** e só complique **quando o código reclamar**.
:::

## A lista de testes

Antes de qualquer código, listamos os exemplos que queremos ver funcionando — e a ordem vai do **mais simples ao mais difícil**:

1. Jogo sem pontos: 20 arremessos de zero → 0.
2. Só quadros abertos (ex.: 5+4, 7+2) → soma.
3. **Um spare** seguido de um arremesso → bônus.
4. **Um strike** seguido de dois arremessos → bônus.
5. Spare/strike **no último quadro**.
6. **Jogo perfeito** (12 strikes) → 300.
7. Todos spare de 5 (5/ × 10 + 5 de bônus) → 150.

## A sessão

::: passos Ciclo 1 — o primeiro teste (vermelho → verde)
Começamos pelo teste mais simples, escrito **como queremos usar** a classe:

```cs JogoTests.cs
[TestFixture]
public class JogoTests
{
    private Jogo jogo;

    [SetUp]
    public void Preparar() { jogo = new Jogo(); }

    private void AdicionarVarios(int vezes, int pinos)
    {
        for (int i = 0; i < vezes; i++) jogo.Adicionar(pinos);
    }

    [Test]
    public void JogoDeZeros_ValeZero()
    {
        AdicionarVarios(20, 0);
        Assert.AreEqual(0, jogo.PlacarDoQuadro(10));
    }
}
```
Nem compila (**vermelho**). O mínimo para compilar e passar:

```cs
public class Jogo
{
    public void Adicionar(int pinos) { }
    public int PlacarDoQuadro(int quadro) { return 0; }
}
```
**Verde.** (Sim, é "trapaça": o teste só exige um 0. Os próximos testes obrigarão a ser honesto.)
===
**Ciclo 2 — quadros abertos.**

```cs
[Test]
public void QuadrosAbertos_SomamOsPinos()
{
    jogo.Adicionar(5); jogo.Adicionar(4);   // quadro 1: 9
    jogo.Adicionar(7); jogo.Adicionar(2);   // quadro 2: 9 + 9 = 18
    Assert.AreEqual(9, jogo.PlacarDoQuadro(1));
    Assert.AreEqual(18, jogo.PlacarDoQuadro(2));
}
```
**Vermelho.** Agora precisamos guardar os arremessos. O jeito mais simples: **um array de inteiros** (21 posições: o máximo possível de arremessos). Nada de `Quadro`, nada de `Arremesso`.

```cs
public class Jogo
{
    private readonly int[] arremessos = new int[21];
    private int atual;

    public void Adicionar(int pinos) { arremessos[atual++] = pinos; }

    public int PlacarDoQuadro(int quadro)
    {
        int bola = 0, placar = 0;
        for (int q = 1; q <= quadro; q++)
        {
            placar += arremessos[bola] + arremessos[bola + 1];
            bola += 2;
        }
        return placar;
    }
}
```
**Verde.**
===
**Ciclo 3 — o spare.** O teste:

```cs
[Test]
public void Spare_GanhaOProximoArremessoComoBonus()
{
    jogo.Adicionar(3); jogo.Adicionar(7);   // spare
    jogo.Adicionar(3);                      // bônus
    AdicionarVarios(17, 0);
    Assert.AreEqual(13, jogo.PlacarDoQuadro(1));
    Assert.AreEqual(16, jogo.PlacarDoQuadro(2));
}
```
**Vermelho** (dá 10 e 13). Ajustamos o laço:

```cs
int primeira = arremessos[bola], segunda = arremessos[bola + 1];
if (primeira + segunda == 10)               // spare
    placar += 10 + arremessos[bola + 2];
else
    placar += primeira + segunda;
bola += 2;
```
**Verde.** E os testes anteriores continuam verdes — a rede de segurança funcionando.
===
**Ciclo 4 — o strike.** O strike tem só **um** arremesso por quadro, então `bola` avança **1**:

```cs
[Test]
public void Strike_GanhaOsDoisProximosArremessosComoBonus()
{
    jogo.Adicionar(10);                      // strike
    jogo.Adicionar(3); jogo.Adicionar(6);    // bônus
    AdicionarVarios(16, 0);
    Assert.AreEqual(19, jogo.PlacarDoQuadro(1));
    Assert.AreEqual(28, jogo.PlacarDoQuadro(2));
}
```
```cs
if (arremessos[bola] == 10)                  // strike
{
    placar += 10 + arremessos[bola + 1] + arremessos[bola + 2];
    bola += 1;
}
else if (arremessos[bola] + arremessos[bola + 1] == 10) { /* spare */ ...; bola += 2; }
else { /* aberto */ ...; bola += 2; }
```
**Verde.**
===
**Ciclo 5 — o jogo perfeito.** 12 strikes → 300.

```cs
[Test]
public void JogoPerfeito_Vale300()
{
    AdicionarVarios(12, 10);
    Assert.AreEqual(300, jogo.PlacarDoQuadro(10));
}
```
Passa **de primeira**: o 11º e o 12º strikes são lidos apenas como bônus do 10º quadro, e o array de 21 posições comporta tudo. Ótimo sinal de que o algoritmo está certo — mas para **desconfiar**, acrescentamos o teste do **último quadro com spare** (bônus na posição 21):

```cs
[Test]
public void SpareNoUltimoQuadro_ContaUmArremessoExtra()
{
    AdicionarVarios(18, 0);
    jogo.Adicionar(2); jogo.Adicionar(8);    // spare no 10º
    jogo.Adicionar(10);                      // extra (só bônus)
    Assert.AreEqual(20, jogo.PlacarDoQuadro(10));
}
```
Também passa. E **todos de 5** (spare em todos os quadros, mais um extra):

```cs
[Test]
public void TodosOsSparesDeCinco_Valem150()
{
    AdicionarVarios(21, 5);
    Assert.AreEqual(150, jogo.PlacarDoQuadro(10));
}
```
**Verde.** A lógica de pontuação está pronta — **sem ter criado `Quadro` nem `Arremesso`**.
:::

::: pense Cadê a classe Quadro?
Se tivéssemos desenhado classes no começo, estaríamos montando uma lista encadeada de quadros que olham para a frente e para trás… e cada um precisaria do próximo para calcular o bônus. Os testes mostraram que **uma lista de inteiros e um laço** resolviam tudo. Esse é o poder do TDD: ele **testa também o seu design**, impedindo que você construa o que não precisa (YAGNI).
:::

## Refatorando: o código está feio

Funciona, mas o método `PlacarDoQuadro` cresceu: um laço com três ramos, índices soltos e `bola + 1`, `bola + 2` por todo lado. Hora do terceiro passo do TDD. Com os testes verdes, **extraímos métodos** que revelam a intenção:

```cs Jogo.cs (refatorado)
public class Jogo
{
    private readonly int[] arremessos = new int[21];
    private int atual;

    public void Adicionar(int pinos) { arremessos[atual++] = pinos; }

    public int PlacarDoQuadro(int quadro)
    {
        int bola = 0, placar = 0;
        for (int q = 1; q <= quadro; q++)
        {
            if (Strike(bola))
            {
                placar += 10 + BonusDoStrike(bola);
                bola += 1;
            }
            else if (Spare(bola))
            {
                placar += 10 + BonusDoSpare(bola);
                bola += 2;
            }
            else
            {
                placar += PinosDoQuadroAberto(bola);
                bola += 2;
            }
        }
        return placar;
    }

    private bool Strike(int bola) { return arremessos[bola] == 10; }
    private bool Spare(int bola) { return arremessos[bola] + arremessos[bola + 1] == 10; }
    private int BonusDoStrike(int bola) { return arremessos[bola + 1] + arremessos[bola + 2]; }
    private int BonusDoSpare(int bola) { return arremessos[bola + 2]; }
    private int PinosDoQuadroAberto(int bola) { return arremessos[bola] + arremessos[bola + 1]; }
}
```

Agora o método principal **se lê como as regras do boliche**: *"se é strike… se é spare… senão…"*. Cada pergunta tem nome.

### Segundo olhar: o SRP

`Jogo` faz **duas coisas**: **guarda os arremessos** (e saberá o quadro atual) **e** **calcula pontos**. São duas razões para mudar: se mudarem as regras de pontuação, mexemos em `Jogo`; se mudar a forma de registrar os arremessos, mexemos em `Jogo`. É a violação do **Princípio da Responsabilidade Única** (capítulo 20). A saída é separar:

```cs
public class Jogo
{
    private readonly int[] arremessos = new int[21];
    private int atual;
    private readonly Pontuador pontuador = new Pontuador();

    public void Adicionar(int pinos) { arremessos[atual++] = pinos; }
    public int PlacarDoQuadro(int quadro) { return pontuador.PlacarAte(quadro, arremessos); }
}

public class Pontuador
{
    public int PlacarAte(int quadro, int[] arremessos) { /* o laço de antes */ }
}
```

::: ide Refatore com a rede de segurança
Selecione o trecho → **Ctrl+.** → *Extrair método* / *Mover para nova classe*. Depois de cada transformação, rode **todos os testes**. Se algum ficar vermelho, desfaça (**Ctrl+Z**) — o erro está no último passo.
:::

## E o "placar atual"?

Num jogo em andamento, qual é **o placar agora**? A discussão no livro mostra um vaivém instrutivo: tentaram um `QuadroAtual` que apontasse "o quadro onde se está arremessando", mas o fim do jogo gerava casos estranhos (quadro 11?). A saída foi **pensar de outro jeito**: o placar do jogo é o **do último quadro completo** — um quadro está *completo* quando seu placar já pode ser calculado (spare precisa de +1 bola; strike de +2).

```cs
// uma ideia simples: contar quadros completos a partir dos arremessos já feitos
public int QuadrosCompletos { get { /* percorre como o Pontuador */ } }
public int Placar { get { return PlacarDoQuadro(QuadrosCompletos); } }
```

::: cuidado O "quadro atual" é traiçoeiro
Duas pessoas inteligentes quase se embananaram com o fim do jogo (e com **programação por coincidência**: somar e subtrair 1 até o teste passar!). Quando você se pega "ajustando números até dar certo", **pare** e redefina o conceito. Aqui: *quadro completo*, em vez de *quadro atual*.
:::

## O que o episódio ensina

- **O design emerge dos testes.** Quadro e Arremesso, "óbvios", nunca foram necessários.
- **Faça a coisa mais simples** que passe — arrays e laços — e complique só quando o código *exigir*.
- **Teste de aceitação** (um cartão real) valida tudo no fim: um jogo irregular, com erros e acertos, tem de dar o placar do papel.
- **Refatorar** só é seguro com **testes verdes**; renomear e extrair métodos dão ao código a voz do domínio.
- **Erros acontecem** (de código, de lógica, de design, de requisito). O processo os **expõe cedo** e os corrige em minutos.
- Programar **em par** cria um diálogo: perguntar *"qual é o comportamento disto?"* evita classes inúteis.

::: exercicio Estenda o kata
1. Adicione um teste para **dois spares seguidos** e um **strike seguido de spare**.
2. Faça `Adicionar` **lançar `ArgumentOutOfRangeException`** se `pinos < 0` ou `> 10`. (Escreva o teste primeiro!)
3. Crie `QuadrosCompletos` e a propriedade `Placar` de modo que `Placar` funcione **durante** o jogo.

--- resposta
1. Dois spares: `Adicionar(5);Adicionar(5);Adicionar(5);Adicionar(5);Adicionar(2);` → quadro 1 = 15, quadro 2 = 15 + 12 = 27. Strike + spare: `Adicionar(10);Adicionar(4);Adicionar(6);Adicionar(3);` → quadro 1 = 10 + 4 + 6 = 20; quadro 2 = 20 + 10 + 3 = 33.
2. Teste: `Assert.Throws<ArgumentOutOfRangeException>(() => jogo.Adicionar(11));` e, na classe: `if (pinos < 0 || pinos > 10) throw new ArgumentOutOfRangeException(nameof(pinos));`. (Lembre do YAGNI: só valide se alguém realmente pode chamar com valores ruins.)
3. `QuadrosCompletos`: percorra como o pontuador; conte um quadro como completo quando houver arremessos suficientes para seu bônus: strike → precisa de bola+2 já lançada; spare → bola+2; aberto → bola+1. `Placar` = `PlacarDoQuadro(QuadrosCompletos)`.
:::

::: resumo
- **TDD** = lista de testes simples → vermelho → verde → refatorar, em ciclos curtos.
- Classes que **não têm comportamento** talvez não precisem existir; deixe o código pedir.
- O mínimo que passa vence; **complique só quando um teste exigir**.
- Depois de verde, **refatore**: extraia métodos nomeados com o vocabulário do domínio.
- Quando uma classe faz duas coisas, **separe** (SRP) — com os testes protegendo.
- Evite **programação por coincidência**: se ajustar números "até funcionar", repense o conceito.
:::

::: quiz
? Por que a classe `Arremesso` acabou não sendo criada?
+ Porque não tinha comportamento: era apenas um número, e os testes nunca exigiram uma classe
- Porque C# não permite classes pequenas
- Porque o NUnit proíbe
- Por esquecimento
> Classes sem comportamento só guardam dados; um `int` bastou.

? No boliche, quanto vale um quadro com strike?
+ 10 mais os pinos dos dois arremessos seguintes
- 10 mais o próximo arremesso
- Só 10
- A soma dos dois últimos quadros
> Strike: bônus de duas bolas; spare: bônus de uma bola.

? Qual é o placar de um jogo perfeito (12 strikes)?
+ 300
- 240
- 360
- 200
> Cada um dos 10 quadros vale 30 (10 + 10 + 10).

? Qual foi a abordagem mais simples para guardar os arremessos?
+ Um array de inteiros de 21 posições e um índice
- Uma lista encadeada de objetos Quadro
- Um banco de dados
- Uma árvore binária
> A solução mais simples que faz os testes passarem.

? O que significa "programação por coincidência"?
+ Alterar valores (somar/subtrair 1) até o teste passar, sem entender por quê
- Programar em pares
- Escrever testes depois do código
- Usar números aleatórios
> Deve-se parar e entender o problema, não acertar por tentativa.

? Que princípio a classe `Jogo` violava ao guardar arremessos E calcular pontos?
+ Responsabilidade Única (SRP)
- Substituição de Liskov
- Inversão de dependência
- Segregação de interface
> Duas razões para mudar → separar em `Jogo` e `Pontuador`.

? Por que o teste de 12 strikes passar "de primeira" ainda pede mais testes?
+ Porque passar de primeira não prova que as bordas (último quadro, spares) estão certas
- Porque o NUnit exige três testes
- Porque 300 é um valor especial
- Não pede
> Casos de borda são onde os bugs se escondem.
:::

::: cartas
Kata :: Exercício de programação repetido para praticar uma técnica (ex.: placar de boliche).
Spare :: Derrubar os 10 pinos em 2 arremessos; bônus = a próxima bola.
Strike :: Derrubar os 10 pinos no 1º arremesso; bônus = as duas próximas bolas.
Lista de testes :: Exemplos ordenados do mais simples ao mais difícil, para guiar o TDD.
Design emergente :: Estrutura que surge a partir dos testes, em vez de planejada de antemão.
Programação por coincidência :: Ajustar valores até funcionar, sem entender; deve ser evitada.
Quadro completo :: Quadro cujo placar já pode ser calculado (bônus já lançados).
:::
