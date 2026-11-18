---
id: agil-cap27
order: 43
kind: capitulo
num: 27
group: C# · Parte 9 · UML na prática
title: Diagramas de sequência e de classes
subtitle: A notação que você realmente usa — linhas de vida, criação, laços, mensagens assíncronas, associação, herança, estereótipos e multiplicidade — e o valor do minimalismo.
minutes: 55
hue: 230
objectives:
- Desenhar e ler **diagramas de sequência** (linhas de vida, ativações, retorno, criação, laços, condições)
- Representar **mensagens assíncronas**, **threads** e **objetos ativos**, e mensagens para **interfaces**
- Desenhar **diagramas de classes**: compartimentos, visibilidade, associação, herança, interface, abstrata
- Usar **estereótipos**, **multiplicidade** e saber quais adornos **evitar** (agregação, composição…)
origem: livros/02-projeto-agil-csharp/content/42-cap27.md
---

::: ilustracao img/agil/cap27.svg | Sequência mostra quem fala com quem e em que ordem; classes mostram quem sabe o quê.
:::

## Diagramas de sequência: poucos e bons

Os diagramas de sequência são os **modelos dinâmicos mais comuns**. A UML oferece muitos recursos para desenhar diagramas **incompreensíveis**; aqui usaremos o essencial — e com **muita moderação**. Existe a história real de uma equipe que decidiu desenhar um diagrama de sequência **para cada método de cada classe**. Não faça isso: é puro desperdício.

::: regra Use quando houver necessidade imediata
Desenhe um diagrama de sequência quando precisar **explicar a alguém como um grupo de objetos colabora** — ou **visualizar essa colaboração** para você mesmo. É uma ferramenta de vez em quando para afiar a análise, **não** documentação obrigatória.
:::

### Os elementos básicos

::: figura Anatomia de um diagrama de sequência
<svg viewBox="0 0 760 330" role="img" aria-label="Ator, objetos, linhas de vida, mensagens, retorno e ativação">
  <text x="46" y="26" text-anchor="middle" font-size="26">🧍</text>
  <g text-anchor="middle" font-size="13"><rect class="f-prim" x="190" y="8" width="130" height="32" rx="4"/><text x="255" y="29" text-decoration="underline">: PaginaLogin</text>
  <rect class="f-info" x="400" y="8" width="130" height="32" rx="4"/><text x="465" y="29" text-decoration="underline">f : Funcionario</text>
  <rect class="f-good" x="610" y="8" width="130" height="32" rx="4"/><text x="675" y="29">RepositorioFunc.</text></g>
  <path d="M46 46V320M255 46V320M465 46V320M675 46V320" stroke="var(--ink-3)" stroke-dasharray="5 4"/>
  <path class="arrow" d="M50 82H250"/><text x="150" y="74" text-anchor="middle" font-size="12" class="t-mono">Entrar(id, senha)</text>
  <rect class="f-prim" x="249" y="82" width="12" height="190"/>
  <path class="arrow" d="M262 120H670"/><text x="470" y="112" text-anchor="middle" font-size="12" class="t-mono">f := Obter(id)  (método estático)</text>
  <path class="arrow" d="M262 170H460"/><text x="360" y="162" text-anchor="middle" font-size="12" class="t-mono">ok := Validar(senha)</text>
  <path d="M460 195H262" stroke="var(--ink-2)" stroke-dasharray="5 4" stroke-width="1.8" fill="none" marker-end="url(#arr2)"/><text x="360" y="212" text-anchor="middle" font-size="11" class="t-dim">retorno: bool</text>
  <path d="M250 262H52" stroke="var(--ink-2)" stroke-dasharray="5 4" stroke-width="1.8" fill="none" marker-end="url(#arr2)"/><text x="150" y="254" text-anchor="middle" font-size="11" class="t-dim">retorno: resultado</text>
  <text x="560" y="250" font-size="12" class="t-dim">Tempo desce ↓</text>
</svg>
:::

- **Objetos e classes** aparecem no topo. **Objeto** = nome **sublinhado** (`f : Funcionario`); **classe** = sem sublinhar (e uma mensagem para ela indica **método estático**).
- O **bonequinho** (ator) é um objeto **anônimo** — origem e destino das mensagens de fora.
- As **linhas tracejadas** são as **linhas de vida**. Uma mensagem é uma **seta** entre duas linhas de vida, com o **nome** e os **argumentos** entre parênteses.
- **Tempo** corre na vertical: mais embaixo = mais tarde.
- A **ativação** (retângulo fino) mostra quanto tempo um método executa; é **opcional**.
- Seta tracejada = **retorno**. `f := Obter(id)` nomeia o valor devolvido: aquele **é** o objeto `f`.

### Criação e "destruição"

Uma mensagem **sem rótulo** que **termina na caixa** do objeto (não na linha de vida) mostra que ele é **criado**:

```cs
public class FabricaDeFormas
{
    public Forma CriarQuadrado() { return new Quadrado(); }   // a seta termina na caixa de "s : Quadrado"
}
```

Em C# não se destrói objetos explicitamente (o GC cuida); mas é possível **mostrar que terminamos de usar** um objeto: a linha de vida acaba num grande **X**, e a seta que chega ao X significa **liberar para o coletor de lixo** — na prática, `topo = null`.

### Laços simples

Desenhe uma **caixa** em volta das mensagens que se repetem, com a condição entre colchetes no canto. Mas **não capture algoritmos** em diagramas de sequência: eles servem para revelar **conexões entre objetos**, não detalhes de lógica.

## Casos e cenários: pequenos, de uma página

::: cuidado Diagramas gigantes não são lidos
Não desenhe um diagrama de sequência com **vinte objetos** e **centenas de mensagens**: ninguém consegue lê-lo, ninguém o lerá. Cada diagrama deve **caber numa página**, com espaço para texto explicativo, **sem reduzir ícones** a tamanhos minúsculos. E não faça **dezenas ou centenas** deles.
:::

Perguntas antes de desenhar:

1. **Realmente preciso?** Muitas vezes o **código é mais expressivo e econômico**. Veja uma folha de pagamento:

```cs
public class FolhaDePagamento
{
    private readonly IRepositorioDeFuncionarios repositorio;
    private readonly IDestinoDePagamento destino;

    public void Processar()
    {
        foreach (Funcionario f in repositorio.Todos())
        {
            if (f.EhDiaDePagamento())
            {
                decimal bruto = f.CalcularPagamento();
                decimal descontos = f.CalcularDescontos();
                destino.Enviar(f, bruto - descontos);
            }
        }
    }
}
```

   Esse código é **autossuficiente**; um diagrama seria **redundante**. Quando o código basta, o diagrama é desperdício. E **quanto mais o código descrever a si mesmo, menos diagramas serão necessários**.

2. **Dá para dividir** em pequenos cenários? Um diagrama enorme vira vários simples.
3. **Que nível retratar?** Detalhes de baixo nível (como se calcula o pagamento por hora) ou uma **visão de alto nível** do fluxo? **Em geral, diagramas de alto nível são mais úteis**: ajudam a compor o sistema na cabeça e mostram **o que há em comum**, mais que as diferenças.

::: regra O comum importa mais que o diferente
No mundo dos diagramas, os **pontos em comuns** são muito mais importantes que as diferenças. Mostre **temas e práticas comuns**; não documente cada detalhe.
:::

## Conceitos avançados (com cuidado)

### Laços e condições completos

É possível especificar um algoritmo completo: a mensagem `*[para cada id em ids] PagarFuncionario(id)` — o `*` indica **iteração**; uma **ativação deslocada** mostra duas execuções no mesmo objeto; uma **guarda** `[ehDiaDePagamento]` indica um `if`. Mas "poder" não é "dever": a UML é **desajeitada** para algoritmos; **o código é melhor**.

### Mensagens que demoram e condições de corrida

Normalmente a transmissão de uma mensagem leva tempo ≈ 0, por isso as setas são **horizontais**. Quando levam tempo (rede, threads), desenhe-as **inclinadas** — o que revela algo valioso: as **condições de corrida**.

::: figura Uma condição de corrida numa ligação telefônica
<svg viewBox="0 0 760 330" role="img" aria-label="Duas setas cruzadas: ring e off hook ao mesmo tempo">
  <g text-anchor="middle" font-size="13"><text x="110" y="22" font-weight="800">Quem liga</text><text x="380" y="22" font-weight="800">Central</text><text x="650" y="22" font-weight="800">Quem recebe</text></g>
  <path d="M110 34V320M380 34V320M650 34V320" stroke="var(--ink-3)" stroke-dasharray="5 4"/>
  <path class="arrow" d="M112 60L376 82"/><text x="230" y="62" font-size="11" class="t-mono">tira do gancho</text>
  <path class="arrow" d="M376 100L114 124"/><text x="220" y="118" font-size="11" class="t-mono">tom de discagem</text>
  <path class="arrow" d="M112 140L376 162"/><text x="230" y="146" font-size="11" class="t-mono">disca o número</text>
  <path class="arrow" d="M384 180L646 232" style="stroke:var(--bad);marker-end:url(#arr3)"/><text x="530" y="188" font-size="11" class="t-mono t-bad">toca (ring)</text>
  <path class="arrow" d="M646 192L384 238" style="stroke:var(--bad);marker-end:url(#arr3)"/><text x="470" y="262" font-size="11" class="t-mono t-bad">tira do gancho (para ligar)</text>
  <text x="520" y="215" font-size="22" class="t-bad">✖</text>
  <text x="380" y="300" text-anchor="middle" font-size="12" class="t-dim">As setas se cruzam: cada participante tem uma ideia diferente do estado.</text>
</svg>
:::

No exemplo, **no mesmo instante** a central manda tocar e o receptor tira o fone do gancho para **ligar**. Quem liga espera um "alô"; o receptor espera um tom de discagem; a central acha que terminou seu trabalho. É uma **condição de corrida**: duas entidades assíncronas executam **operações incompatíveis** simultaneamente. Em software são **muito difíceis** de descobrir e depurar; diagramas assim ajudam a **diagnosticá-las** e, principalmente, a **explicá-las**.

### Mensagens assíncronas, threads e objetos ativos

Numa **mensagem síncrona** o chamador espera o objeto terminar. Numa **assíncrona** ele **recupera o controle imediatamente** e o receptor executa em **outra thread**. Notação: ponta de seta **aberta** (a versão antiga usava meia-ponta; ainda é mais visível).

```cs Um logger assíncrono em C# moderno
public class LoggerAssincrono : ILogger, IDisposable
{
    private readonly BlockingCollection<string> fila = new BlockingCollection<string>();
    private readonly Task consumidor;
    private readonly TextWriter saida;

    public LoggerAssincrono(TextWriter saida)
    {
        this.saida = saida;
        consumidor = Task.Run(() =>                      // "thread T2"
        {
            foreach (string msg in fila.GetConsumingEnumerable())
                saida.WriteLine(msg);
        });
    }

    public void LogMessage(string msg) { fila.Add(msg); }     // retorna JÁ — antes de gravar

    public void Dispose() { fila.CompleteAdding(); consumidor.Wait(); }
}
```

`LogMessage` apenas **enfileira** e retorna; a gravação ocorre em outro lugar, depois. Isso é uma mensagem assíncrona: **o chamador pode esperar que ela retorne antes de a operação desejada ser executada**.

- Para mostrar **várias threads**, prefixe a mensagem com um identificador: `T1:LogMessage(msg)` e `T2:WriteLine(...)`. Os identificadores são do **diagrama**, não do código.
- **Objetos ativos** (com thread interna própria) aparecem com **contorno em negrito**.

### Mensagens para interfaces

E se o aplicativo usa só a interface `ILogger`? Nomeie o objeto pela interface: `logger : ILogger`. Parece violar a regra "não se instancia interface" — mas você só afirma que o **tipo** do objeto é `ILogger`. Se quiser mostrar **o tipo concreto e a interface usada**, use o **pirulito** de interface na linha de vida.

## Diagramas de classes: dependências de código

Um diagrama de classes mostra o conteúdo **estático** e os **relacionamentos**: variáveis, métodos, herança, referências — em resumo, as **dependências do código-fonte** entre as classes. Isso é valioso: é mais fácil avaliar a estrutura de dependências num desenho. Vemos **ciclos de dependência** (e como rompê-los) e **abstrações dependendo de concretos** (e como redirecionar).

### Classes e compartimentos

::: figura Uma classe em três compartimentos
<svg viewBox="0 0 740 230" role="img" aria-label="Símbolo de classe e o código correspondente">
  <rect class="f-plain" x="20" y="20" width="260" height="190" rx="4"/><rect class="f-prim" x="20" y="20" width="260" height="36" rx="4"/><text x="150" y="44" text-anchor="middle" font-weight="800">Discador</text>
  <path class="line" d="M20 56H280M20 106H280"/>
  <g class="t-mono" font-size="12"><text x="30" y="76">- digitos : List&lt;int&gt;</text><text x="30" y="94">- total : int</text><text x="30" y="128">+ Digitar(n : int)</text><text x="30" y="148">+ Ligar() : bool</text><text x="30" y="168"># Registrar(n : int) : bool</text></g>
  <text x="150" y="200" text-anchor="middle" font-size="11" class="t-dim">nome / campos / métodos</text>
  <rect class="f-plain" x="320" y="20" width="400" height="190" rx="8"/>
  <g class="t-mono" font-size="13"><text x="336" y="48">public class Discador</text><text x="336" y="68">{</text><text x="350" y="90">private List&lt;int&gt; digitos;</text><text x="350" y="110">private int total;</text><text x="350" y="132">public void Digitar(int n) {…}</text><text x="350" y="152">public bool Ligar() {…}</text><text x="350" y="172">protected bool Registrar(int n) {…}</text><text x="336" y="196">}</text></g>
</svg>
:::

- Compartimento superior: **nome**; depois **variáveis**; depois **métodos**.
- Visibilidade: **`-` private**, **`#` protected**, **`+` public**.
- Tipos após dois-pontos: `n : int`, retorno após `)`.

::: dica Na maioria das vezes, só o nome
Na maioria dos diagramas a classe aparece **apenas com o nome**. UML **não é lugar** de declarar variáveis e métodos: isso é no código. Use os adornos só quando forem **essenciais** ao propósito do diagrama.
:::

### Associação

Representa, na maioria das vezes, uma **variável de instância** que contém **referência** a outro objeto. A **seta** indica a direção ("`Telefone` conhece `Botao`"); o **nome** junto à ponta é o **nome da variável**; o **número** é quantas referências há (`*` = muitas → coleção).

```cs
public class Telefone { private readonly Botao[] botoes = new Botao[15]; }        // Telefone → Botao [15]
public class Agenda   { private readonly List<Numero> numeros = new List<Numero>(); }   // Agenda → Numero [*]
```

::: pense Evite "tem-um"
O autor evita dizer "`Agenda` **tem** muitos `Numero`s". Os rótulos **"tem-um"** e **"é-um"** causam mal-entendidos. Prefira descrever o que acontece no **software**: "`Agenda` está **conectada a** muitos `Numero`s".
:::

### Herança e realização de interface

Cuidado com as setas: uma **seta de herança** (ponta triangular) aponta para a **classe base**. Regra geral: **todas as setas da UML apontam na direção da dependência do código-fonte** — `Assalariado` menciona o nome de `Funcionario` (`: Funcionario`), então a seta vai para `Funcionario`.

Entre classe e **interface** da linguagem, a UML usa **herança tracejada** (*realização*) ou o **pirulito**. Na prática, **esqueça de tracejar**: "a vida é curta demais para tracejar setas no quadro".

::: dica Convenção para ler rápido
Desenhe **herança na vertical** e **associações na horizontal**: o olho distingue relacionamentos completamente diferentes sem esforço. E **agrupe** as classes em zonas coerentes (por exemplo: transações à esquerda, interfaces de UI à direita, implementação embaixo), com **poucas conexões, regulares** entre os grupos.
:::

::: figura Um diagrama de classes legível (caixa eletrônico, versão nossa)
<svg viewBox="0 0 780 330" role="img" aria-label="Transações à esquerda, interfaces de UI à direita, implementação embaixo">
  <rect x="5" y="5" width="300" height="320" rx="12" fill="none" stroke="var(--line)" stroke-dasharray="5 4"/><text x="155" y="24" text-anchor="middle" font-size="11" class="t-dim">transações</text>
  <rect class="f-info" x="75" y="35" width="160" height="42" rx="4"/><text x="155" y="52" text-anchor="middle" font-size="11" class="t-dim">«interface»</text><text x="155" y="68" text-anchor="middle" font-weight="700">Transacao</text>
  <path class="line" d="M155 80V105M50 105H260M50 105V125M155 105V125M260 105V125"/>
  <rect class="f-plain" x="12" y="125" width="76" height="40" rx="4"/><text x="50" y="150" text-anchor="middle" font-size="11">Deposito</text>
  <rect class="f-plain" x="117" y="125" width="76" height="40" rx="4"/><text x="155" y="150" text-anchor="middle" font-size="11">Saque</text>
  <rect class="f-plain" x="222" y="125" width="76" height="40" rx="4"/><text x="260" y="150" text-anchor="middle" font-size="11">Transferência</text>
  <rect x="460" y="5" width="315" height="190" rx="12" fill="none" stroke="var(--line)" stroke-dasharray="5 4"/><text x="617" y="24" text-anchor="middle" font-size="11" class="t-dim">interfaces de UI (uma por grupo de clientes)</text>
  <rect class="f-good" x="470" y="40" width="95" height="52" rx="4"/><text x="517" y="58" text-anchor="middle" font-size="10" class="t-dim">«interface»</text><text x="517" y="74" text-anchor="middle" font-size="12" font-weight="700">UiDeposito</text>
  <rect class="f-good" x="575" y="40" width="95" height="52" rx="4"/><text x="622" y="58" text-anchor="middle" font-size="10" class="t-dim">«interface»</text><text x="622" y="74" text-anchor="middle" font-size="12" font-weight="700">UiSaque</text>
  <rect class="f-good" x="680" y="40" width="90" height="52" rx="4"/><text x="725" y="58" text-anchor="middle" font-size="10" class="t-dim">«interface»</text><text x="725" y="74" text-anchor="middle" font-size="12" font-weight="700">UiTransf.</text>
  <path class="arrow" d="M90 145H468" style="stroke:var(--ink-3);marker-end:url(#arr2)"/>
  <path class="line" d="M517 94V135M622 94V135M725 94V135M517 135H725M622 135V160"/>
  <rect class="f-prim" x="570" y="160" width="105" height="30" rx="4"/><text x="622" y="180" text-anchor="middle" font-size="12" font-weight="700">UI (agrega)</text>
  <path class="line" d="M622 192V225M540 225H710M540 225V245M710 225V245"/>
  <rect class="f-plain" x="480" y="245" width="120" height="40" rx="4"/><text x="540" y="270" text-anchor="middle" font-size="12">UiPortugues</text>
  <rect class="f-plain" x="650" y="245" width="120" height="40" rx="4"/><text x="710" y="270" text-anchor="middle" font-size="12">UiIngles</text>
</svg>
:::

## Os detalhes (use pouco)

### Estereótipos de classe

Aparecem entre `« »`, acima do nome. Os dois úteis para C#:

- **«interface»**: todos os métodos abstratos e nenhuma variável de instância — exatamente uma `interface` C#. No quadro branco, muita gente abrevia com `«I»` ou com o prefixo `I`.
- **«utility»**: todos os membros **estáticos** (como `Math`).

Você pode **criar os seus** (`«persistent»`, `«struct»`…) desde que quem lê saiba o que significam.

### Classes abstratas

Duas formas padrão: nome em *itálico* ou a propriedade `{abstract}`. No quadro, itálico é difícil; use `{A}` e combine com a equipe.

### Propriedades

Pares `{autor=Ana, arquivo=forma.cs}` podem decorar classes. A única útil em prática é `{abstract}`.

### Agregação e composição: prefira evitar

- **Agregação** ("todo/parte") não tem definição segura na UML: cada programador adota a sua. Sua implementação em código é **indistinguível de uma associação**. A única regra concreta é que um todo **não pode ser parte de si mesmo** (sem ciclos de instâncias) — o que quase nunca ajuda. **Evite.**
- **Composição** é uma agregação forte: o parte **não pode pertencer a dois donos** ao mesmo tempo e o **dono controla seu ciclo de vida** (destruir/copiar junto). Em C# o GC cuida da destruição, e raramente se precisa mostrar **cópia profunda** em diagrama — mas quando precisa (por exemplo, `Endereco` com uma lista de linhas), a composição indica que **a cópia deve ser profunda**:

```cs
public class Endereco : ICloneable
{
    private List<string> linhas = new List<string>();
    public void DefinirLinha(int n, string texto) { linhas[n] = texto; }

    public object Clone()
    {
        var copia = (Endereco)MemberwiseClone();
        copia.linhas = new List<string>(linhas);   // cópia profunda da lista
        return copia;                              // (strings são imutáveis: não precisam ser clonadas)
    }
}
```

### Multiplicidade

| Notação | Significa |
|---|---|
| `2` | exatamente 2 |
| `*` ou `0..*` | zero ou muitos |
| `0..1` | zero ou um (referência que pode ser `null`) |
| `1..*` | um ou muitos |
| `3..5` | de três a cinco |

### Estereótipos de associação

Alteram o significado da seta:

| Estereótipo | Significado | Código típico |
|---|---|---|
| **«create»** | A origem **cria** o destino e o entrega ao resto do sistema (fábrica) | `public B CriarB() { return new B(); }` |
| **«local»** | A origem cria e mantém numa **variável local**; não sobrevive ao método | `B b = new B(); b.Usar();` |
| **«parameter»** | A origem recebe o destino por **parâmetro** e o esquece ao retornar | `void F(B b) { b.Usar(); }` (muitos desenham uma **seta tracejada**) |
| **«delegate»** | A origem **repassa** chamadas ao destino | Proxy, Decorator, Composite |

### Outros: classes aninhadas, associativas, qualificadores

- **Classe aninhada:** associação com um círculo cruzado.
- **Classe associativa:** mostra **qual contêiner** implementa uma associação com multiplicidade (`Lista` entre `Endereco` e `string`).
- **Qualificador:** a associação é feita por **chave** (por exemplo, `idFuncionario` que busca o objeto num banco); útil em pouquíssimas situações.

::: cuidado Menos é mais
A UML tem tantos elementos que você poderia virar um "advogado de UML", "fazendo o que advogados fazem de melhor: escrever documentos que ninguém consegue entender". Aqui mostramos as partes que **realmente se usam**. Aprenda o valor do **minimalismo**: em UML, quase sempre **menos é mais**.
:::

::: exercicio Do diagrama ao código
Leia este diagrama (descrito em texto) e escreva o código C# correspondente:

- `«interface» IPagamento` com `+ Pagar(valor : decimal)`
- `PagamentoPix` e `PagamentoCartao` **realizam** `IPagamento`
- `Checkout` está **associada** a `IPagamento` (variável `pagamento`, multiplicidade 1) e a `ItemDeCompra` (variável `itens`, multiplicidade `*`)
- `Checkout` **cria** `Recibo` (estereótipo «create»)

--- resposta
```cs
public interface IPagamento { void Pagar(decimal valor); }
public class PagamentoPix : IPagamento { public void Pagar(decimal valor) { /*...*/ } }
public class PagamentoCartao : IPagamento { public void Pagar(decimal valor) { /*...*/ } }

public class Checkout
{
    private readonly IPagamento pagamento;                          // associação 1
    private readonly List<ItemDeCompra> itens = new List<ItemDeCompra>();   // associação *

    public Checkout(IPagamento pagamento) { this.pagamento = pagamento; }

    public Recibo Finalizar()                                       // «create»
    {
        pagamento.Pagar(itens.Sum(i => i.Preco));
        return new Recibo(itens);
    }
}
```
Você sabe se entendeu o diagrama quando **consegue visualizar o código**.
:::

::: resumo
- **Sequência:** objetos (nome sublinhado), linhas de vida, setas, retorno, ativação; criação (seta na caixa); laços e guardas. **Poucos, pequenos, de uma página**; prefira alto nível; o código costuma bastar.
- Setas inclinadas revelam **condições de corrida**; **assíncrona** = ponta aberta; **threads** com prefixo `T1:`; **objetos ativos** = contorno grosso; use o **tipo da interface** para mostrar o contrato.
- **Classes:** nome / campos / métodos; `-`, `#`, `+`; associação (variável + multiplicidade), herança para a base, **«interface»**, abstratas.
- Convenção: **herança na vertical, associações na horizontal**; agrupe por zonas; poucas ligações regulares.
- **Evite** agregação (e use composição só para cópia profunda); estereótipos de associação (`«create»`, `«local»`, `«parameter»`, `«delegate»`) são úteis.
- **Minimalismo:** em UML, menos é mais.
:::

::: quiz
? Em um diagrama de sequência, o que indica uma mensagem enviada para uma classe (nome não sublinhado)?
+ Chamada de um método estático
- Chamada de um construtor
- Mensagem assíncrona
- Liberação do objeto
> Objetos têm nome sublinhado; classes, não.

? Como se representa a criação de um objeto num diagrama de sequência?
+ Uma seta sem rótulo que termina na caixa do objeto
- Um grande X na linha de vida
- Uma seta tracejada
- Uma ativação dupla
> O X representa liberar para o GC.

? Qual a recomendação sobre quantos diagramas de sequência desenhar?
+ Poucos, pequenos (cabendo numa página) e de alto nível
- Um para cada método
- Centenas, para documentar tudo
- Nenhum, nunca
> Diagramas demais não são lidos.

? O que significa uma seta de mensagem inclinada?
+ A mensagem leva tempo para chegar (útil para mostrar condições de corrida)
- Mensagem inválida
- Retorno de método
- Mensagem para uma interface
> Normalmente as setas são horizontais (tempo ≈ 0).

? Como a seta de herança aponta num diagrama de classes?
+ Para a classe base (direção da dependência do código-fonte)
- Para a classe derivada
- Para a interface
- Não há regra
> Todas as setas apontam na direção da dependência.

? O que significa `-`, `#` e `+` antes de um membro?
+ private, protected e public
- Obrigatório, opcional e extra
- Estático, abstrato e virtual
- Leitura, escrita e execução
> É a notação de visibilidade.

? Qual convenção ajuda a distinguir herança de associação?
+ Desenhar herança na vertical e associações na horizontal
- Usar sempre cores diferentes
- Desenhar tudo na diagonal
- Não desenhar associações
> Torna a leitura rápida.

? O que faz o estereótipo «create» numa associação?
+ Indica que a origem cria o destino e o repassa para o sistema (como uma fábrica)
- Indica que o destino é uma variável local
- Indica herança
- Indica um método estático
> «local» é para criação e uso dentro do método.
:::

::: cartas
Linha de vida :: Linha tracejada vertical de um objeto num diagrama de sequência.
Ativação :: Retângulo fino que mostra o tempo em que um método executa.
Mensagem síncrona :: O chamador espera o receptor terminar.
Mensagem assíncrona :: O chamador recupera o controle imediatamente; o receptor roda em outra thread.
Condição de corrida :: Duas entidades assíncronas executam operações incompatíveis ao mesmo tempo.
Associação :: Relação em que um objeto mantém referência a outro (variável de instância).
Multiplicidade :: Quantidade de instâncias numa associação (2, *, 0..1, 1..*).
«interface» :: Estereótipo: todos os métodos abstratos, sem variáveis de instância.
«create» / «local» / «parameter» / «delegate» :: Estereótipos de associação: criação, variável local, parâmetro, repasse.
Composição :: Agregação forte: dono controla o ciclo de vida da parte (cópia profunda).
Realização :: Relação de uma classe que implementa uma interface.
:::
