---
id: ang-amigos-cap39
order: 360
kind: capitulo
num: 256
group: Angular · Parte 66 · Seis Maneiras de Fazer os Desenvolvedores Adorarem sua Biblioteca
title: Seja um Bom Ouvinte
subtitle: Escuta Reativa e Acessibilidade Profissional com Angular CDK A11y
minutes: 30
hue: 215
badge: 🤝
image: img/ang-amigos/cap39.svg
- Compreender a história clássica do botânico no banquete e por que ouvir com reverência é o maior elogio concebível: 
- Traduzir a "arte de ouvir" para a escuta de eventos do usuário (teclado, foco e leitores de tela) no Angular 19: 
- Utilizar o serviço LiveAnnouncer do Angular CDK para emitir anúncios falados gentis para tecnologias assistivas: 
- Implementar FocusMonitor para distinguir se o usuário interagiu via mouse, teclado ou toque na tela: 
- Conectar saídas de eventos reativos através da nova API output() e outputFromObservable(): 
origem: angular/livros/03-amigos-angular/content/39-escuta-reativa-cdk-a11y.md
---

::: ilustracao img/ang-amigos/cap39.svg | Chrome DevTools exibindo a Accessibility Tree com nós ARIA, enquanto o serviço LiveAnnouncer emite notificações faladas sem poluição visual.
:::

Certa noite, Dale Carnegie foi convidado para um jantar refinado em Nova York, onde se viu sentado à mesa ao lado de um renomado botânico. Carnegie não entendia absolutamente nada de botânica. No entanto, sentou-se na cadeira e passou a prestar uma atenção quase hipnótica ao cientista, ouvindo com os olhos brilhando enquanto o homem discorria sobre plantas exóticas trazidas do Amazonas, orquídeas raras cultivadas em estufas e experimentos com batatas e cactos.

Durante horas a fio, Carnegie quase não abriu a boca, exceto para fazer perguntas de genuína curiosidade: *"Como o senhor conseguiu salvar aquela flor da geada?"*, *"O que mais o senhor descobriu no solo tropical?"*.

À meia-noite, Carnegie despediu-se e foi embora. Momentos depois, o botânico aproximou-se do anfitrião da festa e fez o elogio mais surpreendente da noite. Disse que Dale Carnegie era *"um homem brilhante"* e, acima de tudo, *"o conversador mais fascinante e inteligente que ele já tivera o privilégio de conhecer"*.

Carnegie ri ao recordar o episódio:

> *"Um conversador fascinante? Eu quase não havia falado nada! O que eu fiz foi ouvir atentamente. Ouvi porque estava sinceramente interessado. Poucos seres humanos resistem à lisonja implícita de uma atenção absorta. Se você quer que as pessoas fujam de você, nunca as escute: fale sem parar sobre si mesmo e interrompa os outros no meio de uma frase."*

---

## O Software que Não Sabe Escutar

Quantas aplicações e componentes web você já usou que sofrem de **surdez digital crônica**?

- Você navega com a tecla `Tab` e, de repente, o foco desaparece em um limbo invisível;
- Você abre um modal pelo teclado, mas ao pressionar `Tab` novamente, o foco pula para o rodapé oculto atrás do painel escurecido;
- Você clica em "Salvar", mas para quem utiliza um leitor de tela (como NVDA ou VoiceOver), não acontece absolutamente nada: nenhum som, nenhuma mensagem, apenas um silêncio angustiante.

Esse tipo de componente é o tagarela egocêntrico que Carnegie descreve. Ele fala pelos cotovelos no visual da tela, mas é completamente incapaz de **ouvir** o usuário que navega por teclado ou que depende de tecnologias assistivas.

Para transformar sua biblioteca em um "conversador fascinante", precisamos ensiná-la a ouvir com perfeição. No Angular, nosso maior aliado é o pacote oficial **`@angular/cdk/a11y`** (Accessibility).

---

## O Poder do Angular CDK A11y

O módulo de acessibilidade do CDK fornece ferramentas de nível cirúrgico para escuta ativa:

1. **`LiveAnnouncer`:** Um serviço que transmite mensagens sonoras instantâneas para leitores de tela através de regiões `aria-live`, sem poluir a interface visual com textos extras.
2. **`FocusMonitor`:** Um serviço que monitora qualquer elemento HTML e identifica **como** o usuário interagiu com ele: se foi via teclado (`keyboard`), mouse (`mouse`) ou toque (`touch`), permitindo aplicar estilos de foco apenas quando necessário.
3. **`FocusTrap`:** Uma armadilha benéfica que mantém o foco do teclado confinado dentro de um modal até que ele seja fechado, evitando que o usuário se perca nos bastidores do DOM.

Vejamos como compor um componente de notificação empático (`AmgNotificationComponent`):

```typescript
// amg-notification.component.ts
import { 
  Component, 
  ChangeDetectionStrategy, 
  inject, 
  input, 
  output, 
  OnInit 
} from '@angular/core';
import { LiveAnnouncer } from '@angular/cdk/a11y';

export type NotificationPoliteness = 'polite' | 'assertive';

@Component({
  selector: 'amg-notification',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <aside 
      class="amg-notification" 
      [class.amg-assertive]="politeness() === 'assertive'"
      role="status">
      <div class="amg-notification-icon" aria-hidden="true">
        {{ politeness() === 'assertive' ? '⚠️' : 'ℹ️' }}
      </div>
      
      <div class="amg-notification-body">
        <ng-content />
      </div>

      <button 
        type="button" 
        class="amg-notification-dismiss" 
        aria-label="Dispensar notificação"
        (click)="dispensar()">
        ✕
      </button>
    </aside>
  `,
  styles: [`
    :host { display: block; margin: 8px 0; }
    .amg-notification {
      display: flex;
      align-items: center;
      gap: 12px;
      padding: 12px 16px;
      border-radius: 8px;
      background: #eff6ff;
      border: 1px solid #bfdbfe;
      color: #1e3a8a;
      font-size: 0.875rem;
    }
    .amg-assertive {
      background: #fef2f2;
      border-color: #fecaca;
      color: #991b1b;
    }
    .amg-notification-body { flex: 1; }
    .amg-notification-dismiss {
      background: none;
      border: none;
      cursor: pointer;
      color: inherit;
      opacity: 0.7;
    }
    .amg-notification-dismiss:hover { opacity: 1; }
  `]
})
export class AmgNotificationComponent implements OnInit {
  private readonly liveAnnouncer = inject(LiveAnnouncer);

  /** Mensagem em texto simples a ser anunciada por voz ao leitor de tela */
  readonly speechText = input.required<string>();

  /**
   * Nível de cortesia do anúncio:
   * 'polite': aguarda o leitor terminar de falar a frase atual (recomendado).
   * 'assertive': interrompe imediatamente qualquer fala em andamento.
   */
  readonly politeness = input<NotificationPoliteness>('polite');

  /** Evento emitido quando o usuário descarta o aviso */
  readonly dismissed = output<void>();

  ngOnInit(): void {
    // Escuta ativa: anuncia para quem não pode enxergar a tela
    this.liveAnnouncer.announce(this.speechText(), this.politeness(), 5000);
  }

  protected dispensar(): void {
    this.dismissed.emit();
  }
}
```

---

## Monitorando o Foco com `FocusMonitor`

Vejamos como transformar um botão em um ouvinte atencioso que sabe se o usuário está navegando pelo teclado ou pelo mouse:

```typescript
// amg-keyboard-listener.directive.ts
import { Directive, ElementRef, inject, OnInit, OnDestroy } from '@angular/core';
import { FocusMonitor, FocusOrigin } from '@angular/cdk/a11y';

@Directive({
  selector: '[amgSmartFocus]',
  standalone: true
})
export class AmgSmartFocusDirective implements OnInit, OnDestroy {
  private readonly elementRef = inject(ElementRef);
  private readonly focusMonitor = inject(FocusMonitor);

  ngOnInit(): void {
    this.focusMonitor
      .monitor(this.elementRef.nativeElement)
      .subscribe((origin: FocusOrigin) => {
        if (origin === 'keyboard') {
          // O usuário está usando a tecla Tab! Aplicamos anel de foco bem contrastado
          this.elementRef.nativeElement.classList.add('amg-focus-visible');
        } else {
          // Clique de mouse ou toque: removemos o anel para não poluir o visual
          this.elementRef.nativeElement.classList.remove('amg-focus-visible');
        }
      });
  }

  ngOnDestroy(): void {
    this.focusMonitor.stopMonitoring(this.elementRef.nativeElement);
  }
}
```

Quando você escuta com esse nível de delicadeza, sua aplicação não é apenas bonita: ela é humana, inclusiva e acolhedora. Como o botânico de Dale Carnegie, o usuário que navega com teclado ou tecnologia assistiva se sentirá respeitado e considerará o seu sistema o mais fascinante do mundo.

---

::: quiz
? Por que o botânico considerou Dale Carnegie um "conversador fascinante", mesmo que Carnegie mal tenha falado durante a noite?
+ Porque Carnegie ouviu com reverência, atenção genuína e perguntas inteligentes, satisfazendo a necessidade do homem de ser ouvido.
- Porque Carnegie contratou um grupo musical para homenagear o botânico no restaurante.
- Porque Carnegie ofereceu uma bolsa de estudos para os netos do botânico.
- Porque Carnegie era um cientista premiado com diversas patentes botânicas.
> Carnegie demonstrou que a atenção absorta e o interesse sincero são o maior elogio que se pode prestar a um ser humano.

? Para que serve o serviço `LiveAnnouncer` do pacote `@angular/cdk/a11y`?
+ Para transmitir anúncios de texto e atualizações de status em voz alta para tecnologias assistivas (leitores de tela) via ARIA.
- Para reproduzir músicas em MP3 durante o carregamento de páginas pesadas.
- Para gravar a voz do desenvolvedor e transcrevê-la em código TypeScript.
- Para emitir alertas sonoros nos alto-falantes de servidores de banco de dados.
> `LiveAnnouncer` gerencia regiões `aria-live` de forma limpa, garantindo que usuários com deficiência visual sejam notificados sobre mudanças de estado.

? Qual é a diferença fundamental entre a cortesia `'polite'` e `'assertive'` no anúncio do `LiveAnnouncer`?
+ `'polite'` espera o sintetizador terminar a fala atual antes de anunciar; `'assertive'` interrompe a fala imediatamente.
- `'polite'` é utilizado apenas na versão gratuita do Angular; `'assertive'` requer licença corporativa paga.
- `'polite'` traduz a mensagem para o francês; `'assertive'` envia um SMS para o celular do usuário.
- `'polite'` aumenta o volume do som em 20 decibéis.
> O modo `'polite'` deve ser o padrão para a grande maioria das notificações, evitando interrupções bruscas e irritantes.

? Qual problema de UX o serviço `FocusMonitor` do CDK resolve com maestria?
+ Diferencia se o foco no elemento foi provocado por tecla (Tab), clique de mouse ou toque, evitando anéis de foco indesejados no mouse.
- Impede que usuários fechem a janela do navegador acidentalmente.
- Monitora se a câmera do usuário está focada na tela do monitor.
- Ajusta o brilho da tela de acordo com a iluminação do quarto do programador.
> O `FocusMonitor` garante que anéis de foco de alto contraste só apareçam para quem realmente está navegando via teclado (`origin === 'keyboard'`).

? Por que componentes que não gerenciam o foco em modais e diálogos são considerados "surdos digitais"?
+ Porque deixam o usuário de teclado perdido em elementos ocultos atrás do modal, tornando a navegação confusa ou impossível.
- Porque queimam a placa de áudio do computador do cliente.
- Porque impedem que a aplicação seja indexada pelos mecanismos de busca do Google.
- Porque geram arquivos de log com mais de 500 gigabytes.
> Sem o gerenciamento adequado de foco (Focus Trap), o usuário fica preso em armadilhas de navegação e perde o contexto da tela.
:::

---

::: cartas
O Botânico de Carnegie :: Relato histórico onde a escuta atenta e sincera transformou Carnegie no "conversador mais fascinante da noite".
Surdez Digital :: Termo metafórico para interfaces que ignoram os eventos de teclado, leitores de tela e acessibilidade.
Angular CDK A11y :: Conjunto oficial de utilitários do Angular para construção de interfaces totalmente acessíveis e universais.
LiveAnnouncer :: Serviço que injeta mensagens dinâmicas para leitores de tela sem impactar a interface visual.
FocusMonitor :: Utilitário que detecta a origem exata do foco (teclado, mouse, toque) em qualquer elemento do DOM.
aria-live polite :: Padrão de acessibilidade que enfileira notificações respeitando a fala atual do leitor de tela.
:::
