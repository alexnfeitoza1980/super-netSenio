import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';

/** Cap. 7 — a rota coringa (**): perder-se faz parte; o importante é ter um caminho de volta. */
@Component({
  selector: 'app-nao-encontrada',
  imports: [RouterLink],
  template: `
    <h1>Esta página não existe</h1>
    <p>Respire. Dá para voltar ao <a routerLink="/diario">Diário</a> ou ao <a routerLink="/capitulos">catálogo</a>.</p>
  `,
})
export class NaoEncontrada {}
