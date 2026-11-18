---
id: lab-sns-repeticao
order: 271
kind: lab
num: 38
group: Laboratório · Parte 45 · super-netSenior · da repetição à solução única
title: O que se repetia entre os projetos
subtitle: Medir antes de unificar: cópias literais eram poucas, mas os mesmos conceitos foram reescritos várias vezes — e cada versão envelheceu de um jeito.
minutes: 20
hue: 190
objectives:
- Distinguir **repetição literal** de **repetição conceitual** e saber medir cada uma
- Entender por que o mesmo conceito, reescrito várias vezes, **diverge em qualidade**
- Ler a tabela de "antes e depois" que guiou o `super-netSenior`
origem: livro/tools/conteudo/sns-01-repeticao.md
---

::: vila Os sete projetos
Ao longo dos estudos para entrevistas .NET sênior nasceram **sete projetos**: uma API bancária, um banco digital modular, uma demonstração de microsserviços, um esqueleto Clean Architecture, um guia de SOLID, um guia de arquitetura em Markdown e uma biblioteca de livros. Cada um com seu repositório, seu site e seu glossário. Antes de juntar tudo, a pergunta foi: **o que, de fato, está repetido?**
:::

## Medir antes de unificar

A primeira intuição é procurar arquivos iguais. Em **1.109 arquivos** de código-fonte, só **21 grupos** tinham conteúdo idêntico, e quase todos irrelevantes: seis imagens duplicadas (cerca de 4,4 MB) e arquivos de boilerplate (`.gitignore`, `appsettings.Development.json`, `.csproj.user`).

A repetição que custava caro era **conceitual**: a mesma ideia reimplementada, com nomes e qualidades diferentes.

| Conceito | Onde aparecia | O problema |
|---|---|---|
| Value Object `Cpf` | monolito e BankMore | Um só conferia o **tamanho**; o outro validava tudo, mas **lançava exceção** |
| `Money` | monolito e esqueleto Clean Architecture | Duas versões mínimas, sem operações |
| `Result` / `Error` | só no monolito | O BankMore usava exceções para fluxo de negócio |
| Outbox | monolito (completo) e demo de microsserviços (só ilustrativo) | Tipo de evento fixo no código do worker |
| Idempotência | BankMore (tabela) e demo (`IMemoryCache`) | Duas estratégias incompatíveis, uma não sobrevive a reinício |
| Clean Architecture (DbContext, repositórios, Swagger) | 4 a 5 projetos | O mesmo esqueleto copiado e adaptado a cada vez |
| Infra | 3 `docker-compose.yml` e 8 `Dockerfile` | Nada compartilhado |
| Versão do .NET | net8, net9 e net10 no mesmo workspace | Três conjuntos de pacotes |

::: pense O que o CPF mostra
Duas implementações do **mesmo** value object: uma aceitava o CPF `111.111.111-11` (tem 11 dígitos), a outra o rejeitava. Quando um conceito é copiado, **ele não envelhece junto**: cada cópia ganha correções que as outras não recebem.
:::

## Por que a repetição conceitual é pior

```csharp Cpf do monolito (só o tamanho)
var digits = new string(raw.Where(char.IsDigit).ToArray());
if (digits.Length != 11)
    return Result.Failure<Cpf>(Error.Validation("Cpf.InvalidLength", "CPF deve conter 11 dígitos."));
```

```csharp Cpf unificado (dígitos verificadores)
private static int CalcularDigito(string cpf, int quantidade)
{
    var soma = 0;
    for (var i = 0; i < quantidade; i++)
        soma += (cpf[i] - '0') * (quantidade + 1 - i);

    var resto = soma % 11;
    return resto < 2 ? 0 : 11 - resto;
}
```

::: regra Uma regra, um lugar
Quando uma regra de negócio vive em **um único** tipo, uma correção de segurança ou de validação vale para **todos** os módulos. É esse o ganho do `BuildingBlocks`.
:::

## Antes e depois

| Antes | Depois (no `super-netSenior`) |
|---|---|
| 2 versões de `Cpf` | **Uma**, com dígitos verificadores e `Result` |
| `Money`, `Email`, `Result` copiados | Um conjunto em `BuildingBlocks` |
| Outbox com tipo fixo | `OutboxDispatcher<TContext>` genérico |
| Idempotência em tabela ou em memória | Índice único `(conta, chave)`; reenvio devolve o mesmo resultado |
| Dono da conta por **nome** | FK real `Usuario → Cliente` |
| Versões net8/net9/net10 | **.NET 10** e `Directory.Packages.props` |

::: exercicio Meça o seu próprio workspace
1. Liste os arquivos sem `bin/`, `obj/` e `node_modules/` e calcule o **hash** de cada um. Quantos grupos de arquivos idênticos existem?
2. Escolha três conceitos (`Money`, `Result`, idempotência, …) e procure as classes com esse nome em todos os projetos. Em quantos projetos cada um aparece?
3. Compare duas implementações do mesmo conceito. Qual validou mais? Qual lançaria exceção?
:::

::: resumo
- **Cópia literal** é fácil de medir (hash) e geralmente barata; **repetição conceitual** é o que custa caro.
- Cópias do mesmo conceito **divergem**: cada uma recebe as correções que lembraram de aplicar nela.
- Unificar não é juntar pastas: é escolher **um** lugar para cada regra e fazer os módulos dependerem dele.
:::

::: quiz
? Em 1.109 arquivos, só 21 grupos eram idênticos. O que isso indica?
- Que os projetos eram independentes e não tinham nada em comum
+ Que a repetição relevante era conceitual (mesmas ideias reescritas), não cópia de arquivos
- Que bastava apagar os arquivos duplicados para resolver
> Os grupos idênticos eram quase só imagens e boilerplate. O custo estava nos mesmos conceitos reimplementados de formas diferentes.

? Qual o principal risco de ter dois `Cpf` diferentes no mesmo workspace?
- O compilador recusa dois tipos com o mesmo nome
+ Eles divergem: um valida os dígitos verificadores e o outro só o tamanho, então o mesmo CPF inválido passa em um e não no outro
- O banco de dados não aceita CPF duplicado
> Cópias não envelhecem juntas; a correção feita numa não chega na outra.
:::

::: cartas
Repetição literal :: Arquivos com conteúdo idêntico (mede-se com hash).
Repetição conceitual :: O mesmo conceito reimplementado várias vezes, com divergências.
BuildingBlocks :: Núcleo compartilhado sem dependência de framework: Result, Cpf, Money, Email, Outbox.
:::
