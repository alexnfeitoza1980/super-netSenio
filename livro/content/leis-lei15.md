---
id: leis-lei15
order: 76
kind: capitulo
num: 56
group: C# & Mentalidade · Parte 12 · As 16 leis do sucesso em C#
title: Lei 15 — Tolerância
subtitle: Seja liberal no que aceita e rigoroso no que envia. Leitor tolerante com System.Text.Json, números em várias culturas, compatibilidade com o futuro — e os limites da tolerância (segurança).
minutes: 55
hue: 130
objectives:
- Entender a lei da **tolerância** como capacidade de lidar com o diferente sem perder o foco
- Aplicar o **princípio do leitor tolerante** (*be liberal in what you accept, conservative in what you send*) ao JSON
- Tratar **cultura e formatos** de números/datas sem ambiguidade escondida
- Projetar **compatibilidade** com versões futuras (campos desconhecidos, enums novos)
- Reconhecer onde a tolerância **deve acabar**: validação de caminhos e segurança
origem: livros/03-leis-do-sucesso-csharp/content/75-lei15.md
---

::: vila O cliente que mandou "30" entre aspas
Uma nova integração começa a falhar às 2h da manhã: o parceiro passou a enviar a idade como `"30"` (texto) e acrescentou um campo novo, `"programaFidelidade"`. O nosso código, **intolerante**, rejeitou tudo — milhares de requisições. Lia ajusta o leitor para aceitar as duas formas e **ignorar** o que não conhece. O sistema volta em dez minutos; e na reunião seguinte ela escreve na parede: *"A rigidez do outro lado não é problema meu — a minha é."*
:::

::: ilustracao img/leis/lei15.svg | Tolerância: ler com folga (maiúsculas, vírgulas sobrando) e gravar sempre no formato canônico.
:::

## A lei, em uma frase

> Você pode ser brilhante no que faz, mas se **não souber lidar com pessoas** — críticas, opiniões contrárias, jeitos diferentes — seu talento será neutralizado por conflitos. Seja **compassivo, solidário e tolerante**: **ninguém é superior, ninguém é inferior** — todos somos singulares; você é livre para ser quem é e livre para permitir que os outros sejam quem são.

A lição dá três conselhos que traduzem bem para engenharia: **(1)** não perca energia reagindo a opiniões e ataques — **uma opinião é apenas uma opinião**; **(2)** descubra **seu caminho do meio**, definindo valores essenciais e agindo por eles; **(3)** o intolerante vive atolado em conflitos e dúvidas — o tolerante mantém a **calma e a solicitude** diante das diferenças.

## Da vida para o código

| O que a lição diz | O que isso significa para quem programa |
|---|---|
| Aceitar o diferente | **Leitor tolerante**: aceitar variações de formato (maiúsculas, texto/número, campos extras) |
| Não reagir a cada crítica | Tratar *warnings*/revisões como **informação**, não como ataque |
| Cada um é singular | **Culturas** diferentes: separador decimal, calendário, idioma |
| Não ser superior/inferior | Interoperar com **qualquer plataforma/linguagem** sem preconceito |
| Calma diante do inesperado | **Compatibilidade futura**: ignorar campos desconhecidos, enums novos |
| Caminho do meio, valores essenciais | **Tolerância na forma, rigor no essencial** (segurança, invariantes) |

::: figura O funil da tolerância
<svg viewBox="0 0 740 200" role="img" aria-label="Muitas formas de entrada entram num normalizador; saída canônica e rigorosa">
  <g font-size="12">
    <rect class="f-plain" x="10" y="20" width="170" height="30" rx="6"/><text x="95" y="40" text-anchor="middle" class="t-mono">"idade": "30"</text>
    <rect class="f-plain" x="10" y="60" width="170" height="30" rx="6"/><text x="95" y="80" text-anchor="middle" class="t-mono">"Idade": 30</text>
    <rect class="f-plain" x="10" y="100" width="170" height="30" rx="6"/><text x="95" y="120" text-anchor="middle" class="t-mono">// comentário, </text>
    <rect class="f-plain" x="10" y="140" width="170" height="30" rx="6"/><text x="95" y="160" text-anchor="middle" class="t-mono">"novoCampo": {…}</text>
  </g>
  <path class="arrow" d="M182 35L290 90"/><path class="arrow" d="M182 75L290 95"/><path class="arrow" d="M182 115L290 105"/><path class="arrow" d="M182 155L290 112"/>
  <path d="M280 70L370 92V108L280 130Z" fill="var(--accent-soft, #ffe9b3)" stroke="var(--warn)"/>
  <text x="318" y="104" text-anchor="middle" font-size="12" font-weight="800">leitor</text>
  <rect class="f-good" x="440" y="70" width="270" height="60" rx="12"/><text x="575" y="96" text-anchor="middle" font-weight="800">Objeto canônico e válido</text><text x="575" y="116" text-anchor="middle" font-size="11" class="t-dim">tipos precisos · regras de negócio rigorosas</text>
  <path class="arrow" d="M372 100H438"/>
  <text x="95" y="190" text-anchor="middle" class="t-dim" font-size="11">entrada: tolerante</text><text x="575" y="190" text-anchor="middle" class="t-dim" font-size="11">saída e núcleo: rigorosos</text>
</svg>
:::

## 1) O leitor tolerante com `System.Text.Json`

Na internet, o **princípio da robustez** (de Jon Postel) diz: *"seja conservador no que você envia e liberal no que você aceita"*. Para APIs, isso significa: **leia** de forma flexível; **escreva** de forma canônica. Com `System.Text.Json`:

```cs Jornada/Lei15/ClienteDto.cs
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Jornada.Lei15;

public enum StatusCliente { Desconhecido, Regular, Vip }

/// <summary>Um enum tolerante: valores novos (que ainda não conhecemos) viram 'Desconhecido' em vez de quebrar tudo.</summary>
public sealed class StatusTolerante : JsonConverter<StatusCliente>
{
    public override StatusCliente Read(ref Utf8JsonReader reader, Type tipo, JsonSerializerOptions opcoes)
    {
        var texto = reader.TokenType == JsonTokenType.String ? reader.GetString() : null;
        return Enum.TryParse<StatusCliente>(texto, ignoreCase: true, out var status) && Enum.IsDefined(status)
            ? status
            : StatusCliente.Desconhecido;
    }

    public override void Write(Utf8JsonWriter writer, StatusCliente valor, JsonSerializerOptions opcoes) =>
        writer.WriteStringValue(valor.ToString());
}

public sealed class ClienteDto
{
    public string? Nome { get; set; }
    public int Idade { get; set; }
    public bool Ativo { get; set; }
    public StatusCliente Status { get; set; }

    /// <summary>Tudo que não conhecemos é preservado aqui (útil para reenviar ou investigar), em vez de descartado.</summary>
    [JsonExtensionData]
    public Dictionary<string, JsonElement>? Extras { get; set; }
}

public static class LeitorDeClientes
{
    private static readonly JsonSerializerOptions Leitura = new()
    {
        PropertyNameCaseInsensitive = true,                       // "Nome", "nome", "NOME"
        NumberHandling = JsonNumberHandling.AllowReadingFromString, // "30" é aceito como 30
        ReadCommentHandling = JsonCommentHandling.Skip,            // // comentários
        AllowTrailingCommas = true,                                // vírgula sobrando
        Converters = { new StatusTolerante() }
    };

    private static readonly JsonSerializerOptions Escrita = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,         // saída sempre no mesmo formato
        Converters = { new StatusTolerante() }
    };

    public static ClienteDto? Ler(string json) => JsonSerializer.Deserialize<ClienteDto>(json, Leitura);

    public static string Escrever(ClienteDto cliente) => JsonSerializer.Serialize(cliente, Escrita);
}
```

```cs Uso
var entradas = new[]
{
    """{"nome":"Ana","idade":30,"ativo":true,"status":"vip"}""",
    """{"Nome":"Bia","Idade":"28","ativo":false,"status":"PLATINA","programaFidelidade":{"nivel":2}}""",
    """
    {
      // cliente vindo do sistema legado
      "nome": "Caio", "idade": 41, "ativo": true,
    }
    """
};

foreach (var json in entradas)
{
    var c = LeitorDeClientes.Ler(json)!;
    Console.WriteLine($"{c.Nome,-5} idade={c.Idade} ativo={c.Ativo} status={c.Status} extras={c.Extras?.Count ?? 0}");
}

Console.WriteLine(LeitorDeClientes.Escrever(new ClienteDto { Nome = "Bia", Idade = 28, Ativo = false, Status = StatusCliente.Desconhecido }));
```

```saida
Ana   idade=30 ativo=True status=Vip extras=0
Bia   idade=28 ativo=False status=Desconhecido extras=1
Caio  idade=41 ativo=True status=Desconhecido extras=0
{"nome":"Bia","idade":28,"ativo":false,"status":"Desconhecido"}
```

Observe o que o leitor **tolerou**: nomes em caixas diferentes, número escrito como texto, comentários, vírgula final, status desconhecido (`"PLATINA"`) e um campo novo (`programaFidelidade`). Mesmo assim, **o que sai é sempre canônico** — mesmo formato, mesmos nomes (e, se `Extras` estivesse preenchido, os campos desconhecidos seriam reenviados junto — decida se é isso que você quer).

::: cuidado Tolerância tem custo e limites
O princípio de Postel é **controverso**: ser liberal demais pode **esconder erros** (aceitar lixo sem avisar), **ambiguidade** e até **vulnerabilidades** (interpretar a mesma entrada de dois jeitos diferentes em dois sistemas). Boas práticas: **tolere a forma, não o significado**; **registre** o que foi tolerado (um *log* de aviso quando cair em `Desconhecido`); **valide** o resultado normalizado (Lei 11); e **nunca** seja tolerante em questões de **segurança**.
:::

## 2) Cultura: o ponto e a vírgula

Para quem é do Brasil, `1.234,56` é "mil duzentos e trinta e quatro e cinquenta e seis". Para quem é dos EUA, `1,234.56` é a **mesma** coisa. E `1.234` é "mil duzentos e trinta e quatro" aqui — mas "um vírgula duzentos e trinta e quatro" lá! Um leitor tolerante precisa **escolher uma regra e documentá-la**:

```cs Jornada/Lei15/NumerosTolerantes.cs
using System.Globalization;

namespace Jornada.Lei15;

public static class NumerosTolerantes
{
    /// <summary>
    /// Aceita "1.234,56", "1,234.56", "R$ 10,50", "10". O ÚLTIMO separador (vírgula ou ponto) é o decimal.
    /// Atenção: um texto com UM só separador e três dígitos depois (como "1.234") é lido como decimal (1,234).
    /// </summary>
    public static bool TryParse(string? texto, out decimal valor)
    {
        valor = 0;
        if (string.IsNullOrWhiteSpace(texto)) return false;

        var t = texto.Trim().Replace(" ", "").Replace("R$", "").Replace("$", "");

        int ultimaVirgula = t.LastIndexOf(',');
        int ultimoPonto   = t.LastIndexOf('.');

        if (ultimaVirgula > ultimoPonto)                    // 1.234,56 → vírgula é o decimal
            t = t.Replace(".", "").Replace(',', '.');
        else if (ultimoPonto > ultimaVirgula)               // 1,234.56 → ponto é o decimal
            t = t.Replace(",", "");

        return decimal.TryParse(t, NumberStyles.Number, CultureInfo.InvariantCulture, out valor);
    }
}
```

```cs JornadaTests/Lei15/NumerosTolerantesTests.cs
using Jornada.Lei15;
using Xunit;

public class NumerosTolerantesTests
{
    [Theory]
    [InlineData("1.234,56", 1234.56)]
    [InlineData("1,234.56", 1234.56)]
    [InlineData("R$ 10,50", 10.50)]
    [InlineData("10",       10)]
    [InlineData("-3,5",     -3.5)]
    public void Entende_varios_formatos(string texto, double esperado)
    {
        Assert.True(NumerosTolerantes.TryParse(texto, out var valor));
        Assert.Equal((decimal)esperado, valor);
    }

    [Theory]
    [InlineData("abc")]
    [InlineData("")]
    [InlineData(null)]
    public void Rejeita_o_que_nao_e_numero(string? texto) =>
        Assert.False(NumerosTolerantes.TryParse(texto, out _));
}
```

::: pense A ambiguidade de "1.234"
Nenhuma heurística resolve tudo: `"1.234"` é ambíguo e nossa regra o lê como **1,234**. Isso é aceitável **se estiver documentado** e **se o contexto permitir** (por exemplo, a fonte nunca usa separador de milhar). Quando o erro custa caro (dinheiro!), a melhor tolerância é **pedir que a fonte use um formato inequívoco** (por exemplo, texto invariante `1234.56` ou inteiros de centavos) — **tolerar o que é seguro e rejeitar o ambíguo** é parte do "caminho do meio".
:::

## 3) Compatibilidade com o futuro

A lei fala de **calma diante do inesperado**. Sistemas duram mais do que as suas versões; então projete para que **mudanças do outro lado não derrubem o seu lado**:

| Situação | Atitude tolerante |
|---|---|
| Chega um **campo novo** no JSON | Ignorar (ou guardar em `[JsonExtensionData]`) |
| Chega um **valor novo de enum** | Mapear para `Desconhecido` + *log* de aviso |
| Um **campo some** (opcional) | Usar valor padrão sensato |
| Você precisa **mudar** sua própria API | **Adicionar** campos (não remover/renomear); versionar (`/v2`); marcar o antigo com `[Obsolete]` |
| Dois sistemas com **versões** diferentes | Negociar versão; manter a anterior por um prazo |

E o **inverso** vale para a sua saída: o que **você** envia deve ser estável, previsível e documentado — o "conservador no que envia".

## 4) Onde a tolerância deve acabar: segurança

Há áreas em que ser liberal é perigoso. Um exemplo clássico: **caminhos de arquivo**. Aceitar `..` em um caminho enviado pelo usuário permite sair da pasta permitida (*path traversal*). Aqui o "caminho do meio" é: **tolerar o separador** (`/` ou `\`), mas **ser rigoroso** quanto ao **destino final**:

```cs Jornada/Lei15/PastaSegura.cs
namespace Jornada.Lei15;

public static class PastaSegura
{
    /// <summary>Resolve 'relativo' dentro de 'raiz' e recusa qualquer caminho que escape dela.</summary>
    public static string Resolver(string raiz, string relativo)
    {
        var raizCompleta = Path.GetFullPath(raiz);
        var alvo = Path.GetFullPath(Path.Combine(raizCompleta, relativo.Replace('\\', '/')));   // tolera \ e /

        var prefixo = raizCompleta.EndsWith(Path.DirectorySeparatorChar) ? raizCompleta : raizCompleta + Path.DirectorySeparatorChar;
        if (!alvo.StartsWith(prefixo, StringComparison.Ordinal))
            throw new UnauthorizedAccessException("O caminho está fora da pasta permitida.");

        return alvo;
    }
}
```

```cs Uso
var raiz = Path.Combine(Path.GetTempPath(), "dados-do-cliente");

Console.WriteLine(Path.GetFileName(PastaSegura.Resolver(raiz, "relatorios/2026\\outubro.txt")));   // aceita os dois separadores

foreach (var tentativa in new[] { "../segredo.txt", "relatorios/../../segredo.txt" })
{
    try { PastaSegura.Resolver(raiz, tentativa); }
    catch (UnauthorizedAccessException e) { Console.WriteLine($"'{tentativa}' → {e.Message}"); }
}
```

```saida
outubro.txt
'../segredo.txt' → O caminho está fora da pasta permitida.
'relatorios/../../segredo.txt' → O caminho está fora da pasta permitida.
```

A regra de ouro desta seção: **normalize primeiro** (`Path.GetFullPath`), **valide depois** (prefixo da pasta permitida) — nunca valide a entrada "crua". O mesmo vale para URLs, SQL (use **parâmetros**: Lei 16), HTML (*encode*) e comandos de sistema.

## 5) Tolerar críticas: informação, não ataque

A primeira parte da lição é sobre **pessoas**: *"uma opinião é apenas uma opinião"*. Em times, isso significa **separar o conteúdo do tom**:

- Um comentário de *code review* ríspido ainda pode conter um **bug real**. Extraia a informação, ignore o tom e responda com **evidência** (um teste, uma medição).
- Um *warning* do compilador não é "ofensa": é **dado**.
- Não responda a críticas ao seu código com críticas à pessoa (Lei 10) — nem concorde automaticamente. O "caminho do meio" é: **ouvir, avaliar, decidir com critério**.

::: exercicio Números contábeis
Notas fiscais costumam escrever valores negativos entre parênteses: `(1.234,56)` significa **−1234,56**. Escreva `NumerosContabeis.TryParse` que aceite esse formato (reaproveitando `NumerosTolerantes.TryParse`) e teste com `"(1.234,56)"` e `"(10)"`.

--- resposta
```cs
using Jornada.Lei15;

public static class NumerosContabeis
{
    public static bool TryParse(string? texto, out decimal valor)
    {
        valor = 0;
        if (string.IsNullOrWhiteSpace(texto)) return false;

        var t = texto.Trim();
        bool negativo = t.StartsWith('(') && t.EndsWith(')');
        if (negativo) t = t[1..^1];

        if (!NumerosTolerantes.TryParse(t, out valor)) return false;
        if (negativo) valor = -valor;
        return true;
    }
}

public class NumerosContabeisTests
{
    [Theory]
    [InlineData("(1.234,56)", -1234.56)]
    [InlineData("(10)",       -10)]
    [InlineData("1.234,56",   1234.56)]
    public void Parenteses_significam_negativo(string texto, double esperado)
    {
        Assert.True(NumerosContabeis.TryParse(texto, out var v));
        Assert.Equal((decimal)esperado, v);
    }
}
```
Perceba a composição: nova tolerância **sem tocar** na anterior (OCP, Lei 2).
:::

::: resumo
- **Lei 15:** saiba lidar com o diferente — críticas, opiniões, formatos — com calma e solicitude; ninguém é superior nem inferior; defina seu **caminho do meio**.
- **Leitor tolerante** (`System.Text.Json`): `PropertyNameCaseInsensitive`, `AllowReadingFromString`, comentários, vírgula final, enum desconhecido, `[JsonExtensionData]`; **saída canônica**.
- **Cultura:** documente a regra de separadores; trate o **ambíguo** com cuidado (ou peça formato inequívoco).
- **Compatibilidade futura:** ignore o desconhecido, não remova campos, versione a API.
- **Limites:** em segurança (caminhos, SQL, HTML), **normalize primeiro e valide depois**; tolere a **forma**, nunca o **significado perigoso**.
- Critiques e *warnings* são **informação**; responda com evidências.
:::

::: quiz
? O que diz o princípio da robustez (Postel)?
+ Seja conservador no que envia e liberal no que aceita
- Seja liberal em tudo
- Seja rigoroso no que aceita
- Nunca envie nada
> Leitura flexível, escrita canônica.

? Para que serve `[JsonExtensionData]`?
+ Guardar os campos desconhecidos do JSON em vez de descartá-los
- Criptografar o JSON
- Validar o schema
- Ordenar propriedades
> Útil para compatibilidade futura e diagnóstico.

? Por que mapear um enum desconhecido para `Desconhecido` pode ser melhor do que lançar exceção?
+ Porque novos valores do outro lado não derrubam o sistema (e podem ser registrados em log)
- Porque enums não aceitam novos valores
- Porque exceções são proibidas
- Porque é mais rápido
> Tolerância ao futuro, com aviso.

? Qual é o risco de ser tolerante demais?
+ Esconder erros, criar ambiguidades e abrir brechas de segurança
- Deixar o código mais curto
- Melhorar a performance
- Nenhum
> Tolere a forma, não o significado.

? Em validação de caminhos, qual a ordem correta?
+ Normalizar (GetFullPath) e só depois validar o prefixo da pasta permitida
- Validar o texto cru e depois normalizar
- Apenas remover ".."
- Aceitar tudo
> Evita path traversal.

? O que a lição sugere ao receber uma crítica ríspida?
+ Extrair a informação útil, ignorar o tom e responder com evidências
- Revidar
- Ignorar sempre
- Concordar sem pensar
> "Uma opinião é apenas uma opinião."
:::

::: cartas
Tolerância :: Capacidade de lidar com o diferente com calma e solicitude, sem abrir mão do essencial.
Princípio da robustez :: Seja liberal no que aceita e conservador no que envia.
Leitor tolerante :: Parser que aceita variações e ignora o desconhecido.
JsonExtensionData :: Atributo que captura propriedades JSON não mapeadas.
Compatibilidade futura :: Projetar para que mudanças do outro lado não quebrem o sistema.
Path traversal :: Ataque que usa ".." para sair da pasta permitida.
Cultura (CultureInfo) :: Conjunto de convenções de formato (números, datas) de uma região.
Caminho do meio :: Tolerância na forma e rigor no essencial.
:::
