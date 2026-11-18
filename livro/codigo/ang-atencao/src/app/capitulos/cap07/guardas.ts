import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { acharDemo } from './demos';

/**
 * Cap. 7 — em vez de só BARRAR (evitar), a guarda REDIRECIONA (aproximar):
 * quem pede uma demonstração que não existe chega ao catálogo, com um aviso do que foi pedido.
 */
export const demoExiste: CanActivateFn = (rota) => {
  const id = rota.paramMap.get('id') ?? '';
  if (acharDemo(id)) return true;
  return inject(Router).createUrlTree(['/capitulos'], { queryParams: { naoAchei: id } });
};
