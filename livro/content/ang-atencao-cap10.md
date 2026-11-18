---
id: ang-atencao-cap10
order: 332
kind: capitulo
num: 234
group: Angular · Parte 58 · As oito semanas
title: "Semana 6 · Passado, futuro e presente"
subtitle: Falar consigo como se fala com um amigo. Formulários com Signal Forms, mensagens de validação gentis e um histórico de desfazer que não carrega o passado inteiro.
minutes: 80
hue: 200
objectives:
- Entender a prática de **gentileza** da sexta semana e a ideia de não ficar preso no passado
- Criar formulários com **Signal Forms** (estáveis no Angular 22): `form()`, `[formField]` e o **esquema** de validação
- Escrever **mensagens de validação** que orientam, sem acusar, e mostrá-las na hora certa (`touched`)
- Enviar com **`submit()`** e reiniciar com **`reset()`**
- Construir um **histórico de desfazer/refazer** com limite, usando signals
origem: angular/livros/01-atencao-plena-angular/content/11-cap10.md
---

::: cena A mensagem que gritava
O formulário antigo dizia, em vermelho e caixa alta: **CAMPO OBRIGATÓRIO INVÁLIDO**, assim que a tela abria, antes de alguém digitar qualquer coisa. Marta leu em voz alta e perguntou: "Se uma pessoa falasse assim com você no balcão, você voltaria?" Caio riu, e depois ficou pensando nas mensagens de erro que ele mesmo tinha escrito ao longo dos anos.
:::

::: ilustracao img/ang-atencao/cap10.svg | Uma conversa gentil com um formulário: a mensagem aparece na hora certa e diz o que fazer, sem apontar o dedo.
:::

## O que o livro diz

A sexta semana fala de como a mente pode ficar **presa ao passado** (repassando o que deu errado) ou **ao futuro** (antecipando o pior), e de como o mau humor puxa lembranças e previsões no mesmo tom. A prática principal é uma meditação de **amizade**, ou gentileza: desejar o bem a si mesmo, depois a alguém querido, a alguém neutro, a alguém difícil, e ao mundo. Os autores reconhecem que, para muita gente, o mais difícil é o primeiro passo: **ser gentil consigo**.

Os hábitos pequenos da semana são **recuperar** uma atividade que você deixou de fazer e fazer um **gesto gentil** por alguém, sem alarde.

Nas nossas palavras: **a forma como dizemos as coisas (para nós e para os outros) muda o que acontece depois.**

## Da vida para o código

| Na vida | No Angular |
|---|---|
| Falar consigo como com um amigo | **Mensagens de validação** que dizem o que fazer, sem acusar |
| Não cobrar antes de a pessoa tentar | Mostrar erros só depois que o campo foi **tocado** (`touched`) |
| Uma regra clara, num lugar só | O **esquema** do formulário: `required`, `min`, `maxLength`... |
| Voltar atrás quando quiser | **Desfazer/refazer** com um histórico |
| Não carregar o passado inteiro | Um histórico com **limite** de tamanho |

## 1) Signal Forms em uma frase

Em Signal Forms, **o modelo é um signal comum**, e o formulário é uma **árvore de campos** derivada dele. Cada campo sabe seu valor, se é válido, quais são os erros, se foi tocado. Não há uma cópia separada dos dados: digitar no campo atualiza o signal do modelo, e mudar o signal atualiza o campo.

::: novo Signal Forms são estáveis no Angular 22
A API `@angular/forms/signals` apareceu como experimental no Angular 21 e virou **estável** no 22. Os formulários reativos (`FormGroup`, `FormControl`) continuam existindo e funcionando, mas para código novo o Angular recomenda Signal Forms, que conversam naturalmente com o resto do app feito de signals.
:::

## 2) O registro gentil

<<< src/app/capitulos/cap10/registro-gentil.ts

Peça por peça:

- **`modelo = signal<RegistroDoMomento>({...})`**: os dados, um objeto comum dentro de um signal.
- **`form(this.modelo, (campo) => { ... })`** cria a árvore de campos. O segundo argumento é o **esquema**: uma função que recebe os caminhos dos campos (`campo.palavra`, `campo.minutos`...) e declara as regras.
- **`required`**, **`min`**, **`max`**, **`maxLength`** são as regras prontas. Todas aceitam `{ message }`.
- **`[formField]="registro.palavra"`** liga o `<input>` ao campo. A diretiva **`FormField`** precisa estar no `imports`. Ela cuida do valor, do evento de digitação, do `touched` ao sair do campo e até converte o valor de um `type="number"` para número.
- **`registro.palavra()`** devolve o **estado** do campo: `value()`, `valid()`, `invalid()`, `errors()`, `touched()`, `dirty()`...
- **`errors()`** é uma lista de objetos com `kind` (o tipo da regra) e `message`.

### Mensagens que orientam

Compare:

| Acusa | Orienta |
|---|---|
| CAMPO OBRIGATÓRIO | Escreva uma palavra, qualquer uma serve: "cansado", "curioso", "ok". |
| VALOR INVÁLIDO | Mesmo um minuto conta. Use 1 ou mais. |
| EXCEDEU O LIMITE | A nota passou de 280 caracteres. Que tal guardar só o essencial? |

Uma boa mensagem diz **o que fazer**, dá **um exemplo** quando ajuda, e não culpa ninguém. É a meditação da amizade aplicada à interface.

### Na hora certa

`mostrarErros(campo)` só devolve `true` se o campo foi **tocado** e está **inválido**. Ninguém vê erro antes de tentar. Quando a pessoa clica em **Guardar**, o `submit()` marca todos os campos como tocados, e as mensagens aparecem onde faltou algo.

E a acessibilidade: o campo recebe `aria-invalid` quando há erro, e `aria-describedby` aponta para a área das mensagens, então o leitor de tela lê a mensagem junto com o campo.

### Enviar e recomeçar

```ts
await submit(this.registro, async () => { ... });
```

**`submit`** marca tudo como tocado e **só chama a função se o formulário for válido**. Dentro dela, guardamos o registro e chamamos **`this.registro().reset({ ...VAZIO })`**, que volta o modelo ao início e limpa o "tocado", para as mensagens não aparecerem de novo na hora.

::: csharp Lembra o DataAnnotations?
O esquema faz o papel de `[Required]`, `[Range(1, 120)]` e `[MaxLength(280)]` de um modelo do ASP.NET, com a vantagem de ficar **fora** do tipo dos dados e poder ser composto e reutilizado. E, como no servidor, **valide de novo no back-end**: validação no navegador é para ajudar a pessoa, não para proteger dados.
:::

## 3) Testando o formulário pelo lado de fora

<<< src/app/capitulos/cap10/formularios.spec.ts#formulario

O teste digita (evento `input`), sai do campo (evento `blur`, que marca como tocado) e envia o formulário (evento `submit`). Repare no primeiro teste: **nenhum** erro aparece antes de a pessoa tocar no campo. É a regra da gentileza, protegida por um teste.

## 4) Passado e presente: desfazer com limite

Desfazer é uma forma de voltar ao passado **sem ficar preso a ele**: você volta quando quer, e o passado guardado tem tamanho limitado.

<<< src/app/capitulos/cap10/historico.ts

- O histórico é **genérico** (`Historico<T>`): serve para um texto, um objeto, um formulário inteiro.
- Três signals: `passado`, `presente` e `futuro`. `podeDesfazer` e `podeRefazer` são `computed`, prontos para o `[disabled]` dos botões.
- **`registrar`** empurra o presente para o passado, **apaga o futuro** (uma mudança nova abre outro caminho) e corta o passado no **limite** com `slice(-limite)`.
- `Object.is(novo, presente)` evita passos vazios quando nada mudou.
- O construtor recusa um limite menor que 1 com `RangeError`: um erro **cedo**, com mensagem clara.

<<< src/app/capitulos/cap10/formularios.spec.ts#historico

::: pratica Gentileza, começando por você
Sente-se por um minuto e repita mentalmente, devagar, algo como: *"Que eu esteja em paz. Que eu cuide de mim com gentileza."* Se soar estranho, tudo bem: note a estranheza também. Depois, se quiser, dirija o mesmo desejo a alguém de quem você gosta.
:::

::: exercicio Desfazer no registro gentil
Ligue o `Historico` ao campo **nota** do `RegistroGentil`: a cada vez que a pessoa sair do campo (blur), registre o texto; acrescente botões **Desfazer** e **Refazer** que trocam a nota no modelo.

--- resposta
Na classe:

```ts
protected readonly historicoDaNota = new Historico('', 10);

protected lembrarNota(): void {
  this.historicoDaNota.registrar(this.modelo().nota);
}

protected voltarNota(direcao: 'desfazer' | 'refazer'): void {
  this.historicoDaNota[direcao]();
  this.modelo.update((m) => ({ ...m, nota: this.historicoDaNota.presente() }));
}
```

No template:

```html
<textarea id="nota" [formField]="registro.nota" (blur)="lembrarNota()"></textarea>
<button type="button" (click)="voltarNota('desfazer')" [disabled]="!historicoDaNota.podeDesfazer()">Desfazer</button>
<button type="button" (click)="voltarNota('refazer')" [disabled]="!historicoDaNota.podeRefazer()">Refazer</button>
```

Como o formulário é derivado do signal `modelo`, basta atualizar o modelo: o `<textarea>` acompanha sozinho. Repare também no `update` com `{ ...m, nota }`: um objeto **novo**, como no capítulo 4.
:::

::: resumo
- A sexta semana fala de não ficar preso ao passado nem ao futuro e propõe uma prática de **gentileza**, começando por si mesmo.
- Em **Signal Forms**, o **modelo é um signal**; **`form(modelo, esquema)`** cria a árvore de campos; **`[formField]`** liga cada campo ao HTML.
- O **esquema** declara regras (`required`, `min`, `max`, `maxLength`) com **mensagens** que orientam, sem acusar.
- Cada campo expõe `value`, `valid`, `invalid`, `errors`, `touched`; mostre erros só **depois de tocado**.
- **`submit()`** marca tudo como tocado e só envia se for válido; **`reset()`** recomeça.
- Validação no navegador ajuda a pessoa; **o servidor valida de novo**.
- Um **`Historico<T>`** com signals oferece desfazer/refazer, apaga o futuro numa mudança nova e tem **limite** de tamanho.
:::

::: quiz
? Qual costuma ser o passo mais difícil da meditação da amizade, segundo os autores?
+ Ser gentil consigo mesmo
- Pensar em alguém querido
- Sentar em silêncio
- Desejar o bem ao mundo
> Muita gente acha mais fácil ser gentil com os outros do que consigo.

? Em Signal Forms, onde ficam os dados do formulário?
+ Num signal comum (o modelo); o formulário é derivado dele
- Numa cópia interna do `FormGroup`
- No DOM
- No `localStorage`
> Não há cópia separada: digitar atualiza o modelo, e o modelo atualiza o campo.

? O que faz `[formField]="registro.palavra"`?
- Cria uma regra de validação
+ Liga o elemento HTML ao campo do formulário (valor, digitação, toque)
- Mostra os erros do campo
- Envia o formulário
> A diretiva `FormField` precisa estar no `imports` do componente.

? Por que mostrar erros só depois de `touched()`?
+ Para não cobrar a pessoa antes de ela tentar preencher
- Porque `errors()` é vazio antes disso
- Porque o Angular não permite
- Para o formulário ficar mais rápido
> É gentileza de interface: o erro aparece quando ajuda.

? O que `submit(form, acao)` faz quando o formulário é inválido?
- Chama a ação mesmo assim
+ Marca os campos como tocados e não chama a ação
- Apaga os dados
- Lança uma exceção
> As mensagens aparecem onde faltou algo, e nada é enviado.

? No `Historico`, por que uma mudança nova apaga o futuro?
+ Porque, depois de desfazer e mudar algo, o "refazer" antigo não faz mais sentido
- Para economizar memória
- Por um bug
- Porque signals não guardam listas
> Uma mudança nova abre outro caminho; o futuro anterior deixou de existir.
:::

::: cartas
Signal Forms :: Formulários do Angular 22 em que o modelo é um signal e o formulário é uma árvore de campos derivada.
form(modelo, esquema) :: Cria a árvore de campos a partir do signal do modelo, com as regras do esquema.
[formField] :: Diretiva que liga um elemento HTML a um campo do formulário.
Esquema :: Função que declara as regras (`required`, `min`, `maxLength`...) com mensagens.
touched :: O campo já foi visitado; bom momento para mostrar erros.
submit() :: Marca tudo como tocado e só executa a ação se o formulário for válido.
reset() :: Volta o formulário a um valor inicial e limpa os estados de toque.
Historico<T> :: Desfazer/refazer com signals: passado, presente, futuro e um limite de tamanho.
:::
