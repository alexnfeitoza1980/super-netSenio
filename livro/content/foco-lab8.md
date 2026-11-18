---
id: foco-lab8
order: 96
kind: lab
num: 8
group: C# & Mentalidade · Parte 13 · Foco em C#
title: Laboratório 8 — Guardião do Foco
subtitle: Um pequeno sistema que protege um bloco de trabalho concentrado: filtra interrupções, resume o resto, captura ideias, acompanha a energia e entrega um relatório honesto no fim.
minutes: 90
hue: 262
badge: L8
objectives:
- **Combinar** módulos dos capítulos 59, 61, 62 e 71 em um **núcleo testável**
- Modelar um **bloco de foco** com relógio simulado, para que os testes sejam **determinísticos**
- Aplicar a **política de interrupções** e registrar **desvios** com duração
- Gerar um **relatório** com minutos produtivos, energia e **recomendação de pausa**
- Verificar o conjunto com **testes de cenário** e entender os **limites** do modelo
origem: livros/04-foco-em-csharp/content/106-lab8.md
---

::: vila O experimento da equipe
Lia propõe um experimento de duas semanas: **blocos de duas horas** de foco, com regras combinadas, e **medir** o que acontece em vez de discutir impressões. Para isso, ela quer uma pequena ferramenta, o **Guardião do Foco**: durante o bloco, decide o que interrompe, junta o resto em um resumo, guarda as ideias que surgem e, no fim, diz **quanto foi produtivo e como está a energia**. Sem julgar ninguém: só dados para a retrospectiva.
:::

::: ilustracao img/foco/lab8.svg | O Guardião do Foco: regras claras decidem quem fura o bloco e o que espera o resumo.
:::

## O que vamos construir

O **Guardião do Foco** é uma biblioteca (com um pequeno programa de demonstração) que:

1. inicia e encerra **blocos de foco** sobre uma **tarefa** (cap. 59);
2. aplica a **política de interrupções**: entregar agora, agrupar no resumo ou silenciar (cap. 59);
3. registra o **desvio** de uma interrupção atendida, com sua **duração** (cap. 71);
4. guarda as **ideias** que surgem sem quebrar o foco (cap. 61);
5. acompanha a **energia** de atenção, com pausas que restauram (cap. 62);
6. devolve um **relatório** do bloco com minutos produtivos e recomendação de pausa.

::: figura Arquitetura do Guardião
<svg viewBox="0 0 760 320" role="img" aria-label="GuardiaoDoFoco usa PoliticaDeInterrupcao, ResumoPeriodico, RegistroDeDesvios, CadernoDeIdeias e Energia, e produz um RelatorioDoBloco">
  <rect class="f-prim" x="290" y="10" width="180" height="46" rx="10"/><text x="380" y="38" text-anchor="middle" font-weight="800">Program (demonstração)</text>
  <rect class="f-info" x="270" y="100" width="220" height="64" rx="10"/><text x="380" y="128" text-anchor="middle" font-weight="800">GuardiaoDoFoco</text><text x="380" y="148" text-anchor="middle" font-size="11" class="t-dim">relógio simulado · blocos</text>
  <rect class="f-good" x="20" y="60" width="200" height="44" rx="10"/><text x="120" y="80" text-anchor="middle" font-size="13">PoliticaDeInterrupcao</text><text x="120" y="96" text-anchor="middle" font-size="10" class="t-dim">cap. 59</text>
  <rect class="f-good" x="20" y="120" width="200" height="44" rx="10"/><text x="120" y="140" text-anchor="middle" font-size="13">ResumoPeriodico</text><text x="120" y="156" text-anchor="middle" font-size="10" class="t-dim">cap. 59</text>
  <rect class="f-good" x="540" y="60" width="200" height="44" rx="10"/><text x="640" y="80" text-anchor="middle" font-size="13">RegistroDeDesvios</text><text x="640" y="96" text-anchor="middle" font-size="10" class="t-dim">cap. 71</text>
  <rect class="f-good" x="540" y="120" width="200" height="44" rx="10"/><text x="640" y="140" text-anchor="middle" font-size="13">CadernoDeIdeias</text><text x="640" y="156" text-anchor="middle" font-size="10" class="t-dim">cap. 61</text>
  <rect class="f-good" x="280" y="200" width="200" height="44" rx="10"/><text x="380" y="220" text-anchor="middle" font-size="13">Energia + Atividade</text><text x="380" y="236" text-anchor="middle" font-size="10" class="t-dim">cap. 62</text>
  <rect class="f-warn" x="270" y="266" width="220" height="44" rx="10"/><text x="380" y="293" text-anchor="middle" font-weight="800">RelatorioDoBloco</text>
  <path class="arrow" d="M380 58V98"/><path class="arrow" d="M268 120L222 90"/><path class="arrow" d="M268 140L222 140"/><path class="arrow" d="M492 120L538 90"/><path class="arrow" d="M492 140L538 140"/><path class="arrow" d="M380 166V198"/><path class="arrow" d="M380 246V264"/>
</svg>
:::

::: dica Antes de começar
Use a solução da Parte 13 (`Jornada` e `JornadaTests`) com os módulos dos capítulos **59** (`PoliticaDeInterrupcao`, `ResumoPeriodico`, `CustoDeTroca`), **61** (`CadernoDeIdeias`, `AgendaDoDia`), **62** (`Energia`, `Atividade`) e **71** (`RegistroDeDesvios`). O código novo deste laboratório fica no *namespace* `Jornada.Lab8`.
:::

## Passo 1 — O resultado que queremos

Começamos pelo **fim**: o que o relatório precisa dizer? Escrevemos o tipo antes da lógica, como na Lei da Imaginação do livro anterior:

```cs Jornada/Lab8/RelatorioDoBloco.cs
using Jornada.Foco.Cap61;

namespace Jornada.Lab8;

public sealed record RelatorioDoBloco(
    string Tarefa,
    int MinutosDoBloco,
    int MinutosDeFoco,
    int Interrupcoes,
    int MinutosEmInterrupcao,
    int MinutosProdutivosEstimados,
    double EnergiaFinal,
    string EstadoDaEnergia,
    bool PrecisaDePausa,
    IReadOnlyList<string> Resumo,
    IReadOnlyList<Ideia> Ideias);
```

## Passo 2 — O Guardião

O Guardião **orquestra** os módulos. Ele não contém as regras de interrupção, de energia ou de ideias: apenas as **combina** e controla o **tempo** (um relógio em minutos, que só anda quando você manda, o que deixa os testes exatos):

```cs Jornada/Lab8/GuardiaoDoFoco.cs
using Jornada.Foco.Cap59;
using Jornada.Foco.Cap61;
using Jornada.Foco.Cap62;
using Jornada.Foco.Cap71;

namespace Jornada.Lab8;

public sealed class GuardiaoDoFoco
{
    public const double LimiteDeParada = 40;

    private readonly PoliticaDeInterrupcao politica;
    private readonly ResumoPeriodico resumo = new();
    private readonly CadernoDeIdeias caderno = new();
    private readonly RegistroDeDesvios desvios = new();

    private Energia energia = Energia.Cheia;
    private string? tarefa;
    private int inicioDoBloco;
    private int minutosDeFoco;
    private int interrupcoes;
    private int minutosEmInterrupcao;
    private bool emInterrupcao;

    public ModoDeAtencao Modo { get; private set; } = ModoDeAtencao.Disponivel;
    public int MinutoAtual { get; private set; }
    public double EnergiaAtual => energia.Valor;

    public GuardiaoDoFoco(IEnumerable<string> origensVip, int minutoInicial = 0)
    {
        politica = new PoliticaDeInterrupcao(origensVip);
        MinutoAtual = minutoInicial;
    }

    public void IniciarFoco(string tarefaDoBloco)
    {
        if (string.IsNullOrWhiteSpace(tarefaDoBloco))
            throw new ArgumentException("Diga qual é a tarefa do bloco.", nameof(tarefaDoBloco));
        if (Modo == ModoDeAtencao.Foco)
            throw new InvalidOperationException("Já existe um bloco de foco em andamento.");

        tarefa = tarefaDoBloco.Trim();
        Modo = ModoDeAtencao.Foco;
        inicioDoBloco = MinutoAtual;
        minutosDeFoco = interrupcoes = minutosEmInterrupcao = 0;
    }

    /// <summary>Aplica a política e devolve a decisão. Só interrupções reais viram "desvio".</summary>
    public Decisao Receber(Notificacao notificacao)
    {
        var decisao = politica.Decidir(notificacao, Modo);

        switch (decisao)
        {
            case Decisao.AgruparNoResumo:
                resumo.Adicionar(notificacao);
                break;

            case Decisao.EntregarAgora when Modo == ModoDeAtencao.Foco && !emInterrupcao:
                desvios.Registrar(MinutoAtual, tarefa!, notificacao.Texto);
                emInterrupcao = true;
                interrupcoes++;
                break;
        }

        return decisao;
    }

    public void Trabalhar(int minutos)
    {
        if (Modo != ModoDeAtencao.Foco)
            throw new InvalidOperationException("Inicie um bloco de foco antes de trabalhar.");
        if (emInterrupcao)
            throw new InvalidOperationException("Há uma interrupção aberta: retome antes de trabalhar.");

        energia = energia.Aplicar(Atividade.TrabalhoFocado, minutos);
        minutosDeFoco += minutos;
        MinutoAtual += minutos;
    }

    /// <summary>Encerra a interrupção depois de 'minutosGastos' e devolve a duração do desvio.</summary>
    public int Retomar(int minutosGastos)
    {
        if (!emInterrupcao)
            throw new InvalidOperationException("Não há interrupção aberta.");

        MinutoAtual += minutosGastos;
        int duracao = desvios.Retomar(MinutoAtual);

        energia = energia.Aplicar(Atividade.ReuniaoDensa, duracao);   // atender também cansa
        minutosEmInterrupcao += duracao;
        emInterrupcao = false;
        return duracao;
    }

    public Ideia AnotarIdeia(string texto) => caderno.Capturar(texto, MinutoAtual);

    public RelatorioDoBloco EncerrarFoco()
    {
        if (Modo != ModoDeAtencao.Foco)
            throw new InvalidOperationException("Não há bloco de foco para encerrar.");
        if (emInterrupcao)
            throw new InvalidOperationException("Há uma interrupção aberta: retome antes de encerrar.");

        Modo = ModoDeAtencao.Disponivel;
        int duracao = MinutoAtual - inicioDoBloco;

        return new RelatorioDoBloco(
            Tarefa: tarefa!,
            MinutosDoBloco: duracao,
            MinutosDeFoco: minutosDeFoco,
            Interrupcoes: interrupcoes,
            MinutosEmInterrupcao: minutosEmInterrupcao,
            MinutosProdutivosEstimados: CustoDeTroca.MinutosProdutivos(duracao, interrupcoes),
            EnergiaFinal: energia.Valor,
            EstadoDaEnergia: energia.Estado,
            PrecisaDePausa: energia.Valor < LimiteDeParada,
            Resumo: resumo.Esvaziar(),
            Ideias: caderno.Revisar());
    }

    public void Pausar(int minutos)
    {
        if (Modo == ModoDeAtencao.Foco)
            throw new InvalidOperationException("Encerre o bloco de foco antes de pausar.");

        energia = energia.Aplicar(Atividade.Caminhada, minutos);
        MinutoAtual += minutos;
    }
}
```

Repare no que **não** está ali: nenhuma regra sobre quem é VIP, nenhum cálculo de energia, nenhuma fila. Cada decisão vive no módulo que a conhece. O Guardião só **coordena** (a *Lei da Mente Mestra*, do livro anterior, aplicada de novo).

::: cuidado O relógio é simulado
Em vez de ler `DateTime.Now`, o Guardião tem um **`MinutoAtual`** que só avança nas chamadas `Trabalhar`, `Retomar` e `Pausar`. É o mesmo cuidado do *Diário da Jornada*, que recebia a data por um delegado: **tempo é dependência** e, injetada, torna o teste **repetível**. Em um aplicativo real, um pequeno adaptador converteria o relógio verdadeiro em minutos.
:::

## Passo 3 — Um bloco de duas horas

Uma manhã de **Caio**, a partir das 9h, com a **Dona Marta** como origem VIP:

```cs Uso
using Jornada.Foco.Cap59;
using Jornada.Foco.Cap61;
using Jornada.Lab8;

var guardiao = new GuardiaoDoFoco(new[] { "Dona Marta" }, minutoInicial: 9 * 60);

guardiao.IniciarFoco("Implementar cache por tenant");
Console.WriteLine($"{AgendaDoDia.Hora(guardiao.MinutoAtual)} foco em: Implementar cache por tenant");

guardiao.Trabalhar(30);
Console.WriteLine($"{guardiao.Receber(new Notificacao("Caio", Canal.Chat, "Lia, viu o PR?", MencionaVoce: true)),-16} ← Caio");
Console.WriteLine($"{guardiao.Receber(new Notificacao("bot-ci", Canal.Alerta, "Build #4812 passou")),-16} ← bot-ci");
guardiao.AnotarIdeia("renomear 'Manager' para algo com sentido");

guardiao.Trabalhar(30);
Console.WriteLine($"{guardiao.Receber(new Notificacao("Dona Marta", Canal.Chat, "Cliente parado", MencionaVoce: true, Urgente: true)),-16} ← Dona Marta");
int minutosNoAtendimento = guardiao.Retomar(10);
Console.WriteLine($"{"",-16}   (atendida em {minutosNoAtendimento} min)");

guardiao.Trabalhar(40);
Console.WriteLine($"{guardiao.Receber(new Notificacao("Bia", Canal.Email, "Notas da reunião de ontem")),-16} ← Bia");
guardiao.Trabalhar(20);

var r = guardiao.EncerrarFoco();
Console.WriteLine("--- relatório do bloco ---");
Console.WriteLine($"Tarefa: {r.Tarefa} ({r.MinutosDoBloco} min no relógio)");
Console.WriteLine($"Foco: {r.MinutosDeFoco} min · interrupções: {r.Interrupcoes} ({r.MinutosEmInterrupcao} min)");
Console.WriteLine($"Produtivos estimados: {r.MinutosProdutivosEstimados} de {r.MinutosDoBloco} min");
Console.WriteLine($"Energia: {r.EnergiaFinal:F0} ({r.EstadoDaEnergia}) · pausa recomendada: {(r.PrecisaDePausa ? "sim" : "não")}");
Console.WriteLine("Resumo:");
foreach (var linha in r.Resumo) Console.WriteLine("  " + linha);
Console.WriteLine("Ideias para o casulo:");
foreach (var ideia in r.Ideias) Console.WriteLine($"  #{ideia.Numero} ({AgendaDoDia.Hora(ideia.MinutoDoDia)}): {ideia.Texto}");

guardiao.Pausar(20);
Console.WriteLine($"Depois de 20 min de caminhada: energia {guardiao.EnergiaAtual:F0}");
```

```saida
09:00 foco em: Implementar cache por tenant
AgruparNoResumo  ← Caio
Silenciar        ← bot-ci
EntregarAgora    ← Dona Marta
                   (atendida em 10 min)
AgruparNoResumo  ← Bia
--- relatório do bloco ---
Tarefa: Implementar cache por tenant (130 min no relógio)
Foco: 120 min · interrupções: 1 (10 min)
Produtivos estimados: 120 de 130 min
Energia: 34 (cansando) · pausa recomendada: sim
Resumo:
  @ Caio: 1 mensagem(ns) — última: Lia, viu o PR?
    Bia: 1 mensagem(ns) — última: Notas da reunião de ontem
Ideias para o casulo:
  #1 (09:30): renomear 'Manager' para algo com sentido
Depois de 20 min de caminhada: energia 50
```

Leia o relatório como **conversa**, não como nota: 120 minutos de trabalho focado, **uma** interrupção legítima (cliente parado), duas mensagens que **esperaram** o resumo, uma ideia que **não** quebrou o foco e uma energia que pede pausa. O número de **"produtivos estimados"** vem do modelo simples do capítulo 59; seus 10 minutos por troca são **ilustrativos** e devem ser medidos.

## Passo 4 — Testes de cenário

Como o tempo é simulado, cada cenário é **exato**:

```cs JornadaTests/Lab8/GuardiaoDoFocoTests.cs
using Jornada.Foco.Cap59;
using Jornada.Lab8;
using Xunit;

public class GuardiaoDoFocoTests
{
    private static GuardiaoDoFoco Novo() => new(new[] { "Dona Marta" }, minutoInicial: 540);

    private static Notificacao Mencao(string origem) =>
        new(origem, Canal.Chat, "oi", MencionaVoce: true);

    [Fact]
    public void Fora_do_foco_tudo_e_entregue()
    {
        var g = Novo();
        Assert.Equal(Decisao.EntregarAgora, g.Receber(Mencao("Caio")));
    }

    [Fact]
    public void Mencao_comum_no_foco_vai_para_o_resumo_do_relatorio()
    {
        var g = Novo();
        g.IniciarFoco("Tarefa");
        Assert.Equal(Decisao.AgruparNoResumo, g.Receber(Mencao("Caio")));
        g.Trabalhar(60);

        var r = g.EncerrarFoco();

        Assert.Equal(0, r.Interrupcoes);
        Assert.Single(r.Resumo);
        Assert.StartsWith("@ Caio", r.Resumo[0]);
    }

    [Fact]
    public void Bot_silenciado_nao_aparece_no_resumo()
    {
        var g = Novo();
        g.IniciarFoco("Tarefa");
        Assert.Equal(Decisao.Silenciar, g.Receber(new Notificacao("bot-ci", Canal.Chat, "ok")));
        g.Trabalhar(30);

        Assert.Empty(g.EncerrarFoco().Resumo);
    }

    [Fact]
    public void Alerta_urgente_interrompe_e_a_duracao_e_registrada()
    {
        var g = Novo();
        g.IniciarFoco("Tarefa");
        g.Trabalhar(30);

        Assert.Equal(Decisao.EntregarAgora,
            g.Receber(new Notificacao("Monitor", Canal.Alerta, "fora do ar", Urgente: true)));
        Assert.Equal(15, g.Retomar(15));
        g.Trabalhar(30);

        var r = g.EncerrarFoco();
        Assert.Equal(1, r.Interrupcoes);
        Assert.Equal(15, r.MinutosEmInterrupcao);
        Assert.Equal(60, r.MinutosDeFoco);
        Assert.Equal(75, r.MinutosDoBloco);
        Assert.Equal(65, r.MinutosProdutivosEstimados);   // 75 - 1 × 10
    }

    [Fact]
    public void Duas_urgencias_seguidas_contam_como_uma_so_interrupcao()
    {
        var g = Novo();
        g.IniciarFoco("Tarefa");
        g.Receber(new Notificacao("Monitor", Canal.Alerta, "a", Urgente: true));
        g.Receber(new Notificacao("Monitor", Canal.Alerta, "b", Urgente: true));
        g.Retomar(5);

        Assert.Equal(1, g.EncerrarFoco().Interrupcoes);
    }

    [Fact]
    public void Nao_se_trabalha_com_interrupcao_aberta()
    {
        var g = Novo();
        g.IniciarFoco("Tarefa");
        g.Receber(new Notificacao("Monitor", Canal.Alerta, "x", Urgente: true));

        Assert.Throws<InvalidOperationException>(() => g.Trabalhar(10));
    }

    [Fact]
    public void Operacoes_fora_de_ordem_sao_recusadas()
    {
        var g = Novo();
        Assert.Throws<InvalidOperationException>(() => g.Trabalhar(10));
        Assert.Throws<InvalidOperationException>(() => g.EncerrarFoco());
        Assert.Throws<InvalidOperationException>(() => g.Retomar(5));

        g.IniciarFoco("Tarefa");
        Assert.Throws<InvalidOperationException>(() => g.IniciarFoco("Outra"));
        Assert.Throws<InvalidOperationException>(() => g.Pausar(10));
    }

    [Fact]
    public void Tarefa_vazia_e_rejeitada() =>
        Assert.Throws<ArgumentException>(() => Novo().IniciarFoco("   "));

    [Fact]
    public void Bloco_longo_pede_pausa_e_a_pausa_restaura()
    {
        var g = Novo();
        g.IniciarFoco("Tarefa");
        g.Trabalhar(150);                       // 100 - 75 = 25

        Assert.True(g.EncerrarFoco().PrecisaDePausa);

        g.Pausar(30);                           // +24
        Assert.Equal(49, g.EnergiaAtual, 6);
    }

    [Fact]
    public void Ideias_sao_entregues_uma_unica_vez()
    {
        var g = Novo();
        g.IniciarFoco("Tarefa");
        g.AnotarIdeia("ideia");
        g.Trabalhar(10);
        Assert.Single(g.EncerrarFoco().Ideias);

        g.IniciarFoco("Outra");
        g.Trabalhar(10);
        Assert.Empty(g.EncerrarFoco().Ideias);
    }
}
```

## Passo 5 — Onde estão os capítulos?

Percorra o projeto e confira se você consegue **apontar** cada ideia do livro de Goleman no código:

| Ideia | Onde aparece no Guardião |
|---|---|
| **Atenção seletiva** (cap. 59) | `PoliticaDeInterrupcao`: o que **merece** furar o foco |
| **Distração sensorial × emocional** (cap. 59) | `bot-ci` silenciado; menção ao seu nome vai ao **resumo**, não "agora" |
| **Custo de troca** (cap. 59) | `CustoDeTroca.MinutosProdutivos` no relatório |
| **Divagação e casulo** (cap. 61) | `CadernoDeIdeias`: captura sem quebrar o foco, revisão depois |
| **Fadiga e restauração** (cap. 62) | `Energia`: trabalho gasta, **caminhada** restaura, limite de parada |
| **Repetição da atenção** (cap. 71) | `RegistroDeDesvios`: cada desvio com **duração** |
| **Sistemas** (cap. 67) | O relatório mostra **o conjunto** (foco, interrupções, energia), não um número só |
| **Liderança de longo prazo** (cap. 73) | Os dados alimentam a **retrospectiva**, e a equipe decide as regras junto |

::: exercicio Desafios para evoluir
Escolha um e implemente, **começando pelo teste**:

1. **Conselheiro:** a partir do `RelatorioDoBloco`, devolva **conselhos** usando `PlanoSeEntao` (cap. 64): "se muitas interrupções, então combinar um horário de resumo"; "se energia baixa, então caminhar antes do próximo bloco".
2. **Agenda:** use `AgendaDoDia` (cap. 61) para **auditar** o dia de blocos e pausas do Guardião, avisando foco acima de 120 minutos e falta de casulo.
3. **Pré-compromisso:** durante o foco, `PreCompromisso` (cap. 64) bloqueia **sites** até o fim do bloco; o Guardião expõe `bool Permitido(string alvo)`.
4. **Persistência:** grave os relatórios de cada bloco em JSON (como no Laboratório 7: leitura tolerante, gravação atômica) para comparar semana a semana.
5. **Parâmetros da equipe:** carregue a **lista de VIPs** e os **limites** (energia, minutos por troca) de um arquivo, em vez de fixá-los no código.

--- resposta
Sugestão para o **desafio 1** (o Conselheiro), testado:

```cs Jornada/Lab8/Conselheiro.cs
using Jornada.Foco.Cap64;

namespace Jornada.Lab8;

public static class Conselheiro
{
    public const int InterrupcoesDeAlerta = 3;

    public static IReadOnlyList<string> Para(RelatorioDoBloco relatorio)
    {
        var plano = new PlanoSeEntao();
        plano.Adicionar("muitas interrupções", "combinar um horário de resumo com a equipe");
        plano.Adicionar("energia baixa",       "caminhar 20 minutos antes do próximo bloco");

        var situacoes = new List<string>();
        if (relatorio.Interrupcoes >= InterrupcoesDeAlerta) situacoes.Add("muitas interrupções");
        if (relatorio.PrecisaDePausa)                       situacoes.Add("energia baixa");

        return situacoes.Select(plano.Responder).ToList();
    }
}
```
```cs JornadaTests/Lab8/ConselheiroTests.cs
using Jornada.Lab8;
using Xunit;

public class ConselheiroTests
{
    private static RelatorioDoBloco Relatorio(int interrupcoes, bool pausa) =>
        new("T", 120, 100, interrupcoes, 0, 100, 50, "cansando", pausa, Array.Empty<string>(), Array.Empty<Jornada.Foco.Cap61.Ideia>());

    [Fact]
    public void Bloco_tranquilo_nao_gera_conselhos() =>
        Assert.Empty(Conselheiro.Para(Relatorio(interrupcoes: 1, pausa: false)));

    [Fact]
    public void Muitas_interrupcoes_e_energia_baixa_geram_dois_conselhos()
    {
        var conselhos = Conselheiro.Para(Relatorio(interrupcoes: 4, pausa: true));
        Assert.Equal(2, conselhos.Count);
        Assert.Contains("resumo", conselhos[0]);
        Assert.Contains("caminhar", conselhos[1]);
    }
}
```
Repare que o Conselheiro **não** toca no Guardião: recebe o **relatório** (um dado) e devolve texto. É a vantagem de separar o que **mede** do que **aconselha**, e de manter cada parte testável sozinha.
:::

## O que você praticou

Mais do que uma ferramenta, você montou um **ambiente de foco em código**: regras de interrupção explícitas, resumo em vez de fluxo contínuo, ideias capturadas sem custo, energia acompanhada e um relatório que **conversa** em vez de julgar. E, tão importante quanto: viu os **limites** do modelo (minutos por troca ilustrativos, energia simplificada) e a regra que vale para todo este livro: **o código ajuda a pensar; a atenção continua sendo sua**.

::: resumo
- O **Guardião do Foco** combina módulos dos capítulos 59, 61, 62 e 71 em um núcleo **testável**, com **relógio simulado**.
- A **política de interrupções** decide o que fura o foco; o resto vai para o **resumo**; ruído é **silenciado**.
- **Desvios** têm duração; **ideias** são capturadas sem quebrar o foco; a **energia** pede pausas que restauram.
- O **relatório** traz minutos produtivos estimados, energia e recomendação de pausa: material para **conversa**, não para punição.
- Os números do modelo são **ilustrativos**: meça seus casos e ajuste, junto com a equipe.
:::

::: quiz
? Por que o Guardião usa um relógio em minutos que avança só por chamadas?
+ Para que os testes sejam determinísticos, tratando o tempo como dependência
- Para economizar memória
- Porque DateTime não existe em C#
- Para evitar testes
> Tempo injetado = teste repetível.

? Por que duas urgências seguidas contam como uma única interrupção?
+ Porque a pessoa ainda está no mesmo desvio, que só termina quando ela retoma o trabalho
- Porque o C# ignora a segunda
- Porque urgências não existem
- Por erro de arredondamento
> O desvio aberto só fecha em Retomar.

? Onde fica a regra sobre quem é VIP?
+ Dentro da PoliticaDeInterrupcao; o Guardião só a usa
- No Guardião
- No relatório
- No caderno de ideias
> Cada decisão mora no módulo que a conhece.

? Por que o relatório deve ser lido como conversa?
+ Os números vêm de um modelo simples e ilustrativo; servem para discutir, não para julgar
- Porque são sempre exatos
- Porque o relatório é secreto
- Porque o C# não permite notas
> Meça seus casos e ajuste.

? O que o Conselheiro do exercício recebe?
+ O RelatorioDoBloco (um dado) e devolve conselhos em texto
- O Guardião inteiro
- O relógio
- O arquivo de ideias
> Separar o que mede do que aconselha.

? Por que as ideias são entregues uma única vez?
+ Revisar marca as ideias como revisadas, evitando repeti-las no bloco seguinte
- Porque o caderno apaga o arquivo
- Porque só cabe uma ideia
- Por limitação do xUnit
> Captura rápida, revisão no casulo.
:::

::: cartas
Guardião do Foco :: Aplicativo do laboratório que protege um bloco de trabalho concentrado.
Relógio simulado :: Tempo controlado pelo código, para testes exatos.
Bloco de foco :: Período dedicado a uma tarefa, com regras de interrupção.
Desvio :: Saída da tarefa por causa de uma interrupção, com duração.
Relatório do bloco :: Resumo de foco, interrupções, energia e recomendação.
Minutos produtivos estimados :: Duração do bloco menos o custo de retomada das interrupções.
Conselheiro :: Parte que transforma o relatório em sugestões "se–então".
Retrospectiva :: Conversa da equipe que usa os dados para ajustar as regras.
:::
