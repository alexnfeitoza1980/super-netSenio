---
id: ang-antimedo-cap16
order: 316
kind: capitulo
num: 221
group: Angular · Parte 55 · Bloco IV — A Invencibilidade
title: Modelagem inspiradora
subtitle: Diretivas — infundindo comportamento nos melhores elementos
minutes: 30
hue: 195
badge: 🦅
- Compreender o princípio da modelagem: absorver padrões de excelência comprovados
- Dominar o conceito de Diretivas de Atributo (@Directive) no Angular: 
- Controlar elementos hospedeiros com @HostBinding e @HostListener: 
- Criar comportamentos reutilizáveis que enriquecem qualquer tag HTML sem acoplamento: 
origem: angular/livros/01-antimedo-angular/content/16-modelagem.md
---

::: ilustracao img/ang-antimedo/cap16.svg | Uma águia imponente transmitindo sua silhueta de asas abertas para uma figura em ascensão.
:::

No capítulo sobre Modelagem Inspiradora do livro *Antimedo*, Pablo Marçal compartilha uma das experiências mais marcantes de sua trajetória:

> *"Quando eu tinha 13 anos, vi um homem sem patrimônio, andando em um carro velho e de bicicleta. Acompanhei e modelei aquele homem. Ouvi mais de 2.000 horas de ensinamentos dele e li mais de 40 livros escritos por ele. Modelei o padrão mental, as reações e a postura dele. Anos depois, eu já havia conquistado milhões em patrimônio aplicando os mesmos princípios."*

Modelar não é invejar. Modelar não é pirataria intelectual.  
**Modelar é decodificar um padrão de comportamento que funciona e aplicá-lo com fidelidade no seu próprio contexto.**

No ecossistema do Angular, você frequentemente se depara com botões comuns, inputs padrão e divs normais. Se você quiser que esses elementos ganhem superpoderes de interação, animação, acessibilidade e destaque, você não cria um componente novo do zero para cada variação.

Você cria uma **Diretiva** (`@Directive`).

A diretiva é o instrumento de **modelagem comportamental** do Angular: ela se conecta a qualquer elemento hospedeiro (*host*) e infunde nele vida, inteligência e comportamento extraordinário.

---

## O que é uma Diretiva de Atributo?

Diferente de um `@Component`, uma diretiva **não possui um template HTML próprio**. Ela "mora" dentro de uma tag HTML que já existe, monitorando seus eventos e alterando seus estilos e atributos:

```typescript
// shared/directives/brilho-antimedo.directive.ts
import { Directive, ElementRef, HostListener, HostBinding, Input, OnInit } from '@angular/core';

@Directive({
  // Seletor entre colchetes indica que ela é aplicada como atributo HTML
  selector: '[appBrilhoAntimedo]',
  standalone: true
})
export class BrilhoAntimedoDirective implements OnInit {
  // Permite customizar a cor do brilho via template: [appBrilhoAntimedo]="'#ff6b35'"
  @Input('appBrilhoAntimedo') corBrilho: string = '#dd1b44';

  // Vincula diretamente com a propriedade CSS box-shadow do elemento hospedeiro
  @HostBinding('style.boxShadow') sombraCss: string = 'none';
  @HostBinding('style.transition') transicaoCss: string = 'all 0.3s cubic-bezier(0.4, 0, 0.2, 1)';
  @HostBinding('style.cursor') cursorCss: string = 'pointer';

  ngOnInit(): void {
    // Configurações iniciais ao acoplar no elemento
  }

  // Escuta quando o mouse entra no elemento (como a mente focando no alvo)
  @HostListener('mouseenter')
  aoEntrarComMouse(): void {
    this.sombraCss = `0 10px 30px ${this.corBrilho}`;
  }

  // Escuta quando o mouse sai do elemento
  @HostListener('mouseleave')
  aoSairComMouse(): void {
    this.sombraCss = 'none';
  }

  // Escuta o clique no elemento hospedeiro
  @HostListener('click', ['$event'])
  aoClicar(evento: MouseEvent): void {
    console.log('[MODELAGEM ATIVADA]: Ação de coragem executada no elemento!', evento);
  }
}
```

---

## Usando a Diretiva no Template: A Modelagem em Ação

Para utilizar a diretiva em um componente standalone, basta adicioná-la ao array `imports` e colocá-la como um atributo qualquer em uma tag:

```typescript
import { Component } from '@angular/core';
import { BrilhoAntimedoDirective } from './brilho-antimedo.directive';

@Component({
  selector: 'app-vitrine-coragem',
  standalone: true,
  imports: [BrilhoAntimedoDirective],
  template: `
    <div class="container">
      <h2>Elementos Modelados com Postura Firme</h2>

      <!-- Um botão comum que agora possui o comportamento de brilho -->
      <button appBrilhoAntimedo="#dd1b44">
        Botão de Decisão Rápida
      </button>

      <!-- Um card que ganha a mesma atitude sem precisar de código duplicado -->
      <div class="card" [appBrilhoAntimedo]="'#2e9e6b'">
        <h3>Card de Vitória</h3>
        <p>Passe o mouse para ver a postura modelada.</p>
      </div>
    </div>
  `
})
export class VitrineCoragemComponent {}
```

Veja o poder disso: com uma única diretiva, você padroniza o comportamento de dezenas de elementos diferentes em toda a sua organização. Se a diretriz mudar amanhã, você ajusta um único arquivo TypeScript e todos os botões e cards do sistema obedecem instantaneamente.

---

## Modelando Entradas: Diretiva de Bloqueio de Ruído

Vamos criar uma diretiva de validação comportamental para campos de entrada (`<input>`). Ela interceptará o teclado do usuário e impedirá a digitação de caracteres de ruído:

```typescript
// shared/directives/bloquear-ruido.directive.ts
import { Directive, HostListener } from '@angular/core';

@Directive({
  selector: '[appApenasNumeros]',
  standalone: true
})
export class ApenasNumerosDirective {
  // Teclas de controle permitidas (Backspace, Tab, Setas)
  private teclasPermitidas = ['Backspace', 'Tab', 'ArrowLeft', 'ArrowRight', 'Delete'];

  @HostListener('keydown', ['$event'])
  aoPressionarTecla(evento: KeyboardEvent): void {
    if (this.teclasPermitidas.includes(evento.key)) {
      return; // Permite a ação natural
    }

    // Se não for número de 0 a 9, bloqueia a digitação no ato
    if (!/^[0-9]$/.test(evento.key)) {
      evento.preventDefault();
      console.warn(`[RUÍDO BARRADO]: Caractere proibido "${evento.key}" bloqueado.`);
    }
  }
}
```

Uso direto no formulário:
```html
<input type="text" appApenasNumeros placeholder="Digite apenas seu alvo numérico..." />
```

---

::: exercicio Exercício: Diretiva de Auto-Foco
Implemente uma diretiva chamada `AutofocoDirective` que use a injeção de `ElementRef` para dar foco automático (`elementRef.nativeElement.focus()`) no elemento assim que ele for renderizado na tela.

--- resposta
```typescript
import { Directive, ElementRef, OnInit, inject } from '@angular/core';

@Directive({
  selector: '[appAutofoco]',
  standalone: true
})
export class AutofocoDirective implements OnInit {
  private el = inject(ElementRef);

  ngOnInit(): void {
    // Dá foco imediatamente após a renderização
    setTimeout(() => {
      this.el.nativeElement.focus();
    }, 50);
  }
}
```
:::

---

::: quiz
? Qual a principal diferença entre um `@Component` e uma `@Directive`?
+ Um componente possui um template visual próprio (HTML), enquanto a diretiva apenas modifica um elemento hospedeiro
- A diretiva não aceita código TypeScript
- O componente não pode ter estilização CSS
- A diretiva só funciona no servidor backend
> Componentes são diretivas com template HTML próprio; diretivas de atributo modulam elementos existentes.

? Para que serve o decorador `@HostListener` dentro de uma diretiva?
+ Para escutar eventos do DOM (como clique, mouseenter, keydown) que ocorrem no elemento onde a diretiva está aplicada
- Para enviar emails automaticamente ao servidor
- Para alterar o título da aba do navegador
- Para reiniciar o computador do usuário
> `@HostListener` registra ouvintes de eventos diretamente no elemento HTML hospedeiro.

? Para que serve o decorador `@HostBinding` dentro de uma diretiva?
+ Para vincular uma propriedade do TypeScript a um atributo, classe ou estilo CSS do elemento hospedeiro
- Para vincular a aplicação a um banco de dados SQL
- Para bloquear o acesso à internet
- Para desinstalar pacotes do npm
> `@HostBinding` sincroniza valores da classe TypeScript com atributos ou estilos do elemento DOM hospedeiro.

? Como se declara o seletor de uma diretiva para que ela seja usada como atributo HTML?
+ Usando colchetes no seletor: `selector: '[appMinhaDiretiva]'`
- Usando ponto no seletor: `selector: '.appMinhaDiretiva'`
- Usando hashtag: `selector: '#appMinhaDiretiva'`
- Usando tags XML: `selector: '<appMinhaDiretiva>'`
> A sintaxe de colchetes `[nome]` define um seletor de atributo CSS/HTML.

? Qual a grande vantagem de usar diretivas para comportamentos de interface?
+ Reutilização massiva de lógica sem necessidade de duplicar HTML ou estender classes complexas
- Eliminação total do uso de JavaScript no navegador
- Garantia de que a aplicação nunca precisará de testes
- Redução obrigatória do tamanho das imagens da página
> Diretivas encapsulam comportamentos puros que podem ser plugados em qualquer tag HTML com facilidade.
:::

---

::: cartas
Modelagem Inspiradora :: Decodificar padrões vencedores e incorporá-los fielmente em novos contextos.
Diretiva de Atributo :: Classe que estende e altera a aparência ou comportamento de um elemento hospedeiro.
@HostListener :: Decorador que intercepta eventos do navegador originados no elemento onde a diretiva reside.
@HostBinding :: Decorador que controla propriedades, classes ou estilos do elemento HTML hospedeiro.
ElementRef :: Invólucro do Angular que concede acesso de baixo nível ao elemento nativo do DOM (`nativeElement`).
Composição por Diretivas :: Estratégia de anexar múltiplas diretivas independentes ao mesmo componente.
:::

::: resumo
Pablo Marçal ensina que para encurtar anos de sofrimento, você deve modelar quem já alcançou o topo da montanha. No Angular, as Diretivas são o ápice da modelagem arquitetural: permitem que você ensine qualquer elemento comum da tela a se comportar com a excelência dos melhores componentes do mundo.
:::
