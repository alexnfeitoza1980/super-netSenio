---
id: ang-amigos-cap47
order: 368
kind: capitulo
num: 264
group: Angular · Parte 67 · Conquistando a Equipe sem Conflitos de Estado
title: A Válvula de Segurança das Queixas
subtitle: CDK Virtual Scroll, Drag & Drop Fluido e Performance Estável a 60 FPS
minutes: 30
hue: 215
badge: 🤝
image: img/ang-amigos/cap47.svg
- Analisar a história de Joseph Sherill e o desabafo do cliente de tecidos no escritório de Nova York: 
- Compreender como interfaces lentas e travadas geram frustração, agressividade e queixas dos usuários: 
- Implementar o componente cdk-virtual-scroll-viewport do Angular CDK Scrolling para renderizar 10.000 nós em 60 FPS: 
- Integrar reordenação fluida com o módulo @angular/cdk/drag-drop e a função moveItemInArray: 
- Monitorar consumo de memória Heap e contagem de nós no DOM através do Chrome DevTools Profiler: 
origem: angular/livros/03-amigos-angular/content/47-cdk-virtual-scroll-drag-drop.md
---

::: ilustracao img/ang-amigos/cap47.svg | Interface de gerenciador de dados exibindo 10.000 itens em rolagem suave a 60 FPS com CDK Virtual Scroll e reordenação via Drag & Drop.
:::

Certo dia, um dos maiores compradores de tecidos dos Estados Unidos invadiu enfurecido o escritório do fabricante têxtil **Joseph S. Sherill**, em Nova York. O cliente estava lívido de cólera: a fábrica supostamente havia enviado um lote de tecidos com cores desbotadas e costuras defeituosas, descumprindo os prazos de entrega e gerando prejuízos vultosos.

O comprador esmurrou a mesa e gritou durante quase quinze minutos sem parar. O que fez Joseph Sherill? Ficou na defensiva? Interrompeu o homem para justificar a integridade do departamento de produção?

Sherill não disse uma única palavra de oposição. Sentou-se e ouviu com calma olímpica e genuína consideração, permitindo que o cliente falasse até esgotar todo o veneno e toda a raiva que acumulava no peito.

Quando o homem finalmente parou de gritar por falta de fôlego, Sherill olhou-o nos olhos com tranquilidade e disse:

> *"Agradeço muito por ter vindo pessoalmente até Nova York para me relatar o que aconteceu. Se a sua empresa foi prejudicada, o senhor tem todo o direito de estar indignado. Se eu estivesse no seu lugar, provavelmente estaria sentindo a mesma revolta. O que o senhor sugere que façamos para corrigir essa situação com justiça?"*

O cliente, que esperava uma discussão acalorada, desarmou-se por completo. Disse com voz mansa: *"Bem... o que o senhor acha de nós inspecionarmos o lote juntos para ver o que pode ser reaproveitado?"*.

Ao final da reunião amigável, o próprio cliente admitiu que alguns de seus funcionários haviam exagerado nas queixas. O lote foi aceito com um pequeno ajuste e, antes de sair do escritório, o comprador assinou um novo pedido no valor de dezenas de milhares de dólares.

Dale Carnegie formula a moral da história:

> *"A maioria das pessoas que tenta convencer os outros fala demais. Deixe a outra pessoa falar e desabafar a maior parte do tempo. Ela conhece seus próprios negócios e problemas muito melhor do que você. Ouça pacientemente e com mente aberta. Deixe-a esgotar sua queixa antes de propor soluções."*

---

## O Travamento que Faz o Usuário Explodir

Qual é a maior queixa que um usuário pode ter contra uma aplicação frontend moderna?

É a **lentidão**. É a sensação angustiante de tentar rolar uma lista com milhares de pedidos, contatos ou transações e ver o navegador engasgar, a ventoinha do computador disparar e a tela congelar por quatro segundos com o cursor em forma de ampulheta girando.

Quando uma interface trava, o usuário se sente exatamente como aquele comprador de tecidos furioso. Ele quer desabafar, quer culpar a equipe de engenharia e quer abandonar o software imediatamente.

Por que isso acontece no navegador? Acontece porque o DOM (*Document Object Model*) é uma estrutura pesada. Se você renderizar dez mil tags `<div>` em uma lista comum com `@for`, o navegador precisará calcular o layout geométrico, as sombras e os vetores de cada um desses dez mil elementos simultaneamente, estourando a memória e derrubando a taxa de quadros (*frames per second*) para miseráveis 8 FPS.

---

## A Válvula de Escape: `@angular/cdk/scrolling`

A resposta da engenharia do Angular para esse problema é o **Virtual Scroll** (Rolagem Virtual) do pacote oficial **`@angular/cdk/scrolling`**.

Em vez de criar dez mil nós no DOM, o CDK Virtual Scroll cria **apenas os 8 ou 10 nós que cabem fisicamente na área visível da tela**, mais um pequeno buffer de segurança acima e abaixo. Conforme o usuário rola a barra de rolagem, o CDK reutiliza as mesmas tags HTML, trocando apenas o conteúdo textual em nanossegundos.

O resultado é mágico:
- 10.000 registros ocupando menos de 4 MB de memória Heap;
- Taxa constante de **60.0 FPS** cravados;
- O usuário pode rolar a lista tão rápido quanto o dedo permitir, sem um único soluço visual.

Vejamos como codificar essa maravilha combinando Virtual Scroll com **Drag & Drop** reativo:

```typescript
// amg-virtual-kanban.component.ts
import { 
  Component, 
  ChangeDetectionStrategy, 
  signal 
} from '@angular/core';
import { 
  ScrollingModule, 
  CdkVirtualScrollViewport 
} from '@angular/cdk/scrolling';
import { 
  DragDropModule, 
  CdkDragDrop, 
  moveItemInArray 
} from '@angular/cdk/drag-drop';

export interface CardConexao {
  readonly id: number;
  readonly nome: string;
  readonly cargo: string;
  readonly status: 'ativo' | 'reconhecido';
}

@Component({
  selector: 'amg-virtual-kanban',
  standalone: true,
  imports: [ScrollingModule, DragDropModule],
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <div class="amg-scroll-container">
      <header class="scroll-header">
        <h3>Quadro de Conexões de Alta Performance</h3>
        <span class="badge-fps">60 FPS · {{ itens().length }} Registros</span>
      </header>

      <!-- Viewport Virtual com Altura Fixa de Cada Linha (itemSize="56px") -->
      <cdk-virtual-scroll-viewport 
        itemSize="56" 
        minBufferPx="200" 
        maxBufferPx="400" 
        class="amg-viewport">
        
        <div 
          cdkDropList 
          (cdkDropListDropped)="aoSoltarItem($event)"
          class="amg-drop-list">
          
          <div 
            *cdkVirtualFor="let item of itens(); trackBy: trackById"
            cdkDrag
            class="amg-item-row">
            
            <div class="drag-handle" cdkDragHandle aria-label="Arrastar item">⠿</div>
            
            <div class="item-info">
              <strong>#{{ item.id }} · {{ item.nome }}</strong>
              <small>{{ item.cargo }}</small>
            </div>

            <span class="item-status" [class.reconhecido]="item.status === 'reconhecido'">
              {{ item.status === 'reconhecido' ? '★ Elogiado' : 'Pendente' }}
            </span>

            <!-- Elemento placeholder exibido durante o arraste -->
            <div *cdkDragPlaceholder class="amg-drag-placeholder"></div>
          </div>

        </div>
      </cdk-virtual-scroll-viewport>
    </div>
  `,
  styles: [`
    .amg-scroll-container {
      background: #ffffff;
      border: 1px solid #cbd5e1;
      border-radius: 12px;
      overflow: hidden;
      max-width: 640px;
      margin: 20px auto;
    }
    .scroll-header {
      padding: 16px 20px;
      background: #0f172a;
      color: #fff;
      display: flex;
      justify-content: space-between;
      align-items: center;
    }
    .scroll-header h3 { margin: 0; font-size: 15px; }
    .badge-fps {
      background: #10b981;
      color: #064e3b;
      font-size: 11px;
      font-weight: 700;
      padding: 2px 8px;
      border-radius: 9999px;
    }
    .amg-viewport {
      height: 380px;
      width: 100%;
    }
    .amg-drop-list { width: 100%; }
    .amg-item-row {
      height: 56px;
      box-sizing: border-box;
      padding: 8px 16px;
      display: flex;
      align-items: center;
      gap: 12px;
      border-bottom: 1px solid #f1f5f9;
      background: #ffffff;
    }
    .drag-handle { cursor: grab; color: #94a3b8; font-size: 18px; user-select: none; }
    .item-info { flex: 1; display: flex; flex-direction: column; }
    .item-info strong { font-size: 13px; color: #1e293b; }
    .item-info small { font-size: 11px; color: #64748b; }
    .item-status { font-size: 11px; padding: 2px 8px; border-radius: 4px; background: #f1f5f9; color: #475569; }
    .item-status.reconhecido { background: #dbeafe; color: #1d4ed8; font-weight: 600; }
    .amg-drag-placeholder { background: #f8fafc; border: 2px dashed #93c5fd; min-height: 56px; }
    .cdk-drag-preview {
      box-sizing: border-box;
      border-radius: 6px;
      box-shadow: 0 10px 15px -3px rgba(0,0,0,0.15);
      background: #ffffff;
      padding: 12px 16px;
      display: flex;
      align-items: center;
      gap: 12px;
    }
  `]
})
export class AmgVirtualKanbanComponent {
  /** Gera 10.000 itens de exemplo para comprovar performance */
  readonly itens = signal<CardConexao[]>(
    Array.from({ length: 10000 }, (_, i) => ({
      id: i + 1,
      nome: `Colaborador ${i + 1}`,
      cargo: i % 2 === 0 ? 'Engenheiro Frontend' : 'Arquiteto de Software',
      status: i % 3 === 0 ? 'reconhecido' : 'ativo'
    }))
  );

  protected trackById(_index: number, item: CardConexao): number {
    return item.id;
  }

  /** Manipulador de reordenação com a utilidade oficial do CDK */
  protected aoSoltarItem(event: CdkDragDrop<CardConexao[]>): void {
    const listaModificavel = [...this.itens()];
    moveItemInArray(listaModificavel, event.previousIndex, event.currentIndex);
    this.itens.set(listaModificavel);
  }
}
```

---

## O Fim das Queixas de Performance

Observe a magnitude do que conquistamos:
1. **Zero atrasos no scroll:** O usuário pode dar um tranco no mouse e navegar do item 1 ao item 10.000 em um segundo; o navegador não trava nem oscila.
2. **Reordenação fluida via Drag & Drop:** A função `moveItemInArray` do CDK recalcula os índices sem recriar toda a árvore do DOM.
3. **Escuta Ativa do Dispositivo:** A memória do cliente fica protegida e o processador opera em temperatura baixa.

Como Joseph Sherill ensinou, ao acolher o usuário com uma interface que nunca engasga nem discute, você abre uma válvula de alívio para qualquer tensão. O usuário sente-se respeitado, no comando de suas tarefas e plenamente satisfeito.

---

::: quiz
? Qual foi a atitude de Joseph Sherill perante a revolta do grande comprador de tecidos em seu escritório?
+ Ouviu com atenção e consideração absoluta durante quinze minutos sem interromper, permitindo que o cliente esgotasse toda a sua queixa.
- Chamou a segurança do prédio para expulsar o cliente à força.
- Provou matematicamente que o cliente estava mentindo sobre a qualidade do lote.
- Cobrou uma multa por difamação comercial.
> Ao permitir que o outro desabafe completamente, a raiva se dissipa e abre espaço para a cooperação e o bom senso.

? Por que renderizar 10.000 itens diretamente no DOM com um loop tradicional causa travamento do navegador?
+ Porque cada elemento HTML consome nós na árvore do DOM, forçando o navegador a recalcular geometria, sombras e layouts para milhares de nós invisíveis.
- Porque o protocolo HTTP não aceita números maiores que cinco mil.
- Porque o cabo de força do computador sobrecarrega com muitas letras.
- Porque o TypeScript só permite listas com até 256 posições.
> O custo computacional de manter milhares de elementos físicos no DOM derruba a taxa de quadros e esgota a memória RAM.

? Como o componente `cdk-virtual-scroll-viewport` resolve o gargalo de grandes listas?
+ Renderiza no DOM exclusivamente a quantidade de nós que cabem na área visível da tela (mais um buffer mínimo), reciclando-os na rolagem.
- Deleta os itens do banco de dados para a lista ficar menor.
- Exige que o usuário compre uma placa de vídeo dedicada mais potente.
- Oculta a barra de rolagem para que ninguém role até o final.
> A virtualização cria a ilusão perfeita de uma lista infinita, mantendo a contagem física de nós no DOM extremamente reduzida.

? Qual é o propósito da propriedade `itemSize` no `cdk-virtual-scroll-viewport`?
+ Informar ao algoritmo a altura física exata (em pixels) de cada linha, permitindo calcular o tamanho total da barra de rolagem instantaneamente.
- Definir o tamanho do arquivo da biblioteca no disco rígido.
- Limitar a quantidade de caracteres que cabem em cada palavra.
- Ajustar a resolução de cor das imagens para 4K.
> O `itemSize` constante viabiliza o cálculo matemático imediato da posição do scroll sem necessidade de medições caras no DOM.

? Para que serve a função utilitária `moveItemInArray()` do pacote `@angular/cdk/drag-drop`?
+ Para reordenar os elementos de um array TypeScript de acordo com os índices de origem e destino após uma ação de arrastar e soltar.
- Para transferir arquivos de um computador para outro via Bluetooth.
- Para excluir elementos duplicados de um banco de dados relacional.
- Para ordenar strings em ordem alfabética automaticamente.
> A função `moveItemInArray` atualiza o estado da lista em memória de forma imutável e ultra-rápida.
:::

---

::: cartas
Joseph Sherill :: Fabricante de tecidos que recuperou um cliente enfurecido apenas ouvindo com serenidade até o final do desabafo.
Válvula de Segurança :: Metáfora de Carnegie: permita que a outra pessoa fale e expresse todas as suas dores antes de propor acordos.
Virtual Scrolling :: Padrão de renderização que mantém no DOM apenas os nós visíveis na janela de visualização do usuário.
itemSize :: Parâmetro de dimensão fixa (em pixels) que alimenta o motor de rolagem virtual de alta precisão.
moveItemInArray() :: Função do CDK Drag and Drop que reordena posições de elementos de forma otimizada.
Taxa de 60 FPS :: Padrão ouro de fluidez visual em interfaces interativas (16,6 milissegundos por quadro de renderização).
:::
