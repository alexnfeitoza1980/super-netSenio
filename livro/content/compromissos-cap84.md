---
id: compromissos-cap84
order: 108
kind: capitulo
num: 84
group: C# & Mentalidade · Oficina · O Caderno dos Quatro Compromissos
title: O que fica apenas na memória pode se perder
subtitle: Uma experiência do tema, uma escolha e uma nova capacidade no mesmo programa.
minutes: 45
hue: 168
objectives:
- Compreender a ideia estudada e seu contexto na obra de Ruiz
- Construir a capacidade deste capítulo no Caderno
- Executar o cenário, variar uma entrada e explicar seu resultado
- Distinguir a proposta do autor das regras do software
origem: livros/05-quatro-compromissos-csharp/content/120-cap84.md
---

::: vila Uma página da nossa história
O caderno começou a ser útil. Lia registra uma reflexão e fecha o programa. Ao abrir uma nova execução, não há nada ali. O computador não a julgou nem perdeu a concentração: nós ainda não ensinamos o programa a guardar dados fora de seu processo. Essa falta cria a próxima necessidade.
:::

::: ilustracao img/compromissos/jornada84.svg | O que fica apenas na memória pode se perder: uma escolha visível e sua consequência no caderno.
:::

## A ideia que vamos experimentar

O texto trata da consciência e da prática ao transformar acordos. Um caderno externo é nossa aplicação pedagógica para revisitar escolhas. A obra não ensina armazenamento de dados nem afirma que lembranças humanas funcionam como RAM. Usamos a necessidade de retomar a prática para perguntar o que o programa precisa conservar.

**Fonte de leitura:** Don Miguel Ruiz, *Os Quatro Compromissos*, páginas físicas 43–44 do PDF fornecido. Este texto é uma explicação própria, não uma citação literal. A paginação de outras edições pode variar.

## Do tema para uma decisão no programa

Antes de gravar no disco, criamos um retrato transportável. `Retrato` reúne as coleções do caderno. `IArquivoDoCaderno` descreve duas operações: salvar e ler. A primeira implementação mantém JSON na memória. Ela serve para experimentar a fronteira entre objeto de trabalho e cópia salva, ainda sem durabilidade.

::: pense Antes de executar
Descreva o que você espera ver mudar. Qual informação já existe? Qual ainda não existe? O que o programa deve recusar? Escreva sua previsão antes de olhar a saída.
:::

## Vamos fazer acontecer

Execute **a partir da pasta deste livro**. Todos os comandos cap74 a cap87 existem no mesmo projeto. Os cenários começam com dados novos e fictícios para serem reproduzíveis; o diário pessoal persistente é apresentado no laboratório.

```bash
dotnet run --project codigo/compromissos/Caderno.Console -- cap84
```

O trecho abaixo está dentro do método `Cap84` em `codigo/compromissos/Caderno.Console/Exemplos.cs`. Usa `using Quatro;`. É um corpo de método, não um arquivo independente: não o cole ao lado das declarações das classes.

```cs
var caderno = new Caderno();
var promessa = caderno.Prometer("Conversar na sexta", new DateOnly(2026, 11, 6));
IArquivoDoCaderno arquivo = new ArquivoEmMemoria();
arquivo.Salvar(caderno.Exportar());
promessa.Cumprir(new DateOnly(2026, 11, 5));
Console.WriteLine($"Objeto de trabalho: {promessa.Estado}");
Console.WriteLine($"Retrato salvo antes: {arquivo.Ler()!.Promessas[0].Estado}");
```

### Leia o que o código faz

A interface não salva nada sozinha. `ArquivoEmMemoria` implementa o contrato serializando dados em uma string. Alterar uma promessa depois de salvar não modifica o JSON anterior. Ler desserializa uma nova cópia. Veja a definição dos DTOs em Caderno.cs: métodos não são gravados no retrato.

Observe os verbos do código: registrar, perguntar, escolher, cumprir, salvar. Cada um deve produzir uma mudança que você consiga explicar. Execute de novo: nos cenários, os dados são recriados. Para conservar sua própria prática, use os comandos do laboratório.

### A saída real desta experiência

```saida
Objeto de trabalho: Cumprida
Retrato salvo antes: Aberta
```

Compare sua previsão com a execução. Não basta obter a mesma tela: explique qual instrução produziu cada linha e qual objeto foi consultado. Troque apenas uma entrada por vez para observar a causa da diferença.

## Por dentro da capacidade

O arquivo de referência é `codigo/compromissos/Caderno.Core/Arquivo.cs`. Alguns membros serão usados nos próximos capítulos; hoje concentre-se no caminho percorrido pela experiência. A definição completa evita que o leitor precise adivinhar membros omitidos.

```cs
using System.Text.Json;
namespace Quatro;

public interface IArquivoDoCaderno
{
    void Salvar(Retrato retrato);
    Retrato? Ler();
}

public sealed class ArquivoJson(string caminho) : IArquivoDoCaderno
{
    private static readonly JsonSerializerOptions Opcoes = new() { WriteIndented = true };

    public void Salvar(Retrato retrato)
    {
        // Um processo, arquivo local: sem transação ou controle de concorrência.
        File.WriteAllText(caminho, JsonSerializer.Serialize(retrato, Opcoes));
    }

    public Retrato? Ler()
    {
        if (!File.Exists(caminho)) return null;
        var r = JsonSerializer.Deserialize<Retrato>(File.ReadAllText(caminho), Opcoes)
            ?? throw new InvalidDataException("O arquivo não contém um caderno.");
        if (r.Versao != 1) throw new InvalidDataException("Versão não suportada.");
        if (r.Acordos is null || r.Promessas is null || r.Reflexoes is null ||
            r.Respostas is null || r.Perguntas is null || r.Praticas is null)
            throw new InvalidDataException("Faltam coleções no arquivo.");
        return r;
    }
}

public sealed class ArquivoEmMemoria : IArquivoDoCaderno
{
    private string? json;
    public void Salvar(Retrato r) => json = JsonSerializer.Serialize(r);
    public Retrato? Ler() => json is null ? null : JsonSerializer.Deserialize<Retrato>(json);
}
```

```cs
namespace Quatro;

// DTOs: retratos transportáveis, separados dos objetos de trabalho.
public sealed record PromessaSalva(Guid Id, string Descricao, DateOnly Prazo,
    EstadoDaPromessa Estado, DateOnly? ConcluidaEm, string? MotivoDoCancelamento,
    RevisaoDePrazo[] Revisoes);
public sealed record PerguntaSalva(string Texto, EstadoDaPergunta Estado, Evidencia? Evidencia);
public sealed record Retrato(int Versao, Acordo[] Acordos, PromessaSalva[] Promessas,
    Reflexao[] Reflexoes, RespostaEscolhida[] Respostas, PerguntaSalva[] Perguntas, Pratica[] Praticas);

public sealed class Caderno
{
    public Inventario Inventario { get; } = new();
    public DiarioDePratica Diario { get; } = new();
    private readonly List<Promessa> promessas = [];
    private readonly List<Reflexao> reflexoes = [];
    private readonly List<RespostaEscolhida> respostas = [];
    private readonly List<Pergunta> perguntas = [];
    public IReadOnlyList<Promessa> Promessas => promessas.ToArray();
    public IReadOnlyList<Pergunta> Perguntas => perguntas.ToArray();

    public Promessa Prometer(string descricao, DateOnly prazo)
    {
        var p = new Promessa(descricao, prazo);
        promessas.Add(p);
        return p;
    }
    public Pergunta Perguntar(string texto)
    {
        var p = new Pergunta(texto);
        perguntas.Add(p);
        return p;
    }
    public void Refletir(Reflexao reflexao) => reflexoes.Add(reflexao ?? throw new ArgumentNullException(nameof(reflexao)));
    public void Responder(RespostaEscolhida resposta) => respostas.Add(resposta ?? throw new ArgumentNullException(nameof(resposta)));
    public IReadOnlyList<Promessa> Pendentes() => promessas
        .Where(p => p.Estado == EstadoDaPromessa.Aberta).OrderBy(p => p.Prazo)
        .ThenBy(p => p.Descricao, StringComparer.Ordinal).ToArray();

    public Retrato Exportar() => new(1, Inventario.Todos.ToArray(), promessas.Select(p =>
        new PromessaSalva(p.Id, p.Descricao, p.Prazo, p.Estado, p.ConcluidaEm,
            p.MotivoDoCancelamento, p.Revisoes.ToArray())).ToArray(),
        reflexoes.ToArray(), respostas.ToArray(), perguntas.Select(p =>
            new PerguntaSalva(p.Texto, p.Estado, p.Evidencia)).ToArray(), Diario.Registros.ToArray());
}
```

## Agora escolha outro caminho

::: exercicio Experimente e explique
Salve uma promessa aberta, cumpra-a e leia o retrato sem salvar novamente. Explique por que os estados são diferentes.

Altere o método Cap84 em uma cópia do projeto, execute o mesmo comando e compare com sua previsão. Quando houver uma exceção esperada, capture-a com `catch (ArgumentException erro)` ou `catch (InvalidOperationException erro)`, conforme a regra estudada, e mostre `erro.Message`.
--- resposta
O objeto de trabalho está Cumprida; o retrato anterior está Aberta. Para registrar a nova situação, exporte e salve novamente. Não existe sincronização automática.

Procure o teste correspondente em `codigo/compromissos/Caderno.Testes/Program.cs`. Os testes exercitam entradas, mudanças e falhas. Eles não comprovam a filosofia do autor: verificam o contrato que escolhemos implementar.
:::

## Limite da analogia

A memória do processo não é uma explicação científica de esquecimento humano. O arquivo em memória não sobrevive ao fim do processo. Serialização também não implica segurança, criptografia ou validação completa de um arquivo recebido.

## O que entrou na sua mochila

A interface não salva nada sozinha. Agora você consegue relacionar uma necessidade humana, uma regra escolhida e um resultado executável. Na próxima etapa reutilizaremos os objetos; não recomeçaremos um curso de sintaxe.

::: quiz Confira sua leitura e sua execução
? A interface salva dados por si?
+ Não; uma implementação realiza as operações.
- O resultado prova uma qualidade moral da pessoa.
- O programa decide sozinho o significado do texto.
- Não há uma regra verificável para esse caso.
> Consulte a experiência e a implementação deste capítulo; o alcance da regra é específico.

? ArquivoEmMemoria sobrevive ao fim do processo?
+ Não; o JSON fica apenas na memória do processo.
- O resultado prova uma qualidade moral da pessoa.
- O programa decide sozinho o significado do texto.
- Não há uma regra verificável para esse caso.
> Consulte a experiência e a implementação deste capítulo; o alcance da regra é específico.

? Mudar o objeto atual atualiza o retrato anterior?
+ Não; é necessário exportar e salvar novamente.
- O resultado prova uma qualidade moral da pessoa.
- O programa decide sozinho o significado do texto.
- Não há uma regra verificável para esse caso.
> Consulte a experiência e a implementação deste capítulo; o alcance da regra é específico.

? O que um DTO transporta aqui?
+ Os dados do retrato, não os métodos do domínio.
- O resultado prova uma qualidade moral da pessoa.
- O programa decide sozinho o significado do texto.
- Não há uma regra verificável para esse caso.
> Consulte a experiência e a implementação deste capítulo; o alcance da regra é específico.

? Salve uma promessa aberta, cumpra-a e leia o retrato sem salvar novamente?
+ O objeto de trabalho está Cumprida; o retrato anterior está Aberta.
- O programa avalia a intenção da pessoa
- Nada pode ser observado pela execução
- A linguagem decide o sentido do texto
> Reproduza a variação descrita no exercício e acompanhe o estado.

? Qual limite precisa acompanhar esta experiência?
+ A memória do processo não é uma explicação científica de esquecimento humano. O arquivo em memória não sobrevive ao fim do processo. Serialização também não implica segurança, criptografia ou validação completa de um arquivo recebido.
- O estado do programa certifica o desenvolvimento pessoal.
- O código define como toda pessoa deve sentir e agir.
- Não precisamos distinguir texto e aplicação.
> A utilidade da comparação depende de manter claro o que o modelo representa e o que não representa.
:::

::: cartas Lembre sem decorar
Interface :: A interface não salva nada sozinha.
DTO :: Antes de gravar no disco, criamos um retrato transportável.
serialização :: A memória do processo não é uma explicação científica de esquecimento humano.
memória volátil :: O objeto de trabalho está Cumprida; o retrato anterior está Aberta.
Passagem estudada :: Páginas físicas 43–44 do PDF recebido; explicação e exemplos são próprios.
Experiência executável :: dotnet run --project codigo/compromissos/Caderno.Console -- cap84
Variação para experimentar :: Salve uma promessa aberta, cumpra-a e leia o retrato sem salvar novamente. Explique por que os estados são diferentes.
Limite desta comparação :: A memória do processo não é uma explicação científica de esquecimento humano. O arquivo em memória não sobrevive ao fim do processo. Serialização também não implica segurança, criptografia ou validação completa de um arquivo recebido.
:::
