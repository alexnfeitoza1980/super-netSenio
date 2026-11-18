import { ChangeDetectorRef, Component, inject, signal } from '@angular/core';

/**
 * Cap. 5 — três jeitos de mudar um número depois de um tempo.
 * Sem zone.js (o padrão desde o Angular 21), o framework só redesenha quando ALGO AVISA:
 * um signal lido no template, um evento do template, ou um aviso explícito (markForCheck).
 */

/** Piloto automático: muda um campo comum fora de qualquer evento. Ninguém fica sabendo. */
@Component({
  selector: 'app-contador-distraido',
  template: `<p>Respirações notadas: {{ respiracoes }}</p>`,
})
export class ContadorDistraido {
  respiracoes = 0;

  contarDepois(ms: number): void {
    setTimeout(() => this.respiracoes++, ms);
  }
}

/** Atento: o valor é um signal, e o template que o lê é avisado sozinho. */
@Component({
  selector: 'app-contador-atento',
  template: `<p>Respirações notadas: {{ respiracoes() }}</p>`,
})
export class ContadorAtento {
  readonly respiracoes = signal(0);

  contarDepois(ms: number): void {
    setTimeout(() => this.respiracoes.update((n) => n + 1), ms);
  }
}

/** Avisado à mão: funciona, mas depende de lembrar de avisar em todo lugar que muda. */
@Component({
  selector: 'app-contador-avisado',
  template: `<p>Respirações notadas: {{ respiracoes }}</p>`,
})
export class ContadorAvisado {
  respiracoes = 0;
  private readonly cdr = inject(ChangeDetectorRef);

  contarDepois(ms: number): void {
    setTimeout(() => {
      this.respiracoes++;
      this.cdr.markForCheck();
    }, ms);
  }
}
