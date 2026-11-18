---
id: ang-amigos-cap35
order: 356
kind: capitulo
num: 252
group: Angular · Parte 65 · Técnicas Fundamentais para Tratar com Desenvolvedores
title: Aquele que Conseguir Isto Terá Todo o Mundo a Seu Lado
subtitle: Consumer-Driven API Design e Composição com Diretivas
minutes: 30
hue: 215
badge: 🤝
image: img/ang-amigos/cap35.svg
- Compreender a metáfora da pescaria no Maine (morangos vs. minhocas) e a história do bezerro de Ralph Waldo Emerson: 
- Aplicar o princípio de focar nas necessidades do consumidor ao arquitetar componentes em Angular 19: 
- Comparar o antipadrão do "Componente Monolítico" com a arquitetura elegante de Composição com Diretivas: 
- Utilizar o recurso hostDirectives do Angular para acoplar comportamentos reusáveis sem herança rígida: 
- Projetar tabelas e listas orientadas a templates customizáveis em vez de listas de 30 propriedades booleanas: 
origem: angular/livros/03-amigos-angular/content/35-consumer-driven-design.md
---

::: ilustracao img/ang-amigos/cap35.svg | Arquitetura comparando um componente de tabela monolítico inchado com 30 inputs rígidos versus uma solução flexível baseada em composição com diretivas no Angular.
:::

Todos os verões, Dale Carnegie costumava ir pescar trutas nos lagos do estado do Maine, no extremo nordeste dos Estados Unidos. Em suas próprias palavras:

> *"Pessoalmente, sou um apaixonado por morangos com chantilly; mas descobri que, por alguma estranha razão, os peixes preferem minhocas. Por isso, quando vou pescar, não penso sobre o que eu mais gosto. Penso sobre o que os peixes desejam. Não coloco morangos com chantilly no anzol; coloco uma minhoca ou um gafanhoto e pergunto ao peixe: 'Você não gostaria de saborear isto?'"*

Por que será que nos negócios, na convivência humana — e especialmente no desenvolvimento de software —, costumamos agir exatamente ao contrário? Construímos componentes pensando exclusivamente no que é conveniente para **nós** que estamos escrevendo, e depois reclamamos que os outros desenvolvedores da equipe se recusam a utilizá-los!

Carnegie ilustra essa armadilha com outra passagem cômica e real sobre o filósofo **Ralph Waldo Emerson** e seu filho. Certa tarde, pai e filho tentavam desesperadamente colocar um jovem bezerro para dentro da cocheira. Emerson puxava o animal pela corda com toda a força intelectual de seus ensaios; o filho empurrava as ancas do bezerro com vigor juvenil. Mas o animal empacava os cascos na grama e se recusava terminantemente a avançar. 

Uma empregada irlandesa da casa assistiu à cena patética. Ela não sabia escrever tratados sobre a filosofia transcendental, mas entendia a psicologia dos bezerros: caminhou até o animal, colocou maternalmente o próprio dedo indicador na boca dele, deixou que o bezerro chupasse seu dedo e conduziu-o com extrema doçura para dentro do celeiro sem fazer força alguma.

---

## O Bezerro Teimoso da Tabela Angular

Quantas vezes você já viu (ou escreveu) um componente como este:

```html
<!-- O Componente "Empurrando o Bezerro" -->
<app-mega-tabela 
  [dados]="usuarios()"
  [temPaginacao]="true"
  [itensPorPagina]="10"
  [corCabecalho]="'azul'"
  [mostrarExportarExcel]="false"
  [permitirFiltroData]="true"
  [iconeAcoes]="'engrenagem'"
  [textoBotaoNovo]="'Adicionar'"
  [permitirOrdenacao]="true"
  [alinhamentoTexto]="'esquerda'"
  [esconderColunaId]="true"
  [habilitarCheckbox]="true"
  [alturaLinha]="48"
  [mensagemVazio]="'Nenhum dado'">
</app-mega-tabela>
```

O autor dessa tabela pensou no que **ele** achava que o mundo precisava: criou uma propriedade para cada capricho visual imaginável. Mas basta um colega precisar colocar uma foto de avatar ao lado do nome do usuário, ou aplicar uma cor condicional de acordo com o status de ativação, e a arquitetura desmorona. O colega é obrigado a abrir um Pull Request pedindo o 31º `@Input() permitirAvatarNaColuna2`.

Isso é querer empurrar o bezerro à força ou colocar morangos com chantilly no anzol das trutas.

---

## Consumer-Driven Design: A Composição Elegante

O que o desenvolvedor consumidor realmente deseja? Ele deseja **soberania sobre a renderização** e **automação sobre a lógica pesada**. Ele quer minhocas suculentas: controle visual total e zero repetição de código.

No Angular moderno, atingimos essa perfeição combinando **Diretivas Estruturais**, **Template Outlets** e o recurso nativo **`hostDirectives`**.

Vejamos como projetar uma arquitetura de tabela amigável (`AmgTableComponent`):

```typescript
// amg-table.directives.ts
import { Directive, TemplateRef, inject, input } from '@angular/core';

/**
 * Diretiva aplicada aos cabeçalhos da coluna.
 */
@Directive({
  selector: '[amgHeader]',
  standalone: true
})
export class AmgHeaderDirective {
  readonly templateRef = inject(TemplateRef);
}

/**
 * Diretiva aplicada às células de dados.
 * Fornece contexto estritamente tipado da linha.
 */
@Directive({
  selector: '[amgCell]',
  standalone: true
})
export class AmgCellDirective<T> {
  readonly templateRef = inject(TemplateRef);

  /** Guarda tipagem de contexto para o compilador do Angular */
  static ngTemplateContextGuard<TContext>(
    dir: AmgCellDirective<TContext>,
    ctx: unknown
  ): ctx is { $implicit: TContext; index: number } {
    return true;
  }
}

/**
 * Diretiva que define uma coluna e suas diretivas filhas.
 */
@Directive({
  selector: 'amg-column',
  standalone: true
})
export class AmgColumnDirective<T> {
  /** Nome identificador único da coluna */
  readonly name = input.required<string>();
}
```

Agora, o componente hospedeiro que reúne essas partes sem ditar regras visuais rígidas:

```typescript
// amg-table.component.ts
import { 
  Component, 
  ChangeDetectionStrategy, 
  input, 
  contentChildren, 
  TemplateRef 
} from '@angular/core';
import { NgTemplateOutlet } from '@angular/common';
import { AmgHeaderDirective, AmgCellDirective } from './amg-table.directives';

@Component({
  selector: 'amg-table',
  standalone: true,
  imports: [NgTemplateOutlet],
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <div class="amg-table-wrapper">
      <table class="amg-table">
        <thead>
          <tr>
            @for (header of headerTemplates(); track $index) {
              <th>
                <ng-container [ngTemplateOutlet]="header.templateRef" />
              </th>
            }
          </tr>
        </thead>
        <tbody>
          @for (item of items(); track $index; let idx = $index) {
            <tr>
              @for (cell of cellTemplates(); track $index) {
                <td>
                  <ng-container 
                    [ngTemplateOutlet]="cell.templateRef" 
                    [ngTemplateOutletContext]="{ $implicit: item, index: idx }" />
                </td>
              }
            </tr>
          } @empty {
            <tr>
              <td [attr.colspan]="headerTemplates().length || 1" class="empty-cell">
                Nenhum registro encontrado.
              </td>
            </tr>
          }
        </tbody>
      </table>
    </div>
  `,
  styles: [`
    .amg-table-wrapper { width: 100%; overflow-x: auto; }
    .amg-table { width: 100%; border-collapse: collapse; text-align: left; }
    th { padding: 12px 16px; background: #f8fafc; border-bottom: 2px solid #e2e8f0; font-weight: 600; color: #475569; }
    td { padding: 12px 16px; border-bottom: 1px solid #e2e8f0; color: #1e293b; }
    tr:hover td { background: #f1f5f9; }
    .empty-cell { text-align: center; color: #94a3b8; padding: 32px; }
  `]
})
export class AmgTableComponent<T> {
  /** Coleção reativa de registros a serem renderizados */
  readonly items = input.required<readonly T[]>();

  /** Captura declarativa de todos os cabeçalhos projetados */
  protected readonly headerTemplates = contentChildren(AmgHeaderDirective);

  /** Captura declarativa de todas as células projetadas */
  protected readonly cellTemplates = contentChildren(AmgCellDirective);
}
```

---

## O Resultado no Template do Consumidor

Veja como fica o template de quem consome a tabela. O desenvolvedor tem total liberdade:

```html
<amg-table [items]="colaboradores()">
  <!-- Coluna 1: Colaborador com Avatar e Cargo -->
  <ng-template amgHeader>Colaborador</ng-template>
  <ng-template amgCell let-usuario>
    <div style="display: flex; align-items: center; gap: 8px;">
      <img [src]="usuario.avatar" style="width: 32px; height: 32px; border-radius: 50%;" />
      <div>
        <strong>{{ usuario.nome }}</strong>
        <div style="font-size: 11px; color: #64748b;">{{ usuario.cargo }}</div>
      </div>
    </div>
  </ng-template>

  <!-- Coluna 2: Status com Badge Condicional -->
  <ng-template amgHeader>Status</ng-template>
  <ng-template amgCell let-usuario>
    <span [class]="usuario.ativo ? 'badge-verde' : 'badge-cinza'">
      {{ usuario.ativo ? 'Disponível' : 'Ausente' }}
    </span>
  </ng-template>
</amg-table>
```

Você deu ao consumidor exatamente o que ele queria: controle absoluto sobre os elementos HTML, tipagem inferida no `let-usuario` e zero dependência de novos inputs na tabela. O bezerro entra na cocheira com a máxima tranquilidade e alegria.

---

::: quiz
? O que Dale Carnegie quis demonstrar com a história da pescaria no Maine (morangos vs. minhocas)?
+ Que para influenciar e conquistar alguém, devemos focar no que a outra pessoa valoriza, e não nas nossas próprias preferências egoístas.
- Que peixes preferem açúcar refinado a insetos naturais.
- Que desenvolvedores frontend devem evitar comer frutas durante o expediente.
- Que a pesca esportiva melhora a velocidade de compilação de projetos em Angular.
> O princípio essencial de Carnegie é: coloque no anzol o que o outro quer comer. Em software, entregue a flexibilidade que o consumidor da biblioteca necessita.

? Por que a empregada irlandesa conseguiu colocar o bezerro de Ralph Waldo Emerson na cocheira com facilidade?
+ Porque entendeu o instinto do bezerro e colocou o dedo para ele chupar, conduzindo-o suavemente sem coerção.
- Porque ela ameaçou o bezerro com um processo judicial.
- Porque ela contratou uma consultoria externa especializada em gado.
- Porque ela leu os ensaios poéticos de Emerson em voz alta para o animal.
> Ela compreendeu o desejo natural do animal e trabalhou em harmonia com ele, em vez de tentar forçá-lo contra a sua vontade.

? Qual é o principal problema do antipadrão do "Componente Monolítico" com dezenas de `@Input()`s de configuração?
+ Torna o componente frágil, rígido e incapaz de atender a novos requisitos visuais sem a criação constante de mais propriedades.
- Faz o servidor Node.js desligar automaticamente após 10 minutos.
- Obriga o desenvolvedor a escrever código em Java puro.
- Impede a renderização de cores na tela do usuário.
> Componentes monolíticos tentam prever todas as variações visuais com flags booleanas, gerando um código inchado e impossível de manter.

? Como a Signal Query `contentChildren()` do Angular moderno auxilia na composição de tabelas flexíveis?
+ Detecta declarativamente todas as instâncias de diretivas projetadas dentro da tag hospedeira, retornando um Signal reativo.
- Converte arquivos JSON externos em tabelas SQL relacionais.
- Deleta automaticamente diretivas que não possuem testes unitários.
- Força a execução síncrona de requisições HTTP na thread principal.
> `contentChildren()` cria consultas reativas limpas para referências de template fornecidas pelo consumidor através de Content Projection.

? Para que serve o método estático `ngTemplateContextGuard` em diretivas como `AmgCellDirective`?
+ Informar ao compilador do Angular e ao TypeScript o tipo exato das variáveis contextuais disponibilizadas no `let-item`.
- Bloquear requisições de rede não autorizadas pelo firewall.
- Impedir que usuários sem privilégios de administrador façam login na aplicação.
- Criptografar as classes CSS no momento do build.
> O Type Guard contextual habilita a inferência de tipos estrita dentro do template do consumidor, fornecendo autocompletion e validação semântica.
:::

---

::: cartas
Morangos vs. Minhocas :: Princípio de Dale Carnegie: atraia as pessoas oferecendo o que elas desejam e valorizam, não o que você prefere.
O Bezerro de Emerson :: Metáfora sobre a futilidade de forçar comportamentos através da força bruta em vez da empatia e cooperação.
Consumer-Driven Design :: Filosofia de arquitetura de software que desenha APIs com base na ergonomia e necessidades de quem as consome.
hostDirectives :: Recurso do Angular que permite compor comportamentos e diretivas diretamente na classe hospedeira sem herança.
contentChildren() :: Signal Query que descobre e monitora dinamicamente elementos e diretivas filhos projetados no componente.
ngTemplateContextGuard :: Mecanismo do compilador Angular para garantir tipagem estrita de variáveis em templates estruturais.
:::
