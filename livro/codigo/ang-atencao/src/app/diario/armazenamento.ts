import { Injectable } from '@angular/core';

/**
 * Lab 2 — onde o Diário guarda os dados. A classe abstrata é o contrato (e o token de injeção);
 * o app usa o localStorage, os testes usam a memória. Nenhum dado sai do navegador.
 */
@Injectable({ providedIn: 'root', useFactory: () => new ArmazenamentoLocal() })
export abstract class Armazenamento {
  abstract ler(chave: string): string | null;
  abstract gravar(chave: string, valor: string): void;
}

export class ArmazenamentoLocal extends Armazenamento {
  ler(chave: string): string | null {
    try {
      return localStorage.getItem(chave);
    } catch {
      return null; // modo privado, cota cheia, armazenamento bloqueado: seguimos sem quebrar
    }
  }

  gravar(chave: string, valor: string): void {
    try {
      localStorage.setItem(chave, valor);
    } catch {
      /* idem: o app continua funcionando só na memória */
    }
  }
}

export class ArmazenamentoEmMemoria extends Armazenamento {
  readonly dados = new Map<string, string>();

  ler(chave: string): string | null {
    return this.dados.get(chave) ?? null;
  }

  gravar(chave: string, valor: string): void {
    this.dados.set(chave, valor);
  }
}
