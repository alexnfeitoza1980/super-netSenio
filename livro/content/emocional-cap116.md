---
id: emocional-cap116
order: 151
kind: capitulo
num: 116
group: C# & Mentalidade · Parte 17 · Emoções e C#
title: Emoções trazem informação, não ordens
subtitle: Perceber um sinal pode ajudar a escolher o próximo passo. Não revela, sozinho, a causa nem a resposta certa.
minutes: 45
hue: 170
objectives:
- Distinguir sentir uma emoção de agir sob seu impulso
- Descrever um sinal e seu contexto sem fazer diagnóstico
- Representar uma experiência declarada com `enum` e `record`
origem: livros/09-inteligencia-emocional-csharp/content/144-cap116.md
---

::: vila Uma revisão atravessada
Ao receber um comentário numa revisão de código, Rui percebe o rosto quente e a vontade de responder depressa. O comentário aponta um caso de concorrência que ele não havia considerado. O sinal corporal é real; a explicação “a colega quer me humilhar” ainda é uma interpretação. Rui fecha o editor por alguns minutos, lê o comentário novamente e escolhe perguntar por um exemplo reproduzível.
:::

::: ilustracao img/emocional/cap116.svg | Um sinal corporal é real; a explicação "a colega quer me humilhar" ainda é uma hipótese. Entre os dois cabe uma escolha.
:::

## Notar, nomear, escolher

Uma emoção pode acompanhar mudanças no corpo, na atenção e na vontade de agir. Notá-la é diferente de obedecer ao impulso, e não obriga a descobrir uma causa única. Sono, tarefa, ambiente, expectativas e acontecimentos recentes podem participar da experiência; às vezes não fica claro o que mais pesou.

Um percurso útil, sem ser uma regra, é:

1. **Notar:** que sinal observável percebi?
2. **Nomear:** que palavra parece adequada para mim agora?
3. **Contextualizar:** o que mais acontecia?
4. **Escolher:** quero continuar, pausar, conversar ou pedir apoio?

O nome pode mudar conforme a pessoa aprende a descrever o que sente. “Não sei ainda” também é uma resposta válida. Esta atividade não transforma um episódio em traço de personalidade.

::: regra Emoção não é comando
Sentir irritação não exige responder com hostilidade; sentir medo não demonstra que há perigo; sentir calma não garante que tudo esteja resolvido. O sentimento merece atenção, e a ação ainda pode ser escolhida.
:::

::: figura Quatro camadas que não precisam ser misturadas
<svg viewBox="0 0 720 190" role="img" aria-label="Um sinal, uma palavra escolhida, uma interpretação e uma ação escolhida são quatro camadas diferentes">
<rect x="10" y="40" width="160" height="70" rx="12" class="f-info"/><text x="90.0" y="72.0" text-anchor="middle" font-weight="800" font-size="13">Sinal</text><text x="90.0" y="89.0" text-anchor="middle" font-size="11" class="t-dim">senti o peito apertado</text><path class="arrow" d="M172 75H192"/><rect x="194" y="40" width="160" height="70" rx="12" class="f-warn"/><text x="274.0" y="72.0" text-anchor="middle" font-weight="800" font-size="13">Palavra escolhida</text><text x="274.0" y="89.0" text-anchor="middle" font-size="11" class="t-dim">talvez ansiedade</text><path class="arrow" d="M356 75H376"/><rect x="378" y="40" width="160" height="70" rx="12" class="f-bad"/><text x="458.0" y="72.0" text-anchor="middle" font-weight="800" font-size="13">Interpretação</text><text x="458.0" y="89.0" text-anchor="middle" font-size="11" class="t-dim">vou falhar</text><path class="arrow" d="M540 75H560"/><rect x="562" y="40" width="148" height="70" rx="12" class="f-good"/><text x="636.0" y="72.0" text-anchor="middle" font-weight="800" font-size="13">Ação escolhida</text><text x="636.0" y="89.0" text-anchor="middle" font-size="11" class="t-dim">pedir que repitam</text><text x="360" y="150" text-anchor="middle" font-size="13" class="t-dim">Cada camada pode ser dita, ou não. Uma não decide a seguinte.</text><text x="360" y="172" text-anchor="middle" font-size="13" class="t-dim">&quot;Ainda não sei&quot; e &quot;prefiro não nomear&quot; também são respostas.</text>
</svg>
:::

## Um tipo para uma palavra escolhida

Um `enum` pode oferecer opções num exercício simples. A pessoa escolhe a palavra; o programa não a deduz:

```cs Jornada/Emocoes/EmocaoNomeada.cs
namespace Jornada.Emocoes;

public enum EmocaoNomeada
{
    Alegria,
    Tristeza,
    Raiva,
    Medo,
    Ansiedade,
    Frustracao,
    Cansaco,
    AindaNaoSei,
    PrefiroNaoNomear,
    Outra
}
```

Uma opção não descreve a pessoa inteira nem cobre todas as experiências. Por isso, o exemplo aceita `Outra`, e outros capítulos separam observação, contexto e interpretação.

## Um sinal observado ainda não é uma explicação

Imagine que uma pessoa erra uma linha durante uma apresentação. A fala acelerada pode ser percebida por quem estava na sala. “Ela está em pânico” já é uma hipótese; “não se preparou” é uma segunda hipótese, e talvez nenhuma corresponda ao que a pessoa viveu. Mesmo quando a própria pessoa nomeia ansiedade, isso não nos autoriza a decidir por ela o que deve fazer.

Esse cuidado é importante porque palavras emocionais podem ser usadas em níveis diferentes:

| O que está sendo relatado | Exemplo | O que ainda não sabemos |
|---|---|---|
| Sensação ou sinal | “Senti o peito apertado.” | O que causou a sensação |
| Palavra escolhida | “Acho que foi ansiedade.” | Se essa palavra é suficiente para descrever o momento |
| Interpretação | “Isso aconteceu porque vou falhar.” | Se a previsão corresponde ao que ocorrerá |
| Ação escolhida | “Vou pedir que repitam a pergunta.” | Como os outros responderão |

Uma conversa respeitosa mantém essas camadas separadas. A pessoa pode dizer “estou ansiosa e quero continuar”, “não sei o que estou sentindo” ou “prefiro não falar disso”. Todas são respostas possíveis. A meta do exercício não é substituir uma palavra imprecisa por uma etiqueta supostamente correta, mas ampliar as opções de linguagem sem obrigar ninguém a usá-las.

## Registrar uma opção informada, não uma leitura do ambiente

No tipo `RegistroEmocional`, o parâmetro `EmocaoNomeada` vem de uma escolha declarada. O código verifica se o número recebido corresponde a algum item válido da enumeração:

```cs
var registro = new RegistroEmocional(
    EmocaoNomeada.AindaNaoSei,
    "Percebi que estava apertando as mãos.",
    "Exemplo fictício de uma apresentação.",
    DateTimeOffset.Parse("2026-10-28T14:00:00-03:00"));
```

`Enum.IsDefined` impede que um valor numérico inventado — por exemplo, `(EmocaoNomeada)999` — atravesse a fronteira do objeto como se fosse uma opção válida. A regra não decide se a descrição é psicologicamente correta; ela garante apenas que o dado respeita o tipo técnico.

O momento usa `DateTimeOffset` porque um instante pode ser comparado entre fusos. A data não prova quando uma emoção começou, quanto durou ou por que apareceu. Aqui ela apenas identifica o horário que o autor do exemplo escolheu registrar. Para este livro, os dados são fictícios e ficam na memória do processo.

O enum inclui `AindaNaoSei` e `PrefiroNaoNomear`: um bom vocabulário de opções precisa permitir a incerteza e a recusa. Sem essas opções, a interface poderia pressionar a pessoa a inventar um nome só para satisfazer o programa.

Os nomes dos membros C# ficam sem acentos porque são identificadores do código. A apresentação usa uma função separada:

```cs Jornada/Emocoes/RotulosEmocionais.cs
namespace Jornada.Emocoes;

public static class RotulosEmocionais
{
    public static string Descrever(EmocaoNomeada emocao) => emocao switch
    {
        EmocaoNomeada.Alegria => "Alegria",
        EmocaoNomeada.Tristeza => "Tristeza",
        EmocaoNomeada.Raiva => "Raiva",
        EmocaoNomeada.Medo => "Medo",
        EmocaoNomeada.Ansiedade => "Ansiedade",
        EmocaoNomeada.Frustracao => "Frustração",
        EmocaoNomeada.Cansaco => "Cansaço",
        EmocaoNomeada.AindaNaoSei => "Ainda não sei",
        EmocaoNomeada.PrefiroNaoNomear => "Prefiro não nomear",
        EmocaoNomeada.Outra => "Outra",
        _ => throw new ArgumentOutOfRangeException(nameof(emocao))
    };
}
```

Uma aplicação localizada poderia selecionar rótulos por idioma em arquivos de recursos. Separar o nome estável do enum do texto exibido evita acoplar o identificador C# à frase apresentada e deixa mais simples traduzir a interface. O rótulo “Prefiro não nomear” continua sendo uma opção escolhida, nunca uma dedução.

Esse mapeamento é uma fronteira entre domínio e apresentação. O modelo mantém `Frustracao` como um valor estável, enquanto a camada de tela pode exibir “Frustração”, outro idioma ou texto alternativo. É melhor que a aplicação não dependa de `Enum.ToString()` para texto final: a grafia do identificador é conveniente para depuração, mas não é necessariamente o rótulo que uma interface deve mostrar.

O caso inválido também importa. Enumerations são tipos numéricos; uma conversão explícita consegue produzir um valor não definido. `Enum.IsDefined` protege `RegistroEmocional` quando ele é criado, e o `switch` protege o formatador caso alguém chame a função diretamente. As duas verificações estão em fronteiras diferentes: validar o objeto não torna todas as outras funções impossíveis de chamar com um valor fabricado.

```cs
Assert.Equal("Frustração", RotulosEmocionais.Descrever(EmocaoNomeada.Frustracao));
Assert.Equal("Ainda não sei", RotulosEmocionais.Descrever(EmocaoNomeada.AindaNaoSei));
Assert.Throws<ArgumentOutOfRangeException>(() =>
    RotulosEmocionais.Descrever((EmocaoNomeada)999));
```

Uma enumeração é apropriada quando o conjunto é relativamente estável. Se o vocabulário de emoções precisasse ser configurado por pessoas ou por cultura, um enum compilado deixaria de ser suficiente. O tipo escolhido deve acompanhar a necessidade real; não precisamos codificar um inventário universal para o exercício.

## Como o tipo protege o contrato

O construtor valida duas categorias diferentes:

1. **Domínio fechado:** `Emocao` deve estar definida na enum.
2. **Presença de texto:** sinal e contexto não podem ser nulos, vazios ou apenas espaços.

Se um campo não é conhecido, não escreva uma frase fictícia para ultrapassar a validação. Reveja o modelo e a finalidade da informação. Uma propriedade obrigatória pode ser inadequada se a pergunta correspondente não deveria ser obrigatória. Validação é uma decisão sobre o contrato do aplicativo, e não um motivo para exigir detalhes privados.

O método `Trim()` remove espaços no início e no fim, mas conserva o texto da pessoa. Não há classificação automática nem uma lista de palavras “permitidas”. Um filtro desse tipo poderia rejeitar uma descrição válida ou insinuar que o programa sabe mais do que quem relatou a experiência.

## Perguntas técnicas para ler o construtor

Ao abrir `RegistroEmocional.cs`, acompanhe a criação do objeto como um contrato:

1. A enum precisa ser válida antes de ser guardada.
2. O sinal e o contexto precisam ter conteúdo.
3. O construtor normaliza apenas espaços externos.
4. As propriedades não têm `set`, então o valor não muda depois da criação.
5. O instante é guardado sem ser convertido para o fuso local.

Esse fluxo é chamado de **validar na fronteira**: o objeto impede que uma chamada inválida entre no domínio e obriga o consumidor a tratar o erro no lugar em que recebe os dados. Se vierem de uma tela, a tela pode validar e mostrar uma mensagem legível; se forem criados diretamente no programa, a exceção mostra ao desenvolvedor qual argumento violou a regra.

Uma alternativa seria declarar um `record` posicional sem construtor explícito. Seria mais curto, mas permitiria números inválidos para a enum e textos em branco. A decisão de escrever o construtor adiciona código, porém deixa as regras verificáveis. Para dados que realmente podem faltar, como a palavra emocional opcional do capítulo 117, o modelo tem de indicar essa ausência em vez de rejeitá-la.

Teste o comportamento positivo e os limites negativos. Uma classe que só testa erros pode rejeitar tudo; uma classe que só testa um caso válido pode deixar passar entradas que quebram suas suposições:

```cs
var valido = new RegistroEmocional(
    EmocaoNomeada.PrefiroNaoNomear,
    "Parei para ler a mensagem duas vezes.",
    "Exemplo fictício de revisão de código.",
    DateTimeOffset.UnixEpoch);

Assert.Equal(EmocaoNomeada.PrefiroNaoNomear, valido.Emocao);
Assert.Throws<ArgumentOutOfRangeException>(() =>
    new RegistroEmocional((EmocaoNomeada)999, "Sinal", "Contexto", DateTimeOffset.UnixEpoch));
```

O nome `UnixEpoch` aparece somente num teste determinístico. Os testes evitam `DateTimeOffset.Now` porque o resultado dependeria do instante real em que forem executados.

::: pense Três frases, três níveis
Classifique estas frases como sinal, interpretação ou escolha: “minha voz acelerou”; “a equipe vai me reprovar”; “vou pedir que repitam a pergunta”. Em seguida, pense numa explicação diferente para a primeira frase.
:::

::: exercicio Observe sem concluir o motivo
Durante uma apresentação, alguém fala mais rápido e perde uma linha do roteiro. Escreva um sinal que poderia ser observado e duas explicações diferentes que ainda precisariam ser verificadas.
---
**Resposta possível:** sinal: “a fala acelerou e uma linha foi pulada”. Pode haver nervosismo, falta de tempo, interrupções ou um problema no slide; o sinal isolado não permite decidir a causa.
:::

::: exercicio Complete um rótulo de interface
No arquivo `RotulosEmocionais.cs`, adicione um caso para `EmocaoNomeada.Tristeza`, com o texto “Tristeza”. Escreva um teste que confirme o rótulo e outro que confirme que um valor não definido gera `ArgumentOutOfRangeException`.
---
**Resposta possível:** o ramo `EmocaoNomeada.Tristeza => "Tristeza"` entra antes do ramo `_`. No teste: `Assert.Equal("Tristeza", RotulosEmocionais.Descrever(EmocaoNomeada.Tristeza))`; para o valor inválido, use `Assert.Throws<ArgumentOutOfRangeException>(() => RotulosEmocionais.Descrever((EmocaoNomeada)999))`. O teste verifica a apresentação e a fronteira técnica, não uma emoção.
:::

::: quiz
? O que podemos concluir de um sinal corporal isolado?
+ Apenas que o sinal foi percebido; a causa ainda precisa de contexto.
- Qual emoção a pessoa certamente sente.
- Que ação ela deve tomar.
> O corpo pode informar, mas um sinal não fecha sozinho uma interpretação.

? Para que serve `EmocaoNomeada` no exemplo?
+ Registrar uma opção escolhida por quem descreve a experiência.
- Classificar automaticamente a pessoa.
- Diagnosticar a causa de uma reação.
> A enum organiza opções explícitas; não substitui a voz de quem sente.

? Sentir raiva obriga alguém a responder imediatamente?
+ Não; sentimento e ação são coisas distintas.
- Sim, porque emoção é uma ordem.
- Sim, se o sentimento for intenso.
> Uma pausa ou outra resposta pode ser escolhida, quando for possível.
? Por que `EmocaoNomeada` inclui `AindaNaoSei` e `PrefiroNaoNomear`?
+ Para permitir incerteza e recusa, sem pressionar a pessoa a inventar um nome
- Para o programa descobrir a emoção
- Para economizar código
- Porque o C# exige três opções neutras
> Um bom vocabulário de opções precisa permitir não saber e não dizer.

? O que `Enum.IsDefined` impede no construtor de `RegistroEmocional`?
+ Que um número inventado, como `(EmocaoNomeada)999`, passe como se fosse uma opção válida
- Que a pessoa sinta emoções inválidas
- Que o texto fique vazio
- Que o instante seja futuro
> Ele verifica o tipo técnico, não a psicologia da descrição.

? Por que os membros do `enum` não têm acento e há uma função de rótulos separada?
+ Identificadores são para o código; o texto exibido pode mudar (idioma, grafia) sem mexer no tipo
- Porque o C# proíbe acentos em texto
- Para esconder a emoção do usuário
- Porque `ToString()` não funciona
> Separar o nome estável do rótulo facilita traduzir e evolui sem quebrar o domínio.
:::

::: resumo
- Notar um sinal não é o mesmo que explicar sua causa.
- Uma emoção pode trazer informação sem determinar a ação.
- `enum` registra uma opção escolhida — inclusive “ainda não sei” — e não infere estados internos.
- Contexto e incerteza fazem parte de uma descrição honesta.
:::

::: cartas
Sinal percebido :: Indício descrito por quem vive a situação; não é diagnóstico.
Emoção nomeada :: Palavra escolhida para descrever uma experiência num momento.
Contexto :: Condições e acontecimentos que ajudam a entender uma situação sem reduzi-la a uma causa.
Enum :: Tipo com um conjunto fixo de opções nomeadas.
`record` :: Tipo de dados com igualdade por valor e propriedades que não mudam depois da criação.
`Enum.IsDefined` :: Verifica se um valor numérico corresponde a uma opção declarada.
Validar na fronteira :: Recusar dados inválidos no momento em que o objeto é criado.
Rótulo de interface :: Texto exibido ao usuário, separado do identificador estável do código.
:::
