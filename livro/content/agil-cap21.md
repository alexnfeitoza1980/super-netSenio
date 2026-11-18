---
id: agil-cap21
order: 37
kind: capitulo
num: 21
group: C# · Parte 8 · Projeto ágil e princípios SOLID
title: Princípio do Aberto/Fechado (OCP)
subtitle: "Abertas para extensão, fechadas para modificação" — abstração, Strategy, Template Method, fechamento estratégico e a regra "engane-me uma vez".
minutes: 50
hue: 15
objectives:
- Enunciar o **OCP** e explicar como **abstração** permite mudar o comportamento sem mudar o código
- Reconhecer o código **que viola o OCP** (cadeias de `switch`/`if` por tipo)
- Aplicar OCP com **Strategy**, **Template Method** e **tabelas de dados**
- Entender por que o fechamento é sempre **estratégico** e quando aplicá-lo ("engane-me uma vez")
origem: livros/02-projeto-agil-csharp/content/36-cap21.md
---

::: ilustracao img/agil/cap21.svg | OCP: aberto para extensão, fechado para modificação: encaixe um módulo novo em vez de reabrir o motor.
:::

## O princípio

Todo sistema muda durante seu ciclo de vida. Como criar projetos que **sobrevivam à primeira versão**? Bertrand Meyer deu a resposta em 1988:

::: regra OCP — Open/Closed Principle
Entidades de software (classes, módulos, funções…) devem ser **abertas para extensão**, mas **fechadas para modificação**.
:::

Quando uma única mudança provoca uma cascata de alterações, o projeto tem o cheiro da **rigidez**. O OCP aconselha refatorar para que mudanças desse tipo sejam feitas **adicionando código novo** — e não alterando código antigo que já funciona.

### Aberto E fechado? Parece contraditório

- **Aberto para extensão:** o *comportamento* do módulo pode ser ampliado conforme os requisitos mudam.
- **Fechado para modificação:** ampliar esse comportamento **não exige alterar** o código-fonte (nem o binário) do módulo.

Como mudar o que o módulo faz **sem alterá-lo**? **Com abstração.** Em C# criamos abstrações fixas (interfaces, classes abstratas) que representam um conjunto **ilimitado** de comportamentos possíveis: as classes derivadas. O módulo manipula **a abstração** — fixa, portanto "fechada" — mas o comportamento muda criando **novas derivadas**.

::: figura Violando e obedecendo ao OCP
<svg viewBox="0 0 740 230" role="img" aria-label="Cliente depende de classe concreta (viola) e de abstração (obedece)">
  <text x="165" y="20" text-anchor="middle" font-weight="800" class="t-bad">Viola o OCP</text>
  <rect class="f-prim" x="20" y="50" width="130" height="50" rx="10"/><text x="85" y="80" text-anchor="middle" font-weight="700">Cliente</text>
  <path class="arrow" d="M152 75H208" style="stroke:var(--bad);marker-end:url(#arr3)"/>
  <rect class="f-bad" x="210" y="50" width="130" height="50" rx="10"/><text x="275" y="80" text-anchor="middle" font-weight="700">Servidor</text>
  <text x="165" y="135" text-anchor="middle" font-size="12" class="t-dim">trocar de servidor = alterar Cliente</text>
  <text x="560" y="20" text-anchor="middle" font-weight="800" class="t-good">Obedece ao OCP (Strategy)</text>
  <rect class="f-prim" x="400" y="50" width="110" height="50" rx="10"/><text x="455" y="80" text-anchor="middle" font-weight="700">Cliente</text>
  <path class="arrow" d="M512 75H548"/>
  <rect class="f-info" x="550" y="50" width="170" height="50" rx="10"/><text x="635" y="80" text-anchor="middle" font-size="13" font-weight="700">«interface» IServico</text>
  <path class="line" d="M600 160V102M670 160V102" style="marker-end:none"/>
  <rect class="f-good" x="545" y="162" width="100" height="42" rx="8"/><text x="595" y="188" text-anchor="middle" font-size="13">ServidorA</text>
  <rect class="f-good" x="660" y="162" width="75" height="42" rx="8"/><text x="697" y="188" text-anchor="middle" font-size="13">ServidorB</text>
  <text x="560" y="222" text-anchor="middle" font-size="12" class="t-dim">novo servidor = nova classe; Cliente intacto</text>
</svg>
:::

Duas estruturas clássicas satisfazem o OCP, e ambas separam **política genérica** de **detalhe de implementação**:

1. **Strategy:** `Cliente` usa uma **interface**; trocar de comportamento = outra implementação.
2. **Template Method:** a classe base define o **esqueleto** do algoritmo em termos de métodos abstratos; as subclasses preenchem os detalhes. (Os dois padrões aparecem em detalhe no capítulo 30.)

## O exemplo das formas

Um aplicativo deve desenhar uma lista de **círculos e quadrados** numa interface gráfica, **na ordem da lista**.

### Violando o OCP

```cs Versão procedural
public enum TipoForma { Circulo, Quadrado }

public class Forma { public TipoForma Tipo; public double Tamanho; }

public static void DesenharTodas(List<Forma> formas)
{
    foreach (var f in formas)
    {
        switch (f.Tipo)
        {
            case TipoForma.Circulo:  DesenharCirculo(f);  break;
            case TipoForma.Quadrado: DesenharQuadrado(f); break;
        }
    }
}
```

`DesenharTodas` **não pode ser fechada** para novos tipos: para aceitar triângulos é preciso **modificá-la** (e o `enum`). Na vida real o `switch` se repete em dezenas de funções (arrastar, esticar, mover, excluir…); adicionar uma forma significa **achar todos** os `switch`/`if-else` — e alguns estarão disfarçados (cases combinados, predicados compostos). O resultado:

- **Rígido:** mudar o `enum` recompila tudo que depende dele.
- **Frágil:** é fácil esquecer um `switch`.
- **Imóvel:** quem quiser reutilizar `DesenharTodas` leva junto círculos e quadrados.

### Obedecendo ao OCP

```cs Versão orientada a objetos
public interface IForma { void Desenhar(); }

public class Quadrado : IForma { public void Desenhar() { /* desenha um quadrado */ } }
public class Circulo  : IForma { public void Desenhar() { /* desenha um círculo  */ } }

public static void DesenharTodas(IEnumerable<IForma> formas)
{
    foreach (IForma forma in formas)
        forma.Desenhar();
}
```

Agora basta criar `Triangulo : IForma`; **nenhum** código existente muda. Só o ponto que **instancia** o triângulo (em geral, `Main` ou uma **fábrica**, capítulo 34) precisa conhecer a nova classe.

::: passos Adicionando um triângulo
**Situação inicial:** `DesenharTodas` conhece `IForma`; existem `Circulo` e `Quadrado`.
===
**Passo 1 — nova classe:** escreva `Triangulo : IForma` com seu `Desenhar()`.
===
**Passo 2 — registrar:** onde as formas são criadas, adicione o triângulo à lista.
===
**Resultado:** `DesenharTodas`, `Circulo`, `Quadrado` e todos os demais módulos permanecem **intactos** — nem recompilados nem retestados. O sistema foi estendido **somando código**, não editando código.
:::

## Mas o fechamento nunca é completo

E se o cliente pedir: **"desenhe todos os círculos antes de qualquer quadrado"**? `DesenharTodas` **não está fechada** para essa mudança: seria preciso alterá-la para percorrer a lista duas vezes.

A abstração que escolhemos (`IForma`, "algo que se desenha") ajuda com *novos tipos*, mas **atrapalha** esta mudança, porque a ordem está acoplada ao **tipo**. Isso leva a uma conclusão importante:

::: regra O fechamento é estratégico
Não importa quão "fechado" um módulo seja: **sempre haverá algum tipo de mudança** contra a qual ele não está fechado. **Não existe modelo "natural" para todos os contextos.** Por isso o projetista escolhe **contra quais mudanças fechar** — as mais prováveis — e constrói abstrações para elas.
:::

Isso exige conhecimento do negócio e experiência, e envolve **apostas**: acertando, você ganha; errando, perde — e vai errar às vezes. Além disso, **obedecer ao OCP custa**: tempo para criar as abstrações e **complexidade** que elas acrescentam. Há um limite de abstração que se pode pagar.

### Os "ganchos" do século passado

Antigamente diziam-se "inserir ganchos" para mudanças que talvez ocorressem. Os ganchos frequentemente estavam **errados** — e pior, eram complexidade **desnecessária**, que precisava ser mantida mesmo sem uso. Hoje: **insira a abstração só quando precisar dela.**

## "Engane-me uma vez…"

> *"Engane-me uma vez e a vergonha é de quem enganou; engane-me duas, e a vergonha é minha."*

Aplicada ao software:

1. **Escreva o código esperando que ele não mude** (mais simples possível).
2. Quando a primeira mudança **realmente acontece**, você "leva o primeiro tiro".
3. **Agora** crie a abstração que o protege contra mudanças **desse tipo**.

Assim você evita o excesso de abstração e protege os pontos que **de fato** mudam.

### Provocando a mudança cedo

Se vamos "levar o primeiro tiro", queremos recebê-lo **cedo e com frequência**, antes de irmos longe demais no caminho errado. Como estimular mudanças?

- **Escreva os testes primeiro** (o teste já é um segundo "uso" do sistema e força abstrações).
- Use **ciclos muito curtos** (dias, não semanas).
- Desenvolva **funcionalidades antes da infraestrutura** e mostre-as aos interessados.
- Faça **primeiro as mais importantes**.
- **Entregue cedo e com frequência**, diante de clientes e usuários.

## Fechando contra a ordem: duas saídas

Voltando ao "círculos antes de quadrados": precisamos de uma **abstração de ordem**.

**Saída 1 — cada forma sabe se comparar (`IComparable<T>`):**

```cs
public interface IForma : IComparable<IForma> { void Desenhar(); }

public class Circulo : IForma
{
    public int CompareTo(IForma outra) { return outra is Quadrado ? -1 : 0; }
    public void Desenhar() { /* ... */ }
}
```

Funciona, mas cada classe passa a **conhecer as outras**: ao criar um novo tipo, todos os `CompareTo` precisam mudar. Não é OCP — apenas moveu o problema.

**Saída 2 — estratégia orientada a dados (tabela):**

```cs
public class ComparadorDeFormas : IComparer<IForma>
{
    private static readonly Dictionary<Type, int> prioridades = new Dictionary<Type, int>
    {
        { typeof(Circulo), 1 },
        { typeof(Quadrado), 2 },
    };

    private static int PrioridadeDe(Type tipo)
    {
        return prioridades.TryGetValue(tipo, out int p) ? p : 0;   // desconhecidos vêm antes
    }

    public int Compare(IForma x, IForma y)
    {
        return PrioridadeDe(x.GetType()).CompareTo(PrioridadeDe(y.GetType()));
    }
}

public static void DesenharTodas(List<IForma> formas)
{
    formas.Sort(new ComparadorDeFormas());     // a ordem é um detalhe plugável
    foreach (IForma forma in formas) forma.Desenhar();
}
```

Agora `DesenharTodas` está fechada para **qualquer política de ordem** (passe outro `IComparer`); cada forma está fechada para novas formas; e **a única coisa** que muda quando a ordem muda é a **tabela** — que vive no seu próprio módulo, isolada.

::: dica Dados em vez de código
Quando a variação é "qual valor corresponde a qual tipo", uma **tabela** (dicionário, arquivo de configuração, banco) costuma ser mais "fechável" do que um `switch`: alterar a regra é alterar **dado**, não **lógica compilada**.
:::

## Outro exemplo: formas de pagamento

```cs Antes — viola o OCP
public decimal CalcularDesconto(string formaDePagamento, decimal valor)
{
    if (formaDePagamento == "pix")       return valor * 0.05m;
    if (formaDePagamento == "boleto")    return valor * 0.02m;
    if (formaDePagamento == "cartao")    return 0m;
    return 0m;
}
```

```cs Depois — aberto para extensão
public interface IRegraDeDesconto { decimal Calcular(decimal valor); }

public class DescontoPix    : IRegraDeDesconto { public decimal Calcular(decimal v) { return v * 0.05m; } }
public class DescontoBoleto : IRegraDeDesconto { public decimal Calcular(decimal v) { return v * 0.02m; } }
public class SemDesconto    : IRegraDeDesconto { public decimal Calcular(decimal v) { return 0m; } }

public class Checkout
{
    private readonly IRegraDeDesconto regra;
    public Checkout(IRegraDeDesconto regra) { this.regra = regra; }
    public decimal TotalAPagar(decimal valor) { return valor - regra.Calcular(valor); }
}
```

Um novo meio de pagamento? **Nova classe.** `Checkout` não muda.

::: cuidado Cuidado com a abstração precipitada
Se você nunca precisou de um segundo desconto, o `if` simples é **melhor**. Aplicar o OCP em tudo, "por precaução", cria uma floresta de interfaces com uma única implementação. **Resistir à abstração precipitada é tão importante quanto a abstração em si.**
:::

::: exercicio Aplique o "engane-me uma vez"
Seu `Notificador` envia só e-mail. Na semana 1 ninguém pede nada. Na semana 4 pedem SMS. Na semana 9, push.

1. O que você faz na semana 1?
2. E na semana 4?
3. Na semana 9?

--- resposta
1. **Nada de abstração:** uma classe `Notificador` que envia e-mail — a coisa mais simples que funciona.
2. É o **primeiro tiro**: extraia `INotificador` com `Enviar(mensagem)`, mova o e-mail para `NotificadorEmail`, crie `NotificadorSms`. O código que notifica passa a depender da interface.
3. Já está protegido: crie `NotificadorPush : INotificador` e registre-o. **Nenhum código existente** é alterado — o investimento em abstração se paga.
:::

::: resumo
- **OCP:** aberto para extensão, fechado para modificação — estender somando código, não editando.
- O segredo é a **abstração** fixa (interface/classe abstrata) com infinitas derivadas.
- `switch`/`if` por **tipo** repetidos pelo sistema são o sintoma típico de violação.
- Soluções: **Strategy**, **Template Method**, **tabelas de dados**.
- O fechamento é **estratégico**: escolha as mudanças **prováveis**; abstrações custam complexidade.
- **Engane-me uma vez:** comece simples; ao primeiro tiro, abstraia contra aquele tipo de mudança. Estimule mudanças cedo (testes, ciclos curtos, entregas frequentes).
:::

::: quiz
? Qual é o enunciado do OCP?
+ Entidades devem ser abertas para extensão e fechadas para modificação
- Entidades devem ser abertas para modificação e fechadas para extensão
- Classes devem ter apenas um motivo para mudar
- Interfaces devem ser pequenas
> Estende-se o comportamento adicionando código novo.

? Qual ferramenta da OO torna possível estender sem modificar?
+ Abstração (interfaces/classes abstratas e suas derivadas)
- Variáveis globais
- Constantes
- Classes seladas
> O módulo depende da abstração fixa; o comportamento vem das derivadas.

? O que há de errado com um `switch` por tipo repetido em muitas funções?
+ Adicionar um tipo exige achar e alterar todos os `switch` (rigidez e fragilidade)
- Nada, é a melhor prática
- Ele deixa o compilador lento
- Ele impede herança
> Cada novo tipo toca em vários lugares.

? O que significa "fechamento estratégico"?
+ Não há módulo fechado contra todas as mudanças; escolhem-se as mais prováveis
- Fechar todos os módulos contra tudo
- Fechar o código-fonte aos clientes
- Fechar o projeto após a entrega
> Abstrair tudo custa caro e é impossível.

? O que faz a regra "engane-me uma vez"?
+ Aceita-se o primeiro tiro (a primeira mudança) e só então se abstrai contra mudanças desse tipo
- Obriga a prever toda mudança
- Proíbe a mudança de requisitos
- Pede para duplicar o código
> Evita a complexidade desnecessária.

? Qual das opções ajuda a descobrir cedo quais mudanças são prováveis?
+ Testes primeiro, ciclos curtos e entregas frequentes
- Documentos longos de requisitos
- Planejar o ano inteiro
- Adiar a entrega ao máximo
> Quanto antes você recebe feedback, melhor a abstração.

? Em C#, um comparador (`IComparer<T>`) orientado a tabela ilustra qual ideia?
+ Tornar a ordem um detalhe plugável (dado), mantendo as classes de formas fechadas
- Que a herança é obrigatória
- Que não se deve usar dicionários
- Que `Sort` não funciona
> A política de ordem fica numa tabela isolada.
:::

::: cartas
OCP :: Aberto para extensão, fechado para modificação.
Extensão :: Ampliar o comportamento sem alterar o código existente.
Abstração fixa :: Interface/classe abstrata estável com infinitas implementações possíveis.
Strategy :: Padrão em que o cliente depende de uma interface e a implementação é intercambiável.
Template Method :: Padrão em que a classe base define o esqueleto e as subclasses os detalhes.
Fechamento estratégico :: Escolher contra quais mudanças prováveis o módulo será fechado.
"Engane-me uma vez" :: Abstraia apenas após a primeira mudança real.
Abstração precipitada :: Criar abstrações antes de haver necessidade; gera complexidade.
:::
