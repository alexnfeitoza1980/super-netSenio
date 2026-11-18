---
id: resiliencia-cap88
order: 114
kind: capitulo
num: 88
group: C# & Mentalidade · Parte 15 · Resiliência em C#
title: A vida não vem com botão de desfazer
subtitle: O que fazemos quando algo muda, falha ou dói, e por que resiliência não é voltar a ser exatamente como antes. Em C#, falhas também acontecem, e dá para tratá-las com cuidado.
minutes: 60
hue: 262
objectives:
- Entender a ideia de **resiliência** e por que ela **não** significa "voltar ao que era"
- Modelar **três caminhos possíveis** (recuperar, adaptar, buscar apoio) com `enum`, `record` e `switch`
- Tratar falhas com **`try / catch / finally`** e saber quando preferir **`TryParse`**
- Escrever **testes** que descrevem o comportamento esperado
origem: livros/06-resiliencia-csharp/content/125-cap88.md
---

::: vila A prova de terça
Na terça, Caio foi mal na prova de banco de dados e quis largar o curso naquela noite. Na quarta, Bia também foi mal; ficou chateada, dormiu mal e, no dia seguinte, pediu ajuda à Lia para entender duas questões. Lia não disse "seja forte": perguntou "o que mudou nessa prova e o que dá para fazer **agora**?". Ninguém ali era "mais resiliente" que o outro. Cada um tinha cansaço, recursos e companhia diferentes.
:::

::: ilustracao img/resiliencia/cap88.svg | Depois de uma dificuldade há mais de um caminho: recuperar, adaptar ou contar com alguém. Eles podem acontecer juntos.
:::

## O que o livro diz

O texto que inspira esta jornada descreve pessoas resilientes como aquelas que **se recuperam depois de dificuldades** e procuram **soluções ou formas de adaptação**: perder um emprego, repetir de ano, atravessar uma perda. A ideia importante é que **a pessoa responde** ao que aconteceu, com os recursos que tem, **inclusive os recursos que vêm de outras pessoas**.

Nas nossas palavras, **resiliência é a possibilidade de responder a uma dificuldade com os recursos disponíveis e encontrar uma forma de continuar ou de se adaptar.** A palavra "possibilidade" importa: ninguém controla todas as condições, e a capacidade de atravessar um problema pode variar de um dia para o outro.

::: analogia A ponte depois da chuva
Depois de uma enchente, uma ponte pode continuar no mesmo lugar, mas precisar de reparos. Uma trilha pode mudar de rota. Às vezes a vida pede **recuperação**; outras vezes, **adaptação** a uma realidade nova. Nem sempre "voltar ao normal" é possível, e nem sempre é o melhor objetivo.
:::

::: cuidado Pessoas não são elásticos
Um elástico estica e volta, e é uma imagem útil, mas pessoas têm lembranças, relações, limites e escolhas. Uma experiência difícil pode nos transformar, e **não existe uma única forma correta de seguir**. Um momento de bloqueio **não prova** "falta de resiliência": sono, pressão e falta de apoio também contam.
:::

::: ilustracao img/resiliencia/photos/referencia-flor-resiliencia.png | Mesmo em terreno difícil, a vida pode encontrar espaço para brotar, sem que isso torne a seca menos real.
:::

## Da vida para o código

| Na vida | Em C# |
|---|---|
| Três respostas possíveis (recuperar, adaptar, buscar apoio) | Um **`enum`** com três valores |
| Uma dificuldade, com seu contexto | Um **`record`** (dados que não mudam) |
| Cada resposta tem seu sentido | Um **`switch`** que descreve cada valor |
| Falhas acontecem, e precisamos reagir sem quebrar | **`try / catch / finally`** |
| Erro previsível não é emergência | **`TryParse`**: devolve `bool`, sem exceção |

::: figura Três respostas possíveis diante de uma mudança
<svg viewBox="0 0 740 225" role="img" aria-label="Depois de uma dificuldade, três caminhos possíveis são recuperar, adaptar e buscar apoio">
  <rect x="18" y="72" width="160" height="72" rx="14" class="f-warn"/>
  <text x="98" y="101" text-anchor="middle" font-weight="800">Acontece algo</text><text x="98" y="122" text-anchor="middle" font-size="12" class="t-dim">mudança ou perda</text>
  <rect x="252" y="28" width="200" height="52" rx="12" class="f-info"/>
  <text x="352" y="51" text-anchor="middle" font-weight="800">Recuperar</text><text x="352" y="68" text-anchor="middle" font-size="11" class="t-dim">retomar parte da rotina</text>
  <rect x="252" y="91" width="200" height="52" rx="12" class="f-good"/>
  <text x="352" y="114" text-anchor="middle" font-weight="800">Adaptar</text><text x="352" y="131" text-anchor="middle" font-size="11" class="t-dim">encontrar outro caminho</text>
  <rect x="252" y="154" width="200" height="52" rx="12" class="f-prim"/>
  <text x="352" y="177" text-anchor="middle" font-weight="800">Buscar apoio</text><text x="352" y="194" text-anchor="middle" font-size="11" class="t-dim">não atravessar só</text>
  <path class="arrow" d="M180 108C214 108 214 54 250 54"/><path class="arrow" d="M180 108H250"/><path class="arrow" d="M180 108C214 108 214 180 250 180"/>
  <text x="585" y="86" text-anchor="middle" font-size="13" class="t-dim">Podem acontecer juntas.</text><text x="585" y="108" text-anchor="middle" font-size="13" class="t-dim">Não são etapas obrigatórias</text><text x="585" y="130" text-anchor="middle" font-size="13" class="t-dim">nem uma escala de valor.</text>
</svg>
:::

## 1) Os caminhos possíveis: `enum`, `record` e `switch`

Começamos pelo vocabulário. Um **`enum`** nomeia um conjunto fechado de opções: aqui, as três respostas. Um **`record`** guarda dados que não mudam depois de criados; é perfeito para descrever uma `Dificuldade`. E o **`switch` expression** devolve um valor para cada caso, com a vantagem de o compilador avisar se esquecermos algum.

```cs Jornada/Resiliencia/Cap88/Caminhos.cs
namespace Jornada.Resiliencia.Cap88;

public enum Resposta { Recuperar, Adaptar, BuscarApoio }

public sealed record Dificuldade(
    string Descricao,
    bool RotinaAindaPossivel,
    bool TemAlguemDeConfianca);

public static class Caminhos
{
    /// <summary>Devolve os caminhos que fazem sentido agora. Nenhum deles é obrigatório.</summary>
    public static IReadOnlyList<Resposta> Possiveis(Dificuldade d)
    {
        var caminhos = new List<Resposta>();

        if (d.RotinaAindaPossivel)
            caminhos.Add(Resposta.Recuperar);

        caminhos.Add(Resposta.Adaptar);          // adaptar-se é sempre uma possibilidade

        if (d.TemAlguemDeConfianca)
            caminhos.Add(Resposta.BuscarApoio);

        return caminhos;
    }

    public static string Descrever(Resposta r) => r switch
    {
        Resposta.Recuperar   => "Retomar o que for possível da rotina, no seu ritmo.",
        Resposta.Adaptar     => "Procurar outro caminho para chegar a um objetivo parecido.",
        Resposta.BuscarApoio => "Contar com alguém de confiança para atravessar junto.",
        _                    => throw new ArgumentOutOfRangeException(nameof(r))
    };
}
```

Repare em três detalhes:

- **`Adaptar` entra sempre**: adaptar-se é uma possibilidade mesmo quando a rotina antiga não volta e ninguém está por perto. Já `Recuperar` e `BuscarApoio` aparecem só quando fazem sentido.
- O método devolve uma **lista de possibilidades**, e não **uma** resposta "certa". O programa não escolhe pela pessoa.
- O `_ => throw ...` é a rede de segurança do `switch`: se alguém adicionar um valor novo ao `enum` e esquecer de tratá-lo, o erro aparece cedo, em vez de uma resposta vazia.

::: ide Rodando o primeiro trecho
No Visual Studio, abra `codigo/resiliencia/Resiliencia.sln`, clique com o botão direito em **Resiliencia.Testes** e escolha **Executar Testes**. No terminal, `dotnet test codigo/resiliencia/Resiliencia.sln` faz o mesmo. Para brincar com os trechos, crie um projeto de console e use o código "Uso" abaixo.
:::

## 2) Quando algo dá errado: `try / catch / finally`

Falhas também acontecem em programas: alguém digita "sete" onde se espera `7`. Em C#, uma operação que não consegue terminar **lança uma exceção**. Se ninguém a trata, o programa **cai**. O bloco **`try / catch`** permite reagir: o código perigoso fica no `try`; cada `catch` trata um tipo de falha; o **`finally`** roda **sempre**, deu certo ou não.

```cs Jornada/Resiliencia/Cap88/Anotador.cs
namespace Jornada.Resiliencia.Cap88;

public sealed class Anotador
{
    public int Tentativas { get; private set; }

    /// <summary>Versão com exceções: tenta converter e trata cada tipo de falha.</summary>
    public string AnotarHorasDeSono(string texto)
    {
        try
        {
            int horas = int.Parse(texto);                       // pode lançar FormatException
            if (horas is < 0 or > 24)
                throw new ArgumentOutOfRangeException(nameof(texto), "Use um valor entre 0 e 24.");

            return $"{horas} h anotadas.";
        }
        catch (FormatException)
        {
            return "Não entendi o número. Tente algo como 7.";
        }
        catch (ArgumentOutOfRangeException)
        {
            return "Esse valor não cabe em um dia. Use de 0 a 24.";
        }
        finally
        {
            Tentativas++;                                        // roda sempre, deu certo ou não
        }
    }

    /// <summary>Versão sem exceções: falha esperada vira um resultado, não um susto.</summary>
    public string AnotarComTryParse(string texto)
    {
        Tentativas++;

        if (!int.TryParse(texto, out int horas))
            return "Não entendi o número. Tente algo como 7.";

        return horas is < 0 or > 24
            ? "Esse valor não cabe em um dia. Use de 0 a 24."
            : $"{horas} h anotadas.";
    }
}
```

Veja como cada falha recebe uma resposta **gentil e específica**: não "erro!", mas "tente algo como 7". E o `finally` cuida do que precisa acontecer **de qualquer jeito**, aqui, contar a tentativa (na vida real: fechar um arquivo, liberar uma conexão).

Mas olhe o segundo método. **`int.TryParse`** faz o mesmo sem exceção: devolve `false` quando não consegue e entrega o número pelo parâmetro `out`. Qual usar?

::: regra Exceção para o inesperado, `TryParse` para o esperado
Entrada errada de uma pessoa é **esperada**: use `TryParse` e responda com uma mensagem. Exceções ficam para o que **não deveria** acontecer (um arquivo que sumiu, uma regra violada por outro trecho do código). Exceções são **caras** e escondem o fluxo normal do programa quando usadas como `if`.
:::

```cs Uso
var dificuldade = new Dificuldade("Fui mal na prova", RotinaAindaPossivel: true, TemAlguemDeConfianca: true);

foreach (var caminho in Caminhos.Possiveis(dificuldade))
    Console.WriteLine($"{caminho}: {Caminhos.Descrever(caminho)}");

var anotador = new Anotador();
Console.WriteLine(anotador.AnotarHorasDeSono("7"));
Console.WriteLine(anotador.AnotarHorasDeSono("sete"));
Console.WriteLine(anotador.AnotarHorasDeSono("40"));
Console.WriteLine($"Tentativas: {anotador.Tentativas}");
```

```saida
Recuperar: Retomar o que for possível da rotina, no seu ritmo.
Adaptar: Procurar outro caminho para chegar a um objetivo parecido.
BuscarApoio: Contar com alguém de confiança para atravessar junto.
7 h anotadas.
Não entendi o número. Tente algo como 7.
Esse valor não cabe em um dia. Use de 0 a 24.
Tentativas: 3
```

Repare que o programa **não escolheu** um caminho por Bia ou por Caio: mostrou o que existe. E que `"sete"` e `"40"` receberam respostas diferentes, porque **falhas diferentes pedem cuidados diferentes**.

## 3) Testes: frases que o computador confere

Um teste descreve, em código, o que esperamos: **dado** isto, **quando** fazemos aquilo, **então** deve acontecer isto. Usamos o **xUnit**: `[Fact]` marca um teste simples; `[Theory]` com `[InlineData]` roda o mesmo teste com vários valores.

```cs Resiliencia.Testes/Cap88Tests.cs
using Jornada.Resiliencia.Cap88;

namespace Resiliencia.Testes;

public class Cap88Tests
{
    [Fact]
    public void Adaptar_esta_sempre_entre_os_caminhos()
    {
        var d = new Dificuldade("Perdi o emprego", RotinaAindaPossivel: false, TemAlguemDeConfianca: false);
        Assert.Equal(new[] { Resposta.Adaptar }, Caminhos.Possiveis(d));
    }

    [Fact]
    public void Os_tres_caminhos_podem_aparecer_juntos()
    {
        var d = new Dificuldade("Fui mal na prova", true, true);
        Assert.Equal(new[] { Resposta.Recuperar, Resposta.Adaptar, Resposta.BuscarApoio }, Caminhos.Possiveis(d));
    }

    [Theory]
    [InlineData(Resposta.Recuperar)]
    [InlineData(Resposta.Adaptar)]
    [InlineData(Resposta.BuscarApoio)]
    public void Todo_caminho_tem_descricao(Resposta r) =>
        Assert.False(string.IsNullOrWhiteSpace(Caminhos.Descrever(r)));

    [Theory]
    [InlineData("7", "7 h anotadas.")]
    [InlineData("sete", "Não entendi o número. Tente algo como 7.")]
    [InlineData("40", "Esse valor não cabe em um dia. Use de 0 a 24.")]
    public void As_duas_versoes_dao_a_mesma_resposta(string entrada, string esperado)
    {
        Assert.Equal(esperado, new Anotador().AnotarHorasDeSono(entrada));
        Assert.Equal(esperado, new Anotador().AnotarComTryParse(entrada));
    }

    [Fact]
    public void Finally_conta_a_tentativa_mesmo_quando_falha()
    {
        var a = new Anotador();
        a.AnotarHorasDeSono("7");
        a.AnotarHorasDeSono("oi");
        a.AnotarHorasDeSono("99");
        Assert.Equal(3, a.Tentativas);
    }
}
```

Dois testes merecem atenção. **`As_duas_versoes_dao_a_mesma_resposta`** mostra que podemos trocar a implementação (exceções por `TryParse`) e provar que o **comportamento** continua o mesmo. E **`Finally_conta_a_tentativa_mesmo_quando_falha`** confirma que o `finally` roda também nos caminhos de erro.

::: dica Nomes de testes são frases
`Adaptar_esta_sempre_entre_os_caminhos` lê-se como uma regra do livro. Quando um teste falha, o nome já diz **o que foi prometido**. Escrever bons nomes é parte do cuidado com quem vai ler o código depois, inclusive você daqui a seis meses.
:::

::: exercicio Ler uma nota sem exceção
Escreva `Notas.LerNota(string? texto)`, que devolve `int?` (um inteiro que **pode ser nulo**): o número, se o texto for uma nota válida de **0 a 10**, ou `null` em qualquer outro caso (texto vazio, letras, `11`, `-1`). Use `TryParse` e **nenhum `try/catch`**.

--- resposta
```cs Jornada/Resiliencia/Exercicios/Cap88Notas.cs
namespace Jornada.Resiliencia.Cap88;

public static class Notas
{
    /// <summary>Lê uma nota de 0 a 10 sem lançar exceção. Devolve null quando não dá.</summary>
    public static int? LerNota(string? texto) =>
        int.TryParse(texto, out int nota) && nota is >= 0 and <= 10
            ? nota
            : null;
}
```

`int?` é um `int` que aceita `null`, a forma honesta de dizer "pode não haver resposta". O `?:` devolve `nota` ou `null` (o compilador converte para `int?` porque o método declara esse retorno), e `nota is >= 0 and <= 10` é um **padrão relacional** que combina as duas condições. Quem chama decide o que fazer com o `null`: pedir de novo, pular, usar um valor padrão.
:::

::: resumo
- **Resiliência** é a possibilidade de responder a uma dificuldade com os recursos disponíveis (inclusive o apoio de outras pessoas), **recuperando, adaptando ou ambos**. Não é "voltar ao que era" nem "nunca se abalar".
- Em C#, um **`enum`** nomeia opções fechadas, um **`record`** guarda dados imutáveis e o **`switch` expression** transforma cada valor em um resultado, com o compilador vigiando os casos.
- **`try / catch / finally`** trata falhas sem derrubar o programa; o `finally` roda **sempre**.
- Para erros **esperados** (entrada de pessoas), prefira **`TryParse`**; guarde as exceções para o **inesperado**.
- **Testes** com xUnit (`[Fact]`, `[Theory]`) são frases executáveis sobre o comportamento prometido.
:::

::: quiz
? O que melhor descreve resiliência neste livro?
- Nunca se abalar com nada
+ Responder a uma dificuldade com os recursos disponíveis, recuperando, adaptando ou contando com apoio
- Voltar sempre exatamente ao estado anterior
- Resolver tudo sozinho e depressa
> Há recuperação, adaptação e apoio, e nenhum deles é obrigatório nem uma medida de valor.

? Por que `Caminhos.Possiveis` devolve uma lista em vez de uma única resposta?
+ Porque o programa organiza possibilidades; quem decide é a pessoa
- Porque o C# não permite devolver só um valor
- Porque listas são mais rápidas
- Porque o `enum` exige isso
> O modelo oferece opções, não escolhe pela pessoa.

? Em qual caso o bloco `finally` é executado?
- Só quando ocorre uma exceção
- Só quando tudo dá certo
+ Sempre, tenha ocorrido exceção ou não
- Nunca, é apenas decorativo
> Ele serve para o que precisa acontecer de qualquer jeito, como fechar recursos.

? Quando preferir `int.TryParse` a `int.Parse` com `try/catch`?
+ Quando a falha é esperada, como texto digitado por uma pessoa
- Quando o arquivo some do disco
- Quando o programa tem poucos usuários
- Nunca; `Parse` é sempre melhor
> Erro previsível é parte do fluxo normal; exceção é para o inesperado.

? O que o `_ => throw ...` no fim de um `switch` faz?
+ Avisa cedo se surgir um valor de `enum` que ninguém tratou
- Apaga os outros casos
- Converte o valor em texto
- Faz o programa sempre falhar
> Ele é a rede de segurança contra casos esquecidos.

? Um momento de bloqueio diante de uma dificuldade prova falta de resiliência?
- Sim, sem exceção
+ Não: cansaço, pressão e falta de apoio também contam
- Sim, se durar mais de um dia
- Só se a pessoa estiver sozinha
> O contexto importa, e a palavra "possibilidade" está na definição por isso.
:::

::: cartas
Resiliência :: Possibilidade de responder a uma dificuldade com os recursos disponíveis, recuperando, adaptando ou buscando apoio.
enum :: Tipo que nomeia um conjunto fechado de opções.
record :: Tipo para dados que não mudam depois de criados.
switch expression :: Forma de devolver um valor para cada caso; o compilador avisa casos esquecidos.
try / catch / finally :: Tenta, trata a falha por tipo e executa o `finally` sempre.
TryParse :: Converte sem lançar exceção: devolve `bool` e entrega o valor por `out`.
int? :: `int` que também pode ser `null` (sem valor).
Teste (xUnit) :: Frase executável sobre o comportamento esperado do código.
:::
