---
id: compromissos-cap79
order: 103
kind: capitulo
num: 79
group: C# & Mentalidade · Oficina · O Caderno dos Quatro Compromissos
title: Escolher o que fica, sem apagar o passado
subtitle: Uma experiência do tema, uma escolha e uma nova capacidade no mesmo programa.
minutes: 45
hue: 168
objectives:
- Compreender a ideia estudada e seu contexto na obra de Ruiz
- Construir a capacidade deste capítulo no Caderno
- Executar o cenário, variar uma entrada e explicar seu resultado
- Distinguir a proposta do autor das regras do software
origem: livros/05-quatro-compromissos-csharp/content/115-cap79.md
---

::: vila Uma página da nossa história
Lia descobre dois acordos que disputam sua atenção: “Responder imediatamente” e “Estar presente durante o jantar”. O programa não vai declarar um deles universalmente correto. Ela decide conversar sobre um horário de resposta e registra a nova escolha. O primeiro acordo continua no inventário como parte de sua história.
:::

::: ilustracao img/compromissos/jornada79.svg | Escolher o que fica, sem apagar o passado: uma escolha visível e sua consequência no caderno.
:::

## A ideia que vamos experimentar

Ruiz descreve vozes e acordos conflitantes e propõe perceber quais queremos transformar. Na parte sobre romper acordos antigos, insiste que a mudança pede prática e paciência. Essa continuidade importa: perceber uma regra hoje não elimina todos os seus efeitos amanhã.

**Fonte de leitura:** Don Miguel Ruiz, *Os Quatro Compromissos*, páginas físicas 25–26; 43–44 do PDF fornecido. Este texto é uma explicação própria, não uma citação literal. A paginação de outras edições pode variar.

## Do tema para uma decisão no programa

Voltamos ao objeto criado no início para usá-lo de forma mais rica. Há uma busca por texto e uma decisão identificada por Id. O programa não chama isso de cura: marca uma escolha. Manter o registro anterior permite comparar de onde veio o hábito com a decisão atual.

::: pense Antes de executar
Descreva o que você espera ver mudar. Qual informação já existe? Qual ainda não existe? O que o programa deve recusar? Escreva sua previsão antes de olhar a saída.
:::

## Vamos fazer acontecer

Execute **a partir da pasta deste livro**. Todos os comandos cap74 a cap87 existem no mesmo projeto. Os cenários começam com dados novos e fictícios para serem reproduzíveis; o diário pessoal persistente é apresentado no laboratório.

```bash
dotnet run --project codigo/compromissos/Caderno.Console -- cap79
```

O trecho abaixo está dentro do método `Cap79` em `codigo/compromissos/Caderno.Console/Exemplos.cs`. Usa `using Quatro;`. É um corpo de método, não um arquivo independente: não o cole ao lado das declarações das classes.

```cs
var caderno = new Caderno();
caderno.Inventario.Registrar("Responder imediatamente", "Hábito antigo");
var novo = caderno.Inventario.Registrar("Combinar um horário para responder", "Escolha consciente");
caderno.Inventario.Escolher(novo.Id);
foreach (var a in caderno.Inventario.Buscar("responder"))
    Console.WriteLine($"{a.Texto}: escolhido={a.Escolhido}");
```

### Leia o que o código faz

`Where` seleciona acordos; `Contains` testa cada texto; `StringComparison.OrdinalIgnoreCase` torna a busca indiferente à caixa, mas não elimina diferenças de acento. `ToArray` materializa o resultado. Buscar uma palavra é diferente de determinar conflito semântico: essa segunda capacidade não existe aqui. O laço imprime cada resultado sem expor a lista interna.

Observe os verbos do código: registrar, perguntar, escolher, cumprir, salvar. Cada um deve produzir uma mudança que você consiga explicar. Execute de novo: nos cenários, os dados são recriados. Para conservar sua própria prática, use os comandos do laboratório.

### A saída real desta experiência

```saida
Responder imediatamente: escolhido=False
Combinar um horário para responder: escolhido=True
```

Compare sua previsão com a execução. Não basta obter a mesma tela: explique qual instrução produziu cada linha e qual objeto foi consultado. Troque apenas uma entrada por vez para observar a causa da diferença.

## Por dentro da capacidade

O arquivo de referência é `codigo/compromissos/Caderno.Core/Inventario.cs`. Alguns membros serão usados nos próximos capítulos; hoje concentre-se no caminho percorrido pela experiência. A definição completa evita que o leitor precise adivinhar membros omitidos.

```cs
namespace Quatro;

public sealed record Acordo(Guid Id, string Texto, string Origem, bool Escolhido);

public sealed class Inventario
{
    private readonly List<Acordo> acordos = [];
    public IReadOnlyList<Acordo> Todos => acordos.ToArray();

    public Acordo Registrar(string texto, string origem)
    {
        var acordo = new Acordo(Guid.NewGuid(), Regras.Texto(texto),
            Regras.Texto(origem), false);
        acordos.Add(acordo);
        return acordo;
    }

    public void Escolher(Guid id)
    {
        int i = acordos.FindIndex(a => a.Id == id);
        if (i < 0) throw new KeyNotFoundException("Acordo não encontrado.");
        acordos[i] = acordos[i] with { Escolhido = true };
    }

    public IReadOnlyList<Acordo> Buscar(string texto)
    {
        string termo = Regras.Texto(texto);
        return acordos.Where(a => a.Texto.Contains(termo,
            StringComparison.OrdinalIgnoreCase)).ToArray();
    }
}
```



## Agora escolha outro caminho

::: exercicio Experimente e explique
Busque uma palavra que não aparece no inventário. Compare o resultado vazio com tentar escolher um Id inexistente.

Altere o método Cap79 em uma cópia do projeto, execute o mesmo comando e compare com sua previsão. Quando houver uma exceção esperada, capture-a com `catch (ArgumentException erro)` ou `catch (InvalidOperationException erro)`, conforme a regra estudada, e mostre `erro.Message`.
--- resposta
Buscar sem correspondência devolve coleção vazia. Escolher um item inexistente lança KeyNotFoundException: uma consulta pode legitimamente não encontrar nada; uma alteração exige um alvo existente.

Procure o teste correspondente em `codigo/compromissos/Caderno.Testes/Program.cs`. Os testes exercitam entradas, mudanças e falhas. Eles não comprovam a filosofia do autor: verificam o contrato que escolhemos implementar.
:::

## Limite da analogia

Dois acordos podem coexistir dependendo do contexto. O programa não resolve sozinho valores, relações ou prioridades. `Escolhido` não desativa automaticamente outros itens e não significa que a prática já virou hábito.

## O que entrou na sua mochila

`Where` seleciona acordos; `Contains` testa cada texto; `StringComparison.OrdinalIgnoreCase` torna a busca indiferente à caixa, mas não elimina diferenças de acento. Agora você consegue relacionar uma necessidade humana, uma regra escolhida e um resultado executável. Na próxima etapa reutilizaremos os objetos; não recomeçaremos um curso de sintaxe.

::: quiz Confira sua leitura e sua execução
? Buscar determina conflito entre valores?
+ Não; encontra trechos de texto.
- O resultado prova uma qualidade moral da pessoa.
- O programa decide sozinho o significado do texto.
- Não há uma regra verificável para esse caso.
> Consulte a experiência e a implementação deste capítulo; o alcance da regra é específico.

? Escolher um acordo desativa todos os outros?
+ Não; nenhuma exclusão automática foi implementada.
- O resultado prova uma qualidade moral da pessoa.
- O programa decide sozinho o significado do texto.
- Não há uma regra verificável para esse caso.
> Consulte a experiência e a implementação deste capítulo; o alcance da regra é específico.

? ToArray executa a consulta nesse momento?
+ Sim; materializa os resultados em um array.
- O resultado prova uma qualidade moral da pessoa.
- O programa decide sozinho o significado do texto.
- Não há uma regra verificável para esse caso.
> Consulte a experiência e a implementação deste capítulo; o alcance da regra é específico.

? Ignorar caixa também elimina acentos?
+ Não; esta comparação não faz essa normalização.
- O resultado prova uma qualidade moral da pessoa.
- O programa decide sozinho o significado do texto.
- Não há uma regra verificável para esse caso.
> Consulte a experiência e a implementação deste capítulo; o alcance da regra é específico.

? Busque uma palavra que não aparece no inventário?
+ Buscar sem correspondência devolve coleção vazia.
- O programa avalia a intenção da pessoa
- Nada pode ser observado pela execução
- A linguagem decide o sentido do texto
> Reproduza a variação descrita no exercício e acompanhe o estado.

? Qual limite precisa acompanhar esta experiência?
+ Dois acordos podem coexistir dependendo do contexto. O programa não resolve sozinho valores, relações ou prioridades. `Escolhido` não desativa automaticamente outros itens e não significa que a prática já virou hábito.
- O estado do programa certifica o desenvolvimento pessoal.
- O código define como toda pessoa deve sentir e agir.
- Não precisamos distinguir texto e aplicação.
> A utilidade da comparação depende de manter claro o que o modelo representa e o que não representa.
:::

::: cartas Lembre sem decorar
LINQ :: `Where` seleciona acordos; `Contains` testa cada texto; `StringComparison.OrdinalIgnoreCase` torna a busca indiferente à caixa, mas não elimina diferenças de acento.
materialização :: Voltamos ao objeto criado no início para usá-lo de forma mais rica.
busca :: Dois acordos podem coexistir dependendo do contexto.
decisão consciente :: Buscar sem correspondência devolve coleção vazia.
Passagem estudada :: Páginas físicas 25–26; 43–44 do PDF recebido; explicação e exemplos são próprios.
Experiência executável :: dotnet run --project codigo/compromissos/Caderno.Console -- cap79
Variação para experimentar :: Busque uma palavra que não aparece no inventário. Compare o resultado vazio com tentar escolher um Id inexistente.
Limite desta comparação :: Dois acordos podem coexistir dependendo do contexto. O programa não resolve sozinho valores, relações ou prioridades. `Escolhido` não desativa automaticamente outros itens e não significa que a prática já virou hábito.
:::
