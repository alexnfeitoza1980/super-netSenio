---
id: aprenda-cap13
order: 25
kind: capitulo
num: 13
group: C# · Parte 6 · Gráficos, memória e LINQ
title: Controles e gráficos
subtitle: Deixe bonito — controles são objetos, `UserControl`, animação, `Bitmap`, `Graphics` (GDI+), o evento `Paint`, buffer duplo e impressão.
minutes: 70
hue: 180
objectives:
- Perceber que **controles de formulário são apenas objetos** que você pode criar em código
- Criar um **UserControl** e animar uma imagem
- Desenhar com **`Graphics`** (GDI+) e entender o evento **`Paint`**
- Eliminar tremulação com **buffer duplo** e imprimir com `PrintDocument`
origem: livros/01-aprenda-csharp/content/15-cap13.md
---

::: ilustracao img/aprenda/cap13.svg | Controles e gráficos: você pinta no Graphics, no tempo e no lugar que escolher.
:::

## Às vezes você precisa fazer os gráficos você mesmo

Até agora você deixou os **controles** cuidarem de tudo que é visual. Mas quando você precisa **animar** uma imagem, criar seu próprio componente ou desenhar direto no formulário, os controles prontos não bastam. Tudo começa pelo objeto **`Graphics`**, pelos **`Bitmap`s** e por uma determinação de não aceitar os gráficos como eles vêm.

## Controles são apenas objetos

Quando você arrasta um botão para o formulário, a IDE escreve `new Button()` para você. Nada impede **você** de fazer o mesmo, em tempo de execução:

```cs
Button b = new Button();
b.Text = "Criado em código!";
b.Location = new Point(20, 20);
b.Size = new Size(160, 36);
b.Click += (s, e) => MessageBox.Show("Funciona!");
this.Controls.Add(b);          // sem isto, o botão existe mas NÃO aparece
```

::: regra Controle = objeto + `Controls.Add`
Um controle de formulário é uma **instância** de uma classe (`Button`, `Label`, `PictureBox`…) com propriedades e eventos como qualquer outra. Para aparecer, precisa ser **adicionado** à coleção `Controls` de um contêiner (formulário, painel).
:::

### Controles de formulário e a estrutura do renderizador

No simulador de colmeia, queremos **ver** as abelhas e flores. Ideia: um **renderizador** (*renderer*) que, a cada quadro, cria/atualiza **um controle por objeto**:

```cs
class Renderer
{
    private World world;
    private HiveForm hiveForm;
    private FieldForm fieldForm;
    private Dictionary<Flower, PictureBox> flowerLookup = new Dictionary<Flower, PictureBox>();
    private List<Flower> deadFlowers = new List<Flower>();

    public void Render()
    {
        DrawFlowers();
        fieldForm.Invalidate();      // pede para o formulário se redesenhar
    }

    private void DrawFlowers()
    {
        foreach (Flower flower in world.Flowers)
        {
            if (!flowerLookup.ContainsKey(flower))
            {
                PictureBox box = new PictureBox() { Image = Properties.Resources.Flower, SizeMode = PictureBoxSizeMode.StretchImage, Size = new Size(40, 40) };
                flowerLookup.Add(flower, box);
                fieldForm.Controls.Add(box);          // cria o controle na hora
            }
            flowerLookup[flower].Location = flower.Location;
        }
        // remove os controles de flores que morreram
        foreach (Flower f in new List<Flower>(flowerLookup.Keys))
            if (!world.Flowers.Contains(f))
            {
                fieldForm.Controls.Remove(flowerLookup[f]);
                flowerLookup[f].Dispose();           // libera os recursos do controle
                flowerLookup.Remove(f);
            }
    }
}
```

Observe o `Dictionary<Flower, PictureBox>`: ele **associa cada objeto do modelo** ao **controle que o representa**.

::: cuidado Seus controles também precisam ser descartados
Controles seguram recursos do Windows (**handles**). Ao remover um controle do formulário com `Controls.Remove`, **chame também `Dispose()`**: sem isso, o programa pode vazar recursos e, depois de milhares de criações, falhar de formas misteriosas.
:::

## `UserControl`: uma forma fácil de criar um controle

Se você precisa de um controle **reutilizável** — por exemplo, uma abelha animada — crie um **`UserControl`**: um mini-formulário que você desenha no designer e usa como qualquer controle.

1. **Projeto → Adicionar Controle de Usuário…** → `BeeControl`.
2. No designer, coloque um `PictureBox` (`pictureBox1`) e defina um **`Timer`** (`animationTimer`).
3. Crie as imagens (`Bee animation 1…4`) nos recursos e alterne-as no `Tick`:

```cs
public partial class BeeControl : UserControl
{
    private Bitmap[] frames = { Properties.Resources.Bee1, Properties.Resources.Bee2, Properties.Resources.Bee3, Properties.Resources.Bee2 };
    private int cell = 0;

    public BeeControl()
    {
        InitializeComponent();
        animationTimer.Interval = 150;
        animationTimer.Tick += (s, e) =>
        {
            cell = (cell + 1) % frames.Length;       // 0,1,2,3,0,1,...
            BackgroundImage = frames[cell];
        };
        animationTimer.Start();
    }
}
```

O truque `(cell + 1) % frames.Length` gira por todos os quadros e volta ao zero — uma **animação por quadros** (*frame animation*), como um flip-book.

::: passos Quadros de uma animação
Um `Timer` troca o **quadro** a cada 150 ms:

::: html
<svg viewBox="0 0 700 110"><g text-anchor="middle" font-size="12"><rect class="f-prim" x="20" y="20" width="130" height="70" rx="10"/><text x="85" y="62" font-weight="800" class="t-prim">quadro 0</text><rect class="f-plain" x="190" y="20" width="130" height="70" rx="10"/><text x="255" y="62" class="t-dim">quadro 1</text><rect class="f-plain" x="360" y="20" width="130" height="70" rx="10"/><text x="425" y="62" class="t-dim">quadro 2</text><rect class="f-plain" x="530" y="20" width="130" height="70" rx="10"/><text x="595" y="62" class="t-dim">quadro 3</text></g></svg>
:::
===
No próximo `Tick`: `cell = 1`.

::: html
<svg viewBox="0 0 700 110"><g text-anchor="middle" font-size="12"><rect class="f-plain" x="20" y="20" width="130" height="70" rx="10"/><text x="85" y="62" class="t-dim">quadro 0</text><rect class="f-prim" x="190" y="20" width="130" height="70" rx="10"/><text x="255" y="62" font-weight="800" class="t-prim">quadro 1</text><rect class="f-plain" x="360" y="20" width="130" height="70" rx="10"/><text x="425" y="62" class="t-dim">quadro 2</text><rect class="f-plain" x="530" y="20" width="130" height="70" rx="10"/><text x="595" y="62" class="t-dim">quadro 3</text></g></svg>
:::
===
Chegando ao último, `(3 + 1) % 4 = 0`: volta ao início e o ciclo recomeça — o **resto da divisão** é o "relógio" que dá a volta.

::: html
<svg viewBox="0 0 700 110"><g text-anchor="middle" font-size="12"><rect class="f-plain" x="20" y="20" width="130" height="70" rx="10"/><text x="85" y="62" class="t-dim">quadro 0</text><rect class="f-plain" x="190" y="20" width="130" height="70" rx="10"/><text x="255" y="62" class="t-dim">quadro 1</text><rect class="f-plain" x="360" y="20" width="130" height="70" rx="10"/><text x="425" y="62" class="t-dim">quadro 2</text><rect class="f-prim" x="530" y="20" width="130" height="70" rx="10"/><text x="595" y="62" font-weight="800" class="t-prim">quadro 3</text></g><path class="arrow" d="M595 94C500 125 200 125 100 94"/></svg>
:::
:::

## Performance: quando o programa fica lento

Criar `PictureBox`es e redimensionar imagens a cada quadro pode **travar** o programa. Boas práticas:

- **Reaproveite** controles (como o `Dictionary` acima) em vez de recriá-los.
- **Redimensione uma vez** — guarde o `Bitmap` já no tamanho final.
- Dispose de `Bitmap`s e `Graphics` que você mesmo criar.
- Meça antes de otimizar (use o **Profiler** ou um `Stopwatch`).

### Redimensionando um `Bitmap`

```cs
Bitmap original = Properties.Resources.Bee;
Bitmap pequeno = new Bitmap(original, new Size(40, 40));   // cria uma cópia redimensionada

// ou, controlando a qualidade com Graphics:
Bitmap resultado = new Bitmap(40, 40);
using (Graphics g = Graphics.FromImage(resultado))
{
    g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
    g.DrawImage(original, 0, 0, 40, 40);
}
```

Imagens vindas dos **recursos** do projeto (`Properties.Resources.X`) são objetos `Bitmap`; cada acesso a `Resources.X` **cria uma nova cópia** — guarde em uma variável!

## Assumindo o controle com `System.Drawing` (GDI+)

O **GDI+** é a biblioteca de desenho 2D do .NET clássico: linhas, retângulos, elipses, textos e imagens. O objeto central é **`Graphics`** — uma **superfície de desenho**.

| Objeto | O que é |
|---|---|
| `Graphics` | A "tela" onde se desenha (um formulário, um bitmap, uma impressora) |
| `Pen` | Caneta para **contornos** (cor, espessura) |
| `Brush` | Pincel para **preenchimentos** (`SolidBrush`, `LinearGradientBrush`) |
| `Font` | Fonte do texto |
| `Bitmap` | Imagem em memória |
| `Point`, `Size`, `Rectangle` | Posições e medidas |

```cs
using (Graphics g = this.CreateGraphics())
{
    g.FillEllipse(Brushes.Gold, 30, 30, 120, 120);                 // elipse preenchida
    g.DrawRectangle(new Pen(Color.DarkBlue, 3), 20, 20, 140, 140); // contorno
    g.DrawString("Olá!", new Font("Segoe UI", 18), Brushes.Black, 60, 70);
    g.DrawLine(Pens.Red, 0, 0, 200, 200);
}
```

::: figura O sistema de coordenadas do GDI+
<svg viewBox="0 0 520 250" role="img" aria-label="A origem fica no canto superior esquerdo; y cresce para baixo">
  <rect class="f-plain" x="60" y="20" width="400" height="200" rx="4"/>
  <circle cx="60" cy="20" r="5" class="svg-ink"/><text x="70" y="14" font-size="12" class="t-mono">(0, 0)</text>
  <path class="arrow" d="M60 20H180"/><text x="190" y="24" font-size="13" font-weight="700" class="t-prim">x →</text>
  <path class="arrow" d="M60 20V130"/><text x="66" y="148" font-size="13" font-weight="700" class="t-prim">y ↓</text>
  <ellipse cx="290" cy="120" rx="70" ry="45" class="f-warn"/><text x="290" y="125" text-anchor="middle" font-size="12" class="t-mono">FillEllipse</text>
  <text x="260" y="238" font-size="12" class="t-dim">o eixo Y cresce para BAIXO (como nas telas)</text>
</svg>
:::

### Desenhe no evento `Paint`

Desenhar com `CreateGraphics()` **funciona** — até você minimizar a janela ou cobri-la com outra: o Windows **apaga** o que estava ali e seu desenho **some**. O formulário não sabe como se repintar.

A solução: desenhe **dentro do evento `Paint`**, que o Windows dispara **sempre que o controle precisa ser repintado**:

```cs
private void Form1_Paint(object sender, PaintEventArgs e)
{
    Graphics g = e.Graphics;                 // já vem pronto, NÃO dê Dispose nele
    g.FillEllipse(Brushes.Gold, 30, 30, 120, 120);
    g.DrawString("Sempre aqui", Font, Brushes.Black, 40, 160);
}
```

Para **pedir** um redesenho (depois de mudar algo), chame `Invalidate()` (ou `Refresh()`):

```cs
x += 5;
Invalidate();      // "mande o evento Paint ser disparado"
```

::: passos Como um formulário se repinta
O Windows decide repintar quando a janela é revelada, redimensionada ou **você pede**.
===
O **formulário** limpa o fundo e dispara o evento **`Paint`**, com um `Graphics` apontando para a área a pintar.
===
Seu tratador **desenha tudo** (do zero!) — o `Paint` pode ser chamado a qualquer momento e deve deixar o resultado completo e correto.
===
Cada **controle filho** (`Button`, `PictureBox`) repinta a si mesmo; por isso muitos controles + animação rápida = ficar lento.
:::

## Buffer duplo: acabando com a tremulação

Ao animar com `Paint`, você pode notar **tremulação** (*flicker*): a tela é apagada e desenhada em etapas, e o olho vê isso. A solução é o **buffer duplo** (*double buffering*): o programa desenha primeiro numa **imagem fora da tela** (o buffer) e só então **copia tudo de uma vez** para a janela.

```cs
public Form1()
{
    InitializeComponent();
    this.DoubleBuffered = true;      // formulários e controles já suportam!
}
```

::: analogia Palco e bastidores
Num teatro, a mudança de cenário é feita **nos bastidores**; a plateia só vê o cenário **pronto**. O buffer duplo é o mesmo: desenhe nos bastidores, mostre pronto.
:::

## Imprimindo com `Graphics`

O mesmo objeto `Graphics` serve para a **impressora**: o `PrintDocument` dispara o evento `PrintPage`, no qual você desenha a página.

```cs
PrintDocument doc = new PrintDocument();
doc.PrintPage += (s, e) =>
{
    e.Graphics.DrawString("Relatório da colmeia", new Font("Arial", 20), Brushes.Black, 50, 50);
    e.Graphics.DrawImage(meuBitmap, 50, 100);
};

PrintPreviewDialog previa = new PrintPreviewDialog() { Document = doc };   // visualização
previa.ShowDialog();

PrintDialog dlg = new PrintDialog() { Document = doc };
if (dlg.ShowDialog() == DialogResult.OK) doc.Print();
```

::: exercicio Uma bola que quica
Faça um formulário com um `Timer` (`Interval = 20`) que move uma bola (círculo) pela janela e a faz **quicar** nas bordas. Use `Paint` e `DoubleBuffered`.

--- resposta
```cs
public partial class Form1 : Form
{
    int x = 50, y = 50, dx = 4, dy = 3;
    const int D = 40;

    public Form1()
    {
        InitializeComponent();
        DoubleBuffered = true;
        timer1.Interval = 20;
        timer1.Start();
    }

    private void timer1_Tick(object sender, EventArgs e)
    {
        x += dx; y += dy;
        if (x < 0 || x + D > ClientSize.Width)  dx = -dx;
        if (y < 0 || y + D > ClientSize.Height) dy = -dy;
        Invalidate();                              // pede para repintar
    }

    private void Form1_Paint(object sender, PaintEventArgs e)
    {
        e.Graphics.FillEllipse(Brushes.OrangeRed, x, y, D, D);
    }
}
```
Se tirar `DoubleBuffered = true` você verá o tremor. O `Invalidate()` **dispara** o `Paint`; nunca desenhe fora dele.
:::

::: resumo
- Controles são **objetos**: crie-os com `new` e adicione a `Controls`; descarte (`Dispose`) ao remover.
- **`UserControl`** cria componentes reutilizáveis (e animados com `Timer`).
- Guarde `Bitmap`s redimensionados; recursos criam cópia a cada acesso.
- **GDI+**: `Graphics`, `Pen`, `Brush`, `Font`; coordenadas com origem no canto **superior esquerdo**.
- Desenhe no evento **`Paint`**; use **`Invalidate()`** para pedir redesenho.
- **Buffer duplo** (`DoubleBuffered = true`) elimina a tremulação.
- **`PrintDocument`** + `PrintPage` imprimem usando o mesmo `Graphics`.
:::

::: quiz
? O que é preciso fazer para um controle criado em código aparecer no formulário?
- Nada; aparece sozinho
+ Adicioná-lo à coleção `Controls` do formulário/contêiner
- Chamar `Application.Run`
- Declará-lo como `static`
> `this.Controls.Add(controle)`.

? Por que chamar `Dispose()` ao remover um controle?
+ Para liberar os recursos do Windows que ele segura
- Para apagar o formulário
- Para evitar o evento `Paint`
- Não é necessário
> Controles usam handles do sistema.

? Em qual evento você deve desenhar com `Graphics` para o desenho não sumir?
- `Load`
- `Click`
+ `Paint`
- `Resize`
> O Windows chama `Paint` sempre que precisa repintar.

? Qual método pede ao Windows para repintar um controle?
- `Draw()`
- `Repaint()`
+ `Invalidate()`
- `Close()`
> `Invalidate()` marca a área como suja e dispara `Paint`.

? Qual a origem (0,0) no sistema de coordenadas do GDI+?
- Canto inferior esquerdo
+ Canto superior esquerdo, com o eixo Y crescendo para baixo
- Centro da janela
- Canto superior direito
> É o padrão em telas.

? O que o buffer duplo resolve?
+ A tremulação durante animações
- Falta de memória
- Erros de compilação
- Lentidão de rede
> Desenha fora da tela e copia o quadro pronto.

? Para que serve um `UserControl`?
+ Criar componentes visuais reutilizáveis desenhados no designer
- Substituir o `Program.cs`
- Controlar usuários do Windows
- Salvar arquivos
> É um "mini formulário" que você usa como controle.
:::

::: cartas
Controle :: Objeto visual de formulário (Button, PictureBox…); precisa estar em `Controls` para aparecer.
UserControl :: Controle personalizado criado a partir do designer, reutilizável.
Graphics :: Superfície de desenho do GDI+ (formulário, bitmap, impressora).
Pen / Brush :: Caneta para contorno / pincel para preenchimento.
Bitmap :: Imagem em memória; objetos de `Properties.Resources` são Bitmaps.
Paint :: Evento disparado quando o controle precisa ser repintado.
Invalidate() :: Solicita o repintar (dispara Paint).
Buffer duplo :: Desenhar fora da tela e copiar de uma vez para evitar flicker.
PrintDocument :: Objeto que gera páginas para impressão via evento PrintPage.
Dispose() :: Libera recursos não gerenciados de um objeto.
:::
