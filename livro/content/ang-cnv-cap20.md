---
id: ang-cnv-cap20
order: 339
kind: capitulo
num: 237
group: Angular · Parte 59 · Bloco I — A Fundação Compassiva
title: Do Fundo do Coração
subtitle: Comunicação entre Componentes com input(), output(), model() e Contratos Limpos
minutes: 30
hue: 32
badge: ♥
image: img/ang-cnv/cap20.svg
- Compreender o relato histórico de Marshall Rosenberg no campo de refugiados de Deheishe em Belém: 
- Dominar a nova Signal-based Component API do Angular (input, input.required, output e model): 
- Implementar ligação bidirecional declarativa com model() sem manipulação oculta de estado: 
- Utilizar Signal Queries (viewChild, contentChild) e Content Projection com múltiplos slots: 
origem: angular/livros/02-cnv-angular/content/20-do-fundo-do-coracao.md
---

::: ilustracao img/ang-cnv/cap20.svg | IDE VS Code à esquerda exibindo a Signal-based Component API (input, output, model) e Browser à direita com a interface renderizada em localhost:4200.
:::

No primeiro capítulo de *Comunicação Não-Violenta*, Marshall Rosenberg compartilha um dos momentos mais tensos e reveladores de sua vida como facilitador internacional de conflitos. Marshall estava em uma mesquita no campo de refugiados de **Deheishe**, nos arredores de Belém, na Cisjordânia, palestrando para cerca de 170 homens muçulmanos palestinos. A atmosfera era pesada; na época, os refugiados viviam sob condições precárias de saneamento básico e racionamento severo de água.

De repente, uma onda de comoção percorreu o recinto. O tradutor sussurrou no ouvido de Marshall: *"Eles acabaram de descobrir que você é norte-americano!"*

Naquele instante, um homem no meio da multidão se colocou de pé, apontou o dedo indicador furiosamente para o rosto de Marshall e gritou a plenos pulmões:
> **"Assassino!"**

Imediatamente, dezenas de outros homens se levantaram em uníssono, gritando em coro: *"Assassino! Fabricante de armas! Assassino!"*

Qual seria a reação comum de um ser humano diante desse ataque?
1. **Entrar em pânico e fugir.**
2. **Revidar na mesma moeda**, gritando que ele nada tinha a ver com a política militar de seu país natal.
3. **Sentir raiva e acusar a plateia de ingratidão**.

Marshall não fez nenhuma dessas coisas. Ele respirou fundo, colocou suas **"orelhas de girafa"** e buscou se conectar com o que estava vivo no coração daquele homem:

> — *Você está com raiva porque gostaria que o meu governo usasse os recursos para apoiar o seu povo com saneamento e hospitais, em vez de enviar aviões de guerra?* — perguntou Marshall.
>
> — *É exatamente isso!* — gritou o homem. — *Você sabe o que é viver aqui? Você tem esgoto a céu aberto na sua rua? Você tem água para seus filhos beberem?*
>
> — *Você está se sentindo profundamente angustiado e com necessidade de dignidade, justiça e saneamento seguro para sua família?* — continuou Marshall, sustentando o contato empático sem se defender.

Por quase uma hora, aquele homem desabafou sua dor e indignação. Marshall apenas escutou, refletindo cada necessidade expressa. Ao término do encontro, o mesmo homem que o chamara de assassino aproximou-se de Marshall, pegou sua mão e o convidou formalmente para um jantar tradicional de Ramadã em sua casa.

Aquele episódio encerra a essência de "doar de coração": **a comunicação transparente e compassiva é capaz de transformar a mais violenta agressão em um laço inquebrável de confiança mútua.**

---

## A Anatomia Moderna de um Componente Angular (angular.dev)

No ecossistema de software, a forma como os componentes se comunicam define a saúde psicológica e técnica de toda a aplicação. No Angular moderno (a partir do Angular 17.1 e 18), a arquitetura de componentes foi completamente reconstruída para eliminar contratos opacos e mutações abusivas.

Um componente moderno é declarado com o decorador `@Component`:

```typescript
@Component({
  selector: 'app-cartao-dialogo',
  standalone: true, // Padrão nativo do Angular moderno
  imports: [CommonModule],
  templateUrl: './cartao-dialogo.component.html',
  styleUrl: './cartao-dialogo.component.css',
  changeDetection: ChangeDetectionStrategy.OnPush, // Máxima performance
  host: {
    'role': 'article',
    'class': 'cartao-escuta-ativo',
    '[class.em-tensao]': 'tensaoElevada()',
    '(keydown.enter)': 'aoPressionarEnter()'
  }
})
export class CartaoDialogoComponent { ... }
```

::: nota
Observe a propriedade `host` no `@Component`. O Angular moderno desencoraja o uso dos decoradores legados `@HostBinding()` e `@HostListener()`. O objeto literal `host` no próprio decorador oferece uma declaração centralizada, limpa e de fácil leitura para classes, atributos ARIA e ouvintes de eventos nativos do elemento hospedeiro.
:::

---

## 1. Doação Factual com input() e input.required()

No Angular anterior ao 17, utilizava-se o decorador `@Input()`. Ele possuía um problema grave de tipagem: uma propriedade declarada como `@Input() perfil!: PerfilUsuario` nascia inicialmente como `undefined` durante a inicialização da classe, forçando o desenvolvedor a escrever verificações defensivas em todo o código.

A nova API de **Signal Inputs** resolve isso com precisão contratual:

```typescript
import { Component, input, computed } from '@angular/core';

export interface MensagemEmpatia {
  readonly id: string;
  readonly autor: string;
  readonly relatoBruto: string;
  readonly nivelUrgencia: number; // 1 a 10
}

@Component({
  selector: 'app-item-conversa',
  standalone: true,
  template: `
    <div class="conversa-box">
      <h4>{{ mensagem().autor }}</h4>
      <p>{{ mensagem().relatoBruto }}</p>
      
      @if (urgenciaCritica()) {
        <span class="alerta-urgente">Acolhimento Imediato Necessário</span>
      }
    </div>
  `
})
export class ItemConversaComponent {
  // 1. Sinal de entrada obrigatório: o build do Angular falha se o pai não passar
  readonly mensagem = input.required<MensagemEmpatia>();

  // 2. Sinal de entrada opcional com valor default seguro
  readonly modoDiscreto = input<boolean>(false);

  // 3. Sinal de entrada com transformação pura integrada
  readonly limiteCaracteres = input(140, {
    transform: (valor: number | string) => {
      const num = typeof valor === 'string' ? parseInt(valor, 10) : valor;
      return isNaN(num) ? 140 : Math.max(20, num);
    }
  });

  // 4. Derivação pura reativa via computed: recalculada sob demanda sem custos extras
  readonly urgenciaCritica = computed(() => this.mensagem().nivelUrgencia >= 8);
}
```

### Por que `input()` retorna um `InputSignal<T>`?

Porque `this.mensagem` é um **Signal somente-leitura**. Se alguém tentar fazer `this.mensagem.set(...)` ou mutar o objeto recebido do pai, o compilador do TypeScript emite um erro imediato:

```saida
error TS2540: Cannot assign to 'mensagem' because it is a read-only property.
```

Essa restrição protege a soberania do componente pai: **quem recebe o dado pode observá-lo, mas nunca corrompê-lo.**

---

## 2. Pedidos Sem Exigência com output() e outputFromObservable()

Quando o componente filho precisa se comunicar com o pai, ele não deve tentar adivinhar ou manipular o estado externo. Ele emite um evento afirmativo através da nova função `output()`:

```typescript
import { Component, output } from '@angular/core';

@Component({
  selector: 'app-acoes-escuta',
  standalone: true,
  template: `
    <button type="button" (click)="aoAcolher()">Acolher com Amor</button>
    <button type="button" (click)="aoPedirPausa()">Solicitar Silêncio</button>
  `
})
export class AcoesEscutaComponent {
  // Emissor de eventos nativo, tipado e independente de RxJS
  readonly acolhimentoDisparado = output<{ idAcao: string; momento: Date }>();
  readonly pausaSolicitada = output<void>();

  aoAcolher(): void {
    this.acolhimentoDisparado.emit({
      idAcao: 'acolher-urgente',
      momento: new Date()
    });
  }

  aoPedirPausa(): void {
    this.pausaSolicitada.emit();
  }
}
```

Se você estiver integrando fluxos reativos do RxJS com eventos de saída, o Angular fornece a função `outputFromObservable()`:

```typescript
import { outputFromObservable } from '@angular/core/rxjs-interop';

// Emite eventos de saída diretamente a partir de um Observable com cancelamento automático
readonly buscaModificada = outputFromObservable(this.termoBusca$.pipe(debounceTime(300)));
```

---

## 3. A Conexão Sincronizada com model()

Muitas vezes precisamos de sincronização bidirecional (*Two-Way Binding*), como em um interruptor de modo escuro ou em uma caixa de seleção. Antes, era necessário declarar um `@Input()` e um `@Output()` com o sufixo `Change` (ex: `[(aberto)]="estaAberto"` exigia `@Input() aberto` e `@Output() abertoChange`).

O Angular 17.2+ introduziu a elegante primitiva **`model()`**:

```typescript
import { Component, model } from '@angular/core';

@Component({
  selector: 'app-switch-empatia',
  standalone: true,
  template: `
    <button type="button" [class.ativo]="ativo()" (click)="alternar()">
      Modo Girafa: {{ ativo() ? 'Ligado' : 'Desligado' }}
    </button>
  `
})
export class SwitchEmpatiaComponent {
  // model() cria um ModelSignal: pode ser lido, atualizado localmente e sincronizado com o pai
  readonly ativo = model<boolean>(false);

  alternar(): void {
    // Altera o estado local E notifica automaticamente o componente pai
    this.ativo.update(estadoAnterior => !estadoAnterior);
  }
}
```

No template do componente pai, o consumo é direto:

```html
<!-- Ligação bidirecional com a sintaxe banana-in-a-box [()] -->
<app-switch-empatia [(ativo)]="modoGirafaHabilitado" />
```

---

## 4. Consultas Reativas: viewChild e contentChild

Para inspecionar elementos do template ou do conteúdo projetado, o Angular moderno substituiu os decoradores `@ViewChild` e `@ContentChild` por **Signal Queries**:

```typescript
import { Component, viewChild, ElementRef, afterNextRender } from '@angular/core';

@Component({
  selector: 'app-foco-escuta',
  standalone: true,
  template: `
    <input #campoTexto type="text" placeholder="Expresse sua observação..." />
  `
})
export class FocoEscutaComponent {
  // viewChild retorna um Signal: Signal<ElementRef<HTMLInputElement> | undefined>
  readonly campoTexto = viewChild<ElementRef<HTMLInputElement>>('campoTexto');

  constructor() {
    // afterNextRender: hook oficial para manipulação segura do DOM após renderização
    afterNextRender(() => {
      this.campoTexto()?.nativeElement.focus();
    });
  }
}
```

::: dica
Observe o uso de `afterNextRender()`. Ele executa **apenas no navegador** e substitui as antigas checagens manuais de plataforma (`isPlatformBrowser`) dentro de `ngAfterViewInit`, garantindo compatibilidade total com Server-Side Rendering (SSR) e Hydration.
:::

---

## 5. Projeção de Conteúdo Multi-Slot com ng-content

Para construir componentes flexíveis que acolhem diferentes partes de layout (como cabeçalho, corpo e rodapé de um cartão de diálogo), utilizamos a projeção de conteúdo com o atributo `select`:

```html
<!-- Template de app-painel-compassivo -->
<div class="painel-compassivo">
  <header class="painel-topo">
    <!-- Slot 1: Projeta apenas o elemento com o atributo [cabecalho] -->
    <ng-content select="[cabecalho]" />
  </header>

  <main class="painel-corpo">
    <!-- Slot Padrão: Projeta qualquer conteúdo não selecionado -->
    <ng-content />
  </main>

  <footer class="painel-rodape">
    <!-- Slot 3 com Fallback: Se o pai não passar rodapé, exibe o conteúdo padrão abaixo -->
    <ng-content select="[rodape]">
      <span class="fallback-rodape">Espaço de escuta ativa protegido por CNV.</span>
    </ng-content>
  </footer>
</div>
```

---

::: quiz
? Qual foi a atitude de Marshall Rosenberg ao ser chamado de "assassino" no campo de refugiados de Deheishe?
- Chamou a segurança do acampamento e saiu escoltado
- Gritou de volta dizendo que ele não era culpado pelas ações dos militares
+ Escutou ativamente a dor e as necessidades de saneamento, água e dignidade por trás daquele grito, sem se defender nem contra-atacar
- Processou os organizadores da mesquita
> Marshall demonstrou na prática o poder da CNV: enxergar o ser humano e a necessidade não atendida por trás de palavras aparentemente hostis e violentas.

? O que a nova função `model()` do Angular 17.2+ simplifica na arquitetura de componentes?
- Permite conectar o Angular diretamente a bancos de dados SQL
+ Cria uma propriedade de ligação bidirecional (two-way binding `[()]`) de forma concisa através de um `ModelSignal`, dispensando o par manual `@Input()` e `@Output() ...Change`
- Desativa a compilação do TypeScript
- Converte formulários em arquivos de texto
> `model()` unifica a declaração de propriedades bidirecionais: ele se comporta como um sinal gravável que emite notificações de sincronização para o pai automaticamente.

? Qual é a vantagem de utilizar a propriedade `host` no decorador `@Component` em vez de `@HostBinding` e `@HostListener`?
- Permite rodar código C++ no navegador
+ Centraliza a declaração de classes, atributos ARIA e ouvintes de eventos do elemento hospedeiro em um único local declarativo, melhorando a manutenibilidade
- Aumenta o tamanho do pacote JavaScript compilado
- Impede que o componente seja estilizado com CSS
> A propriedade `host` do `@Component` é o padrão moderno recomendado pela equipe do Angular em angular.dev, tornando o código mais conciso e fácil de ler.

? Para que serve a função de ciclo de vida `afterNextRender()` introduzida no Angular moderno?
- Para fechar o navegador após 10 segundos
+ Para executar código de manipulação de DOM de forma segura no navegador após a renderização, prevenindo falhas durante Server-Side Rendering (SSR)
- Para reiniciar o servidor Node.js
- Para limpar os cookies de sessão a cada clique
> `afterNextRender` e `afterRender` são projetados para interações diretas com APIs do navegador (como focar elementos ou calcular dimensões), garantindo que esse código nunca seja invocado indevidamente no servidor.

? O que a propriedade `transform` na função `input()` permite realizar?
- Traduzir a tela para o inglês
+ Higienizar, validar ou converter o tipo do dado recebido do componente pai (ex: string para number) de forma pura antes de disponibilizá-lo internamente
- Criptografar o HTML da página
- Enviar notificações push para celulares
> A opção `transform` acolhe o dado bruto e o converte suavemente, evitando que o componente precise de métodos verbosos de ciclo de vida como `ngOnChanges`.
:::

---

::: cartas
Doar de Coração (CNV) :: Agir motivado pelo desejo espontâneo e compassivo de contribuir para a vida do outro, livre de coerção ou barganha.
Campo de Deheishe :: Local histórico onde Marshall Rosenberg desarmou a hostilidade de 170 refugiados através da escuta atenta das necessidades de água e dignidade.
input.required() :: Sinal de entrada obrigatório cuja presença é validada rigorosamente pelo compilador em tempo de build.
output() no Angular :: API moderna para emissão declarativa de eventos entre componentes sem dependência de EventEmitter legados.
model() no Angular :: Primitiva que cria uma ligação bidirecional reativa (`[(prop)]`), integrando leitura e escrita sincronizadas.
viewChild() Signal Query :: Consulta de template moderna que retorna um Signal contendo a referência do elemento ou componente filho.
afterNextRender :: Callback moderno executado exclusivamente no cliente após a próxima conclusão de renderização do DOM.
Multi-Slot Content Projection :: Capacidade de projetar múltiplos blocos de template em slots nomeados via `<ng-content select="...">`.
:::
