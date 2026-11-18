---
id: ang-amigos-lab3
order: 373
kind: lab
num: 50
group: Angular · Parte 69 · Laboratório Prático
title: "Laboratório 3: O Design System Amigos UI"
subtitle: Construção Completa de um Design System com Angular 19, CDK, Signals e Component Harnesses
minutes: 45
hue: 215
badge: 🧪
image: img/ang-amigos/lab3.svg
- Integrar todos os princípios de Dale Carnegie e a arquitetura do Angular 19 em um Design System completo: 
- Implementar AmgButtonComponent com suporte a variantes, estados e acessibilidade nativa: 
- Construir AmgDialogComponent desacoplado com Angular CDK Overlay e FocusTrap: 
- Desenvolver AmgSelectComponent com tipagem genérica <T> e integração com ControlValueAccessor: 
- Criar o componente AmgVirtualListComponent com CDK Scrolling operando a 60 FPS com 10.000 nós: 
- Implementar suíte completa de testes automatizados com CDK Component Harnesses oficiais: 
origem: angular/livros/03-amigos-angular/content/52-lab-amigos-ui.md
---

::: ilustracao img/ang-amigos/lab3.svg | Interface de showcase completa da biblioteca Amigos UI exibindo componentes visuais, seletor de temas e terminal de testes 100% verde com Component Harnesses.
:::

## 1. Visão Geral do Laboratório

Chegou o momento culminante de nossa jornada. Ao longo dos dezenove capítulos anteriores, navegamos pelas páginas imortais de **Dale Carnegie** e traduzimos cada um de seus princípios de convivência e empatia humana para a mais moderna arquitetura de engenharia frontend em **Angular 19** ([angular.dev/overview](https://angular.dev/overview)).

Neste **Laboratório 3**, vamos construir juntos a versão completa e executável do **Design System Amigos UI** (`@amigos/ui`), acompanhada de uma aplicação de demonstração (*Showcase*) e de uma suíte rigorosa de testes automatizados com **Component Harnesses**.

Nossa biblioteca conterá os seguintes blocos de construção de classe mundial:
1. **`AmgButtonComponent`:** O botão que não machuca: variantes visuais, estados reativos com Signals e acessibilidade via teclado.
2. **`AmgDialogComponent`:** O diálogo acolhedor: desacoplado de guerras de `z-index` através do **Angular CDK Overlay** e com contenção de foco com `FocusTrap`.
3. **`AmgSelectComponent<T>`:** O seletor que fala a língua do cliente: tipagem estrita com TypeScript Generics e integração transparente com `ControlValueAccessor` para formulários reativos.
4. **`AmgVirtualListComponent`:** A válvula de alívio: navegação fluida por 10.000 registros mantendo taxa de quadros de **60 FPS** com o `@angular/cdk/scrolling`.
5. **Suíte de Testes com `ComponentHarness`:** Automação resiliente que nunca quebra por alterações em classes CSS cosméticas.

---

## 2. A Arquitetura do Design System

A estrutura do nosso laboratório em `codigo/ang-amigos/amigos-app/src/app` está organizada em módulos de alta coesão:

```text
codigo/ang-amigos/amigos-app/src/app/
├── core/
│   ├── models/
│   │   └── amigo.model.ts              # Modelos de domínio tipados
│   ├── tokens/
│   │   └── amigos-config.token.ts      # InjectionTokens com defaults seguros
│   └── services/
│       └── elogios.store.ts            # Store reativa com Signals puros
├── ui/
│   ├── button/
│   │   ├── amg-button.component.ts     # Botão com Signal Inputs e Transform
│   │   └── amg-button.harness.ts       # Test Harness oficial
│   ├── dialog/
│   │   ├── amg-dialog.component.ts     # Modal desacoplado com CDK Overlay
│   │   └── amg-dialog.harness.ts       # Test Harness oficial
│   ├── select/
│   │   ├── amg-select.component.ts     # Dropdown com ControlValueAccessor
│   │   └── amg-select.harness.ts       # Test Harness oficial
│   └── virtual-list/
│       └── amg-virtual-list.component.ts # CDK Scrolling de alta performance
├── showcase/
│   ├── showcase.component.ts          # Aplicação de demonstração viva
│   └── showcase.component.html        # Template integrador
└── app.config.ts                      # Provedores globais com provideAmigosUI()
```

---

## 3. Implementação dos Componentes Centrais

### 3.1 O Botão Amigável (`AmgButtonComponent`)

```typescript
// src/app/ui/button/amg-button.component.ts
import { 
  Component, 
  ChangeDetectionStrategy, 
  input, 
  computed, 
  booleanAttribute 
} from '@angular/core';

export type AmgButtonVariant = 'primary' | 'secondary' | 'ghost' | 'success';

@Component({
  selector: 'amg-button',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <button 
      [class]="classesComputadas()"
      [disabled]="disabled()"
      type="button">
      <ng-content />
    </button>
  `,
  styles: [`
    :host { display: inline-block; }
    button {
      display: inline-flex;
      align-items: center;
      justify-content: center;
      gap: 8px;
      padding: 8px 16px;
      border-radius: 6px;
      font-size: 14px;
      font-weight: 600;
      font-family: inherit;
      cursor: pointer;
      border: 1px solid transparent;
      transition: all 0.15s ease-in-out;
    }
    button:focus-visible {
      outline: 2px solid #2563eb;
      outline-offset: 2px;
    }
    .amg-btn-primary { background: #2563eb; color: #ffffff; }
    .amg-btn-primary:hover:not(:disabled) { background: #1d4ed8; }
    
    .amg-btn-secondary { background: #e2e8f0; color: #1e293b; }
    .amg-btn-secondary:hover:not(:disabled) { background: #cbd5e1; }
    
    .amg-btn-ghost { background: transparent; border-color: #cbd5e1; color: #475569; }
    .amg-btn-ghost:hover:not(:disabled) { background: #f8fafc; color: #0f172a; }

    .amg-btn-success { background: #16a34a; color: #ffffff; }
    .amg-btn-success:hover:not(:disabled) { background: #15803d; }

    button:disabled { opacity: 0.5; cursor: not-allowed; }
  `]
})
export class AmgButtonComponent {
  /** Variante semântica visual do botão */
  readonly variant = input<AmgButtonVariant>('primary');

  /** Suporte nativo a atributo HTML booleano (<amg-button disabled>) */
  readonly disabled = input<boolean, boolean | string>(false, {
    transform: booleanAttribute
  });

  protected readonly classesComputadas = computed(() => {
    return `amg-btn amg-btn-${this.variant()}`;
  });
}
```

---

### 3.2 O Seletor com Formulários e CVA (`AmgSelectComponent<T>`)

```typescript
// src/app/ui/select/amg-select.component.ts
import { 
  Component, 
  ChangeDetectionStrategy, 
  input, 
  output, 
  signal, 
  computed, 
  forwardRef, 
  inject 
} from '@angular/core';
import { ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';
import { LiveAnnouncer } from '@angular/cdk/a11y';

export interface AmgOption<T> {
  readonly label: string;
  readonly value: T;
  readonly badge?: string;
}

@Component({
  selector: 'amg-select',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => AmgSelectComponent),
      multi: true
    }
  ],
  template: `
    <div class="amg-select-wrapper">
      <button
        type="button"
        class="amg-select-trigger"
        [attr.aria-expanded]="isOpen()"
        [attr.aria-haspopup]="'listbox'"
        [disabled]="disabled()"
        (click)="alternarDropdown()">
        <span>{{ textoSelecionado() ?? placeholder() }}</span>
        <span class="seta" aria-hidden="true">▾</span>
      </button>

      @if (isOpen()) {
        <ul class="amg-select-dropdown" role="listbox" tabindex="-1">
          @for (opcao of options(); track opcao.value) {
            <li
              role="option"
              class="amg-select-item"
              [class.amg-selected]="estaSelecionado(opcao.value)"
              [attr.aria-selected]="estaSelecionado(opcao.value)"
              (click)="selecionarItem(opcao)">
              <span>{{ opcao.label }}</span>
              @if (opcao.badge) {
                <small class="badge">{{ opcao.badge }}</small>
              }
            </li>
          } @empty {
            <li class="amg-empty">Nenhuma opção disponível</li>
          }
        </ul>
      }
    </div>
  `,
  styles: [`
    .amg-select-wrapper { position: relative; width: 100%; font-family: inherit; }
    .amg-select-trigger {
      display: flex; justify-content: space-between; align-items: center;
      width: 100%; padding: 10px 14px; border: 1px solid #cbd5e1;
      border-radius: 6px; background: #ffffff; color: #0f172a;
      cursor: pointer; font-size: 14px;
    }
    .amg-select-dropdown {
      position: absolute; top: calc(100% + 4px); left: 0; right: 0;
      max-height: 220px; overflow-y: auto; background: #ffffff;
      border: 1px solid #cbd5e1; border-radius: 6px; padding: 4px;
      margin: 0; list-style: none; box-shadow: 0 10px 15px -3px rgba(0,0,0,0.1);
      z-index: 50;
    }
    .amg-select-item {
      padding: 8px 12px; border-radius: 4px; cursor: pointer;
      display: flex; justify-content: space-between; align-items: center; font-size: 13px;
    }
    .amg-select-item:hover { background: #eff6ff; color: #1d4ed8; }
    .amg-selected { background: #dbeafe; font-weight: 600; color: #1e40af; }
    .badge { background: #e2e8f0; padding: 2px 6px; border-radius: 4px; font-size: 10px; }
    .amg-empty { padding: 12px; text-align: center; color: #94a3b8; font-size: 12px; }
  `]
})
export class AmgSelectComponent<T> implements ControlValueAccessor {
  private readonly live = inject(LiveAnnouncer);

  readonly options = input.required<readonly AmgOption<T>[]>();
  readonly placeholder = input<string>('Selecione uma opção...');

  readonly selectionChange = output<T>();

  protected readonly valor = signal<T | null>(null);
  protected readonly disabled = signal<boolean>(false);
  protected readonly isOpen = signal<boolean>(false);

  private onChange: (v: T) => void = () => {};
  private onTouched: () => void = () => {};

  protected readonly textoSelecionado = computed(() => {
    const val = this.valor();
    if (val === null || val === undefined) return null;
    const achado = this.options().find(opt => opt.value === val);
    return achado ? achado.label : null;
  });

  writeValue(obj: T | null): void {
    this.valor.set(obj);
  }

  registerOnChange(fn: (v: T) => void): void {
    this.onChange = fn;
  }

  registerOnTouched(fn: () => void): void {
    this.onTouched = fn;
  }

  setDisabledState(isDisabled: boolean): void {
    this.disabled.set(isDisabled);
  }

  protected alternarDropdown(): void {
    if (this.disabled()) return;
    this.isOpen.update(v => !v);
  }

  protected estaSelecionado(val: T): boolean {
    return this.valor() === val;
  }

  protected selecionarItem(opcao: AmgOption<T>): void {
    this.valor.set(opcao.value);
    this.onChange(opcao.value);
    this.onTouched();
    this.selectionChange.emit(opcao.value);
    this.isOpen.set(false);

    // Escuta ativa para leitores de tela
    this.live.announce(`Selecionado: ${opcao.label}`, 'polite');
  }
}
```

---

## 4. Testes com Component Harnesses

Vejamos como codificar os Harnesses de testes para que qualquer aplicação corporativa teste o Design System com total desacoplamento:

```typescript
// src/app/ui/button/amg-button.harness.ts
import { ComponentHarness } from '@angular/cdk/testing';

export class AmgButtonHarness extends ComponentHarness {
  static hostSelector = 'amg-button';

  private locateButton = this.locatorFor('button');

  async getText(): Promise<string> {
    const btn = await this.locateButton();
    return btn.text();
  }

  async isDisabled(): Promise<boolean> {
    const btn = await this.locateButton();
    return (await btn.getAttribute('disabled')) !== null;
  }

  async click(): Promise<void> {
    const btn = await this.locateButton();
    return btn.click();
  }
}
```

E a suíte de testes Jasmine/Vitest rodando com o `TestbedHarnessEnvironment`:

```typescript
// src/app/ui/button/amg-button.spec.ts
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { Component } from '@angular/core';
import { TestbedHarnessEnvironment } from '@angular/cdk/testing/testbed';
import { AmgButtonComponent } from './amg-button.component';
import { AmgButtonHarness } from './amg-button.harness';

@Component({
  standalone: true,
  imports: [AmgButtonComponent],
  template: `
    <amg-button variant="primary" (click)="aoClicar()">
      Elogiar Colega
    </amg-button>
    <amg-button variant="ghost" [disabled]="true">
      Desabilitado
    </amg-button>
  `
})
class TestHostComponent {
  clicado = false;
  aoClicar() { this.clicado = true; }
}

describe('AmgButtonComponent com Component Harness', () => {
  let fixture: ComponentFixture<TestHostComponent>;
  let loader: any;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TestHostComponent]
    }).compileComponents();

    fixture = TestBed.createComponent(TestHostComponent);
    fixture.detectChanges();
    loader = TestbedHarnessEnvironment.loader(fixture);
  });

  it('deve interagir com o botão sem tocar em classes CSS internas', async () => {
    const botoes = await loader.getAllHarnesses(AmgButtonHarness);
    expect(botoes.length).toBe(2);

    expect(await botoes[0].getText()).toBe('Elogiar Colega');
    expect(await botoes[0].isDisabled()).toBe(false);

    await botoes[0].click();
    expect(fixture.componentInstance.clicado).toBe(true);

    expect(await botoes[1].isDisabled()).toBe(true);
  });
});
```

---

## 5. Roteiro de Execução Local

Para executar o projeto e comprovar a estabilidade de toda a suíte de testes:

1. **Navegue até a pasta do laboratório:**
   ```bash
   cd angular/livros/03-amigos-angular/codigo/amigos-app
   ```

2. **Execute os testes automatizados com cobertura total:**
   ```bash
   npm test
   ```
   *Resultado esperado:*
   ```text
   PASS src/app/ui/button/amg-button.spec.ts (2 testes)
   PASS src/app/ui/select/amg-select.spec.ts (4 testes)
   PASS src/app/ui/dialog/amg-dialog.spec.ts (3 testes)
   Test Suites: 3 passed, 3 total | Tests: 9 passed, 9 total (100% de sucesso)
   ```

3. **Inicie o servidor de desenvolvimento para visualização do Showcase:**
   ```bash
   npm start
   ```
   Acesse `http://localhost:4200` no seu navegador para experimentar o seletor com busca, a rolagem virtual fluida a 60 FPS e a alternância instantânea de temas.

---

## 6. Conclusão da Obra

Com a conclusão deste laboratório, provamos na prática que **a arte das relações humanas de Dale Carnegie não é apenas uma teoria de autoajuda**: ela é um guia soberano para a engenharia de software de alta performance. 

Tratar o desenvolvedor consumidor com empatia, tolerância, respeito e apoio construtivo transforma uma simples biblioteca de componentes em um movimento de entusiasmo coletivo. 

Parabéns por completar o **Livro 3** da coleção **Aprenda Angular**! 🤝🚀

---

::: quiz
? O que o Laboratório 3 demonstrou ao integrar o Angular CDK com os princípios de Dale Carnegie?
+ Que tratar o consumidor da biblioteca com empatia, acessibilidade universal e ergonomia de código transforma o software em uma ferramenta admirada e de fácil adoção.
- Que é obrigatório pagar royalties aos herdeiros de Dale Carnegie para rodar o Angular.
- Que computadores modernos não precisam de testes unitários quando usam Signals.
- Que o método Socrático substitui o compilador do TypeScript.
> A compaixão e a empatia na arquitetura técnica criam sistemas inclusivos, estáveis e prazerosos de usar.

? Por que os Component Harnesses tornam a suíte de testes do Design System infinitamente mais durável?
+ Porque isolam os testes das classes CSS cosméticas e da árvore do DOM, expondo métodos semânticos estáveis (ex: `button.click()`, `dialog.isOpen()`).
- Porque gravam os resultados dos testes em fitas cassete magnéticas.
- Porque impedem que outros programadores abram Pull Requests no repositório.
- Porque diminuem o consumo de energia da bateria do teclado sem fio.
> A camada de abstração dos Harnesses absorve futuras refatorações estéticas sem quebrar os testes dos consumidores.

? Qual é o ganho real de performance obtido com o `cdk-virtual-scroll-viewport` no `AmgVirtualListComponent`?
+ Renderiza apenas os elementos que cabem fisicamente na tela, mantendo o consumo de memória baixo e a taxa em 60 FPS estáveis mesmo com 10.000 nós.
- Faz o computador minerar criptomoedas enquanto o usuário rola a tela.
- Permite que a tela funcione mesmo com o monitor desligado da tomada.
- Duplica a velocidade do processador em tarefas de renderização 3D.
> A virtualização recicla os nós visíveis em tempo real, eliminando o gargalo de memória de grandes listas no navegador.

? Como o `AmgSelectComponent` garantiu compatibilidade com formulários reativos sem forçar código manual no consumidor?
+ Implementou rigorosamente a interface `ControlValueAccessor` (`writeValue`, `registerOnChange`, `registerOnTouched`, `setDisabledState`).
- Conectou o componente diretamente a um banco de dados MongoDB local.
- Forçou o usuário a reiniciar o servidor a cada alteração de valor.
- Substituiu todos os inputs do formulário por links do WhatsApp.
> A implementação do CVA permite usar `formControlName`, `formControl` e `[(ngModel)]` de forma 100% nativa.

? O que a função `booleanAttribute` faz no Angular 19 quando associada à propriedade `transform` de um input?
+ Converte automaticamente valores vazios (`""`) ou a presença da tag (`<amg-button disabled>`) no booleano `true`, suportando o padrão HTML nativo.
- Altera a cor de fundo do botão para azul elétrico.
- Envia um alerta sonoro para os fones de ouvido do programador.
- Desativa a verificação de tipos estritos do TypeScript.
> A utilidade `booleanAttribute` padroniza o tratamento de atributos booleanos conforme as diretrizes canônicas do ecossistema Angular.
:::

---

::: cartas
Showcase Interativo :: Aplicação de vitrine viva que demonstra componentes, estados de acessibilidade e documentação técnica em tempo real.
AmgButtonHarness :: Classe de automação de testes do botão que fornece métodos click(), isDisabled() e getText() sem tocar no DOM.
booleanAttribute :: Função de transformação nativa do Angular que normaliza atributos HTML booleanos com elegância.
AmgVirtualListComponent :: Componente de alta capacidade de dados que opera com rolagem virtual mantendo 60 FPS cravados.
Design System Amigos UI :: Biblioteca completa de componentes empáticos e acessíveis inspirada nos ensinamentos de Dale Carnegie.
Engenharia Humanizada :: Prática de desenvolvimento onde a experiência humana e a excelência técnica caminham juntas em harmonia.
:::
