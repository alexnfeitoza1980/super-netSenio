---
id: emocional-cap121
order: 156
kind: capitulo
num: 121
group: C# & Mentalidade · Parte 17 · Emoções e C#
title: Conversas difíceis com respeito
subtitle: Descrever o que aconteceu, explicar o impacto e fazer um pedido concreto pode abrir diálogo sem apagar limites.
minutes: 50
hue: 224
objectives:
- Formular uma mensagem com observação, impacto e pedido
- Separar intenção presumida de consequência observada
- Validar os elementos de uma conversa sem gerar acusações
origem: livros/09-inteligencia-emocional-csharp/content/149-cap121.md
---

::: vila A alteração no código
Após uma alteração urgente, Bruno percebe que a versão do arquivo foi substituída sem aviso. Ele pensa “ninguém respeita meu trabalho”, mas decide conversar em particular. Descreve o que viu, explica que perdeu mudanças locais e pede que futuras substituições sejam anunciadas antes. A conversa não determina a intenção de quem fez a alteração; ela permite corrigir o processo.
:::

::: ilustracao img/emocional/cap121.svg | Descrever o que aconteceu, dizer o impacto e fazer um pedido claro abre diálogo sem acusar nem adivinhar intenção.
:::

## Um pedido claro, sem adivinhar intenção

Uma mensagem pode ser montada com três partes:

1. **Observação:** o que ocorreu, com o máximo de especificidade possível.
2. **Impacto:** o efeito que teve sobre você ou sobre o trabalho.
3. **Pedido:** uma ação concreta, realizável e aberta a conversa.

“Você sempre atrapalha” não descreve uma situação específica. “A alteração substituiu meu arquivo local; perdi vinte minutos refazendo a mudança. Você pode avisar antes de substituir arquivos compartilhados?” mantém o foco no evento, no impacto e no próximo passo.

Essa estrutura não garante acordo. Se houver diferença de poder, risco ou retaliação, prefira um canal seguro e apropriado. Uma conversa direta não é obrigação nem substituto de processo formal.

::: figura De uma acusação para três informações
<svg viewBox="0 0 720 210" role="img" aria-label="Observação, impacto e pedido formam uma mensagem em vez de uma acusação">
<rect x="10" y="50" width="130" height="70" rx="12" class="f-bad"/><text x="75.0" y="82.0" text-anchor="middle" font-weight="800" font-size="13">Acusação</text><text x="75.0" y="99.0" text-anchor="middle" font-size="11" class="t-dim">você nunca avisa</text><path class="arrow" d="M142 85H168"/><rect x="172" y="30" width="170" height="36" rx="12" class="f-info"/><text x="257.0" y="45.0" text-anchor="middle" font-weight="800" font-size="13">Observação</text><rect x="172" y="76" width="170" height="36" rx="12" class="f-warn"/><text x="257.0" y="91.0" text-anchor="middle" font-weight="800" font-size="13">Impacto</text><rect x="172" y="122" width="170" height="36" rx="12" class="f-good"/><text x="257.0" y="137.0" text-anchor="middle" font-weight="800" font-size="13">Pedido concreto</text><path class="arrow" d="M344 48 H384"/><path class="arrow" d="M344 94 H384"/><path class="arrow" d="M344 140 H384"/><rect x="388" y="56" width="322" height="76" rx="12" class="f-prim"/><text x="549.0" y="91.0" text-anchor="middle" font-weight="800" font-size="15">Formatar()</text><text x="549.0" y="108.0" text-anchor="middle" font-size="11" class="t-dim">Observação: … | Impacto: … | Pedido: …</text><text x="360" y="190" text-anchor="middle" font-size="13" class="t-dim">O pedido pode ser aceito, negociado ou recusado.</text>
</svg>
:::

## Um modelo explícito de pedido

```cs Jornada/Emocoes/PedidoDeConversa.cs
namespace Jornada.Emocoes;

public sealed record PedidoDeConversa
{
    public string Observacao { get; }
    public string Impacto { get; }
    public string Pedido { get; }
    public string Formatar() =>
        $"Observação: {Observacao} | Impacto: {Impacto} | Pedido: {Pedido}";

    public PedidoDeConversa(string observacao, string impacto, string pedido)
    {
        Observacao = ExigirTexto(observacao, nameof(observacao));
        Impacto = ExigirTexto(impacto, nameof(impacto));
        Pedido = ExigirTexto(pedido, nameof(pedido));
    }

    private static string ExigirTexto(string valor, string parametro)
    {
        if (string.IsNullOrWhiteSpace(valor))
            throw new ArgumentException("Descreva o item ou use um exemplo fictício.", parametro);

        return valor.Trim();
    }
}
```

O tipo exige conteúdo para cada parte, mas não consegue confirmar que a observação é neutra, que o impacto é compartilhado ou que o pedido será aceito. A conversa continua humana. Mais adiante vamos observar como `Formatar()` combina esses campos sem enviar a mensagem.

## Montar a mensagem sem misturar as partes

O método `Formatar()` junta as três propriedades para apresentar um rascunho:

```cs
public string Formatar() =>
    $"Observação: {Observacao} | Impacto: {Impacto} | Pedido: {Pedido}";
```

Para a situação da cena, a saída fica parecida com:

```text
Observação: A alteração substituiu meu arquivo local. | Impacto: Perdi vinte minutos refazendo a mudança. | Pedido: Você pode avisar antes de substituir arquivos compartilhados?
```

O método não escolhe os textos nem os envia a ninguém; apenas concatena. Uma aplicação real poderia preferir rótulos mais neutros (“observação”, “impacto que relatei”, “pedido”) para não parecer que a estrutura é uma regra universal. Também precisaria formatar pontuação e nomes conforme o idioma e o público.

Interpolação de strings é conveniente porque a ordem das variáveis aparece junto com o texto. Se o produto usasse recursos de localização, não seria correto espalhar frases fixas pelos métodos: poderíamos injetar um recurso de tradução ou devolver campos estruturados para uma camada de apresentação. O exemplo é deliberadamente pequeno, não uma arquitetura de i18n completa.

## De uma acusação para três informações

Veja a transformação passo a passo:

| Rascunho inicial | Pergunta de revisão | Versão revisada |
|---|---|---|
| “Você sempre bagunça meus arquivos.” | Qual evento específico ocorreu? | “A alteração substituiu meu arquivo local.” |
| “Você não respeita ninguém.” | Que efeito observável houve? | “Perdi vinte minutos refazendo a mudança.” |
| “Nunca mais faça isso.” | Qual ação concreta ajudaria no futuro? | “Você pode avisar antes de substituir arquivos compartilhados?” |

A versão revisada não é automaticamente “gentil” ou perfeita. Talvez o tempo estimado esteja errado, talvez a alteração fosse parte de uma rotina acordada, ou talvez o pedido precise ser negociado com a equipe. Seu valor é tornar o assunto mais conversável, não garantir que ninguém se incomode.

Um bom pedido pode ser recusado ou precisar de alternativa. “Você consegue avisar antes de substituir arquivos? Se isso não couber no fluxo atual, podemos escolher outro sinal para proteger as mudanças locais?” mantém a necessidade e abre espaço para uma solução técnica.

## Contexto, poder e canal

Considere a relação entre as pessoas e o risco de retaliação antes de enviar uma mensagem. Uma conversa em particular pode proteger a privacidade, mas talvez não seja segura ou suficiente em uma relação de poder desigual. Um processo formal, um mediador ou uma liderança de confiança pode ser mais adequado. O modelo C# não pode decidir isso a partir de três strings.

Antes de usar um rascunho, reveja:

- Ele descreve um evento verificável, sem generalizações como “sempre” e “nunca”?
- O impacto é algo que posso relatar em primeira pessoa?
- O pedido diz o que eu gostaria que acontecesse e deixa espaço para resposta?
- É seguro falar diretamente com essa pessoa?
- Preciso de apoio ou de um canal institucional?

Se alguma resposta indicar risco, não é preciso enviar o rascunho. Um objeto válido no programa não obriga a uma conversa real.

## Escrever uma mensagem que pode ser revista

O método `Formatar()` é uma projeção de dados para texto. Isso é útil para visualizar a estrutura, mas o rascunho gerado ainda precisa ser lido e revisado:

```cs
var rascunho = new PedidoDeConversa(
    "a etapa de deploy começou antes da confirmação do teste",
    "não ficou claro se o risco foi aceito",
    "podemos confirmar o resultado do teste antes do próximo deploy?");

Console.WriteLine(rascunho.Formatar());
```

O método separa os trechos com rótulos e barras verticais, sem tentar encaixar qualquer texto numa frase gramatical. Uma saída formatada não é automaticamente pronta para enviar. A pessoa pode perceber que o pedido está amplo, que a observação mistura hipótese com fato ou que o canal não é seguro. Por isso, o nome “rascunho” é mais adequado do que “resposta ideal”.

Uma versão de produto poderia devolver as três propriedades para um formulário em vez de concatenar uma única linha. Poderia ainda aceitar um modelo de mensagem apropriado ao contexto, com recursos localizados. O exemplo de `Formatar()` prioriza demonstrar interpolação e preservar as três partes; não é uma norma linguística para toda conversa.

## Um teste de resultado previsível

Como o formatador é uma função determinística que recebe três valores, um teste unitário pode confirmar exatamente como os combina:

```cs
var pedido = new PedidoDeConversa("o prazo mudou", "perdi tempo de revisão", "podemos rever a estimativa?");

Assert.Equal(
    "Observação: o prazo mudou | Impacto: perdi tempo de revisão | Pedido: podemos rever a estimativa?",
    pedido.Formatar());
```

Esse é um caso em que assertar a frase completa faz sentido: o texto é o comportamento oferecido. Se o produto decidir mudar pontuação e rótulos, a alteração de teste registra uma revisão deliberada. Em contrapartida, testes de uma saída extensa da aplicação podem selecionar algumas linhas relevantes para não ficarem acoplados à ordem de todos os detalhes.

Uma mensagem pode conter caracteres especiais ou dados confidenciais. Num aplicativo com entrada de pessoas, seria necessário evitar que o formatador enviasse o texto a logs ou a serviços externos sem finalidade e consentimento adequados. Aqui as entradas são constantes fictícias. O formato em si não garante privacidade; a ausência de coleta e transporte no laboratório é o que mantém o escopo reduzido.

## Pedido não é exigência disfarçada

Um pedido descreve uma ação que a outra pessoa pode negociar, esclarecer ou recusar. “Você pode avisar antes de substituir o arquivo?” pergunta por uma prática. “Se você se importasse comigo, nunca faria isso” associa a aceitação do pedido a uma avaliação moral. A diferença importa porque conversas respeitosas preservam a capacidade de responder.

Há situações em que a equipe precisa de uma regra operacional, como proteger alterações locais antes de substituir arquivos compartilhados. Nesse caso, não é apenas um pedido interpessoal: é um requisito técnico. Vale registrá-lo em política de repositório, automação ou processo de revisão. Uma conversa pode revelar a necessidade; um acordo de equipe ou uma mudança no sistema talvez seja a solução duradoura.

Pergunte qual nível de resposta resolve o problema:

- Um pedido pessoal precisa de uma conversa e de uma resposta?
- Uma instrução de trabalho precisa ficar documentada para todas as pessoas?
- Um erro de ferramenta deveria ser impedido por validação automática?
- Existe uma relação de poder que torna a negociação insegura?

O livro usa o modelo de mensagem para praticar a comunicação, não para substituir um processo técnico ou institucional. Um campo de texto e um teste unitário não resolvem uma política mal definida.

::: pense Revise a forma, não julgue a pessoa
Na frase “você ignorou meu pedido porque só pensa em si”, sublinhe o evento que pode ser observado, circule a interpretação sobre intenção e escreva um pedido que ainda faria sentido se a interpretação estivesse errada.
:::

::: exercicio Faça um pedido que possa ser respondido
Reescreva “você nunca me escuta” para uma situação de trabalho, incluindo observação, impacto e um pedido específico. Use uma cena fictícia.
---
**Resposta possível:** observação: “Nas duas últimas revisões, meu comentário foi encerrado antes de eu explicar o teste”; impacto: “não consegui registrar o risco de regressão”; pedido: “na próxima, podemos reservar cinco minutos para eu mostrar o caso?” A outra pessoa pode explicar seu ponto e negociar.
:::

::: exercicio Leve um problema para o nível certo
O time perde alterações locais quando um arquivo compartilhado é substituído. Escreva uma mensagem de observação, impacto e pedido e sugira uma mudança de ferramenta que também possa reduzir o risco. Não presuma por que alguém fez a substituição.
---
**Resposta possível:** observação: “O arquivo local foi substituído durante a sincronização”; impacto: “precisei refazer as mudanças”; pedido: “podemos avisar antes de substituir?” Mudança de ferramenta: configurar controle de versão ou aviso de conflito antes de sobrescrever. A solução técnica reduz recorrência sem afirmar intenção.
:::

::: quiz
? Qual frase é um pedido mais concreto?
+ “Você pode me avisar antes de substituir o arquivo compartilhado?”
- “Seja uma pessoa melhor.”
- “Pare de não se importar.”
> Um pedido útil descreve uma ação observável que pode ser discutida.

? A classe `PedidoDeConversa` valida:
+ Que os campos não estejam vazios.
- Que a observação é imparcial e universalmente verdadeira.
- Que a pessoa destinatária aceitará o pedido.
> Regras da classe não certificam interpretações nem resultados sociais.

? É sempre obrigatório ter uma conversa direta?
+ Não; segurança, poder e canais apropriados importam.
- Sim, mesmo em caso de risco.
- Sim, se o pedido estiver bem escrito.
> Escolher outro canal ou pedir apoio pode ser mais adequado.
? Quais são as três partes de `PedidoDeConversa`?
+ Observação, impacto e pedido
- Sentimento, culpa e castigo
- Data, hora e local
- Nome, cargo e nota
> Cada parte é um campo obrigatório do record.

? O que `Formatar()` faz?
+ Junta os três campos em uma linha de texto; não envia a mensagem nem avalia o pedido
- Envia a mensagem à outra pessoa
- Decide se o pedido é justo
- Garante que haverá acordo
> O código organiza texto informado por alguém.

? Qual versão troca uma acusação por uma observação?
+ "No último ciclo, três mudanças entraram sem revisão."
- "Você nunca revisa nada."
- "Você faz isso de propósito."
- "Todo mundo sabe que você não liga."
> Observação específica diminui o espaço para acusações amplas.
:::

::: resumo
- Observação, impacto e pedido são camadas diferentes de uma mensagem.
- Especificidade diminui acusações amplas, sem prometer concordância.
- `Formatar()` combina texto informado; não envia nem valida o mérito do pedido.
- Código pode validar campos, não a imparcialidade nem o sucesso da conversa.
- Segurança e canais de apoio fazem parte da escolha.
:::

::: cartas
Observação :: Descrição específica de algo que ocorreu.
Impacto :: Efeito percebido na pessoa ou no trabalho.
Pedido concreto :: Ação clara que pode ser aceita, negociada ou recusada.
Acusação ampla :: Frase com "sempre" ou "nunca" que atribui intenção sem descrever um fato.
Canal :: Meio escolhido para conversar (pessoalmente, mensagem, reunião).
Poder na conversa :: Diferença de posição que pode tornar uma conversa direta insegura.
Pedido x exigência :: O pedido pode ser aceito, negociado ou recusado.
Mensagem revisável :: Texto que pode ser relido e ajustado antes de ser usado.
:::
