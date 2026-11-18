---
id: ang-antimedo-cap10
order: 310
kind: capitulo
num: 215
group: Angular · Parte 53 · Bloco II — Quebrando o Ciclo
title: As irmãs do medo
subtitle: Pipes — transformando estados brutos em poder visual
minutes: 25
hue: 280
badge: 🎭
- Identificar como estados emocionais desordenados podem ser filtrados e transformados: 
- Dominar os Pipes nativos do Angular para datas, moedas e strings: 
- Criar Pipes personalizados implementando a interface PipeTransform: 
- Compreender a diferença crítica entre Pure Pipes e Impure Pipes na performance: 
origem: angular/livros/01-antimedo-angular/content/10-irmas-do-medo.md
---

::: ilustracao img/ang-antimedo/cap10.svg | Um prisma óptico recebendo um raio cinzento caótico e refratando um feixe de cores limpas e organizadas.
:::

No livro *Antimedo*, Pablo Marçal dedica um capítulo inteiro para expor o que ele chama de **as irmãs do medo**:

- **A Ansiedade:** o medo antecipado do futuro que ainda não existe.
- **A Procrastinação:** o medo da dor do esforço no presente.
- **A Vitimização:** o medo de assumir a responsabilidade pelas próprias decisões.
- **A Paralisia:** o congelamento físico e mental diante da incerteza.

Essas emoções não são entidades desconectadas. Elas são simplesmente **dados brutos não processados** que a mente interpreta de forma destrutiva.

Se você deixar a ansiedade em seu estado bruto, ela te adoece. Mas se você passá-la por uma **transformação consciente**, ela se converte em senso de urgência, foco aguçado e preparo antecipado.

No frontend com Angular, nós recebemos dados brutos o tempo todo: números de ponto flutuante estranhos (`1543.89201`), datas em formato ISO esquisito (`2026-10-29T21:40:00Z`), booleanos frios (`true` ou `false`).

Mostrar o dado bruto ao usuário gera confusão. Para transformá-lo de forma elegante, puramente visual e sem alterar a essência original do dado no código, o Angular nos oferece os **Pipes** (representados pelo caractere `|`).

---

## Os Pipes Nativos Essenciais

O Angular já vem de fábrica com uma coleção de transformadores prontos no `CommonModule`:

```html
<!-- Exibição de Moeda Brasileira -->
<p>Valor da Vitória: {{ 1499.90 | currency:'BRL':'symbol':'1.2-2':'pt-BR' }}</p>
<!-- Saída: R$ 1.499,90 -->

<!-- Formatação de Datas Humanizadas -->
<p>Data do Desafio: {{ dataAtual | date:'dd/MM/yyyy HH:mm' }}</p>
<!-- Saída: 29/10/2026 19:40 -->

<!-- Formatação de Textos -->
<p>Grito de Guerra: {{ 'eu sou invencivel' | uppercase }}</p>
<!-- Saída: EU SOU INVENCIVEL -->

<!-- Depuração Rápida de Objetos no Template -->
<pre>{{ perfilUsuario | json }}</pre>
```

Pipes podem ser encadeados em sequência, como uma linha de montagem:

```html
<!-- Transforma em maiúsculo e depois fatia os primeiros 10 caracteres -->
<span>{{ 'antimedo transformador' | uppercase | slice:0:8 }}</span>
<!-- Saída: ANTIMEDO -->
```

---

## Criando um Custom Pipe: Transformando Medo em Ação

Quando os pipes nativos não forem suficientes para expressar as regras do seu domínio, você pode criar o seu próprio pipe customizado. Basta implementar a interface `PipeTransform` e utilizar o decorador `@Pipe`:

```typescript
// shared/pipes/antimedo.pipe.ts
import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'antimedo',
  standalone: true
})
export class AntimedoPipe implements PipeTransform {
  /**
   * Transforma um estado emocional negativo em uma ordem de ação imediata
   */
  transform(emocao: string, nivelIntensidade: number = 1): string {
    if (!emocao) return '';

    const mapaDeAcao: Record<string, string> = {
      ansiedade: 'Respire fundo, planeje as próximas 2 horas e execute o primeiro passo.',
      procrastinacao: 'Ligue o cronômetro de 5 minutos e faça a tarefa mais ridícula da lista.',
      paralisia: 'Levante da cadeira agora, beba um copo d’água e dê um comando em voz alta.',
      vitimismo: 'Assuma 100% da culpa pelo seu resultado e comece a consertar.'
    };

    const chave = emocao.toLowerCase().trim();
    const acaoSugerida = mapaDeAcao[chave] || `Enfrente "${emocao}" com pequenos passos diários.`;

    const prefixo = nivelIntensidade >= 5 ? '🚨 [AÇÃO URGENTE]: ' : '⚡ [FOCO]: ';
    return `${prefixo}${acaoSugerida}`;
  }
}
```

Usando o pipe customizado no seu template:

```html
<div>
  <!-- Usando o pipe com parâmetros -->
  <p>{{ 'ansiedade' | antimedo:8 }}</p>
</div>
```

```saida
🚨 [AÇÃO URGENTE]: Respire fundo, planeje as próximas 2 horas e execute o primeiro passo.
```

---

## Pure Pipes vs. Impure Pipes: A Regra de Ouro da Performance

Por padrão, todo pipe no Angular é **Puro** (`pure: true`).

- **Pure Pipe (Puro):** O Angular só executa o método `transform()` quando a **referência** do valor de entrada mudar (uma nova string, um novo número, um novo objeto). Se o valor primitivo for o mesmo, ele reutiliza o resultado memorizado do cache. É extremamente rápido e leve!
- **Impure Pipe (Impuro):** Se você declarar `@Pipe({ name: '...', pure: false })`, o método `transform()` será executado a cada milissegundo, a cada ciclo de detecção de mudanças, a cada movimento do mouse ou clique na tela.

::: cuidado Evite Impure Pipes a Todo Custo
Usar pipes impuros para filtrar arrays grandes dentro de um `*ngFor` é uma das formas mais fáceis de destruir a taxa de quadros (FPS) da sua aplicação e travar o navegador do usuário. Filtre dados em Services ou com Signals/RxJS antes de enviar ao template.
:::

---

::: exercicio Exercício: Pipe de Tempo Decorrido
Implemente um pipe chamado `TempoPassadoPipe` que receba uma data no passado e retorne `"Hoje"`, `"Ontem"` ou `"Há X dias"`.

--- resposta
```typescript
import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'tempoPassado',
  standalone: true
})
export class TempoPassadoPipe implements PipeTransform {
  transform(data: Date | string | number): string {
    if (!data) return '';
    const passado = new Date(data).getTime();
    const agora = Date.now();
    const diferencaDias = Math.floor((agora - passado) / (1000 * 60 * 60 * 24));

    if (diferencaDias <= 0) return 'Hoje';
    if (diferencaDias === 1) return 'Ontem';
    return `Há ${diferencaDias} dias`;
  }
}
```
:::

---

::: quiz
? Qual a principal função de um Pipe no Angular?
+ Transformar a exibição visual de dados no template sem alterar o dado original no TypeScript
- Fazer o download de pacotes npm adicionais
- Abrir conexões WebSocket com o servidor backend
- Criar tabelas no banco de dados
> Pipes transformam valores formatando-os para exibição sem modificar o estado em memória.

? Qual caractere é utilizado no template para aplicar um Pipe a uma expressão?
+ | (barra vertical / pipe)
- & (e comercial)
- # (jogo da velha)
- @ (arroba)
> Usa-se o caractere `|`, herdado da tradição de pipelines do Unix.

? Qual interface todo Pipe customizado do Angular deve implementar?
+ PipeTransform
- OnInit
- OnChanges
- HttpHandler
> Todo pipe customizado deve implementar o método `transform()` da interface `PipeTransform`.

? Por que Pure Pipes (pipes puros) são altamente recomendados para performance?
+ Porque eles só recalculam o valor quando a referência da entrada de dados realmente mudar
- Porque eles desativam o CSS da página
- Porque eles rodam em uma thread isolada no servidor
- Porque eles não aceitam parâmetros adicionais
> Pipes puros memorizam os resultados e economizam ciclos de CPU da detecção de mudanças.

? Como se passa um argumento para um pipe no template Angular?
+ Usando dois-pontos após o nome do pipe: `valor | meuPipe:arg1:arg2`
- Usando parênteses: `valor | meuPipe(arg1)`
- Usando colchetes: `valor | meuPipe[arg1]`
- Usando sinal de igual: `valor | meuPipe=arg1`
> Argumentos em pipes são passados com `:` (ex: `preco | currency:'BRL':'symbol'`).
:::

---

::: cartas
Irmãs do Medo :: Emoções desordenadas (ansiedade, procrastinação) que representam dados brutos precisando de transformação.
Pipe no Angular :: Função pura aplicada no template para transformar dados visuais (`{{ valor | pipe }}`).
DatePipe :: Pipe embutido para formatar datas e horas em diferentes padrões e locais.
CurrencyPipe :: Pipe embutido para conversão e exibição de valores monetários com símbolos locais.
PipeTransform :: Interface TypeScript obrigatória que define o contrato do método `transform()`.
Pure Pipe :: Pipe com cache automático que só executa quando a referência do dado de entrada muda.
:::

::: resumo
Pablo Marçal ensina que as irmãs do medo não devem ser temidas, mas compreendidas e transmutadas em ação. No Angular, os Pipes desempenham esse papel exato na interface: recebem dados brutos, complexos ou desformatados e os devolvem claros, elegantes e cheios de propósito para o usuário.
:::
