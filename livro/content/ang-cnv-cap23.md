---
id: ang-cnv-cap23
order: 342
kind: capitulo
num: 240
group: Angular · Parte 60 · Bloco II — Os Quatro Pilares da Interface
title: Identificando e Expressando Sentimentos
subtitle: Controle de Fluxo Nativo (@if, @for, @switch) e Deferrable Views (@defer)
minutes: 30
hue: 32
badge: 🎭
image: img/ang-cnv/cap23.svg
- Analisar a citação do psicanalista Rollo May sobre o vocabulário das emoções humanas: 
- Distinguir sentimentos genuínos de avaliações mentais disfarçadas na CNV ("sinto-me ignorado"): 
- Dominar o novo fluxo de controle nativo do Angular: @if, @for com tracking obrigatório e @switch
- Implementar Deferrable Views (@defer) com triggers (viewport, interaction, idle), placeholders e loading: 
origem: angular/livros/02-cnv-angular/content/23-expressando-sentimentos.md
---

::: ilustracao img/ang-cnv/cap23.svg | Matriz de alta fidelidade dos 4 estados de UI (Idle, Loading, Success, Error): cada estado expressa honestamente a condição interna da aplicação sem deixar o usuário no vácuo.
:::

No quarto capítulo de *Comunicação Não-Violenta*, Marshall Rosenberg nos confronta com uma constatação surpreendente sobre a sociedade contemporânea: **somos analfabetos emocionais**.

Marshall cita o célebre psicanalista norte-americano Rollo May:
> *"A pessoa madura se torna capaz de diferenciar sentimentos em muitas nuances: algumas experiências são fortes e apaixonadas, ao passo que outras são delicadas e sensíveis, tal qual os diferentes trechos de uma sinfonia. Entretanto, para muitos de nós, os sentimentos são limitados como as notas de um toque de clarim."*

Marshall relata que passou 21 anos em escolas americanas e ninguém jamais lhe perguntou como ele estava se sentindo. A cultura ocidental tradicional nos condicionou a focar exclusivamente em *"o que os outros esperam que eu pense"* ou *"qual é a resposta correta de acordo com a autoridade"*.

O resultado é que nosso vocabulário para rotular os outros ("você é arrogante", "ele é incompetente") é imenso, enquanto nosso vocabulário para expressar o que realmente sentimos é paupérrimo.

### A Grande Armadilha: Pensamentos Disfarçados de Sentimentos

Marshall adverte que muitas expressões que começam com a palavra *"sinto"* **não são sentimentos de forma alguma**, mas julgamentos sobre o comportamento alheio:

```
+---------------------------------------------------------------------------------------------------------+
|                               FALSOS SENTIMENTOS VS SENTIMENTOS REAIS                                  |
+----------------------------------------------------+----------------------------------------------------+
| PALAVRA USADA (AVALIAÇÃO SOBRE O OUTRO)            | O SENTIMENTO REAL SUBJACENTE (VULNERABILIDADE)     |
+----------------------------------------------------+----------------------------------------------------+
| "Sinto-me ignorado por você no projeto."           | Sinto-me solitário, inseguro e triste.             |
| "Sinto-me traído pela decisão da equipe."          | Sinto-me magoado, desamparado e temeroso.          |
| "Sinto-me manipulado pelo meu gestor."             | Sinto-me impotente, frustrado e exausto.           |
| "Sinto-me desvalorizado pelo cliente."             | Sinto-me desanimado e necessito de reconhecimento. |
| "Sinto que você não se importa comigo."            | (Isso é uma dedução puramente mental, não emoção). |
+----------------------------------------------------+----------------------------------------------------+
```

Quando dizemos *"sinto-me ignorado"*, não estamos revelando nossa vulnerabilidade interior; estamos acusando a outra pessoa de ser negligente. Ela ouvirá uma acusação e se defenderá. Mas quando dizemos *"sinto-me triste e desamparado"*, nos conectamos como seres humanos.

---

## O Limbo Emocional das Interfaces Web

Uma interface web mal projetada reproduz essa mesma patologia:
- O usuário clica em "Gerar Relatório de Diálogo".
- A tela fica imóvel. Nenhum cursor muda.
- O botão continua com a aparência de clicável.
- Nenhum spinner surge.
- Dez segundos depois, a página simplesmente pisca ou desarticula.

A aplicação se recusou a expressar honestamente seu estado. Ela deixou o usuário no **limbo da ansiedade**. O usuário não sabe se o clique foi registrado, se o servidor caiu, se seu computador travou ou se ele deve clicar dez vezes mais.

---

## O Novo Fluxo de Controle do Angular 17+ (angular.dev)

Para dar voz e expressão clara aos estados da aplicação, o Angular 17 introduziu o **Built-in Control Flow**, substituindo as antigas diretivas `*ngIf`, `*ngFor` e `*ngSwitch` por uma sintaxe nativa de compilador:

### 1. Condicionais Claras com @if, @else if e @else
```html
@if (estado().status === 'carregando') {
  <app-indicador-progresso [mensagem]="estado().mensagem" />
} @else if (estado().status === 'erro') {
  <app-acolhimento-erro [motivo]="estado().motivo" />
} @else {
  <app-conteudo-sereno [dados]="estado().dados" />
}
```

### 2. Loops Determinísticos com @for e Cláusula @empty
No Angular moderno, **a cláusula `track` é estritamente obrigatória**. Por quê? Porque sem tracking de identidade única, o framework é forçado a destruir e recriar nós do DOM a cada pequena alteração de array. O tracking assegura que apenas os itens novos sejam criados e os modificados sejam reposicionados:

```html
<ul class="lista-conversas">
  @for (conversa of conversas(); track conversa.id; let idx = $index, let total = $count, let primeira = $first) {
    <li class="item-conversa" [class.destaque]="primeira">
      <span class="indice">#{{ idx + 1 }} de {{ total }}</span>
      <strong>{{ conversa.titulo }}</strong>
      <span class="badge-status">{{ conversa.status }}</span>
    </li>
  } @empty {
    <!-- Tratamento respeitoso e acolhedor do estado de lista vazia -->
    <li class="lista-vazia">
      <span class="icone-paz">🕊️</span>
      <p>Nenhuma pendência de diálogo registrada no momento. Tudo flui em paz.</p>
    </li>
  }
</ul>
```

### 3. Matriz Finita com @switch e @case
```html
@switch (estado().status) {
  @case ('idle') { <app-estado-repouso /> }
  @case ('carregando') { <app-estado-progresso /> }
  @case ('sucesso') { <app-estado-realizacao [dados]="estado().dados" /> }
  @case ('erro') { <app-estado-vulnerabilidade [falha]="estado().motivo" /> }
  @default { <app-estado-indeterminado /> }
}
```

---

## Deferrable Views (@defer): Carregamento Progressivo Empático

Uma das maiores revoluções de performance e empatia cognitiva do Angular 17+ é a introdução das **Deferrable Views (`@defer`)**.

Imagine uma área complexa de gráficos estatísticos ou um chat com histórico pesado. Carregar todo esse código JavaScript no carregamento inicial da página força o usuário (especialmente em conexões móveis lentas) a baixar megabytes desnecessários.

Com `@defer`, o Angular adia o download do bundle do componente até que haja uma **necessidade humana real**:

```html
<!-- Carrega o chat de conciliação apenas quando o usuário rolar até ele -->
@defer (on viewport; prefetch on idle) {
  <app-sala-chat-pesada [idSala]="idSala()" />
} @placeholder (minimum 300ms) {
  <!-- Esqueleto visual suave enquanto o componente não entrou no campo de visão -->
  <div class="skeleton-chat">
    <div class="skeleton-avatar"></div>
    <div class="skeleton-linha"></div>
  </div>
} @loading (after 100ms; minimum 500ms) {
  <!-- Exibido apenas se o download do bundle demorar mais de 100ms -->
  <div class="loading-empatico">
    <span class="spinner-suave"></span>
    <p>Preparando o espaço seguro de diálogo...</p>
  </div>
} @error {
  <!-- Acolhimento se a conexão cair durante o download do chunk -->
  <div class="erro-chunk">
    <p>Não foi possível carregar o módulo de diálogo devido a uma oscilação na rede.</p>
    <button type="button" (click)="tentarNovamente()">Recarregar Espaço</button>
  </div>
}
```

### O Catálogo de Gatilhos (Triggers) do @defer (angular.dev)

O `@defer` oferece uma gama impressionante de gatilhos declarativos:
- `on idle`: carrega quando a thread principal do navegador estiver livre.
- `on viewport` ou `on viewport(elementoRef)`: carrega quando o elemento entra na área visível da tela (via `IntersectionObserver` nativo).
- `on interaction` ou `on interaction(botaoRef)`: carrega quando o usuário clica ou pressiona uma tecla no elemento acionador.
- `on hover` ou `on hover(cardRef)`: carrega quando o cursor do mouse paira sobre a área.
- `on immediate`: carrega imediatamente após o término do script do cliente.
- `on timer(500ms)`: carrega após um atraso temporal estipulado.
- `when condicaoReativa()`: carrega quando uma expressão lógica retornar verdadeira.
- `prefetch on ...`: baixa o arquivo JavaScript em segundo plano antes mesmo que o gatilho de renderização seja acionado!

---

## Modelando a Matriz de Sentimentos com TypeScript

Combinando Signals, Uniões Discriminadas e o novo fluxo de controle, temos uma arquitetura emocionalmente madura:

```typescript
// src/app/features/acolhimento/painel-sentimento.component.ts
import { Component, signal } from '@angular/core';
import { CommonModule } from '@angular/common';

export type EstadoConexao<T> =
  | { status: 'idle' }
  | { status: 'carregando'; mensagemProgresso: string }
  | { status: 'sucesso'; dados: T; dataConfirmacao: Date }
  | { status: 'erro'; motivoAcolhido: string; tentativa: number; recuperavel: boolean };

@Component({
  selector: 'app-painel-sentimento',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="caixa-sentimento">
      @switch (estado().status) {
        @case ('idle') {
          <div class="estado-box idle">
            <span class="humor-icone">🌱</span>
            <h4>Pronto para Escutar</h4>
            <p>Estamos aguardando o relato das partes em conflito.</p>
            <button type="button" (click)="iniciar()">Abrir Diálogo</button>
          </div>
        }

        @case ('carregando') {
          @let atual = $any(estado());
          <div class="estado-box loading" aria-busy="true">
            <div class="spinner"></div>
            <h4>{{ atual.mensagemProgresso }}</h4>
            <small>Respeitando o tempo de reflexão do sistema...</small>
          </div>
        }

        @case ('sucesso') {
          @let ok = $any(estado());
          <div class="estado-box sucesso">
            <span class="humor-icone">✨</span>
            <h4>Acordo Estabelecido!</h4>
            <p>{{ ok.dados }}</p>
            <small>Confirmado às {{ ok.dataConfirmacao | date:'HH:mm' }}</small>
          </div>
        }

        @case ('erro') {
          @let err = $any(estado());
          <div class="estado-box erro" role="alert">
            <span class="humor-icone">🌧️</span>
            <h4>Sentimos muito pela dificuldade</h4>
            <p>{{ err.motivoAcolhido }}</p>
            @if (err.recuperavel) {
              <button type="button" (click)="iniciar()">Tentar Novamente (Tentativa {{ err.tentativa }})</button>
            }
          </div>
        }
      }
    </div>
  `
})
export class PainelSentimentoComponent {
  readonly estado = signal<EstadoConexao<string>>({ status: 'idle' });
  private contador = 0;

  iniciar(): void {
    this.contador++;
    this.estado.set({
      status: 'carregando',
      mensagemProgresso: 'Conectando ao serviço de mediação compassiva...'
    });

    setTimeout(() => {
      if (this.contador % 2 === 1) {
        this.estado.set({
          status: 'sucesso',
          dados: 'Ambas as partes expressaram concordância com as diretrizes.',
          dataConfirmacao: new Date()
        });
      } else {
        this.estado.set({
          status: 'erro',
          motivoAcolhido: 'A conexão com a sala virtual oscilou durante o aperto de mãos.',
          tentativa: this.contador,
          recuperavel: true
        });
      }
    }, 1000);
  }
}
```

::: dica
Ao utilizar `@loading (after 100ms; minimum 500ms)`, o Angular previne o terrível "piscar de spinners" (*spinner flickering*): se a rede for ultrarrápida (menos de 100ms), o spinner sequer aparece; se aparecer, permanece visível por no mínimo 500ms para que o cérebro humano possa processar visualmente a transição com calma.
:::

---

::: quiz
? Qual é a diferença fundamental entre uma expressão como "sinto-me desrespeitado" e um sentimento autêntico na CNV?
- "Sinto-me desrespeitado" é uma palavra de origem grega
+ "Sinto-me desrespeitado" é uma avaliação/interpretação sobre a atitude da outra pessoa, enquanto sentimentos genuínos (tristeza, medo, frustração) expressam o estado interior do sujeito
- Não existe nenhuma diferença; ambos são sinônimos no dicionário
- Sentimentos só podem ser ditos em reuniões de família
> Marshall Rosenberg destaca que palavras como "desrespeitado", "traído" ou "ignorado" apontam o dedo para o outro. Sentimentos autênticos revelam nossa própria vulnerabilidade humana.

? Por que o bloco `@for` do Angular 17+ exige obrigatoriamente a declaração da cláusula `track`?
- Para contar o número de cliques do mouse
+ Para associar a identidade única de cada item do array a nós específicos do DOM, permitindo reaproveitamento eficiente e evitando destruições desnecessárias de elementos
- Porque o TypeScript proíbe loops for sem a palavra track
- Para enviar telemetria para o servidor do Google
> A cláusula `track` é um divisor de águas de performance: ao fornecer um identificador estável (`track item.id`), o Angular recicla componentes do DOM com velocidade cirúrgica.

? O que a configuração `@loading (after 100ms; minimum 500ms)` realiza em uma Deferrable View?
- Faz o computador travar por 500 milissegundos
+ Evita o piscar de tela (*flickering*): só exibe o loading se a espera passar de 100ms e, se exibido, mantém-no por ao menos 500ms para suavizar a transição cognitiva do usuário
- Desativa todos os botões da página por 1 minuto
- Aumenta a velocidade do processador do celular
> O controle de tempo do `@loading` demonstra profunda empatia com a percepção visual do usuário, evitando telas que piscam freneticamente em conexões rápidas.

? Qual é o benefício do gatilho `prefetch on idle` em uma diretiva `@defer`?
- Desligar a internet quando o usuário parar de mexer no mouse
+ Baixar o arquivo JavaScript do módulo em segundo plano durante os momentos em que o navegador estiver ocioso, deixando o componente pronto para renderização instantânea quando necessário
- Deletar o cache do navegador automaticamente
- Converter o código para uma linguagem mais rápida
> O `prefetch` antecipa a busca dos recursos sem competir com as tarefas críticas de renderização do usuário, combinando o melhor da performance com carregamento sob demanda.

? Como o bloco `@empty` melhora a legibilidade do template em comparação com o Angular clássico?
- Ele apaga todas as variáveis que estiverem vazias
+ Ele fornece uma sintaxe nativa e declarativa para exibir conteúdo de fallback caso a coleção do `@for` esteja vazia, sem exigir condicionais `*ngIf` aninhadas
- Ele desativa as animações CSS
- Ele fecha o modal dialog automaticamente
> O `@empty` resolve de forma limpa o estado de coleção vazia, tornando a expressão de listas no template direta, elegante e autoexplicativa.
:::

---

::: cartas
Expressão de Sentimentos :: Segundo passo da CNV: comunicar com honestidade e vulnerabilidade as emoções reais vivenciadas interiormente.
Falsos Sentimentos :: Avaliações e diagnósticos mentais disfarçados de emoção (ex: "sinto-me ignorado", "sinto-me traído").
Rollo May :: Psicanalista citado por Marshall Rosenberg sobre a riqueza de nuances do vocabulário emocional maduro.
Built-in Control Flow :: Nova linguagem de controle nativa de templates do Angular: `@if`, `@for` e `@switch`.
Cláusula track no @for :: Diretiva obrigatória de rastreamento de identidade que assegura a reciclagem ótima de nós do DOM.
Deferrable Views (@defer) :: Mecanismo declarativo do Angular 17+ para adiar o carregamento e a renderização de componentes até que um gatilho ocorra.
Prefetch no @defer :: Estratégia de baixar o bundle de código em segundo plano antes do disparo visual do gatilho de renderização.
Spinner Flickering :: Fenômeno desagradável em que indicadores de carregamento piscam na tela por microssegundos, gerando fadiga visual.
:::
