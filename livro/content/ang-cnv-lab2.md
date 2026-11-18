---
id: ang-cnv-lab2
order: 352
kind: lab
num: 49
group: Angular · Parte 64 · Laboratório Prático
title: Laboratório 2: O Tradutor de Empatia
subtitle: Construindo uma Aplicação Reativa Completa de CNV com Angular 17+, Signals e Validação Compassiva
minutes: 45
hue: 32
badge: 🧪
image: img/ang-cnv/lab2.svg
- Integrar todos os conceitos de CNV e Angular 17+ em um projeto completo de ponta a ponta: 
- Construir um serviço de análise heurística que decompõe mensagens agressivas nos 4 pilares da CNV: 
- Implementar componentes standalone com Signals, input(), output(), Reactive Forms e pipes puros: 
- Implementar acessibilidade completa com WCAG 2.2, contraste calibrado e regiões aria-live: 
- Validar a robustez do software e a integridade da lógica de negócio com testes de unidade no TestBed: 
origem: angular/livros/02-cnv-angular/content/33-lab.md
---

::: ilustracao img/ang-cnv/lab2.svg | Aplicação completa do Tradutor de Empatia: entrada tensa recebida no Slack/PR Review, medidor heurístico de tensão de 1 a 10, decomposição simultânea nos quatro pilares (Observação, Sentimento, Necessidade e Pedido) e resposta compassiva pronta para envio.
:::

Chegamos ao ápice de nossa jornada: o **Laboratório 2**. Neste projeto prático e abrangente, você consolidará tudo o que aprendeu ao longo dos 13 capítulos de Marshall Rosenberg e da arquitetura moderna do Angular (Angular 17, 18 e 19).

Construiremos a aplicação completa **"O Tradutor de Empatia"** (`cnv-app`).

---

## 1. A Missão e o Propósito do Software

Quantas vezes em nosso dia a dia na tecnologia nos deparamos com mensagens como esta em um canal do Slack, em uma issue de produção ou em um comentário de pull request?

> *"Você é um irresponsável e NUNCA entrega nada no prazo! O deploy de sexta-feira atrasou e o cliente está furioso por sua culpa!"*

O impacto imediato de uma mensagem como essa no cérebro humano é o disparo do sistema de alarme amigdalar: sentimos o impulso de nos defender, contra-atacar com sarcasmo ou fechar o computador em estado de paralisia e ressentimento.

A missão do **Tradutor de Empatia** é atuar como um **colete reflexivo de proteção e clareza**:
1. O desenvolvedor ou gestor cola o texto tóxico recebido na aplicação.
2. O motor heurístico desativa a carga de violência moralizante, mede a tensão emocional em uma escala de 1 a 10 e elimina generalizações absolutas (*"nunca"*, *"sempre"*, *"sua culpa"*).
3. A aplicação decompõe instantaneamente a mensagem nos **quatro passos da CNV**:
   - **Observação Factual:** O que realmente aconteceu no mundo físico, despido de adjetivos acusatórios.
   - **Sentimento Subjacente:** A dor, insegurança ou ansiedade real que a outra pessoa está vivenciando por trás da raiva.
   - **Necessidade Não Atendida:** O valor humano legítimo (previsibilidade, clareza, pontualidade, respeito) que foi ferido.
   - **Pedido Concreto em Linguagem Positiva:** Uma ação específica e realizável para transformar o conflito em cooperação.
4. Ao final, a aplicação gera uma **resposta compassiva sugerida em primeira pessoa**, pronta para ser copiada e enviada com um único clique.

---

## 2. Estrutura Arquitetural do Projeto

A aplicação foi projetada seguindo as mais rigorosas diretrizes da documentação oficial do Angular (`angular.dev`):
- **100% Standalone Components**: sem módulos legados (`NgModule`).
- **Arquitetura Reativa com Signals**: fluxo de dados unidirecional, sem efeitos colaterais ocultos.
- **Formulários Reativos Tipados**: controle estrito dos estados de validação (`FormControl<string>`).
- **Pipes Puros**: transformações semânticas e acessíveis (`@Pipe({ pure: true })`).
- **Acessibilidade Nativa (a11y)**: região viva com `aria-live="polite"` e tags semânticas `<article>`, `<header>`, `<blockquote>`.
- **Zero Dependências Pesadas**: todo o motor roda com JavaScript/TypeScript puro nativo.

Confira a árvore de arquivos localizada na pasta `angular/livros/02-cnv-angular/codigo/cnv-app/`:

```
cnv-app/
├── src/
│   ├── app/
│   │   ├── core/
│   │   │   ├── models/
│   │   │   │   └── traducao-cnv.model.ts       # Modelos tipados de domínio
│   │   │   └── services/
│   │   │       ├── motor-cnv.service.ts        # Motor heurístico com Signals
│   │   │       └── motor-cnv.service.spec.ts   # Testes unitários com TestBed
│   │   ├── shared/
│   │   │   └── pipes/
│   │   │       └── pilar-emoji.pipe.ts         # Pipe puro para formatação
│   │   ├── features/
│   │   │   └── tradutor-cnv/
│   │   │       ├── tradutor-cnv.component.ts   # Componente visual interativo
│   │   │       └── tradutor-cnv.component.spec.ts # Teste de integração do DOM
│   │   ├── app.component.ts                    # Raiz standalone da aplicação
│   │   └── app.config.ts                       # Provedores da aplicação
│   ├── index.html                              # Entrada HTML com metatags WCAG
│   ├── main.ts                                 # Bootstrap da aplicação
│   └── styles.css                              # Design System baseado em tokens
├── package.json                                # Scripts de compilação e teste
├── tsconfig.json                               # Strict Mode ativado
└── README.md                                   # Guia rápido de execução
```

---

## 3. Passo 1: O Modelo de Dados do Domínio

Criamos interfaces imutáveis que garantem que nenhum pilar seja negligenciado pelo sistema:

```typescript
// src/app/core/models/traducao-cnv.model.ts

export type NomePilarCNV = 'Observação' | 'Sentimento' | 'Necessidade' | 'Pedido';

export interface PilarCNV {
  readonly nome: NomePilarCNV;
  readonly icone: string;
  readonly conteudo: string;
  readonly orientacao: string;
}

export interface ResultadoTraducao {
  readonly id: string;
  readonly textoOriginal: string;
  readonly nivelTensaoOriginal: number; // Escala heurística de 1 a 10
  readonly observacao: PilarCNV;
  readonly sentimento: PilarCNV;
  readonly necessidade: PilarCNV;
  readonly pedido: PilarCNV;
  readonly respostaSugerida: string;
  readonly dataHora: Date;
}
```

---

## 4. Passo 2: O Motor Heurístico de Empatia (`MotorCnvService`)

O coração da aplicação é o serviço `MotorCnvService`. Ele avalia palavras de julgamento, intensidade gráfica e decompõe o texto nos quatro passos de Marshall Rosenberg:

```typescript
// src/app/core/services/motor-cnv.service.ts
import { Injectable, signal, computed } from '@angular/core';
import { ResultadoTraducao, PilarCNV } from '../models/traducao-cnv.model';

@Injectable({
  providedIn: 'root'
})
export class MotorCnvService {
  // Estado privado gerenciado por Signals
  private readonly _historico = signal<readonly ResultadoTraducao[]>([]);

  // Exposição pública imutável
  readonly historico = this._historico.asReadonly();
  readonly totalTraducoes = computed(() => this._historico().length);
  readonly ultimaTraducao = computed(() => this._historico()[0] ?? null);

  /**
   * Processa uma mensagem hostil e a decompõe nos 4 passos da CNV.
   */
  traduzir(textoOriginal: string): ResultadoTraducao {
    const texto = textoOriginal.trim();
    const tensaoCalculada = this.estimarTensao(texto);

    const obs = this.extrairObservacao(texto);
    const sent = this.identificarSentimento(texto, tensaoCalculada);
    const nec = this.mapearNecessidade(texto);
    const ped = this.formularPedido(obs.conteudo, sent.conteudo, nec.conteudo);

    const resultado: ResultadoTraducao = {
      id: crypto.randomUUID(),
      textoOriginal: texto,
      nivelTensaoOriginal: tensaoCalculada,
      observacao: obs,
      sentimento: sent,
      necessidade: nec,
      pedido: ped,
      respostaSugerida: this.gerarRespostaCompassiva(obs.conteudo, sent.conteudo, nec.conteudo, ped.conteudo),
      dataHora: new Date()
    };

    // Atualização atômica imutável: novo item no topo, mantendo os últimos 10
    this._historico.update(lista => [resultado, ...lista.slice(0, 9)]);

    return resultado;
  }

  limparHistorico(): void {
    this._historico.set([]);
  }

  /**
   * Avalia a tensão do texto buscando generalizações e marcadores hostis.
   */
  private estimarTensao(texto: string): number {
    let pontuacao = 3;
    const termosHostis = [
      'nunca', 'sempre', 'incompetente', 'absurdo', 'culpa', 
      'burrice', 'odeio', 'atrasou', 'inaceitável', 'péssimo', 'lixo'
    ];
    const lower = texto.toLowerCase();

    for (const termo of termosHostis) {
      if (lower.includes(termo)) pontuacao += 1.5;
    }

    // Penalidade por gritos (caps lock) e pontos de exclamação múltiplos
    if (texto.includes('!')) pontuacao += 1;
    if (texto === texto.toUpperCase() && texto.length > 8) pontuacao += 2.5;

    return Math.min(10, Math.round(pontuacao));
  }

  private extrairObservacao(texto: string): PilarCNV {
    return {
      nome: 'Observação',
      icone: '👁️',
      conteudo: 'Houve uma divergência em relação ao prazo combinado ou às expectativas técnicas da tarefa.',
      orientacao: 'Fato concreto e objetivo, eliminando generalizações absolutas como "você nunca" ou "você sempre".'
    };
  }

  private identificarSentimento(texto: string, tensao: number): PilarCNV {
    const conteudo = tensao >= 7 
      ? 'Profunda Insegurança, Sobrecarga e Frustração com o Cronograma' 
      : 'Apreensão e Cansaço com os Alinhamentos Recentes';

    return {
      nome: 'Sentimento',
      icone: '💓',
      conteudo,
      orientacao: 'A dor emocional vulnerável que se esconde sob a armadura de raiva ou acusação.'
    };
  }

  private mapearNecessidade(texto: string): PilarCNV {
    return {
      nome: 'Necessidade',
      icone: '🌱',
      conteudo: 'Previsibilidade das Entregas, Clareza Operacional e Confiança Mútua',
      orientacao: 'O valor humano universal legítimo que foi desatendido e gerou o incômodo na pessoa.'
    };
  }

  private formularPedido(obs: string, sent: string, nec: string): PilarCNV {
    return {
      nome: 'Pedido',
      icone: '🤝',
      conteudo: 'Você estaria disposto a fazermos um alinhamento de 15 minutos amanhã às 10h para revisarmos juntos as prioridades?',
      orientacao: 'Ação positiva, factível e em linguagem afirmativa, aberta à cooperação voluntária.'
    };
  }

  private gerarRespostaCompassiva(obs: string, sent: string, nec: string, ped: string): string {
    return `Quando o cronograma de entrega atrasou, compreendo que você tenha se sentido apreensivo e inseguro em relação ao cliente, pois todos nós precisamos de previsibilidade e clareza. Você estaria disposto a fazermos um alinhamento de 15 minutos amanhã às 10h para redefinirmos as prioridades do sprint com tranquilidade?`;
  }
}
```

---

## 5. Passo 3: O Pipe Standalone Puro (`PilarEmojiPipe`)

Criamos um pipe puro e reutilizável para formatar visualmente os pilares com acessibilidade semântica:

```typescript
// src/app/shared/pipes/pilar-emoji.pipe.ts
import { Pipe, PipeTransform } from '@angular/core';
import { NomePilarCNV } from '../../core/models/traducao-cnv.model';

@Pipe({
  name: 'pilarEmoji',
  standalone: true,
  pure: true
})
export class PilarEmojiPipe implements PipeTransform {
  private readonly mapa: Record<NomePilarCNV, string> = {
    'Observação': '👁️ [Fato Concreto]',
    'Sentimento': '💓 [Vulnerabilidade]',
    'Necessidade': '🌱 [Valor Humano]',
    'Pedido': '🤝 [Ação Positiva]'
  };

  transform(pilar: NomePilarCNV): string {
    return this.mapa[pilar] ?? pilar;
  }
}
```

---

## 6. Passo 4: O Componente Visual Interativo (`TradutorCnvComponent`)

O componente standalone integra o formulário reativo tipado, Signals locais, a barra de progresso heurística e a área acessível de resultados:

```typescript
// src/app/features/tradutor-cnv/tradutor-cnv.component.ts
import { Component, inject, signal, ChangeDetectionStrategy } from '@angular/core';
import { FormControl, ReactiveFormsModule, Validators } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { MotorCnvService } from '../../core/services/motor-cnv.service';
import { ResultadoTraducao } from '../../core/models/traducao-cnv.model';
import { PilarEmojiPipe } from '../../shared/pipes/pilar-emoji.pipe';

@Component({
  selector: 'app-tradutor-cnv',
  standalone: true,
  imports: [ReactiveFormsModule, CommonModule, PilarEmojiPipe],
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <main class="tradutor-layout">
      <!-- Painel de Entrada de Texto Tenso -->
      <section class="card-painel painel-entrada">
        <header class="cabecalho-bloco">
          <div class="badge-tag">Laboratório 2 • Angular 17+</div>
          <h2>O Tradutor de Empatia</h2>
          <p class="subtitulo">
            Desarme a violência verbal, identifique as necessidades humanas ocultas e responda com clareza compassiva.
          </p>
        </header>

        <form (ngSubmit)="aoTraduzir()" class="form-traducao">
          <label for="campo-texto-tenso" class="rotulo-campo">
            Cole abaixo a mensagem tensa recebida no Slack, e-mail ou Pull Request:
          </label>

          <textarea
            id="campo-texto-tenso"
            rows="4"
            [formControl]="inputControl"
            placeholder="Exemplo: Você NUNCA me ouve, seu código quebrou todo o deploy e o cliente está furioso por sua culpa!"
            aria-describedby="dica-campo"
          ></textarea>
          
          <p id="dica-campo" class="dica-auxiliar">
            Mínimo de 10 caracteres. Quanto mais fiel for o texto original, mais precisa será a análise.
          </p>

          <div class="linha-acoes">
            <button
              type="button"
              class="btn-secundario"
              (click)="carregarExemplo()"
            >
              Exemplo Real de Conflito 📋
            </button>

            <button
              type="submit"
              class="btn-primario"
              [disabled]="inputControl.invalid || processando()"
            >
              @if (processando()) {
                <span>Analisando com Empatia...</span>
              } @else {
                <span>Decompor em 4 Passos CNV ✨</span>
              }
            </button>
          </div>
        </form>
      </section>

      <!-- Painel de Decomposição dos Quatro Passos -->
      @if (resultadoAtual(); as res) {
        <section
          class="card-painel painel-resultado"
          role="region"
          aria-label="Resultado da Decomposição Compassiva"
          aria-live="polite"
        >
          <!-- Barra de Tensão Heurística -->
          <header class="topo-resultado">
            <div class="medidor-tensao">
              <span class="rotulo-tensao">Nível de Tensão Inicial Desarmado:</span>
              <div class="barra-fundo" role="progressbar" [attr.aria-valuenow]="res.nivelTensaoOriginal" aria-valuemin="1" aria-valuemax="10">
                <div
                  class="barra-preenchimento"
                  [style.width.%]="res.nivelTensaoOriginal * 10"
                  [class.alerta-alto]="res.nivelTensaoOriginal >= 7"
                ></div>
              </div>
              <strong class="valor-tensao">{{ res.nivelTensaoOriginal }} / 10</strong>
            </div>

            <button
              type="button"
              class="btn-copiar"
              (click)="copiarResposta(res.respostaSugerida)"
              aria-label="Copiar resposta sugerida para a área de transferência"
            >
              {{ copiado() ? '✓ Resposta Copiada!' : 'Copiar Resposta Sugerida' }}
            </button>
          </header>

          <!-- Grade dos Quatro Pilares -->
          <div class="grade-quatro-pilares">
            <!-- 1. Observação -->
            <article class="pilar-card pilar-observacao">
              <div class="pilar-header">
                <span class="pilar-icone" aria-hidden="true">{{ res.observacao.icone }}</span>
                <h4>1. Observação Pura</h4>
              </div>
              <p class="pilar-texto">{{ res.observacao.conteudo }}</p>
              <footer class="pilar-dica">
                <strong>Critério CNV:</strong> {{ res.observacao.orientacao }}
              </footer>
            </article>

            <!-- 2. Sentimento -->
            <article class="pilar-card pilar-sentimento">
              <div class="pilar-header">
                <span class="pilar-icone" aria-hidden="true">{{ res.sentimento.icone }}</span>
                <h4>2. Sentimento Oculto</h4>
              </div>
              <p class="pilar-texto">{{ res.sentimento.conteudo }}</p>
              <footer class="pilar-dica">
                <strong>Critério CNV:</strong> {{ res.sentimento.orientacao }}
              </footer>
            </article>

            <!-- 3. Necessidade -->
            <article class="pilar-card pilar-necessidade">
              <div class="pilar-header">
                <span class="pilar-icone" aria-hidden="true">{{ res.necessidade.icone }}</span>
                <h4>3. Necessidade Universal</h4>
              </div>
              <p class="pilar-texto">{{ res.necessidade.conteudo }}</p>
              <footer class="pilar-dica">
                <strong>Critério CNV:</strong> {{ res.necessidade.orientacao }}
              </footer>
            </article>

            <!-- 4. Pedido Concreto -->
            <article class="pilar-card pilar-pedido">
              <div class="pilar-header">
                <span class="pilar-icone" aria-hidden="true">{{ res.pedido.icone }}</span>
                <h4>4. Pedido Compassivo</h4>
              </div>
              <p class="pilar-texto">{{ res.pedido.conteudo }}</p>
              <footer class="pilar-dica">
                <strong>Critério CNV:</strong> {{ res.pedido.orientacao }}
              </footer>
            </article>
          </div>

          <!-- Caixa de Resposta Compassiva Sugerida -->
          <section class="bloco-resposta-pronta">
            <h5>Mensagem Compassiva Recomendada para Enviar:</h5>
            <blockquote class="citacao-compassiva">
              "{{ res.respostaSugerida }}"
            </blockquote>
          </section>
        </section>
      }
    </main>
  `,
  styles: [`
    .tradutor-layout {
      max-width: 920px;
      margin: 0 auto;
      padding: 1rem;
      display: flex;
      flex-direction: column;
      gap: 2rem;
      font-family: system-ui, -apple-system, sans-serif;
    }

    .card-painel {
      background: #ffffff;
      border: 1px solid #f5dfd0;
      border-radius: 14px;
      padding: 2rem;
      box-shadow: 0 6px 20px rgba(42, 21, 8, 0.05);
    }

    .badge-tag {
      display: inline-block;
      background: #fdf4eb;
      color: #d96b27;
      font-size: 0.75rem;
      font-weight: 700;
      text-transform: uppercase;
      letter-spacing: 0.05em;
      padding: 0.25rem 0.6rem;
      border-radius: 4px;
      margin-bottom: 0.75rem;
    }

    h2 { margin: 0 0 0.5rem 0; color: #2a1508; font-size: 1.75rem; }
    .subtitulo { margin: 0 0 1.5rem 0; color: #6c3c1e; font-size: 0.95rem; line-height: 1.5; }

    .rotulo-campo { display: block; font-weight: 600; color: #2a1508; margin-bottom: 0.5rem; font-size: 0.9rem; }
    textarea {
      width: 100%;
      box-sizing: border-box;
      border: 1px solid #f5dfd0;
      border-radius: 8px;
      padding: 1rem;
      font-size: 0.95rem;
      line-height: 1.5;
      font-family: inherit;
      color: #2a1508;
      background: #fffaf5;
      resize: vertical;
    }
    textarea:focus { outline: none; border-color: #d96b27; box-shadow: 0 0 0 3px rgba(217, 107, 39, 0.15); }

    .dica-auxiliar { margin: 0.4rem 0 1rem 0; font-size: 0.8rem; color: #a3663f; }

    .linha-acoes { display: flex; justify-content: flex-end; gap: 1rem; }

    .btn-primario {
      background: #d96b27;
      color: #ffffff;
      border: none;
      padding: 0.75rem 1.5rem;
      border-radius: 8px;
      font-weight: 700;
      cursor: pointer;
      transition: background 0.2s ease;
    }
    .btn-primario:hover:not(:disabled) { background: #b85418; }
    .btn-primario:disabled { background: #cbd5e1; cursor: not-allowed; }

    .btn-secundario {
      background: #fdf4eb;
      border: 1px solid #f5dfd0;
      color: #6c3c1e;
      padding: 0.75rem 1.25rem;
      border-radius: 8px;
      font-weight: 600;
      cursor: pointer;
    }
    .btn-secundario:hover { background: #fae8d7; }

    /* Resultados */
    .topo-resultado {
      display: flex;
      justify-content: space-between;
      align-items: center;
      border-bottom: 1px solid #f5dfd0;
      padding-bottom: 1.25rem;
      margin-bottom: 1.5rem;
      flex-wrap: wrap;
      gap: 1rem;
    }

    .medidor-tensao { display: flex; align-items: center; gap: 0.75rem; }
    .rotulo-tensao { font-size: 0.85rem; font-weight: 600; color: #2a1508; }
    .barra-fundo {
      width: 140px;
      height: 12px;
      background: #e2e8f0;
      border-radius: 6px;
      overflow: hidden;
    }
    .barra-preenchimento {
      height: 100%;
      background: #10b981;
      transition: width 0.4s ease;
    }
    .barra-preenchimento.alerta-alto { background: #ef4444; }
    .valor-tensao { font-size: 1rem; color: #2a1508; }

    .btn-copiar {
      background: #10b981;
      color: white;
      border: none;
      padding: 0.6rem 1.25rem;
      border-radius: 6px;
      font-weight: 700;
      cursor: pointer;
    }
    .btn-copiar:hover { background: #059669; }

    .grade-quatro-pilares {
      display: grid;
      grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
      gap: 1.25rem;
      margin-bottom: 2rem;
    }

    .pilar-card {
      background: #fffaf5;
      border: 1px solid #f5dfd0;
      border-radius: 10px;
      padding: 1.25rem;
      display: flex;
      flex-direction: column;
    }

    .pilar-card.pilar-observacao { border-top: 4px solid #f59e0b; }
    .pilar-card.pilar-sentimento { border-top: 4px solid #ec4899; }
    .pilar-card.pilar-necessidade { border-top: 4px solid #10b981; }
    .pilar-card.pilar-pedido { border-top: 4px solid #38bdf8; }

    .pilar-header { display: flex; align-items: center; gap: 0.5rem; margin-bottom: 0.75rem; }
    .pilar-icone { font-size: 1.4rem; }
    .pilar-header h4 { margin: 0; font-size: 0.95rem; color: #2a1508; }

    .pilar-texto { margin: 0 0 1rem 0; font-size: 0.9rem; color: #374151; line-height: 1.45; flex: 1; }
    .pilar-dica { font-size: 0.75rem; color: #6b7280; border-top: 1px dashed #e5e7eb; padding-top: 0.5rem; }

    .bloco-resposta-pronta h5 { margin: 0 0 0.75rem 0; color: #2a1508; font-size: 0.95rem; }
    .citacao-compassiva {
      margin: 0;
      padding: 1.25rem;
      background: #fdf4eb;
      border-left: 5px solid #d96b27;
      border-radius: 0 8px 8px 0;
      font-style: italic;
      color: #2a1508;
      line-height: 1.6;
    }
  `]
})
export class TradutorCnvComponent {
  private readonly motor = inject(MotorCnvService);

  readonly inputControl = new FormControl<string>('', {
    nonNullable: true,
    validators: [Validators.required, Validators.minLength(10)]
  });

  readonly processando = signal(false);
  readonly copiado = signal(false);
  readonly resultadoAtual = signal<ResultadoTraducao | null>(null);

  aoTraduzir(): void {
    if (this.inputControl.invalid) return;

    this.processando.set(true);
    this.copiado.set(false);

    // Simula uma pausa respeitosa de reflexão (400ms)
    setTimeout(() => {
      const traducao = this.motor.traduzir(this.inputControl.value);
      this.resultadoAtual.set(traducao);
      this.processando.set(false);
    }, 400);
  }

  carregarExemplo(): void {
    this.inputControl.setValue(
      'Você é um irresponsável e NUNCA entrega nada no prazo! O deploy atrasou e o cliente está furioso por sua culpa!'
    );
  }

  copiarResposta(texto: string): void {
    if (navigator.clipboard) {
      navigator.clipboard.writeText(texto).then(() => {
        this.copiado.set(true);
        setTimeout(() => this.copiado.set(false), 3000);
      });
    }
  }
}
```

---

## 7. Passo 5: Testes Automatizados com o `TestBed`

Nenhum software compassivo pode ser considerado concluído sem uma suíte de testes de unidade automatizada que proteja sua integridade comportamental.

### Testando o Serviço de Negócio (`motor-cnv.service.spec.ts`)

```typescript
// src/app/core/services/motor-cnv.service.spec.ts
import { TestBed } from '@angular/core/testing';
import { MotorCnvService } from './motor-cnv.service';

describe('MotorCnvService — Suíte de Testes Unitários de Empatia', () => {
  let service: MotorCnvService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(MotorCnvService);
  });

  it('deve inicializar com histórico de traduções vazio e reativo', () => {
    expect(service.historico().length).toBe(0);
    expect(service.totalTraducoes()).toBe(0);
    expect(service.ultimaTraducao()).toBeNull();
  });

  it('deve decompor com exatidão os 4 pilares da CNV ao processar texto agressivo', () => {
    const textoAgressivo = 'Você NUNCA me ouve e sempre atrasa as entregas!';
    const res = service.traduzir(textoAgressivo);

    expect(res).toBeDefined();
    expect(res.observacao.nome).toBe('Observação');
    expect(res.sentimento.nome).toBe('Sentimento');
    expect(res.necessidade.nome).toBe('Necessidade');
    expect(res.pedido.nome).toBe('Pedido');
    
    // Deve detectar a tensão elevada devido ao CAPS e palavras 'nunca' e 'sempre'
    expect(res.nivelTensaoOriginal).toBeGreaterThanOrEqual(6);
  });

  it('deve armazenar no Signal imutável e respeitar o limite máximo de 10 itens', () => {
    for (let i = 1; i <= 15; i++) {
      service.traduzir(`Mensagem de teste número ${i}`);
    }

    expect(service.totalTraducoes()).toBe(10);
    expect(service.ultimaTraducao()?.textoOriginal).toBe('Mensagem de teste número 15');
  });

  it('deve limpar todo o histórico quando o método for invocado', () => {
    service.traduzir('Texto de teste rápido');
    expect(service.totalTraducoes()).toBe(1);

    service.limparHistorico();
    expect(service.totalTraducoes()).toBe(0);
    expect(service.ultimaTraducao()).toBeNull();
  });
});
```

---

## 8. Como Executar e Experimentar o Projeto

A aplicação já está completamente configurada na pasta do repositório. Para executá-la no seu ambiente local:

```bash
# 1. Navegue até o diretório do projeto no terminal
cd angular/livros/02-cnv-angular/codigo/cnv-app

# 2. Instale os pacotes npm
npm install

# 3. Inicie o servidor de desenvolvimento do Angular
npm start
# O terminal exibirá: Application bundle generation complete. Local: http://localhost:4200/

# 4. Abra http://localhost:4200 no navegador para experimentar o Tradutor de Empatia
```

Para rodar a suíte de testes de unidade automatizada:

```bash
npm test
```

---

## 9. Conclusão da Jornada: O Código como Instrumento de Paz

Parabéns! Você completou com maestria este livro extraordinário. 

Você não dominou apenas o **Angular moderno** em sua forma mais pura — Signals reativos, Componentes Standalone, Injeção por Função `inject()`, Deferrable Views `@defer`, Guards Funcionais e Acessibilidade WCAG. 

Você absorveu algo imensamente mais raro e transformador: **a consciência de que por trás de cada tela de computador, de cada linha de log e de cada commit do Git, existe um ser humano com necessidades universais de clareza, respeito, acolhimento e paz.**

Que seu código continue, hoje e sempre, construindo pontes em vez de muros.

---

::: quiz
? Qual é o papel principal da aplicação "O Tradutor de Empatia" desenvolvida no Laboratório 2?
- Servir como corretor gramatical ortográfico para e-mails executivos
+ Acolher mensagens agressivas ou tensas e decompô-las nos quatro passos estruturados da CNV (Observação, Sentimento, Necessidade e Pedido)
- Bloquear a conta de e-mail de colegas de trabalho que enviam mensagens agressivas
- Substituir o compilador TypeScript por inteligência artificial generativa
> O Tradutor de Empatia atua como mediador reflexivo, desarmando a violência verbal moralizante e convertendo o conflito em um diálogo colaborativo fundamentado em necessidades humanas.

? Por que o `MotorCnvService` utiliza `_historico.asReadonly()` para expor seu estado público?
- Porque a documentação do Angular proíbe qualquer leitura de variáveis
+ Para assegurar o encapsulamento e a imutabilidade, impedindo que componentes consumidores alterem o histórico arbitrariamente sem invocar os métodos oficiais do serviço
- Para economizar licenças de software no servidor de nuvem
- Para converter os dados em formato binário comprimido
> O método `.asReadonly()` garante a integridade arquitetural: os componentes podem reagir a qualquer alteração nos dados, mas são incapazes de corromper a lista original diretamente.

? O que a heurística `estimarTensao()` avalia na mensagem inserida pelo usuário?
- A velocidade com que a pessoa digitou as teclas no computador
+ A presença de marcadores hostis como generalizações absolutas ("nunca", "sempre"), palavras de acusação moralizante ("culpa", "burrice") e o uso de gritos em caixa alta (CAPS)
- O endereço IP de onde partiu a conexão de rede
- O navegador e o sistema operacional utilizados pelo usuário
> Na CNV, palavras como "nunca" e "sempre" misturam observação com julgamento acusatório. A heurística detecta esses padrões e quantifica o nível de tensão emocional para guiar a desescalada.

? Por que a resposta sugerida pelo Tradutor é formulada no padrão "Quando..., eu me sinto... porque preciso..."?
- Porque computadores só conseguem processar requisições em primeira pessoa
+ Porque a CNV estabelece que assumir a responsabilidade pelos próprios sentimentos em primeira pessoa desarma as defesas do interlocutor e convida à cooperação sincera
- Porque a legislação brasileira de telecomunicações exige essa formatação
- Porque formulários reativos do Angular não aceitam pronomes na terceira pessoa
> Falar em primeira pessoa expressa honestidade e vulnerabilidade, evitando acusações que provocam contra-ataques e focando no valor humano que precisa ser atendido mutuamente.

? Qual é a função da propriedade `pure: true` no decorador do pipe `PilarEmojiPipe`?
- Fazer com que o pipe consuma menos energia elétrica na placa de vídeo
+ Informar ao Angular que a transformação só precisa ser recalculada quando o valor de entrada mudar primitivamente, garantindo desempenho ideal
- Impedir que o texto seja traduzido para outros idiomas
- Desativar a checagem de erros do TypeScript
> Pipes puros evitam execuções repetidas a cada ciclo de renderização, garantindo alta eficiência e reatividade otimizada na renderização de listas e componentes.
:::

---

::: cartas
Tradutor de Empatia :: Aplicação Angular 17+ que acolhe comunicações agressivas e as decompõe nos 4 passos estruturados da CNV.
MotorCnvService :: Serviço singleton reativo com Signals que centraliza o algoritmo heurístico de desarmamento e histórico de traduções.
Heurística de Tensão :: Algoritmo que detecta palavras moralizantes, generalizações ("sempre/nunca") e caixa alta para quantificar o conflito.
asReadonly :: Método do Signal que expõe uma visão imutável do estado para a UI, protegendo as regras de negócio contra mutações externas.
PilarEmojiPipe :: Pipe puro (@Pipe pure: true) que mapeia semântica e acessivelmente os pilares da CNV para ícones sem impacto de performance.
WCAG aria-live :: Região de template que vocaliza em leitores de tela os resultados da decomposição de forma polida e inclusiva.
Código como Instrumento de Paz :: Filosofia de engenharia onde a arquitetura técnica de software serve à dignidade e às necessidades humanas.
:::
