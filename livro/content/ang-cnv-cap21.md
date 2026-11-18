---
id: ang-cnv-cap21
order: 340
kind: capitulo
num: 238
group: Angular · Parte 59 · Bloco I — A Fundação Compassiva
title: A Comunicação que Bloqueia a Compaixão
subtitle: Eliminando Prop Drilling, Injeção Hierárquica e Mensagens Punitivas
minutes: 30
hue: 32
badge: ⚡
image: img/ang-cnv/cap21.svg
- Analisar as 4 formas de comunicação alienante da vida descritas por Marshall Rosenberg: 
- Compreender a crítica ao conceito de "Amtssprache" (linguagem burocrática de negação de responsabilidade): 
- Dominar a Injeção de Dependência Hierárquica do Angular: EnvironmentInjector vs ElementInjector
- Utilizar modificadores de resolução de injeção ({ optional, self, skipSelf, host }) e formulários empáticos: 
origem: angular/livros/02-cnv-angular/content/21-comunicacao-bloqueadora.md
---

::: ilustracao img/ang-cnv/cap21.svg | Comparativo de alta fidelidade: tela hostil com mensagens punitivas e prop drilling caótico à esquerda versus tela compassiva com injeção limpa e orientação contextual à direita.
:::

No segundo capítulo de sua obra, Marshall Rosenberg mergulha nas raízes da linguagem destrutiva. Ele batiza essas estruturas linguísticas com um nome marcante: **comunicação alienante da vida**.

Marshall identifica quatro grandes armadilhas linguísticas que nos desconectam de nosso estado compassivo natural:

### 1. Julgamentos Moralizantes
Classificam o outro como "errado", "mau", "preguiçoso" ou "incompetente". Marshall cita o célebre poeta sufi Rumi:
> *"Para além das ideias de certo e errado, existe um campo. Encontrarei você lá."*

Quando ficamos presos a quem "está certo" e quem "está errado", o diálogo morre e a guerra começa.

### 2. Comparações
Marshall cita o livro satírico de Dan Greenburg (*Como Tornar Sua Vida Miserável*), demonstrando que, se alguém deseja se sentir completamente infeliz e amargo, o método mais eficaz do mundo é comparar suas habilidades e conquistas às de outras pessoas. A comparação destrói a autoestima e fomenta a rivalidade predatória.

### 3. Negação de Responsabilidade (*Amtssprache*)
Durante os julgamentos de criminosos de guerra em Nuremberg e o julgamento de Adolf Eichmann em Jerusalém, os acusados usavam uma linguagem burocrática institucional chamada *Amtssprache* (literalmente, "linguagem de repartição"). Quando perguntados por que cometeram atrocidades, respondiam: *"Eu tive que fazer isso. O regulamento mandava. O meu superior ordenou. O sistema exigiu."*

Marshall nos convoca a banir a expressão *"eu tenho que"* e substituí-la pela coragem autorresponsável de *"eu escolho fazer isso porque valorizo..."*.

### 4. Comunicação por Exigências
Imposições acompanhadas de ameaças implícitas de culpa ou punição quando o outro não obedece cegamente aos nossos caprichos.

---

## O Prop Drilling como Negação de Responsabilidade

Na engenharia de software com Angular, o antipattern conhecido como **Prop Drilling** é a materialização exata do *Amtssprache*:

```
[DashboardComponent] ➔ passa dados
    └── [MainPanelComponent] ➔ "Eu tive que receber isso porque o pai mandou"
            └── [SidebarComponent] ➔ "Eu só estou repassando porque a regra manda"
                    └── [UserCardComponent] ➔ "Não tenho nada com isso, só repasso"
                            └── [AvatarBadgeComponent] ➔ FINALMENTE CONSOME
```

Três componentes intermediários foram escravizados para transportar uma propriedade com a qual nada têm a ver! Se a estrutura do dado mudar, **todos os três componentes quebram**, mesmo não utilizando a informação. Isso polui a arquitetura com acoplamento rígido e desnecessário.

---

## Injeção de Dependência no Angular.dev: A Cura da Hierarquia

O Angular possui um dos sistemas de Injeção de Dependência (DI) mais avançados da computação. Ele opera através de duas árvores hierárquicas distintas:

1. **EnvironmentInjector**: criado no nível de aplicação (`bootstrapApplication` ou provedores de rota `provideRouter`). Seus serviços são singleton para aquele contexto.
2. **ElementInjector**: criado em cada nó do DOM onde um componente ou diretiva declara a propriedade `providers: [...]`.

```
                  [ Root EnvironmentInjector ]
                   (SessaoUsuarioService)
                             │
            ┌────────────────┴────────────────┐
            ▼                                 ▼
   [ ElementInjector: Dashboard ]    [ ElementInjector: Settings ]
            │
            ▼
   [ ElementInjector: AvatarBadge ] ➔ inject(SessaoUsuarioService) diretamente!
```

### O Serviço Singleton Desacoplado com Signals

Em vez de empurrar propriedades de cima para baixo, criamos um serviço de sessão limpo com estado reativo:

```typescript
// src/app/core/services/sessao-usuario.service.ts
import { Injectable, signal, computed } from '@angular/core';

export interface PerfilUsuario {
  readonly id: string;
  readonly nome: string;
  readonly email: string;
  readonly papel: 'desenvolvedor' | 'designer' | 'facilitador';
}

@Injectable({
  providedIn: 'root' // Registrado no EnvironmentInjector raiz da aplicação
})
export class SessaoUsuarioService {
  // Estado privado: ninguém de fora altera arbitrariamente
  private readonly _perfil = signal<PerfilUsuario | null>({
    id: 'usr-404',
    nome: 'Carlos Drummond',
    email: 'carlos@dialogo.org',
    papel: 'facilitador'
  });

  // Exposição pública estritamente imutável
  readonly perfil = this._perfil.asReadonly();
  readonly estaAutenticado = computed(() => this._perfil() !== null);

  alterarNome(novoNome: string): void {
    if (!novoNome.trim()) return;
    this._perfil.update(atual => atual ? { ...atual, nome: novoNome.trim() } : null);
  }
}
```

---

## Modificadores de Resolução de Injeção ({ optional, self, skipSelf, host })

Quando injetamos dependências usando a função `inject()`, o Angular nos permite declarar com exatidão como a busca hierárquica deve se comportar:

```typescript
import { Component, inject } from '@angular/core';
import { SessaoUsuarioService } from './sessao-usuario.service';
import { LOG_PREFIXO_TOKEN } from './tokens';

@Component({
  selector: 'app-avatar-badge',
  standalone: true,
  template: `
    @if (sessao.perfil(); as user) {
      <div class="avatar-badge">
        <span class="nome">{{ user.nome }}</span>
        <span class="papel">({{ user.papel }})</span>
      </div>
    }
  `
})
export class AvatarBadgeComponent {
  // Injeção direta sem passar por intermediários
  protected readonly sessao = inject(SessaoUsuarioService);

  // Exemplo de injeção opcional: não lança erro se o token não existir
  private readonly prefixoLog = inject(LOG_PREFIXO_TOKEN, { optional: true });

  // Outros modificadores da documentação oficial:
  // inject(MeuServico, { self: true })     -> Busca APENAS no ElementInjector local
  // inject(MeuServico, { skipSelf: true }) -> Pula o local e busca apenas nos pais
  // inject(MeuServico, { host: true })     -> Busca até o componente hospedeiro
}
```

Os três componentes intermediários (`MainPanel`, `Sidebar`, `UserCard`) agora têm zero linhas de código dedicadas a transportar propriedades alheias!

---

## Redesenhando Formulários: Da Violência Punitiva à Validação Empática

A violência dos julgamentos moralizantes é frequente em formulários web tradicionais:

```html
<!-- CÓDIGO DA LINGUAGEM DE CHACAL: CULPA, CRÍTICA E ACUSAÇÃO -->
<span class="erro-fatal">
  VOCÊ ERROU! Este campo é obrigatório e sua senha é ridícula de tão fraca.
</span>
```

Na **Linguagem de Girafa**, o formulário reconhece a intenção humana e expressa a necessidade técnica com delicadeza e precisão:

```typescript
// src/app/features/registro/registro-empatico.component.ts
import { Component, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators, AbstractControl, ValidationErrors } from '@angular/forms';
import { CommonModule } from '@angular/common';

/**
 * Validador Customizado Não-Violento:
 * Retorna dados detalhados para orientar construtivamente o usuário.
 */
export function validadorSegurancaCompassiva(control: AbstractControl): ValidationErrors | null {
  const valor = String(control.value || '');
  if (!valor) return null;

  const temMaiuscula = /[A-Z]/.test(valor);
  const temNumero = /[0-9]/.test(valor);
  const tamanhoMinimo = valor.length >= 8;

  if (temMaiuscula && temNumero && tamanhoMinimo) {
    return null; // Necessidade atendida com sucesso
  }

  // Retorna o diagnóstico das necessidades faltantes sem julgamento pejorativo
  return {
    requisitosSeguranca: {
      faltamCaracteres: Math.max(0, 8 - valor.length),
      precisaMaiuscula: !temMaiuscula,
      precisaNumero: !temNumero
    }
  };
}

@Component({
  selector: 'app-registro-empatico',
  standalone: true,
  imports: [ReactiveFormsModule, CommonModule],
  template: `
    <form [formGroup]="form" (ngSubmit)="aoSalvar()" class="form-acolhedor">
      <h3>Criação de Chave Segura</h3>

      <div class="campo-bloco">
        <label for="senha">Chave de Acesso</label>
        <input id="senha" formControlName="senha" type="password" />

        <!-- Feedback Empático Baseado nas Necessidades do Sistema -->
        @if (senhaControl?.touched && senhaControl?.errors?.['requisitosSeguranca']; as req) {
          <div class="caixa-ajuda" role="alert">
            <span class="icone">💡</span>
            <div>
              <p class="titulo-ajuda">Para mantermos seus diálogos seguros, solicitamos:</p>
              <ul>
                @if (req.faltamCaracteres > 0) {
                  <li>Adicionar mais {{ req.faltamCaracteres }} caractere(s).</li>
                }
                @if (req.precisaMaiuscula) {
                  <li>Incluir ao menos uma letra maiúscula.</li>
                }
                @if (req.precisaNumero) {
                  <li>Incluir ao menos um número.</li>
                }
              </ul>
            </div>
          </div>
        }
      </div>

      <button type="submit" [disabled]="form.invalid" class="btn-conectar">
        Confirmar Chave Segura
      </button>
    </form>
  `,
  styles: [`
    .form-acolhedor { max-width: 480px; padding: 1.5rem; background: #fffaf5; border: 1px solid #f5dfd0; border-radius: 12px; }
    .campo-bloco { display: flex; flex-direction: column; gap: 0.5rem; margin-bottom: 1.5rem; }
    input { padding: 0.75rem; border: 1px solid #f5dfd0; border-radius: 6px; font-size: 1rem; }
    .caixa-ajuda { background: #fef3c7; border-left: 4px solid #f59e0b; padding: 0.75rem 1rem; border-radius: 6px; display: flex; gap: 0.75rem; margin-top: 0.5rem; font-size: 0.85rem; color: #78350f; }
    .titulo-ajuda { margin: 0 0 0.25rem 0; font-weight: bold; }
    ul { margin: 0; padding-left: 1.25rem; }
    .btn-conectar { background: #d96b27; color: white; border: none; padding: 0.75rem 1.5rem; border-radius: 6px; font-weight: bold; cursor: pointer; }
    .btn-conectar:disabled { background: #cbd5e1; cursor: not-allowed; }
  `]
})
export class RegistroEmpaticoComponent {
  private readonly fb = inject(FormBuilder);

  readonly form = this.fb.group({
    senha: ['', [Validators.required, validadorSegurancaCompassiva]]
  });

  get senhaControl() {
    return this.form.get('senha');
  }

  aoSalvar(): void {
    if (this.form.valid) {
      console.log('Chave estabelecida em paz:', this.form.value);
    }
  }
}
```

::: dica
Observe como a interface substituiu palavras acusatórias por **uma lista construtiva de itens restantes**. O usuário não se sente humilhado ou julgado; ele se sente convidado a participar de uma parceria para garantir sua própria segurança.
:::

---

::: quiz
? O que o conceito histórico de "Amtssprache" criticado por Marshall Rosenberg representa?
- Um tipo de dialeto antigo da Alemanha Oriental
+ Uma linguagem burocrática institucional utilizada para negar a própria responsabilidade por atos cometidos, atribuindo a culpa a ordens superiores ou sistemas externos
- Um método moderno de gestão de projetos de software
- Uma técnica de oratória para falar mais rápido
> Marshall denunciou como o "Amtssprache" anestesiava a consciência moral dos oficiais em Nuremberg, que justificavam atrocidades dizendo: "eu não tive escolha, apenas segui ordens". Na CNV, assumimos plena responsabilidade por nossas escolhas.

? Qual é a diferença entre o `EnvironmentInjector` e o `ElementInjector` na árvore hierárquica do Angular?
- O EnvironmentInjector roda no backend e o ElementInjector roda no frontend
+ O EnvironmentInjector provê dependências na raiz ou em rotas com escopo de aplicação, enquanto o ElementInjector reside nos nós do DOM de componentes e diretivas
- O ElementInjector foi descontinuado no Angular 15
- Ambos são idênticos e servem apenas para importar arquivos CSS
> O Angular organiza suas dependências em duas árvores complementares: a árvore de ambiente (módulos/rotas) e a árvore de elementos do DOM (componentes/diretivas).

? O que o modificador `{ optional: true }` faz quando passado para a função `inject()`?
- Faz o download do serviço apenas quando o usuário clicar
+ Permite que o Angular retorne `null` se a dependência não for encontrada na hierarquia, sem lançar uma exceção catastrófica no console
- Desativa a verificação de tipos do TypeScript
- Torna o serviço obrigatório em todos os testes
> O modificador `optional` previne falhas quando um serviço ou token de configuração pode ou não estar disponível naquele contexto.

? Por que o Prop Drilling é considerado um antipattern que viola a compaixão arquitetural?
- Porque ele aumenta a conta de luz do servidor
+ Porque força componentes intermediários a transportar dados que não lhes pertencem, quebrando a separação de responsabilidades e propagando quebras em cadeia a cada refatoração
- Porque impede o uso de imagens no template
- Porque obriga todos os arquivos a terem o mesmo nome
> No Prop Drilling, componentes inocentes tornam-se reféns de modelos de dados alheios, inflando seus inputs e outputs sem nenhum benefício funcional real.

? Como o validador customizado reativo exemplificado no capítulo aplica a Linguagem de Girafa?
- Apagando o texto digitado se a senha for fraca
+ Retornando dados específicos sobre quais necessidades de segurança ainda não foram atendidas (quantos caracteres faltam, se precisa de maiúscula), orientando construtivamente o usuário
- Bloqueando o teclado do usuário por 5 minutos
- Exibindo fotos de girafas na tela
> Em vez de retornar um booleano de rejeição simplista, o validador compassivo informa o usuário sobre o que o sistema necessita para manter a segurança de todos.
:::

---

::: cartas
Comunicação Alienante da Vida :: Padrões de linguagem (julgamentos, comparações, negação de responsabilidade, exigências) que bloqueiam a compaixão natural.
Amtssprache :: Linguagem burocrática institucional de repartição que oculta escolhas individuais sob a desculpa de "regras do sistema".
EnvironmentInjector :: Injetor do Angular que provê serviços no escopo de ambiente global ou de rotas carregadas tardiamente.
ElementInjector :: Injetor do Angular associado a componentes e diretivas específicos na árvore do DOM.
inject() com Modificadores :: Uso de opções como `{ optional: true, self: true, skipSelf: true }` para controlar a resolução hierárquica da injeção.
Validação Não-Violenta :: Padrão de formulário que instrui com clareza quais necessidades faltam ser atendidas, sem usar termos moralizantes ou punitivos.
Rumi :: Poeta persa do século XIII que formulou o lema da reconciliação: "Para além das ideias de certo e errado, existe um campo. Encontrarei você lá."
:::
