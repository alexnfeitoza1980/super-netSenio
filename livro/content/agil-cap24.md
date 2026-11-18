---
id: agil-cap24
order: 40
kind: capitulo
num: 24
group: C# · Parte 8 · Projeto ágil e princípios SOLID
title: Princípio da Segregação de Interface (ISP)
subtitle: "Clientes não devem depender de métodos que não usam" — interfaces gordas, poluição de interface e como separá-las (herança de interface, adaptadores).
minutes: 45
hue: 15
objectives:
- Enunciar o **ISP** e reconhecer **interfaces gordas** e **poluição de interface**
- Entender por que **clientes diferentes** exercem **forças** diferentes sobre uma interface
- Separar interfaces com **múltiplas interfaces implementadas** ou com **adaptadores/delegação**
- Aplicar o ISP com **moderação** (evitar a explosão de interfaces)
origem: livros/02-projeto-agil-csharp/content/39-cap24.md
---

::: ilustracao img/agil/cap24.svg | ISP: ninguém deve depender de botões que não usa; interfaces pequenas e específicas.
:::

## O problema das interfaces gordas

Uma classe tem uma **interface gorda** quando seus métodos podem ser divididos em **grupos**, cada grupo atendendo a **clientes diferentes**: alguns clientes usam um grupo; outros, outro. O ISP reconhece que há objetos que *precisam* de uma interface não coesa — mas diz que **os clientes não devem enxergá-la como uma única classe**. Eles devem enxergar **interfaces pequenas e coesas**.

::: regra ISP — Interface Segregation Principle
**Os clientes não devem ser forçados a depender de métodos que não utilizam.**
:::

Por quê? Porque quem depende de um método que não usa fica sujeito às **mudanças** nesse método — que foram pedidas por **outros** clientes. O resultado é um **acoplamento involuntário** entre clientes que nada têm a ver uns com os outros.

## Poluição de interface: a porta com temporizador

Um sistema de segurança tem objetos `Porta` com `Trancar()`, `Destrancar()` e `EstaAberta()`. `Porta` é uma interface para que os clientes usem qualquer porta sem depender de implementações.

Agora, uma implementação especial, `PortaTemporizada`, deve disparar um **alarme** se ficar aberta por muito tempo. Para isso, ela conversa com um `Temporizador`, que chama de volta objetos que implementam `IClienteDeTemporizador` (com um método `AoExpirar()`).

A solução "óbvia": **fazer `Porta` herdar de `IClienteDeTemporizador`** — assim qualquer porta pode se registrar.

::: figura Interface poluída × interfaces segregadas
<svg viewBox="0 0 760 280" role="img" aria-label="Porta herda de IClienteDeTemporizador (ruim); PortaTemporizada implementa duas interfaces (bom)">
  <text x="170" y="18" text-anchor="middle" font-weight="800" class="t-bad">Poluída</text>
  <rect class="f-info" x="90" y="35" width="170" height="40" rx="8"/><text x="175" y="60" text-anchor="middle" font-size="12">«interface» IClienteDeTemporizador</text>
  <path class="line" d="M175 112V77" style="marker-end:url(#arr2)"/>
  <rect class="f-bad" x="90" y="112" width="170" height="40" rx="8"/><text x="175" y="137" text-anchor="middle" font-size="13" font-weight="700">«interface» Porta</text>
  <path class="line" d="M130 188V154M220 188V154" style="marker-end:url(#arr2)"/>
  <rect class="f-plain" x="60" y="188" width="110" height="40" rx="8"/><text x="115" y="213" text-anchor="middle" font-size="12">PortaComum</text><text x="115" y="244" text-anchor="middle" font-size="10" class="t-bad">AoExpirar() vazio!</text>
  <rect class="f-plain" x="190" y="188" width="140" height="40" rx="8"/><text x="260" y="213" text-anchor="middle" font-size="12">PortaTemporizada</text>
  <text x="560" y="18" text-anchor="middle" font-weight="800" class="t-good">Segregada</text>
  <rect class="f-info" x="410" y="35" width="150" height="40" rx="8"/><text x="485" y="60" text-anchor="middle" font-size="13" font-weight="700">«interface» Porta</text>
  <rect class="f-info" x="580" y="35" width="170" height="40" rx="8"/><text x="665" y="60" text-anchor="middle" font-size="12" font-weight="700">«interface» IClienteDeTemporizador</text>
  <path class="line" d="M485 130V77M665 130V77" style="marker-end:url(#arr2)"/>
  <rect class="f-good" x="500" y="130" width="170" height="40" rx="8"/><text x="585" y="155" text-anchor="middle" font-size="13" font-weight="700">PortaTemporizada</text>
  <path class="line" d="M585 130L520 77M585 130L640 77" style="marker-end:url(#arr2)"/>
  <rect class="f-plain" x="410" y="190" width="110" height="40" rx="8"/><text x="465" y="215" text-anchor="middle" font-size="12">PortaComum</text>
  <path class="line" d="M465 190L485 77" style="marker-end:url(#arr2)"/>
  <text x="560" y="262" text-anchor="middle" font-size="12" class="t-dim">cada cliente enxerga só a interface de que precisa</text>
</svg>
:::

```cs Poluída
public interface IClienteDeTemporizador { void AoExpirar(); }

public interface Porta : IClienteDeTemporizador     // 🚫 toda porta agora "é cliente de timer"
{
    void Trancar();
    void Destrancar();
    bool EstaAberta();
}
```

Problemas dessa solução:

1. `Porta` passa a depender de `IClienteDeTemporizador`, embora **nem toda porta** precise de temporização — a abstração original de `Porta` nada tinha a ver com tempo.
2. As portas sem temporização têm de **implementar `AoExpirar()` de forma degenerada** (vazio ou lançando exceção): um **risco de violação do LSP**.
3. Qualquer aplicativo que use portas simples precisa **importar a definição** de `IClienteDeTemporizador`, mesmo sem usá-la: **complexidade e dependência desnecessárias**.

Esse é o sintoma chamado **poluição de interface** — comum em linguagens estaticamente tipadas (C#, Java, C++): a interface base foi obrigada a incluir um método **só para beneficiar uma de suas subclasses**. Repetida a prática, a base engorda a cada necessidade de uma derivada — e as implementações "padrão" vazias vão minando o LSP.

## Clientes diferentes ⇒ interfaces diferentes

`Porta` e `IClienteDeTemporizador` são usadas por **clientes totalmente distintos**: o `Temporizador` usa a segunda; o código que lida com portas usa a primeira. Como os clientes são separados, **as interfaces também devem ser**.

Isso porque **os clientes exercem forças sobre suas interfaces**. Normalmente pensamos na força que vai da interface para o cliente ("se a interface mudar, os clientes sofrem"). Mas existe a força **contrária**: um cliente pode **forçar uma mudança** na interface.

::: pense Uma força que vem do cliente
`PortaTemporizada` registra um prazo quando a porta abre. Mas a porta fecha e reabre antes do prazo antigo expirar; então há **dois** prazos pendentes e, quando o primeiro vence, o alarme dispara **à toa**. A correção natural é incluir um **identificador** em cada pedido: `Registrar(prazo, id, cliente)` e `AoExpirar(id)`. É uma mudança em `IClienteDeTemporizador`.

Se `Porta` herdasse dessa interface, **a correção se espalharia** para todas as portas e todos os seus clientes — que nada têm a ver com temporização! Isso cheira a **rigidez** e **viscosidade**. Quando uma mudança numa parte afeta outras totalmente sem relação, o custo e o risco da mudança se tornam imprevisíveis.
:::

## Como separar quando o objeto precisa das duas interfaces?

Um mesmo objeto, `PortaTemporizada`, tem **duas interfaces** usadas por dois clientes, e as duas manipulam os **mesmos dados** (devem ficar juntas). Como obedecer ao ISP?

O truque: **os clientes não precisam acessar o objeto pela *sua* interface**; podem acessá-lo por uma **interface base** ou por **delegação**.

### Solução 1 — implementar várias interfaces (preferida)

```cs
public interface Porta { void Trancar(); void Destrancar(); bool EstaAberta(); }
public interface IClienteDeTemporizador { void AoExpirar(int idDoPrazo); }

public class PortaTemporizada : Porta, IClienteDeTemporizador
{
    private readonly Temporizador temporizador;
    private int ultimoPrazo;

    public PortaTemporizada(Temporizador t) { temporizador = t; }

    public void Trancar() { /* ... */ }
    public void Destrancar() { /* ... */ }
    public bool EstaAberta() { /* ... */ return false; }

    public void AoAbrir() { temporizador.Registrar(30, ++ultimoPrazo, this); }   // passa a si mesma
    public void AoExpirar(int id) { if (id == ultimoPrazo && EstaAberta()) SoarAlarme(); }
    private void SoarAlarme() { /* ... */ }
}
```

Os clientes de `Porta` e o `Temporizador` usam **o mesmo objeto por meio de interfaces distintas**; nenhum depende de `PortaTemporizada`. É a solução **normalmente preferida** em C#: classe implementa quantas interfaces quiser.

### Solução 2 — adaptador (delegação)

Quando é preciso **transformar** uma interface em outra, ou quando **transformações diferentes** são necessárias em momentos diferentes, crie um objeto que implementa `IClienteDeTemporizador` e **delega** para a porta:

```cs
public interface IPortaTemporizada : Porta { void AoExpirarPrazo(int id); }

public class AdaptadorDePortaTemporizada : IClienteDeTemporizador
{
    private readonly IPortaTemporizada porta;
    public AdaptadorDePortaTemporizada(IPortaTemporizada porta) { this.porta = porta; }
    public void AoExpirar(int id) { porta.AoExpirarPrazo(id); }     // traduz e delega
}

// ao abrir: temporizador.Registrar(30, id, new AdaptadorDePortaTemporizada(this));
```

Custo: **um objeto a mais** a cada registro e um pouco de memória e tempo. Em sistemas embarcados de tempo real isso pode importar; em aplicativos de negócio, quase nunca.

## Exemplo maior: a interface de um caixa eletrônico

A interface de usuário (`IUi`) de um caixa eletrônico precisa ser **muito flexível**: a saída pode ser em tela, em braille ou por voz; os textos, traduzidos. A ideia natural: uma interface abstrata com **um método por mensagem**:

```cs Interface gorda
public interface IUi
{
    void PedirValorDoDeposito();
    void PedirValorDoSaque();
    void PedirValorDaTransferencia();
    void InformarSaldoInsuficiente();
    // a cada nova transação, mais métodos...
}
```

Cada transação (`Deposito`, `Saque`, `Transferencia`) só usa **alguns** desses métodos. Se criarmos `PagamentoDeConta`, precisaremos **adicionar métodos** em `IUi` — e **todas as transações existentes** e quem depende de `IUi` serão recompilados e, em sistemas entregues em componentes, **reentregues**, sem que sua lógica tenha mudado. Esse é o cheiro de **viscosidade**.

A saída: **segregar** a interface por **grupo de clientes**:

```cs Segregada
public interface IUiDeposito      { void PedirValorDoDeposito(); }
public interface IUiSaque         { void PedirValorDoSaque(); void InformarSaldoInsuficiente(); }
public interface IUiTransferencia { void PedirValorDaTransferencia(); }

// a UI concreta implementa todas
public interface IUi : IUiDeposito, IUiSaque, IUiTransferencia { }

public class Deposito
{
    private readonly IUiDeposito ui;                      // só enxerga o que usa
    public Deposito(IUiDeposito ui) { this.ui = ui; }
    public void Executar() { /* ... */ ui.PedirValorDoDeposito(); /* ... */ }
}
```

Uma nova transação exige uma **nova interface** e a `IUi` agregadora muda — mas **quase ninguém** usa `IUi` diretamente (só quem monta o sistema), então o impacto é mínimo. As transações antigas **não** são tocadas.

### Cuidado: o "pacote de globais" desfaz tudo

É tentador reunir as UIs numa classe estática `UiGlobais` com `Deposito`, `Saque` e `Transferencia`. **Não faça**: quem usar **qualquer uma** delas passa a depender **transitivamente de todas** — exatamente o que o ISP quer evitar. Você teria reunido o que tanto trabalho deu para separar.

### Parâmetros: pedir o que se usa

Uma função `G` precisa de `IUiDeposito` e `IUiTransferencia`. Duas assinaturas:

```cs
void G(IUiDeposito deposito, IUiTransferencia transferencia);   // "polidária" — prefira
void G(IUi ui);                                                  // "monadária" — tentadora, pior
```

A segunda força `G` (e seus clientes) a depender de **todas** as interfaces dentro de `IUi`; se `IUiSaque` mudar, `G` pode ser afetada. Além disso, ela **codifica** que os dois lados são o mesmo objeto — informação de que `G` não precisa. A chamada `G(ui, ui)` parece estranha, mas é **mais honesta**.

::: dica Agrupe clientes, não crie uma interface por método
Muitas vezes os clientes se agrupam pelos métodos de serviço que chamam. Crie interfaces **por grupo de clientes**, não por cliente individual: reduz o número de interfaces. Se os grupos se **sobrepõem pouco**, mantenha as interfaces separadas e repita as poucas funções comuns em cada uma — a classe servidora as herda e implementa **uma vez**.
:::

### Evolução: acrescente interfaces em vez de alterar as existentes

Ao manter aplicativos, as interfaces mudam e isso pode forçar recompilação e redistribuição enormes. Em vez de alterar uma interface existente, **acrescente uma nova** ao objeto. Os clientes antigos nada percebem; os novos consultam o objeto:

```cs
void Cliente(IServico s)
{
    if (s is IServicoV2 v2)        // quer a novidade? pergunte
        v2.NovoRecurso();
    s.RecursoAntigo();
}
```

::: cuidado Não exagere
Como os demais princípios, o ISP tem limite. A visão de **uma classe com centenas de interfaces**, algumas segregadas por cliente e outras por versão, é assustadora. Segregue quando **houver sintoma** (clientes afetados por mudanças que não lhes dizem respeito, implementações degeneradas, dependências desnecessárias).
:::

## Sinais de que é hora de segregar

- Implementações com `throw new NotImplementedException()` ou métodos vazios "só para cumprir a interface".
- Interfaces com **dezenas de membros** usados por grupos diferentes.
- Alterar a interface para um cliente **recompila** ou **quebra** outros clientes.
- Classes `Mock` enormes nos testes, porque a interface exige "implementar tudo".
- Nomes como `IServico`, `IManager`, `IUtil` com assuntos misturados.

::: exercicio Segregue esta interface
```cs
public interface IImpressoraMultifuncional
{
    void Imprimir(Documento d);
    void Digitalizar(Documento d);
    void EnviarFax(Documento d, string numero);
    void Grampear();
}
```
Uma "ImpressoraSimples" só imprime. Como projetar?

--- resposta
```cs
public interface IImpressora   { void Imprimir(Documento d); }
public interface IScanner      { void Digitalizar(Documento d); }
public interface IFax          { void EnviarFax(Documento d, string numero); }
public interface IGrampeador   { void Grampear(); }

public class ImpressoraSimples : IImpressora { public void Imprimir(Documento d) { } }
public class Multifuncional : IImpressora, IScanner, IFax, IGrampeador { /* todos */ public void Imprimir(Documento d){} public void Digitalizar(Documento d){} public void EnviarFax(Documento d,string n){} public void Grampear(){} }
```
`ImpressoraSimples` não precisa implementar fax ou scanner (sem métodos degenerados — o LSP agradece). Quem só imprime pede `IImpressora`.
:::

::: resumo
- **ISP:** nenhum cliente deve depender de métodos que não usa.
- **Interface gorda** atende grupos de clientes diferentes ⇒ acoplamento involuntário e mudanças em cascata (rigidez, viscosidade).
- **Poluição de interface:** engordar a base só para beneficiar uma derivada; leva a implementações degeneradas (risco para o LSP).
- Separe interfaces **por grupo de clientes**; o objeto pode implementar **várias** delas (ou usar **adaptador/delegação**).
- Prefira **pedir só o que usa** nos parâmetros; evite agrupar tudo em "globais".
- Evolua **adicionando** interfaces, não alterando as existentes.
- Aplique com **moderação** e quando houver sintoma.
:::

::: quiz
? Qual é o enunciado do ISP?
+ Clientes não devem ser forçados a depender de métodos que não utilizam
- Classes devem ter um único motivo para mudar
- Módulos de alto nível não dependem dos de baixo nível
- Subtipos devem ser substituíveis
> Interfaces devem ser específicas para os clientes.

? O que é "poluição de interface"?
+ Acrescentar à interface base um método só para beneficiar uma subclasse
- Ter muitos comentários na interface
- Usar nomes longos
- Colocar interfaces em muitos arquivos
> Gera interfaces gordas e implementações degeneradas.

? Por que fazer `Porta` herdar de `IClienteDeTemporizador` é problemático?
+ Nem toda porta precisa de temporização e todas seriam afetadas por mudanças no timer
- Porque interfaces não podem herdar
- Porque o C# proíbe métodos `void`
- Porque `Temporizador` é abstrato
> As portas simples passam a depender de algo que não usam.

? Qual solução em C# costuma ser preferida para o mesmo objeto servir a dois clientes com interfaces distintas?
+ A classe implementar as duas interfaces
- Criar uma classe estática global
- Copiar a classe
- Usar `object` nos parâmetros
> Cada cliente vê o objeto pela interface que precisa.

? Quando preferir o adaptador (delegação) à implementação de múltiplas interfaces?
+ Quando é preciso transformar uma interface em outra ou há transformações diferentes em momentos diferentes
- Sempre, porque é mais rápido
- Nunca
- Quando a classe é `static`
> O adaptador traduz entre interfaces.

? Por que reunir as interfaces segregadas numa classe `UiGlobais` desfaz o ISP?
+ Quem usar qualquer uma passa a depender transitivamente de todas
- Porque classes estáticas são proibidas
- Porque deixa o código mais lento
- Porque duplica métodos
> Reacopla o que foi separado.

? Como evoluir uma interface publicada sem quebrar os clientes antigos?
+ Adicionar uma nova interface ao objeto e deixar os novos clientes consultá-la
- Alterar a interface antiga e recompilar tudo
- Apagar métodos antigos
- Trocar o nome da classe
> Clientes antigos continuam intactos.
:::

::: cartas
ISP :: Clientes não devem depender de métodos que não usam.
Interface gorda :: Interface com grupos de métodos usados por clientes diferentes.
Poluição de interface :: Engordar a base para beneficiar apenas uma derivada.
Segregar :: Dividir uma interface em interfaces menores e coesas por grupo de clientes.
Adaptador (delegação) :: Objeto que implementa uma interface e delega para outro, traduzindo chamadas.
Força do cliente :: Um cliente pode exigir mudança numa interface compartilhada, afetando os demais.
Implementação degenerada :: Método vazio/que lança exceção só para cumprir um contrato.
:::
