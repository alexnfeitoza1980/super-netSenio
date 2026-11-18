import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: '',
    pathMatch: 'full',
    redirectTo: 'painel'
  },
  {
    path: 'painel',
    loadComponent: () => import('./features/painel/painel.component')
      .then(m => m.PainelComponent)
  },
  {
    path: '**',
    redirectTo: 'painel'
  }
];

