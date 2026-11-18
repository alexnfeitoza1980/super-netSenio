import { computed, signal } from '@angular/core';
import { SessaoAtuante } from './sessao-atuante';
import { SessaoExistente } from './sessao-existente';

describe('modo Atuante x modo Existente (cap. 3)', () => {
  // #region atuante
  it('Atuante: depois de replanejar, o estado copiado fica errado', () => {
    const s = new SessaoAtuante();
    s.praticar(6);
    s.replanejar(5);

    expect(s.minutosFeitos).toBeGreaterThanOrEqual(s.minutosPlanejados);
    expect(s.concluida).toBe(false); // deveria ser true: o bug do "esqueci de atualizar"
    expect(s.restante).toBe(4); //      deveria ser 0
  });
  // #endregion

  // #region existente
  it('Existente: o derivado nunca se desencontra da fonte', () => {
    const s = new SessaoExistente();
    s.praticar(6);
    s.replanejar(5);

    expect(s.concluida()).toBe(true);
    expect(s.restante()).toBe(0);
  });
  // #endregion

  // #region preguicoso
  it('computed é preguiçoso e só recalcula quando uma fonte muda', () => {
    const minutos = signal(3);
    let calculos = 0;
    const segundos = computed(() => {
      calculos++;
      return minutos() * 60;
    });

    segundos();
    segundos();
    expect(calculos).toBe(1); // a segunda leitura veio da memória

    minutos.set(4);
    expect(calculos).toBe(1); // preguiçoso: ninguém leu ainda
    expect(segundos()).toBe(240);
    expect(calculos).toBe(2);
  });
  // #endregion
});
