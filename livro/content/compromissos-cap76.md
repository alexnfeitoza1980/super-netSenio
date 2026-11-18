---
id: compromissos-cap76
order: 100
kind: capitulo
num: 76
group: C# & Mentalidade · Oficina · O Caderno dos Quatro Compromissos
title: Sua palavra começa a ter estado
subtitle: Uma experiência do tema, uma escolha e uma nova capacidade no mesmo programa.
minutes: 45
hue: 168
objectives:
- Compreender a ideia estudada e seu contexto na obra de Ruiz
- Construir a capacidade deste capítulo no Caderno
- Executar o cenário, variar uma entrada e explicar seu resultado
- Distinguir a proposta do autor das regras do software
origem: livros/05-quatro-compromissos-csharp/content/112-cap76.md
---

::: vila Uma página da nossa história
Caio diz “Eu envio a resposta na sexta”. Antes, isso desaparecia no meio das mensagens. Agora ele abre o mesmo caderno que recebeu os acordos e registra a promessa. Quando entrega, não apaga o item: muda seu estado. A diferença entre dizer e fazer finalmente fica observável.
:::

::: ilustracao img/compromissos/jornada76.svg | Sua palavra começa a ter estado: uma escolha visível e sua consequência no caderno.
:::

::: html
<figure style="max-width:420px;margin:2rem auto"><div style="overflow:hidden;aspect-ratio:3/4;border-radius:24px"><img src="img/compromissos/quatro-jornada.png" alt="A palavra que pode nutrir" style="display:block;width:400%;max-width:none;transform:translateX(-0%)"></div><figcaption>A palavra que pode nutrir. Uma imagem para acompanhar a experiência deste compromisso.</figcaption></figure>
:::

## A ideia que vamos experimentar

O primeiro compromisso associa a palavra à verdade e ao cuidado consigo e com os outros. Ruiz inclui o que dizemos sobre nós, as opiniões que espalhamos e as relações que criamos pela linguagem. Ele usa imagens espirituais de sementes e magia. Cumprir uma promessa é uma aplicação possível desse princípio; não resume toda a amplitude da palavra impecável.

**Fonte de leitura:** Don Miguel Ruiz, *Os Quatro Compromissos*, páginas físicas 14–21 do PDF fornecido. Este texto é uma explicação própria, não uma citação literal. A paginação de outras edições pode variar.

## Do tema para uma decisão no programa

Escolhemos representar uma promessa com descrição, prazo e estado. O prazo não é uma medida do caráter. Uma promessa começa aberta e pode ser cumprida ou cancelada com motivo. Atraso é consultado a partir da data recebida: não precisamos inventar um processo secreto que muda a situação à meia-noite.

::: pense Antes de executar
Descreva o que você espera ver mudar. Qual informação já existe? Qual ainda não existe? O que o programa deve recusar? Escreva sua previsão antes de olhar a saída.
:::

## Vamos fazer acontecer

Execute **a partir da pasta deste livro**. Todos os comandos cap74 a cap87 existem no mesmo projeto. Os cenários começam com dados novos e fictícios para serem reproduzíveis; o diário pessoal persistente é apresentado no laboratório.

```bash
dotnet run --project codigo/compromissos/Caderno.Console -- cap76
```

O trecho abaixo está dentro do método `Cap76` em `codigo/compromissos/Caderno.Console/Exemplos.cs`. Usa `using Quatro;`. É um corpo de método, não um arquivo independente: não o cole ao lado das declarações das classes.

```cs
var caderno = new Caderno();
var promessa = caderno.Prometer("Enviar a resposta", new DateOnly(2026, 11, 6));
Console.WriteLine(promessa.Estado);
promessa.Cumprir(new DateOnly(2026, 11, 5));
Console.WriteLine(promessa.Estado);
```

### Leia o que o código faz

`EstadoDaPromessa` dá nomes às alternativas. O `private set` permite que somente a própria classe altere suas propriedades. `Cumprir` verifica se o item está aberto antes de modificá-lo. A consulta `EstaAtrasada(hoje)` recebe a data explicitamente: o teste não depende do relógio da máquina. Na demonstração, acompanhe a mesma variável antes e depois do método.

Observe os verbos do código: registrar, perguntar, escolher, cumprir, salvar. Cada um deve produzir uma mudança que você consiga explicar. Execute de novo: nos cenários, os dados são recriados. Para conservar sua própria prática, use os comandos do laboratório.

### A saída real desta experiência

```saida
Aberta
Cumprida
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
Mude a entrega para o dia seguinte ao prazo. Consulte FoiCumpridaComAtraso. Depois tente chamar Cumprir novamente.

Altere o método Cap76 em uma cópia do projeto, execute o mesmo comando e compare com sua previsão. Quando houver uma exceção esperada, capture-a com `catch (ArgumentException erro)` ou `catch (InvalidOperationException erro)`, conforme a regra estudada, e mostre `erro.Message`.
--- resposta
A primeira entrega é aceita e o indicador de atraso fica verdadeiro. A segunda chamada é recusada porque o estado já não é Aberta. A regra evita sobrescrever a data da entrega anterior.

Procure o teste correspondente em `codigo/compromissos/Caderno.Testes/Program.cs`. Os testes exercitam entradas, mudanças e falhas. Eles não comprovam a filosofia do autor: verificam o contrato que escolhemos implementar.
:::

## Limite da analogia

Cumprida é um estado de registro, não um selo de integridade moral. A classe não observa a entrega: confia na ação informada. O modelo é local e de uma pessoa; não confirma o aceite de outra parte.

## O que entrou na sua mochila

`EstadoDaPromessa` dá nomes às alternativas. Agora você consegue relacionar uma necessidade humana, uma regra escolhida e um resultado executável. Na próxima etapa reutilizaremos os objetos; não recomeçaremos um curso de sintaxe.

::: quiz Confira sua leitura e sua execução
? O que uma promessa nova registra?
+ Descrição, prazo, identidade e estado Aberta.
- O resultado prova uma qualidade moral da pessoa.
- O programa decide sozinho o significado do texto.
- Não há uma regra verificável para esse caso.
> Consulte a experiência e a implementação deste capítulo; o alcance da regra é específico.

? Quem pode mudar diretamente Estado?
+ A própria classe; o setter é privado.
- O resultado prova uma qualidade moral da pessoa.
- O programa decide sozinho o significado do texto.
- Não há uma regra verificável para esse caso.
> Consulte a experiência e a implementação deste capítulo; o alcance da regra é específico.

? Por que receber hoje como argumento?
+ Para tornar a comparação de datas explícita e testável.
- O resultado prova uma qualidade moral da pessoa.
- O programa decide sozinho o significado do texto.
- Não há uma regra verificável para esse caso.
> Consulte a experiência e a implementação deste capítulo; o alcance da regra é específico.

? Entregar depois do prazo é recusado?
+ Não; a entrega é registrada e o atraso fica observável.
- O resultado prova uma qualidade moral da pessoa.
- O programa decide sozinho o significado do texto.
- Não há uma regra verificável para esse caso.
> Consulte a experiência e a implementação deste capítulo; o alcance da regra é específico.

? Mude a entrega para o dia seguinte ao prazo?
+ A primeira entrega é aceita e o indicador de atraso fica verdadeiro.
- O programa avalia a intenção da pessoa
- Nada pode ser observado pela execução
- A linguagem decide o sentido do texto
> Reproduza a variação descrita no exercício e acompanhe o estado.

? Qual limite precisa acompanhar esta experiência?
+ Cumprida é um estado de registro, não um selo de integridade moral. A classe não observa a entrega: confia na ação informada. O modelo é local e de uma pessoa; não confirma o aceite de outra parte.
- O estado do programa certifica o desenvolvimento pessoal.
- O código define como toda pessoa deve sentir e agir.
- Não precisamos distinguir texto e aplicação.
> A utilidade da comparação depende de manter claro o que o modelo representa e o que não representa.
:::

::: cartas Lembre sem decorar
Estado :: `EstadoDaPromessa` dá nomes às alternativas.
encapsulamento :: Escolhemos representar uma promessa com descrição, prazo e estado.
enum :: Cumprida é um estado de registro, não um selo de integridade moral.
método :: A primeira entrega é aceita e o indicador de atraso fica verdadeiro.
Passagem estudada :: Páginas físicas 14–21 do PDF recebido; explicação e exemplos são próprios.
Experiência executável :: dotnet run --project codigo/compromissos/Caderno.Console -- cap76
Variação para experimentar :: Mude a entrega para o dia seguinte ao prazo. Consulte FoiCumpridaComAtraso. Depois tente chamar Cumprir novamente.
Limite desta comparação :: Cumprida é um estado de registro, não um selo de integridade moral. A classe não observa a entrega: confia na ação informada. O modelo é local e de uma pessoa; não confirma o aceite de outra parte.
:::
