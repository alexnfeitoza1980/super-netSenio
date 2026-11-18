---
id: agil-cap17
order: 32
kind: capitulo
num: 17
group: C# · Parte 7 · Desenvolvimento ágil
title: Planejamento e testes
subtitle: Histórias, pontos, velocidade e burndown; o ciclo do TDD, objetos simulados (mocks) e testes de aceitação.
minutes: 60
hue: 95
objectives:
- Planejar com **histórias**, **pontos**, **velocidade** e **gráficos de burndown**
- Seguir as **3 regras do TDD** e escrever **testes de unidade** em C#
- Usar **mocks/stubs** para isolar o código e **desacoplar** o projeto
- Distinguir **testes de unidade** (caixa branca) de **testes de aceitação** (caixa preta)
origem: livros/02-projeto-agil-csharp/content/31-cap17.md
---

::: ilustracao img/agil/cap17.svg | Histórias pequenas e testes primeiro: o quadro mostra o que falta e o teste diz quando está pronto.
:::

## Planejar com números honestos

Lord Kelvin dizia que só conhecemos de verdade o que conseguimos medir. Em projetos de software a ideia vale em dobro: sem **medidas**, o plano é só esperança. O jogo do planejamento da XP (e de vários métodos ágeis) transforma o planejamento em **medição contínua**.

### 1. Exploração inicial: histórias em fichas

No início, desenvolvedores e cliente conversam e listam as funcionalidades que conseguirem — **sem esgotar o assunto**; novas aparecerão até o fim. Cada funcionalidade vira uma ou mais **histórias**, escritas numa ficha com pouco mais que um **nome** (*Login*, *Adicionar usuário*, *Alterar senha*). A ficha é só um lembrete da conversa.

### 2. Estimativas **relativas**

Os desenvolvedores estimam juntos, em **pontos de história**. Não sabemos quantas horas vale um ponto, mas sabemos que uma história de **8 pontos** custa **o dobro** de uma de **4**. Comparar é mais fácil e mais preciso que prever horas.

### 3. Dividir e combinar

História grande demais é subestimada; pequena demais, superestimada. Por isso:

- **Divida** o que é grande: *"usuários transferem dinheiro com segurança"* vira *fazer login*, *fazer logout*, *depositar*, *sacar*, *transferir entre contas*.
- **Combine** o que é minúsculo.
- **Reestime** depois de dividir; não some nem subtraia: 25 pontos podem virar 30 — e 30 é a estimativa mais honesta.

### 4. Velocidade

A soma dos pontos das histórias **concluídas** numa semana (ou iteração) é a **velocidade**. Depois de 3 ou 4 iterações você tem uma boa média e consegue **prever** o futuro. No começo, um "chute informado" basta — a precisão inicial não é crítica.

::: regra "Ontem" vira o orçamento de hoje
A velocidade planejada para uma iteração é **a velocidade medida na anterior**. Se a equipe fez 31 pontos, planeja 31. A velocidade sobe com habilidade e boa arquitetura; cai quando alguém sai. Nunca se "negocia" a velocidade com pressão.
:::

### 5. Plano de entrega e plano de iteração

- **Entrega (2–4 meses):** o cliente escolhe as histórias e a ordem aproximada, **sem ultrapassar** a velocidade × número de iterações. Decidir é uma questão de **valor por custo**: algo importante porém caríssimo pode esperar por algo menos importante e baratíssimo.
- **Iteração (1–2 semanas):** o cliente escolhe histórias dentro do orçamento; a **ordem** de implementação é decisão **técnica**. Iniciada a iteração, **o cliente não troca as histórias dela** (as demais, pode reordenar à vontade). A iteração termina **na data marcada**, mesmo que falte algo; só conta o que ficou **pronto**.

::: figura Quem decide o quê
<svg viewBox="0 0 740 230" role="img" aria-label="Negócio decide valor e prioridade; desenvolvimento decide estimativa e ordem técnica">
  <rect class="f-prim" x="10" y="20" width="350" height="190" rx="14"/><text x="185" y="50" text-anchor="middle" font-weight="800" class="t-prim" font-size="16">Negócio (cliente)</text>
  <g font-size="13"><text x="30" y="85">• diz o que é importante</text><text x="30" y="110">• escolhe as histórias da entrega</text><text x="30" y="135">• escolhe as histórias da iteração</text><text x="30" y="160">• define os testes de aceitação</text><text x="30" y="185">• não troca histórias da iteração em curso</text></g>
  <rect class="f-good" x="380" y="20" width="350" height="190" rx="14"/><text x="555" y="50" text-anchor="middle" font-weight="800" class="t-good" font-size="16">Desenvolvimento</text>
  <g font-size="13"><text x="400" y="85">• estima o custo (pontos)</text><text x="400" y="110">• informa a velocidade e o orçamento</text><text x="400" y="135">• divide histórias em tarefas</text><text x="400" y="160">• decide a ordem técnica</text><text x="400" y="185">• avisa cedo quando algo não cabe</text></g>
</svg>
:::

### 6. "Pronto" significa: testes de aceitação passando

Uma história **não está pronta** até que **todos os seus testes de aceitação** — automatizados — passem. Eles são escritos pelo cliente, analistas, testadores (e até programadores) **no início da iteração**.

### 7. Planejamento de tarefas

No começo da iteração, a equipe decompõe cada história em **tarefas** (de 4 a 16 horas) e os desenvolvedores **se inscrevem** nelas, estimando em "pontos de tarefa" (muitos usam horas cheias). Cada um tem um **orçamento pessoal**: o que entregou na iteração anterior. Não existe "tarefa de banco só para o especialista de banco": quem sabe mais ensina, quem sabe menos aprende, e o conhecimento se espalha.

::: pense A reunião da metade
Na metade da iteração, **metade das histórias** (não metade das tarefas!) deve estar concluída. O pesadelo é chegar ao fim com 90% das tarefas feitas e **nenhuma história pronta**. Foco em **terminar histórias**, não em acumular tarefas pela metade.
:::

### 8. Acompanhando: velocidade e burndown

Dois gráficos no mural da equipe dizem quase tudo:

::: figura Gráfico de velocidade e burndown
<svg viewBox="0 0 740 260" role="img" aria-label="Barras de velocidade por semana e linha de burndown descendo">
  <text x="170" y="18" text-anchor="middle" font-weight="800" class="t-info">Velocidade (pontos concluídos / semana)</text>
  <path class="line" d="M20 220H340M20 220V30"/>
  <g class="f-info"><rect x="40" y="110" width="30" height="110" rx="3"/><rect x="85" y="95" width="30" height="125" rx="3"/><rect x="130" y="105" width="30" height="115" rx="3"/><rect x="175" y="88" width="30" height="132" rx="3"/><rect x="220" y="100" width="30" height="120" rx="3"/><rect x="265" y="92" width="30" height="128" rx="3"/></g>
  <path d="M20 100H340" stroke="var(--warn)" stroke-width="2" stroke-dasharray="6 4"/><text x="344" y="104" font-size="11" class="t-warn">média ≈ 42</text>
  <text x="560" y="18" text-anchor="middle" font-weight="800" class="t-good">Burndown (pontos restantes)</text>
  <path class="line" d="M400 220H720M400 220V30"/>
  <polyline points="410,45 460,70 510,92 560,118 610,138 660,165 710,185" fill="none" stroke="var(--good)" stroke-width="3"/>
  <g fill="var(--good)"><circle cx="410" cy="45" r="4"/><circle cx="460" cy="70" r="4"/><circle cx="510" cy="92" r="4"/><circle cx="560" cy="118" r="4"/><circle cx="610" cy="138" r="4"/><circle cx="660" cy="165" r="4"/><circle cx="710" cy="185" r="4"/></g>
  <path d="M710 185L720 200" stroke="var(--ink-3)" stroke-dasharray="4 3"/>
  <text x="560" y="245" text-anchor="middle" font-size="11" class="t-dim">a inclinação indica a data final provável</text>
</svg>
:::

A inclinação do burndown **não** é igual à altura das barras de velocidade porque histórias novas entram e estimativas são revistas. Com os dois gráficos na parede, qualquer pessoa sabe, em segundos, quando o próximo marco será cumprido e **quanto o escopo cresceu**. No fundo, o método ágil consiste em **gerar informação confiável de gestão**.

::: cuidado Agilidade não garante contentamento
Os interessados nem sempre gostarão do que os dados mostram — principalmente no começo. Ser ágil **não significa que eles terão o que querem**; significa que terão **controle** para obter o maior valor ao menor custo.
:::

::: exercicio Planeje uma iteração
Sua velocidade nas últimas iterações foi 20, 24 e 22 pontos. O cliente quer estas histórias: **Login (5)**, **Cadastro de cliente (8)**, **Relatório mensal (13)**, **Exportar PDF (5)**, **Alterar senha (3)**.

1. Qual orçamento você oferece?
2. Quais histórias cabem (prioridade: Login, Cadastro, Alterar senha, Exportar PDF, Relatório)?
3. O que fazer com o relatório?

--- resposta
1. A média das últimas é 22, mas a regra mais simples é usar a **última medida**: **22 pontos**.
2. Login (5) + Cadastro (8) + Alterar senha (3) + Exportar PDF (5) = **21 pontos** — cabe. Somar o relatório (13) estouraria muito.
3. O Relatório (13) fica para a próxima iteração (ou é **dividido** em partes menores, como *Relatório por cliente* e *Relatório totalizado*, e a primeira parte entra se couber).
:::

## Desenvolvimento guiado por testes (TDD)

Escrever um teste de unidade é mais um ato de **projeto** do que de verificação: você decide **como o código será usado** antes de existir. Seguindo três regras simples:

::: regra As três regras do TDD
1. **Não escreva código de produção** até ter escrito um **teste que falha**.
2. **Não escreva mais teste** do que o suficiente para falhar (e não compilar já conta como falha).
3. **Não escreva mais código de produção** do que o suficiente para **passar** no teste que falhava.
:::

Isso cria um ciclo de **1 a 2 minutos**, conhecido como **vermelho → verde → refatorar**:

::: figura O ciclo do TDD
<svg viewBox="0 0 700 220" role="img" aria-label="Vermelho, verde, refatorar">
  <g text-anchor="middle" font-size="14">
    <rect class="f-bad" x="10" y="70" width="190" height="80" rx="16"/><text x="105" y="102" font-weight="800" class="t-bad">1 · VERMELHO</text><text x="105" y="126" font-size="12" class="t-dim">escreva um teste que falha</text>
    <rect class="f-good" x="255" y="70" width="190" height="80" rx="16"/><text x="350" y="102" font-weight="800" class="t-good">2 · VERDE</text><text x="350" y="126" font-size="12" class="t-dim">o mínimo para passar</text>
    <rect class="f-info" x="500" y="70" width="190" height="80" rx="16"/><text x="595" y="102" font-weight="800" class="t-info">3 · REFATORAR</text><text x="595" y="126" font-size="12" class="t-dim">limpe, mantendo verde</text>
  </g>
  <path class="arrow" d="M202 110H251"/><path class="arrow" d="M447 110H496"/><path class="arrow" d="M595 152C595 205 105 205 105 152"/>
</svg>
:::

### Um exemplo em C# (com NUnit/xUnit)

Queremos uma classe `Carrinho` com desconto. Começamos **pelo teste**, escrito como gostaríamos de usar o código (*programação por intenção*):

```cs CarrinhoTests.cs
using NUnit.Framework;

[TestFixture]
public class CarrinhoTests
{
    [Test]
    public void CarrinhoNovo_TemTotalZero()
    {
        var carrinho = new Carrinho();
        Assert.AreEqual(0m, carrinho.Total);
    }

    [Test]
    public void AdicionarItens_SomaOsPrecos()
    {
        var carrinho = new Carrinho();
        carrinho.Adicionar("Caneta", 3.50m);
        carrinho.Adicionar("Caderno", 12.00m);
        Assert.AreEqual(15.50m, carrinho.Total);
    }

    [Test]
    public void ComprasAcimaDe100_GanhamDezPorCentoDeDesconto()
    {
        var carrinho = new Carrinho();
        carrinho.Adicionar("Mochila", 200m);
        Assert.AreEqual(180m, carrinho.Total);
    }
}
```

1. O código nem compila (`Carrinho` não existe) — **vermelho**. Criamos a classe vazia, só para compilar.
2. O **primeiro** teste falha (ou passa, se `Total` valer 0). Fazemos o **mínimo** para passar:

```cs Carrinho.cs
public class Carrinho
{
    private decimal soma;
    public decimal Total { get { return soma; } }
    public void Adicionar(string produto, decimal preco) { soma += preco; }
}
```

3. O segundo passa de graça. O **terceiro** falha — escrevemos o desconto:

```cs
public decimal Total { get { return soma > 100m ? soma * 0.9m : soma; } }
```

4. **Refatoramos** (extrair constantes, dar nomes) e rodamos **todos** os testes de novo.

::: dica Convenções úteis
Nomeie testes pelo comportamento (`Cenario_ResultadoEsperado`). Siga **Arrange / Act / Assert** (preparar, executar, verificar). Um teste verifica **uma** coisa. Testes rápidos e independentes entre si.
:::

### O que o TDD faz por você

- **Rede de segurança:** todo comportamento tem teste; se algo quebrar, você vê **na hora** e perde menos tempo depurando.
- **Liberdade para mudar:** como os testes avisam, você acrescenta funções e **altera a estrutura** sem medo.
- **Projeto melhor:** escrevendo o teste primeiro você enxerga o código **do ponto de vista do chamador** — a interface sai mais natural.
- **Documentação que não mente:** os testes mostram como criar e chamar cada coisa; compilam, rodam e **estão sempre atualizados**.
- **Desacoplamento:** para testar uma classe isoladamente, ela **precisa** ser independente do ambiente. Testar primeiro **força** o desacoplamento.

::: pense Programação por intenção
Escreva o teste **da maneira que você quer lê-lo**. Ele mostra a intenção de forma simples e clara — e esse jeito costuma apontar para uma boa estrutura. Às vezes ele revela decisões de projeto cedo: um teste de "mover o jogador" pode mostrar que nem era preciso uma classe `Sala` — bastava ligar números de salas entre si.
:::

## Isolamento de teste: stubs e mocks

Imagine uma classe `FolhaDePagamento` que lê funcionários de um banco, calcula pagamentos e **imprime cheques**:

::: figura Acoplada × desacoplada para teste
<svg viewBox="0 0 740 280" role="img" aria-label="Antes: Payroll depende das classes reais; depois: depende de interfaces com mocks">
  <text x="170" y="20" text-anchor="middle" font-weight="800" class="t-bad">Acoplada (difícil de testar)</text>
  <rect class="f-prim" x="90" y="35" width="160" height="46" rx="10"/><text x="170" y="64" text-anchor="middle" font-weight="700">FolhaDePagamento</text>
  <path class="arrow" d="M120 82V120"/><path class="arrow" d="M170 82V120"/><path class="arrow" d="M220 82V120"/>
  <g font-size="12" text-anchor="middle"><rect class="f-bad" x="10" y="122" width="100" height="44" rx="8"/><text x="60" y="148">BancoReal</text><rect class="f-bad" x="125" y="122" width="100" height="44" rx="8"/><text x="175" y="148">Impressora</text><rect class="f-bad" x="240" y="122" width="100" height="44" rx="8"/><text x="290" y="148">Funcionário</text></g>
  <text x="170" y="195" text-anchor="middle" font-size="12" class="t-dim">precisa de banco, impressora…</text>
  <text x="560" y="20" text-anchor="middle" font-weight="800" class="t-good">Desacoplada por interfaces</text>
  <rect class="f-prim" x="480" y="35" width="160" height="46" rx="10"/><text x="560" y="64" text-anchor="middle" font-weight="700">FolhaDePagamento</text>
  <path class="arrow" d="M510 82V112"/><path class="arrow" d="M560 82V112"/><path class="arrow" d="M610 82V112"/>
  <g font-size="11" text-anchor="middle"><rect class="f-info" x="400" y="114" width="100" height="40" rx="8"/><text x="450" y="138">«interface» IBanco</text><rect class="f-info" x="515" y="114" width="100" height="40" rx="8"/><text x="565" y="138">«interface» IImpressor</text><rect class="f-info" x="630" y="114" width="100" height="40" rx="8"/><text x="680" y="138">«interface» IFunc.</text></g>
  <g font-size="11" text-anchor="middle"><rect class="f-good" x="400" y="190" width="100" height="40" rx="8"/><text x="450" y="214">BancoFalso</text><rect class="f-good" x="515" y="190" width="100" height="40" rx="8"/><text x="565" y="214">ImpressorFalso</text><rect class="f-good" x="630" y="190" width="100" height="40" rx="8"/><text x="680" y="214">FuncFalso</text></g>
  <path class="line" d="M450 156V188M565 156V188M680 156V188" stroke-dasharray="4 3"/>
  <text x="560" y="262" text-anchor="middle" font-size="12" class="t-dim">no teste, trocamos pelos falsos</text>
</svg>
:::

Como testar o pagamento sem montar um banco **de verdade** e sem verificar um cheque **impresso**? Inserimos **interfaces** entre a classe e seus colaboradores e criamos **objetos simulados** (*mock objects*, *stubs*, *fakes*) que as implementam — o teste então **pergunta aos simulados** se foram usados corretamente.

```cs
public interface IBancoDeFuncionarios { IEnumerable<Funcionario> Todos(); void RegistrarPagamento(int id, decimal valor); }
public interface IEmissorDeCheques   { void Emitir(string nome, decimal valor); }

public class FolhaDePagamento
{
    private readonly IBancoDeFuncionarios banco;
    private readonly IEmissorDeCheques emissor;
    public FolhaDePagamento(IBancoDeFuncionarios banco, IEmissorDeCheques emissor)
    { this.banco = banco; this.emissor = emissor; }

    public void PagarTodos()
    {
        foreach (var f in banco.Todos())
        {
            decimal valor = f.CalcularPagamento();
            emissor.Emitir(f.Nome, valor);
            banco.RegistrarPagamento(f.Id, valor);
        }
    }
}

// simulados escritos à mão
class EmissorFalso : IEmissorDeCheques
{
    public List<(string Nome, decimal Valor)> Emitidos = new List<(string, decimal)>();
    public void Emitir(string nome, decimal valor) { Emitidos.Add((nome, valor)); }
}
class BancoFalso : IBancoDeFuncionarios
{
    public List<Funcionario> Lista = new List<Funcionario>();
    public List<(int Id, decimal Valor)> Pagamentos = new List<(int, decimal)>();
    public IEnumerable<Funcionario> Todos() { return Lista; }
    public void RegistrarPagamento(int id, decimal valor) { Pagamentos.Add((id, valor)); }
}

[Test]
public void PagarTodos_EmiteChequeERegistraPagamento()
{
    var banco = new BancoFalso();
    banco.Lista.Add(new FuncionarioMensalista(1, "Ana", 3000m));
    var emissor = new EmissorFalso();

    new FolhaDePagamento(banco, emissor).PagarTodos();

    Assert.AreEqual(1, emissor.Emitidos.Count);
    Assert.AreEqual(3000m, emissor.Emitidos[0].Valor);
    Assert.AreEqual(1, banco.Pagamentos[0].Id);
}
```

::: dica Bibliotecas de mock
Hoje é comum usar **Moq**, **NSubstitute** ou **FakeItEasy** para criar simulados em poucas linhas (`Substitute.For<IEmissorDeCheques>()`) e verificar chamadas (`emissor.Received().Emitir("Ana", 3000m)`). O princípio é o mesmo dos falsos escritos à mão.
:::

::: regra O desacoplamento "de graça"
Isolar para testar nos obriga a depender de **interfaces** e não de classes concretas — exatamente o que os princípios de projeto da Parte 8 pregam. Os testes de unidade são o incentivo e a **bússola** do bom desacoplamento.
:::

## Testes de aceitação

Os testes de unidade são **necessários**, mas **insuficientes**: eles verificam as peças. É preciso verificar se o **sistema como um todo** faz o que o cliente quer.

| | **Teste de unidade** | **Teste de aceitação** |
|---|---|---|
| Tipo | **Caixa branca** (conhece a estrutura interna) | **Caixa preta** (só entradas e saídas) |
| Quem escreve | Programadores | Cliente, analistas, testadores (e programadores) |
| Linguagem | A da programação (C#) | Tabelas simples ou texto legível (ex.: FitNesse, SpecFlow/Gherkin) |
| Responde a | "A classe faz certo?" | "O sistema atende ao requisito?" |
| Documenta | Detalhes internos | As **funcionalidades** — vira o *documento de requisitos* |

Exemplo em formato "dado / quando / então" (usado por ferramentas como SpecFlow):

```text
Funcionalidade: Pagar funcionários mensalistas
  Cenário: Pagamento no último dia útil
    Dado o funcionário Ana, mensalista com salário de 3000
    E o funcionário Beto, mensalista com salário de 2000
    Quando o pagamento é processado em 31/01/2025
    Então o contracheque de Ana tem líquido de 2400 (desconto fixo de 20%)
    E o contracheque de Beto tem líquido de 1600
```

::: pense Testes de aceitação **moldam a arquitetura**
Para o sistema ser testável sem passar pela tela, as **regras de negócio** precisam ficar **fora** da interface gráfica, acessíveis por uma **API**. Pensar nos testes de aceitação antes da primeira iteração obriga a esse desacoplamento em **grande escala** — assim como os testes de unidade o fazem em pequena escala. Por isso é um erro rodá-los à mão nas primeiras iterações: você perde essa pressão arquitetural.
:::

::: resumo
- Planeje com **histórias**, **pontos relativos**, **velocidade** medida e **orçamento** = velocidade anterior.
- O **negócio** decide valor e prioridade; os **desenvolvedores**, custo e ordem técnica.
- **Pronto** = testes de aceitação passando; acompanhe com **velocidade** e **burndown**.
- **TDD**: teste que falha → código mínimo → refatorar, em ciclos de 1–2 min.
- Testes dão **segurança, liberdade, projeto melhor e documentação**; forçam o **desacoplamento**.
- **Mocks/stubs** isolam a unidade das suas dependências (via interfaces).
- **Unidade** = caixa branca; **aceitação** = caixa preta, escritos com/ pelo cliente.
:::

::: quiz
? Em pontos de história, o que significa uma história de 8 pontos comparada a uma de 4?
+ Custa aproximadamente o dobro
- Leva exatamente 8 horas
- É duas vezes mais importante
- Deve ser feita primeiro
> Estimativas são relativas, não absolutas.

? Como se calcula a velocidade?
+ Somando as estimativas das histórias concluídas (prontas) na iteração
- Contando as linhas de código
- Somando as horas trabalhadas
- Contando o número de tarefas iniciadas
> Só histórias totalmente prontas contam.

? Qual é a primeira regra do TDD?
+ Não escrever código de produção sem antes ter um teste de unidade que falhe
- Escrever todos os testes antes de qualquer código
- Escrever testes só no fim do projeto
- Testar apenas a interface gráfica
> O teste que falha define o próximo pequeno passo.

? Qual é a ordem do ciclo do TDD?
+ Vermelho (teste falha) → verde (passa) → refatorar
- Verde → vermelho → documentar
- Refatorar → vermelho → verde
- Compilar → publicar → testar
> O ciclo dura um ou dois minutos.

? Para que serve um mock/stub em um teste de unidade?
+ Substituir uma dependência (banco, impressora…) para isolar a classe testada
- Acelerar a compilação
- Substituir o compilador
- Gerar documentação
> Evita depender de recursos pesados ou difíceis de verificar.

? Qual a diferença central entre teste de unidade e de aceitação?
+ Unidade verifica as peças por dentro; aceitação verifica o requisito do cliente por fora
- Unidade é manual; aceitação é automatizado
- Aceitação só se escreve depois de entregar
- Não há diferença
> Caixa branca × caixa preta.

? Por que escrever testes de aceitação desde a primeira iteração influencia a arquitetura?
+ Porque o sistema precisa ser acessível sem passar pela interface gráfica, o que exige desacoplar a UI das regras de negócio
- Porque obriga a usar um banco de dados específico
- Porque substitui os testes de unidade
- Porque elimina a necessidade de design
> A testabilidade em alto nível pressiona por uma boa arquitetura.

? O que o gráfico de burndown mostra?
+ Quantos pontos de história ainda restam até o próximo marco
- Quantas horas a equipe trabalhou
- O número de bugs abertos
- A velocidade média
> A inclinação indica a data provável de término.
:::

::: cartas
Ponto de história :: Unidade relativa de custo; 8 pontos custam o dobro de 4.
Velocidade :: Soma dos pontos das histórias concluídas por iteração; define o orçamento da próxima.
Burndown :: Gráfico dos pontos que faltam para o marco/entrega.
Tarefa :: Parte de uma história que um desenvolvedor faz em 4–16 horas.
Definição de "pronto" :: Todos os testes de aceitação da história passam.
TDD (3 regras) :: Teste que falha primeiro; teste mínimo para falhar; código mínimo para passar.
Vermelho-verde-refatorar :: Ciclo do TDD.
Programação por intenção :: Escrever o teste como se o código ideal já existisse.
Mock / stub / fake :: Objeto simulado que substitui uma dependência nos testes.
Teste de caixa branca :: Conhece a estrutura interna (unidade).
Teste de caixa preta :: Só entradas e saídas (aceitação).
Arrange-Act-Assert :: Estrutura de um teste: preparar, executar, verificar.
:::
