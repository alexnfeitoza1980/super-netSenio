---
id: ang-cnv-cap27
order: 346
kind: capitulo
num: 244
group: Angular · Parte 61 · Bloco III — Empatia na Prática
title: O Poder da Empatia
subtitle: Acessibilidade Real (a11y) com Angular CDK: LiveAnnouncer, FocusTrap e FocusMonitor
minutes: 30
hue: 32
badge: ♿
image: img/ang-cnv/cap27.svg
- Analisar a citação do psicólogo humanista Carl Rogers sobre o poder curativo da escuta sem julgamento: 
- Compreender o relato comovente da estudante Milly e da diretora Sra. Anderson: 
- Dominar o módulo @angular/cdk/a11y do Angular CDK em profundidade: 
- Implementar diálogos totalmente acessíveis com cdkTrapFocus, LiveAnnouncer e FocusMonitor: 
origem: angular/livros/02-cnv-angular/content/27-poder-da-empatia.md
---

::: ilustracao img/ang-cnv/cap27.svg | Visualizador de Acessibilidade em Alta Resolução: Diálogo modal com barreira de proteção de foco (cdkTrapFocus) e balão de áudio do LiveAnnouncer vocalizando notificações com aria-live="polite".
:::

No oitavo capítulo de *Comunicação Não-Violenta*, Marshall Rosenberg nos conduz pelo impacto curativo e restaurador da empatia humana. Marshall resgata uma das mais belas passagens de seu mentor, o lendário psicólogo humanista **Carl Rogers**:

> *"Quando alguém realmente o escuta sem julgá-lo, sem tentar assumir a responsabilidade por você, sem tentar moldá-lo, é extraordinariamente bom. [...] Quando sinto que fui ouvido e escutado, consigo perceber meu mundo de uma maneira nova e ir em frente.*  
>  
> *É espantoso como problemas que parecem insolúveis se tornam solúveis quando alguém escuta. Como confusões que parecem irremediáveis viram riachos relativamente claros correndo, quando se é escutado."*

### A História de Milly e da Diretora Sra. Anderson

Marshall ilustra a empatia que cura através de uma história tocante que aconteceu em uma escola pública norte-americana.

A diretora da escola, Sra. Anderson, voltou de seu intervalo de almoço e encontrou uma aluna do ensino fundamental chamada **Milly** sentada em seu gabinete. Milly estava arrasada, com lágrimas descendo pelo rosto e os ombros encolhidos.

A diretora se sentou ao lado de Milly, em silêncio. A menina começou a falar em soluços:
> — *Sra. Anderson, a senhora já teve uma semana inteira em que tudo que você faz magoa alguém, mas você nunca quis magoar ninguém de forma alguma?*

Se a diretora reagisse com a mentalidade tradicional de julgamento ou consolo superficial, ela diria: *"Ora, Milly, você é uma boa menina, não fique assim!"* ou *"O que foi que você aprontou desta vez?"*.

A Sra. Anderson, no entanto, colocou as orelhas de girafa e acolheu a dor da aluna:
> — *Sim, Milly. Acho que compreendo perfeitamente o que você está sentindo. É uma sensação horrível querer acertar e parecer que tudo dá errado.*

Milly desabafou sua semana inteira: o atraso na entrega do trabalho, a amiga que se ofendeu com uma brincadeira inocente, o tropeço no corredor. A diretora não a aconselhou, não a corrigiu e não a mandou de volta para a sala. Ela apenas escutou, validando cada sentimento. Ao ser ouvida em sua vulnerabilidade, Milly respirou fundo, enxugou as lágrimas e encontrou por si mesma a clareza e a coragem para pedir desculpas à amiga e recomeçar sua semana em paz.

---

## Acessibilidade Web (a11y) como Expressão Ética de Empatia

Quando transportamos a sabedoria de Carl Rogers e da Sra. Anderson para a engenharia de software com Angular em [angular.dev/overview](https://angular.dev/overview), chegamos a uma conclusão incontornável:

> **A Acessibilidade Web (a11y) é a maior prova de empatia e respeito que uma equipe de tecnologia pode dedicar a outros seres humanos.**

Construir uma aplicação inacessível é dizer a uma pessoa com deficiência visual, motora ou neurodivergente: *"O seu sofrimento não me importa; eu só projeto para pessoas perfeitas iguais a mim"*. Isso é a quintessência da violência estrutural e excludente.

---

## O Arsenal de Empatia do Angular CDK A11y

A equipe oficial do Google no Angular desenvolveu o pacote `@angular/cdk/a11y` (Component Development Kit - Accessibility). Ele contém primitivas de nível de sistema operacional projetadas para derrubar todas as barreiras digitais:

```
+---------------------------------------------------------------------------------------------------------+
|                                ARSENAL DO PACOTE @angular/cdk/a11y                                      |
+--------------------------+------------------------------------------------------------------------------+
| FERRAMENTA               | PROPÓSITO DE EMPATIA NA INTERFACE                                            |
+--------------------------+------------------------------------------------------------------------------+
| LiveAnnouncer            | Comunica alterações visuais dinâmicas aos leitores de tela via aria-live.   |
| cdkTrapFocus             | Aprisiona o foco da tecla TAB dentro de modais, impedindo desorientação.    |
| cdkTrapFocusAutoCapture  | Move o foco inicial automaticamente para o primeiro elemento útil da janela.|
| FocusMonitor             | Rastreia COMO o elemento foi focado (teclado vs mouse) para :focus-visible. |
| HighContrastModeDetector | Detecta se o sistema operacional está em modo de alto contraste para visão.  |
| InteractivityChecker     | Avalia se um elemento HTML pode ser focado, clicado ou percebido.           |
+--------------------------+------------------------------------------------------------------------------+
```

---

## 1. Dando Voz com o LiveAnnouncer (Leitores de Tela)

Quando um dado muda no template de forma dinâmica (por exemplo, um novo acordo de diálogo é gravado ou um erro de rede é acolhido), pessoas cegas que utilizam softwares leitores de tela (como **NVDA**, **VoiceOver** ou **JAWS**) não percebem a mudança porque o foco do teclado não mudou de posição.

O `LiveAnnouncer` injeta uma notificação audível no canal de voz do sistema operacional:

```typescript
import { Component, inject, signal } from '@angular/core';
import { LiveAnnouncer } from '@angular/cdk/a11y';

@Component({
  selector: 'app-contador-escuta',
  standalone: true,
  template: `
    <div class="bloco-contador">
      <p>Total de diálogos pacificadores acolhidos: <strong>{{ total() }}</strong></p>
      <button type="button" (click)="aoRegistrarNovo()">Registrar Novo Diálogo</button>
    </div>
  `
})
export class ContadorEscutaComponent {
  private readonly announcer = inject(LiveAnnouncer);
  readonly total = signal(24);

  aoRegistrarNovo(): void {
    this.total.update(n => n + 1);

    // Anúncio compassivo vocalizado pelo leitor de tela
    this.announcer.announce(
      `Novo diálogo acolhido com sucesso. Total atualizado para ${this.total()} processos.`,
      'polite' // Modo cortês: aguarda a fala em andamento terminar sem interromper bruscamente
    );
  }
}
```

::: nota
Diferença entre `'polite'` e `'assertive'`:
- `'polite'`: espera o leitor de tela terminar de ler o parágrafo ou link atual. É cortês, paciente e não interrompe a fala.
- `'assertive'`: interrompe imediatamente qualquer áudio em andamento. Deve ser reservado exclusivamente para emergências graves de segurança do usuário.
:::

---

## 2. Diálogo Modal com cdkTrapFocus e Semântica ARIA

Quando um diálogo modal se abre, o usuário de teclado navega pressionando a tecla `Tab`. Sem a proteção de aprisionamento de foco (*Focus Trap*), o cursor continua navegando pelos links ocultos atrás do modal, gerando profunda desorientação espacial.

Vejamos a implementação de um Diálogo de Conciliação acessível:

```typescript
// src/app/shared/ui/dialogo-conciliacao/dialogo-conciliacao.component.ts
import { Component, inject, output, ElementRef, viewChild } from '@angular/core';
import { A11yModule, LiveAnnouncer } from '@angular/cdk/a11y';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-dialogo-conciliacao',
  standalone: true,
  imports: [A11yModule, CommonModule],
  template: `
    <!-- Fundo protetor de desfoque -->
    <div class="backdrop" (click)="fechar()"></div>

    <!-- Contêiner do Diálogo com Atributos ARIA e Diretiva cdkTrapFocus -->
    <div
      class="dialogo-modal"
      role="dialog"
      aria-modal="true"
      aria-labelledby="titulo-dialogo"
      aria-describedby="descricao-dialogo"
      cdkTrapFocus
      cdkTrapFocusAutoCapture="true"
      (keydown.escape)="fechar()"
    >
      <header class="modal-topo">
        <h3 id="titulo-dialogo">Espaço Protegido de Conciliação</h3>
      </header>

      <main class="modal-corpo">
        <p id="descricao-dialogo">
          Você está prestes a enviar uma proposta de paz para sua equipe.
          Esta mensagem foi redigida sob os preceitos da Comunicação Não-Violenta,
          sem acusações ou exigências moralizantes.
        </p>
      </main>

      <footer class="modal-acoes">
        <!-- Primeiro botão focado automaticamente pelo AutoCapture -->
        <button
          type="button"
          class="btn-secundario"
          (click)="fechar()"
        >
          Revisar Rascunho [Esc]
        </button>

        <button
          type="button"
          class="btn-primario"
          (click)="aoConfirmar()"
        >
          Confirmar e Enviar [Enter]
        </button>
      </footer>
    </div>
  `,
  styles: [`
    .backdrop { position: fixed; inset: 0; background: rgba(15, 23, 42, 0.6); backdrop-filter: blur(3px); z-index: 1000; }
    .dialogo-modal {
      position: fixed; top: 50%; left: 50%; transform: translate(-50%, -50%);
      background: #ffffff; border: 1px solid #f5dfd0; border-radius: 14px;
      padding: 2rem; max-width: 520px; width: 90%; z-index: 1001;
      box-shadow: 0 20px 40px rgba(42, 21, 8, 0.25);
    }
    .modal-topo h3 { margin: 0 0 1rem 0; color: #2a1508; font-size: 1.3rem; }
    .modal-corpo p { color: #475569; line-height: 1.5; font-size: 0.95rem; }
    .modal-acoes { display: flex; justify-content: flex-end; gap: 1rem; margin-top: 1.5rem; }
    .btn-secundario { background: #fdf4eb; border: 1px solid #f5dfd0; color: #6c3c1e; padding: 0.6rem 1.2rem; border-radius: 6px; font-weight: 600; cursor: pointer; }
    .btn-primario { background: #d96b27; color: white; border: none; padding: 0.6rem 1.4rem; border-radius: 6px; font-weight: bold; cursor: pointer; }
    
    /* Anel de foco nítido para quem navega via teclado */
    button:focus-visible {
      outline: 3px solid #f59e0b;
      outline-offset: 3px;
    }
  `]
})
export class DialogoConciliacaoComponent {
  private readonly announcer = inject(LiveAnnouncer);

  readonly encerrado = output<boolean>();

  fechar(): void {
    this.announcer.announce('Diálogo encerrado sem alterações. Foco retornado à página principal.', 'polite');
    this.encerrado.emit(false);
  }

  aoConfirmar(): void {
    this.announcer.announce('Proposta de conciliação enviada com sucesso.', 'assertive');
    this.encerrado.emit(true);
  }
}
```

::: dica
Observe a propriedade `cdkTrapFocusAutoCapture="true"`. Ela captura imediatamente o foco do cursor e o posiciona no primeiro botão interativo assim que o modal se materializa na tela, poupando usuários com deficiência visual de terem que navegar por dezenas de elementos da página até alcançar o diálogo.
:::

---

::: quiz
? Qual foi a conclusão do psicólogo Carl Rogers sobre o poder da escuta empática?
- Que a escuta deve ser cobrada por minuto
+ Que quando uma pessoa se sente verdadeiramente ouvida sem ser julgada, problemas que pareciam insolúveis tornam-se solúveis e ela é capaz de perceber seu mundo de forma nova e seguir em frente
- Que ninguém tem tempo para escutar os outros em empresas
- Que as pessoas que escutam demais se tornam fracas
> Rogers demonstrou cientificamente o poder curativo da empatia: o alívio profundo de ser acolhido exatamente como se é desarma defesas neurais e liberta a criatividade humana para a resolução de conflitos.

? Como a diretora Sra. Anderson acolheu a aluna Milly quando ela entrou chorando em seu gabinete?
- Aplicou uma suspensão de dois dias para Milly aprender a não chorar
+ Ouviu com presença total e empatia profunda, validando a dor de quem tentou acertar a semana toda mas sentiu que tudo saiu errado, sem dar sermões
- Mandou Milly lavar o rosto e voltar para a sala de aula imediatamente
- Chamou os pais de Milly para puni-la
> A atitude da Sra. Anderson é o exemplo perfeito de empatia curativa: em vez de consolar superficialmente ou julgar, ela ofereceu a presença segura que permitiu a Milly se restabelecer emocionalmente.

? Para que serve a diretiva `cdkTrapFocus` do módulo `@angular/cdk/a11y`?
- Para fechar o navegador se o usuário demorar a responder
+ Para aprisionar a navegação por tabulação (tecla TAB) dentro de um elemento contêiner (como um modal), impedindo que o cursor se perca em elementos inacessíveis no fundo da página
- Para aumentar a velocidade das requisições HTTP
- Para criptografar o banco de dados do sistema
> O Focus Trap assegura que a atenção do teclado permaneça estritamente contida na tarefa ativa do diálogo até que a pessoa decida conscientemente fechar ou confirmar a ação.

? O que o parâmetro `'polite'` no método `LiveAnnouncer.announce()` assegura?
- Que a mensagem será lida apenas com voz feminina
+ Que o leitor de tela aguardará uma pausa natural na fala atual antes de vocalizar a nova notificação, sem atropelar a fala do usuário
- Que o anúncio só será falado se o usuário for educado
- Que a notificação será impressa em papel
> Regiões `aria-live="polite"` demonstram respeito pelo fluxo cognitivo de quem utiliza tecnologia assistiva, vocalizando novidades com elegância e paciência.

? Por que o pseudo-seletor CSS `:focus-visible` é considerado mais empático do que o `:focus` genérico?
- Porque ele deixa as fontes em negrito
+ Porque ele exibe o indicador destacado de foco apenas para quem está interagindo via teclado (onde o foco é indispensável), sem poluir visualmente usuários que clicam com o mouse
- Porque ele desativa todas as bordas dos botões
- Porque ele impede que pessoas com deficiência usem a aplicação
> O `:focus-visible` é o padrão moderno de ergonomia: ele acolhe com anéis de foco nítidos quem depende do teclado, mantendo uma estética limpa para cliques de mouse.
:::

---

::: cartas
Poder da Empatia :: Capacidade restauradora de acolher o ser humano em sua vulnerabilidade, devolvendo-lhe a dignidade e a autonomia.
Carl Rogers :: Psicólogo pioneiro da abordagem centrada na pessoa que descreveu o alívio curativo de ser escutado sem julgamento.
História de Milly :: Caso verídico narrado por Marshall Rosenberg onde a escuta atenta da diretora ajudou uma aluna a superar a dor de uma semana difícil.
Acessibilidade Web (a11y) :: Compromisso ético de projetar sistemas digitais plenamente utilizáveis por todas as pessoas, incluindo pessoas com deficiência.
Angular CDK A11y :: Pacote oficial de utilitários de acessibilidade: LiveAnnouncer, cdkTrapFocus, FocusMonitor e detecção de alto contraste.
LiveAnnouncer :: Serviço do Angular CDK que vocaliza mensagens dinâmicas para leitores de tela através de regiões acessíveis ARIA.
cdkTrapFocus :: Diretiva que restringe a navegação de tabulação do teclado dentro do diálogo ativo.
focus-visible :: Seletor CSS que destaca visualmente o elemento ativo exclusivamente durante a navegação por teclado.
:::
