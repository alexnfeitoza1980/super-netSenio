---
id: foco-cap66
order: 88
kind: capitulo
num: 66
group: C# & Mentalidade · Parte 13 · Foco em C#
title: Sensibilidade social: contexto, tom e latência de review
subtitle: Ler o contexto é parte do foco no outro. Adaptar a mensagem ao canal e à pessoa, e medir quanto tempo as pessoas esperam por você, em especial no code review, onde poder e atenção se cruzam.
minutes: 50
hue: 305
objectives:
- Entender a **sensibilidade social** como leitura de **contexto** e de **tom**
- Adaptar uma **mensagem técnica** ao canal e ao destinatário em C#
- Medir a **latência de review** (tempo de espera) por pessoa, de forma justa
- Perceber como **hierarquia** e **atenção** aparecem em métricas de espera
- Usar as métricas para **conversar**, não para punir
origem: livros/04-foco-em-csharp/content/98-cap66.md
---

::: vila O comentário seco
Caio escreve no *pull request* de uma estagiária: "Isso está errado. Refaz." A estagiária não responde por três dias. Lia lê o comentário e pergunta: "Se um colega dissesse isso a você, com a sua experiência, como você leria?". Depois olha o painel da equipe e percebe outra coisa: **os PRs de quem está em início de carreira esperam o dobro do tempo** por uma revisão.
:::

::: ilustracao img/foco/cap66.svg | Mesmo conteúdo, forma diferente: ler o contexto é adaptar o tom, sem trocar a verdade.
:::

## O que o livro diz

> **Sensibilidade social** é perceber o que o outro sente (**sintonia**), **entender o contexto** e saber como agir. Sem ela, mensagens corretas aterrissam mal.

Pontos do capítulo:

- Antes de agir, é preciso **ler o contexto**: quem são as pessoas, qual o canal, que regras (explícitas ou não) valem, o que está em jogo.
- Pessoas muito sensíveis ao contexto parecem "camaleões" no bom sentido: **mudam o tom**, não os valores.
- O **poder** reduz a atenção ao outro; um sinal medível é o **tempo de resposta**: quanto mais baixo na hierarquia, mais se espera.
- Em grupo, **quem fala primeiro** e **quem é ouvido** reflete a hierarquia; boas equipes **distribuem** a palavra.
- A **sintonia** (rapport) depende de sincronia no tom e na atenção, e vem de **estar presente**.

::: cuidado Métrica não é juiz
Medir quanto cada pessoa espera é útil para **perceber padrões**, mas pode virar **arma** se usado para ranquear e punir. E o tempo de espera depende de fatores legítimos (tamanho do PR, urgência, fuso horário). Os números iniciam **conversas**, não as encerram.
:::

## Da vida para o código

| No livro | Em engenharia de software |
|---|---|
| Ler o contexto | **Adaptar** a mensagem ao canal e ao destinatário |
| Mudar o tom, não os valores | Mesmo feedback técnico, **forma** diferente |
| Tempo de resposta e poder | **Latência de review** por pessoa/nível |
| Distribuir a palavra | **Rotacionar** quem revisa e quem fala nas reuniões |
| Sintonia | **Atenção real** ao outro no pareamento |

::: figura Mesma mensagem, três contextos
<svg viewBox="0 0 740 210" role="img" aria-label="Uma mensagem técnica passa por um adaptador e sai em três formas: PR, chat e incidente">
  <rect class="f-plain" x="20" y="80" width="150" height="50" rx="10"/>
  <text x="95" y="110" text-anchor="middle" font-weight="800">"A query é lenta"</text>
  <path class="arrow" d="M172 105L250 105"/>
  <rect class="f-warn" x="252" y="70" width="170" height="70" rx="12"/>
  <text x="337" y="100" text-anchor="middle" font-weight="800">Adaptador</text>
  <text x="337" y="120" text-anchor="middle" font-size="11" class="t-dim">canal · pessoa · urgência</text>
  <path class="arrow" d="M424 90L510 40"/><path class="arrow" d="M424 105L510 105"/><path class="arrow" d="M424 120L510 170"/>
  <rect class="f-info" x="512" y="18" width="210" height="44" rx="8"/><text x="617" y="38" text-anchor="middle" font-size="12" font-weight="800">PR (colega júnior)</text><text x="617" y="54" text-anchor="middle" font-size="11">com contexto e sugestão</text>
  <rect class="f-info" x="512" y="84" width="210" height="44" rx="8"/><text x="617" y="104" text-anchor="middle" font-size="12" font-weight="800">Chat (par)</text><text x="617" y="120" text-anchor="middle" font-size="11">curta e direta</text>
  <rect class="f-bad" x="512" y="148" width="210" height="44" rx="8"/><text x="617" y="168" text-anchor="middle" font-size="12" font-weight="800">Incidente</text><text x="617" y="184" text-anchor="middle" font-size="11">só fatos e próximo passo</text>
</svg>
:::

## 1) Adaptar a mensagem ao contexto

```cs Jornada/Foco/Cap66/Mensagem.cs
namespace Jornada.Foco.Cap66;

public enum Canal { PullRequest, Chat, Incidente }
public enum Experiencia { Iniciante, Pleno, Senior }

public sealed record Contexto(Canal Canal, Experiencia Destinatario);

public static class AdaptadorDeMensagem
{
    /// <param name="problema">O fato técnico, sem julgamento. Ex.: "a query faz N+1 consultas".</param>
    /// <param name="sugestao">O próximo passo concreto. Ex.: "usar Include para carregar os itens".</param>
    public static string Adaptar(string problema, string sugestao, Contexto contexto) => contexto switch
    {
        { Canal: Canal.Incidente } =>
            $"Fato: {problema}. Próximo passo: {sugestao}.",

        { Canal: Canal.Chat } =>
            $"Acho que {problema} — que tal {sugestao}?",

        { Canal: Canal.PullRequest, Destinatario: Experiencia.Iniciante } =>
            $"Boa evolução! Um ponto para aprender: {problema}. Uma ideia é {sugestao}. " +
            "Se quiser, a gente vê junto em um pareamento.",

        { Canal: Canal.PullRequest } =>
            $"Observação: {problema}. Sugestão: {sugestao}.",

        _ => throw new ArgumentOutOfRangeException(nameof(contexto)),
    };
}
```

```cs Uso
const string problema = "a consulta faz uma busca por item (N+1)";
const string sugestao = "carregar os itens de uma vez com Include";

var contextos = new[]
{
    new Contexto(Canal.PullRequest, Experiencia.Iniciante),
    new Contexto(Canal.PullRequest, Experiencia.Senior),
    new Contexto(Canal.Chat,        Experiencia.Pleno),
    new Contexto(Canal.Incidente,   Experiencia.Pleno),
};

foreach (var c in contextos)
{
    Console.WriteLine($"[{c.Canal} · {c.Destinatario}]");
    Console.WriteLine("  " + AdaptadorDeMensagem.Adaptar(problema, sugestao, c));
}
```

```saida
[PullRequest · Iniciante]
  Boa evolução! Um ponto para aprender: a consulta faz uma busca por item (N+1). Uma ideia é carregar os itens de uma vez com Include. Se quiser, a gente vê junto em um pareamento.
[PullRequest · Senior]
  Observação: a consulta faz uma busca por item (N+1). Sugestão: carregar os itens de uma vez com Include.
[Chat · Pleno]
  Acho que a consulta faz uma busca por item (N+1) — que tal carregar os itens de uma vez com Include?
[Incidente · Pleno]
  Fato: a consulta faz uma busca por item (N+1). Próximo passo: carregar os itens de uma vez com Include.
```

Repare: o **conteúdo técnico** é o mesmo nas quatro; o que muda é o **tom e a moldura**. A regra de ouro é separar o **fato** ("a consulta faz N+1") do **julgamento** ("isso está errado"): o fato pode ser verificado; o julgamento só gera defesa.

::: cuidado Tom não substitui conteúdo
Adaptar o tom **não** é suavizar a verdade nem adular. O iniciante **precisa saber** que há um N+1; o que muda é a forma de ele **ouvir**. Se a mensagem fica tão macia que o problema some, o adaptador falhou.
:::

## 2) Latência de review por pessoa

Quanto tempo cada autor espera pela **primeira resposta**? Calculamos a **mediana** (resistente a casos extremos) por **nível de experiência**:

```cs Jornada/Foco/Cap66/LatenciaDeReview.cs
namespace Jornada.Foco.Cap66;

public sealed record PullRequestRevisado(string Autor, Experiencia Nivel, double HorasAtePrimeiraResposta);

public sealed record LatenciaPorNivel(Experiencia Nivel, int Quantidade, double MedianaEmHoras);

public static class AnaliseDeLatencia
{
    public static double Mediana(IReadOnlyList<double> valores)
    {
        if (valores.Count == 0) throw new ArgumentException("Sem valores.", nameof(valores));
        var ordenados = valores.OrderBy(v => v).ToList();
        int meio = ordenados.Count / 2;
        return ordenados.Count % 2 == 1
            ? ordenados[meio]
            : (ordenados[meio - 1] + ordenados[meio]) / 2;
    }

    public static IReadOnlyList<LatenciaPorNivel> PorNivel(IEnumerable<PullRequestRevisado> prs) =>
        prs.GroupBy(p => p.Nivel)
           .OrderBy(g => g.Key)
           .Select(g => new LatenciaPorNivel(
               g.Key, g.Count(), Mediana(g.Select(p => p.HorasAtePrimeiraResposta).ToList())))
           .ToList();

    /// <summary>Razão entre a mediana do nível mais espera e a do que menos espera (1 = igual).</summary>
    public static double RazaoEntreExtremos(IReadOnlyList<LatenciaPorNivel> niveis) =>
        niveis.Max(n => n.MedianaEmHoras) / niveis.Min(n => n.MedianaEmHoras);
}
```

```cs Uso
var prs = new[]
{
    new PullRequestRevisado("Ana",   Experiencia.Iniciante, 20),
    new PullRequestRevisado("Ana",   Experiencia.Iniciante, 30),
    new PullRequestRevisado("Ana",   Experiencia.Iniciante, 26),
    new PullRequestRevisado("Bruno", Experiencia.Pleno,      9),
    new PullRequestRevisado("Bruno", Experiencia.Pleno,     11),
    new PullRequestRevisado("Carla", Experiencia.Senior,     4),
    new PullRequestRevisado("Carla", Experiencia.Senior,     6),
    new PullRequestRevisado("Carla", Experiencia.Senior,     2),
};

var niveis = AnaliseDeLatencia.PorNivel(prs);
foreach (var n in niveis)
    Console.WriteLine($"{n.Nivel,-9} {n.Quantidade} PR(s) · mediana {n.MedianaEmHoras:F1} h");

Console.WriteLine($"Quem espera mais espera {AnaliseDeLatencia.RazaoEntreExtremos(niveis):F1}x o tempo de quem espera menos.");
```

```saida
Iniciante 3 PR(s) · mediana 26,0 h
Pleno     2 PR(s) · mediana 10,0 h
Senior    3 PR(s) · mediana 4,0 h
Quem espera mais espera 6,5x o tempo de quem espera menos.
```

O padrão (quanto **menos poder**, mais espera) é **exatamente** o que o livro descreve com o e-mail. Mas, antes de concluir "a equipe ignora os iniciantes", **faça perguntas**: os PRs de iniciantes são maiores? Há menos revisores disponíveis para eles? Os seniores têm prioridade por incidentes? O dado é o **começo** da investigação.

```cs JornadaTests/Foco/Cap66/Cap66Tests.cs
using Jornada.Foco.Cap66;
using Xunit;

public class Cap66Tests
{
    [Fact]
    public void Mediana_de_quantidade_impar() =>
        Assert.Equal(26, AnaliseDeLatencia.Mediana(new double[] { 20, 30, 26 }));

    [Fact]
    public void Mediana_de_quantidade_par() =>
        Assert.Equal(10, AnaliseDeLatencia.Mediana(new double[] { 9, 11 }));

    [Fact]
    public void Mediana_de_lista_vazia_falha() =>
        Assert.Throws<ArgumentException>(() => AnaliseDeLatencia.Mediana(Array.Empty<double>()));

    [Fact]
    public void Mediana_resiste_a_valor_extremo() =>
        Assert.Equal(5.5, AnaliseDeLatencia.Mediana(new double[] { 4, 5, 6, 500 }));

    [Fact]
    public void Incidente_tem_so_fato_e_proximo_passo()
    {
        var texto = AdaptadorDeMensagem.Adaptar("a API caiu", "reiniciar o serviço", new Contexto(Canal.Incidente, Experiencia.Pleno));
        Assert.StartsWith("Fato:", texto);
        Assert.DoesNotContain("Boa evolução", texto);
    }

    [Fact]
    public void Iniciante_em_pr_recebe_convite_ao_pareamento()
    {
        var texto = AdaptadorDeMensagem.Adaptar("x", "y", new Contexto(Canal.PullRequest, Experiencia.Iniciante));
        Assert.Contains("pareamento", texto);
    }

    [Fact]
    public void Razao_entre_extremos()
    {
        var niveis = new[]
        {
            new LatenciaPorNivel(Experiencia.Iniciante, 1, 20),
            new LatenciaPorNivel(Experiencia.Senior, 1, 5),
        };
        Assert.Equal(4, AnaliseDeLatencia.RazaoEntreExtremos(niveis));
    }
}
```

::: exercicio Ver o dado completo
A mediana esconde os casos longos. Escreva `double Percentil90(IReadOnlyList<double> valores)` (método do "mais próximo": ordene e pegue o elemento de índice `ceil(0,9 × n) − 1`) e um teste para a lista `1..10`, em que o resultado deve ser `9`.

--- resposta
```cs Jornada/Foco/Cap66/Percentil.cs
namespace Jornada.Foco.Cap66;

public static class Percentil
{
    public static double Percentil90(IReadOnlyList<double> valores)
    {
        if (valores.Count == 0) throw new ArgumentException("Sem valores.", nameof(valores));
        var ordenados = valores.OrderBy(v => v).ToList();
        int indice = (int)Math.Ceiling(0.9 * ordenados.Count) - 1;
        return ordenados[indice];
    }
}
```
```cs Jornada/Foco/Cap66/PercentilTests.cs
using Jornada.Foco.Cap66;
using Xunit;

public class PercentilTests
{
    [Fact]
    public void De_um_a_dez_o_p90_e_nove() =>
        Assert.Equal(9, Percentil.Percentil90(Enumerable.Range(1, 10).Select(i => (double)i).ToList()));

    [Fact]
    public void Um_unico_valor_e_ele_mesmo() =>
        Assert.Equal(7, Percentil.Percentil90(new double[] { 7 }));
}
```
Em operação, olhar **mediana e P90** juntos mostra o caso típico **e** o pior caso comum: quem espera três dias por uma revisão está no P90, não na mediana.
:::

::: resumo
- **Sensibilidade social** = ler contexto e tom; mude a **forma**, não o **conteúdo** nem os valores.
- Separe **fato** de **julgamento** em feedback técnico; adapte à pessoa e ao canal sem suavizar a verdade.
- **Latência de review por nível** (com **mediana**, não média) pode revelar padrões ligados a **hierarquia e atenção**.
- Métricas abrem **conversas**: investigue tamanho do PR, disponibilidade e prioridades antes de concluir.
- Use **mediana e P90** juntos para ver o caso típico e o pior comum.
:::

::: quiz
? O que significa mudar o tom sem mudar os valores?
+ Adaptar a forma da mensagem ao contexto mantendo a verdade técnica
- Dizer o que cada um quer ouvir
- Omitir problemas
- Responder só em chat
> Contexto muda a forma, não o conteúdo.

? Por que separar fato de julgamento em um comentário de PR?
+ O fato é verificável; o julgamento gera defesa
- Porque julgamentos são proibidos por lei
- Para escrever menos
- Porque o fato é sempre mais longo
> "A consulta faz N+1" versus "isso está errado".

? Por que usar mediana e não média para a latência?
+ A mediana resiste a valores extremos
- Porque é mais rápida de calcular
- Porque a média não existe
- Porque dá sempre o mesmo número
> Um PR parado por semanas distorce a média.

? Qual o risco de usar a latência de review para ranquear pessoas?
+ Pode virar arma e ignorar fatores legítimos, como tamanho do PR e fuso horário
- Nenhum
- Ela sempre está errada
- Ela só vale para seniores
> Métrica não é juiz.

? O que o padrão "iniciantes esperam mais" sugere?
+ Um ponto de investigação sobre hierarquia e atenção, não uma conclusão automática
- Que iniciantes escrevem pior
- Que não se deve revisar PR
- Que o relógio está errado
> Perguntas antes de conclusões.

? Para que olhar o P90 além da mediana?
+ Ver o pior caso comum, não só o típico
- Para aumentar a mediana
- Para evitar relatórios
- Para esconder dados
> Quem espera três dias está no P90.
:::

::: cartas
Sensibilidade social :: Perceber o outro, o contexto e agir de forma adequada.
Sintonia (rapport) :: Conexão baseada em atenção e tom compatíveis.
Fato × julgamento :: O que se pode verificar × o que se opina sobre isso.
Latência de review :: Tempo até a primeira resposta a um pull request.
Mediana :: Valor do meio, resistente a extremos.
Percentil 90 :: Valor abaixo do qual estão 90% dos casos.
Adaptador de mensagem :: Código que escolhe a forma conforme canal e pessoa.
Hierarquia e atenção :: Quem tem menos poder tende a esperar mais por resposta.
:::
