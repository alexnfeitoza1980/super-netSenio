import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MedoService } from '../../core/services/medo.service';
import { CadastrarMedoComponent } from '../cadastrar-medo/cadastrar-medo.component';
import { DiagnosticoMedoPipe } from '../../shared/pipes/diagnostico-medo.pipe';
import { BrilhoAntimedoDirective } from '../../shared/directives/brilho-antimedo.directive';

@Component({
  selector: 'app-painel',
  standalone: true,
  imports: [CommonModule, CadastrarMedoComponent, DiagnosticoMedoPipe, BrilhoAntimedoDirective],
  template: `
    <div class="painel-wrap">
      <!-- HEADER COM INDICADORES COMPUTADOS (SIGNALS) -->
      <section class="metricas-grid">
        <div class="card-metrica" appBrilhoAntimedo="#ff6b35">
          <span>Total de Desafios</span>
          <b>{{ medoService.totalMedos() }}</b>
        </div>
        <div class="card-metrica" appBrilhoAntimedo="#168a5c">
          <span>Desafios Vencidos</span>
          <b class="texto-verde">{{ medoService.totalVencidos() }}</b>
        </div>
        <div class="card-metrica" appBrilhoAntimedo="#dd1b44">
          <span>Taxa de Coragem</span>
          <b class="texto-primario">{{ medoService.percentualCoragem() }}%</b>
        </div>
        <div class="card-metrica" appBrilhoAntimedo="#c77700">
          <span>Ruídos para Cortar</span>
          <b class="texto-aviso">{{ medoService.ruidosDetectados() }}</b>
        </div>
      </section>

      <!-- FORMULÁRIO DE ENTRADA -->
      <app-cadastrar-medo></app-cadastrar-medo>

      <!-- LISTA DE ENFRENTAMENTO TÁTICO -->
      <section class="lista-container">
        <h2>Radar de Enfrentamento Ativo</h2>

        <div *ngIf="medoService.totalMedos() === 0" class="vazio">
          Nenhum desafio registrado. Parabéns pela coragem ou adicione seu primeiro medo acima!
        </div>

        <div *ngFor="let m of medoService.medos()" class="card-medo" [class.card-vencido]="m.vencido">
          <div class="medo-header">
            <span class="badge-cat">{{ m.categoria }}</span>
            <span class="diagnostico">{{ m | diagnosticoMedo }}</span>
            <span class="grau">Intensidade: {{ m.nivelParalisia }}/10</span>
          </div>

          <h3 class="medo-titulo" [class.riscado]="m.vencido">{{ m.titulo }}</h3>

          <div class="medo-acoes">
            <button 
              class="btn-acao btn-enfrentar" 
              (click)="medoService.alternarVencido(m.id)">
              {{ m.vencido ? '↺ Reabrir Desafio' : '✓ Marcar como Vencido' }}
            </button>
            <button 
              class="btn-acao btn-excluir" 
              (click)="medoService.remover(m.id)" 
              title="Excluir">
              🗑
            </button>
          </div>
        </div>
      </section>
    </div>
  `,
  styles: [`
    .painel-wrap { max-width: 900px; margin: 0 auto; padding: 10px 0; }
    .metricas-grid { display: grid; grid-template-columns: repeat(auto-fit, minmax(180px, 1fr)); gap: 16px; margin-bottom: 24px; }
    .card-metrica { background: #fff; border: 1px solid #f5d8de; border-radius: 12px; padding: 18px; text-align: center; }
    .card-metrica span { font-size: 13px; color: #673040; text-transform: uppercase; font-weight: 700; letter-spacing: 0.05em; }
    .card-metrica b { display: block; font-size: 32px; font-weight: 800; margin-top: 6px; }
    .texto-verde { color: #168a5c; }
    .texto-primario { color: #dd1b44; }
    .texto-aviso { color: #c77700; }
    .card-medo { background: #fff; border-left: 6px solid #dd1b44; border-radius: 10px; padding: 18px; margin-bottom: 14px; box-shadow: 0 2px 8px rgba(0,0,0,0.04); transition: all .2s; }
    .card-medo.card-vencido { border-left-color: #168a5c; opacity: 0.85; background: #f6fcf8; }
    .medo-header { display: flex; align-items: center; gap: 12px; font-size: 12.5px; margin-bottom: 8px; flex-wrap: wrap; }
    .badge-cat { background: #fde8ed; color: #dd1b44; padding: 2px 8px; border-radius: 4px; font-weight: 800; }
    .diagnostico { font-weight: 700; color: #4a5164; }
    .grau { margin-left: auto; color: #7a8196; }
    .medo-titulo { margin: 6px 0 14px; font-size: 19px; }
    .medo-titulo.riscado { text-decoration: line-through; color: #7a8196; }
    .medo-acoes { display: flex; gap: 10px; }
    .btn-acao { border: none; padding: 8px 16px; border-radius: 6px; font-weight: 700; cursor: pointer; }
    .btn-enfrentar { background: #dd1b44; color: #fff; }
    .card-vencido .btn-enfrentar { background: #168a5c; }
    .btn-excluir { background: #f0f0f0; color: #555; }
    .vazio { text-align: center; padding: 40px; color: #7a8196; font-style: italic; }
  `]
})
export class PainelComponent {
  medoService = inject(MedoService);
}

