---
id: agil-cap19
order: 35
kind: capitulo
num: 19
group: C# · Parte 8 · Projeto ágil e princípios SOLID
title: O que é projeto ágil?
subtitle: O código é o projeto. Os sete "maus cheiros" do software em decomposição, por que ele apodrece e como equipes ágeis o mantêm saudável.
minutes: 45
hue: 15
objectives:
- Entender que o **projeto de software** é, no fim, o **código-fonte**
- Reconhecer os **sete maus cheiros**: rigidez, fragilidade, imobilidade, viscosidade, complexidade e repetição desnecessárias, opacidade
- Explicar **por que o software apodrece** e por que "culpar a mudança" não resolve
- Descrever o ciclo ágil: **detectar** (práticas) → **diagnosticar** (princípios) → **resolver** (padrões)
origem: livros/02-projeto-agil-csharp/content/34-cap19.md
---

::: ilustracao img/agil/cap19.svg | Sinais de projeto podre: rigidez, fragilidade e imobilidade. O projeto ágil mantém a torre flexível.
:::

## O que é "projeto" de software?

Em 1992, o engenheiro Jack Reeves defendeu uma ideia que virou precursora do movimento ágil: **o projeto de um sistema de software é documentado, principalmente, pelo seu código-fonte**. Os diagramas são auxiliares; **o código é o projeto**.

Nas próximas páginas, "projeto" **não** quer dizer um conjunto de diagramas UML descolado do código. Ele é um conceito abstrato: a **forma e a estrutura** do programa inteiro **e** de cada módulo, classe e método. Diagramas podem representar partes dele, mas a materialização final é sempre o código.

::: regra O código-fonte é o projeto
Se o código está sujo, **o projeto está sujo** — não importa quão bonitos sejam os diagramas na parede.
:::

## Os maus cheiros do projeto

No começo de um projeto, o sistema é uma imagem clara na mente da equipe. Com sorte, a primeira versão reflete essa imagem. Depois algo dá errado: o software começa a **apodrecer**. Os remendos se acumulam até que mexer em qualquer coisa fica tão caro que a equipe pede "uma reescrita" — que raramente dá certo, porque o sistema antigo continua mudando enquanto o novo é feito (um alvo móvel), e o novo já nasce com verrugas.

Você sabe que o código está apodrecendo quando sente um destes **cheiros**:

::: figura Os sete cheiros em uma olhada
<svg viewBox="0 0 760 330" role="img" aria-label="Sete maus cheiros do projeto">
  <g text-anchor="middle" font-size="13">
    <rect class="f-bad" x="10" y="10" width="170" height="90" rx="12"/><text x="95" y="38" font-weight="800" class="t-bad">Rigidez</text><text x="95" y="60" font-size="12" class="t-dim">uma mudança simples</text><text x="95" y="78" font-size="12" class="t-dim">exige mexer em muitos módulos</text>
    <rect class="f-bad" x="195" y="10" width="170" height="90" rx="12"/><text x="280" y="38" font-weight="800" class="t-bad">Fragilidade</text><text x="280" y="60" font-size="12" class="t-dim">uma mudança quebra coisas</text><text x="280" y="78" font-size="12" class="t-dim">sem relação aparente</text>
    <rect class="f-bad" x="380" y="10" width="170" height="90" rx="12"/><text x="465" y="38" font-weight="800" class="t-bad">Imobilidade</text><text x="465" y="60" font-size="12" class="t-dim">partes úteis não podem ser</text><text x="465" y="78" font-size="12" class="t-dim">reaproveitadas</text>
    <rect class="f-bad" x="565" y="10" width="185" height="90" rx="12"/><text x="657" y="38" font-weight="800" class="t-bad">Viscosidade</text><text x="657" y="60" font-size="12" class="t-dim">fazer a coisa certa é mais</text><text x="657" y="78" font-size="12" class="t-dim">difícil que fazer gambiarra</text>
    <rect class="f-warn" x="10" y="125" width="235" height="90" rx="12"/><text x="127" y="153" font-weight="800" class="t-warn">Complexidade desnecessária</text><text x="127" y="175" font-size="12" class="t-dim">construções que ninguém usa,</text><text x="127" y="193" font-size="12" class="t-dim">feitas "para o futuro"</text>
    <rect class="f-warn" x="260" y="125" width="235" height="90" rx="12"/><text x="377" y="153" font-weight="800" class="t-warn">Repetição desnecessária</text><text x="377" y="175" font-size="12" class="t-dim">copiar e colar em vez</text><text x="377" y="193" font-size="12" class="t-dim">de criar uma abstração</text>
    <rect class="f-warn" x="510" y="125" width="240" height="90" rx="12"/><text x="630" y="153" font-weight="800" class="t-warn">Opacidade</text><text x="630" y="175" font-size="12" class="t-dim">o módulo é difícil de</text><text x="630" y="193" font-size="12" class="t-dim">entender, e só piora</text>
  </g>
  <text x="380" y="260" text-anchor="middle" font-size="14" class="t-dim">Os quatro primeiros pesam na mudança; os três últimos, na leitura e no excesso.</text>
</svg>
:::

### Rigidez
O software é **difícil de alterar**, mesmo de maneira simples. Uma única modificação provoca uma sucessão de outras em módulos dependentes. Quanto mais módulos mudam, mais rígido o projeto. O sintoma clássico: você estima *"1 hora"*, descobre consequências imprevistas e termina em um dia — *"foi mais complicado do que eu pensava!"*.

### Fragilidade
Uma mudança **estraga muitos lugares**, frequentemente em áreas sem relação conceitual com a alterada. Corrigir gera novos problemas e a equipe vira um cachorro correndo atrás do rabo. São os módulos que nunca saem da lista de bugs: quanto mais você os conserta, pior ficam.

### Imobilidade
O projeto tem partes que **poderiam ser úteis em outros sistemas**, mas o trabalho e o risco de separá-las são grandes demais. Acontece muito: ninguém consegue "extrair só o módulo de relatórios" porque ele está emaranhado com o resto.

### Viscosidade
Duas formas:

- **do software:** quando as maneiras que **preservam** o projeto são mais difíceis do que as **gambiarras**, é fácil fazer a coisa errada e difícil fazer a certa;
- **do ambiente:** compilação lenta, controle de versão que demora horas… e os desenvolvedores passam a evitar mudanças que exijam muita recompilação, mesmo que estraguem o projeto.

### Complexidade desnecessária
O projeto contém elementos **que não são úteis no momento**: surgem quando os desenvolvedores antecipam mudanças e "preparam" o código para elas. Parece prudente, mas o efeito costuma ser o oposto — o sistema fica pesado e difícil de entender, carregando estruturas nunca usadas.

### Repetição desnecessária
Copiar e colar é ótimo para texto e **desastroso para código**. Cada cópia é ligeiramente diferente; um bug achado numa delas precisa ser corrigido em todas — e a correção nem sempre é igual. Quando o mesmo trecho aparece muitas vezes, **falta uma abstração**.

### Opacidade
O módulo é **difícil de entender**. O código tende a ficar mais opaco com o tempo, a menos que haja esforço constante. Quem escreve acha tudo claro (afinal, conhece cada detalhe); seis meses depois pergunta *"como pude escrever isso?"*. A cura: **pôr-se no lugar do leitor**, refatorar e fazer **revisão de código**.

## Por que o software apodrece

Em ambientes não ágeis, a mudança de requisitos chega de forma **não prevista** no projeto inicial, geralmente com pressa e por quem **não conhece a filosofia original**. A alteração funciona, mas viola o projeto; com o tempo as violações se acumulam até a decomposição.

Dá vontade de **culpar os requisitos**. Mas todos sabemos que os requisitos são o elemento **mais volátil** de qualquer projeto: mudam sempre. Se nosso projeto fracassa por causa da mudança, **o projeto e as práticas estão errados**. Nosso trabalho é garantir que o software **sobreviva** à mudança.

::: pense "O programa foi bem projetado para a especificação original"
Essa frase é o lamento de quem descobre que o mundo mudou. Um projeto que só funciona **enquanto nada muda** não é um bom projeto — é um projeto frágil. Equipes ágeis aceitam a mudança como **dado do problema**.
:::

## Uma história que acontece o tempo todo: o exportador

Vamos ver o apodrecimento em câmera lenta (exemplo nosso, inspirado no capítulo). Na segunda-feira, seu chefe pede um programa que **leia números de uma fonte e os grave na tela**. Simples:

```cs Versão 1
public class Relatorio
{
    public static void Gerar()
    {
        string linha;
        while ((linha = Console.ReadLine()) != null)
            Console.WriteLine(linha.ToUpper());
    }
}
```

Elegante: 6 linhas. Meses depois: *"agora precisa ler de um arquivo, às vezes."* Muitos programas já chamam `Gerar()`, então você não quer mudar a assinatura. A saída "rápida": uma variável **global** e um operador ternário.

```cs Versão 2 — o primeiro remendo
public class Relatorio
{
    // lembrar de zerar este flag depois!
    public static bool LerDeArquivo = false;

    public static void Gerar()
    {
        string linha;
        while ((linha = (LerDeArquivo ? ArquivoEntrada.Ler() : Console.ReadLine())) != null)
            Console.WriteLine(linha.ToUpper());
    }
}
```

Semanas depois: *"e às vezes gravar em arquivo."* Outro flag, outro ternário:

```cs Versão 3 — o segundo remendo
public static bool LerDeArquivo = false;
public static bool GravarEmArquivo = false;   // lembrar de zerar!

while ((linha = (LerDeArquivo ? ArquivoEntrada.Ler() : Console.ReadLine())) != null)
{
    if (GravarEmArquivo) ArquivoSaida.Gravar(linha.ToUpper());
    else Console.WriteLine(linha.ToUpper());
}
```

Em duas mudanças já estão presentes **todos os cheiros**: rígido (qualquer fonte nova mexe no laço), frágil (esquecer de zerar o flag faz **outro** chamador ler do arquivo), imóvel, viscoso (a gambiarra é mais fácil que a solução), complexo e opaco. Daqui para a frente a trajetória é conhecida.

### A resposta ágil

Uma equipe ágil começa **exatamente igual** (versão 1, a mais simples). Mas quando chega o primeiro pedido de mudança, em vez de "fazer caber", ela **aproveita a oportunidade para melhorar o projeto**, de modo a ficar flexível *àquele tipo de mudança*:

```cs Versão ágil
public interface IFonteDeLinhas { string LerLinha(); }

public class FonteConsole : IFonteDeLinhas { public string LerLinha() { return Console.ReadLine(); } }
public class FonteArquivo : IFonteDeLinhas
{
    private readonly StreamReader leitor;
    public FonteArquivo(string caminho) { leitor = new StreamReader(caminho); }
    public string LerLinha() { return leitor.ReadLine(); }
}

public class Relatorio
{
    private readonly IFonteDeLinhas fonte;
    public Relatorio(IFonteDeLinhas fonte) { this.fonte = fonte; }

    public void Gerar()
    {
        string linha;
        while ((linha = fonte.LerLinha()) != null)
            Console.WriteLine(linha.ToUpper());
    }
}
```

Agora um **novo tipo de fonte** (um serviço web, um banco…) é **uma classe nova** — `Relatorio` **não muda**. Mas note o detalhe: a equipe protegeu-se **só contra a mudança que aconteceu** (fontes de entrada). Não adivinhou que a **saída** mudaria. Se mudar, haverá tempo de abstraí-la: *proteger-se antes não ajudaria* (e talvez apontasse na direção errada).

::: figura Antes e depois: a direção das dependências
<svg viewBox="0 0 740 230" role="img" aria-label="Relatório depende diretamente de console e arquivo; depois depende de uma interface">
  <text x="170" y="20" text-anchor="middle" font-weight="800" class="t-bad">Rígido: política depende dos detalhes</text>
  <rect class="f-prim" x="90" y="35" width="160" height="46" rx="10"/><text x="170" y="64" text-anchor="middle" font-weight="700">Relatorio</text>
  <path class="arrow" d="M120 82V130" style="stroke:var(--bad);marker-end:url(#arr3)"/><path class="arrow" d="M220 82V130" style="stroke:var(--bad);marker-end:url(#arr3)"/>
  <rect class="f-bad" x="40" y="132" width="110" height="46" rx="8"/><text x="95" y="160" text-anchor="middle" font-size="13">Console</text>
  <rect class="f-bad" x="190" y="132" width="110" height="46" rx="8"/><text x="245" y="160" text-anchor="middle" font-size="13">Arquivo</text>
  <text x="560" y="20" text-anchor="middle" font-weight="800" class="t-good">Flexível: tudo depende da abstração</text>
  <rect class="f-prim" x="480" y="35" width="160" height="46" rx="10"/><text x="560" y="64" text-anchor="middle" font-weight="700">Relatorio</text>
  <path class="arrow" d="M560 82V112"/>
  <rect class="f-info" x="470" y="114" width="180" height="40" rx="8"/><text x="560" y="139" text-anchor="middle" font-size="13">«interface» IFonteDeLinhas</text>
  <path class="line" d="M520 190V156M600 190V156" style="marker-end:none"/>
  <rect class="f-good" x="450" y="190" width="100" height="38" rx="8"/><text x="500" y="214" text-anchor="middle" font-size="13">Console</text>
  <rect class="f-good" x="580" y="190" width="100" height="38" rx="8"/><text x="630" y="214" text-anchor="middle" font-size="13">Arquivo</text>
</svg>
:::

O problema da versão rígida era a **direção das dependências**: `Relatorio` (política de alto nível) dependia diretamente dos detalhes de baixo nível (console, arquivo). Quando os detalhes mudam, a política é arrastada. A solução usou o **Princípio de Inversão de Dependência** (capítulo 23) por meio do padrão **Strategy** (capítulo 30), e deixou o módulo **aberto para extensão, fechado para modificação** (**OCP**, capítulo 21).

::: regra Detectar → diagnosticar → resolver
1. **Detectar** o problema seguindo as **práticas ágeis** (o teste/a mudança mostrou a rigidez).
2. **Diagnosticar** aplicando **princípios de projeto** (o que está errado nas dependências?).
3. **Resolver** com um **padrão de projeto** apropriado (Strategy, Factory, Observer…).

A interação entre práticas, princípios e padrões **é** o ato de projetar.
:::

## Manter o projeto o melhor possível, o tempo todo

Equipes ágeis **não** "limpam o projeto a cada poucas semanas": mantêm o software **limpo, simples e expressivo a toda hora**. Nunca dizem *"corrigimos isso depois"*. Pense num cirurgião e a esterilização: ela é o que **torna a cirurgia possível**; sem ela o risco de infecção seria intolerável. O mesmo vale para a decomposição de código: **o risco de permitir que ela comece é alto demais**. Profissionalismo é **não tolerar** a podridão do código.

::: dica Quando aplicar os princípios?
Não "de antemão", num grande projeto inicial. Eles são aplicados **de iteração em iteração**, quando as práticas (testes, refatoração, feedback) sinalizam um cheiro. **Projeto ágil é um processo, não uma circunstância.**
:::

::: exercicio Diagnostique os cheiros
Para cada situação, diga o cheiro principal:

1. Para mudar o formato da data do relatório você precisa editar 14 classes.
2. Ao corrigir um bug no módulo de *login*, o módulo de *relatórios* passou a falhar.
3. O compilador leva 40 minutos, então a equipe evita "mexer em classes base" mesmo quando seria o certo.
4. Existe uma fábrica genérica de fábricas, usada em um único lugar.
5. A lógica de imposto está copiada em seis telas.

--- resposta
1. **Rigidez**. 2. **Fragilidade**. 3. **Viscosidade (do ambiente)**. 4. **Complexidade desnecessária**. 5. **Repetição desnecessária** (falta uma abstração: uma classe/método de cálculo de imposto).
:::

::: resumo
- O **código-fonte é o projeto**; diagramas são só auxiliares.
- Sete cheiros: **rigidez, fragilidade, imobilidade, viscosidade, complexidade desnecessária, repetição desnecessária, opacidade**.
- O software apodrece porque as mudanças **violam** o projeto original; **requisitos sempre mudam**, então o projeto deve resistir a isso.
- Equipes ágeis começam **simples** e **melhoram o projeto na primeira mudança** do tipo, sem adivinhar o futuro.
- Ciclo: **detectar** (práticas) → **diagnosticar** (princípios) → **resolver** (padrões).
- Mantenha o projeto limpo **todo dia**, não "depois".
:::

::: quiz
? Segundo o capítulo, onde está, em última instância, o projeto de um software?
+ No código-fonte
- Nos diagramas UML
- No documento de requisitos
- Na cabeça do arquiteto
> Os diagramas representam o projeto; o código o materializa.

? Qual cheiro descreve "uma única mudança quebra vários lugares sem relação conceitual"?
+ Fragilidade
- Rigidez
- Opacidade
- Imobilidade
> Na rigidez muda-se muita coisa; na fragilidade, quebra-se muita coisa.

? O que é viscosidade do projeto?
+ Quando é mais fácil fazer uma gambiarra do que preservar o projeto
- Quando o sistema é muito lento
- Quando há muitos comentários
- Quando o código é muito curto
> Queremos que a mudança correta seja a mais fácil.

? Por que a equipe ágil não "prepara" o código para mudanças futuras que ainda não aconteceram?
+ Porque isso gera complexidade desnecessária e pode apontar para a direção errada
- Porque é proibido pelo contrato
- Porque o compilador não permite
- Porque ela não sabe programar interfaces
> Reage-se à primeira mudança real, tornando o projeto flexível àquele tipo de mudança.

? Qual é a causa raiz do apodrecimento segundo o capítulo?
+ Mudanças feitas sem respeitar o projeto original, somadas a um projeto que não resiste à mudança
- O excesso de testes
- Linguagens modernas
- Clientes que pedem demais
> Culpar os requisitos não resolve: eles sempre mudam.

? O que o código em `Relatorio(IFonteDeLinhas fonte)` ilustra?
+ A inversão da dependência: a política depende de uma abstração, não de detalhes
- A criação de uma variável global
- O uso de herança múltipla
- A cópia e cola de código
> Novas fontes viram novas classes sem alterar `Relatorio`.
:::

::: cartas
Rigidez :: Mudança simples exige muitas alterações em cascata.
Fragilidade :: Uma mudança quebra coisas sem relação aparente.
Imobilidade :: Partes úteis não conseguem ser reaproveitadas.
Viscosidade :: É mais fácil fazer a gambiarra do que preservar o projeto.
Complexidade desnecessária :: Estruturas "para o futuro" que ninguém usa.
Repetição desnecessária :: Código copiado e colado; falta abstração.
Opacidade :: Módulo difícil de entender.
Projeto ágil :: Processo contínuo de aplicar princípios, padrões e práticas para manter o código limpo.
Detectar–diagnosticar–resolver :: Práticas detectam, princípios diagnosticam, padrões resolvem.
:::
