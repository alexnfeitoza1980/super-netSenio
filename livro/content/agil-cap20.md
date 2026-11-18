---
id: agil-cap20
order: 36
kind: capitulo
num: 20
group: C# · Parte 8 · Projeto ágil e princípios SOLID
title: Princípio da Responsabilidade Única (SRP)
subtitle: "Uma classe deve ter um, e somente um, motivo para mudar" — coesão, eixos de mudança e como separar responsabilidades acopladas.
minutes: 35
hue: 15
objectives:
- Enunciar o **SRP** e definir **responsabilidade** como "motivo para mudar"
- Identificar violações comuns (regra de negócio + persistência, tela + cálculo)
- Separar responsabilidades por **interfaces** e saber **quando não** separar
- Relacionar o SRP com **coesão**, **rigidez** e **fragilidade**
origem: livros/02-projeto-agil-csharp/content/35-cap20.md
---

::: ilustracao img/agil/cap20.svg | SRP: uma classe, uma razão para mudar. Ferramentas separadas, cada uma boa em uma coisa.
:::

## O princípio

::: regra SRP — Single Responsibility Principle
**Uma classe deve ter apenas um motivo para mudar.**
:::

Esse princípio vem da ideia de **coesão** (Tom DeMarco, Meilir Page-Jones): afinidade funcional entre os elementos de um módulo. Aqui a coesão ganha uma versão prática: relacionamos o módulo com as **forças que o fazem mudar**.

No laboratório do boliche, a classe `Jogo` acumulava duas tarefas: **acompanhar os arremessos/quadros** e **calcular o placar**. No fim, foram separadas em `Jogo` e `Pontuador`. Por quê? Porque **cada responsabilidade é um eixo de mudança**. Quando os requisitos mudam, a alteração aparece como mudança numa responsabilidade. Se uma classe tem duas, ela tem dois motivos para mudar — e as duas ficam **acopladas**: mexer numa pode **quebrar** a outra. É assim que nascem projetos **frágeis** e **rígidos**.

::: analogia O canivete suíço
Um canivete suíço faz tudo — e nada *bem*. Se a tesoura quebra, o conserto mexe no cabo inteiro, que também segura a lâmina e o abridor. Numa cozinha profissional, cada faca tem um uso; quando uma precisa de afiação, as demais seguem intactas.
:::

## Exemplo clássico: forma geométrica com desenho

Considere uma classe `Retangulo` com dois métodos: `CalcularArea()` e `Desenhar()`. Dois aplicativos a usam:

- um de **geometria computacional**, que só faz contas (nunca desenha);
- um **gráfico**, que desenha (e também faz algumas contas).

::: figura Retângulo com duas responsabilidades
<svg viewBox="0 0 740 260" role="img" aria-label="Antes: Retângulo com Area e Desenhar acoplando dois aplicativos; depois: duas classes">
  <text x="170" y="18" text-anchor="middle" font-weight="800" class="t-bad">Antes: duas responsabilidades</text>
  <rect class="f-bad" x="100" y="90" width="140" height="70" rx="10"/><text x="170" y="116" text-anchor="middle" font-weight="800">Retangulo</text><text x="170" y="138" text-anchor="middle" font-size="12" class="t-mono">Desenhar()  Area()</text>
  <rect class="f-plain" x="10" y="30" width="150" height="38" rx="8"/><text x="85" y="54" text-anchor="middle" font-size="12">App de geometria</text>
  <rect class="f-plain" x="180" y="30" width="150" height="38" rx="8"/><text x="255" y="54" text-anchor="middle" font-size="12">App gráfico</text>
  <path class="arrow" d="M110 70L140 88"/><path class="arrow" d="M240 70L210 88"/>
  <rect class="f-warn" x="110" y="190" width="120" height="40" rx="8"/><text x="170" y="215" text-anchor="middle" font-size="12">biblioteca GUI</text>
  <path class="arrow" d="M170 162V188"/>
  <text x="560" y="18" text-anchor="middle" font-weight="800" class="t-good">Depois: responsabilidades separadas</text>
  <rect class="f-good" x="400" y="90" width="150" height="70" rx="10"/><text x="475" y="116" text-anchor="middle" font-weight="800">RetanguloGeometrico</text><text x="475" y="138" text-anchor="middle" font-size="12" class="t-mono">Area()</text>
  <rect class="f-info" x="590" y="90" width="140" height="70" rx="10"/><text x="660" y="116" text-anchor="middle" font-weight="800">RetanguloGrafico</text><text x="660" y="138" text-anchor="middle" font-size="12" class="t-mono">Desenhar()</text>
  <rect class="f-plain" x="370" y="30" width="150" height="38" rx="8"/><text x="445" y="54" text-anchor="middle" font-size="12">App de geometria</text>
  <path class="arrow" d="M450 70V88"/>
  <rect class="f-plain" x="600" y="30" width="130" height="38" rx="8"/><text x="665" y="54" text-anchor="middle" font-size="12">App gráfico</text>
  <path class="arrow" d="M665 70V88"/>
  <path class="line" d="M552 125H588" stroke-dasharray="4 3"/>
  <rect class="f-warn" x="600" y="190" width="120" height="40" rx="8"/><text x="660" y="215" text-anchor="middle" font-size="12">biblioteca GUI</text>
  <path class="arrow" d="M660 162V188"/>
</svg>
:::

Os problemas da versão acoplada:

1. O app de geometria **precisa levar a biblioteca de interface gráfica junto**, mesmo sem usá-la.
2. Se uma mudança no app gráfico alterar `Retangulo`, o app de geometria precisa ser **recompilado, retestado e reentregue** — e se alguém esquecer, ele pode quebrar de formas imprevisíveis.

A solução é separar: o cálculo fica em `RetanguloGeometrico`; o desenho, numa classe própria (que **usa** o geométrico). Mudanças no desenho **não podem mais** afetar o app de geometria.

## Definir "responsabilidade" = motivo para mudar

Se você consegue pensar em **mais de um motivo** para alterar uma classe, ela tem mais de uma responsabilidade. A dificuldade: estamos acostumados a agrupar coisas por *afinidade* e não por *motivos de mudança*.

```cs Antes — uma interface "razoável"
public interface IConexao
{
    void Conectar(string endereco);
    void Desconectar();
    void Enviar(byte[] dados);
    byte[] Receber();
}
```

Parece perfeitamente normal para uma conexão. Mas há **duas responsabilidades**: **gerenciar a conexão** (`Conectar`/`Desconectar`) e **transportar dados** (`Enviar`/`Receber`). Elas devem ser separadas? **Depende de como o aplicativo muda.**

- Se a forma de **conectar** muda (autenticação, proxy, novos parâmetros) e isso força recompilar quem apenas **envia/recebe** — o projeto cheira a **rigidez**. Separe.
- Se os dois lados **nunca mudam em momentos diferentes**, separá-los é **complexidade desnecessária**.

::: regra Um eixo de mudança só existe se a mudança acontecer
Não aplique o SRP (nem qualquer outro princípio) **sem um sintoma**. Separar "por via das dúvidas" gera classes demais e indireção inútil. Observe as mudanças reais; reaja quando houver dor.
:::

### Separando responsabilidades acopladas

```cs Depois
public interface IGerenciadorDeConexao
{
    void Conectar(string endereco);
    void Desconectar();
}

public interface ICanalDeDados
{
    void Enviar(byte[] dados);
    byte[] Receber();
}

// a implementação pode, por razões de hardware/SO, juntar as duas coisas
public class ConexaoTcp : IGerenciadorDeConexao, ICanalDeDados
{
    public void Conectar(string endereco) { /* ... */ }
    public void Desconectar() { /* ... */ }
    public void Enviar(byte[] dados) { /* ... */ }
    public byte[] Receber() { return null; }
}
```

A classe `ConexaoTcp` continua acoplando as duas coisas — às vezes **é necessário** (detalhes do sistema operacional, do hardware). Mas, **para o resto do aplicativo**, os conceitos estão desacoplados: quem só envia dados depende só de `ICanalDeDados`. A "verruga" fica **atrás de uma cerca**: só quem cria o objeto precisa saber que ela existe.

## A violação mais comum: regras de negócio + persistência

```cs Violação
public class Funcionario
{
    public decimal SalarioBase { get; set; }

    public decimal CalcularPagamento()            // REGRA DE NEGÓCIO
    {
        return SalarioBase * 1.1m;
    }

    public void Salvar()                          // PERSISTÊNCIA
    {
        using (var conn = new SqlConnection("..."))
        { /* INSERT / UPDATE */ }
    }
}
```

Regras de negócio mudam com frequência (o RH cria um novo adicional). A persistência muda por **outros motivos** (trocar de banco, de tabela, de ORM). Misturá-las é **procurar problema**: toda mudança no banco recompila e retesta as regras, e vice-versa.

::: figura Separando regra de negócio e persistência
<svg viewBox="0 0 740 200" role="img" aria-label="Funcionario e RepositorioDeFuncionarios separados">
  <rect class="f-bad" x="10" y="50" width="190" height="90" rx="12"/><text x="105" y="80" text-anchor="middle" font-weight="800">Funcionario</text><text x="105" y="104" text-anchor="middle" font-size="12" class="t-mono">CalcularPagamento()</text><text x="105" y="124" text-anchor="middle" font-size="12" class="t-mono">Salvar()</text>
  <path class="arrow" d="M215 95H275" style="stroke:var(--ink-2);marker-end:url(#arr2)"/><text x="245" y="85" text-anchor="middle" font-size="12" class="t-dim">refatorar</text>
  <rect class="f-good" x="290" y="20" width="200" height="60" rx="12"/><text x="390" y="46" text-anchor="middle" font-weight="800" class="t-good">Funcionario</text><text x="390" y="66" text-anchor="middle" font-size="12" class="t-mono">CalcularPagamento()</text>
  <rect class="f-info" x="290" y="115" width="200" height="60" rx="12"/><text x="390" y="141" text-anchor="middle" font-weight="800" class="t-info">IRepositorioDeFuncionarios</text><text x="390" y="161" text-anchor="middle" font-size="12" class="t-mono">Salvar(f)  Obter(id)</text>
  <rect class="f-plain" x="540" y="115" width="190" height="60" rx="12"/><text x="635" y="141" text-anchor="middle" font-weight="700">RepositorioSql</text><text x="635" y="161" text-anchor="middle" font-size="12" class="t-dim">detalhes de banco</text>
  <path class="line" d="M540 145H492" stroke-dasharray="4 3"/>
</svg>
:::

```cs Depois
public class Funcionario
{
    public decimal SalarioBase { get; set; }
    public decimal CalcularPagamento() { return SalarioBase * 1.1m; }
}

public interface IRepositorioDeFuncionarios
{
    void Salvar(Funcionario f);
    Funcionario Obter(int id);
}

public class RepositorioSql : IRepositorioDeFuncionarios { /* SQL aqui */ public void Salvar(Funcionario f) { } public Funcionario Obter(int id) { return null; } }
```

::: dica TDD já empurra para o SRP
Para testar `CalcularPagamento` você **não vai querer** um banco real. O teste força a separação muito antes de o projeto cheirar mal. Se os testes não forçarem e a rigidez/fragilidade aparecerem, refatore com padrões como **Façade**, **DAO/Repository** ou **Proxy**.
:::

## Sinais de que uma classe tem responsabilidades demais

- O nome só se explica com **"e"** ou **"Gerenciador"/"Utilitário"** genérico (`ClienteEPedidoManager`).
- Os `using` do arquivo misturam **assuntos distintos** (banco, interface gráfica, e-mail, regras).
- Métodos agrupam-se em "ilhas" que **usam campos diferentes**.
- Toda alteração exige mexer **na mesma classe** por razões não relacionadas (conflitos de merge frequentes).
- Os testes precisam de **muita preparação** (banco, arquivos, tela) para verificar uma regra simples.

::: exercicio Quantas responsabilidades?
Analise esta classe e proponha uma divisão:

```cs
public class Pedido
{
    public List<Item> Itens;
    public decimal CalcularTotal() { /* soma + frete + impostos */ return 0; }
    public void EnviarEmailDeConfirmacao() { /* SMTP */ }
    public void SalvarNoBanco() { /* SQL */ }
    public string GerarHtmlParaTela() { return "<table>..."; }
}
```

--- resposta
Quatro motivos para mudar: **regra de preço/imposto**, **comunicação por e-mail**, **persistência** e **apresentação**. Divisão sugerida:

- `Pedido` (dados + `CalcularTotal`, talvez via `CalculadoraDePreco`);
- `INotificadorDePedido`/`EmailNotificador`;
- `IRepositorioDePedidos`;
- `PedidoView`/`RenderizadorHtml`.

Um serviço de aplicação (`FinalizarPedido`) **coordena** as peças. Cada uma pode mudar (e ser testada) sem afetar as outras.
:::

::: resumo
- **SRP:** uma classe deve ter **um único motivo para mudar**.
- **Responsabilidade = eixo de mudança**; responsabilidades acopladas geram projetos frágeis e rígidos.
- Separe por **interfaces** quando as mudanças realmente ocorrem em momentos diferentes; senão, é complexidade desnecessária.
- Violação típica: **regra de negócio + persistência** (ou + interface gráfica).
- Às vezes a implementação mantém o acoplamento, mas as **interfaces** isolam o resto do sistema.
- Aplique o SRP **quando houver sintoma** e deixe o TDD ajudar.
:::

::: quiz
? Qual é o enunciado do SRP?
+ Uma classe deve ter apenas um motivo para mudar
- Uma classe deve ter apenas um método
- Uma classe deve ter apenas um campo
- Uma classe deve ter apenas um construtor
> "Responsabilidade" é sinônimo de "motivo para mudar".

? Por que misturar regras de negócio e persistência é problemático?
+ Porque mudam por motivos diferentes e uma mudança pode afetar/quebrar a outra
- Porque o C# não permite acessar banco em classes de negócio
- Porque deixa o programa mais lento
- Porque duplica o número de classes
> Acoplar responsabilidades gera fragilidade e rigidez.

? Quando NÃO vale a pena separar duas responsabilidades?
+ Quando elas nunca mudam em momentos diferentes (separar seria complexidade desnecessária)
- Quando a classe tem mais de 5 métodos
- Quando há interfaces
- Nunca: sempre se deve separar
> Um eixo de mudança só existe se as mudanças acontecem.

? Qual a vantagem de interfaces separadas mesmo que uma classe implemente ambas?
+ O restante do sistema depende só do conceito de que precisa; a "verruga" fica atrás de uma cerca
- A classe implementadora fica menor
- Dispensa a classe implementadora
- Impede a herança
> Clientes ficam desacoplados.

? Qual prática ajuda a forçar o SRP desde cedo?
+ TDD: testar sem banco/tela exige separar responsabilidades
- Escrever mais comentários
- Usar classes estáticas
- Evitar interfaces
> Testabilidade pressiona o desacoplamento.

? Qual é um sintoma de classe com responsabilidades demais?
+ Nome com "e", ou "Gerenciador/Util" genérico, e testes que exigem muita preparação
- Uso de propriedades automáticas
- Ter um construtor
- Ser pública
> Esses sinais indicam assuntos misturados.
:::

::: cartas
SRP :: Uma classe deve ter um único motivo para mudar.
Responsabilidade :: Um eixo de mudança; um motivo para alterar a classe.
Coesão :: Afinidade funcional dos elementos de um módulo.
Persistência :: Armazenar e recuperar dados (banco, arquivo).
Repository / DAO :: Objeto que isola o acesso a dados.
Verruga atrás da cerca :: Acoplamento inevitável isolado por interfaces, sem poluir o resto.
Sintoma :: Evidência real de dor (rigidez/fragilidade) que justifica aplicar um princípio.
:::
