import { provideHttpClient, withFetch, withInterceptors } from '@angular/common/http';
import { ApplicationConfig, ErrorHandler, provideBrowserGlobalErrorListeners } from '@angular/core';
import { provideRouter, withComponentInputBinding, withInMemoryScrolling } from '@angular/router';
import { routes } from './app.routes';
import { ampulhetaInterceptor } from './capitulos/cap09/ampulheta.interceptor';
import { TratadorDeErros } from './capitulos/cap09/tratador-de-erros';

/**
 * A configuração do app inteiro, num lugar só. Sem zone.js e com OnPush por padrão (Angular 22),
 * não há nada a ligar aqui para a detecção de mudanças: é o comportamento normal.
 */
export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideRouter(routes, withComponentInputBinding(), withInMemoryScrolling({ scrollPositionRestoration: 'top' })),
    provideHttpClient(withFetch(), withInterceptors([ampulhetaInterceptor])),
    { provide: ErrorHandler, useExisting: TratadorDeErros },
  ],
};
