---
id: amor-cap110
order: 142
kind: capitulo
num: 110
group: C# & Mentalidade · Parte 17 · Você é aquilo que ama, em C#
title: Reabituando o apetite
subtitle: Querer também se aprende, e se reaprende, com prática, companhia e paciência. Em C#, é o capítulo das funções como dados: curvas que se cruzam, sequências preguiçosas e o Zip para comparar dois caminhos.
minutes: 70
hue: 295
objectives:
- Entender que o **que queremos** (nossos "apetites") é, em boa parte, **aprendido**, e portanto **reaprendível**
- Ver por que **saber** não basta, e como **prática e comunidade** reformam o desejo
- Usar **delegates** (`Func<int, double>`) para representar curvas, e compô-los
- Trabalhar com sequências preguiçosas: **`Zip`**, **`TakeWhile`**, **`Enumerable.Range`**
origem: livros/08-voce-e-aquilo-que-ama-csharp/content/130-cap110.md
---

::: vila A mudança de editor
Caio decidiu trocar o editor de texto que usava há anos por outro, cheio de atalhos de teclado. Os primeiros dias foram **horríveis**: ele era mais lento, errava, queria voltar ao antigo. Lia propôs: "fiquem os dois por duas semanas, com um colega de prática". Na terceira semana, Caio percebeu que **tinha vontade** de usar os atalhos: o jeito novo tinha deixado de custar. Ele não tinha se convencido de nada; o hábito antigo tinha **perdido espaço**.
:::

::: ilustracao img/amor/cap110.svg | Dois hábitos competindo: o antigo perde força aos poucos, o novo ganha. Há um dia em que as curvas se cruzam, e antes dele o novo dá mais trabalho.
:::

## O que o livro diz

Smith usa um exemplo pessoal e bem-humorado para mostrar o ponto. Durante anos, ele leu livros convincentes sobre comida e agricultura, concordou com tudo... e **continuou comendo igual**. Ele cita essa distância para dizer que **o que sabemos não muda o que queremos comer**. A fome é uma característica biológica, mas **o objeto da fome**, **o que** desejamos comer, é, em grande parte, **aprendido** por hábito.

Quando, anos depois, resolveu mudar, o autor descreve um caminho que ele próprio chama de uma espécie de "alegoria" da formação do desejo:

1. **Primeiro ele passou a querer querer.** Os argumentos, as conversas em casa e as consultas médicas fizeram nascer o desejo de desejar algo melhor. Isso, por si só, **não bastou**.
2. **Comprometeu-se em comunidade.** A mudança foi uma promessa mútua com a esposa: ser parceiros, prestar contas, cozinhar juntos, suportar juntos as noites difíceis. Para ele, **a reabituação é comunitária**.
3. **Praticou o que ainda não desejava.** Começou a se exercitar **não porque gostava**, e sim porque sabia que era bom. Mais tarde, a prática criou o hábito, e o hábito **criou o desejo**: hoje ele quer correr.
4. **Usou ferramentas que carregam a sabedoria de outros.** Um aplicativo de acompanhamento alimentar funcionou, segundo ele, como **ponte para uma comunidade** de especialistas. A atenção consciente é como o adolescente que aprende a dirigir (tudo exige pensar), e a meta é que, com repetição, os novos hábitos fiquem **automáticos**.

E um vídeo famoso reforça a ideia: um engenheiro construiu uma bicicleta em que o guidão **funciona ao contrário** (virar à esquerda leva a roda para a direita). Depois de uma vida andando de bicicleta, ele **não conseguia** andar nela. Aprendeu, **depois de oito meses** de prática. O filho dele, com hábitos de equilíbrio menos arraigados, aprendeu em **duas semanas**. Moral do autor: hábitos antigos são difíceis de mudar, e **formar hábitos cedo** ajuda.

::: cuidado Sobre o que o autor recomenda
O autor conduz esse argumento para a vida de fé e de comunidade religiosa, onde propõe práticas de adoração como o "ginásio" que treina o desejo. Aqui, tiramos só a **mecânica**: conhecimento não basta; **prática repetida, companhia e paciência** reformam o que queremos. Tampouco é uma receita: tudo isso varia muito entre pessoas, e condições como saúde, dinheiro e tempo **pesam** na capacidade de mudar. Se um hábito é um sofrimento grave (como um vício), procure **apoio profissional**.
:::

::: analogia Trocar de idioma
Quem aprende um idioma novo passa por uma fase em que o antigo "atrapalha" e o novo "custa". Um dia, percebe que **pensou** no idioma novo, sem traduzir. Ninguém decidiu isso: foi o treino acumulado.
:::

## Da vida para o código

| No livro | Em C# |
|---|---|
| A força de um hábito ao longo dos dias | **`Func<int, double>`**: uma função "dia → força" (uma curva) |
| Hábito novo ganhando | `Curvas.Crescimento` |
| Hábito antigo perdendo | `Curvas.Declinio` |
| O dia em que o novo supera o antigo | `DiaDoCruzamento` |
| O período "em que o novo dá mais trabalho" | `DiasDeDesconforto` (`TakeWhile`) |
| Praticar acompanhado "encurta" o caminho | `ComParceria`: uma função que **recebe e devolve** uma função |
| Comparar dois caminhos lado a lado | **`Zip`** |
| Calcular só o necessário | Sequências **preguiçosas** (`yield`/LINQ) |

::: figura Duas curvas que se cruzam
<svg viewBox="0 0 740 250" role="img" aria-label="Curva do hábito antigo descendo, curva do hábito novo subindo, e um ponto de cruzamento por volta do dia 14, com o intervalo anterior marcado como período de desconforto">
  <line x1="50" y1="210" x2="700" y2="210" stroke="var(--ink-3)" stroke-width="1.5"/><line x1="50" y1="30" x2="50" y2="210" stroke="var(--ink-3)" stroke-width="1.5"/>
  <rect x="50" y="30" width="270" height="180" fill="var(--warn-soft)" opacity=".6"/><text x="185" y="50" text-anchor="middle" font-size="12" class="t-dim">desconforto: o novo ainda dá mais trabalho</text>
  <path d="M50 40 C170 70, 270 130, 700 200" fill="none" stroke="#d8454f" stroke-width="4"/>
  <path d="M50 210 C170 180, 270 120, 700 50" fill="none" stroke="var(--good)" stroke-width="4"/>
  <circle cx="320" cy="122" r="8" fill="var(--primary)"/><text x="330" y="112" font-size="12" font-weight="800">cruzamento (dia 14)</text>
  <text x="640" y="190" text-anchor="end" font-size="12" fill="#d8454f">hábito antigo</text><text x="640" y="78" text-anchor="end" font-size="12" fill="var(--good)">hábito novo</text>
  <text x="370" y="236" text-anchor="middle" font-size="12" class="t-dim">dias de prática →</text>
</svg>
:::

## 1) Funções como dados: o `Func<int, double>`

Até aqui, passamos **valores** para métodos. Um **delegate** permite passar **comportamento**: uma função inteira, como se fosse um dado. `Func<int, double>` quer dizer "uma função que recebe um `int` (o dia) e devolve um `double` (a força)".

Isso nos deixa descrever uma **curva** inteira em uma linha:

```cs Duas curvas em uma linha cada
Func<int, double> novo   = dia => 1 - Math.Pow(1 - 0.05, dia);   // sobe de 0 a 1
Func<int, double> antigo = dia => Math.Pow(1 - 0.05, dia);       // desce de 1 a 0
```

E, como funções são valores, podemos **criar funções a partir de funções**. `ComParceria` recebe uma curva e um fator e **devolve uma nova curva** em que "cada dia conta mais" (a parceria acelera o caminho). Funções que recebem ou devolvem funções chamam-se de **ordem superior**.

```cs Jornada/Habitos/Cap110/Reabituacao.cs
namespace Jornada.Habitos.Cap110;

/// <summary>
/// Uma curva é só uma função: dado o dia, devolve a força de um hábito (de 0 a 1).
/// Func&lt;int, double&gt; deixa passar "comportamento" como se fosse dado.
/// </summary>
public static class Curvas
{
    /// <summary>Hábito novo: começa em 0 e se aproxima de 1 a cada dia de prática.</summary>
    public static Func<int, double> Crescimento(double ganhoPorDia) =>
        dia => 1 - Math.Pow(1 - ganhoPorDia, dia);

    /// <summary>Hábito antigo: começa em 1 e perde força a cada dia em que se pratica o novo.</summary>
    public static Func<int, double> Declinio(double perdaPorDia) =>
        dia => Math.Pow(1 - perdaPorDia, dia);

    /// <summary>Praticar em grupo (ou com alguém que cobra com carinho) "encurta" o caminho: o dia conta mais.</summary>
    public static Func<int, double> ComParceria(Func<int, double> curva, double fator) =>
        dia => curva((int)Math.Round(dia * fator));
}

public sealed record Ponto(int Dia, double Antigo, double Novo)
{
    public bool NovoJaPassou => Novo > Antigo;
}

public sealed class Reabituacao
{
    private readonly Func<int, double> antigo;
    private readonly Func<int, double> novo;

    public Reabituacao(Func<int, double> antigo, Func<int, double> novo)
    {
        this.antigo = antigo;
        this.novo = novo;
    }

    /// <summary>Gera os pontos dia a dia. É "preguiçoso": só calcula o que for pedido.</summary>
    public IEnumerable<Ponto> Simular(int dias) =>
        Enumerable.Range(0, dias + 1).Select(d => new Ponto(d, antigo(d), novo(d)));

    /// <summary>Primeiro dia em que o hábito novo supera o antigo (null se não acontecer no prazo).</summary>
    public int? DiaDoCruzamento(int limiteDeDias) =>
        Simular(limiteDeDias).FirstOrDefault(p => p.NovoJaPassou)?.Dia;

    /// <summary>Quantos dias o novo ainda "dá mais trabalho" que o antigo: o período em que quase todo mundo desiste.</summary>
    public int DiasDeDesconforto(int limiteDeDias) =>
        Simular(limiteDeDias).TakeWhile(p => !p.NovoJaPassou).Count();

    /// <summary>Compara dois caminhos lado a lado, dia a dia, com Zip.</summary>
    public static IEnumerable<(int Dia, double Diferenca)> Comparar(Reabituacao a, Reabituacao b, int dias) =>
        a.Simular(dias).Zip(b.Simular(dias), (pa, pb) => (pa.Dia, pb.Novo - pa.Novo));
}
```

Leia as operações com calma, porque elas usam recursos do LINQ que valem o capítulo:

- **`Enumerable.Range(0, dias + 1)`** gera os números de 0 a `dias`, sem criar uma lista. **`Select`** transforma cada número em um `Ponto`. Tudo é **preguiçoso**: nada é calculado até alguém consumir.
- **`FirstOrDefault(p => p.NovoJaPassou)?.Dia`** acha o **primeiro** ponto em que o novo passou o antigo e devolve o **dia** dele (ou `null`, com o `?.`, se não achar).
- **`TakeWhile(p => !p.NovoJaPassou).Count()`** conta quantos pontos passam **antes** do cruzamento: o período de desconforto.
- **`Zip(a.Simular(...), b.Simular(...), (pa, pb) => ...)`** "fecha o zíper" de duas sequências, combinando o item 1 com o item 1, o 2 com o 2, e assim por diante: perfeito para comparar dois caminhos dia a dia.

```cs Uso
var sozinho = new Reabituacao(Curvas.Declinio(0.05), Curvas.Crescimento(0.05));

foreach (var p in sozinho.Simular(30).Where(p => p.Dia % 5 == 0))
    Console.WriteLine($"dia {p.Dia,2}: hábito antigo {p.Antigo:0.00} · hábito novo {p.Novo:0.00}");

var acompanhado = new Reabituacao(
    Curvas.ComParceria(Curvas.Declinio(0.05), 1.5),
    Curvas.ComParceria(Curvas.Crescimento(0.05), 1.5));

Console.WriteLine($"Sozinho, o novo passa o antigo no dia {sozinho.DiaDoCruzamento(100)}");
Console.WriteLine($"Com parceria, no dia {acompanhado.DiaDoCruzamento(100)}");
Console.WriteLine($"Dias em que o novo ainda dá mais trabalho: {sozinho.DiasDeDesconforto(100)}");
```

```saida
dia  0: hábito antigo 1,00 · hábito novo 0,00
dia  5: hábito antigo 0,77 · hábito novo 0,23
dia 10: hábito antigo 0,60 · hábito novo 0,40
dia 15: hábito antigo 0,46 · hábito novo 0,54
dia 20: hábito antigo 0,36 · hábito novo 0,64
dia 25: hábito antigo 0,28 · hábito novo 0,72
dia 30: hábito antigo 0,21 · hábito novo 0,79
Sozinho, o novo passa o antigo no dia 14
Com parceria, no dia 9
Dias em que o novo ainda dá mais trabalho: 14
```

A tabela da saída conta a história do capítulo: nos primeiros dias, o antigo **domina** (1,00 contra 0,00). Por volta do dia 14, as curvas se cruzam, e **até lá o novo "custa"**, que é justamente o trecho em que quase todo mundo desiste. Com parceria, o cruzamento chega no dia 9.

::: dica Por que a função é "preguiçosa"?
`Simular(1000)` **não** calcula 1000 pontos: devolve uma "receita". Quem chamar `.Take(3)` só vê três cálculos acontecerem (o teste `Simular_e_preguicoso...` prova isso contando as chamadas). Para sequências grandes ou infinitas, essa propriedade é uma salvação.
:::

## 2) Testes

```cs Habitos.Testes/Cap110Tests.cs
using Jornada.Habitos.Cap110;

namespace Habitos.Testes;

public class Cap110Tests
{
    [Fact]
    public void Curvas_comecam_e_terminam_onde_esperado()
    {
        var novo = Curvas.Crescimento(0.1);
        var antigo = Curvas.Declinio(0.1);

        Assert.Equal(0, novo(0), 6);
        Assert.Equal(1, antigo(0), 6);
        Assert.True(novo(60) > 0.99);
        Assert.True(antigo(60) < 0.01);
    }

    [Fact]
    public void O_novo_ultrapassa_o_antigo_quando_as_curvas_se_cruzam()
    {
        var r = new Reabituacao(Curvas.Declinio(0.05), Curvas.Crescimento(0.05));

        int? dia = r.DiaDoCruzamento(200);

        Assert.NotNull(dia);
        var ponto = r.Simular(200).First(p => p.Dia == dia);
        Assert.True(ponto.Novo > ponto.Antigo);
        Assert.False(r.Simular(200).First(p => p.Dia == dia - 1).NovoJaPassou);
    }

    [Fact]
    public void Dias_de_desconforto_e_o_tempo_ate_o_cruzamento()
    {
        var r = new Reabituacao(Curvas.Declinio(0.05), Curvas.Crescimento(0.05));
        Assert.Equal(r.DiaDoCruzamento(200), r.DiasDeDesconforto(200));
    }

    [Fact]
    public void Quando_o_novo_cresce_devagar_demais_nao_cruza_no_prazo()
    {
        var r = new Reabituacao(Curvas.Declinio(0.001), Curvas.Crescimento(0.01));
        Assert.Null(r.DiaDoCruzamento(20));
    }

    [Fact]
    public void Com_parceria_o_cruzamento_chega_antes()
    {
        var sozinho = new Reabituacao(Curvas.Declinio(0.04), Curvas.Crescimento(0.04));
        var acompanhado = new Reabituacao(Curvas.ComParceria(Curvas.Declinio(0.04), 1.5), Curvas.ComParceria(Curvas.Crescimento(0.04), 1.5));

        Assert.True(acompanhado.DiaDoCruzamento(200) < sozinho.DiaDoCruzamento(200));
    }

    [Fact]
    public void Comparar_usa_zip_e_mostra_a_vantagem_dia_a_dia()
    {
        var sozinho = new Reabituacao(Curvas.Declinio(0.04), Curvas.Crescimento(0.04));
        var acompanhado = new Reabituacao(Curvas.Declinio(0.04), Curvas.ComParceria(Curvas.Crescimento(0.04), 2));

        var diferencas = Reabituacao.Comparar(sozinho, acompanhado, 30).ToList();

        Assert.Equal(31, diferencas.Count);
        Assert.Equal(0, diferencas[0].Diferenca, 6);
        Assert.True(diferencas[10].Diferenca > 0);
    }

    [Fact]
    public void Simular_e_preguicoso_e_so_calcula_o_que_o_chamador_consome()
    {
        int chamadas = 0;
        var r = new Reabituacao(d => { chamadas++; return 1; }, _ => 0);

        _ = r.Simular(1000).Take(3).ToList();

        Assert.Equal(3, chamadas);
    }
}
```

Vale olhar o teste **`Com_parceria_o_cruzamento_chega_antes`**: ele afirma o comportamento do capítulo (parceria encurta o caminho) comparando dois caminhos, e não valores soltos. E **`Quando_o_novo_cresce_devagar_demais_nao_cruza_no_prazo`** mostra a outra face: se o ganho for pequeno demais, **dentro do prazo** o novo nunca passa o antigo. Isso é um bom retrato do desânimo, e também uma razão para dar **mais tempo** (ou companhia) a quem tenta mudar.

::: exercicio Uma pausa não apaga o caminho
Doença, viagem, uma semana ruim: a vida interrompe. Escreva o método de extensão `ComPausa(this Func<int, double> curva, int diaDoInicio, int diasDePausa)`, que devolve uma **nova curva** em que, durante a pausa, o "contador de dias" **fica parado**, e depois continua de onde parou. Teste que o cruzamento chega **depois** com a pausa, e que, **durante** a pausa, o valor não muda.

--- resposta
```cs Jornada/Habitos/Exercicios/Cap110Pausa.cs
namespace Jornada.Habitos.Cap110;

public static class CurvasExtensoes
{
    /// <summary>
    /// Uma pausa (doença, viagem, semana ruim) atrasa o caminho, mas não o apaga:
    /// durante a pausa o "contador de dias" fica parado e depois continua de onde parou.
    /// </summary>
    public static Func<int, double> ComPausa(this Func<int, double> curva, int diaDoInicio, int diasDePausa) =>
        dia => curva(dia switch
        {
            _ when dia < diaDoInicio                 => dia,
            _ when dia < diaDoInicio + diasDePausa   => diaDoInicio,
            _                                        => dia - diasDePausa
        });
}
```

O `switch` com **`_ when ...`** (um padrão com condição) escolhe entre três casos: antes da pausa, o dia vale ele mesmo; durante a pausa, o dia **fica congelado** no início; depois, **desconta** os dias parados. Note que um método de extensão sobre um **delegate** (`this Func<int, double> curva`) é possível e funciona como qualquer outro: `Curvas.Crescimento(0.05).ComPausa(5, 7)`. E o importante: **uma pausa atrasa, mas não apaga**.
:::

::: resumo
- Para o autor, **o que queremos é, em grande parte, aprendido**, e reaprender exige **prática** (inclusive do que ainda não se deseja), **companhia** e tempo; só informação não basta.
- **Hábitos antigos competem** com os novos; há um período de desconforto antes de o novo passar o antigo, e é aí que é mais fácil desistir.
- Um **delegate** (`Func<int, double>`) trata funções como dados; funções que **recebem e devolvem funções** (`ComParceria`, `ComPausa`) são de ordem superior.
- **Sequências preguiçosas** (`Range`, `Select`, `TakeWhile`, `Zip`) só calculam o que é consumido.
- **Pausas atrasam, não apagam**: o modelo ajuda a lembrar que recomeçar é parte do caminho.
:::

::: quiz
? O que o autor quer dizer com "o objeto da fome é aprendido"?
+ Que, embora a fome seja biológica, aquilo que desejamos comer é formado por hábito
- Que não sentimos fome naturalmente
- Que comer é um erro
- Que a fome é só psicológica
> A direção do apetite se aprende, e pode ser reaprendida.

? Segundo o capítulo, por que saber (ler, concordar) não basta para mudar um hábito?
+ Porque o desejo é formado por prática, e não só por convicções
- Porque o conhecimento é falso
- Porque ler é cansativo
- Porque os hábitos não mudam
> Mudanças de hábito pedem repetição e, de preferência, companhia.

? O que é `Func<int, double>`?
+ Um delegate: uma função que recebe um int e devolve um double
- Um tipo de número
- Uma lista de inteiros
- Uma classe abstrata
> Delegates permitem passar comportamento como dado.

? O que `Zip` faz com duas sequências?
+ Combina os itens de mesma posição, um com o outro
- Junta as duas em uma lista maior
- Remove os repetidos
- Ordena as duas
> Perfeito para comparar dois caminhos dia a dia.

? Por que `Simular` é "preguiçosa"?
+ Porque só calcula os pontos que forem consumidos
- Porque é mais lenta
- Porque usa um laço `for` visível
- Porque guarda tudo em memória
> Com `Take(3)`, apenas três cálculos acontecem.

? O que o capítulo diz sobre fazer uma pausa no caminho de mudança?
+ Atrasa, mas não apaga o que foi construído
- Zera tudo
- É sinal de fraqueza
- Impede definitivamente a mudança
> Recomeçar faz parte do processo.
:::

::: cartas
Querer querer :: Desejar desejar algo melhor; um primeiro passo que, sozinho, não basta.
Reabituação :: Substituir hábitos antigos por novos, com prática e, de preferência, em comunidade.
Delegate :: Tipo que representa uma função; permite passar comportamento como dado.
Função de ordem superior :: Função que recebe ou devolve outras funções.
Func<int, double> :: Delegate pronto: recebe um `int` e devolve um `double`.
Zip :: LINQ que combina duas sequências posição a posição.
TakeWhile :: LINQ que pega itens enquanto a condição for verdadeira.
Preguiçoso (lazy) :: Só calcula quando o resultado é de fato consumido.
:::
