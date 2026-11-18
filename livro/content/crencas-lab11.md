---
id: crencas-lab11
order: 134
kind: lab
num: 11
group: C# & Mentalidade · Parte 16 · Crenças e C#
title: Laboratório 11 — Caderno de Hipóteses
subtitle: Reunimos origem, observações, contexto e um próximo passo em um modelo pequeno, verificável e sem armazenamento de relatos.
minutes: 90
hue: 12
badge: L11
objectives:
- Integrar `record`, `enum`, coleções, validação e testes
- Registrar evidências que apoiem, contrariem ou contextualizem uma hipótese
- Gerar uma síntese que descreve registros sem declarar uma crença verdadeira
- Executar um exemplo fictício sem persistir dados pessoais
origem: livros/07-crencas-csharp/content/143-lab11.md
---

::: vila Uma hipótese de aprendizagem
O grupo de estudos quer observar uma hipótese técnica: “um exemplo menor ajuda a entender `GroupBy`”. A equipe escolhe um exemplo fictício, registra o que já sabe e planeja testar a consulta em uma lista pequena. O caderno não pergunta sobre renda, família ou história pessoal. Ele organiza o raciocínio; não avalia as pessoas que participam.
:::

## O que vamos construir

O **Caderno de Hipóteses** mantém uma coleção temporária de:

1. uma hipótese com origem declarada;
2. observações que apoiam, contradizem ou contextualizam essa hipótese;
3. uma síntese simples das categorias presentes.

Esse programa é uma demonstração de modelagem e coleções em C#. Não é ferramenta terapêutica, diário privado nem detector de crenças. O exemplo de execução usa dados fictícios, fica em memória e não pede entrada pessoal.

::: cuidado Privacidade e escolha
Não coloque no caderno informações que você não deseja compartilhar. O projeto não salva em arquivo nem envia dados, mas isso não torna apropriado registrar segredos em um exemplo de sala ou computador compartilhado. Você pode estudar o código usando somente os dados fictícios abaixo.
:::

## Passo 1 — Os tipos do domínio

O modelo usa tipos pequenos, com nomes que mostram a intenção:

```cs Jornada/Crencas/OrigemDaCrenca.cs
namespace Jornada.Crencas;

public enum OrigemDaCrenca
{
    Familiar,
    Social,
    ExperienciaPessoal,
    Desconhecida
}
```

```cs Jornada/Crencas/RelacaoDaEvidencia.cs
namespace Jornada.Crencas;

public enum RelacaoDaEvidencia
{
    Apoia,
    Contradiz,
    Contextualiza
}
```

```cs Jornada/Crencas/SinteseDaInvestigacao.cs
namespace Jornada.Crencas;

public enum SinteseDaInvestigacao
{
    SemObservacoes,
    SoApoio,
    SoContradicao,
    SoContexto,
    ApoioEContradicao,
    ApoioEContexto,
    ContradicaoEContexto,
    ApoioContradicaoEContexto
}
```

```cs Jornada/Crencas/Hipotese.cs
namespace Jornada.Crencas;

public sealed record Hipotese(string Enunciado, OrigemDaCrenca Origem);
```

```cs Jornada/Crencas/Evidencia.cs
namespace Jornada.Crencas;

public sealed record Evidencia(string Observacao, RelacaoDaEvidencia Relacao);
```

Uma `Hipotese` não é uma pessoa nem uma afirmação comprovada. A `Evidencia` descreve uma observação ligada à hipótese. O contexto merece ser explicado junto da observação, para evitar que um rótulo “apoia” faça o papel de prova.

## Passo 2 — Uma investigação e sua síntese

Cada `Investigacao` guarda uma hipótese e suas evidências. `Sintetizar` pergunta apenas quais categorias estão presentes:

```cs Jornada/Crencas/Investigacao.cs
namespace Jornada.Crencas;

public sealed class Investigacao
{
    private readonly List<Evidencia> evidencias = new();

    public Hipotese Hipotese { get; }
    public IReadOnlyList<Evidencia> Evidencias => evidencias.AsReadOnly();

    public Investigacao(Hipotese hipotese)
    {
        ArgumentNullException.ThrowIfNull(hipotese);
        if (string.IsNullOrWhiteSpace(hipotese.Enunciado))
            throw new ArgumentException("Descreva a hipótese.", nameof(hipotese));

        Hipotese = hipotese;
    }

    public void Registrar(Evidencia evidencia)
    {
        ArgumentNullException.ThrowIfNull(evidencia);
        if (string.IsNullOrWhiteSpace(evidencia.Observacao))
            throw new ArgumentException("Descreva a observação.", nameof(evidencia));

        evidencias.Add(evidencia);
    }

    public SinteseDaInvestigacao Sintetizar()
    {
        bool apoia = evidencias.Any(e => e.Relacao == RelacaoDaEvidencia.Apoia);
        bool contradiz = evidencias.Any(e => e.Relacao == RelacaoDaEvidencia.Contradiz);
        bool contextualiza = evidencias.Any(e => e.Relacao == RelacaoDaEvidencia.Contextualiza);

        return (apoia, contradiz, contextualiza) switch
        {
            (false, false, false) => SinteseDaInvestigacao.SemObservacoes,
            (true, false, false) => SinteseDaInvestigacao.SoApoio,
            (false, true, false) => SinteseDaInvestigacao.SoContradicao,
            (false, false, true) => SinteseDaInvestigacao.SoContexto,
            (true, true, false) => SinteseDaInvestigacao.ApoioEContradicao,
            (true, false, true) => SinteseDaInvestigacao.ApoioEContexto,
            (false, true, true) => SinteseDaInvestigacao.ContradicaoEContexto,
            _ => SinteseDaInvestigacao.ApoioContradicaoEContexto
        };
    }
}
```

Observe duas escolhas:

- A lista é privada; quem usa o modelo recebe uma visão de leitura, não pode substituir a coleção interna.
- Os estados preservam combinações de apoio, contraponto e contexto. Nenhum diz “verdadeiro”, “falso”, “capaz” ou “incapaz”.

## Passo 3 — O caderno em memória

O caderno reúne investigações e oferece uma consulta LINQ para localizar uma hipótese específica:

```cs Jornada/Crencas/CadernoDeHipoteses.cs
namespace Jornada.Crencas;

public sealed class CadernoDeHipoteses
{
    private readonly List<Investigacao> investigacoes = new();

    public IReadOnlyList<Investigacao> Investigacoes => investigacoes.AsReadOnly();

    public Investigacao Adicionar(Hipotese hipotese)
    {
        var nova = new Investigacao(hipotese);
        investigacoes.Add(nova);
        return nova;
    }

    public Investigacao? Encontrar(string enunciado) =>
        investigacoes.FirstOrDefault(i =>
            i.Hipotese.Enunciado.Equals(enunciado, StringComparison.OrdinalIgnoreCase));
}
```

O método não escreve no disco. Se o processo terminar, a lista temporária desaparece. Se a atividade precisar de confidencialidade, prefira exemplos genéricos e não compartilhe a tela.

## Passo 4 — Um cenário fictício

```cs Program.cs
using Jornada.Crencas;

var caderno = new CadernoDeHipoteses();
var investigacao = caderno.Adicionar(new Hipotese(
    "Um exemplo menor ajuda a entender GroupBy.",
    OrigemDaCrenca.Desconhecida));

investigacao.Registrar(new Evidencia(
    "A consulta com três itens ficou mais fácil de acompanhar.",
    RelacaoDaEvidencia.Apoia));

investigacao.Registrar(new Evidencia(
    "Ainda não está claro como tratar uma chave ausente.",
    RelacaoDaEvidencia.Contextualiza));

Console.WriteLine(investigacao.Sintetizar());
```

Saída:

```saida
ApoioEContexto
```

O resultado não significa que “exemplos menores sempre funcionam”. Apenas indica que a lista contém uma observação de apoio e outra que contextualiza uma pergunta não resolvida.

## Passo 5 — Testes como contratos

Os testes confirmam os estados possíveis, a validação de dados vazios e a consulta que ignora diferenças entre maiúsculas e minúsculas:

```cs CadernoDeHipotesesTests.cs
using Jornada.Crencas;

namespace Crencas.Testes;

public class CadernoDeHipotesesTests
{
    [Fact]
    public void Sem_observacoes_nao_declara_a_hipotese_comprovada()
    {
        var investigacao = new Investigacao(
            new Hipotese("Um exemplo ajuda.", OrigemDaCrenca.Desconhecida));

        Assert.Equal(SinteseDaInvestigacao.SemObservacoes, investigacao.Sintetizar());
    }

    [Fact]
    public void Observacoes_em_direcoes_diferentes_sao_preservadas()
    {
        var investigacao = new Investigacao(
            new Hipotese("Um exemplo ajuda.", OrigemDaCrenca.Social));
        investigacao.Registrar(new Evidencia("Ajudou neste caso.", RelacaoDaEvidencia.Apoia));
        investigacao.Registrar(new Evidencia("Outro caso precisou de desenho.", RelacaoDaEvidencia.Contradiz));

        Assert.Equal(SinteseDaInvestigacao.ApoioEContradicao, investigacao.Sintetizar());
    }

    [Fact]
    public void Evidencia_vazia_e_recusada()
    {
        var investigacao = new Investigacao(
            new Hipotese("Um exemplo ajuda.", OrigemDaCrenca.Desconhecida));

        Assert.Throws<ArgumentException>(() =>
            investigacao.Registrar(new Evidencia(" ", RelacaoDaEvidencia.Apoia)));
    }
}
```

Os testes protegem o contrato do código. Eles não demonstram uma teoria psicológica nem avaliam o comportamento de quem usa o programa.

::: exercicio Acrescente uma terceira observação
Inclua uma evidência `Contextualiza` sobre uma tarefa que exigia mais tempo. O que deve mudar na síntese se já havia observações que apoiam e contradizem a hipótese?
---
**Resposta:** a síntese continua `ApoioEContradicao`, pois as observações aparecem nas duas direções. A nova observação acrescenta contexto sem apagar as categorias anteriores.
:::

::: quiz
? O que `ApoioEContexto` afirma?
+ Que a lista contém observações classificadas como apoio e contexto.
- Que a hipótese é metade verdadeira.
- Que a pessoa está indecisa.
> A síntese é sobre as categorias registradas, não sobre uma pessoa nem sobre uma verdade matemática.

? Onde o caderno guarda as investigações?
+ Em uma lista em memória, sem persistir os dados.
- Em um serviço remoto.
- Em um arquivo automático no perfil do usuário.
> O projeto de demonstração mantém os objetos apenas enquanto o processo está ativo.

? Para que servem os testes do laboratório?
+ Verificar regras explícitas de validação e síntese do código.
- Confirmar que todas as crenças humanas foram modeladas.
- Medir a capacidade de quem estuda.
> Testes verificam o programa, não fazem avaliação psicológica.
:::

::: resumo
- O laboratório integra modelos imutáveis, listas privadas e consultas LINQ.
- A síntese descreve as evidências registradas, não a verdade da hipótese.
- O programa usa dados fictícios e não grava nem envia conteúdo.
- Testes verificam o comportamento do software; não validam uma teoria sobre pessoas.
:::

::: cartas
Caderno de Hipóteses :: Coleção temporária de hipóteses e observações para um exercício técnico.
`IReadOnlyList<T>` :: Interface que permite ler uma lista sem expor suas operações de alteração.
Teste de unidade :: Verificação automatizada de uma regra específica do código.
:::
