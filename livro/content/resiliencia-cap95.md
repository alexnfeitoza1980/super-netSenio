---
id: resiliencia-cap95
order: 121
kind: capitulo
num: 95
group: C# & Mentalidade · Parte 15 · Resiliência em C#
title: Resiliência não é sorrir por obrigação
subtitle: Felicidade, acolhimento e limites: por que ninguém precisa fingir que está bem. Em C#, é o capítulo do Resultado<T>: devolver o que aconteceu em vez de lançar exceções para o que é esperado.
minutes: 65
hue: 350
objectives:
- Entender que **resiliência não é felicidade obrigatória** nem aceitar abuso
- Criar um **`Resultado<T>`** genérico com `Ok`, `Falha` e `Combinar`
- Tratar entradas inesperadas **sem exceções** e **sem cobrar** nenhuma resposta
- Montar um **limite pessoal** validando vários campos e listando **tudo** o que falta de uma vez
origem: livros/06-resiliencia-csharp/content/132-cap95.md
---

::: vila O sorriso da foto
Na foto da formatura, todo mundo sorria, inclusive Caio, que tinha passado a noite anterior sem dormir, com medo do emprego novo. Uma colega comentou: "nossa, você parece tão bem!". Ele quase respondeu "estou ótimo". Em vez disso, disse: "estou feliz com a formatura e nervoso com o que vem". A colega respondeu: "pode ser as duas coisas, né?". Caio sentiu o peito aliviar.
:::

::: ilustracao img/resiliencia/cap95.svg | Uma janela aberta não faz a tempestade desaparecer. Acolher é deixar o que se sente ter lugar, sem fingir e sem se cobrar.
:::

## O que o livro diz

O texto sobre resiliência também trata da relação entre resiliência e **bem-estar**. Na nossa leitura, o ponto é este: pessoas resilientes **também** sentem tristeza, raiva, medo e cansaço. Resiliência **não** é um sorriso permanente, nem "dar conta de tudo sem incomodar ninguém". Ela convive com dias difíceis, e convive **melhor** quando há espaço para dizer a verdade.

::: analogia A janela pode ficar aberta sem que a tempestade desapareça
Abrir a janela em um dia de temporal não acaba com o temporal; só deixa o ar circular. Reconhecer o que se sente **não resolve** o problema sozinho, mas **tira a pressão** de fingir. E, do lado de dentro, dá para pensar com mais clareza no que fazer.
:::

::: cuidado Acolhimento não é aceitar abuso
Resiliência **não significa** suportar desrespeito, humilhação ou violência. "Aguentar" situações que machucam não é virtude: é um sinal para **buscar proteção e apoio**. Colocar um **limite** ("assim, não") é uma forma de cuidado consigo, e vale ser dito com clareza e calma. Se houver risco, procure ajuda: **CVV 188** (apoio emocional, 24 h) ou **SAMU 192** (emergência).
:::

## Da vida para o código

| Na vida | Em C# |
|---|---|
| "Como você está?", com a opção **"prefiro não dizer"** | `enum RespostaDoCheckIn` com `PreferoNaoDizer` como resposta **de primeira classe** |
| Silêncio também é resposta | `Ler(null)` devolve **sucesso**, não erro |
| Entrada estranha ("talvez") | **Falha gentil**, sem exceção |
| Um resultado que pode ser sucesso **ou** falha | **`Resultado<T>`** genérico |
| Obrigar quem usa a tratar **os dois** casos | Método **`Combinar`** |
| Acolher sem cobrar positividade | Mensagens testadas: **nenhuma** contém "sorria" ou "seja forte" |
| Um limite claro: situação, impacto, pedido | `Limite.Criar` valida e lista **tudo** o que falta |

::: figura Check-in: todas as respostas são bem-vindas
<svg viewBox="0 0 740 220" role="img" aria-label="Quatro respostas possíveis a como você está, cada uma com uma resposta acolhedora, inclusive prefiro não dizer">
  <g font-size="12" text-anchor="middle">
    <rect x="20" y="30" width="160" height="70" rx="12" class="f-good"/><text x="100" y="56" font-weight="800">1 · Bem</text><text x="100" y="78" class="t-dim">"Quer anotar o que ajudou?"</text>
    <rect x="195" y="30" width="160" height="70" rx="12" class="f-info"/><text x="275" y="56" font-weight="800">2 · Mais ou menos</text><text x="275" y="78" class="t-dim">"Dias assim existem."</text>
    <rect x="370" y="30" width="160" height="70" rx="12" class="f-warn"/><text x="450" y="56" font-weight="800">3 · Não estou bem</text><text x="450" y="78" class="t-dim">"Posso mostrar quem apoia."</text>
    <rect x="545" y="30" width="175" height="70" rx="12" class="f-prim"/><text x="632" y="56" font-weight="800">4 · Prefiro não dizer</text><text x="632" y="78" class="t-dim">"A porta fica aberta."</text>
  </g>
  <text x="370" y="140" text-anchor="middle" font-size="14" font-weight="800">Em branco = também vale (vira "prefiro não dizer")</text>
  <text x="370" y="166" text-anchor="middle" font-size="13" class="t-dim">Nenhuma resposta recebe "sorria", "seja forte" ou "supere".</text>
  <text x="370" y="192" text-anchor="middle" font-size="13" class="t-dim">Entrada que o programa não entendeu vira uma pergunta gentil, e não uma tela de erro.</text>
</svg>
:::

## 1) `Resultado<T>`: devolver o que aconteceu

No capítulo 88, vimos que **exceções** servem para o inesperado. Para falhas **esperadas** (uma entrada que não faz sentido), outra técnica elegante é devolver um **resultado** que diz "deu certo, aqui está o valor" **ou** "não deu, aqui está o porquê". Essa ideia vira um tipo **genérico**:

```cs Jornada/Resiliencia/Cap95/Resultado.cs
namespace Jornada.Resiliencia.Cap95;

/// <summary>Em vez de lançar exceção para uma falha esperada, devolvemos o que aconteceu.</summary>
public sealed class Resultado<T>
{
    private readonly T? valor;
    private readonly string? erro;

    private Resultado(bool deu, T? valor, string? erro)
    {
        Deu = deu;
        this.valor = valor;
        this.erro = erro;
    }

    public bool Deu { get; }

    public static Resultado<T> Ok(T valor) => new(true, valor, null);
    public static Resultado<T> Falha(string erro) => new(false, default, erro);

    /// <summary>Obriga quem usa a decidir os dois caminhos: o que fazer se deu e o que fazer se não deu.</summary>
    public TSaida Combinar<TSaida>(Func<T, TSaida> seDeu, Func<string, TSaida> seFalhou) =>
        Deu ? seDeu(valor!) : seFalhou(erro!);
}
```

Três escolhas de projeto:

- O construtor é **privado**: só se cria um `Resultado` por `Ok` ou `Falha`. Isso evita estados impossíveis ("deu certo, mas sem valor").
- `Combinar` **obriga** quem usa a tratar **os dois** caminhos: sem esquecer o erro. Você passa duas funções, uma para o sucesso e outra para a falha, e recebe de volta **um valor do mesmo tipo**.
- O `!` em `valor!` (e `erro!`) diz ao compilador "eu sei que aqui não é nulo", porque os dois só são lidos no ramo certo.

## 2) Um check-in que não cobra nada

`CheckIn.Ler` traduz o que a pessoa escreveu em uma `RespostaDoCheckIn`. Três regras, todas de cuidado: **silêncio é resposta válida** (vira "prefiro não dizer"); **"prefiro não dizer" tem número próprio**, sem constrangimento; e uma entrada que o programa não entende vira uma **falha gentil**, que **ensina** o formato em vez de repreender.

```cs Jornada/Resiliencia/Cap95/CheckIn.cs
namespace Jornada.Resiliencia.Cap95;

public enum RespostaDoCheckIn { Bem = 1, MaisOuMenos = 2, NaoEstouBem = 3, PreferoNaoDizer = 4 }

public static class CheckIn
{
    /// <summary>Lê a resposta. Silêncio é uma resposta válida: nunca obrigamos ninguém a dizer algo.</summary>
    public static Resultado<RespostaDoCheckIn> Ler(string? texto)
    {
        if (string.IsNullOrWhiteSpace(texto))
            return Resultado<RespostaDoCheckIn>.Ok(RespostaDoCheckIn.PreferoNaoDizer);

        return texto.Trim() switch
        {
            "1" => Resultado<RespostaDoCheckIn>.Ok(RespostaDoCheckIn.Bem),
            "2" => Resultado<RespostaDoCheckIn>.Ok(RespostaDoCheckIn.MaisOuMenos),
            "3" => Resultado<RespostaDoCheckIn>.Ok(RespostaDoCheckIn.NaoEstouBem),
            "4" => Resultado<RespostaDoCheckIn>.Ok(RespostaDoCheckIn.PreferoNaoDizer),
            _   => Resultado<RespostaDoCheckIn>.Falha("Escolha de 1 a 4. Você também pode deixar em branco.")
        };
    }

    public static string Acolher(RespostaDoCheckIn r) => r switch
    {
        RespostaDoCheckIn.Bem            => "Que bom saber. Se quiser, anote o que ajudou.",
        RespostaDoCheckIn.MaisOuMenos    => "Dias assim existem. Quer só anotar ou também pensar num passo pequeno?",
        RespostaDoCheckIn.NaoEstouBem    => "Obrigado por dizer. Você não precisa resolver isso agora. Posso mostrar quem pode apoiar.",
        RespostaDoCheckIn.PreferoNaoDizer => "Tudo bem. A porta fica aberta para quando você quiser.",
        _                                => throw new ArgumentOutOfRangeException(nameof(r))
    };
}
```

## 3) Um limite dito com clareza

Um limite saudável conta **o que acontece**, **como isso afeta** e **o que se pede**, sem rótulos sobre a outra pessoa. O `Limite.Criar` valida os três campos e, se faltar algo, **lista tudo de uma vez** ("Falta: ...; ..."), em vez de reclamar um campo por vez. Quem preenche não precisa tentar três vezes para descobrir o que falta.

```cs Jornada/Resiliencia/Cap95/Limite.cs
namespace Jornada.Resiliencia.Cap95;

/// <summary>Um limite pessoal dito com clareza: o que acontece, como afeta e o que se pede.</summary>
public sealed record Limite(string Situacao, string Impacto, string Pedido)
{
    public static Resultado<Limite> Criar(string? situacao, string? impacto, string? pedido)
    {
        var problemas = new List<string>();

        if (string.IsNullOrWhiteSpace(situacao)) problemas.Add("descreva o que acontece (sem rótulos sobre a pessoa)");
        if (string.IsNullOrWhiteSpace(impacto))  problemas.Add("diga como isso afeta você");
        if (string.IsNullOrWhiteSpace(pedido))   problemas.Add("faça um pedido concreto");

        return problemas.Count == 0
            ? Resultado<Limite>.Ok(new Limite(situacao!.Trim(), impacto!.Trim(), pedido!.Trim()))
            : Resultado<Limite>.Falha("Falta: " + string.Join("; ", problemas) + ".");
    }

    public string Montar() =>
        $"Quando {Situacao}, eu me sinto {Impacto}. Peço que {Pedido}.";
}
```

```cs Uso
foreach (var entrada in new[] { "2", "", "talvez", "3" })
{
    string linha = CheckIn.Ler(entrada).Combinar(
        resposta => $"{resposta}: {CheckIn.Acolher(resposta)}",
        erro => "(entrada não reconhecida) " + erro);

    Console.WriteLine($"[{entrada}] {linha}");
}

var limite = Limite.Criar("as mensagens chegam depois das 22h", "sem tempo para descansar",
                          "combinemos responder só até as 20h");
Console.WriteLine(limite.Combinar(l => l.Montar(), erro => erro));

Console.WriteLine(Limite.Criar("algo acontece", null, "").Combinar(l => l.Montar(), erro => erro));
```

```saida
[2] MaisOuMenos: Dias assim existem. Quer só anotar ou também pensar num passo pequeno?
[] PreferoNaoDizer: Tudo bem. A porta fica aberta para quando você quiser.
[talvez] (entrada não reconhecida) Escolha de 1 a 4. Você também pode deixar em branco.
[3] NaoEstouBem: Obrigado por dizer. Você não precisa resolver isso agora. Posso mostrar quem pode apoiar.
Quando as mensagens chegam depois das 22h, eu me sinto sem tempo para descansar. Peço que combinemos responder só até as 20h.
Falta: diga como isso afeta você; faça um pedido concreto.
```

Veja a primeira linha da saída: `[2]` recebeu um acolhimento sem cobrança; `[]` (vazio) virou "prefiro não dizer", **sem erro**; `talvez` virou uma pergunta gentil; e `3` mostrou a resposta mais cuidadosa, que **oferece** apoio, sem impor.

## 4) Testes

```cs Resiliencia.Testes/Cap95Tests.cs
using Jornada.Resiliencia.Cap95;

namespace Resiliencia.Testes;

public class Cap95Tests
{
    [Theory]
    [InlineData("1", RespostaDoCheckIn.Bem)]
    [InlineData("2", RespostaDoCheckIn.MaisOuMenos)]
    [InlineData(" 3 ", RespostaDoCheckIn.NaoEstouBem)]
    [InlineData("4", RespostaDoCheckIn.PreferoNaoDizer)]
    public void Numeros_de_1_a_4_sao_lidos(string entrada, RespostaDoCheckIn esperado)
    {
        var r = CheckIn.Ler(entrada).Combinar(valor => valor, _ => throw new Exception("não era para falhar"));
        Assert.Equal(esperado, r);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Silencio_e_uma_resposta_valida(string? entrada)
    {
        var r = CheckIn.Ler(entrada).Combinar(valor => valor, _ => throw new Exception("silêncio não é erro"));
        Assert.Equal(RespostaDoCheckIn.PreferoNaoDizer, r);
    }

    [Fact]
    public void Entrada_estranha_vira_uma_falha_gentil()
    {
        var mensagem = CheckIn.Ler("talvez").Combinar(_ => "", erro => erro);
        Assert.Contains("1 a 4", mensagem);
    }

    [Theory]
    [InlineData(RespostaDoCheckIn.Bem)]
    [InlineData(RespostaDoCheckIn.MaisOuMenos)]
    [InlineData(RespostaDoCheckIn.NaoEstouBem)]
    [InlineData(RespostaDoCheckIn.PreferoNaoDizer)]
    public void Nenhum_acolhimento_cobra_positividade(RespostaDoCheckIn resposta)
    {
        var texto = CheckIn.Acolher(resposta).ToLowerInvariant();

        foreach (var palavra in new[] { "sorria", "seja forte", "pense positivo", "supere", "ânimo", "força" })
            Assert.DoesNotContain(palavra, texto);
    }

    [Fact]
    public void Limite_completo_vira_uma_frase_clara()
    {
        var frase = Limite.Criar("as mensagens chegam depois das 22h", "sem tempo para descansar",
                                 "combinemos responder só até as 20h")
                          .Combinar(l => l.Montar(), e => e);

        Assert.Equal(
            "Quando as mensagens chegam depois das 22h, eu me sinto sem tempo para descansar. Peço que combinemos responder só até as 20h.",
            frase);
    }

    [Fact]
    public void Limite_incompleto_lista_tudo_que_falta_de_uma_vez()
    {
        var erro = Limite.Criar("algo acontece", null, "  ").Combinar(_ => "", e => e);

        Assert.Contains("como isso afeta", erro);
        Assert.Contains("pedido concreto", erro);
        Assert.DoesNotContain("o que acontece", erro);
    }
}
```

O teste mais incomum do livro é o **`Nenhum_acolhimento_cobra_positividade`**: ele garante, **em código**, que nenhuma mensagem acolhedora contém "sorria", "seja forte", "pense positivo", "supere", "ânimo" ou "força". Testes não servem só para números: também podem **proteger o tom** do programa.

::: dica `Combinar` nos testes
Em `Numeros_de_1_a_4_sao_lidos`, o segundo argumento de `Combinar` é `_ => throw new Exception("não era para falhar")`: se a leitura falhar, o teste **explode com uma mensagem clara**. Já no teste de falha, é o primeiro argumento que devolve texto vazio. É uma forma compacta de dizer "quero o ramo de sucesso" ou "quero o de falha".
:::

::: exercicio Mapear o sucesso
Escreva o método de extensão `Mapear<T, TNovo>(this Resultado<T> resultado, Func<T, TNovo> transformar)`, que **transforma o valor** quando deu certo e **repassa a mesma falha** quando não deu. Use `Combinar`. Teste com `CheckIn.Ler("1").Mapear(r => r.ToString().ToUpperInvariant())`.

--- resposta
```cs Jornada/Resiliencia/Exercicios/Cap95Mapear.cs
namespace Jornada.Resiliencia.Cap95;

public static class ResultadoExtensoes
{
    /// <summary>Transforma o valor se deu certo; se falhou, repassa a mesma falha.</summary>
    public static Resultado<TNovo> Mapear<T, TNovo>(this Resultado<T> resultado, Func<T, TNovo> transformar) =>
        resultado.Combinar(
            valor => Resultado<TNovo>.Ok(transformar(valor)),
            erro => Resultado<TNovo>.Falha(erro));
}
```

Dois **parâmetros de tipo** (`T` e `TNovo`) deixam o método transformar um `Resultado<RespostaDoCheckIn>` em um `Resultado<string>`, por exemplo. O compilador **infere** os tipos pelo uso, então se escreve só `.Mapear(r => ...)`. Esse padrão (`Map`) existe em várias linguagens porque permite **encadear transformações** sem checar a falha a cada passo.
:::

::: resumo
- Resiliência **não é felicidade obrigatória** nem **aceitar abuso**: reconhecer o que se sente tira a pressão de fingir, e colocar um **limite** é cuidado.
- **`Resultado<T>`** devolve "deu certo (com valor)" ou "não deu (com motivo)" **sem exceções**, ótimo para falhas **esperadas**.
- O **construtor privado** evita estados impossíveis; **`Combinar`** obriga a tratar **os dois** caminhos.
- Um bom check-in **aceita silêncio**, tem "**prefiro não dizer**" e **não cobra positividade**, e testes podem **proteger o tom**.
- Validar **todos** os campos e listar o que falta **de uma vez** respeita o tempo de quem usa.
:::

::: quiz
? Resiliência significa estar sempre sorrindo?
- Sim, quem é resiliente não sofre
+ Não: pessoas resilientes também sentem tristeza, raiva, medo e cansaço
- Sim, em público
- Só quando há plateia
> Reconhecer o que se sente faz parte do caminho.

? Quando um `Resultado<T>` é mais adequado do que lançar uma exceção?
+ Quando a falha é esperada, como uma entrada que não faz sentido
- Quando o disco quebra
- Quando a memória acaba
- Nunca
> Exceções ficam para o inesperado; resultados modelam o esperado.

? Por que o construtor de `Resultado<T>` é privado?
+ Para garantir que só existam resultados válidos, criados por `Ok` ou `Falha`
- Para esconder o código
- Porque classes genéricas exigem isso
- Para economizar memória
> Sem isso, seria possível criar "sucesso sem valor".

? O que o método `Combinar` obriga quem o usa a fazer?
+ Tratar o caminho de sucesso e o de falha
- Ignorar erros
- Converter o resultado em `int`
- Lançar uma exceção
> Você passa duas funções, e o `Resultado` escolhe a certa.

? Por que um texto em branco no check-in vira "prefiro não dizer" e não um erro?
+ Porque o silêncio é uma resposta válida: o programa nunca obriga a dizer algo
- Porque o programa esqueceu de validar
- Porque o teclado falhou
- Porque o C# não aceita erros
> Respeitar o silêncio é parte do cuidado.

? Por que `Limite.Criar` lista tudo o que falta de uma vez?
+ Para a pessoa corrigir tudo em uma só tentativa, sem reclamar campo por campo
- Para deixar a mensagem mais longa
- Porque só pode haver um erro
- Para punir campos vazios
> Respeita o tempo e a energia de quem preenche.
:::

::: cartas
Bem-estar :: Estado em que cabem alegria e também tristeza, medo e cansaço, sem exigência de sorrir sempre.
Acolhimento :: Dar lugar ao que a pessoa sente, sem pressionar, julgar ou resolver por ela.
Limite pessoal :: Fronteira dita com clareza: o que acontece, como afeta e o que se pede.
Resultado<T> :: Tipo que representa sucesso (com valor) ou falha (com motivo), sem exceção.
Combinar :: Método que obriga a tratar o caminho de sucesso e o de falha.
Construtor privado :: Impede criar objetos por fora, forçando fábricas como `Ok` e `Falha`.
Falha esperada :: Erro previsível (como entrada estranha); trate com resultado, não com exceção.
Método Mapear :: Transforma o valor de um resultado que deu certo e repassa a falha.
:::
