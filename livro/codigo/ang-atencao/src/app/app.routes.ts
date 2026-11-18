import { Routes } from '@angular/router';
import { demoExiste } from './capitulos/cap07/guardas';

/** Cap. 7 — o mapa do app. Tudo além da casca é carregado sob demanda (loadComponent/loadChildren). */
export const routes: Routes = [
  { path: '', pathMatch: 'full', redirectTo: 'diario' },
  { path: 'diario', loadChildren: () => import('./diario/diario.routes').then((m) => m.DIARIO_ROTAS) },
  {
    path: 'pausa',
    title: 'Pausa de um minuto',
    loadComponent: () => import('./labs/lab01/temporizador').then((m) => m.Temporizador),
  },
  {
    path: 'capitulos',
    title: 'Demonstrações',
    loadComponent: () => import('./capitulos/cap07/catalogo').then((m) => m.Catalogo),
  },
  {
    path: 'capitulos/:id',
    title: 'Demonstração',
    canActivate: [demoExiste],
    loadComponent: () => import('./capitulos/cap07/demo-do-capitulo').then((m) => m.DemoDoCapitulo),
  },
  {
    path: '**',
    title: 'Página não encontrada',
    loadComponent: () => import('./capitulos/cap07/nao-encontrada').then((m) => m.NaoEncontrada),
  },
];
