---
id: compromissos-cap86
order: 110
kind: capitulo
num: 86
group: C# & Mentalidade · Oficina · O Caderno dos Quatro Compromissos
title: Fechar o programa e encontrar o caminho de volta
subtitle: Uma experiência do tema, uma escolha e uma nova capacidade no mesmo programa.
minutes: 45
hue: 168
objectives:
- Compreender a ideia estudada e seu contexto na obra de Ruiz
- Construir a capacidade deste capítulo no Caderno
- Executar o cenário, variar uma entrada e explicar seu resultado
- Distinguir a proposta do autor das regras do software
origem: livros/05-quatro-compromissos-csharp/content/122-cap86.md
---

::: vila Uma página da nossa história
Desta vez Lia quer fechar o computador de verdade. Quando voltar, deseja encontrar o aprendizado que escreveu, não uma lista de demonstração inventada pelo programa. O caderno ganha um arquivo local. O exemplo cria uma pasta temporária, grava e abre o mesmo caminho com outra instância.
:::

::: ilustracao img/compromissos/jornada86.svg | Fechar o programa e encontrar o caminho de volta: uma escolha visível e sua consequência no caderno.
:::

## A ideia que vamos experimentar

Na parte final, Ruiz convida a rever como usamos o presente e como escolhemos agir nas relações. Sua linguagem inclui a figura espiritual do anjo da morte. Podemos estudar esse convite à presença sem simular morte como exclusão de dados. Nosso vínculo é a continuidade de escolhas registradas para uma próxima leitura.

**Fonte de leitura:** Don Miguel Ruiz, *Os Quatro Compromissos*, páginas físicas 43–44; 47–49 do PDF fornecido. Este texto é uma explicação própria, não uma citação literal. A paginação de outras edições pode variar.

## Do tema para uma decisão no programa

ArquivoJson usa o mesmo contrato da implementação em memória. O caminho informa onde os bytes ficam. A nova instância de arquivo demonstra que o registro não pertence apenas ao objeto que o gravou. O console pessoal permitirá separar gravação e leitura em execuções distintas.

::: pense Antes de executar
Descreva o que você espera ver mudar. Qual informação já existe? Qual ainda não existe? O que o programa deve recusar? Escreva sua previsão antes de olhar a saída.
:::

## Vamos fazer acontecer

Execute **a partir da pasta deste livro**. Todos os comandos cap74 a cap87 existem no mesmo projeto. Os cenários começam com dados novos e fictícios para serem reproduzíveis; o diário pessoal persistente é apresentado no laboratório.

```bash
dotnet run --project codigo/compromissos/Caderno.Console -- cap86
```

O trecho abaixo está dentro do método `Cap86` em `codigo/compromissos/Caderno.Console/Exemplos.cs`. Usa `using Quatro;`. É um corpo de método, não um arquivo independente: não o cole ao lado das declarações das classes.

```cs
var caderno = new Caderno();
caderno.Diario.Registrar(new DateOnly(2026, 11, 5), "Pedi esclarecimento", "Não precisava adivinhar");
string pasta = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
Directory.CreateDirectory(pasta);
try
{
    IArquivoDoCaderno arquivo = new ArquivoJson(Path.Combine(pasta, "caderno.json"));
    arquivo.Salvar(caderno.Exportar());
    var reaberto = new ArquivoJson(Path.Combine(pasta, "caderno.json")).Ler()!;
    Console.WriteLine($"Registros recuperados: {reaberto.Praticas.Length}");
    Console.WriteLine(reaberto.Praticas[0].Aprendizado);
}
finally { Directory.Delete(pasta, true); }
```

### Leia o que o código faz

`JsonSerializer.Serialize` transforma os dados em JSON; `File.WriteAllText` grava; `ReadAllText` lê. Arquivo ausente devolve null. JSON quebrado gera erro: não fingimos que um caderno corrompido é um caderno vazio. A versão do formato evita aceitar silenciosamente uma estrutura futura desconhecida.

Observe os verbos do código: registrar, perguntar, escolher, cumprir, salvar. Cada um deve produzir uma mudança que você consiga explicar. Execute de novo: nos cenários, os dados são recriados. Para conservar sua própria prática, use os comandos do laboratório.

### A saída real desta experiência

```saida
Registros recuperados: 1
Não precisava adivinhar
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



## Agora escolha outro caminho

::: exercicio Experimente e explique
Crie um caderno pelo console, registre uma prática, encerre e execute ler em outro processo. Depois teste ler com um caminho inexistente.

Altere o método Cap86 em uma cópia do projeto, execute o mesmo comando e compare com sua previsão. Quando houver uma exceção esperada, capture-a com `catch (ArgumentException erro)` ou `catch (InvalidOperationException erro)`, conforme a regra estudada, e mostre `erro.Message`.
--- resposta
A prática permanece no arquivo entre processos. Um caminho inexistente produz a mensagem de ausência. No laboratório há comandos separados para reproduzir essa experiência sem editar código.

Procure o teste correspondente em `codigo/compromissos/Caderno.Testes/Program.cs`. Os testes exercitam entradas, mudanças e falhas. Eles não comprovam a filosofia do autor: verificam o contrato que escolhemos implementar.
:::

## Limite da analogia

O arquivo é texto simples, sem criptografia, trava de concorrência, backup automático ou transação. Use dados fictícios para estudar. A leitura verifica versão e coleções presentes, mas não revalida cada registro como um importador de produção faria. O retrato é recuperado para consulta; não é reconstruído em objetos mutáveis do domínio.

## O que entrou na sua mochila

`JsonSerializer.Serialize` transforma os dados em JSON; `File.WriteAllText` grava; `ReadAllText` lê. Agora você consegue relacionar uma necessidade humana, uma regra escolhida e um resultado executável. Na próxima etapa reutilizaremos os objetos; não recomeçaremos um curso de sintaxe.

::: quiz Confira sua leitura e sua execução
? O que muda ao usar ArquivoJson?
+ Os dados passam a ser gravados em um arquivo fora do processo.
- O resultado prova uma qualidade moral da pessoa.
- O programa decide sozinho o significado do texto.
- Não há uma regra verificável para esse caso.
> Consulte a experiência e a implementação deste capítulo; o alcance da regra é específico.

? Arquivo inválido deve ser tratado como vazio?
+ Não; o erro é informado.
- O resultado prova uma qualidade moral da pessoa.
- O programa decide sozinho o significado do texto.
- Não há uma regra verificável para esse caso.
> Consulte a experiência e a implementação deste capítulo; o alcance da regra é específico.

? O formato guarda os métodos de Promessa?
+ Não; JSON guarda o retrato de dados.
- O resultado prova uma qualidade moral da pessoa.
- O programa decide sozinho o significado do texto.
- Não há uma regra verificável para esse caso.
> Consulte a experiência e a implementação deste capítulo; o alcance da regra é específico.

? Este arquivo é seguro para várias gravações simultâneas?
+ Não há controle de concorrência nessa implementação.
- O resultado prova uma qualidade moral da pessoa.
- O programa decide sozinho o significado do texto.
- Não há uma regra verificável para esse caso.
> Consulte a experiência e a implementação deste capítulo; o alcance da regra é específico.

? Crie um caderno pelo console, registre uma prática, encerre e execute ler em outro processo?
+ A prática permanece no arquivo entre processos.
- O programa avalia a intenção da pessoa
- Nada pode ser observado pela execução
- A linguagem decide o sentido do texto
> Reproduza a variação descrita no exercício e acompanhe o estado.

? Qual limite precisa acompanhar esta experiência?
+ O arquivo é texto simples, sem criptografia, trava de concorrência, backup automático ou transação. Use dados fictícios para estudar. A leitura verifica versão e coleções presentes, mas não revalida cada registro como um importador de produção faria. O retrato é recuperado para consulta; não é reconstruído em objetos mutáveis do domínio.
- O estado do programa certifica o desenvolvimento pessoal.
- O código define como toda pessoa deve sentir e agir.
- Não precisamos distinguir texto e aplicação.
> A utilidade da comparação depende de manter claro o que o modelo representa e o que não representa.
:::

::: cartas Lembre sem decorar
Persistência :: `JsonSerializer.Serialize` transforma os dados em JSON; `File.WriteAllText` grava; `ReadAllText` lê.
JSON :: ArquivoJson usa o mesmo contrato da implementação em memória.
versão :: O arquivo é texto simples, sem criptografia, trava de concorrência, backup automático ou transação.
tratamento de erros :: A prática permanece no arquivo entre processos.
Passagem estudada :: Páginas físicas 43–44; 47–49 do PDF recebido; explicação e exemplos são próprios.
Experiência executável :: dotnet run --project codigo/compromissos/Caderno.Console -- cap86
Variação para experimentar :: Crie um caderno pelo console, registre uma prática, encerre e execute ler em outro processo. Depois teste ler com um caminho inexistente.
Limite desta comparação :: O arquivo é texto simples, sem criptografia, trava de concorrência, backup automático ou transação. Use dados fictícios para estudar. A leitura verifica versão e coleções presentes, mas não revalida cada registro como um importador de produção faria. O retrato é recuperado para consulta; não é reconstruído em objetos mutáveis do domínio.
:::
