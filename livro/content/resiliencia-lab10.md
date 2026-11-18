---
id: resiliencia-lab10
order: 123
kind: lab
num: 10
group: C# & Mentalidade · Parte 15 · Resiliência em C#
title: Laboratório: Caderno de um próximo passo
subtitle: Juntamos tudo em um programa de console: check-in, sentimento, fato e história, foco, apoio e um passo pequeno, com testes, privacidade e escolha em cada etapa.
minutes: 90
hue: 265
objectives:
- **Integrar** os modelos dos capítulos 88 a 95 em um programa de console completo
- Receber **entrada e saída de fora** (`TextReader`/`TextWriter`) para testar a conversa inteira sem teclado
- Usar **`init`**, **property patterns** (`is { } s`) e tuplas na montagem do resumo
- Garantir **privacidade** (nada gravado), **autonomia** (tudo pode ser pulado) e **cuidado** (urgência leva aos serviços)
origem: livros/06-resiliencia-csharp/content/134-lab10.md
---

::: ilustracao img/resiliencia/lab10.svg | O Caderno de um próximo passo: cinco perguntas, tudo só na memória, e o direito de pular qualquer uma.
:::

## O que vamos construir

Ao longo dos capítulos, encontramos perguntas simples: **como estou? o que aconteceu? o que é fato e o que é história? com o que eu conto? qual é um passo possível? quem pode apoiar?** Vamos reuni-las em um **Caderno de um próximo passo**, um pequeno programa de console que faz essas perguntas **uma de cada vez** e monta um **resumo** com o que a pessoa quis responder.

O programa foi desenhado com três compromissos:

1. **Privacidade**: tudo fica só na **memória**; nada é gravado em arquivo nem enviado.
2. **Autonomia**: **qualquer pergunta pode ser pulada** (basta Enter); silêncio nunca é erro.
3. **Cuidado**: quem diz que **não está bem** recebe, **antes de tudo**, o acolhimento e os serviços de apoio, e **escolhe** se quer continuar.

::: cuidado Antes de começar
Use o caderno com situações **leves e comuns** (uma tarefa atrasada, uma dúvida na prova). Ele **não** é um instrumento de avaliação psicológica, de diagnóstico ou de tratamento, e **não** deve receber informações que você não gostaria de compartilhar. Em sofrimento intenso, procure apoio profissional: **CVV 188** (24 h, gratuito) ou **SAMU 192** em emergências.
:::

O código completo está em [codigo/resiliencia/Resiliencia.Console](codigo/resiliencia/Resiliencia.Console/) (o `Program.cs`) e [codigo/resiliencia/Resiliencia.Capitulos/Lab10](codigo/resiliencia/Resiliencia.Capitulos/Lab10/) (a lógica). Abra a solução `codigo/resiliencia/Resiliencia.sln` no Visual Studio ou rode com o .NET 8 SDK:

```bash
dotnet run --project codigo/resiliencia/Resiliencia.Console
```

::: figura Como as peças se encaixam
<svg viewBox="0 0 760 270" role="img" aria-label="O Roteiro conversa com a pessoa e preenche o Caderno, que reúne os modelos dos capítulos 89 a 95">
  <rect x="20" y="100" width="150" height="70" rx="12" class="f-info"/><text x="95" y="128" text-anchor="middle" font-weight="800">Roteiro</text><text x="95" y="148" text-anchor="middle" font-size="11" class="t-dim">TextReader / TextWriter</text>
  <rect x="230" y="100" width="170" height="70" rx="12" class="f-prim"/><text x="315" y="128" text-anchor="middle" font-weight="800">CadernoDeProximoPasso</text><text x="315" y="148" text-anchor="middle" font-size="11" class="t-dim">reúne e resume</text>
  <path class="arrow" d="M172 135H228"/>
  <g font-size="12" text-anchor="middle">
    <rect x="470" y="10" width="270" height="40" rx="10" class="f-good"/><text x="605" y="35">Cap. 95 · CheckIn, Resultado&lt;T&gt;</text>
    <rect x="470" y="58" width="270" height="40" rx="10" class="f-good"/><text x="605" y="83">Cap. 89 · Sentimento, Pausa, Vocabulário</text>
    <rect x="470" y="106" width="270" height="40" rx="10" class="f-good"/><text x="605" y="131">Cap. 90 · Fatos, histórias, foco, mochila</text>
    <rect x="470" y="154" width="270" height="40" rx="10" class="f-good"/><text x="605" y="179">Cap. 92 · Meta e passos pequenos</text>
    <rect x="470" y="202" width="270" height="40" rx="10" class="f-good"/><text x="605" y="227">Cap. 93 · Mapa de apoio e serviços</text>
  </g>
  <path class="arrow" d="M402 135C430 135 440 30 468 30"/><path class="arrow" d="M402 135C430 135 440 78 468 78"/><path class="arrow" d="M402 135H468"/><path class="arrow" d="M402 135C430 135 440 174 468 174"/><path class="arrow" d="M402 135C430 135 440 222 468 222"/>
</svg>
:::

## Passo 1: o caderno

O `CadernoDeProximoPasso` é só o **conjunto de respostas**, sem lógica de conversa. Veja três recursos novos:

- **`init`** em `Situacao`: a propriedade pode ser definida **na criação** (`new CadernoDeProximoPasso { Situacao = "..." }`), mas não depois.
- **`is { } s`**: um *property pattern* que significa "se `Sentimento` **não é nulo**, guarde-o em `s`". É o `if (x != null) { var s = x; ... }` em uma linha.
- A **tupla** `var (feitos, total) = Meta.Andamento;`, que desconstrói o par do capítulo 92.

```cs Jornada/Resiliencia/Lab10/CadernoDeProximoPasso.cs
using Jornada.Resiliencia.Cap89;
using Jornada.Resiliencia.Cap90;
using Jornada.Resiliencia.Cap92;
using Jornada.Resiliencia.Cap93;

namespace Jornada.Resiliencia.Lab10;

public sealed class CadernoDeProximoPasso
{
    public string Situacao { get; init; } = "uma tarefa que quero organizar";
    public Sentimento? Sentimento { get; set; }

    public List<Anotacao> Anotacoes { get; } = new();
    public List<Preocupacao> Preocupacoes { get; } = new();
    public Mochila Mochila { get; } = new();
    public Meta? Meta { get; set; }
    public MapaDeApoio Apoio { get; } = new();

    /// <summary>Monta o resumo da sessão. Só usa o que a pessoa escolheu preencher.</summary>
    public IReadOnlyList<string> Resumo()
    {
        var linhas = new List<string> { $"Situação: {Situacao}" };

        if (Sentimento is { } s)
        {
            linhas.Add($"Como me sinto: {Vocabulario.Nomear(s)} ({s.Intensidade}/10).");
            if (SugestaoDePausa.Para(s) is { } pausa && pausa > TimeSpan.Zero)
                linhas.Add($"Uma pausa de {pausa.TotalMinutes:0} min antes de decidir pode ajudar. É uma sugestão.");
        }

        var (fatos, historias) = Organizador.Contar(Anotacoes);
        if (fatos + historias > 0)
        {
            linhas.Add($"Fatos: {fatos} · Histórias que preciso checar: {historias}.");
            linhas.AddRange(Organizador.PerguntasParaAsHistorias(Anotacoes).Select(p => "  ? " + p));
        }

        if (Organizador.ProximoFoco(Preocupacoes) is { } foco)
            linhas.Add($"Onde pôr energia primeiro: {foco.Texto}.");

        if (Meta?.ProximoPasso is { } passo)
        {
            var (feitos, total) = Meta.Andamento;
            linhas.Add($"Próximo passo possível: {passo.Descricao} ({passo.Minutos} min). Andamento: {feitos}/{total}.");
        }

        var pessoas = Apoio.Quem(Necessidade.Estudar).Concat(Apoio.Quem(Necessidade.Ouvir)).Select(a => a.Nome).Distinct().ToList();
        if (pessoas.Count > 0)
            linhas.Add("Quem pode apoiar: " + string.Join(", ", pessoas) + ".");

        foreach (var pergunta in Mochila.Perguntas().Take(1))
            linhas.Add("Uma pergunta sobre a sua mochila: " + pergunta);

        return linhas;
    }
}
```

Repare que o `Resumo()` **só inclui o que foi preenchido**: se a pessoa pulou o sentimento, não há linha de sentimento. Nada de "campo obrigatório".

## Passo 2: o roteiro da conversa

O `Roteiro` faz as perguntas. A decisão de projeto mais importante: ele **não usa `Console` diretamente**. Recebe um `TextReader` (de onde vêm as respostas) e um `TextWriter` (para onde vai o texto). No programa real, passamos `Console.In` e `Console.Out`; nos **testes**, um `StringReader` e um `StringWriter`: **a conversa inteira fica testável**, sem teclado nem tela.

Acompanhe o fluxo do `Executar`:

1. O **check-in** vem **primeiro**; usa o `Resultado<T>` do capítulo 95 (`.Combinar(r => r, _ => PreferoNaoDizer)`).
2. Se a pessoa **não está bem**, aparecem **os serviços** e uma pergunta: "quer continuar?". **Só continua se disser "s"**; qualquer outra coisa encerra com um "estará aqui quando você quiser".
3. Cada bloco seguinte (`LerSentimento`, `LerAnotacoes`, `LerPreocupacao`, `LerApoio`, `LerMeta`) trata o **Enter vazio** como "pular" (`Perguntar` devolve `null`).

```cs Jornada/Resiliencia/Lab10/Roteiro.cs
using Jornada.Resiliencia.Cap89;
using Jornada.Resiliencia.Cap90;
using Jornada.Resiliencia.Cap92;
using Jornada.Resiliencia.Cap93;
using Jornada.Resiliencia.Cap95;

namespace Jornada.Resiliencia.Lab10;

/// <summary>A conversa no console. Recebe a entrada e a saída de fora, então também dá para testar sem teclado.</summary>
public sealed class Roteiro
{
    private readonly TextReader entrada;
    private readonly TextWriter saida;
    private readonly DateOnly hoje;

    public Roteiro(TextReader entrada, TextWriter saida, DateOnly hoje)
    {
        this.entrada = entrada;
        this.saida = saida;
        this.hoje = hoje;
    }

    /// <summary>Faz uma pergunta. Resposta em branco devolve null: pular é sempre permitido.</summary>
    private string? Perguntar(string pergunta)
    {
        saida.Write(pergunta + " ");
        var texto = entrada.ReadLine();
        return string.IsNullOrWhiteSpace(texto) ? null : texto.Trim();
    }

    public CadernoDeProximoPasso? Executar()
    {
        saida.WriteLine("Caderno de um próximo passo. Nada é gravado: tudo fica só na memória.");
        saida.WriteLine("Deixe em branco o que preferir pular. Use situações leves e comuns.");
        saida.WriteLine();

        var resposta = CheckIn
            .Ler(Perguntar("Como você está? 1) bem  2) mais ou menos  3) não estou bem  4) prefiro não dizer"))
            .Combinar(r => r, _ => RespostaDoCheckIn.PreferoNaoDizer);
        saida.WriteLine(CheckIn.Acolher(resposta));

        if (resposta == RespostaDoCheckIn.NaoEstouBem)
        {
            foreach (var servico in ServicosPublicos.Brasil)
                saida.WriteLine("  - " + servico.ComoPedir(Necessidade.Urgencia));

            if (Perguntar("Quer continuar o caderno agora? (s/n)")?.ToLowerInvariant() != "s")
            {
                saida.WriteLine("Tudo bem. Ele estará aqui quando você quiser.");
                return null;
            }
        }

        var caderno = new CadernoDeProximoPasso
        {
            Situacao = Perguntar("Que situação comum você quer organizar?") ?? "uma tarefa que quero organizar"
        };

        LerSentimento(caderno);
        LerAnotacoes(caderno);
        LerPreocupacao(caderno);
        LerApoio(caderno);
        LerMeta(caderno);

        saida.WriteLine();
        saida.WriteLine("--- Seu resumo ---");
        foreach (var linha in caderno.Resumo())
            saida.WriteLine(linha);

        return caderno;
    }

    private void LerSentimento(CadernoDeProximoPasso c)
    {
        var emocao = Perguntar("Emoção (calma, alegria, tristeza, raiva, medo, cansaco):");
        if (emocao is null || !Enum.TryParse<Emocao>(emocao, ignoreCase: true, out var e))
            return;

        int intensidade = int.TryParse(Perguntar("Intensidade de 0 a 10:"), out var n) ? Math.Clamp(n, 0, 10) : 5;
        c.Sentimento = new Sentimento(e, intensidade);
    }

    private void LerAnotacoes(CadernoDeProximoPasso c)
    {
        if (Perguntar("Um FATO (o que dá para observar):") is { } fato)
            c.Anotacoes.Add(new Anotacao(fato, NaturezaDaFrase.Fato));

        if (Perguntar("Uma HISTÓRIA (a explicação que você imaginou):") is { } historia)
            c.Anotacoes.Add(new Anotacao(historia, NaturezaDaFrase.Historia));
    }

    private void LerPreocupacao(CadernoDeProximoPasso c)
    {
        if (Perguntar("Uma preocupação:") is not { } texto)
            return;

        var controle = Perguntar("Depende de você? 1) sim  2) posso influenciar  3) não depende") switch
        {
            "1" => Controle.DependeDeMim,
            "3" => Controle.NaoDepende,
            _   => Controle.PossoInfluenciar
        };

        c.Preocupacoes.Add(new Preocupacao(texto, controle));
    }

    private void LerApoio(CadernoDeProximoPasso c)
    {
        if (Perguntar("Alguém que poderia ajudar (nome):") is not { } nome)
            return;

        c.Apoio.Adicionar(new Pessoa(nome, Necessidade.Ouvir, Necessidade.Estudar));
        c.Mochila.Guardar(nome, TipoRecurso.Pessoas);
    }

    private void LerMeta(CadernoDeProximoPasso c)
    {
        if (Perguntar("Uma tarefa para dividir em passos:") is not { } tarefa)
            return;

        int minutos = int.TryParse(Perguntar("Quantos minutos ela tomaria no total?"), out var n) && n > 0 ? n : 25;
        c.Meta = new Meta(tarefa).AdicionarDividindo(tarefa, minutos);
    }
}
```

Veja como o código **desarma erros pequenos em vez de repreender**: intensidade fora da escala? `Math.Clamp` ajusta para 0–10. Emoção desconhecida? Pula. Minutos inválidos? Usa 25. Esse é o espírito do capítulo 95 aplicado ao programa inteiro.

## Passo 3: o `Program.cs`

Com a lógica no projeto de classes, o programa de console fica minúsculo:

```cs Resiliencia.Console/Program.cs
using Jornada.Resiliencia.Lab10;

Console.OutputEncoding = System.Text.Encoding.UTF8;

var roteiro = new Roteiro(Console.In, Console.Out, DateOnly.FromDateTime(DateTime.Today));
roteiro.Executar();
```

Esta é uma **sessão de verdade** (as respostas aparecem depois de cada pergunta):

```saida
Caderno de um próximo passo. Nada é gravado: tudo fica só na memória.
Deixe em branco o que preferir pular. Use situações leves e comuns.

Como você está? 1) bem  2) mais ou menos  3) não estou bem  4) prefiro não dizer 2
Dias assim existem. Quer só anotar ou também pensar num passo pequeno?
Que situação comum você quer organizar? Tenho uma tarefa atrasada do curso
Emoção (calma, alegria, tristeza, raiva, medo, cansaco): medo
Intensidade de 0 a 10: 5
Um FATO (o que dá para observar): Ainda não comecei a primeira parte
Uma HISTÓRIA (a explicação que você imaginou): Vou reprovar no curso inteiro
Uma preocupação: Começar o primeiro exercício
Depende de você? 1) sim  2) posso influenciar  3) não depende 1
Alguém que poderia ajudar (nome): Beto
Uma tarefa para dividir em passos: Resolver a lista de exercícios
Quantos minutos ela tomaria no total? 50

--- Seu resumo ---
Situação: Tenho uma tarefa atrasada do curso
Como me sinto: medo (5/10).
Uma pausa de 2 min antes de decidir pode ajudar. É uma sugestão.
Fatos: 1 · Histórias que preciso checar: 1.
  ? "Vou reprovar no curso inteiro": o que me faz pensar assim? Existe outra explicação possível?
Onde pôr energia primeiro: Começar o primeiro exercício.
Próximo passo possível: Resolver a lista de exercícios (parte 1 de 2) (25 min). Andamento: 0/2.
Quem pode apoiar: Beto.
Uma pergunta sobre a sua mochila: Que habilidade ou qualidade sua já ajudou em outra ocasião?
```

E o caminho de **cuidado**: quando a pessoa diz que não está bem e prefere parar.

```saida
Caderno de um próximo passo. Nada é gravado: tudo fica só na memória.
Deixe em branco o que preferir pular. Use situações leves e comuns.

Como você está? 1) bem  2) mais ou menos  3) não estou bem  4) prefiro não dizer 3
Obrigado por dizer. Você não precisa resolver isso agora. Posso mostrar quem pode apoiar.
  - CVV (apoio emocional): ligue 188. Você não precisa explicar tudo de uma vez.
  - SAMU (emergência médica): ligue 192. Você não precisa explicar tudo de uma vez.
Quer continuar o caderno agora? (s/n) n
Tudo bem. Ele estará aqui quando você quiser.
```

Nada foi perguntado além do necessário, os serviços apareceram **sem que a pessoa precisasse pedir** e a saída foi **sem cobrança**.

## Passo 4: os testes do laboratório

Os testes usam um **auxiliar** `Rodar(params string[] respostas)` que monta a entrada e a saída em memória e devolve o caderno e o texto impresso. Cada teste é uma **cena** da vida do programa:

```cs Resiliencia.Testes/Lab10Tests.cs
using Jornada.Resiliencia.Cap89;
using Jornada.Resiliencia.Lab10;

namespace Resiliencia.Testes;

public class Lab10Tests
{
    private static readonly DateOnly Hoje = new(2026, 11, 2);

    private static (CadernoDeProximoPasso? Caderno, string Saida) Rodar(params string[] respostas)
    {
        var entrada = new StringReader(string.Join(Environment.NewLine, respostas));
        var saida = new StringWriter();
        var caderno = new Roteiro(entrada, saida, Hoje).Executar();
        return (caderno, saida.ToString());
    }

    [Fact]
    public void Uma_sessao_completa_monta_o_resumo_inteiro()
    {
        var (caderno, saida) = Rodar(
            "2",                                        // mais ou menos
            "Tenho uma tarefa atrasada do curso",
            "medo", "5",
            "Ainda não comecei a primeira parte",       // fato
            "Vou reprovar no curso inteiro",            // história
            "Começar o primeiro exercício", "1",        // depende de mim
            "Beto",
            "Resolver a lista de exercícios", "50");

        Assert.NotNull(caderno);
        Assert.Contains("Como me sinto: medo (5/10).", saida);
        Assert.Contains("Uma pausa de 2 min", saida);
        Assert.Contains("Fatos: 1 · Histórias que preciso checar: 1.", saida);
        Assert.Contains("Onde pôr energia primeiro: Começar o primeiro exercício.", saida);
        Assert.Contains("Próximo passo possível: Resolver a lista de exercícios (parte 1 de 2) (25 min).", saida);
        Assert.Contains("Quem pode apoiar: Beto.", saida);
    }

    [Fact]
    public void Pular_tudo_funciona_e_nao_quebra()
    {
        var (caderno, saida) = Rodar("", "", "", "", "", "", "", "", "", "");

        Assert.NotNull(caderno);
        Assert.Contains("Situação: uma tarefa que quero organizar", saida);
        Assert.Null(caderno!.Sentimento);
        Assert.Null(caderno.Meta);
    }

    [Fact]
    public void Quem_nao_esta_bem_recebe_os_servicos_e_pode_parar_sem_cobranca()
    {
        var (caderno, saida) = Rodar("3", "n");

        Assert.Null(caderno);
        Assert.Contains("188", saida);
        Assert.Contains("192", saida);
        Assert.Contains("quando você quiser", saida);
        Assert.DoesNotContain("Situação:", saida);
    }

    [Fact]
    public void Quem_nao_esta_bem_pode_escolher_continuar()
    {
        var (caderno, _) = Rodar("3", "s", "Organizar a mesa", "", "", "", "", "", "", "");
        Assert.NotNull(caderno);
        Assert.Equal("Organizar a mesa", caderno!.Situacao);
    }

    [Fact]
    public void Intensidade_fora_da_escala_e_ajustada_em_vez_de_quebrar()
    {
        var (caderno, _) = Rodar("1", "x", "raiva", "99");
        Assert.Equal(10, caderno!.Sentimento!.Intensidade);
        Assert.Equal(Emocao.Raiva, caderno.Sentimento.Emocao);
    }
}
```

Quatro testes merecem destaque:

- **`Pular_tudo_funciona_e_nao_quebra`**: uma pessoa que só aperta Enter recebe um resumo válido, sem exceção.
- **`Quem_nao_esta_bem_recebe_os_servicos_e_pode_parar_sem_cobranca`**: confirma que aparecem **188** e **192**, que **nada** do caderno é perguntado e que o texto de despedida **não cobra**.
- **`Quem_nao_esta_bem_pode_escolher_continuar`**: o cuidado **não** vira tutela; a pessoa decide.
- **`Intensidade_fora_da_escala_e_ajustada_em_vez_de_quebrar`**: 99 vira 10 sem exceção.

::: dica Rode, quebre, conserte
Um bom exercício: apague uma linha do `Roteiro` (por exemplo, o `return null` quando a pessoa não quer continuar) e rode `dotnet test`. O teste que falha **conta** o que você acabou de prometer à pessoa que usa o programa. É a melhor forma de sentir o valor de um teste.
:::

## Para ir além

::: exercicio Desafios do laboratório
**1.** Acrescente ao `Resumo()` uma linha com `Conversa.Gerar(...)` do capítulo 94, usando a situação e **duas** lentes.

**2.** Crie, no `Roteiro`, uma pergunta opcional "Que evidência você tem de que esse passo é possível?" usando o `QuadroDeEvidencias` do capítulo 91.

**3.** Faça o `Roteiro` oferecer **trocar a pausa** sugerida por uma pausa de verdade: `PausaConsciente` com `TimeProvider.System`, imprimindo "Faltam X s" a cada 5 segundos (cuidado: é opcional, e **cancelável** com Enter).

**4.** **Privacidade**: escreva um teste que garanta que o programa **não cria nenhum arquivo** durante uma sessão completa (dica: compare `Directory.GetFiles` antes e depois).

--- resposta
**1.** No `Resumo()`, depois do bloco do sentimento:

```cs Trecho do desafio 1
var situacao = new Situacao(Situacao);
foreach (var pergunta in Conversa.Gerar(situacao, Lentes.Padrao[0], Lentes.Padrao[1]).Take(2))
    linhas.Add("  ? " + pergunta);
```

**2.** Peça o texto e a fonte; se ambos existirem, crie `new Evidencia(texto, fonte)` e registre com `quadro.Possibilidade(...)`. Depois mostre `QuadroDeEvidencias.Mensagem(quadro.Ler())` no resumo.

**3.** Use `Task.Delay` (ou `Console.KeyAvailable` em um laço) para checar o `Enter` e `pausa.Restante` para o texto. Lembre-se de oferecer um **"pular a pausa"**.

**4.** O teste cria um diretório temporário, define-o como diretório atual (`Directory.SetCurrentDirectory`), roda uma sessão completa e verifica `Assert.Empty(Directory.GetFiles(temp))`. Privacidade é um comportamento: **teste-o**.
:::

::: resumo
- O laboratório reúne **check-in, sentimento, fato/história, foco, mochila, apoio e passos pequenos** em um só programa.
- Receber **`TextReader` e `TextWriter`** de fora deixa a conversa inteira **testável**, sem teclado.
- **`init`**, **`is { } s`** e **desconstrução de tuplas** deixam o código curto e legível.
- Três compromissos guiam o design: **privacidade** (só memória), **autonomia** (tudo pode ser pulado) e **cuidado** (urgência leva aos serviços, e a pessoa escolhe continuar).
- Testes **protegem o que foi prometido** à pessoa que usa o programa, inclusive o tom.
:::

::: quiz
? Por que o `Roteiro` recebe `TextReader` e `TextWriter` em vez de usar `Console` diretamente?
+ Para que a conversa inteira possa ser testada sem teclado nem tela
- Porque `Console` não existe em .NET 8
- Para gravar a conversa em arquivo
- Para deixar o código mais lento
> Entrada e saída injetadas dão controle total aos testes.

? O que significa `if (Sentimento is { } s)`?
+ Se `Sentimento` não for nulo, use o valor na variável `s`
- Se `Sentimento` for vazio, crie `s`
- Compare `Sentimento` com `s`
- Declare uma classe `s`
> É um property pattern que testa e captura em uma linha.

? Como o caderno trata uma pergunta que a pessoa não quer responder?
+ Aceita o Enter vazio como "pular" e segue sem cobrar
- Repete a pergunta até receber algo
- Encerra com erro
- Preenche com um valor padrão invisível
> Autonomia: silêncio nunca é erro.

? O que acontece quando a pessoa diz "não estou bem"?
+ Recebe acolhimento e os serviços de apoio, e escolhe se quer continuar
- O programa fecha imediatamente
- O programa a obriga a continuar
- O programa ignora a resposta
> Cuidado sem tutela.

? Onde ficam os dados digitados durante uma sessão?
+ Somente na memória; nada é gravado nem enviado
- Em um arquivo de texto na pasta do usuário
- Em um banco de dados na nuvem
- Em um servidor da empresa
> Privacidade por construção: o que não é guardado não pode vazar.

? Para que serve `Math.Clamp(n, 0, 10)` na leitura da intensidade?
+ Ajustar um valor fora da escala para o limite mais próximo, sem lançar exceção
- Sortear um número entre 0 e 10
- Arredondar para o inteiro mais próximo
- Converter o número em texto
> Em vez de repreender, o programa ajusta.
:::

::: cartas
Caderno de um próximo passo :: Programa de console que organiza uma situação em perguntas e um passo pequeno, sem gravar nada.
TextReader / TextWriter :: Abstrações de entrada e saída de texto; permitem trocar teclado e tela por memória nos testes.
init :: Propriedade que só pode ser definida na criação do objeto.
Property pattern :: Padrão como `is { } s`, que testa e captura um valor não nulo.
Math.Clamp :: Limita um número a um intervalo.
Privacidade por construção :: Não guardar o que não é necessário, para que não possa vazar.
Autonomia :: Poder pular, parar ou escolher, em qualquer etapa.
Cuidado sem tutela :: Oferecer apoio sem decidir pela pessoa.
:::
