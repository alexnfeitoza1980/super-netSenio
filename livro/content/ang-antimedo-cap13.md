---
id: ang-antimedo-cap13
order: 313
kind: capitulo
num: 218
group: Angular · Parte 54 · Bloco III — A Transformação
title: Autoimagem
subtitle: Angular Signals — o estado que o componente tem de si mesmo
minutes: 35
hue: 310
badge: 🪞
- Compreender como a autoimagem determina comportamentos e resultados: 
- Criar e usar Signals com signal(), computed() e effect(): 
- Comparar Signals com BehaviorSubject e entender as vantagens: 
- Usar toSignal() e toObservable() para interoperabilidade: 
origem: angular/livros/01-antimedo-angular/content/13-autoimagem.md
---

# Autoimagem

> *"Autoimagem é a percepção que você tem de si mesmo. Ela determina seus resultados."*
> — Pablo Marçal

Você não age de acordo com a realidade. Você age de acordo com a **percepção que tem de si mesmo dentro da realidade**. Essa distinção muda tudo.

Duas pessoas com o mesmo talento, na mesma situação, com as mesmas oportunidades — terão resultados completamente diferentes se tiverem autoimagens diferentes. A pessoa que se vê como vencedor age como vencedor. A que se vê como fracassado sabota inconscientemente cada oportunidade.

Em Angular, cada componente também tem uma "autoimagem" — seu **estado interno**. E assim como a autoimagem humana determina todos os comportamentos derivados, o estado do componente determina tudo que é renderizado e como a aplicação se comporta.

No Angular 16+, esse conceito ganhou uma representação perfeita: os **Signals**.

::: nota
**Signal** é uma variável reativa que o Angular "observa" diretamente. Quando o signal muda, apenas os componentes que o usam são re-renderizados — sem dirty checking global, sem detecção de mudanças desnecessária.
:::

---

## O problema antes dos Signals

Antes dos Signals, gerenciar estado reativo em Angular exigia `BehaviorSubject` + `async` pipe, ou `ChangeDetectionStrategy.OnPush` com complexidade crescente:

```typescript
// ANTES: Autoimagem gerenciada com BehaviorSubject
import { BehaviorSubject } from 'rxjs';

@Component({ selector: 'app-usuario', template: `
  <p>{{ nome$ | async }}</p>
  <p>{{ nivelConfianca$ | async }}</p>
` })
export class UsuarioComponent {
  private _nome = new BehaviorSubject<string>('');
  private _nivelConfianca = new BehaviorSubject<number>(0);

  nome$ = this._nome.asObservable();
  nivelConfianca$ = this._nivelConfianca.asObservable();

  atualizarNome(novoNome: string): void {
    this._nome.next(novoNome); // Verboso
  }
}
```

---

## Signals — a autoimagem que se conhece e se atualiza

```typescript
// AGORA: Autoimagem gerenciada com Signals
import { signal, computed, effect } from '@angular/core';

@Component({
  selector: 'app-usuario',
  standalone: true,
  template: `
    <p>{{ nome() }}</p>
    <p>{{ nivelConfianca() }}</p>
    <p>{{ categoria() }}</p>
  `
})
export class UsuarioComponent {
  // Signal = autoimagem central
  nome = signal<string>('');
  nivelConfianca = signal<number>(0);

  // computed = comportamento derivado da autoimagem
  categoria = computed(() => {
    const nivel = this.nivelConfianca();
    if (nivel >= 8) return '🏆 Líder';
    if (nivel >= 5) return '📈 Em crescimento';
    return '🌱 Iniciante';
  });

  atualizarNome(novoNome: string): void {
    this.nome.set(novoNome); // Limpo e direto
  }

  aumentarConfianca(): void {
    this.nivelConfianca.update(n => n + 1); // update deriva do valor atual
  }
}
```

---

## As três operações de um Signal

```typescript
import { signal } from '@angular/core';

const autoimagem = signal<number>(5); // Cria com valor inicial

// 1. LEITURA — como você se vê agora
console.log(autoimagem()); // 5

// 2. SET — define diretamente (transformação consciente)
autoimagem.set(8); // "Decidi que valho 8"

// 3. UPDATE — deriva do valor atual (evolução progressiva)
autoimagem.update(valor => valor + 1); // "Cresci um ponto"
console.log(autoimagem()); // 9
```

::: dica
Use `.set()` quando você tem o novo valor completo. Use `.update()` quando o novo valor depende do valor atual — como na evolução progressiva da autoimagem: você parte de onde está.
:::

---

## `computed()` — comportamentos derivados da autoimagem

Assim como a autoimagem determina comportamentos (confiança → atitude → resultados), `computed()` cria valores que derivam automaticamente dos signals:

```typescript
// src/app/features/perfil/perfil.component.ts
import { Component, signal, computed, effect } from '@angular/core';

interface Conquista {
  nome: string;
  pontos: number;
}

@Component({
  selector: 'app-perfil',
  standalone: true,
  template: `
    <div class="perfil">
      <h2>{{ nomeCompleto() }}</h2>
      <p class="nivel">{{ nivel() }}</p>
      <p class="pontos">{{ totalPontos() }} pontos</p>
      <ul>
        @for (c of conquistas(); track c.nome) {
          <li>{{ c.nome }} — {{ c.pontos }}pts</li>
        }
      </ul>
    </div>
  `
})
export class PerfilComponent {
  // Autoimagem: identidade
  nome = signal('');
  sobrenome = signal('');
  conquistas = signal<Conquista[]>([]);

  // Derivados: comportamentos que emergem da autoimagem
  nomeCompleto = computed(() =>
    `${this.nome()} ${this.sobrenome()}`.trim()
  );

  totalPontos = computed(() =>
    this.conquistas().reduce((soma, c) => soma + c.pontos, 0)
  );

  nivel = computed(() => {
    const pontos = this.totalPontos();
    if (pontos >= 1000) return '🥇 Mestre';
    if (pontos >= 500) return '🥈 Avançado';
    if (pontos >= 100) return '🥉 Intermediário';
    return '🌱 Iniciante';
  });

  adicionarConquista(conquista: Conquista): void {
    this.conquistas.update(lista => [...lista, conquista]);
    // Automaticamente: totalPontos() e nivel() se atualizam
  }
}
```

---

## `effect()` — reações a mudanças na autoimagem

`effect()` executa uma função toda vez que qualquer signal lido dentro dela muda. É o equivalente a um "efeito colateral" — como quando sua autoimagem muda e você naturalmente começa a agir diferente.

```typescript
import { effect, signal, Component, OnInit } from '@angular/core';

@Component({ selector: 'app-sessao', standalone: true, template: '' })
export class SessaoComponent implements OnInit {
  private autoimagem = signal<number>(5);
  private nome = signal<string>('');

  ngOnInit(): void {
    // Effect: toda vez que autoimagem ou nome mudam, salva no storage
    effect(() => {
      const dados = {
        nivel: this.autoimagem(),
        nome: this.nome(),
        timestamp: new Date().toISOString()
      };
      localStorage.setItem('sessao_usuario', JSON.stringify(dados));
      console.log('[Sessao] Estado salvo:', dados);
    });
  }
}
```

::: cuidado
`effect()` deve ser criado dentro de um contexto de injeção (construtor, `ngOnInit` com `inject()`, ou `runInInjectionContext()`). Evite criar efeitos em loops ou condicionais — isso pode causar comportamento imprevisível.
:::

---

## Comparação: Signal vs BehaviorSubject

| Aspecto | `signal()` | `BehaviorSubject` |
|---------|-----------|------------------|
| Leitura no template | `valor()` | `valor$ \| async` |
| Atualização | `.set()` / `.update()` | `.next()` |
| Derivação | `computed()` | `map()` + async pipe |
| Efeito colateral | `effect()` | `subscribe()` |
| Lazy evaluation | ✅ Sim | ❌ Não |
| Sem assinatura manual | ✅ Sim | ❌ Precisa de `takeUntil` |
| Granularidade de update | ✅ Componente específico | ⚠️ Zona inteira |

---

## `toSignal()` e `toObservable()` — interoperabilidade

O mundo Angular tem muito código com Observables. `toSignal` e `toObservable` fazem a ponte:

```typescript
// src/app/features/busca/busca.component.ts
import { Component, signal, computed } from '@angular/core';
import { toSignal, toObservable } from '@angular/core/rxjs-interop';
import { debounceTime, switchMap, startWith } from 'rxjs/operators';
import { inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-busca',
  standalone: true,
  imports: [FormsModule],
  template: `
    <input [(ngModel)]="termoBusca" placeholder="Buscar medo..." />
    <p>Resultados: {{ resultados().length }}</p>
    @for (r of resultados(); track r.id) {
      <p>{{ r.descricao }}</p>
    }
  `
})
export class BuscaComponent {
  private http = inject(HttpClient);

  termoBusca = signal('');

  // Observable → Signal (com valor inicial e limpeza automática)
  resultados = toSignal(
    toObservable(this.termoBusca).pipe(
      debounceTime(300),
      startWith(''),
      switchMap(termo =>
        this.http.get<any[]>(`/api/medos?q=${termo}`)
      )
    ),
    { initialValue: [] }
  );
}
```

---

::: resumo
**Autoimagem** em Angular = o estado central do componente, representado por `signal()`.

- `signal(valorInicial)` — cria o estado reativo
- `computed(() => ...)` — deriva valores automaticamente (nunca fica desatualizado)
- `effect(() => ...)` — reage a mudanças (efeitos colaterais)
- `.set(novoValor)` — substitui o valor
- `.update(fn)` — transforma o valor atual
- `toSignal(obs$)` — converte Observable para Signal
- `toObservable(sig)` — converte Signal para Observable

Quando a autoimagem (signal) muda, tudo que depende dela (computed) se atualiza automaticamente. Sem esforço. Sem `subscribe`. Sem `takeUntil`.
:::

---

::: exercicio Signal de autoimagem com persistência
Crie um `PerfilService` usando Signals que:
1. Armazena `nome`, `nivel` (1-10) e `conquistas` como signals
2. Expõe `nivelDescricao` como `computed`
3. Persiste automaticamente no `localStorage` com `effect`
4. Restaura os dados no `constructor`

--- resposta

```typescript
// src/app/core/services/perfil.service.ts
import { Injectable, signal, computed, effect } from '@angular/core';

interface PerfilData {
  nome: string;
  nivel: number;
  conquistas: string[];
}

@Injectable({ providedIn: 'root' })
export class PerfilService {
  private readonly CHAVE = 'perfil_antimedo';

  nome = signal<string>('');
  nivel = signal<number>(1);
  conquistas = signal<string[]>([]);

  nivelDescricao = computed(() => {
    const n = this.nivel();
    const descricoes = [
      '', '🌱 Semente', '🌿 Broto', '🌲 Árvore',
      '⚡ Ativo', '🔥 Determinado', '💪 Forte',
      '🎯 Focado', '🏆 Líder', '👑 Mestre', '🦁 Antimedo'
    ];
    return descricoes[Math.min(n, 10)];
  });

  totalConquistas = computed(() => this.conquistas().length);

  constructor() {
    this.restaurarDados();

    effect(() => {
      const dados: PerfilData = {
        nome: this.nome(),
        nivel: this.nivel(),
        conquistas: this.conquistas()
      };
      localStorage.setItem(this.CHAVE, JSON.stringify(dados));
    });
  }

  private restaurarDados(): void {
    const json = localStorage.getItem(this.CHAVE);
    if (json) {
      const dados: PerfilData = JSON.parse(json);
      this.nome.set(dados.nome);
      this.nivel.set(dados.nivel);
      this.conquistas.set(dados.conquistas);
    }
  }

  evoluir(): void {
    this.nivel.update(n => Math.min(n + 1, 10));
  }

  adicionarConquista(nome: string): void {
    this.conquistas.update(lista => [...lista, nome]);
    this.evoluir();
  }
}
```

:::

---

::: quiz

? Como se lê o valor atual de um signal no TypeScript?

- `signal.value`
- `signal.get()`
+ `signal()` — chamando-o como função
- `signal.subscribe()`

> Signals são implementados como funções getter. Chamar `meuSignal()` retorna o valor atual e registra a dependência no contexto reativo atual.

? O que `computed()` retorna?

- Um Observable
- Um valor primitivo
+ Um Signal somente leitura que recalcula quando suas dependências mudam
- Uma Promise

> `computed()` cria um Signal derivado. Ele é "lazy" — só recalcula quando é lido E alguma dependência mudou. É somente leitura (sem `.set()` ou `.update()`).

? Qual é a diferença entre `.set()` e `.update()`?

- São idênticos
+ `.set()` define o novo valor diretamente; `.update()` recebe o valor atual e retorna o novo
- `.update()` é assíncrono
- `.set()` só funciona com primitivos

> `signal.set(10)` define para `10`. `signal.update(n => n + 1)` lê o atual e deriva o próximo — útil para incrementos, toggles e transformações baseadas no estado atual.

? O que `toSignal(observable$)` faz?

- Converte um Signal em Observable
+ Converte um Observable em Signal para uso no template sem `async` pipe
- Cria um efeito colateral
- Destrói o Observable

> `toSignal` da lib `@angular/core/rxjs-interop` converte um Observable em Signal, gerenciando automaticamente a assinatura e limpeza quando o componente é destruído.

? Em que contexto `effect()` deve ser criado?

- Em qualquer lugar
- Dentro de `ngOnChanges`
+ Dentro de um contexto de injeção (construtor, `runInInjectionContext`, ou com `inject()`)
- Dentro de `subscribe()`

> `effect()` precisa de um `Injector` para gerenciar seu ciclo de vida. O contexto de injeção mais comum é o construtor do componente ou serviço.

? Qual analogia com autoimagem é mais precisa?

- Signal é o template, computed é o componente
+ Signal é a autoimagem central; computed são comportamentos que derivam dela automaticamente
- Effect é a autoimagem; signal é o resultado
- Computed é a memória; signal é a ação

> Exatamente como a autoimagem (signal central) determina todos os comportamentos derivados (computed), qualquer mudança no signal se propaga automaticamente para todos os computed que dependem dele.

:::

---

::: cartas

`signal(valor)` :: Cria um estado reativo com valor inicial — a "autoimagem" do componente
`computed(() => ...)` :: Deriva valor automaticamente de signals — comportamento que emerge do estado
`effect(() => ...)` :: Executa código quando signals mudam — efeito colateral reativo
`.set(novoValor)` :: Define o valor do signal diretamente
`.update(fn)` :: Transforma o valor atual do signal com uma função
`toSignal(obs$)` :: Converte Observable em Signal — sem `async` pipe no template
`toObservable(sig)` :: Converte Signal em Observable — para usar operadores RxJS
Signal somente leitura :: `signal.asReadonly()` — expõe o signal sem permitir mutação externa

:::
