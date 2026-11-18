import { Foco } from './foco';

describe('Foco com linkedSignal (cap. 3)', () => {
  it('começa na primeira prática e aceita escolha manual', () => {
    const f = new Foco();
    expect(f.escolhida()).toBe('respiração');
    f.escolhida.set('sons');
    expect(f.escolhida()).toBe('sons');
  });

  it('mantém a escolha quando a lista muda e ela ainda existe', () => {
    const f = new Foco();
    f.escolhida.set('corpo');
    f.praticas.set(['corpo', 'caminhada']);
    expect(f.escolhida()).toBe('corpo');
  });

  it('volta para a primeira quando a escolhida some da lista', () => {
    const f = new Foco();
    f.escolhida.set('sons');
    f.praticas.set(['caminhada', 'corpo']);
    expect(f.escolhida()).toBe('caminhada');
  });

  it('lista vazia: não há foco possível', () => {
    const f = new Foco();
    f.praticas.set([]);
    expect(f.escolhida()).toBeUndefined();
  });
});
