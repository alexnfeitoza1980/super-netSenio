---
id: resiliencia-cap91
order: 117
kind: capitulo
num: 91
group: C# & Mentalidade · Parte 15 · Resiliência em C#
title: Otimismo com os pés no chão
subtitle: Esperança que enxerga os obstáculos e se apoia em evidências, não em frases prontas. Em C#, é o capítulo do "pode ser nulo": valores opcionais, fontes que faltam e padrões que leem o quadro inteiro.
minutes: 60
hue: 205
objectives:
- Diferenciar **otimismo realista** de otimismo ingênuo e de pessimismo
- Entender **tipos anuláveis** (`string?`, `DateOnly?`) e o aviso do compilador contra `NullReferenceException`
- Combinar **padrões** (`is not null`, tupla com relacionais) para ler um quadro de evidências
- Usar `MaxBy` e métodos de extensão sobre dados opcionais
origem: livros/06-resiliencia-csharp/content/128-cap91.md
---

::: vila A horta que não cresceu em um dia
Lia plantou tomates na varanda e, na terceira semana, só via folhas. Caio disse: "não vai dar nada, plantar não é com você". Marta disse: "vai dar um monte, é só esperar". Nenhum dos dois ajudou. Lia pegou um caderno e anotou **o que já havia**: o broto da semana passada, a terra úmida, a previsão de sol. E **o que atrapalhava**: o vaso pequeno, o vento. Com as duas colunas, decidiu trocar o vaso, e **seguir esperando com um plano**.
:::

::: ilustracao img/resiliencia/cap91.svg | Uma lanterna, não um mapa completo: o otimismo realista ilumina o próximo trecho, sem esconder as pedras do caminho.
:::

## O que o livro diz

Entre os fatores de resiliência que o texto reúne está o **otimismo**: a disposição de acreditar que **há possibilidades** mesmo diante de dificuldades. Na nossa leitura, o ponto delicado é este: otimismo **não** é negar o problema. Quem diz "vai dar tudo certo" sem olhar para nada pode se frustrar; quem só vê obstáculos pode paralisar. O caminho do meio é **olhar para os dois** e procurar, entre as possibilidades, aquelas que **se sustentam em algo que dá para conferir**.

::: analogia Uma lanterna, não um mapa completo
Uma lanterna não mostra o caminho inteiro; mostra **o próximo trecho**. O otimismo realista é assim: ilumina o suficiente para dar o passo seguinte, **sem fingir** que as pedras não estão lá. Quando alguém pede "só pense positivo", está oferecendo um mapa que não existe.
:::

::: cuidado Esperança emprestada não é obrigação
Se a outra pessoa não consegue ver possibilidades **agora**, insistir em "pense positivo" machuca. Às vezes, o mais cuidadoso é **reconhecer o peso** e oferecer companhia. Esperança não se impõe: se **constrói**, com evidências pequenas e verdadeiras.
:::

::: ilustracao img/resiliencia/photos/muda.jpg | Uma muda não promete a árvore inteira; mostra o próximo broto. É disso que o otimismo realista se alimenta.
:::

## Da vida para o código

| Na vida | Em C# |
|---|---|
| Algo que posso conferir, com fonte | `Evidencia(Texto, Fonte?, Data?)` com **tipos anuláveis** |
| "Frase pronta" sem base | Possibilidade **sem fonte** (`TemFonte == false`) |
| Olhar obstáculos **e** possibilidades | Um `switch` sobre uma **tupla** de três contagens |
| "O que ainda preciso conferir?" | `Where(e => !e.TemFonte)` |
| A evidência mais recente | **`MaxBy`** com dados opcionais |

::: figura Quatro leituras possíveis de um quadro
<svg viewBox="0 0 740 230" role="img" aria-label="Quatro combinações de obstáculos e possibilidades e a leitura de cada uma">
  <g font-size="13" font-weight="800" text-anchor="middle">
    <rect x="20" y="20" width="170" height="80" rx="12" class="f-bad"/><text x="105" y="50">Só obstáculos</text><text x="105" y="72" font-size="11" class="t-dim" font-weight="400">que possibilidade confirmo?</text>
    <rect x="205" y="20" width="170" height="80" rx="12" class="f-warn"/><text x="290" y="50">Possibilidade sem fonte</text><text x="290" y="72" font-size="11" class="t-dim" font-weight="400">ainda é uma ideia</text>
    <rect x="390" y="20" width="170" height="80" rx="12" class="f-info"/><text x="475" y="50">Só possibilidades</text><text x="475" y="72" font-size="11" class="t-dim" font-weight="400">e os obstáculos?</text>
    <rect x="575" y="20" width="145" height="80" rx="12" class="f-good"/><text x="647" y="50">Equilibrado</text><text x="647" y="72" font-size="11" class="t-dim" font-weight="400">próximo passo possível</text>
  </g>
  <text x="370" y="140" text-anchor="middle" font-size="14" font-weight="800">Otimismo realista = as duas colunas, com fonte nas possibilidades</text>
  <rect x="150" y="160" width="190" height="52" rx="10" class="f-prim"/><text x="245" y="183" text-anchor="middle" font-weight="800">Obstáculos reais</text><text x="245" y="201" text-anchor="middle" font-size="11" class="t-dim">o que pesa de verdade</text>
  <rect x="400" y="160" width="190" height="52" rx="10" class="f-good"/><text x="495" y="183" text-anchor="middle" font-weight="800">Possibilidades</text><text x="495" y="201" text-anchor="middle" font-size="11" class="t-dim">com algo para conferir</text>
</svg>
:::

## 1) "Pode ser nulo": o `?` que muda tudo

Em C#, **`string?`** significa "um texto **ou nada** (`null`)", e **`string`** (sem o `?`) promete que **sempre há** um texto. O compilador usa essa diferença para **avisar** quando você usa algo que pode não existir, a cura para a famosa `NullReferenceException`. Aqui, uma `Evidencia` **precisa** de um texto, mas a `Fonte` e a `Data` podem faltar, e essa falta é **informação**: sem fonte, a "possibilidade" ainda é uma frase pronta.

O `Ler()` abaixo olha três números, as contagens de obstáculos, de possibilidades e de possibilidades **com fonte**, e as lê juntas com um `switch` sobre a **tupla**. Cada linha é um padrão: `(_, > 0, 0)` quer dizer "qualquer número de obstáculos, **alguma** possibilidade, mas **nenhuma com fonte**".

```cs Jornada/Resiliencia/Cap91/QuadroDeEvidencias.cs
namespace Jornada.Resiliencia.Cap91;

/// <summary>Uma evidência é um fato que se pode conferir. "Fonte" pode faltar, e isso muda a leitura.</summary>
public sealed record Evidencia(string Texto, string? Fonte = null, DateOnly? Data = null)
{
    public bool TemFonte => !string.IsNullOrWhiteSpace(Fonte);
}

public enum Leitura
{
    Vazio,
    SoObstaculos,
    SoPossibilidades,
    PossibilidadesSemFonte,
    Equilibrado
}

public sealed class QuadroDeEvidencias
{
    private readonly List<Evidencia> obstaculos = new();
    private readonly List<Evidencia> possibilidades = new();

    public QuadroDeEvidencias Obstaculo(Evidencia e) { obstaculos.Add(e); return this; }
    public QuadroDeEvidencias Possibilidade(Evidencia e) { possibilidades.Add(e); return this; }

    public Leitura Ler()
    {
        int o = obstaculos.Count;
        int pTotal = possibilidades.Count;
        int pComFonte = possibilidades.Count(e => e.TemFonte);

        return (o, pTotal, pComFonte) switch
        {
            (0, 0, _)    => Leitura.Vazio,
            (_, > 0, 0)  => Leitura.PossibilidadesSemFonte,   // só frases prontas, sem evidência
            (> 0, 0, _)  => Leitura.SoObstaculos,
            (0, _, > 0)  => Leitura.SoPossibilidades,
            _            => Leitura.Equilibrado
        };
    }

    /// <summary>Possibilidades que ainda são "frases prontas" até alguém conferir.</summary>
    public IEnumerable<Evidencia> AConferir() => possibilidades.Where(e => !e.TemFonte);

    public static string Mensagem(Leitura leitura) => leitura switch
    {
        Leitura.Vazio                  => "Comece anotando um obstáculo real e uma possibilidade.",
        Leitura.SoObstaculos           => "Os obstáculos são reais. Que possibilidade, mesmo pequena, você consegue confirmar?",
        Leitura.SoPossibilidades       => "Ótimo ter possibilidades. E quais obstáculos também existem? Olhar os dois é realismo.",
        Leitura.PossibilidadesSemFonte => "Essas possibilidades ainda são ideias. De onde vem cada uma? Dá para conferir?",
        Leitura.Equilibrado            => "Você olhou os obstáculos e as possibilidades. Qual é o próximo passo possível?",
        _                              => throw new ArgumentOutOfRangeException(nameof(leitura))
    };
}
```

Veja o que a ordem dos padrões faz:

- `(0, 0, _)`: nada foi anotado ainda.
- `(_, > 0, 0)`: há possibilidades, mas nenhuma tem fonte → são **ideias**, não evidências.
- `(> 0, 0, _)`: só obstáculos.
- `(0, _, > 0)`: só possibilidades confirmadas, e os obstáculos? Otimismo realista olha os dois.
- `_`: o equilíbrio.

::: dica `is not null` e `string.IsNullOrWhiteSpace`
Para testar "tem valor?", prefira `is not null` (e `is null`): lê-se como português e não pode ser sobrescrito por um operador `==` customizado. Para **texto**, `string.IsNullOrWhiteSpace` cobre três casos de uma vez: `null`, `""` e `"   "`.
:::

```cs Uso
var quadro = new QuadroDeEvidencias()
    .Obstaculo(new Evidencia("Tenho pouco tempo", "minha agenda da semana"))
    .Possibilidade(new Evidencia("Vai dar tudo certo"));

Console.WriteLine(QuadroDeEvidencias.Mensagem(quadro.Ler()));

quadro.Possibilidade(new Evidencia("Já resolvi uma lista parecida", "exercícios de março", new DateOnly(2026, 3, 14)));
Console.WriteLine(QuadroDeEvidencias.Mensagem(quadro.Ler()));
```

```saida
Essas possibilidades ainda são ideias. De onde vem cada uma? Dá para conferir?
Você olhou os obstáculos e as possibilidades. Qual é o próximo passo possível?
```

A primeira leitura reconhece que "vai dar tudo certo" **ainda é uma ideia**. Só quando entra uma possibilidade com **fonte e data**, o quadro vira equilibrado e a mensagem pergunta pelo **próximo passo possível**.

## 2) Testes

```cs Resiliencia.Testes/Cap91Tests.cs
using Jornada.Resiliencia.Cap91;

namespace Resiliencia.Testes;

public class Cap91Tests
{
    private static Evidencia Obst(string t) => new(t, "o próprio edital");
    private static Evidencia ComFonte(string t) => new(t, "nota da última prova");
    private static Evidencia SemFonte(string t) => new(t);

    [Fact]
    public void Quadro_vazio_pede_o_primeiro_registro() =>
        Assert.Equal(Leitura.Vazio, new QuadroDeEvidencias().Ler());

    [Fact]
    public void So_obstaculos()
    {
        var q = new QuadroDeEvidencias().Obstaculo(Obst("A vaga é concorrida"));
        Assert.Equal(Leitura.SoObstaculos, q.Ler());
    }

    [Fact]
    public void So_possibilidades_com_fonte_pede_olhar_os_obstaculos()
    {
        var q = new QuadroDeEvidencias().Possibilidade(ComFonte("Melhorei 2 pontos desde março"));
        Assert.Equal(Leitura.SoPossibilidades, q.Ler());
    }

    [Fact]
    public void Possibilidade_sem_fonte_e_so_uma_ideia()
    {
        var q = new QuadroDeEvidencias()
            .Obstaculo(Obst("Tenho pouco tempo"))
            .Possibilidade(SemFonte("Vai dar tudo certo"));

        Assert.Equal(Leitura.PossibilidadesSemFonte, q.Ler());
        Assert.Single(q.AConferir());
    }

    [Fact]
    public void Obstaculo_e_possibilidade_confirmada_dao_equilibrio()
    {
        var q = new QuadroDeEvidencias()
            .Obstaculo(Obst("Tenho pouco tempo"))
            .Possibilidade(ComFonte("Já resolvi problemas parecidos"));

        Assert.Equal(Leitura.Equilibrado, q.Ler());
        Assert.Empty(q.AConferir());
    }

    [Theory]
    [InlineData(Leitura.Vazio)]
    [InlineData(Leitura.SoObstaculos)]
    [InlineData(Leitura.SoPossibilidades)]
    [InlineData(Leitura.PossibilidadesSemFonte)]
    [InlineData(Leitura.Equilibrado)]
    public void Toda_leitura_tem_mensagem(Leitura l) =>
        Assert.False(string.IsNullOrWhiteSpace(QuadroDeEvidencias.Mensagem(l)));
}
```

Veja os três "construtores" no começo da classe de teste: `Obst`, `ComFonte` e `SemFonte` são **funções auxiliares** que deixam os testes lendo como frases. Boa prática: quando o dado de teste se repete, dê a ele um nome.

::: exercicio A evidência mais recente
Escreva o método de extensão `MaisRecente(this IEnumerable<Evidencia> evidencias)`, que devolve a evidência de **data mais recente**, **ignorando** as que não têm data. Se nenhuma tiver data, devolve `null`. Dica: `Where` e **`MaxBy`**.

--- resposta
```cs Jornada/Resiliencia/Exercicios/Cap91Recente.cs
namespace Jornada.Resiliencia.Cap91;

public static class EvidenciasExtensoes
{
    /// <summary>A evidência mais recente entre as que têm data. Sem nenhuma data, devolve null.</summary>
    public static Evidencia? MaisRecente(this IEnumerable<Evidencia> evidencias) =>
        evidencias
            .Where(e => e.Data is not null)
            .MaxBy(e => e.Data);
}
```

`MaxBy(e => e.Data)` devolve **o elemento** cujo valor é o maior (diferente de `Max`, que devolve só o valor). Depois do `Where(e => e.Data is not null)`, todas as datas existem; e, se não sobrar nenhum item, `MaxBy` devolve `null`, exatamente o que o tipo `Evidencia?` anuncia.
:::

::: resumo
- **Otimismo realista** olha os **obstáculos** e as **possibilidades**; esperança que se apoia em algo **conferível** dura mais que frase pronta.
- `string?` e `DateOnly?` dizem "**pode faltar**"; o compilador avisa quando você usa algo anulável sem checar.
- Uma **possibilidade sem fonte** ainda é uma ideia; o modelo pede para conferi-la, **sem desmerecê-la**.
- `switch` sobre **tupla** com padrões `(_, > 0, 0)` lê vários números de uma só vez; a **ordem** importa.
- `MaxBy`, `Where` e `is not null` trabalham bem juntos com dados opcionais.
:::

::: quiz
? O que distingue otimismo realista de otimismo ingênuo?
+ O realista reconhece os obstáculos e apoia as possibilidades em algo que dá para conferir
- O realista só vê os problemas
- O ingênuo é mais profundo
- Não há diferença
> Olhar os dois lados é o que torna a esperança sustentável.

? O que significa `string?` em C#?
+ Um texto que pode ser nulo (ausente)
- Um texto de qualquer tamanho
- Uma pergunta ao usuário
- Um texto obrigatório
> O `?` marca que o valor pode faltar, e o compilador passa a vigiar o uso.

? Por que uma possibilidade sem fonte aparece como "ideia a conferir" e não como evidência?
+ Porque ainda não há nada concreto que a sustente
- Porque ideias são ruins
- Porque o programa não gosta de otimismo
- Porque a fonte é só decoração
> O modelo convida a conferir, não a desistir.

? No padrão `(_, > 0, 0)`, o que significa o `_`?
+ Qualquer valor naquela posição
- O valor zero
- Um erro
- Um valor negativo
> O `_` é o "coringa" dos padrões.

? Qual a diferença entre `Max` e `MaxBy` do LINQ?
+ `MaxBy` devolve o elemento com o maior valor; `Max` devolve só o valor
- São idênticos
- `MaxBy` ordena a lista
- `Max` só funciona com texto
> `MaxBy(e => e.Data)` entrega a evidência inteira.

? Se alguém não consegue ver possibilidades agora, qual a atitude mais cuidadosa?
+ Reconhecer o peso e oferecer companhia, sem impor positividade
- Repetir "pense positivo"
- Dizer que é exagero
- Mudar de assunto
> Esperança se constrói; não se impõe.
:::

::: cartas
Otimismo realista :: Reconhecer obstáculos e incertezas sem fechar a porta para possibilidades.
Evidência :: Algo conferível que sustenta uma possibilidade ou um obstáculo.
Tipo anulável :: Tipo marcado com `?` que aceita `null`, como `string?`.
NullReferenceException :: Erro ao usar algo que era `null`; os tipos anuláveis ajudam a evitá-lo.
is not null :: Forma legível de testar a presença de um valor.
Padrão com tupla :: `switch` que combina várias condições, como `(0, _, > 0)`.
MaxBy :: LINQ que devolve o elemento de maior valor segundo uma chave.
Método de extensão :: Método estático que se usa como se fosse da classe: `colecao.Metodo()`.
:::
