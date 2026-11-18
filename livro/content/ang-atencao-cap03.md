---
id: ang-atencao-cap03
order: 324
kind: capitulo
num: 227
group: Angular · Parte 57 · Sair do piloto automático
title: Dois modos da mente
subtitle: O modo que faz e o modo que percebe. Em Angular, o estado copiado à mão contra o estado declarado com signal, computed, effect e linkedSignal.
minutes: 65
hue: 320
objectives:
- Conhecer os modos **Atuante** e **Existente** descritos pelo livro, e por que nenhum deles é inimigo
- Ver na prática o bug clássico do **estado copiado à mão** (modo imperativo)
- Declarar estado derivado com **`computed`**, que é preguiçoso e guarda o último valor
- Saber quando usar **`effect`** (quase nunca) e quando usar **`linkedSignal`**
- Ler signals no **template** e deixar o Angular cuidar da atualização
origem: angular/livros/01-atencao-plena-angular/content/03-cap03.md
---

::: cena O número que mentia
Beto abriu um chamado: "A tela diz *faltam 4 minutos*, mas a sessão já acabou." Caio achou o culpado em minutos: um método mudava a meta e esquecia de recalcular o que faltava. "Conserto fácil, é só lembrar de atualizar ali também." Bia perguntou: "E no próximo método que alguém escrever, quem vai lembrar?"
:::

::: ilustracao img/ang-atencao/cap03.svg | Dois jeitos de estar diante da mesma tarefa: correndo para manter tudo em dia, ou declarando o que é e deixando as consequências seguirem.
:::

## O que o livro diz

Williams e Penman descrevem dois **modos** em que a mente opera. O modo **Atuante** (*doing*) é o que resolve problemas: compara onde estamos com onde queremos estar, planeja, analisa, cobra. Ele é essencial, e é ele que nos faz terminar um projeto. O problema é quando ele assume tarefas para as quais não serve, como "consertar" uma emoção, e quando vira **piloto automático**, fazendo tudo sem que a gente perceba.

O modo **Existente** (*being*) é o de **perceber**: estar com a experiência como ela é, agora, sem a pressa de corrigi-la. Os autores listam várias diferenças entre os dois, como agir no automático ou com escolha, analisar ou sentir, lutar contra o que é ou aceitar, tratar pensamentos como fatos ou como eventos mentais, e insistem num ponto: **o modo Atuante não é o inimigo**. A habilidade está em **trocar de marcha** quando faz sentido.

Nas nossas palavras: **há coisas que a gente precisa fazer, e coisas que basta deixar que sejam o que são.**

## Da vida para o código

| Na vida | No Angular |
|---|---|
| Modo Atuante: correr para manter tudo em dia | Código **imperativo**: cada método copia e recalcula o estado à mão |
| O que acontece quando esquecemos um detalhe | O **bug do estado dessincronizado** |
| Modo Existente: declarar o que é | **`computed`**: o valor derivado é uma consequência, sempre em dia |
| Agir quando é preciso agir | **`effect`**: para efeitos colaterais de verdade (log, `localStorage`), e só para eles |
| Mudar de foco sem perder o fio | **`linkedSignal`**: um valor ajustável que se reacomoda quando a fonte muda |

## 1) O modo Atuante, em código

Uma sessão de prática tem minutos planejados, minutos feitos, o quanto falta e se terminou. Primeiro, do jeito imperativo, com campos comuns:

<<< src/app/capitulos/cap03/sessao-atuante.ts

`restante` e `concluida` são **cópias** de um cálculo. `praticar` lembra de refazê-lo; `replanejar` esquece. Nada no código impede o esquecimento, e o objeto passa a **mentir**: diz que faltam 4 minutos de uma sessão que já acabou. O teste do capítulo documenta o bug de propósito:

<<< src/app/capitulos/cap03/sessoes.spec.ts#atuante

Repare: não há erro de compilação nem exceção. É o tipo de defeito mais caro, o que parece funcionar.

## 2) O modo Existente: `signal` e `computed`

Agora, declarando o que **é**:

<<< src/app/capitulos/cap03/sessao-existente.ts

- `planejados` e `feitos` são **fontes**: signals graváveis.
- `restante` e `concluida` são **consequências**: `computed` recebe uma função que **lê** outros signals. O Angular registra essas leituras e passa a saber de quem cada `computed` depende.
- Não existe mais um lugar para esquecer: qualquer método que mude uma fonte, hoje ou daqui a um ano, mantém as consequências em dia.

<<< src/app/capitulos/cap03/sessoes.spec.ts#existente

::: analogia A planilha
Pense numa planilha: a célula C1 tem a fórmula `=A1-B1`. Você nunca "atualiza" C1 à mão; muda A1 ou B1 e C1 acompanha. `signal` é a célula com valor digitado; `computed` é a célula com fórmula.
:::

### `computed` é preguiçoso e tem memória

Um `computed` só calcula quando alguém **lê**, e guarda o resultado até uma fonte mudar:

<<< src/app/capitulos/cap03/sessoes.spec.ts#preguicoso

Isso tem duas consequências práticas:

1. **Pode ler à vontade.** Ler o mesmo `computed` dez vezes no template custa um cálculo.
2. **A função precisa ser pura**: só ler signals e devolver um valor. Nada de gravar arquivo, chamar API ou mudar outro signal dentro de um `computed`.

::: erro Mudar um signal dentro de um `computed`
`computed(() => { this.contador.set(1); return ... })` é um erro (o Angular avisa em tempo de execução). Um `computed` **descreve** um valor; não **faz** coisas. Para fazer, existe o `effect`, e quase sempre existe um jeito melhor que ele.
:::

## 3) O template só lê

Um componente que usa a sessão:

<<< src/app/capitulos/cap03/painel-da-sessao.ts

O template lê `sessao.planejados()`, `sessao.feitos()`, `sessao.concluida()` e `sessao.restante()`. Quando um clique chama `praticar(1)`, o Angular sabe exatamente quais trechos da tela dependem do que mudou e atualiza só eles. Nenhuma linha diz "atualize a tela". O capítulo 5 mostra por que isso importa tanto num app sem zone.js.

### `effect`: só para o que está fora do mundo dos signals

O construtor tem um **`effect`**: uma função que roda sempre que um signal lido por ela muda. Aqui, ela escreve no console quando a sessão termina. Use `effect` para **sincronizar com o mundo de fora** (o título da aba, o `localStorage`, uma biblioteca de gráficos), e nunca para calcular um valor que poderia ser um `computed`.

::: regra Antes de escrever um effect
Pergunte: "Isso é um **valor** que depende de outros?" Se sim, é `computed`. "Isso precisa **acontecer** fora da tela?" Talvez seja `effect`. "Isso acontece por causa de uma **ação** (um clique, um envio)?" Então faça no método que trata a ação, não num effect. A maioria dos effects que aparecem em revisão de código é uma dessas duas coisas disfarçada.
:::

## 4) `linkedSignal`: escolher, e se reacomodar

Às vezes um valor é **escolhido** pela pessoa, mas depende de uma lista que pode mudar. Exemplo: há uma lista de práticas, e uma delas está em foco. Se a lista muda e a escolhida continua lá, o foco fica; se ela some, o foco volta para a primeira opção. Com `signal` + `effect`, isso vira uma pequena gambiarra. Com **`linkedSignal`**, é uma declaração:

<<< src/app/capitulos/cap03/foco.ts

- **`source`** é a fonte (a lista); **`computation`** recebe a lista nova e o **valor anterior** (`anterior?.value`), e decide o novo valor.
- Ao contrário de um `computed`, um `linkedSignal` **aceita `set`**: a pessoa pode escolher outra prática à mão.
- Quando a fonte muda, a `computation` roda de novo e decide se mantém a escolha ou se reacomoda.

<<< src/app/capitulos/cap03/foco.spec.ts

::: csharp Se você conhece INotifyPropertyChanged
No WPF ou no MAUI, uma propriedade calculada precisa disparar `PropertyChanged` à mão sempre que suas dependências mudam: exatamente o modo Atuante deste capítulo. Signals fazem esse rastreamento sozinhos, como se cada `get` anotasse de quem depende.
:::

::: pratica Fazer × perceber
Escolha uma tarefa rotineira de hoje (lavar uma xícara, subir uma escada). Faça-a uma vez **percebendo**: a temperatura, o peso, o som, sem pressa de terminar. Não é para fazer sempre assim. É só para lembrar como é a outra marcha.
:::

::: exercicio Quanto falta, em porcentagem
Acrescente a `SessaoExistente` um `computed` chamado `progresso`, de 0 a 1, que nunca passa de 1 e que vale 1 quando os minutos planejados forem 0. Escreva um teste em que a pessoa pratica 15 minutos de uma sessão de 10.

--- resposta
```ts
readonly progresso = computed(() => {
  const planejados = this.planejados();
  return planejados <= 0 ? 1 : Math.min(1, this.feitos() / planejados);
});
```

```ts
it('progresso nunca passa de 100%', () => {
  const s = new SessaoExistente();
  s.praticar(15);
  expect(s.progresso()).toBe(1);
  s.replanejar(30);
  expect(s.progresso()).toBe(0.5);
});
```

Repare que o teste chama `replanejar` **depois** e confere de novo: é o tipo de verificação que teria pego o bug do modo Atuante.
:::

::: resumo
- O livro descreve o modo **Atuante** (resolver, comparar, cobrar) e o modo **Existente** (perceber). O Atuante não é inimigo; o problema é usá-lo para tudo, no automático.
- **Estado copiado à mão** (imperativo) dessincroniza no dia em que alguém esquece de recalcular. É um bug sem erro nem exceção.
- **`signal`** é uma fonte gravável; **`computed`** é uma consequência declarada, preguiçosa e com memória. A função do `computed` precisa ser **pura**.
- O **template** só lê signals; o Angular sabe o que depende do quê e atualiza só o necessário.
- **`effect`** é para sincronizar com o mundo de fora, raramente. Valores derivados são `computed`; reações a ações ficam no método da ação.
- **`linkedSignal`** é um valor ajustável à mão que se **reacomoda** quando a fonte muda.
:::

::: quiz
? Segundo o livro, qual é o problema do modo Atuante?
- Ele é sempre prejudicial e deve ser eliminado
+ Ele é útil, mas atrapalha quando assume tarefas para as quais não serve e quando vira piloto automático
- Ele só existe em pessoas ansiosas
- Ele é o mesmo que o modo Existente
> A habilidade é trocar de marcha, não eliminar um dos modos.

? O que dá errado em `SessaoAtuante.replanejar`?
+ Muda a meta e esquece de recalcular `restante` e `concluida`, que ficam com valores errados
- Lança uma exceção
- Não compila
- Apaga os minutos feitos
> É o bug do estado copiado: nada avisa, e o objeto passa a mentir.

? Quando a função de um `computed` roda?
- A cada segundo
- Toda vez que o template é verificado
+ Quando alguém lê o valor e alguma fonte mudou desde o último cálculo
- Só uma vez, na criação
> É preguiçoso e guarda o último resultado.

? Qual destes é um bom uso de `effect`?
- Calcular o total de minutos a partir de outros signals
+ Gravar no `localStorage` quando um valor muda
- Mudar outro signal para manter os dois iguais
- Validar um formulário
> `effect` é para sincronizar com o mundo de fora; valores derivados são `computed`.

? O que diferencia um `linkedSignal` de um `computed`?
+ O `linkedSignal` aceita `set` e se reacomoda quando a fonte muda
- O `linkedSignal` não depende de nenhuma fonte
- O `computed` aceita `set`
- Não há diferença
> É um valor escolhido que respeita mudanças na fonte.

? Por que a função de um `computed` precisa ser pura?
- Para o TypeScript compilar
+ Porque o Angular decide quando (e se) ela roda; efeitos colaterais rodariam em momentos imprevisíveis
- Porque `computed` não aceita funções
- Para o teste passar
> Descrever um valor, sim; fazer coisas, não.
:::

::: cartas
Modo Atuante :: Modo da mente que resolve, compara e cobra; útil, mas problemático no automático ou com emoções.
Modo Existente :: Modo da mente que percebe a experiência como ela é, agora, sem pressa de corrigir.
Estado copiado à mão :: Valor derivado guardado num campo comum; dessincroniza quando alguém esquece de recalcular.
computed :: Signal somente leitura derivado de outros; preguiçoso, com memória, função pura.
effect :: Função que roda quando os signals que ela lê mudam; para efeitos fora da tela (log, armazenamento).
linkedSignal :: Signal gravável que se recalcula quando a fonte muda, considerando o valor anterior.
Função pura :: Só lê suas entradas e devolve um valor; não muda nada fora dela.
:::
