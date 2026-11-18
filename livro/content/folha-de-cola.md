---
id: folha-de-cola
order: 375
kind: extra
group: Referência
title: Folhas de cola
subtitle: Resumos de consulta rápida de cada livro, reunidos.
minutes: 20
hue: 30
origem: folhas de cola de todos os livros
---

## Aprenda C#

### Estrutura de um programa

```cs
using System;

namespace MeuApp
{
    class Programa
    {
        static void Main(string[] args)          // ponto de entrada
        {
            Console.WriteLine("Olá!");
        }
    }
}
```

### Variáveis e tipos (cap. 2 e 4)

```cs
int idade = 30;            long grande = 5_000_000_000L;
double d = 3.14;           float f = 3.14f;       decimal preco = 19.90m;
bool ativo = true;         char letra = 'A';      string nome = "Ana";
var qualquer = 10;         // tipo inferido: int
const int Limite = 100;    // valor fixo
int n = (int) 3.9;         // cast → 3
int[] v = new int[5];      string[] nomes = { "a", "b" };
```

### Operadores

| Tipo | Operadores |
|---|---|
| Aritméticos | `+ - * / %` e `++ --` |
| Atribuição | `= += -= *= /= %=` |
| Comparação | `== != < > <= >=` |
| Lógicos | `&& \|\| !` |
| Texto | `+` concatena; `$"Olá {nome}"` interpola |
| Condicional | `x > 0 ? "pos" : "não pos"` |

### Decisões e laços (cap. 2)

```cs
if (x > 0) { ... } else if (x == 0) { ... } else { ... }

switch (op) { case "a": ...; break; default: ...; break; }

while (cond) { ... }
for (int i = 0; i < 10; i++) { ... }
foreach (string s in lista) { ... }
do { ... } while (cond);
break;  continue;  return valor;
```

### Classes e objetos (caps. 3, 5 e 6)

```cs
class Pessoa
{
    private int idade;                       // campo privado
    public string Nome { get; set; }         // propriedade automática
    public int Idade                         // propriedade com regra
    {
        get { return idade; }
        set { if (value >= 0) idade = value; }
    }

    public Pessoa(string nome) { Nome = nome; }          // construtor
    public virtual string Apresentar() { return "Oi, " + Nome; }
    public static int Contador;                          // da classe, não da instância
}

class Aluno : Pessoa                                      // herança
{
    public Aluno(string nome) : base(nome) { }
    public override string Apresentar() { return base.Apresentar() + " (aluno)"; }
}

Pessoa p = new Pessoa("Ana") { Idade = 20 };              // inicializador de objeto
```

### Interfaces, abstração, tipos (cap. 7)

```cs
interface IVoador { void Voar(); }
abstract class Animal { public abstract void Som(); }
class Pato : Animal, IVoador { public override void Som() { } public void Voar() { } }

if (obj is IVoador)  { ... }
IVoador v = obj as IVoador;     // null se não for
Animal a = (Animal) obj;        // pode lançar exceção
```

| Modificador | Visível para |
|---|---|
| `public` | todos |
| `private` | só a classe |
| `protected` | a classe e as subclasses |
| `internal` | o mesmo projeto |

### Coleções (cap. 8)

```cs
List<string> l = new List<string>() { "a", "b" };
l.Add("c"); l.Remove("a"); l.Contains("b"); l.Count; l.Sort();

Dictionary<string, int> d = new Dictionary<string, int>();
d["x"] = 1; d.ContainsKey("x"); d.TryGetValue("x", out int v);
foreach (var par in d) { /* par.Key, par.Value */ }

Queue<int> fila = new Queue<int>();  fila.Enqueue(1);  fila.Dequeue();
Stack<int> pilha = new Stack<int>(); pilha.Push(1);    pilha.Pop();
enum Cor { Vermelho, Verde, Azul }
```

### Arquivos (cap. 9)

```cs
File.ReadAllText(c);  File.WriteAllText(c, t);  File.Exists(c);  File.Delete(c);
using (var sw = new StreamWriter(c)) { sw.WriteLine("..."); }
using (var sr = new StreamReader(c)) { string l = sr.ReadLine(); }
Path.Combine(a, b);  Directory.GetFiles(pasta, "*.txt");
string json = JsonSerializer.Serialize(obj);
```

### Exceções (cap. 10)

```cs
try { ... }
catch (FormatException ex) { ... }          // do mais específico…
catch (Exception ex) { ...; throw; }        // …ao mais geral
finally { /* sempre executa */ }

throw new ArgumentException("mensagem");
int.TryParse(texto, out int n);             // evita exceção
```

### Eventos e delegados (cap. 11)

```cs
public event EventHandler<MeusArgs> Algo;     // declarar
Algo?.Invoke(this, new MeusArgs());           // disparar
obj.Algo += Tratador;   obj.Algo -= Tratador; // assinar / cancelar
Func<int,int> dobro = x => x * 2;             // lambda
Action<string> log = s => Console.WriteLine(s);
```

### Gráficos (cap. 13)

```cs
protected override void OnPaint(PaintEventArgs e)
{
    e.Graphics.FillEllipse(Brushes.Gold, 10, 10, 50, 50);
    e.Graphics.DrawString("Oi", Font, Brushes.Black, 10, 80);
}
Invalidate();            // pede o Paint
DoubleBuffered = true;   // sem flicker
```

### Memória (caps. 4 e 14)

| | Tipos de valor | Tipos de referência |
|---|---|---|
| Exemplos | `int double bool struct enum` | `class string array delegate` |
| Atribuir | **copia** o valor | copia a **referência** |
| `null`? | não (exceto `int?`) | sim |

```cs
using (var r = new Recurso()) { }      // Dispose automático
static string Trunc(this string s, int n) => ...;   // método de extensão (classe estática)
```

### LINQ (cap. 15)

```cs
using System.Linq;
var r = from q in lista where q.Preco > 10 orderby q.Nome select q.Nome;
var r2 = lista.Where(q => q.Preco > 10).OrderBy(q => q.Nome).Select(q => q.Nome).ToList();
lista.Count(); Sum(x => x.V); Average(); Min(); Max(); Any(); First(); FirstOrDefault();
var g = lista.GroupBy(q => q.Categoria);
var j = from a in A join b in B on a.Id equals b.AId select new { a.X, b.Y };
```

### Atalhos úteis do Visual Studio

| Atalho | Faz |
|---|---|
| [[F5]] | Executar com depuração |
| [[Ctrl]]+[[F5]] | Executar sem depuração |
| [[F9]] | Ponto de interrupção |
| [[F10]] / [[F11]] | Passo adiante / para dentro |
| [[Shift]]+[[F11]] | Passo para fora |
| [[Ctrl]]+[[K]], [[Ctrl]]+[[D]] | Formatar documento |
| [[Ctrl]]+[[.]] | Ações rápidas (lâmpada) |
| [[Ctrl]]+[[R]], [[R]] | Renomear |
| [[Ctrl]]+[[Shift]]+[[F]] | Localizar em arquivos |
| [[F12]] | Ir para definição |
| `prop` + [[Tab]] [[Tab]] | Snippet de propriedade |
| `for` / `foreach` + [[Tab]] [[Tab]] | Snippet de laço |
| `cw` + [[Tab]] [[Tab]] | `Console.WriteLine` |
| `mbox` + [[Tab]] [[Tab]] | `MessageBox.Show` |

::: dica Imprima esta página
Use o botão de imprimir do navegador: o estilo de impressão remove menus e deixa só o conteúdo.
:::

## Projeto Ágil em C#

### Princípios SOLID (caps. 20–24)

| Sigla | Princípio | Em uma frase |
|---|---|---|
| **SRP** | Responsabilidade Única | Uma classe, **um motivo** para mudar |
| **OCP** | Aberto/Fechado | Estenda **sem modificar** (abstração + polimorfismo) |
| **LSP** | Substituição de Liskov | Um subtipo cumpre o **contrato** do tipo base |
| **ISP** | Segregação de Interfaces | Interfaces **pequenas e específicas** para cada cliente |
| **DIP** | Inversão de Dependência | Dependa de **abstrações**; detalhes dependem delas |

```cs
public interface IRepositorio { Pedido Obter(int id); }          // abstração (alto nível "dono" da interface)
public class Servico { public Servico(IRepositorio r) { ... } }   // injeção por construtor (DIP)
```

### Padrões de projeto (caps. 29–40)

| Padrão | Intenção | Pista de uso |
|---|---|---|
| **Command** | Pedido como objeto | Fila, desfazer, log, thread pool |
| **Active Object** | Command + fio próprio | Tarefas assíncronas em ordem |
| **Template Method** | Esqueleto na base, passos nas derivadas | Mesma sequência, detalhes variam |
| **Strategy** | Algoritmo intercambiável | Trocar regra por injeção |
| **Façade** | Interface simples p/ subsistema | Esconder complexidade |
| **Mediator** | Intermediário entre objetos | Muitos objetos que se falam |
| **Null Object** | "Faz nada" no lugar de `null` | Evitar `if (x != null)` |
| **Singleton / Monostate** | Instância única / estado compartilhado | Recurso global (use DI!) |
| **Factory** | Encapsula a criação | Esconder classes concretas |
| **Composite** | Árvore uniforme parte/todo | Menus, arquivos, formas |
| **Observer** | Notificar dependentes | Eventos (`event`), `INotifyPropertyChanged` |
| **Abstract Server / Adapter / Bridge** | Inverter dependência / converter / separar hierarquias | Integrar APIs de terceiros |
| **Proxy / Gateway** | Controlar acesso / isolar persistência | Banco, rede, lazy loading |
| **Visitor** | Nova operação sem mexer na hierarquia | Relatórios, árvores de sintaxe |
| **Decorator** | Embrulho que acrescenta comportamento | Streams, middlewares |
| **Extension Object** | Capacidades plugáveis por tipo | Exportadores por formato |
| **State** | Estado como classe | FSM, workflows, interações |
| **MVP / MVVM** | UI fina + lógica testável | Windows Forms, WPF, MAUI |

### Pacotes e métricas (cap. 33)

| Métrica | Fórmula | Leitura |
|---|---|---|
| **Ca / Ce** | acoplamentos aferentes (entram) / eferentes (saem) | quantos dependem de mim / de quantos dependo |
| **I** (instabilidade) | Ce / (Ca + Ce) | 0 = estável · 1 = instável |
| **A** (abstração) | classes abstratas / total | 0 = concreto · 1 = abstrato |
| **D′** (distância) | \|A + I − 1\| | 0 = **sequência principal** (ótimo) |

Princípios: **REP/CRP** (reutilizar = entregar junto) · **CCP** (mudam juntas, ficam juntas) · **ADP** (sem ciclos) · **SDP** (dependa na direção da estabilidade) · **SAP** (estável ⇒ abstrato).

### XP, TDD e refatoração (caps. 16–18)

- **TDD:** 🔴 escreva um teste que falha → 🟢 o código mínimo que passa → 🔵 refatore → repita.
- **Refatoração:** só com testes verdes; passos pequenos (extrair método, renomear, mover, extrair classe).
- **Práticas XP:** integração contínua, programação em pares, propriedade coletiva, ritmo sustentável, entregas pequenas, histórias de usuário, planejamento por iteração.

```cs
[Test]
public void SomaDoisNumeros()
{
    Assert.AreEqual(5, new Calculadora().Somar(2, 3));    // NUnit
}
```

### UML essencial (caps. 25–27)

| Diagrama | Mostra | Use para |
|---|---|---|
| **Classes** | Estrutura estática, herança, associação | Discutir o projeto |
| **Sequência** | Mensagens no tempo | Fluxo de uma operação |
| **Estados** | Estados e transições | Comportamento dependente de histórico |
| **Casos de uso** | Atores e funcionalidades | Escopo/requisitos |
| **Objetos** | Instâncias num instante | Exemplificar estruturas |

## As 16 Leis do Sucesso em C#

### As 16 leis do sucesso, em C# (caps. 41–57)

| # | Lei | Em C#, pense em… |
|---|---|---|
| 1 | Propósito | `record` + `required` para metas; teste de aceitação; `Utils` = sem propósito |
| 2 | Mente Mestra | Interfaces, composição, DI (`IEnumerable<IX>`), code review |
| 3 | Autoconfiança | xUnit (`[Fact]`/`[Theory]`), testes de caracterização, CI, `string?` |
| 4 | Economia | Dívida técnica, reserva de 10%, `using`, `StringBuilder`, `Span<T>`, cache LRU |
| 5 | Iniciativa | `async/await`, `Task.WhenAll`, `Parallel.ForEachAsync`, `BackgroundService` |
| 6 | Imaginação | API fluente, `record` + *pattern matching*, LINQ, *spikes*, YAGNI |
| 7 | Entusiasmo | `event`, `IProgress<T>`, marcos, sequência de dias |
| 8 | Autocontrole | *Guard clauses*, `throw;`, retentativas com recuo exponencial, `TreatWarningsAsErrors` |
| 9 | Além do exigido | XML docs, `ILogger` estruturado, testes de borda, definição de pronto |
| 10 | Personalidade agradável | Menor surpresa, `TryParse`, mensagens úteis, review gentil |
| 11 | Precisão | `decimal`, `Dinheiro`/`Email`, `DateTimeOffset`, `enum`, *parse, don't validate* |
| 12 | Concentração | WIP, `Channel<T>` limitado, Pomodoro, SRP |
| 13 | Cooperação | `CancellationToken`, *graceful shutdown*, middleware, `dotnet list package` |
| 14 | Fracassos | `Resultado<T>`, `Meter`/`Counter`, *post-mortem* sem culpa, ciclo praticar→monitorar |
| 15 | Tolerância | `System.Text.Json` tolerante, cultura, compatibilidade futura, *path traversal* |
| 16 | Regra de Ouro | LSP + testes de contrato, APIs honestas, `Segredo`, SQL parametrizado |

```cs
// Retentativa só para falhas passageiras, com recuo exponencial (Lei 8)
catch (Exception e) when (ehPassageira(e) && tentativa < max) { await Task.Delay(Exponencial(tentativa)); }

// Cancelamento cooperativo (Lei 13)
if (ct.IsCancellationRequested) return resultadoParcial;

// Falha como valor (Lei 14)
return erros.Count == 0 ? Resultado<T>.Ok(valor) : Resultado<T>.Falha(erros);

// Consulta parametrizada (Lei 16)
cmd.CommandText = "SELECT * FROM clientes WHERE nome = $nome"; cmd.Parameters.AddWithValue("$nome", nome);
```

## Foco em C#

### Foco em C#: atenção de quem programa (caps. 58–73)

| Ideia (Goleman) | Cap. | Em C#, pense em… |
|---|---|---|
| Triplo foco: interno, no outro, externo | 58 | `record` de autoavaliação (0–10) com `MaisFraco()` e `Equilibrio` |
| Atenção seletiva e distrações | 59 | `enum Decisao`, política de interrupções, resumo periódico, custo de troca |
| Piloto automático × deliberado | 60 | Priorização `custo × frequência`, checklist crítico, pausa antes de reagir |
| Divagação e casulo criativo | 61 | Auditoria de agenda (`Bloco`), `CadernoDeIdeias` |
| Fadiga e restauração da atenção | 62 | `Energia` 0–100 com `Math.Clamp`, taxas ilustrativas, planejador de pausas |
| Autoconsciência | 63 | Avaliação 360 (`Diferenca`, ponto cego), diário de palpites e calibração |
| Autocontrole | 64 | Plano "se–então", `valor / (1 + k × atraso)`, pré-compromisso |
| Empatia (cognitiva, emocional, compaixão) | 65 | Pesos de sinais de frustração, resposta em três partes |
| Sensibilidade social | 66 | `switch` por contexto, mediana e P90 da latência de *review* |
| Pensamento sistêmico | 67 | Simulação semana a semana com laço de reforço e atraso |
| Alavancas e impacto | 68 | Busca em largura no grafo de dependências, Pareto 80/20 |
| Prática deliberada | 69 | Treinador adaptativo (sobe/desce nível), limite diário |
| Jogos de atenção e transferência | 70 | Go/No-Go: acertos, omissões, comissões; ganho na tarefa **não treinada** |
| Atenção plena | 71 | Repetições = retornos; tempo para perceber; contador de respirações |
| Líder bem focado | 72 | *Strategy* para estilos, seletor com regras em ordem, repertório |
| Longo prazo | 73 | Saldo por horizonte, custos externos, "quem não está na sala", pré-mortem |

```cs
// Interrupção: atrai ≠ precisa (cap. 59)
return n.Urgente && vips.Contains(n.Origem) ? Decisao.EntregarAgora : Decisao.AgruparNoResumo;

// Desconto hiperbólico: o futuro "encolhe" com a impaciência k (cap. 64)
double percebido = valor / (1 + k * atrasoEmDias);

// Gatilho concreto + ação decidida de antemão (cap. 64)
plano.Adicionar("o chat apitar", "ignorar até o fim do bloco");

// Mediana resiste a extremos; olhe também o P90 (cap. 66)
var ordenados = valores.OrderBy(v => v).ToList();
```

::: cuidado Modelo é mapa, não território
Os números dos capítulos 59 a 73 (minutos por troca, taxas de energia, limiares, pesos) são **ilustrativos**. Meça o seu caso, e lembre que várias ideias populares do livro (marshmallow, razão de positividade, treino cerebral, esgotamento do ego) têm **ressalvas** de pesquisa, indicadas nos capítulos.
:::

## Os Quatro Compromissos em C#

| Tema | No caderno | O que o código garante | O que não garante |
|---|---|---|---|
| Acordos herdados | Inventario | Identidade e busca por texto | Descobrir crenças inconscientes |
| Palavra | Promessa | Estados e histórico de revisões | Consentimento ou entrega real |
| Lado pessoal | Reflexao e RespostaEscolhida | Campos separados e presentes | Dizer o que outra pessoa sentiu |
| Suposições | Pergunta | Pendência até registrar evidência | Certificar a fonte |
| Melhor possível | PlanoDoDia | Soma dentro do tempo declarado | Medir energia ou esforço humano |
| Prática | DiarioDePratica | Preservar ações e datas | Medir felicidade ou liberdade |
| Retomada | ArquivoJson | Salvar e ler um retrato local | Segurança e concorrência de produção |

### Comandos para não decorar

```bash
dotnet build codigo/compromissos/Caderno.slnx -m:1
dotnet run --project codigo/compromissos/Caderno.Console -- cap74
dotnet run --project codigo/compromissos/Caderno.Console -- cap87
dotnet run --project codigo/compromissos/Caderno.Console -- novo meu-caderno.json
dotnet run --project codigo/compromissos/Caderno.Console -- praticar meu-caderno.json "Uma ação" "Um aprendizado"
dotnet run --project codigo/compromissos/Caderno.Console -- ler meu-caderno.json
dotnet run --project codigo/compromissos/Caderno.Testes
python ferramentas/verificar.py
python ferramentas/testar-console.py
```

### O que revisar quando algo não funcionar

Confira a pasta atual, a existência do SDK 10 e os argumentos entre aspas. Um arquivo ausente é diferente de JSON inválido. Uma pergunta refutada é diferente de pergunta sem evidência. Um objeto atualizado é diferente do retrato salvo antes da atualização. Uma consulta filtrada é diferente de excluir o histórico.

## Resiliência em C#

### Resiliência em C# (caps. 88–96 e Lab 10)

| Ideia | Cap. | Em C#, pense em… |
|---|---|---|
| Recuperar, adaptar, buscar apoio | 88 | `enum Resposta`, `record Dificuldade`, `switch` expression |
| Falhas existem; tratar sem quebrar | 88 | `try / catch / finally`; `int.TryParse` para erros esperados |
| Nomear a emoção com precisão | 89 | `switch` com tupla `(emoção, intensidade)` e padrões `<= 3` |
| Dar tempo antes de decidir | 89 | `TimeSpan`, `PausaConsciente` com `TimeProvider` injetado |
| Perceber padrões sem julgar | 89 | `GroupBy` + `ToDictionary`; sem notas nem rankings |
| Três zonas de controle | 90 | `ToLookup(p => p.Controle)`, `FirstOrDefault`, `??` |
| Fato × história | 90 | `Where` + `Select`; tupla `(Fatos, Historias)` |
| Mochila de recursos | 90 | API fluente (`return this`), `Enum.GetValues<T>().Except(...)` |
| Otimismo com evidências | 91 | `string?`, `DateOnly?`, `switch` em tupla `(_, > 0, 0)`, `MaxBy` |
| Passos pequenos, provas pequenas | 92 | `const`, `internal`, `private set`, `IReadOnlyList`, `yield return` |
| Rede de apoio | 93 | `interface IApoio`, `abstract class`, `override`, `HashSet<T>` |
| Várias lentes | 94 | `Lente<T>`, `Func<T, IEnumerable<string>>`, `params`, `SelectMany` |
| Sem sorrir por obrigação | 95 | `Resultado<T>` (`Ok`, `Falha`, `Combinar`), construtor privado |
| Tentar de novo com juízo | 96 | `async/await`, `Task<T>`, `CancellationToken`, *backoff* |
| Plano B e disjuntor | 96 | `ComPlanoBAsync`, `DisjuntorDeCircuito` (fechado, aberto, meio aberto) |
| Tudo junto, sem gravar nada | Lab 10 | `TextReader/TextWriter`, `init`, `is { } s`, `Math.Clamp` |

### Trechos para consultar

```cs
// Falha esperada? TryParse, sem exceção (cap. 88)
if (!int.TryParse(texto, out int horas)) return "Tente algo como 7.";

// Padrão em tupla com relacional (cap. 89)
(Emocao.Raiva, <= 3) => "irritação",

// Relógio injetado: o teste controla o tempo (cap. 89)
public PausaConsciente(TimeProvider relogio, TimeSpan duracao) { ... }

// Primeiro valor não nulo (cap. 90)
var foco = itens.FirstOrDefault(p => p.Controle == Controle.DependeDeMim)
           ?? itens.FirstOrDefault(p => p.Controle == Controle.PossoInfluenciar);

// Tipo anulável e checagem (cap. 91)
public sealed record Evidencia(string Texto, string? Fonte = null, DateOnly? Data = null);

// Iterador: um item de cada vez (cap. 92)
yield return ($"{tarefa} (parte {parte} de {partes})", minutos);

// Programar para o contrato (cap. 93)
foreach (IApoio apoio in mapa.Quem(Necessidade.Estudar)) Console.WriteLine(apoio.ComoPedir(...));

// Genérico com restrição (cap. 94)
public static IReadOnlyList<string> Gerar<T>(T situacao, params Lente<T>[] lentes) where T : notnull

// Resultado em vez de exceção (cap. 95)
texto.Combinar(valor => valor.ToString(), erro => erro);

// Tentar de novo só falhas passageiras (cap. 96)
catch (FalhaPassageiraException falha) { await esperar(politica.EsperaApos(tentativa), ct); }
```

### Quando usar o quê

| Situação | Prefira |
|---|---|
| Entrada de pessoa que pode estar errada | `TryParse` / `Resultado<T>` |
| Algo que "não deveria acontecer" | Exceção (`throw`) |
| Código que depende do relógio | `TimeProvider` injetado |
| Vários tipos que respondem à mesma pergunta | `interface` + polimorfismo |
| Mesmo algoritmo para tipos diferentes | Genéricos (`<T>`) |
| Falha de rede passageira | Retentativa com *backoff* e limite |
| Serviço fora do ar há tempo | Disjuntor de circuito + plano B |
| Valor que pode faltar | Tipo anulável (`?`) e `is not null` |

### Cuidados que valem para todo o livro

- Nenhum modelo **mede**, **compara** ou **rotula** pessoas; todos organizam ideias e perguntas.
- Dados ficam **só na memória**; nada é gravado nem enviado.
- **Pular** é sempre permitido; **silêncio** não é erro.
- Em urgência: **CVV 188** (apoio emocional, 24 h) e **SAMU 192** (emergências).

## Crenças em C#

### Perguntas úteis

| Etapa | Pergunta |
|---|---|
| Origem | Em que contexto aprendi ou ouvi esta ideia? |
| Observação | O que uma pessoa poderia verificar neste episódio? |
| Interpretação | Que explicação estou dando para o que aconteceu? |
| Contraponto | Que observação diferente também existe? |
| Contexto | Que recurso, condição ou barreira influenciou? |
| Próximo passo | Que ação pequena, segura ou apoiada posso escolher? |
| Revisão | O que aprendi e o que continua incerto? |

### Modelos C#

```cs
enum OrigemDaCrenca { Familiar, Social, ExperienciaPessoal, Desconhecida }
enum RelacaoDaEvidencia { Apoia, Contradiz, Contextualiza }
record Hipotese(string Enunciado, OrigemDaCrenca Origem);
record Evidencia(string Observacao, RelacaoDaEvidencia Relacao);
```

Uma síntese pode ser `SemObservacoes`, `SoApoio`, `SoContradicao`, `SoContexto`, `ApoioEContradicao`, `ApoioEContexto`, `ContradicaoEContexto` ou `ApoioContradicaoEContexto`. Os nomes preservam as combinações presentes nos registros; nenhum confirma uma verdade universal ou descreve uma pessoa.

::: cuidado Segurança e limites
Este guia não diagnostica, não substitui terapia e não promete resultados. Evite registrar informações privadas e use exemplos técnicos neutros. Barreiras sociais, econômicas, de saúde e de acesso são reais; pensamento positivo não as remove por si só.
:::

## Você é Aquilo que Ama em C#

### Você é aquilo que ama, em C# (caps. 105–107 e Lab 12)

| Ideia (Smith) | Cap. | Em C#, pense em… |
|---|---|---|
| Desejo antes de conhecimento | 97 | `[Flags] enum Dimensao`, `Aggregate` com o operador "ou", `Decisor` por pesos |
| Bússola (telos) e mapa errado | 98 | `readonly record struct Vetor`, `operator +`, `Cosseno`, `double?` |
| Amar é hábito (imitação e prática) | 99 | `record` + `with`, parâmetro opcional, meia-vida |
| Você pode não amar o que acredita amar | 100 | `GroupJoin`, `ExceptBy`, `Select((x, i) => …)` |
| Liturgias seculares: a forma ensina | 101 | `1 << n`, `PopCount`, `required` + `init`, padrão cuidadoso |
| Reabituar o apetite | 102 | `Func<int,double>`, `Zip`, `TakeWhile`, `Enumerable.Range` |
| O arco da história (reunir, escutar, partilhar, enviar) | 103 | Máquina de estados, `event`, `EventHandler<T>`, `IDisposable` |
| Práticas estruturantes e éthos | 104 | `IComparable<T>`, `SortedSet<T>`, `yield return` sobre `DateOnly` |
| Aprender de cor | 105 | Leitner, `record`+`with`, `Random` injetado |
| Restrições que libertam | 106 | Value objects, `IParsable<T>`, `T.Parse`, `implicit operator` |
| Tudo importa: trabalho como liturgia | 107 | Pipeline `async`, closures, função local, fábrica de etapas |
| Auditoria litúrgica pessoal | Lab 12 | `System.Text.Json`, raw string literal, `is { Count: > 0 } x` |

### Trechos para consultar

```cs
// Combinar, perguntar e tirar bits (cap. 105 e 101)
var coberto = Dimensao.Cabeca | Dimensao.Maos;     // junta
bool temMaos = coberto.HasFlag(Dimensao.Maos);     // pergunta
var falta = Dimensao.Todas & ~coberto;             // o que falta

// Operador sobrecarregado em um struct imutável (cap. 106)
public static Vetor operator +(Vetor a, Vetor b) => new(a.X + b.X, a.Y + b.Y);

// Imutável: "muda" criando outro (cap. 107)
var depois = habito with { Forca = 0.5, UltimaPratica = hoje };

// Junção à esquerda: o que não tem par continua na lista (cap. 108)
prioridades.GroupJoin(porArea, p => p.Area, a => a.Area, (p, g) => new { p.Area, Minutos = g.Sum(a => a.Minutos) });

// Propriedade obrigatória e fixa (cap. 109)
public required string Produto { get; init; }

// Função que recebe e devolve funções (cap. 110)
Func<int, double> ComParceria(Func<int, double> curva, double fator) => dia => curva((int)Math.Round(dia * fator));

// Evento (cap. 111)
public event EventHandler<MovimentoConcluidoEventArgs>? MovimentoConcluido;
MovimentoConcluido?.Invoke(this, new(concluido, registro, Atual));

// Ordem própria (cap. 112)
public int CompareTo(Ocorrencia? outra) => /* dia, depois nome */;

// Intervalos de revisão (cap. 113)
cartao with { Caixa = nova, Proxima = hoje.AddDays(Intervalos[nova - 1]) };

// Genérico com membro estático (cap. 114)
public static T Ler<T>(string texto) where T : IParsable<T> => T.Parse(texto, null);

// Etapa do pipeline (cap. 115)
async (contexto, proximo) => { if (await verificacao(contexto)) await proximo(); else contexto.Interromper(motivo); }
```

### Quando usar o quê

| Situação | Prefira |
|---|---|
| Várias opções ao mesmo tempo | `[Flags] enum` (bits) |
| Valor pequeno, imutável, sem identidade | `readonly record struct` |
| "Mudar" algo que não deve mudar | `record` + `with` |
| Comparar duas listas sem perder itens | `GroupJoin` (à esquerda) |
| Muitos objetos reagindo ao mesmo aviso | `event` |
| Ordenar um tipo seu | `IComparable<T>` + `SortedSet<T>`/`OrderBy` |
| Algo aleatório que precisa ser testado | Injetar o `Random` (ou `TimeProvider`) |
| Entrada que pode estar errada | Objeto de valor + `TryParse`/`IParsable<T>` |
| Passos em ordem que podem parar a esteira | Pipeline de `Etapa`s |
| Ler JSON feito à mão | `JsonSerializerOptions` tolerantes + `try/catch JsonException` |

### Cuidados que valem para todo o livro

- Nenhum modelo **mede, compara ou rotula** pessoas; todos organizam ideias e perguntas.
- As ideias do autor que dependem de **fé** são **dele**; o livro as identifica e **não exige concordância**.
- Dados ficam **só na memória** (ou no seu arquivo local); nada é gravado nem enviado.
- **Pular** é sempre permitido; mensagens **perguntam**, não acusam, e isso **é testado**.
- Se pesar demais, **converse com alguém**: **CVV 188** (24 h, gratuito) e **SAMU 192** (emergências).

## Inteligência Emocional em C#

### A jornada em uma tabela

| Ideia | Capítulo | Em C# |
|---|---:|---|
| Uma emoção não é uma ordem | 116 | `enum` para opções declaradas |
| Separar fato, interpretação e palavra | 117 | `record` e validação |
| Planejar uma pausa possível | 118 | `enum`, `switch` |
| Considerar tempo e ambiente | 119 | `DateTimeOffset`, `record` |
| Perguntar sem adivinhar | 120 | `IReadOnlyList<string>` |
| Descrever observação, impacto e pedido | 121 | classe imutável com validação |
| Definir rotina de equipe sem perfis emocionais | 122 | composição de `record` |
| Rever sem pontuar | 123 | campos de reflexão opcionais |
| Apresentar um exemplo fictício | Lab 13 | console, biblioteca e xUnit |

### Trechos para consultar

```cs
// Uma escolha declarada, não uma inferência.
var opcao = OpcaoDePausa.PedirUmIntervalo;

// Um próximo passo revisto em data e fuso explícitos.
var retomarEm = DateTimeOffset.Parse("2026-10-29T10:00:00-03:00");

// Uma coleção consultável, não um questionário obrigatório.
IReadOnlyList<string> perguntas = PerguntasDeEscuta.Exemplos;

// Mensagem dividida em camadas distintas.
var pedido = new PedidoDeConversa(observacao, impacto, pedidoConcreto);
```

### Quando usar o quê

| Situação | Prefira |
|---|---|
| Um conjunto fixo de opções escolhidas pela pessoa | `enum` (com opções para "não sei" e "prefiro não dizer") |
| Dados pequenos que não mudam depois de criados | `record` com construtor que valida |
| Um valor que pode faltar de propósito | tipo anulável (`string?`), modelado como opção |
| Garantir que um número é uma opção declarada | `Enum.IsDefined` |
| Escolher um texto ou ação conforme uma opção | expressão `switch` |
| Mostrar uma lista que ninguém deve alterar | `IReadOnlyList<T>` |
| Marcar um instante comparável entre fusos | `DateTimeOffset` |
| Mostrar um texto ao usuário sem acoplar ao identificador | função de rótulos separada |
| Testar uma regra e o seu limite | um teste do caso válido e outro do inválido |

### Cuidados que valem para todo o livro

- Nenhum modelo mede, compara ou rotula pessoas; todos organizam opções e textos que alguém escolheu informar.
- As atividades são opcionais; **pular é sempre permitido**.
- Mensagens perguntam, não acusam.
- Se um tema pesar, interrompa e procure alguém de confiança ou apoio profissional. No Brasil: **CVV 188** (24 h, gratuito) e **SAMU 192**.

### Limites do projeto

- Não há reconhecimento de emoção em texto, voz, rosto ou comportamento.
- Não há pontuações, diagnósticos, previsões ou comparações entre pessoas.
- O console demonstra dados fictícios criados no código.
- Não há armazenamento de relatos nem envio pela rede.
- Validar campos garante o contrato técnico, não a verdade de uma experiência.
- Atividades pessoais são opcionais; exemplos neutros são suficientes.

## O Evangelho segundo João em C#

### O Evangelho de João em C# (caps. 124–133 e Lab 14)

| Cena | Cap. | Lupa no grego (contagens no texto de João) | Em C#, pense em… |
|---|---|---|---|
| No princípio era a Palavra | 124 | *phōs* 23, *skotia* 8, *logos* 40; *ēn* (1,1) × *egeneto* (1,3.14) | `: Deus` (herança), `EstavaCom` (referência), `is`, `ReferenceEquals`; *logging* com `AsyncLocal`, `IDisposable`, objeto nulo |
| "Eu sou" | 125 | *egō eimi* 24 (13 com predicado), *artos* 24 (21 no cap. 6) | `Attribute`, `GetCustomAttribute<T>`, `Activator.CreateInstance`, `IComparable<T>` |
| Da água ao vinho | 126 | *sēmeion* 17 (só 2 numerados: 2,11 e 4,54), *hydria* 3 | `Transformacao<TEntrada,TSaida>`, `Func`, `yield return`, `with` |
| O vento | 127 | *anōthen* 5, *pneuma* 24, *pneō* 2, *phōnē* 15 | `IObservable<T>`, `IObserver<T>`, `IDisposable`, operadores |
| O poço de Jacó | 128 | *hydōr* 21 (9 no cap. 4), *phrear* 2, *pēgē* 3 | `ConcurrentDictionary`, `Lazy<T>`, `TimeProvider`, `Interlocked` |
| Pães e peixes | 129 | *paidarion* 1, *klasma* 2, *apollymi* 10 | `Channel<T>`, `IAsyncEnumerable<T>`, `await foreach`, `Chunk` |
| A porta e a voz | 130 | *phōnē* 15 (5 em Jo 10), *poimēn* 6, *thyra* 7 | `HMACSHA256`, base64url, `FixedTimeEquals`, `Func<Identidade,bool>` |
| Lázaro | 131 | *edakrysen* 1, *lyō* 6, *aphiēmi* 15, *Lazaros* 11 | `abstract record`, `switch` com `when`, `Aggregate`, `TakeWhile` |
| O lava-pés | 132 | *lention* 2, *niptēr* 1, *hypodeigma* 1, *niptō* 13 | Decorator, métodos de extensão, `throw;`, `catch … when` |
| A videira | 133 | *menō* 40 (11 no cap. 15), *klēma* 4, *karpos* 10 | Injeção de dependência, reflexão, ciclos de vida, `Dispose` em ordem inversa |
| Concordância e roteiro | Lab 14 | 15.438 palavras; 66 lemas acompanhados | `System.Text.Json`, LINQ, `MaxBy`, BFS, Dijkstra, `PriorityQueue` |

### Trechos para consultar

```cs
// Escopo que fecha sozinho (cap. 124)
using (lanterna.Escopo("entrega"))
    lanterna.Escrever(Nivel.Aviso, "revisão pendente", ("dias", 2));

// Descobrir tipos etiquetados (cap. 125)
assembly.GetTypes().Where(t => t.GetCustomAttribute<EuSouAttribute>() is not null);

// Execução adiada: só trabalha quando se pede (cap. 126)
IEnumerable<Taca> Servir(int n) { for (int i = 1; i <= n; i++) yield return Preparar(i); }

// Ouvir um fluxo (cap. 127)
vento.Onde(n => n % 2 == 0).Transformar(n => $"par {n}").Ouvir(Console.WriteLine);

// Cache: uma ida à fonte por chave (cap. 128)
agua.AddOrUpdate(chave, nova, (_, atual) => agora - atual.CriadaEm < validade ? atual : nova);

// Canal limitado: o produtor espera quando está cheio (cap. 129)
Channel.CreateBounded<Porcao>(new BoundedChannelOptions(3) { FullMode = BoundedChannelFullMode.Wait });

// Assinar e comparar em tempo constante (cap. 130)
CryptographicOperations.FixedTimeEquals(HMACSHA256.HashData(segredo, dados), enviada);

// Estado = soma dos eventos (cap. 131)
eventos.Aggregate(Documento.Vazio, (documento, evento) => documento.Aplicar(evento));

// Empilhar decoradores (cap. 132)
IRepositorioDeTarefas r = chao.ComCache().ComTentativas(3).ComRegistro(lanterna);

// Registrar e obter (cap. 133)
using var videira = new Recipiente().Registrar<ISeiva, SeivaDaTerra>(Ciclo.Permanente).Registrar<IRamo, Ramo>();

// Densidade por mil palavras (Lab 14)
lema.Em(capitulo) * 1000.0 / palavrasDoCapitulo;
```

### Quando usar o quê

| Situação | Prefira |
|---|---|
| Entender o que o sistema fez | Registros estruturados com níveis e escopos |
| Descobrir tipos em execução (catálogos, plugins) | Atributos + reflexão, nas bordas |
| Resultado que talvez ninguém use | Execução adiada (`yield`) |
| Reutilizar um resultado várias vezes | `ToList()` (materializar) |
| Muitos interessados no mesmo aviso | `IObservable<T>`, não acoplar o emissor |
| Dado caro de buscar e que pode envelhecer | Cache com validade e `Invalidar` |
| Produtor mais rápido que consumidor | `Channel<T>` limitado (contrapressão) |
| Provar que um dado não foi alterado | HMAC + comparação em tempo constante |
| Auditoria e "como chegou aqui?" | Event sourcing |
| Acrescentar cuidado sem mexer no núcleo | Decorator |
| Classes que se montam sozinhas | Injeção de dependência |
| Menor número de passos / menor custo | BFS / Dijkstra |

### Cuidados que valem para todo o livro

- Os **números do grego** vêm do SBLGNT/MorphGNT e estão **fixados em testes** (`dotnet test`).
- Onde o texto **não diz** algo (os "sete sinais", o momento da mudança em Caná), o livro **não inventa**: avisa e marca como escolha nossa.
- As **analogias são ferramentas de ensino**, e não afirmações doutrinárias: cada capítulo tem um **Limite da analogia** que diz onde a comparação deixa de valer; nenhum programa mede ou avalia pessoas ou crenças.
- Código de **segurança** (cap. 130) é para estudo: em produção, use bibliotecas consolidadas.
- Os **dias de viagem** do mapa (Lab 14) são ilustrativos.
- Se algum tema pesar, **converse com alguém**: **CVV 188** (24 h, gratuito) e **SAMU 192** (emergências).

## Atenção Plena em Angular

### Terminal

```bash
npx @angular/cli@22 new meu-app     # criar
npm start                           # servir em http://localhost:4200
npm run test:ci                     # testar uma vez (Vitest)
npm run build                       # build de produção
npx ng generate component nome      # gerar um componente
npx ng update @angular/core @angular/cli   # atualizar com migrações
```

### Componente

```ts
@Component({
  selector: 'app-exemplo',
  imports: [OutroComponente, RouterLink],
  template: `<h2>{{ titulo() }}</h2>`,
  styles: `h2 { color: var(--primaria); }`,
})
export class Exemplo {
  readonly item = input.required<Item>();     // entrada obrigatória
  readonly tamanho = input(10);               // entrada com padrão
  readonly aberto = model(false);             // duas vias: [(aberto)]
  readonly escolheu = output<string>();       // evento: (escolheu)="..."
  protected readonly titulo = computed(() => this.item().nome);
  private readonly servico = inject(MeuServico);
}
```

### Template

| Sintaxe | Uso |
|---|---|
| `{{ valor() }}` | Interpolação |
| `[disabled]="cond()"` | Propriedade |
| `[attr.aria-label]="texto"` | Atributo |
| `[class.ativo]="cond()"` / `[style.width.px]="n"` | Classe / estilo |
| `(click)="fazer($event)"` / `(keyup.enter)="..."` | Evento |
| `[(aberto)]="sinal"` | Duas vias (com `model`) |
| `#ref` | Variável de template |
| `@if (x(); as v) { } @else { }` | Condição |
| `@for (i of lista(); track i.id) { } @empty { }` | Repetição |
| `@switch (x) { @case (1) { } @default { } }` | Escolha |
| `@let total = soma();` | Variável no template |
| `@defer (on viewport) { } @placeholder { }` | Carregar depois |
| `{{ n \| minutos }}` | Pipe |

### Signals

| API | Para quê |
|---|---|
| `signal(v)` · `.set(v)` · `.update(f)` | Estado gravável |
| `computed(() => ...)` | Valor derivado (preguiçoso, com memória, função pura) |
| `linkedSignal({ source, computation })` | Valor ajustável que se reacomoda quando a fonte muda |
| `effect(() => ...)` | Efeito fora da tela (log, armazenamento), raramente |
| `resource({ params, loader })` | Assíncrono derivado de signals |
| `toSignal(obs$)` · `toObservable(sig)` | Pontes com RxJS |

### Rotas

```ts
export const routes: Routes = [
  { path: '', pathMatch: 'full', redirectTo: 'inicio' },
  { path: 'item/:id', title: 'Item', canActivate: [guarda], loadComponent: () => import('./item').then((m) => m.Item) },
  { path: 'area', loadChildren: () => import('./area.routes').then((m) => m.ROTAS) },
  { path: '**', loadComponent: () => import('./nao-encontrada').then((m) => m.NaoEncontrada) },
];
// app.config.ts
provideRouter(routes, withComponentInputBinding());
```

### Serviços e DI

```ts
@Injectable({ providedIn: 'root' })
export class MeuServico { private readonly http = inject(HttpClient); }

export const CONFIG = new InjectionToken<Config>('CONFIG', { providedIn: 'root', factory: () => ({ ... }) });
```

### HTTP

```ts
provideHttpClient(withFetch(), withInterceptors([meuInterceptor]));

readonly dados = httpResource<Item[]>(() => `api/itens?q=${this.q()}`, { defaultValue: [], parse: conferir });
// dados.value(), dados.isLoading(), dados.error(), dados.status(), dados.reload()

export const meuInterceptor: HttpInterceptorFn = (req, next) => next(req).pipe(...);
```

### Signal Forms

```ts
readonly modelo = signal({ nome: '', idade: 0 });
readonly f = form(this.modelo, (c) => {
  required(c.nome, { message: 'Diga seu nome.' });
  min(c.idade, 0, { message: 'Idade não pode ser negativa.' });
});
// template: <input [formField]="f.nome" />  ·  f.nome().errors(), .touched(), .invalid()
// enviar:   await submit(this.f, async () => { ... });  ·  this.f().reset(valor)
```

### Testes (Vitest)

```ts
describe('Coisa', () => {
  it('faz algo', async () => {
    TestBed.configureTestingModule({ providers: [{ provide: RELOGIO, useValue: () => agora }] });
    const fixture = TestBed.createComponent(Coisa);
    fixture.componentRef.setInput('item', item);
    await fixture.whenStable();
    (fixture.nativeElement as HTMLElement).querySelector('button')!.click();
    await fixture.whenStable();
    expect(fixture.nativeElement.textContent).toContain('feito');
  });
});
```

| Precisa de... | Use |
|---|---|
| Tabela de casos | `it.each([...])` |
| Espião | `vi.fn()` / `vi.spyOn(obj, 'metodo')` |
| Tempo falso | `vi.useFakeTimers({ toFake: ['setInterval', 'clearInterval'] })` |
| HTTP falso | `provideHttpClientTesting()` + `HttpTestingController` |
| Rotas | `provideRouter(...)` + `RouterTestingHarness` |
| `@defer` | `deferBlockBehavior: DeferBlockBehavior.Manual` + `getDeferBlocks()` |
| Tempo no RxJS | `TestScheduler` e diagramas de bolinhas |
