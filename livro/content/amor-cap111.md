---
id: amor-cap111
order: 143
kind: capitulo
num: 111
group: C# & Mentalidade · Parte 17 · Você é aquilo que ama, em C#
title: Em que história você está inserido?
subtitle: A imaginação se forma por histórias, ritmos e imagens, mais do que por argumentos. Em C#, é o capítulo da máquina de estados e dos eventos: um ritual de reunião com começo, meio e fim, que avisa quem se inscreveu.
minutes: 70
hue: 285
objectives:
- Entender por que histórias e imagens **formam a imaginação** mais que listas de argumentos ("mostre, não conte")
- Ver o **arco de quatro movimentos** (reunir, escutar, partilhar, enviar) como um desenho de encontro que conta uma história
- Modelar uma **máquina de estados** com `enum` e tabela de transições
- Usar **eventos** (`event`, `EventHandler<T>`, `EventArgs`) para que objetos se avisem sem se conhecer
origem: livros/08-voce-e-aquilo-que-ama-csharp/content/131-cap111.md
---

::: vila A reunião que nunca terminava
A reunião semanal do time havia virado uma maratona: cada um falava de sua tarefa, alguém desviava para um problema técnico, e acabava sem ninguém saber **o que fora decidido** nem **quem faria o quê**. Marta propôs um formato novo, em quatro momentos curtos: **chegar** (cada um diz como está), **escutar** (dados, o que não vai bem), **decidir juntos e agradecer**, e **sair com combinados**. Na primeira vez, durou 35 minutos, e todo mundo saiu sabendo **o que fazer na segunda**.
:::

::: ilustracao img/amor/cap111.svg | Um encontro que tem começo, meio e fim conta uma história. Os quatro movimentos dão forma: reunir, escutar, partilhar, enviar.
:::

## O que o livro diz

Smith defende que a **imaginação** (o modo como enxergamos o mundo antes de pensar sobre ele) é uma faculdade **estética**: é movida mais por **histórias, poesia, metáforas e imagens** do que por argumentos. Ele chama nosso coração de "instrumento de cordas, dedilhado por histórias". Para ilustrar, cita o quadro *A infância de Raleigh*, de Millais: um garoto e um amigo ouvem, hipnotizados, um velho marinheiro apontar para o oceano e contar o que há do outro lado. A interpretação sugerida é que **aquela história fez nascer um anseio** que dirigiria a vida inteira do menino.

Daí vem uma regra que ele toma dos romancistas, **"mostre, não conte"**: um crítico literário que ele cita observa que a ficção **não nos pede para crer em coisas**, e sim para **imaginá-las**. Persuadir pela forma (e não só pela lista de razões) é o que o autor chama de uma persuasão "mimética".

A partir disso, ele argumenta que o culto cristão tradicional tem uma **forma**, um **arco narrativo** em quatro movimentos, que **encena** uma história: **reunião** (somos chamados), **escuta** (inclui a confissão e a escuta de um texto), **mesa** (comunhão) e **envio** (somos mandados de volta ao mundo). Cada movimento ensina algo pela **repetição**. E, para ele, **a forma importa**: seria um erro trocar esse arco por formatos que "contam" outra história sem querer.

::: cuidado Do culto ao time
Para o autor, esse arco é **religioso** e tem um conteúdo próprio; não vamos reproduzi-lo nem avaliá-lo. O que adaptamos é o **princípio estrutural**: encontros que se repetem **têm um enredo**, e esse enredo **forma** quem passa por ele. A inspiração para uma reunião de time vem do **desenho** (reunir, escutar, partilhar, enviar), não da liturgia em si.
:::

::: analogia A mesa de jantar
Uma refeição em família tem um arco sem que ninguém o planeje: a mesa é posta (reunir), contam como foi o dia (escutar), dividem a comida e as conversas (partilhar), alguém lava a louça, e cada um vai para seus compromissos (enviar). Se o jantar perde o arco (todo mundo come em frente à tela), perde também aquilo que ele formava.
:::

## Da vida para o código

| No livro | Em C# |
|---|---|
| Quatro movimentos em ordem | **`enum Movimento`** e uma **tabela de transições** (`Dictionary`) |
| Cada movimento tem uma pergunta própria | `PerguntaDe(movimento)` com `switch` |
| Não dá para pular o que forma | `Concluir` exige um **registro**; o último passo (enviar) é obrigatório |
| "Quem se interessa em saber" | **`event EventHandler<MovimentoConcluidoEventArgs>`** |
| Alguém que escuta o ritual sem ser conhecido por ele | `Ata` se **inscreve** com `+=` |
| Sair do ritual com compromissos | `Completo` só vale ao final do "enviar" |

::: figura Um arco de quatro movimentos
<svg viewBox="0 0 740 220" role="img" aria-label="Quatro movimentos em sequência: reunir, escutar, partilhar, enviar, cada um com uma pergunta guia, e uma seta de retorno à vida cotidiana">
  <g font-size="13" text-anchor="middle" font-weight="800">
    <rect x="20" y="46" width="160" height="70" rx="14" class="f-info"/><text x="100" y="76">1 · Reunir</text><text x="100" y="98" font-size="11" font-weight="400" class="t-dim">quem está aqui?</text>
    <rect x="200" y="46" width="160" height="70" rx="14" class="f-warn"/><text x="280" y="76">2 · Escutar</text><text x="280" y="98" font-size="11" font-weight="400" class="t-dim">o que precisamos ouvir?</text>
    <rect x="380" y="46" width="160" height="70" rx="14" class="f-good"/><text x="460" y="76">3 · Partilhar</text><text x="460" y="98" font-size="11" font-weight="400" class="t-dim">decidir e agradecer</text>
    <rect x="560" y="46" width="160" height="70" rx="14" class="f-prim"/><text x="640" y="76">4 · Enviar</text><text x="640" y="98" font-size="11" font-weight="400" class="t-dim">quem faz o quê, até quando?</text>
  </g>
  <path class="arrow" d="M182 81H198"/><path class="arrow" d="M362 81H378"/><path class="arrow" d="M542 81H558"/>
  <path class="arrow" d="M640 118C640 190 100 190 100 120"/><text x="370" y="176" text-anchor="middle" font-size="12" class="t-dim">a semana volta a pedir o próximo encontro: o ritmo é parte da forma</text>
</svg>
:::

## 1) Uma máquina de estados

Uma **máquina de estados** é um modelo em que algo está **sempre em um estado** e só pode **passar** para certos estados, por regras claras. Aqui, os estados são os movimentos, e a regra é: `Reunir → Escutar → Partilhar → Enviar → Encerrado`, sem pular.

A forma mais legível de escrevê-la em C# é uma **tabela de transições**: um `Dictionary` que diz "de cada estado, para onde se vai". Toda a lógica de "o que vem depois" fica **em um só lugar**, à vista, e é fácil de ler e de testar.

```cs Jornada/Habitos/Cap111/Ritual.cs
namespace Jornada.Habitos.Cap111;

/// <summary>Os quatro "capítulos" de um encontro que forma: reunir, escutar, partilhar e enviar.</summary>
public enum Movimento { Reunir, Escutar, Partilhar, Enviar, Encerrado }

/// <summary>Os dados que acompanham o evento: o que foi concluído, o que foi registrado e qual é o próximo.</summary>
public sealed class MovimentoConcluidoEventArgs : EventArgs
{
    public MovimentoConcluidoEventArgs(Movimento concluido, string registro, Movimento proximo)
    {
        Concluido = concluido;
        Registro = registro;
        Proximo = proximo;
    }

    public Movimento Concluido { get; }
    public string Registro { get; }
    public Movimento Proximo { get; }
}

public sealed class RitualDeReuniao
{
    // A tabela de transições: de onde se pode ir. O fluxo é dado, não improvisado.
    private static readonly IReadOnlyDictionary<Movimento, Movimento> Seguinte = new Dictionary<Movimento, Movimento>
    {
        [Movimento.Reunir]    = Movimento.Escutar,
        [Movimento.Escutar]   = Movimento.Partilhar,
        [Movimento.Partilhar] = Movimento.Enviar,
        [Movimento.Enviar]    = Movimento.Encerrado,
    };

    public static string PerguntaDe(Movimento movimento) => movimento switch
    {
        Movimento.Reunir    => "Quem está aqui e o que cada pessoa trouxe consigo hoje?",
        Movimento.Escutar   => "O que precisamos ouvir: dados, feedback, o que não vai bem?",
        Movimento.Partilhar => "O que decidimos juntos? O que merece ser agradecido?",
        Movimento.Enviar    => "Quem faz o quê, até quando?",
        _                   => "Encontro encerrado."
    };

    private readonly List<(Movimento Movimento, string Registro)> registros = new();

    public Movimento Atual { get; private set; } = Movimento.Reunir;

    public IReadOnlyList<(Movimento Movimento, string Registro)> Registros => registros;

    /// <summary>Um "evento" do C#: quem quiser ser avisado se inscreve com +=, sem que o ritual conheça os inscritos.</summary>
    public event EventHandler<MovimentoConcluidoEventArgs>? MovimentoConcluido;

    public void Concluir(string registro)
    {
        if (Atual == Movimento.Encerrado)
            throw new InvalidOperationException("O encontro já foi encerrado.");

        if (string.IsNullOrWhiteSpace(registro))
            throw new ArgumentException($"Registre algo antes de concluir: {PerguntaDe(Atual)}", nameof(registro));

        var concluido = Atual;
        Atual = Seguinte[concluido];
        registros.Add((concluido, registro.Trim()));

        MovimentoConcluido?.Invoke(this, new MovimentoConcluidoEventArgs(concluido, registro.Trim(), Atual));
    }

    /// <summary>Um encontro sem "enviar" não mudou nada: terminou sem compromissos. Só é completo se passou por todos.</summary>
    public bool Completo => Atual == Movimento.Encerrado;
}
```

Três recursos novos merecem atenção:

- **`event EventHandler<MovimentoConcluidoEventArgs>? MovimentoConcluido`**: um **evento** é um "aviso público": **quem quiser** se inscreve (com `+=`) para ser chamado quando algo acontecer, sem que o ritual precise **saber quem são** os inscritos. O `EventHandler<T>` é o delegate padrão: recebe quem avisou (`object? origem`) e os dados (`T`).
- **`MovimentoConcluidoEventArgs : EventArgs`**: o pacote de dados do aviso (o que foi concluído, o que foi registrado, qual é o próximo).
- **`MovimentoConcluido?.Invoke(this, ...)`**: o `?.` é essencial. Se **ninguém** se inscreveu, o evento é `null`, e chamar `Invoke` sem o `?.` quebraria. Com ele, nada acontece.

Note também a regra de cuidado em `Concluir`: **não aceita registro vazio**, e a mensagem de erro **repete a pergunta guia** do movimento. Assim, o próprio código **ensina o formato**.

## 2) Quem escuta o ritual

A `Ata` é um **ouvinte**: se inscreve no evento (`ritual.MovimentoConcluido += AoConcluir`) e monta as linhas da ata, sem que o ritual saiba que ela existe.

```cs Jornada/Habitos/Cap111/Ata.cs
namespace Jornada.Habitos.Cap111;

/// <summary>Um "ouvinte" do ritual: se inscreve no evento e monta a ata sem que o ritual saiba que ela existe.</summary>
public sealed class Ata
{
    private readonly List<string> linhas = new();

    public Ata(RitualDeReuniao ritual) => ritual.MovimentoConcluido += AoConcluir;

    public IReadOnlyList<string> Linhas => linhas;

    private void AoConcluir(object? origem, MovimentoConcluidoEventArgs e) =>
        linhas.Add($"{linhas.Count + 1}. {e.Concluido}: {e.Registro}");
}
```

Isso é o que chamamos de **baixo acoplamento**: o ritual **publica**; a ata **assina**. Amanhã, alguém pode adicionar um ouvinte que **manda um resumo por e-mail**, ou um que **conta quantos encontros terminaram sem "enviar"**, sem mexer **uma linha** no ritual.

```cs Uso
var ritual = new RitualDeReuniao();
var ata = new Ata(ritual);
ritual.MovimentoConcluido += (_, e) => Console.WriteLine($"→ {e.Concluido} concluído; agora: {e.Proximo}");

Console.WriteLine(RitualDeReuniao.PerguntaDe(ritual.Atual));
ritual.Concluir("Todos presentes; a Bia trouxe um bloqueio de acesso");
ritual.Concluir("O erro de pagamento subiu 5% depois da última versão");
ritual.Concluir("Pausar a funcionalidade nova; agradecer ao Caio pela correção rápida");
ritual.Concluir("Lia investiga o erro de pagamento até quinta");

Console.WriteLine($"Completo? {ritual.Completo}");
foreach (var linha in ata.Linhas)
    Console.WriteLine(linha);
```

```saida
Quem está aqui e o que cada pessoa trouxe consigo hoje?
→ Reunir concluído; agora: Escutar
→ Escutar concluído; agora: Partilhar
→ Partilhar concluído; agora: Enviar
→ Enviar concluído; agora: Encerrado
Completo? True
1. Reunir: Todos presentes; a Bia trouxe um bloqueio de acesso
2. Escutar: O erro de pagamento subiu 5% depois da última versão
3. Partilhar: Pausar a funcionalidade nova; agradecer ao Caio pela correção rápida
4. Enviar: Lia investiga o erro de pagamento até quinta
```

O ritual percorreu os quatro movimentos; cada conclusão foi **avisada** a quem estava ouvindo (o `Console.WriteLine` que inscrevemos com uma lambda) e a **ata** foi montada sozinha. O quarto movimento, **enviar**, registrou **quem faz o quê até quando**: sem ele, o encontro **não** estaria completo.

::: dica Cuidado com "inscrições esquecidas"
Quem se inscreve com `+=` **mantém uma referência** com o publicador. Se o ouvinte deveria "sair de cena" mas continua inscrito, ele **não é coletado** pela memória e continua recebendo avisos. Por isso, ouvintes de vida curta devem **se desinscrever** (`-=`), um bom uso para `IDisposable` (veja o exercício).
:::

## 3) Testes

```cs Habitos.Testes/Cap111Tests.cs
using Jornada.Habitos.Cap111;

namespace Habitos.Testes;

public class Cap111Tests
{
    private static void ConcluirTudo(RitualDeReuniao r)
    {
        r.Concluir("Todos presentes; a Bia trouxe um bloqueio");
        r.Concluir("O erro de pagamento subiu 5%");
        r.Concluir("Decidimos pausar a funcionalidade nova; agradecer ao Caio");
        r.Concluir("Lia investiga o erro até quinta");
    }

    [Fact]
    public void O_ritual_percorre_os_quatro_movimentos_em_ordem_e_termina()
    {
        var r = new RitualDeReuniao();
        Assert.Equal(Movimento.Reunir, r.Atual);

        r.Concluir("a");
        Assert.Equal(Movimento.Escutar, r.Atual);
        r.Concluir("b");
        Assert.Equal(Movimento.Partilhar, r.Atual);
        r.Concluir("c");
        Assert.Equal(Movimento.Enviar, r.Atual);
        Assert.False(r.Completo);
        r.Concluir("d");

        Assert.Equal(Movimento.Encerrado, r.Atual);
        Assert.True(r.Completo);
    }

    [Fact]
    public void Nao_da_para_pular_um_movimento_sem_registrar_nada()
    {
        var r = new RitualDeReuniao();
        var e = Assert.Throws<ArgumentException>(() => r.Concluir("   "));

        Assert.Contains("Quem está aqui", e.Message);
        Assert.Equal(Movimento.Reunir, r.Atual);
    }

    [Fact]
    public void Depois_de_encerrado_nao_aceita_mais_registros()
    {
        var r = new RitualDeReuniao();
        ConcluirTudo(r);
        Assert.Throws<InvalidOperationException>(() => r.Concluir("mais uma coisa"));
    }

    [Fact]
    public void O_evento_avisa_quem_se_inscreveu_a_cada_movimento()
    {
        var r = new RitualDeReuniao();
        var avisos = new List<(Movimento, Movimento)>();
        r.MovimentoConcluido += (origem, e) => avisos.Add((e.Concluido, e.Proximo));

        r.Concluir("a");
        r.Concluir("b");

        Assert.Equal(new[] { (Movimento.Reunir, Movimento.Escutar), (Movimento.Escutar, Movimento.Partilhar) }, avisos);
    }

    [Fact]
    public void Varios_ouvintes_recebem_o_mesmo_aviso_e_o_ritual_nao_conhece_nenhum()
    {
        var r = new RitualDeReuniao();
        var ata = new Ata(r);
        int contagem = 0;
        r.MovimentoConcluido += (_, _) => contagem++;

        ConcluirTudo(r);

        Assert.Equal(4, contagem);
        Assert.Equal(4, ata.Linhas.Count);
        Assert.StartsWith("1. Reunir:", ata.Linhas[0]);
        Assert.StartsWith("4. Enviar: Lia investiga", ata.Linhas[3]);
    }

    [Fact]
    public void Funciona_sem_nenhum_inscrito()
    {
        var r = new RitualDeReuniao();
        r.Concluir("a");                       // MovimentoConcluido é null: o "?." evita o erro
        Assert.Single(r.Registros);
    }

    [Theory]
    [InlineData(Movimento.Reunir)]
    [InlineData(Movimento.Escutar)]
    [InlineData(Movimento.Partilhar)]
    [InlineData(Movimento.Enviar)]
    public void Cada_movimento_tem_a_sua_pergunta(Movimento m) =>
        Assert.EndsWith("?", RitualDeReuniao.PerguntaDe(m));
}
```

O teste **`Varios_ouvintes_recebem_o_mesmo_aviso_e_o_ritual_nao_conhece_nenhum`** é a razão de ser dos eventos. E **`Funciona_sem_nenhum_inscrito`** protege o `?.`: um evento sem inscritos **não pode quebrar** o ritual.

::: exercicio Quem se inscreve também sai
Crie a classe `ContadorDeMovimentos`, que **conta** quantos movimentos foram concluídos em um `RitualDeReuniao` e implementa **`IDisposable`**: ao ser descartada (`Dispose`), **se desinscreve** do evento (`-=`) e **para de contar**. Teste: depois do `Dispose`, novos movimentos **não** aumentam o contador.

--- resposta
```cs Jornada/Habitos/Exercicios/Cap111Observador.cs
namespace Jornada.Habitos.Cap111;

/// <summary>
/// Conta quantos movimentos já foram concluídos. Quem se inscreve com += precisa poder sair com -=:
/// por isso implementa IDisposable e se "desinscreve" ao ser descartado.
/// </summary>
public sealed class ContadorDeMovimentos : IDisposable
{
    private readonly RitualDeReuniao ritual;

    public ContadorDeMovimentos(RitualDeReuniao ritual)
    {
        this.ritual = ritual;
        ritual.MovimentoConcluido += AoConcluir;
    }

    public int Total { get; private set; }

    private void AoConcluir(object? origem, MovimentoConcluidoEventArgs e) => Total++;

    public void Dispose() => ritual.MovimentoConcluido -= AoConcluir;
}
```

O **mesmo método** (`AoConcluir`) é usado em `+=` e em `-=`: para o C# remover uma inscrição, precisa receber o **mesmo delegate**. Por isso o método é nomeado, e não uma lambda anônima. Num uso real, o padrão `using var contador = new ContadorDeMovimentos(ritual);` garante a desinscrição ao fim do bloco.
:::

::: resumo
- Para o autor, a **imaginação** é formada por **histórias, ritmos e imagens**, e o seu "mostre, não conte" vale para a forma de um encontro: o **enredo** conta mais do que a lista de pontos.
- Um encontro recorrente com **arco** (reunir, escutar, partilhar, enviar) forma quem participa; sem **"enviar"**, ele acaba sem consequências.
- Uma **máquina de estados** com **tabela de transições** deixa visível o fluxo permitido; `enum` + `Dictionary` bastam.
- **Eventos** (`event`, `EventHandler<T>`, `EventArgs`) permitem que objetos se avisem **sem se conhecer**; o `?.Invoke` protege do caso sem inscritos.
- Quem se inscreve com `+=` deve poder sair com `-=`: **`IDisposable`** ajuda a não deixar inscrições esquecidas.
:::

::: quiz
? O que o "mostre, não conte" propõe, no capítulo?
+ Que a forma de um encontro ou de uma história persuade por imaginação, não só por argumentos
- Que não se deve explicar nada
- Que imagens substituem tudo
- Que só filmes ensinam
> A imaginação se forma por histórias e imagens.

? Quais são os quatro movimentos do arco adaptado?
+ Reunir, escutar, partilhar e enviar
- Planejar, executar, medir e corrigir
- Abrir, discutir, votar e fechar
- Começar, esperar, decidir e esquecer
> Cada um tem uma pergunta guia que conduz o encontro.

? O que é uma tabela de transições?
+ Um mapa que diz, para cada estado, qual é o próximo
- Uma tabela de preços
- Uma lista de erros
- Um relatório de reuniões
> Ela concentra a lógica do fluxo em um só lugar.

? Para que serve o `?.Invoke(...)` ao disparar um evento?
+ Evitar um erro quando ninguém está inscrito (o evento é nulo)
- Acelerar o evento
- Criar novos inscritos
- Converter o evento em texto
> Sem inscritos, o evento é `null`.

? O que significa "baixo acoplamento" no exemplo da `Ata`?
+ O ritual não conhece quem o escuta, e novos ouvintes entram sem mudar o ritual
- A ata é independente do computador
- O ritual não usa classes
- A ata não tem dados
> Publicar e assinar separam as responsabilidades.

? Por que o ouvinte deveria se desinscrever com `-=`?
+ Para não ficar preso ao publicador e continuar recebendo avisos sem necessidade
- Porque `-=` é mais rápido
- Porque o evento seria apagado
- Porque é uma regra do Visual Studio
> Inscrições esquecidas mantêm objetos vivos e ativos.
:::

::: cartas
Imaginação (para o autor) :: Modo de enxergar o mundo antes de pensar; formada por histórias, metáforas e imagens.
Mostre, não conte :: Regra do romance: persuadir fazendo imaginar, e não só afirmando.
Arco narrativo :: Sequência com começo, meio e fim que dá forma a um encontro recorrente.
Máquina de estados :: Modelo em que algo está sempre em um estado, com transições permitidas.
Tabela de transições :: `Dictionary` que mapeia cada estado ao próximo.
event :: Membro que permite a objetos inscreverem-se para serem avisados.
EventHandler<T> :: Delegate padrão de evento: recebe a origem e os dados do aviso.
Baixo acoplamento :: Quem publica não conhece quem escuta.
:::
