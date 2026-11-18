---
id: agil-cap33
order: 50
kind: capitulo
num: 33
group: C# · Parte 11 · Pacotes, padrões e arquitetura
title: Princípios de projeto de pacotes e componentes
subtitle: Seis princípios para decidir o que vai em cada assembly e como os assemblies dependem uns dos outros — REP, CRP, CCP, ADP, SDP, SAP — e as métricas de estabilidade e abstração.
minutes: 70
hue: 165
objectives:
- Decidir **quais classes** colocar em cada componente (**REP, CRP, CCP**)
- Gerenciar **dependências entre componentes** (**ADP, SDP, SAP**) e quebrar ciclos
- Calcular **instabilidade (I)**, **abstração (A)** e **distância da sequência principal (D′)**
- Entender por que a estrutura de componentes **evolui** e não se projeta de antemão
origem: livros/02-projeto-agil-csharp/content/49-cap33.md
---

::: ilustracao img/agil/cap33.svg | Pacotes: dependa na direção do que é estável e abstrato; o que muda muito fica nas pontas.
:::

## Quando classes não bastam

À medida que um aplicativo cresce, é preciso uma organização de nível mais alto. **Classes** são ótimas para sistemas pequenos, mas têm **granulação fina demais** para organizar sistemas grandes. É necessário algo **maior**: o **pacote** ou **componente**.

Aqui "componente" é uma **unidade binária que pode ser entregue de forma independente**. No .NET, são os **assemblies** (DLLs/EXEs; hoje, também pacotes **NuGet**). Componentes permitem decompor um sistema grande em **entregáveis menores**: se as dependências entre eles forem bem gerenciadas, corrige-se um bug ou acrescenta-se uma funcionalidade **reentregando só o que mudou**. E, mais importante, **equipes** podem se concentrar em **componentes isolados**, sem se preocupar com o sistema inteiro.

Isso levanta perguntas:

1. Que princípios governam **a alocação de classes** em componentes?
2. Que princípios governam **as relações entre componentes**?
3. Projetamos componentes **antes** das classes (de cima para baixo) ou **depois** (de baixo para cima)?
4. Como se representam fisicamente? (projetos/assemblies no Visual Studio.)
5. Para quê os usaremos depois de criados?

Os seis princípios do capítulo respondem a essas perguntas: três de **coesão** (o que agrupar) e três de **acoplamento** (como se relacionam).

::: figura Os seis princípios
<svg viewBox="0 0 760 240" role="img" aria-label="Três princípios de coesão e três de acoplamento">
  <rect class="f-prim" x="10" y="10" width="360" height="220" rx="14"/><text x="190" y="38" text-anchor="middle" font-weight="800" class="t-prim" font-size="16">Coesão (o que agrupar)</text>
  <g font-size="13"><text x="28" y="76" font-weight="700">REP</text><text x="75" y="76" class="t-dim">Equivalência Reutilização/Entrega</text><text x="28" y="116" font-weight="700">CRP</text><text x="75" y="116" class="t-dim">Reutilização Comum</text><text x="28" y="156" font-weight="700">CCP</text><text x="75" y="156" class="t-dim">Fechamento Comum</text><text x="28" y="204" font-size="12" class="t-dim">visão de baixo para cima: já há classes</text></g>
  <rect class="f-good" x="390" y="10" width="360" height="220" rx="14"/><text x="570" y="38" text-anchor="middle" font-weight="800" class="t-good" font-size="16">Acoplamento (como se relacionam)</text>
  <g font-size="13"><text x="408" y="76" font-weight="700">ADP</text><text x="455" y="76" class="t-dim">Dependências Acíclicas</text><text x="408" y="116" font-weight="700">SDP</text><text x="455" y="116" class="t-dim">Dependências Estáveis</text><text x="408" y="156" font-weight="700">SAP</text><text x="455" y="156" class="t-dim">Abstrações Estáveis</text><text x="408" y="204" font-size="12" class="t-dim">os dois últimos trazem métricas (I, A, D′)</text></g>
</svg>
:::

## Coesão de componentes: granularidade

### REP — Princípio da Equivalência Reutilização/Entrega

::: regra REP
**O grão da reutilização é o grão da entrega.**
:::

O que você espera do autor de uma biblioteca que pretende reutilizar? Além de código que funcione e boa documentação:

1. **Garantia de manutenção** — se você tiver que manter, perde o benefício de reutilizar.
2. **Aviso de mudanças** de interface/funcionalidade — e o **direito de recusar** novas versões, com **suporte à versão antiga** por um tempo combinado.

Ou seja, a reutilização só existe quando há **controle de versões**: o autor organiza o software em **componentes reutilizáveis** e os controla por **números de versão**. É o que fazem NuGet e o **versionamento semântico** (`MAIOR.MENOR.CORREÇÃO`). Portanto o **componente reutilizável não pode ser menor que o grão entregue**: escrever uma classe e declará-la "reutilizável" não basta.

Implicações para o conteúdo interno:

- Um componente com software **reutilizável** não deve misturar software **não reutilizável**. **Todas** as classes são reutilizáveis ou **nenhuma**.
- E devem ter **o mesmo público**: uma biblioteca de coleções e um framework financeiro podem ser ambos reutilizáveis, mas quem quer as coleções raramente quer o framework financeiro — **não** devem estar no mesmo componente.

É uma **questão política e administrativa**, mas ela molda a estrutura do software — porque software é um **produto humano**, para humanos.

### CRP — Princípio da Reutilização Comum

::: regra CRP
**As classes de um componente são reutilizadas juntas.** Se você reutiliza uma, reutiliza todas.
:::

Ajuda a decidir **quais classes ficam juntas** (as que se usam em conjunto, como uma coleção e seus iteradores) e, principalmente, **quais não devem ficar**. Quando o componente A usa **uma só classe** do componente B, ainda **depende de B inteiro**: sempre que B for reentregue — mesmo por mudança numa classe irrelevante para A — A deve ser **revalidado e reimplantado**. Em DLLs, o código usuário depende da **DLL toda**.

Logo: **classes não fortemente ligadas entre si não devem compartilhar componente**; queremos que, ao depender de um componente, se dependa **de tudo** o que há nele — nada de revalidar à toa.

### CCP — Princípio do Fechamento Comum

::: regra CCP
**Classes que mudam juntas pelos mesmos motivos devem estar juntas.** Uma mudança que afete um componente deve afetar **todas** as suas classes e **nenhum outro componente**.
:::

É o **SRP para componentes**: um componente não deve ter vários motivos para mudar. Na maioria dos aplicativos, **manutenibilidade importa mais que reutilização**; se o código vai mudar, é melhor que as mudanças se concentrem **num só componente**, reentregando **apenas ele**. O CCP também se liga ao **OCP**: como o fechamento total é impossível, projetamos fechados contra as mudanças mais comuns e **agrupamos** no mesmo componente as classes abertas ao **mesmo tipo** de mudança.

### Equilibrando as três forças

::: figura O triângulo da tensão
<svg viewBox="0 0 720 280" role="img" aria-label="REP, CRP e CCP em tensão">
  <polygon points="360,20 60,250 660,250" fill="var(--surface)" stroke="var(--line)" stroke-width="2"/>
  <g text-anchor="middle"><text x="360" y="14" font-weight="800" class="t-prim">CCP</text><text x="360" y="42" font-size="12" class="t-dim">menos componentes para</text><text x="360" y="58" font-size="12" class="t-dim">mudar (manutenção)</text>
  <text x="40" y="262" font-weight="800" class="t-good">REP</text><text x="125" y="236" font-size="12" class="t-dim">componentes reutilizáveis</text>
  <text x="680" y="262" font-weight="800" class="t-warn">CRP</text><text x="580" y="236" font-size="12" class="t-dim">não depender do que não usa</text></g>
  <text x="360" y="160" text-anchor="middle" font-size="13" font-weight="700">Escolha onde ficar no triângulo</text><text x="360" y="180" text-anchor="middle" font-size="12" class="t-dim">e mude de lugar com o tempo</text>
</svg>
:::

| Princípio | Tende a… | Custo |
|---|---|---|
| **REP / CCP** | **Juntar** classes (componentes maiores) | Quem usa pode depender de mais do que precisa |
| **CRP** | **Separar** classes (componentes menores) | Mais componentes para entregar; mudanças se espalham |

O equilíbrio é **dinâmico**: a divisão certa hoje pode ser errada no ano que vem. No início, projetos priorizam **desenvolvimento** (CCP — mudanças localizadas); com a maturidade, o foco migra para **reutilização** (CRP/REP). Por isso a composição dos componentes **evolui**.

## Acoplamento de componentes: estabilidade

### ADP — Princípio das Dependências Acíclicas

::: regra ADP
**Não permita ciclos no grafo de dependências entre componentes.**
:::

Você já trabalhou o dia todo e, na manhã seguinte, **nada compila**, porque alguém que ficou até tarde alterou algo de que você dependia? É a **síndrome da manhã seguinte**. Em projetos pequenos, tudo bem; em grandes, pode levar **semanas** sem uma versão estável.

Duas soluções históricas:

1. **A compilação semanal (weekly build):** quatro dias isolados; **sexta** é dia de integrar. Funciona enquanto o projeto é pequeno; depois a integração invade o sábado, passa para quinta, a build vira quinzenal… o ciclo desenvolvimento/integração encolhe, a eficiência cai e o risco cresce.
2. **Eliminar ciclos:** dividir o ambiente em **componentes liberáveis**. Cada equipe cuida de um; quando ele funciona, **libera** com número de versão num lugar comum e segue modificando sua cópia. **Os outros usam a versão liberada** e decidem **quando** migrar. Ninguém fica à mercê dos demais; a integração é **incremental**. Mas isso só funciona **sem ciclos**.

::: figura Um grafo acíclico (DAG) × um com ciclo
<svg viewBox="0 0 760 260" role="img" aria-label="Estrutura acíclica e a mesma com um ciclo entre Dialogos e Aplicacao">
  <text x="170" y="16" text-anchor="middle" font-weight="800" class="t-good">Acíclico ✔</text>
  <g text-anchor="middle" font-size="12">
    <rect class="f-prim" x="110" y="30" width="120" height="34" rx="6"/><text x="170" y="52">Aplicacao</text>
    <rect class="f-info" x="30" y="100" width="100" height="34" rx="6"/><text x="80" y="122">Tarefas</text>
    <rect class="f-info" x="200" y="100" width="100" height="34" rx="6"/><text x="250" y="122">Mensagens</text>
    <rect class="f-plain" x="30" y="170" width="100" height="34" rx="6"/><text x="80" y="192">Dialogos</text>
    <rect class="f-plain" x="200" y="170" width="100" height="34" rx="6"/><text x="250" y="192">Janelas</text>
    <rect class="f-plain" x="110" y="222" width="120" height="30" rx="6"/><text x="170" y="242">BancoDeDados</text>
  </g>
  <path class="arrow" d="M140 66L90 98"/><path class="arrow" d="M200 66L240 98"/><path class="arrow" d="M80 136V168"/><path class="arrow" d="M250 136V168"/><path class="arrow" d="M130 187H198"/><path class="arrow" d="M110 206L150 222"/>
  <text x="570" y="16" text-anchor="middle" font-weight="800" class="t-bad">Com ciclo ✘</text>
  <g text-anchor="middle" font-size="12">
    <rect class="f-prim" x="500" y="30" width="120" height="34" rx="6"/><text x="560" y="52">Aplicacao</text>
    <rect class="f-info" x="420" y="100" width="100" height="34" rx="6"/><text x="470" y="122">Tarefas</text>
    <rect class="f-bad" x="420" y="170" width="100" height="34" rx="6"/><text x="470" y="192">Dialogos</text>
  </g>
  <path class="arrow" d="M530 66L480 98"/><path class="arrow" d="M470 136V168"/><path class="arrow" d="M520 175C620 150 640 90 622 52" style="stroke:var(--bad);marker-end:url(#arr3)"/>
  <text x="660" y="130" font-size="12" class="t-bad">Dialogos passou a</text><text x="660" y="146" font-size="12" class="t-bad">usar Aplicacao!</text>
</svg>
:::

#### O efeito de um ciclo

Suponha que um requisito obrigue uma classe de `Dialogos` a usar uma classe de `Aplicacao`. Surge um ciclo (`Aplicacao → Tarefas → Dialogos → Aplicacao`):

- Quem trabalha em `Tarefas` agora precisa ser compatível com **tudo**: `Aplicacao`, `Janelas`, `Mensagens`… `Tarefas` passou a depender **de todo o sistema**.
- `Aplicacao`, `Tarefas` e `Dialogos` precisam ser **liberados juntos**: viraram **um componente gigante**. Todos os seus desenvolvedores voltam à **síndrome da manhã seguinte**.
- Para **testar** `Dialogos` você precisa referenciar o sistema inteiro (inclusive o banco).
- A **ordem de compilação** pode ficar indefinida — talvez nem exista uma ordem correta.

Se você já se perguntou por que precisa referenciar tantas bibliotecas para um simples teste de unidade, provavelmente há **ciclos** no grafo.

#### Quebrando o ciclo — sempre é possível

1. **Aplicar o DIP:** criar uma interface/classe abstrata com o que `Dialogos` precisa, colocá-la **em `Dialogos`** e fazer `Aplicacao` implementá-la. A dependência se **inverte** (e a interface é nomeada pelo **cliente**).
2. **Criar um novo componente** do qual **ambos** dependam e mover para lá a(s) classe(s) comuns.

::: figura Duas formas de quebrar o ciclo
<svg viewBox="0 0 760 210" role="img" aria-label="Inversão de dependência e novo componente">
  <text x="190" y="16" text-anchor="middle" font-weight="800" class="t-prim">1 · DIP</text>
  <g text-anchor="middle" font-size="12"><rect class="f-bad" x="20" y="40" width="110" height="34" rx="6"/><text x="75" y="62">Dialogos</text><rect class="f-info" x="30" y="95" width="90" height="28" rx="6"/><text x="75" y="114" font-size="11">«interface» IX</text><rect class="f-prim" x="250" y="40" width="120" height="34" rx="6"/><text x="310" y="62">Aplicacao</text><rect class="f-plain" x="250" y="95" width="120" height="28" rx="6"/><text x="310" y="114" font-size="11">X : IX</text></g>
  <path class="arrow" d="M75 76V93"/><path class="arrow" d="M250 109H122"/><path class="arrow" d="M310 76V93" style="marker-end:none"/><text x="190" y="160" text-anchor="middle" font-size="12" class="t-dim">Aplicacao agora depende de Dialogos (IX mora lá)</text>
  <text x="570" y="16" text-anchor="middle" font-weight="800" class="t-good">2 · Novo componente</text>
  <g text-anchor="middle" font-size="12"><rect class="f-plain" x="450" y="40" width="110" height="34" rx="6"/><text x="505" y="62">Dialogos</text><rect class="f-prim" x="620" y="40" width="120" height="34" rx="6"/><text x="680" y="62">Aplicacao</text><rect class="f-good" x="535" y="130" width="130" height="34" rx="6"/><text x="600" y="152">Novo (classe comum)</text></g>
  <path class="arrow" d="M505 76L575 128"/><path class="arrow" d="M680 76L630 128"/>
</svg>
:::

A segunda opção mostra que a estrutura de componentes é **volátil**: a cada requisito novo pode surgir ciclo; **monitore** o grafo (ferramentas como NDepend, o *Dependency Validation* do Visual Studio ou o `dotnet list reference`), e quando houver ciclo, quebre — às vezes criando **mais um componente**.

#### Projeto de cima para baixo? Não!

Não dá para **projetar a estrutura de componentes de cima para baixo na ausência de código**. Ela **evolui** com o sistema. Por quê? Porque a estrutura de componentes **não é uma decomposição funcional**: é um **mapa da compilabilidade** e da **entregabilidade**. À medida que as classes surgem, passamos a gerenciar dependências (para evitar a manhã seguinte), depois ao **SRP/CCP** (agrupar o que muda junto), depois ao **CRP/REP** (reutilização) e, quando aparecem ciclos, ao **ADP**. Tentar desenhar tudo antes, sem saber o que muda junto nem o que é reutilizável, quase certamente produzirá **ciclos**.

Logo a estrutura se estabiliza o bastante para vários times trabalharem em paralelo com comunicação restrita às **fronteiras**; mas ela continuará a se mover, e os times precisarão se ajustar uns aos outros.

### SDP — Princípio das Dependências Estáveis

::: regra SDP
**Dependa na direção da estabilidade.**
:::

Projetos não podem ser totalmente estáticos: algo precisa ser **volátil** (CCP cria componentes sensíveis a certas mudanças). **Qualquer componente que você espera que mude não deve depender de um componente difícil de mudar** — senão ele também ficará difícil. E um módulo fácil de mudar pode ficar **difícil** só porque alguém colocou uma **dependência** nele, sem alterar uma linha do seu código!

**Estabilidade** (moeda em pé × mesa) não é "frequência de mudança", mas **esforço necessário para mudar**. Muitos fatores tornam algo difícil de mudar (tamanho, complexidade, clareza), mas há um infalível: **muitos outros componentes dependerem dele**.

- **X** (3 dependentes, 0 dependências): **estável** — "responsável" por 3, "independente".
- **Y** (0 dependentes, 3 dependências): **instável** — "irresponsável" e "dependente".

#### Métrica de instabilidade

- **Ca (acoplamento aferente):** nº de classes **fora** do componente que **dependem** de classes dentro dele (quem **entra**).
- **Ce (acoplamento eferente):** nº de classes **dentro** que **dependem** de classes fora (quem **sai**).
- **Instabilidade:** **I = Ce / (Ca + Ce)**, de 0 a 1.

| I | Significado |
|---|---|
| **0** | Estável ao máximo: muitos dependem dele, ele de ninguém (responsável e independente) |
| **1** | Instável ao máximo: ninguém depende dele, ele depende de outros (irresponsável e dependente) |

Exemplo: três classes de fora dependem do componente (Ca = 3) e uma classe dele depende de fora (Ce = 1): **I = 1/(3+1) = 0,25**.

Em C#, as dependências aparecem como **`using`** e nomes totalmente qualificados (mais fácil de calcular com **uma classe por arquivo**).

**Regra do SDP:** o **I** de um componente deve ser **maior** que o **I** dos componentes de que **ele depende**. Os valores de I **diminuem na direção da dependência**.

::: figura Configuração ideal e violação
<svg viewBox="0 0 740 230" role="img" aria-label="Instáveis no topo dependendo do estável embaixo; setas para cima violam o SDP">
  <text x="170" y="16" text-anchor="middle" font-weight="800" class="t-good">Configuração ideal</text>
  <g text-anchor="middle" font-size="12"><rect class="f-bad" x="30" y="30" width="110" height="40" rx="6"/><text x="85" y="52">Instável I=1</text><rect class="f-bad" x="200" y="30" width="110" height="40" rx="6"/><text x="255" y="52">Instável I=1</text><rect class="f-good" x="115" y="150" width="110" height="40" rx="6"/><text x="170" y="172">Estável I=0</text></g>
  <path class="arrow" d="M85 72L150 148"/><path class="arrow" d="M255 72L190 148"/>
  <text x="560" y="16" text-anchor="middle" font-weight="800" class="t-bad">Violação do SDP</text>
  <g text-anchor="middle" font-size="12"><rect class="f-bad" x="440" y="30" width="130" height="40" rx="6"/><text x="505" y="52">Flexible I=0,75</text><rect class="f-good" x="440" y="150" width="130" height="40" rx="6"/><text x="505" y="172">Stable I=0,25</text></g>
  <path class="arrow" d="M505 150V72" style="stroke:var(--bad);marker-end:url(#arr3)"/><text x="660" y="115" text-anchor="middle" font-size="12" class="t-bad">dependência "para cima"</text>
</svg>
:::

Convenção útil: desenhe **instáveis em cima** e **estáveis embaixo**; **qualquer seta para cima viola o SDP**.

#### Corrigindo uma violação com o DIP

`Stable` depende de `Flexible` porque uma classe `U` (em `Stable`) usa uma classe `C` (em `Flexible`). Crie uma **interface** `IU` num componente novo muito estável (`UInterface`, I = 0), com os métodos de que `U` precisa; faça `C` **implementar** `IU`. Agora `Stable` e `Flexible` **dependem de `UInterface`**; `Flexible` mantém sua instabilidade e **tudo fluxe na direção decrescente de I**.

#### Onde guardar o projeto de alto nível?

As **decisões de arquitetura** não devem mudar com frequência: ficam em componentes **estáveis (I = 0)**; os instáveis guardam o que provavelmente muda. Mas, se o projeto de alto nível está num componente estável, não ficará **rígido**? A resposta é o **OCP**: classes **abstratas** podem ser estendidas **sem modificar** o componente estável.

### SAP — Princípio das Abstrações Estáveis

::: regra SAP
**Um componente deve ser tão abstrato quanto é estável.**
:::

Componentes **estáveis** devem ser **abstratos**, para que a estabilidade não impeça a extensão; componentes **instáveis** devem ser **concretos**, pois sua instabilidade permite alterar o código concreto com facilidade. **SDP + SAP = DIP para componentes**: dependências fluem na direção da **estabilidade**, e estabilidade implica **abstração**, logo as dependências fluem na direção da **abstração**. A diferença: o DIP trata de **classes** (ou é abstrata ou não é); SDP+SAP tratam de **componentes**, que podem ser **parcialmente** abstratos e estáveis.

#### Métrica de abstração

**A = Na / Nc**, onde **Nc** é o número de classes do componente e **Na** o de **abstratas** (interfaces e classes abstratas). **0** = nenhuma abstrata; **1** = só abstratas.

#### A sequência principal

No gráfico com **A** na vertical e **I** na horizontal:

::: figura O gráfico A × I
<svg viewBox="0 0 520 380" role="img" aria-label="Gráfico A por I com a sequência principal, a zona de sofrimento e a zona de inutilidade">
  <path class="line" d="M60 20V320H500" style="marker-end:none"/>
  <text x="26" y="170" font-size="13" transform="rotate(-90 26 170)" text-anchor="middle">A (abstração)</text><text x="280" y="352" font-size="13" text-anchor="middle">I (instabilidade)</text>
  <text x="56" y="332" font-size="12" text-anchor="end">0</text><text x="56" y="30" font-size="12" text-anchor="end">1</text><text x="500" y="338" font-size="12" text-anchor="end">1</text>
  <circle cx="90" cy="320" r="70" fill="var(--bad-soft)" stroke="var(--bad)" stroke-dasharray="5 4"/><text x="95" y="300" font-size="12" class="t-bad" font-weight="700">zona de</text><text x="95" y="316" font-size="12" class="t-bad" font-weight="700">sofrimento</text>
  <circle cx="460" cy="20" r="70" fill="var(--warn-soft)" stroke="var(--warn)" stroke-dasharray="5 4"/><text x="400" y="62" font-size="12" class="t-warn" font-weight="700">zona de</text><text x="400" y="78" font-size="12" class="t-warn" font-weight="700">inutilidade</text>
  <path d="M60 20L460 320" stroke="var(--good)" stroke-width="3"/><text x="260" y="150" font-size="13" font-weight="800" class="t-good" transform="rotate(37 260 150)">sequência principal</text>
  <circle cx="60" cy="20" r="6" fill="var(--good)"/><text x="72" y="16" font-size="11">(0,1) estável e abstrato</text>
  <circle cx="460" cy="320" r="6" fill="var(--good)"/><text x="470" y="306" font-size="11" text-anchor="end">(1,0) instável e concreto</text>
  <text x="84" y="344" font-size="11" class="t-dim">(0,0)</text>
</svg>
:::

- **(0,1):** estável **e** abstrato — ideal para **abstrações** das quais todos dependem.
- **(1,0):** instável **e** concreto — ideal para **implementações** que ninguém referencia.
- A linha entre os dois é a **sequência principal** (nome inspirado nos diagramas H-R da astronomia).
- **Zona de sofrimento (perto de (0,0)):** estável **e** concreto — **rígido**: difícil de mudar **e** impossível de estender. Exemplos aceitáveis: o **esquema de um banco de dados** (volátil, concreto e muito usado — por isso a ponte objetos ↔ banco é tão dolorosa) e **bibliotecas utilitárias concretas e não voláteis** (como o componente de `string`: não muda, então **não sofre**).
- **Zona de inutilidade (perto de (1,1)):** abstrato ao máximo **e** sem dependentes: componentes **inúteis**.

Queremos os componentes **voláteis longe das duas zonas**, isto é, **na sequência principal ou perto dela**. Em geral, **menos da metade** dos componentes chega aos extremos ideais; o restante está melhor **na** linha.

#### Distância da sequência principal

- **D′ = |A + I − 1|** (varia de 0 a 1). **0** = exatamente na sequência principal; **1** = o mais longe possível.

Componentes com D′ **longe de 0** merecem reexame e reestruturação. Também é possível fazer **análise estatística** (média e desvio-padrão de D′ de todos os componentes; os acima de ±1 desvio são **anômalos**) e acompanhar D′ **ao longo das versões**: um componente cujo D′ passa de um limite de controle (por exemplo, 0,1) deve ser investigado.

::: passos Calculando as métricas de um componente
**Componente `Dominio`:** 4 classes, das quais 1 interface e 1 classe abstrata. 
===
**Abstração:** `A = Na / Nc = 2 / 4 = 0,5`.
===
**Acoplamentos:** 5 classes de **outros** componentes dependem de `Dominio` (`Ca = 5`); 1 classe de `Dominio` depende de fora (`Ce = 1`). 
**Instabilidade:** `I = Ce / (Ca + Ce) = 1 / 6 ≈ 0,17` (bastante estável).
===
**Distância:** `D′ = |A + I − 1| = |0,5 + 0,17 − 1| ≈ 0,33`. Está **abaixo** da sequência principal (estável, mas pouco abstrato): vale **introduzir interfaces** nos pontos de dependência para aumentar A.
:::

::: dica Ferramentas
No .NET você pode obter essas métricas com **NDepend**, **Visual Studio (Calcular Métricas de Código / Dependency Graph)**, analisadores Roslyn, ou até um script que conte `using` e tipos abstratos. Não é preciso exatidão científica: é um **termômetro** para decidir onde olhar.
:::

::: cuidado Métricas são medidas, não leis
As métricas medem a conformidade com **um padrão** que a experiência mostrou ser bom; **não são irrevogáveis**. Podem servir a certos aplicativos e não a outros. Use-as para **conversar** sobre o projeto, não para cumprir números.
:::

## O que isso significa na prática (.NET)

| Conceito | No dia a dia |
|---|---|
| **Componente** | Projeto (`.csproj`) → assembly (DLL) → pacote NuGet |
| **Dependência entre componentes** | `<ProjectReference>` / `<PackageReference>` |
| **REP** | Versione e publique pacotes coesos; mantenha *changelog* e suporte à versão antiga |
| **CRP** | Não crie um "Common.dll" com tudo: quem usa um helper arrasta o resto |
| **CCP** | Agrupe o que muda junto (ex.: `Pedidos.Dominio`, `Pedidos.Persistencia`) |
| **ADP** | O compilador já **proíbe** referências circulares entre projetos — o ciclo costuma aparecer em *namespaces* dentro de um projeto; quebre com interfaces |
| **SDP / SAP** | Domínio (abstrato, estável) no centro; infraestrutura (concreta, instável) nas bordas, dependendo do domínio (arquitetura "hexagonal"/"limpa") |

::: exercicio Diagnostique a estrutura
Um sistema tem os projetos: `Util` (200 classes concretas variadas; todo mundo usa), `Pedidos` (regras, usa `Util` e `Banco`), `Banco` (SQL concreto; usa `Util`; **`Util` usa uma classe de `Pedidos`**), `Interface` (telas; usa `Pedidos`).

1. Que problemas você identifica?
2. Como você os corrigiria?

--- resposta
1. **Ciclo** (`Util → Pedidos → Util`: viola o **ADP**; na prática, projetos com referência circular nem compilam, então a "gambiarra" costuma ser duplicar código ou usar reflexão). **`Util` gigante** viola o **CRP** (quem usa uma função arrasta 200 classes) e o **CCP** (mistura motivos de mudança). `Util` estável e concreto fica na **zona de sofrimento**. `Banco` concreto sendo referenciado por `Pedidos` faz a **política depender do detalhe** (viola SDP/SAP/DIP).
2. Quebrar `Util` em componentes **coesos** (`Util.Texto`, `Util.Datas`…); mover para `Pedidos` ou criar `Pedidos.Abstracoes` a classe de `Pedidos` que `Util` usava (ou invertê-la com uma **interface**); criar uma interface `IRepositorioDePedidos` em `Pedidos` e fazer `Banco` **implementá-la** (inverte a dependência: `Banco → Pedidos`). Resultado: `Interface → Pedidos ← Banco`, com o domínio **abstrato e estável** no meio.
:::

::: resumo
- **Componente** = unidade binária entregável (assembly/NuGet). Classes são pequenas demais para organizar sistemas grandes.
- **Coesão:** **REP** (grão de reutilização = grão de entrega), **CRP** (reutilizadas juntas), **CCP** (mudam juntas) — em tensão constante.
- **Acoplamento:** **ADP** (sem ciclos — quebre com DIP ou criando componente), **SDP** (dependa na direção da estabilidade), **SAP** (tão abstrato quanto estável).
- Métricas: **I = Ce/(Ca+Ce)**, **A = Na/Nc**, **D′ = |A+I−1|**; mire a **sequência principal**; evite as zonas de **sofrimento** e **inutilidade**.
- A estrutura de componentes **evolui com o código**; não se projeta de cima para baixo.
- Métricas orientam conversas; **não** são lei.
:::

::: quiz
? O que afirma o REP (Equivalência Reutilização/Entrega)?
+ O grão da reutilização é o grão da entrega: só se reutiliza o que é versionado e entregue como componente
- Toda classe deve ser reutilizável
- Componentes devem ter uma só classe
- Reutilizar é opcional
> A reutilização exige controle de versões e garantias de suporte.

? O que diz o CRP?
+ As classes de um componente são reutilizadas juntas; classes não fortemente ligadas não devem ficar juntas
- Todas as classes devem herdar da mesma base
- Componentes devem ser grandes
- Classes devem mudar juntas
> Quem depende de uma classe depende do componente inteiro.

? O CCP é equivalente a qual princípio, mas para componentes?
+ SRP
- OCP
- LSP
- ISP
> Um componente não deve ter vários motivos para mudar.

? O que o ADP proíbe?
+ Ciclos no grafo de dependências entre componentes
- Dependências entre componentes
- Interfaces
- Herança
> Ciclos geram a "síndrome da manhã seguinte" e componentes gigantes.

? Como quebrar um ciclo de dependências entre componentes?
+ Aplicando o DIP (interface no cliente) ou criando um novo componente com o que é comum
- Copiando as classes nos dois componentes
- Removendo os testes
- Usando variáveis globais
> Ambas as formas restabelecem um grafo acíclico.

? Como se calcula a instabilidade I?
+ I = Ce / (Ca + Ce)
- I = Ca / (Ca + Ce)
- I = Na / Nc
- I = |A + I − 1|
> I = 0 é estável; I = 1 é instável.

? O que o SAP diz?
+ Um componente deve ser tão abstrato quanto é estável
- Todo componente deve ser abstrato
- Componentes instáveis devem ser abstratos
- Abstrações devem depender de detalhes
> Estáveis → abstratos (extensíveis); instáveis → concretos (fáceis de mudar).

? O que é a "zona de sofrimento"?
+ A região próxima de (0,0): estável e concreto, rígido e difícil de estender
- A região (1,1)
- O centro da sequência principal
- A região (1,0)
> Exemplos aceitáveis: esquemas de banco e bibliotecas utilitárias não voláteis.

? Qual o significado de D′ = 0?
+ O componente está exatamente na sequência principal
- O componente é inútil
- O componente é instável
- O componente tem ciclos
> D′ = |A + I − 1|.
:::

::: cartas
Componente :: Unidade binária entregável de forma independente (assembly/NuGet).
REP :: O grão da reutilização é o grão da entrega.
CRP :: Classes de um componente são reutilizadas juntas.
CCP :: Classes que mudam juntas ficam juntas (SRP para componentes).
ADP :: Sem ciclos no grafo de dependências de componentes.
SDP :: Dependa na direção da estabilidade.
SAP :: Um componente deve ser tão abstrato quanto é estável.
Ca / Ce :: Acoplamento aferente (entra) / eferente (sai).
I (instabilidade) :: Ce / (Ca + Ce); 0 = estável, 1 = instável.
A (abstração) :: Classes abstratas / total de classes.
Sequência principal :: Linha A + I = 1 no gráfico; posição ideal.
D′ :: |A + I − 1|: distância normalizada da sequência principal.
Síndrome da manhã seguinte :: Trabalho que quebra por mudanças de dependências alheias.
:::
