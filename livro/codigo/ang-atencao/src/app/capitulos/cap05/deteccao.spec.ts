import { Type } from '@angular/core';
import { TestBed } from '@angular/core/testing';
import { ContadorAtento, ContadorAvisado, ContadorDistraido } from './contadores';
import { Vizinhos } from './vizinhos';

const esperar = (ms: number) => new Promise((r) => setTimeout(r, ms));

describe('detecção de mudanças sem zone.js (cap. 5)', () => {
  // #region contadores
  async function depoisDeUmTempo<T extends { contarDepois(ms: number): void }>(tipo: Type<T>) {
    const fixture = TestBed.createComponent(tipo);
    await fixture.whenStable();
    fixture.componentInstance.contarDepois(5);
    await esperar(20);
    await fixture.whenStable();
    return (fixture.nativeElement as HTMLElement).textContent!.trim();
  }

  it('campo comum mudado num setTimeout: a tela fica para trás', async () => {
    expect(await depoisDeUmTempo(ContadorDistraido)).toBe('Respirações notadas: 0');
  });

  it('signal: a tela acompanha sozinha', async () => {
    expect(await depoisDeUmTempo(ContadorAtento)).toBe('Respirações notadas: 1');
  });

  it('markForCheck: acompanha, desde que alguém lembre de avisar', async () => {
    expect(await depoisDeUmTempo(ContadorAvisado)).toBe('Respirações notadas: 1');
  });
  // #endregion

  // #region vizinhos
  it('um evento no pai atualiza o filho Eager, mas não o filho OnPush', async () => {
    const fixture = TestBed.createComponent(Vizinhos);
    await fixture.whenStable();
    const tela = fixture.nativeElement as HTMLElement;

    tela.querySelector('button')!.click();
    await fixture.whenStable();

    expect(tela.querySelector('app-placa-onpush')!.textContent).toBe('antes');
    expect(tela.querySelector('app-placa-eager')!.textContent).toBe('depois');
    expect(tela.querySelector('button')!.textContent).toContain('(1)');
  });
  // #endregion
});
