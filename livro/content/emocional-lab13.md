---
id: emocional-lab13
order: 159
kind: lab
num: 13
group: C# & Mentalidade · Parte 17 · Emoções e C#
title: Laboratório: Roteiro de Pausa e Conversa
subtitle: Um programa de console com dados fictícios que organiza uma observação, uma opção escolhida e um pedido claro — sem inferir sentimentos nem salvar relatos.
minutes: 90
hue: 170
objectives:
- Integrar modelos construídos nos capítulos 116 a 123
- Executar uma demonstração de console com dados escritos no código
- Testar validação, escolhas explícitas e ausência de classificação automática
origem: livros/09-inteligencia-emocional-csharp/content/152-lab13.md
---

::: ilustracao img/emocional/lab13.svg | O Roteiro de Pausa e Conversa reúne os modelos em um console fictício: nada é salvo, enviado ou avaliado.
:::

## O que vamos construir

O laboratório combina registros declarados, uma opção de pausa e um pedido de conversa. A demonstração é **fictícia**, está escrita no código e é apresentada somente na tela. O programa não pergunta ao usuário sobre sua vida, não grava arquivos, não envia dados, não identifica emoções por texto ou imagem e não produz notas ou diagnósticos.

Se quiser adaptar o exemplo, use dados neutros inventados. Não inclua informações pessoais, relatos de terceiros ou detalhes que você não gostaria de ver numa tela.

::: figura Do exemplo à tela
<svg viewBox="0 0 720 160" role="img" aria-label="Dados fictícios são compostos em modelos C#, apresentados no console e verificados por testes, sem armazenamento nem inferência de emoções">
  <rect x="10" y="46" width="160" height="62" rx="12" class="f-warn"/><text x="90" y="72" text-anchor="middle" font-weight="800">Exemplo fictício</text><text x="90" y="92" text-anchor="middle" font-size="12" class="t-dim">escrito no código</text>
  <path class="arrow" d="M172 77H204"/>
  <rect x="208" y="46" width="160" height="62" rx="12" class="f-info"/><text x="288" y="72" text-anchor="middle" font-weight="800">Modelos C#</text><text x="288" y="92" text-anchor="middle" font-size="12" class="t-dim">opções declaradas</text>
  <path class="arrow" d="M370 77H402"/>
  <rect x="406" y="46" width="140" height="62" rx="12" class="f-good"/><text x="476" y="72" text-anchor="middle" font-weight="800">Console</text><text x="476" y="92" text-anchor="middle" font-size="12" class="t-dim">somente exibe</text>
  <path class="arrow" d="M548 77H580"/>
  <rect x="584" y="46" width="126" height="62" rx="12" class="f-prim"/><text x="647" y="72" text-anchor="middle" font-weight="800">Testes</text><text x="647" y="92" text-anchor="middle" font-size="12" class="t-dim">contratos e limites</text>
</svg>
:::

## Passo 1: abrir e executar

Abra `codigo/emocional/Emocoes.sln` no Visual Studio ou use o .NET 8 SDK:

```bash
dotnet test codigo/emocional/Emocoes.sln
dotnet run --project codigo/emocional/Emocoes.Console
```

O primeiro comando executa os testes da biblioteca. O segundo imprime uma situação já preparada no `Program.cs`. O programa não solicita entrada interativa.

::: pense Antes de rodar
Qual parte do programa pode verificar que um campo está vazio? Qual parte não consegue determinar se a descrição corresponde à experiência de alguém?
:::

## Passo 2: entender a solução e as referências

A solução tem três projetos. `Emocoes.Console` depende de `Emocoes.Core`; `Emocoes.Testes` também depende de `Emocoes.Core`. A biblioteca não depende do console, e o teste não precisa executar a interface para verificar as regras. Esse sentido único das referências mantém os tipos reutilizáveis e deixa os testes rápidos.

```text
Emocoes.sln
├── Emocoes.Core       modelos e apresentação do roteiro
├── Emocoes.Console    um cenário fictício para o terminal
└── Emocoes.Testes     verificações xUnit
```

O SDK inclui os arquivos `*.cs` da pasta automaticamente. O arquivo `.csproj` da biblioteca define .NET 8, C# 12, referências globais comuns e tipos anuláveis. `Nullable` está habilitado: `string?` sinaliza que a emoção pode ser omitida em `ObservacaoEmocional`; `string` indica que o campo deve estar presente. O compilador avisa sobre muitos usos inconsistentes, mas tipos anuláveis não provam que um dado é verdadeiro ou correto.

Confira os arquivos em `codigo/emocional/Emocoes.Core/`. Não existe serviço de análise de sentimento, cálculo de pontuação, webcam, cliente HTTP, armazenamento em arquivo ou banco de dados. A ausência desses recursos é parte do escopo, não uma promessa de segurança para um futuro sistema com outros requisitos.

## Passo 3: acompanhar a composição

O console cria dados fictícios e os passa para um roteiro. Estes objetos representam as escolhas de uma cena de equipe; os nomes e frases estão declarados no próprio código:

```cs Jornada/Emocoes.Console/Program.cs
using Jornada.Emocoes;

var registro = new RegistroEmocional(
    EmocaoNomeada.Frustracao,
    "A revisão terminou sem responsáveis definidos.",
    "Exemplo fictício de uma reunião de projeto.",
    DateTimeOffset.Parse("2026-10-29T10:00:00-03:00"));

var observacao = new ObservacaoEmocional(
    "A revisão terminou sem responsáveis definidos.",
    "Talvez as pendências tenham ficado pouco claras.",
    "Frustração");

var pausa = new PlanoDePausa(
    OpcaoDePausa.PedirUmIntervalo,
    "Retomar a revisão depois do almoço.",
    "Se o prazo mudar, avisar a equipe.");

var retomada = new PlanoDeRetomada(
    "Revisar as pendências abertas.",
    "Listar responsáveis antes de encerrar.",
    "Pedir à pessoa facilitadora que registre as decisões.",
    DateTimeOffset.Parse("2026-10-29T13:00:00-03:00"));

var pedido = new PedidoDeConversa(
    "A revisão terminou sem responsáveis definidos.",
    "Ficou incerto quem valida o próximo passo.",
    "Podemos definir responsáveis antes de encerrar?");

var acordo = new AcordoDeEquipe(
    "Encerrar revisões listando decisões e pendências.",
    "Qualquer pessoa pode pedir intervalo e combinar a retomada.",
    "A pessoa facilitadora ajuda a distribuir a conversa.",
    "Rever o formato na retrospectiva seguinte.");

var revisao = new RevisaoDePratica(
    "A reunião terminou sem definir responsáveis.",
    "Pedi para rever as opções.",
    "Combinar responsáveis antes da próxima revisão.",
    "Pedir apoio à pessoa facilitadora.");

var roteiro = new RoteiroDePausaEConversa(
    registro,
    observacao,
    pausa,
    retomada,
    pedido,
    acordo,
    revisao);

foreach (var linha in roteiro.Apresentar())
    Console.WriteLine(linha);
```

O `RoteiroDePausaEConversa` recebe sete objetos: o nome emocional escolhido, a separação de fato e interpretação, a opção de pausa, o contexto de retomada, a conversa, o acordo de equipe e uma reflexão. A reflexão é opcional: o construtor aceita `null` como último argumento. Assim, a integração demonstra os capítulos sem transformar reflexão individual em pré-requisito.

O método `Apresentar()` cria uma lista local e devolve `AsReadOnly()`. O console percorre essa lista e imprime cada linha. Não há método `Save`, `File.WriteAllText`, `HttpClient` ou uma chamada de telemetria nessa sequência. O processo termina e os objetos deixam de existir na memória do programa.

## Passo 4: ler uma saída sem dar nota

A saída começa por avisar que é um exemplo fictício e mostra sinal, contexto, opções, pedido e acordo. Veja a diferença entre apresentar e inferir:

```text
Sinal observado: A revisão terminou sem responsáveis definidos.
Palavra escolhida: Frustração
Opção escolhida: Pedir um intervalo
Próximo passo: Retomar a revisão depois do almoço.
Pedido: Podemos definir responsáveis antes de encerrar?
Mensagem estruturada: Observação: A revisão terminou sem responsáveis definidos. | Impacto: Ficou incerto quem valida o próximo passo. | Pedido: Podemos definir responsáveis antes de encerrar?
```

Essas linhas repetem os valores que estavam no código. Não dizem que toda pessoa numa revisão está frustrada, que a pausa é a opção correta ou que o pedido será aceito. A palavra “Frustracao” aparece sem acento porque esse é o identificador C# do enum; a camada de apresentação poderia mapear identificadores para rótulos localizados, mas esse mapeamento ainda teria de exibir uma escolha explícita.

A classe apresenta também perguntas de escuta, mas não escolhe uma delas como a pergunta “ideal”. A lista é um conjunto de exemplos para consulta. Na próxima versão didática, uma pessoa poderia escolher manualmente se quer incluir uma pergunta, mas não seria necessário armazenar a resposta.

## Passo 5: testar contratos e escolhas

Os testes xUnit separam regras pequenas:

| Teste | Contrato verificado |
|---|---|
| Texto do registro vazio | Rejeitado com `ArgumentException` e nome do parâmetro |
| Número que não existe no enum | Rejeitado com `ArgumentOutOfRangeException` |
| Opção “ainda não sei” | Continua sendo uma opção válida |
| Campo emocional opcional | Pode permanecer `null` sem fabricar uma resposta |
| Pedido vazio | Não forma uma mensagem incompleta |
| Data de revisão ausente | Não cria um plano sem instante explícito |
| Revisão sem apoio informado | O detalhe opcional pode ser omitido |
| Roteiro sem reflexão | Apresenta os demais campos e não inventa reflexão |

Uma regra de entrada deve ser testada na exceção e também no nome do parâmetro quando a API o considera parte do contrato. O teste de apresentação verifica linhas conhecidas e a presença de uma pergunta possível. Ele não depende de comparar cada caractere da saída completa, o que deixaria a manutenção frágil quando uma linha explicativa mudasse.

Rode `dotnet test codigo/emocional/Emocoes.sln`. Se um teste falhar, leia o primeiro erro e o valor esperado antes de mudar a implementação. Um teste vermelho pode indicar uma regressão, mas também pode mostrar que a regra foi alterada e que o teste precisa ser revisto conscientemente.

## Passo 6: experimente uma mudança segura

Troque apenas textos fictícios no `Program.cs`, por exemplo, substituindo “definir responsáveis” por “registrar a dependência do teste”. Rode o console e os testes novamente. Depois escolha uma mudança de modelo:

- remova o argumento de reflexão do construtor do roteiro;
- use `EmocaoNomeada.PrefiroNaoNomear`;
- tente criar um `PedidoDeConversa` com espaço em branco;
- altere o deslocamento do horário e observe a saída ISO 8601;
- tente alterar a lista retornada pelo roteiro e confira que ela é somente leitura.

Cada tentativa explora um contrato C#; nenhuma exige que você conte algo pessoal. Não adicione persistência para “completar” o laboratório: o objetivo desta demonstração é integrar modelos e testar fronteiras, não construir um diário emocional.

## O que o laboratório não pretende resolver

Uma aplicação real que recolha dados de pessoas exigiria análise de finalidade, consentimento, segurança, retenção, acesso e exclusão, além de uma avaliação das consequências de cada decisão de produto. Nada disso é obtido só por usar classes `record`, validar strings ou passar nos testes. Este laboratório evita a coleta desde o começo, porque não precisa dela para ensinar os conceitos de C#.

O projeto também não é uma ferramenta de terapia, recrutamento, triagem, produtividade ou gestão de desempenho. Não use uma lista de emoções para classificar colegas. Se a atividade provocar desconforto, encerre-a; a leitura e o exercício são opcionais.

## Passo 7: testar os limites

Os testes confirmam que campos obrigatórios vazios são rejeitados, que descrições são mantidas e que as mensagens do roteiro não classificam pessoas. Um teste não prova que uma conversa real será respeitosa; ele protege um contrato de software delimitado.

::: exercicio Experimente sem inserir dados pessoais
Mude apenas o exemplo fictício em `Program.cs`. Use `EmocaoNomeada.PrefiroNaoNomear`, retire a reflexão opcional, faça o pedido mais concreto e rode `dotnet run`. Depois rode `dotnet test`. O que o programa apresenta? O que ele continua sem inferir?
---
**Resposta possível:** apresenta os textos fornecidos no código e não mostra a reflexão omitida. Continua sem identificar emoções, avaliar a pessoa, julgar intenções ou guardar o relato. A opção `PrefiroNaoNomear` é exibida como escolha explícita, não como classificação.
:::

::: quiz
? Qual entrada o programa do laboratório lê?
+ Um cenário fictício definido no próprio `Program.cs`.
- Um diário emocional pessoal salvo no computador.
- Imagens e mensagens de uma equipe.
> A demonstração é fixa; não há coleta de relato no console.

? O que os testes podem garantir?
+ Regras explícitas do modelo e do texto apresentado.
- Que a interpretação de uma pessoa é universalmente verdadeira.
- Que a conversa terminará em acordo.
> Testes verificam o comportamento do software, não resultados sociais.

? O roteiro escolhe automaticamente a pausa adequada?
+ Não; apresenta a opção que foi explicitamente criada.
- Sim, usando a emoção declarada como pontuação.
- Sim, avaliando o texto do pedido.
> A decisão pertence a quem escolhe a opção.
? Que comando roda a demonstração do laboratório?
+ `dotnet run --project Emocoes.Console`, na pasta `codigo`
- `dotnet test` apenas
- `python build.py`
- Abrir o `index.html`
> O console mostra um cenário fictício; não lê arquivos nem pede relatos.

? O programa do laboratório grava ou envia dados?
+ Não: tudo fica na memória e o cenário é fictício
- Sim, grava em um arquivo local
- Sim, envia para um servidor
- Só quando há erro
> Privacidade por construção.

? O que é um teste de contrato?
+ Uma verificação automatizada das regras definidas para o código, e não de experiências humanas
- Um teste das emoções de quem usa
- Uma prova de que a conversa vai dar certo
- Uma nota para a equipe
> Os testes cobrem validação e texto apresentado.
:::

::: resumo
- O laboratório combina modelos simples com um exemplo fictício.
- A aplicação exibe informações em memória e não pede relatos pessoais.
- Testes verificam contratos de software, não experiências humanas.
- Nenhum componente infere emoções, pontua ou avalia pessoas.
:::

::: cartas
Modelo explícito :: Tipo que representa apenas dados e escolhas informados no código.
Demonstração fictícia :: Exemplo inventado, separado de um relato pessoal.
Teste de contrato :: Verificação automatizada de regras definidas para o programa.
Composição de modelos :: Reunir tipos pequenos em um roteiro único.
Saída sem nota :: Resultado que descreve, sem pontuar nem classificar.
Mudança segura :: Alteração pequena que os testes conseguem conferir.
Limite do laboratório :: O que a demonstração não pretende resolver.
Dados em memória :: Informação que existe só durante a execução do programa.
:::
