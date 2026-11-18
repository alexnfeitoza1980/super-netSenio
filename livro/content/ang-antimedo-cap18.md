---
id: ang-antimedo-cap18
order: 318
kind: capitulo
num: 223
group: Angular · Parte 55 · Bloco IV — A Invencibilidade
title: Estado desejado
subtitle: Server-Side Rendering (SSR) — a aplicação pronta antes de ser vista
minutes: 30
hue: 220
badge: 🚀
- Compreender o princípio do estado desejado: a mente chega antes do corpo
- Dominar o funcionamento do Server-Side Rendering (SSR) no Angular moderno: 
- Configurar a hidratação não-destrutiva (provideClientHydration): 
- Prevenir falhas de plataforma usando isPlatformBrowser, isPlatformServer e TransferState: 
origem: angular/livros/01-antimedo-angular/content/18-estado-desejado.md
---

::: ilustracao img/ang-antimedo/cap18.svg | Uma cidade inteira visível no horizonte antes mesmo do viajante dar o primeiro passo na estrada.
:::

No livro *Antimedo*, Pablo Marçal enfatiza repetidamente o poder da antecipação mental:

> *"A sua mente deve chegar sempre antes do seu corpo. Quem só acredita quando vê já chegou atrasado. Você precisa fabricar na imaginação o estado desejado, construir a estrutura na invisibilidade e permitir que a realidade física apenas preencha o molde que você já desenhou."*

No desenvolvimento web tradicional (SPA Client-Side clássico), a maioria dos sites comete o erro inverso:

1. O usuário digita a URL no navegador.
2. O servidor envia de volta um arquivo HTML praticamente vazio:
   ```html
   <body>
     <app-root></app-root> <!-- VAZIO! NADA PARA O USUÁRIO VER -->
     <script src="main.js"></script>
   </body>
   ```
3. O navegador baixa megabytes de JavaScript enquanto o usuário encara uma tela branca.
4. Os robôs de busca do Google e redes sociais passam pela página e encontram um deserto de conteúdo.
5. Só depois que todo o JavaScript é processado é que a tela finalmente "pisca" e aparece.

Isso é o oposto do estado desejado. É forçar o usuário a esperar o corpo da aplicação nascer na frente dele.

A resposta da engenharia moderna do Angular para esse desafio é o **Server-Side Rendering (SSR)** com **Hidratação Não-Destrutiva**.

---

## Como Funciona o Angular SSR

Com o Angular SSR (antigo Angular Universal, hoje integrado nativamente pelo pacote `@angular/ssr`), o fluxo se inverte:

```
[ Usuário requisita a URL ]
             │
             ▼
[ Servidor Node.js ] ──> Renderiza o componente Angular no servidor
             │           Gera o HTML final completo com textos, imagens e CSS
             ▼
[ Navegador recebe HTML completo instantaneamente ]
             │           Tela aparece no 1º frame! (FCP imediato)
             ▼
[ Download do JavaScript em segundo plano ]
             │
             ▼
[ Hidratação Não-Destrutiva ] ──> O Angular "acorda" o HTML já desenhado
                                 conecta cliques e reatividade sem piscar!
```

O usuário não vê tela branca. O conteúdo está visível no instante zero.

---

## Habilitando SSR e Hidratação no Angular 17+

Nas versões modernas do Angular, você pode habilitar o SSR no momento de criar o projeto:

```bash
ng new meu-app-antimedo --ssr
```

Ou adicionar em um projeto já existente:

```bash
ng add @angular/ssr
```

Veja como fica a configuração no `app.config.ts`:

```typescript
// app.config.ts
import { ApplicationConfig } from '@angular/core';
import { provideRouter } from '@angular/router';
import { provideClientHydration } from '@angular/platform-browser';
import { routes } from './app.routes';

export const appConfig: ApplicationConfig = {
  providers: [
    provideRouter(routes),
    // HABILITA A HIDRATAÇÃO NÃO-DESTRUTIVA:
    // O Angular reaproveita os nós do DOM gerados pelo servidor sem recriá-los!
    provideClientHydration()
  ]
};
```

---

## O Cuidado com APIs do Navegador: `isPlatformBrowser`

Quando sua aplicação roda no servidor (Node.js), objetos específicos do navegador como `window`, `document`, `navigator` e `localStorage` **não existem**. Se você tentar acessá-los diretamente, o servidor lançará uma exceção e quebrará a renderização:

```typescript
// ❌ CÓDIGO PERIGOSO EM SSR (QUEBRA NO SERVIDOR NODE)
export class PerfilComponent implements OnInit {
  ngOnInit() {
    // ReferenceError: localStorage is not defined no Node.js!
    const token = localStorage.getItem('token');
  }
}
```

Para proteger o código e garantir que certas ações só rodem no navegador, usamos o token **`PLATFORM_ID`** e a função auxiliar **`isPlatformBrowser`**:

```typescript
// core/utils/armazenamento.service.ts
import { Injectable, inject, PLATFORM_ID } from '@angular/core';
import { isPlatformBrowser, isPlatformServer } from '@angular/common';

@Injectable({ providedIn: 'root' })
export class ArmazenamentoSeguroService {
  private platformId = inject(PLATFORM_ID);

  obterItem(chave: string): string | null {
    // Só tenta ler o localStorage se estivermos rodando no navegador real!
    if (isPlatformBrowser(this.platformId)) {
      return localStorage.getItem(chave);
    }

    // Se estivermos no servidor, retorna valor seguro padrão
    console.log('[SSR]: Execução no servidor — ignorando localStorage');
    return null;
  }
}
```

---

## Evitando Requisições Duplicadas com `TransferState`

Pense comigo: se o servidor fez uma chamada HTTP para buscar a lista de desafios para renderizar o HTML inicial, faz sentido o navegador fazer a mesma chamada HTTP de novo quando o JavaScript acordar?

**Não! Isso seria desperdício de banda e tempo.**

O Angular resolve isso com o **`TransferState`**: os dados buscados no servidor são embutidos como um JSON invisível no rodapé do HTML. Quando o cliente acorda, ele lê os dados de lá diretamente, poupando uma requisição ao backend!

```typescript
import { Injectable, inject, makeStateKey, TransferState } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { tap } from 'rxjs/operators';

const DESAFIOS_KEY = makeStateKey<any[]>('lista-desafios');

@Injectable({ providedIn: 'root' })
export class DesafiosSsrService {
  private http = inject(HttpClient);
  private transferState = inject(TransferState);

  carregarDesafios(): Observable<any[]> {
    // 1. Se o dado já veio do servidor pelo TransferState, usa ele direto!
    if (this.transferState.hasKey(DESAFIOS_KEY)) {
      const dadosDoServidor = this.transferState.get(DESAFIOS_KEY, []);
      this.transferState.remove(DESAFIOS_KEY); // Limpa a chave
      return of(dadosDoServidor);
    }

    // 2. Senão, faz a requisição e guarda no estado
    return this.http.get<any[]>('https://api.antimedo.dev/desafios').pipe(
      tap(dados => this.transferState.set(DESAFIOS_KEY, dados))
    );
  }
}
```

---

::: exercicio Exercício: Verificador de Plataforma
Implemente um componente que exiba no template a mensagem `"Renderizado no Servidor (Node.js)"` se estiver em SSR ou `"Renderizado no Cliente (Navegador)"` se estiver no browser.

--- resposta
```typescript
import { Component, inject, PLATFORM_ID } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';

@Component({
  selector: 'app-origem-render',
  standalone: true,
  template: `<p class="badge-origem">{{ origem }}</p>`
})
export class OrigemRenderComponent {
  private platformId = inject(PLATFORM_ID);

  origem = isPlatformBrowser(this.platformId)
    ? 'Renderizado no Cliente (Navegador)'
    : 'Renderizado no Servidor (Node.js)';
}
```
:::

---

::: quiz
? Qual a principal vantagem do Server-Side Rendering (SSR) em relação a SPAs tradicionais?
+ O HTML chega completo ao navegador, oferecendo First Contentful Paint imediato e excelente indexação no Google (SEO)
- Ele dispensa a necessidade de programar em TypeScript
- Ele substitui os bancos de dados por planilhas Excel
- Ele torna o aplicativo compatível com computadores sem internet
> O SSR entrega o HTML pronto logo no primeiro pacote de rede, melhorando performance e SEO.

? O que significa "Hidratação Não-Destrutiva" no Angular?
+ O Angular reutiliza a estrutura DOM gerada pelo servidor, apenas anexando ouvintes de eventos sem recriar os elementos
- O computador do usuário precisa ser resfriado a água
- Todos os arquivos CSS são apagados para economizar espaço
- Os componentes são recriados 10 vezes para garantir estabilidade
> A hidratação conecta a lógica reativa do Angular ao HTML existente de forma cirúrgica e sem piscadas.

? Por que o código que usa `window` ou `localStorage` quebra no ambiente de SSR?
+ Porque esses objetos são exclusivos do navegador e não existem no ambiente de execução do Node.js
- Porque o TypeScript não aceita palavras em inglês
- Porque o Google descontinuou o suporte a navegadores
- Porque a memória RAM do servidor é apagada a cada requisição
> No servidor Node.js não há navegador (`window`, `localStorage`), exigindo verificações com `isPlatformBrowser`.

? Qual função do Angular é usada para verificar se o código está executando no navegador real?
+ isPlatformBrowser(platformId)
- isChrome()
- checkWebStatus()
- hasWindow()
> A função `isPlatformBrowser(this.platformId)` avalia o token injetado e retorna um booleano confiável.

? Para que serve a API `TransferState` no Angular SSR?
+ Para transportar dados buscados no servidor para o cliente via HTML, evitando chamadas HTTP repetidas
- Para transferir dinheiro entre contas bancárias
- Para mudar a rota da página sem usar o teclado
- Para converter imagens PNG em JPG
> `TransferState` compartilha o estado gerado no servidor com o cliente, prevenindo requisições duplicadas.
:::

---

::: cartas
Estado Desejado :: Construir a realidade na antecipação antes que o corpo precise alcançá-la fisicamente.
Server-Side Rendering (SSR) :: Processo de compilar e renderizar os componentes Angular em HTML completo no servidor.
Hidratação :: Processo no qual o JavaScript do Angular se anexa ao HTML pré-renderizado no cliente.
PLATFORM_ID :: Token de injeção que identifica se o código está rodando em servidor (server) ou navegador (browser).
isPlatformBrowser :: Utilitário para condicionar chamadas de APIs exclusivas do navegador (window, storage).
TransferState :: Mecanismo para embutir dados do servidor no HTML e impedir requisições duplicadas no boot.
:::

::: resumo
Pablo Marçal ensina que sua mente deve habitar o estado desejado antes mesmo do primeiro passo. No Angular, o Server-Side Rendering é essa personificação exata: o usuário recebe a interface pronta, estruturada e vitoriosa antes mesmo do JavaScript precisar acordar no navegador.
:::
