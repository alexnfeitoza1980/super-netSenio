---
id: agil-cap25
order: 41
kind: capitulo
num: 25
group: C# · Parte 9 · UML na prática
title: UML para programadores C#: visão geral e uso com bom senso
subtitle: Os níveis e tipos de diagramas, um giro rápido pelos principais e a política de quando desenhar (e quando parar).
minutes: 55
hue: 230
objectives:
- Distinguir os **níveis** conceitual, de especificação e de implementação
- Ler um **diagrama de classes, de objetos, de sequência, de colaboração e de estados**
- Saber **quando e por que** desenhar diagramas — e quando **não**
- Refinar diagramas **em passos curtos**, alternando comportamento (dinâmico) e estrutura (estático)
origem: livros/02-projeto-agil-csharp/content/40-cap25.md
---

::: ilustracao img/agil/cap25.svg | UML com bom senso: desenhe o que ajuda a conversar e jogue fora o resto.
:::

## O que é a UML

A **UML** (*Unified Modeling Language*) é uma **notação gráfica** para desenhar conceitos de software. Dá para desenhar com ela: um **domínio de problema**, um **projeto proposto** ou uma **implementação já pronta**. Martin Fowler descreve esses três usos como níveis:

| Nível | Do que trata | Relação com o código |
|---|---|---|
| **Conceitual** | O mundo do problema (linguagem humana) | Quase nenhuma; é abreviatura de ideias, **sujeita a interpretação** |
| **Especificação** | Um projeto de software proposto | Forte: o diagrama **vira código** |
| **Implementação** | Código que já existe | Forte: o diagrama **descreve código** |

::: figura Um mesmo desenho, dois significados
<svg viewBox="0 0 740 190" role="img" aria-label="Animal e Cachorro: conceitual × código">
  <g text-anchor="middle"><rect class="f-prim" x="40" y="20" width="130" height="40" rx="6"/><text x="105" y="46" font-weight="800">Animal</text>
  <path class="line" d="M105 62V100" /><path d="M95 100L105 82L115 100Z" class="f-plain"/>
  <rect class="f-info" x="40" y="104" width="130" height="40" rx="6"/><text x="105" y="130" font-weight="800">Cachorro</text></g>
  <text x="210" y="60" font-size="13" class="t-dim">Conceitual: "um cachorro é um animal"</text><text x="210" y="80" font-size="13" class="t-dim">(a espécie? o Rex? ninguém sabe — é interpretação)</text>
  <rect class="f-plain" x="210" y="100" width="500" height="68" rx="8"/>
  <text class="t-mono" x="226" y="128" font-size="14">public class Animal { }</text><text class="t-mono" x="226" y="152" font-size="14">public class Cachorro : Animal { }</text>
  <text x="460" y="96" text-anchor="middle" font-size="12" class="t-dim">Especificação/implementação: significado preciso</text>
</svg>
:::

Infelizmente o desenho **não diz em que nível foi feito** — fonte comum de falhas de comunicação entre analistas e programadores. Um diagrama conceitual **não define código**, nem deveria. Nesta parte do livro todos os diagramas são de **especificação/implementação** e vêm acompanhados do código correspondente.

### Três famílias de diagramas

- **Estáticos:** estrutura lógica que não muda — classes, objetos, estruturas de dados e relações.
- **Dinâmicos:** como as entidades **mudam durante a execução** — fluxo de execução ou mudanças de estado.
- **Físicos:** estrutura **física** — arquivos de código, bibliotecas, binários, arquivos de dados e suas relações.

## Um giro pelos principais diagramas

Vamos usar um código pequeno: um **dicionário em árvore binária**, escrito por nós.

```cs MapaArvore.cs
public class MapaArvore
{
    private No topo;

    public void Adicionar(IComparable chave, object valor)
    {
        if (topo == null) topo = new No(chave, valor);
        else topo.Adicionar(chave, valor);
    }

    public object Obter(IComparable chave)
    {
        return topo == null ? null : topo.Buscar(chave);
    }
}

internal class No
{
    private const int Menor = 0, Maior = 1;
    private readonly IComparable chave;
    private object valor;
    private readonly No[] filhos = new No[2];

    public No(IComparable chave, object valor) { this.chave = chave; this.valor = valor; }

    public void Adicionar(IComparable c, object v)
    {
        if (c.CompareTo(chave) == 0) { valor = v; return; }
        int lado = c.CompareTo(chave) < 0 ? Menor : Maior;
        if (filhos[lado] == null) filhos[lado] = new No(c, v);
        else filhos[lado].Adicionar(c, v);
    }

    public object Buscar(IComparable c)
    {
        if (c.CompareTo(chave) == 0) return valor;
        int lado = c.CompareTo(chave) < 0 ? Menor : Maior;
        return filhos[lado] == null ? null : filhos[lado].Buscar(c);
    }
}
```

### Diagrama de classes (estático)

Mostra as **classes** e seus **relacionamentos**.

::: figura Diagrama de classes de MapaArvore
<svg viewBox="0 0 760 260" role="img" aria-label="MapaArvore tem topo: No; No tem filhos: No (2) e chave: IComparable">
  <rect class="f-plain" x="20" y="30" width="190" height="110" rx="4"/><rect class="f-prim" x="20" y="30" width="190" height="34" rx="4"/><text x="115" y="53" text-anchor="middle" font-weight="800">MapaArvore</text>
  <path class="line" d="M20 64H210M20 104H210"/><text class="t-mono" x="30" y="86" font-size="12">- topo : No</text><text class="t-mono" x="30" y="124" font-size="12">+ Adicionar(chave, valor)</text><text class="t-mono" x="30" y="138" font-size="12">+ Obter(chave)</text>
  <rect class="f-plain" x="330" y="30" width="210" height="150" rx="4"/><rect class="f-info" x="330" y="30" width="210" height="34" rx="4"/><text x="435" y="53" text-anchor="middle" font-weight="800">No</text>
  <path class="line" d="M330 64H540M330 124H540"/><text class="t-mono" x="340" y="82" font-size="12">- valor : object</text><text class="t-mono" x="340" y="98" font-size="12">- filhos : No[2]</text><text class="t-mono" x="340" y="114" font-size="12">- chave : IComparable</text><text class="t-mono" x="340" y="146" font-size="12">+ Adicionar(chave, valor)</text><text class="t-mono" x="340" y="164" font-size="12">+ Buscar(chave)</text>
  <path class="arrow" d="M212 85H328"/><text x="270" y="78" text-anchor="middle" font-size="12">topo</text><text x="318" y="80" font-size="11" class="t-dim">0..1</text>
  <path class="arrow" d="M540 100C600 100 600 40 545 44"/><text x="610" y="70" font-size="12">filhos</text><text x="560" y="40" font-size="11" class="t-dim">0..2</text>
  <rect class="f-plain" x="590" y="130" width="150" height="50" rx="4"/><text x="665" y="150" text-anchor="middle" font-size="11" class="t-dim">«interface»</text><text x="665" y="168" text-anchor="middle" font-weight="700">IComparable</text>
  <path class="arrow" d="M542 150H588"/><text x="565" y="143" text-anchor="middle" font-size="11">chave</text>
</svg>
:::

Como ler:

- **Retângulos** são classes; **setas**, relacionamentos.
- Aqui todos são **associações**: um objeto **guarda uma referência** a outro e chama seus métodos. O **nome** da associação (`topo`, `filhos`, `chave`) é o **nome da variável** que guarda a referência.
- O **número** junto à seta (`0..2`) é a multiplicidade; maior que 1 implica um contêiner (array, lista).
- O retângulo tem **compartimentos**: nome; (opcional) campos; (opcional) métodos. `+` público, `-` privado.
- `«interface»` marca uma interface. **Quase tudo é opcional.**

### Diagrama de objetos (estático, uma "foto")

Mostra **objetos concretos** e os **links** entre eles **em um instante** da execução — um instantâneo da memória. O nome fica **sublinhado** no formato `nome : Classe`.

::: figura Uma árvore com cinco chaves
<svg viewBox="0 0 720 270" role="img" aria-label="Árvore binária de objetos No">
  <g text-anchor="middle" font-size="12">
    <rect class="f-prim" x="10" y="20" width="110" height="40" rx="4"/><text x="65" y="45" text-decoration="underline">: MapaArvore</text>
    <rect class="f-info" x="190" y="20" width="130" height="50" rx="4"/><text x="255" y="40" text-decoration="underline">: No</text><text x="255" y="58" class="t-mono">chave = "Mario"</text>
    <rect class="f-info" x="90" y="120" width="130" height="50" rx="4"/><text x="155" y="140" text-decoration="underline">: No</text><text x="155" y="158" class="t-mono">chave = "Ana"</text>
    <rect class="f-info" x="290" y="120" width="130" height="50" rx="4"/><text x="355" y="140" text-decoration="underline">: No</text><text x="355" y="158" class="t-mono">chave = "Rita"</text>
    <rect class="f-info" x="10" y="215" width="130" height="45" rx="4"/><text x="75" y="235" text-decoration="underline">: No</text><text x="75" y="252" class="t-mono">chave = "Alan"</text>
    <rect class="f-info" x="360" y="215" width="130" height="45" rx="4"/><text x="425" y="235" text-decoration="underline">: No</text><text x="425" y="252" class="t-mono">chave = "Sam"</text>
  </g>
  <path class="arrow" d="M122 40H188"/><text x="155" y="33" text-anchor="middle" font-size="11">topo</text>
  <path class="arrow" d="M215 72L175 118"/><text x="170" y="95" font-size="11">filhos[MENOR]</text>
  <path class="arrow" d="M290 72L335 118"/><text x="340" y="95" font-size="11">filhos[MAIOR]</text>
  <path class="arrow" d="M120 172L85 213"/><path class="arrow" d="M390 172L415 213"/>
</svg>
:::

### Diagrama de sequência (dinâmico)

Mostra **a ordem das mensagens** entre objetos, de cima para baixo. As **caixas** no topo são objetos; as linhas verticais, suas **linhas de vida**; as setas, **chamadas**; a faixa fina sobre a linha de vida é a **ativação** (enquanto o método executa). Entre colchetes, **guardas** (condições).

::: figura MapaArvore.Adicionar
<svg viewBox="0 0 700 300" role="img" aria-label="Sequência: chamador chama Adicionar; se topo é nulo cria No; senão delega ao No">
  <g text-anchor="middle" font-size="13">
    <rect class="f-prim" x="130" y="10" width="120" height="34" rx="4"/><text x="190" y="32" text-decoration="underline">: MapaArvore</text>
    <rect class="f-info" x="450" y="10" width="130" height="34" rx="4"/><text x="515" y="32" text-decoration="underline">topo : No</text>
  </g>
  <path d="M190 46V280M515 46V280" stroke="var(--ink-3)" stroke-dasharray="5 4"/>
  <text x="40" y="82" font-size="26">🧍</text>
  <path class="arrow" d="M70 95H186"/><text x="128" y="88" text-anchor="middle" font-size="12" class="t-mono">Adicionar(c, v)</text>
  <rect class="f-prim" x="184" y="95" width="12" height="165"/>
  <text x="200" y="130" font-size="12" class="t-mono t-bad">[topo == null]</text>
  <path class="arrow" d="M198 148H470"/><text x="335" y="141" text-anchor="middle" font-size="12" class="t-mono">«cria» No(c, v)</text>
  <rect class="f-info" x="470" y="140" width="90" height="20" rx="3" opacity=".0"/>
  <text x="200" y="196" font-size="12" class="t-mono t-good">[topo != null]</text>
  <path class="arrow" d="M198 214H509"/><text x="350" y="207" text-anchor="middle" font-size="12" class="t-mono">Adicionar(c, v)</text>
  <rect class="f-info" x="509" y="214" width="12" height="30"/>
</svg>
:::

### Diagrama de colaboração/comunicação (dinâmico)

Contém **a mesma informação** que o de sequência, mas dá ênfase aos **relacionamentos entre os objetos** (os *links*) em vez da **ordem** no tempo. As mensagens são numeradas hierarquicamente: `1` chama `1.1`, que chama `1.1.1`… Existe um *link* sempre que um objeto pode enviar mensagem a outro.

### Diagrama de estados (dinâmico)

Descreve uma **máquina de estados finitos**. Cada seta é uma **transição**, rotulada com `evento / ação`. Excelente para descobrir **como o sistema se comporta em casos inesperados** — por exemplo, o que acontece se alguém insere duas moedas seguidas?

::: figura Catraca de ônibus
<svg viewBox="0 0 680 220" role="img" aria-label="Estados Travada e Livre; eventos moeda e passagem">
  <g text-anchor="middle" font-size="14"><rect class="f-bad" x="60" y="70" width="140" height="64" rx="32"/><text x="130" y="108" font-weight="800" class="t-bad">Travada</text>
  <rect class="f-good" x="440" y="70" width="140" height="64" rx="32"/><text x="510" y="108" font-weight="800" class="t-good">Livre</text></g>
  <path class="arrow" d="M200 90C290 50 350 50 438 88"/><text x="320" y="55" text-anchor="middle" font-size="12" class="t-mono">moeda / Destravar</text>
  <path class="arrow" d="M440 118C350 160 290 160 202 120"/><text x="320" y="170" text-anchor="middle" font-size="12" class="t-mono">passagem / Travar</text>
  <path class="arrow" d="M100 72C60 20 160 20 150 70"/><text x="105" y="22" text-anchor="middle" font-size="11" class="t-mono">passagem / Alarme</text>
  <path class="arrow" d="M480 72C440 20 540 20 530 70"/><text x="500" y="22" text-anchor="middle" font-size="11" class="t-mono">moeda / Agradecer</text>
</svg>
:::

Em português: *se Travada e recebe moeda → vai para Livre e destrava; se Livre e recebe passagem → vai para Travada e trava; se Livre e recebe outra moeda → permanece Livre e agradece; se Travada e recebe passagem → permanece Travada e toca o alarme.* A maioria dos programadores sobrevive bem com esse conjunto de diagramas.

## Por que modelar? (e por que o software é diferente)

Engenheiros aeroespaciais e estruturais constroem **modelos** de aviões e pontes para **descobrir se vão funcionar**: modelos são **testáveis** (túnel de vento, simulação) e **muito mais baratos** que a coisa real. Arquitetos desenham plantas porque **uma pessoa** pode planejar o que exigirá dezenas de construtores, e jogar fora uma planta é barato comparado a demolir uma casa.

No software, **nada disso é tão claro**:

- Não há **critérios sólidos** para "testar" um diagrama — só avaliação subjetiva (princípios, padrões, revisão).
- Desenhar diagramas **nem sempre é muito mais barato** que escrever código; às vezes é mais fácil mudar o código que o desenho.
- Jogar fora um diagrama **não custa tão menos** que jogar fora código.

Conclusão: usamos UML **quando precisamos testar uma ideia** e **desenhar é mais barato que codificar**: por exemplo, para saber se os colegas acham boa uma ideia — desenha-se no quadro e pergunta-se.

::: regra Diagramas servem para comunicar e raciocinar
Use **só** o nível de detalhe necessário ao objetivo. Um diagrama cheio de adornos é contraproducente. **UML não é código-fonte** e não é lugar de declarar cada método, variável e relacionamento.
:::

## Para que a UML serve bem (e mal)

**Serve bem:**

- **Comunicar ideias de projeto específicas** numa conversa diante do quadro: "`LoginPage` herda de `Page` e usa `UserDatabase`" fica óbvio num desenho simples.
- **Mapas/roteiros** de grandes estruturas: quem depende de quem, que objeto contém quais. Uma estrutura que seria maçante de descobrir no código vira trivial num diagrama. Mas qualquer membro da equipe deveria conseguir **redesenhá-lo no quadro de memória** — por isso guardá-lo com tanto cuidado faz pouco sentido.
- **Documentação final**: o melhor momento para criar um documento de projeto **que será guardado** é o **fim do projeto**, pois reflete o último estado real.

**Serve mal:**

- **Detalhes algorítmicos**. Um simples *bubble sort* em UML sai desajeitado e não mais legível que o código: o código é melhor.

::: cuidado Menos é mais — mesmo na documentação final
Não queremos milhares de páginas de diagramas de sequência: queremos **poucos diagramas importantes** que descrevam as grandes questões. Ainda assim, **nenhum diagrama é pior** que um **confuso**, cheio de linhas e caixas.
:::

## O que manter e o que jogar fora

- **Aprenda a jogar diagramas fora.** Melhor: crie-os em meio **efêmero** — **quadro branco** e folhas de papel. Apague o quadro com frequência.
- Não use uma ferramenta CASE/desenho como regra. A maioria dos diagramas deve ter **vida curta**.
- **Guarde** apenas os que: expressam uma **solução de projeto comum** no sistema; registram **protocolos complexos** difíceis de ver no código; são **roteiros** para áreas pouco tocadas; registram **a intenção** do projetista melhor que o código.
- Você **reconhece** os diagramas que merecem guarda: eles **aparecem várias vezes** em sessões diferentes. Quando alguém copia um desenho "só para não ter que desenhar de novo", é hora de guardá-lo num lugar comum (wiki/base de conhecimento) — e **sem deixar acumular centenas deles**.

## Refinamento iterativo: como criar diagramas úteis

Não se desenha tudo "num momento de inspiração", nem toda a estrutura antes dos detalhes. **Tudo que os humanos fazem bem, fazem em passos minúsculos, avaliando o resultado.** Para diagramas:

1. **Comece pelo comportamento:** desenhe um diagrama de sequência/colaboração simples do problema.
2. **Veja o que isso implica na estrutura:** derive um diagrama de classes (uma classe por objeto, uma associação por link).
3. **Analise as dependências** à luz dos princípios (SRP, OCP, DIP…) e **altere a estrutura**.
4. **Volte e atualize o diagrama dinâmico.** Repita.

Cada passo dura **no máximo uns 5 minutos**. O objetivo **não** é acertar numeração e notação; é que **todos diante do quadro entendam** a discussão — e então **parar e escrever código**.

### Um exemplo: controle remoto e televisão

Pensemos num software de **controle remoto**. *Como a TV muda de canal quando o usuário aperta um número?*

::: passos Refinando comportamento e estrutura
**1. Comportamento (dinâmico).** Inventamos objetos à medida que precisamos: um `Botao` envia a mensagem `digito(n)` para um `Sintonizador`; este manda `mostrarCanal(n)` ao `Visor` e `trocar(n)` ao `Receptor`.

::: html
<svg viewBox="0 0 700 110"><g text-anchor="middle" font-size="12"><rect class="f-prim" x="5" y="30" width="100" height="40" rx="4"/><text x="55" y="55" text-decoration="underline">: Botao</text><rect class="f-info" x="190" y="30" width="120" height="40" rx="4"/><text x="250" y="55" text-decoration="underline">: Sintonizador</text><rect class="f-good" x="400" y="5" width="90" height="36" rx="4"/><text x="445" y="28" text-decoration="underline">: Visor</text><rect class="f-warn" x="400" y="62" width="100" height="36" rx="4"/><text x="450" y="85" text-decoration="underline">: Receptor</text></g><path class="arrow" d="M107 50H188"/><text x="148" y="42" font-size="11" class="t-mono">1*: digito(n)</text><path class="arrow" d="M312 42L398 24"/><text x="360" y="22" font-size="11" class="t-mono">1.1: mostrarCanal(n)</text><path class="arrow" d="M312 58L398 78"/><text x="368" y="96" font-size="11" class="t-mono">1.2: trocar(n)</text></svg>
:::
===
**2. Estrutura (estático).** Uma classe por objeto, uma associação por link. Mas olhe as **dependências**: `Botao` depende de `Sintonizador`! No código:

```cs
public class Botao
{
    private readonly Sintonizador sintonizador;       // 🚫 Botao só serve para sintonizar
    public Botao(Sintonizador s) { sintonizador = s; }
}
```

Não queremos reaproveitar `Botao` também para **ligar/desligar** e **volume**? Se ele conhece `Sintonizador`, não dá.
===
**3. Aplique o DIP.** Insira uma **interface** entre os dois (`IOuvinteDeBotao`), de posse de `Botao`:

```cs
public interface IOuvinteDeBotao { void BotaoPressionado(int token); }
```

`Botao` agora avisa "fui apertado (token 7)" sem saber quem ouve. Mas… agora o `Sintonizador` sabe algo sobre botões! Por que ele implementaria `BotaoPressionado`? Nada a ver.
===
**4. Aplique um adaptador.** Crie `AdaptadorBotaoSintonizador : IOuvinteDeBotao` que guarda o dígito, recebe `BotaoPressionado()` e chama `sintonizador.Digito(digito)`:

```cs
public class AdaptadorBotaoSintonizador : IOuvinteDeBotao
{
    private readonly int digito;
    private readonly Sintonizador sintonizador;
    public AdaptadorBotaoSintonizador(int digito, Sintonizador s) { this.digito = digito; sintonizador = s; }
    public void BotaoPressionado() { sintonizador.Digito(digito); }
}
```

`Botao` e `Sintonizador` ficam **independentes**. 
===
**5. Atualize o diagrama dinâmico.** O modelo do passo 1 não conhece os adaptadores: acrescente-os à colaboração (`Botao → Adaptador → Sintonizador`) e siga em frente. Repare: cada passo levou minutos, e o resultado é um projeto **flexível** — pronto para virar código.
:::

::: regra Sempre visualize o código
Diagramas são **atalhos para o código, não substitutos**. Se você desenha e **não consegue imaginar o código** correspondente, está construindo castelos no ar: pare e descubra como isso vira C#. Nunca deixe o diagrama virar um fim em si mesmo.
:::

## Quando desenhar — e quando não

**Desenhe quando:**

- Várias pessoas precisam entender a estrutura de uma parte do projeto **porque vão trabalhar nela ao mesmo tempo**. *Pare quando todos concordarem que entenderam.*
- Há **discordância** sobre como projetar um elemento e você quer **consenso**. *Dê um tempo limite, decida (voto ou juiz) e apague o diagrama.*
- Você quer **pensar** numa ideia e o desenho ajuda. *Pare quando conseguir concluir no código; descarte.*
- Precisa **explicar** a estrutura de um código a outra pessoa (ou a si mesmo). *Pare quando o código explicar melhor.*
- O fim do projeto se aproxima e o cliente pediu os diagramas como **documentação**.

**Não desenhe:**

- **Porque o processo manda.**
- Porque se sente **culpado** por não desenhar ("é o que bons projetistas fazem"). **Bons projetistas escrevem código**; só desenham quando necessário.
- Para criar **documentação abrangente antes de codificar** — quase nunca tem valor e consome tempo imenso.
- Para **outras pessoas codificarem**. Arquitetos de verdade participam da codificação.

## Ferramentas CASE: cuidado

| Promessa | Realidade (segundo a experiência do autor) |
|---|---|
| "Facilitam desenhar" | Têm curva de aprendizagem longa e são **mais desajeitadas** que um quadro branco |
| "Facilitam colaboração em equipes grandes" | Só quando há **muitos** diagramas e complexidade; adote **depois** de um sistema manual mostrar deficiências |
| "Geram código" | É improvável que diagramar + gerar + usar seja **bem mais barato** que escrever; o ganho, se houver, **dificilmente chega a 2×** — **meça** antes de gastar |
| "IDE com diagrama e código sincronizados" | Interessante, mas a presença constante da UML pouco ajuda; prefira uma IDE boa em manipular **código** |

Conselho: **observe antes de dar o salto**, e observe muito bem. Hoje há ferramentas gratuitas e leves (PlantUML, Mermaid, draw.io, o *Designer de Classes* do Visual Studio) que cabem em experimentos baratos.

::: dica Diagramas como texto
Ferramentas "diagrama como código" (**Mermaid**, **PlantUML**) permitem versionar diagramas junto com o código e atualizá-los em segundos — bom para **roteiros** que valem guardar.
:::

## E a documentação?

Boa documentação é fundamental; **documentação demais do tipo errado é pior** que nenhuma, porque distrai e engana. Documente **protocolos de comunicação complexos**, **esquemas relacionais complexos** e **frameworks reutilizáveis** complexos — mas nenhum deles precisa de 100 páginas de UML. **O valor de um documento de software é inversamente proporcional ao seu tamanho.**

Para uma equipe de 12 pessoas num sistema de um milhão de linhas, o autor aceitaria **de 25 a 200 páginas** (preferindo o extremo menor): diagramas de alto nível dos módulos importantes, o diagrama ER do banco, uma ou duas páginas de **como compilar**, instruções de teste e de controle de versão — tudo num **wiki**, acessível e editável por todos. **Dá trabalho tornar um documento pequeno, mas vale a pena: pessoas leem documentos pequenos.**

::: exercicio Quando desenhar?
Diga se **vale desenhar** e que tipo de diagrama:

1. Duas pessoas discordam de como modelar "assinaturas e planos". Reunião de 20 minutos.
2. Seu chefe pede "o diagrama de classes de todo o sistema antes de começar a codificar".
3. Você precisa explicar a um novato como funciona o fluxo de uma compra.
4. Quer documentar um algoritmo de ordenação em 30 linhas.

--- resposta
1. **Sim.** Quadro branco, um diagrama de classes (e talvez de sequência) para decidir; tempo limitado; **apague** depois.
2. **Não** (como regra): "documentação abrangente antes de codificar" quase nunca tem valor. Proponha diagramas **pontuais** por iteração.
3. **Sim:** um **diagrama de sequência** simples (cliente → carrinho → pagamento → estoque), no quadro; guarde apenas se for reaproveitado.
4. **Não:** UML é ruim para detalhes algorítmicos. O próprio código (bem nomeado) é a melhor descrição; no máximo um comentário.
:::

::: resumo
- **UML** é notação gráfica com três níveis (conceitual, especificação, implementação) e três famílias (estáticos, dinâmicos, físicos).
- Principais: **classes** (estrutura), **objetos** (foto), **sequência** e **colaboração** (mensagens), **estados** (comportamento).
- Modele para **comunicar e raciocinar**; **UML não é código** e detalhes demais atrapalham.
- Diagramas são **efêmeros** por padrão; guarde só os **reutilizados** e poucos.
- Refine em **passos de ~5 min**: comportamento → estrutura → princípios → comportamento.
- Sempre **visualize o código** correspondente; não crie regras de "tudo deve ser diagramado".
- **CASE**: experimente antes de comprar. **Documentação**: curta e útil.
:::

::: quiz
? Qual nível de UML descreve conceitos do domínio do problema, sem ligação direta com código?
+ Conceitual
- Especificação
- Implementação
- Físico
> Os dois últimos têm forte ligação com o código.

? O que mostra um diagrama de objetos?
+ Objetos e links em um instante da execução (um instantâneo da memória)
- A hierarquia de herança
- A ordem das mensagens
- Os estados do sistema
> Os nomes aparecem sublinhados no formato `nome : Classe`.

? Qual diagrama enfatiza a ordem temporal das mensagens entre objetos?
+ Diagrama de sequência
- Diagrama de classes
- Diagrama de objetos
- Diagrama de estados
> O de colaboração traz a mesma informação, mas enfatiza os relacionamentos.

? Para que servem os diagramas de estados?
+ Descrever o comportamento de máquinas de estados e explorar casos inesperados
- Mostrar arquivos físicos
- Listar métodos de uma classe
- Gerar código automaticamente
> Setas são transições `evento / ação`.

? Quando a UML é uma má ferramenta?
+ Para comunicar detalhes algorítmicos
- Para esboçar a estrutura de classes em uma conversa
- Para fazer um roteiro de estrutura
- Para discutir uma colaboração
> Código bem escrito é a melhor expressão do algoritmo.

? Qual é a política recomendada para a maioria dos diagramas?
+ Criá-los em meio efêmero (quadro) e jogá-los fora; guardar só os realmente reutilizados
- Guardar todos no repositório
- Exigir um diagrama para cada classe
- Usar sempre uma ferramenta CASE
> Diagramas úteis aparecem repetidas vezes; esses merecem guarda.

? Como refinar diagramas de forma iterativa?
+ Alternar comportamento (dinâmico) e estrutura (estático) em ciclos de poucos minutos
- Fazer toda a estrutura antes de qualquer comportamento
- Desenhar tudo de uma vez
- Evitar rever o desenho
> Passos pequenos permitem avaliar e corrigir.

? O que o autor recomenda antes de comprar uma ferramenta CASE?
+ Experimentar e medir o ganho de produtividade
- Comprar a mais cara
- Adotar para toda a empresa imediatamente
- Nunca usar diagramas
> Observe antes de dar o salto.
:::

::: cartas
UML :: Notação gráfica unificada para representar conceitos de software.
Nível conceitual :: Diagrama do domínio do problema; sujeito a interpretação.
Nível de especificação/implementação :: Diagramas com forte ligação ao código.
Diagrama de classes :: Mostra classes e relacionamentos (estático).
Diagrama de objetos :: Instantâneo de objetos e links em um momento.
Diagrama de sequência :: Ordem das mensagens entre objetos ao longo do tempo.
Diagrama de colaboração :: Mensagens numeradas sobre os links entre objetos.
Diagrama de estados :: Máquina de estados: estados, eventos e transições.
Refinamento iterativo :: Alternar dinâmico e estático em passos de poucos minutos.
Ferramenta CASE :: Software de modelagem/geração de código; avalie antes de adotar.
:::
