---
id: ang-antimedo-cap02
order: 302
kind: capitulo
num: 207
group: Angular · Parte 52 · Bloco I — A Anatomia do Medo
title: Você é o que você pensa
subtitle: "@Component e o template — o Angular pensa em HTML"
minutes: 30
hue: 350
badge: 💭
- Entender o decorator @Component em profundidade: 
- Dominar interpolação, property binding e event binding: 
- Criar templates dinâmicos que refletem o estado do componente: 
- Compreender a relação entre pensamento (lógica) e manifestação (template): 
origem: angular/livros/01-antimedo-angular/content/02-voce-e-o-que-pensa.md
---

::: ilustracao img/ang-antimedo/cap02.svg | Uma mente humana com pensamentos se transformando em elementos HTML na tela.
:::

"Penso, logo existo." — Descartes disse isso no século XVII, e Pablo Marçal releu essa frase com outros olhos.

Para Pablo, o significado vai além da filosofia: **você se torna aquilo que você pensa repetidamente**. Se você alimenta pensamentos de fracasso, seu comportamento se alinha ao fracasso. Se você alimenta pensamentos de crescimento, seus atos constroem crescimento.

Os pensamentos são **sementes**. O que você planta na mente, você colhe na vida.

Em Angular, o componente funciona exatamente assim. A **classe TypeScript** é onde vivem os pensamentos — as propriedades, os valores, a lógica. O **template HTML** é onde esses pensamentos se manifestam na tela. O que você declara na classe, o Angular renderiza no DOM.

Você é o que você pensa. **O Angular mostra o que você declara.**

---

## O @Component em profundidade

Até o capítulo anterior, usamos o `@Component` de forma superficial. Agora vamos entender cada uma de suas propriedades.

```typescript
import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-pensamento',       // Tag HTML do componente
  standalone: true,                  // Não precisa de NgModule
  imports: [CommonModule],           // Dependências do template
  template: `<p>Olá, Mundo!</p>`,   // Template inline (ou templateUrl)
  styles: [`p { color: red; }`],    // Estilos inline (ou styleUrl)
  changeDetection: ChangeDetectionStrategy.OnPush  // Veremos no cap. 3
})
export class PensamentoComponent implements OnInit {
  // Propriedades = pensamentos
  mensagem = 'Penso, logo existo em Angular';

  ngOnInit(): void {
    console.log('Componente inicializado!');
  }
}
```

Há dois lugares para colocar o template:

| Propriedade | Descrição | Quando usar |
|-------------|-----------|-------------|
| `template` | Template inline, dentro do TS | Templates pequenos (< 10 linhas) |
| `templateUrl` | Arquivo HTML externo | Templates complexos |
| `styles` | Array de strings CSS inline | Estilos simples |
| `styleUrl` | Arquivo SCSS/CSS externo | Estilos complexos |

---

## Os três pilares da comunicação template-classe

Angular tem três formas de conectar a classe TypeScript com o template HTML. Pense nelas como os três tipos de pensamento:

1. **Interpolação `{{ }}`** — o pensamento se torna palavra
2. **Property Binding `[ ]`** — o pensamento controla um atributo
3. **Event Binding `( )`** — uma ação externa alimenta um pensamento

### 1. Interpolação — `{{ expressão }}`

A interpolação pega um valor da classe e o exibe como texto no template. É o pensamento mais básico: declarar e mostrar.

```typescript
import { Component } from '@angular/core';

@Component({
  selector: 'app-afirmacao',
  standalone: true,
  template: `
    <h1>{{ titulo }}</h1>
    <p>{{ descricao }}</p>
    <p>Hoje é: {{ hoje | date:'dd/MM/yyyy' }}</p>
    <p>2 + 2 = {{ 2 + 2 }}</p>
    <p>Nome em maiúsculas: {{ nome.toUpperCase() }}</p>
  `
})
export class AfirmacaoComponent {
  titulo = 'Antimedo em Angular';
  descricao = 'Você é o que você pensa.';
  hoje = new Date();
  nome = 'pablo';
}
```

::: nota
Dentro de `{{ }}` você pode usar qualquer **expressão TypeScript simples**: propriedades, métodos, operações matemáticas, operações com strings. O que você não pode usar: `if`, `for`, `while`, declarações de variáveis. Para lógica condicional no template, Angular tem suas próprias diretivas (`*ngIf`, `@if`).
:::

### 2. Property Binding — `[propriedade]="valor"`

O property binding conecta uma propriedade do **elemento HTML** a uma **expressão da classe**. É diferente da interpolação: aqui você controla o comportamento do elemento, não apenas seu texto.

```typescript
import { Component } from '@angular/core';

@Component({
  selector: 'app-controle',
  standalone: true,
  template: `
    <input
      [value]="textoInicial"
      [disabled]="bloqueado"
      [placeholder]="dica"
    />

    <img
      [src]="urlImagem"
      [alt]="descricaoImagem"
      [width]="largura"
    />

    <button [disabled]="!podeClicar">
      {{ podeClicar ? 'Clique aqui' : 'Aguarde...' }}
    </button>
  `
})
export class ControleComponent {
  textoInicial = 'Digite seus pensamentos aqui';
  bloqueado = false;
  dica = 'Ex: Eu sou capaz de aprender Angular';
  urlImagem = 'assets/logo.png';
  descricaoImagem = 'Logo do Antimedo Angular';
  largura = 200;
  podeClicar = true;
}
```

A diferença crucial entre `[src]="urlImagem"` e `src="{{ urlImagem }}"`:

- `[src]="urlImagem"` — Angular avalia `urlImagem` como **expressão TypeScript** e passa o resultado para a propriedade DOM
- `src="{{ urlImagem }}"` — Angular interpola o valor como **string** e atribui ao atributo HTML

Para a maioria dos casos, o resultado é o mesmo. Mas `[property]` é o caminho correto e mais eficiente.

### 3. Event Binding — `(evento)="handler()"`

O event binding ouve um evento do DOM (clique, tecla, foco, etc.) e chama um método da classe. É como se o mundo externo alimentasse seus pensamentos.

```typescript
import { Component } from '@angular/core';

@Component({
  selector: 'app-pensamentos',
  standalone: true,
  template: `
    <div class="container">
      <h2>Meus Pensamentos</h2>

      <input
        [value]="pensamentoAtual"
        (input)="atualizarPensamento($event)"
        (keydown.enter)="adicionarPensamento()"
        placeholder="O que você está pensando?"
      />

      <button (click)="adicionarPensamento()">Adicionar</button>
      <button (click)="limparTudo()">Limpar Tudo</button>

      <ul>
        <li *ngFor="let p of pensamentos" (dblclick)="remover(p)">
          {{ p }}
        </li>
      </ul>

      <p>Total de pensamentos: {{ pensamentos.length }}</p>
    </div>
  `
})
export class PensamentosComponent {
  pensamentoAtual = '';
  pensamentos: string[] = [];

  atualizarPensamento(event: Event): void {
    const input = event.target as HTMLInputElement;
    this.pensamentoAtual = input.value;
  }

  adicionarPensamento(): void {
    const texto = this.pensamentoAtual.trim();
    if (texto) {
      this.pensamentos.push(texto);
      this.pensamentoAtual = '';
    }
  }

  remover(pensamento: string): void {
    this.pensamentos = this.pensamentos.filter(p => p !== pensamento);
  }

  limparTudo(): void {
    this.pensamentos = [];
    this.pensamentoAtual = '';
  }
}
```

---

## Two-way Binding — `[(ngModel)]`

Há um quarto tipo de binding: o **two-way binding**. Ele combina property e event binding em um só. O template atualiza a classe E a classe atualiza o template automaticamente.

Para usar `[(ngModel)]`, você precisa importar o `FormsModule`:

```typescript
import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-meditacao',
  standalone: true,
  imports: [FormsModule],
  template: `
    <div class="meditacao">
      <h2>Afirmação Diária</h2>
      <input [(ngModel)]="afirmacao" placeholder="Digite sua afirmação" />
      <p>Você está afirmando: <strong>{{ afirmacao }}</strong></p>

      <select [(ngModel)]="humor">
        <option value="motivado">Motivado</option>
        <option value="ansioso">Ansioso</option>
        <option value="focado">Focado</option>
        <option value="com_medo">Com medo, mas determinado</option>
      </select>

      <p>Seu humor hoje: {{ humor }}</p>
    </div>
  `
})
export class MeditacaoComponent {
  afirmacao = 'Eu sou capaz de aprender Angular';
  humor = 'motivado';
}
```

::: dica
A "caixinha de banana" `[(ngModel)]` é a sintaxe de two-way binding. O `[]` representa o property binding (classe → template) e o `()` representa o event binding (template → classe). Juntos, formam a sincronização bidirecional.
:::

---

## Construindo um componente completo: o Diário do Medo

Vamos unir tudo que aprendemos em um componente prático:

```typescript
import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

interface EntradaDiario {
  id: number;
  medo: string;
  intensidade: number;
  data: Date;
  superado: boolean;
}

@Component({
  selector: 'app-diario-medo',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div class="diario">
      <h1>📓 Diário do Medo</h1>
      <p class="subtitulo">
        Você é o que você pensa. Registre e enfrente.
      </p>

      <div class="form-novo">
        <input
          [(ngModel)]="novoMedo"
          placeholder="Qual é o seu medo hoje?"
          (keydown.enter)="registrar()"
        />
        <input
          type="range"
          [(ngModel)]="novaIntensidade"
          min="1"
          max="10"
        />
        <span>Intensidade: {{ novaIntensidade }}/10</span>
        <button (click)="registrar()" [disabled]="!novoMedo.trim()">
          Registrar Medo
        </button>
      </div>

      <div class="lista">
        <div
          *ngFor="let entrada of entradas"
          class="entrada"
          [class.superado]="entrada.superado"
        >
          <p>
            <strong>{{ entrada.medo }}</strong>
            — Intensidade: {{ entrada.intensidade }}/10
          </p>
          <small>{{ entrada.data | date:'dd/MM/yyyy HH:mm' }}</small>
          <button (click)="toggleSuperado(entrada)">
            {{ entrada.superado ? '↩ Reabrir' : '✅ Superei!' }}
          </button>
        </div>
      </div>

      <div class="resumo" *ngIf="entradas.length > 0">
        <p>Total: {{ entradas.length }} medos registrados</p>
        <p>Superados: {{ medosSuperados }}</p>
        <p>Intensidade média: {{ intensidadeMedia | number:'1.1-1' }}</p>
      </div>
    </div>
  `,
  styles: [`
    .diario { max-width: 600px; margin: 0 auto; padding: 1rem; }
    .form-novo { display: flex; flex-direction: column; gap: 0.5rem; margin-bottom: 1.5rem; }
    .entrada { border: 1px solid #eee; padding: 1rem; margin: 0.5rem 0; border-radius: 4px; }
    .superado { opacity: 0.5; text-decoration: line-through; }
    .resumo { background: #f0f0f0; padding: 1rem; border-radius: 4px; }
  `]
})
export class DiarioMedoComponent {
  novoMedo = '';
  novaIntensidade = 5;
  entradas: EntradaDiario[] = [];
  private proximoId = 1;

  get medosSuperados(): number {
    return this.entradas.filter(e => e.superado).length;
  }

  get intensidadeMedia(): number {
    if (this.entradas.length === 0) return 0;
    const soma = this.entradas.reduce((acc, e) => acc + e.intensidade, 0);
    return soma / this.entradas.length;
  }

  registrar(): void {
    if (!this.novoMedo.trim()) return;
    this.entradas.unshift({
      id: this.proximoId++,
      medo: this.novoMedo.trim(),
      intensidade: this.novaIntensidade,
      data: new Date(),
      superado: false
    });
    this.novoMedo = '';
    this.novaIntensidade = 5;
  }

  toggleSuperado(entrada: EntradaDiario): void {
    entrada.superado = !entrada.superado;
  }
}
```

::: resumo
**O que aprendemos neste capítulo:**
- `@Component` tem propriedades essenciais: `selector`, `template`, `styles`, `imports`
- **Interpolação** `{{ }}`: exibe valores da classe no template como texto
- **Property Binding** `[prop]="valor"`: conecta propriedades HTML a expressões TypeScript
- **Event Binding** `(evento)="método()"`: reage a eventos do DOM chamando métodos da classe
- **Two-way Binding** `[(ngModel)]`: sincronização bidirecional com `FormsModule`
- Você é o que você pensa — o Angular renderiza o que você declara
:::

---

::: exercicio Componente de Afirmações Diárias
Crie um componente `AfirmacoesComponent` que:
1. Mostre uma afirmação diária aleatória de uma lista pré-definida
2. Permita o usuário adicionar novas afirmações via formulário
3. Mostre quantas afirmações existem na lista
4. Tenha um botão "Próxima" que avança para a próxima afirmação

--- resposta
```typescript
import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-afirmacoes',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div class="afirmacoes">
      <h2>💪 Afirmações do Dia</h2>

      <div class="card-afirmacao">
        <p class="afirmacao-texto">{{ afirmacaoAtual }}</p>
        <small>{{ indiceAtual + 1 }} de {{ afirmacoes.length }}</small>
      </div>

      <div class="controles">
        <button (click)="anterior()" [disabled]="indiceAtual === 0">
          ← Anterior
        </button>
        <button (click)="proxima()" [disabled]="indiceAtual === afirmacoes.length - 1">
          Próxima →
        </button>
        <button (click)="aleatoria()">🎲 Aleatória</button>
      </div>

      <div class="adicionar">
        <input
          [(ngModel)]="novaAfirmacao"
          placeholder="Digite uma nova afirmação..."
          (keydown.enter)="adicionar()"
        />
        <button (click)="adicionar()" [disabled]="!novaAfirmacao.trim()">
          Adicionar
        </button>
      </div>
    </div>
  `,
  styles: [`
    .afirmacoes { max-width: 500px; margin: 0 auto; padding: 1rem; text-align: center; }
    .card-afirmacao { background: #e91e63; color: white; padding: 2rem; border-radius: 8px; margin: 1rem 0; }
    .afirmacao-texto { font-size: 1.2rem; font-style: italic; }
    .controles { display: flex; gap: 0.5rem; justify-content: center; margin: 1rem 0; }
    .adicionar { display: flex; gap: 0.5rem; margin-top: 1rem; }
    .adicionar input { flex: 1; padding: 0.5rem; }
  `]
})
export class AfirmacoesComponent {
  afirmacoes: string[] = [
    'Eu sou capaz de aprender qualquer tecnologia.',
    'O medo é temporário. O conhecimento é permanente.',
    'Cada erro é uma aula gratuita.',
    'Eu progresso a cada linha de código.',
    'Angular é meu aliado, não meu inimigo.',
  ];

  indiceAtual = 0;
  novaAfirmacao = '';

  get afirmacaoAtual(): string {
    return this.afirmacoes[this.indiceAtual];
  }

  proxima(): void {
    if (this.indiceAtual < this.afirmacoes.length - 1) {
      this.indiceAtual++;
    }
  }

  anterior(): void {
    if (this.indiceAtual > 0) {
      this.indiceAtual--;
    }
  }

  aleatoria(): void {
    const novoIndice = Math.floor(Math.random() * this.afirmacoes.length);
    this.indiceAtual = novoIndice;
  }

  adicionar(): void {
    const texto = this.novaAfirmacao.trim();
    if (texto) {
      this.afirmacoes.push(texto);
      this.indiceAtual = this.afirmacoes.length - 1;
      this.novaAfirmacao = '';
    }
  }
}
```
:::

---

::: quiz
? O que é interpolação no Angular?
- Uma forma de importar módulos externos
- Um método para fazer requisições HTTP
+ A sintaxe `{{ expressão }}` que exibe valores da classe no template como texto
- O processo de compilar TypeScript para JavaScript
> Interpolação é a forma mais simples de binding: você coloca uma expressão entre `{{ }}` e o Angular a avalia e renderiza como texto no DOM.

? Qual a diferença entre `[src]="url"` e `src="{{ url }}"`?
- Não há diferença, são equivalentes
+ `[src]` faz property binding (avalia como TypeScript); `src="{{ }}"` interpola como string
- `[src]` só funciona com imagens; `src="{{ }}"` funciona com qualquer elemento
- `[src]` é mais lento porque cria um listener de eventos
> `[src]` é property binding: Angular avalia a expressão como TypeScript e a passa para a propriedade DOM. `src="{{ }}"` interpola o valor como string e o atribui ao atributo HTML. Para a maioria dos casos o resultado é similar, mas `[prop]` é o padrão recomendado.

? Para usar `[(ngModel)]`, qual módulo você deve importar?
- `CommonModule`
- `ReactiveFormsModule`
+ `FormsModule`
- `BrowserModule`
> `[(ngModel)]` faz parte do `FormsModule`, que é importado de `@angular/forms`. Sem essa importação, o Angular lança um erro de propriedade desconhecida.

? O que o event binding `(click)="salvar()"` faz?
- Cria uma propriedade `click` na classe do componente
+ Registra um listener no evento `click` do elemento e chama o método `salvar()` quando ativado
- Desabilita o elemento quando `salvar()` retorna false
- Vincula a propriedade `click` ao método `salvar` usando two-way binding
> `(evento)="handler()"` registra um listener no evento DOM especificado. Quando o evento ocorre, Angular chama o método correspondente na classe.

? O que é two-way binding?
- Uma forma de binding que funciona em dois componentes diferentes simultaneamente
+ A combinação de property binding e event binding que sincroniza bidiretcionalmente classe e template
- Um tipo de binding apenas disponível em formulários reativos
- O binding de duas propriedades CSS ao mesmo elemento
> Two-way binding (`[(ngModel)]`) é a "caixinha de banana": `[()]`. O `[]` sincroniza classe→template, e o `()` sincroniza template→classe. O resultado é que qualquer mudança em um lado reflete no outro automaticamente.

? O que acontece se você colocar uma declaração `if` dentro de `{{ }}`?
- Funciona normalmente, como em TypeScript
- O Angular converte o `if` para uma diretiva automaticamente
+ Gera um erro de compilação — interpolação só aceita expressões, não declarações
- Funciona, mas apenas com `if` ternário
> Interpolação aceita apenas **expressões** TypeScript (que retornam um valor): propriedades, chamadas de método, operadores ternários, operações matemáticas. Declarações como `if`, `for`, `let` não são permitidas dentro de `{{ }}`.
:::

---

::: cartas
O que é interpolação no Angular? :: A sintaxe `{{ expressão }}` que avalia uma expressão TypeScript e a exibe como texto no template HTML.
O que é property binding? :: A sintaxe `[propriedade]="expressão"` que conecta uma propriedade de elemento HTML a uma expressão TypeScript da classe.
O que é event binding? :: A sintaxe `(evento)="método()"` que registra um listener de evento e chama um método da classe quando o evento ocorre.
O que é two-way binding? :: A sintaxe `[(ngModel)]="propriedade"` que sincroniza bidiretcionalmente uma propriedade entre a classe e o template.
Qual módulo é necessário para `[(ngModel)]`? :: `FormsModule`, importado de `@angular/forms`.
Qual a diferença entre `templateUrl` e `template`? :: `templateUrl` aponta para um arquivo HTML externo; `template` contém o HTML inline dentro do decorator `@Component`.
Como acessar o valor de um `$event` em um event binding de input? :: `(input)="método($event)"` e no método: `const input = event.target as HTMLInputElement; const valor = input.value;`
O que o `CommonModule` fornece para os templates? :: Diretivas estruturais como `*ngIf`, `*ngFor`, `*ngSwitch` e pipes comuns como `DatePipe`, `CurrencyPipe`.
:::
