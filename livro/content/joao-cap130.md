---
id: joao-cap130
order: 168
kind: capitulo
num: 130
group: C# & Mentalidade · Parte 18 · O Evangelho segundo João em C#
title: A porta e a voz do pastor
subtitle: As ovelhas seguem a voz que conhecem e fogem da voz dos estranhos. Em C#, primeiro conferir a voz; depois, uma pergunta nossa (e se alguém a repetir?) e como uma assinatura responde.
minutes: 75
hue: 208
objectives:
- Ler o discurso do **pastor e das ovelhas** (Jo 10,1-5 e 10,27) frase por frase
- Transformar "ouvem sua voz" e "não conhecem a voz dos estranhos" em código simples
- Separar o que **João 10 diz** da **extensão pedagógica** (a imitação da voz) e conhecer a solução: uma **assinatura** (HMAC)
origem: livros/10-evangelho-joao-csharp/content/107-cap130.md
---

::: vila O crachá que qualquer um copiava
O sistema de acesso do prédio guardava o crachá de cada pessoa num banco: um "número" a ser consultado a cada porta. Bia mostrou o problema: "se alguém **inventa um número**, a porta o aceita. E toda porta tem que **chamar o banco**". A alternativa: o crachá trazia **um selo** que só o prédio sabia produzir. A porta só **conferia o selo**.
:::

::: ilustracao img/joao/cap130.svg | A voz que só quem tem o segredo produz: a porta confere a assinatura em tempo constante, e só então olha quem é e se ainda vale.
:::

## O texto

> - **Jo 10,1** Em verdade, em verdade vos digo, que aquele que no curral das ovelhas não entra pela porta, mas sobe por outra parte, é ladrão, e assaltante. Mas aquele que entra pela porta é o pastor de ovelhas. A este o porteiro abre, e as ovelhas ouvem sua voz, e a suas ovelhas chama nome por nome, e as leva fora. E quando tira fora suas ovelhas, vai adiante delas, e as ovelhas o seguem, porque conhecem sua voz. Mas ao estranho em maneira nenhuma seguirão, ao invés disso dele fugirão; porque não conhecem a voz dos estranhos.
> - **Jo 10,27** Minhas ovelhas ouvem minha voz, e eu as conheço, e elas me seguem.
>
> *Bíblia Livre (CC BY 4.0)*

## Lendo devagar

- Quem **não entra pela porta** no curral das ovelhas, mas sobe por outra parte, é **ladrão e assaltante** (10,1). Quem **entra pela porta** é o pastor de ovelhas (10,2).
- O porteiro abre ao pastor; as ovelhas **ouvem sua voz**; ele **chama as suas ovelhas nome por nome** e as leva para fora (10,3).
- Elas o **seguem**, "porque **conhecem sua voz**" (10,4).
- De um **estranho**, elas não seguem: **fogem**, "porque **não conhecem a voz dos estranhos**" (10,5).
- Em 10,27 o mesmo ponto: "Minhas ovelhas **ouvem minha voz**, e eu **as conheço**, e elas me seguem".

A ideia central do texto: **reconhecer pela voz**. A ovelha não precisa de crachá nem de lista: basta **ouvir e conferir** se é a voz que conhece.

::: dica O que é do texto e o que é nosso
- **Está em João 10:** as ovelhas conhecem a voz do pastor, o seguem, e fogem da voz dos estranhos.
- **Não está em João 10:** a ideia de alguém **imitar** a voz do pastor, ou de provar por uma **assinatura** que uma voz é verdadeira. Essa é uma **extensão nossa**, feita para ensinar um problema real de programação (autenticar quem fala). Vamos marcá-la quando chegar.
:::

## Do versículo ao código

### Passo 1: "as ovelhas ouvem sua voz... porque conhecem sua voz"

Uma ovelha **guarda a voz que conhece**. Em C#, é uma classe com um campo que guarda essa voz (aqui, um texto):

```cs
public class Ovelha
{
    private readonly string vozQueConhece;

    public Ovelha(string vozQueConhece) => this.vozQueConhece = vozQueConhece;
}
```

### Passo 2: "ao estranho em maneira nenhuma seguirão"

Ao ouvir uma voz, a ovelha **compara** com a que conhece: se for a mesma, segue; se não, foge. Em C#, comparar dois textos é `==`:

```cs
public bool Segue(string voz) => voz == vozQueConhece;      // "não conhecem a voz dos estranhos"
```

### O código completo, rodando

```cs Jornada/Joao/Texto/Cap130Pastor.cs
namespace Jornada.Joao.Texto;

// Jo 10,3-5 (Bíblia Livre): "[...] as ovelhas ouvem sua voz, e a suas ovelhas chama nome por nome, e as leva fora. [...]
//  Mas ao estranho em maneira nenhuma seguirão, ao invés disso dele fugirão; porque não conhecem a voz dos estranhos."
//
// TEXTO     : as ovelhas seguem a voz que conhecem e fogem da voz dos estranhos.
// ANALOGIA  : "conhecer a voz" vira "conferir se a voz é a que se conhece".
// LIMITE    : o texto NÃO fala de alguém imitar a voz do pastor. Essa pergunta (e a assinatura que a resolve) é uma
//             EXTENSÃO nossa, para ensinar um problema real de programação; não está em João 10.

// <ovelha>
public class Ovelha
{
    private readonly string vozQueConhece;

    public Ovelha(string vozQueConhece) => this.vozQueConhece = vozQueConhece;

    // "não conhecem a voz dos estranhos": só segue a voz que conhece
    public bool Segue(string voz) => voz == vozQueConhece;
}
// </ovelha>
```

```bash
dotnet run --project codigo/joao/Joao.Console -- exemplo t130
```

```cs Uso
var ovelha = new Ovelha("voz do pastor");

Console.WriteLine($"Segue a voz do pastor? {ovelha.Segue("voz do pastor")}");
Console.WriteLine($"Segue a voz de um estranho? {ovelha.Segue("voz de um estranho")}");

// EXTENSÃO NOSSA (não está em João 10): e se o estranho repetir a voz que ouviu?
string vozOuvidaPeloEstranho = "voz do pastor";
Console.WriteLine($"Segue um estranho que repete a voz? {ovelha.Segue(vozOuvidaPeloEstranho)}");
```

```saida
Segue a voz do pastor? True
Segue a voz de um estranho? False
Segue um estranho que repete a voz? True
```

As duas primeiras linhas funcionam como o texto: a ovelha segue a voz do pastor e não segue a de um estranho.

### Passo 3 (extensão nossa): e se o estranho repetir a voz?

::: dica Extensão pedagógica: isto não está em João 10
A **terceira linha** mostra um problema de **programação**: o estranho **ouviu** a voz e a **repetiu igualzinha**, e a ovelha o seguiu. O código só compara textos; não tem como saber se **quem falou é mesmo** o pastor. O texto de João 10 **não** trata disso (nele, a ovelha conhece a voz de verdade). Usamos o código aqui para **pensar em um problema real**: reconhecer quem fala, em um sistema, sem ser enganado.
:::

Como resolver? Fazendo com que a "voz" leve junto uma **assinatura** que **só o pastor consegue produzir**, mesmo que todo mundo possa **ler** o que foi dito. Quem ouve **confere a assinatura**. É o que as próximas seções constroem, com um recurso chamado **HMAC**.

::: cuidado Limite da analogia
- **"Conhecer a voz" no texto é relacional** ("eu as conheço, e elas me seguem", 10,27); no código, é **comparar dados**. A assinatura **não** é uma versão melhor de "conhecer": é só um jeito de **verificar** uma identidade quando não há relação.
- **A imitação da voz e a assinatura são extensões nossas**, e não de João 10.
- **Código de segurança é só de estudo.** O que vem a seguir ensina a ideia; em um sistema de verdade, use bibliotecas consolidadas e guarde segredos com cuidado. Uma voz assinada **não é secreta**: qualquer um pode ler o que ela diz. A assinatura só garante **quem falou** e que **nada foi alterado**.
:::

::: curiosidade Lupa no grego
- **φωνή** (*phōnē*, "voz") aparece **15 vezes** em João, **5** em Jo 10: 10,3.4.5.16.27 (`onde phōnē`).
- **ποιμήν** (*poimēn*, "pastor", G4166) aparece **6 vezes**, **todas** em Jo 10; **θύρα** (*thyra*, "porta, entrada", G2374) aparece **7 vezes**: 10,1.2.7.9; 18,16; 20,19.26 (`onde poimēn`, `onde thyra`).
- Em 10,3, o texto grego diz *ta idia probata phōnei kat' onoma*: "chama as suas ovelhas **nome por nome**" (SBLGNT).
- **ἀλλότριος** (*allotrios*, "estranho, de fora", G245) aparece **2 vezes**, ambas em 10,5; **μισθωτός** (*misthōtos*, "assalariado") aparece **2 vezes**, em 10,12-13 (`onde allotrios`, `onde misthōtos`).
- **ἀκολουθέω** (*akoloutheō*, "seguir") aparece **19 vezes** (`onde akoloutheō`).

*Fontes: texto grego SBLGNT (CC BY 4.0); contagens por lema do MorphGNT (CC BY-SA 3.0), reproduzíveis com `dotnet run --project codigo/joao/Joao.Console -- onde <palavra>`; sentidos do dicionário de Strong (OpenScriptures), com a entrada indicada.*
:::


## Indo além: a voz assinada

O segredo do pastor é um valor que **só ele conhece**. Com ele, calcula-se uma **assinatura** para cada "voz" emitida. A porta, que conhece o mesmo segredo, **recalcula** a assinatura e compara: se bater, a voz é do pastor; se não, é de estranho. As seções abaixo mostram o código completo e **dizem o que falta** para uso real.

::: figura Anatomia de uma voz
<svg viewBox="0 0 740 220" role="img" aria-label="Um token tem corpo em base64url, um ponto e a assinatura HMAC; a porta recalcula a assinatura, compara em tempo constante, confere a expiração e aplica a política">
  <rect x="20" y="30" width="330" height="50" rx="10" class="f-info"/><text x="185" y="60" text-anchor="middle" font-weight="800">corpo = base64url("nome|perfil|expira")</text>
  <text x="362" y="62" font-size="22" font-weight="800">.</text>
  <rect x="380" y="30" width="340" height="50" rx="10" class="f-warn"/><text x="550" y="60" text-anchor="middle" font-weight="800">assinatura = HMAC-SHA256(segredo, corpo)</text>
  <g font-size="12" text-anchor="middle">
    <rect x="20" y="118" width="150" height="62" rx="12" class="f-prim"/><text x="95" y="144" font-weight="800">1. Formato?</text><text x="95" y="162" class="t-dim">senão: Malformado</text>
    <rect x="190" y="118" width="170" height="62" rx="12" class="f-prim"/><text x="275" y="144" font-weight="800">2. Selo bate?</text><text x="275" y="162" class="t-dim">tempo constante · Estranho</text>
    <rect x="380" y="118" width="150" height="62" rx="12" class="f-prim"/><text x="455" y="144" font-weight="800">3. Ainda vale?</text><text x="455" y="162" class="t-dim">senão: Expirado</text>
    <rect x="550" y="118" width="170" height="62" rx="12" class="f-good"/><text x="635" y="144" font-weight="800">4. Política</text><text x="635" y="162" class="t-dim">Permitido ou SemPermissao</text>
  </g>
  <path class="arrow" d="M172 149H188"/><path class="arrow" d="M362 149H378"/><path class="arrow" d="M532 149H548"/>
  <text x="370" y="208" text-anchor="middle" font-size="13" class="t-dim">A ordem importa: nada de ler o conteúdo antes de confirmar a assinatura.</text>
</svg>
:::

### 1) Assinar e conferir

Um **HMAC** (*hash-based message authentication code*) combina uma função de resumo (aqui **SHA-256**) com um **segredo**: a mesma mensagem e o mesmo segredo dão sempre o mesmo resultado; sem o segredo, é inviável forjar um valor que "bata". `HMACSHA256.HashData(segredo, dados)` faz isso numa linha.

O formato da nossa voz é `corpo.assinatura`. O **corpo** é `nome|perfil|expira` (expira em segundos Unix), codificado em **base64url**, uma variante do base64 que troca `+` e `/` por `-` e `_` e remove o `=` do final, para caber em uma URL ou cabeçalho.

O método `Reconhecer` segue uma ordem cuidadosa, e cada passo tem seu motivo:

1. **Formato**: sem exatamente duas partes, ou com base64 inválido, é `Malformado` (nunca uma exceção).
2. **Assinatura**: recalculamos o HMAC do corpo recebido e comparamos com `CryptographicOperations.FixedTimeEquals`. Se não bate, é `Estranho`. Só então abrimos o corpo.
3. **Campos e expiração**: três campos, expiração no futuro; se não, `Expirado`.
4. A `Porta` aplica por fim a **política** (`Func<Identidade, bool>`), devolvendo `Permitido` ou `SemPermissao`.

```cs Jornada/Joao/Cap130/Voz.cs
using System.Security.Cryptography;
using System.Text;

namespace Jornada.Joao.Cap130;

public sealed record Identidade(string Nome, string Perfil, DateTimeOffset ExpiraEm);

public enum Acesso { Permitido, Malformado, Estranho, Expirado, SemPermissao }

/// <summary>
/// A "voz": um pedaço de texto que só quem tem o segredo consegue produzir e que qualquer um com o segredo confere.
/// Formato: corpo.assinatura (ambos em base64url). Material de estudo: em produção, use bibliotecas consolidadas (como JWT).
/// </summary>
public sealed class Voz
{
    private readonly byte[] segredo;
    private readonly TimeProvider relogio;

    public Voz(byte[] segredo, TimeProvider? relogio = null)
    {
        if (segredo.Length < 16)
            throw new ArgumentException("O segredo precisa ter ao menos 16 bytes.", nameof(segredo));

        this.segredo = segredo;
        this.relogio = relogio ?? TimeProvider.System;
    }

    public string Emitir(string nome, string perfil, TimeSpan validade)
    {
        if (nome.Contains('|') || perfil.Contains('|'))
            throw new ArgumentException("Nome e perfil não podem conter o caractere '|'.");

        var expira = relogio.GetUtcNow().Add(validade).ToUnixTimeSeconds();
        var corpo = CodificarBase64Url(Encoding.UTF8.GetBytes($"{nome}|{perfil}|{expira}"));

        return $"{corpo}.{CodificarBase64Url(Assinar(corpo))}";
    }

    public Acesso Reconhecer(string? token, out Identidade? identidade)
    {
        identidade = null;

        var partes = token?.Split('.');
        if (partes is not { Length: 2 })
            return Acesso.Malformado;

        byte[] enviada;
        try { enviada = DecodificarBase64Url(partes[1]); }
        catch (FormatException) { return Acesso.Malformado; }

        // Comparação em tempo constante: o tempo gasto não revela quantos bytes já "bateram".
        if (!CryptographicOperations.FixedTimeEquals(Assinar(partes[0]), enviada))
            return Acesso.Estranho;                      // a voz não é a que conhecemos

        string[] campos;
        try { campos = Encoding.UTF8.GetString(DecodificarBase64Url(partes[0])).Split('|'); }
        catch (FormatException) { return Acesso.Malformado; }

        if (campos.Length != 3 || !long.TryParse(campos[2], out var segundos))
            return Acesso.Malformado;

        var expiraEm = DateTimeOffset.FromUnixTimeSeconds(segundos);
        if (relogio.GetUtcNow() >= expiraEm)
            return Acesso.Expirado;

        identidade = new Identidade(campos[0], campos[1], expiraEm);
        return Acesso.Permitido;
    }

    private byte[] Assinar(string dados) => HMACSHA256.HashData(segredo, Encoding.UTF8.GetBytes(dados));

    private static string CodificarBase64Url(byte[] bytes) =>
        Convert.ToBase64String(bytes).TrimEnd('=').Replace('+', '-').Replace('/', '_');

    private static byte[] DecodificarBase64Url(string texto)
    {
        var padrao = texto.Replace('-', '+').Replace('_', '/');
        padrao = padrao.PadRight(padrao.Length + (4 - padrao.Length % 4) % 4, '=');
        return Convert.FromBase64String(padrao);
    }
}

/// <summary>A porta: só deixa entrar quem tem uma voz reconhecida, e que passe na política desta porta.</summary>
public sealed class Porta
{
    private readonly Voz voz;

    public Porta(Voz voz) => this.voz = voz;

    public Acesso Entrar(string? token, Func<Identidade, bool>? politica = null)
    {
        var acesso = voz.Reconhecer(token, out var identidade);
        if (acesso != Acesso.Permitido)
            return acesso;

        return politica is null || politica(identidade!) ? Acesso.Permitido : Acesso.SemPermissao;
    }
}
```

::: dica Por que "tempo constante"?
Uma comparação comum (`==`) pode parar no **primeiro byte diferente**. Um atacante que mede o tempo da resposta descobre, byte a byte, quantos já estão certos (*timing attack*). `FixedTimeEquals` leva **o mesmo tempo** não importa onde esteja a diferença.
:::

```cs Uso
var voz = new Voz(Encoding.UTF8.GetBytes("um-segredo-longo-de-estudo-123"), relogio);
var porta = new Porta(voz);

var token = voz.Emitir("Ana", "cuidadora", TimeSpan.FromMinutes(30));
Console.WriteLine($"a voz começa assim: {token[..20]}...");

Console.WriteLine($"voz reconhecida: {porta.Entrar(token)}");
Console.WriteLine($"política exige perfil admin: {porta.Entrar(token, quem => quem.Perfil == "admin")}");

var deEstranho = new Voz(Encoding.UTF8.GetBytes("outro-segredo-qualquer-0000000"), relogio)
    .Emitir("Ana", "cuidadora", TimeSpan.FromMinutes(30));
Console.WriteLine($"voz de um estranho: {porta.Entrar(deEstranho)}");
Console.WriteLine($"texto qualquer: {porta.Entrar("não-sou-uma-voz")}");

relogio.Avancar(TimeSpan.FromMinutes(31));
Console.WriteLine($"31 minutos depois: {porta.Entrar(token)}");
```

```saida
a voz começa assim: QW5hfGN1aWRhZG9yYXwx...
voz reconhecida: Permitido
política exige perfil admin: SemPermissao
voz de um estranho: Estranho
texto qualquer: Malformado
31 minutos depois: Expirado
```

Cinco situações, cinco respostas: a voz de Ana é **reconhecida**; com uma política que exige o perfil `admin`, ela é reconhecida mas tem **SemPermissao**; uma voz feita com **outro segredo** é de **Estranho**; texto qualquer é **Malformado**; e 31 minutos depois, a voz de 30 minutos está **Expirada**. O token começa com `QW5hfGN1aWRhZG9yYXwx...`, que é só a codificação de `Ana|cuidadora|...`, **legível por qualquer um**.

::: cuidado Limites desta implementação (o que falta para uso real)
- **Formato próprio.** Em produção, use um padrão consolidado (por exemplo, JWT, com biblioteca) em vez de um formato inventado.
- **Sem revogação** e **sem proteção contra reuso**: quem copia uma voz válida pode usá-la até ela vencer.
- **Segredo fixo em memória**, sem rotação e sem armazenamento seguro.
- A **expiração** depende do relógio da máquina; sem sincronização, pode falhar.
- O corpo **não é secreto**: qualquer um lê o nome e o perfil.
:::

### 2) Testes

```cs Joao.Testes/Cap130Tests.cs
using System.Text;
using Jornada.Joao.Cap130;

namespace Joao.Testes;

public class Cap130Tests
{
    private static readonly byte[] Segredo = Encoding.UTF8.GetBytes("segredo-de-estudo-com-mais-de-16-bytes");

    private static (Voz Voz, RelogioDeTeste Relogio) Nova()
    {
        var relogio = new RelogioDeTeste();
        return (new Voz(Segredo, relogio), relogio);
    }

    [Fact]
    public void Uma_voz_emitida_e_reconhecida_com_nome_e_perfil()
    {
        var (voz, _) = Nova();

        var token = voz.Emitir("Ana", "cuidadora", TimeSpan.FromHours(1));

        Assert.Equal(Acesso.Permitido, voz.Reconhecer(token, out var quem));
        Assert.Equal("Ana", quem!.Nome);
        Assert.Equal("cuidadora", quem.Perfil);
    }

    [Fact]
    public void Voz_adulterada_e_estranha_mesmo_que_pareca_certa()
    {
        var (voz, _) = Nova();
        var token = voz.Emitir("Beto", "visitante", TimeSpan.FromHours(1));

        // o ladrão troca o corpo (o que diz quem é) mas não sabe refazer a assinatura
        var corpoFalso = Convert.ToBase64String(Encoding.UTF8.GetBytes("Beto|admin|9999999999")).TrimEnd('=');
        var falsificado = corpoFalso + "." + token.Split('.')[1];

        Assert.Equal(Acesso.Estranho, voz.Reconhecer(falsificado, out var quem));
        Assert.Null(quem);
    }

    [Fact]
    public void Voz_de_outro_pastor_com_outro_segredo_nao_e_reconhecida()
    {
        var (voz, _) = Nova();
        var outro = new Voz(Encoding.UTF8.GetBytes("outro-segredo-qualquer-0123456789"), new RelogioDeTeste());

        var token = outro.Emitir("Lia", "dev", TimeSpan.FromHours(1));

        Assert.Equal(Acesso.Estranho, voz.Reconhecer(token, out _));
    }

    [Fact]
    public void Com_o_tempo_a_voz_expira()
    {
        var (voz, relogio) = Nova();
        var token = voz.Emitir("Caio", "dev", TimeSpan.FromMinutes(10));

        relogio.Avancar(TimeSpan.FromMinutes(9));
        Assert.Equal(Acesso.Permitido, voz.Reconhecer(token, out _));

        relogio.Avancar(TimeSpan.FromMinutes(2));
        Assert.Equal(Acesso.Expirado, voz.Reconhecer(token, out _));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("sem-ponto")]
    [InlineData("a.b.c")]
    [InlineData("corpo.@@@")]
    public void Lixo_e_malformado_e_nunca_derruba_o_programa(string? token)
    {
        var (voz, _) = Nova();
        Assert.Equal(Acesso.Malformado, voz.Reconhecer(token, out _));
    }

    [Fact]
    public void Segredo_curto_demais_e_recusado() =>
        Assert.Throws<ArgumentException>(() => new Voz(new byte[8]));

    [Fact]
    public void Nome_com_separador_e_recusado_para_nao_forjar_campos()
    {
        var (voz, _) = Nova();
        Assert.Throws<ArgumentException>(() => voz.Emitir("Ana|admin", "dev", TimeSpan.FromHours(1)));
    }

    [Fact]
    public void A_porta_confere_a_voz_e_depois_a_politica()
    {
        var (voz, _) = Nova();
        var porta = new Porta(voz);
        var token = voz.Emitir("Ana", "cuidadora", TimeSpan.FromHours(1));

        Assert.Equal(Acesso.Permitido, porta.Entrar(token));
        Assert.Equal(Acesso.Permitido, porta.Entrar(token, id => id.Perfil == "cuidadora"));
        Assert.Equal(Acesso.SemPermissao, porta.Entrar(token, id => id.Perfil == "admin"));
        Assert.Equal(Acesso.Estranho, porta.Entrar(token + "x"));
        Assert.Equal(Acesso.Malformado, porta.Entrar(null));
    }
}
```

O teste **`Voz_adulterada_e_estranha_mesmo_que_pareca_certa`** altera um caractere e confirma o resultado. **`Lixo_e_malformado_e_nunca_derruba_o_programa`** é um `[Theory]` com `null`, texto vazio, pontos extras e base64 inválido. E **`Nome_com_separador_e_recusado_para_nao_forjar_campos`** protege contra uma fraude sutil: um nome com `|` poderia "esticar" os campos do corpo.

::: exercicio Políticas combináveis
Escreva a classe estática `Politicas` com dois métodos: `Perfil(params string[] perfis)`, que devolve uma política que aceita **qualquer um dos perfis**, e `Todas(params Func<Identidade, bool>[] politicas)`, que só aprova quem passar em **todas**.

--- resposta
```cs Jornada/Joao/Exercicios/Cap130Politicas.cs
using Jornada.Joao.Cap130;

namespace Jornada.Joao.Exercicios;

public static class Politicas
{
    public static Func<Identidade, bool> Perfil(params string[] perfisPermitidos) =>
        identidade => perfisPermitidos.Contains(identidade.Perfil);

    /// <summary>Junta políticas: só passa quem passar em todas.</summary>
    public static Func<Identidade, bool> Todas(params Func<Identidade, bool>[] politicas) =>
        identidade => politicas.All(p => p(identidade));
}
```

Ambas **devolvem funções**: funções de ordem superior. `Perfil` captura o array `perfisPermitidos` (uma *closure*) e `Todas` combina outras políticas com `All`. Assim você monta regras sem escrever uma nova classe: `Politicas.Todas(Politicas.Perfil("admin", "cuidadora"), quem => quem.Nome != "")`.
:::

::: resumo
- A ovelha **só segue a voz que conhece** (`voz == vozQueConhece`), como em Jo 10,5.
- A **imitação da voz** e a **assinatura** (HMAC) são **extensão nossa**: não estão em João 10.
- Em João, **phōnē** (voz) aparece 15 vezes (5 em Jo 10), **poimēn** 6 (todas em Jo 10) e **thyra** 7; em 10,3 o pastor **chama pelo nome**.
- **HMAC-SHA256** prova que os dados vêm de quem tem o segredo e não foram alterados; **não** os esconde.
- **base64url** é o base64 próprio para URLs e cabeçalhos.
- Compare assinaturas com `FixedTimeEquals`, que leva o mesmo tempo, e **confira a assinatura antes de ler o corpo**.
- Separe **identidade** (quem), **validade** (quando) e **política** (o que pode).
:::

::: quiz
? A ideia de alguém imitar a voz do pastor está em João 10?
+ Não: é uma extensão nossa, para ensinar um problema real de programação
- Sim, em 10,1
- Sim, em 10,5
- Sim, em 10,27
> João 10 diz que as ovelhas conhecem a voz do pastor e fogem da dos estranhos.

? O que a assinatura acrescenta à voz?
+ Uma prova de quem a emitiu, que só quem tem o segredo produz
- Esconde o conteúdo
- Deixa a voz mais alta
- Apaga a voz
> A assinatura não é segredo: é prova de origem.

? Quantas das 15 ocorrências de *phōnē* ("voz") em João estão em Jo 10?
+ Cinco
- Todas
- Nenhuma
- Duas
> 10,3.4.5.16.27.

? O que significa *phōnei kat᾽ onoma* em 10,3?
+ "Chama pelo nome"
- "Fecha a porta"
- "Fala baixo"
- "Conta as ovelhas"
> O pastor chama as suas ovelhas pelo nome.

? O que o HMAC garante em um token?
+ Que o conteúdo foi produzido por quem tem o segredo e não foi alterado
- Que o conteúdo é secreto
- Que o token nunca expira
- Que o usuário é administrador
> A assinatura não esconde o corpo.

? Por que usar `FixedTimeEquals` e não `==` para comparar assinaturas?
+ Para o tempo de resposta não revelar quantos bytes já batem
- Porque é mais curto
- Porque `==` não existe para arrays
- Para economizar memória
> Evita ataques de temporização.

? Qual a ordem correta em `Reconhecer`?
+ Formato, assinatura, campos/expiração e só então a política
- Política, expiração, assinatura
- Expiração, formato, política
- Assinatura, depois nada
> Nunca se confia no corpo antes de conferir a assinatura.

? O que é um token "Malformado"?
+ Um texto que não segue o formato, e que deve virar um resultado, não uma exceção
- Um token expirado
- Um token de administrador
- Um token vazio válido
> Entradas ruins não devem derrubar o programa.
:::

::: cartas
Voz do pastor (Jo 10,4-5) :: As ovelhas seguem a voz que conhecem e fogem da dos estranhos.
Aprisco (Jo 10) :: Discurso do pastor, da porta e das ovelhas que conhecem sua voz.
Phōnē :: "Voz": 15 vezes em João, 5 em Jo 10.
Thyra :: "Porta": 7 vezes; 10,1.2.7.9; 18,16; 20,19.26.
HMAC-SHA256 :: Selo calculado com um segredo; prova origem e integridade.
base64url :: Base64 sem `+`, `/` e `=`, apropriado para URLs.
FixedTimeEquals :: Comparação em tempo constante, que evita ataques de temporização.
Token assinado :: Corpo legível mais assinatura; prova quem emitiu, não esconde o conteúdo.
Política :: Regra que decide o que uma identidade reconhecida pode fazer.
:::
