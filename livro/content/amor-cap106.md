---
id: amor-cap106
order: 138
kind: capitulo
num: 106
group: C# & Mentalidade · Parte 17 · Você é aquilo que ama, em C#
title: A bússola e o mapa
subtitle: O desejo como uma bússola que aponta para uma visão de vida boa, e o risco de navegar bem com o mapa errado. Em C#, é o capítulo dos vetores: structs imutáveis, operadores sobrecarregados e um pouco de trigonometria útil.
minutes: 65
hue: 335
objectives:
- Entender a imagem do **coração como bússola** e a pergunta "**para que estamos aqui?**" (o *telos*)
- Distinguir **bússola** (direção) de **mapa** (a imagem que temos do destino) e por que os dois importam
- Escrever um **`readonly record struct`** com **operadores sobrecarregados** (`+`, `*`)
- Usar `Math` (norma, cosseno, ângulo), `Aggregate` com valor inicial e `double?` para "não há o que medir"
origem: livros/08-voce-e-aquilo-que-ama-csharp/content/126-cap106.md
---

::: vila A última dessas "iniciativas"
No planejamento do trimestre, o time listou doze iniciativas. Onze eram funcionalidades novas, todas com um bom motivo. A décima segunda era "reduzir a dívida técnica do módulo de pagamentos". Bia disse: "essa a gente faz depois, quando sobrar tempo". Ninguém discordou, e ninguém decidiu nada. Três meses depois, o sistema estava cheio de coisas novas e cada entrega ficava mais lenta. Caio comentou: "a gente trabalhou muito... mas em que direção?".
:::

::: ilustracao img/amor/cap106.svg | A bússola indica o rumo; as pequenas escolhas somam um desvio. Sem olhar a soma, cada passo parece certo.
:::

## O que o livro diz

Smith descreve o **coração**, no sentido bíblico e antigo, como o **centro dos nossos anseios**: uma espécie de **bússola** (ou de aparelho que ao mesmo tempo localiza e empurra) que nos orienta para uma **visão de vida boa**. Essa visão nem sempre é pensada: costuma ser uma imagem vaga e atraente de "onde a felicidade se encontra" (ele lembra a canção da menina Cosette, em *Os miseráveis*, com seu "castelo nas nuvens"). É por ela que, em geral, **vivemos**: não por uma lista de ideias, e sim porque algo **nos atrai**.

Uma palavra importante é **telos**, "fim" ou "meta" em grego: aquilo **para o qual** algo é orientado. Para o autor, seres humanos são **teleológicos**: estamos sempre indo em direção a algo. A pergunta, portanto, não é **se** queremos alguma versão de "vida boa", e sim **qual**. Ele retoma uma imagem de Agostinho, filósofo do século IV: um corpo é movido pelo seu **"peso"**, e o peso de uma pessoa é o seu **amor**: para onde quer que ela vá, é o que a leva.

Duas histórias de navegação ilustram o risco:

- **Uma bússola mal calibrada.** Dois navios colidiram na neblina, na costa da Virgínia (EUA), e 41 marinheiros morreram. Nas audiências, soube-se que o capitão de um deles navegava com uma bússola que desviava até **dois graus** do padrão e que **nunca fora ajustada** em um ano de comando. Ele a considerava "boa o bastante", e era comum que outros também a considerassem. O autor tira a lição: **se o coração é uma bússola, precisa ser calibrado com regularidade**.
- **Um mapa errado.** A expedição do navio *Jeannette* ao Ártico, no século 19, baseou-se em mapas que sugeriam uma passagem de clima ameno até um vasto oceano aberto no topo do mundo. A bússola funcionava; **o mapa descrevia um mundo que não existia**, e a tripulação teve de abandonar as ideias que a guiavam. Para o autor, nossos amores se perdem não só por falha de bússola, mas por **imagens rivais de "vida boa"** (mapas errados) que absorvemos sem notar, porque agem sobre a imaginação, e não sobre o intelecto.

::: cuidado Fé e metáfora
Para Smith, o "norte verdadeiro" dessa bússola é uma convicção religiosa dele. Aqui, usamos a **estrutura** da imagem, que vale em qualquer rumo escolhido: toda equipe e toda pessoa tem um *telos* implícito, e as pequenas escolhas **somam** um desvio ou um alinhamento com ele.
:::

::: analogia O piloto automático
Um navio com o piloto automático ligado em 2° de erro **parece** estar no rumo durante horas. O erro só aparece quando se olha a **soma** do percurso, e não cada manobra. É por isso que equipes precisam, de tempos em tempos, **parar e comparar** o rumo que as escolhas somaram com o rumo que se queria.
:::

## Da vida para o código

| No livro | Em C# |
|---|---|
| Uma direção: "para onde estou indo?" | `Vetor` (um `readonly record struct` com X e Y) |
| O telos (o norte) | `Bussola.Telos` |
| Cada escolha empurra um pouco | `Iniciativa` com um `Efeito` (vetor) |
| A soma de tudo que se fez | `Deriva` (`Aggregate` somando vetores) |
| "Estou no rumo?" | `DesvioEmGraus`, com o cosseno do ângulo |
| Recalibrar | `Recalibrar` devolve **outra** bússola |
| "Sem movimento, sem desvio" | `double?` (`null` quando não há o que medir) |

::: figura Cada iniciativa empurra um pouco; a soma é o rumo real
<svg viewBox="0 0 740 250" role="img" aria-label="Setas de iniciativas somando um vetor de deriva que se afasta do rumo desejado; ao somar iniciativas de cuidado, a deriva se aproxima do rumo">
  <line x1="60" y1="200" x2="330" y2="60" stroke="var(--primary)" stroke-width="3" stroke-dasharray="7 6"/>
  <text x="330" y="50" text-anchor="middle" font-size="13" font-weight="800">rumo (telos): valor E saúde</text>
  <path class="arrow" d="M60 200L160 200"/><path class="arrow" d="M160 200L260 200"/><path class="arrow" d="M260 200L360 200"/>
  <text x="210" y="226" text-anchor="middle" font-size="12" class="t-dim">só funcionalidades (→)</text>
  <path class="arrow" d="M60 200L360 200" stroke="#d8454f" opacity=".6"/>
  <text x="520" y="86" font-weight="800" font-size="14">Duas situações</text>
  <rect x="440" y="98" width="260" height="46" rx="10" class="f-bad"/><text x="570" y="118" text-anchor="middle" font-size="13">Só funcionalidades: ~62° de desvio</text><text x="570" y="136" text-anchor="middle" font-size="11" class="t-dim">cada passo parecia certo</text>
  <rect x="440" y="156" width="260" height="46" rx="10" class="f-good"/><text x="570" y="176" text-anchor="middle" font-size="13">Com cuidado da base: ~22°</text><text x="570" y="194" text-anchor="middle" font-size="11" class="t-dim">mesma equipe, outra soma</text>
</svg>
:::

## 1) Um vetor como `readonly record struct`

Um **vetor** é uma direção com tamanho: aqui, no plano, com duas coordenadas. Vamos usar o eixo **X** para "valor entregue às pessoas" e o eixo **Y** para "saúde do que construímos" (código limpo, testes, base sólida). Uma iniciativa empurra para algum lado desse plano.

Para o vetor, escolhemos um **`struct`**, e não uma `class`:

| | `class` | `struct` |
|---|---|---|
| Fica em | memória "compartilhada" (referência) | dentro de quem o contém (**valor**) |
| Ao atribuir | duas variáveis apontam para o **mesmo** objeto | **copia** o valor |
| Bom para | coisas com identidade e vida longa | valores pequenos e imutáveis (como um número) |

O **`readonly record struct`** une isso a tudo que o `record` já traz: igualdade por valor, `with`, `ToString` bonito. E, como o nome diz, é **imutável**.

Ainda mais interessante: podemos **sobrecarregar operadores**, ensinando o C# a somar vetores com `+` e multiplicar por um número com `*`. O código fica parecido com a matemática.

```cs Jornada/Habitos/Cap106/Bussola.cs
namespace Jornada.Habitos.Cap106;

/// <summary>
/// Uma direção no plano (X: valor entregue às pessoas; Y: saúde do que construímos).
/// É um <c>struct</c> imutável: pequeno, sem identidade, copiado por valor.
/// </summary>
public readonly record struct Vetor(double X, double Y)
{
    public static Vetor Zero => new(0, 0);

    public static Vetor operator +(Vetor a, Vetor b) => new(a.X + b.X, a.Y + b.Y);
    public static Vetor operator *(double k, Vetor v) => new(k * v.X, k * v.Y);

    public double Norma => Math.Sqrt(X * X + Y * Y);

    public double ProdutoEscalar(Vetor outro) => X * outro.X + Y * outro.Y;

    /// <summary>Cosseno do ângulo entre dois vetores: 1 = mesma direção, 0 = perpendiculares, -1 = opostas.</summary>
    public double Cosseno(Vetor outro) =>
        Norma == 0 || outro.Norma == 0 ? 0 : ProdutoEscalar(outro) / (Norma * outro.Norma);

    public double AnguloEmGraus(Vetor outro) =>
        Math.Acos(Math.Clamp(Cosseno(outro), -1, 1)) * 180 / Math.PI;
}

public sealed record Iniciativa(string Nome, Vetor Efeito);

public enum Leitura { NoRumo, Desviando, RumoErrado }

public sealed class Bussola
{
    public Vetor Telos { get; }

    public Bussola(Vetor telos)
    {
        if (telos.Norma == 0)
            throw new ArgumentException("Um rumo precisa apontar para algum lugar.", nameof(telos));
        Telos = telos;
    }

    /// <summary>Recalibrar não muda a bússola: devolve outra, apontando para o novo rumo.</summary>
    public Bussola Recalibrar(Vetor novoTelos) => new(novoTelos);

    /// <summary>Quanto uma iniciativa, sozinha, aponta para o rumo (de -1 a 1).</summary>
    public double Alinhamento(Iniciativa iniciativa) => iniciativa.Efeito.Cosseno(Telos);

    /// <summary>Para onde o conjunto de tudo que foi feito está nos levando.</summary>
    public Vetor Deriva(IEnumerable<Iniciativa> feitas) =>
        feitas.Aggregate(Vetor.Zero, (soma, i) => soma + i.Efeito);

    /// <summary>Desvio total em graus. Sem nenhum movimento, não há desvio a medir (null).</summary>
    public double? DesvioEmGraus(IEnumerable<Iniciativa> feitas)
    {
        var deriva = Deriva(feitas);
        return deriva.Norma == 0 ? null : deriva.AnguloEmGraus(Telos);
    }

    public static Leitura Ler(double graus) => graus switch
    {
        < 25 => Leitura.NoRumo,
        < 70 => Leitura.Desviando,
        _    => Leitura.RumoErrado
    };
}
```

Pontos de atenção:

- **`operator +(Vetor a, Vetor b)`**: agora `new Vetor(1, 2) + new Vetor(3, -1)` funciona, devolvendo `Vetor(4, 1)`.
- **`Cosseno`** é o produto escalar dividido pelas normas: vale **1** se as direções coincidem, **0** se são perpendiculares e **−1** se opostas. É a medida clássica de "quanto duas direções concordam".
- **`Math.Clamp(valor, -1, 1)`** protege o `Math.Acos`: por erros minúsculos de arredondamento, um cosseno poderia dar 1,0000000002, o que faria o `Acos` devolver `NaN`.
- **`Aggregate(Vetor.Zero, (soma, i) => soma + i.Efeito)`**: a "dobra" com **valor inicial** `Zero`, somando todos os efeitos.
- **`double?`**: se **nada foi feito**, a soma é o vetor zero, que **não aponta para lugar nenhum**, então o ângulo não existe. Em vez de inventar um número, devolvemos `null`.
- **`Recalibrar`** não altera a bússola: devolve **outra**. Imutabilidade de novo, e por uma boa razão: ninguém que já usou a primeira vai ter o rumo trocado por baixo dos pés.

```cs Uso
var rumo = new Bussola(new Vetor(1, 1));      // valor E saúde, juntos

var feitas = Enumerable.Range(1, 6)
    .Select(i => new Iniciativa($"Funcionalidade {i}", new Vetor(1, -0.3)))
    .ToList();

double graus = rumo.DesvioEmGraus(feitas)!.Value;
Console.WriteLine($"Só funcionalidades: {graus:0} graus de desvio ({Bussola.Ler(graus)})");

feitas.Add(new Iniciativa("Pagar dívida técnica", new Vetor(0.2, 1)));
feitas.Add(new Iniciativa("Testes e documentação", new Vetor(0, 1)));
feitas.Add(new Iniciativa("Refatorar o que mais dói", new Vetor(0.5, 1)));

graus = rumo.DesvioEmGraus(feitas)!.Value;
Console.WriteLine($"Com três cuidados com a base: {graus:0} graus ({Bussola.Ler(graus)})");

feitas.Add(new Iniciativa("Automatizar a publicação", new Vetor(0.3, 1)));
feitas.Add(new Iniciativa("Simplificar o módulo de pagamentos", new Vetor(0.4, 1)));

graus = rumo.DesvioEmGraus(feitas)!.Value;
Console.WriteLine($"Com mais dois: {graus:0} graus ({Bussola.Ler(graus)})");

foreach (var i in feitas.TakeLast(3))
    Console.WriteLine($"  {i.Nome}: alinhamento {rumo.Alinhamento(i):0.00}");
```

```saida
Só funcionalidades: 62 graus de desvio (Desviando)
Com três cuidados com a base: 35 graus (Desviando)
Com mais dois: 22 graus (NoRumo)
  Refatorar o que mais dói: alinhamento 0,95
  Automatizar a publicação: alinhamento 0,88
  Simplificar o módulo de pagamentos: alinhamento 0,92
```

Leia a história que os números contam. Seis funcionalidades seguidas deram **62°** de desvio: cada uma, sozinha, apontava "para frente"; **juntas**, nos levaram para o lado errado. Três cuidados com a base trouxeram o desvio para 35°, e mais dois o levaram para **22°**, dentro da faixa "no rumo". Nenhuma iniciativa "errou": o que mudou foi **o que somou**.

::: dica Struct ou class?
Regra prática: se o tipo é **pequeno** (até uns 16 bytes), **imutável** e se comporta como um **valor** (ninguém pergunta "este vetor é o mesmo objeto daquele?", só "são iguais?"), use `struct`. Se tem identidade, ciclo de vida, ou é grande, use `class`. Na dúvida, comece com `record` (classe), que já serve para a maioria dos casos.
:::

## 2) Testes

```cs Habitos.Testes/Cap106Tests.cs
using Jornada.Habitos.Cap106;

namespace Habitos.Testes;

public class Cap106Tests
{
    [Fact]
    public void Vetores_somam_e_multiplicam_por_numero()
    {
        var soma = new Vetor(1, 2) + new Vetor(3, -1);
        Assert.Equal(new Vetor(4, 1), soma);
        Assert.Equal(new Vetor(2, 4), 2 * new Vetor(1, 2));
    }

    [Fact]
    public void Struct_se_compara_pelo_valor_e_copia_nao_altera_o_original()
    {
        var a = new Vetor(1, 1);
        var b = a;                              // cópia
        b = b with { X = 9 };

        Assert.Equal(new Vetor(1, 1), a);
        Assert.NotEqual(a, b);
        Assert.Equal(new Vetor(1, 1), new Vetor(1, 1));
    }

    [Theory]
    [InlineData(1, 0, 1, 0, 1)]
    [InlineData(1, 0, 0, 1, 0)]
    [InlineData(1, 0, -1, 0, -1)]
    public void Cosseno_mede_o_alinhamento(double x1, double y1, double x2, double y2, double esperado) =>
        Assert.Equal(esperado, new Vetor(x1, y1).Cosseno(new Vetor(x2, y2)), 6);

    [Fact]
    public void Angulo_de_perpendiculares_e_noventa_graus() =>
        Assert.Equal(90, new Vetor(1, 0).AnguloEmGraus(new Vetor(0, 1)), 6);

    [Fact]
    public void Rumo_nulo_e_recusado() =>
        Assert.Throws<ArgumentException>(() => new Bussola(Vetor.Zero));

    private static readonly Bussola Rumo = new(new Vetor(1, 1));       // valor E saúde, juntos

    [Fact]
    public void Iniciativas_que_apontam_para_o_rumo_tem_alinhamento_alto()
    {
        Assert.True(Rumo.Alinhamento(new Iniciativa("Refatorar o que mais dói", new Vetor(1, 1))) > 0.99);
        Assert.True(Rumo.Alinhamento(new Iniciativa("Só novas telas", new Vetor(1, -0.2))) < 0.7);
    }

    [Fact]
    public void Varias_pequenas_entregas_so_de_valor_levam_a_um_desvio_acumulado()
    {
        var feitas = Enumerable.Range(1, 6).Select(i => new Iniciativa($"Funcionalidade {i}", new Vetor(1, -0.3))).ToList();

        double graus = Rumo.DesvioEmGraus(feitas)!.Value;

        Assert.InRange(graus, 55, 70);
        Assert.Equal(Leitura.Desviando, Bussola.Ler(graus));
    }

    [Fact]
    public void Misturar_valor_e_saude_mantem_o_rumo()
    {
        var feitas = new[]
        {
            new Iniciativa("Funcionalidade", new Vetor(1, -0.3)),
            new Iniciativa("Pagar dívida técnica", new Vetor(0.2, 1)),
            new Iniciativa("Funcionalidade", new Vetor(1, -0.3)),
            new Iniciativa("Testes e documentação", new Vetor(0, 1)),
        };

        Assert.Equal(Leitura.NoRumo, Bussola.Ler(Rumo.DesvioEmGraus(feitas)!.Value));
    }

    [Fact]
    public void Sem_nada_feito_nao_ha_desvio_para_medir() =>
        Assert.Null(Rumo.DesvioEmGraus(Array.Empty<Iniciativa>()));

    [Fact]
    public void Recalibrar_devolve_outra_bussola_e_a_primeira_continua_igual()
    {
        var nova = Rumo.Recalibrar(new Vetor(0, 1));
        Assert.Equal(new Vetor(1, 1), Rumo.Telos);
        Assert.Equal(new Vetor(0, 1), nova.Telos);
    }

    [Theory]
    [InlineData(10, Leitura.NoRumo)]
    [InlineData(40, Leitura.Desviando)]
    [InlineData(120, Leitura.RumoErrado)]
    public void Leitura_por_faixas(double graus, Leitura esperado) => Assert.Equal(esperado, Bussola.Ler(graus));
}
```

O teste `Struct_se_compara_pelo_valor_e_copia_nao_altera_o_original` mostra a diferença para uma `class`: `b = a` **copia**, e mudar `b` (com `with`) não toca em `a`. E `Varias_pequenas_entregas_so_de_valor_levam_a_um_desvio_acumulado` é o capítulo inteiro em um teste.

::: exercicio As iniciativas que mais ajudam
Escreva o método de extensão `MaisAlinhadas(this Bussola bussola, IEnumerable<Iniciativa> iniciativas, int quantas)`, que devolve as iniciativas com **maior alinhamento** com o rumo, em ordem decrescente, desempatando pelo **nome**. Use `OrderByDescending(bussola.Alinhamento)`: passar um **método** direto, sem lambda.

--- resposta
```cs Jornada/Habitos/Exercicios/Cap106Alinhadas.cs
namespace Jornada.Habitos.Cap106;

public static class BussolaExtensoes
{
    /// <summary>As iniciativas que mais apontam para o rumo; no empate, a ordem alfabética.</summary>
    public static IReadOnlyList<Iniciativa> MaisAlinhadas(this Bussola bussola, IEnumerable<Iniciativa> iniciativas, int quantas) =>
        iniciativas
            .OrderByDescending(bussola.Alinhamento)
            .ThenBy(i => i.Nome)
            .Take(quantas)
            .ToList();
}
```

`OrderByDescending(bussola.Alinhamento)` passa o **grupo de métodos** `Alinhamento` como a função de chave: é o mesmo que `i => bussola.Alinhamento(i)`, só que mais curto. E `Take(quantas)` pega as primeiras. Repare que o resultado é **ordenado por alinhamento, não por "importância" declarada**: a pergunta que o programa responde é "o que mais aponta para onde queremos ir?".
:::

::: resumo
- Para Smith, o coração é uma **bússola** que orienta para uma visão de vida boa (o **telos**); nossos desvios vêm de bússola mal calibrada **ou de mapa errado** (imagens rivais de "vida boa").
- Escolhas pequenas **somam** um rumo: avaliar uma por uma esconde o desvio; **somar** o mostra.
- **`readonly record struct`** é um tipo de valor, imutável e comparável por valor; **operadores** (`+`, `*`) podem ser sobrecarregados para o código imitar a matemática.
- **Cosseno** mede o alinhamento entre duas direções; `Math.Clamp` protege `Math.Acos`; `Aggregate` com valor inicial soma sequências; `double?` diz "não há o que medir".
- **Recalibrar** devolve uma nova bússola, em vez de mudar a existente.
:::

::: quiz
? O que é o "telos", neste livro?
+ Aquilo para o qual algo (ou alguém) está orientado: um fim ou meta
- Um tipo de erro de navegação
- Um recurso do C#
- Um modelo de relatório
> Seres teleológicos estão sempre indo em direção a algo.

? Qual a diferença entre uma bússola mal calibrada e um mapa errado, na imagem do autor?
+ A bússola mal calibrada desvia a direção; o mapa errado leva a uma imagem falsa do destino mesmo com a bússola certa
- Não há diferença
- O mapa errado só importa no mar
- A bússola não é um instrumento confiável
> Nossos amores podem se perder pela direção ou pela imagem de "vida boa" que seguimos.

? Por que `Vetor` é um `struct` e não uma `class`?
+ Porque é pequeno, imutável e se comporta como um valor
- Porque structs são mais seguros em rede
- Porque classes não aceitam operadores
- Porque o C# obriga
> Atribuir copia o valor, e a comparação é por valor.

? O que acontece em `Math.Acos` se o cosseno passar de 1 por erro de arredondamento?
+ Devolve NaN, por isso usamos `Math.Clamp` antes
- Devolve 0
- Lança uma exceção
- Devolve 180
> `Clamp` mantém o valor no intervalo válido de -1 a 1.

? Por que `DesvioEmGraus` devolve `double?`?
+ Porque, sem nenhum movimento, não há direção, e portanto não há ângulo para medir
- Porque o ângulo pode ser negativo
- Porque `double` não aceita zero
- Porque ficaria mais rápido
> `null` diz "não há o que medir" sem inventar um número.

? O que o capítulo quer mostrar com as seis funcionalidades seguidas?
+ Que escolhas individualmente boas podem somar um desvio de rumo
- Que funcionalidades são ruins
- Que a equipe errou
- Que dívida técnica é um erro moral
> O que importa é a soma, e não o julgamento de cada item.
:::

::: cartas
Telos :: Fim ou meta para o qual algo é orientado.
Bússola do coração :: Imagem do autor: o desejo orienta a uma visão de vida boa.
Mapa errado :: Imagem falsa do destino; a bússola funciona, mas o rumo se perde.
readonly record struct :: Tipo de valor, imutável, com igualdade por valor.
Sobrecarga de operador :: Ensinar o C# a usar `+`, `*` etc. com um tipo seu.
Produto escalar :: Soma dos produtos das coordenadas; base do cosseno entre vetores.
Cosseno :: 1 = mesma direção, 0 = perpendiculares, -1 = opostas.
Deriva :: Soma de todos os empurrões: para onde o conjunto das escolhas nos leva.
:::
