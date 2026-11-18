---
id: agil-reflexoes
order: 59
kind: apendice
group: C# · Parte 11 · Pacotes, padrões e arquitetura
title: Reflexões — duas empresas e o que é software (Projeto Ágil em C#)
subtitle: Uma fábula comparando um projeto "pesado" com um projeto ágil, e uma ideia que muda a forma de pensar a profissão — o código-fonte é o projeto.
minutes: 30
hue: 20
badge: B
objectives:
- Comparar, numa **mesma situação**, um processo guiado por documentos e um processo ágil
- Reconhecer os **sintomas** de um projeto que vai mal (e de um que vai bem)
- Entender a tese de que o **código-fonte é o projeto de software** — e suas consequências práticas
origem: livros/02-projeto-agil-csharp/content/58-apendice-reflexoes.md
---

::: vila Antes de começar
Este apêndice conta **duas histórias** com texto próprio, inspiradas nas ideias do material estudado: uma **fábula** (imaginária) para sentir a diferença entre dois jeitos de conduzir um projeto, e um **ensaio curto** sobre o que, afinal, o software *é*. Leia com calma: muita coisa dos capítulos 16 a 40 ganha sentido aqui.
:::

::: ilustracao img/agil/reflexoes.svg | A fábula das duas empresas: documentos demais atrasam; ciclos curtos com código funcionando entregam.
:::

## Parte A — Uma fábula de duas empresas

Dois pontos de partida **idênticos**: em 3 de janeiro, duas empresas concorrentes — a **Cinza S.A.** e a **Verde Ltda.** — decidem criar um produto novo para o mesmo mercado, com a **mesma data-limite** (uma feira em julho) e **times de tamanho parecido**. O que muda é **como** cada uma trabalha.

### O que acontece na Cinza S.A.

1. **A reunião de abertura.** A diretoria apresenta o produto em dez minutos, anuncia o prazo ("nada tem prioridade mais alta") e **cancela os projetos em andamento**. Perguntam à equipe: *"quanto tempo leva a análise?"* — **antes** de existir qualquer requisito. Diz-se: *"finjam que vocês têm a análise"*. As datas são **negociadas de cima para baixo**: análise até abril, projeto até julho, "implementação nos três meses restantes".
2. **A fase de análise.** Sem requisitos, a equipe passa semanas em **debates filosóficos** sobre notação de casos de uso. O documento de requisitos chega em versões que **se contradizem** a cada semana. Ninguém consegue avaliar os modelos concorrentes.
3. **A fase de projeto.** Uma **ferramenta CASE** cara gera diagramas e código — com defeitos. Quando alguém tenta discutir o projeto fora do "momento certo", o gerente corta: *"a fase de projeto terminou; só se permite codificar"*. Um banco de dados da ferramenta se corrompe lentamente, inclusive nos backups.
4. **A fase de codificação.** O projeto **não tem significado** na prática; falta metade. O código gerado é ruim e mantê-lo sincronizado com os diagramas é inviável — os diagramas são **abandonados**. O gerente passa a **medir linhas de código** num termômetro de parede ("precisamos de um milhão até outubro") e institui regras absurdas, como **linhas de no máximo 20 caracteres**, depois **50 horas semanais**, **sábados** e **65 horas**.
5. **O desfecho.** A **qualidade** desaba (relatórios de defeitos, entregas diárias piores que as anteriores), o **moral** cai, pessoas pedem demissão, clientes cancelam pedidos. Em abril do ano seguinte, a empresa **desiste** e passa a revender o produto **de outra**.

### O que acontece na Verde Ltda.

1. **A reunião de abertura.** A gerente de produto (Jane) conversa com a equipe e **escreve histórias de usuário em fichas** — lembretes de conversas, não especificações completas. Os desenvolvedores pedem **histórias pequenas** (estimáveis e testáveis); Jane cuida para que cada uma tenha **valor e prioridade** claros. Estimam **grosseiramente**.
2. **O primeiro planejamento.** Jane escolhe cinco histórias para a primeira **iteração** (3 semanas); a equipe as **decompõe em tarefas**, e **ajusta a expectativa**: a *velocidade* (quanto de trabalho cabe numa iteração) ainda é um **palpite**, que será corrigido.
3. **A iteração.** Programação em **pares**, **TDD**, **integração contínua** várias vezes ao dia. Os pares **se revezam** (o conhecimento do código se espalha). Jane entrega **testes de aceitação** todos os dias; os desenvolvedores os leem e entendem melhor o que ela espera.
4. **A primeira demonstração.** Mostra-se **o que funciona de verdade**. Percebe-se que a velocidade real é **menor** que a imaginada; a equipe **avisa cedo**, e Jane **escolhe** qual história sair da iteração. A demonstração acontece sem sustos, a gerência fica serena.
5. **O ritmo.** Na quarta iteração a equipe encontra um **ritmo sustentável**: surpresas na agenda caem a quase zero; **mudanças de requisitos continuam** (as pessoas veem o sistema e pedem ajustes), mas **são agendadas** — e ninguém se frustra. Os testes de aceitação são **automatizados**, o projeto é **refatorado** continuamente.
6. **O desfecho.** A primeira versão sai antes da feira, **com menos recursos do que se queria, mas os mais importantes**. Na segunda versão, tudo corre mais suave. Meses depois, a **Cinza** (!) procura a Verde para **licenciar** a tecnologia.

### Lado a lado

| Aspecto | **Cinza S.A.** | **Verde Ltda.** |
|---|---|---|
| Prazos | Impostos de cima, **antes** de saber o que fazer | **Estimativas** que a equipe dá e corrige com dados |
| Requisitos | Um **documento** extenso e instável | **Histórias** + conversa contínua + **testes de aceitação** |
| Fases | **Análise → projeto → código** em blocos rígidos, "proibido voltar" | **Iterações curtas** em que tudo acontece junto |
| Projeto | **Diagramas** gerados por ferramenta, logo abandonados | O **código** limpo e refatorado é o projeto |
| Feedback | Só no fim (quando já é tarde) | **A cada iteração** (demonstração) |
| Medida de progresso | **Linhas de código** | **Funcionalidades** que funcionam |
| Qualidade | Verificada **depois** por outro departamento | **Testes automatizados** durante o desenvolvimento |
| Pessoas | **Horas extras** crescentes | **Ritmo sustentável** |
| Comunicação | Memorandos, relatórios, auditorias de "maturidade" | Conversa diária, **trabalho em pares**, parede de histórias |
| Resultado | Fracasso, cancelamento | Produto no prazo (escopo ajustado), cliente satisfeito |

::: pense O que a fábula **não** diz
A fábula **exagera** de propósito (é uma sátira). Nem todo processo "pesado" é um desastre, nem toda equipe ágil é um sucesso garantido. A lição **não é** "documento é ruim" nem "processo é ruim": é que **medir o progresso por artefatos, impor prazos sem dados, proibir o retorno entre fases, ignorar o feedback e esmagar as pessoas com horas extras** são caminhos que **raramente** terminam bem — e que práticas de **feedback curto, testes, comunicação e ritmo sustentável** dão **mais chances** de acerto.
:::

::: dica Sintomas para reconhecer no seu projeto
**Alarme vermelho:** "o requisito sempre muda" *sem* ninguém ver o sistema funcionando; progresso medido por linhas/horas; QA só no fim; reuniões de "alinhamento" sem decisões; heroísmo em vez de ritmo.
**Sinal verde:** demonstração a cada poucas semanas; testes que rodam a cada commit; fichas/lista de pendências visível e priorizada; equipe que **avisa cedo** quando algo não vai caber.
:::

::: exercicio Diagnóstico
Você entra num projeto e descobre: (a) integração de código só **uma vez por mês**, (b) testes manuais feitos por outro time **uma semana antes** da entrega, (c) requisitos em um documento de 200 páginas "aprovado" há 6 meses, (d) o gerente cobra **número de linhas** por semana.

1. Cite os **riscos** mais urgentes.
2. Proponha **três mudanças pequenas** que melhorem a situação **já**.

--- resposta
1. **Integração tardia** → "inferno de merge" e bugs de integração descobertos tarde; **teste no fim** → feedback demorado e correções caras; **requisitos congelados** → desconexão com a necessidade real; **linhas de código** como métrica → incentiva código inchado e mal feito.
2. (i) **Integração contínua** com *build* e testes automáticos a cada commit; (ii) começar a **automatizar testes** de aceitação dos fluxos mais críticos e rodá-los em toda entrega; (iii) **demonstrações quinzenais** com o cliente e uma lista priorizada de histórias; trocar a métrica por **funcionalidades entregues e funcionando**.
:::

## Parte B — O que é software?

### A pergunta provocadora

Será que desenvolvedores de software são **engenheiros**? Para responder, vale perguntar: **o que um projeto de engenharia produz?** Em engenharias tradicionais, o resultado final de todo o trabalho de projeto é um **documento** (plantas, especificações, diagramas). Esse documento vai para **outra equipe** — a de **fabricação** — que constrói o produto, às vezes em grande quantidade, **sem os projetistas**.

Agora olhe o ciclo de desenvolvimento de software com essa lente: de tudo que produzimos (requisitos, diagramas, especificações, código…), **qual artefato é suficientemente completo e preciso para ser "fabricado" sem mais intervenção dos projetistas**? Só **um**: o **código-fonte**. Os diagramas e documentos de alto nível são **esquemas parciais**; o código é o **projeto detalhado**, completo o bastante para um **processo automático** — o **compilador** — gerar o produto.

::: regra A tese
**O código-fonte é o projeto do software.** Quem "programa" está, na verdade, **projetando**. A fabricação é o *build* — compilar e vincular —, feita por máquinas.
:::

### Consequências importantes

1. **Fabricar software é quase de graça.** Um computador, um compilador e alguns segundos (ou minutos): o *build* custa quase nada. O que é **caro** é **projetar** — e aí o código inteiro é projeto.
2. **Projetos de software ficam enormes e complexos muito rápido.** Como projetar é barato e rápido *em comparação com construir hardware*, os sistemas crescem aos milhares, centenas de milhares, milhões de linhas, e **evoluem constantemente**. O trabalho central é **gerenciar complexidade**.
3. **Testar e depurar são engenharia.** Engenheiros de pontes calculam, simulam e testam **maquetes** antes de construir. Nós, como construir é barato, **construímos o próprio produto** e o testamos: o ciclo *construir/testar* **é** a validação do projeto. Não é "limpeza de erros de fabricação" — é **refinamento do projeto**.
4. **Retrabalho é natural.** Ninguém espera que um projetista acerte o desenho na primeira tentativa. Não faz sentido tratar o programador como "pedreiro" que deveria acertar de primeira — e culpá-lo por cada defeito é contraproducente. **Refatorar** e **reescrever** são parte do processo criativo.
5. **Todos os níveis do projeto se influenciam.** Não existe "alto nível" que se possa fechar sem olhar o código: a escolha de um algoritmo num módulo pode decidir o sucesso do sistema inteiro. Por isso **separar rigidamente** "projeto de alto nível" (congelado) de "codificação" (mecânica) **não funciona** — é o erro da Cinza S.A.
6. **Os diagramas continuam úteis — como esboços.** São ótimos para **comunicar e explorar** ideias (como nos capítulos de UML), mas **não substituem** o código, nem precisam estar "sincronizados" a qualquer custo.
7. **Linguagens que facilitam projetar e programar ao mesmo tempo** (como C++ em seu tempo, e hoje C#) **vencem**: o código bem escrito é **legível como documento** de projeto.

::: figura Engenharia tradicional × software
<svg viewBox="0 0 720 220" role="img" aria-label="Em engenharia tradicional: projeto vira documento e a fabricação é cara; em software: o código é o projeto e a fabricação (build) é barata">
  <text x="170" y="22" text-anchor="middle" font-weight="800">Engenharia tradicional</text>
  <rect class="f-info" x="20" y="40" width="130" height="60" rx="10"/><text x="85" y="66" text-anchor="middle" font-weight="800">Projeto</text><text x="85" y="86" text-anchor="middle" font-size="11" class="t-dim">caro, rigoroso</text>
  <path class="arrow" d="M152 70H190"/>
  <rect class="f-plain" x="192" y="40" width="130" height="60" rx="10"/><text x="257" y="66" text-anchor="middle" font-weight="800">Documentos</text><text x="257" y="86" text-anchor="middle" font-size="11" class="t-dim">plantas, desenhos</text>
  <path class="arrow" d="M170 118V150" style="opacity:0"/>
  <rect class="f-warn" x="106" y="140" width="130" height="60" rx="10"/><text x="171" y="166" text-anchor="middle" font-weight="800">Fabricação</text><text x="171" y="186" text-anchor="middle" font-size="11" class="t-dim">muito cara</text>
  <path class="arrow" d="M257 102Q250 120 220 140"/>
  <text x="540" y="22" text-anchor="middle" font-weight="800">Software</text>
  <rect class="f-info" x="400" y="40" width="130" height="60" rx="10"/><text x="465" y="66" text-anchor="middle" font-weight="800">Projeto</text><text x="465" y="86" text-anchor="middle" font-size="11" class="t-dim">= código-fonte</text>
  <path class="arrow" d="M532 70H570"/>
  <rect class="f-good" x="572" y="40" width="130" height="60" rx="10"/><text x="637" y="66" text-anchor="middle" font-weight="800">Build</text><text x="637" y="86" text-anchor="middle" font-size="11" class="t-dim">quase de graça</text>
  <path d="M572 130Q465 190 465 102" stroke="var(--ink-2)" stroke-dasharray="5 4" fill="none" marker-end="url(#arr2)"/><text x="520" y="178" text-anchor="middle" font-size="11" class="t-dim">testar e refinar</text>
</svg>
:::

### E o que isso significa para você?

| A ideia | Na prática |
|---|---|
| Código é projeto | Escreva código **para ser lido**: nomes bons, funções pequenas, abstrações claras (capítulos de SOLID e refatoração) |
| Build é barato | **Rode o build e os testes o tempo todo** (integração contínua) — é seu "túnel de vento" |
| Testar é refinar o projeto | **TDD** e testes de aceitação **fazem parte do projeto**, não são "uma fase" |
| Complexidade é o inimigo | **Princípios** (SRP, OCP, DIP…) e **padrões** existem para gerenciar complexidade |
| Retrabalho é natural | **Refatore** sempre; não espere o projeto "perfeito" na primeira vez |
| Os níveis se influenciam | Faça o **projeto evoluir** junto com o código; use diagramas como esboços |

::: pense Software como "sistema orgânico"
Especificações mudam, equipes mudam, o mundo muda: o software se parece **mais com um sistema social/orgânico** do que com uma ponte. Por isso, **ritmo sustentável**, **feedback frequente** e **comunicação** valem tanto quanto qualquer técnica.
:::

::: resumo
- **Fábula:** processos que impõem prazos sem dados, medem progresso por artefatos e proíbem o retorno entre fases **fracassam**; práticas ágeis dão **feedback curto**, **qualidade contínua** e **ritmo sustentável**.
- A **Verde** estima, entrega em iterações curtas, automatiza testes de aceitação, refatora e comunica; a **Cinza** mede linhas de código e acumula horas extras.
- **Tese:** o **código-fonte é o projeto de software**; **build** (compilar) é a fabricação e é quase gratuita.
- Consequências: o caro é **projetar**; **testar e depurar** são parte do projeto; **retrabalho** é natural; **todos os níveis** de projeto se influenciam; diagramas são **esboços**.
- Na prática: código legível, **CI + TDD**, **refatoração contínua**, princípios e padrões para **gerenciar complexidade**.
:::

::: quiz
? Segundo a tese deste apêndice, qual artefato é o "projeto" do software?
+ O código-fonte
- O documento de requisitos
- O diagrama de classes
- O manual do usuário
> É o único artefato completo e preciso o bastante para ser "fabricado" (compilado) automaticamente.

? Por que "fabricar" software é quase gratuito?
+ Porque o build (compilar e vincular) é automático, rápido e barato
- Porque não se testa
- Porque não há projeto
- Porque compiladores escrevem o código
> O custo está em projetar, não em construir.

? Qual o papel do teste e da depuração nessa visão?
+ Parte da validação e refinamento do projeto
- Só limpar erros de fabricação
- Uma fase opcional
- Substituir o projeto
> Como construir é barato, construímos e testamos o próprio produto.

? Qual o problema de congelar o "projeto de alto nível" antes de codificar?
+ Detalhes do código influenciam o projeto de alto nível; ignorá-los leva a projetos ruins
- Nenhum
- Aumenta a velocidade
- Impede o uso de diagramas
> Todos os níveis do projeto se influenciam.

? Na fábula, por que a Verde obteve melhores resultados?
+ Feedback curto, testes automatizados, ritmo sustentável e estimativas baseadas em dados
- Porque tinha mais pessoas
- Porque usava ferramentas mais caras
- Porque trabalhava mais horas
> A Cinza mediu linhas de código e multiplicou horas extras.

? Como usar diagramas segundo esta visão?
+ Como esboços para comunicar e explorar ideias, sem exigir sincronização a qualquer custo com o código
- Como substituto do código
- Nunca usar
- Gerar todo o código a partir deles
> Úteis, mas não são o projeto final.
:::

::: cartas
Código = projeto :: O código-fonte é o documento que descreve completamente o software.
Build :: Processo de compilar e vincular (a "fabricação" do software); barato.
Iteração :: Período curto (1–3 semanas) com planejamento, desenvolvimento e demonstração.
Velocidade :: Quanto de trabalho a equipe consegue concluir por iteração.
Ritmo sustentável :: Jornada que a equipe pode manter indefinidamente, sem exaustão.
Teste de aceitação :: Verificação de que uma história atende ao que o cliente espera.
Integração contínua :: Integrar e testar o código várias vezes ao dia.
Complexidade :: Principal desafio do desenvolvimento de software, gerida por princípios e padrões.
:::
