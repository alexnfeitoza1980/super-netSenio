---
id: ang-pratica-cap11
order: 286
kind: capitulo
num: 196
group: Angular · Parte 48 · Dados e integração
title: RxJS, eventos e cancelamento
subtitle: Streams representam sequências no tempo; entender seu ciclo de vida evita respostas antigas e assinaturas esquecidas.
minutes: 55
hue: 210
objectives:
- Entender Observable, subscribe, operadores e cancelamento
- Evitar condições de corrida em busca remota
- Integrar streams a signals com APIs do Angular
origem: angular/livros/01-biblioteca-angular/content/11-cap11.md
---

## Valor único ou sequência?

Uma Promise entrega um resultado futuro ou falha. Um Observable pode entregar vários valores ao longo do tempo, concluir ou falhar. Eventos de teclado, websockets e fluxos HTTP podem ser modelados como streams. Nem todo dado precisa virar Observable: signals são excelentes para estado atual síncrono da interface.

Operadores RxJS transformam sequências. `map` projeta valores, `filter` seleciona, `debounceTime` espera uma pausa entre eventos, `switchMap` troca para a operação mais recente, `catchError` trata falhas num fluxo definido. Cada operador muda semântica; não os empilhe sem saber o que fazem.

```ts
import { debounceTime, distinctUntilChanged, map, switchMap } from 'rxjs';

const resultados$ = campoBusca.valueChanges.pipe(
  map((termo) => termo.trim()),
  debounceTime(250),
  distinctUntilChanged(),
  switchMap((termo) => repositorio.buscar(termo)),
);
```

Esse padrão é útil quando a busca realmente consulta um servidor; para o catálogo local do protótipo, o signal e `computed` são menores e mais simples. `switchMap` não serve universalmente: para eventos de escrita, ignorar ou cancelar uma operação pode ser incorreto.

## A busca remota e a resposta velha

Se uma pessoa digita “ma” e logo depois “map”, duas requisições podem terminar fora de ordem. `switchMap` cancela a inscrição anterior para que a resposta antiga não sobrescreva a nova. A ordem importa: debounce pode reduzir chamadas; `distinctUntilChanged` evita repetir o mesmo termo.

O `AsyncPipe` administra inscrição e cancelamento no template. `toSignal` transforma um Observable em signal com uma inscrição vinculada ao contexto de injeção. Para fluxo inverso, `toObservable` observa um signal. Não crie inscrição manual sem plano para encerrar.

::: cuidado Cancelar não desfaz tudo
Cancelar a assinatura no cliente não garante que o servidor cancelou ou desfez uma operação. Para comandos de escrita, desenhe idempotência e estado da operação no backend.
:::

::: quiz
? Qual operador é adequado para manter apenas a busca remota mais recente?
+ `switchMap`, porque troca a assinatura interna pela operação do novo termo.
- `mergeMap`, porque sempre garante ordem de resposta.
- `subscribe`, porque ele cancela todas as inscrições anteriores automaticamente.
:::

::: cartas
Quando signals são uma boa escolha? :: Para estado atual síncrono e derivação reativa da interface.
O que `AsyncPipe` faz com a assinatura? :: Gerencia a inscrição no template e a encerra quando a view é destruída.
:::
