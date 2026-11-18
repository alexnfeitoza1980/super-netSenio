---
id: amor-lab12
order: 148
kind: lab
num: 12
group: C# & Mentalidade · Parte 17 · Você é aquilo que ama, em C#
title: Laboratório: Auditoria litúrgica
subtitle: Um programa de console que lê um arquivo JSON seu (prioridades, registros de tempo, ritmos do time) e devolve perguntas curiosas, sem julgar, sem gravar e sem enviar nada.
minutes: 100
hue: 335
objectives:
- **Reunir** peças dos capítulos 108 (o que se diz × o que se faz) e 112 (os ritmos do time) em uma auditoria
- Ler dados de um arquivo com **`System.Text.Json`**: opções, `JsonStringEnumConverter` e *raw string literals*
- Tratar **falhas esperadas** de leitura (arquivo escrito à mão) com mensagens que ajudam
- Garantir **privacidade** (tudo local), **autonomia** (tudo é opcional) e **tom** (perguntas, não notas)
origem: livros/08-voce-e-aquilo-que-ama-csharp/content/136-lab12.md
---

::: ilustracao img/amor/lab12.svg | A Auditoria litúrgica: um arquivo seu, perguntas curiosas e tudo só no seu computador.
:::

## O que vamos construir

Ao longo do livro, encontramos perguntas parecidas: **o que eu digo que importa e para onde meu tempo vai? Que histórias os ritmos do meu time contam? O que está faltando, sem culpa?** Vamos reuni-las em uma **Auditoria litúrgica**: um programa que lê **um arquivo JSON que você escreve** (suas prioridades, os registros do seu tempo, os ritmos do seu time) e responde com **observações e perguntas**.

Três compromissos guiam o desenho:

1. **Privacidade.** O programa lê **um arquivo do seu computador**, mostra o resultado **na tela** e **não grava nem envia** nada. Sem contas, sem rede.
2. **Autonomia.** Tudo é **opcional**: se você não preencher as prioridades, ou os ritmos, ou ambos, o programa **pula a parte** e segue. Nada de "campo obrigatório".
3. **Tom.** As frases **perguntam** ("o que está no caminho?"), e **nunca** acusam. Um teste garante que palavras como "culpa", "fracasso" e "deveria" **não aparecem**.

::: cuidado Antes de começar
Use dados **seus** e **leves** (tempo por área de vida, ritmos de trabalho). **Não** coloque informações sobre outras pessoas, dados sensíveis ou qualquer coisa que você não gostaria de ver impressa em uma tela. O programa **não é** um instrumento de avaliação psicológica, de produtividade ou de desempenho; é uma **conversa consigo** (ou com o seu time) sobre o que os hábitos mostram. Se olhar para esses números lhe causar sofrimento, **feche o programa** e converse com alguém de confiança.
:::

O código completo está em [codigo/amor/Habitos.Console](codigo/amor/Habitos.Console/) (o `Program.cs`) e [codigo/amor/Habitos.Capitulos/Lab12](codigo/amor/Habitos.Capitulos/Lab12/) (a lógica). Abra a solução `codigo/amor/Habitos.sln` no Visual Studio ou rode com o .NET 8 SDK:

```bash
dotnet run --project codigo/amor/Habitos.Console
dotnet run --project codigo/amor/Habitos.Console -- meu-diario.json
```

O primeiro comando usa o **exemplo embutido**; o segundo, o **seu arquivo**.

::: figura Como as peças se encaixam
<svg viewBox="0 0 760 250" role="img" aria-label="Um arquivo JSON local é lido pelo Carregador, que produz um DiarioDaSemana; o Auditor usa a comparação do capítulo 108 e o calendário do capítulo 112 para gerar as linhas do relatório, exibidas na tela">
  <rect x="14" y="90" width="130" height="70" rx="12" class="f-warn"/><text x="79" y="120" text-anchor="middle" font-weight="800">meu-diario.json</text><text x="79" y="140" text-anchor="middle" font-size="11" class="t-dim">no seu computador</text>
  <rect x="180" y="90" width="150" height="70" rx="12" class="f-info"/><text x="255" y="120" text-anchor="middle" font-weight="800">Carregador</text><text x="255" y="140" text-anchor="middle" font-size="11" class="t-dim">System.Text.Json</text>
  <rect x="370" y="90" width="130" height="70" rx="12" class="f-prim"/><text x="435" y="120" text-anchor="middle" font-weight="800">Auditor</text><text x="435" y="140" text-anchor="middle" font-size="11" class="t-dim">junta e pergunta</text>
  <rect x="540" y="90" width="200" height="70" rx="12" class="f-good"/><text x="640" y="120" text-anchor="middle" font-weight="800">Tela (Console)</text><text x="640" y="140" text-anchor="middle" font-size="11" class="t-dim">nada gravado, nada enviado</text>
  <path class="arrow" d="M146 125H178"/><path class="arrow" d="M332 125H368"/><path class="arrow" d="M502 125H538"/>
  <g font-size="12" text-anchor="middle">
    <rect x="330" y="10" width="170" height="40" rx="10" class="f-good" opacity=".7"/><text x="415" y="35">Cap. 108 · Revelação (GroupJoin)</text>
    <rect x="330" y="190" width="170" height="40" rx="10" class="f-good" opacity=".7"/><text x="415" y="215">Cap. 112 · Calendário do time</text>
  </g>
  <path class="arrow" d="M435 52V88"/><path class="arrow" d="M435 188V162"/>
</svg>
:::

## Passo 1: ler o arquivo com `System.Text.Json`

O arquivo é escrito **à mão** (por você), e quem escreve à mão erra uma vírgula. Então a leitura é uma **falha esperada**, tratada como nos capítulos do Livro 6: sem susto, com mensagem útil.

Algumas peças novas do C#:

- **`record` como molde dos dados.** O `DiarioDaSemana` é um `record` com três listas **anuláveis** (`IReadOnlyList<...>?`): cada parte pode faltar. O `System.Text.Json` sabe criar `record`s pelo **construtor**, casando os nomes das propriedades com os do JSON.
- **`JsonSerializerOptions`**: `PropertyNameCaseInsensitive` (aceita `area` ou `Area`), `AllowTrailingCommas` (perdoa uma vírgula sobrando), `ReadCommentHandling.Skip` (aceita **comentários** `//`), e **`JsonStringEnumConverter`**, que lê `"Alinhar"` e `"Monday"` como **enums** (em vez de números).
- **`DateOnly`**: o `System.Text.Json` do .NET 8 lê `"2026-11-02"` direto como `DateOnly`.
- **`"""..."""`**: um *raw string literal* (C# 11), que permite escrever o JSON de exemplo **como ele é**, com aspas e quebras de linha, sem escapes.
- **`catch (JsonException e)`**: um JSON quebrado lança `JsonException`; a mensagem do programa diz a **linha** e dá pistas ("confira vírgulas, aspas e nomes").

O `Auditor` vem logo em seguida: junta a **comparação do capítulo 108** com o **calendário do capítulo 112**, e termina sempre com **uma pergunta** para levar.

```cs Jornada/Habitos/Lab12/Auditoria.cs
using System.Text.Json;
using System.Text.Json.Serialization;
using Jornada.Habitos.Cap108;
using Jornada.Habitos.Cap112;

namespace Jornada.Habitos.Lab12;

/// <summary>Os dados de UMA semana, como a pessoa os escreveu num arquivo JSON dela. Nada sai do computador.</summary>
public sealed record DiarioDaSemana(
    IReadOnlyList<Prioridade>? Prioridades,
    IReadOnlyList<Registro>? Registros,
    IReadOnlyList<Pratica>? Praticas);

public static class Carregador
{
    private static readonly JsonSerializerOptions Opcoes = new()
    {
        PropertyNameCaseInsensitive = true,          // "area" ou "Area": tanto faz
        AllowTrailingCommas = true,
        ReadCommentHandling = JsonCommentHandling.Skip,
        Converters = { new JsonStringEnumConverter() }  // "Alinhar" em vez de 0; "Monday" em vez de 1
    };

    /// <summary>Lê o JSON. Arquivo escrito à mão erra com facilidade, então a falha é esperada: devolvemos um aviso claro.</summary>
    public static bool TentarCarregar(string json, out DiarioDaSemana? diario, out string? problema)
    {
        diario = null;
        problema = null;

        try
        {
            diario = JsonSerializer.Deserialize<DiarioDaSemana>(json, Opcoes);
            if (diario is null)
                problema = "O arquivo está vazio.";
        }
        catch (JsonException e)
        {
            problema = $"Não consegui ler o arquivo (linha {e.LineNumber! + 1}): confira vírgulas, aspas e nomes.";
        }

        return diario is not null;
    }

    public static string Exemplo => """
        {
          // Tudo aqui é só um exemplo. Troque pelos seus dados, ou apague o que preferir não dizer.
          "prioridades": [
            { "area": "Família", "ordem": 1 },
            { "area": "Saúde",   "ordem": 2 },
            { "area": "Estudo",  "ordem": 3 },
            { "area": "Trabalho","ordem": 4 }
          ],
          "registros": [
            { "dia": "2026-11-02", "area": "Trabalho",      "minutos": 540 },
            { "dia": "2026-11-02", "area": "Redes sociais", "minutos": 80 },
            { "dia": "2026-11-03", "area": "Trabalho",      "minutos": 520 },
            { "dia": "2026-11-03", "area": "Estudo",        "minutos": 45 },
            { "dia": "2026-11-04", "area": "Família",       "minutos": 90 },
            { "dia": "2026-11-04", "area": "Redes sociais", "minutos": 100 },
            { "dia": "2026-11-05", "area": "Saúde",         "minutos": 30 }
          ],
          "praticas": [
            { "nome": "Daily", "intencao": "Alinhar", "minutos": 15, "dias": ["Monday", "Tuesday", "Wednesday", "Thursday", "Friday"] },
            { "nome": "Demo",  "intencao": "Entregar", "minutos": 45, "dias": ["Friday"] }
          ]
        }
        """;
}

public static class Auditor
{
    /// <summary>
    /// Monta o relatório juntando as peças dos capítulos 108 (o que se diz × o que se faz)
    /// e 104 (as histórias que os ritmos contam). Só observa: não dá nota, não compara pessoas.
    /// </summary>
    public static IReadOnlyList<string> Auditar(DiarioDaSemana diario)
    {
        var linhas = new List<string>();

        if (diario.Prioridades is { Count: > 0 } prioridades && diario.Registros is { Count: > 0 } registros)
        {
            var relatorio = Revelacao.Comparar(prioridades, registros);

            linhas.Add($"Tempo registrado: {relatorio.TotalDeMinutos / 60.0:0.#} h.");
            linhas.AddRange(relatorio.Declaradas.Select(Revelacao.Descrever));

            foreach (var area in relatorio.NaoDeclaradas)
                linhas.Add($"{area.Area}: apareceu em {area.Parte:P0} do tempo sem estar na sua lista. Quer dar um nome (e um lugar) a isso?");
        }
        else
        {
            linhas.Add("Sem prioridades ou registros nesta semana: tudo bem, pule esta parte.");
        }

        if (diario.Praticas is { Count: > 0 } praticas)
        {
            var calendario = praticas.Aggregate(new CalendarioDoTime(), (c, p) => c.Adicionar(p));

            foreach (var (intencao, minutos) in calendario.MinutosPorSemana().OrderByDescending(p => p.Value))
                linhas.Add($"Ritmo: {intencao} recebe {minutos} min por semana.");

            if (calendario.Lacunas() is { Count: > 0 } lacunas)
                linhas.Add($"Nenhum ritmo cuida de: {string.Join(", ", lacunas)}. Quer incluir algo pequeno?");
        }

        linhas.Add("Pergunta para levar: qual prática pequena você repetiria esta semana, sabendo o que ela vai formar?");
        return linhas;
    }
}
```

Detalhes que merecem atenção:

- **`is { Count: > 0 } prioridades`**: um **padrão de propriedade** que testa "não é nulo **e** tem pelo menos um item" e já **captura** a lista em `prioridades`. Se faltar qualquer parte, o programa **pula** a seção correspondente.
- **`relatorio.Declaradas.Select(Revelacao.Descrever)`**: passa o método **direto** (*method group*), sem lambda, como no exercício do capítulo 106.
- **`praticas.Aggregate(new CalendarioDoTime(), (c, p) => c.Adicionar(p))`**: monta o calendário **dobrando** a lista (o `Adicionar` devolve `this`, e por isso encadeia).
- **`foreach (var (intencao, minutos) in ...)`**: **desconstrução** de pares do dicionário.

## Passo 2: o `Program.cs`

Com a lógica em outro projeto, o programa de console é curto: escolhe a **fonte** do JSON (o exemplo ou o arquivo informado), **tenta carregar**, e imprime. Se algo falha, mostra o aviso e **encerra com código 1** (convenção para "deu problema").

```cs Habitos.Console/Program.cs
using Jornada.Habitos.Lab12;

Console.OutputEncoding = System.Text.Encoding.UTF8;

Console.WriteLine("Auditoria litúrgica: o que o seu calendário diz que você ama?");
Console.WriteLine("Tudo é lido do seu computador e nada é gravado nem enviado.");
Console.WriteLine();

string json;
if (args.Length == 0)
{
    Console.WriteLine("(Sem arquivo informado: usando o exemplo. Para usar o seu: dotnet run -- meu-diario.json)");
    json = Carregador.Exemplo;
}
else if (File.Exists(args[0]))
{
    json = File.ReadAllText(args[0]);
}
else
{
    Console.WriteLine($"Não encontrei o arquivo \"{args[0]}\".");
    return 1;
}

if (!Carregador.TentarCarregar(json, out var diario, out var problema))
{
    Console.WriteLine(problema);
    return 1;
}

foreach (var linha in Auditor.Auditar(diario!))
    Console.WriteLine("• " + linha);

return 0;
```

Esta é a auditoria do **exemplo embutido** (uma semana fictícia):

```cs Uso
if (Carregador.TentarCarregar(Carregador.Exemplo, out var diario, out var problema))
{
    foreach (var linha in Auditor.Auditar(diario!))
        Console.WriteLine("• " + linha);
}
else
{
    Console.WriteLine(problema);
}
```

```saida
• Tempo registrado: 23,4 h.
• Família: você a pôs em 1º, mas ela aparece em 2º (6% do tempo). O que está no caminho?
• Saúde: você a pôs em 2º, mas ela aparece em 4º (2% do tempo). O que está no caminho?
• Estudo: o que você disse e o que o calendário mostra caminham juntos (3% do tempo).
• Trabalho: você a pôs em 4º e ela aparece em 1º (75% do tempo). Isso é uma escolha ou um acaso?
• Redes sociais: apareceu em 13% do tempo sem estar na sua lista. Quer dar um nome (e um lugar) a isso?
• Ritmo: Alinhar recebe 75 min por semana.
• Ritmo: Entregar recebe 45 min por semana.
• Nenhum ritmo cuida de: Aprender, Cuidar, Celebrar. Quer incluir algo pequeno?
• Pergunta para levar: qual prática pequena você repetiria esta semana, sabendo o que ela vai formar?
```

Leia como uma **conversa**, e não como um boletim. O trabalho ocupa três quartos do tempo, e a família (a primeira prioridade) aparece em segundo, com 6%. O programa **não conclui nada**: pergunta "**o que está no caminho?**". Em "Redes sociais", que não estava na lista, ele pergunta se você quer "dar um nome (e um lugar) a isso". Nos ritmos, nota que **nada cuida de "Aprender", "Cuidar" e "Celebrar"** e deixa um convite.

E o que acontece com um arquivo **quebrado** (com um `]` no lugar de `}`):

```cs Uso
var quebrado = "{ \"prioridades\": [ { \"area\": \"Família\", \"ordem\": 1 ] }";

if (!Carregador.TentarCarregar(quebrado, out _, out var problema))
    Console.WriteLine(problema);
```

```saida
Não consegui ler o arquivo (linha 1): confira vírgulas, aspas e nomes.
```

Uma mensagem **clara**, e **sem um traço de culpa**: o erro está no arquivo, e não em quem o escreveu.

## Passo 3: os testes do laboratório

Os testes usam o **exemplo embutido** como dado de teste, e confirmam o comportamento, inclusive o **tom**:

```cs Habitos.Testes/Lab12Tests.cs
using Jornada.Habitos.Cap112;
using Jornada.Habitos.Lab12;

namespace Habitos.Testes;

public class Lab12Tests
{
    [Fact]
    public void O_exemplo_do_livro_e_um_json_valido_com_comentarios_e_virgulas_soltas()
    {
        Assert.True(Carregador.TentarCarregar(Carregador.Exemplo, out var diario, out var problema), problema);

        Assert.Equal(4, diario!.Prioridades!.Count);
        Assert.Equal(7, diario.Registros!.Count);
        Assert.Equal(Intencao.Alinhar, diario.Praticas![0].Intencao);
        Assert.Contains(DayOfWeek.Friday, diario.Praticas[1].Dias);
        Assert.Equal(new DateOnly(2026, 11, 2), diario.Registros[0].Dia);
    }

    [Fact]
    public void Json_quebrado_vira_um_aviso_claro_em_vez_de_um_susto()
    {
        Assert.False(Carregador.TentarCarregar("{ \"prioridades\": [ { \"area\": \"X\" ", out var diario, out var problema));

        Assert.Null(diario);
        Assert.Contains("confira vírgulas", problema);
    }

    [Fact]
    public void Texto_vazio_ou_nulo_nao_quebra()
    {
        Assert.False(Carregador.TentarCarregar("null", out _, out var problema));
        Assert.Contains("vazio", problema);
    }

    [Fact]
    public void Valor_de_enum_desconhecido_tambem_e_tratado_como_problema_de_leitura()
    {
        var json = """{ "praticas": [ { "nome": "x", "intencao": "Inventada", "minutos": 5, "dias": ["Monday"] } ] }""";
        Assert.False(Carregador.TentarCarregar(json, out _, out var problema));
        Assert.NotNull(problema);
    }

    [Fact]
    public void A_auditoria_junta_o_que_se_diz_o_que_se_faz_e_os_ritmos()
    {
        Carregador.TentarCarregar(Carregador.Exemplo, out var diario, out _);

        var linhas = Auditor.Auditar(diario!);

        Assert.Contains(linhas, l => l.StartsWith("Tempo registrado:"));
        Assert.Contains(linhas, l => l.StartsWith("Família:") && l.Contains("O que está no caminho?"));
        Assert.Contains(linhas, l => l.StartsWith("Redes sociais:") && l.Contains("sem estar na sua lista"));
        Assert.Contains(linhas, l => l.StartsWith("Ritmo: Alinhar recebe 75 min"));
        Assert.Contains(linhas, l => l.Contains("Nenhum ritmo cuida de: Aprender, Cuidar, Celebrar"));
        Assert.EndsWith("formar?", linhas[^1]);
    }

    [Fact]
    public void Quem_nao_preenche_nada_recebe_um_convite_e_nao_uma_cobranca()
    {
        var linhas = Auditor.Auditar(new DiarioDaSemana(null, null, null));

        Assert.Equal(2, linhas.Count);
        Assert.Contains("pule esta parte", linhas[0]);
    }

    [Fact]
    public void Nenhuma_frase_do_relatorio_julga_a_pessoa()
    {
        Carregador.TentarCarregar(Carregador.Exemplo, out var diario, out _);

        foreach (var linha in Auditor.Auditar(diario!))
        {
            var texto = linha.ToLowerInvariant();
            foreach (var palavra in new[] { "culpa", "fracasso", "preguiça", "deveria", "falhou" })
                Assert.DoesNotContain(palavra, texto);
        }
    }
}
```

Três testes valem um destaque:

- **`Json_quebrado_vira_um_aviso_claro_em_vez_de_um_susto`**: o programa **nunca** lança exceção para quem usa; devolve uma **mensagem**.
- **`Quem_nao_preenche_nada_recebe_um_convite_e_nao_uma_cobranca`**: um `DiarioDaSemana` **sem nada** produz um relatório simpático, e não um erro.
- **`Nenhuma_frase_do_relatorio_julga_a_pessoa`**: as frases de **todo** o relatório são varridas por palavras que **não** devem aparecer. O tom é parte do comportamento, e **comportamento se testa**.

::: dica Rode, quebre, conserte
Um bom exercício: troque, em `Revelacao.Descrever`, "O que está no caminho?" por "Você falhou." e rode `dotnet test`. O teste `Nenhuma_frase_do_relatorio_julga_a_pessoa` **falha**, e o próprio nome do teste explica o que você prometeu a quem usa o programa. É a melhor maneira de sentir o valor de **testar o tom**.
:::

## Para ir além

::: exercicio Desafios do laboratório
**1.** No `Auditor`, acrescente uma linha com a **próxima ocorrência** de cada prática do time, usando `ProximaOcorrencia` do exercício do capítulo 112.

**2.** Use o **`Habito`** do capítulo 107: aceite um campo opcional `habitos` no JSON, com o nome e as datas de prática de cada um, e mostre, para cada hábito, se **já é automático** e **quantos dias sem praticar** o deixariam de ser (`DiasAteDeixarDeSerAutomatico`).

**3.** Acrescente ao JSON uma lista opcional `cartoes` (frente e verso) e mostre **quantos cartões vencem hoje** em uma `CaixaDeLeitner` do capítulo 113.

**4.** **Privacidade:** escreva um teste que garanta que a auditoria **não cria nenhum arquivo** (dica: crie um diretório temporário, defina-o como diretório atual com `Directory.SetCurrentDirectory`, rode a auditoria e confira que `Directory.GetFiles` devolve vazio).

--- resposta
**1.** Depois de montar o `calendario`, dentro do `if` das práticas:

```cs Trecho do desafio 1
var hoje = new DateOnly(2026, 11, 2);   // ou DateOnly.FromDateTime(DateTime.Today)
foreach (var pratica in calendario.Praticas)
    if (calendario.ProximaOcorrencia(pratica.Nome, hoje) is { } proxima)
        linhas.Add($"Próximo: {pratica.Nome} em {proxima.Dia:dd/MM}.");
```

**2.** Crie um `record HabitoNoDiario(string Nome, IReadOnlyList<DateOnly> Dias)` e um campo `IReadOnlyList<HabitoNoDiario>? Habitos` no `DiarioDaSemana`. Para cada um, `Formacao.PraticarVarias(new Habito(nome), dias)` devolve o hábito; `Automatico` e `DiasAteDeixarDeSerAutomatico()` completam a frase. Lembre de redigir como **pergunta ou observação**, e nunca como cobrança.

**3.** Adicione `record CartaoNoDiario(string Frente, string Verso)`, monte a caixa com `Adicionar(frente, verso, hoje)` e mostre `caixa.Devidos(hoje).Count`.

**4.** O teste cria uma pasta temporária (`Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString())`), faz `Directory.SetCurrentDirectory`, roda `Auditor.Auditar(...)` e verifica `Assert.Empty(Directory.GetFiles(pasta))`. Privacidade é um comportamento: **teste-o**.
:::

::: resumo
- A auditoria reúne **o que se diz × o que se faz** (cap. 108) e **os ritmos do time** (cap. 112) em um relatório de **perguntas curiosas**.
- **`System.Text.Json`** lê o arquivo: `record` como molde, opções (`PropertyNameCaseInsensitive`, comentários, vírgulas soltas), **`JsonStringEnumConverter`** e `DateOnly`; *raw string literals* escrevem o exemplo sem escapes.
- Falhas de leitura são **esperadas**: viram mensagens que ajudam, e **nunca** exceções para quem usa.
- Três compromissos guiam o desenho: **privacidade** (tudo local), **autonomia** (tudo opcional) e **tom** (perguntas, não notas), e **o tom também se testa**.
- Rituais, como a auditoria, só formam se forem **escolhidos**: use o programa como **espelho curioso**, e não como régua.
:::

::: quiz
? Por que a leitura do arquivo JSON é tratada como "falha esperada"?
+ Porque um arquivo escrito à mão pode ter erros, e isso deve gerar um aviso claro, não um susto
- Porque o JSON nunca funciona
- Porque o C# não lê arquivos
- Porque erros não existem
> A mensagem diz a linha e dá pistas de correção.

? O que faz o `JsonStringEnumConverter`?
+ Lê enums como texto (`"Monday"`) em vez de números
- Converte enums em listas
- Criptografa os dados
- Remove comentários
> Assim o JSON fica legível para quem escreve à mão.

? O que significa `is { Count: > 0 } prioridades`?
+ Se a lista não é nula e tem pelo menos um item, guarde-a em `prioridades`
- Se a lista é nula, crie uma nova
- Conte as prioridades
- Ordene a lista
> É um padrão de propriedade que testa e captura em uma só expressão.

? Qual é o papel do teste `Nenhuma_frase_do_relatorio_julga_a_pessoa`?
+ Garantir que o tom do relatório seja de pergunta, sem culpa nem acusação
- Verificar a velocidade do programa
- Contar palavras do relatório
- Testar a internet
> O tom é parte do comportamento prometido.

? Onde ficam os dados lidos e o relatório gerado?
+ Só na memória e na tela: nada é gravado nem enviado
- Em um servidor da empresa
- Em um banco de dados na nuvem
- Em um arquivo oculto
> Privacidade por construção: o que não é guardado não pode vazar.

? Como o programa trata quem não preencheu nada no arquivo?
+ Com um convite a pular essa parte, sem cobrança
- Com uma mensagem de erro
- Com uma nota baixa
- Encerrando com código de erro
> Autonomia: tudo é opcional.
:::

::: cartas
Auditoria litúrgica :: Autoexame curioso: que histórias meus (ou nossos) ritmos contam?
System.Text.Json :: Biblioteca padrão do .NET para ler e escrever JSON.
JsonStringEnumConverter :: Lê e escreve enums como texto no JSON.
Raw string literal :: Texto entre `"""` que permite aspas e quebras de linha sem escapes (C# 11).
Padrão de propriedade :: `is { Count: > 0 } x`: testa e captura um valor em uma expressão.
Method group :: Passar um método direto, no lugar de uma lambda (`Select(Revelacao.Descrever)`).
Privacidade por construção :: Não guardar o que não é necessário, para que não possa vazar.
Tom testado :: Garantir por teste que as mensagens não acusam nem julgam.
:::
