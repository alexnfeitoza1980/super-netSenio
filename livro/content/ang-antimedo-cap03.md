---
id: ang-antimedo-cap03
order: 303
kind: capitulo
num: 208
group: Angular · Parte 52 · Bloco I — A Anatomia do Medo
title: Medo × Medo — Prudência vs. Paralisia
subtitle: ChangeDetection.OnPush — detectar apenas mudanças reais
minutes: 28
hue: 10
badge: ⚖️
- Compreender como o Angular detecta mudanças por padrão: 
- Entender o ChangeDetectionStrategy.OnPush e seus benefícios: 
- Usar o ChangeDetectorRef para controle manual: 
- Distinguir quando reagir (prudência) de quando paralisar (medo): 
origem: angular/livros/01-antimedo-angular/content/03-medo-x-medo.md
---

::: ilustracao img/ang-antimedo/cap03.svg | Uma balança com medo de um lado e prudência do outro, sobre um circuito de detecção de mudanças.
:::

Pablo Marçal faz uma distinção que parece simples, mas muda tudo:

**Medo não é o mesmo que prudência.**

O medo paralisa. Ele reage a qualquer sinal, real ou imaginário, e te congela no lugar. A pessoa com medo não sai da cama porque "pode dar errado". Não investe porque "e se perder tudo". Não começa porque "ainda não está pronto".

A prudência protege. Ela avalia o sinal, verifica se é real, e age com cuidado — mas age. O prudente usa capacete na moto porque o risco de acidentes é real. Mas não deixa de andar de moto por isso.

Pablo diz: **instale o driver de prudência, não o driver de medo**.

O Angular tem exatamente esse mesmo dilema interno. Por padrão, ele age com medo: reage a tudo. Mas você pode ensiná-lo a agir com prudência: reagir só ao que realmente mudou.

---

## Change Detection: como o Angular "sente" o mundo

Angular precisa saber quando atualizar a tela. Mas como ele detecta que algo mudou?

Isso se chama **Change Detection** (Detecção de Mudanças). O Angular monitora eventos e, quando algo acontece, verifica todos os componentes para ver se precisam ser re-renderizados.

Por padrão, ele verifica **tudo**. Qualquer evento — clique, requisição HTTP, `setTimeout`, `setInterval` — dispara uma varredura em toda a árvore de componentes. Isso é o **medo**: reagir a qualquer coisa.

```
Evento ocorre
     ↓
Angular varre TODA a árvore de componentes
     ↓
Para cada componente: "alguma propriedade mudou?"
     ↓
Se sim: re-renderiza o componente
```

Para aplicações pequenas, isso não é problema. Para aplicações grandes com dezenas de componentes, esse comportamento pode ser um pesadelo de performance.

### Demonstrando o problema

```typescript
import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-lista-medos',
  standalone: true,
  imports: [CommonModule],
  template: `
    <ul>
      <li *ngFor="let medo of medos">{{ medo }}</li>
    </ul>
    <p>Renderizações: {{ contadorRenders }}</p>
  `
})
export class ListaMedosComponent implements OnInit {
  medos = ['Errar na frente do cliente', 'O código não funcionar em prod'];
  contadorRenders = 0;

  ngDoCheck(): void {
    // Este método é chamado TODA vez que o Angular faz change detection
    this.contadorRenders++;
    console.log('Change detection rodou!', this.contadorRenders, 'vezes');
  }

  ngOnInit(): void {
    // Simula eventos externos que não mudam este componente
    setInterval(() => {
      // Mesmo que nada mude aqui, o Angular vai verificar este componente
    }, 1000);
  }
}
```

::: cuidado
Com a estratégia padrão (`ChangeDetectionStrategy.Default`), o `ngDoCheck` pode ser chamado **dezenas de vezes por segundo** em uma aplicação real, mesmo quando os dados do componente não mudaram. Isso é o "medo" do Angular: verificar tudo sempre.
:::

---

## ChangeDetectionStrategy.OnPush — a prudência

`OnPush` é o "driver de prudência" do Angular. Com essa estratégia, o componente **só é verificado** quando:

1. Uma das suas `@Input()` propriedades recebe uma **nova referência** (não apenas uma mutação)
2. Um evento ocorre **dentro** do próprio componente
3. Um `Observable` subscrito via `async` pipe emite um novo valor
4. Você manualmente dispara a detecção via `ChangeDetectorRef`

```typescript
import { Component, Input, ChangeDetectionStrategy } from '@angular/core';
import { CommonModule } from '@angular/common';

interface Medo {
  id: number;
  descricao: string;
  nivel: number;
}

@Component({
  selector: 'app-card-medo',
  standalone: true,
  imports: [CommonModule],
  changeDetection: ChangeDetectionStrategy.OnPush,  // ← A chave!
  template: `
    <div class="card">
      <h3>{{ medo.descricao }}</h3>
      <div class="nivel">
        <span *ngFor="let i of niveis">
          {{ i <= medo.nivel ? '🔴' : '⚪' }}
        </span>
      </div>
      <p>Nível: {{ medo.nivel }}/10</p>
    </div>
  `,
  styles: [`
    .card { border: 1px solid #ccc; padding: 1rem; border-radius: 8px; margin: 0.5rem; }
    .nivel { font-size: 1.2rem; }
  `]
})
export class CardMedoComponent {
  @Input({ required: true }) medo!: Medo;

  niveis = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10];
}
```

### Imutabilidade: a regra de ouro do OnPush

Com `OnPush`, **mutações diretas não disparam re-renderização**. Você precisa criar um novo objeto:

```typescript
import { Component, ChangeDetectionStrategy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CardMedoComponent } from './card-medo.component';

interface Medo {
  id: number;
  descricao: string;
  nivel: number;
}

@Component({
  selector: 'app-gerenciador-medos',
  standalone: true,
  imports: [CommonModule, CardMedoComponent],
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <div>
      <app-card-medo
        *ngFor="let medo of medos"
        [medo]="medo"
      />
      <button (click)="aumentarNivel(0)">Aumentar nível do primeiro medo</button>
    </div>
  `
})
export class GerenciadorMedosComponent {
  medos: Medo[] = [
    { id: 1, descricao: 'Apresentar código para o líder técnico', nivel: 7 },
    { id: 2, descricao: 'Fazer deploy às sextas-feiras', nivel: 9 },
    { id: 3, descricao: 'Ler código legado sem documentação', nivel: 6 },
  ];

  aumentarNivel(indice: number): void {
    // ❌ ERRADO com OnPush: mutação direta não dispara re-render
    // this.medos[indice].nivel++;

    // ✅ CORRETO: criar nova referência
    this.medos = this.medos.map((medo, i) =>
      i === indice
        ? { ...medo, nivel: Math.min(10, medo.nivel + 1) }
        : medo
    );
  }
}
```

::: nota
O operador spread `{ ...medo, nivel: novoNivel }` cria um **novo objeto** com as mesmas propriedades, mas com `nivel` atualizado. Isso é fundamental com `OnPush`: uma nova referência de objeto dispara a re-renderização; uma mutação do mesmo objeto, não.
:::

---

## ChangeDetectorRef — controle manual

Às vezes você precisa disparar a detecção manualmente. Para isso existe o `ChangeDetectorRef`:

```typescript
import {
  Component,
  ChangeDetectionStrategy,
  ChangeDetectorRef,
  OnInit,
  OnDestroy
} from '@angular/core';

@Component({
  selector: 'app-relogio-medo',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <div class="relogio">
      <h3>⏱ Tempo enfrentando seus medos</h3>
      <p class="tempo">{{ tempoFormatado }}</p>
      <button (click)="togglePausa()">
        {{ pausado ? '▶ Continuar' : '⏸ Pausar' }}
      </button>
      <button (click)="reiniciar()">🔄 Reiniciar</button>
    </div>
  `,
  styles: [`
    .relogio { text-align: center; padding: 2rem; }
    .tempo { font-size: 3rem; font-weight: bold; color: #e91e63; }
  `]
})
export class RelogioMedoComponent implements OnInit, OnDestroy {
  segundos = 0;
  pausado = false;
  private intervalo?: ReturnType<typeof setInterval>;

  constructor(private cdr: ChangeDetectorRef) {}

  get tempoFormatado(): string {
    const horas = Math.floor(this.segundos / 3600);
    const minutos = Math.floor((this.segundos % 3600) / 60);
    const segs = this.segundos % 60;
    return [horas, minutos, segs]
      .map(v => v.toString().padStart(2, '0'))
      .join(':');
  }

  ngOnInit(): void {
    this.iniciarIntervalo();
  }

  ngOnDestroy(): void {
    this.pararIntervalo();
  }

  private iniciarIntervalo(): void {
    this.intervalo = setInterval(() => {
      if (!this.pausado) {
        this.segundos++;
        // Com OnPush, setInterval não dispara CD automaticamente
        // Precisamos disparar manualmente:
        this.cdr.markForCheck();
      }
    }, 1000);
  }

  private pararIntervalo(): void {
    if (this.intervalo) {
      clearInterval(this.intervalo);
    }
  }

  togglePausa(): void {
    this.pausado = !this.pausado;
  }

  reiniciar(): void {
    this.segundos = 0;
    this.pausado = false;
  }
}
```

### Os métodos do ChangeDetectorRef

| Método | O que faz |
|--------|-----------|
| `markForCheck()` | Marca o componente para ser verificado no próximo ciclo de CD |
| `detectChanges()` | Dispara CD imediatamente neste componente e seus filhos |
| `detach()` | Remove o componente da árvore de CD (controle total) |
| `reattach()` | Reconecta o componente à árvore de CD |

::: dica
**`markForCheck()` vs `detectChanges()`:** Use `markForCheck()` na maioria dos casos — ele agenda a verificação para o próximo ciclo, o que é mais eficiente. Use `detectChanges()` quando precisa de atualização **imediata**, como em integrações com bibliotecas externas.
:::

---

## Comparativo: Default vs OnPush

| Característica | Default | OnPush |
|----------------|---------|--------|
| Quando verifica | Qualquer evento em qualquer lugar | Apenas quando inputs mudam ou evento interno ocorre |
| Performance | Menor (verifica tudo) | Maior (verifica só o necessário) |
| Complexidade | Simples | Requer atenção à imutabilidade |
| Ideal para | Componentes simples, protótipos | Componentes de lista, alta frequência de eventos |
| Analogia | Medo: reage a tudo | Prudência: reage só ao necessário |

---

::: passos Implementando OnPush do zero
=== Passo 1: Crie um componente com Default
Crie um componente simples sem `OnPush`. Adicione um `console.log` no `ngDoCheck` para ver quantas vezes ele é chamado. Rode a aplicação e clique em qualquer botão na página.
=== Passo 2: Adicione ChangeDetectionStrategy.OnPush
Adicione `changeDetection: ChangeDetectionStrategy.OnPush` ao decorator. Observe que o `ngDoCheck` agora é chamado muito menos vezes.
=== Passo 3: Teste a imutabilidade
Tente mutar um array diretamente (`this.lista.push(item)`) e veja que o template não atualiza. Depois corrija para `this.lista = [...this.lista, item]` e veja funcionar.
=== Passo 4: Use markForCheck quando necessário
Se precisar atualizar a partir de um `setInterval` ou callback externo, injete `ChangeDetectorRef` e chame `this.cdr.markForCheck()` após a mudança.
:::

::: resumo
**O que aprendemos neste capítulo:**
- Change Detection Default verifica todos os componentes em qualquer evento (medo)
- `ChangeDetectionStrategy.OnPush` verifica só quando necessário (prudência)
- Com `OnPush`, use imutabilidade: crie novos objetos em vez de mutá-los
- `ChangeDetectorRef.markForCheck()` dispara a verificação manualmente quando necessário
- A prudência protege sem paralisar; o `OnPush` performa sem omitir
:::

---

::: exercicio Componente de Progresso com OnPush
Crie um componente `ProgressoComponent` com `OnPush` que:
1. Receba uma lista de tarefas como `@Input()`
2. Mostre o percentual de conclusão
3. Permita marcar tarefas como concluídas (atualizando imutavelmente)
4. Mostre uma mensagem de parabéns quando todas forem concluídas

--- resposta
```typescript
import { Component, Input, ChangeDetectionStrategy } from '@angular/core';
import { CommonModule } from '@angular/common';

export interface Tarefa {
  id: number;
  descricao: string;
  concluida: boolean;
}

@Component({
  selector: 'app-progresso',
  standalone: true,
  imports: [CommonModule],
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <div class="progresso-container">
      <h2>📈 Meu Progresso</h2>

      <div class="barra">
        <div class="barra-preenchimento" [style.width.%]="percentual"></div>
      </div>
      <p>{{ percentual | number:'1.0-0' }}% concluído</p>

      <div *ngIf="todasConcluidas" class="parabens">
        🎉 Parabéns! Você superou todos os seus medos!
      </div>

      <ul>
        <li *ngFor="let tarefa of tarefas">
          <label>
            <input
              type="checkbox"
              [checked]="tarefa.concluida"
              (change)="toggleTarefa(tarefa)"
            />
            <span [class.concluida]="tarefa.concluida">
              {{ tarefa.descricao }}
            </span>
          </label>
        </li>
      </ul>
    </div>
  `,
  styles: [`
    .barra { background: #eee; height: 20px; border-radius: 10px; overflow: hidden; }
    .barra-preenchimento { height: 100%; background: #e91e63; transition: width 0.3s; }
    .concluida { text-decoration: line-through; color: #999; }
    .parabens { background: #4caf50; color: white; padding: 1rem; border-radius: 8px; }
  `]
})
export class ProgressoComponent {
  @Input() tarefas: Tarefa[] = [];

  get percentual(): number {
    if (this.tarefas.length === 0) return 0;
    return (this.tarefas.filter(t => t.concluida).length / this.tarefas.length) * 100;
  }

  get todasConcluidas(): boolean {
    return this.tarefas.length > 0 && this.tarefas.every(t => t.concluida);
  }

  toggleTarefa(tarefa: Tarefa): void {
    // Imutável: criar novo array com novo objeto
    this.tarefas = this.tarefas.map(t =>
      t.id === tarefa.id ? { ...t, concluida: !t.concluida } : t
    );
  }
}
```
:::

---

::: quiz
? Qual é a estratégia de Change Detection padrão do Angular?
+ `ChangeDetectionStrategy.Default` — verifica todos os componentes em qualquer evento
- `ChangeDetectionStrategy.OnPush` — verifica apenas quando inputs mudam
- `ChangeDetectionStrategy.Lazy` — verifica apenas componentes visíveis
- `ChangeDetectionStrategy.Manual` — só verifica quando chamado explicitamente
> Por padrão, o Angular usa `Default`, que varre toda a árvore de componentes em qualquer evento (clique, timer, requisição HTTP, etc.).

? Quais situações disparam a re-verificação em um componente `OnPush`?
- Qualquer evento do mouse na página
+ Novo valor de `@Input()`, evento interno, `async` pipe emitindo, `markForCheck()`
- Apenas quando o componente é criado pela primeira vez
- Apenas chamadas HTTP completadas
> Com `OnPush`, o Angular só verifica o componente em situações específicas: nova referência em `@Input`, eventos dentro do componente, observables via `async` pipe, ou chamada manual de `markForCheck()`.

? Por que `this.lista.push(item)` não atualiza a tela com OnPush?
+ Porque `push` muta o array original sem criar uma nova referência, e OnPush compara referências
- Porque `push` é muito lento para o Angular detectar
- Porque arrays não funcionam com OnPush
- Porque o Angular não aceita arrays em componentes OnPush
> `OnPush` compara referências de objetos (não os valores internos). `push` muta o mesmo array, mantendo a mesma referência. O Angular não detecta a mudança porque a referência é idêntica.

? Qual método do `ChangeDetectorRef` você usaria para atualizar a tela imediatamente, sem esperar o próximo ciclo?
- `markForCheck()`
- `reattach()`
+ `detectChanges()`
- `checkNoChanges()`
> `detectChanges()` dispara a Change Detection imediatamente naquele componente e seus filhos. `markForCheck()` apenas agenda para o próximo ciclo, sendo mais eficiente para a maioria dos casos.

? Qual é a analogia com o livro Antimedo para a diferença entre Default e OnPush?
+ Default = medo (reage a tudo); OnPush = prudência (reage só ao necessário)
- Default = coragem; OnPush = covardia
- Default = ação; OnPush = inércia
- Default = ruído; OnPush = sinal
> Exatamente como Pablo distingue medo de prudência: o medo (Default) reage a qualquer sinal, real ou imaginário. A prudência (OnPush) avalia antes de reagir, economizando energia para o que realmente importa.
:::

---

::: cartas
O que é Change Detection? :: O mecanismo pelo qual o Angular verifica se o estado do componente mudou e decide se precisa atualizar o DOM.
O que é `ChangeDetectionStrategy.Default`? :: A estratégia padrão onde o Angular verifica todos os componentes em qualquer evento — como o medo que reage a tudo.
O que é `ChangeDetectionStrategy.OnPush`? :: A estratégia otimizada onde o Angular só verifica o componente quando um `@Input` muda de referência, um evento interno ocorre ou `markForCheck()` é chamado.
Por que imutabilidade é importante com OnPush? :: Porque o OnPush compara referências de objetos. Mutações diretas mantêm a mesma referência e não disparam re-renderização.
O que faz `ChangeDetectorRef.markForCheck()`? :: Marca o componente para ser verificado no próximo ciclo de Change Detection — útil com timers e callbacks externos.
O que faz `ChangeDetectorRef.detectChanges()`? :: Dispara Change Detection imediatamente naquele componente e seus filhos, sem esperar o próximo ciclo.
Como criar um novo objeto imutavelmente em TypeScript? :: Com o operador spread: `const novo = { ...original, propriedadeAlterada: novoValor };`
Qual decorator é verificado por OnPush ao receber nova referência? :: `@Input()` — quando um componente pai passa uma nova referência de objeto para uma propriedade `@Input`, o OnPush dispara a verificação.
:::
