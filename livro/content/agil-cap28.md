---
id: agil-cap28
order: 44
kind: capitulo
num: 28
group: C# · Parte 9 · UML na prática
title: Heurísticas de projeto: o problema da cafeteira (e do irrigador)
subtitle: Os erros que todo iniciante comete — classes sem métodos, classes vapor, abstrações imaginárias, classes deusas, linhas cruzadas — e como projetar a partir do comportamento.
minutes: 60
hue: 230
objectives:
- Reconhecer os **erros clássicos** de projeto: *métodos ausentes*, *classes vapor*, *abstração imaginária*, *classe deusa*, *linhas cruzadas*
- Projetar **a partir do comportamento** (casos de uso → colaborações → classes)
- Separar **diretiva de alto nível** de **detalhes de hardware/plataforma** com **DIP**, **SRP** e classes abstratas
- Adiar decisões como **polling × multitarefa** e organizar a execução com **`IPollable`**
origem: livros/02-projeto-agil-csharp/content/43-cap28.md
---

::: ilustracao img/agil/cap28.svg | Heurísticas de projeto: abstrações pequenas (sensor, válvula, aquecedor) escondem os detalhes do hardware.
:::

## O exercício que revela os erros

Um instrutor que ensina projeto orientado a objetos há mais de uma década notou algo: nas turmas, as pessoas cometem **sempre o mesmo conjunto de erros**. O exercício da tarde é projetar o software de **uma máquina simples com sensores e atuadores** — no livro, uma cafeteira elétrica; aqui usaremos um problema equivalente, com a mesma estrutura, para você treinar: **um irrigador automático de jardim**.

::: vila O irrigador Jardim-1
O Jardim-1 rega um canteiro. O usuário **enche o reservatório**, **posiciona o regador** sobre o canteiro e **aperta o botão Regar**. Uma **bomba** empurra a água por uma **válvula**; um **sensor de umidade do solo** mede quando o canteiro está encharcado. Se o **regador for retirado** do lugar durante a rega, o fluxo **pausa** (para não molhar o chão de cimento); quando voltar, **retoma**. Ao terminar, um **indicador luminoso** acende; ele apaga quando o usuário apertar o botão de novo.

Hardware a controlar: **bomba** (liga/desliga), **válvula** (abre/fecha), **indicador** (liga/desliga). Hardware a monitorar: **sensor de nível** do reservatório (`Vazio`/`ComAgua`), **sensor de umidade** (`Seco`/`Umido`/`Encharcado`), **sensor de posição** do regador (`Ausente`/`Presente`) e o **botão** (`Apertado`/`Solto`).

A equipe de hardware já entregou uma **API de baixo nível** (`IApiJardim1`) com métodos como `LerNivel()`, `LerUmidade()`, `LerPosicao()`, `LerBotao()`, `LigarBomba(estado)`, `AbrirValvula(estado)`, `AcenderIndicador(estado)`.
:::

Se quiser um desafio, **pare aqui** e tente projetar: diagramas de classes, sequência e uma máquina de estados. Depois compare.

## A solução mais comum — e horrível

A maioria desenha **uma classe central `Irrigador`** cercada por **classes subordinadas**, uma por dispositivo: `Bomba`, `Valvula`, `Botao`, `Luz`, `SensorDeNivel`, `SensorDeUmidade`… com duas classes base: `Sensor` e `Atuador`.

::: figura Irrigador "hiperconcreto"
<svg viewBox="0 0 740 280" role="img" aria-label="Irrigador no centro, rodeado por Bomba, Valvula, Botao, Luz, sensores, com Sensor e Atuador como bases">
  <rect class="f-bad" x="290" y="115" width="160" height="50" rx="8"/><text x="370" y="146" text-anchor="middle" font-weight="800">Irrigador</text>
  <g text-anchor="middle" font-size="12">
    <rect class="f-plain" x="20" y="20" width="110" height="38" rx="6"/><text x="75" y="44">Bomba</text>
    <rect class="f-plain" x="150" y="20" width="110" height="38" rx="6"/><text x="205" y="44">Valvula</text>
    <rect class="f-plain" x="280" y="20" width="110" height="38" rx="6"/><text x="335" y="44">Botao</text>
    <rect class="f-plain" x="410" y="20" width="110" height="38" rx="6"/><text x="465" y="44">Luz</text>
    <rect class="f-plain" x="540" y="20" width="170" height="38" rx="6"/><text x="625" y="44">SensorDeUmidade</text>
    <rect class="f-plain" x="20" y="215" width="140" height="38" rx="6"/><text x="90" y="239">SensorDeNivel</text>
    <rect class="f-plain" x="190" y="215" width="150" height="38" rx="6"/><text x="265" y="239">SensorDePosicao</text>
    <rect class="f-plain" x="440" y="215" width="120" height="38" rx="6"/><text x="500" y="239">«abstrata» Sensor</text>
    <rect class="f-plain" x="590" y="215" width="130" height="38" rx="6"/><text x="655" y="239">«abstrata» Atuador</text>
  </g>
  <path class="line" d="M75 60L300 115M205 60L320 113M335 60L360 113M465 60L420 113M625 60L440 113M90 213L310 167M265 213L350 167" style="marker-end:none"/>
</svg>
:::

Para o iniciante é difícil perceber **como isso é horrível**. Muitos erros graves estão escondidos — e só aparecem quando você tenta **codificar** e vê que o código é absurdo. Vamos catalogá-los.

### Erro 1 — Métodos ausentes

Olhe o diagrama: **não há um único método**. Estamos escrevendo um programa, e programas **são comportamento**! Quando um diagrama não tem métodos, o projetista pode estar **dividindo o software em algo que não é comportamento**. Divisões que não se baseiam em comportamento são quase sempre erros significativos: **o comportamento é a primeira pista de como dividir o software**.

### Erro 2 — Classes vapor

Imagine os métodos da classe `Luz`: claramente `Acender()` e `Apagar()`. Como seriam implementados?

```cs
public class Luz
{
    public void Acender() { Jardim1.Api.AcenderIndicador(Estado.Ligado); }
    public void Apagar()  { Jardim1.Api.AcenderIndicador(Estado.Desligado); }
}
```

Esta classe **não tem variáveis** (estranho: objetos costumam ter estado) e seus métodos só **delegam** para a API. É um **adaptador sem utilidade**. O mesmo vale para `Botao`, `Bomba`, `Valvula`: poderiam ser **removidas sem alterar** a lógica do `Irrigador`, que chamaria a API diretamente. Rebaixadas da posição "eminente" de contêineres a **espaços reservados**, são **classes vapor** — evaporam quando você examina o código.

### Erro 3 — Abstração imaginária

E as bases `Sensor` e `Atuador`? À primeira vista parecem sensatas. Mas pergunte: **quem as usa?**

```cs
public interface ISensor   { int Ler(); }          // quem chama isso? ninguém.
public interface IAtuador  { void Ligar(); void Desligar(); }
```

Nenhuma classe do sistema usa `ISensor` ou `IAtuador`. Uma interface **sem usuários e sem código** é oficialmente inútil. Pior: o retorno de `Ler()` é **ambíguo** — o nível tem 2 valores possíveis; a posição, 2; a umidade, 3. O melhor contrato possível é "devolve um `int`": **nenhum contrato**.

::: regra Abstrações precisam de clientes
Seres humanos veem abstrações em todo lugar, mas **muitas não merecem virar classes base**. O teste: **quem usa essa abstração?** Se ninguém a usa polimorficamente e ela não compartilha código, ela não tem razão de existir.
:::

O que aconteceu? Leu-se a especificação, **caçaram-se os substantivos**, inferiram-se relações e desenhou-se. Se esse "modelo" virasse arquitetura, teríamos uma classe todo-poderosa cercada de subordinadas vaporosas. Poderíamos escrever o mesmo em **C puro**.

### Erro 4 — Classes deusas

"Todo mundo sabe" que classes deusas são más: **toda a inteligência numa classe só**. Mas muitos modelos **parecem** distribuídos e na verdade são redutos de deuses. No irrigador, só `Irrigador` tem comportamento; as outras são vapor ou abstrações imaginárias. **Parece distribuição; é concentração.**

## Uma solução melhor: recue e encontre o problema essencial

O truque para resolver este (ou qualquer) problema é **recuar e separar o essencial dos detalhes**. Esqueça bombas, válvulas, sensores; foque no problema subjacente: **como se rega um canteiro?**

Resposta simples: **faz-se a água fluir de uma fonte para algo que a recebe**, até que esse algo esteja satisfeito. Dê nomes:

- **`FonteDeAgua`**: de onde vem a água e como se controla o fluxo (começar, pausar, retomar, terminar).
- **`Recebedor`** (canteiro/recipiente): onde a água vai; sabe **quando está satisfeito** e **quando a rega pode ocorrer**.
- E precisamos de **alguém que fale com humanos**: **`PainelDoUsuario`** — detecta o comando e **informa** o status.

No Jardim-1, bomba + válvula + sensor de nível fazem o papel de `FonteDeAgua`; o sensor de umidade + o sensor de posição do regador fazem o de `Recebedor`; o botão + o indicador, o de `PainelDoUsuario`.

### Erro 5 — Linhas cruzadas

Uma primeira tentação é ligar `FonteDeAgua` → `Recebedor` por uma associação rotulada **"fluxo de água"**: porque a água física flui de um a outro. Está **errado**.

::: regra Associações são caminhos de mensagens
Uma associação UML mostra **por onde mensagens são enviadas** — **não** o fluxo de objetos **físicos**. O fato de a água sair da bomba e chegar ao canteiro **não** significa que `FonteDeAgua` deva **conhecer** `Recebedor`. Misturar o domínio **físico** com o domínio **lógico** é o erro das **linhas cruzadas**.
:::

Se for o `Recebedor` quem avisa "comece/pare o fluxo", então a dependência é **ao contrário**: `Recebedor` → `FonteDeAgua`.

## Descobrindo o comportamento com casos de uso

Agora, com as três classes, perguntamos: **quem faz o quê?** Cada caso de uso revela mensagens.

::: passos Os casos de uso revelam as mensagens
**Caso 1 — O usuário aperta Regar.** Quem detecta? O `PainelDoUsuario`. Antes de começar, ele **pergunta** à `FonteDeAgua` e ao `Recebedor`: *"está pronto?"* (`EstaPronto()`). Se um dos dois responder não, o painel **recusa** (e, por exemplo, pisca o indicador).

::: html
<svg viewBox="0 0 700 160"><g text-anchor="middle" font-size="13"><rect class="f-prim" x="10" y="55" width="150" height="40" rx="4"/><text x="85" y="80" text-decoration="underline">: PainelDoUsuario</text><rect class="f-info" x="290" y="10" width="140" height="40" rx="4"/><text x="360" y="35" text-decoration="underline">: FonteDeAgua</text><rect class="f-good" x="290" y="105" width="140" height="40" rx="4"/><text x="360" y="130" text-decoration="underline">: Recebedor</text></g><path class="arrow" d="M162 62L288 34"/><text x="210" y="42" font-size="11" class="t-mono">1: EstaPronto()</text><path class="arrow" d="M162 88L288 120"/><text x="205" y="125" font-size="11" class="t-mono">2: EstaPronto()</text></svg>
:::
===
**Se os dois estão prontos**, o painel manda `Iniciar()` à fonte (e, para que o recebedor também se prepare, `Iniciar()` a ele).

::: html
<svg viewBox="0 0 700 160"><g text-anchor="middle" font-size="13"><rect class="f-prim" x="10" y="55" width="150" height="40" rx="4"/><text x="85" y="80" text-decoration="underline">: PainelDoUsuario</text><rect class="f-info" x="290" y="10" width="140" height="40" rx="4"/><text x="360" y="35" text-decoration="underline">: FonteDeAgua</text><rect class="f-good" x="290" y="105" width="140" height="40" rx="4"/><text x="360" y="130" text-decoration="underline">: Recebedor</text></g><path class="arrow" d="M162 62L288 34"/><text x="210" y="42" font-size="11" class="t-mono">3: Iniciar()</text><path class="arrow" d="M162 88L288 120"/><text x="205" y="125" font-size="11" class="t-mono">4: Iniciar()</text></svg>
:::
===
**Caso 2 — O regador sai do lugar.** Quem percebe? O `Recebedor` (ele monitora a posição). Ele manda `Pausar()` à `FonteDeAgua`; quando o regador volta, manda `Retomar()`.

::: html
<svg viewBox="0 0 700 100"><g text-anchor="middle" font-size="13"><rect class="f-good" x="10" y="30" width="140" height="40" rx="4"/><text x="80" y="55" text-decoration="underline">: Recebedor</text><rect class="f-info" x="330" y="30" width="140" height="40" rx="4"/><text x="400" y="55" text-decoration="underline">: FonteDeAgua</text></g><path class="arrow" d="M152 42H328"/><text x="240" y="36" text-anchor="middle" font-size="11" class="t-mono">1: Pausar()</text><path class="arrow" d="M328 62H152"/><text x="240" y="80" text-anchor="middle" font-size="11" class="t-mono">2: Retomar()</text></svg>
:::
===
**Caso 3 — A rega termina.** Quem sabe? Na versão do Jardim-1, o **sensor de umidade** (no `Recebedor`) detecta o solo encharcado; num outro hardware poderia ser o **reservatório** (na `FonteDeAgua`) acusando que a água acabou. No domínio **abstrato** nenhum dos dois é candidato óbvio — **então ignoramos o problema**: qualquer um dos dois pode enviar `Terminou()` aos demais. Quem precisa saber? O **painel** (acende a luz), a **fonte** (fecha tudo) e o **recebedor** (para de monitorar umidade).
===
**Caso 4 — O usuário "confirma" a rega.** O `Recebedor` detecta que o regador foi retirado após o fim (o usuário já usou/guardou) e avisa o painel: `Concluido()`; o painel apaga o indicador.
:::

Com esses casos temos o **diagrama de classes** (setas = mensagens): `PainelDoUsuario` → `FonteDeAgua`, `PainelDoUsuario` → `Recebedor`, `Recebedor` → `FonteDeAgua`, `Recebedor` → `PainelDoUsuario`, `FonteDeAgua` → `Recebedor`/`PainelDoUsuario` (via `Terminou()`).

Observe: **três áreas de responsabilidade** distintas, mensagens **equilibradas**, **nenhuma classe deusa**, **nenhuma classe vapor**.

## Implementando o modelo abstrato — sem acoplá-lo ao hardware

Será que agora basta implementar os três métodos chamando a API do hardware? **Seria uma vergonha!** Capturamos a **essência de regar**; amarrá-la ao Jardim-1 destruiria o projeto.

::: regra DIP em ação
**Nenhuma das três classes de alto nível deve saber qualquer coisa sobre o Jardim-1.** A **diretiva** ("só começar se ambos estão prontos; pausar se o regador sair") não pode depender do **detalhe** (a API do hardware).
:::

Aplicamos o **DIP** e o **SRP**:

- `FonteDeAgua`, `Recebedor` e `PainelDoUsuario` ficam com **todo o código reaproveitável** (a lógica de alto nível) e declaram **métodos abstratos** para o que depende da plataforma;
- as **derivadas** `FonteJ1`, `RecebedorJ1`, `PainelJ1` implementam **somente o que está inextricavelmente ligado ao hardware**.

```cs Alto nível — não sabe nada do Jardim-1
public abstract class FonteDeAgua
{
    public abstract bool EstaPronta();
    public abstract void Iniciar();
    public abstract void Pausar();
    public abstract void Retomar();
    public abstract void Encerrar();
}

public abstract class Recebedor
{
    public abstract bool EstaPronto();
    public abstract void Iniciar();
}

public class PainelDoUsuario
{
    private FonteDeAgua fonte;
    private Recebedor recebedor;

    public void Ligar(FonteDeAgua fonte, Recebedor recebedor) { this.fonte = fonte; this.recebedor = recebedor; }

    // diretiva de alto nível: vale para QUALQUER hardware
    protected void ComecarARegar()
    {
        if (fonte.EstaPronta() && recebedor.EstaPronto())
        {
            fonte.Iniciar();
            recebedor.Iniciar();
        }
    }

    public virtual void Terminou() { }
    public virtual void Concluido() { }
}
```

```cs Baixo nível — só o que é do Jardim-1
public class PainelJ1 : PainelDoUsuario, IPollable
{
    private readonly IApiJardim1 api;
    public PainelJ1(IApiJardim1 api) { this.api = api; }

    public void Poll()
    {
        if (api.LerBotao() == EstadoBotao.Apertado)
            ComecarARegar();                 // a regra está na base; aqui só o "como detectar o botão"
    }

    public override void Terminou()  { api.AcenderIndicador(Estado.Ligado); }
    public override void Concluido() { api.AcenderIndicador(Estado.Desligado); }
}

public class FonteJ1 : FonteDeAgua
{
    private readonly IApiJardim1 api;
    public FonteJ1(IApiJardim1 api) { this.api = api; }

    public override bool EstaPronta() { return api.LerNivel() == NivelReservatorio.ComAgua; }
    public override void Iniciar()    { api.AbrirValvula(Estado.Ligado); api.LigarBomba(Estado.Ligado); }
    public override void Pausar()     { api.LigarBomba(Estado.Desligado); api.AbrirValvula(Estado.Desligado); }
    public override void Retomar()    { Iniciar(); }
    public override void Encerrar()   { Pausar(); }
}
```

Por que o método protegido `ComecarARegar()` **na base** em vez de a derivada chamar `Iniciar()` diretamente? Porque **as consultas `EstaPronta`/`EstaPronto` e as chamadas a `Iniciar`** são **diretivas de alto nível** — valem **independentemente** do hardware — e **não podem** ficar acopladas à derivada. É o **SRP** de novo: **mantenha o máximo de código nas classes de alto nível** e, nas derivadas, **apenas o que é inseparável do dispositivo**.

::: figura A estrutura final
<svg viewBox="0 0 760 300" role="img" aria-label="Três classes abstratas no topo; derivadas específicas do Jardim-1 embaixo; a API do hardware só é usada embaixo">
  <text x="380" y="18" text-anchor="middle" font-size="12" class="t-dim">ALTO NÍVEL — não conhece o Jardim-1</text>
  <g text-anchor="middle" font-size="13"><rect class="f-prim" x="40" y="30" width="190" height="50" rx="8"/><text x="135" y="60" font-weight="800">PainelDoUsuario</text>
  <rect class="f-info" x="285" y="30" width="190" height="50" rx="8"/><text x="380" y="60" font-weight="800">FonteDeAgua {A}</text>
  <rect class="f-good" x="530" y="30" width="190" height="50" rx="8"/><text x="625" y="60" font-weight="800">Recebedor {A}</text></g>
  <path d="M10 125H750" stroke="var(--ink-3)" stroke-dasharray="6 4"/><text x="380" y="118" text-anchor="middle" font-size="11" class="t-dim">— fronteira de abstração (DIP) —</text>
  <g text-anchor="middle" font-size="13"><rect class="f-plain" x="40" y="150" width="190" height="50" rx="8"/><text x="135" y="180" font-weight="700">PainelJ1</text>
  <rect class="f-plain" x="285" y="150" width="190" height="50" rx="8"/><text x="380" y="180" font-weight="700">FonteJ1</text>
  <rect class="f-plain" x="530" y="150" width="190" height="50" rx="8"/><text x="625" y="180" font-weight="700">RecebedorJ1</text></g>
  <path class="line" d="M135 148V82M380 148V82M625 148V82" style="marker-end:url(#arr2)"/>
  <rect class="f-warn" x="285" y="245" width="190" height="42" rx="8"/><text x="380" y="271" text-anchor="middle" font-weight="700">«interface» IApiJardim1</text>
  <path class="arrow" d="M135 202V262H283" style="stroke:var(--warn);marker-end:url(#arr)"/><path class="arrow" d="M380 202V243"/><path class="arrow" d="M625 202V262H477"/>
</svg>
:::

## Polling × multitarefa: uma decisão para o último minuto

Resta uma pergunta: **como o fluxo de controle chega** ao `PainelJ1` para ele ler o botão? Muitas equipes **travam aqui** por uma hora: *"vamos supor um sistema operacional multitarefa?"* ou *"vamos sondar os sensores (polling)?"* O erro delas: **essa escolha é irrelevante para o projeto**.

::: regra Adie decisões que dá para adiar
Suponha que as mensagens podem chegar de forma **assíncrona**, como se cada objeto fosse uma thread independente; **a escolha entre polling e multitarefa pode ser feita no último minuto**, sem prejudicar o projeto.
:::

Para uma plataforma mínima, **sem threads**, uma solução simples: uma interface com **um único método**.

```cs
public interface IPollable { void Poll(); }

public class Programa
{
    public static void Main()
    {
        IApiJardim1 api = new ApiJardim1();
        var painel = new PainelJ1(api);
        var fonte = new FonteJ1(api);
        var recebedor = new RecebedorJ1(api);
        painel.Ligar(fonte, recebedor);
        recebedor.Ligar(fonte, painel);

        var todos = new List<IPollable> { painel, recebedor };    // quem precisa olhar sensores
        while (true)
            foreach (IPollable p in todos) p.Poll();               // laço de sondagem
    }
}
```

Se amanhã a plataforma ganhar threads, troca-se o laço por uma thread/Timer por objeto — **sem mexer** nas classes de alto nível. E se for um dia assíncrono (`async`/`await`)? Idem. **O projeto sobrevive à decisão.**

## Resumo das heurísticas

| Cheiro / erro | Pergunta de diagnóstico | Remédio |
|---|---|---|
| **Métodos ausentes** | "Onde está o comportamento?" | Comece por **colaborações** (sequência/colaboração) e **casos de uso** |
| **Classes vapor** | "Esta classe tem estado ou só delega?" | Remova ou funda; dê-lhe responsabilidade real |
| **Abstração imaginária** | "Quem usa esta base/interface?" | Elimine se não há clientes nem código comum |
| **Classe deusa** | "Quantas classes realmente têm comportamento?" | Distribua responsabilidades por **comportamento**, não por substantivos |
| **Linhas cruzadas** | "A seta mostra mensagem ou fluxo físico?" | Associações = **caminhos de mensagens** |
| **Alto nível acoplado ao hardware** | "A política conhece o dispositivo?" | **DIP + SRP**: política na base; detalhe na derivada |
| **Brigas sobre polling × threads** | "Isso muda a lógica?" | **Adie**; suponha assíncrono |

::: pense Substantivos não são classes
Caçar **substantivos** no texto dos requisitos e transformá-los em classes é um atalho enganoso: leva a classes sem comportamento. Procure primeiro **verbos e colaborações**: *quem decide*, *quem avisa*, *quem executa*. As classes surgem como **papéis** nessas conversas.
:::

::: exercicio Critique este modelo
Um colega projetou um **sistema de elevador**: `Elevador` (deus), `Botao`, `Porta`, `Motor`, `Andar`, todos só com getters/setters, e uma base `Componente` com `Ligar()`/`Desligar()` que ninguém chama polimorficamente.

1. Que erros aparecem?
2. Como você recomeçaria?

--- resposta
1. **Métodos ausentes** (getters/setters), **classes vapor** (`Botao`, `Porta`, `Motor` só repassam), **abstração imaginária** (`Componente` sem clientes), **classe deusa** (`Elevador` concentra toda a lógica).
2. Recuar até o **problema essencial** ("atender chamadas movendo uma cabine entre andares com segurança"). Descobrir papéis por **casos de uso**: *Passageiro chama*, *Cabine se move*, *Porta emperra*. Por exemplo: `Despachante` (decide qual cabine atende qual chamada), `Cabine` (move-se, abre/fecha portas quando é seguro), `Painel` (comunicação com o usuário). Mensagens: `Despachante → Cabine.Ir(andar)`, `Cabine → Painel.Chegou()`. Detalhes de motor/portas atrás de **interfaces** implementadas na borda (DIP), com a política na base.
:::

::: resumo
- Os mesmos erros se repetem: **métodos ausentes**, **classes vapor**, **abstrações imaginárias**, **classes deusas**, **linhas cruzadas**.
- Projete a partir do **comportamento**: casos de uso → colaborações → classes (papéis), não a partir de substantivos.
- Recue ao **problema essencial** (como se faz isso?) e abstraia: fonte, recebedor, painel.
- **Associações = caminhos de mensagens**, não fluxo físico.
- Aplique **DIP + SRP**: a política (alto nível) em classes abstratas/base; só o **inextricável do dispositivo** nas derivadas.
- **Adie** decisões sobre plataforma (polling × threads); `IPollable` e um laço bastam no início.
:::

::: quiz
? O que significa o erro "métodos ausentes" em um diagrama de classes?
+ O diagrama divide o software em algo que não é comportamento
- O compilador não gerou métodos
- As classes são muito grandes
- Faltam construtores
> O comportamento é a primeira pista de como dividir o software.

? O que é uma "classe vapor"?
+ Classe sem estado cujos métodos apenas delegam, sem lógica real
- Classe muito grande
- Classe abstrata
- Classe estática
> Ela "evapora" quando se examina o código.

? Por que uma interface `ISensor` com `int Ler()` que ninguém usa polimorficamente é problemática?
+ É uma abstração imaginária: sem clientes e sem contrato claro
- Porque interfaces não podem retornar int
- Porque sensores não existem
- Porque deveria ser classe concreta
> Abstrações precisam de usuários.

? O que são "linhas cruzadas"?
+ Associações que refletem fluxos físicos em vez de caminhos de mensagens
- Setas que se cruzam no diagrama
- Dependências cíclicas entre pacotes
- Classes ligadas por herança múltipla
> Misturam o domínio físico com o lógico.

? Qual a razão do método protegido `ComecarARegar()` na base?
+ Manter a diretiva de alto nível (verificar prontidão e iniciar) fora da derivada, independente do hardware
- Esconder o código do usuário
- Aumentar o desempenho
- Permitir herança múltipla
> SRP/DIP: máximo de código na classe de alto nível.

? Como lidar com a discussão "polling ou multitarefa"?
+ Adiar: supor mensagens assíncronas e decidir no último minuto, sem afetar o projeto
- Decidir no início do projeto, para travar a arquitetura
- Usar sempre multitarefa
- Evitar sensores
> A escolha é um detalhe de plataforma.

? Qual é a vantagem de projetar a partir de casos de uso em vez de substantivos?
+ Revela quem envia mensagens a quem (comportamento), evitando classes sem papel real
- Gera menos diagramas
- Evita o uso de interfaces
- Dispensa testes
> As classes surgem como papéis nas colaborações.
:::

::: cartas
Métodos ausentes :: Diagrama sem comportamento: sinal de divisão errada.
Classe vapor :: Classe sem estado cujos métodos só delegam.
Abstração imaginária :: Classe base/interface sem clientes e sem código comum.
Classe deusa :: Classe que concentra toda a inteligência; as demais são vapor.
Linhas cruzadas :: Confundir fluxo físico com associação lógica entre classes.
Diretiva de alto nível :: Regra do negócio independente da plataforma.
IPollable :: Interface com `Poll()` para varrer sensores num laço simples.
Decisão adiável :: Escolhas de plataforma (polling×threads) que não devem moldar o projeto.
:::
