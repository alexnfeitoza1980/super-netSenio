---
id: ang-amigos-cap49
order: 370
kind: capitulo
num: 266
group: Angular · Parte 68 · Liderança Técnica e o Ecossistema Amigos UI
title: Se Você é Obrigado a Descobrir Erros, Comece com Elogios
subtitle: Angular ESLint Educativo, Code Reviews Humanizados e Sugestões com 1 Clique
minutes: 30
hue: 215
badge: 🤝
image: img/ang-amigos/cap49.svg
- Analisar a histórica carta de Abraham Lincoln ao General Joseph Hooker durante a Guerra Civil Americana: 
- Compreender a psicologia de iniciar qualquer apontamento crítico com apreciação honesta e generosa: 
- Configurar regras personalizadas de Angular ESLint como guias educativos e não como barreiras punitivas: 
- Automatizar sugestões de refatoração no GitHub Pull Request com a sintaxe de 1-click fix: 
- Transformar a cultura de Code Review da equipe técnica de uma inquisição em uma mentoria acolhedora: 
origem: angular/livros/03-amigos-angular/content/49-eslint-educativo-code-review.md
---

::: ilustracao img/ang-amigos/cap49.svg | Interface de Pull Request no GitHub com comentário humanizado do bot de linter Amigos Mentor elogiando a arquitetura e oferecendo botão de 1-click apply suggestion.
:::

Em 1863, nos dias mais sombrios da Guerra Civil Americana, o presidente **Abraham Lincoln** enfrentava uma crise dramática de liderança militar. Os exércitos do Norte haviam sofrido sucessivas derrotas humilhantes; o general Ambrose Burnside fracassara fragorosamente na batalha de Fredericksburg. Lincoln precisava nomear um novo comandante para o Exército do Potomac.

O homem escolhido foi o general **Joseph "Fighting Joe" Hooker**. Mas Hooker possuía graves defeitos de temperamento: era vaidoso, impulsivo, conspirador e havia chegado a declarar publicamente aos jornais que o país e o exército necessitavam urgentemente de um *ditador militar*.

Como Lincoln lidou com a situação? Será que repreendeu Hooker asperamente? Será que o ameaçou com corte marcial?

Lincoln escreveu-lhe uma das cartas mais famosas e comoventes de toda a história política da humanidade. Observe como Lincoln iniciou a correspondência:

> *"General Hooker: Nomeei o senhor para o comando do Exército do Potomac. Fiz isso, naturalmente, por razões que considero suficientes. No entanto, creio que é melhor que o senhor saiba que há algumas coisas a seu respeito pelas quais não estou de todo satisfeito.*
> 
> *Acredito que o senhor é um soldado valente e hábil, o que, naturalmente, me agrada. Também acredito que o senhor não mistura política com sua profissão, no que está absolutamente certo. O senhor tem confiança em si mesmo, o que é uma virtude valiosa, senão indispensável. O senhor é ambicioso, o que, dentro de limites razoáveis, faz mais bem do que mal."*

Apenas após reconhecer e celebrar as virtudes genuínas de Hooker, Lincoln tocou com delicadeza no ponto crítico:

> *"O senhor disse recentemente que o exército e o governo necessitam de um ditador. Não foi por isso, mas apesar disso, que lhe dei o comando. Apenas os generais que obtêm sucessos militares podem tornar-se ditadores. O que exijo do senhor neste momento são vitórias; então arriscarei a ditadura.*
> 
> *E agora, cuidado com a precipitação. Não tenha pressa excessiva, mas avance com vigilância e dê-nos vitórias. Que Deus o proteja."*

Hooker leu a carta em lágrimas. Guardou o manuscrito em seu peito como o tesouro mais sagrado de sua vida e costumava mostrá-lo com orgulho aos seus amigos, dizendo: *"Essa é uma carta que só um verdadeiro pai poderia ter escrito a um filho"*.

Carnegie registra o princípio de liderança:

> *"É sempre mais fácil ouvir coisas desagradáveis depois que ouvimos elogios sinceros sobre os nossos próprios pontos fortes. O dentista começa aplicando a novocaína antes de iniciar a broca; o paciente ainda sentirá a broca, mas a anestesia elimina a dor intolerável. Comece sempre com um elogio honesto."*

---

## O Code Review Inquisitorial no Desenvolvimento Web

Como os times de desenvolvimento costumam conduzir seus processos de Code Review e automação de linters no GitHub e GitLab?

Costumam agir como inquisidores implacáveis:
- O programador passa três dias dedicando seu suor para construir uma tela complexa;
- Abre o Pull Request com entusiasmo;
- Minutos depois, um robô de CI bloqueia o merge e cospe:

```text
ESLint Rule Violation: @angular-eslint/no-input-rename (Error)
Linha 42: Você renomeou o input. Isso é expressamente proibido e reprova o build.
```

E logo abaixo, um colega sênior escreve um comentário ríspido de uma linha: *"Código confuso. Por que não usou Signals aqui? Refaça"*.

O desenvolvedor que abriu o PR fecha o notebook com o estômago embrulhado. Todo o seu esforço foi ignorado; seus méritos foram desconsiderados; ele foi reduzido a um criminoso que cometeu uma infração de trânsito sintática.

Isso destrói a confiança da equipe e cria um clima tóxico de medo.

---

## O Mentor Educativo: ESLint com Sugestões Construtivas

No ecossistema da biblioteca Amigos UI, transformamos o **Angular ESLint** em um mentor compassivo, como Abraham Lincoln.

Em vez de regras misteriosas com nomes opacos, configuramos regras personalizadas que:
1. Reconhecem o esforço e apontam a virtude do que foi feito;
2. Explicam **o porquê** da recomendação técnica;
3. Oferecem a correção automática (*autofix*) para aplicação imediata.

Vejamos como codificar uma regra personalizada de ESLint (`amg-prefer-model.ts`):

```typescript
// eslint-rules/amg-prefer-model.ts
import { Rule } from 'eslint';

export const preferModelRule: Rule.RuleModule = {
  meta: {
    type: 'suggestion',
    docs: {
      description: 'Sugere o uso do Signal Two-Way model() no lugar de duplas @Input/@Output manuais.',
      category: 'Best Practices',
      recommended: true
    },
    fixable: 'code',
    messages: {
      sugestaoCarnegie: 
        '👏 Excelente iniciativa! Vimos que você declarou "{{inputName}}" e seu evento correspondente. ' +
        'No Angular moderno, você pode unificar essa dupla usando o novo Signal "model()". ' +
        'Isso simplifica seu código e reduz boilerplate.'
    }
  },
  create(context) {
    return {
      ClassDeclaration(node) {
        // Lógica de análise AST para detectar @Input() foo e @Output() fooChange
        // Quando detectado, emite a mensagem encorajadora com o autofix pronto!
      }
    };
  }
};
```

---

## Pull Request Humanizado com 1-Click Fix no GitHub

Quando configuramos um bot de CI na equipe (como o `amigos-mentor-bot`), ele posta sugestões no PR formatadas em blocos de sugestão do Markdown do GitHub (` ```suggestion `).

Veja o resultado real na interface do PR:

```markdown
<!-- Comentário do amigos-mentor-bot no GitHub PR -->
### 👏 Belo trabalho nesta tela, Carla!
A separação dos componentes e o uso do ChangeDetectionStrategy.OnPush ficaram impecáveis.

Notamos apenas que a sincronização da propriedade `isOpen` pode se beneficiar do Signal bidirecional `model()`:

~~~suggestion
  readonly isOpen = model<boolean>(false);
~~~

💡 *Vantagem:* Reduz 4 linhas de código e previne bugs de descompasso de eventos. 
Basta clicar no botão **"Apply suggestion"** acima para aplicar diretamente ao branch!
```

O desenvolvedor vê seus méritos reconhecidos na primeira linha, entende a vantagem técnica na segunda linha e resolve a pendência com **um único clique**, sem atritos e sem mágoas.

Lincoln e Carnegie sabiam que a liderança autêntica não consiste em apontar o dedo; consiste em aplicar o bálsamo da valorização sincera antes de polir as imperfeições.

---

::: quiz
? Qual foi a atitude de Abraham Lincoln ao escrever a histórica carta de nomeação ao General Joseph Hooker?
+ Iniciou elogiando com generosidade e sinceridade a coragem, a habilidade militar e o patriotismo de Hooker antes de apontar seu desejo temerário por uma ditadura.
- Ordenou a prisão imediata de Hooker por traição à pátria.
- Nomeou o maior inimigo de Hooker para ser seu comandante em chefe.
- Concedeu a Hooker uma medalha de ouro em troca de sua renúncia.
> Lincoln aplicou a regra de ouro da liderança: anestesiou a crítica severa com a valorização honesta das qualidades reais do subordinado.

? Por que comentários secos e ríspidos em Code Reviews (como "Refaça, está confuso") prejudicam a equipe?
+ Ferem o senso de dignidade do desenvolvedor, geram medo de inovar e criam um clima corporativo tóxico e defensivo.
- Fazem o GitHub cobrar taxas adicionais por caracteres negativos.
- Desconfiguram o arquivo de chave pública SSH do desenvolvedor.
- Bloqueiam a conexão do computador com o banco de dados.
> Críticas desprovidas de empatia e contexto destroem a motivação dos engenheiros e fecham suas mentes para o aprendizado construtivo.

? Qual é o benefício dos blocos de sugestão (` ```suggestion `) nos Code Reviews do GitHub e GitLab?
+ Permitem que a refatoração sugerida pelo revisor seja visualizada em diff e aplicada ao branch com um único clique do desenvolvedor.
- Executam a compilação do projeto em servidores quânticos.
- Enviam um e-mail com áudio para o diretor da empresa.
- Deletam o repositório caso o autor não concorde com a mudança.
> O recurso de 1-click suggestion elimina o atrito manual e transforma a revisão em uma colaboração ágil e acolhedora.

? Como um linter de código (ESLint) pode atuar de forma educativa em vez de punitiva?
+ Explicando a razão técnica da recomendação na mensagem de diagnóstico e fornecendo correção automática (*fixable*) sempre que viável.
- Multiplicando o número de erros por dois a cada execução.
- Bloqueando o teclado do programador por 15 minutos.
- Publicando os erros dos estagiários nas redes sociais da empresa.
> Mensagens pedagógicas orientam o desenvolvedor sobre as melhores práticas sem humilhá-lo nem travar seu progresso desnecessariamente.

? Por que a anestesia do elogio sincero é indispensável antes de apontar ajustes técnicos?
+ Porque desarma a reação instintiva de defesa do ser humano, tornando a mente receptiva às sugestões de aprimoramento.
- Porque a legislação trabalhista proíbe correções de código sem elogios prévios.
- Porque elogios aumentam a taxa de compressão dos arquivos gzip.
- Porque faz o editor de código carregar mais rápido.
> Como Dale Carnegie ensina, quando o interlocutor sente que suas virtudes foram reconhecidas, ele acolhe a crítica construtiva com gratidão.
:::

---

::: cartas
Abraham Lincoln e Hooker :: Carta clássica da Guerra Civil onde Lincoln elogiou sinceramente o general antes de repreender sua vaidade.
Começar com Elogios :: Princípio de liderança de Dale Carnegie: elogie os pontos fortes antes de apontar oportunidades de melhoria.
Angular ESLint Educativo :: Configuração de linters com mensagens didáticas, links da documentação e correções automáticas.
1-Click Suggestion :: Recurso do GitHub que permite aceitar refatorações propostas no PR com um único clique.
Code Review Humanizado :: Prática de revisão de código focada no crescimento do colega e na celebração das boas soluções técnicas.
Anestesia Psicológica :: Efeito calmante da apreciação sincera que neutraliza a dor e o ressentimento de um apontamento crítico.
:::
