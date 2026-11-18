import { Component, effect } from '@angular/core';
import { SessaoExistente } from './sessao-existente';

/** Cap. 3 — o template só LÊ signals; quem muda o estado são os métodos da sessão. */
@Component({
  selector: 'app-painel-da-sessao',
  template: `
    <p>Planejado: {{ sessao.planejados() }} min · Feito: {{ sessao.feitos() }} min</p>
    <p>
      @if (sessao.concluida()) {
        Sessão concluída. Que tal só notar como você está agora?
      } @else {
        Faltam {{ sessao.restante() }} min.
      }
    </p>
    <button type="button" (click)="sessao.praticar(1)">+1 minuto</button>
    <button type="button" (click)="sessao.replanejar(sessao.planejados() - 1)">Planejar 1 min a menos</button>
  `,
})
export class PainelDaSessao {
  protected readonly sessao = new SessaoExistente();

  constructor() {
    // effect: para EFEITOS fora do mundo dos signals (log, localStorage, título da aba).
    // Nunca para calcular um valor: para isso existe computed.
    effect(() => {
      if (this.sessao.concluida()) console.info(`[diário] sessão de ${this.sessao.planejados()} min concluída`);
    });
  }
}
