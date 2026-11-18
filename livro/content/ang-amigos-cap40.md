---
id: ang-amigos-cap40
order: 361
kind: capitulo
num: 257
group: Angular · Parte 66 · Seis Maneiras de Fazer os Desenvolvedores Adorarem sua Biblioteca
title: Fale Sobre o que Interessa ao Outro
subtitle: Formulários Interoperáveis e a Interface ControlValueAccessor
minutes: 30
hue: 215
badge: 🤝
image: img/ang-amigos/cap40.svg
- Analisar o hábito de Theodore Roosevelt de estudar os interesses de cada hóspede na noite anterior à visita: 
- Reconhecer que desenvolvedores Angular respiram formulários reativos e esperam integração nativa com eles: 
- Dominar o contrato da interface ControlValueAccessor (writeValue, registerOnChange, registerOnTouched, setDisabledState): 
- Registrar o provedor NG_VALUE_ACCESSOR com forwardRef em componentes standalone: 
- Criar um componente de Toggle amigável que funciona perfeitamente com formControl, formControlName e ngModel: 
origem: angular/livros/03-amigos-angular/content/40-control-value-accessor-formularios.md
---

::: ilustracao img/ang-amigos/cap40.svg | Diagrama de fluxo de dados conectando o Reactive Form do Angular ao componente customizado através dos métodos do ControlValueAccessor.
:::

Todos os que visitavam a Casa Branca durante a presidência de **Theodore Roosevelt** ficavam profundamente assombrados com a impressionante variedade de seus conhecimentos. Fosse o visitante um vaqueiro do Texas, um diplomata de Nova York, um cientista francês ou um político búlgaro, Roosevelt parecia dominar exatamente aquele tema com a maestria de um veterano.

Qual era o segredo do presidente? Será que ele era uma enciclopédia ambulante sobre todos os assuntos da Terra?

A resposta era simples e generosa: **sempre que Roosevelt esperava a chegada de um visitante para o café da manhã ou para uma reunião matinal, ele ficava acordado até altas horas da noite anterior lendo avidamente sobre o assunto que sabia ser o favorito daquele homem**. Se o convidado era um criador de cavalos, Roosevelt lia sobre raças equinas; se era um escritor, estudava suas obras mais recentes.

Como Dale Carnegie sintetiza com agudeza:

> *"A estrada real para o coração de um homem é falar sobre as coisas que ele mais valoriza. Se você falar sobre o assunto que interessa ao outro, você não apenas conquistará sua atenção imediata, mas o deixará lisonjeado, grato e eternamente receptivo às suas próprias ideias."*

---

## O Assunto que Interessa ao Desenvolvedor Angular

No ecossistema corporativo de frontend, **o que mais interessa aos desenvolvedores de aplicações Angular?**

A resposta é categórica: **formulários**. Toda aplicação de negócios no planeta Terra gira em torno de telas de cadastro, filtros, configurações, regras de validação síncronas e assíncronas, estados de *dirty*, *touched*, *valid* e *invalid*.

Agora imagine o choque de um programador quando ele encontra um lindo componente de seleção, calendário ou botão de alternância (Toggle) na sua biblioteca, tenta escrever no template:

```html
<amg-toggle formControlName="notificarPorEmail" />
```

E o Angular cospe no console o aviso fatal:

```text
NG01203: No value accessor for form control with name: 'notificarPorEmail'.
```

Nesse exato momento, todo o brilho visual do seu componente vai pelo ralo. O desenvolvedor percebe que terá que inventar "gambiarras": ouvir eventos manuais, disparar `setValue` com Signals, criar flags auxiliares para validação. Ele amaldiçoa a biblioteca porque o autor do componente **falou apenas sobre o próprio umbigo**, esquecendo-se de falar a língua que o consumidor mais valoriza: **o ecossistema de formulários do Angular**.

---

## A Ponte Diplomática: `ControlValueAccessor`

Para falar a língua que os desenvolvedores amam, seu componente precisa implementar a interface oficial **`ControlValueAccessor`** do `@angular/forms`. Essa interface atua como o embaixador diplomático entre o motor de formulários do Angular e o seu DOM interno.

Ela exige quatro métodos simples e vitais:
1. `writeValue(valor: any)`: O Angular entrega o dado do formulário para o seu componente exibir.
2. `registerOnChange(fn: any)`: O seu componente armazena a função que deve ser chamada sempre que o usuário alterar o valor.
3. `registerOnTouched(fn: any)`: O seu componente armazena a função que avisa ao Angular que o campo foi tocado (útil para disparar mensagens de erro de validação).
4. `setDisabledState?(isDisabled: boolean)`: O Angular avisa quando o formulário inteiro for desabilitado via código.

Vejamos como construir o `AmgToggleComponent`:

```typescript
// amg-toggle.component.ts
import { 
  Component, 
  ChangeDetectionStrategy, 
  signal, 
  forwardRef 
} from '@angular/core';
import { 
  ControlValueAccessor, 
  NG_VALUE_ACCESSOR 
} from '@angular/forms';

@Component({
  selector: 'amg-toggle',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => AmgToggleComponent),
      multi: true
    }
  ],
  template: `
    <button
      type="button"
      role="switch"
      class="amg-toggle-switch"
      [class.amg-toggle-checked]="checked()"
      [attr.aria-checked]="checked()"
      [disabled]="disabled()"
      (click)="alternar()">
      <span class="amg-toggle-thumb" aria-hidden="true"></span>
    </button>
  `,
  styles: [`
    :host { display: inline-flex; align-items: center; }
    .amg-toggle-switch {
      position: relative;
      width: 44px;
      height: 24px;
      border-radius: 9999px;
      background: #cbd5e1;
      border: 2px solid transparent;
      cursor: pointer;
      transition: background-color 0.2s ease, border-color 0.2s ease;
      padding: 0;
    }
    .amg-toggle-switch:focus-visible {
      outline: 2px solid #2563eb;
      outline-offset: 2px;
    }
    .amg-toggle-checked {
      background: #2563eb;
    }
    .amg-toggle-thumb {
      position: absolute;
      top: 2px;
      left: 2px;
      width: 16px;
      height: 16px;
      border-radius: 50%;
      background: #ffffff;
      box-shadow: 0 1px 3px rgba(0, 0, 0, 0.2);
      transition: transform 0.2s ease;
    }
    .amg-toggle-checked .amg-toggle-thumb {
      transform: translateX(20px);
    }
    .amg-toggle-switch:disabled {
      opacity: 0.5;
      cursor: not-allowed;
    }
  `]
})
export class AmgToggleComponent implements ControlValueAccessor {
  /** Estado reativo interno com Signal */
  protected readonly checked = signal<boolean>(false);
  protected readonly disabled = signal<boolean>(false);

  /** Callbacks do ControlValueAccessor */
  private onChange: (valor: boolean) => void = () => {};
  private onTouched: () => void = () => {};

  /**
   * 1. writeValue: Chamado pelo Angular quando o valor do FormControl muda
   */
  writeValue(valor: boolean | null | undefined): void {
    this.checked.set(!!valor);
  }

  /**
   * 2. registerOnChange: Registra a função que notifica o formulário
   */
  registerOnChange(fn: (valor: boolean) => void): void {
    this.onChange = fn;
  }

  /**
   * 3. registerOnTouched: Registra a função de blur/interação
   */
  registerOnTouched(fn: () => void): void {
    this.onTouched = fn;
  }

  /**
   * 4. setDisabledState: Responde a status disabled do FormControl
   */
  setDisabledState(isDisabled: boolean): void {
    this.disabled.set(isDisabled);
  }

  /** Método disparado pelo clique do usuário */
  protected alternar(): void {
    if (this.disabled()) return;
    
    const novoValor = !this.checked();
    this.checked.set(novoValor);
    
    // Notifica o motor de formulários do Angular
    this.onChange(novoValor);
    this.onTouched();
  }
}
```

---

## O Deleite de Quem Consome

Agora observe a facilidade arrebatadora de um desenvolvedor da sua equipe usando esse Toggle dentro de um `FormGroup` reativo:

```typescript
// perfil-notificacoes.component.ts
import { Component, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { AmgToggleComponent } from '@amigos/ui';

@Component({
  selector: 'app-perfil-notificacoes',
  standalone: true,
  imports: [ReactiveFormsModule, AmgToggleComponent],
  template: `
    <form [formGroup]="form">
      <label style="display: flex; align-items: center; gap: 12px;">
        <amg-toggle formControlName="elogiosPorEmail" />
        <span>Receber notificações de novos elogios por e-mail</span>
      </label>

      @if (form.get('elogiosPorEmail')?.dirty) {
        <small style="color: #2563eb;">Configuração alterada com sucesso!</small>
      }
    </form>
  `
})
export class PerfilNotificacoesComponent {
  private fb = inject(FormBuilder);

  readonly form = this.fb.group({
    elogiosPorEmail: [true, Validators.required]
  });
}
```

O desenvolvedor usou `formControlName`, observou `dirty`, aplicou `Validators.required` e tudo funcionou de primeira, sem atrito e sem gambiarra. Você estudou o que interessava a ele na noite anterior, tal como Theodore Roosevelt, e abriu caminho direto para o seu coração.

---

::: quiz
? Qual hábito de Theodore Roosevelt impressionava profundamente seus convidados na Casa Branca?
+ Passar a noite anterior à visita estudando o assunto que era da preferência pessoal daquele hóspede.
- Desafiar seus convidados para duelos de esgrima antes do almoço.
- Enviar cartas anônimas criticando os hábitos alimentares dos senadores.
- Decorar os nomes dos cavalos de todos os fazendeiros da Europa.
> Roosevelt sabia que falar sobre os interesses do outro é a via régia para conquistar estima, simpatia e alianças sólidas.

? Por que desenvolvedores de software corporativo valorizam tanto a interface `ControlValueAccessor`?
+ Porque ela permite que componentes de interface customizados se integrem perfeitamente ao ecossistema de formulários do Angular (`ReactiveFormsModule` e `FormsModule`).
- Porque ela dobra a velocidade de rotação do cooler do computador.
- Porque ela remove a necessidade de escrever testes unitários.
- Porque ela permite salvar dados diretamente em fitas magnéticas antigas.
> Sem o `ControlValueAccessor`, componentes de formulário viram caixas isoladas que exigem códigos de sincronização manuais e propensos a falhas.

? Qual método da interface `ControlValueAccessor` é invocado pelo Angular para transmitir um novo valor para o componente?
+ `writeValue(value: any)`
- `sendValueToCloud(data: any)`
- `renderNow()`
- `destruirComponente()`
> O método `writeValue` é o ponto de entrada oficial por onde o Angular alimenta o componente com o dado atualizado do formulário.

? Por que o token `NG_VALUE_ACCESSOR` precisa ser fornecido com a propriedade `multi: true` no array de providers?
+ Porque o Angular mantém uma coleção extensível de Value Accessors para gerenciar diferentes tipos de entradas no mesmo formulário.
- Porque o código só roda se for compilado mais de uma vez.
- Porque permite que múltiplos monitores exibam o mesmo componente.
- Para habilitar suporte a idiomas orientais.
> O `multi: true` indica que o token aceita múltiplos provedores registrados sem sobrescrever os accessors nativos do Angular.

? Para que serve o método `registerOnTouched(fn)` no fluxo de validação de formulários?
+ Para registrar o callback que informa ao Angular que o usuário interagiu com o campo (blur), permitindo disparar validações visuais de erro.
- Para verificar a temperatura da tela touch do celular.
- Para contar quantas vezes o usuário tocou no teclado mecânico.
- Para enviar a foto do usuário para o servidor da aplicação.
> Notificar o toque (`onTouched()`) é vital para que diretivas de validação como `ng-touched` funcionem e mensagens de erro apareçam no momento oportuno.
:::

---

::: cartas
Theodore Roosevelt :: Presidente americano famoso por estudar os interesses de cada hóspede na noite anterior à visita.
Falar do que Interessa :: Princípio de Dale Carnegie: a estrada mestra para o coração de alguém é falar sobre o que ela mais valoriza.
ControlValueAccessor :: Interface oficial do Angular para conectar componentes visuais customizados ao motor de formulários.
writeValue() :: Método do CVA que recebe o dado do formulário e atualiza o estado interno do componente.
NG_VALUE_ACCESSOR :: InjectionToken de injeção que cadastra o componente customizado como um manipulador legítimo de formulários.
registerOnTouched() :: Callback que sinaliza ao Angular que o usuário interagiu com o controle, ativando estados de validação.
:::
