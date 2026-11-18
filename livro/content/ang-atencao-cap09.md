---
id: ang-atencao-cap09
order: 331
kind: capitulo
num: 233
group: Angular · Parte 58 · As oito semanas
title: "Semana 5 · Encarar o que é difícil"
subtitle: Chegar perto do problema em vez de escondê-lo. HTTP com httpResource, interceptores, retentativas com critério, erros que viram mensagens e o ErrorHandler.
minutes: 80
hue: 220
objectives:
- Entender a proposta da quinta semana: **aproximar-se** das dificuldades com gentileza, em vez de lutar ou fugir
- Fazer requisições com **`httpResource`** e tratar **todos** os estados: carregando, erro e valor
- Conferir dados que chegam da rede (**`parse`**) antes de confiar neles
- Escrever um **interceptor funcional** que registra, repete só o que faz sentido e traduz erros
- Capturar erros não tratados com um **`ErrorHandler`** próprio
- Testar HTTP com **`HttpTestingController`**
origem: angular/livros/01-atencao-plena-angular/content/10-cap09.md
---

::: cena O erro engolido
No código antigo, toda chamada ao servidor terminava num `catch` vazio. Quando a rede caía, a lista simplesmente ficava vazia, e as pessoas achavam que tinham perdido os dados. Bia fez uma pergunta que ninguém queria ouvir: "Quantas vezes isso aconteceu no último mês?" Ninguém sabia. O erro tinha sido escondido com tanto cuidado que nem os registros sabiam dele.
:::

::: ilustracao img/ang-atencao/cap09.svg | Uma ampulheta: perceber o que chegou, estreitar a atenção no que dá para fazer, ampliar de novo para o todo.
:::

## O que o livro diz

Na quinta semana, a prática principal convida a fazer algo contraintuitivo: **chegar perto** de uma dificuldade. Trazer à mente uma situação incômoda (pequena, no começo), notar **onde ela aparece no corpo** e ficar ali com gentileza, respirando "com" a sensação, sem tentar consertá-la nem empurrá-la para longe. Os autores separam com cuidado **aceitar** de **resignar-se**: aceitar é reconhecer o que já está acontecendo, para então escolher como agir; não é desistir.

O espaço de respiração ganha um passo extra (ampliar a atenção para as sensações difíceis, se fizer sentido), e o hábito pequeno da semana é **cuidar de uma planta** ou plantar algumas sementes.

Nas nossas palavras: **o que a gente evita olhar continua lá, e costuma crescer no escuro.**

::: cuidado Dificuldades pequenas primeiro
Se você experimentar a prática, comece com algo **leve** (uma conversa adiada, um prazo). Situações muito dolorosas pedem companhia profissional, e não um exercício de livro.
:::

## Da vida para o código

| Na vida | No Angular |
|---|---|
| Esconder o problema | `catch` vazio: a tela mente e ninguém fica sabendo |
| Perceber o que chegou | Registrar cada requisição e quanto ela demorou |
| Escolher o que dá para fazer | Repetir **só** o que é passageiro e **seguro** de repetir |
| Ampliar para o todo | Transformar o erro técnico numa mensagem que a pessoa entende |
| Aceitar não é desistir | Mostrar o erro **e** oferecer "tentar de novo" |
| Cuidar do que foi plantado | Conferir os dados que chegam antes de usá-los |

## 1) Preparando o HTTP

No `app.config.ts`, uma linha liga o cliente HTTP com o nosso interceptor:

```ts
provideHttpClient(withFetch(), withInterceptors([ampulhetaInterceptor])),
```

**`withFetch()`** usa a API `fetch` do navegador. **`withInterceptors([...])`** registra funções que veem **toda** requisição que sai e **toda** resposta que chega.

## 2) Frases que chegam pela rede, com `httpResource`

O app tem arquivos JSON em `public/api/` (um por tema), servidos como se viessem de um servidor. O componente:

<<< src/app/capitulos/cap09/frases.ts

- **`httpResource<Frase[]>(() => url)`** é o `resource` do capítulo 7, especializado em HTTP. A função lê o signal `tema()`: **mudou o tema, nova requisição**, e a anterior é cancelada.
- **`defaultValue: []`** é o valor enquanto nada chegou.
- **`parse`** recebe o corpo cru da resposta (`unknown`) e devolve o tipo certo, ou lança um erro. É a porta de entrada.
- O template usa **`@switch (frases.status())`** para tratar **cada** estado: `loading`, `reloading`, `error` e o resto (resolvido). Nenhum estado fica sem tela.
- No erro, há uma mensagem **e** um botão que chama **`frases.reload()`**: aceitar não é desistir.

A conferência do formato fica numa função pura:

<<< src/app/capitulos/cap09/frase.ts

::: regra O tipo genérico não confere nada
`httpResource<Frase[]>` e `http.get<Frase[]>` **não verificam** a resposta: o `<Frase[]>` é só uma promessa que você faz ao compilador. Se o servidor mandar outra coisa, o erro aparece longe, no template. Confira na entrada, com `parse` (ou uma biblioteca de validação de esquema).
:::

## 3) O interceptor ampulheta

<<< src/app/capitulos/cap09/ampulheta.interceptor.ts

Um interceptor funcional (`HttpInterceptorFn`) recebe a requisição e uma função `next` que a envia. Ele pode mexer na requisição antes e no fluxo de resposta depois. O nosso tem os três passos do espaço de respiração:

1. **Perceber**: registra `→ GET url` na saída e `← GET url (N ms)` no fim (`finalize` roda com sucesso **ou** erro).
2. **Redirecionar**: `retry` com critério. Só repete **GET** (repetir um POST poderia gravar duas vezes) e só falhas **passageiras** (status 0, rede caída, ou 503, serviço ocupado). Um 404 não vai mudar se você insistir. A espera **dobra** a cada tentativa (*backoff* exponencial).
3. **Ampliar**: `catchError` troca o erro técnico por um `ProblemaDeRede` com uma **mensagem para pessoas**. "Não é com você" faz diferença para quem está do outro lado.

A configuração vem de um **token** com valores padrão, e os testes trocam por valores pequenos.

::: csharp Polly, de novo
Quem leu o livro de Resiliência da coleção *Aprenda C#* vai reconhecer o padrão: retentativa com *backoff*, só para falhas passageiras, só para operações idempotentes. O `retry({ count, delay })` do RxJS faz o papel de uma política do Polly.
:::

::: erro Repetir qualquer coisa
`retry(3)` sem critério repete POSTs (duplicando registros), repete 400 e 404 (que nunca vão mudar) e martela um servidor que já está sofrendo. Toda retentativa precisa responder: **é seguro repetir?** E **pode dar certo da próxima vez?**
:::

## 4) O último recurso: `ErrorHandler`

Erros que ninguém tratou (uma exceção num clique, por exemplo) chegam ao **`ErrorHandler`** do Angular. O padrão só escreve no console. O nosso guarda os últimos erros num signal, que uma tela de diagnóstico poderia mostrar, e que um serviço de observabilidade poderia receber:

<<< src/app/capitulos/cap09/tratador-de-erros.ts

No `app.config.ts`, ele substitui o padrão: `{ provide: ErrorHandler, useExisting: TratadorDeErros }`. O `useExisting` reaproveita a **mesma** instância do serviço, para que quem injetar `TratadorDeErros` veja os mesmos erros que o Angular registrou.

::: cuidado Erros e privacidade
Mensagens de erro às vezes carregam dados pessoais (um e-mail na URL, um nome no corpo). Antes de mandar erros para fora do navegador, filtre o que vai junto.
:::

## 5) Testando HTTP sem servidor

<<< src/app/capitulos/cap09/http.spec.ts

- **`provideHttpClientTesting()`** troca a rede por um controlador: as requisições ficam paradas até o teste responder.
- **`HttpTestingController.expectOne(url)`** confere que **uma** requisição saiu para aquela URL e devolve um objeto para respondê-la: `flush(corpo)` (sucesso), `flush(corpo, { status })` (erro HTTP) ou `error(...)` (rede caída).
- **`verify()`** no `afterEach` garante que nenhuma requisição ficou sem resposta.
- No teste do `httpResource`, há um detalhe importante: **com uma requisição pendente, o app não está estável**, então `whenStable()` esperaria para sempre. Por isso o teste usa **`TestBed.tick()`** para disparar o resource, responde e **só então** espera.
- No teste "desiste depois das tentativas", a expectativa de erro é preparada **antes** do laço, porque a promessa falha no meio dele (senão o Vitest acusa um erro não tratado).

::: pratica Chegar perto, um pouco
Pense numa dificuldade **pequena** de hoje. Note onde ela aparece no corpo: aperto no peito, mandíbula, ombros. Respire "com" esse lugar por algumas respirações, sem tentar soltar. Depois, amplie a atenção para o corpo inteiro. Se ficar pesado, pare: isso também é cuidar.
:::

::: exercicio Um aviso de "sem conexão"
Mostre no topo do app uma faixa "Você está sem conexão" quando o navegador ficar *offline*, e esconda-a quando a conexão voltar. Use um serviço com um signal, alimentado pelos eventos `online` e `offline` da `window`.

--- resposta
```ts
import { DestroyRef, inject, Injectable, signal } from '@angular/core';

@Injectable({ providedIn: 'root' })
export class Conexao {
  readonly online = signal(navigator.onLine);

  constructor() {
    const atualizar = () => this.online.set(navigator.onLine);
    window.addEventListener('online', atualizar);
    window.addEventListener('offline', atualizar);
    inject(DestroyRef).onDestroy(() => {
      window.removeEventListener('online', atualizar);
      window.removeEventListener('offline', atualizar);
    });
  }
}
```

No `App`:

```html
@if (!conexao.online()) {
  <p role="status" class="faixa">Você está sem conexão. O que você registrar fica guardado aqui.</p>
}
```

O `role="status"` faz o aviso ser anunciado por leitores de tela. Num serviço `root`, o `DestroyRef` é o do app inteiro, então os ouvintes são removidos quando o app é destruído (importante nos testes, que criam e destroem o app várias vezes).
:::

::: resumo
- A quinta semana propõe **aproximar-se** das dificuldades com gentileza; aceitar é reconhecer, não desistir.
- **`httpResource`** deriva requisições de signals, cancela a anterior e expõe estados; trate **todos** no template, inclusive o erro com **`reload()`**.
- O tipo genérico de uma requisição **não confere** a resposta: use **`parse`** para validar na entrada.
- Um **interceptor funcional** vê toda requisição: registre (perceber), repita só GET com falha passageira e *backoff* (redirecionar) e traduza o erro para pessoas (ampliar).
- O **`ErrorHandler`** recebe o que ninguém tratou; substitua-o para registrar com cuidado (e sem dados pessoais).
- Nos testes, **`provideHttpClientTesting`** + **`HttpTestingController`** respondem às requisições; com requisição pendente, use **`TestBed.tick()`** antes de responder.
:::

::: quiz
? O que o livro diferencia de "aceitar" na quinta semana?
+ Resignar-se: aceitar é reconhecer o que acontece para então escolher como agir, não desistir
- Esquecer o problema
- Resolver o problema imediatamente
- Fingir que está tudo bem
> Aceitação abre espaço para agir com mais clareza.

? O que `httpResource<Frase[]>(...)` garante sobre a resposta?
- Que ela é sempre uma lista de frases
+ Nada: o tipo é só uma promessa ao compilador; a conferência precisa ser feita com `parse`
- Que ela nunca falha
- Que ela é guardada em cache
> Dados da rede são boatos até serem conferidos.

? Por que o interceptor não repete um POST?
+ Porque repetir uma gravação pode criar registros duplicados
- Porque POST nunca falha
- Porque o Angular não permite
- Porque POST é mais lento
> Só operações seguras de repetir (idempotentes) entram na retentativa.

? Por que não repetir um erro 404?
- Porque 404 é raro
+ Porque repetir não vai mudar a resposta: o recurso não existe
- Porque o servidor bloqueia
- Porque o `retry` não aceita 404
> Retentativa é para falhas passageiras, como 503 e rede caída.

? Para que serve o `ErrorHandler`?
+ Receber os erros que nenhum código tratou
- Tratar erros de compilação
- Validar formulários
- Interceptar requisições
> É o último recurso; substituí-lo permite registrar com cuidado.

? No teste do `httpResource`, por que usar `TestBed.tick()` em vez de `await fixture.whenStable()` logo depois de criar o componente?
+ Porque, com uma requisição pendente, o app não fica estável e o `whenStable` esperaria para sempre
- Porque `whenStable` não existe em testes de HTTP
- Porque `tick` é mais rápido
- Porque o resource não usa signals
> Dispare, responda, e só então espere.
:::

::: cartas
httpResource :: Requisição HTTP derivada de signals; refaz quando a URL muda e expõe `status`, `value`, `error`, `reload`.
parse (httpResource) :: Função que confere e converte o corpo da resposta antes de virar `value`.
Interceptor funcional :: `HttpInterceptorFn`: vê toda requisição e resposta; registrado com `withInterceptors`.
retry com critério :: Repetir só operações seguras (GET) e falhas passageiras (0, 503), com espera crescente.
Backoff exponencial :: Esperar o dobro a cada nova tentativa: 0,5 s, 1 s, 2 s...
ErrorHandler :: Recebe erros não tratados; pode ser substituído por uma implementação própria.
HttpTestingController :: Controla as requisições nos testes: `expectOne`, `flush`, `error`, `verify`.
:::
