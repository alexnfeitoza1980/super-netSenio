---
id: ang-cnv-cap26
order: 345
kind: capitulo
num: 243
group: Angular · Parte 61 · Bloco III — Empatia na Prática
title: Receber com Empatia
subtitle: Streams de Escuta Ativa, toSignal(), debounceTime e takeUntilDestroyed
minutes: 30
hue: 32
badge: 👂
image: img/ang-cnv/cap26.svg
- Compreender a máxima "Não faça nada, apenas fique presente" ensinada por Marshall Rosenberg: 
- Reconhecer os 7 comportamentos cotidianos que destroem a empatia (aconselhar, consolar, competir): 
- Dominar operadores RxJS de escuta calma: debounceTime, distinctUntilChanged e filter
- Integrar Observables com Signals usando toSignal() e limpeza com takeUntilDestroyed(): 
origem: angular/livros/02-cnv-angular/content/26-receber-com-empatia.md
---

::: ilustracao img/ang-cnv/cap26.svg | Visualizador de Streams RxJS: linha do tempo demonstrando a digitação rápida sendo acolhida pela janela de silêncio respeitoso de debounceTime(300ms) gerando uma única chamada serena para a API.
:::

No sétimo capítulo de *Comunicação Não-Violenta*, Marshall Rosenberg chega à outra face da mesma moeda: depois de aprender a nos expressar do fundo do coração, precisamos aprender a **receber o outro com empatia**.

Marshall cita um ensinamento budista milenar que se choca frontalmente contra a nossa cultura corporativa ocidental obcecada por consertos rápidos:
> *"Não faça nada; apenas fique presente!"*

A empatia requer **presença plena**. Requer silenciar o nosso diálogo interno, esvaziar a mente de julgamentos prévios e colocar toda a nossa atenção nos sentimentos e necessidades da outra pessoa.

### Os 7 Comportamentos que Destroem a Empatia

Marshall adverte que, quando alguém vem até nós vulnerável compartilhando uma angústia ou dor, nossa tendência quase irresistível é cometer uma das seguintes violências conversacionais:

1. **Aconselhar precipitadamente**: *"Acho que você devia falar com seu chefe e pedir um aumento..."*
2. **Competir pelo sofrimento**: *"Você acha isso ruim? Deixa eu te contar o desastre que aconteceu comigo ontem!"*
3. **Educar e filosofar**: *"Toda dificuldade na vida é uma oportunidade de aprendizado espiritual..."*
4. **Consolar superficialmente**: *"Não fique assim, vai passar, não foi nada de mais..."*
5. **Contar histórias próprias**: *"Isso me lembra uma vez em 2018 quando eu..."*
6. **Encerrar o assunto com otimismo tóxico**: *"Anime-se! Veja pelo lado positivo, poderia ter sido pior!"*
7. **Fazer interrogatório policial**: *"Quando isso começou exatamente? Quem estava na sala? Por que você disse aquilo?"*

Nenhuma dessas respostas é empatia! Elas são manobras defensivas para **nos livrarmos do desconforto de presenciar a dor alheia**. 

A empatia autêntica simplesmente escuta:
> *"Você está se sentindo exausto porque precisa desesperadamente de descanso e consideração pelo seu esforço?"*

---

## A Ansiedade no Frontend: O Antipattern da Interrupção Tecnológica

Nas interfaces web, a falta de escuta empática se manifesta como **ansiedade tecnológica**:

- O usuário começa a digitar no campo de busca: `"c"`.
- A aplicação dispara uma requisição de rede imediata para `/api/busca?q=c`.
- O usuário digita a segunda letra: `"co"`.
- Outra requisição é disparada: `/api/busca?q=co`.
- O usuário digita a terceira letra: `"com"`.
- A tela pisca, o teclado perde o foco por um milissegundo e um alerta grita: *"NENHUM RESULTADO ENCONTRADO!"*, antes mesmo que a pessoa tenha tido o tempo de concluir a palavra `"compaixão"`.

Isso é o equivalente digital de **interromper a fala da pessoa a cada sílaba para dar conselhos desastrados**. A aplicação não sabe escutar com paciência.

---

## O Arsenal de Escuta Ativa do RxJS (angular.dev)

O ecossistema **RxJS** no Angular é a ferramenta definitiva para modelar a escuta respeitosa e a presença serena:

```
[ Usuário digita rapidamente: "c" ➔ "co" ➔ "com" ➔ "compaixão" ]
                          │
                          ▼
            [ debounceTime(300ms) ]  ➔ A Janela de Silêncio Respeitoso
                          │
                          ▼
         [ distinctUntilChanged() ]  ➔ Discernimento: ignora repetições vazias
                          │
                          ▼
          [ filter(termo.length >= 2) ]  ➔ Respeito ao contexto mínimo
                          │
                          ▼
         [ switchMap(api.buscar) ]  ➔ UMA Única Chamada Serena à API
```

---

## Integrando RxJS e Signals: toSignal() e takeUntilDestroyed()

No Angular moderno em [angular.dev/overview](https://angular.dev/overview), não precisamos escolher entre RxJS e Signals: nós os integramos com maestria através do pacote `@angular/core/rxjs-interop`:

- **`toSignal(observable$, options)`**: converte uma stream reativa em um Signal síncrono limpo para uso direto no template com `@for` ou `@if`.
- **`takeUntilDestroyed()`**: encerra e desinscreve o canal de escuta automaticamente quando o componente for destruído, eliminando dezenas de linhas de código boilerplate em `ngOnDestroy`.

Vejamos uma implementação standalone de busca empática de tópicos:

```typescript
// src/app/features/busca-dialogo/busca-empatica.component.ts
import { Component, inject } from '@angular/core';
import { FormControl, ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { debounceTime, distinctUntilChanged, filter, switchMap, tap, catchError } from 'rxjs/operators';
import { of } from 'rxjs';
import { toSignal, takeUntilDestroyed } from '@angular/core/rxjs-interop';

export interface TopicoDialogo {
  readonly id: string;
  readonly titulo: string;
  readonly necessidadeSubjacente: string;
}

@Component({
  selector: 'app-busca-empatica',
  standalone: true,
  imports: [ReactiveFormsModule, CommonModule],
  template: `
    <section class="container-escuta">
      <header>
        <h3>Centro de Escuta Ativa</h3>
        <p>Digite seus sentimentos ou reflexões. Ouvimos com calma e presença plena.</p>
      </header>

      <div class="campo-pesquisa">
        <input
          type="text"
          [formControl]="buscaControl"
          placeholder="Procure por temas (ex: escuta, clareza, limites)..."
          class="input-escuta"
        />
        
        @if (estaEscutando()) {
          <span class="indicador-escuta" aria-live="polite">
            👂 Ouvindo com atenção plena...
          </span>
        }
      </div>

      <!-- Consumo direto do Signal derivado via toSignal() -->
      <div class="resultados-bloco">
        <ul class="lista-resultados">
          @for (item of topicos(); track item.id) {
            <li class="card-resultado">
              <strong>{{ item.titulo }}</strong>
              <span class="tag-necessidade">Necessidade: {{ item.necessidadeSubjacente }}</span>
            </li>
          } @empty {
            @if (buscaControl.value.length >= 2 && !estaEscutando()) {
              <li class="aviso-vazio">
                Acolhemos sua busca por "{{ buscaControl.value }}", mas ainda não temos tópicos catalogados com este termo.
              </li>
            }
          }
        </ul>
      </div>
    </section>
  `,
  styles: [`
    .container-escuta { background: #fffaf5; border: 1px solid #f5dfd0; border-radius: 14px; padding: 2rem; max-width: 640px; }
    .input-escuta { width: 100%; padding: 0.85rem 1rem; border: 1px solid #f5dfd0; border-radius: 8px; font-size: 1rem; box-sizing: border-box; }
    .input-escuta:focus { outline: 2px solid #d96b27; border-color: #d96b27; }
    .indicador-escuta { display: block; font-size: 0.85rem; color: #d96b27; margin-top: 0.5rem; font-style: italic; }
    .lista-resultados { list-style: none; padding: 0; margin-top: 1.5rem; display: flex; flex-direction: column; gap: 0.75rem; }
    .card-resultado { background: white; border: 1px solid #f5dfd0; border-radius: 8px; padding: 1rem; display: flex; justify-content: space-between; align-items: center; }
    .tag-necessidade { background: #fef3c7; color: #92400e; font-size: 0.8rem; padding: 0.25rem 0.6rem; border-radius: 4px; font-weight: 600; }
    .aviso-vazio { color: #6c3c1e; font-style: italic; padding: 1rem 0; }
  `]
})
export class BuscaEmpaticaComponent {
  private readonly http = inject(HttpClient);

  // Controle de formulário reativo
  readonly buscaControl = new FormControl('', { nonNullable: true });

  // Signal booleano indicando o momento da escuta ativa
  readonly estaEscutando = signal(false);

  // 1. Pipeline Reativo de Escuta Ativa
  private readonly topicos$ = this.buscaControl.valueChanges.pipe(
    takeUntilDestroyed(), // Garante limpeza automática na destruição do componente
    tap(() => this.estaEscutando.set(true)),
    debounceTime(350), // Pausa generosa: respeita o tempo de reflexão de quem digita
    distinctUntilChanged(), // Ignora se o termo final não foi alterado
    tap(() => this.estaEscutando.set(false)),
    filter(termo => termo.trim().length >= 2 || termo.trim().length === 0),
    switchMap(termo => {
      if (!termo.trim()) return of([]);
      return this.http.get<TopicoDialogo[]>(`/api/topicos?q=${encodeURIComponent(termo)}`).pipe(
        catchError(() => of([])) // Acolhe falhas de rede com serenidade
      );
    })
  );

  // 2. Ponte Mágica: Converte a stream RxJS em um Signal puro do Angular!
  readonly topicos = toSignal(this.topicos$, { initialValue: [] });
}
```

::: nota
Observe o poder de `toSignal(this.topicos$, { initialValue: [] })`. O template consome `topicos()` como um simples Signal nativo, usufruindo da detecção de mudanças ultrafina do Angular, enquanto a complexidade assíncrona de debounce, cancelamento e requisições HTTP permanece elegantemente encapsulada na stream do RxJS.
:::

---

::: quiz
? Qual é a recomendação de Marshall Rosenberg e de ensinamentos budistas diante da dor ou vulnerabilidade alheia?
- Dizer imediatamente o que a pessoa deve fazer para consertar
+ "Não faça nada, apenas fique presente!" — sustentando a escuta empática com todo o nosso ser antes de qualquer ação
- Mudar de assunto para algo mais engraçado
- Explicar detalhadamente como você já sofreu muito mais no passado
> Marshall demonstra que a maior dádiva que podemos oferecer a quem sofre é nossa presença atenta. Conselhos precipitados costumam ser manobras para aliviar nosso próprio desconforto.

? Quais são alguns dos comportamentos cotidianos que bloqueiam a empatia segundo a CNV?
- Beber água e respirar fundo
+ Aconselhar precipitadamente, competir pelo sofrimento, filosofar/educar, consolar superficialmente e fazer interrogatórios policiais
- Sorrir e dar bom dia
- Escutar em silêncio
> Quando saltamos para educar ("tudo na vida é lição") ou competir ("isso não é nada, veja o meu caso"), roubamos o espaço do outro e interrompemos o fluxo de conexão humana.

? Qual é o papel da função `toSignal()` do pacote `@angular/core/rxjs-interop`?
- Converter sinais de rádio em código JavaScript
+ Converter um Observable do RxJS em um Signal nativo do Angular, permitindo consumo reativo síncrono no template com valor inicial garantido
- Desativar todos os operadores do RxJS
- Excluir o arquivo de teste unitário
> `toSignal()` é a ponte oficial do Angular moderno entre o universo assíncrono avançado do RxJS e a reatividade granular e ergonômica dos Signals no template.

? Como o operador `distinctUntilChanged()` contribui para a escuta empática no código?
- Formatando o texto para maiúsculas
+ Evitando disparar requisições ou processamentos repetidos se o valor emitido for idêntico ao imediatamente anterior
- Apagando o histórico de pesquisas
- Fechando a janela do navegador
> `distinctUntilChanged()` discerne o ruído do sinal real: se o usuário pressionou teclas de controle (como setas ou Shift) sem alterar o texto de fato, o sistema não reage desnecessariamente.

? Por que o operador `takeUntilDestroyed()` eliminou a necessidade de implementar `ngOnDestroy` para desinscrições manuais?
- Porque ele desativa a destruição de componentes
+ Porque ele amarra automaticamente o ciclo de vida da subscrição ao contexto de injeção atual, cancelando o fluxo assim que o componente morre
- Porque ele faz o garbage collector do JavaScript rodar a cada 1 segundo
- Porque ele reinicia a aplicação a cada erro
> `takeUntilDestroyed()` resolve o problema clássico de vazamentos de memória (*memory leaks*) no Angular de forma puramente declarativa e segura.
:::

---

::: cartas
Receber com Empatia :: Esvaziar a mente de julgamentos para ouvir os sentimentos e necessidades do outro com presença total e respeito.
Presença Plena na CNV :: "Não faça nada, apenas fique presente" — acolher o outro sem a ansiedade de consertar ou aconselhar de imediato.
7 Bloqueadores de Empatia :: Aconselhar, competir pelo sofrimento, educar, consolar superficialmente, contar histórias, encerrar com otimismo tóxico e interrogar.
debounceTime (RxJS) :: Janela temporal de silêncio respeitoso que aguarda o usuário concluir sua fala antes de emitir o valor final.
distinctUntilChanged :: Operador do RxJS que ignora valores consecutivos idênticos, prevenindo ruído no processamento.
toSignal() :: Utilitário oficial do Angular para converter fluxos Observables em Signals nativos para templates.
takeUntilDestroyed :: Função do Angular 16+ que desinscreve automaticamente fluxos reativos na destruição do contexto do componente.
Memory Leak no Frontend :: Vazamento de recursos decorrente de subscrições abertas em Observables que continuam vivas após a destruição da tela.
:::
