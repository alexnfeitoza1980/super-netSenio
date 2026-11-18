# Super .NET Sênior — o livro único

Os livros e sites de ensino que estavam espalhados pelos sete projetos de estudo, reunidos em **um só livro interativo**:
um sumário, uma busca, um glossário, um conjunto de cartões de revisão e um progresso de leitura para tudo.

## Como abrir

Abra `index.html` no navegador. Não precisa de servidor nem de internet, com uma exceção: os **diagramas Mermaid**
(77 no livro, 71 deles no guia de arquitetura) são carregados de um CDN na primeira vez que um capítulo com diagrama é aberto. Sem
internet, o texto do diagrama continua legível, só não vira desenho.

O progresso de leitura e as preferências ficam **no seu navegador** (`localStorage`, chaves `sns.*`).

## O que tem dentro

| Área | Origem | Conteúdo |
|---|---|---|
| **C#** | `livro-csharp-senior` (livros 1–2) | *Aprenda C#* e *Projeto Ágil em C#* |
| **C# & Mentalidade** | `livro-csharp-senior` (livros 3–10) | Oito livros de hábitos e mentalidade aplicados a C#, com exemplos e testes |
| **Arquitetura .NET** | `Arquitetura-microservicos.NET` | 21 módulos: 73 seções (21 visões gerais + 52 aulas) |
| **Laboratório** | `microservico-senior`, `aula-guiada-monolito-gemini`, `aula_guiada_mcroservicos_net_ia`, `Anfsusax-solid-principles`, `bankmore-microservices`, `super-netSenior` | Aulas práticas: Gateway/Saga/Circuit Breaker/Tracing, Outbox/Inbox/Pix, Clean Architecture, SOLID, BankMore e o próprio `super-netSenior` |
| **Angular** | `livro-csharp-senior/angular` | Cinco livros Angular |
| **Referência** | todos | Glossário geral e folhas de cola, **fundidos** |

O mapa completo (partes, número de seções e atalho para o começo de cada uma) é **gerado a cada build** e aparece no
capítulo *Mapa do livro*, dentro do próprio leitor.

O código que acompanha os livros fica em `codigo/<livro>/` (soluções .NET e apps Angular, sem `bin/` e `obj/`). Os
capítulos que citam `codigo/...` já apontam para esses caminhos.

## Estrutura

```text
livro/
├─ index.html              página do livro (gerada)
├─ livro.json              título, textos da capa e paleta
├─ content/                os capítulos em Markdown (frontmatter + diretivas `:::`) — FONTE do livro
├─ img/<livro>/            imagens, uma pasta por livro (nomes como cap01.svg se repetiam entre livros)
├─ codigo/<livro>/         código-fonte que acompanha cada livro
├─ assets/                 book-data.js e tema.css (gerados)
├─ motor/                  o leitor (app.js, estilo.css, modelo-livro.html)
├─ build.py                content/ -> index.html + assets/, e roda a validação
└─ tools/                  importação e validação (veja abaixo)
```

## Como reconstruir

```bash
python build.py            # gera index.html e assets/ e valida o livro
python build.py --sem-validar
```

O `build.py` termina com código diferente de zero se a validação encontrar **erros** (id repetido, imagem ou link
inexistente, bloco ``` ou `:::` sem fechamento, glossário vazio…).

## Código que acompanha os livros

Cada pasta de `codigo/<livro>/` é independente (24 projetos .NET e 5 apps Angular), com o `README-do-livro.md` original.

```bash
dotnet test codigo/joao/Joao.Testes                  # idem: amor, crencas, emocional, resiliencia
dotnet run --project codigo/compromissos/Caderno.Testes   # o livro 5 tem suíte executável própria (net10)
```

Estado verificado: as seis suítes .NET passam (**510 testes**: Joao 206, Habitos 131, Resiliencia 99, Caderno 44,
Emocoes 17, Crencas 13). Os apps Angular não foram executados (pedem `npm install`).

`codigo/Directory.Build.props` e `codigo/Directory.Packages.props` existem **de propósito e vazios/desligados**: sem
eles, os arquivos de mesmo nome na raiz do `super-netSenior` (que ligam o gerenciamento *central* de pacotes e fixam o
.NET 10) se aplicariam a estes projetos e o build dos testes falharia com `NU1008`.

## Como o conteúdo foi importado

`content/` é gerado por `tools/importar.py` a partir dos projetos de origem e pode ser regenerado:

```bash
# 1) Snapshot do Git do livro (inclui livros que foram apagados do disco mas continuam no HEAD)
git -C ../../livro-csharp-senior archive HEAD | tar -x -C <pasta-snapshot>

# 2) Importar (sobrescreve content/<id>.md; nunca apaga nada)
python tools/importar.py --snapshot <pasta-snapshot> --origens ..\..
python build.py
```

| Módulo | Faz |
|---|---|
| `tools/fontes_livros.py` | livros C# e Angular: prefixa ids por livro, move imagens para `img/<livro>/`, reescreve caminhos e links entre livros, copia `codigo/` |
| `tools/fontes_arquitetura.py` | guia de arquitetura: tira o frontmatter do Obsidian, converte callouts `> [!NOTE]` em diretivas, reescreve links entre aulas |
| `tools/fontes_laboratorios.py` | aulas que viviam em HTML/JS/C#: `site-ensino`, `pix-lessons.js`, `lessons.js` do ANFX, `PrincipiosSolid.cs`, `SETUP-COMPLETO.md` e os capítulos próprios em `tools/conteudo/` |
| `tools/html2md.py` | conversor HTML → Markdown por regras de classe (só biblioteca padrão) |
| `tools/extrair_js.js` | lê dados de arquivos JS via `node` (isolado em `vm`) |
| `tools/validar.py` | checagens de integridade, com as mesmas regras de leitura do motor |

Os capítulos `00-introducao.md` e `01-mapa-do-livro.md` são escritos à mão (ids `intro` e `mapa`).

### Numeração

Os livros C# **preservam** a numeração original de capítulos (1–133), laboratórios (1–14) e **partes** (1–18): ela já
era contínua entre os livros e o glossário e alguns textos a citam. O conteúdo novo continua a partir daí. Nos demais
livros as partes são renumeradas em sequência (o prefixo `Parte I ·` do Angular sai do título).

Duas peculiaridades **da origem**, mantidas como estão: os livros 8 (*Você é aquilo que ama*) e 9 (*Emoções*) já
usavam ambos a "Parte 17", e o livro 5 (*Quatro Compromissos*) nunca teve número de parte (aparece como "Oficina").
A apresentação de cada livro entra na primeira parte dele, e os apêndices/encerramento, na última.

## Correções feitas na importação

Defeitos que **já existiam** nos livros de origem e foram corrigidos de forma reproduzível (em `tools/`):

| Onde | Defeito | Correção |
|---|---|---|
| 17 SVGs de livros Angular | XML inválido (`&` solto em texto; um atributo sem aspas) — o navegador não desenhava a imagem | `sanear_svg()` repara e confere que passou a parsear |
| `ang-amigos-cap49` | ```` ```suggestion ```` aninhado em ```` ```markdown ````; o bloco externo fechava cedo e o resto do capítulo virava "código" | trecho interno passa a usar `~~~` |
| `joao-cap125` | faltava o `:::` que fecha o quiz; o bloco `cartas` ficava aninhado nele e os cartões sumiam | fechamento inserido |

## O que ficou de fora de propósito

- **Simuladores interativos** do `site-ensino` (disjuntor, saga, carga): dependem do JS do site de origem. O quiz que
  estava junto deles **foi** convertido.
- **Aulas "em planejamento"** do portal ANFX: eram placeholders sem conteúdo.
- **`obsidia-reader`** e os módulos **Blazor** de `microservico-senior`: eram leitores/cópias do mesmo conteúdo, substituídos por este livro.
- Arquivos de meta do guia de arquitetura (`README.md` da raiz, `00-MOTOR-GERADOR-UNIVERSAL.md`, `Sem título.md`, `extensions.txt`).
- Pastas de apoio dos livros originais que não são conteúdo: `ESTUDO/`, `ferramentas/`, `saidas/`, `AUDITORIA/`.
