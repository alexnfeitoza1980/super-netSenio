import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MedoService } from '../../core/services/medo.service';
import { CategoriaMedo } from '../../core/models/medo.model';

@Component({
  selector: 'app-cadastrar-medo',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  template: `
    <section class="form-container">
      <h3>Cadastrar Novo Bloqueio para Enfrentamento</h3>
      
      <form [formGroup]="form" (ngSubmit)="salvar()">
        <div class="campo">
          <label for="titulo">Qual o nome do medo ou bloqueio?</label>
          <input 
            id="titulo" 
            type="text" 
            formControlName="titulo" 
            placeholder="Ex: Medo de refatorar código legado"
          />
          <span class="erro" *ngIf="form.get('titulo')?.touched && form.get('titulo')?.invalid">
            O nome é obrigatório e precisa de no mínimo 5 caracteres.
          </span>
        </div>

        <div class="linha-dupla">
          <div class="campo">
            <label for="categoria">Categoria de Vida</label>
            <select id="categoria" formControlName="categoria">
              <option value="TECNICO">Técnico / Código</option>
              <option value="PROFISSIONAL">Carreira / Equipe</option>
              <option value="PESSOAL">Pessoal / Emocional</option>
              <option value="FINANCEIRO">Financeiro / Negócios</option>
            </select>
          </div>

          <div class="campo">
            <label for="nivel">Nível de Paralisia (1 a 10): {{ form.get('nivelParalisia')?.value }}</label>
            <input 
              id="nivel" 
              type="range" 
              min="1" 
              max="10" 
              formControlName="nivelParalisia"
            />
          </div>
        </div>

        <div class="campo-check">
          <label>
            <input type="checkbox" formControlName="ehRuido" />
            Isso é apenas <strong>ruído</strong> (opinião alheia, boatos de internet)?
          </label>
        </div>

        <button type="submit" [disabled]="form.invalid" class="btn-cadastrar">
          Registrar Desafio no Radar
        </button>
      </form>
    </section>
  `,
  styles: [`
    .form-container { background: #fff; border: 1px solid #f5d8de; border-radius: 14px; padding: 24px; margin-bottom: 24px; box-shadow: 0 4px 12px rgba(0,0,0,0.04); }
    .campo { display: flex; flex-direction: column; gap: 6px; margin-bottom: 14px; }
    .linha-dupla { display: grid; grid-template-columns: 1fr 1fr; gap: 16px; }
    .campo input[type="text"], .campo select { padding: 10px 14px; border: 1px solid #ddd; border-radius: 8px; font-size: 15px; }
    .erro { color: #cf3f4b; font-size: 12.5px; font-weight: 600; }
    .campo-check { margin: 12px 0 18px; }
    .btn-cadastrar { background: #dd1b44; color: #fff; font-weight: 700; border: none; padding: 12px 24px; border-radius: 8px; cursor: pointer; transition: opacity .2s; }
    .btn-cadastrar:disabled { opacity: 0.5; cursor: not-allowed; }
  `]
})
export class CadastrarMedoComponent {
  private fb = inject(FormBuilder);
  private medoService = inject(MedoService);

  form = this.fb.group({
    titulo: ['', [Validators.required, Validators.minLength(5)]],
    categoria: ['TECNICO' as CategoriaMedo, Validators.required],
    nivelParalisia: [5, [Validators.required, Validators.min(1), Validators.max(10)]],
    ehRuido: [false]
  });

  salvar(): void {
    if (this.form.invalid) return;

    const val = this.form.value;
    this.medoService.adicionar({
      titulo: val.titulo!,
      categoria: val.categoria as CategoriaMedo,
      nivelParalisia: Number(val.nivelParalisia),
      ehRuido: Boolean(val.ehRuido),
      acoes: []
    });

    this.form.reset({
      titulo: '',
      categoria: 'TECNICO',
      nivelParalisia: 5,
      ehRuido: false
    });
  }
}

