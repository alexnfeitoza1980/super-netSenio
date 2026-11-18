import { Component, inject, signal } from '@angular/core';
import { FormControl, ReactiveFormsModule, Validators } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { MotorCnvService } from '../../core/services/motor-cnv.service';
import { ResultadoTraducao } from '../../core/models/traducao-cnv.model';

@Component({
  selector: 'app-tradutor-cnv',
  standalone: true,
  imports: [ReactiveFormsModule, CommonModule],
  template: `
    <div class="tradutor-layout">
      <section class="painel-entrada">
        <header>
          <h2>O Tradutor de Empatia</h2>
          <p class="subtitulo">Transforme mensagens tensas e julgamentos em pontes de cooperação e diálogo construtivo.</p>
        </header>

        <form (ngSubmit)="aoTraduzir()" class="form-traducao">
          <label for="texto-entrada">Cole abaixo a mensagem tensa recebida:</label>
          <textarea
            id="texto-entrada"
            rows="4"
            [formControl]="inputControl"
            placeholder="Exemplo: Você nunca me ouve, seu código atrasou todo o sprint e agora está tudo quebrado!"
          ></textarea>

          <div class="botoes-acao">
            <button
              type="button"
              class="btn-exemplo"
              (click)="carregarExemplo()"
            >
              Carregar Exemplo Típico
            </button>
            <button
              type="submit"
              class="btn-traduzir"
              [disabled]="inputControl.invalid || processando()"
            >
              @if (processando()) {
                Traduzindo com Empatia...
              } @else {
                Decompor em 4 Passos CNV ✨
              }
            </button>
          </div>
        </form>
      </section>

      @if (resultadoAtual(); as res) {
        <section class="painel-resultado" aria-live="polite">
          <header class="resultado-cabecalho">
            <div class="indicador-tensao">
              <span>Nível de Tensão Inicial Desarmado:</span>
              <strong class="tensao-valor">{{ res.nivelTensaoOriginal }} / 10</strong>
            </div>
            <button type="button" class="btn-copiar" (click)="copiarResposta(res.respostaSugerida)">
              {{ copiado() ? '✓ Resposta Copiada!' : 'Copiar Resposta Sugerida' }}
            </button>
          </header>

          <div class="grade-pilares">
            <article class="card-pilar pilar-observacao">
              <span class="pilar-icone">{{ res.observacao.icone }}</span>
              <h4>1. {{ res.observacao.nome }}</h4>
              <p class="pilar-conteudo">{{ res.observacao.conteudo }}</p>
              <small class="pilar-dica">{{ res.observacao.orientacao }}</small>
            </article>

            <article class="card-pilar pilar-sentimento">
              <span class="pilar-icone">{{ res.sentimento.icone }}</span>
              <h4>2. {{ res.sentimento.nome }}</h4>
              <p class="pilar-conteudo">{{ res.sentimento.conteudo }}</p>
              <small class="pilar-dica">{{ res.sentimento.orientacao }}</small>
            </article>

            <article class="card-pilar pilar-necessidade">
              <span class="pilar-icone">{{ res.necessidade.icone }}</span>
              <h4>3. {{ res.necessidade.nome }}</h4>
              <p class="pilar-conteudo">{{ res.necessidade.conteudo }}</p>
              <small class="pilar-dica">{{ res.necessidade.orientacao }}</small>
            </article>

            <article class="card-pilar pilar-pedido">
              <span class="pilar-icone">{{ res.pedido.icone }}</span>
              <h4>4. {{ res.pedido.nome }}</h4>
              <p class="pilar-conteudo">{{ res.pedido.conteudo }}</p>
              <small class="pilar-dica">{{ res.pedido.orientacao }}</small>
            </article>
          </div>

          <footer class="resposta-sugerida-bloco">
            <h5>Mensagem Compassiva Recomendada para Enviar:</h5>
            <blockquote class="citacao-compassiva">
              "{{ res.respostaSugerida }}"
            </blockquote>
          </footer>
        </section>
      }
    </div>
  `,
  styles: [`
    .tradutor-layout { max-width: 860px; margin: 0 auto; display: flex; flex-direction: column; gap: 2rem; font-family: system-ui, sans-serif; }
    .painel-entrada { background: #fffaf5; border: 1px solid #f5dfd0; border-radius: 14px; padding: 2rem; box-shadow: 0 4px 16px rgba(42,21,8,0.04); }
    .subtitulo { color: #6c3c1e; margin-top: -0.5rem; margin-bottom: 1.5rem; }
    textarea { width: 100%; border: 1px solid #f5dfd0; border-radius: 8px; padding: 1rem; font-family: inherit; font-size: 1rem; box-sizing: border-box; }
    .botoes-acao { display: flex; justify-content: flex-end; gap: 1rem; margin-top: 1rem; }
    .btn-traduzir { background: #d96b27; color: white; border: none; padding: 0.75rem 1.5rem; border-radius: 8px; font-weight: bold; cursor: pointer; }
    .btn-traduzir:disabled { background: #cbd5e1; cursor: not-allowed; }
    .btn-exemplo { background: #fdf4eb; border: 1px solid #f5dfd0; color: #6c3c1e; padding: 0.75rem 1rem; border-radius: 8px; cursor: pointer; }
    
    .painel-resultado { background: white; border: 1px solid #f5dfd0; border-radius: 14px; padding: 2rem; box-shadow: 0 8px 24px rgba(42,21,8,0.06); }
    .resultado-cabecalho { display: flex; justify-content: space-between; align-items: center; border-bottom: 1px solid #f5dfd0; padding-bottom: 1rem; margin-bottom: 1.5rem; }
    .tensao-valor { color: #d97706; margin-left: 0.5rem; font-size: 1.1rem; }
    .btn-copiar { background: #10b981; color: white; border: none; padding: 0.5rem 1rem; border-radius: 6px; cursor: pointer; font-weight: 600; }
    
    .grade-pilares { display: grid; grid-template-columns: repeat(auto-fit, minmax(180px, 1fr)); gap: 1rem; }
    .card-pilar { background: #fffaf5; border: 1px solid #f5dfd0; border-radius: 10px; padding: 1.25rem; }
    .pilar-icone { font-size: 1.8rem; display: block; margin-bottom: 0.5rem; }
    .card-pilar h4 { margin: 0 0 0.5rem 0; font-size: 1rem; color: #2a1508; }
    .pilar-conteudo { font-size: 0.9rem; color: #4b5563; margin-bottom: 0.75rem; line-height: 1.4; }
    .pilar-dica { font-size: 0.75rem; color: #9ca3af; display: block; font-style: italic; }
    
    .resposta-sugerida-bloco { margin-top: 1.5rem; border-top: 1px solid #f5dfd0; padding-top: 1.25rem; }
    .resposta-sugerida-bloco h5 { margin: 0 0 0.5rem 0; color: #2a1508; }
    .citacao-compassiva { background: #fdf4eb; border-left: 4px solid #d96b27; margin: 0; padding: 1rem; font-style: italic; border-radius: 0 8px 8px 0; color: #2a1508; }
  `]
})
export class TradutorCnvComponent {
  private readonly motor = inject(MotorCnvService);

  readonly inputControl = new FormControl('', [Validators.required, Validators.minLength(10)]);
  readonly processando = signal(false);
  readonly copiado = signal(false);
  readonly resultadoAtual = signal<ResultadoTraducao | null>(null);

  aoTraduzir(): void {
    if (this.inputControl.invalid) return;

    this.processando.set(true);
    this.copiado.set(false);

    setTimeout(() => {
      const traducao = this.motor.traduzir(this.inputControl.value!);
      this.resultadoAtual.set(traducao);
      this.processando.set(false);
    }, 300);
  }

  carregarExemplo(): void {
    this.inputControl.setValue(
      'Você é um irresponsável e nunca entrega nada no prazo! O cliente está furioso por sua culpa!'
    );
  }

  copiarResposta(texto: string): void {
    navigator.clipboard.writeText(texto).then(() => {
      this.copiado.set(true);
      setTimeout(() => this.copiado.set(false), 3000);
    });
  }
}

