---
id: foco-cap59
order: 81
kind: capitulo
num: 59
group: C# & Mentalidade · Parte 13 · Foco em C#
title: Atenção seletiva e o custo das distrações
subtitle: Concentrar é escolher um alvo e inibir o resto. Distrações sensoriais e emocionais, o mito da multitarefa, uma política de interrupções em C# e o cálculo do que cada troca de contexto realmente custa.
minutes: 55
hue: 200
objectives:
- Entender a **atenção seletiva**: escolher um alvo e **inibir** o resto
- Distinguir **distrações sensoriais** de **distrações emocionais** (a "isca" do seu nome)
- Aceitar que **multitarefa é troca rápida**, não divisão, e que cada troca tem custo
- Implementar uma **política de interrupções** (entregar agora, agrupar no resumo, silenciar)
- Estimar o **custo de troca de contexto** com um modelo simples e ajustável
origem: livros/04-foco-em-csharp/content/91-cap59.md
---

::: vila O barulho que Lia não ouve
Numa redação lotada, jornalistas escrevem no prazo, sem pedir silêncio: conversas, telefones, gritos entre mesas. Lia lembra dessa cena do livro quando o chat apita a vinte centímetros do teclado. O barulho em si não a atrapalha; o que a derruba é quando alguém digita o **nome dela**. Dessa vez ela decide fazer o que engenheiros fazem: **modelar o problema**.
:::

::: ilustracao img/foco/cap59.svg | A política de interrupções é um funil: o que atrai nem sempre é o que precisa de você.
:::

## O que o livro diz

> **Atenção seletiva** é a capacidade de mirar um alvo e **ignorar um mar de estímulos**, cada um um possível foco. Quanto mais o foco é interrompido, pior o desempenho — e aprendemos melhor quando a atenção está concentrada.

Alguns pontos importantes do capítulo original:

- Há **dois tipos de distração**: **sensorial** (ruído, luz, margens da página) e **emocional** (sinais carregados: ouvir o próprio nome, uma preocupação persistente). Os emocionais são os mais difíceis de inibir.
- A atenção é um **canal estreito e limitado**; o que chamamos de "dividir a atenção" é, na prática, **trocar rapidamente** de alvo — e a troca **enfraquece** o envolvimento.
- A mente de quem lê **divaga** em uma fração relevante do tempo (20–40% em alguns estudos), e quanto mais divaga, menos compreende.
- A "riqueza de informação cria **pobreza de atenção**" (Herbert Simon, 1977).
- Interagir com o telefone no meio de uma conversa é um **"fora"**: um gesto que diz à outra pessoa "isto aqui não me interessa".

## Da vida para o código

| No livro | Em engenharia de software |
|---|---|
| Atenção seletiva (alvo + inibição do resto) | **Modo foco**: filtrar o que chega |
| Distração sensorial | **Ruído**: bots, notificações genéricas, *badges* |
| Distração emocional (seu nome) | **Menções, "urgente", mensagens de pessoas importantes** |
| Troca rápida em vez de divisão | **Custo de troca de contexto** |
| Abundância de informação = pobreza de atenção | **Resumos periódicos** em vez de fluxo contínuo |
| O "fora" na conversa | **Não olhar o celular** em reuniões e *pair programming* |

::: figura A política de interrupções
<svg viewBox="0 0 740 220" role="img" aria-label="Notificações entram num filtro que as manda para agora, para o resumo ou para o silêncio">
  <g font-size="12">
    <rect class="f-plain" x="10" y="20" width="150" height="34" rx="6"/><text x="85" y="42" text-anchor="middle">chat: @você</text>
    <rect class="f-plain" x="10" y="64" width="150" height="34" rx="6"/><text x="85" y="86" text-anchor="middle">e-mail da semana</text>
    <rect class="f-plain" x="10" y="108" width="150" height="34" rx="6"/><text x="85" y="130" text-anchor="middle">bot de CI</text>
    <rect class="f-plain" x="10" y="152" width="150" height="34" rx="6"/><text x="85" y="174" text-anchor="middle">alerta de produção</text>
  </g>
  <path class="arrow" d="M162 37L270 100"/><path class="arrow" d="M162 81L270 104"/><path class="arrow" d="M162 125L270 108"/><path class="arrow" d="M162 169L270 112"/>
  <rect class="f-warn" x="272" y="70" width="170" height="70" rx="12"/><text x="357" y="100" text-anchor="middle" font-weight="800">Política</text><text x="357" y="120" text-anchor="middle" font-size="11" class="t-dim">modo foco ligado</text>
  <path class="arrow" d="M444 90L560 40"/><path class="arrow" d="M444 105L560 105"/><path class="arrow" d="M444 120L560 170"/>
  <rect class="f-bad" x="562" y="20" width="168" height="40" rx="8"/><text x="646" y="45" text-anchor="middle" font-weight="800">agora</text>
  <rect class="f-info" x="562" y="86" width="168" height="40" rx="8"/><text x="646" y="111" text-anchor="middle" font-weight="800">resumo das 11h</text>
  <rect class="f-plain" x="562" y="152" width="168" height="40" rx="8"/><text x="646" y="177" text-anchor="middle" font-weight="800">silêncio</text>
</svg>
:::

## 1) Uma política de interrupções

Em modo foco, **nem tudo merece interromper**. O que merece? Uma decisão consciente, escrita em um só lugar — em vez de ser tomada, no susto, a cada apito:

```cs Jornada/Foco/Cap59/PoliticaDeInterrupcao.cs
namespace Jornada.Foco.Cap59;

public enum Canal { Chat, Email, Telefone, Alerta }
public enum ModoDeAtencao { Disponivel, Foco }
public enum Decisao { EntregarAgora, AgruparNoResumo, Silenciar }

public sealed record Notificacao(
    string Origem,
    Canal Canal,
    string Texto,
    bool MencionaVoce = false,
    bool Urgente = false);

public sealed class PoliticaDeInterrupcao
{
    private readonly HashSet<string> origensVip;

    public PoliticaDeInterrupcao(IEnumerable<string> origensVip) =>
        this.origensVip = new HashSet<string>(origensVip, StringComparer.OrdinalIgnoreCase);

    public Decisao Decidir(Notificacao n, ModoDeAtencao modo)
    {
        if (modo == ModoDeAtencao.Disponivel)
            return Decisao.EntregarAgora;

        // Em modo foco:
        if (n.Canal == Canal.Alerta && n.Urgente)                    // incidente de verdade: interrompe
            return Decisao.EntregarAgora;

        if (n.Urgente && origensVip.Contains(n.Origem))              // urgente, vindo de quem importa
            return Decisao.EntregarAgora;

        if (n.Origem.StartsWith("bot", StringComparison.OrdinalIgnoreCase))
            return Decisao.Silenciar;                                // ruído sensorial: nem no resumo

        // Menção ao seu nome é a isca emocional: sedutora, mas quase sempre pode esperar o resumo.
        return Decisao.AgruparNoResumo;
    }
}
```

O ponto importante é o último comentário: a **menção ao seu nome** chama sua atenção *automaticamente* (é uma isca emocional), mas isso **não significa que seja urgente**. A política separa o que **atrai** do que **precisa** de você.

## 2) O resumo periódico

Em vez de um fluxo contínuo, receba um **resumo** em horários combinados — o que o livro chama de reduzir a "atenção parcial contínua":

```cs Jornada/Foco/Cap59/ResumoPeriodico.cs
namespace Jornada.Foco.Cap59;

public sealed class ResumoPeriodico
{
    private readonly List<Notificacao> pendentes = new();

    public int Quantidade => pendentes.Count;

    public void Adicionar(Notificacao n) => pendentes.Add(n);

    /// <summary>Esvazia a fila e devolve linhas agrupadas por origem, com as menções primeiro.</summary>
    public IReadOnlyList<string> Esvaziar()
    {
        var linhas = pendentes
            .GroupBy(n => n.Origem)
            .OrderByDescending(g => g.Any(n => n.MencionaVoce))
            .ThenBy(g => g.Key)
            .Select(g =>
            {
                string marca = g.Any(n => n.MencionaVoce) ? "@ " : "  ";
                return $"{marca}{g.Key}: {g.Count()} mensagem(ns) — última: {g.Last().Texto}";
            })
            .ToList();

        pendentes.Clear();
        return linhas;
    }
}
```

```cs Uso
var politica = new PoliticaDeInterrupcao(new[] { "Dona Marta" });
var resumo = new ResumoPeriodico();

var chegando = new[]
{
    new Notificacao("Caio",       Canal.Chat,   "Lia, viu o PR?",              MencionaVoce: true),
    new Notificacao("bot-ci",     Canal.Alerta, "Build #4812 passou"),
    new Notificacao("Dona Marta", Canal.Chat,   "Cliente parado, preciso hoje", MencionaVoce: true, Urgente: true),
    new Notificacao("Bia",        Canal.Email,  "Notas da reunião de ontem"),
    new Notificacao("Monitor",    Canal.Alerta, "API de pagamentos fora do ar", Urgente: true),
    new Notificacao("Caio",       Canal.Chat,   "Ah, e o teste de ontem?",      MencionaVoce: true),
};

foreach (var n in chegando)
{
    var d = politica.Decidir(n, ModoDeAtencao.Foco);
    Console.WriteLine($"{d,-16} ← {n.Origem}: {n.Texto}");
    if (d == Decisao.AgruparNoResumo) resumo.Adicionar(n);
}

Console.WriteLine("--- resumo das 11h ---");
resumo.Esvaziar().ToList().ForEach(Console.WriteLine);
```

```saida
AgruparNoResumo  ← Caio: Lia, viu o PR?
Silenciar        ← bot-ci: Build #4812 passou
EntregarAgora    ← Dona Marta: Cliente parado, preciso hoje
AgruparNoResumo  ← Bia: Notas da reunião de ontem
EntregarAgora    ← Monitor: API de pagamentos fora do ar
AgruparNoResumo  ← Caio: Ah, e o teste de ontem?
--- resumo das 11h ---
@ Caio: 2 mensagem(ns) — última: Ah, e o teste de ontem?
  Bia: 1 mensagem(ns) — última: Notas da reunião de ontem
```

```cs JornadaTests/Foco/Cap59/PoliticaTests.cs
using Jornada.Foco.Cap59;
using Xunit;

public class PoliticaTests
{
    private readonly PoliticaDeInterrupcao politica = new(new[] { "Dona Marta" });

    [Fact]
    public void Disponivel_entrega_tudo() =>
        Assert.Equal(Decisao.EntregarAgora,
            politica.Decidir(new Notificacao("Caio", Canal.Chat, "oi"), ModoDeAtencao.Disponivel));

    [Fact]
    public void Alerta_urgente_interrompe_o_foco() =>
        Assert.Equal(Decisao.EntregarAgora,
            politica.Decidir(new Notificacao("Monitor", Canal.Alerta, "fora do ar", Urgente: true), ModoDeAtencao.Foco));

    [Fact]
    public void Mencao_comum_espera_o_resumo() =>
        Assert.Equal(Decisao.AgruparNoResumo,
            politica.Decidir(new Notificacao("Caio", Canal.Chat, "viu o PR?", MencionaVoce: true), ModoDeAtencao.Foco));

    [Fact]
    public void Bots_sao_silenciados_no_foco() =>
        Assert.Equal(Decisao.Silenciar,
            politica.Decidir(new Notificacao("bot-ci", Canal.Chat, "build ok"), ModoDeAtencao.Foco));
}
```

::: dica Ajuste a política com a equipe
A lista de **origens VIP** e a definição de "urgente" são **acordos de equipe**, não decisões individuais. Combine: *"O que justifica furar o foco de alguém?"* (produção fora do ar? cliente parado?). Escreva o acordo; revise a cada trimestre. Isso evita dois males: a pessoa "sempre disponível" e a pessoa "inalcançável".
:::

## 3) O que cada troca de contexto custa

Se a multitarefa é, na verdade, **troca rápida**, então cada troca tem um custo de **retomada**: reler o código, lembrar o que faltava, reconstruir o modelo mental. Um modelo simples:

```cs Jornada/Foco/Cap59/CustoDeTroca.cs
namespace Jornada.Foco.Cap59;

public static class CustoDeTroca
{
    /// <summary>
    /// Tempo realmente produtivo em um bloco: cada interrupção consome 'minutosPorTroca' para retomar o fio.
    /// O valor padrão (10 min) é ILUSTRATIVO: meça o seu caso.
    /// </summary>
    public static int MinutosProdutivos(int minutosDoBloco, int interrupcoes, int minutosPorTroca = 10)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(minutosDoBloco);
        ArgumentOutOfRangeException.ThrowIfNegative(interrupcoes);
        return Math.Max(0, minutosDoBloco - interrupcoes * minutosPorTroca);
    }

    public static double AproveitamentoPercentual(int minutosDoBloco, int interrupcoes, int minutosPorTroca = 10) =>
        minutosDoBloco == 0 ? 0 : 100.0 * MinutosProdutivos(minutosDoBloco, interrupcoes, minutosPorTroca) / minutosDoBloco;
}
```

```cs Uso
foreach (int interrupcoes in new[] { 0, 2, 5, 8 })
{
    int util = CustoDeTroca.MinutosProdutivos(240, interrupcoes);
    double pct = CustoDeTroca.AproveitamentoPercentual(240, interrupcoes);
    Console.WriteLine($"{interrupcoes} interrupção(ões) → {util,3} min úteis de 240 ({pct:F0}%)");
}
```

```saida
0 interrupção(ões) → 240 min úteis de 240 (100%)
2 interrupção(ões) → 220 min úteis de 240 (92%)
5 interrupção(ões) → 190 min úteis de 240 (79%)
8 interrupção(ões) → 160 min úteis de 240 (67%)
```

::: cuidado Modelo simplificado
Esse cálculo trata cada interrupção como igual e o custo como fixo. Na prática, o custo varia com a **tarefa** (depurar um problema difícil custa mais para retomar do que responder um e-mail) e com a **pessoa**. Use o modelo para **conversar** (*"quanto isso nos custa?"*), não como medida exata. Em um experimento sério, meça: registre quando as interrupções ocorrem e quanto tempo você leva para voltar.
:::

## 4) O "fora": atenção compartilhada

O livro lembra que **empatia exige atenção conjunta**. Em *pair programming*, em reuniões e em conversas de corredor, o gesto de olhar o telefone comunica *"não estou aqui"*. Dois acordos simples, fáceis de adotar: **laptops fechados** em reuniões que não exigem tela; e **celulares na gaveta** durante programação em par (como o casal da história original, que guarda os telefones ao chegar em casa). Isso não é regra de código — mas é parte do **ambiente** que torna o código bom.

::: exercicio Evolua a política
1. Faça com que **mensagens do seu próprio chefe** que contenham a palavra "urgente" (sem diferença de maiúsculas) sejam entregues agora, mesmo se o campo `Urgente` for `false`.
2. Escreva o teste que prova isso.

--- resposta
```cs Jornada/Foco/Cap59/PoliticaComTextoUrgente.cs
namespace Jornada.Foco.Cap59;

// Decorador: acrescenta a regra nova sem mexer na política original.
public sealed class PoliticaComTextoUrgente
{
    private readonly PoliticaDeInterrupcao basica;
    private readonly HashSet<string> vips;

    public PoliticaComTextoUrgente(PoliticaDeInterrupcao basica, IEnumerable<string> vips)
    {
        this.basica = basica;
        this.vips = new HashSet<string>(vips, StringComparer.OrdinalIgnoreCase);
    }

    public Decisao Decidir(Notificacao n, ModoDeAtencao modo)
    {
        if (vips.Contains(n.Origem) &&
            n.Texto.Contains("urgente", StringComparison.OrdinalIgnoreCase))
            return Decisao.EntregarAgora;

        return basica.Decidir(n, modo);
    }
}
```
```cs JornadaTests/Foco/Cap59/PoliticaComTextoUrgenteTests.cs
using Jornada.Foco.Cap59;
using Xunit;

public class PoliticaComTextoUrgenteTests
{
    private static PoliticaComTextoUrgente Criar() =>
        new(new PoliticaDeInterrupcao(Array.Empty<string>()), new[] { "Chefe" });

    [Fact]
    public void Texto_com_urgente_de_vip_interrompe()
    {
        var n = new Notificacao("Chefe", Canal.Chat, "Preciso disso URGENTE");
        Assert.Equal(Decisao.EntregarAgora, Criar().Decidir(n, ModoDeAtencao.Foco));
    }

    [Fact]
    public void Texto_com_urgente_de_quem_nao_e_vip_continua_esperando()
    {
        var n = new Notificacao("Caio", Canal.Chat, "urgente!!!");
        Assert.Equal(Decisao.AgruparNoResumo, Criar().Decidir(n, ModoDeAtencao.Foco));
    }
}
```
Usei um **decorador** para não mexer na política original (princípio aberto/fechado). Cuidado com **excesso de regras**: cada exceção ao foco precisa ser acordada pela equipe, senão volta o caos.
:::

::: resumo
- **Atenção seletiva:** mirar um alvo e **inibir** o resto; distrações **sensoriais** (ruído) e **emocionais** (seu nome, preocupações) — as emocionais são as mais fortes.
- **Multitarefa é troca rápida**; cada troca tem **custo de retomada**. A abundância de informação gera **pobreza de atenção**.
- Em C#: uma **política de interrupções** (`EntregarAgora` / `AgruparNoResumo` / `Silenciar`) e um **resumo periódico** separam o que **atrai** do que **precisa** de você.
- O **custo de troca** pode ser modelado com parâmetros ajustáveis (o 10 min é ilustrativo): use para conversar e **meça seu caso**.
- O "**fora**": olhar o telefone numa conversa corrói a atenção conjunta — combine acordos de equipe.
:::

::: quiz
? Qual a diferença entre distração sensorial e emocional?
+ A sensorial vem de estímulos do ambiente; a emocional, de sinais carregados de significado, como seu nome
- Não há diferença
- Sensorial é sempre pior
- Emocional só ocorre dormindo
> As emocionais costumam ser as mais difíceis de ignorar.

? O que a ciência cognitiva diz sobre "dividir a atenção" em multitarefas?
+ Na prática, trocamos rapidamente de alvo, e isso enfraquece o envolvimento
- A atenção é um balão elástico
- Podemos fazer várias coisas complexas ao mesmo tempo sem perda
- Só crianças trocam de foco
> Há um canal estreito que se reparte por troca.

? Por que a menção ao seu nome vai para o resumo (e não "agora") em modo foco?
+ Porque atrai a atenção automaticamente, mas raramente é urgente
- Porque menções são spam
- Porque o chat não funciona em modo foco
- Para economizar memória
> Separar o que atrai do que precisa.

? Para que serve o resumo periódico?
+ Receber o que não é urgente em lotes, em horários combinados, em vez de um fluxo contínuo
- Esconder mensagens para sempre
- Substituir o e-mail
- Aumentar o número de notificações
> Reduz a atenção parcial contínua.

? Como tratar o valor "10 minutos por troca" do modelo?
+ Como parâmetro ilustrativo, que você deve medir e ajustar
- Como constante científica
- Como lei
- Como erro
> O custo varia com tarefa e pessoa.

? O que é um "fora" numa conversa?
+ Um gesto (como checar o celular) que sinaliza falta de interesse no momento presente
- Um erro de compilação
- Uma mensagem urgente
- Um tipo de reunião
> Prejudica a atenção conjunta e a empatia.
:::

::: cartas
Atenção seletiva :: Mirar um alvo e inibir os demais estímulos.
Distração sensorial :: Estímulo físico do ambiente (ruído, luz).
Distração emocional :: Sinal carregado de significado (seu nome, preocupação).
Troca de contexto :: Mudança de uma tarefa para outra, com custo de retomada.
Modo foco :: Estado em que só o que é essencial interrompe.
Resumo periódico :: Entrega das mensagens não urgentes em horários combinados.
Atenção parcial contínua :: Estar meio presente em tudo, sem profundidade.
"Fora" (Goffman) :: Gesto que mostra desinteresse no que acontece aqui e agora.
:::
