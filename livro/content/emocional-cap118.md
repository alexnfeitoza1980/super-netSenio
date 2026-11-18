---
id: emocional-cap118
order: 153
kind: capitulo
num: 118
group: C# & Mentalidade · Parte 17 · Emoções e C#
title: Criar espaço antes de responder
subtitle: Uma pausa pode abrir opções, mas não é sempre possível nem precisa ter a mesma duração ou forma para todo mundo.
minutes: 45
hue: 194
objectives:
- Diferenciar regular uma resposta de suprimir uma emoção
- Escolher uma opção de pausa de acordo com o contexto
- Modelar opções explícitas com `enum`, `switch` e validação
origem: livros/09-inteligencia-emocional-csharp/content/146-cap118.md
---

::: vila A mensagem no chat
Uma mensagem curta no chat do projeto soa ríspida para Paulo. Ele começa a digitar uma resposta defensiva, percebe a tensão nos ombros e para. Como o prazo não é imediato, sai da conversa por alguns minutos; depois pergunta se a colega quis dizer que o teste está incompleto ou se está pedindo uma alteração maior. O intervalo não resolveu tudo: deu espaço para esclarecer o pedido.
:::

::: ilustracao img/emocional/cap118.svg | Pausar é uma opção, não uma obrigação: abre espaço entre o impulso e a resposta quando o contexto permite.
:::

## Pausar é uma opção, não uma obrigação

Uma pausa pode ser breve, pode envolver mudar de tarefa ou pedir que uma conversa continue depois. Às vezes a pessoa não pode sair da situação, ou precisa primeiro buscar segurança e apoio. Não há uma técnica única que funcione sempre, e “respirar fundo” não é resposta suficiente para toda circunstância.

Antes de escolher, considere:

- há risco ou urgência que exige ação imediata?
- é possível adiar a resposta?
- a pessoa precisa de apoio, tradução, acessibilidade ou limite?
- qual passo é pequeno o bastante para este momento?

Regular uma resposta não significa negar o sentimento. Também não significa tolerar desrespeito: pedir ajuda, encerrar uma interação ou estabelecer um limite podem ser escolhas adequadas.

::: figura Opções entre sentir e agir
<svg viewBox="0 0 720 214" role="img" aria-label="Um sinal percebido leva a quatro opções de pausa: continuar com calma, pedir um intervalo, pedir apoio ou encerrar por agora">
<rect x="10" y="62" width="130" height="70" rx="12" class="f-info"/><text x="75.0" y="94.0" text-anchor="middle" font-weight="800" font-size="13">Sinal</text><text x="75.0" y="111.0" text-anchor="middle" font-size="11" class="t-dim">percebido</text><path class="arrow" d="M142 97H170"/><rect x="176" y="18" width="270" height="40" rx="12" class="f-good"/><text x="311.0" y="35.0" text-anchor="middle" font-weight="800" font-size="13">ContinuarComCalma</text><rect x="176" y="64" width="270" height="40" rx="12" class="f-warn"/><text x="311.0" y="81.0" text-anchor="middle" font-weight="800" font-size="13">PedirUmIntervalo</text><rect x="176" y="110" width="270" height="40" rx="12" class="f-prim"/><text x="311.0" y="127.0" text-anchor="middle" font-weight="800" font-size="13">PedirApoio</text><rect x="176" y="156" width="270" height="40" rx="12" class="f-bad"/><text x="311.0" y="173.0" text-anchor="middle" font-weight="800" font-size="13">EncerrarPorAgora</text><text x="470" y="92" font-size="14" font-weight="800">O menu lista opções;</text><text x="470" y="112" font-size="14" font-weight="800">quem escolhe é a pessoa.</text><text x="470" y="140" font-size="12" class="t-dim">Se há urgência ou risco,</text><text x="470" y="158" font-size="12" class="t-dim">a pausa pode não caber.</text>
</svg>
:::

## Representar alternativas no domínio

Um `enum` registra opções que a pessoa pode selecionar:

```cs Jornada/Emocoes/OpcaoDePausa.cs
namespace Jornada.Emocoes;

public enum OpcaoDePausa
{
    ContinuarComCalma,
    PedirUmIntervalo,
    PedirApoio,
    EncerrarPorAgora
}
```

Uma escolha pode ser acompanhada por um próximo passo e por um limite:

```cs Jornada/Emocoes/PlanoDePausa.cs
namespace Jornada.Emocoes;

public sealed record PlanoDePausa
{
    public OpcaoDePausa Opcao { get; }
    public string ProximoPasso { get; }
    public string LimiteOuApoio { get; }

    public PlanoDePausa(OpcaoDePausa opcao, string proximoPasso, string limiteOuApoio)
    {
        if (!Enum.IsDefined(opcao))
            throw new ArgumentOutOfRangeException(nameof(opcao));

        Opcao = opcao;
        ProximoPasso = ExigirTexto(proximoPasso, nameof(proximoPasso));
        LimiteOuApoio = ExigirTexto(limiteOuApoio, nameof(limiteOuApoio));
    }

    private static string ExigirTexto(string valor, string parametro)
    {
        if (string.IsNullOrWhiteSpace(valor))
            throw new ArgumentException("Informe um próximo passo ou use um exemplo neutro.", parametro);

        return valor.Trim();
    }
}
```

O construtor valida a opção (`Enum.IsDefined`) e exige texto nos dois campos, como os tipos dos capítulos anteriores. O `switch` abaixo apenas explica um exemplo escolhido; não seleciona o plano automaticamente:

```cs
var plano = new PlanoDePausa(
    OpcaoDePausa.PedirUmIntervalo,
    "Retomar a revisão depois do almoço.",
    "Se o prazo mudar, avisar a pessoa responsável.");

string mensagem = plano.Opcao switch
{
    OpcaoDePausa.ContinuarComCalma => "Continuar no ritmo possível.",
    OpcaoDePausa.PedirUmIntervalo => "Combinar quando retomar.",
    OpcaoDePausa.PedirApoio => "Conversar com alguém de confiança.",
    OpcaoDePausa.EncerrarPorAgora => "Fechar a atividade por enquanto.",
    _ => throw new ArgumentOutOfRangeException()
};
```

O código não infere qual alternativa a pessoa “deveria” escolher.

## Transformar uma escolha em mensagem

Uma enumeração é útil quando o conjunto de alternativas é conhecido e cada alternativa possui um significado claro:

```cs
static string Descrever(OpcaoDePausa opcao) => opcao switch
{
    OpcaoDePausa.ContinuarComCalma => "Continuar no ritmo possível.",
    OpcaoDePausa.PedirUmIntervalo => "Combinar quando retomar.",
    OpcaoDePausa.PedirApoio => "Conversar com alguém de confiança.",
    OpcaoDePausa.EncerrarPorAgora => "Fechar a atividade por enquanto.",
    _ => throw new ArgumentOutOfRangeException(nameof(opcao))
};
```

O `switch` faz um **mapeamento**: dado um valor já selecionado, devolve um texto. Ele não lê a descrição, não escolhe uma opção, não muda o estado da pessoa e não impede que ela altere a escolha. A exceção final protege contra valores numéricos inválidos que poderiam ser construídos por conversões explícitas.

Um tipo de domínio também pode verificar o enum e os textos antes de aceitar o plano:

```cs
var plano = new PlanoDePausa(
    OpcaoDePausa.PedirApoio,
    "Pedir a uma pessoa de confiança para revisar o próximo passo.",
    "Se não for possível agora, interromper a tarefa e combinar retorno.");
```

`ArgumentOutOfRangeException` indica que o valor está fora do conjunto previsto. `ArgumentException` indica que um texto obrigatório está ausente. As exceções nomeiam o contrato violado; não são mensagens para a pessoa destinatária da conversa. Uma aplicação com formulário deveria capturar erros esperados no ponto de entrada e mostrar uma orientação simples, sem exibir detalhes internos.

## Planejar sem transformar a pausa em regra universal

Antes de escolher, vale considerar condições concretas. Um intervalo pode ser possível numa tarefa individual e inviável durante uma operação que exige uma decisão segura. “Encerrar por agora” pode ser uma proteção necessária numa conversa hostil, enquanto pedir apoio pode ser o primeiro passo numa situação de maior responsabilidade. Não existe uma ordenação universal em que uma opção seja sempre melhor.

Podemos tratar esses quatro casos como **alternativas disponíveis**, mas o programa não deve ranqueá-los:

| Contexto fictício | Alternativa que a pessoa poderia escolher | Informação que precisa continuar aberta |
|---|---|---|
| Revisão ainda pode esperar | Pedir um intervalo | Quando retomar e quem precisa saber |
| Dúvida técnica específica | Pedir apoio | Que tipo de revisão é necessário |
| Conversa sem segurança | Encerrar por agora | Que canal seguro pode ser usado depois |
| Tarefa simples e tempo suficiente | Continuar com calma | Se o ritmo segue viável |

Uma escolha pode mudar quando o contexto muda. Se um pedido de ajuda não obtém resposta, por exemplo, talvez seja necessário procurar outro canal. A capacidade de rever o plano é uma característica importante, não uma falha de consistência.

## Tratar erro sem esconder o que aconteceu

Os construtores usam exceções para entradas que violam o contrato. Em um programa de console sem entrada externa, os dados são fixos e a falha apontaria um erro de programação; deixá-la aparecer ajuda a corrigir o exemplo. Se os valores viessem de um formulário, a aplicação deveria capturar **a exceção esperada** perto dessa fronteira e exibir uma instrução útil:

```cs
var opcao = OpcaoDePausa.PedirUmIntervalo;
var proximoPasso = "Retomar depois da revisão.";
var limiteOuApoio = "Avisar a equipe se o prazo mudar.";

try
{
    var plano = new PlanoDePausa(opcao, proximoPasso, limiteOuApoio);
    Console.WriteLine($"Plano criado: {plano.Opcao}");
}
catch (ArgumentException erro)
{
    Console.WriteLine($"Revise o campo {erro.ParamName} e tente novamente.");
}
```

Não use `catch (Exception)` para transformar qualquer falha em “tudo certo” nem crie um plano vazio quando a validação falhar. Isso esconderia uma quebra do contrato. O exemplo também não precisa guardar o texto completo da exceção numa mensagem pública: detalhes técnicos ficam para depuração, e o aviso ao usuário é breve.

Se o problema for um enum inválido vindo de uma conversão externa, `ArgumentOutOfRangeException` herda de `ArgumentException`. Uma aplicação pode optar por uma mensagem específica, mas deve manter a distinção entre entrada incorreta e uma falha inesperada do programa.

## Menu de opções não é recomendador

Uma interface poderia imprimir cada item do enum numerado para que a pessoa escolha manualmente. Isso apenas apresenta alternativas. Um sistema que lê outras respostas e escolhe uma opção em nome dela seria outra funcionalidade, com riscos e pressupostos que não fazem parte deste livro.

```cs
foreach (OpcaoDePausa opcao in Enum.GetValues<OpcaoDePausa>())
    Console.WriteLine($"{(int)opcao}: {opcao}");
```

Como o primeiro valor num enum começa em zero, um menu para pessoas normalmente deve exibir uma posição amigável a partir de 1, validar a conversão da entrada e só então mapear para a enum. O trecho acima é uma inspeção para o desenvolvedor, não a interface final nem um conselho. Este laboratório evita entrada interativa para não convidar o leitor a digitar um relato pessoal.

Uma lista de opções também precisa oferecer uma saída: “nenhuma delas”, “não agora” ou encerramento. Se todas as escolhas parecem inadequadas, não force a seleção. Em programação, `Enum.IsDefined` resolve valores técnicos inválidos, mas não garante que o conjunto de alternativas seja suficiente para o contexto de alguém.

Para apresentar rótulos em português, o projeto mantém o mapeamento separado em `RotulosDePausa`:

```cs Jornada/Emocoes/RotulosDePausa.cs
namespace Jornada.Emocoes;

public static class RotulosDePausa
{
    public static string Descrever(OpcaoDePausa opcao) => opcao switch
    {
        OpcaoDePausa.ContinuarComCalma => "Continuar no ritmo possível",
        OpcaoDePausa.PedirUmIntervalo => "Pedir um intervalo",
        OpcaoDePausa.PedirApoio => "Pedir apoio",
        OpcaoDePausa.EncerrarPorAgora => "Encerrar por agora",
        _ => throw new ArgumentOutOfRangeException(nameof(opcao))
    };
}
```

O caso `_` fecha o `switch` para valores construídos fora da enum. Uma chamada com `(OpcaoDePausa)999` não recebe um rótulo inventado: falha claramente. Nos testes, verificamos os rótulos conhecidos e essa entrada inválida. A função apenas traduz uma opção selecionada; não faz recomendação.

## Uma pausa não apaga o conflito

Na cena do chat, Paulo não concluiu que o conflito estava resolvido só porque esperou alguns minutos. Ele voltou com uma pergunta técnica e uma oportunidade de esclarecer a intenção. Se o comentário continuar sendo desrespeitoso, ainda poderá precisar de um limite ou apoio. Esperar pode alterar o momento da resposta, mas não substitui uma conversa nem a correção de um processo injusto.

Também é possível não perceber o impulso antes de responder. Isso não transforma a tentativa numa prova de caráter. O exercício é praticar a identificação de opções para quando houver espaço, reconhecendo que atenção, tempo e segurança variam. Se a situação envolver risco imediato, o foco deixa de ser completar um exercício e passa a ser buscar ajuda apropriada.

::: pense Investigue a regra oculta
Se uma aplicação sempre recomendar “pedir intervalo” quando alguém informa `Raiva`, que pressuposto foi programado? Que opções legítimas essa regra deixa de fora?
:::

::: exercicio Encontre a pausa disponível
Uma conversa de revisão ficou difícil, mas o PR precisa de uma decisão para o deploy. Cite uma pergunta útil e uma opção que não dependa de interromper o deploy sem necessidade.
---
**Resposta possível:** perguntar “qual risco específico impede a decisão agora?” e pedir a uma segunda pessoa para revisar o teste em paralelo. Se não houver segurança suficiente, o limite pode ser adiar a publicação e avisar a equipe. O contexto técnico e a segurança vêm antes de uma receita de autocontrole.
:::

::: exercicio Converta a opção escolhida em rótulo
Use `RotulosDePausa.Descrever` para mostrar `PedirUmIntervalo` no console. Em seguida, explique por que o método não deve receber uma emoção e escolher a pausa por conta própria.
---
**Resposta possível:** `Console.WriteLine(RotulosDePausa.Descrever(OpcaoDePausa.PedirUmIntervalo));` imprime “Pedir um intervalo”. O método recebe uma opção já escolhida e só a apresenta; usar emoção como entrada e selecionar automaticamente presumiria uma resposta adequada sem conhecer o contexto nem a vontade da pessoa.
:::

::: quiz
? Regular uma resposta significa:
+ Criar opções entre sentir e agir, quando o contexto permite.
- Impedir que emoções apareçam.
- Concordar com qualquer fala para manter a calma.
> Uma pausa pode ajudar, mas não apaga o sentimento nem obriga a aceitar desrespeito.

? Quem escolhe o `OpcaoDePausa`?
+ A pessoa que está planejando a própria resposta.
- O código, com base na emoção registrada.
- Uma pontuação automática de intensidade.
> O modelo organiza alternativas; não toma decisões pela pessoa.

? Se há urgência ou risco, uma pausa longa é sempre melhor?
+ Não; a resposta depende da segurança, do prazo e do contexto.
- Sim; toda situação precisa esperar.
- Sim, porque urgência é sempre uma interpretação.
> O plano deve levar as condições reais em conta.
? Quais opções existem em `OpcaoDePausa`?
+ `ContinuarComCalma`, `PedirUmIntervalo`, `PedirApoio` e `EncerrarPorAgora`
- `Sentir`, `Pensar` e `Agir`
- `Brigar` e `Fugir`
- Apenas `PedirUmIntervalo`
> O menu lista opções; não recomenda nenhuma.

? O que o `PlanoDePausa` guarda além da opção escolhida?
+ O próximo passo e o limite ou apoio, como texto
- A emoção da pessoa
- Uma nota de calma
- O histórico de todas as pausas
> São os três campos do construtor: opção, próximo passo e limite ou apoio.

? Uma pausa apaga o conflito?
+ Não: cria espaço, e o assunto pode precisar ser retomado
- Sim, sempre
- Sim, se durar mais de uma hora
- Só se for feita em silêncio
> Pausar abre opções; não resolve por si só.
:::

::: resumo
- Pausar pode criar espaço para uma resposta, mas nem sempre é viável.
- Regular não é suprimir nem aceitar comportamento prejudicial.
- `enum` e `switch` tornam opções explícitas; o mapeamento de uma opção não é uma recomendação automática.
- Contexto, segurança e limites fazem parte da decisão.
:::

::: cartas
Regulação :: Escolha de como responder a uma experiência, sem exigir que ela desapareça.
Pausa :: Intervalo escolhido quando possível para rever opções ou buscar apoio.
Limite :: Condição que protege tempo, segurança ou respeito numa interação.
`OpcaoDePausa` :: `enum` com as opções de pausa que o exemplo oferece.
`PlanoDePausa` :: `record` que reúne a opção escolhida, o próximo passo e o limite ou apoio.
Expressão `switch` :: Forma de escolher um resultado conforme o valor, de modo explícito e verificado.
Exceção como sinal de erro :: Aviso claro de que um dado violou o contrato.
Encerrar por agora :: Opção de interromper uma interação por ora, sem encerrar a relação.
:::
