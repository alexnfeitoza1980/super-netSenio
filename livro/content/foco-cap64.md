---
id: foco-cap64
order: 86
kind: capitulo
num: 64
group: C# & Mentalidade · Parte 13 · Foco em C#
title: Autocontrole: planos "se–então" e pré-compromisso
subtitle: Resistir à tentação não é só força de vontade: é desenho. Planos "se–então", a conta do desconto hiperbólico e o pré-compromisso, com uma dose de honestidade sobre o teste do marshmallow.
minutes: 55
hue: 275
objectives:
- Entender o **autocontrole** como habilidade treinável, ligada às **funções executivas**
- Contar o **teste do marshmallow** com suas ressalvas
- Escrever **planos "se–então"** (intenções de implementação) e executá-los em C#
- Calcular o **desconto hiperbólico**: por que o presente vence o futuro
- Usar o **pré-compromisso** para tirar a tentação do caminho
origem: livros/04-foco-em-csharp/content/96-cap64.md
---

::: vila Os cinco minutos que viram uma hora
"Só vou olhar um instante." Caio abre as redes sociais durante a compilação e, quando percebe, passou uma hora. Lia sorri: "Você não precisa de mais força de vontade. Precisa de **menos oportunidade** e de **um plano pronto** para quando a vontade aparecer".
:::

::: ilustracao img/foco/cap64.svg | O autocontrole pode ser desenhado: um plano 'se–então' decide antes da tentação chegar.
:::

## O que o livro diz

> **Autocontrole** (a capacidade de adiar uma recompensa e controlar impulsos) é um dos melhores previsores de como a vida se desenrola; e, felizmente, pode ser **treinado**.

O que vale guardar:

- No clássico **teste do marshmallow**, crianças escolhem entre um doce agora ou dois depois. Em estudos de seguimento, as que esperavam tendiam a ter melhor desempenho depois. Uma pesquisa de longo prazo na Nova Zelândia (Dunedin) também ligou o autocontrole na infância a saúde, patrimônio e menos problemas legais na vida adulta.
- A mente tem um modo **"frio"** (deliberado, de longo prazo) e um **"quente"** (emocional, impulsivo). Quem consegue **transformar o desejo em algo "frio"** (focar nas propriedades abstratas do doce, não no gosto) espera mais.
- As **funções executivas** (memória de trabalho, flexibilidade e **inibição**) sustentam o autocontrole e podem ser fortalecidas com prática.
- Estratégias que funcionam são **concretas**: mudar o ambiente, distrair-se de propósito, e ter **planos prontos** para a tentação.

::: cuidado O marshmallow, com ressalvas
Réplicas do teste, com amostras maiores e mais diversas, mostraram que o **contexto familiar e social** explica boa parte da diferença: crianças de lares mais estáveis esperam mais **porque confiam** que a promessa será cumprida. Ou seja: o resultado **não** é um destino fixo, e o autocontrole depende também do ambiente. O livro (e este capítulo) usa o experimento como ilustração, não como sentença.
:::

## Da vida para o código

| No livro | Em engenharia de software |
|---|---|
| Doce agora × dois depois | **Atalho agora** × qualidade depois (dívida técnica) |
| Modo quente × frio | **Reagir ao incidente** × pensar no projeto |
| Funções executivas | **Planejar, inibir, alternar** tarefas |
| Planos "se–então" | **Regras prontas** para as tentações previsíveis |
| Mudar o ambiente | **Bloquear** distrações durante o foco |
| Confiança na promessa | **Equipe confiável** → decisões de longo prazo |

::: figura Desconto: o presente parece maior
<svg viewBox="0 0 740 230" role="img" aria-label="Duas curvas de valor percebido pelo tempo: a paciente cai devagar e a impulsiva cai depressa, abaixo da recompensa imediata">
  <path class="arrow" d="M50 190L50 20"/><path class="arrow" d="M50 190L720 190"/>
  <text x="44" y="30" text-anchor="end" font-size="11">valor</text>
  <text x="716" y="210" text-anchor="end" font-size="11">tempo até a recompensa →</text>
  <line x1="50" y1="118" x2="720" y2="118" class="f-info" stroke-dasharray="6 6" style="fill:none;stroke-width:1.5"/>
  <text x="716" y="112" text-anchor="end" font-size="11">recompensa imediata (10)</text>
  <path d="M50 60 C200 80 350 130 700 160" class="f-good" style="fill:none;stroke-width:3"/>
  <path d="M50 60 C120 130 220 170 700 185" class="f-bad" style="fill:none;stroke-width:3"/>
  <line x1="330" y1="36" x2="360" y2="36" class="f-good" style="stroke-width:3"/><text x="368" y="40" font-size="12">paciente (k baixo): esperar compensa</text>
  <line x1="330" y1="58" x2="360" y2="58" class="f-bad" style="stroke-width:3"/><text x="368" y="62" font-size="12">impulsivo (k alto): o futuro encolhe depressa</text>
</svg>
:::

## 1) O plano "se–então"

Pesquisadores chamam de **intenção de implementação**: em vez de "vou me concentrar", você decide **de antemão** o que fará quando surgir a tentação. A decisão já está tomada quando a vontade chega.

```cs Jornada/Foco/Cap64/PlanoSeEntao.cs
namespace Jornada.Foco.Cap64;

public sealed record RegraSeEntao(string Gatilho, string Acao)
{
    public override string ToString() => $"Se {Gatilho}, então {Acao}.";
}

public sealed class PlanoSeEntao
{
    private readonly List<RegraSeEntao> regras = new();

    public void Adicionar(string gatilho, string acao)
    {
        if (string.IsNullOrWhiteSpace(gatilho) || string.IsNullOrWhiteSpace(acao))
            throw new ArgumentException("Gatilho e ação são obrigatórios.");

        if (regras.Any(r => r.Gatilho.Equals(gatilho, StringComparison.OrdinalIgnoreCase)))
            throw new InvalidOperationException($"Já existe uma regra para: {gatilho}");

        regras.Add(new RegraSeEntao(gatilho.Trim(), acao.Trim()));
    }

    /// <summary>Procura um gatilho contido na situação descrita; sem regra, pede uma pausa deliberada.</summary>
    public string Responder(string situacao)
    {
        var regra = regras.FirstOrDefault(r =>
            situacao.Contains(r.Gatilho, StringComparison.OrdinalIgnoreCase));

        return regra?.Acao ?? "Parar 10 segundos e decidir de forma deliberada";
    }

    public IReadOnlyList<string> Descrever() => regras.Select(r => r.ToString()).ToList();
}
```

```cs Uso
var plano = new PlanoSeEntao();
plano.Adicionar("o chat apitar",                       "ignorar até o fim do bloco");
plano.Adicionar("a vontade de abrir as redes surgir",  "levantar e beber água");
plano.Adicionar("o teste falhar de novo",              "reverter a mudança antes de mexer em mais nada");

plano.Descrever().ToList().ForEach(Console.WriteLine);
Console.WriteLine("---");
Console.WriteLine(plano.Responder("O CHAT APITAR de novo"));
Console.WriteLine(plano.Responder("chegou um e-mail estranho"));
```

```saida
Se o chat apitar, então ignorar até o fim do bloco.
Se a vontade de abrir as redes surgir, então levantar e beber água.
Se o teste falhar de novo, então reverter a mudança antes de mexer em mais nada.
---
ignorar até o fim do bloco
Parar 10 segundos e decidir de forma deliberada
```

::: dica Um bom gatilho é concreto
"Se eu ficar distraído" não funciona: **como você sabe?** Prefira gatilhos **observáveis**: *"se eu abrir uma aba nova sem objetivo"*, *"se a compilação demorar mais de 30 s"*. E prefira ações **pequenas e físicas** (levantar, respirar, escrever uma linha), não promessas abstratas.
:::

## 2) Por que o presente vence: o desconto hiperbólico

Por que, mesmo sabendo que esperar compensa, cedemos? Porque o valor **percebido** de uma recompensa cai rapidamente com o atraso. Um modelo clássico é o **desconto hiperbólico**: `valor percebido = valor / (1 + k × atraso)`. O parâmetro `k` mede a "impaciência".

```cs Jornada/Foco/Cap64/Desconto.cs
namespace Jornada.Foco.Cap64;

public static class Desconto
{
    public static double ValorPercebido(double valor, int atrasoEmDias, double k)
    {
        if (atrasoEmDias < 0) throw new ArgumentOutOfRangeException(nameof(atrasoEmDias));
        if (k < 0) throw new ArgumentOutOfRangeException(nameof(k));
        return valor / (1 + k * atrasoEmDias);
    }

    public static bool EsperaCompensa(double valorAgora, double valorDepois, int atrasoEmDias, double k) =>
        ValorPercebido(valorDepois, atrasoEmDias, k) > valorAgora;
}
```

```cs Uso
// Recompensa: 10 agora ou 15 daqui a 7 dias.
foreach (double k in new[] { 0.05, 0.1, 0.2, 0.5 })
{
    double percebido = Desconto.ValorPercebido(15, 7, k);
    string decisao = Desconto.EsperaCompensa(10, 15, 7, k) ? "espera" : "pega agora";
    Console.WriteLine($"k = {k,4:F2} → 15 em 7 dias vale {percebido,5:F2} → {decisao}");
}
```

```saida
k = 0,05 → 15 em 7 dias vale 11,11 → espera
k = 0,10 → 15 em 7 dias vale  8,82 → pega agora
k = 0,20 → 15 em 7 dias vale  6,25 → pega agora
k = 0,50 → 15 em 7 dias vale  3,33 → pega agora
```

Em engenharia: "fazer o atalho agora" (valor 10, imediato) contra "fazer certo depois" (valor 15, mas só no futuro). Quanto **mais impaciente** o time (k alto), mais tende a acumular dívida técnica. Dá para **reduzir o k percebido** tornando o benefício futuro **mais próximo e visível** (um painel que mostra o ganho do teste, por exemplo).

::: cuidado Um modelo, não a realidade
O desconto hiperbólico é uma **descrição matemática útil** do comportamento, com muitas variações na literatura, e o `k` não é um número que você "tem" fixo: muda com cansaço, humor, confiança no futuro. Use-o para **pensar**, não para classificar pessoas.
:::

## 3) Pré-compromisso: tirar a tentação do caminho

A estratégia mais robusta contra a tentação é **não depender de vencê-la**. O **pré-compromisso** é decidir hoje de modo que amanhã a escolha errada fique difícil:

```cs Jornada/Foco/Cap64/PreCompromisso.cs
namespace Jornada.Foco.Cap64;

public sealed class PreCompromisso
{
    private readonly HashSet<string> bloqueados;
    private readonly int liberadoAPartirDoMinuto;

    public PreCompromisso(IEnumerable<string> bloqueados, int liberadoAPartirDoMinuto)
    {
        this.bloqueados = new HashSet<string>(bloqueados, StringComparer.OrdinalIgnoreCase);
        this.liberadoAPartirDoMinuto = liberadoAPartirDoMinuto;
    }

    public bool Permitido(string alvo, int minutoAtual) =>
        minutoAtual >= liberadoAPartirDoMinuto || !bloqueados.Contains(alvo);
}
```

```cs Uso
var foco = new PreCompromisso(new[] { "redes-sociais", "noticias" }, liberadoAPartirDoMinuto: 12 * 60);

Console.WriteLine($"redes às 10h: {foco.Permitido("redes-sociais", 10 * 60)}");
Console.WriteLine($"docs  às 10h: {foco.Permitido("docs", 10 * 60)}");
Console.WriteLine($"redes ao meio-dia: {foco.Permitido("redes-sociais", 12 * 60)}");
```

```saida
redes às 10h: False
docs  às 10h: True
redes ao meio-dia: True
```

```cs JornadaTests/Foco/Cap64/Cap64Tests.cs
using Jornada.Foco.Cap64;
using Xunit;

public class Cap64Tests
{
    [Fact]
    public void Responde_com_a_acao_planejada()
    {
        var p = new PlanoSeEntao();
        p.Adicionar("chat apitar", "ignorar");
        Assert.Equal("ignorar", p.Responder("O Chat Apitar!"));
    }

    [Fact]
    public void Sem_regra_pede_pausa() =>
        Assert.StartsWith("Parar", new PlanoSeEntao().Responder("qualquer coisa"));

    [Fact]
    public void Gatilho_duplicado_e_rejeitado()
    {
        var p = new PlanoSeEntao();
        p.Adicionar("x", "a");
        Assert.Throws<InvalidOperationException>(() => p.Adicionar("X", "b"));
    }

    [Fact]
    public void Quanto_maior_o_k_menor_o_valor_percebido() =>
        Assert.True(Desconto.ValorPercebido(15, 7, 0.5) < Desconto.ValorPercebido(15, 7, 0.05));

    [Fact]
    public void Sem_atraso_nao_ha_desconto() =>
        Assert.Equal(15, Desconto.ValorPercebido(15, 0, 0.9));

    [Fact]
    public void Pre_compromisso_bloqueia_so_ate_o_horario()
    {
        var c = new PreCompromisso(new[] { "redes" }, 600);
        Assert.False(c.Permitido("redes", 599));
        Assert.True(c.Permitido("redes", 600));
    }
}
```

::: exercicio Quanto custa esperar?
Escreva `int AtrasoMaximoQueCompensa(double valorAgora, double valorDepois, double k)` que devolve o **maior número de dias** de espera em que ainda compensa esperar (retorna `0` se nem o 1º dia compensa), procurando de 1 a 365. Teste com `valorAgora=10`, `valorDepois=15`, `k=0.1` (a resposta é **4**: `15/(1+0,1·4)=10,71 > 10`, mas em 5 dias `15/1,5 = 10`, que não é maior).

--- resposta
```cs Jornada/Foco/Cap64/AtrasoMaximo.cs
namespace Jornada.Foco.Cap64;

public static class AtrasoMaximo
{
    public static int AtrasoMaximoQueCompensa(double valorAgora, double valorDepois, double k)
    {
        int ultimo = 0;
        for (int dias = 1; dias <= 365; dias++)
        {
            if (!Desconto.EsperaCompensa(valorAgora, valorDepois, dias, k))
                break;
            ultimo = dias;
        }
        return ultimo;
    }
}
```
```cs Jornada/Foco/Cap64/AtrasoMaximoTests.cs
using Jornada.Foco.Cap64;
using Xunit;

public class AtrasoMaximoTests
{
    [Fact]
    public void Exemplo_do_enunciado() =>
        Assert.Equal(4, AtrasoMaximo.AtrasoMaximoQueCompensa(10, 15, 0.1));

    [Fact]
    public void Muito_impaciente_nunca_espera() =>
        Assert.Equal(0, AtrasoMaximo.AtrasoMaximoQueCompensa(10, 15, 5));
}
```
Como o valor percebido **só diminui** com o atraso, o `break` na primeira falha é seguro (a função é monotônica).
:::

::: resumo
- **Autocontrole** é treinável e apoiado pelas **funções executivas**; o teste do marshmallow é uma boa **ilustração**, mas o contexto e a confiança no ambiente pesam muito.
- Planos **"se–então"** decidem a resposta **antes** da tentação, com gatilhos observáveis e ações pequenas.
- O **desconto hiperbólico** explica por que o presente vence; é um **modelo**, e o `k` varia com o estado da pessoa.
- **Pré-compromisso** tira a tentação do caminho em vez de depender da força de vontade.
- Equipes de confiança tomam **decisões de longo prazo**: previsibilidade também ensina a esperar.
:::

::: quiz
? O que é um plano "se–então"?
+ Decidir de antemão a ação a executar quando um gatilho específico surgir
- Uma promessa vaga de ter mais foco
- Uma instrução condicional que só vale em código
- Um tipo de reunião
> Gatilhos concretos, ações pequenas.

? Que ressalva importante se faz ao teste do marshmallow?
+ Réplicas mostram que o contexto familiar e social explica boa parte da diferença
- Ele foi desmentido por completo e não tem valor
- Só vale para adultos
- Mede inteligência
> Crianças esperam mais quando confiam na promessa.

? O que o parâmetro k representa no desconto hiperbólico?
+ A "impaciência": quanto maior, mais rápido o futuro perde valor
- O número de dias
- O valor da recompensa
- O tamanho do time
> k alto: pega agora.

? Por que o pré-compromisso é mais robusto que a força de vontade?
+ Porque remove a tentação do caminho em vez de depender de vencê-la
- Porque proíbe qualquer pausa
- Porque exige menos código
- Porque é obrigatório
> Decida hoje, para que amanhã a escolha errada fique difícil.

? Qual é um bom gatilho para um plano "se–então"?
+ "Se eu abrir uma aba nova sem objetivo"
- "Se eu ficar distraído"
- "Se eu estiver triste"
- "Se algo acontecer"
> Observável e específico.

? Por que o `break` no exercício é seguro?
+ O valor percebido só diminui com o atraso (monotônico)
- Porque o laço é curto
- Porque o C# otimiza
- Porque k é sempre zero
> Se não compensa em N dias, não compensa depois.
:::

::: cartas
Autocontrole :: Capacidade de adiar recompensa e controlar impulsos.
Funções executivas :: Memória de trabalho, flexibilidade e inibição.
Cognição quente × fria :: Modo emocional e impulsivo × modo deliberado e de longo prazo.
Plano "se–então" :: Intenção de implementação: gatilho concreto + ação já decidida.
Desconto hiperbólico :: Modelo em que o valor percebido cai rápido com o atraso.
Pré-compromisso :: Decisão presente que dificulta a escolha impulsiva futura.
Teste do marshmallow :: Experimento de adiar um doce, com ressalvas sobre contexto.
Dívida técnica :: Atalho que rende agora e cobra juros depois.
:::
