---
id: ang-atencao-lab02
order: 335
kind: lab
num: 48
group: Angular · Parte 58 · As oito semanas
title: "Laboratório: Diário de Atenção"
subtitle: Um app de verdade, juntando o livro inteiro: rotas sob demanda, store com signals, Signal Forms, armazenamento à prova de falhas, privacidade e testes.
minutes: 120
hue: 330
objectives:
- **Integrar** os capítulos 1 a 12 num app completo e utilizável
- Organizar estado compartilhado numa **store com signals**: uma fonte, muitas consequências
- Guardar dados no navegador com uma **abstração de armazenamento** (testável e à prova de falhas)
- Tratar **dados corrompidos** sem apagar nada que a pessoa escreveu
- Montar a **casca do app**: navegação, `routerLinkActive`, acessibilidade, rodapé de cuidado
- Testar a store e as páginas pelo lado de fora
origem: angular/livros/01-atencao-plena-angular/content/14-lab02.md
---

::: cena O app que a equipe usa
No fim das oito semanas, a equipe decidiu usar o próprio Diário: um registro curto, por dia, de uma prática de atenção. Bia listou os requisitos numa folha: "Rápido de registrar. Nada sai do navegador. Nunca perder o que alguém escreveu. E nada de culpa." Caio olhou a lista e percebeu que já tinha visto cada pedaço em algum capítulo.
:::

::: ilustracao img/ang-atencao/lab02.svg | Um caderno aberto com registros curtos, um por dia, e espaço para os dias em branco. O caderno não cobra; só guarda.
:::

## O que vamos construir

| Página | Rota | O que faz | Capítulos |
|---|---|---|---|
| Hoje | `/diario/hoje` | Registrar uma prática (prática, minutos, palavra, nota) e ver as de hoje | 4, 6, 10, 12 |
| Histórico | `/diario/historico` | Ver tudo agrupado por dia, filtrar por prática, totais e retornos | 3, 6, 11, 12 |
| Registro | `/diario/registro/:id` | Ver um registro e apagá-lo | 7 |
| Pausa | `/pausa` | O temporizador do laboratório 1 | Lab 1 |
| Demonstrações | `/capitulos` | Os componentes de cada capítulo | 7 |

Rode com `npm start` e abra `http://localhost:4200`: a raiz redireciona para **Hoje**.

## 1) O modelo: o que o Diário guarda

<<< src/app/diario/entrada.ts

- **`PRATICAS ... as const`** + **`(typeof PRATICAS)[number]`** criam o tipo `Pratica` **a partir da lista**: uma só fonte para a lista do `<select>` e para o tipo.
- **`Omit<Entrada, 'id'>`** é o tipo de uma entrada **antes** de ganhar um id.
- **`ehEntrada`** confere, campo a campo, um valor `unknown`. Dados do armazenamento podem ser antigos, editados à mão ou estar corrompidos: são boatos até serem conferidos (capítulo 9).

## 2) Onde guardar: uma abstração

<<< src/app/diario/armazenamento.ts

A store não fala com o `localStorage` diretamente: ela pede um **`Armazenamento`**. O app usa a versão com `localStorage` (que **nunca** lança exceção: modo privado, cota cheia ou armazenamento bloqueado viram "sem dados", e o app segue funcionando); os testes usam a versão em memória. É o mesmo truque da classe abstrata como token do capítulo 8.

::: cuidado Privacidade
Um diário de práticas de atenção pode conter notas pessoais. Este app guarda tudo **só no navegador**, não envia nada a servidor nenhum e não tem análise de uso. Se um dia ele sincronizar dados, isso precisa ser opcional, explícito e protegido. Lembre também que `localStorage` não é criptografado: qualquer pessoa com acesso ao computador pode lê-lo.
:::

## 3) A store: uma fonte, muitas consequências

<<< src/app/diario/diario.store.ts

Este arquivo é o capítulo 3 em tamanho real:

- **Uma fonte**: `_entradas`, um signal **privado**. Ninguém de fora consegue mudá-lo sem passar pelos métodos.
- **Muitas consequências**, todas `computed`: a lista ordenada, `hoje`, `deHoje`, `minutosTotais`, o `resumo` (o `retrospecto` do capítulo 12) e o total `porPratica`.
- **Gravar é explícito**: `adicionar` e `remover` chamam `persistir()`. Daria para gravar com um `effect` observando `_entradas`, mas assim fica claro **quando** e **por que** se grava, e os testes não dependem do agendamento de effects.
- **`adicionar` valida** com `ehEntrada` antes de mudar qualquer coisa: entrada inválida lança `RangeError` e o estado não muda.
- **`crypto.randomUUID()`** gera ids únicos no navegador, sem servidor.
- O arquivo guardado tem **`versao: 1`**. Se um dia o formato mudar, o código saberá migrar os dados antigos em vez de quebrar.

### Nunca perder o que alguém escreveu

O método `carregar` trata três situações:

1. **Nada guardado**: lista vazia.
2. **Alguns registros ilegíveis**: ficam os válidos, e um aviso conta quantos foram ignorados.
3. **Arquivo ilegível** (JSON quebrado, formato desconhecido): o app começa vazio, **mas antes guarda uma cópia** do conteúdo original numa chave separada, e avisa a pessoa.

Sem o passo 3, a primeira gravação depois de um arquivo corrompido **sobrescreveria** os dados que talvez desse para recuperar.

::: erro A ordem dos campos importa
`avisoDeCarga` é declarado **antes** de `_entradas`. Em TypeScript (e em C#), campos são iniciados na ordem em que aparecem. Como a inicialização de `_entradas` chama `carregar()`, que usa `avisoDeCarga`, invertê-los faria `carregar` encontrar `avisoDeCarga` ainda indefinido.
:::

## 4) As páginas

**Hoje**, com o formulário do capítulo 10 e o convite do capítulo 12:

<<< src/app/diario/hoje.ts

**Histórico**, com o pipe do capítulo 11 e um agrupamento em `computed`:

<<< src/app/diario/historico.ts

**Registro**, com o `:id` como input (capítulo 7) e um caminho de volta quando o registro não existe:

<<< src/app/diario/entrada-detalhe.ts

E as rotas do Diário, carregadas só quando alguém entra em `/diario`:

<<< src/app/diario/diario.routes.ts

## 5) A casca do app

<<< src/app/app.ts

- **`routerLinkActive="ativo"`** põe uma classe no link da página atual, e **`ariaCurrentWhenActive="page"`** marca o mesmo link com `aria-current="page"` para leitores de tela.
- O link **"Pular para o conteúdo"** aparece no primeiro Tab e leva direto ao `<main>`: quem navega por teclado não precisa passar pelo menu em toda página.
- O **rodapé** repete o essencial: nada aqui é terapia, os dados ficam no navegador, e onde buscar ajuda.

## 6) Testes

<<< src/app/diario/diario.spec.ts

A função `configurar` monta o ambiente de teste: armazenamento **em memória**, relógio **parado** em 10 de novembro e as rotas do Diário. Os testes da store conferem regras (ordenação, validação, remoção, dados corrompidos). Os das páginas navegam com `RouterTestingHarness`, digitam, enviam e leem a tela, como uma pessoa faria.

::: pratica Usar o que você construiu
Use o Diário por uma semana, de verdade: um registro curto por dia, mesmo que seja de um minuto. Repare no que você gostaria de mudar no app, e anote. É a melhor lista de próximos passos que existe.
:::

::: exercicio Exportar o diário
Acrescente à página Histórico um botão **Exportar** que baixa um arquivo `diario-de-atencao.json` com todas as entradas. Onde deve ficar a lógica: no componente ou na store? Como testar sem baixar arquivo nenhum?

--- resposta
A **montagem do conteúdo** é regra do Diário, então fica na store, como uma função pura de verificar:

```ts
// em DiarioStore
exportar(): string {
  return JSON.stringify({ versao: 1, exportadoEm: this.agora().toISOString(), entradas: this._entradas() }, null, 2);
}
```

O **download** é coisa de tela e de navegador, então fica no componente:

```ts
protected baixar(): void {
  const blob = new Blob([this.store.exportar()], { type: 'application/json' });
  const url = URL.createObjectURL(blob);
  const a = Object.assign(document.createElement('a'), { href: url, download: 'diario-de-atencao.json' });
  a.click();
  URL.revokeObjectURL(url);
}
```

O teste mais valioso é o da store, sem navegador nenhum:

```ts
it('exporta com versão, data e todas as entradas', () => {
  configurar();
  const store = TestBed.inject(DiarioStore);
  store.adicionar(nova());
  const arquivo = JSON.parse(store.exportar());
  expect(arquivo.versao).toBe(1);
  expect(arquivo.exportadoEm).toBe('2026-11-10T12:00:00.000Z');
  expect(arquivo.entradas.length).toBe(1);
});
```

Separar "o que gerar" (regra, testável) de "como entregar" (efeito do navegador) é o mesmo movimento de todo o livro: o modo Existente para o que **é**, o modo Atuante só onde é preciso **agir**.
:::

::: resumo
- O Diário junta o livro: **rotas sob demanda**, **store com signals**, **Signal Forms**, **pipes**, **`resource`**, **interceptor** e **testes**.
- Uma **store** tem uma fonte privada e muitas consequências `computed`; mudanças passam por métodos que **validam** e **gravam explicitamente**.
- Uma **abstração de armazenamento** deixa a store testável e protege o app de falhas do `localStorage`.
- Dados guardados são **conferidos ao carregar**; arquivos ilegíveis são **preservados** antes de qualquer gravação.
- Guarde uma **versão** no formato salvo, para migrar no futuro.
- **Privacidade**: tudo fica no navegador; nada é enviado; avise o que é guardado.
- Na casca: **`routerLinkActive`**, **`aria-current`**, link para pular a navegação e um rodapé de cuidado.
:::

::: quiz
? Por que a store pede um `Armazenamento` em vez de usar `localStorage` diretamente?
+ Para trocar por uma versão em memória nos testes e isolar as falhas do navegador num lugar só
- Porque o Angular proíbe `localStorage`
- Porque é mais rápido
- Para enviar os dados a um servidor
> É a injeção de dependência do capítulo 8 aplicada ao armazenamento.

? O que o Diário faz quando encontra um arquivo guardado ilegível?
- Apaga e começa de novo
+ Guarda uma cópia do conteúdo original, começa vazio e avisa a pessoa
- Trava o app
- Ignora em silêncio
> Nunca perder o que alguém escreveu.

? Por que gravar dentro de `adicionar` e `remover`, em vez de usar um `effect`?
+ Para deixar claro quando e por que se grava, e não depender do agendamento de effects nos testes
- Porque `effect` não funciona com `localStorage`
- Porque signals não podem ser gravados
- Por desempenho
> Reações a ações ficam no método da ação (capítulo 3).

? Para que serve `versao: 1` no arquivo guardado?
+ Para, no futuro, reconhecer o formato antigo e migrar os dados em vez de quebrar
- Para contar quantas vezes o app abriu
- Para o Angular saber a versão do framework
- Não serve para nada
> Formatos guardados vivem mais que o código que os escreveu.

? O que `ariaCurrentWhenActive="page"` faz?
+ Marca o link da página atual com `aria-current="page"`, anunciado por leitores de tela
- Muda a cor do link
- Navega para a página
- Esconde o link
> A classe `ativo` é visual; o `aria-current` é para quem não vê.

? Por que `avisoDeCarga` precisa ser declarado antes de `_entradas`?
+ Porque campos são iniciados na ordem em que aparecem, e iniciar `_entradas` chama `carregar()`, que usa `avisoDeCarga`
- Por ordem alfabética
- Porque signals precisam ser declarados em ordem de tipo
- Não faz diferença
> Uma armadilha clássica de inicialização, em TypeScript e em C#.
:::

::: cartas
Store com signals :: Serviço com uma fonte privada (signal) e consequências públicas (`computed`), alterada só por métodos.
Abstração de armazenamento :: Classe abstrata usada como token; implementação real (localStorage) e de teste (memória).
Dados guardados são boatos :: Confira campo a campo ao carregar; ignore o ilegível e avise.
Preservar antes de sobrescrever :: Guardar uma cópia do que não deu para ler antes de gravar por cima.
Versão do formato :: Campo `versao` no que é salvo, para migrar dados antigos no futuro.
routerLinkActive :: Põe uma classe no link da rota ativa; com `ariaCurrentWhenActive`, também `aria-current`.
crypto.randomUUID :: Gera um id único no navegador, sem servidor.
:::
