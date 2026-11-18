---
id: ang-cnv-cap29
order: 348
kind: capitulo
num: 246
group: Angular · Parte 62 · Bloco IV — Cura e Transformação Técnica
title: Expressando a Raiva Plenamente
subtitle: Desarmando Concorrência Tóxica: switchMap, exhaustMap, concatMap e Race Conditions
minutes: 30
hue: 32
badge: 🔥
image: img/ang-cnv/cap29.svg
- Analisar a afirmação chocante de Marshall Rosenberg: "Matar pessoas é superficial demais!"
- Compreender os 4 passos para expressar a raiva plenamente sem violência ou repressão: 
- Dominar os 4 operadores de ordem superior do RxJS: switchMap, exhaustMap, concatMap e mergeMap
- Eliminar Race Conditions (condições de corrida) e cliques furiosos repetidos na interface: 
origem: angular/livros/02-cnv-angular/content/29-expressando-raiva.md
---

::: ilustracao img/ang-cnv/cap29.svg | Comparativo de concorrência em alta fidelidade: Sem switchMap requisições obsoletas colidem e corrompem a interface; com switchMap requisições antigas são canceladas serenamente, restaurando a paz na tela.
:::

No décimo capítulo de *Comunicação Não-Violenta*, Marshall Rosenberg aborda um tema que frequentemente causa perplexidade em leitores desavisados: **a expressão plena da raiva**.

Muitas correntes filosóficas e religiosas pregam que a raiva é um sentimento pecaminoso, feio ou infantil que deve ser reprimido, engolido ou disfarçado com sorrisos falsos. Marshall discorda veementemente:
> *"Queremos expressar a nossa raiva plenamente! Não queremos reprimir nada do que está vivo dentro de nós.*  
>  
> *Porém, eu gostaria de sugerir que matar, espancar, agredir, humilhar e culpar as outras pessoas é superficial demais! Essas atitudes são expressões incrivelmente pobres e superficiais do vulcão que está acontecendo dentro de nós quando sentimos raiva. Se estivermos verdadeiramente com raiva, vamos querer uma maneira infinitamente mais poderosa de nos expressar."*

### A Raiva como o Alarme no Painel do Carro

Marshall explica que a raiva funciona exatamente como **a luz de advertência no painel do carro**:
- Quando a luz do óleo acende em vermelho no painel, você pega um martelo e quebra a lâmpada?
- Claro que não! A lâmpada não é o problema; ela é um **alarme valioso** nos avisando que o motor está sem lubrificação e corre o risco de fundir.

Da mesma forma, a raiva não é a causa de nada. **A raiva é o alarme sonoro que dispara no nosso cérebro quando nos desconectamos de nossas necessidades reais e mergulhamos no mundo dos julgamentos moralizantes, da vingança e do desejo de punir.**

### Os 4 Passos da CNV para Expressar a Raiva Plenamente

1. **Passo 1: Pare. Respire fundo. Não faça nada.** Impeça qualquer ação imediata motivada pelo impulso de revidar.
2. **Passo 2: Identifique os pensamentos de julgamento na mente.** Escute o chacal interno gritando: *"Ele é um incompetente! Ele merecia ser demitido! Isso é um absurdo!"*.
3. **Passo 3: Conecte-se com a necessidade não atendida por trás desses pensamentos.** Pergunte a si mesmo: *"O que eu estou precisando com tanta intensidade que está me fazendo gerar esse julgamento feroz? Estou precisando de respeito pelo meu tempo? Estou precisando de segurança no projeto?"*.
4. **Passo 4: Abra a boca e expresse seus sentimentos e necessidades vulneráveis.** Em vez de gritar *"Você é um irresponsável!"*, diga: *"Quando vi o prazo estourar, senti uma raiva e um medo profundos, porque necessito de previsibilidade para cumprir o acordo com a minha equipe"*.

---

## A Raiva do Usuário na Interface: O Caos da Concorrência

Como a raiva e a impaciência humana se manifestam diante de uma aplicação web?
- O usuário está com pressa ou irritado.
- Clica no botão "Confirmar Pagamento" seis vezes em dois segundos.
- Digita termos furiosamente no campo de busca, apagando e reescrevendo sem parar.
- Troca de abas rapidamente tentando forçar o sistema a responder no ritmo de sua ansiedade.

Se a sua arquitetura Angular não tiver sido projetada com sabedoria reativa, o sistema cai em uma **Condição de Corrida Tóxica (*Race Condition*)**:

```
Tempo 0ms   ➔ Usuário pesquisa "amor"  ➔ Requisição A disparada (demora 800ms)
Tempo 200ms ➔ Usuário pesquisa "paz"   ➔ Requisição B disparada (demora 200ms)
Tempo 400ms ➔ Requisição B termina!    ➔ A tela exibe os resultados de "paz" ✓
Tempo 800ms ➔ Requisição A termina atrasada! ➔ A tela é sobrescrita com "amor" ❌
```

O usuário pesquisou por "paz" por último, mas a tela está exibindo os dados de "amor"! O sistema gerou uma mentira visual porque não soube gerenciar a concorrência assíncrona.

---

## Os 4 Pacificadores de Ordem Superior do RxJS (angular.dev)

O **RxJS** possui quatro operadores de achatamento de streams (*Higher-Order Mapping Operators*) que correspondem a posturas específicas diante da concorrência:

```
+---------------------------------------------------------------------------------------------------------+
|                               OS 4 OPERADORES PACIFICADORES DO RxJS                                     |
+-------------------+--------------------+--------------------------------+-------------------------------+
| OPERADOR          | FILOSOFIA CNV      | COMPORTAMENTO TÉCNICO          | CASO DE USO IDEAL             |
+-------------------+--------------------+--------------------------------+-------------------------------+
| switchMap         | "O presente        | Cancela a requisição anterior  | Buscas em tempo real, filtros,|
|                   | importa"           | assim que uma nova chega.      | troca de abas e rotas.        |
| exhaustMap        | "Foco sereno e     | Ignora qualquer nova ação      | Botões de salvar, pagamentos, |
|                   | paciência firme"   | enquanto a atual estiver viva. | checkouts contra clique duplo.|
| concatMap         | "Fila ordeira e    | Enfileira em ordem estrita,    | Sincronização de logs, filas  |
|                   | respeitosa"        | processando uma por uma.       | de auditoria e gravações.     |
| mergeMap          | "Acolhimento       | Processa tudo simultaneamente  | Uploads independentes de      |
|                   | irrestrito"        | em paralelo, sem cancelamento. | arquivos múltiplos.           |
+-------------------+--------------------+--------------------------------+-------------------------------+
```

---

## 1. switchMap: Desarmando a Obsolescência

O `switchMap` atua como a respiração consciente do Passo 1: quando um novo estímulo chega, ele cancela pacificamente a subscrição interna anterior que estava em trânsito no navegador (`ERR_ABORTED`), garantindo que apenas a **intenção mais recente** seja renderizada:

```typescript
// Desarmando buscas concorrentes com switchMap
readonly resultados$ = this.campoBusca.valueChanges.pipe(
  debounceTime(250),
  distinctUntilChanged(),
  // Cancela qualquer requisição anterior que tenha ficado para trás
  switchMap(termo => {
    if (!termo.trim()) return of([]);
    return this.http.get<Artigo[]>(`/api/artigos?busca=${encodeURIComponent(termo)}`).pipe(
      catchError(() => of([]))
    );
  })
);
```

---

## 2. exhaustMap: Acalmando o Clique Furioso de Transações

E quando o usuário, tomado pela impaciência ou pela pressa, clica furiosamente no botão de submissão do formulário?

Se você usar `mergeMap` ou `switchMap` em uma rota de pagamento, o servidor receberá múltiplas chamadas e poderá cobrar o cartão de crédito do cliente duas ou três vezes!

O operador **`exhaustMap`** atua como uma muralha protetora: enquanto a operação atual estiver em andamento, **ele descarta sumariamente qualquer novo clique**, sem permitir duplicidade de transações:

```typescript
// src/app/features/doacao/botao-doacao.component.ts
import { Component, inject, signal } from '@angular/core';
import { Subject } from 'rxjs';
import { exhaustMap, tap, delay } from 'rxjs/operators';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { HttpClient } from '@angular/common/http';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-botao-doacao-pacifica',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="transacao-box">
      <button
        type="button"
        class="btn-doar"
        [class.processando]="processando()"
        (click)="disparosClique.next()"
      >
        @if (processando()) {
          <span class="spinner-mini"></span>
          Processando com Segurança Atômica...
        } @else {
          Confirmar Contribuição de Paz [Clique Seguro]
        }
      </button>

      @if (confirmado()) {
        <p class="sucesso-msg" role="status">
          ✓ Sua contribuição foi acolhida uma única vez com sucesso!
        </p>
      }
    </div>
  `,
  styles: [`
    .transacao-box { padding: 1.5rem; background: #fffaf5; border: 1px solid #f5dfd0; border-radius: 10px; max-width: 440px; }
    .btn-doar { background: #d96b27; color: white; border: none; padding: 0.85rem 1.5rem; border-radius: 8px; font-weight: bold; cursor: pointer; display: flex; align-items: center; gap: 0.5rem; }
    .btn-doar.processando { opacity: 0.75; cursor: wait; background: #b45309; }
    .sucesso-msg { color: #047857; font-weight: 600; margin-top: 1rem; }
    .spinner-mini { width: 14px; height: 14px; border: 2px solid white; border-top-color: transparent; border-radius: 50%; animation: girar 0.8s linear infinite; }
    @keyframes girar { to { transform: rotate(360deg); } }
  `]
})
export class BotaoDoacaoPacificaComponent {
  private readonly http = inject(HttpClient);

  readonly disparosClique = new Subject<void>();
  readonly processando = signal(false);
  readonly confirmado = signal(false);

  constructor() {
    this.disparosClique.pipe(
      takeUntilDestroyed(),
      // exhaustMap: ignora sumariamente qualquer clique subsequente até que o anterior termine!
      exhaustMap(() => {
        this.processando.set(true);
        this.confirmado.set(false);

        return this.http.post('/api/doacoes', { valor: 50 }).pipe(
          tap(() => {
            this.processando.set(false);
            this.confirmado.set(true);
          })
        );
      })
    ).subscribe();
  }
}
```

::: dica
Ao combinar `switchMap` nas consultas de leitura com `exhaustMap` nas transações críticas de escrita, você desarma por completo a violência das *race conditions*. Mesmo que o usuário clique freneticamente sob forte tensão emocional, o sistema absorve o impacto com elegância e consistência matemática.
:::

---

::: quiz
? Qual é a razão pela qual Marshall Rosenberg afirma que "matar pessoas é superficial demais"?
- Porque as prisões estão lotadas
+ Porque agredir, punir, matar ou culpar os outros são expressões pobres e superficiais que falham em comunicar a profundidade vulcânica dos sentimentos e necessidades que estão vivos em nós
- Porque armas de fogo custam caro
- Porque a vingança demora muito para ser executada
> Marshall nos desafia a enxergar que a agressão física ou verbal é um grito desesperado e infantil de quem não aprendeu a expressar a dor de suas necessidades não atendidas. A CNV oferece uma forma infinitamente mais potente e transformadora de expressão.

? O que a metáfora da "luz no painel do carro" ensina sobre a raiva?
- Que devemos desconectar a bateria do carro para o alarme parar
+ Que a raiva não é a causa do problema, mas um alarme precioso nos alertando que nos desconectamos de nossas necessidades reais e mergulhamos em julgamentos moralizantes
- Que devemos trocar de carro sempre que a luz acender
- Que a raiva só acontece quando estamos dirigindo
> Tentar suprimir a raiva equivale a quebrar a lâmpada do painel com um martelo: você silencia o aviso, mas o motor continua fundindo por falta de óleo. A raiva nos avisa: olhe para dentro agora!

? Qual operador do RxJS é a escolha definitiva para proteger botões de pagamento e finalização contra cliques múltiplos e repetidos?
- `mergeMap`
- `switchMap`
+ `exhaustMap`
- `map`
> O `exhaustMap` ignora qualquer evento subsequente até que a operação assíncrona interna atual tenha completado seu ciclo, impedindo cobranças duplicadas ou transações concorrentes indesejadas.

? O que acontece com requisições HTTP anteriores em andamento quando utilizamos o operador `switchMap`?
- Elas são salvas em um arquivo de log permanente
+ A subscrição do Observable anterior é cancelada imediatamente pelo RxJS, abortando a requisição pendente no navegador e prevenindo race conditions
- Elas continuam rodando em segundo plano até o final dos tempos
- O servidor é desligado automaticamente
> O cancelamento via `switchMap` previne que respostas lentas de buscas antigas sobrescrevam resultados de buscas mais recentes, garantindo a coerência visual da interface.

? Em qual cenário o operador `concatMap` é a escolha técnica recomendada?
- Para cancelar requisições de digitação rápida
+ Quando a ordem estrita de execução das requisições assíncronas precisa ser preservada rigorosamente, processando uma por uma sequencialmente (como em logs de auditoria)
- Para rodar 50 requisições simultâneas em paralelo
- Para apagar variáveis do localStorage
> O `concatMap` respeita a fila cronológica de eventos: nenhuma emissão é iniciada antes que a anterior seja finalizada com êxito, garantindo ordenação perfeita.
:::

---

::: cartas
Expressão Plena da Raiva :: Quarto passo avançado da CNV: desarmar o impulso de julgar e punir, conectando-se com a necessidade não atendida que acionou o alarme.
Alarme no Painel :: Metáfora de Marshall Rosenberg demonstrando que a raiva é apenas o aviso de que a nossa necessidade foi esquecida.
Race Condition :: Falha de concorrência onde respostas assíncronas chegam fora da ordem pretendida, corrompendo o estado da tela.
switchMap (RxJS) :: Operador de cancelamento de concorrência que desliga a stream anterior assim que um novo estímulo chega.
exhaustMap (RxJS) :: Operador de blindagem atômica que ignora novos estímulos enquanto a operação assíncrona atual não tiver terminado.
concatMap (RxJS) :: Operador de fila sequencial que processa tarefas assíncronas uma a uma, preservando a ordem cronológica exata.
mergeMap (RxJS) :: Operador de paralelismo que processa múltiplas tarefas simultâneas sem cancelamento ou bloqueio.
Cliques Furiosos :: Reação impaciente e comum do usuário que deve ser acolhida e neutralizada arquiteturalmente por operadores do RxJS.
:::
