---
id: ang-amigos-cap43
order: 364
kind: capitulo
num: 260
group: Angular · Parte 67 · Conquistando a Equipe sem Conflitos de Estado
title: Respeite a Opinião Alheia
subtitle: Migrações Suaves, Depreciação Construtiva e Angular Schematics com ng update
minutes: 30
hue: 215
badge: 🤝
image: img/ang-amigos/cap43.svg
- Estudar o conselho de Lord Chesterfield ao filho e a revolução da diplomacia pessoal de Benjamin Franklin: 
- Compreender como a introdução de Breaking Changes sem aviso fere e desrespeita a equipe consumidora: 
- Implementar tags @deprecated ricas no TSDoc orientando caminhos de substituição seguros: 
- Criar um Schematic de migração automatizada para o comando ng update do Angular CLI: 
- Utilizar a árvore de arquivos do Schematics (Tree) e TypeScript AST para refatorar templates sem intervenção manual: 
origem: angular/livros/03-amigos-angular/content/43-migracoes-suaves-schematics.md
---

::: ilustracao img/ang-amigos/cap43.svg | Terminal executando ng update @amigos/ui com logs verdes de migração automática transformando sintaxes legadas em código Angular 19 moderno.
:::

O aristocrata e diplomata britânico **Lord Chesterfield** escreveu uma famosa série de cartas a seu filho com lições de elegância moral e liderança política. Em uma de suas passagens mais célebres, Chesterfield aconselhou o jovem:

> *"Seja mais sábio que os outros se puder; mas nunca diga isso a eles."*

Séculos antes, Sócrates repetia incessantemente aos seus discípulos em Atenas: *"Só sei que nada sei"*. No entanto, quantos de nós caímos na tentação arrogante de dizer aos nossos semelhantes, com todas as letras: *"Você está errado!"*?

Em sua clássica autobiografia, o estadista americano **Benjamin Franklin** revelou a extraordinária transformação de seu temperamento. Na juventude, Franklin era conhecido por ser um debatedor mordaz, sarcástico e agressivo: adorava encurralar seus interlocutores e provar publicamente os erros lógicos alheios. Percebendo que essa postura afastava as pessoas e tornava suas ideias impopulares, Franklin tomou uma decisão radical:

> *"Impeli a mim mesmo a regra de jamais contradizer abertamente a opinião dos outros e de nunca fazer afirmações categóricas ou dogmáticas. Bani do meu vocabulário palavras como 'certamente', 'indubitavelmente' ou 'com certeza absoluta'. No lugar delas, adotei expressões suaves: 'Eu imagino...', 'Segundo compreendo...', 'Parece-me neste momento...'. E quando alguém afirmava algo com que eu não concordava, eu não me precipitava a corrigir o seu erro; começava reconhecendo que, em certas circunstâncias, a visão dele seria perfeita, mas que no caso presente parecia haver alguma diferença."*

Franklin tornou-se o mais querido diplomata dos Estados Unidos, tendo sido o homem crucial para conquistar o apoio militar e financeiro da França na Guerra da Independência americana.

---

## O "Você Está Errado!" dos Mantenedores de Bibliotecas

Na indústria de software, o equivalente a gritar *"Você está errado!"* no rosto de um colega é **lançar uma nova versão da biblioteca que quebra silenciosamente metade das telas do sistema dele**.

O desenvolvedor abre o projeto na segunda-feira de manhã, roda `npm update`, e repentinamente trinta componentes deixam de compilar. As propriedades mudaram de nome, seletores foram renomeados, métodos sumiram do mapa e a documentação online diz com frieza burocrática:

```text
Breaking Change na v2.0:
Removemos [btnStyle] e (actionClick). Se você ainda usa isso, seu código é legado e está errado.
Reescreva seus templates para usar variant="" e (closed).
```

O autor da biblioteca age como o Franklin jovem: arroga-se a posição de sabichão e transfere todo o trabalho braçal da mudança para as costas dos seus companheiros de equipe. O resultado é óbvio: revolta, equipes congelando versões antigas do pacote e uma recusa tácita a qualquer inovação futura.

---

## O Respeito pelas Bases Instaladas: Depreciação Gentil

Um arquiteto maduro adota a diplomacia de Franklin. Ele reconhece que o código antigo das pessoas foi escrito de boa-fé e atende a negócios reais.

Quando precisamos evoluir uma API:
1. **Nunca removemos a propriedade bruscamente:** Mantemos a assinatura antiga operando por pelo menos uma versão maior (*Major*);
2. **Aplicamos `@deprecated` com a alternativa explícita:**

```typescript
// amg-button.component.ts
import { Component, input } from '@angular/core';

@Component({
  selector: 'amg-button',
  standalone: true,
  template: `<button [class]="variant()"><ng-content/></button>`
})
export class AmgButtonComponent {
  /**
   * Variante moderna do botão.
   */
  readonly variant = input<'primary' | 'secondary' | 'ghost'>('primary');

  /**
   * @deprecated Desde a versão 2.0.0. Use a propriedade `variant` no lugar.
   * Este suporte será mantido até a versão 3.0.0 para garantir sua tranquilidade.
   */
  readonly btnStyle = input<string | null, string | null>(null, {
    transform: (valorAntigo) => {
      // Mapeia suavemente o formato antigo para o novo sem quebrar o app do colega
      return valorAntigo;
    }
  });
}
```

Ao passar o mouse no template, o VS Code exibe o texto tachado (`~~btnStyle~~`) e avisa gentilmente: *"Use variant no lugar. Seu suporte será mantido até a v3.0.0 para sua tranquilidade"*. O desenvolvedor é informado com respeito, sem sobressaltos.

---

## O Milagre do `ng update`: Schematics de Migração Automática

Mas a forma máxima de respeito preconizada por Benjamin Franklin é: **em vez de pedir para o outro corrigir o código, forneça uma ferramenta que faça isso por ele automaticamente**.

O Angular CLI possui um mecanismo consagrado mundialmente para essa finalidade: o comando **`ng update`**.

Quando o consumidor digita:

```bash
$ ng update @amigos/ui
```

O CLI lê o arquivo `migration.json` da sua biblioteca e executa uma regra de **Schematics** que percorre a árvore de arquivos (`Tree`) do projeto dele, encontra as tags antigas e as refatora de forma cirúrgica.

Vejamos como codificar uma regra de migração do Schematics:

```typescript
// schematics/migration-v2/index.ts
import { Rule, SchematicContext, Tree } from '@angular-devkit/schematics';

/**
 * Regra que migra automaticamente <amg-button btnStyle="..."> para variant="..."
 */
export function migrateToV2(): Rule {
  return (tree: Tree, context: SchematicContext) => {
    context.logger.info('🤝 Amigos UI: Iniciando migração respeitosa para a versão 2.0...');

    let totalMigrados = 0;

    // Percorre todos os arquivos HTML do projeto consumidor
    tree.visit((filePath) => {
      if (!filePath.endsWith('.html')) return;

      const conteudoOriginal = tree.readText(filePath);
      
      // Procura ocorrências da sintaxe legada
      if (conteudoOriginal.includes('btnStyle=')) {
        // Substitui suavemente a propriedade
        const conteudoAtualizado = conteudoOriginal
          .replace(/\[btnStyle\]=/g, '[variant]=')
          .replace(/btnStyle=/g, 'variant=');

        tree.overwrite(filePath, conteudoAtualizado);
        totalMigrados++;
        context.logger.info(`  ✔ Atualizado com carinho: ${filePath}`);
      }
    });

    context.logger.info(
      `🎉 Migração concluída! ${totalMigrados} templates foram modernizados automaticamente sem dor de cabeça.`
    );

    return tree;
  };
}
```

---

## O Resumo no Arquivo de Migração

Para conectar esse script ao Angular CLI, registramos a migração no `package.json` e no `migration.json` da biblioteca:

```json
// migration.json
{
  "$schema": "../../node_modules/@angular-devkit/schematics/collection-schema.json",
  "schematics": {
    "migration-v2": {
      "version": "2.0.0",
      "description": "Atualiza automaticamente o uso de btnStyle para variant no Amigos UI",
      "factory": "./schematics/migration-v2/index#migrateToV2"
    }
  }
}
```

Quando você entrega uma biblioteca com esse nível de maturidade e cortesia, os desenvolvedores da sua organização não temem atualizações; pelo contrário, eles anseiam por novas versões porque sabem que a sua engenharia cuida deles como Benjamin Franklin cuidava dos diplomatas da Europa.

---

::: quiz
? Qual foi a grande lição de diplomacia pessoal que Benjamin Franklin adotou em sua autobiografia?
+ Eliminar a contradição direta, banir palavras dogmáticas como "certamente" e expressar opiniões com modéstia e respeito ao ponto de vista alheio.
- Imprimir panfletos anônimos com insultos aos membros do parlamento inglês.
- Convidar seus oponentes políticos para desafios de natação no rio Tâmisa.
- Abandonar a ciência e a política para viver isolado nas montanhas da Pensilvânia.
> Franklin compreendeu que a humildade verbal desarma resistências e permite que ideias frutifiquem sem ferir o ego dos ouvintes.

? Por que lançar Breaking Changes sem alternativas ou aviso prévio destrói a reputação de uma biblioteca?
+ Porque quebra o fluxo de trabalho de times inteiros, gera insegurança nas equipes e paralisa as atualizações de software por medo de prejuízos.
- Porque faz o Google encerrar o suporte ao navegador Chrome.
- Porque o protocolo TCP/IP proíbe alterações de código em dias de chuva.
- Porque o computador do usuário desliga automaticamente.
> Quebras abruptas demonstram falta de consideração pelos investimentos de tempo e código já realizados pelos consumidores da biblioteca.

? Qual é o propósito da anotação `@deprecated` no TSDoc do TypeScript?
+ Avisar aos desenvolvedores que determinado recurso será descontinuado no futuro, indicando a alternativa recomendada sem quebrar a compilação imediata.
- Deletar a função do disco rígido logo na primeira execução.
- Impedir que arquivos sejam commitados no repositório Git.
- Multiplicar por dois o tempo de build da aplicação.
> O `@deprecated` sinaliza visualmente a obsolescência no editor, oferecendo um período de transição confortável para os times.

? Como o comando `ng update` do Angular CLI auxilia na manutenção de aplicações corporativas?
+ Executa Schematics de migração automatizados que refatoram o código e os arquivos de configuração do projeto de forma transparente.
- Envia flores para o escritório dos desenvolvedores que concluíram suas tarefas.
- Reinicia os roteadores da empresa para limpar o cache da rede.
- Instala jogos de videogame nos computadores dos estagiários.
> O `ng update` transforma grandes migrações de versão em processos automatizados, rápidos e seguros.

? O que representa a abstração `Tree` dentro do ecossistema do Angular Schematics?
+ Uma representação virtual e encadeada do sistema de arquivos do projeto consumidor, permitindo ler, criar e alterar arquivos com segurança transacional.
- Um organograma hierárquico com as fotos dos diretores da empresa.
- Um algoritmo de ordenação de árvores binárias para banco de dados.
- Uma lista de árvores que foram salvas pela reciclagem de papel.
> O `Tree` permite que o Schematic aplique alterações em memória e só persista no disco se todas as regras forem concluídas com sucesso.
:::

---

::: cartas
Lord Chesterfield :: Diplomata britânico que aconselhou: "Seja mais sábio que os outros se puder; mas nunca diga isso a eles".
Benjamin Franklin :: Estadista que revolucionou sua influência pessoal substituindo a arrogância por modéstia e diplomacia.
Breaking Changes :: Mudanças incompatíveis em bibliotecas que forçam os consumidores a reescrever código sob pena de falha de build.
Anotação @deprecated :: Mecanismo padrão de documentação para alertar sobre descontinuações futuras com alternativas claras.
Angular Schematics :: Mecanismo oficial do Angular para automação de tarefas, geração de código e migração de versões.
Comando ng update :: Utilitário do CLI que atualiza dependências e executa scripts de migração automática do código-fonte.
:::
