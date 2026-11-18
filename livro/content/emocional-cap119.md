---
id: emocional-cap119
order: 154
kind: capitulo
num: 119
group: C# & Mentalidade · Parte 17 · Emoções e C#
title: Atenção, tarefa e contexto
subtitle: Uma experiência pode influenciar o foco, mas prazos, ferramentas, interrupções e condições de trabalho também moldam o que acontece.
minutes: 45
hue: 206
objectives:
- Considerar fatores de contexto ao planejar uma tarefa
- Evitar atribuir um erro ou atraso somente a uma emoção
- Criar um plano de retomada com apoio e revisão definidos
origem: livros/09-inteligencia-emocional-csharp/content/147-cap119.md
---

::: vila O deploy às pressas
Na tarde de uma entrega, Marina está preocupada com um teste instável. Ela lê a mesma linha várias vezes e não nota que o ambiente de CI mudou de versão. “Não tenho foco” parece explicar o dia inteiro, mas a tarefa, as interrupções, a falta de aviso e o prazo também contam. A equipe decide dividir a investigação e registrar a alteração de ambiente antes de retomar.
:::

::: ilustracao img/emocional/cap119.svg | Atenção e tarefa dependem também de prazos, ferramentas e interrupções: nem tudo está "dentro da pessoa".
:::

## Não explique tudo por dentro da pessoa

Emoções podem participar da atenção, assim como o ambiente e as exigências da tarefa. Se alguém está cansado, interrompido ou sem acesso a uma ferramenta, orientar-se apenas a “controlar a emoção” ignora causas práticas que podem ser mudadas coletivamente.

Ao planejar uma retomada, separe:

1. o que a tarefa exige;
2. que condição está dificultando o trabalho;
3. que parte pode ser ajustada agora;
4. de que apoio ou acordo a equipe precisa;
5. quando será útil revisar o plano.

Uma revisão não precisa medir produtividade. Seu propósito é verificar se a ação ainda cabe no contexto.

::: figura Um plano de retomada que pode ser revisto
<svg viewBox="0 0 720 186" role="img" aria-label="Tarefa, ajuste de contexto e apoio levam a um instante de revisão, que reabre o plano">
<rect x="10" y="36" width="150" height="70" rx="12" class="f-info"/><text x="85.0" y="68.0" text-anchor="middle" font-weight="800" font-size="13">Tarefa</text><text x="85.0" y="85.0" text-anchor="middle" font-size="11" class="t-dim">o que retomar</text><path class="arrow" d="M162 71H180"/><rect x="184" y="36" width="170" height="70" rx="12" class="f-warn"/><text x="269.0" y="68.0" text-anchor="middle" font-weight="800" font-size="13">AjusteDeContexto</text><text x="269.0" y="85.0" text-anchor="middle" font-size="11" class="t-dim">o que mudar no entorno</text><path class="arrow" d="M356 71H374"/><rect x="378" y="36" width="150" height="70" rx="12" class="f-good"/><text x="453.0" y="68.0" text-anchor="middle" font-weight="800" font-size="13">Apoio</text><text x="453.0" y="85.0" text-anchor="middle" font-size="11" class="t-dim">quem ou o quê ajuda</text><path class="arrow" d="M530 71H548"/><rect x="552" y="36" width="158" height="70" rx="12" class="f-prim"/><text x="631.0" y="68.0" text-anchor="middle" font-weight="800" font-size="13">RevisarEm</text><text x="631.0" y="85.0" text-anchor="middle" font-size="11" class="t-dim">DateTimeOffset</text><path d="M630 108 V140 H85 V108" fill="none" class="arrow"/><text x="360" y="166" text-anchor="middle" font-size="13" class="t-dim">A revisão atualiza o plano. Ela não dá nota a ninguém.</text>
</svg>
:::

## Um plano temporal explícito

`DateTimeOffset` representa um instante com informação de fuso. Para um roteiro técnico, ele pode ajudar a registrar quando rever uma tarefa:

```cs Jornada/Emocoes/PlanoDeRetomada.cs
namespace Jornada.Emocoes;

public sealed record PlanoDeRetomada
{
    public string Tarefa { get; }
    public string AjusteDeContexto { get; }
    public string Apoio { get; }
    public DateTimeOffset RevisarEm { get; }

    public PlanoDeRetomada(
        string tarefa,
        string ajusteDeContexto,
        string apoio,
        DateTimeOffset revisarEm)
    {
        Tarefa = ExigirTexto(tarefa, nameof(tarefa));
        AjusteDeContexto = ExigirTexto(ajusteDeContexto, nameof(ajusteDeContexto));
        Apoio = ExigirTexto(apoio, nameof(apoio));
        if (revisarEm == default)
            throw new ArgumentOutOfRangeException(nameof(revisarEm), "Informe um instante de revisão explícito.");

        RevisarEm = revisarEm;
    }

    private static string ExigirTexto(string valor, string parametro)
    {
        if (string.IsNullOrWhiteSpace(valor))
            throw new ArgumentException("Informe o campo ou use um exemplo neutro.", parametro);

        return valor.Trim();
    }
}
```

Um exemplo fictício:

```cs
var plano = new PlanoDeRetomada(
    tarefa: "Investigar o teste instável do deploy.",
    ajusteDeContexto: "Separar o ambiente de CI do teste local.",
    apoio: "Pedir revisão a outra pessoa da equipe.",
    revisarEm: DateTimeOffset.Parse("2026-10-29T10:00:00-03:00"));
```

O plano registra um acordo, não uma promessa de que a tarefa estará concluída. Se surgirem novas informações, a revisão pode mudar.

## Separar o que é da tarefa do que é da pessoa

Considere um teste que falhou só no ambiente de integração contínua. Existem pelo menos três perguntas diferentes:

- **Sobre o trabalho:** o teste falha sempre ou apenas em certas execuções?
- **Sobre o ambiente:** que versão do runtime, dependência ou variável mudou?
- **Sobre a experiência:** que efeito a incerteza da entrega está tendo para quem investiga?

Uma resposta que pula direto para “precisa se concentrar” personaliza um defeito que pode estar no pipeline. A direção oposta também pode falhar: dizer que “é só um problema técnico” pode ignorar que a pessoa está sobrecarregada. A prática completa reconhece os dois planos, sem transformar um no diagnóstico do outro.

Uma breve análise de contexto pode listar fatos verificáveis e perguntas abertas:

| Dado conhecido | Pergunta que ainda falta |
|---|---|
| A falha ocorre só no CI | Qual etapa e versão do ambiente diferem? |
| Há várias tarefas concorrentes | Qual delas tem prazo ou dependência externa? |
| Outra pessoa pode revisar | Quem está disponível e que parte deve revisar? |
| O plano tem horário de retorno | Esse horário ainda é realista? |

Esse método ajuda a mudar condições de trabalho quando possível. Não se trata de coletar um histórico privado nem de inferir estados internos; é uma lista para planejar a próxima ação da equipe.

## Por que `DateTimeOffset`?

O tipo representa uma data e hora com deslocamento UTC. Por isso, um instante expresso em `2026-10-29T10:00:00-03:00` contém o deslocamento `-03:00` e pode ser comparado com a mesma hora escrita noutro fuso. Uma string sem deslocamento não deixa claro se é hora local, UTC ou apenas um horário sem zona.

```cs
var revisarEm = DateTimeOffset.Parse("2026-10-29T10:00:00-03:00");
Console.WriteLine(revisarEm.ToUniversalTime());
```

Este tipo não decide quando a pessoa deveria parar. Também não é a mesma coisa que guardar uma duração: para representar “trabalhar por vinte minutos”, precisaríamos de `TimeSpan`. A agenda de revisão é um acordo prático que pode ser alterado.

No construtor de `PlanoDeRetomada`, o valor `default` é recusado porque não representa um horário útil para uma revisão deliberada. Em aplicações de verdade, também seria necessário decidir como validar datas passadas ou janelas de operação; essa regra depende do produto e não foi inventada para este exercício.

## Uma unidade de função para cada propriedade

`PlanoDeRetomada` combina quatro dados com papéis diferentes:

- `Tarefa` identifica o trabalho a retomar;
- `AjusteDeContexto` registra uma mudança de condição;
- `Apoio` nomeia um recurso ou outra pessoa;
- `RevisarEm` lembra quando voltar ao plano.

Se todos os campos fossem condensados numa única frase, ficaria difícil testar e atualizar o horário sem reescrever os demais dados. Em contrapartida, criar dezenas de campos para cada detalhe também tornaria o modelo pesado. Comece pelas informações necessárias para uma decisão concreta e não inclua sentimentos, identidade ou dados privados que o exercício não precisa.

Uma saída possível:

```cs
var plano = new PlanoDeRetomada(
    "Investigar o teste instável do deploy.",
    "Comparar as versões do ambiente CI e local.",
    "Pedir revisão a outra pessoa da equipe.",
    DateTimeOffset.Parse("2026-10-29T10:00:00-03:00"));

Console.WriteLine($"{plano.Tarefa} · apoio: {plano.Apoio} · rever em {plano.RevisarEm:O}");
```

O formato `"O"` imprime um timestamp ISO 8601 round-trip, que preserva informação suficiente para análise e comparação. Aqui é útil em logs de demonstração; não significa que se deva registrar a vida emocional de alguém em logs de produção.

## Instante não é duração nem fuso da equipe

Um instante pode ser escrito com deslocamentos diferentes e ainda apontar para o mesmo momento. Por exemplo, `10:00-03:00` corresponde a `13:00+00:00`. A ordenação compara esses instantes de forma coerente; a exibição pode converter para UTC ou preservar o deslocamento original.

```cs
var local = DateTimeOffset.Parse("2026-10-29T10:00:00-03:00");
var utc = local.ToUniversalTime();

Console.WriteLine(local.ToString("O"));
Console.WriteLine(utc.ToString("O"));
```

Se a necessidade for “revisar daqui a uma hora”, use `TimeSpan` para a duração e uma política explícita para definir o instante de início. Se a necessidade for “revisar às 10h no horário local de São Paulo”, o deslocamento isolado pode não ser suficiente para lidar com mudanças históricas de horário; um produto com agenda precisa de `TimeZoneInfo` e uma regra de fuso. Este capítulo só registra um instante fixo de exemplo.

Evite `DateTime.Now` em lógica de domínio sem controle de relógio: testes passam a depender do relógio da máquina e da zona configurada. Nosso construtor recebe o instante pronto, o que torna o contrato simples e o teste determinístico. Uma aplicação que calcula datas em execução pode injetar `TimeProvider` (.NET 8) ou um serviço equivalente para controlar a fonte de tempo.

## Um plano pode ser revisto

`PlanoDeRetomada` é imutável; editar o plano significa criar outro. Isso torna claro que o acordo mudou:

```cs
var inicial = new PlanoDeRetomada(
    "Investigar a falha no CI.",
    "Comparar ambiente local e remoto.",
    "Solicitar uma revisão.",
    DateTimeOffset.Parse("2026-10-29T10:00:00-03:00"));

var atualizado = new PlanoDeRetomada(
    inicial.Tarefa,
    "Fixar a versão do runtime no pipeline.",
    inicial.Apoio,
    DateTimeOffset.Parse("2026-10-29T13:00:00-03:00"));
```

O primeiro plano não desaparece da memória só porque criamos o segundo, mas nenhum deles é gravado automaticamente. Se uma aplicação persistisse versões, isso seria uma decisão separada, exigindo finalidade, acesso, retenção e mecanismo de exclusão. Para os exercícios, a cópia serve para comparar o que mudou no plano técnico.

## Converter texto de entrada com uma falha clara

Uma interface poderia receber um horário em texto. `DateTimeOffset.Parse` é conveniente quando o texto é fixo e conhecido; para uma entrada variável, `TryParse` permite tratar o erro como um resultado esperado:

```cs
using System.Globalization;

const string texto = "2026-10-29T13:00:00-03:00";

if (DateTimeOffset.TryParse(
        texto,
        CultureInfo.InvariantCulture,
        DateTimeStyles.None,
        out var revisarEm))
{
    Console.WriteLine(revisarEm.ToString("O"));
}
else
{
    Console.WriteLine("Use uma data e hora com deslocamento, como 2026-10-29T13:00:00-03:00.");
}
```

`CultureInfo.InvariantCulture` torna previsível a leitura do formato internacional do exemplo, independentemente do idioma da máquina. A interface deveria explicar o formato esperado antes de pedir a entrada e aceitar um cancelamento. Não substitua uma data inválida por `DateTimeOffset.Now`: isso produziria um plano com aparência de válido e esconderia o erro original.

Para uma aplicação de calendário, é melhor escolher um formato de entrada explícito e validar também as regras do negócio, como expediente, finais de semana ou datas passadas. O livro não define essas regras porque elas dependem do grupo e da tarefa.

::: pense Mude a condição
Imagine que a equipe não tem uma segunda pessoa disponível. O que muda no campo `Apoio`? O plano deve manter um apoio fictício só porque a propriedade foi definida?
:::

::: exercicio Planeje uma retomada com contexto
Uma pessoa não conseguiu estudar porque o computador compartilhado ficou indisponível. Proponha um ajuste individual, um apoio possível e uma condição que ainda está fora do controle dela.
---
**Resposta possível:** individual: separar uma pergunta para estudar offline; apoio: pedir acesso ao laboratório; fora do controle imediato: disponibilidade do computador. O plano pode ser revisto sem atribuir culpa à pessoa.
:::

::: exercicio Instante ou duração?
Uma equipe combina rever um teste às 14h de amanhã e reservar 30 minutos para essa tarefa. Que tipo representa o horário e que tipo representa o tempo reservado? O que precisa ser combinado se cada integrante trabalha num fuso diferente?
---
**Resposta possível:** `DateTimeOffset` representa o instante combinado; `TimeSpan.FromMinutes(30)` representa a duração. Para equipes em fusos distintos, é preciso combinar a zona e a forma de exibição, não apenas assumir que “14h” significa a mesma hora local para todo mundo.
:::

::: quiz
? Uma tarefa atrasou. Que explicação é mais cuidadosa?
+ Examinar emoção, instruções, recursos, interrupções e prazo.
- Atribuir o atraso à falta de autocontrole.
- Concluir que a pessoa não se importa.
> Um resultado pode ter várias causas; contexto operacional importa.

? Para que serve `RevisarEm` no exemplo?
+ Marcar quando rever o plano, que pode ser alterado.
- Garantir que o trabalho termine nesse instante.
- Medir o estado emocional.
> Um prazo de revisão é um lembrete, não uma promessa nem avaliação.

? Se a barreira é um recurso indisponível:
+ Planejar apoio ou alternativa sem transformar a barreira em falha pessoal.
- Prescrever que a pessoa se concentre mais.
- Ignorar o contexto e manter o plano original.
> Mudanças individuais e condições compartilhadas devem ser distinguidas.
? Que campos tem `PlanoDeRetomada`?
+ Tarefa, ajuste de contexto, apoio e o instante de revisão
- Emoção, causa e nota
- Nome, idade e humor
- Somente a data
> É o que o construtor exige.

? Por que o construtor recusa `revisarEm == default`?
+ Para exigir que o instante de revisão seja informado de propósito, e não um valor padrão sem significado
- Porque `default` é uma data futura
- Porque o C# proíbe datas
- Para medir a pontualidade da pessoa
> O valor padrão de `DateTimeOffset` não é uma escolha de ninguém.

? O que `DateTimeOffset` guarda que ajuda a comparar instantes entre fusos?
+ A data, a hora e o deslocamento em relação ao UTC
- Só o dia da semana
- O fuso horário da cidade
- A duração da pausa
> Ele identifica um instante; não define duração nem o fuso da equipe.
:::

::: resumo
- Atenção e tarefa são influenciadas por condições individuais e ambientais.
- Uma explicação única pode esconder problemas de processo ou acesso.
- Planos de retomada incluem ajuste, apoio e revisão.
- `DateTimeOffset` identifica instante e deslocamento; não determina a duração da pausa.
- A revisão atualiza o plano; não dá nota a ninguém.
:::

::: cartas
Retomada :: Voltar a uma tarefa com contexto ou apoio ajustado.
Fator contextual :: Condição de ambiente, tarefa ou recurso que participa da experiência.
`DateTimeOffset` :: Tipo .NET que representa uma data e hora com deslocamento UTC.
Instante :: Ponto no tempo; não é duração.
Deslocamento UTC :: Diferença entre a hora local e o tempo universal.
Barreira de processo :: Dificuldade que vem de tarefa, ferramenta ou prazo, e não da pessoa.
Ajuste de contexto :: Mudança no entorno que ajuda a retomar a tarefa.
Plano revisável :: Plano com data de revisão, que pode ser alterado.
:::
