---
id: agil-cap16
order: 31
kind: capitulo
num: 16
group: C# · Parte 7 · Desenvolvimento ágil
title: Práticas ágeis e Programação Extrema
subtitle: O Manifesto Ágil, os 12 princípios e as práticas da XP — por que equipes bem-sucedidas trocam processos pesados por ciclos curtos, testes e conversa.
minutes: 55
hue: 95
objectives:
- Explicar o que é **inflação de processo** e por que o movimento ágil nasceu
- Recitar os **4 valores** do Manifesto Ágil e agrupar os **12 princípios**
- Descrever as principais **práticas da Programação Extrema (XP)**
- Aplicar as ideias de **planejamento por horizontes** e **projeto simples**
origem: livros/02-projeto-agil-csharp/content/30-cap16.md
---

::: ilustracao img/agil/cap16.svg | Ciclos curtos com feedback: planejar, programar em par, testar e entregar, de novo.
:::

## De onde vem a "agilidade"

Quem já passou por um projeto **sem nenhuma prática** sabe como é: prazos estourados, erros que se repetem, clientes frustrados, programadores desanimados. A reação natural é criar **regras**: "da próxima vez teremos um documento para isso, uma aprovação para aquilo". Cada erro gera mais uma regra, mais um papel a preencher.

Com o tempo surge a **inflação de processo**: um processo tão grande e desajeitado que, em vez de evitar os problemas, **os causa** — atrasa a equipe e a deixa lenta demais para reagir. O pior: muitas equipes concluem que "falta processo" e... aumentam ainda mais o processo.

::: figura O círculo vicioso da inflação de processo
<svg viewBox="0 0 700 250" role="img" aria-label="Ciclo: erro, nova regra, equipe mais lenta, mais erros">
  <g text-anchor="middle" font-size="14">
    <rect class="f-bad" x="10" y="90" width="150" height="70" rx="14"/><text x="85" y="120" font-weight="800" class="t-bad">Projeto dá errado</text><text x="85" y="140" font-size="12" class="t-dim">atrasos, erros</text>
    <rect class="f-warn" x="200" y="15" width="170" height="70" rx="14"/><text x="285" y="45" font-weight="800" class="t-warn">Cria-se uma regra</text><text x="285" y="65" font-size="12" class="t-dim">mais documentos, mais aprovações</text>
    <rect class="f-prim" x="410" y="90" width="150" height="70" rx="14"/><text x="485" y="120" font-weight="800" class="t-prim">Processo pesado</text><text x="485" y="140" font-size="12" class="t-dim">equipe lenta e rígida</text>
    <rect class="f-info" x="200" y="165" width="170" height="70" rx="14"/><text x="285" y="195" font-weight="800" class="t-info">Entrega errado</text><text x="285" y="215" font-size="12" class="t-dim">tarde e fora do que o cliente quer</text>
  </g>
  <path class="arrow" d="M120 90C150 55 170 50 198 50"/><path class="arrow" d="M372 50C410 55 440 70 470 88"/><path class="arrow" d="M490 162C470 200 420 205 372 200"/><path class="arrow" d="M198 200C150 200 110 185 90 162"/>
</svg>
:::

## A Aliança Ágil e o Manifesto

No início de 2001, um grupo de especialistas se reuniu para esboçar **valores** que permitissem às equipes desenvolver rápido e **responder a mudanças**. O resultado, o **Manifesto para o Desenvolvimento Ágil de Software**, é uma declaração de prioridades: mesmo havendo valor nos itens da direita, **valorizamos mais os da esquerda**.

| Valorizamos mais… | …do que |
|---|---|
| **Indivíduos e interações** | processos e ferramentas |
| **Software em funcionamento** | documentação abrangente |
| **Colaboração com o cliente** | negociação de contratos |
| **Resposta a mudanças** | seguir um plano |

### Indivíduos e interações

As **pessoas** são o ingrediente mais importante. Um bom processo não salva uma equipe fraca, mas um processo ruim paralisa até uma equipe ótima. "Competente" aqui não significa gênio: significa alguém que **se comunica bem**. Programadores comuns que conversam vencem celebridades que não funcionam como equipe.

Ferramentas ajudam, mas **comece pequeno**: use o controle de versão gratuito antes de comprar o caro, o quadro de avisos antes da ferramenta CASE, arquivos simples antes do banco gigante. E lembre: **montar a equipe vem antes de montar o ambiente**.

### Software funcionando

Sem documentação, o software vira um desastre — mas **documentação demais é pior**: leva tempo para escrever, mais tempo para manter e, se envelhece, vira **mentira detalhada**. Ideal: um documento curto (10 a 20 páginas) com o *porquê* e a estrutura de alto nível. Os melhores "documentos" para novatos são **o código e a equipe**: o código não mente sobre o que faz, e a equipe carrega o mapa vivo do sistema.

::: regra Primeira Lei da Documentação
**Não produza um documento a não ser que sua necessidade seja imediata e significativa.**
:::

### Colaboração com o cliente

Software **não se compra como mercadoria**: não dá para descrever tudo, fechar prazo e preço e esperar o resultado. Projetos bem-sucedidos têm **feedback frequente**. O melhor contrato não lista requisitos: **governa como equipe e cliente trabalharão juntos** — por exemplo, pagando por blocos de funcionalidade que passam nos **testes de aceitação** do cliente, definidos ao longo do caminho.

### Resposta a mudanças

Planos mudam: o negócio muda, o cliente muda de ideia ao ver o sistema, e nós somos péssimos em estimar. O gráfico de Gantt que parece dar controle logo **degrada na forma**, não só nas datas. A alternativa: **planejar em horizontes**.

::: figura Planejamento em horizontes
<svg viewBox="0 0 720 230" role="img" aria-label="Plano detalhado para a próxima semana, aproximado para três meses, vago para um ano">
  <rect class="f-good" x="10" y="150" width="220" height="70" rx="12"/><text x="120" y="180" text-anchor="middle" font-weight="800" class="t-good">Próxima semana</text><text x="120" y="202" text-anchor="middle" font-size="12" class="t-dim">tarefas individuais, detalhadas</text>
  <rect class="f-info" x="250" y="95" width="230" height="125" rx="12"/><text x="365" y="130" text-anchor="middle" font-weight="800" class="t-info">Próximos 3 meses</text><text x="365" y="152" text-anchor="middle" font-size="12" class="t-dim">requisitos em linhas gerais</text>
  <rect class="f-plain" x="500" y="40" width="210" height="180" rx="12" stroke-dasharray="6 5"/><text x="605" y="80" text-anchor="middle" font-weight="800">Daqui a 1 ano</text><text x="605" y="102" text-anchor="middle" font-size="12" class="t-dim">só uma vaga ideia</text>
  <path class="arrow" d="M10 25H700" style="stroke:var(--ink-3)"/><text x="360" y="18" text-anchor="middle" font-size="12" class="t-dim">precisão do plano diminui com a distância</text>
</svg>
:::

Investimos em plano detalhado **apenas** no que é imediato — e como ele governa só uma semana, o resto permanece flexível.

## Os 12 princípios, em 4 grupos

::: figura Os princípios agrupados
<svg viewBox="0 0 740 260" role="img" aria-label="Quatro grupos de princípios">
  <g text-anchor="middle" font-size="13">
    <rect class="f-good" x="5" y="10" width="175" height="240" rx="14"/><text x="92" y="38" font-weight="800" class="t-good" font-size="15">Entrega</text><text x="92" y="68" class="t-dim">1 · Satisfazer o cliente</text><text x="92" y="88" class="t-dim">com entregas contínuas</text><text x="92" y="118" class="t-dim">3 · Software funcionando</text><text x="92" y="138" class="t-dim">a cada poucas semanas</text><text x="92" y="168" class="t-dim">7 · Software funcionando</text><text x="92" y="188" class="t-dim">é a medida do progresso</text>
    <rect class="f-info" x="195" y="10" width="175" height="240" rx="14"/><text x="282" y="38" font-weight="800" class="t-info" font-size="15">Mudança</text><text x="282" y="68" class="t-dim">2 · Mudar requisitos é</text><text x="282" y="88" class="t-dim">bem-vindo, até tarde</text><text x="282" y="118" class="t-dim">4 · Negócio e devs juntos</text><text x="282" y="138" class="t-dim">todos os dias</text>
    <rect class="f-warn" x="385" y="10" width="175" height="240" rx="14"/><text x="472" y="38" font-weight="800" class="t-warn" font-size="15">Pessoas</text><text x="472" y="68" class="t-dim">5 · Indivíduos motivados</text><text x="472" y="98" class="t-dim">6 · Conversa face a face</text><text x="472" y="128" class="t-dim">8 · Ritmo sustentável</text><text x="472" y="158" class="t-dim">11 · Equipes auto-</text><text x="472" y="178" class="t-dim">organizadas</text>
    <rect class="f-prim" x="575" y="10" width="160" height="240" rx="14"/><text x="655" y="38" font-weight="800" class="t-prim" font-size="15">Qualidade</text><text x="655" y="68" class="t-dim">9 · Excelência técnica e</text><text x="655" y="88" class="t-dim">bom projeto</text><text x="655" y="118" class="t-dim">10 · Simplicidade: maximizar</text><text x="655" y="138" class="t-dim">o trabalho não feito</text><text x="655" y="168" class="t-dim">12 · Refletir e ajustar</text><text x="655" y="188" class="t-dim">em intervalos regulares</text>
  </g>
</svg>
:::

Alguns merecem destaque:

- **Entregue cedo e sempre.** Estudos sobre desenvolvimento de produtos mostram que quanto **mais cedo e mais frequente** a entrega, **maior a qualidade final**.
- **Mudança é boa notícia.** Significa que a equipe aprendeu mais sobre o que o cliente precisa. Por isso mantemos a estrutura do software **flexível** — o tema da Parte 8.
- **Maratona, não 50 metros.** Correr demais leva a atalhos, esgotamento e fracasso.
- **Qualidade é o segredo da velocidade.** Não se faz bagunça "para limpar depois": limpa-se quando se suja.
- **Simplicidade** é a arte de maximizar o trabalho **que não precisou ser feito**.

::: pense Seu projeto mede progresso pelo quê?
Pela fase em que está, pelo volume de documentos, pelas linhas de código de infraestrutura? Para os ágeis, só vale **software funcionando que atende a necessidade do cliente**: se 30% da funcionalidade necessária funciona, o projeto está 30% pronto.
:::

## Programação Extrema (XP): as práticas, uma a uma

O Manifesto diz *o quê*; a **XP** dá *como*. É um conjunto de práticas simples e concretas que se reforçam entre si.

::: figura Mapa das práticas da XP
<svg viewBox="0 0 760 330" role="img" aria-label="Práticas da XP organizadas em três círculos">
  <ellipse cx="380" cy="165" rx="360" ry="150" class="f-plain" stroke-dasharray="6 5"/><text x="380" y="40" text-anchor="middle" font-weight="800" class="t-dim">Práticas do negócio</text>
  <g text-anchor="middle" font-size="12"><text x="150" y="66">Equipe coesa</text><text x="380" y="66">Jogo do planejamento</text><text x="610" y="66">Histórias · Ciclos curtos</text></g>
  <ellipse cx="380" cy="190" rx="270" ry="105" class="f-info"/><text x="380" y="110" text-anchor="middle" font-weight="800" class="t-info">Práticas da equipe</text>
  <g text-anchor="middle" font-size="12"><text x="230" y="140">Ritmo sustentável</text><text x="380" y="140">Área de trabalho aberta</text><text x="530" y="140">Metáfora</text><text x="290" y="162">Integração contínua</text><text x="470" y="162">Posse coletiva</text></g>
  <ellipse cx="380" cy="220" rx="170" ry="60" class="f-prim"/><text x="380" y="190" text-anchor="middle" font-weight="800" class="t-prim">Práticas técnicas</text>
  <g text-anchor="middle" font-size="12"><text x="380" y="212">TDD · Programação em pares</text><text x="380" y="232">Refatoração · Projeto simples</text><text x="380" y="252">Testes de aceitação</text></g>
</svg>
:::

### Práticas do negócio

**Equipe coesa.** Cliente, gerentes e programadores trabalham **juntos**. O "cliente" é quem define e prioriza funcionalidades — um analista, um representante dos usuários, quem paga. O ideal é na mesma sala; o segundo melhor é a poucos minutos de distância.

**Histórias de usuário.** Para planejar, basta saber **o suficiente para estimar**. O cliente escreve algumas palavras numa **ficha** que funciona como lembrete de uma **conversa em andamento**; os desenvolvedores anotam a estimativa. Os detalhes só se capturam quando a história está para ser implementada — antes disso mudariam de qualquer forma.

**Ciclos curtos.** Entrega de software funcionando a cada **duas semanas** (a *iteração*), demonstrada aos interessados. A cada ~3 meses (seis iterações) há uma **entrega** (*release*) que normalmente pode ir para produção.

- **Plano de iteração:** histórias escolhidas pelo cliente dentro de um **orçamento** — o quanto a equipe conseguiu fazer na iteração anterior (a *velocidade*). Iniciada a iteração, o negócio **não muda** as histórias dela.
- **Plano de entrega:** coleções de histórias priorizadas, também limitadas pelo orçamento. Pode mudar a qualquer momento — mas a iteração em curso, não.

**Jogo do planejamento.** Divisão clara de responsabilidade: **o negócio decide a importância**; **os desenvolvedores estimam o custo**. Sem trapaça de nenhum dos lados, em poucas iterações todos conhecem a velocidade real da equipe — e o cliente consegue prever prazo e custo.

**Testes de aceitação.** Os detalhes de cada história são escritos como **testes executáveis**, definidos pelo cliente (com analistas e testadores), antes ou durante a implementação. Tornam-se o **verdadeiro documento de requisitos**. Depois que passa, um teste **nunca mais pode falhar**: roda a cada construção do sistema.

### Práticas da equipe

**Ritmo sustentável.** Sem horas extras (exceto, talvez, na última semana de uma entrega). Equipe esgotada comete erros.

**Área de trabalho aberta.** Uma sala comum, pares ao alcance do ouvido, paredes com gráficos e diagramas. Parece barulhento, mas todos percebem quando alguém trava — e a comunicação flui.

**Posse coletiva.** Qualquer par pode abrir **qualquer módulo** e melhorá-lo. Há especialistas, mas ninguém "é dono" de uma parte; todos transitam por interface, banco, camada intermediária.

**Integração contínua.** Registre e integre **várias vezes por dia**. Antes de registrar, rode todos os testes. A regra: *quem registra primeiro vence; os outros mesclam*. A equipe **constrói o sistema inteiro**, de ponta a ponta, várias vezes ao dia.

**Metáfora.** A prática mais difícil de definir: é a **visão global** que dá sentido às peças — o desenho da caixa do quebra-cabeça. Muitas vezes se reduz a um **vocabulário comum** de nomes que ajuda a achar o lugar de cada coisa.

### Práticas técnicas

**Programação em pares.** Duas pessoas, um computador: um **digita**, o outro **observa** (erros, melhorias, estrutura). Trocam de papel o tempo todo e **trocam de parceiro** pelo menos uma vez por dia. Espalha conhecimento e reduz falhas.

**Desenvolvimento guiado por testes (TDD).** Todo código de produção é escrito para **fazer passar um teste que falhava**. O ciclo teste → código dura cerca de um minuto. O resultado: uma suíte completa de testes e um projeto naturalmente **menos acoplado** (código testável precisa ser independente). Veja o capítulo 17 e o Laboratório 4.

**Refatoração.** Pequenas transformações que melhoram a estrutura **sem mudar o comportamento**, com os testes rodando a cada passo, feitas **continuamente** — a cada hora, não "no fim do projeto". Veja o capítulo 18.

**Projeto simples.** Faça o projeto o mais simples e expressivo que atenda as histórias **atuais**; evolua-o a cada iteração. Três "mantras":

1. **Pense na coisa mais simples que possa funcionar.** (Dá para resolver com arquivos antes de comprar um banco de dados.)
2. **Você não vai precisar disso (YAGNI).** Não coloque infraestrutura "por precaução": só com evidência de que será mais barato fazer agora do que esperar.
3. **Uma vez e somente uma vez.** Duplicação é eliminada onde quer que apareça — o que força a criar abstrações e reduz o acoplamento.

::: passos Uma iteração de duas semanas
**Dia 1 — planejamento.** Os desenvolvedores informam o orçamento (velocidade da iteração anterior). O cliente escolhe histórias que caibam nele; a equipe as divide em tarefas.
===
**Dias 2 a 9 — construção.** Pares escolhem tarefas. Para cada uma: escrevem um teste que falha, fazem-no passar, refatoram, integram e rodam **todos** os testes. Os testes de aceitação das histórias vão sendo escritos e executados.
===
**Dia 10 — demonstração.** O software funcionando é mostrado aos interessados. O cliente dá feedback.
===
**Retrospectiva.** A equipe discute o que melhorar no jeito de trabalhar (princípio 12) e mede a nova velocidade, que dimensiona a próxima iteração.
:::

## E hoje?

Os autores observam que a maioria das equipes ágeis bem-sucedidas **mistura** abordagens (Scrum, Crystal, FDD, ASD, XP) e tende a uma combinação: **Scrum** para gerenciar várias equipes e **XP** como práticas de engenharia dentro delas. Para quem programa em C#, a mensagem prática é: **testes, integração frequente, refatoração e design simples** são competências técnicas; o resto é organização para que elas funcionem.

::: exercicio Aplique os mantras
Sua equipe vai criar um programa que cadastra clientes. Um colega propõe: *"vamos já montar um banco de dados SQL com camada de acesso genérica, caso um dia tenhamos muitos usuários"*. Que mantra você invocaria e o que proporia no lugar?

--- resposta
**"Você não vai precisar disso"** e **"pense na coisa mais simples que possa funcionar"**. Comece gravando os clientes num arquivo (ou em memória), por trás de uma **interface** simples (como `IRepositorioClientes`). Se, mais adiante, uma história realmente exigir um banco, troque a implementação — o resto do sistema não percebe. Você paga pela infraestrutura **só quando precisar**.
:::

::: resumo
- **Inflação de processo**: regras demais tornam a equipe lenta e rígida.
- O **Manifesto Ágil** valoriza: pessoas/interações, software funcionando, colaboração com o cliente e resposta a mudanças.
- 12 princípios: entrega contínua, mudança bem-vinda, pessoas motivadas e em ritmo sustentável, qualidade técnica, simplicidade e reflexão regular.
- **XP**: equipe coesa, histórias, ciclos curtos, jogo do planejamento, testes de aceitação, pares, TDD, posse coletiva, integração contínua, ritmo sustentável, sala aberta, projeto simples, refatoração, metáfora.
- Planeje **em detalhe só o curto prazo**; o orçamento de cada iteração é a **velocidade** medida.
- **Simplicidade**: faça o mais simples; não construa o que não precisa; elimine duplicação.
:::

::: quiz
? O que é "inflação de processo"?
+ Aumentar o processo (regras, documentos) a cada erro, até ele atrapalhar o trabalho
- A alta do preço das ferramentas
- O aumento do número de programadores
- Um tipo de bug de software
> O excesso de processo diminui a velocidade e a capacidade de resposta da equipe.

? Qual par de valores NÃO é do Manifesto Ágil?
- Software funcionando mais que documentação abrangente
- Resposta a mudanças mais que seguir um plano
+ Plano detalhado mais que colaboração com o cliente
- Indivíduos e interações mais que processos e ferramentas
> O manifesto prioriza a colaboração com o cliente sobre a negociação de contratos, e a resposta a mudanças sobre o plano.

? Em XP, o que é uma história de usuário?
+ Um lembrete (ficha) de uma conversa sobre um requisito, usado para planejar e estimar
- Um documento de 50 páginas de requisitos
- Um caso de teste de unidade
- O contrato com o cliente
> Os detalhes são capturados mais tarde, nos testes de aceitação.

? Quem define o orçamento de uma iteração?
+ Os desenvolvedores, com base no que fizeram na iteração anterior (velocidade)
- O cliente, conforme a sua vontade
- O gerente de projeto, pela data da entrega
- O departamento jurídico
> O cliente escolhe histórias, mas dentro desse orçamento.

? O que significa "posse coletiva" do código?
+ Qualquer par pode alterar qualquer módulo; ninguém é dono exclusivo
- O código pertence à empresa e não à equipe
- Todos devem alterar o mesmo arquivo ao mesmo tempo
- Só os seniores podem alterar módulos centrais
> Isso espalha conhecimento e evita gargalos.

? Qual é o mantra "YAGNI"?
+ Você não vai precisar disso: não construa infraestrutura sem necessidade comprovada
- Você sempre vai precisar de testes
- Você deve alterar tudo
- Nunca refatore
> Evita complexidade especulativa.

? O que a regra de integração contínua pede?
+ Integrar e construir o sistema várias vezes ao dia, rodando todos os testes
- Integrar uma vez por mês, num dia dedicado
- Integrar só no fim da iteração
- Nunca mesclar alterações
> Integrações pequenas e frequentes evitam longas sessões de mesclagem.

? Por que o TDD tende a gerar projetos menos acoplados?
+ Porque código testável precisa ser independente de outros módulos
- Porque testes apagam dependências
- Porque o compilador exige
- Porque testes são lentos
> Módulos acoplados são difíceis de testar isoladamente.
:::

::: cartas
Manifesto Ágil :: Declaração de 2001 que prioriza pessoas, software funcionando, colaboração com o cliente e resposta a mudanças.
Inflação de processo :: Crescimento de regras e artefatos que torna a equipe lenta e rígida.
Iteração :: Ciclo curto (≈2 semanas) que entrega software funcionando.
Entrega (release) :: Conjunto de iterações (≈3 meses) que normalmente pode ir para produção.
Velocidade :: Quanto a equipe entregou na iteração anterior; define o orçamento da próxima.
História de usuário :: Ficha que lembra uma conversa sobre um requisito; ferramenta de planejamento.
Teste de aceitação :: Teste executável, definido pelo cliente, que diz se uma história está pronta.
Programação em pares :: Dois programadores, um computador: um digita, o outro revisa; os papéis alternam.
TDD :: Escrever primeiro um teste que falha, depois o código que o faz passar.
Posse coletiva :: Qualquer par pode alterar qualquer parte do código.
Integração contínua :: Integrar e construir o sistema inteiro várias vezes por dia.
YAGNI :: "Você não vai precisar disso": não antecipe infraestrutura.
Metáfora (XP) :: Visão global e vocabulário comum que guia nomes e estrutura do sistema.
:::
