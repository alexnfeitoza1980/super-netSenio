---
id: ang-cnv-cap30
order: 349
kind: capitulo
num: 247
group: Angular · Parte 62 · Bloco IV — Cura e Transformação Técnica
title: O Uso Protetor da Força
subtitle: Route Guards (CanActivate, CanDeactivate), DomSanitizer e Segurança XSS
minutes: 30
hue: 32
badge: 🏰
image: img/ang-cnv/cap30.svg
- Analisar a distinção ética entre o "uso protetor da força" e o "uso punitivo da força" na CNV: 
- Implementar guardas de rota funcionais modernos no Angular: CanActivateFn, CanDeactivateFn e CanMatchFn
- Utilizar withComponentInputBinding() para mapear parâmetros de rota diretamente em inputs: 
- Compreender a blindagem imunológica contra ataques XSS com DomSanitizer e SecurityContext: 
origem: angular/livros/02-cnv-angular/content/30-forca-para-proteger.md
---

::: ilustracao img/ang-cnv/cap30.svg | Guarda de Rota canDeactivate interceptando a saída acidental do usuário com diálogo protetor e DomSanitizer neutralizando injeções XSS sem hostilidade.
:::

No décimo primeiro capítulo de *Comunicação Não-Violenta*, Marshall Rosenberg enfrenta uma questão prática incontornável: **o que fazer quando o diálogo pacífico não é possível ou quando há perigo iminente de dano ou destruição?**

Marshall esclarece que, em um mundo imperfeito, existem situações de emergência em que não há tempo para conversar:
- Quando uma criança pequena solta a mão dos pais e corre em direção a uma rodovia movimentada cheia de caminhões em alta velocidade, nós não nos sentamos no meio-fio para perguntar: *"Filho, quais são os seus sentimentos e necessidades que estão levando você a correr para a pista?"*.
- Nós corremos, usamos nossa força física e **agarramos a criança com firmeza pelo braço**.

Isso é força? Sim, indiscutivelmente. Mas Marshall nos ensina a olhar para **a intenção que move a força**:

```
+---------------------------------------------------------------------------------------------------------+
|                                  USO PROTETOR VS USO PUNITIVO DA FORÇA                                  |
+----------------------------------------------------+----------------------------------------------------+
| USO PROTETOR DA FORÇA (COMPASSIVO)                 | USO PUNITIVO DA FORÇA (VIOLENTO)                   |
+----------------------------------------------------+----------------------------------------------------+
| • A intenção única é PREVENIR danos ou ferimentos. | • A intenção é fazer a pessoa SOFRER para aprender.|
| • Foca exclusivamente na proteção da vida.         | • Foca na culpa, vingança e retaliação moral.      |
| • Não humilha, não insulta, não agride.            | • Condena o indivíduo como "mau" ou "pecador".     |
| • Gera alívio, segurança e preservação.            | • Gera ressentimento, rebeldia e hostilidade futura|
+----------------------------------------------------+----------------------------------------------------+
```

Quando você agarra a criança na rua com o uso protetor da força, você a protege do caminhão. No segundo seguinte, você a abraça, aliviado por sua vida ter sido preservada. No uso punitivo, a pessoa daria palmadas na criança gritando: *"Você é um pestinha desobediente, tome isso para aprender!"*. A dor infligida não tem relação com o perigo; ela busca vingar o medo do adulto.

---

## A Força Protetora no Angular: Route Guards e Proteção de Dados

Na arquitetura de software com Angular em [angular.dev/overview](https://angular.dev/overview), o sistema precisa exercer autoridade sobre certas transições do usuário:

1. **Proteção contra Perda Acidental**: impedir que alguém feche uma aba e perca duas horas de preenchimento de um documento importante (**`CanDeactivateFn`**).
2. **Proteção de Espaços Restritos**: impedir o acesso a áreas confidenciais sem credenciais válidas, sem humilhar ou punir o visitante (**`CanActivateFn`**).
3. **Proteção contra Injeção Maliciosa**: desarmar scripts perigosos inseridos em caixas de comentários sem corromper a mensagem genuína do usuário (**`DomSanitizer`**).

---

## 1. canDeactivate: Protegendo o Trabalho do Usuário

O guarda funcional `CanDeactivateFn` do Angular Router atua como o abraço protetor: ele impede a transição de rota até confirmar se o usuário está ciente do risco de perda:

```typescript
// src/app/core/guards/proteger-formulario.guard.ts
import { CanDeactivateFn } from '@angular/router';

export interface FormularioComSalvaPendente {
  temAlteracoesNaoSalvas(): boolean;
}

export const protegerFormularioGuard: CanDeactivateFn<FormularioComSalvaPendente> = (
  componente,
  currentRoute,
  currentState,
  nextState
) => {
  // Se não houver risco de perda, a passagem é livre e pacífica
  if (!componente.temAlteracoesNaoSalvas()) {
    return true;
  }

  // Uso protetor da força: solicita confirmação antes de permitir o dano irreversível
  return confirm(
    'Identificamos reflexões ainda não salvas neste formulário.\n\n' +
    'Se sair agora, seus pensamentos serão perdidos.\n' +
    'Deseja realmente sair sem salvar?'
  );
};
```

---

## 2. canActivate e canMatch: Protegendo Espaços Sagrados

O guarda funcional `CanActivateFn` protege rotas confidenciais redirecionando gentilmente o usuário desautenticado, sem mensagens acusatórias:

```typescript
// src/app/core/guards/autenticacao-compassiva.guard.ts
import { CanActivateFn, Router } from '@angular/router';
import { inject } from '@angular/core';
import { SessaoUsuarioService } from '../services/sessao-usuario.service';

export const autenticacaoCompassivaGuard: CanActivateFn = (route, state) => {
  const sessao = inject(SessaoUsuarioService);
  const router = inject(Router);

  if (sessao.estaAutenticado()) {
    return true; // Acesso fraternal garantido
  }

  console.info(`[Guarda Protetor] Redirecionando para login a partir de: ${state.url}`);

  // Redireciona com preservação serena do destino original para restauração posterior
  return router.createUrlTree(['/entrar'], {
    queryParams: { destinoOriginal: state.url }
  });
};
```

---

## 3. Roteamento Moderno com withComponentInputBinding()

No Angular moderno, os parâmetros de rota (parâmetros de caminho como `:idTopico` e parâmetros de consulta como `?origem=dashboard`) não precisam mais de subscrições complexas a `ActivatedRoute.params`.

Com **`withComponentInputBinding()`**, o roteador injeta os parâmetros de rota diretamente nos `input()` do componente:

```typescript
// Configuração de rotas no app.config.ts
import { provideRouter, withComponentInputBinding, withViewTransitions, Routes } from '@angular/router';
import { autenticacaoCompassivaGuard } from './core/guards/autenticacao-compassiva.guard';
import { protegerFormularioGuard } from './core/guards/proteger-formulario.guard';

export const routes: Routes = [
  {
    path: 'dialogos/:idTopico',
    canActivate: [autenticacaoCompassivaGuard],
    canDeactivate: [protegerFormularioGuard],
    loadComponent: () => import('./features/dialogo/dialogo-detalhe.component')
  }
];

export const appConfig = {
  providers: [
    provideRouter(
      routes,
      withComponentInputBinding(), // Transforma :idTopico em input('idTopico') automaticamente!
      withViewTransitions()        // Transições visuais suaves e cinemáticas
    )
  ]
};
```

No componente de destino:

```typescript
import { Component, input } from '@angular/core';

@Component({
  selector: 'app-dialogo-detalhe',
  standalone: true,
  template: `<h3>Visualizando Diálogo ID: {{ idTopico() }}</h3>`
})
export default class DialogoDetalheComponent {
  // Injetado automaticamente pelo roteador!
  readonly idTopico = input.required<string>();
}
```

---

## 4. DomSanitizer: O Escudo contra Ataques XSS

Quando permitimos que usuários insiram formatações de texto ou HTML (como em um editor de rascunhos de conciliação), pessoas mal-intencionadas podem tentar injetar scripts executáveis maliciosos (**Cross-Site Scripting - XSS**).

Por padrão, **o compilador e o runtime do Angular consideram todo valor não confiável**. Quando você utiliza a diretiva `[innerHTML]`, o Angular inspeciona e higieniza a string automaticamente, desarmando o código perigoso:

```typescript
import { Component, inject, SecurityContext } from '@angular/core';
import { DomSanitizer } from '@angular/platform-browser';

@Component({
  selector: 'app-mensagem-comunitaria',
  standalone: true,
  template: `
    <div class="comentario-card">
      <h5>Relato da Comunidade:</h5>
      <!-- O Angular higieniza a string preservando a formatação segura -->
      <div [innerHTML]="conteudoSeguro"></div>
    </div>
  `
})
export class MensagemComunitariaComponent {
  private readonly sanitizer = inject(DomSanitizer);

  // Exemplo de texto vindo da internet com tentativa de injeção XSS
  private readonly relatoCru = `
    Agradeço a todos pela paciência!
    <script>alert('Tentativa de roubo de sessão!');</script>
    <img src="invalido" onerror="console.log('Tentativa de ataque');" />
    <strong>Juntos construímos um ambiente seguro.</strong>
  `;

  // Sanitização protetora: neutraliza scripts sem destruir a mensagem de carinho
  get conteudoSeguro(): string {
    return this.sanitizer.sanitize(SecurityContext.HTML, this.relatoCru) ?? '';
  }
}
```

::: nota
O Angular remove o `<script>` e o atributo malicioso `onerror`, mas mantém intactas a formatação `<strong>` e a mensagem de gratidão! Isso é a essência do **uso protetor da força**: neutralizar o dano sem aniquilar ou humilhar o outro.
:::

---

::: quiz
? Qual é a diferença fundamental entre o uso protetor da força e o uso punitivo da força na CNV?
- O uso protetor da força requer armas de fogo
+ O uso protetor tem como único objetivo prevenir um dano físico ou injustiça iminente sem intenção de infligir sofrimento; o uso punitivo busca fazer o outro sofrer por julgar que ele merece castigo
- O uso punitivo funciona muito melhor na educação de crianças
- O uso protetor da força é proibido pela lei
> Marshall ensina que a intenção da força protetora é a preservação da integridade da vida, acompanhada de alívio e acolhimento posterior, e não de vingança ou humilhação.

? Qual guarda funcional do Angular Router impede a perda acidental de alterações em um formulário ao navegar para fora da página?
- `CanActivateFn`
- `CanMatchFn`
+ `CanDeactivateFn`
- `ResolveFn`
> O guarda `CanDeactivateFn` é invocado antes que a rota atual seja destruída, permitindo verificar rascunhos não salvos e solicitar confirmação protetora ao usuário.

? O que a configuração `withComponentInputBinding()` do Angular Router proporciona ao desenvolvedor?
- Acelera a velocidade de download de imagens
+ Mapeia automaticamente parâmetros de rota (path params) e de consulta (query params) diretamente para os Signal Inputs (`input()`) do componente de destino
- Desativa todos os guardas de autenticação
- Converte todas as rotas em janelas popup
> `withComponentInputBinding()` elimina a necessidade de injetar `ActivatedRoute` manualmente para subscrever em `paramMap`, integrando o roteador nativamente à nova API de Inputs.

? Como o Angular trata valores inseridos em tags via `[innerHTML]` por padrão?
- Executa todos os scripts imediatamente sem verificar
+ Sanitiza e neutraliza automaticamente códigos executáveis maliciosos (tags script e eventos inline), protegendo a aplicação contra vulnerabilidades XSS
- Converte o HTML em imagens JPEG
- Bloqueia todo o conteúdo da tela permanentemente
> O sistema imunológico do Angular é um dos mais robustos da web: qualquer valor atribuído a `[innerHTML]` passa pelo filtro do `DomSanitizer` no contexto de HTML.

? Por que o redirecionamento de um guarda `CanActivateFn` deve preferencialmente salvar o `destinoOriginal` do usuário?
- Para aplicar uma multa financeira ao usuário
+ Para que, após realizar o login de forma segura, a aplicação possa conduzi-lo diretamente ao recurso que ele desejava acessar, demonstrando respeito pelo seu tempo
- Para bloquear o endereço IP do visitante
- Para forçar o usuário a preencher todo o cadastro novamente
> Em vez de criar barreiras punitivas, a interface compassiva protege a área restrita e oferece um caminho acolhedor e direto de retorno para o usuário.
:::

---

::: cartas
Uso Protetor da Força :: Intervenção firme motivada exclusivamente pela prevenção de danos à vida ou integridade, sem intuito de punição ou humilhação.
Uso Punitivo da Força :: Agressão justificada pela crença moralista de que pessoas más merecem sofrer para aprenderem a lição.
CanDeactivateFn :: Guarda funcional do Angular Router que avalia se a saída de uma rota ativa é segura contra perda acidental de trabalho.
CanActivateFn :: Guarda funcional que autoriza ou redireciona a navegação com base no perfil de acesso do usuário.
withComponentInputBinding :: Recurso do Angular Router que liga parâmetros de rota diretamente aos Signal Inputs do componente.
Cross-Site Scripting (XSS) :: Vulnerabilidade de segurança web em que scripts maliciosos de terceiros são injetados em páginas confiáveis.
DomSanitizer no Angular :: Serviço de plataforma que inspeciona e limpa strings HTML, URLs e estilos antes de renderizá-los no DOM.
SecurityContext :: Enumerador do Angular que define a regra de sanitização apropriada (HTML, STYLE, SCRIPT, URL, RESOURCE_URL).
:::
