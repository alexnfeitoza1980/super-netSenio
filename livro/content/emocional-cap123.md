---
id: emocional-cap123
order: 158
kind: capitulo
num: 123
group: C# & Mentalidade · Parte 17 · Emoções e C#
title: Praticar, rever e pedir apoio
subtitle: Uma prática pequena pode ser revisada com curiosidade; o objetivo não é obter uma nota sobre si mesmo.
minutes: 45
hue: 236
objectives:
- Criar uma pergunta de revisão que não classifique pessoas
- Distinguir aprendizado de pontuação ou promessa de resultado
- Consolidar observação, escolha, limite e apoio num registro opcional
origem: livros/09-inteligencia-emocional-csharp/content/151-cap123.md
---

::: vila O que funcionou desta vez?
Depois de uma conversa difícil, Lucas relembra que conseguiu pedir um intervalo antes de responder. A discussão continuou, e nem todos concordaram. Em vez de concluir “fui emocionalmente inteligente” ou “fracassei”, ele anota para um exercício fictício: percebeu a pressa, pediu alguns minutos e gostaria de combinar antes quem registrará as decisões. A revisão encontra uma próxima tentativa sem transformar a conversa numa nota.
:::

::: ilustracao img/emocional/cap123.svg | Rever uma prática com curiosidade: o que notei, o que escolhi e o que tentar na próxima vez. Sem nota, sem placar.
:::

## Revisão sem placar

Aprender envolve tentar, observar o que aconteceu e rever uma escolha. Não há uma pontuação única para empatia ou autorregulação. Também não se pode garantir que uma frase ou pausa produzirá certo resultado: a resposta depende de outras pessoas e das condições da situação.

Uma revisão opcional pode perguntar:

- O que eu notei, sem explicar além do que sei?
- Que opção escolhi e o que tornou essa opção possível?
- Que limite ou apoio esteve disponível?
- O que eu gostaria de experimentar numa próxima situação?
- Há algo que precisa de um canal de suporte mais apropriado?

Pular a atividade, manter o registro apenas para si ou conversar com alguém são opções. Não é necessário produzir um relato pessoal para aprender C#.

::: figura Revisar sem placar
<svg viewBox="0 0 720 196" role="img" aria-label="O que notei, a escolha feita e a próxima tentativa formam a revisão; o apoio ou limite é opcional e não há nota">
<rect x="10" y="40" width="160" height="70" rx="12" class="f-info"/><text x="90.0" y="72.0" text-anchor="middle" font-weight="800" font-size="13">OQueNotei</text><text x="90.0" y="89.0" text-anchor="middle" font-size="11" class="t-dim">um sinal ou ação</text><path class="arrow" d="M172 75H190"/><rect x="194" y="40" width="160" height="70" rx="12" class="f-warn"/><text x="274.0" y="72.0" text-anchor="middle" font-weight="800" font-size="13">EscolhaFeita</text><text x="274.0" y="89.0" text-anchor="middle" font-size="11" class="t-dim">o que fiz</text><path class="arrow" d="M356 75H374"/><rect x="378" y="40" width="160" height="70" rx="12" class="f-good"/><text x="458.0" y="72.0" text-anchor="middle" font-weight="800" font-size="13">ProximaTentativa</text><text x="458.0" y="89.0" text-anchor="middle" font-size="11" class="t-dim">algo pequeno</text><rect x="562" y="40" width="148" height="70" rx="12" class="f-prim" opacity=".55" stroke-dasharray="6 5" stroke="currentColor" fill-opacity=".25"/><text x="636" y="72" text-anchor="middle" font-weight="800" font-size="13">ApoioOuLimite?</text><text x="636" y="92" text-anchor="middle" font-size="11" class="t-dim">opcional (string?)</text><text x="360" y="150" text-anchor="middle" font-size="26" fill="#d8454f" font-weight="900">nota 10 ✕</text><text x="360" y="176" text-anchor="middle" font-size="13" class="t-dim">O modelo não tem nota nem ranking. Registrar a revisão também é opcional.</text>
</svg>
:::

## Uma reflexão, não uma nota

```cs Jornada/Emocoes/RevisaoDePratica.cs
namespace Jornada.Emocoes;

public sealed record RevisaoDePratica
{
    public string OQueNotei { get; }
    public string EscolhaFeita { get; }
    public string ProximaTentativa { get; }
    public string? ApoioOuLimite { get; }

    public RevisaoDePratica(
        string oQueNotei,
        string escolhaFeita,
        string proximaTentativa,
        string? apoioOuLimite = null)
    {
        OQueNotei = ExigirTexto(oQueNotei, nameof(oQueNotei));
        EscolhaFeita = ExigirTexto(escolhaFeita, nameof(escolhaFeita));
        ProximaTentativa = ExigirTexto(proximaTentativa, nameof(proximaTentativa));
        ApoioOuLimite = apoioOuLimite is null
            ? null
            : ExigirTexto(apoioOuLimite, nameof(apoioOuLimite));
    }

    private static string ExigirTexto(string valor, string parametro)
    {
        if (string.IsNullOrWhiteSpace(valor))
            throw new ArgumentException("Informe um campo ou escolha não registrar uma revisão.", parametro);

        return valor.Trim();
    }
}
```

Cada campo é um convite para pensar; nenhum deles alimenta um total, ranking ou conclusão automática. No laboratório, usaremos apenas um exemplo criado para a demonstração.

## Observação não é nota

Uma reflexão após uma conversa pode registrar algo que a pessoa fez, mas é diferente de avaliar sua personalidade. Compare:

```text
Nota: 4/10 em empatia.
Observação: interrompi a explicação antes de ela terminar.
Próxima tentativa: esperar uma pausa e perguntar se a pessoa concluiu.
```

A nota sugere que existe uma escala universal e que um observador pode transformar a interação em medida de valor. A observação identifica uma ação específica que pode ser retomada. Mesmo ela não explica todas as condições: a reunião pode ter sido interrompida por uma emergência, ou alguém pode ter pedido uma resposta rápida. Uma reflexão útil permite acrescentar contexto e revisar a hipótese.

Por isso, o modelo `RevisaoDePratica` não inclui `Nota`, `Classificacao`, `Nivel` ou `Pontuacao`. Uma propriedade inexistente não será calculada escondida pelo console. O método correto é testar o comportamento que de fato importa e evitar projetar medidas sem necessidade.

## Tornar uma tentativa pequena

“Nunca mais vou interromper ninguém” é uma promessa absoluta, difícil de observar e fácil de transformar em vergonha quando falha. Uma tentativa pequena descreve um comportamento verificável e pode ser revista:

| Promessa ampla | Tentativa específica |
|---|---|
| “Vou ser mais calmo.” | “Na próxima revisão, vou anotar a dúvida antes de responder.” |
| “Vou ouvir melhor.” | “Vou deixar a pessoa concluir e depois perguntar se entendi.” |
| “Não vou precisar de ajuda.” | “Se o prazo não couber, vou pedir apoio cedo.” |

A tentativa não determina o resultado. A outra pessoa pode não querer conversar, o prazo pode mudar e um apoio pode não estar disponível. O processo de revisão considera essas condições, em vez de atribuir todo o resultado a uma qualidade pessoal.

## Decidir o que registrar

O exercício do livro usa dados fictícios e o console apresenta apenas o exemplo escrito no código. Em outro cenário, a decisão de registrar uma reflexão exigiria perguntas adicionais:

- Qual é a finalidade do registro e ela pode ser alcançada sem armazenar dados?
- Quem poderá ler, editar ou apagar esse conteúdo?
- Por quanto tempo ele permanecerá disponível?
- A pessoa pode participar sem informar detalhes privados?
- Há uma alternativa offline ou a atividade pode ser pulada?

Se não existir necessidade clara e salvaguardas adequadas, não colete. Aqui a resposta é simples: a solução não grava arquivos, não envia rede e não pede entrada pessoal. O objeto existe só durante a execução de uma demonstração criada pelo desenvolvedor.

Na classe do capítulo, os três primeiros campos da reflexão são necessários quando alguém decide construir esse registro, mas `ApoioOuLimite` é opcional:

```cs
var revisao = new RevisaoDePratica(
    "A reunião terminou sem definir responsáveis.",
    "Pedi para rever as opções.",
    "Combinar responsáveis antes da próxima revisão.");
```

O quarto argumento pode ser omitido porque talvez nenhum apoio tenha sido identificado ou porque a pessoa simplesmente não queira descrevê-lo. A reflexão inteira também pode ser omitida ao montar o laboratório: `RoteiroDePausaEConversa` aceita `null` para esse componente. Isso é diferente de registrar quatro strings vazias, que pareceriam um registro preenchido, mas sem conteúdo.

`string?` representa ausência no tipo; não implementa consentimento por si só. Um formulário ainda precisaria dizer claramente para que serve o campo, permitir não responder e não impedir a pessoa de terminar uma atividade opcional. O modelo C# ajuda a explicitar estados possíveis, mas não desenha a experiência completa.

## Não construir um placar por acidente

Uma sequência de reflexões pode tentar virar estatística: contar quantas vezes alguém pediu pausa, somar palavras emocionais ou classificar “avanço”. Não há base neste projeto para fazer isso, e os dados seriam sensíveis ao contexto. A mesma quantidade de pausas pode representar necessidades, tarefas e ambientes totalmente diferentes; uma contagem não revela por que aconteceram.

O laboratório deliberadamente não armazena as revisões e não implementa `CalcularPontuacao`, `ClassificarPessoa` ou outro resumo de desempenho. Os testes verificam regras concretas de construção e apresentação. Um teste que confirme que não há pontuação não é substituto de uma revisão de produto, mas documenta uma fronteira importante do exemplo.

Também não precisamos inventar sucesso para fechar o capítulo. Uma próxima tentativa pode ser adiada, abandonada ou substituída ao surgir nova informação. Revisar significa escolher conscientemente o que fazer com o aprendizado, não provar uma melhoria linear.

## Testar a opção de não registrar

É comum testar só o caminho com todos os dados presentes. Aqui, também queremos demonstrar que a reflexão inteira é opcional e que o quarto campo pode ficar ausente:

```cs
var semRelatoDeApoio = new RevisaoDePratica(
    "Exemplo fictício: a reunião terminou sem decisão.",
    "Pedi para rever as opções.",
    "Combinar responsáveis antes da próxima conversa.");

Assert.Null(semRelatoDeApoio.ApoioOuLimite);
```

Não é necessário adicionar um campo “motivo para não responder”; isso recriaria a pressão que tentamos evitar. Uma interface poderia simplesmente omitir a pergunta ou apresentar “prefiro não informar”. Se um sistema precisa de métricas de completude, não transforme a recusa em falha.

Uma bateria de testes útil cobre cada classe sem avaliar comportamento humano: campo obrigatório vazio, enum fora do intervalo, valor opcional omitido, formatação previsível e ausência de reflexão no roteiro. Esses testes protegem decisões explícitas do programa e facilitam uma mudança futura. Se a política mudar, altere o código e a expectativa deliberadamente, em vez de apagar um teste porque ele falhou.

Ao ler nomes de testes, pergunte “qual promessa de software está sendo verificada?”. Por exemplo, `RoteiroNaoExigeUmaReflexaoPessoal` documenta um limite melhor do que um teste genérico chamado `TudoFunciona`. Bons nomes permitem que outra pessoa entenda o propósito sem precisar interpretar o resultado do teste como uma avaliação de alguém.

## Quando pedir apoio

Reflexão cotidiana não substitui cuidado especializado. Uma atividade didática não deve interpretar sofrimento intenso, sugerir tratamento ou prescrever respostas. Se a conversa ou o tema trouxer sofrimento persistente, uma pessoa pode interromper o exercício e procurar alguém de confiança ou profissional qualificado. Se houver risco imediato, procure os serviços locais apropriados.

Também vale pedir apoio para questões organizacionais: assédio, sobrecarga, falta de recursos e conflitos de responsabilidade não se resolvem necessariamente com autocontrole individual. Um processo de equipe ou canal formal pode ser necessário. Reconhecer o limite da atividade é parte do aprendizado, não um fracasso.

::: pense Escreva uma reflexão sem julgamento
Use uma cena inventada e preencha: “notei ___; escolhi ___; na próxima situação talvez eu ___; um apoio útil seria ___”. Remova qualquer frase que defina alguém como “bom”, “ruim”, “fraco” ou “forte”.
:::

::: exercicio Revise com gentileza e especificidade
Imagine que interrompeu uma colega e depois pediu desculpas. Escreva uma observação do que fez e uma próxima tentativa, sem declarar que é uma pessoa “boa” ou “ruim”.
---
**Resposta possível:** “Interrompi a explicação antes de ela terminar; na próxima conversa vou esperar a pausa e perguntar se ela concluiu.” A descrição aponta um comportamento revisável, não define identidade.
:::

::: exercicio Leia um teste sem transformá-lo em nota
O teste confirma que um roteiro com reflexão `null` ainda é apresentado. O que esse resultado garante no programa? Cite uma coisa que ele não permite concluir sobre uma pessoa que não registrou reflexão.
---
**Resposta possível:** garante que o software não exige aquele objeto para exibir o roteiro. Não permite concluir por que alguém omitiu a reflexão, como se sente ou se aprendeu; essas conclusões não estão nos dados nem no teste.
:::

::: quiz
? Uma revisão cuidadosa serve para:
+ Identificar uma ação possível para a próxima vez sem atribuir uma nota.
- Comparar a pessoa com colegas.
- Provar que uma prática sempre funciona.
> A reflexão orienta uma tentativa, não mede valor nem garante resultado.

? O campo `ApoioOuLimite`:
+ Pode registrar o apoio ou limite que a pessoa escolheu mencionar.
- Deve obrigar a revelar uma história privada.
- É usado para criar um índice de desempenho.
> Compartilhar é opcional; não há pontuação no modelo.

? Se uma conversa não chegou a um acordo:
+ É possível revisar o processo e considerar outro apoio ou canal.
- Isso prova que a pessoa falhou emocionalmente.
- A emoção de alguém deve ser inferida pelo código.
> Resultados dependem do contexto e de várias pessoas; não são teste individual.
? Que campo de `RevisaoDePratica` pode ficar vazio?
+ `ApoioOuLimite`, que é opcional (`string?`)
- `OQueNotei`
- `EscolhaFeita`
- `ProximaTentativa`
> Os três primeiros são obrigatórios; o apoio ou limite depende do que a pessoa escolhe mencionar.

? Por que o modelo não tem nota nem ranking?
+ Porque revisar é aprender sobre ações possíveis, e não pontuar pessoas
- Porque o C# não faz contas
- Porque notas são ilegais
- Para o código ficar menor
> Uma reflexão útil descreve ações específicas, não identidades.

? Registrar a revisão é obrigatório?
+ Não: escolher não registrar também é válido
- Sim, todo dia
- Sim, no fim do projeto
- Sim, se houve conflito
> O livro trata a prática como opcional.
:::

::: resumo
- Prática é uma oportunidade de observar e escolher uma próxima tentativa.
- O modelo não contém nota ou ranking e não garante o resultado da conversa.
- Relatos pessoais são opcionais; apoio e limites podem ser escolhidos.
- Uma reflexão útil descreve ações específicas, não identidades.
:::

::: cartas
Revisão de prática :: Reflexão opcional sobre sinais, escolhas e próximos passos.
Aprendizado sem placar :: Revisar um comportamento sem classificar a pessoa.
Próxima tentativa :: Ação pequena que pode ser escolhida para outra situação.
Placar (risco) :: Pontuação ou ranking que transforma uma reflexão em comparação entre pessoas.
Ação específica :: Algo observável que se pode repetir ou mudar.
Apoio profissional :: Ajuda de alguém qualificado, quando o assunto pesa; no Brasil, CVV 188 e SAMU 192.
Valor opcional :: Campo que pode ficar sem valor, sem erro.
Reflexão opcional :: Pensar sobre uma prática sem ser obrigado a registrar.
:::
