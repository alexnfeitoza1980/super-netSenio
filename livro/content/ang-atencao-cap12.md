---
id: ang-atencao-cap12
order: 334
kind: capitulo
num: 236
group: Angular · Parte 58 · As oito semanas
title: "Semana 8 · Seguir praticando"
subtitle: A oitava semana é o resto da vida. Testes como hábito, tempo injetado, um bug que só os testes viram, e o que manter depois do livro.
minutes: 70
hue: 130
objectives:
- Entender a proposta da oitava semana: escolher a prática que fica, sem buscar perfeição
- Organizar testes que **servem de documentação**: nomes como frases, *Arrange / Act / Assert*, tabelas com `it.each`
- Testar código que depende do **tempo** injetando um relógio, e usar **espiões** (`vi.fn`)
- Ver um **bug real** que só o teste pegou
- Montar uma rotina de manutenção: `ng test`, `ng build`, `ng update` e integração contínua
origem: angular/livros/01-atencao-plena-angular/content/13-cap12.md
---

::: cena A sequência perfeita
Beto propôs uma funcionalidade nova para o Diário: "dias seguidos de prática", com uma chama que apaga quando a pessoa falha um dia. Marta franziu a testa. "Quem perdeu um dia porque o filho ficou doente vai abrir o app e ver uma chama apagada. Isso ajuda a voltar, ou a desistir?" Bia sugeriu contar outra coisa: **quantas vezes a pessoa voltou**.
:::

::: ilustracao img/ang-atencao/cap12.svg | Um caminho com pausas no meio. O que se conta não são os dias sem falhar, e sim as vezes em que se voltou ao caminho.
:::

## O que o livro diz

A oitava semana, dizem os autores, **é o resto da sua vida**. O convite é escolher uma prática para manter (a que fez mais sentido, mesmo que curta) e lembrar **por que** ela importa para você, porque haverá dias em que ela não vai acontecer. A atenção plena não é uma meta a atingir nem uma sequência a manter: é algo a que se **volta**, quantas vezes for preciso, sem se cobrar perfeição.

O livro termina lembrando que a prática não promete uma vida sem dificuldades. Promete uma forma diferente de estar com elas.

Nas nossas palavras: **o hábito que fica não é o que nunca falha; é o que a gente retoma.**

## Da vida para o código

| Na vida | No Angular |
|---|---|
| A prática que fica | Os **testes**: o hábito que mantém o código saudável |
| Lembrar por que pratica | Testes com **nomes que são frases**: dizem o que foi prometido |
| Voltar depois de uma pausa | Rodar os testes **sempre**, e consertar quando quebram |
| Não contar "dias perdidos" | Um modelo de dados que **conta retornos**, e não falhas |
| O tempo passa para todo mundo | **Relógio injetado**: o teste controla o tempo |

## 1) Contar retornos, e não falhas

<<< src/app/capitulos/cap12/retornos.ts

- **`retrospecto`** recebe datas (em qualquer ordem, com repetições), descarta as inválidas, conta os **dias distintos** e quantas vezes a prática **recomeçou depois de pelo menos um dia sem ela**.
- **`convite`** escolhe uma frase a partir do retrospecto e do dia de hoje. Nenhuma frase fala em "perdeu", "falhou" ou "quebrou", e há um teste garantindo isso.
- **`ehDataValida`** tem uma história, contada logo abaixo.

## 2) Um bug que só o teste viu

A primeira versão de `retrospecto` validava datas assim:

```ts
/^\d{4}-\d{2}-\d{2}$/.test(d) && !Number.isNaN(Date.parse(d))
```

Parece correto. O teste com a data `'2026-02-30'` falhou: o JavaScript **aceita** 30 de fevereiro e a transforma, sem avisar, em 2 de março. Uma data impossível virava um dia de prática que nunca existiu.

A correção é conferir a **ida e a volta**: converter para data, converter de novo para texto e ver se deu a mesma coisa. Isso só foi descoberto porque alguém escreveu um teste com um caso estranho. É para isso que eles servem.

::: regra Teste as bordas
Datas impossíveis, listas vazias, textos com espaços, números negativos, `undefined` vindo da rota (laboratório 1), formato inesperado do servidor (capítulo 9). Os bugs moram nas bordas, e os casos "normais" quase sempre funcionam.
:::

## 3) Um arquivo de testes que serve de modelo

<<< src/app/capitulos/cap12/testes.spec.ts

O comentário no topo resume as convenções que o livro seguiu do começo ao fim:

1. **Nomes que são frases.** "cada recomeço depois de pelo menos um dia sem prática conta como retorno" lê-se como uma regra do produto. Quando falhar, o nome já diz o que foi prometido.
2. **Arrange / Act / Assert.** Preparar, agir, conferir, de preferência com uma linha em branco entre as partes.
3. **Tabelas com `it.each`** quando o mesmo comportamento vale para vários casos.
4. **Dependências trocadas por injeção.** O `LembreteDePratica` pede `inject(RELOGIO)`; o teste entrega um relógio parado às 21h de 10 de novembro.
5. **Um comportamento por teste.** Se o teste tem "e" demais no nome, talvez sejam dois.

O serviço testado:

<<< src/app/capitulos/cap12/lembrete.ts

E **`vi.fn(...)`** cria um **espião**: uma função que registra como foi chamada. `expect(relogio).toHaveBeenCalled()` confere que o serviço realmente usou o relógio injetado, e não um `new Date()` escondido.

::: csharp Do xUnit para o Vitest
`describe` agrupa como uma classe de testes; `it` é o `[Fact]`; `it.each` é o `[Theory]` com `[InlineData]`; `vi.fn()` faz o papel de um *mock* do Moq ou NSubstitute; `beforeEach` é o construtor da classe de teste. E `TestBed.configureTestingModule({ providers })` é como montar o `ServiceCollection` do teste.
:::

## 4) A pirâmide, na prática

| Tipo | Exemplo no livro | Quantidade |
|---|---|---|
| **Funções puras** | `formatarTempo`, `retrospecto`, `classificar`, `resumir` | Muitos: rápidos e precisos |
| **Serviços** | `DiarioStore`, `FluxoDePensamentos`, `LembreteDePratica` | Vários: com dependências falsas |
| **Componentes** | `Pausa`, `ExploracaoDoCorpo`, `RegistroGentil` | Vários: pelo lado de fora (clicar, digitar, ler) |
| **Rotas e integração** | `rotas.spec.ts`, as páginas do Diário | Alguns: o caminho completo |
| **De ponta a ponta** (navegador real) | Fora do escopo do livro (Playwright, Cypress) | Poucos: os fluxos principais |

## 5) Manutenção: a prática que fica

Uma rotina simples mantém um projeto Angular saudável:

::: terminal Os comandos do dia a dia
```bash
npm run test:ci          # todos os testes, uma vez
npm run build            # compila em modo produção (confere templates e budgets)
npx ng update            # mostra o que pode ser atualizado
npx ng update @angular/core @angular/cli   # atualiza e aplica as migrações automáticas
```
:::

- **`ng update`** não só troca versões: aplica **migrações** que reescrevem o código para a API nova. Há também migrações que você roda quando quiser, como a que troca `*ngIf` e `*ngFor` por `@if` e `@for` (`npx ng generate @angular/core:control-flow`).
- O Angular lança uma versão **maior** a cada seis meses, com 18 meses de suporte. Atualizar uma versão por vez é bem mais fácil que pular três.
- A **integração contínua** roda testes e build a cada mudança. Este repositório tem um *workflow* do GitHub Actions que faz exatamente isso para o livro e para o código (veja o `README` da coleção).

::: pratica Escolher a prática que fica
Das práticas do livro (um minuto, a pausa de três passos, a exploração do corpo, sons e pensamentos, a gentileza, o balanço do dia), qual fez mais sentido para você? Escolha **uma** e **um momento** do dia para ela. Escreva em algum lugar por que ela importa para você. Nos dias em que não acontecer, a próxima oportunidade é sempre agora.
:::

::: exercicio Um teste que faltou
`convite` trata pausas de 0, 1 e mais dias. Mas e se a última prática estiver **no futuro** (relógio errado do computador, por exemplo)? Escreva o teste, veja o que acontece e decida qual deve ser o comportamento.

--- resposta
```ts
it('prática no futuro (relógio adiantado) é tratada como hoje', () => {
  expect(convite(retrospecto(['2026-11-12']), '2026-11-10')).toContain('praticou hoje');
});
```

O teste **passa** com o código atual: `pausa` fica negativa (-2), e a primeira condição é `pausa <= 0`. Foi uma escolha consciente, e agora ela está **documentada** num teste. Se um dia alguém trocar `<=` por `===`, o teste avisa. Às vezes um teste não acha um bug; ele registra uma decisão.
:::

::: resumo
- A oitava semana é **o resto da vida**: escolher a prática que fica e voltar a ela quantas vezes for preciso, sem buscar perfeição.
- Modelar o produto com cuidado também é atenção: **contar retornos**, e não falhas.
- Testes de borda pegam bugs reais: `Date.parse('2026-02-30')` não é `NaN`; a validação certa confere ida e volta.
- Bons testes têm **nomes que são frases**, seguem **Arrange / Act / Assert**, usam **`it.each`** para tabelas e um comportamento por teste.
- Injete o **tempo** (`RELOGIO`) e use **espiões** (`vi.fn`) para conferir como as dependências foram usadas.
- Mantenha a rotina: **`test:ci`**, **`build`**, **`ng update`** a cada versão e **integração contínua**.
:::

::: quiz
? Segundo os autores, o que é a oitava semana?
+ O resto da sua vida: manter uma prática escolhida e voltar a ela quantas vezes for preciso
- A semana mais difícil do programa
- Uma revisão de todas as práticas em um dia
- O fim da prática
> Não é uma meta a atingir, e sim algo a que se volta.

? Por que o Diário conta "retornos" em vez de "dias seguidos"?
+ Porque contar falhas desestimula quem teve uma pausa; contar retornos valoriza voltar
- Porque é mais fácil de calcular
- Porque datas não podem ser comparadas
- Porque o Angular não suporta sequências
> É uma decisão de produto feita com atenção a quem usa.

? O que o teste com `'2026-02-30'` revelou?
+ Que `Date.parse` aceita a data impossível e a transforma em outra, sem dar `NaN`
- Que o JavaScript não tem datas
- Que o teste estava errado
- Que fevereiro tem 30 dias
> A correção confere a ida e a volta da conversão.

? O que `vi.fn()` cria?
- Um componente falso
+ Um espião: uma função que registra como foi chamada
- Um relógio
- Um teste novo
> Com ele, dá para conferir `toHaveBeenCalled`, os argumentos, quantas vezes...

? Qual é o equivalente do `[Theory]` com `[InlineData]` do xUnit?
- `describe`
- `beforeEach`
+ `it.each`
- `vi.fn`
> Uma tabela de casos para o mesmo teste.

? O que `ng update` faz além de trocar versões?
+ Aplica migrações automáticas que adaptam o código à API nova
- Apaga os testes antigos
- Publica o app
- Nada além disso
> Atualizar uma versão maior por vez deixa as migrações simples.
:::

::: cartas
Semana oito :: O resto da vida: escolher a prática que fica e voltar a ela sem buscar perfeição.
Teste de borda :: Caso no limite (vazio, inválido, impossível); é onde os bugs costumam morar.
Arrange / Act / Assert :: Preparar, agir e conferir: a estrutura de um teste legível.
it.each :: Roda o mesmo teste para cada linha de uma tabela de casos.
vi.fn :: Espião do Vitest: registra chamadas e argumentos.
Relógio injetado :: Dependência de tempo (`RELOGIO`) que o teste substitui por um valor fixo.
ng update :: Atualiza o Angular e aplica migrações automáticas no código.
Integração contínua :: Rodar testes e build automaticamente a cada mudança.
:::
