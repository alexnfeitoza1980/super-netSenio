---
id: emocional-cap122
order: 157
kind: capitulo
num: 122
group: C# & Mentalidade · Parte 17 · Emoções e C#
title: Acordos que cuidam do trabalho
subtitle: Uma equipe pode combinar rotinas, limites e apoio sem coletar ou classificar emoções individuais.
minutes: 45
hue: 230
objectives:
- Diferenciar um acordo de processo de uma regra sobre sentimentos
- Registrar opções de apoio sem identificar estados emocionais
- Compor informações pequenas com um `record`
origem: livros/09-inteligencia-emocional-csharp/content/150-cap122.md
---

::: vila Um acordo para revisão
Uma equipe nota que revisões de código longas terminam sem decisão e com pessoas falando por cima umas das outras. Em vez de criar um formulário de “estado emocional” obrigatório, combina blocos menores, uma pausa entre revisões e uma pessoa responsável por registrar perguntas em aberto. Qualquer integrante pode pedir um intervalo sem explicar motivo pessoal.
:::

::: ilustracao img/emocional/cap122.svg | Equipes combinam rotina, opção de pausa, apoio e revisão. O acordo organiza o trabalho e não coleta emoções individuais.
:::

## O processo pertence ao grupo

O contexto de trabalho pode ser alterado: tamanho de uma reunião, distribuição de fala, tempo para responder e canais de ajuda. Uma equipe não precisa guardar emoções individuais para definir essas práticas. Acordos são melhores quando são simples, revisáveis, acessíveis e compatíveis com as responsabilidades reais.

Perguntas para construir um acordo:

- Que parte do fluxo de trabalho queremos tornar mais previsível?
- Que opções de pausa estão disponíveis sem exigir justificativa pessoal?
- Quem pode apoiar quando um acordo não funciona?
- Quando e como vamos revisar a prática?

Não se deve usar um registro de acordo para vigiar quem pediu pausa, atribuir “maturidade” ou comparar pessoas. Se o processo envolve dados pessoais, defina necessidade, consentimento e retenção antes de coletá-los; neste livro, o programa não armazena esses dados.

::: figura Um acordo é do grupo e não coleta emoções
<svg viewBox="0 0 720 188" role="img" aria-label="Rotina, opção de pausa, apoio disponível e revisão compõem o acordo de equipe, sem dados sobre sentimentos individuais">
<rect x="10" y="40" width="150" height="70" rx="12" class="f-info"/><text x="85.0" y="72.0" text-anchor="middle" font-weight="800" font-size="13">Rotina</text><text x="85.0" y="89.0" text-anchor="middle" font-size="11" class="t-dim">como trabalhamos</text><rect x="176" y="40" width="160" height="70" rx="12" class="f-warn"/><text x="256.0" y="72.0" text-anchor="middle" font-weight="800" font-size="13">OpcaoDePausa</text><text x="256.0" y="89.0" text-anchor="middle" font-size="11" class="t-dim">pode pedir intervalo</text><rect x="352" y="40" width="160" height="70" rx="12" class="f-good"/><text x="432.0" y="72.0" text-anchor="middle" font-weight="800" font-size="13">ApoioDisponivel</text><text x="432.0" y="89.0" text-anchor="middle" font-size="11" class="t-dim">a quem recorrer</text><rect x="528" y="40" width="182" height="70" rx="12" class="f-prim"/><text x="619.0" y="72.0" text-anchor="middle" font-weight="800" font-size="13">Revisao</text><text x="619.0" y="89.0" text-anchor="middle" font-size="11" class="t-dim">quando reavaliar</text><path d="M85 112 V138 H620 V112" fill="none" class="arrow"/><text x="360" y="130" text-anchor="middle" font-weight="800" font-size="14" fill="currentColor">AcordoDeEquipe</text><text x="360" y="168" text-anchor="middle" font-size="13" class="t-dim">Nenhum campo guarda sentimento de uma pessoa: organizar o trabalho não exige isso.</text>
</svg>
:::

## Compor sem criar um perfil

```cs Jornada/Emocoes/AcordoDeEquipe.cs
namespace Jornada.Emocoes;

public sealed record AcordoDeEquipe
{
    public string Rotina { get; }
    public string OpcaoDePausa { get; }
    public string ApoioDisponivel { get; }
    public string Revisao { get; }

    public AcordoDeEquipe(
        string rotina,
        string opcaoDePausa,
        string apoioDisponivel,
        string revisao)
    {
        Rotina = ExigirTexto(rotina, nameof(rotina));
        OpcaoDePausa = ExigirTexto(opcaoDePausa, nameof(opcaoDePausa));
        ApoioDisponivel = ExigirTexto(apoioDisponivel, nameof(apoioDisponivel));
        Revisao = ExigirTexto(revisao, nameof(revisao));
    }

    private static string ExigirTexto(string valor, string parametro)
    {
        if (string.IsNullOrWhiteSpace(valor))
            throw new ArgumentException("Descreva o acordo ou use um exemplo fictício.", parametro);

        return valor.Trim();
    }
}
```

Exemplo de acordo de processo, sem nomes ou estados emocionais:

```cs
var acordo = new AcordoDeEquipe(
    rotina: "Revisões de até 25 minutos, com perguntas em aberto registradas.",
    opcaoDePausa: "Qualquer pessoa pode pedir intervalo e combinar retomada.",
    apoioDisponivel: "A pessoa facilitadora ajuda a reorganizar a conversa.",
    revisao: "Revisar o formato em duas semanas.");
```

O objeto descreve como o grupo trabalha; não cria um perfil emocional dos integrantes.

## Um acordo precisa caber no fluxo real

“Vamos nos comunicar melhor” expressa uma intenção, mas não explica o que fazer quando duas revisões se sobrepõem. Um acordo operacional deixa claro qual rotina muda, que alternativa está disponível e como verificar se ela ainda funciona:

| Campo do acordo | Exemplo de processo |
|---|---|
| `Rotina` | Encerrar a revisão com decisões e perguntas pendentes registradas |
| `OpcaoDePausa` | Qualquer pessoa pode solicitar intervalo e sugerir uma retomada |
| `ApoioDisponivel` | A pessoa facilitadora ajuda a distribuir a conversa |
| `Revisao` | Rever o formato na retrospectiva seguinte |

Escrever o acordo como dado ajuda a torná-lo explícito. O construtor valida que as descrições não estejam vazias, mas não pode determinar se uma rotina é inclusiva, viável ou justa. Isso exige conversa e revisão com quem será afetado.

## De uma regra geral para algo negociável

Compare duas propostas:

- **Regra rígida:** “Toda pessoa precisa dizer como está emocionalmente antes da reunião.”
- **Acordo de processo:** “Antes de encerrar, vamos confirmar responsáveis e perguntas pendentes; qualquer pessoa pode pedir uma pausa sem explicar o motivo.”

A primeira recolhe informação íntima e torna a participação condicional à exposição. A segunda modifica uma rotina compartilhada e oferece uma opção de pausa. Ainda assim, a equipe deve confirmar se a pausa é realmente aceita na prática, se há tempo para ela e como atividades urgentes serão tratadas.

Um acordo pode prever mais de uma alternativa sem rastrear quem a usou. Por exemplo, a pessoa facilitadora anuncia uma pausa de cinco minutos entre tópicos; quem não puder participar pode pedir um resumo escrito; perguntas técnicas podem ser estacionadas numa lista. O foco está no desenho do trabalho, não em associar comportamento a um estado emocional.

## Rever dados e responsabilidades

`AcordoDeEquipe` é um `record` imutável com propriedades que podem ser comparadas em testes. O construtor elimina espaços externos e rejeita campos ausentes, mas não corrige frases nem impõe a mesma regra para todas as equipes:

```cs
var acordo = new AcordoDeEquipe(
    "Fechar cada revisão listando decisões e pendências.",
    "Qualquer pessoa pode pedir intervalo e combinar a retomada.",
    "A pessoa facilitadora ajuda a redistribuir o tempo de fala.",
    "Revisar o formato em duas semanas.");

Console.WriteLine(acordo.Revisao);
```

Se a equipe decidir que uma revisão é opcional, o modelo poderia representar essa escolha explicitamente em vez de exigir uma string vazia. Se for necessário registrar a data, o tipo de dado deveria ser escolhido com cuidado; uma descrição livre não oferece a mesma validação de uma data tipada. Não introduzimos uma data automática porque o acordo fictício ainda não precisa dela.

Em uma aplicação real, a pergunta “quem tem acesso ao acordo?” também importa. A rotina pode ser pública para o time, mas comentários de reflexão individuais não devem ser acoplados a esse registro. Neste projeto não existe persistência nem conta de usuário; não há local para guardar uma avaliação pessoal.

## Imutabilidade favorece revisão explícita

O acordo é um `record` com propriedades somente de leitura. Se a rotina mudar, um novo acordo pode ser criado e comparado com o anterior:

```cs
var atual = new AcordoDeEquipe(
    "Revisões de até 25 minutos.",
    "Qualquer pessoa pode solicitar intervalo.",
    "Facilitação ajuda a distribuir a conversa.",
    "Rever no próximo encontro.");

var proximo = new AcordoDeEquipe(
    "Revisões de até 20 minutos, com pendências registradas.",
    "Qualquer pessoa pode solicitar intervalo.",
    "Facilitação ajuda a distribuir a conversa.",
    "Rever após duas semanas.");

Console.WriteLine(atual == proximo); // False: a rotina e a revisão mudaram.
```

A igualdade por valor ajuda num teste ou numa comparação de versões em memória. Não prova que o grupo adotou a nova rotina, que todos concordaram ou que a prática funcionou. O objeto representa a proposta escrita; a decisão é social e precisa ser tomada fora do código.

## Acessibilidade como parte do desenho

Uma pausa só é uma opção se o fluxo permite usá-la sem penalizar quem a solicita. Uma equipe pode oferecer retomada assíncrona, agenda antecipada, pauta escrita ou outra maneira de contribuir. A escolha depende das pessoas e do trabalho; não é apropriado supor que toda pausa seja igualmente acessível.

O modelo simples usa `string` para descrever as opções, porque o vocabulário de acordos varia. Se o software precisasse executar regras específicas — por exemplo, reservar um bloco no calendário — poderia separar a política executável da frase de apresentação e testá-la. Não se deve interpretar um campo de texto como autorização para automação.

Uma revisão de acordo deve buscar informações úteis sobre o processo: “as decisões ficaram mais claras?” é diferente de “quem demonstrou menos controle?”. A primeira pergunta abre caminho para mudar a rotina; a segunda cria uma comparação de pessoas e pode pressionar a equipe a esconder dificuldades.

## Ciclo curto para testar um acordo

Um acordo novo pode ser tratado como uma hipótese sobre o processo, sem aplicar um experimento sobre a vida privada das pessoas:

1. Escolha um problema de fluxo específico, como encerrar reuniões sem responsáveis.
2. Combine uma mudança pequena, como ler as pendências antes de terminar.
3. Defina uma forma leve de observar o processo: a ata lista responsável e próximo passo?
4. Combine quando rever a rotina e quem facilitará a conversa.
5. Mantenha, ajuste ou retire o acordo conforme a equipe decidir.

Não é necessário registrar emoções, nomes de quem pediu pausa ou quem falou mais. A observação pode se limitar ao artefato de trabalho — por exemplo, se a lista de pendências está preenchida. Também pode haver motivos legítimos para alguém não participar da reunião; a equipe não deve transformar uma métrica de presença em medida de comprometimento.

O campo `Revisao` de `AcordoDeEquipe` no exemplo é uma descrição curta (“rever o formato na retrospectiva seguinte”), não uma data confiável nem um mecanismo de lembrete. Se o código precisasse disparar uma notificação, essa seria outra funcionalidade: calendário, fuso, permissão e dados de destinatários teriam de ser especificados e testados.

## Testar uma mudança do processo

Como o acordo é um `record`, valores iguais podem ser comparados com igualdade por valor. Um teste simples pode assegurar que os campos são preservados e textos em branco são rejeitados:

```cs
var proposta = new AcordoDeEquipe(
    "Listar pendências antes de encerrar.",
    "Qualquer pessoa pode solicitar intervalo.",
    "A pessoa facilitadora registra as decisões.",
    "Rever na próxima retrospectiva.");

Assert.Equal("Listar pendências antes de encerrar.", proposta.Rotina);
Assert.Throws<ArgumentException>(() =>
    new AcordoDeEquipe("", "Pausa", "Apoio", "Revisão"));
```

O teste verifica integridade do objeto, não consenso entre integrantes. A equipe precisa concordar fora do código e ter espaço para apontar que a proposta não atende a todos.

::: pense O acordo protege a escolha?
Uma equipe criou uma pausa que só pode ser solicitada depois que a pessoa explica em voz alta o motivo. Que condição de privacidade o acordo impõe? Como mudar o processo para que a pausa esteja acessível sem uma justificativa íntima?
:::

::: exercicio Substitua uma regra invasiva
Uma equipe quer exigir que todos registrem diariamente “quanto de ansiedade sentiram”. Proponha um acordo de processo que ajude o trabalho sem coletar relatos emocionais individuais.
---
**Resposta possível:** revisar a carga de trabalho semanalmente, oferecer uma opção de pausa e explicitar a quem pedir ajuda se a tarefa estiver bloqueada. Perguntar sobre obstáculos do processo não obriga a revelar sentimentos.
:::

::: exercicio Faça um acordo verificável sem vigiar pessoas
Escreva uma rotina para encerrar reuniões de revisão que termine sem responsáveis ou pendências definidos. Inclua uma opção de pausa, um apoio possível e uma data ou evento para rever o acordo.
---
**Resposta possível:** rotina: “nos últimos cinco minutos, ler decisões e pendências”; pausa: “qualquer participante pode pedir intervalo e combinar retomada”; apoio: “a pessoa facilitadora atualiza a lista”; revisão: “avaliar o formato na retrospectiva seguinte”. Nenhuma parte requer registrar emoções ou quem pediu pausa.
:::

::: quiz
? Qual exemplo é um acordo de processo?
+ “Qualquer pessoa pode pedir uma pausa e combinar a retomada.”
- “Todos devem informar seu humor antes da reunião.”
- “Quem parecer frustrado não pode revisar código.”
> O acordo saudável organiza o trabalho, não controla experiências pessoais.

? O `AcordoDeEquipe` do exemplo guarda:
+ Rotina e formas de apoio definidas pelo grupo.
- Medidas individuais de inteligência emocional.
- Diagnósticos feitos por observação.
> Os campos descrevem práticas, não pessoas.

? Como saber se o acordo está funcionando?
+ Rever o fluxo e convidar a equipe a sugerir ajustes, sem pontuar indivíduos.
- Comparar quem pede mais pausas.
- Inferir satisfação por expressões faciais.
> A revisão deve olhar para o processo e proteger a participação voluntária.
? Quais campos tem `AcordoDeEquipe`?
+ Rotina, opção de pausa, apoio disponível e revisão
- Humor, desempenho e nota
- Emoções de cada pessoa
- Apenas a data
> São os quatro textos do construtor.

? Por que o acordo não guarda emoções individuais?
+ Porque organizar o trabalho não exige coletar sentimentos, e isso evitaria vigilância e perfis
- Porque o C# não permite
- Porque emoções são secretas por lei
- Para o código ficar menor
> O acordo descreve o processo do grupo.

? Por que tratar o acordo como um `record` que se revê?
+ Mudar o acordo é criar uma nova versão combinada com o grupo, deixando a revisão explícita
- Porque `record` é mais rápido
- Para impedir que a equipe discuta
- Porque acordos nunca mudam
> Imutabilidade favorece revisão explícita.
:::

::: resumo
- Equipes podem ajustar processos sem coletar emoções individuais.
- Um acordo claro inclui rotina, opção de pausa, apoio e revisão.
- Acordos precisam ser viáveis, negociados e revistos com as pessoas afetadas.
- Dados sobre sentimento não são necessários para organizar trabalho respeitoso.
- Acordos devem ser revisáveis e não usados para comparar pessoas.
:::

::: cartas
Acordo de processo :: Combinado que organiza como a equipe trabalha e se apoia.
Privacidade :: Limite sobre coleta, uso e exposição de informações pessoais.
Revisão do acordo :: Conversa sobre o processo para decidir se ele precisa mudar.
Composição :: Montar um tipo maior a partir de campos menores.
Perfil (risco) :: Conjunto de dados que descreve uma pessoa e pode ser usado para comparar ou vigiar.
Acessibilidade :: Desenhar o processo para que mais pessoas possam participar.
Rotina :: Combinado regular de como o grupo trabalha.
Ciclo curto de teste :: Experimentar um acordo por pouco tempo e rever.
:::
