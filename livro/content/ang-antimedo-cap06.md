---
id: ang-antimedo-cap06
order: 306
kind: capitulo
num: 211
group: Angular · Parte 53 · Bloco II — Quebrando o Ciclo
title: Sinal × Ruído
subtitle: RxJS e Observables — filtrando o que realmente importa
minutes: 35
hue: 200
badge: 📡
- Distinguir sinal de ruído na mente e nos fluxos de dados assíncronos: 
- Dominar o conceito de Observable e Subject no Angular: 
- Aplicar operadores essenciais do RxJS: filter, debounceTime, distinctUntilChanged e switchMap
- Usar o AsyncPipe no template para gerenciamento automático de subscrições: 
origem: angular/livros/01-antimedo-angular/content/06-sinal-x-ruido.md
---

::: ilustracao img/ang-antimedo/cap06.svg | Um filtro luminoso separando os sinais limpos de uma tempestade caótica de ruídos estáticos.
:::

Em 2006, Pablo Marçal começou a bloquear as opiniões alheias. Em 2012, parou de assistir televisão. Em 2016, encerrou debates infrutíferos na internet. Em 2017, abandonou portais de notícias e jornais.

Por que tantas renúncias radicais?

Porque ele descobriu um princípio implacável: **95% do que circula como informação é mero ruído**. Ruído é distração que cansa, mente que não produz e emoção sem propósito. O sinal, por outro lado, é o dado verdadeiro, a coordenada exata, a instrução que gera transformação.

No desenvolvimento de software moderno, a sua aplicação web sofre do mesmíssimo mal.

---

## O Caos dos Eventos no Frontend

Pense em um campo de busca onde o usuário digita *"aprenda angular antimedo"*.

Se você escutar cada tecla pressionada (`keydown` ou `input`) e imediatamente disparar uma requisição HTTP para o servidor, você enviará dezenas de chamadas pela rede. A maioria dessas requisições é puro ruído: o usuário ainda estava no meio de uma palavra, corrigiu uma letra, digitou rápido demais.

Se a sua aplicação tentar responder a cada lampejo de ruído, ela sofrerá de **sobrecarga cognitiva digital**: lentidão, condições de corrida (*race conditions*) e desperdício de memória.

Para separar o sinal do ruído no Angular, nós não usamos *callbacks* desgovernados. Nós usamos a ferramenta mais poderosa de manipulação de fluxos contínuos: **RxJS** (*Reactive Extensions for JavaScript*).

---

## O que é um Observable?

Um `Observable` é um fluxo contínuo de dados emitidos ao longo do tempo. Pense nele como um cano por onde passam mensagens:

- Pode emitir **zero, um ou múltiplos valores**.
- Pode **completar** com sucesso ou emitir um **erro**.
- É **preguiçoso (*lazy*)**: nada acontece até que alguém se inscreva (*subscribe*).

```typescript
import { Observable } from 'rxjs';

// Criando um fluxo que emite dados controlados
const fluxoDePensamentos$ = new Observable<string>(subscriber => {
  subscriber.next('Ruído: O que os outros vão pensar?');
  subscriber.next('Ruído: Angular é difícil demais');
  subscriber.next('SINAL: Vou dominar componentes hoje');
  subscriber.complete();
});

// Escutando o fluxo
fluxoDePensamentos$.subscribe({
  next: valor => console.log('Recebido:', valor),
  complete: () => console.log('Fluxo finalizado!')
});
```

```saida
Recebido: Ruído: O que os outros vão pensar?
Recebido: Ruído: Angular é difícil demais
Recebido: SINAL: Vou dominar componentes hoje
Fluxo finalizado!
```

::: nota Convenção do Cifrão
No universo Angular e RxJS, é convenção adicionar um `$` ao final do nome de variáveis que guardam Observables (ex: `termoBusca$`, `usuarios$`). Isso sinaliza instantaneamente aos colegas que aquilo é um fluxo assíncrono.
:::

---

## Os Quatro Filtros Antimedo do RxJS

Assim como você deve filtrar as vozes que entram na sua cabeça, o RxJS nos fornece operadores puros encadeados pelo método `.pipe()`.

### 1. `filter()`: Bloqueio do que não tem substância
Impede que valores vazios ou insuficientes prossigam na cadeia.

### 2. `debounceTime()`: O tempo de assentamento
Espera o usuário parar de disparar impulsos por alguns milissegundos antes de emitir o valor final. Se novas teclas forem digitadas antes do tempo expirar, o cronômetro reinicia.

### 3. `distinctUntilChanged()`: Bloqueio de repetições inúteis
Se o valor atual for idêntico ao anterior, ele é descartado. Por que processar a mesma informação duas vezes?

### 4. `switchMap()`: Cancelamento do passado obsoleto
Se uma nova busca for disparada antes da requisição anterior responder, ele cancela a requisição anterior e foca unicamente na mais recente.

Veja a combinação em ação num componente real:

```typescript
import { Component, OnInit } from '@angular/core';
import { FormControl, ReactiveFormsModule } from '@angular/forms';
import { debounceTime, distinctUntilChanged, filter, switchMap } from 'rxjs/operators';
import { Observable, of } from 'rxjs';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-filtro-ruido',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  template: `
    <div class="busca-container">
      <h3>Central de Sinal Puro</h3>
      <input 
        [formControl]="buscaControl" 
        placeholder="Digite para buscar sinais reais..." 
        class="input-busca"
      />

      <div class="status">
        <p *ngIf="resultado$ | async as resultado">
          <strong>Sinal Processado:</strong> {{ resultado }}
        </p>
      </div>
    </div>
  `
})
export class FiltroRuidoComponent implements OnInit {
  buscaControl = new FormControl('');
  resultado$!: Observable<string>;

  ngOnInit(): void {
    this.resultado$ = this.buscaControl.valueChanges.pipe(
      // 1. Converte null em string e remove espaços
      filter((termo): termo is string => termo !== null && termo.trim().length >= 3),
      // 2. Aguarda 400ms de silêncio após a última tecla
      debounceTime(400),
      // 3. Ignora se o termo for idêntico ao da última busca
      distinctUntilChanged(),
      // 4. Cancela buscas antigas e inicia a nova busca
      switchMap(termoLimpo => this.simularBuscaApi(termoLimpo))
    );
  }

  private simularBuscaApi(termo: string): Observable<string> {
    console.log(`[REQUISIÇÃO DISPARADA]: Sinal verificado para "${termo}"`);
    return of(`Resultado oficial para: "${termo}"`);
  }
}
```

::: dica O Poder do AsyncPipe
Note que no template usamos `resultado$ | async`. O `AsyncPipe` faz o `.subscribe()` automaticamente quando o componente aparece na tela e faz o `.unsubscribe()` quando o componente é destruído. Você nunca mais terá vazamento de memória (*memory leaks*) por esquecer de cancelar inscrições.
:::

---

## Subjects: Quando Você é a Fonte do Sinal

Um `Observable` comum é unidirecional e frio (*cold*). Se você precisa de um canal onde você mesmo pode emitir novos sinais a qualquer momento e múltiplos ouvintes podem escutar, use um **Subject** ou **BehaviorSubject**.

```typescript
import { BehaviorSubject } from 'rxjs';

// O BehaviorSubject guarda o estado atual e o entrega a novos inscritos
export class GerenciadorFocoService {
  private nivelClareza$ = new BehaviorSubject<number>(100);

  // Expõe apenas como Observable para leitura pública (encapsulamento)
  public clarezaAtual$ = this.nivelClareza$.asObservable();

  descartarRuido(pontosRuido: number): void {
    const atual = this.nivelClareza$.value;
    const novoValor = Math.min(100, atual + pontosRuido);
    this.nivelClareza$.next(novoValor);
  }

  receberDistracao(impacto: number): void {
    const atual = this.nivelClareza$.value;
    const novoValor = Math.max(0, atual - impacto);
    this.nivelClareza$.next(novoValor);
  }
}
```

---

::: exercicio Exercício: Criando um Rastreador de Sinal
Implemente um serviço que receba notificações de alerta e só permita que o alerta passe para o usuário se o nível de severidade for `'CRITICO'` ou `'ALTO'`.

--- resposta
```typescript
import { Injectable } from '@angular/core';
import { Subject, Observable } from 'rxjs';
import { filter } from 'rxjs/operators';

export interface Alerta {
  mensagem: string;
  severidade: 'BAIXO' | 'MEDIO' | 'ALTO' | 'CRITICO';
}

@Injectable({ providedIn: 'root' })
export class AlertaService {
  private alertasSource$ = new Subject<Alerta>();

  // Apenas sinais urgentes chegam ao desenvolvedor
  public alertasUrgentes$: Observable<Alerta> = this.alertasSource$.pipe(
    filter(alerta => alerta.severidade === 'ALTO' || alerta.severidade === 'CRITICO')
  );

  emitir(alerta: Alerta): void {
    this.alertasSource$.next(alerta);
  }
}
```
:::

---

::: quiz
? Qual operador RxJS aguarda uma pausa nas emissões antes de passar o valor adiante?
+ debounceTime
- throttleTime
- distinctUntilChanged
- switchMap
> O `debounceTime` aguarda o tempo estipulado sem novas emissões para liberar o valor mais recente.

? O que acontece ao usar o AsyncPipe `{{ dados$ | async }}` no template?
+ O Angular faz o subscribe automaticamente e cancela a subscrição no ngOnDestroy
- O Angular congela a renderização até a Promise resolver
- O código compila para uma requisição síncrona bloqueante
- É obrigatório chamar unsubscribe manualmente no componente
> O AsyncPipe cuida do ciclo completo de subscrição e cancelamento, prevenindo vazamentos de memória.

? Qual operador cancela automaticamente a requisição anterior caso uma nova emissão ocorra?
+ switchMap
- mergeMap
- concatMap
- exhaustMap
> `switchMap` "troca" de fluxo, cancelando o fluxo interno anterior se um novo valor chegar.

? Qual a principal característica de um BehaviorSubject em relação a um Subject comum?
+ Ele armazena um valor inicial/atual e o entrega imediatamente a novos inscritos
- Ele só aceita valores numéricos
- Ele bloqueia a emissão de erros
- Ele roda fora da Zone do Angular
> O `BehaviorSubject` exige um valor inicial e sempre mantém em cache o último valor emitido.

? Qual operador é ideal para evitar que duas pesquisas idênticas seguidas chamem o servidor?
+ distinctUntilChanged
- takeUntil
- catchError
- tap
> `distinctUntilChanged` compara o valor atual com o anterior e barra emissões duplicadas em sequência.
:::

---

::: cartas
Sinal no RxJS :: Informação real e relevante emitida por um Observable que a aplicação deve processar.
Ruído no RxJS :: Eventos desnecessários, cliques frenéticos ou dados duplicados que devem ser filtrados com operadores.
debounceTime(ms) :: Operador que pausa o fluxo e só emite o valor após um intervalo de silêncio configurado.
distinctUntilChanged() :: Filtro que impede que valores iguais ao imediatamente anterior sejam reemitidos.
switchMap() :: Transforma um valor em outro Observable, cancelando qualquer fluxo anterior ainda em andamento.
AsyncPipe :: Pipe do Angular que se inscreve em Observables no template e gerencia o ciclo de vida sem memory leaks.
:::

::: resumo
Pablo Marçal desligou a televisão para salvar sua mente do ruído. Você aprendeu a usar o RxJS para salvar a sua aplicação das requisições desnecessárias. Com `filter`, `debounceTime`, `distinctUntilChanged` e `switchMap`, seu código processa apenas sinais verdadeiros.
:::
