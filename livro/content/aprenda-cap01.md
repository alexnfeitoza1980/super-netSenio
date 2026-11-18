---
id: aprenda-cap01
order: 11
kind: capitulo
num: 1
group: C# · Parte 1 · Primeiros passos
title: Seja produtivo com C#
subtitle: Um aplicativo visual com banco de dados e instalador — em poucos minutos, sem escrever código obscuro.
minutes: 40
hue: 262
objectives:
- Entender o que a **IDE** do Visual Studio faz por você (e o que continua sendo seu trabalho)
- Criar um formulário Windows com controles, propriedades e eventos
- Montar um **banco de dados SQL**, uma tabela e ligar tudo ao formulário
- **Publicar** o aplicativo para outras pessoas instalarem
origem: livros/01-aprenda-csharp/content/01-cap01.md
---

::: ilustracao img/aprenda/cap01.svg | Em poucos minutos, arrastar controles monta um aplicativo visual: a IDE faz o trabalho repetitivo.
:::

## Por que aprender C#?

C# é uma linguagem poderosa, mas o grande trunfo dela é o par **C# + Visual Studio**. A IDE (*Integrated Development Environment*, ambiente integrado de desenvolvimento) cuida das tarefas repetitivas e chatas para você se concentrar no **problema real**.

Pense no que um aplicativo simples precisa só para mostrar **um botão** numa janela: dezenas de linhas de código para criar o botão, definir nome, tamanho, posição, texto, cor, ligar o clique, adicionar à janela... Ninguém quer escrever isso à mão toda vez.

::: figura O que a IDE automatiza para você
<svg viewBox="0 0 760 210" role="img" aria-label="Você arrasta um botão; a IDE escreve o código">
  <rect class="f-plain" x="20" y="30" width="190" height="150" rx="14"/>
  <text x="115" y="58" text-anchor="middle" font-weight="800">Você</text>
  <rect class="f-prim" x="55" y="80" width="120" height="40" rx="8"/><text x="115" y="106" text-anchor="middle" font-size="15" class="t-prim" font-weight="700">[ Button ]</text>
  <text x="115" y="150" text-anchor="middle" font-size="13" class="t-dim">arrasta e solta</text>
  <path class="arrow" d="M215 105H300"/>
  <rect class="f-info" x="305" y="20" width="190" height="170" rx="14"/>
  <text x="400" y="48" text-anchor="middle" font-weight="800" class="t-info">Visual Studio</text>
  <text x="400" y="78" text-anchor="middle" font-size="13" class="t-dim">gera classes</text>
  <text x="400" y="100" text-anchor="middle" font-size="13" class="t-dim">gera propriedades</text>
  <text x="400" y="122" text-anchor="middle" font-size="13" class="t-dim">gerencia arquivos</text>
  <text x="400" y="144" text-anchor="middle" font-size="13" class="t-dim">compila e publica</text>
  <path class="arrow" d="M500 105H555"/>
  <rect class="f-good" x="560" y="30" width="180" height="150" rx="14"/>
  <text x="650" y="58" text-anchor="middle" font-weight="800" class="t-good">Código C#</text>
  <text class="t-mono" x="574" y="88" font-size="11">button1 = new Button();</text>
  <text class="t-mono" x="574" y="108" font-size="11">button1.Text = "OK";</text>
  <text class="t-mono" x="574" y="128" font-size="11">button1.Size = ...;</text>
  <text class="t-mono" x="574" y="148" font-size="11">Controls.Add(button1);</text>
</svg>
:::

Com C# e Visual Studio você consegue:

- **Criar aplicativos rápido** — o C# é fácil de aprender e a IDE faz muito sozinha.
- **Montar interfaces bonitas** — o *Form Designer* (designer de formulários) é uma das ferramentas visuais mais agradáveis que existem.
- **Conectar bancos de dados** — a IDE tem editor de tabelas e se integra ao SQL Server.
- **Focar no que importa** — seu tempo vai para as regras do seu negócio, não para gerenciar imagens, ícones e arquivos de recursos.

## A missão: a Lista de Contatos da Vila Objeto

::: vila O novo diretor-geral
A Empresa de Papel Vila Objeto contratou um novo diretor-geral, que ama natureza e decidiu virar um executivo **sem papel**. Ele começa pelos contatos: quer um aplicativo de agenda pronto quando voltar do fim de semana de esqui. Prazo apertado — e ele parece ter pouca paciência.
:::

Antes de abrir o Visual Studio, **pare e pense em quem vai usar o programa**. Esse é o hábito mais valioso de qualquer programador: levantar as necessidades antes de codificar.

- O diretor vai usar no escritório **e** no notebook de viagem → precisa de um **instalador**.
- A equipe de vendas quer usar os contatos para listas de e-mail → os dados ficam em um **banco de dados** compartilhável.
- Todos esperam um programa **intuitivo**, como qualquer aplicativo Windows.

::: pense Do cartão de papel para a tela
Se você tivesse que passar um cartão de visita de papel (nome, empresa, telefone, e-mail, "é cliente?", data do último contato) para um sistema, **onde** guardaria cada informação? Pense nisso — a resposta vem nas próximas seções.
:::

### As quatro peças do aplicativo

::: figura Arquitetura da Lista de Contatos
<svg viewBox="0 0 780 200" role="img" aria-label="Formulário, objetos .NET, banco de dados e instalador">
  <rect class="f-prim" x="10" y="40" width="165" height="110" rx="14"/>
  <text x="92" y="76" text-anchor="middle" font-weight="800" class="t-prim">Formulário</text>
  <text x="92" y="100" text-anchor="middle" font-size="13" class="t-dim">(a tela que o</text><text x="92" y="118" text-anchor="middle" font-size="13" class="t-dim">usuário vê)</text>
  <path class="line" d="M178 95H214" marker-start="url(#arr2)" marker-end="url(#arr2)"/>
  <rect class="f-info" x="217" y="40" width="165" height="110" rx="14"/>
  <text x="300" y="76" text-anchor="middle" font-weight="800" class="t-info">Objetos .NET</text>
  <text x="300" y="100" text-anchor="middle" font-size="13" class="t-dim">(acesso a dados:</text><text x="300" y="118" text-anchor="middle" font-size="13" class="t-dim">DataSet, TableAdapter)</text>
  <path class="line" d="M385 95H421" marker-start="url(#arr2)" marker-end="url(#arr2)"/>
  <rect class="f-good" x="424" y="40" width="165" height="110" rx="14"/>
  <text x="506" y="76" text-anchor="middle" font-weight="800" class="t-good">Banco SQL</text>
  <text x="506" y="100" text-anchor="middle" font-size="13" class="t-dim">(tabela People com</text><text x="506" y="118" text-anchor="middle" font-size="13" class="t-dim">todos os contatos)</text>
  <path class="arrow" d="M592 95H620"/>
  <rect class="f-warn" x="623" y="40" width="150" height="110" rx="14"/>
  <text x="698" y="76" text-anchor="middle" font-weight="800" class="t-warn">Instalador</text>
  <text x="698" y="100" text-anchor="middle" font-size="13" class="t-dim">(setup.exe para</text><text x="698" y="118" text-anchor="middle" font-size="13" class="t-dim">outros PCs)</text>
</svg>
:::

## Conhecendo a IDE

Crie um projeto: **Arquivo → Novo → Projeto → Aplicativo do Windows Forms** e chame de `Contatos`.

::: ide Qual template escolher hoje?
Para acompanhar o livro fielmente, escolha **"Windows Forms App (.NET Framework)"** (C#). O template "Windows Forms App" para .NET moderno também serve para quase tudo; só o assistente de banco de dados da seção "Fonte de dados" é mais completo no .NET Framework. Se não encontrar o template, rode o **Visual Studio Installer** e adicione a carga *Desenvolvimento para desktop com .NET*.
:::

Ao criar o projeto, a IDE gera três arquivos: **`Form1.cs`**, **`Form1.Designer.cs`** e **`Program.cs`**. Sua tela terá estas áreas:

| Janela | Para que serve |
|---|---|
| **Gerenciador de Soluções** (*Solution Explorer*) | Lista todos os arquivos do projeto — é o "explorador" do seu programa. |
| **Designer / Editor** | A área central: desenha o formulário ou mostra o código. Cada arquivo abre numa aba. |
| **Caixa de Ferramentas** (*Toolbox*) | Os controles prontos (botões, caixas de texto, imagens…) para arrastar para o formulário. |
| **Propriedades** (*Properties*) | Ajusta aparência e comportamento do item selecionado (texto, tamanho, nome…). |
| **Lista de Erros** (*Error List*) | Mostra problemas de compilação para você corrigir. |

::: dica Perdeu uma janela?
Menu **Exibir** (*View*) → escolha a janela. Ou **Janela → Redefinir Layout** para voltar ao padrão.
:::

### Montando a interface: arrastar, soltar, configurar

1. Arraste um **PictureBox** (caixa de imagem) da Caixa de Ferramentas até o formulário.
2. Na pequena seta do controle, escolha **Escolher Imagem** e importe o logotipo da empresa.
3. Defina a propriedade **SizeMode = Zoom** para a imagem se ajustar sem distorcer.

Todo controle tem **propriedades**: valores que você pode ajustar (texto, cor, tamanho…). Mudou na janela Propriedades? A IDE reescreve o código correspondente.

::: passos O que a IDE faz quando você mexe no designer
**Passo 1 — você arrasta o PictureBox.** A IDE adiciona ao `Form1.Designer.cs` as linhas que **criam** o controle e o **adicionam** ao formulário.

```cs Form1.Designer.cs (trecho gerado)
this.pictureBox1 = new System.Windows.Forms.PictureBox();
// ...
this.Controls.Add(this.pictureBox1);
```
===
**Passo 2 — você escolhe uma imagem.** A imagem vira um **recurso**: um arquivo gráfico, de áudio ou ícone embutido no aplicativo. Fica guardado em `Form1.resx`, então viaja junto com o programa quando ele é instalado em outro computador.
===
**Passo 3 — você muda uma propriedade.** Ao definir `SizeMode`, a IDE escreve:

```cs
this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
```

Moral: **tudo que você faz no designer vira código C#**. Nada de mágica — só código escrito por uma ferramenta.
:::

### Reagindo a um clique (seu primeiro evento)

Dê um **duplo clique** no PictureBox. A IDE cria um método que será chamado sempre que o usuário clicar nele:

```cs Form1.cs
private void pictureBox1_Click(object sender, EventArgs e)
{
    MessageBox.Show("Lista de Contatos 1.0\nEscrito por: Seu Nome",
                    "Sobre");
}
```

`\n` é uma **quebra de linha** dentro do texto. Pressione **F5** (ou o botão ▶) para executar: isso é **depurar**, ou seja, rodar o programa dentro da IDE. Clique no logo e veja a caixa de mensagem.

::: regra Nome do evento = o que aconteceu
O C# chama o método `pictureBox1_Click` quando o evento *Click* do `pictureBox1` ocorre. Você só escreve o que deve acontecer **dentro** das chaves `{ }`. Eventos serão o tema do capítulo 11.
:::

## Criando o banco de dados

O diretor quer guardar nome, empresa, telefone, e-mail, se é cliente e a data do último contato. Esses dados vão para um **banco de dados SQL**: um sistema que armazena dados de forma organizada em **tabelas**.

::: analogia Tabela = planilha
Uma tabela se parece com uma planilha: **colunas** são as categorias (Nome, Telefone…) e cada **linha** (registro) é um contato completo. A diferença: um banco impõe regras (tipos, unicidade) e vários programas podem acessá-lo ao mesmo tempo.
:::

### Passo a passo no Visual Studio

1. No Gerenciador de Soluções: botão direito no projeto → **Adicionar → Novo Item → Banco de Dados Baseado em Serviço** (*SQL Database*). Nome: `ContactDB`.
2. Abra o **Gerenciador de Servidores/Bancos de Dados**. Em *Tabelas*, botão direito → **Adicionar Nova Tabela** e chame de `People`.
3. Crie as colunas, escolhendo o **tipo de dado** de cada uma.

::: ide Banco de dados no Visual Studio de hoje
O livro usa o *SQL Server Express* com o assistente de 2008. No Visual Studio atual a ideia é a mesma, mas o banco local normalmente é o **SQL Server LocalDB** (`.mdf` dentro do projeto) e o *SQL Server Object Explorer*. Os **diagramas de banco de dados** que o livro cria são hoje dispensáveis: o assistente de fonte de dados lê direto a tabela. Em projetos novos .NET você também pode usar **SQLite + Entity Framework Core**, mas aqui seguimos o caminho visual e rápido do livro.
:::

A coluna mais importante é a **chave primária**: um valor **único** que identifica cada linha. Criamos `ContactID` do tipo `int`, **sem permitir nulos** e com **Identidade** ligada (*Is Identity = Yes*): o próprio banco gera 1, 2, 3… para cada novo contato — você nunca precisa escrever código para isso.

| Campo do cartão | Coluna | Tipo de dado | Por quê |
|---|---|---|---|
| (identificador) | `ContactID` | `int` | Chave primária, gerada pelo banco |
| Nome | `Name` | `nvarchar(50)` | Texto de até 50 caracteres |
| Empresa | `Company` | `nvarchar(50)` | Texto |
| Telefone | `Telephone` | `nvarchar(20)` | Texto (telefone tem parênteses, hífens…) |
| E-mail | `Email` | `nvarchar(50)` | Texto |
| É cliente? | `Client` | `bit` | Sim/Não (verdadeiro/falso) |
| Último contato | `LastContact` | `datetime` | Data |

::: cuidado Telefone não é número!
Um telefone pode ter `(11)`, `+55`, hífens e zero à esquerda. Você nunca vai **somar** telefones, então guarde-os como **texto**. Regra geral: se você não vai fazer conta com o valor, provavelmente é texto.
:::

Salve a tabela (**Salvar**), depois **insira os contatos** abrindo *Mostrar dados da tabela* e digitando os registros diretamente. Use **Salvar tudo** para gravar.

::: exercicio Qual tipo combina com cada coluna?
Associe cada informação ao tipo de dado mais adequado: `int`, `nvarchar(50)`, `bit`, `datetime`.

1. O número de identificação do contato
2. O nome da empresa
3. Se a pessoa é cliente (sim/não)
4. A data da última ligação

--- resposta
1. `int` (e chave primária com identidade)
2. `nvarchar(50)`
3. `bit` (0 ou 1; no formulário vira uma caixa de seleção)
4. `datetime`
:::

## Conectando o formulário ao banco

O Visual Studio consegue gerar o código de acesso aos dados — mas só depois que o banco e a tabela existem. O caminho é criar uma **Fonte de Dados** (*Data Source*): um conjunto de objetos .NET e comandos SQL que o programa usa para conversar com o banco.

1. Menu **Dados → Adicionar Nova Fonte de Dados**.
2. Escolha **Banco de dados → Conjunto de dados (DataSet)**, a conexão `ContactDB.mdf`, marque a tabela `People`.
3. Nomeie o conjunto de dados como `ContactDBDataSet`.

Abra a janela **Fontes de Dados** e **arraste a tabela `People` para o formulário**. Pronto: surgem rótulos e controles *vinculados* (*bound controls*) para cada coluna — e uma barra de navegação (próximo, anterior, adicionar, excluir, salvar).

::: analogia Controle vinculado = mangueira ligada
Um controle vinculado é uma **mangueira** entre a caixa de texto e a coluna do banco. Mexeu na caixa? A água (o dado) flui para o banco. Navegou para o próximo registro? A caixa mostra o novo valor. Você não escreve esse encanamento.
:::

::: pense Quem escreveu todo esse código?
Procure em `Form1.Designer.cs`: haverá instruções que criam `contactDBDataSet`, `peopleBindingSource`, `peopleTableAdapter`… E no `Form1.cs`, um método `Form1_Load` com `this.peopleTableAdapter.Fill(...)` — a linha que **carrega os dados da tabela para o formulário**. Tudo gerado pela IDE, tudo código C# que você pode ler e alterar.
:::

## Bons programas são intuitivos

Um programa funcionando ainda não é um bom programa. Pequenos ajustes fazem grande diferença na usabilidade:

- **Alinhe** campos e rótulos pela esquerda, como nos aplicativos que as pessoas já conhecem.
- Apague o texto `checkBox1` que sobra ao lado da caixa de seleção (propriedade **Text**).
- Dê um título claro à janela: propriedade **Text** do formulário.
- Desative **MaximizeBox** e **MinimizeBox** (`False`) se a janela não precisa mudar de tamanho.

::: regra Teste como um usuário comum
Depois de pronto, abra o app e use-o **como alguém que nunca o viu**. Se você precisar explicar um passo, falta clareza na interface.
:::

## Compilar, executar e entender o que aconteceu

Quando você aperta **F5**, a IDE faz duas coisas:

1. **Compila** o código C# → gera um arquivo executável (`.exe`).
2. **Executa** o resultado.

Os arquivos compilados (o `.exe`, os recursos e uma **cópia** do banco `.mdf`) ficam em `bin\Debug`.

::: cuidado Seus dados "sumiram"?
Cada vez que você compila, a IDE pode **sobrescrever** o banco de `bin\Debug` com a cópia original do projeto. Dados digitados enquanto você depura somem na próxima execução. Se abrir o `.exe` fora da IDE, os dados são mantidos — até a próxima compilação. (Dá para mudar a propriedade *Copy to Output Directory* do `.mdf` para *Copiar se for mais recente*.)
:::

## Distribuir: entregar o app aos usuários

O aplicativo só funciona na sua máquina até ser **publicado**. No Visual Studio: **Compilar → Publicar** (*Publish*) → siga o assistente (tecnologia **ClickOnce**). Ele cria uma pasta `publish` com um `setup.exe` que instala o programa em qualquer PC com o .NET Framework.

::: dica Teste a instalação — sempre
Feche a IDE, rode o `setup.exe` como um usuário comum e confira se o app abre e salva os dados. Nunca entregue a alguém um instalador que você não instalou.
:::

::: resumo
- A **IDE** automatiza o trabalho repetitivo; **você** decide o que o programa faz.
- Tudo que você faz no designer vira **código C#** (em `Form1.Designer.cs`).
- Um **banco de dados** guarda dados em tabelas; a **chave primária** identifica cada linha.
- **Fonte de dados + controles vinculados** conectam formulário e banco quase sem código.
- **F5** compila e executa; **Publicar** gera o instalador.
- Levante as **necessidades dos usuários** *antes* de programar.
:::

## Exercício final

::: exercicio Troque o logotipo e o título
Sem consultar o capítulo, faça no seu projeto:
1. Altere o texto da caixa de mensagem do clique para incluir a data de hoje (dica: `DateTime.Now`).
2. Mude o título da janela para "Contatos — Vila Objeto".
3. Adicione um botão "Sair" que fecha a janela (dica: `this.Close();` no evento `Click`).

--- resposta
```cs
private void pictureBox1_Click(object sender, EventArgs e)
{
    MessageBox.Show("Lista de Contatos 1.0\nHoje é " + DateTime.Now.ToShortDateString());
}

private void buttonSair_Click(object sender, EventArgs e)
{
    this.Close();   // fecha o formulário e encerra o programa
}
```
O título é a propriedade **Text** do formulário. Para criar o evento do botão, dê duplo clique nele no designer.
:::

::: quiz
? O que é uma **propriedade** de um controle?
- Um método que o controle executa
+ Um valor ajustável do controle, como Text, Size ou Name
- Um arquivo de recurso
- O código gerado pelo compilador
> Propriedades descrevem aparência e comportamento do controle. Mudar uma na janela Propriedades faz a IDE reescrever o código.

? Em qual arquivo a IDE escreve o código que cria e posiciona os controles do formulário?
- Program.cs
+ Form1.Designer.cs
- Form1.resx
- ContactDB.mdf
> O Form1.Designer.cs guarda o código gerado pelo designer; o .resx guarda recursos (como imagens).

? Para que serve a **chave primária** de uma tabela?
- Para criptografar os dados
- Para ordenar as colunas alfabeticamente
+ Para identificar cada linha de forma única
- Para impedir que alguém apague registros
> Cada valor de chave primária aparece uma só vez na tabela, então dá para localizar um registro específico.

? Por que `Telephone` foi guardado como texto e não como número?
- O banco não aceita números
+ Telefones têm símbolos e zeros à esquerda e nunca entram em contas
- Texto ocupa menos espaço
- Porque o C# proíbe números de telefone
> Dados em que você não faz aritmética (telefone, CEP, CPF) costumam ser texto.

? O que acontece quando você pressiona **F5** no Visual Studio?
- Apenas salva o projeto
+ Compila o programa e o executa dentro da IDE (depuração)
- Publica o instalador
- Fecha o formulário atual
> F5 = compilar + executar com o depurador anexado.

? O que é um **recurso** (resource) no projeto?
- Uma variável global
+ Um arquivo gráfico, de áudio, ícone etc. embutido no aplicativo
- Um tipo de banco de dados
- Uma janela da IDE
> Recursos viajam junto com o programa e ficam em arquivos .resx.

? O que é um **controle vinculado** (bound control)?
+ Um controle ligado a uma coluna do banco, que atualiza os dados automaticamente
- Um controle que não pode ser movido
- Um botão sem evento
- Um controle desenhado manualmente
> Mudar o valor no controle altera o dado na fonte (e vice-versa).

? Qual é a ferramenta do Visual Studio que cria o instalador do seu aplicativo (ClickOnce)?
- Debug
- Toolbox
+ Publish (Publicar)
- Error List
> A publicação gera a pasta com o setup.exe para instalar em outros computadores.
:::

::: cartas
IDE :: Ambiente Integrado de Desenvolvimento: editor + designer + compilador + depurador.
Form (formulário) :: A janela do aplicativo Windows; é uma classe C# cujos controles você posiciona no designer.
Controle :: Elemento visual do formulário (botão, caixa de texto, imagem...).
Propriedade :: Valor configurável de um controle ou objeto (ex.: Text, Size).
Evento :: Algo que acontece (um clique) e para o qual você escreve um método que responde.
Recurso (resource) :: Arquivo embutido no app (imagem, ícone, som), guardado em .resx.
Banco de dados SQL :: Sistema que guarda dados em tabelas relacionadas.
Tabela / coluna / linha :: Estrutura do banco: colunas são categorias; cada linha é um registro.
Chave primária :: Coluna cujo valor identifica unicamente cada linha.
Fonte de dados (DataSet) :: Objetos .NET que levam dados do banco ao formulário e de volta.
Controle vinculado (bound) :: Controle ligado a uma coluna: alterações fluem nos dois sentidos.
F5 / Depurar :: Compila e executa o programa dentro da IDE.
Publicar (ClickOnce) :: Gera um instalador para distribuir o aplicativo.
:::
