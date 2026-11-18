import { Component, signal } from '@angular/core';
import { form, FormField, max, maxLength, min, required, submit } from '@angular/forms/signals';

export interface RegistroDoMomento {
  palavra: string;
  minutos: number;
  nota: string;
}

const VAZIO: RegistroDoMomento = { palavra: '', minutos: 5, nota: '' };

/**
 * Cap. 10 — Signal Forms (estáveis no Angular 22): o modelo é um signal comum; o formulário é
 * uma árvore de campos derivada dele. As regras ficam num só lugar e as mensagens são gentis:
 * dizem o que fazer, sem acusar.
 */
@Component({
  selector: 'app-registro-gentil',
  imports: [FormField],
  template: `
    <form (submit)="salvar($event)" novalidate>
      <label for="palavra">Uma palavra para este momento</label>
      <input id="palavra" [formField]="registro.palavra" [attr.aria-invalid]="mostrarErros(registro.palavra)" aria-describedby="erros-palavra" />
      <div id="erros-palavra">
        @if (mostrarErros(registro.palavra)) {
          @for (e of registro.palavra().errors(); track e.kind) {
            <p class="erro">{{ e.message }}</p>
          }
        }
      </div>

      <label for="minutos">Quantos minutos você praticou?</label>
      <input id="minutos" type="number" [formField]="registro.minutos" aria-describedby="erros-minutos" />
      <div id="erros-minutos">
        @if (mostrarErros(registro.minutos)) {
          @for (e of registro.minutos().errors(); track e.kind) {
            <p class="erro">{{ e.message }}</p>
          }
        }
      </div>

      <label for="nota">Nota (opcional)</label>
      <textarea id="nota" [formField]="registro.nota" aria-describedby="contador"></textarea>
      <p id="contador">{{ modelo().nota.length }} / 280</p>

      <button type="submit" class="primario">Guardar</button>
    </form>

    @if (salvos().length) {
      <h3>Hoje</h3>
      <ul>
        @for (r of salvos(); track $index) {
          <li>{{ r.palavra }} · {{ r.minutos }} min</li>
        }
      </ul>
    }
  `,
})
export class RegistroGentil {
  protected readonly modelo = signal<RegistroDoMomento>({ ...VAZIO });
  protected readonly salvos = signal<readonly RegistroDoMomento[]>([]);

  protected readonly registro = form(this.modelo, (campo) => {
    required(campo.palavra, { message: 'Escreva uma palavra, qualquer uma serve: "cansado", "curioso", "ok".' });
    maxLength(campo.palavra, 30, { message: 'Uma palavra curta basta (até 30 letras).' });
    min(campo.minutos, 1, { message: 'Mesmo um minuto conta. Use 1 ou mais.' });
    max(campo.minutos, 120, { message: 'Até 120 minutos por registro; divida sessões mais longas.' });
    maxLength(campo.nota, 280, { message: 'A nota passou de 280 caracteres. Que tal guardar só o essencial?' });
  });

  protected mostrarErros(campo: () => { touched(): boolean; invalid(): boolean }): boolean {
    return campo().touched() && campo().invalid();
  }

  protected async salvar(evento: Event): Promise<void> {
    evento.preventDefault();
    await submit(this.registro, async () => {
      this.salvos.update((s) => [...s, { ...this.modelo(), palavra: this.modelo().palavra.trim() }]);
      this.registro().reset({ ...VAZIO });
      return undefined;
    });
  }
}
