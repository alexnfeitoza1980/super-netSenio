import { Pipe, PipeTransform } from '@angular/core';
import { Medo } from '../../core/models/medo.model';

@Pipe({
  name: 'diagnosticoMedo',
  standalone: true
})
export class DiagnosticoMedoPipe implements PipeTransform {
  transform(medo: Medo | null | undefined): string {
    if (!medo) return '';
    if (medo.vencido) {
      return '🏆 CONQUISTADO: Paralisia Neutralizada!';
    }
    if (medo.ehRuido) {
      return '🔇 MERO RUÍDO: Ignore e corte a fonte de distração.';
    }
    if (medo.nivelParalisia >= 8) {
      return '🚨 DESAFIO CRÍTICO: Quebre em microações de 15 minutos.';
    }
    return '⚡ DESAFIO NORMAL: Encare hoje mesmo com foco total.';
  }
}

