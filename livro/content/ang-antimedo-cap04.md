---
id: ang-antimedo-cap04
order: 304
kind: capitulo
num: 209
group: Angular · Parte 52 · Bloco I — A Anatomia do Medo
title: Estágios cerebrais do medo
subtitle: Ciclo de vida dos componentes — nascimento, mudança e morte
minutes: 30
hue: 20
badge: 🧬
- Mapear os 5 estágios cerebrais do medo para os lifecycle hooks do Angular: 
- Implementar ngOnInit, ngOnChanges, ngAfterViewInit e ngOnDestroy: 
- Saber exatamente quando cada hook é chamado e o que fazer em cada um: 
- Evitar memory leaks usando ngOnDestroy corretamente: 
origem: angular/livros/01-antimedo-angular/content/04-estagios-cerebrais.md
---

::: ilustracao img/ang-antimedo/cap04.svg | Um cérebro humano com suas regiões mapeadas para os lifecycle hooks do Angular.
:::

Pablo Marçal descreve com precisão os **5 estágios cerebrais do medo**. Não é magia, não é fraqueza — é neurociência. Seu cérebro tem um protocolo de segurança que foi programado há milhares de anos para te manter vivo.

Conhecer esse protocolo é o primeiro passo para hackear o medo.

Os 5 estágios são:

1. **Tálamo** — recebe o sinal bruto do ambiente (ruído ou ameaça)
2. **Córtex Pré-frontal** — interpreta o sinal e tenta dar sentido a ele
3. **Hipocampo** — consulta memórias passadas relacionadas
4. **Amígdala** — dispara o alarme emocional (medo, ansiedade)
5. **Hipotálamo** — comanda o corpo: lutar ou fugir

Um componente Angular tem exatamente o mesmo protocolo. Ele nasce, interpreta dados, consulta o DOM, reage a mudanças e, quando não é mais necessário, morre. Cada etapa tem um nome: **lifecycle hook**.

---

## Os Lifecycle Hooks do Angular

Um lifecycle hook é uma interface que você implementa para ser notificado de momentos específicos na vida do componente. Existem 8 hooks principais, mas focaremos nos 4 mais importantes:

| Hook | Estágio Cerebral | Quando é chamado |
|------|-----------------|-----------------|
| `ngOnChanges` | Tálamo | Antes do `ngOnInit` e quando `@Input` muda |
| `ngOnInit` | Córtex Pré-frontal | Uma vez, após a primeira inicialização |
| `ngAfterViewInit` | Hipocampo | Quando o template HTML foi renderizado |
| `ngOnDestroy` | Hipotálamo | Antes do componente ser destruído |

---

## ngOnChanges — o Tálamo (recebendo sinais)

O Tálamo é o primeiro a receber qualquer sinal externo. Ele não julga, não interpreta — apenas recebe e repassa. O `ngOnChanges` funciona igual: é o primeiro hook chamado, e é acionado **toda vez que um `@Input` muda**.

```typescript
import {
  Component,
  Input,
  OnChanges,
  SimpleChanges
} from '@angular/core';

@Component({
  selector: 'app-medidor-medo',
  standalone: true,
  template: `
    <div class="medidor">
      <h3>{{ titulo }}</h3>
      <p>Nível atual: {{ nivel }}/10</p>
      <p *ngIf="historico.length > 0">
        Mudanças detectadas: {{ historico.length }}
      </p>
      <ul>
        <li *ngFor="let h of historico">
          {{ h.de }} → {{ h.para }} ({{ h.quando | date:'HH:mm:ss' }})
        </li>
      </ul>
    </div>
  `
})
export class MedidorMedoComponent implements OnChanges {
  @Input({ required: true }) titulo!: string;
  @Input({ required: true }) nivel!: number;

  historico: Array<{ de: number; para: number; quando: Date }> = [];

  ngOnChanges(changes: SimpleChanges): void {
    // SimpleChanges é um mapa de todas as propriedades que mudaram
    if (changes['nivel']) {
      const mudanca = changes['nivel'];

      // firstChange: é a primeira vez que este input foi definido?
      if (!mudanca.firstChange) {
        this.historico.unshift({
          de: mudanca.previousValue,
          para: mudanca.currentValue,
          quando: new Date()
        });
      }

      console.log('Nível mudou:', mudanca.previousValue, '→', mudanca.currentValue);
    }
  }
}
```

::: nota
`SimpleChanges` contém objetos do tipo `SimpleChange` com três propriedades:
- `previousValue`: o valor anterior
- `currentValue`: o valor atual
- `firstChange`: `true` se é a primeira vez que o input foi definido

Use `firstChange` para evitar lógica desnecessária na inicialização.
:::

---

## ngOnInit — o Córtex Pré-frontal (interpretando)

O Córtex Pré-frontal recebe o sinal do Tálamo e começa a **dar sentido** ao que foi recebido. Ele busca contexto, analisa. O `ngOnInit` é o momento em que o componente "acorda" completamente e está pronto para executar sua lógica de inicialização.

```typescript
import {
  Component,
  OnInit,
  OnDestroy,
  Input
} from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-analise-medo',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="analise" *ngIf="!carregando; else loading">
      <h2>Análise do medo: {{ nomeMedo }}</h2>
      <ul>
        <li *ngFor="let causa of causas">{{ causa }}</li>
      </ul>
      <p><strong>Diagnóstico:</strong> {{ diagnostico }}</p>
    </div>
    <ng-template #loading>
      <p>🔍 Analisando seu medo...</p>
    </ng-template>
  `
})
export class AnaliseMedoComponent implements OnInit, OnDestroy {
  @Input({ required: true }) nomeMedo!: string;

  causas: string[] = [];
  diagnostico = '';
  carregando = true;
  private timer?: ReturnType<typeof setTimeout>;

  ngOnInit(): void {
    // Este é o lugar certo para:
    // ✅ Carregar dados iniciais
    // ✅ Configurar subscriptions
    // ✅ Inicializar estado complexo
    // ❌ NÃO use o constructor para isso

    console.log('Componente inicializado para analisar:', this.nomeMedo);

    this.timer = setTimeout(() => {
      this.causas = this.analisarCausas(this.nomeMedo);
      this.diagnostico = this.gerarDiagnostico(this.causas.length);
      this.carregando = false;
    }, 1500);
  }

  ngOnDestroy(): void {
    // Limpeza obrigatória para evitar memory leaks
    if (this.timer) {
      clearTimeout(this.timer);
    }
  }

  private analisarCausas(medo: string): string[] {
    // Simulação de análise
    return [
      `Experiência negativa passada com ${medo}`,
      `Falta de familiaridade com o tema`,
      `Comparação com outros desenvolvedores`,
      `Perfeccionismo: medo de errar`,
    ];
  }

  private gerarDiagnostico(numCausas: number): string {
    if (numCausas <= 2) return 'Medo leve — apenas comece!';
    if (numCausas <= 4) return 'Medo moderado — requer ação consciente';
    return 'Medo intenso — decomponha em partes menores';
  }
}
```

::: dica
**Por que não usar o `constructor` para inicialização?**
No `constructor`, os `@Input()` ainda não foram preenchidos — eles são `undefined`. O `ngOnInit` só é chamado depois que o Angular processou todos os bindings. Use o `constructor` apenas para injeção de dependências.
:::

---

## ngAfterViewInit — o Hipocampo (acessando memórias/DOM)

O Hipocampo consolida memórias e associa o sinal atual a experiências passadas. No Angular, o `ngAfterViewInit` é chamado quando o **template foi totalmente renderizado no DOM** — você pode agora acessar elementos HTML via `@ViewChild`.

```typescript
import {
  Component,
  AfterViewInit,
  ViewChild,
  ElementRef,
  OnDestroy
} from '@angular/core';

@Component({
  selector: 'app-foco-medo',
  standalone: true,
  template: `
    <div class="foco">
      <h2>🎯 Foco no Medo</h2>
      <canvas #grafico width="300" height="200"></canvas>
      <input #inputFoco type="text" placeholder="Escreva seu medo aqui" />
      <p>Tamanho do texto: {{ tamanhoTexto }}</p>
    </div>
  `
})
export class FocoMedoComponent implements AfterViewInit, OnDestroy {
  @ViewChild('grafico') canvasRef!: ElementRef<HTMLCanvasElement>;
  @ViewChild('inputFoco') inputRef!: ElementRef<HTMLInputElement>;

  tamanhoTexto = 0;
  private contexto?: CanvasRenderingContext2D;
  private listener?: () => void;

  ngAfterViewInit(): void {
    // DOM está disponível aqui — pode acessar elementos reais
    // ANTES do ngAfterViewInit, canvasRef e inputRef seriam undefined

    // Acessar o canvas e desenhar
    this.contexto = this.canvasRef.nativeElement.getContext('2d')!;
    this.desenharGrafico();

    // Dar foco automático ao input
    this.inputRef.nativeElement.focus();

    // Adicionar listener no elemento DOM
    this.listener = () => {
      this.tamanhoTexto = this.inputRef.nativeElement.value.length;
    };
    this.inputRef.nativeElement.addEventListener('input', this.listener);
  }

  ngOnDestroy(): void {
    // Remover listener para evitar memory leak
    if (this.listener) {
      this.inputRef.nativeElement.removeEventListener('input', this.listener);
    }
  }

  private desenharGrafico(): void {
    if (!this.contexto) return;
    const ctx = this.contexto;
    const canvas = this.canvasRef.nativeElement;

    // Limpar canvas
    ctx.clearRect(0, 0, canvas.width, canvas.height);

    // Desenhar barras representando níveis de medo
    const medos = [8, 6, 9, 4, 7, 3, 5];
    const larguraBarra = canvas.width / medos.length;

    medos.forEach((nivel, i) => {
      const altura = (nivel / 10) * canvas.height;
      const x = i * larguraBarra;
      const y = canvas.height - altura;

      ctx.fillStyle = `hsl(${340 - nivel * 15}, 70%, 60%)`;
      ctx.fillRect(x + 5, y, larguraBarra - 10, altura);
    });
  }
}
```

---

## ngOnDestroy — o Hipotálamo (o fim e a limpeza)

O Hipotálamo decide a resposta final: lutar ou fugir. No Angular, `ngOnDestroy` é a resposta final do componente antes de ser removido do DOM. É aqui que você **limpa tudo** para evitar memory leaks.

```typescript
import { Component, OnInit, OnDestroy } from '@angular/core';
import { Subject, interval } from 'rxjs';
import { takeUntil, map } from 'rxjs/operators';

@Component({
  selector: 'app-monitor-medo',
  standalone: true,
  template: `
    <div>
      <h2>📊 Monitor do Medo</h2>
      <p>Pulsos de medo detectados: {{ pulsos }}</p>
      <p>Último nível: {{ nivelAtual }}/10</p>
    </div>
  `
})
export class MonitorMedoComponent implements OnInit, OnDestroy {
  pulsos = 0;
  nivelAtual = 0;

  // Subject usado para cancelar subscriptions
  private destroy$ = new Subject<void>();

  ngOnInit(): void {
    // interval(1000) emite 0, 1, 2, 3... a cada segundo
    interval(1000).pipe(
      map(() => Math.floor(Math.random() * 10) + 1),
      takeUntil(this.destroy$)  // Cancela quando destroy$ emitir
    ).subscribe(nivel => {
      this.pulsos++;
      this.nivelAtual = nivel;
    });
  }

  ngOnDestroy(): void {
    // Emite um valor em destroy$ — cancela TODAS as subscriptions
    // que usam takeUntil(this.destroy$)
    this.destroy$.next();
    this.destroy$.complete();

    console.log('Componente destruído. Subscriptions canceladas. Sem memory leaks!');
  }
}
```

::: cuidado
**Memory leaks são como medos não resolvidos**: ficam crescendo em segundo plano até consumirem todos os seus recursos. Sempre cancele subscriptions, timers e event listeners no `ngOnDestroy`. O padrão `takeUntil(destroy$)` é o mais elegante para Observables.
:::

---

## Todos os hooks em sequência

```typescript
import {
  Component, Input,
  OnChanges, OnInit, DoCheck,
  AfterContentInit, AfterContentChecked,
  AfterViewInit, AfterViewChecked,
  OnDestroy, SimpleChanges
} from '@angular/core';

@Component({
  selector: 'app-ciclo-completo',
  standalone: true,
  template: `<p>{{ titulo }} — {{ valor }}</p>`
})
export class CicloCompletoComponent implements
  OnChanges, OnInit, DoCheck,
  AfterContentInit, AfterContentChecked,
  AfterViewInit, AfterViewChecked,
  OnDestroy {

  @Input() titulo = '';
  @Input() valor = 0;

  // 1. Chamado antes de ngOnInit e quando @Inputs mudam
  ngOnChanges(changes: SimpleChanges): void {
    console.log('1. ngOnChanges:', changes);
  }

  // 2. Chamado UMA VEZ após o primeiro ngOnChanges
  ngOnInit(): void {
    console.log('2. ngOnInit — componente inicializado');
  }

  // 3. Chamado em CADA ciclo de Change Detection
  ngDoCheck(): void {
    console.log('3. ngDoCheck — verificando mudanças');
  }

  // 4. Após conteúdo projetado (ng-content) ser inicializado
  ngAfterContentInit(): void {
    console.log('4. ngAfterContentInit');
  }

  // 5. Após cada verificação do conteúdo projetado
  ngAfterContentChecked(): void {
    console.log('5. ngAfterContentChecked');
  }

  // 6. Após o template do componente ser renderizado
  ngAfterViewInit(): void {
    console.log('6. ngAfterViewInit — DOM disponível');
  }

  // 7. Após cada verificação do template
  ngAfterViewChecked(): void {
    console.log('7. ngAfterViewChecked');
  }

  // 8. Antes da destruição do componente
  ngOnDestroy(): void {
    console.log('8. ngOnDestroy — limpando...');
  }
}
```

::: resumo
**O que aprendemos neste capítulo:**
- `ngOnChanges` → Tálamo: recebe sinais externos (`@Input` changes) com `SimpleChanges`
- `ngOnInit` → Córtex: inicializa lógica, carrega dados (não use o constructor para isso)
- `ngAfterViewInit` → Hipocampo: DOM disponível, acesse elementos com `@ViewChild`
- `ngOnDestroy` → Hipotálamo: limpeza obrigatória — cancele subscriptions, timers, listeners
- O padrão `takeUntil(destroy$)` é elegante para cancelar múltiplas subscriptions
:::

---

::: exercicio Componente com Ciclo de Vida Completo
Crie um componente `ContadorRegressivoComponent` que:
1. Receba um `@Input() segundosIniciais: number`
2. No `ngOnInit`, inicie a contagem regressiva com `setInterval`
3. No `ngOnChanges`, reinicie o contador se o input mudar
4. No `ngAfterViewInit`, dê foco a um botão de pausa via `@ViewChild`
5. No `ngOnDestroy`, limpe o intervalo e mostre um log

--- resposta
```typescript
import {
  Component, Input, OnChanges, OnInit,
  AfterViewInit, OnDestroy, ViewChild,
  ElementRef, SimpleChanges
} from '@angular/core';

@Component({
  selector: 'app-contador-regressivo',
  standalone: true,
  template: `
    <div class="contador">
      <h2>⏳ Contagem Regressiva do Medo</h2>
      <p class="numero">{{ segundosRestantes }}</p>
      <p *ngIf="segundosRestantes === 0" class="fim">
        🎉 Tempo esgotado — o medo passou!
      </p>
      <button #btnPausa (click)="togglePausa()">
        {{ pausado ? '▶ Continuar' : '⏸ Pausar' }}
      </button>
    </div>
  `,
  styles: [`.numero { font-size: 4rem; color: #e91e63; }`]
})
export class ContadorRegressivoComponent implements
  OnChanges, OnInit, AfterViewInit, OnDestroy {

  @Input({ required: true }) segundosIniciais!: number;
  @ViewChild('btnPausa') btnPausa!: ElementRef<HTMLButtonElement>;

  segundosRestantes = 0;
  pausado = false;
  private intervalo?: ReturnType<typeof setInterval>;

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['segundosIniciais'] && !changes['segundosIniciais'].firstChange) {
      this.reiniciar();
    }
  }

  ngOnInit(): void {
    this.segundosRestantes = this.segundosIniciais;
    this.iniciarIntervalo();
  }

  ngAfterViewInit(): void {
    this.btnPausa.nativeElement.focus();
  }

  ngOnDestroy(): void {
    this.pararIntervalo();
    console.log('ContadorRegressivo destruído — intervalo limpo');
  }

  togglePausa(): void {
    this.pausado = !this.pausado;
  }

  private iniciarIntervalo(): void {
    this.intervalo = setInterval(() => {
      if (!this.pausado && this.segundosRestantes > 0) {
        this.segundosRestantes--;
        if (this.segundosRestantes === 0) {
          this.pararIntervalo();
        }
      }
    }, 1000);
  }

  private pararIntervalo(): void {
    if (this.intervalo) {
      clearInterval(this.intervalo);
      this.intervalo = undefined;
    }
  }

  private reiniciar(): void {
    this.pararIntervalo();
    this.segundosRestantes = this.segundosIniciais;
    this.pausado = false;
    this.iniciarIntervalo();
  }
}
```
:::

---

::: quiz
? Em qual lifecycle hook você deve carregar dados de uma API?
- No `constructor` do componente
+ No `ngOnInit`, após os inputs estarem disponíveis
- No `ngAfterViewInit`, após o DOM estar renderizado
- No `ngOnChanges`, para reagir a mudanças de input
> O `ngOnInit` é o lugar certo: os `@Input()` já foram inicializados e o componente está pronto. O `constructor` não tem `@Input()` ainda; o `ngAfterViewInit` é tardio demais para inicialização.

? Qual é o propósito principal do `ngOnDestroy`?
- Animar a remoção do componente do DOM
- Salvar o estado do componente antes de destruí-lo
+ Limpar recursos (subscriptions, timers, listeners) para evitar memory leaks
- Notificar o componente pai sobre a destruição
> O `ngOnDestroy` é o momento certo para limpar qualquer recurso que o componente criou: `clearInterval`, `clearTimeout`, cancelar subscriptions RxJS, remover event listeners. Sem isso, recursos continuam rodando mesmo após o componente ser destruído — memory leak.

? O que contém o objeto `SimpleChanges` passado para `ngOnChanges`?
- Apenas o valor atual de cada input modificado
+ Um mapa de todas as propriedades @Input que mudaram, com `previousValue`, `currentValue` e `firstChange`
- O nome do componente filho que disparou a mudança
- A lista de todos os @Input do componente, modificados ou não
> `SimpleChanges` é um objeto onde cada chave é o nome de um `@Input` que mudou. O valor associado é um `SimpleChange` com `previousValue`, `currentValue` (novo valor) e `firstChange` (se é a primeira vez).

? Em que momento o `@ViewChild` fica disponível (não undefined)?
- No `ngOnInit`
- No `ngOnChanges`
+ No `ngAfterViewInit`
- No `constructor`
> `@ViewChild` referencia elementos do template HTML. Esses elementos só existem no DOM após o template ser renderizado, o que acontece no `ngAfterViewInit`. Tentar acessar `@ViewChild` antes disso retornará `undefined`.

? Qual é o padrão recomendado para cancelar múltiplas subscriptions RxJS no `ngOnDestroy`?
- Chamar `.unsubscribe()` em cada subscription separadamente
- Usar `clearInterval` em cada observable
+ Usar `Subject` + `takeUntil(destroy$)` e emitir no `ngOnDestroy`
- Usar `setTimeout` para atrasar a destruição
> O padrão `takeUntil(destroy$)` é elegante: cria um `Subject`, usa `takeUntil(this.destroy$)` em cada pipe de observable, e no `ngOnDestroy` emite `this.destroy$.next()` para cancelar todas de uma vez.
:::

---

::: cartas
O que é `ngOnChanges`? :: Hook chamado antes do `ngOnInit` e sempre que um `@Input` recebe um novo valor. Recebe `SimpleChanges` com os valores anterior e atual.
O que é `ngOnInit`? :: Hook chamado uma vez após a inicialização do componente, quando todos os `@Input()` já estão disponíveis. Ideal para carregar dados e configurar state.
O que é `ngAfterViewInit`? :: Hook chamado após o template do componente ser renderizado no DOM. É quando `@ViewChild` e `@ViewChildren` ficam disponíveis.
O que é `ngOnDestroy`? :: Hook chamado antes do componente ser destruído. Usado para limpar subscriptions, timers e event listeners — prevenindo memory leaks.
O que é `SimpleChanges`? :: Objeto passado para `ngOnChanges` contendo as propriedades @Input que mudaram, com `previousValue`, `currentValue` e `firstChange`.
O que é `firstChange` em um `SimpleChange`? :: Propriedade booleana que indica se é a primeira vez que o `@Input` foi definido. `true` na criação, `false` nas mudanças subsequentes.
Por que não usar o `constructor` para carregar dados? :: Porque no `constructor`, os `@Input()` ainda não foram populados pelo Angular — eles são `undefined`. Use `ngOnInit`.
O que é o padrão `takeUntil(destroy$)`? :: Padrão RxJS para cancelar múltiplas subscriptions: usa um `Subject` como `destroy$`, aplica `takeUntil(this.destroy$)` nos pipes e emite no `ngOnDestroy`.
:::
