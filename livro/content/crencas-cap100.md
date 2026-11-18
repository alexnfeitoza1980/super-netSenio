---
id: crencas-cap100
order: 129
kind: capitulo
num: 100
group: C# & Mentalidade · Parte 16 · Crenças e C#
title: Circunstâncias, limites e escolhas possíveis
subtitle: Reconhecer uma ação possível não é fingir que todos começam do mesmo lugar. Contexto também faz parte do modelo.
minutes: 50
hue: 316
objectives:
- Distinguir um passo individual de um resultado que depende de outras pessoas ou condições
- Evitar a ideia de que crenças, sozinhas, determinam conquistas
- Modelar contexto sem criar uma classificação de valor para pessoas
origem: livros/07-crencas-csharp/content/138-cap100.md
---

::: vila Um curso que não cabe no horário
Bia quer estudar à noite, mas divide o computador com a família e cuida do irmão em vários dias da semana. A frase “se eu quisesse de verdade, daria um jeito” ignora tempo e acesso. Ela encontra uma janela possível no sábado e também pergunta se a equipe pode compartilhar um computador do laboratório. Há passos individuais e condições compartilhadas; nenhum apaga o outro.
:::

## Possibilidade não é promessa

Reconhecer que conquistas não dependem apenas das circunstâncias pode incentivar ação, mas essa ideia fica perigosa se for lida como “basta acreditar e se esforçar”. Resultados dependem de **recursos, oportunidades, saúde, segurança, regras e outras pessoas**, além das escolhas individuais. Dizer isso não significa desistir: significa planejar sem transformar obstáculos reais em culpa.

Uma pergunta mais útil do que “por que não consegui?” pode ser: **qual parte depende de mim, qual depende de acordo ou apoio, e qual condição não posso mudar agora?**

| Alcance | Exemplo | Pergunta prática |
|---|---|---|
| Individual | Dividir uma tarefa grande em 15 minutos | Qual próximo passo cabe hoje? |
| Compartilhado | Pedir acesso ao ambiente de teste | Com quem preciso combinar? |
| Fora do meu alcance agora | Uma falha de serviço externo | Qual alternativa ou apoio está disponível? |

Essas categorias descrevem a **ação**, não o valor ou a capacidade de quem enfrenta a situação. Podem mudar conforme tempo e contexto.

## Um modelo de ações, não de pessoas

```cs Jornada/Crencas/AcaoPossivel.cs
namespace Jornada.Crencas;

public enum AlcanceDaAcao { Individual, Compartilhado, IndisponivelAgora }

public sealed record AcaoPossivel(
    string Descricao,
    AlcanceDaAcao Alcance,
    string? ApoioNecessario);
```

`IndisponivelAgora` não quer dizer “impossível para sempre”. Ele registra que, dadas as condições descritas, essa ação não está acessível neste momento. O modelo não atribui causa moral nem promete que uma ação pequena resolva um problema estrutural.

```cs Uso
var acoes = new[]
{
    new AcaoPossivel("Rever um exemplo por 15 minutos", AlcanceDaAcao.Individual, null),
    new AcaoPossivel("Reservar o computador do laboratório", AlcanceDaAcao.Compartilhado, "Equipe"),
    new AcaoPossivel("Conectar-se ao servidor fora de manutenção", AlcanceDaAcao.IndisponivelAgora, null)
};
```

::: exercicio Planeje sem culpar
Uma estudante não consegue compilar porque a rede institucional está indisponível. Dê um exemplo de ação individual e um passo que depende de outras pessoas ou condições. Inclua também uma opção para pausar.
---
**Resposta possível:** individual: revisar a mensagem de erro já salva localmente; compartilhado: avisar a equipe de infraestrutura e perguntar pelo prazo; pausa: fazer outra atividade ou encerrar o estudo por hoje. Nenhuma opção garante quando a rede voltará.
:::

::: quiz
? “Ainda não tenho acesso à máquina do laboratório” deve ser tratado como:
+ Uma condição contextual que pode exigir apoio ou alternativa.
- Evidência de falta de esforço.
- Prova de incapacidade permanente.
> A descrição de uma barreira não autoriza julgar a pessoa.

? Por que incluir `AlcanceDaAcao`?
+ Para mostrar dependências possíveis de cada próximo passo.
- Para ordenar pessoas entre capazes e incapazes.
- Para provar que qualquer obstáculo pode ser removido individualmente.
> O modelo deve representar limites, não criar um ranking.

? Uma ação fora do alcance agora significa:
+ Que as condições atuais não permitem aquela ação, e podem mudar.
- Que ela será impossível para sempre.
- Que a pessoa deveria tentar com mais força.
> O estado descreve o contexto no momento, sem fazer uma previsão definitiva.
:::

::: resumo
- Escolhas existem dentro de condições e recursos desiguais.
- Diferencie ações individuais, compartilhadas e indisponíveis no momento.
- Uma barreira não prova falta de vontade ou capacidade.
- Um próximo passo pode ser pedir apoio, procurar alternativa ou pausar.
:::

::: cartas
Contexto :: Condições e recursos que ajudam a entender uma situação.
Alcance de uma ação :: O que depende da pessoa, de um acordo ou de condições externas.
“Ainda não” :: Descrição de disponibilidade atual, não uma previsão permanente.
:::
