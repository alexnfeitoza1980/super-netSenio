import { Pipe, PipeTransform } from '@angular/core';

/**
 * Cap. 11 — um pipe PURO: o Angular só o executa de novo quando a entrada muda.
 * Uma função chamada no template ({{ formatar(x) }}) rodaria a cada verificação daquele componente.
 */
@Pipe({ name: 'minutos' })
export class MinutosPipe implements PipeTransform {
  transform(total: number): string {
    if (!Number.isFinite(total) || total <= 0) return '0 min';
    const h = Math.floor(total / 60), m = Math.round(total % 60);
    if (!h) return `${m} min`;
    return m ? `${h} h ${m} min` : `${h} h`;
  }
}
