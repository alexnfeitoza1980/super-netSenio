---
id: emocional-cap117
order: 152
kind: capitulo
num: 117
group: C# & Mentalidade · Parte 17 · Emoções e C#
title: Dar nome sem virar rótulo
subtitle: Palavras mais precisas podem ajudar a comunicar uma experiência. Elas não definem identidade, personalidade ou capacidade.
minutes: 45
hue: 182
objectives:
- Separar fato observado, interpretação e palavra emocional
- Reconhecer que vocabulários emocionais variam entre pessoas e contextos
- Usar validação em C# sem afirmar que o programa sabe o que alguém sente
origem: livros/09-inteligencia-emocional-csharp/content/145-cap117.md
---

::: vila “Sou uma pessoa difícil”
Depois de uma reunião tensa, Ana anota “sou uma pessoa difícil”. Quando descreve o episódio, encontra dados mais específicos: duas pessoas falaram ao mesmo tempo, ela interrompeu para corrigir uma data e saiu sem combinar o próximo passo. “Difícil” era uma avaliação ampla; os comportamentos podem ser discutidos sem transformar o episódio numa identidade.
:::

::: ilustracao img/emocional/cap117.svg | Uma palavra ampla resume demais. Descrever o que aconteceu devolve opções: fatos, interpretação e a palavra que a pessoa escolhe.
:::

## A palavra é um convite, não uma sentença

É comum usar uma palavra ampla — “mal”, “bem”, “nervoso” — quando ainda não sabemos detalhar a experiência. Podemos perguntar se há frustração, cansaço, preocupação, alívio ou uma mistura. Não existe uma tabela universal que converta gesto, frase ou expressão em emoção certa. A própria pessoa pode preferir outra palavra ou não querer nomear nada.

Também convém separar:

| Tipo de informação | Exemplo |
|---|---|
| Fato observado | “A reunião terminou sem definir responsáveis.” |
| Interpretação | “Ninguém valoriza meu trabalho.” |
| Palavra escolhida | “Estou frustrada.” |
| Pergunta em aberto | “O que levou a equipe a encerrar sem esse acordo?” |

Uma interpretação pode fazer sentido para quem a vive e ainda assim não ser o único relato possível. Ouvir e perguntar é mais respeitoso do que corrigir a emoção alheia.

::: figura Três camadas num mesmo relato
<svg viewBox="0 0 720 190" role="img" aria-label="Fato observado, interpretação e emoção nomeada são informações diferentes guardadas em campos diferentes">
<rect x="20" y="36" width="200" height="78" rx="12" class="f-info"/><text x="120.0" y="72.0" text-anchor="middle" font-weight="800" font-size="13">FatoObservado</text><text x="120.0" y="89.0" text-anchor="middle" font-size="11" class="t-dim">o que ocorreu, específico</text><rect x="260" y="36" width="200" height="78" rx="12" class="f-warn"/><text x="360.0" y="72.0" text-anchor="middle" font-weight="800" font-size="13">Interpretacao</text><text x="360.0" y="89.0" text-anchor="middle" font-size="11" class="t-dim">uma hipótese sobre o que significa</text><rect x="500" y="36" width="200" height="78" rx="12" class="f-good"/><text x="600.0" y="72.0" text-anchor="middle" font-weight="800" font-size="13">EmocaoNomeada?</text><text x="600.0" y="89.0" text-anchor="middle" font-size="11" class="t-dim">opcional: pode ser null</text><text x="360" y="154" text-anchor="middle" font-size="13" class="t-dim">O código confere que há texto; não confere se a interpretação é verdadeira.</text><text x="360" y="176" text-anchor="middle" font-size="13" class="t-dim">Ausência (null) é um estado a modelar, e não um erro da pessoa.</text>
</svg>
:::

## Validar o que foi informado

Um tipo explícito ajuda a manter separadas as três camadas:

```cs Jornada/Emocoes/RegistroEmocional.cs
namespace Jornada.Emocoes;

public sealed record RegistroEmocional
{
    public EmocaoNomeada Emocao { get; }
    public string SinalObservado { get; }
    public string Contexto { get; }
    public DateTimeOffset Momento { get; }

    public RegistroEmocional(
        EmocaoNomeada emocao,
        string sinalObservado,
        string contexto,
        DateTimeOffset momento)
    {
        if (!Enum.IsDefined(emocao))
            throw new ArgumentOutOfRangeException(nameof(emocao));

        Emocao = emocao;
        SinalObservado = ExigirTexto(sinalObservado, nameof(sinalObservado));
        Contexto = ExigirTexto(contexto, nameof(contexto));
        Momento = momento;
    }

    private static string ExigirTexto(string valor, string parametro)
    {
        if (string.IsNullOrWhiteSpace(valor))
            throw new ArgumentException("Informe uma descrição ou use um exemplo neutro.", parametro);

        return valor.Trim();
    }
}
```

A validação verifica campos do software, não a verdade do sentimento. Neste projeto o registro será usado com dados fictícios e permanecerá em memória.

## Três camadas podem coexistir

Uma descrição mais completa pode manter um fato, uma interpretação e a palavra escolhida em propriedades diferentes:

```cs Jornada/Emocoes/ObservacaoEmocional.cs
namespace Jornada.Emocoes;

public sealed record ObservacaoEmocional
{
    public string FatoObservado { get; }
    public string Interpretacao { get; }
    public string? EmocaoNomeada { get; }

    public ObservacaoEmocional(string fatoObservado, string interpretacao, string? emocaoNomeada = null)
    {
        FatoObservado = ExigirTexto(fatoObservado, nameof(fatoObservado));
        Interpretacao = ExigirTexto(interpretacao, nameof(interpretacao));
        EmocaoNomeada = emocaoNomeada is null
            ? null
            : ExigirTexto(emocaoNomeada, nameof(emocaoNomeada));
    }

    private static string ExigirTexto(string valor, string parametro)
    {
        if (string.IsNullOrWhiteSpace(valor))
            throw new ArgumentException("Informe uma descrição ou use um exemplo neutro.", parametro);

        return valor.Trim();
    }
}
```

Com o registro preenchido, a informação fica visível sem ser misturada:

```cs
var observacao = new ObservacaoEmocional(
    "A conversa terminou sem definir responsáveis.",
    "Talvez minhas contribuições não tenham sido ouvidas.",
    "Frustração");
```

Repare que a interpretação começa com “talvez”. Ela pode expressar uma hipótese significativa para a pessoa, mas não descreve a intenção de todo mundo na reunião. O sistema não tenta reescrever a hipótese como fato nem apaga a palavra escolhida.

## O que `record` faz — e o que não faz

`record` oferece igualdade baseada nos valores de seus membros. Dois registros com o mesmo fato, interpretação e palavra podem ser considerados iguais, mesmo que tenham sido criados em momentos distintos. Isso é útil para comparar valores em testes, mas não deve ser confundido com identificar duas pessoas ou afirmar que duas experiências são equivalentes.

```cs
var primeiro = new ObservacaoEmocional("A reunião terminou.", "Fiquei sem resposta.", "Frustração");
var segundo = new ObservacaoEmocional("A reunião terminou.", "Fiquei sem resposta.", "Frustração");

Console.WriteLine(primeiro == segundo); // True: os valores coincidem.
```

O exemplo não está dizendo que as pessoas sentem a mesma coisa; ele compara apenas os três textos. Se a igualdade por valor não combinar com a finalidade de um futuro sistema, não se deve usar um record só por conveniência.

As propriedades são somente leitura e inicializadas pelo construtor. Essa escolha evita que o objeto comece válido e seja alterado depois para conter um campo vazio:

```cs
// Não compila: as propriedades de ObservacaoEmocional não têm setter.
// observacao.FatoObservado = "";
```

Há um limite importante: a classe exige texto nas propriedades de fato e interpretação, mas permite que `EmocaoNomeada` fique ausente. Isso respeita uma interface em que a pessoa prefere não nomear uma emoção. Não altere a experiência humana para se adequar ao tipo; mude o contrato quando as opções e a finalidade exigirem isso. O tipo `RegistroEmocional`, do capítulo anterior, também oferece `AindaNaoSei` e `PrefiroNaoNomear`.

## Validar sem fingir neutralidade

A validação de texto vazio é objetiva. Já afirmar que um fato está “neutro” ou que uma interpretação está “distorcida” seria uma análise que o programa não faz. Até o campo chamado `FatoObservado` depende da descrição de alguém; a classe apenas separa uma categoria de informação, não garante imparcialidade. Em ferramentas de escrita, nomes claros ajudam, mas não substituem a revisão de quem escreve.

Uma regra de validação razoável tem uma pergunta por trás: que erro técnico específico ela impede? `IsNullOrWhiteSpace` impede a ausência de texto. Não impede omissões, não certifica memórias e não julga a interpretação. Ao escrever testes, mantenha essa fronteira explícita:

```cs
Assert.Throws<ArgumentException>(() =>
    new ObservacaoEmocional("", "Talvez tenha havido uma interrupção."));
```

Esse teste mostra que o contrato de entrada está sendo aplicado. Não valida se “frustração” é a palavra ideal.

## Nulo é um estado que deve ser modelado

Com `string? EmocaoNomeada`, a interrogação informa ao compilador que a propriedade pode não conter uma string. O valor nulo tem significado deliberado: neste exemplo, a pessoa não informou uma palavra. Não é o mesmo que `""`, que é texto vazio, nem que `"Ainda não sei"`, que é uma resposta escrita.

```cs
var semNome = new ObservacaoEmocional(
    "A reunião terminou sem responsáveis.",
    "Ainda não sei como interpretar o encerramento.");

Console.WriteLine(semNome.EmocaoNomeada ?? "Não informado");
```

O operador `??` escolhe o texto de apresentação quando o valor é nulo. Não altera o objeto original nem inventa uma resposta. Outro uso é o padrão `is null`, que deixa explícito um ramo:

```cs
if (semNome.EmocaoNomeada is null)
    Console.WriteLine("A pessoa preferiu não informar um nome.");
else
    Console.WriteLine($"Palavra informada: {semNome.EmocaoNomeada}");
```

Note que a mensagem “preferiu não informar” só é correta se esse for o significado definido para `null`. Em sistemas com vários motivos para a ausência, seria melhor representá-los explicitamente com uma opção ou tipo próprio. O código do livro reduz os estados para fins didáticos; não deduz o motivo do nulo.

## Construtor e imutabilidade

Uma propriedade sem `set` é inicializada no construtor e permanece estável para os chamadores. O registro completo se beneficia dessa imutabilidade: testes conseguem comparar dois estados e o consumidor não pode apagar o fato depois de validado.

Records oferecem `with`, mas esse operador cria uma cópia com propriedades alteradas apenas quando elas permitem `init` ou `set`. Neste tipo, as propriedades têm apenas `get`; por isso não é possível substituir silenciosamente o fato:

```cs
// Não compila com as propriedades somente de leitura do modelo:
// var revisado = observacao with { FatoObservado = "Outro fato" };
```

Se uma edição posterior fosse necessária, poderíamos criar um novo objeto usando o construtor. Essa escolha torna a transição visível e faz as validações rodarem novamente. Para um caderno real, ainda seria preciso decidir se cópias antigas devem existir; esta biblioteca não conserva histórico.

Uma implementação poderia ser mais genérica e usar um tipo `Optional<T>`, distinguindo “não perguntado”, “prefiro não dizer” e “informado”. Para um exemplo introdutório, `string?` é suficiente porque a finalidade e o escopo são pequenos. Não introduzimos abstrações que o laboratório não utiliza.

## Testes pequenos explicam o limite

Um bom conjunto de testes cobre tanto o caso em que a pessoa escolhe uma palavra quanto o caso em que a omite:

```cs
var comPalavra = new ObservacaoEmocional("Fato", "Hipótese", "Frustração");
var semPalavra = new ObservacaoEmocional("Fato", "Hipótese");

Assert.Equal("Frustração", comPalavra.EmocaoNomeada);
Assert.Null(semPalavra.EmocaoNomeada);
```

Esses testes demonstram o contrato opcional. Nenhum classifica a hipótese, valida o sentimento ou compara experiências. Ao escrever testes para modelos humanos, nomeie o que o código realmente garante — e deixe claro o que permanece fora do programa.

## Escolher entre `null`, vazio e opção

Uma decisão de modelagem define quais estados são possíveis:

| Representação | Significado possível | Cuidado |
|---|---|---|
| `string?` nula | Nenhuma palavra foi fornecida | Não distingue “não perguntado” de “preferi não responder” |
| `""` | Texto fornecido, mas vazio | Pode ser confundido com falta de validação |
| `"Ainda não sei"` | A pessoa informou essa resposta | É diferente de não responder |
| `EmocaoNomeada.PrefiroNaoNomear` | A pessoa selecionou uma opção explícita | Exige que essa opção exista no conjunto |

O tipo do capítulo permite ausência com `null`, enquanto a enum do capítulo 116 inclui respostas nomeadas. São alternativas de design para contextos diferentes. Em um formulário, é possível que a pessoa nem tenha visto a pergunta; em outro, ela pode ter escolhido não responder. Se essa diferença mudar o comportamento do sistema, use um tipo que a preserve. Se não mudar nada e o aplicativo nem precisa do dado, a opção ainda melhor pode ser não perguntar.

Com `string?`, o compilador ajuda a localizar acessos que não tratam o caso nulo. Não desative os avisos com `!` só para silenciar um diagnóstico: isso afirma ao compilador que você conhece um fato que o programa não verificou. Prefira `if`, `??` ou um retorno que mantenha a ausência clara.

```cs
string MostrarNome(ObservacaoEmocional observacao) =>
    observacao.EmocaoNomeada is { Length: > 0 } nome
        ? $"Palavra informada: {nome}"
        : "Nenhuma palavra foi informada.";
```

Esse método resume dois estados — nulo e texto vazio — porque ambos significam “sem palavra” para a apresentação. A distinção original continua no objeto; a decisão de resumir é local e explícita. Em outras funcionalidades, o mesmo resumo poderia ser incorreto.

::: pense Escolha um nome de campo melhor
Se fosse necessário permitir que o campo de emoção ficasse sem preenchimento, que tipo ou opção de interface respeitaria essa escolha? Considere um valor anulável, uma opção explícita “prefiro não nomear” ou separar a pergunta do registro.
:::

::: exercicio Troque o rótulo amplo por detalhes
Reescreva “sou péssimo em equipe” separando um fato de uma interpretação e uma palavra emocional possível. A situação pode ser inventada.
---
**Resposta possível:** fato: “falei por cima de uma colega na reunião”; interpretação: “não sei colaborar”; emoção: “vergonha” ou “frustração”. Um episódio é algo a revisar, não um rótulo permanente.
:::

::: exercicio Compare os valores do registro
Crie dois objetos `ObservacaoEmocional` com o mesmo fato e a mesma interpretação; no primeiro, informe “Frustração”, e no segundo, omita a emoção. O que a igualdade de `record` deve retornar? Escreva também uma asserção para o segundo campo opcional.
---
**Resposta possível:** os registros não são iguais porque os valores dos membros não coincidem (`"Frustração"` e `null`). `Assert.NotEqual(comNome, semNome)` testa isso, e `Assert.Null(semNome.EmocaoNomeada)` confirma que a ausência foi preservada. A desigualdade compara os objetos de dados, não a identidade nem a validade de uma experiência.
:::

::: quiz
? Qual frase é mais próxima de um fato observável?
+ “A reunião terminou sem responsáveis definidos.”
- “Ninguém se importa comigo.”
- “Sou incapaz de trabalhar em grupo.”
> A primeira descreve o que aconteceu; as outras incluem interpretações sobre intenções e identidade.

? O que a validação de `RegistroEmocional` comprova?
+ Que os campos obrigatórios têm texto e a enum tem um valor válido.
- Que a emoção registrada é a emoção “correta”.
- Que o episódio tem uma causa psicológica específica.
> Validação de entrada protege o contrato do programa, não avalia a experiência humana.

? Uma pessoa escolhe uma palavra diferente da nossa. O melhor é:
+ Perguntar o que essa palavra significa para ela e respeitar sua descrição.
- Substituí-la pela palavra que julgamos mais precisa.
- Dizer que a pessoa não sabe o que sente.
> Vocabulário emocional é pessoal e depende de contexto.
? Por que `EmocaoNomeada` em `ObservacaoEmocional` é `string?`?
+ Porque a palavra emocional é opcional: a ausência é um estado a modelar
- Porque o C# obriga todo texto a ser anulável
- Para esconder o fato observado
- Para economizar memória
> Se a pessoa não quer ou não sabe nomear, o modelo precisa permitir.

? Qual a diferença entre fato observado e interpretação?
+ O fato descreve o que ocorreu; a interpretação é uma hipótese sobre o que significa
- Não há diferença
- A interpretação é sempre verdadeira
- O fato depende do que a pessoa sente
> Separar as camadas evita tratar uma hipótese como fato.

? O que `==` compara em dois `record`s do mesmo tipo?
+ Os valores das propriedades
- A identidade dos objetos na memória
- As emoções das pessoas
- O instante de criação
> `record` tem igualdade por valor; isso não compara experiências humanas.
:::

::: resumo
- Fato, interpretação e emoção nomeada são informações diferentes.
- Uma palavra pode ajudar a conversar sem definir a identidade.
- `record` compara valores; não compara identidades nem experiências humanas.
- O código valida a presença de campos; não valida a emoção nem a neutralidade do relato.
- Respeite as palavras e os limites escolhidos por quem relata a experiência.
:::

::: cartas
Rótulo :: Palavra ampla que pode resumir demais uma situação ou identidade.
Validação :: Verificação de regras dos dados de entrada, sem julgar sua verdade humana.
Descrição observável :: Relato específico do que ocorreu no contexto apresentado.
Interpretação :: Hipótese sobre o significado de um fato; pode ser revista.
Valor opcional (`null`) :: Ausência de valor, modelada de propósito.
Imutabilidade :: O objeto não muda depois de criado; para alterar, cria-se outro.
Igualdade por valor :: Dois objetos são iguais quando suas propriedades são iguais.
Camadas do relato :: Fato, interpretação e palavra emocional, mantidos em campos separados.
:::
