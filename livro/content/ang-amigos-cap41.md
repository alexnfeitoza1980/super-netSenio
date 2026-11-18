---
id: ang-amigos-cap41
order: 362
kind: capitulo
num: 258
group: Angular · Parte 66 · Seis Maneiras de Fazer os Desenvolvedores Adorarem sua Biblioteca
title: Faça o Outro Sentir-se Importante
subtitle: Theming com CSS Custom Properties e Menus Flutuantes via Angular CDK Overlay
minutes: 30
hue: 215
badge: 🤝
image: img/ang-amigos/cap41.svg
- Analisar o episódio do funcionário do correio de Dale Carnegie e a lei suprema da conduta humana: 
- Reconhecer que bibliotecas amigáveis não aprisionam o consumidor em estilos CSS rígidos ou seletores com !important: 
- Estruturar um sistema de Theming flexível baseado em CSS Custom Properties nativas (--amg-primary, --amg-surface): 
- Dominar o pacote @angular/cdk/overlay para renderizar painéis flutuantes fora da hierarquia visual do DOM: 
- Eliminar guerras de z-index e problemas de corte visual causados por overflow: hidden em menus e tooltips
origem: angular/livros/03-amigos-angular/content/41-theming-cdk-overlay.md
---

::: ilustracao img/ang-amigos/cap41.svg | Browser exibindo menu flutuante ancorado com CDK Overlay desacoplado do fluxo normal e seletor de temas baseado em variáveis CSS dinâmicas.
:::

Certa tarde, Dale Carnegie encontrava-se em uma longa e entediante fila na agência dos Correios na Rua Trinta e Três, em Nova York. O atendente atrás do guichê parecia exausto, desanimado e mecanizado: pesava envelopes, colava selos, passava o troco e carimbava recibos com o olhar vago de quem odiava a própria rotina.

Carnegie pensou consigo mesmo: *"Vou tentar fazer com que esse rapaz goste de mim. Para conseguir isso, preciso dizer algo agradável sobre ele — e não sobre mim. O que há nele que eu possa admirar com honestidade sincera?"*.

Carnegie olhou com atenção e notou que o homem possuía uma cabeça de cabelos espessos, ondulados e impecavelmente tratados. Quando chegou a sua vez no guichê, enquanto o atendente pesava sua correspondência, Carnegie disse com entusiasmo:

> *"Como eu gostaria de ter uma cabeleira bonita como a sua!"*

O funcionário ergueu os olhos, surpreso. Seu rosto iluminou-se com um sorriso largo e genuíno. *"Bem, não está tão boa agora quanto já esteve no passado"*, respondeu ele com modéstia tocante. Carnegie assegurou-lhe que, embora talvez tivesse perdido um pouco do brilho da juventude, continuava magnífica. Os dois conversaram com extrema cordialidade, e Carnegie saiu da agência sabendo que havia colocado um raio de sol no dia daquele trabalhador.

Carnegie formula então a lei número um das relações humanas:

> *"Faça sempre a outra pessoa sentir-se importante. Obedeça à Regra de Ouro: faça aos outros o que você gostaria que os outros fizessem a você. Todos nós desejamos a aprovação daqueles com quem convivemos; queremos ser reconhecidos em nossos méritos e respeitados em nossa dignidade."*

---

## O Desrespeito do CSS Inflexível e a Prisão do `z-index`

Traga essa lei de ouro para a arquitetura de estilização de componentes frontend.

Quantas vezes você já instalou uma biblioteca e se deparou com este horror:

```css
/* A biblioteca egocêntrica */
.btn-corp {
  background-color: #7c3aed !important;
  font-family: 'Comic Sans MS', sans-serif !important;
  border-radius: 0px !important;
  z-index: 999999 !important;
}
```

O autor da biblioteca decidiu que o botão dele **precisa** ser roxo com bordas quadradas. Ele usou `!important` para impedir que você, desenvolvedor da empresa, adeque o componente à identidade visual da sua marca. Ele cuspiu na sua importância.

E para piorar, quando você coloca um dropdown dentro de uma tabela com scroll ou de um modal com `overflow: hidden`, o menu flutuante é cortado pela metade! O desenvolvedor passa três dias brigando contra o CSS, aplicando `z-index: 2147483647`, apenas para descobrir que o contexto de empilhamento do CSS (*Stacking Context*) impede que o elemento saia da caixa-mãe.

Isso é uma humilhação ao desenvolvedor consumidor.

---

## Theming Empático: O Poder das CSS Custom Properties

Uma biblioteca que faz o desenvolvedor sentir-se importante não impõe suas cores como dogmas; ela fornece **CSS Custom Properties** (variáveis CSS) documentadas e acessíveis no seletor `:root` ou no próprio `:host`.

Vejamos como definir o contrato de estilo do `AmgMenuComponent`:

```css
/* amigos-theme-contract.css */
:root {
  --amg-primary: #2563eb;
  --amg-primary-hover: #1d4ed8;
  --amg-surface: #ffffff;
  --amg-text: #0f172a;
  --amg-border: #e2e8f0;
  --amg-radius: 8px;
  --amg-shadow: 0 10px 15px -3px rgba(0, 0, 0, 0.1);
}

/* O consumidor pode trocar de tema mudando apenas um atributo data-theme */
[data-theme="dark"] {
  --amg-primary: #3b82f6;
  --amg-surface: #0f172a;
  --amg-text: #f8fafc;
  --amg-border: #334155;
}
```

Qualquer componente da biblioteca consome essas variáveis com fallbacks seguros:

```css
.amg-menu-panel {
  background: var(--amg-surface, #ffffff);
  color: var(--amg-text, #0f172a);
  border: 1px solid var(--amg-border, #e2e8f0);
  border-radius: var(--amg-radius, 8px);
  box-shadow: var(--amg-shadow);
}
```

---

## Libertando Menus com `@angular/cdk/overlay`

Para resolver de vez as guerras de `z-index` e os cortes de `overflow: hidden`, utilizamos o pacote **`@angular/cdk/overlay`**. 

O CDK Overlay não renderiza o menu dentro da hierarquia da tabela ou do card; ele renderiza o painel **no final da tag `<body>`**, dentro de uma camada especial (`.cdk-overlay-container`), e o conecta visualmente ao elemento gatilho com cálculos matemáticos de posicionamento flexível.

Vejamos como implementar um menu flutuante desacoplado:

```typescript
// amg-menu-trigger.directive.ts
import { 
  Directive, 
  ElementRef, 
  TemplateRef, 
  inject, 
  input, 
  OnDestroy 
} from '@angular/core';
import { 
  Overlay, 
  OverlayRef, 
  FlexibleConnectedPositionStrategy 
} from '@angular/cdk/overlay';
import { TemplatePortal } from '@angular/cdk/portal';

@Directive({
  selector: '[amgMenuTriggerFor]',
  standalone: true,
  host: {
    '(click)': 'toggleMenu()',
    '[attr.aria-haspopup]': '"menu"',
    '[attr.aria-expanded]': 'isOpen'
  }
})
export class AmgMenuTriggerDirective implements OnDestroy {
  private readonly elementRef = inject(ElementRef);
  private readonly overlay = inject(Overlay);

  /** Referência do ng-template que contém os itens do menu */
  readonly menuTemplate = input.required<TemplateRef<unknown>>({
    alias: 'amgMenuTriggerFor'
  });

  private overlayRef: OverlayRef | null = null;
  protected isOpen = false;

  protected toggleMenu(): void {
    if (this.isOpen) {
      this.closeMenu();
    } else {
      this.openMenu();
    }
  }

  private openMenu(): void {
    // 1. Estratégia de Posicionamento Conectado Inteligente
    const positionStrategy: FlexibleConnectedPositionStrategy = this.overlay
      .position()
      .flexibleConnectedTo(this.elementRef)
      .withPositions([
        // Posição preferencial: abaixo e alinhado à esquerda
        { originX: 'start', originY: 'bottom', overlayX: 'start', overlayY: 'top', offsetY: 6 },
        // Fallback: se não couber embaixo, exibe acima sem estourar a tela!
        { originX: 'start', originY: 'top', overlayX: 'start', overlayY: 'bottom', offsetY: -6 }
      ]);

    // 2. Criação do Overlay no final do DOM
    this.overlayRef = this.overlay.create({
      positionStrategy,
      hasBackdrop: true,
      backdropClass: 'cdk-overlay-transparent-backdrop',
      scrollStrategy: this.overlay.scrollStrategies.reposition()
    });

    // 3. Fechamento suave ao clicar fora ou apertar ESC
    this.overlayRef.backdropClick().subscribe(() => this.closeMenu());
    this.overlayRef.keydownEvents().subscribe(event => {
      if (event.key === 'Escape') this.closeMenu();
    });

    // 4. Conecta o template ao portal
    const portal = new TemplatePortal(this.menuTemplate(), null!);
    this.overlayRef.attach(portal);
    this.isOpen = true;
  }

  private closeMenu(): void {
    if (this.overlayRef) {
      this.overlayRef.dispose();
      this.overlayRef = null;
    }
    this.isOpen = false;
  }

  ngOnDestroy(): void {
    this.closeMenu();
  }
}
```

---

## O Uso Elegante no Template

Veja como o desenvolvedor consome o menu. Ele pode colocar o gatilho em qualquer lugar, até mesmo dentro de uma célula minúscula de tabela:

```html
<!-- Gatilho que ancora o menu -->
<button 
  type="button" 
  class="btn-acao"
  [amgMenuTriggerFor]="menuOpcoes">
  Ações ▾
</button>

<!-- Template do Menu desacoplado, sem conflito de overflow ou z-index -->
<ng-template #menuOpcoes>
  <div class="amg-menu-panel" role="menu">
    <button type="button" role="menuitem" (click)="elogiar()">✨ Elogiar Colega</button>
    <button type="button" role="menuitem" (click)="agradecer()">💌 Agradecer em Particular</button>
    <hr style="border: 0; border-top: 1px solid var(--amg-border);" />
    <button type="button" role="menuitem" style="color: #ef4444;" (click)="remover()">🗑️ Excluir Registro</button>
  </div>
</ng-template>
```

Você concedeu total importância ao desenvolvedor. As variáveis CSS permitiram que ele adotasse a paleta da própria empresa em dois minutos; o CDK Overlay garantiu que seu menu flutue soberanamente sobre o restante da tela sem cortes nem dores de cabeça.

A Regra de Ouro foi cumprida com maestria.

---

::: quiz
? Qual foi a atitude de Dale Carnegie com o atendente dos correios que transformou o clima do atendimento?
+ Elogiou sinceramente a beleza natural dos cabelos do rapaz, fazendo-o sentir-se valorizado em sua dignidade.
- Prometeu pagar o dobro pelo frete de suas cartas.
- Denunciou a lentidão do atendimento para o gerente da agência.
- Pediu para tirar uma selfie com o funcionário para postar nas redes sociais.
> Carnegie procurou um ponto real e genuíno para admirar no outro, aplicando a máxima de fazer o interlocutor sentir-se importante.

? Por que o uso indiscriminado de `!important` e estilos rígidos em bibliotecas de componentes é considerado um desrespeito ao consumidor?
+ Porque impede a customização legítima e dificulta a adequação dos componentes à identidade visual e às diretrizes da marca da empresa consumidora.
- Porque faz o navegador do cliente consumir 100% da CPU em cálculos trigonométricos.
- Porque anula a validade da licença do software de código aberto.
- Porque faz o editor de código fechar sem salvar as alterações.
> Estilos rígidos tiram o poder das mãos do desenvolvedor, gerando frustração e gambiarras visuais.

? Qual é o benefício arquitetural de utilizar CSS Custom Properties (`--variavel`) para o theming de uma biblioteca?
+ Permite que temas sejam trocados em tempo de execução mudando apenas atributos de escopo no DOM, com herança natural e sem refazer o build.
- Elimina a necessidade de instalar o Node.js na máquina do desenvolvedor.
- Converte cores RGB em frequências de rádio AM/FM.
- Reduz a conta de energia do data center em 50%.
> As variáveis CSS são dinâmicas, herdam valores através do DOM e funcionam nativamente em todos os navegadores modernos sem overhead.

? Qual problema crítico o pacote `@angular/cdk/overlay` resolve em menus, tooltips e popovers?
+ Renderiza o elemento flutuante no final da tag `<body>`, livrando-o de cortes visuais provocados por `overflow: hidden` e conflitos de `z-index`.
- Conecta o computador do usuário a satélites espaciais para acelerar o ping.
- Substitui o motor de renderização do Chrome pelo motor do Safari.
- Bloqueia anúncios indesejados em vídeos do YouTube.
> O CDK Overlay desassocia a renderização física do menu do elemento pai que o invocou, garantindo posicionamento matemático livre no topo do DOM.

? O que a estratégia `FlexibleConnectedPositionStrategy` do CDK faz quando um menu não cabe abaixo do botão gatilho?
+ Recalcula a posição em tempo real e inverte a renderização para cima do botão, evitando que o menu transborde para fora da tela.
- Exibe uma mensagem de erro vermelha no meio da página.
- Corta as opções que não couberem na tela sem avisar o usuário.
- Reduz o tamanho da fonte para 2 pixels.
> A flexibilidade de posicionamento do CDK calcula os limites da janela de visualização (*viewport*) e escolhe o melhor encaixe geométrico com suavidade.
:::

---

::: cartas
A Regra de Ouro :: Faça aos outros o que gostaria que fizessem a você: faça a outra pessoa sentir-se importante com sinceridade.
O Atendente do Correio :: Caso real de Carnegie demonstrando como um elogio honesto ilumina o dia de quem está sobrecarregado.
Theming Empático :: Estrutura de design system que expõe variáveis CSS para que cada time aplique suas próprias cores sem atrito.
CSS Custom Properties :: Padrão web nativo (--nome) para parametrização declarativa de estilos em tempo de execução.
Angular CDK Overlay :: Mecanismo de alta performance para renderizar elementos flutuantes ancorados fora da árvore DOM imediata.
Contexto de Empilhamento :: Regra do CSS (Stacking Context) que isola z-index dentro de elementos ancestrais com transform ou overflow.
:::
