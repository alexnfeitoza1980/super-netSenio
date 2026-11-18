---
id: agil-cap18
order: 33
kind: capitulo
num: 18
group: C# · Parte 7 · Desenvolvimento ágil
title: Refatoração
subtitle: Fazer funcionar não basta — fazer direito. Pequenos passos, testes verdes e código que se explica sozinho.
minutes: 45
hue: 95
objectives:
- Definir **refatoração** e explicar as **três funções** de um módulo
- Aplicar **extrair método**, **renomear** e **eliminar duplicação** em passos pequenos
- Usar os **testes** como rede de segurança a cada transformação
- Fazer a **releitura final** e decidir quando parar
origem: livros/02-projeto-agil-csharp/content/32-cap18.md
---

::: ilustracao img/agil/cap18.svg | Refatorar é arrumar a casa sem mudar o que o programa faz.
:::

## Prestar atenção ao que se faz

Este capítulo é sobre **atenção** e **disciplina**: a diferença entre *fazer algo funcionar* e *fazer algo direito*. A definição clássica (de Martin Fowler) diz que **refatorar** é alterar a estrutura interna do software **sem mudar o comportamento externo**.

"Mas se já funciona, por que mexer?" Porque todo módulo de software tem **três funções**:

::: figura As três funções de um módulo
<svg viewBox="0 0 740 190" role="img" aria-label="Funcionar, permitir mudança e comunicar">
  <g text-anchor="middle">
    <rect class="f-good" x="10" y="20" width="230" height="150" rx="14"/><text x="125" y="52" font-weight="800" class="t-good" font-size="16">1 · Funcionar</text><text x="125" y="82" font-size="13" class="t-dim">fazer o que prometeu</text><text x="125" y="102" font-size="13" class="t-dim">quando executa — é a razão</text><text x="125" y="122" font-size="13" class="t-dim">de ele existir</text>
    <rect class="f-info" x="255" y="20" width="230" height="150" rx="14"/><text x="370" y="52" font-weight="800" class="t-info" font-size="16">2 · Permitir mudança</text><text x="370" y="82" font-size="13" class="t-dim">quase todo módulo vai mudar;</text><text x="370" y="102" font-size="13" class="t-dim">a mudança deve ser fácil</text>
    <rect class="f-warn" x="500" y="20" width="230" height="150" rx="14"/><text x="615" y="52" font-weight="800" class="t-warn" font-size="16">3 · Comunicar</text><text x="615" y="82" font-size="13" class="t-dim">um leitor novo deve</text><text x="615" y="102" font-size="13" class="t-dim">entendê-lo sem esforço</text><text x="615" y="122" font-size="13" class="t-dim">mental excessivo</text>
  </g>
</svg>
:::

Um módulo que funciona mas é **difícil de mudar** está quebrado. Um que funciona mas **não se comunica** também. Refatorar conserta as funções 2 e 3 sem arriscar a 1 — desde que haja **testes**.

::: regra Antes de investir em princípios e padrões, invista em código limpo
Todos os princípios e padrões dos próximos capítulos de nada servem se o código por dentro for uma bagunça. A limpeza diária é o que torna possível ampliar e modificar o sistema com pouco esforço.
:::

## Um exemplo passo a passo

Vamos refatorar um método que calcula o **frete** de um pedido. Funciona, mas é um "monolito" de variáveis curtas e comentários que tentam explicar o inexplicável. (Exemplo nosso, no mesmo espírito do capítulo.)

```cs Antes.cs
public class Util
{
    // calcula frete
    public static decimal F(decimal p, string r, bool e)
    {
        decimal x = 0;
        // peso até 1kg
        if (p <= 1) x = 10;
        else if (p <= 5) x = 10 + (p - 1) * 4; // acima de 1kg
        else x = 10 + 4 * 4 + (p - 5) * 3;     // acima de 5kg
        // região
        if (r == "N") x = x * 1.5m;
        if (r == "NE") x = x * 1.3m;
        if (r == "S") x = x * 1.1m;
        // expresso
        if (e) x = x * 1.4m;
        return x;
    }
}
```

### Passo 0 — Garanta uma rede de segurança

Antes de tocar numa linha, escreva testes que **fixam o comportamento atual**:

```cs
[TestFixture]
public class FreteTests
{
    [TestCase(0.5, "SE", false, 10)]
    [TestCase(3,   "SE", false, 18)]
    [TestCase(10,  "SE", false, 41)]
    [TestCase(3,   "N",  false, 27)]
    [TestCase(3,   "SE", true,  25.2)]
    public void CalculaFrete(decimal peso, string regiao, bool expresso, decimal esperado)
    {
        Assert.AreEqual(esperado, Util.F(peso, regiao, expresso));
    }
}
```

::: passos Pequenos passos, testes sempre verdes
**1) Renomear.** `F` → `CalcularFrete`; `p` → `pesoEmKg`; `r` → `regiao`; `e` → `expresso`; `x` → `frete`. Os comentários do tipo "calcula frete" passam a ser redundantes: **o nome já diz**. Rode os testes: verde.
===
**2) Extrair método** para cada bloco comentado. O comentário `// peso` vira o **nome** de um método: `FretePorPeso(pesoEmKg)`. Idem para `MultiplicadorDaRegiao(regiao)` e para o expresso. Rode os testes: verde.
===
**3) Trocar números mágicos por constantes nomeadas:** `10` → `TarifaBase`, `4` → `PorKgAte5`, `3` → `PorKgAcima5`, `1.4m` → `AdicionalExpresso`. Verde.
===
**4) Trocar a cadeia de `if` por uma tabela:** um `Dictionary<string, decimal>` das regiões. Cada região nova passa a ser **uma linha de dado**, não um `if` novo. Verde.
===
**5) Releitura final.** Leia a classe de cima a baixo, como uma prova matemática. Algum nome ainda confunde? Algum comentário mente? Ajuste e rode **todos** os testes uma última vez.
:::

Resultado:

```cs Depois.cs
public class CalculadoraDeFrete
{
    private const decimal TarifaBase = 10m;
    private const decimal PorKgAte5Kg = 4m;
    private const decimal PorKgAcima5Kg = 3m;
    private const decimal AdicionalExpresso = 1.4m;

    private static readonly Dictionary<string, decimal> MultiplicadorPorRegiao =
        new Dictionary<string, decimal> { { "N", 1.5m }, { "NE", 1.3m }, { "S", 1.1m } };

    public static decimal Calcular(decimal pesoEmKg, string regiao, bool expresso)
    {
        decimal frete = FretePorPeso(pesoEmKg) * MultiplicadorDaRegiao(regiao);
        return expresso ? frete * AdicionalExpresso : frete;
    }

    private static decimal FretePorPeso(decimal pesoEmKg)
    {
        if (pesoEmKg <= 1) return TarifaBase;
        if (pesoEmKg <= 5) return TarifaBase + (pesoEmKg - 1) * PorKgAte5Kg;
        return TarifaBase + 4 * PorKgAte5Kg + (pesoEmKg - 5) * PorKgAcima5Kg;
    }

    private static decimal MultiplicadorDaRegiao(string regiao)
    {
        return MultiplicadorPorRegiao.TryGetValue(regiao, out decimal m) ? m : 1m;
    }
}
```

Compare: o método público agora **se lê como uma frase** ("frete por peso, vezes o multiplicador da região, mais o adicional expresso"). Cada pedaço pode ser **testado e alterado isoladamente**.

::: dica Um nome que você não gosta é um sinal
Durante a releitura, se um nome parece errado, **pare** e pergunte *o que a função realmente faz?* Escrever um comentário explicativo muitas vezes revela que o **nome** (ou o raciocínio) estava errado — e é mais barato renomear do que explicar.
:::

## O catálogo básico de refatorações

| Refatoração | Quando usar | Como (em C#) |
|---|---|---|
| **Renomear** | Nome vago ou enganoso | `Ctrl+R, R` (renomeia todos os usos) |
| **Extrair método** | Trecho comentado ou longo demais | Selecione o trecho → `Ctrl+.` → *Extrair método* |
| **Extrair variável / constante** | Expressão complexa ou número mágico | `const decimal Taxa = 0.1m;` |
| **Introduzir parâmetro / objeto** | Lista longa de parâmetros | Agrupar em uma classe |
| **Mover método** | Método usa mais dados de outra classe | Mover para a classe que o merece |
| **Substituir condicional por polimorfismo** | `switch` repetido por tipo | Subclasses ou *Strategy* (cap. 30) |
| **Extrair classe** | Classe com muitos assuntos | Aplicar o SRP (cap. 20) |
| **Eliminar duplicação** | Trechos iguais/semelhantes | Método comum ou *Template Method* |

::: ide Ferramentas ajudam
O Visual Studio traz as refatorações básicas (*Ações rápidas* `Ctrl+.`); **ReSharper** e **Rider** (JetBrains) oferecem muitas mais. Elas fazem a transformação **mecânica** com segurança — mas *quando* e *por quê* ainda é com você.
:::

## Como refatorar sem medo

1. **Tenha testes verdes primeiro.** Sem rede, é reescrita arriscada, não refatoração.
2. **Um passo de cada vez**, pequeno o bastante para que um erro seja óbvio.
3. **Rode os testes a cada passo.** Se falhar, **desfaça** o último passo (e entenda por quê).
4. **Não misture** refatorar com **mudar comportamento**. Primeiro limpe, depois adicione a funcionalidade nova (ou vice-versa) — nunca as duas coisas no mesmo passo.
5. Se uma mudança lhe deixa **inseguro**, escreva **mais um teste** que prove que está certa (por exemplo, um teste exaustivo).
6. Mantenha a prática **contínua**: a cada meia hora, não "no fim do projeto".

::: pense "Mas vai ficar mais lento!"
Quebrar uma função em várias pequenas **raramente** pesa. Na maioria dos casos, a clareza vale mais que alguns nanossegundos — e se um dia o desempenho for problema, você **mede** (com profiler) e otimiza o ponto exato. Presuma que o custo é desprezível até que se prove o contrário.
:::

::: analogia A oficina
Um marceneiro que **não limpa a bancada** termina o móvel de hoje mais cedo. Amanhã, perde meia hora procurando a ferramenta, desengatando a cola endurecida, desviando de aparas. Cada dia fica pior — e ele passa a **fugir** da limpeza. Quem arruma enquanto trabalha mantém um ritmo constante e **sempre** pode começar algo novo. Refatorar é arrumar a bancada **todo dia**.
:::

::: exercicio Refatore este método
```cs
public string Rel(List<Pedido> l)
{
    string s = "";
    foreach (var p in l)
    {
        if (p.Status == 1) s += p.Id + ": pendente\n";
        else if (p.Status == 2) s += p.Id + ": pago\n";
        else if (p.Status == 3) s += p.Id + ": enviado\n";
    }
    return s;
}
```
Liste 4 refatorações em ordem.

--- resposta
1. **Renomear** `Rel` → `GerarRelatorioDeStatus`; `l` → `pedidos`; `s` → `relatorio`; `p` → `pedido`.
2. **Trocar números mágicos por `enum`:** `enum StatusPedido { Pendente = 1, Pago = 2, Enviado = 3 }` e `pedido.Status` com esse tipo.
3. **Extrair método** `DescreverStatus(StatusPedido)` com um `switch` (ou dicionário) — a lógica de texto sai do laço.
4. **Usar `StringBuilder`** (ou `string.Join` + LINQ) no lugar de concatenar em laço: `pedidos.Select(p => p.Id + ": " + Descrever(p.Status))`.
Entre cada passo: rodar os testes.
:::

::: resumo
- **Refatorar** = melhorar a estrutura interna **sem** mudar o comportamento externo.
- Todo módulo deve **funcionar**, **permitir mudança** e **comunicar**.
- Passos pequenos + **testes** a cada um: renomear, extrair método, nomear constantes, eliminar duplicação.
- Faça a **releitura final** do código inteiro, como quem lê uma demonstração.
- Refatore **continuamente**: limpeza diária custa muito menos que uma "grande faxina".
- **Código limpo vem antes** de princípios e padrões.
:::

::: quiz
? Qual é a definição de refatoração?
+ Melhorar a estrutura interna do código sem alterar seu comportamento externo
- Reescrever o programa em outra linguagem
- Adicionar novas funcionalidades rapidamente
- Remover todos os comentários
> Refatorar preserva o comportamento — por isso os testes são essenciais.

? Quais as três funções de um módulo de software?
+ Funcionar, permitir mudança e comunicar-se com o leitor
- Compilar, executar e ser publicado
- Ser rápido, pequeno e barato
- Ser testado, documentado e vendido
> Um módulo difícil de mudar ou de entender precisa ser consertado, mesmo funcionando.

? O que fazer se um teste falha logo após um passo de refatoração?
+ Desfazer o último passo e investigar
- Alterar o teste para passar
- Ignorar e seguir adiante
- Apagar o teste
> Passos pequenos permitem localizar rapidamente o que quebrou.

? Qual das opções é uma boa substituição para um número mágico?
+ Uma constante com nome (`const decimal TarifaBase = 10m;`)
- Um comentário ao lado
- Duplicar o número onde precisar
- Uma variável global qualquer
> Nomes expressam a intenção.

? Por que fazer uma "releitura final" do código?
+ Para verificar se o programa inteiro continua legível como um todo, não só em fragmentos
- Para contar as linhas
- Para satisfazer o compilador
- Para eliminar os testes
> Refatorar por partes pode deixar incoerências globais.

? Quando refatorar, segundo a prática ágil?
+ Continuamente, a cada meia hora ou hora, com testes rodando
- Só no fim do projeto
- Só quando o cliente reclamar
- Apenas antes da entrega
> A limpeza contínua evita a bagunça acumulada.

? Por que não misturar refatoração com mudança de comportamento no mesmo passo?
+ Porque se algo quebrar, você não saberá qual das duas coisas causou
- Porque o compilador proíbe
- Porque refatoração é mais lenta
- Porque os testes não funcionam
> Separar os passos mantém a rede de segurança confiável.
:::

::: cartas
Refatoração :: Alterar a estrutura interna do código sem mudar o comportamento externo.
Extrair método :: Transformar um trecho de código em um método com nome expressivo.
Renomear :: Dar nomes que revelem a intenção; a IDE atualiza todos os usos.
Número mágico :: Valor literal sem nome no código; substitua por constante nomeada.
Código limpo :: Código fácil de ler e de alterar.
Releitura final :: Ler o módulo inteiro, de ponta a ponta, após refatorar partes.
Duplicação :: Mesma lógica em vários lugares; deve ser eliminada.
Rede de segurança :: Conjunto de testes que permite alterar o código com confiança.
:::
