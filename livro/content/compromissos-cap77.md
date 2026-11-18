---
id: compromissos-cap77
order: 101
kind: capitulo
num: 77
group: C# & Mentalidade · Oficina · O Caderno dos Quatro Compromissos
title: Mudar o combinado sem apagar o que foi dito
subtitle: Uma experiência do tema, uma escolha e uma nova capacidade no mesmo programa.
minutes: 45
hue: 168
objectives:
- Compreender a ideia estudada e seu contexto na obra de Ruiz
- Construir a capacidade deste capítulo no Caderno
- Executar o cenário, variar uma entrada e explicar seu resultado
- Distinguir a proposta do autor das regras do software
origem: livros/05-quatro-compromissos-csharp/content/113-cap77.md
---

::: vila Uma página da nossa história
Caio percebe na quinta que não terminará a revisão na sexta. Em vez de editar o prazo silenciosamente, conversa com Lia. Ela aceita segunda-feira. O caderno precisa guardar a mudança e seu motivo. Se apenas trocasse a data, o programa pareceria organizado enquanto escondia a história.
:::

::: ilustracao img/compromissos/jornada77.svg | Mudar o combinado sem apagar o que foi dito: uma escolha visível e sua consequência no caderno.
:::

## A ideia que vamos experimentar

Ruiz orienta a usar a palavra de maneira coerente com o que se quer expressar, sem voltá-la contra si ou contra os outros. Ele também discute opiniões e fofoca. Nosso recorte transforma o cuidado com a palavra em transparência sobre alterações. O livro não especifica uma máquina de prazos: esse protocolo é uma decisão nossa.

**Fonte de leitura:** Don Miguel Ruiz, *Os Quatro Compromissos*, páginas físicas 14–21 do PDF fornecido. Este texto é uma explicação própria, não uma citação literal. A paginação de outras edições pode variar.

## Do tema para uma decisão no programa

Reutilizamos Promessa. Acrescentar RevisaoDePrazo é guardar uma relação entre o antes e o depois. O histórico não precisa ser um segundo programa. A operação verifica todos os argumentos antes de mudar a data. Se faltar motivo, nada muda; a falha não pode deixar metade da alteração aplicada.

::: pense Antes de executar
Descreva o que você espera ver mudar. Qual informação já existe? Qual ainda não existe? O que o programa deve recusar? Escreva sua previsão antes de olhar a saída.
:::

## Vamos fazer acontecer

Execute **a partir da pasta deste livro**. Todos os comandos cap74 a cap87 existem no mesmo projeto. Os cenários começam com dados novos e fictícios para serem reproduzíveis; o diário pessoal persistente é apresentado no laboratório.

```bash
dotnet run --project codigo/compromissos/Caderno.Console -- cap77
```

O trecho abaixo está dentro do método `Cap77` em `codigo/compromissos/Caderno.Console/Exemplos.cs`. Usa `using Quatro;`. É um corpo de método, não um arquivo independente: não o cole ao lado das declarações das classes.

```cs
var caderno = new Caderno();
var promessa = caderno.Prometer("Revisar o texto", new DateOnly(2026, 11, 6));
promessa.Renegociar(new DateOnly(2026, 11, 9), new DateOnly(2026, 11, 5), "Combinamos outro dia");
Console.WriteLine($"Revisões: {promessa.Revisoes.Count}");
Console.WriteLine($"Motivo: {promessa.Revisoes[0].Motivo}");
```

### Leia o que o código faz

Leia `Renegociar` no arquivo: exigir aberta, validar motivo, conferir data, registrar revisão, trocar prazo. A lista privada protege o histórico; a propriedade pública entrega uma cópia. Uma renegociação após atraso também é permitida neste modelo, desde que o novo prazo esteja no futuro. Guardar a revisão permite discutir o atraso sem transformá-lo em sentença sobre a pessoa.

Observe os verbos do código: registrar, perguntar, escolher, cumprir, salvar. Cada um deve produzir uma mudança que você consiga explicar. Execute de novo: nos cenários, os dados são recriados. Para conservar sua própria prática, use os comandos do laboratório.

### A saída real desta experiência

```saida
Revisões: 1
Motivo: Combinamos outro dia
```

Compare sua previsão com a execução. Não basta obter a mesma tela: explique qual instrução produziu cada linha e qual objeto foi consultado. Troque apenas uma entrada por vez para observar a causa da diferença.

## Por dentro da capacidade

O arquivo de referência é `codigo/compromissos/Caderno.Core/Promessa.cs`. Alguns membros serão usados nos próximos capítulos; hoje concentre-se no caminho percorrido pela experiência. A definição completa evita que o leitor precise adivinhar membros omitidos.

```cs
namespace Quatro;

public enum EstadoDaPromessa { Aberta, Cumprida, Cancelada }
public sealed record RevisaoDePrazo(DateOnly Antes, DateOnly Depois, string Motivo);

public sealed class Promessa
{
    private readonly List<RevisaoDePrazo> revisoes = [];
    public Guid Id { get; } = Guid.NewGuid();
    public string Descricao { get; }
    public DateOnly Prazo { get; private set; }
    public EstadoDaPromessa Estado { get; private set; }
    public DateOnly? ConcluidaEm { get; private set; }
    public string? MotivoDoCancelamento { get; private set; }
    public IReadOnlyList<RevisaoDePrazo> Revisoes => revisoes.ToArray();

    public Promessa(string descricao, DateOnly prazo)
    {
        Descricao = Regras.Texto(descricao);
        Prazo = prazo;
    }

    private void ExigirAberta()
    {
        if (Estado != EstadoDaPromessa.Aberta)
            throw new InvalidOperationException("A promessa já foi encerrada.");
    }

    public void Cumprir(DateOnly hoje)
    {
        ExigirAberta();
        ConcluidaEm = hoje;
        Estado = EstadoDaPromessa.Cumprida;
    }

    public bool EstaAtrasada(DateOnly hoje) =>
        Estado == EstadoDaPromessa.Aberta && hoje > Prazo;

    public bool FoiCumpridaComAtraso => ConcluidaEm is DateOnly dia && dia > Prazo;

    public void Renegociar(DateOnly novoPrazo, DateOnly hoje, string motivo)
    {
        ExigirAberta();
        string justificativa = Regras.Texto(motivo);
        if (novoPrazo <= hoje || novoPrazo == Prazo)
            throw new ArgumentException("Escolha outro prazo, posterior a hoje.");
        revisoes.Add(new(Prazo, novoPrazo, justificativa));
        Prazo = novoPrazo;
    }

    public void Cancelar(string motivo)
    {
        ExigirAberta();
        MotivoDoCancelamento = Regras.Texto(motivo);
        Estado = EstadoDaPromessa.Cancelada;
    }
}
```



## Agora escolha outro caminho

::: exercicio Experimente e explique
Tente renegociar sem motivo e confira a data e o tamanho do histórico depois do erro.

Altere o método Cap77 em uma cópia do projeto, execute o mesmo comando e compare com sua previsão. Quando houver uma exceção esperada, capture-a com `catch (ArgumentException erro)` ou `catch (InvalidOperationException erro)`, conforme a regra estudada, e mostre `erro.Message`.
--- resposta
A data permanece igual e nenhuma revisão é acrescentada. Os testes verificam que validar antes de modificar preserva esse comportamento.

Procure o teste correspondente em `codigo/compromissos/Caderno.Testes/Program.cs`. Os testes exercitam entradas, mudanças e falhas. Eles não comprovam a filosofia do autor: verificam o contrato que escolhemos implementar.
:::

## Limite da analogia

O método registra uma renegociação que a pessoa afirma ter combinado. Não realiza uma conversa nem comprova consentimento. Uma aplicação compartilhada precisaria identificar participantes, registrar o aceite e resolver alterações simultâneas.

## O que entrou na sua mochila

Leia `Renegociar` no arquivo: exigir aberta, validar motivo, conferir data, registrar revisão, trocar prazo. Agora você consegue relacionar uma necessidade humana, uma regra escolhida e um resultado executável. Na próxima etapa reutilizaremos os objetos; não recomeçaremos um curso de sintaxe.

::: quiz Confira sua leitura e sua execução
? O que a revisão preserva?
+ Prazo anterior, novo prazo e motivo.
- O resultado prova uma qualidade moral da pessoa.
- O programa decide sozinho o significado do texto.
- Não há uma regra verificável para esse caso.
> Consulte a experiência e a implementação deste capítulo; o alcance da regra é específico.

? Um motivo inválido muda a data parcialmente?
+ Não; a validação ocorre antes das alterações.
- O resultado prova uma qualidade moral da pessoa.
- O programa decide sozinho o significado do texto.
- Não há uma regra verificável para esse caso.
> Consulte a experiência e a implementação deste capítulo; o alcance da regra é específico.

? A classe comprova o aceite de outra pessoa?
+ Não; registra o combinado que o usuário informa.
- O resultado prova uma qualidade moral da pessoa.
- O programa decide sozinho o significado do texto.
- Não há uma regra verificável para esse caso.
> Consulte a experiência e a implementação deste capítulo; o alcance da regra é específico.

? Uma promessa encerrada pode ser renegociada?
+ Não; ExigirAberta recusa a operação.
- O resultado prova uma qualidade moral da pessoa.
- O programa decide sozinho o significado do texto.
- Não há uma regra verificável para esse caso.
> Consulte a experiência e a implementação deste capítulo; o alcance da regra é específico.

? Tente renegociar sem motivo e confira a data e o tamanho do histórico depois do erro.?
+ A data permanece igual e nenhuma revisão é acrescentada.
- O programa avalia a intenção da pessoa
- Nada pode ser observado pela execução
- A linguagem decide o sentido do texto
> Reproduza a variação descrita no exercício e acompanhe o estado.

? Qual limite precisa acompanhar esta experiência?
+ O método registra uma renegociação que a pessoa afirma ter combinado. Não realiza uma conversa nem comprova consentimento. Uma aplicação compartilhada precisaria identificar participantes, registrar o aceite e resolver alterações simultâneas.
- O estado do programa certifica o desenvolvimento pessoal.
- O código define como toda pessoa deve sentir e agir.
- Não precisamos distinguir texto e aplicação.
> A utilidade da comparação depende de manter claro o que o modelo representa e o que não representa.
:::

::: cartas Lembre sem decorar
Histórico :: Leia `Renegociar` no arquivo: exigir aberta, validar motivo, conferir data, registrar revisão, trocar prazo.
pré-condição :: Reutilizamos Promessa.
imutabilidade :: O método registra uma renegociação que a pessoa afirma ter combinado.
falha sem mutação :: A data permanece igual e nenhuma revisão é acrescentada.
Passagem estudada :: Páginas físicas 14–21 do PDF recebido; explicação e exemplos são próprios.
Experiência executável :: dotnet run --project codigo/compromissos/Caderno.Console -- cap77
Variação para experimentar :: Tente renegociar sem motivo e confira a data e o tamanho do histórico depois do erro.
Limite desta comparação :: O método registra uma renegociação que a pessoa afirma ter combinado. Não realiza uma conversa nem comprova consentimento. Uma aplicação compartilhada precisaria identificar participantes, registrar o aceite e resolver alterações simultâneas.
:::
