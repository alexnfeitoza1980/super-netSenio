---
id: ang-cnv-cap31
order: 350
kind: capitulo
num: 248
group: Angular · Parte 63 · Bloco V — Maturidade e Celebração
title: Libertando-nos e Aconselhando os Outros
subtitle: Design Systems Agnósticos, Diretivas Customizadas e Componentes Puros
minutes: 30
hue: 32
badge: 🕊️
image: img/ang-cnv/cap31.svg
- Analisar a história de Buddy Hackett sobre o condicionamento cultural invisível na CNV: 
- Diferenciar com rigor arquitetural Componentes Inteligentes (Smart) de Componentes Puros (Dumb): 
- Criar um Design System resiliente com Design Tokens e CSS Custom Properties: 
- Implementar Diretivas e Pipes customizados e agnósticos no ecossistema Angular moderno: 
origem: angular/livros/02-cnv-angular/content/31-libertando-condicionamentos.md
---

::: ilustracao img/ang-cnv/cap31.svg | Catálogo do Storybook / Design System em Alta Fidelidade: Componentes puros agnósticos (BotaoCompassivo, AvatarBadge, PillSentimento) estilizados com Design Tokens sem acoplamento de negócio.
:::

No décimo segundo capítulo de sua obra clássica, Marshall Rosenberg nos confronta com uma das realidades mais sutis e perigosas da experiência humana: **o condicionamento cultural invisível**.

Marshall cita uma anedota clássica do comediante norte-americano Buddy Hackett. Hackett contava que passou toda a sua infância e juventude comendo a comida extremamente gordurosa e pesada que sua mãe cozinhava todos os dias. Somente quando completou 18 anos e ingressou no Exército dos Estados Unidos é que ele descobriu, com espanto, **que era possível terminar uma refeição sem sentir azia e dor no estômago!**

A azia era uma dor tão crônica e constante na vida de Buddy Hackett que ele achava que aquele mal-estar era uma lei da natureza, e não uma consequência direta dos hábitos culinários de sua família.

Marshall explica:
> *"Da mesma maneira, a dor ocasionada por condicionamentos culturais nocivos — a crença de que devemos nos culpar, que devemos obedecer cegamente por obrigação e que temos que agradar aos outros a todo custo — é uma parte tão integrante da nossa vida que nem temos mais consciência de sua presença."*

Na engenharia de software com Angular em [angular.dev/overview](https://angular.dev/overview), testemunhamos exatamente essa "azia arquitetural":
- Desenvolvedores acostumados a criar componentes de 1.500 linhas que misturam CSS, requisições diretas a cinco APIs diferentes, cálculos de impostos e manipulações imperativas de DOM.
- A equipe acha que esse sofrimento é "normal" em projetos corporativos grandes.
- Nenhum botão ou modal pode ser reaproveitado em outra página porque está amarrado aos condicionamentos e preconceitos do módulo legado.

Atingir a **libertação arquitetural** significa quebrar esse ciclo vicioso através de **Componentes Puros e um Design System Agnóstico**.

---

## A Dança Sagrada: Smart Components vs Dumb Components

Para libertar nossos componentes visuais, dividimos a arquitetura em dois papéis claros e respeitosos:

```
[ Componente Inteligente (Smart / Container) ]
  • Gerencia Signals de domínio e regras de negócio
  • Injeta serviços, APIs e roteamento
  • Decide o que fazer quando ações acontecem
                     │
                     ▼  Doa dados limpos (input)
┌─────────────────────────────────────────────────────┐
│    Componente de UI Puro (Dumb / Presentational)    │
│  • Totalmente agnóstico a regras de negócio        │
│  • Vive no Design System                           │
│  • Emite intenções puras (output)                  │
└─────────────────────────────────────────────────────┘
                     ▲
                     │  Recebe cliques e gestos do ser humano
```

Um componente de UI puro não sabe se está em um e-commerce, em um banco ou em uma organização pacifista. Ele é livre. Ele apenas recebe dados visuais e notifica eventos do usuário.

---

## Construindo um Botão Compassivo de Design System

Vejamos como criar um componente de botão totalmente agnóstico, estilizado através de **Design Tokens** (variáveis CSS) e pronto para ser usado em qualquer lugar da aplicação:

```typescript
// src/app/shared/ui/botao-compassivo/botao-compassivo.component.ts
import { Component, input, output, computed } from '@angular/core';
import { CommonModule } from '@angular/common';

export type VarianteBotao = 'compassivo' | 'neutro' | 'alerta';
export type TamanhoBotao = 'compacto' | 'normal' | 'amplo';

@Component({
  selector: 'app-botao-compassivo',
  standalone: true,
  imports: [CommonModule],
  template: `
    <button
      [type]="tipoBotao()"
      [disabled]="desabilitado() || carregando()"
      [class]="classesCalculadas()"
      (click)="aoClicar($event)"
      [attr.aria-busy]="carregando()"
    >
      @if (carregando()) {
        <span class="spinner-suave" aria-hidden="true"></span>
        <span class="texto-loading">{{ textoCarregamento() }}</span>
      } @else {
        @if (icone()) {
          <span class="icone" aria-hidden="true">{{ icone() }}</span>
        }
        <ng-content />
      }
    </button>
  `,
  styles: [`
    button {
      display: inline-flex;
      align-items: center;
      justify-content: center;
      gap: 0.5rem;
      border: 1px solid transparent;
      border-radius: var(--raio-borda, 8px);
      font-family: inherit;
      font-weight: 600;
      cursor: pointer;
      transition: all 0.2s cubic-bezier(0.4, 0, 0.2, 1);
    }

    /* Variantes baseadas em Design Tokens */
    .btn-compassivo {
      background-color: var(--cor-compassiva, #d96b27);
      color: white;
    }
    .btn-compassivo:hover:not(:disabled) {
      background-color: var(--cor-compassiva-hover, #b45309);
    }

    .btn-neutro {
      background-color: var(--cor-superficie-2, #fdf4eb);
      color: var(--cor-tinta, #2a1508);
      border-color: var(--cor-linha, #f5dfd0);
    }
    .btn-neutro:hover:not(:disabled) {
      background-color: #fce7d2;
    }

    .btn-alerta {
      background-color: #ef4444;
      color: white;
    }

    /* Tamanhos */
    .tam-compacto { padding: 0.35rem 0.75rem; font-size: 0.85rem; }
    .tam-normal { padding: 0.6rem 1.25rem; font-size: 0.95rem; }
    .tam-amplo { padding: 0.85rem 1.75rem; font-size: 1.1rem; }

    /* Estados */
    button:disabled { opacity: 0.55; cursor: not-allowed; }
    button:focus-visible { outline: 3px solid #f59e0b; outline-offset: 2px; }

    .spinner-suave {
      width: 14px; height: 14px;
      border: 2px solid currentColor;
      border-top-color: transparent;
      border-radius: 50%;
      animation: rodar 0.8s linear infinite;
    }
    @keyframes rodar { to { transform: rotate(360deg); } }
  `]
})
export class BotaoCompassivoComponent {
  // Contratos limpos, tipados e agnósticos
  readonly variante = input<VarianteBotao>('compassivo');
  readonly tamanho = input<TamanhoBotao>('normal');
  readonly tipoBotao = input<'button' | 'submit' | 'reset'>('button');
  readonly desabilitado = input<boolean>(false);
  readonly carregando = input<boolean>(false);
  readonly icone = input<string>('');
  readonly textoCarregamento = input<string>('Processando com cuidado...');

  // Evento puro emitido para o componente hospedeiro
  readonly clique = output<MouseEvent>();

  readonly classesCalculadas = computed(() => {
    return `btn-${this.variante()} tam-${this.tamanho()}`;
  });

  aoClicar(evento: MouseEvent): void {
    if (!this.desabilitado() && !this.carregando()) {
      this.clique.emit(evento);
    }
  }
}
```

---

## Diretivas Customizadas: Estendendo o DOM com Empatia

No Angular moderno, criamos diretivas standalone que acolhem e enriquecem o comportamento dos elementos HTML sem acoplá-los:

```typescript
// src/app/shared/directives/realce-empatia.directive.ts
import { Directive, ElementRef, inject, input, afterNextRender } from '@angular/core';

@Directive({
  selector: '[appRealceEmpatia]',
  standalone: true,
  host: {
    '[style.transition]': '"all 0.3s ease"',
    '(mouseenter)': 'aoEntrar()',
    '(mouseleave)': 'aoSair()'
  }
})
export class RealceEmpatiaDirective {
  private readonly el = inject(ElementRef<HTMLElement>);

  // Permite customizar a cor de realce via Input Signal
  readonly corRealce = input<string>('rgba(217, 107, 39, 0.08)');

  aoEntrar(): void {
    this.el.nativeElement.style.backgroundColor = this.corRealce();
  }

  aoSair(): void {
    this.el.nativeElement.style.backgroundColor = '';
  }
}
```

---

## Pipes Customizados Puros: Transformação Serena de Dados

Para formatar tempos e durações de diálogos sem sobrecarregar a detecção de mudanças, usamos **Pipes Puros (`pure: true`)**:

```typescript
// src/app/shared/pipes/tempo-acolhimento.pipe.ts
import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'tempoAcolhimento',
  standalone: true,
  pure: true // Memorizado: só recalcula se a referência de entrada mudar
})
export class TempoAcolhimentoPipe implements PipeTransform {
  transform(data: Date | string | null | undefined): string {
    if (!data) return 'Momento sereno';

    const d = typeof data === 'string' ? new Date(data) : data;
    const diferencaMinutos = Math.round((Date.now() - d.getTime()) / 60000);

    if (diferencaMinutos <= 1) return 'Acolhido agora mesmo';
    if (diferencaMinutos < 60) return `Acolhido há ${diferencaMinutos} minutos`;
    const horas = Math.floor(diferencaMinutos / 60);
    return `Acolhido há ${horas} hora(s)`;
  }
}
```

---

::: quiz
? O que a história de Buddy Hackett ensina sobre os condicionamentos na CNV e no desenvolvimento de software?
- Que devemos comer comida pesada antes de programar
+ Que nos acostumamos tanto com a dor crônica de processos e códigos ruins que passamos a achar que o sofrimento é natural, até conhecermos uma arquitetura limpa e compassiva
- Que cozinhar é mais fácil do que aprender Angular
- Que o Exército é o melhor lugar para aprender TypeScript
> Hackett só descobriu que a azia não era uma lei biológica quando experimentou outro padrão. Da mesma forma, desenvolvedores só percebem o peso do código acoplado quando experimentam a leveza de componentes puros e Signals.

? Qual é a diferença fundamental entre um Componente Inteligente (Smart) e um Componente Puro (Dumb)?
- Componentes inteligentes usam inteligência artificial do Google
+ Componentes inteligentes orquestram regras de negócio e serviços de dados, enquanto componentes puros cuidam unicamente da apresentação visual, recebendo dados via inputs e emitindo eventos via outputs
- Componentes puros não aceitam CSS
- Componentes inteligentes são proibidos em produção
> A separação estrita liberta os componentes visuais de regras de negócio específicas, permitindo que eles vivam em Design Systems e sejam reutilizados em qualquer tela sem quebras.

? O que são Design Tokens em uma arquitetura de Design System moderno?
- Moedas para comprar licenças de software
+ Variáveis semânticas padronizadas (CSS Custom Properties) que encapsulam decisões visuais (cores, espaçamentos, bordas, fontes), unificando a linguagem de designers e engenheiros
- Plugins do Visual Studio Code
- Scripts que formatam o código automaticamente
> Design Tokens criam uma linguagem comum e facilitam temas globais (claro, escuro, alto contraste) com manutenção centralizada e desacoplada.

? Por que os Pipes do Angular são configurados como puros (`pure: true`) por padrão?
- Porque eles são escritos em linguagens puras
+ Porque o Angular memoriza o resultado e só reexecuta o método `transform()` se os argumentos de entrada mudarem, evitando recálculos pesados a cada ciclo de renderização
- Porque eles não aceitam números negativos
- Porque eles funcionam sem internet
> Pipes puros garantem máxima performance e determinismo matemático na transformação de dados no template.

? Para que serve a diretiva customizada standalone criada neste capítulo?
- Para formatar o disco rígido do usuário
+ Para adicionar comportamentos reutilizáveis e acolhedores a qualquer elemento do DOM (como realces visuais suaves) de forma declarativa e desacoplada
- Para impedir o uso do teclado no formulário
- Para deletar elementos da página
> Diretivas standalone estendem o comportamento dos elementos HTML de maneira limpa, sem poluir os componentes com manipulações manuais de estilos ou eventos.
:::

---

::: cartas
Libertação de Condicionamentos :: Processo de reconhecer e transcender hábitos automáticos nocivos herdados da cultura ou de códigos legados.
História de Buddy Hackett :: Anedota de Marshall Rosenberg ilustrando como nos acostumamos com o sofrimento crônico achando que ele é inevitável.
Smart Component :: Componente orquestrador de domínio que gerencia serviços, regras de negócio e estado reativo.
Dumb Component :: Componente de UI puro, agnóstico de regras de negócio, que depende estritamente de inputs e outputs.
Design Tokens :: Variáveis centrais (CSS Custom Properties) que expressam decisões visuais de cores, tipografia e espaçamentos.
Diretiva Standalone :: Classe decorada com `@Directive` que adiciona comportamentos customizados a elementos do DOM.
Pipe Puro (pure: true) :: Função de transformação de template memorizada que só reexecuta quando seus argumentos de entrada sofrem alteração.
Content Projection :: Injeção declarativa de blocos de template arbitrários dentro de um componente filho via `<ng-content>`.
:::
