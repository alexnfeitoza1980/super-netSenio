import { ErrorHandler, Injectable, signal } from '@angular/core';

/**
 * Cap. 9 — o último recurso: erros que ninguém tratou chegam aqui.
 * Em vez de sumirem no console, viram um registro que a tela pode mostrar (e que um serviço de
 * observabilidade poderia receber). Nunca guarde dados pessoais nesse registro.
 */
@Injectable({ providedIn: 'root' })
export class TratadorDeErros implements ErrorHandler {
  readonly ultimos = signal<readonly string[]>([]);

  handleError(erro: unknown): void {
    const texto = erro instanceof Error ? `${erro.name}: ${erro.message}` : String(erro);
    this.ultimos.update((l) => [texto, ...l].slice(0, 10));
    console.error(erro);
  }
}
