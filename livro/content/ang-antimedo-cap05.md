---
id: ang-antimedo-cap05
order: 305
kind: capitulo
num: 210
group: Angular · Parte 52 · Bloco I — A Anatomia do Medo
title: Quais são os seus medos?
subtitle: Formulários reativos — expondo o que estava oculto
minutes: 35
hue: 30
badge: 📋
- Criar formulários com ReactiveFormsModule: 
- Usar FormBuilder, FormGroup e FormControl: 
- Implementar validações síncronas e assíncronas: 
- Exibir mensagens de erro contextuais no template: 
origem: angular/livros/01-antimedo-angular/content/05-quais-sao-seus-medos.md
---

::: ilustracao img/ang-antimedo/cap05.svg | Uma criança em uma bicicleta olhando para o caminho à frente, com bloqueios visíveis mas superáveis.
:::

Pablo conta a história de uma menina que tinha medo de andar de bicicleta. Toda vez que alguém perguntava o que a assustava, ela dizia "não sei". Mas quando alguém sentou com ela e disse _"me conta o que passa na sua cabeça quando você vê a bicicleta"_, a resposta foi detalhada: medo de cair, medo de machucar o joelho, medo de que os outros rissem.

**Nomear o medo é o primeiro passo para superá-lo.**

Bebês não têm medo de aprender. Eles caem, levantam, tentam de novo. O medo de errar é aprendido. E coisas aprendidas podem ser desaprendidas — especialmente quando expostas à luz.

Formulários reativos do Angular são exatamente isso: uma forma de **expor o que estava oculto**. O usuário preenche, os validadores analisam cada campo, e os erros aparecem de forma clara e objetiva. Não há como esconder um campo inválido quando você tem `Validators` no trabalho.

---

## Template-driven vs Reactive Forms

Angular oferece duas abordagens para formulários:

| Característica | Template-driven | Reactive |
|----------------|----------------|---------|
| Definição | No template HTML | Na classe TypeScript |
| Testabilidade | Difícil (depende do DOM) | Fácil (objetos TypeScript) |
| Validações | No HTML com diretivas | Na classe com `Validators` |
| Tipagem | Fraca | Forte (TypeScript completo) |
| Complexidade | Simples | Poderosa |
| Analogia | Listar medos de cabeça | Escrever os medos com análise criteriosa |

Para formulários complexos, sempre prefira **Reactive Forms**.

---

## Configurando o ambiente

```typescript
// app.config.ts
import { ApplicationConfig } from '@angular/core';
import { provideRouter } from '@angular/router';
import { provideAnimations } from '@angular/platform-browser/animations';

export const appConfig: ApplicationConfig = {
  providers: [
    provideRouter([]),
    provideAnimations(),
  ]
};
```

No componente que usará formulários reativos, importe `ReactiveFormsModule`:

```typescript
import { ReactiveFormsModule } from '@angular/forms';

@Component({
  imports: [ReactiveFormsModule, CommonModule],
  // ...
})
```

---

## FormControl — o controle individual

Um `FormControl` rastreia o valor e o estado de um único campo de formulário:

```typescript
import { Component, OnInit } from '@angular/core';
import { FormControl, Validators, ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-campo-medo',
  standalone: true,
  imports: [ReactiveFormsModule, CommonModule],
  template: `
    <div class="campo">
      <label>Descreva seu medo:</label>
      <input [formControl]="campoMedo" placeholder="Ex: Não entender RxJS" />

      <div class="erros" *ngIf="campoMedo.invalid && campoMedo.touched">
        <span *ngIf="campoMedo.hasError('required')">
          ❌ Você precisa nomear seu medo para superá-lo.
        </span>
        <span *ngIf="campoMedo.hasError('minlength')">
          ❌ Seja mais específico (mínimo 10 caracteres).
        </span>
        <span *ngIf="campoMedo.hasError('maxlength')">
          ❌ Resumo, por favor! (máximo 100 caracteres).
        </span>
      </div>

      <p>Status: {{ campoMedo.status }}</p>
      <p>Valor: {{ campoMedo.value }}</p>
      <p>Sujo (modificado): {{ campoMedo.dirty }}</p>
      <p>Tocado: {{ campoMedo.touched }}</p>
    </div>
  `
})
export class CampoMedoComponent {
  campoMedo = new FormControl('', [
    Validators.required,
    Validators.minLength(10),
    Validators.maxLength(100)
  ]);
}
```

### Estados de um FormControl

| Estado | Significado |
|--------|-------------|
| `valid` / `invalid` | Se passa em todas as validações |
| `pristine` / `dirty` | Se o valor foi modificado pelo usuário |
| `untouched` / `touched` | Se o campo recebeu e perdeu foco |
| `pending` | Aguardando validação assíncrona |

---

## FormGroup e FormBuilder — o formulário completo

Na vida real, formulários têm múltiplos campos. O `FormGroup` agrupa vários `FormControl`:

```typescript
import { Component, OnInit } from '@angular/core';
import {
  FormBuilder,
  FormGroup,
  Validators,
  ReactiveFormsModule,
  AbstractControl,
  ValidationErrors
} from '@angular/forms';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-auto-analise',
  standalone: true,
  imports: [ReactiveFormsModule, CommonModule],
  template: `
    <form [formGroup]="formulario" (ngSubmit)="analisar()" class="form-analise">
      <h2>📋 Auto-análise do Medo</h2>

      <!-- Campo: Nome do medo -->
      <div class="campo-grupo" [class.invalido]="erroVisivel('nomeMedo')">
        <label>Nome do medo *</label>
        <input formControlName="nomeMedo" placeholder="Ex: Apresentar código em público" />
        <div class="erro" *ngIf="erroVisivel('nomeMedo')">
          <span *ngIf="formulario.get('nomeMedo')?.hasError('required')">
            Campo obrigatório.
          </span>
          <span *ngIf="formulario.get('nomeMedo')?.hasError('minlength')">
            Mínimo de 5 caracteres.
          </span>
        </div>
      </div>

      <!-- Campo: Intensidade -->
      <div class="campo-grupo">
        <label>Intensidade (1 a 10) *</label>
        <input
          type="number"
          formControlName="intensidade"
          min="1"
          max="10"
        />
        <div class="erro" *ngIf="erroVisivel('intensidade')">
          <span *ngIf="formulario.get('intensidade')?.hasError('required')">
            Campo obrigatório.
          </span>
          <span *ngIf="formulario.get('intensidade')?.hasError('min')">
            Mínimo: 1.
          </span>
          <span *ngIf="formulario.get('intensidade')?.hasError('max')">
            Máximo: 10.
          </span>
        </div>
      </div>

      <!-- Campo: Origem -->
      <div class="campo-grupo">
        <label>Origem do medo</label>
        <select formControlName="origem">
          <option value="">Selecione...</option>
          <option value="experiencia">Experiência negativa passada</option>
          <option value="desconhecido">Medo do desconhecido</option>
          <option value="julgamento">Medo do julgamento</option>
          <option value="fracasso">Medo do fracasso</option>
        </select>
      </div>

      <!-- Campo: Ação -->
      <div class="campo-grupo" [class.invalido]="erroVisivel('acaoEnfrentamento')">
        <label>Como você vai enfrentar? *</label>
        <textarea
          formControlName="acaoEnfrentamento"
          rows="3"
          placeholder="Ex: Vou praticar apresentações para o espelho..."
        ></textarea>
        <div class="erro" *ngIf="erroVisivel('acaoEnfrentamento')">
          Descreva pelo menos uma ação concreta (mínimo 20 caracteres).
        </div>
      </div>

      <!-- Campo: Prazo -->
      <div class="campo-grupo">
        <label>Prazo para superar</label>
        <input type="date" formControlName="prazo" [min]="hoje" />
      </div>

      <div class="acoes">
        <button
          type="submit"
          [disabled]="formulario.invalid || enviando"
        >
          {{ enviando ? 'Analisando...' : '🔍 Analisar Medo' }}
        </button>
        <button type="button" (click)="resetar()">🔄 Recomeçar</button>
      </div>

      <div class="resultado" *ngIf="resultado">
        <h3>✅ Análise Concluída</h3>
        <p><strong>Medo:</strong> {{ resultado.nomeMedo }}</p>
        <p><strong>Intensidade:</strong> {{ resultado.intensidade }}/10</p>
        <p><strong>Classificação:</strong> {{ resultado.classificacao }}</p>
        <p><strong>Sua ação:</strong> {{ resultado.acaoEnfrentamento }}</p>
      </div>
    </form>
  `,
  styles: [`
    .form-analise { max-width: 500px; margin: 0 auto; padding: 1.5rem; }
    .campo-grupo { margin-bottom: 1rem; }
    .campo-grupo label { display: block; font-weight: bold; margin-bottom: 0.25rem; }
    .campo-grupo input, .campo-grupo select, .campo-grupo textarea {
      width: 100%; padding: 0.5rem; border: 1px solid #ccc; border-radius: 4px;
    }
    .invalido input, .invalido select, .invalido textarea { border-color: #e53935; }
    .erro { color: #e53935; font-size: 0.85rem; margin-top: 0.25rem; }
    .acoes { display: flex; gap: 0.5rem; margin-top: 1rem; }
    .resultado { background: #e8f5e9; padding: 1rem; border-radius: 8px; margin-top: 1.5rem; }
  `]
})
export class AutoAnaliseComponent implements OnInit {
  formulario!: FormGroup;
  enviando = false;
  resultado: any = null;
  hoje = new Date().toISOString().split('T')[0];

  constructor(private fb: FormBuilder) {}

  ngOnInit(): void {
    this.formulario = this.fb.group({
      nomeMedo: ['', [Validators.required, Validators.minLength(5)]],
      intensidade: [5, [Validators.required, Validators.min(1), Validators.max(10)]],
      origem: [''],
      acaoEnfrentamento: ['', [Validators.required, Validators.minLength(20)]],
      prazo: ['']
    });
  }

  erroVisivel(campo: string): boolean {
    const control = this.formulario.get(campo);
    return !!(control?.invalid && (control.dirty || control.touched));
  }

  analisar(): void {
    if (this.formulario.invalid) {
      this.formulario.markAllAsTouched();
      return;
    }

    this.enviando = true;
    const valores = this.formulario.value;

    setTimeout(() => {
      this.resultado = {
        ...valores,
        classificacao: this.classificar(valores.intensidade)
      };
      this.enviando = false;
    }, 1000);
  }

  resetar(): void {
    this.formulario.reset({ intensidade: 5 });
    this.resultado = null;
  }

  private classificar(intensidade: number): string {
    if (intensidade <= 3) return '🟢 Medo leve — comece imediatamente';
    if (intensidade <= 6) return '🟡 Medo moderado — planeje e execute';
    return '🔴 Medo intenso — divida em pequenos passos';
  }
}
```

---

## Validações customizadas

Às vezes os validators padrão não são suficientes. Você pode criar seus próprios:

```typescript
import { AbstractControl, ValidationErrors, ValidatorFn } from '@angular/forms';

// Validator que verifica se o medo é específico (não genérico)
export function medoEspecificoValidator(): ValidatorFn {
  return (control: AbstractControl): ValidationErrors | null => {
    const valor = control.value as string;
    if (!valor) return null;

    const palavrasGenericas = ['tudo', 'nada', 'algo', 'coisa', 'problema'];
    const ehGenerico = palavrasGenericas.some(p =>
      valor.toLowerCase().includes(p) && valor.trim().length < 20
    );

    return ehGenerico ? { medoGenerico: true } : null;
  };
}

// Validator assíncrono — simula verificação no servidor
export function medoJaCadastradoValidator(medosExistentes: string[]): AsyncValidatorFn {
  return (control: AbstractControl): Observable<ValidationErrors | null> => {
    return timer(500).pipe(
      map(() => {
        const existe = medosExistentes
          .map(m => m.toLowerCase())
          .includes(control.value?.toLowerCase());
        return existe ? { medoJaCadastrado: true } : null;
      })
    );
  };
}
```

Uso no FormBuilder:

```typescript
import { timer } from 'rxjs';
import { map } from 'rxjs/operators';
import { AsyncValidatorFn } from '@angular/forms';

ngOnInit(): void {
  const medosCadastrados = ['Angular', 'TypeScript', 'RxJS'];

  this.formulario = this.fb.group({
    nomeMedo: [
      '',
      [
        Validators.required,
        Validators.minLength(5),
        medoEspecificoValidator()     // Validator síncrono customizado
      ],
      [
        medoJaCadastradoValidator(medosCadastrados)  // Validator assíncrono
      ]
    ]
  });
}
```

::: resumo
**O que aprendemos neste capítulo:**
- `FormControl` rastreia valor e estado de um campo individual
- `FormGroup` agrupa múltiplos controls em um formulário
- `FormBuilder` simplifica a criação de formulários com sintaxe concisa
- `Validators` oferece validações prontas: `required`, `minLength`, `min`, `max`, `email`
- Validators customizados são funções que retornam `ValidationErrors | null`
- `markAllAsTouched()` exibe todos os erros quando o formulário é submetido inválido
- Nomear o medo (validar o campo) é o primeiro passo para superá-lo
:::

---

::: exercicio Formulário de Lista de Medos Priorizada
Crie um formulário com `FormArray` que permita ao usuário:
1. Adicionar múltiplos medos (até 5)
2. Cada medo tem: descrição (obrigatória, mín. 10 chars) e prioridade (1-5)
3. Um botão "Adicionar Medo" adiciona uma nova linha
4. Um botão "Remover" em cada linha remove o medo
5. Ao submeter, exiba a lista ordenada por prioridade

--- resposta
```typescript
import { Component, OnInit } from '@angular/core';
import {
  FormBuilder, FormGroup, FormArray,
  Validators, ReactiveFormsModule
} from '@angular/forms';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-lista-priorizada',
  standalone: true,
  imports: [ReactiveFormsModule, CommonModule],
  template: `
    <form [formGroup]="form" (ngSubmit)="submeter()">
      <h2>📊 Lista de Medos Priorizada</h2>

      <div formArrayName="medos">
        <div
          *ngFor="let medo of medosArray.controls; let i = index"
          [formGroupName]="i"
          class="linha-medo"
        >
          <span>#{{ i + 1 }}</span>
          <input formControlName="descricao" placeholder="Descrição do medo" />
          <input type="number" formControlName="prioridade" min="1" max="5" />
          <button type="button" (click)="remover(i)">✕</button>
          <div class="erro" *ngIf="medo.get('descricao')?.invalid && medo.get('descricao')?.touched">
            Mínimo 10 caracteres.
          </div>
        </div>
      </div>

      <button
        type="button"
        (click)="adicionar()"
        [disabled]="medosArray.length >= 5"
      >
        + Adicionar Medo
      </button>

      <button type="submit" [disabled]="form.invalid">
        📋 Gerar Lista Priorizada
      </button>

      <div *ngIf="listaFinal.length > 0" class="resultado">
        <h3>Seus medos (por prioridade):</h3>
        <ol>
          <li *ngFor="let m of listaFinal">
            [P{{ m.prioridade }}] {{ m.descricao }}
          </li>
        </ol>
      </div>
    </form>
  `
})
export class ListaPriorizadaComponent implements OnInit {
  form!: FormGroup;
  listaFinal: any[] = [];

  constructor(private fb: FormBuilder) {}

  ngOnInit(): void {
    this.form = this.fb.group({ medos: this.fb.array([]) });
    this.adicionar(); // Começa com um campo
  }

  get medosArray(): FormArray {
    return this.form.get('medos') as FormArray;
  }

  adicionar(): void {
    this.medosArray.push(this.fb.group({
      descricao: ['', [Validators.required, Validators.minLength(10)]],
      prioridade: [3, [Validators.required, Validators.min(1), Validators.max(5)]]
    }));
  }

  remover(i: number): void {
    this.medosArray.removeAt(i);
  }

  submeter(): void {
    if (this.form.invalid) { this.form.markAllAsTouched(); return; }
    this.listaFinal = [...this.medosArray.value]
      .sort((a, b) => b.prioridade - a.prioridade);
  }
}
```
:::

---

::: quiz
? Qual a principal vantagem dos Reactive Forms sobre os Template-driven Forms?
- São mais fáceis de usar para iniciantes
+ Oferecem tipagem forte, melhor testabilidade e controle programático total
- Requerem menos código para implementar
- São a única forma de fazer validação assíncrona
> Reactive Forms são definidos na classe TypeScript, o que os torna totalmente tipados, facilmente testáveis (sem depender do DOM) e com controle total sobre cada aspecto do formulário.

? O que o `FormBuilder` faz?
- Renderiza o formulário HTML automaticamente
+ É um serviço que simplifica a criação de `FormGroup` e `FormControl` com sintaxe concisa
- Valida formulários automaticamente sem configuração
- É o equivalente angular do `<form>` HTML
> `FormBuilder` é um serviço injetável que fornece métodos (`group`, `control`, `array`) para criar instâncias de `FormGroup`, `FormControl` e `FormArray` com sintaxe mais concisa que usar `new FormControl()` diretamente.

? O que `markAllAsTouched()` faz em um FormGroup?
- Remove todos os valores do formulário
- Marca todos os campos como válidos
+ Marca todos os controles como "touched", fazendo as mensagens de erro aparecerem
- Submete o formulário programaticamente
> `markAllAsTouched()` é chamado quando o usuário tenta submeter um formulário inválido. Ele marca todos os controles como "touched", o que faz as mensagens de erro (que normalmente só aparecem após interação) se tornarem visíveis.

? O que um validator customizado deve retornar quando o valor é INVÁLIDO?
- `false`
- `new Error('mensagem')`
+ Um objeto `ValidationErrors` como `{ nomeDoErro: true }` ou `{ nomeDoErro: 'mensagem' }`
- `null` com uma mensagem de erro como segundo argumento
> Um validator retorna `null` quando o valor é válido, e um objeto `ValidationErrors` quando é inválido. A chave do objeto é o nome do erro (usada no template com `hasError('nomeDoErro')`).

? Qual é a diferença entre `dirty` e `touched` em um FormControl?
+ `dirty` significa que o valor foi alterado; `touched` significa que o campo recebeu e perdeu foco (blur)
- `dirty` significa que o campo tem erros; `touched` significa que é obrigatório
- `dirty` e `touched` são sinônimos no Angular
- `dirty` é para campos de texto; `touched` é para checkboxes e radio buttons
> `pristine`/`dirty` rastreiam se o valor foi modificado (pelo usuário ou programaticamente via `setValue`). `untouched`/`touched` rastreiam se o campo recebeu e perdeu foco (`blur`). Ambos juntos definem quando mostrar mensagens de erro.
:::

---

::: cartas
O que é `FormControl`? :: A unidade básica dos Reactive Forms: rastreia o valor, estado de validação e interação de um único campo de formulário.
O que é `FormGroup`? :: Um agrupamento de `FormControl`s (e outros `FormGroup`s ou `FormArray`s) que rastreia o valor e estado de validação do conjunto.
O que é `FormBuilder`? :: Um serviço injetável que simplifica a criação de `FormGroup`, `FormControl` e `FormArray` com sintaxe mais enxuta.
O que é `FormArray`? :: Uma coleção dinâmica de `FormControl`s ou `FormGroup`s — ideal para listas de campos que o usuário pode adicionar ou remover.
O que retorna um validator válido? :: `null` — indica que o campo passou na validação.
O que retorna um validator inválido? :: Um objeto `ValidationErrors`, ex: `{ required: true }` ou `{ minlength: { requiredLength: 10, actualLength: 5 } }`.
O que é `Validators.required`? :: Um validator padrão que retorna erro se o campo estiver vazio ou `null`.
O que faz `formulario.markAllAsTouched()`? :: Marca todos os controles do formulário como "touched", tornando visíveis as mensagens de erro mesmo sem o usuário ter interagido com cada campo.
:::
