---
id: ang-antimedo-lab1
order: 320
kind: lab
num: 46
group: Angular · Parte 56 · Laboratório Prático
title: Laboratório: A Aplicação Antimedo Completa
subtitle: Construindo um sistema reativo ponta a ponta com Angular 17+
minutes: 90
hue: 340
badge: 🛠️
- Integrar todos os conceitos da coleção em um único projeto Angular moderno: 
- Implementar formulários reativos, Signals, Services e Pipes em harmonia: 
- Construir um fluxo completo de cadastro, filtragem, enfrentamento e persistência: 
- Criar um painel de métricas analíticas calculado automaticamente com computed(): 
origem: angular/livros/01-antimedo-angular/content/20-lab.md
---

::: ilustracao img/ang-antimedo/lab1.svg | Uma central de controle futurista onde todos os medos cadastrados viram missões táticas concluídas.
:::

Chegou o momento da consolidação. Você não é mais um observador passivo da documentação; agora você é o arquiteto do seu próprio ecossistema de software.

Neste laboratório final, nós vamos construir a aplicação **Antimedo Hub**: uma plataforma completa que permite ao usuário:

1. **Catalogar Medos e Bloqueios:** Formulário reativo com validação de dados reais (descrição, categoria, nível de paralisia de 1 a 10).
2. **Avaliação Sinal vs. Ruído:** Classificação analítica de cada medo via Pipe customizado.
3. **Plano de Ação Tático:** Registro de ações de enfrentamento com datas de execução.
4. **Estado Reativo com Signals:** Contadores em tempo real de desafios vencidos, taxa de coragem e nível de clareza mental calculados automaticamente via `computed()`.
5. **Persistência Segura em Storage:** Dados preservados no navegador sem perda de estado entre recarregamentos.

Vamos construir o projeto passo a passo, arquivo por arquivo.

---

::: passos O Guia de Construção da Aplicação Antimedo
### Passo 1: O Modelo de Dados do Domínio
Crie a pasta `src/app/core/models/` e o arquivo `medo.model.ts`. Este arquivo define as estruturas TypeScript que blindam nosso domínio contra inconsistências:

```typescript
// src/app/core/models/medo.model.ts
export type CategoriaMedo = 'TECNICO' | 'PROFISSIONAL' | 'PESSOAL' | 'FINANCEIRO';

export interface AcaoEnfrentamento {
  id: string;
  descricao: string;
  concluida: boolean;
  dataLimite: string;
}

export interface Medo {
  id: string;
  titulo: string;
  categoria: CategoriaMedo;
  nivelParalisia: number; // 1 a 10
  ehRuido: boolean;       // true se for mera opinião alheia, false se for desafio real
  vencido: boolean;
  acoes: AcaoEnfrentamento[];
  criadoEm: string;
}
```
===
### Passo 2: O Serviço Reativo Central (Signals + LocalStorage)
Crie o arquivo `src/app/core/services/medo.service.ts`. O serviço utiliza **Angular Signals** (`signal`, `computed`) para fornecer um estado reativo ultrarrápido sem necessidade de subscrições manuais:

```typescript
// src/app/core/services/medo.service.ts
import { Injectable, signal, computed, effect } from '@angular/core';
import { Medo, CategoriaMedo } from '../models/medo.model';

const STORAGE_KEY = 'antimedo_registros_v1';

@Injectable({ providedIn: 'root' })
export class MedoService {
  // Sinal principal contendo a lista imutável de medos
  private medosSignal = signal<Medo[]>(this.carregarDoStorage());

  // Sinal público somente-leitura
  public medos = this.medosSignal.asReadonly();

  // Métricas computadas automaticamente em tempo real!
  public totalMedos = computed(() => this.medosSignal().length);
  
  public totalVencidos = computed(() => 
    this.medosSignal().filter(m => m.vencido).length
  );

  public percentualCoragem = computed(() => {
    const total = this.totalMedos();
    if (total === 0) return 100;
    return Math.round((this.totalVencidos() / total) * 100);
  });

  public ruidosDetectados = computed(() => 
    this.medosSignal().filter(m => m.ehRuido && !m.vencido).length
  );

  constructor() {
    // Efeito colateral: toda vez que o sinal mudar, salva automaticamente no localStorage!
    effect(() => {
      const dados = this.medosSignal();
      try {
        localStorage.setItem(STORAGE_KEY, JSON.stringify(dados));
      } catch (e) {
        console.warn('Não foi possível salvar no storage local:', e);
      }
    });
  }

  adicionar(medo: Omit<Medo, 'id' | 'vencido' | 'criadoEm'>): void {
    const novo: Medo = {
      ...medo,
      id: crypto.randomUUID(),
      vencido: false,
      criadoEm: new Date().toISOString()
    };
    this.medosSignal.update(lista => [novo, ...lista]);
  }

  alternarVencido(id: string): void {
    this.medosSignal.update(lista =>
      lista.map(m => m.id === id ? { ...m, vencido: !m.vencido } : m)
    );
  }

  remover(id: string): void {
    this.medosSignal.update(lista => lista.filter(m => m.id !== id));
  }

  private carregarDoStorage(): Medo[] {
    try {
      const salvo = localStorage.getItem(STORAGE_KEY);
      if (salvo) return JSON.parse(salvo);
    } catch {}
    // Dados iniciais de boas-vindas caso o storage esteja vazio
    return [
      {
        id: '1',
        titulo: 'Medo de fazer deploy em produção na sexta',
        categoria: 'TECNICO',
        nivelParalisia: 8,
        ehRuido: false,
        vencido: false,
        acoes: [],
        criadoEm: new Date().toISOString()
      },
      {
        id: '2',
        titulo: 'Opinião de quem nunca construiu nada na web',
        categoria: 'PROFISSIONAL',
        nivelParalisia: 6,
        ehRuido: true,
        vencido: true,
        acoes: [],
        criadoEm: new Date().toISOString()
      }
    ];
  }
}
```
===
### Passo 3: O Pipe de Diagnóstico Analítico
Crie `src/app/shared/pipes/diagnostico-medo.pipe.ts`. Este pipe transforma o nível de paralisia e a flag de ruído em uma leitura tática instantânea:

```typescript
// src/app/shared/pipes/diagnostico-medo.pipe.ts
import { Pipe, PipeTransform } from '@angular/core';
import { Medo } from '../../core/models/medo.model';

@Pipe({
  name: 'diagnosticoMedo',
  standalone: true
})
export class DiagnosticoMedoPipe implements PipeTransform {
  transform(medo: Medo): string {
    if (medo.vencido) {
      return '🏆 CONQUISTADO: Paralisia Neutralizada!';
    }
    if (medo.ehRuido) {
      return '🔇 MERO RUÍDO: Ignore e corte a fonte de distração.';
    }
    if (medo.nivelParalisia >= 8) {
      return '🚨 DESAFIO CRÍTICO: Quebre em microações de 15 minutos.';
    }
    return '⚡ DESAFIO NORMAL: Encare hoje mesmo com foco total.';
  }
}
```
===
### Passo 4: O Formulário Reativo de Cadastro
Crie `src/app/features/cadastrar-medo/cadastrar-medo.component.ts`. Usamos `ReactiveFormsModule` com validação estrita para impedir o cadastro de pensamentos vazios:

```typescript
// src/app/features/cadastrar-medo/cadastrar-medo.component.ts
import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MedoService } from '../../core/services/medo.service';
import { CategoriaMedo } from '../../core/models/medo.model';

@Component({
  selector: 'app-cadastrar-medo',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  template: `
    <section class="form-container">
      <h3>Cadastrar Novo Bloqueio para Enfrentamento</h3>
      
      <form [formGroup]="form" (ngSubmit)="salvar()">
        <div class="campo">
          <label for="titulo">Qual o nome do medo ou bloqueio?</label>
          <input 
            id="titulo" 
            type="text" 
            formControlName="titulo" 
            placeholder="Ex: Medo de refatorar código legado"
          />
          <span class="erro" *ngIf="form.get('titulo')?.touched && form.get('titulo')?.invalid">
            O nome é obrigatório e precisa de no mínimo 5 caracteres.
          </span>
        </div>

        <div class="linha-dupla">
          <div class="campo">
            <label for="categoria">Categoria de Vida</label>
            <select id="categoria" formControlName="categoria">
              <option value="TECNICO">Técnico / Código</option>
              <option value="PROFISSIONAL">Carreira / Equipe</option>
              <option value="PESSOAL">Pessoal / Emocional</option>
              <option value="FINANCEIRO">Financeiro / Negócios</option>
            </select>
          </div>

          <div class="campo">
            <label for="nivel">Nível de Paralisia (1 a 10): {{ form.get('nivelParalisia')?.value }}</label>
            <input 
              id="nivel" 
              type="range" 
              min="1" 
              max="10" 
              formControlName="nivelParalisia"
            />
          </div>
        </div>

        <div class="campo-check">
          <label>
            <input type="checkbox" formControlName="ehRuido" />
            Isso é apenas <strong>ruído</strong> (opinião alheia, boatos de internet)?
          </label>
        </div>

        <button type="submit" [disabled]="form.invalid" class="btn-cadastrar">
          Registrar Desafio no Radar
        </button>
      </form>
    </section>
  `,
  styles: [`
    .form-container { background: #fff; border: 1px solid #f5d8de; border-radius: 14px; padding: 24px; margin-bottom: 24px; box-shadow: 0 4px 12px rgba(0,0,0,0.04); }
    .campo { display: flex; flex-direction: column; gap: 6px; margin-bottom: 14px; }
    .linha-dupla { display: grid; grid-template-columns: 1fr 1fr; gap: 16px; }
    .campo input[type="text"], .campo select { padding: 10px 14px; border: 1px solid #ddd; border-radius: 8px; font-size: 15px; }
    .erro { color: #cf3f4b; font-size: 12.5px; font-weight: 600; }
    .campo-check { margin: 12px 0 18px; }
    .btn-cadastrar { background: #dd1b44; color: #fff; font-weight: 700; border: none; padding: 12px 24px; border-radius: 8px; cursor: pointer; transition: opacity .2s; }
    .btn-cadastrar:disabled { opacity: 0.5; cursor: not-allowed; }
  `]
})
export class CadastrarMedoComponent {
  private fb = inject(FormBuilder);
  private medoService = inject(MedoService);

  form = this.fb.group({
    titulo: ['', [Validators.required, Validators.minLength(5)]],
    categoria: ['TECNICO' as CategoriaMedo, Validators.required],
    nivelParalisia: [5, [Validators.required, Validators.min(1), Validators.max(10)]],
    ehRuido: [false]
  });

  salvar(): void {
    if (this.form.invalid) return;

    const val = this.form.value;
    this.medoService.adicionar({
      titulo: val.titulo!,
      categoria: val.categoria as CategoriaMedo,
      nivelParalisia: Number(val.nivelParalisia),
      ehRuido: Boolean(val.ehRuido),
      acoes: []
    });

    this.form.reset({
      titulo: '',
      categoria: 'TECNICO',
      nivelParalisia: 5,
      ehRuido: false
    });
  }
}
```
===
### Passo 5: O Painel Principal de Indicadores e Lista Tática
Crie `src/app/features/painel/painel.component.ts`. Este componente consome diretamente os Signals computados e renderiza o dashboard e a lista com botões de ação:

```typescript
// src/app/features/painel/painel.component.ts
import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MedoService } from '../../core/services/medo.service';
import { CadastrarMedoComponent } from '../cadastrar-medo/cadastrar-medo.component';
import { DiagnosticoMedoPipe } from '../../shared/pipes/diagnostico-medo.pipe';

@Component({
  selector: 'app-painel',
  standalone: true,
  imports: [CommonModule, CadastrarMedoComponent, DiagnosticoMedoPipe],
  template: `
    <div class="painel-wrap">
      <!-- HEADER COM INDICADORES COMPUTADOS (SIGNALS) -->
      <section class="metricas-grid">
        <div class="card-metrica">
          <span>Total de Desafios</span>
          <b>{{ medoService.totalMedos() }}</b>
        </div>
        <div class="card-metrica">
          <span>Desafios Vencidos</span>
          <b class="texto-verde">{{ medoService.totalVencidos() }}</b>
        </div>
        <div class="card-metrica">
          <span>Taxa de Coragem</span>
          <b class="texto-primario">{{ medoService.percentualCoragem() }}%</b>
        </div>
        <div class="card-metrica">
          <span>Ruídos para Cortar</span>
          <b class="texto-aviso">{{ medoService.ruidosDetectados() }}</b>
        </div>
      </section>

      <!-- FORMULÁRIO DE ENTRADA -->
      <app-cadastrar-medo></app-cadastrar-medo>

      <!-- LISTA DE ENFRENTAMENTO TÁTICO -->
      <section class="lista-container">
        <h2>Radar de Enfrentamento Ativo</h2>

        <div *ngIf="medoService.totalMedos() === 0" class="vazio">
          Nenhum desafio registrado. Parabéns pela coragem ou adicione seu primeiro medo acima!
        </div>

        <div *ngFor="let m of medoService.medos()" class="card-medo" [class.card-vencido]="m.vencido">
          <div class="medo-header">
            <span class="badge-cat">{{ m.categoria }}</span>
            <span class="diagnostico">{{ m | diagnosticoMedo }}</span>
            <span class="grau">Intensidade: {{ m.nivelParalisia }}/10</span>
          </div>

          <h3 class="medo-titulo" [class.riscado]="m.vencido">{{ m.titulo }}</h3>

          <div class="medo-acoes">
            <button 
              class="btn-acao btn-enfrentar" 
              (click)="medoService.alternarVencido(m.id)">
              {{ m.vencido ? '↺ Reabrir Desafio' : '✓ Marcar como Vencido' }}
            </button>
            <button 
              class="btn-acao btn-excluir" 
              (click)="medoService.remover(m.id)" 
              title="Excluir">
              🗑
            </button>
          </div>
        </div>
      </section>
    </div>
  `,
  styles: [`
    .painel-wrap { max-width: 900px; margin: 0 auto; padding: 20px 0; }
    .metricas-grid { display: grid; grid-template-columns: repeat(auto-fit, minmax(180px, 1fr)); gap: 16px; margin-bottom: 24px; }
    .card-metrica { background: #fff; border: 1px solid #f5d8de; border-radius: 12px; padding: 18px; text-align: center; }
    .card-metrica span { font-size: 13px; color: #673040; text-transform: uppercase; font-weight: 700; letter-spacing: 0.05em; }
    .card-metrica b { display: block; font-size: 32px; font-weight: 800; margin-top: 6px; }
    .texto-verde { color: #168a5c; }
    .texto-primario { color: #dd1b44; }
    .texto-aviso { color: #c77700; }
    .card-medo { background: #fff; border-left: 6px solid #dd1b44; border-radius: 10px; padding: 18px; margin-bottom: 14px; box-shadow: 0 2px 8px rgba(0,0,0,0.04); transition: all .2s; }
    .card-medo.card-vencido { border-left-color: #168a5c; opacity: 0.85; background: #f6fcf8; }
    .medo-header { display: flex; align-items: center; gap: 12px; font-size: 12.5px; margin-bottom: 8px; flex-wrap: wrap; }
    .badge-cat { background: #fde8ed; color: #dd1b44; padding: 2px 8px; border-radius: 4px; font-weight: 800; }
    .diagnostico { font-weight: 700; color: #4a5164; }
    .grau { margin-left: auto; color: #7a8196; }
    .medo-titulo { margin: 6px 0 14px; font-size: 19px; }
    .medo-titulo.riscado { text-decoration: line-through; color: #7a8196; }
    .medo-acoes { display: flex; gap: 10px; }
    .btn-acao { border: none; padding: 8px 16px; border-radius: 6px; font-weight: 700; cursor: pointer; }
    .btn-enfrentar { background: #dd1b44; color: #fff; }
    .card-vencido .btn-enfrentar { background: #168a5c; }
    .btn-excluir { background: #f0f0f0; color: #555; }
    .vazio { text-align: center; padding: 40px; color: #7a8196; font-style: italic; }
  `]
})
export class PainelComponent {
  // Injeta o serviço; todas as métricas são consumidas como chamadas de função Signal: medoService.totalMedos()
  medoService = inject(MedoService);
}
```
===
### Passo 6: O Roteador e a Casca da Aplicação
Configure `src/app/app.routes.ts` e `src/app/app.component.ts`:

```typescript
// src/app/app.routes.ts
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
  }
];
```

```typescript
// src/app/app.component.ts
import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet],
  template: `
    <header style="background:#1a0008; color:#fff; padding:16px 24px; border-bottom:3px solid #dd1b44; display:flex; align-items:center; gap:12px;">
      <span style="font-size:24px; font-weight:900; color:#dd1b44;">▲</span>
      <div>
        <h1 style="margin:0; font-size:18px; font-weight:800;">Antimedo Hub — Sistema Operacional de Coragem</h1>
        <small style="opacity:0.75;">Construído com Angular 17+ Standalone & Signals</small>
      </div>
    </header>
    <main style="padding: 24px;">
      <router-outlet></router-outlet>
    </main>
  `
})
export class AppComponent {}
```
:::

---

## Como Executar e Validar sua Aplicação

Abra o terminal na raiz do projeto e execute:

```bash
ng serve -o
```

O navegador abrirá automaticamente em `http://localhost:4200`.

### Checklist de Conquistas do Laboratório:
1. **Teste a Reatividade:** Adicione um novo medo com nível de paralisia 9. Veja como o contador total e a taxa de coragem recalculam instantaneamente sem refresh.
2. **Teste o LocalStorage:** Clique em *"✓ Marcar como Vencido"*. Recarregue a página com `F5`. Os dados permanecem lá, fiéis e intactos.
3. **Teste o Pipe:** Observe como a linha de diagnóstico muda de cor e instrução dependendo se o item é ruído ou desafio real.

---

::: quiz
? Como os Signals do Angular dispensam a necessidade de chamar ChangeDetectorRef.markForCheck()?
+ O Angular rastreia as leituras de sinal nos templates e sabe com precisão cirúrgica qual nó do DOM precisa ser atualizado
- Os Signals desativam todo o JavaScript do navegador
- Os Signals usam um timer de 1 milissegundo em loop infinito
- Os Signals gravam os dados em disco antes de renderizar
> Signals fornecem reatividade de granularidade fina: o framework sabe exatamente quem depende de qual valor.

? Qual a vantagem de usar `effect()` no serviço para sincronizar com o localStorage?
+ Ele executa automaticamente sempre que qualquer sinal lido em seu interior sofrer uma alteração de valor
- Ele impede que o computador desligue enquanto o arquivo estiver aberto
- Ele criptografa os dados com biometria facial
- Ele deleta os arquivos após 24 horas
> O `effect` reage às mudanças dos Signals automaticamente, mantendo a persistência sempre atualizada.

? Por que o formulário reativo deste laboratório utiliza `standalone: true` e `imports: [ReactiveFormsModule]`?
+ Para garantir que o componente seja autossuficiente e não precise de declaração em nenhum NgModule
- Para exigir que o usuário digite com o teclado virtual
- Para impedir o uso de folhas de estilo CSS
- Para rodar apenas em smartphones Android
> Standalone components importam apenas os módulos de formulários de que necessitam localmente.

? O que a função `crypto.randomUUID()` nativa do navegador produz?
+ Um identificador universal único de 36 caracteres seguro e sem risco de colisão (ex: UUID v4)
- Uma senha alfanumérica de 4 dígitos
- O endereço IP do roteador do usuário
- Um número decimal entre 0 e 1
> `crypto.randomUUID()` gera UUIDs nativamente no navegador com alto padrão criptográfico.

? O que acontece com o resultado de `computed()` caso os sinais dos quais ele depende não tenham mudado?
+ O Angular devolve o valor memorizado em cache imediatamente, poupando processamento de CPU
- Ele lança um erro do tipo NullPointerException
- Ele apaga o banco de dados
- Ele força um download completo da página
> Sinais computados são memorizados (memoized): só recalculam quando suas fontes de dependência mudam.
:::

---

::: cartas
Antimedo Hub :: Aplicação Angular completa integrando Formulários, Signals, Services e Pipes em arquitetura moderna.
signal() :: Primitiva de estado reativo que armazena um valor mutável com rastreamento de leitura.
computed() :: Valor reativo derivado com cache inteligente que só recalcula quando suas dependências se alteram.
effect() :: Operação de efeito colateral que executa automaticamente em resposta a mudanças de sinais.
FormBuilder :: Utilitário para construção concisa de instâncias de FormGroup e FormControl com validadores.
crypto.randomUUID() :: Método nativo do JavaScript moderno para gerar identificadores únicos sem pacotes externos.
:::

::: resumo
Parabéns! Você completou a jornada inteira. Da gagueira inicial do medo até a maestria de componentes, RxJS, Signals, arquitetura standalone e testes: você agora possui não apenas o conhecimento técnico de ponta do Angular, mas a mentalidade inabalável do Antimedo para construir aplicações que transformam o mundo.
:::
