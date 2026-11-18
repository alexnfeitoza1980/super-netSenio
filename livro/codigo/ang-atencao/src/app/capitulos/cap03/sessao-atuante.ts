/**
 * Cap. 3 — modo Atuante: o estado derivado é copiado à mão.
 * Cada método precisa LEMBRAR de recalcular tudo o que depende do que mudou.
 */
export class SessaoAtuante {
  minutosPlanejados = 10;
  minutosFeitos = 0;
  restante = 10;
  concluida = false;

  praticar(minutos: number): void {
    this.minutosFeitos += minutos;
    this.restante = Math.max(0, this.minutosPlanejados - this.minutosFeitos);
    this.concluida = this.minutosFeitos >= this.minutosPlanejados;
  }

  replanejar(minutos: number): void {
    this.minutosPlanejados = minutos;
    // Esqueceu de recalcular `restante` e `concluida`. Ninguém avisa: o objeto agora mente.
  }
}
