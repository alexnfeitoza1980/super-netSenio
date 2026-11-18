---
id: agil-cap26
order: 42
kind: capitulo
num: 26
group: C# · Parte 9 · UML na prática
title: Diagramas de estados, de objetos e casos de uso
subtitle: Máquinas de estados (e suas tabelas em texto), instantâneos de objetos — inclusive objetos ativos — e casos de uso escritos de forma simples.
minutes: 50
hue: 230
objectives:
- Ler e desenhar **máquinas de estados finitos**: estados, transições, entry/exit, superestados, pseudoestados
- Representar uma FSM como **tabela de transição** em texto e implementá-la em C#
- Usar **diagramas de objetos** (inclusive com **objetos ativos**) quando ajudam
- Escrever **casos de uso** simples (curso principal e cursos alternativos) e saber o que ignorar
origem: livros/02-projeto-agil-csharp/content/41-cap26.md
---

::: ilustracao img/agil/cap26.svg | Estados mostram o ciclo de vida; objetos, um instante; casos de uso, quem faz o quê.
:::

## Máquinas de estados finitos (FSM)

A UML tem uma notação rica para **máquinas de estados finitos** (*Finite State Machines*). São ferramentas valiosas para **interfaces gráficas**, **protocolos de comunicação** e qualquer sistema **baseado em eventos**. Muitos desenvolvedores não as conhecem e perdem a chance de simplificar bastante o código.

### O básico

::: figura Máquina de estados de um pedido
<svg viewBox="0 0 780 290" role="img" aria-label="Estados do pedido: Criado, Pago, Enviado, Entregue, Cancelado">
  <circle cx="20" cy="40" r="8" fill="var(--ink)"/>
  <path class="arrow" d="M28 40H70"/>
  <g text-anchor="middle" font-size="13">
    <rect class="f-prim" x="72" y="12" width="170" height="64" rx="14"/><text x="157" y="38" font-weight="800">Criado</text><text x="157" y="60" font-size="11" class="t-mono t-dim">entry / mostrarResumo</text>
    <rect class="f-info" x="320" y="12" width="150" height="64" rx="14"/><text x="395" y="38" font-weight="800">Pago</text><text x="395" y="60" font-size="11" class="t-mono t-dim">entry / emitirNota</text>
    <rect class="f-good" x="548" y="12" width="150" height="64" rx="14"/><text x="623" y="38" font-weight="800">Enviado</text><text x="623" y="60" font-size="11" class="t-mono t-dim">entry / avisarCliente</text>
    <rect class="f-good" x="548" y="150" width="150" height="64" rx="14"/><text x="623" y="186" font-weight="800">Entregue</text>
    <rect class="f-bad" x="200" y="170" width="190" height="64" rx="14"/><text x="295" y="196" font-weight="800">Cancelado</text><text x="295" y="218" font-size="11" class="t-mono t-dim">entry / estornar</text>
  </g>
  <path class="arrow" d="M244 44H318"/><text x="281" y="36" text-anchor="middle" font-size="11" class="t-mono">pagar</text>
  <path class="arrow" d="M472 44H546"/><text x="509" y="36" text-anchor="middle" font-size="11" class="t-mono">despachar</text>
  <path class="arrow" d="M623 78V148"/><text x="650" y="118" font-size="11" class="t-mono">confirmar</text>
  <path class="arrow" d="M120 78C120 130 150 170 198 190" style="stroke:var(--bad);marker-end:url(#arr3)"/><text x="82" y="136" font-size="11" class="t-mono">cancelar</text>
  <path class="arrow" d="M370 78C340 110 320 140 310 168" style="stroke:var(--bad);marker-end:url(#arr3)"/><text x="372" y="125" font-size="11" class="t-mono">cancelar / estornar</text>
</svg>
:::

- **Retângulos arredondados** são **estados**. Num compartimento inferior ficam as **ações especiais**: `entry/…` (ao entrar) e `exit/…` (ao sair).
- **Setas** são **transições**, rotuladas com o **evento** que as dispara e, opcionalmente, com uma **ação** (`evento / ação`).
- O **círculo preto** é o **pseudoestado inicial**: a máquina nasce seguindo a transição que sai dele.
- Quando uma transição dispara, o estado do sistema **muda**.

O diagrama deixa claro o **processo** e o decompõe em **funções pequenas** (as ações). Se você implementar cada ação (`emitirNota`, `estornar`…) e ligá-las à lógica do diagrama, tem **certeza** de que o fluxo funciona.

### Eventos especiais e transições reflexivas

No compartimento inferior você pode ter **seus próprios eventos** além de `entry` e `exit`: `meuEvento / minhaAção` (a ação roda se o evento ocorre **enquanto** a máquina está nesse estado, **sem sair** dele).

Cuidado com a **transição reflexiva** (seta que sai do estado e volta a ele): na UML, **toda transição que sai** de um estado ativa seu `exit`, e **toda que entra** ativa o `entry`. Assim, uma reflexiva executa **`exit` → ação → `entry`**. Um evento interno do compartimento inferior **não** dispara `entry`/`exit`.

### Superestados

Quando vários estados reagem a um mesmo evento da mesma maneira, desenhe um **superestado** em volta deles e **uma única seta** saindo dele (em vez de repeti-la em cada estado).

::: figura Superestado: uma seta só
<svg viewBox="0 0 700 190" role="img" aria-label="Cancelável envolve E1, E2 e E3; cancelar vai para Início">
  <rect x="120" y="15" width="400" height="120" rx="18" fill="none" stroke="var(--ink-3)" stroke-dasharray="6 4" stroke-width="2"/><text x="140" y="36" font-size="12" font-weight="700" class="t-dim">Cancelável (superestado)</text>
  <g text-anchor="middle" font-size="13"><rect class="f-info" x="140" y="62" width="90" height="48" rx="12"/><text x="185" y="91" font-weight="700">E1</text><rect class="f-info" x="272" y="62" width="90" height="48" rx="12"/><text x="317" y="91" font-weight="700">E2</text><rect class="f-info" x="404" y="62" width="90" height="48" rx="12"/><text x="449" y="91" font-weight="700">E3</text>
  <rect class="f-prim" x="580" y="62" width="100" height="48" rx="24"/><text x="630" y="91" font-weight="800">Início</text></g>
  <path class="arrow" d="M232 86H270"/><path class="arrow" d="M364 86H402"/>
  <path class="arrow" d="M522 75H578"/><text x="550" y="66" text-anchor="middle" font-size="11" class="t-mono">cancelar / reiniciar</text>
</svg>
:::

Um subestado pode **sobrescrever** a transição do superestado (desenhando a sua própria) — parecido com uma subclasse sobrescrevendo um método. Mas **não leve a metáfora longe**: não é herança de verdade. Superestados também têm `entry`/`exit`, ativados **de fora para dentro** ao entrar (`entrarSuper` → `entrarSub`) e **de dentro para fora** ao sair (`sairSub` → `sairSuper`); ao ir de um subestado a outro **dentro** do mesmo superestado, só `sairSub`/`entrarSub2` rodam.

### Pseudoestados inicial e final

A máquina **começa** existindo ao seguir a transição do **pseudoestado inicial** (que **não** tem evento, mas pode ter uma ação — a primeira executada). Ela **termina** ao seguir uma transição ao **pseudoestado final** (ícone de "alvo"); a ação dessa transição é a última da máquina.

## Diagrama ou texto? A tabela de transição de estados

Diagramas de FSM são ótimos para **descobrir** a máquina de subsistemas de comportamento **conhecido**. Mas a maioria dos sistemas **evolui**; e diagramas são um meio **ruim para o que muda muito**: problemas de layout e espaço interferem; o "fantasma de refazer o desenho" faz equipes adotarem gambiarras para não mexer nele.

**Texto**, ao contrário, é flexível para a mudança: sempre há espaço para mais uma linha. Por isso, em sistemas evolutivos usam-se **tabelas de transição de estado (STT)**, em arquivos de texto.

Uma catraca de ônibus (dois estados, dois eventos):

| Estado atual | Evento | Novo estado | Ação |
|---|---|---|---|
| Travada | moeda | Livre | Destravar |
| Travada | passagem | Travada | Alarme |
| Livre | moeda | Livre | Devolver |
| Livre | passagem | Travada | Travar |

Leia: *"se estamos em **Travada** e recebemos **moeda**, vamos para **Livre** e chamamos **Destravar**."* Em texto puro, as 16 palavras da tabela contêm **toda a lógica** da máquina — e uma ferramenta pode **gerar o código** a partir dela.

### Implementando a STT em C#

```cs Catraca.cs
public enum Estado { Travada, Livre }
public enum Evento { Moeda, Passagem }

public class Catraca
{
    private Estado estado = Estado.Travada;

    // a tabela É o programa
    private readonly Dictionary<(Estado, Evento), (Estado Novo, Action Acao)> tabela;

    public Catraca()
    {
        tabela = new Dictionary<(Estado, Evento), (Estado, Action)>
        {
            { (Estado.Travada, Evento.Moeda),    (Estado.Livre,   Destravar) },
            { (Estado.Travada, Evento.Passagem), (Estado.Travada, Alarme)    },
            { (Estado.Livre,   Evento.Moeda),    (Estado.Livre,   Devolver)  },
            { (Estado.Livre,   Evento.Passagem), (Estado.Travada, Travar)    },
        };
    }

    public void Receber(Evento e)
    {
        var (novo, acao) = tabela[(estado, e)];
        acao();               // executa a ação da transição
        estado = novo;        // muda de estado
    }

    private void Destravar() { Console.WriteLine("Giro liberado"); }
    private void Travar()    { Console.WriteLine("Giro bloqueado"); }
    private void Alarme()    { Console.WriteLine("ALARME!"); }
    private void Devolver()  { Console.WriteLine("Moeda devolvida"); }
}
```

::: dica Mudou o requisito? Mudou uma linha
Quer que, na catraca livre, uma moeda extra **gere um crédito** em vez de ser devolvida? Troque `Devolver` por `GuardarCredito` — **uma linha**. É a vantagem de ter a lógica em **dados** (e o motivo de o padrão **State** — capítulo 39 — e de geradores de código para FSM existirem).
:::

::: exercicio Monte uma STT
Modele um **semáforo para pedestres** com estados `Fechado`, `Aberto` e `Piscando`, e eventos `Botao`, `Tempo`. Regras: apertar o botão no fechado → aberto (liga verde); com o tempo, aberto → piscando; com o tempo, piscando → fechado. Apertar o botão em qualquer outro estado não faz nada.

--- resposta
| Estado atual | Evento | Novo estado | Ação |
|---|---|---|---|
| Fechado | Botao | Aberto | LigarVerde |
| Fechado | Tempo | Fechado | (nada) |
| Aberto | Botao | Aberto | (nada) |
| Aberto | Tempo | Piscando | PiscarVerde |
| Piscando | Botao | Piscando | (nada) |
| Piscando | Tempo | Fechado | LigarVermelho |

Perceba como a tabela força a pensar em **todas** as combinações (o que acontece com `Botao` no aberto?) — exatamente a utilidade das FSMs para os casos inesperados.
:::

## Diagramas de objetos

Um **diagrama de objetos** é um **instantâneo**: mostra objetos, relacionamentos (*links*) e valores de atributos **num momento**. É útil para mostrar a **estrutura interna** do sistema num determinado estado, a **intenção do projetista** (como certas classes e relações serão usadas) e como o sistema mudará conforme as entradas.

::: figura Do diagrama de classes ao de objetos
<svg viewBox="0 0 760 240" role="img" aria-label="Classes Casa, Comodo, Abertura; objetos concretos em um instante">
  <text x="130" y="16" text-anchor="middle" font-weight="800" class="t-prim">Estático: o que é possível</text>
  <g text-anchor="middle" font-size="13"><rect class="f-prim" x="10" y="30" width="100" height="38" rx="4"/><text x="60" y="54" font-weight="700">Casa</text><rect class="f-info" x="10" y="110" width="100" height="38" rx="4"/><text x="60" y="134" font-weight="700">Comodo</text><rect class="f-good" x="180" y="110" width="100" height="38" rx="4"/><text x="230" y="134" font-weight="700">Abertura</text><rect class="f-plain" x="150" y="190" width="70" height="34" rx="4"/><text x="185" y="212" font-size="12">Porta</text><rect class="f-plain" x="230" y="190" width="70" height="34" rx="4"/><text x="265" y="212" font-size="12">Janela</text></g>
  <path class="arrow" d="M60 70V108"/><text x="72" y="92" font-size="11">*</text><path class="arrow" d="M112 129H178"/><text x="145" y="121" font-size="11">*  2</text><path class="line" d="M185 190V150M265 190V150"/>
  <text x="560" y="16" text-anchor="middle" font-weight="800" class="t-good">Instantâneo: o que existe agora</text>
  <g text-anchor="middle" font-size="12"><rect class="f-prim" x="400" y="30" width="140" height="38" rx="4"/><text x="470" y="54" text-decoration="underline">terreo : Casa</text>
  <rect class="f-info" x="380" y="110" width="110" height="38" rx="4"/><text x="435" y="134" text-decoration="underline">cozinha : Comodo</text>
  <rect class="f-info" x="620" y="110" width="120" height="38" rx="4"/><text x="680" y="134" text-decoration="underline">sala : Comodo</text>
  <rect class="f-good" x="500" y="110" width="110" height="38" rx="4"/><text x="555" y="134" text-decoration="underline">: Passagem</text>
  <rect class="f-plain" x="380" y="190" width="100" height="34" rx="4"/><text x="430" y="212" text-decoration="underline">: Janela</text>
  <rect class="f-plain" x="640" y="190" width="100" height="34" rx="4"/><text x="690" y="212" text-decoration="underline">: Porta</text></g>
  <path class="arrow" d="M440 70L435 108"/><path class="arrow" d="M510 70L640 108"/><path class="line" d="M491 129H498M612 129H618"/><path class="arrow" d="M430 150V188"/><path class="arrow" d="M690 150V188"/>
</svg>
:::

::: cuidado Com moderação
Raramente são necessários. Muitos podem ser **inferidos** do diagrama de classes e acrescentam pouco. Não suponha que precisa de um por cenário (ou por sistema). Quando faltam, porém, são indispensáveis.
:::

### Objetos ativos

Em sistemas **multitarefa**, diagramas de objetos revelam o que o diagrama de classes **esconde**. Um servidor de sockets que cria uma **thread por conexão** tem, no diagrama de classes, "SocketServer, Thread, ServiceRunner" — nada inspirador. Mas no diagrama de **objetos** vê-se:

::: figura Servidor com uma thread por conexão
<svg viewBox="0 0 760 250" role="img" aria-label="Servidor, thread do servidor, e threads de atendimento criando runners">
  <g text-anchor="middle" font-size="12">
    <rect class="f-prim" x="10" y="100" width="130" height="44" rx="4"/><text x="75" y="127" text-decoration="underline">: Servidor</text>
    <rect class="f-info" x="190" y="100" width="150" height="44" rx="4" stroke-width="4"/><text x="265" y="120" text-decoration="underline">threadServidor : Thread</text><text x="265" y="136" font-size="10" class="t-dim">objeto ativo</text>
    <rect class="f-info" x="420" y="20" width="160" height="44" rx="4" stroke-width="4"/><text x="500" y="40" text-decoration="underline">threadAtend1 : Thread</text><text x="500" y="56" font-size="10" class="t-dim">objeto ativo</text>
    <rect class="f-info" x="420" y="100" width="160" height="44" rx="4" stroke-width="4"/><text x="500" y="120" text-decoration="underline">threadAtend2 : Thread</text><text x="500" y="136" font-size="10" class="t-dim">objeto ativo</text>
    <rect class="f-good" x="630" y="20" width="120" height="44" rx="4"/><text x="690" y="47" text-decoration="underline">: Atendente</text>
    <rect class="f-good" x="630" y="100" width="120" height="44" rx="4"/><text x="690" y="127" text-decoration="underline">: Atendente</text>
    <rect class="f-plain" x="420" y="190" width="160" height="40" rx="4"/><text x="500" y="215" text-decoration="underline">threads : Lista</text>
  </g>
  <path class="arrow" d="M142 122H188"/><path class="arrow" d="M342 112L418 50"/><text x="380" y="72" font-size="11" class="t-mono">«cria»</text><path class="arrow" d="M342 122H418"/><path class="arrow" d="M582 42H628"/><path class="arrow" d="M582 122H628"/><path class="arrow" d="M300 146L420 205"/>
</svg>
:::

As **bordas grossas** marcam **objetos ativos**: quem é a **cabeça de uma thread de controle**. O diagrama mostra que a *thread do servidor* é responsável por **criar** todos os atendentes, e cada atendente roda em sua thread. Aqui a estrutura é construída **em tempo de execução**, então **objetos** dizem mais que **classes**.

## Casos de uso

Casos de uso são uma ideia maravilhosa — que muitas equipes **complicam demais**, discutindo **forma** (pré-condições, pós-condições, atores secundários…) em vez de **conteúdo**. O segredo é a **simplicidade**.

::: regra Casos de uso: escreva, não desenhe
Um caso de uso é uma **descrição textual** do comportamento do sistema do ponto de vista de **um usuário que pediu algo**: a **sequência visível de eventos** em resposta ao estímulo de um único usuário. **Visível** = o que o usuário vê; mecanismos ocultos não entram.
:::

### Curso principal

Escreve-se o "caminho feliz", supondo que **nada dá errado**:

> **Pagar pedido (padaria)**
> 1. O atendente informa o código do produto (por leitor ou digitando).
> 2. O sistema mostra a descrição, o preço e o subtotal na tela do atendente e na do cliente.
> 3. O sistema imprime o item no cupom.
> 4. O sistema emite um bipe de confirmação.

Nada mais complexo é necessário. **Até esse nível** de detalhe pode ser demais se a implementação estiver a semanas de distância.

### Quando registrar os detalhes?

**Amanhã eles mudam.** Se vão mudar, **não anote hoje**. Registre só o **nome** do caso de uso (numa ficha, planilha ou lista) e **converse** com os interessados para estimar. Preencha os detalhes **o mais tarde possível**, perto da implementação. Registrá-los cedo demais **não é eficaz em custo**.

### Cursos alternativos

Conforme a implementação se aproxima, pense no que pode **dar errado** e acrescente os **cursos alternativos**:

> **Código de barras não lido:** o sistema emite o bipe "passe de novo"; após 3 tentativas, o atendente digita o código manualmente.
>
> **Produto sem código:** o atendente informa o preço manualmente.

Cursos alternativos costumam **revelar novos casos de uso** que ninguém tinha identificado (digitar código manualmente, informar preço manualmente).

### O que mais?

Atores, atores secundários, pré e pós-condições, níveis… para a ampla maioria dos sistemas você **não precisa**. **Aprenda a andar antes de correr**: domine casos de uso simples; depois, **com parcimônia**, adote técnicas mais sofisticadas (veja o trabalho de Alistair Cockburn). **Não enrole.**

### Diagramas de caso de uso: evite

De todos os diagramas da UML, os de caso de uso são os **mais confusos e menos úteis**. Use **apenas** o **diagrama de limite do sistema**: um retângulo (o sistema), **casos de uso** como elipses dentro, **atores** fora, ligados por **linhas sem setas** (ninguém sabe o que a direção significa).

::: figura Limite do sistema
<svg viewBox="0 0 720 260" role="img" aria-label="Atendente, Gerente e Cliente fora do retângulo; casos de uso dentro">
  <rect x="190" y="10" width="340" height="240" rx="10" fill="none" stroke="var(--ink-3)" stroke-width="2"/><text x="360" y="30" text-anchor="middle" font-size="12" class="t-dim">Sistema de Vendas</text>
  <g text-anchor="middle" font-size="13"><ellipse cx="360" cy="75" rx="100" ry="24" class="f-info"/><text x="360" y="80">Pagar pedido</text><ellipse cx="360" cy="130" rx="100" ry="24" class="f-info"/><text x="360" y="135">Fechar caixa</text><ellipse cx="360" cy="185" rx="100" ry="24" class="f-info"/><text x="360" y="190">Registrar devolução</text></g>
  <g font-size="26"><text x="60" y="85">🧍</text><text x="60" y="190">🧍</text><text x="620" y="130">🧍</text></g>
  <text x="85" y="110" font-size="12">Atendente</text><text x="75" y="215" font-size="12">Gerente</text><text x="620" y="155" font-size="12">Cliente</text>
  <path class="line" d="M95 80L262 75M95 80L262 130M95 190L262 130M95 190L262 185M620 125L460 78" style="marker-end:none"/>
</svg>
:::

Esse diagrama tem **pouca utilidade técnica**, mas serve como **capa** de apresentação para interessados. Ignore por completo os **relacionamentos entre casos de uso** (`«extends»`, generalização): geram debates improdutivos e nenhum entendimento a mais.

::: exercicio Escreva um caso de uso
Escreva o **curso principal** e **dois cursos alternativos** de **"Sacar dinheiro no caixa eletrônico"** — sem preocupações de forma, só o que o usuário vê.

--- resposta
**Curso principal:**
1. O cliente insere o cartão e digita a senha.
2. O sistema mostra o menu; o cliente escolhe *Saque* e digita o valor.
3. O sistema entrega as notas, imprime o comprovante e devolve o cartão.

**Senha incorreta:** o sistema informa o erro e permite tentar de novo; na terceira falha retém o cartão.

**Saldo insuficiente:** o sistema informa o saldo disponível e permite digitar outro valor ou cancelar.

*(Possíveis novos casos de uso revelados: "Consultar saldo", "Bloquear cartão".)*
:::

::: resumo
- **FSM:** estados, transições `evento / ação`, `entry`/`exit`, pseudoestado inicial/final, **superestados**; transição reflexiva dispara `exit` e `entry`.
- Em sistemas que **evoluem**, prefira a **tabela de transição** (texto) ao diagrama; ela vira código (ou dados) facilmente.
- **Diagrama de objetos** = instantâneo; **objetos ativos** (bordas grossas) = cabeças de threads; use com moderação.
- **Casos de uso** são **texto**: curso principal + cursos alternativos, **detalhados tarde** (amanhã mudam).
- Evite diagramas de caso de uso, exceto o de **limite do sistema**; ignore `«extends»`.
:::

::: quiz
? O que o pseudoestado inicial de uma FSM representa?
+ O ponto de partida: a máquina começa seguindo a transição que sai dele
- Um estado de erro
- O estado mais importante
- O fim da máquina
> Essa transição não tem evento (o evento é a criação da máquina), mas pode ter ação.

? O que ocorre numa transição reflexiva na UML?
+ Dispara a ação `exit`, a ação da transição e a ação `entry`
- Apenas a ação da transição
- Nenhuma ação
- Apenas `entry`
> Qualquer transição que sai/entra num estado ativa `exit`/`entry`.

? Para que serve um superestado?
+ Evitar repetir a mesma transição em vários estados que reagem igual a um evento
- Criar classes base
- Representar o estado final
- Guardar variáveis
> A seta sai do superestado uma única vez.

? Por que usar tabelas de transição de estados em sistemas que evoluem?
+ Texto é flexível para mudanças; diagramas sofrem com layout e espaço
- Porque tabelas são mais bonitas
- Porque a UML proíbe diagramas
- Porque diagramas não podem ter estados
> Adicionar uma linha é fácil; reorganizar um desenho é trabalhoso.

? O que as bordas grossas num diagrama de objetos indicam?
+ Objetos ativos (cabeça de uma thread de controle)
- Objetos estáticos
- Objetos nulos
- Interfaces
> Úteis em sistemas multitarefa.

? Como deve ser um caso de uso, segundo o capítulo?
+ Um texto simples com curso principal e cursos alternativos, do ponto de vista visível ao usuário
- Um diagrama detalhado de elipses e setas
- Um documento de 20 páginas por caso de uso
- Uma descrição dos mecanismos internos
> Detalhes só se registram perto da implementação.

? Por que adiar o registro dos detalhes de um caso de uso?
+ Porque os detalhes mudam; registrar cedo demais não compensa
- Porque o cliente não deve saber
- Porque a UML proíbe
- Porque eles não importam nunca
> Registre o nome e converse; detalhe depois.

? Qual é o único diagrama de caso de uso recomendado?
+ O de limite do sistema
- O de herança entre casos de uso
- O de `«extends»`
- Nenhum, nunca
> Mesmo assim serve mais como "capa" para interessados.
:::

::: cartas
FSM :: Máquina de estados finitos: estados, eventos e transições.
Transição :: Seta `evento / ação` que muda o estado.
entry / exit :: Ações executadas ao entrar/sair de um estado.
Superestado :: Estado que agrupa subestados que partilham transições.
Pseudoestado inicial/final :: Início e fim da máquina.
STT :: Tabela de transição de estados em texto: estado, evento, novo estado, ação.
Diagrama de objetos :: Instantâneo de objetos e links em um momento.
Objeto ativo :: Objeto que é a cabeça de uma thread de controle.
Caso de uso :: Descrição textual do comportamento visível ao usuário para um pedido.
Curso alternativo :: Variação do curso principal quando algo dá errado.
Limite do sistema :: Diagrama com atores fora e casos de uso dentro do retângulo do sistema.
:::
