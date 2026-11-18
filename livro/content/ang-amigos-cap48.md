---
id: ang-amigos-cap48
order: 369
kind: capitulo
num: 265
group: Angular · Parte 67 · Conquistando a Equipe sem Conflitos de Estado
title: Deixe a Ideia Ser Dele
subtitle: Testes Desacoplados e Resilientes com CDK Component Harnesses
minutes: 30
hue: 215
badge: 🤝
image: img/ang-amigos/cap48.svg
- Analisar o caso real de Adolph Seltz e o desenhista de estampas de Nova York sobre a arte da co-criação: 
- Identificar a fragilidade dos testes unitários tradicionais acoplados a seletores CSS internos (By.css('.btn-v2')): 
- Compreender a arquitetura de Component Harnesses do pacote @angular/cdk/testing: 
- Criar um Harness oficial para o componente AmgDialog (AmgDialogHarness) expondo métodos semânticos de domínio: 
- Executar testes automatizados no Testbed sem quebrar quando a estrutura interna de estilos do componente for refatorada: 
origem: angular/livros/03-amigos-angular/content/48-testes-com-component-harnesses.md
---

::: ilustracao img/ang-amigos/cap48.svg | VS Code exibindo teste unitário limpo com TestbedHarnessEnvironment executando métodos semânticos do AmgDialogHarness com 100% de passagem no terminal.
:::

Durante anos a fio, um talentoso desenhista comercial da Filadélfia chamado **Adolph Seltz** tentou vender ilustrações e esboços de moda para um dos maiores estilistas e compradores de tecidos de Nova York. Seltz visitava o escritório do cliente todas as semanas; o comprador examinava os desenhos finalizados com ar de enfado e dizia invariavelmente: *"Não, Adolph. Isso não é bem o que estamos procurando hoje"*.

Após dezenas de fracassos frustrantes, Seltz decidiu estudar a psicologia das relações humanas. Perguntou a si mesmo: *"Por que continuo insistindo em enfiar os meus gostos goela abaixo desse homem? Não seria infinitamente mais inteligente convidá-lo a colocar as ideias dele nos meus desenhos?"*.

Seltz pegou uma pasta de rascunhos inacabados — meros traços a lápis sem cores definitivas —, viajou até Nova York e entrou na sala do comprador. Desta vez, não tentou vender nada. Colocou as folhas sobre a mesa e disse:

> *"Sr. X, gostaria de lhe pedir um pequeno favor. Aqui estão alguns esboços incompletos de estampas. O senhor teria a gentileza de me dizer como nós poderíamos terminá-los de modo que se tornassem úteis para as suas novas coleções?"*

O comprador olhou os esboços com interesse renovado. Pela primeira vez, ninguém estava tentando vender-lhe algo pronto; estavam pedindo o seu conselho de mestre. *"Deixe-os aqui comigo por alguns dias, Adolph"*, disse ele.

Três dias depois, Seltz retornou. O cliente fez sugestões detalhadas: *"Coloque um tom mais pastel aqui, arredonde esta curva, aumente o espaçamento desta borda"*. Seltz levou os desenhos para seu estúdio, finalizou-os rigorosamente de acordo com as instruções do cliente e voltou a Nova York.

O resultado? **Todas as ilustrações foram compradas na hora**. A partir daquele dia, o cliente passou a encomendar centenas de desenhos exclusivamente com Adolph Seltz. O comprador sentia que as ideias eram dele — e, de fato, eram!

Dale Carnegie destaca:

> *"Não confia você muito mais nas ideias que descobriu por si mesmo do que naquelas que lhe são empurradas numa bandeja de prata? Se é assim, não é um erro crasso tentar impor as suas opiniões aos outros? Deixe a outra pessoa sentir que a ideia pertence a ela. Consulte-a, peça seus conselhos e faça-a sentir-se co-autora da solução."*

---

## O Pesadelo dos Testes com `By.css('.btn-azul-antigo')`

Traga essa lição luminosa para o ecossistema de testes unitários no Angular.

Imagine que uma equipe consome a sua biblioteca de botões e diálogos. Nos testes automatizados deles, para testar se um diálogo abre e confirma um pedido, os programadores são forçados a escrever coisas como:

```typescript
// O Teste Frágil e Acoplado (O "Desenho Pronto na Bandeja")
it('deve confirmar o diálogo', async () => {
  const botao = fixture.debugElement.query(By.css('.amg-btn-primary.btn-confirm-action-v1'));
  botao.nativeElement.click();
  fixture.detectChanges();

  const texto = fixture.debugElement.query(By.css('.dialog-body-txt > p:first-child'));
  expect(texto.nativeElement.textContent).toContain('Sucesso');
});
```

Esse teste é uma bomba-relógio. O desenvolvedor que consumiu sua biblioteca foi forçado a decorar suas classes CSS internas (`.amg-btn-primary.btn-confirm-action-v1`). 

Basta você, autor da biblioteca, fazer uma pequena refatoração visual — trocando o nome da classe para `.amg-btn-solid` ou reorganizando a hierarquia do HTML —, e **trezentos testes unitários na aplicação da empresa quebram instantaneamente**.

A equipe consumidora fica revoltada: *"Toda vez que a biblioteca Amigos UI atualiza, nossos testes quebram! Não dá para confiar nesse pacote!"*.

---

## A Co-autoria com `@angular/cdk/testing` Component Harnesses

A solução madura e amiga do desenvolvedor é fornecer um **Component Harness oficial**.

O padrão de *Harness* (introduzido pelo Angular CDK no pacote `@angular/cdk/testing`) atua como uma API oficial de automação para testes. Ele esconde a fragilidade dos seletores CSS internos e expõe métodos limpos orientados ao domínio:
- `dialog.isOpen()`
- `dialog.confirm()`
- `dialog.getCloseButton()`
- `dialog.getTitleText()`

O consumidor escreve os testes na linguagem dele, e você, autor do componente, mantém o controle dos seletores dentro do Harness. Se o seu HTML interno mudar, você atualiza o Harness na biblioteca e **zero testes quebram no projeto do consumidor**.

Vejamos como codificar o `AmgDialogHarness`:

```typescript
// testing/amg-dialog.harness.ts
import { ComponentHarness } from '@angular/cdk/testing';

/**
 * Harness oficial para testes do componente AmgDialog.
 * Desacopla quem testa das classes CSS internas da biblioteca.
 */
export class AmgDialogHarness extends ComponentHarness {
  /** Seletor raiz do componente no DOM */
  static hostSelector = 'amg-dialog';

  /** Localizadores privados para elementos internos */
  private locateBackdrop = this.locatorForOptional('.amg-dialog-backdrop');
  private locatePanel = this.locatorForOptional('.amg-dialog-panel');
  private locateCloseBtn = this.locatorForOptional('.amg-dialog-close');
  private locateContent = this.locatorFor('.amg-dialog-content');

  /**
   * Verifica se o diálogo está aberto no momento.
   */
  async isOpen(): Promise<boolean> {
    const backdrop = await this.locateBackdrop();
    return backdrop !== null;
  }

  /**
   * Obtém o texto completo contido no corpo do diálogo.
   */
  async getContentText(): Promise<string> {
    const content = await this.locateContent();
    return content.text();
  }

  /**
   * Clica no botão de fechar (X), caso esteja presente.
   */
  async clickCloseButton(): Promise<void> {
    const btn = await this.locateCloseBtn();
    if (!btn) {
      throw new Error('O botão de fechar não está presente no diálogo.');
    }
    await btn.click();
  }

  /**
   * Clica no fundo escuro (backdrop) para simular o fechamento externo.
   */
  async clickBackdrop(): Promise<void> {
    const backdrop = await this.locateBackdrop();
    if (backdrop) {
      await backdrop.click();
    }
  }
}
```

---

## Como o Desenvolvedor Testa a Aplicação Dele

Agora veja o teste unitário escrito pelo time consumidor utilizando o `TestbedHarnessEnvironment`:

```typescript
// perfil-usuario.component.spec.ts
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { TestbedHarnessEnvironment } from '@angular/cdk/testing/testbed';
import { PerfilUsuarioComponent } from './perfil-usuario.component';
import { AmgDialogHarness } from '@amigos/ui/testing';

describe('PerfilUsuarioComponent', () => {
  let fixture: ComponentFixture<PerfilUsuarioComponent>;
  let loader: any;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PerfilUsuarioComponent]
    }).compileComponents();

    fixture = TestBed.createComponent(PerfilUsuarioComponent);
    fixture.detectChanges();
    
    // Cria o carregador de Harnesses oficial do Angular CDK
    loader = TestbedHarnessEnvironment.loader(fixture);
  });

  it('deve abrir o diálogo de confirmação e exibir mensagem clara', async () => {
    // Busca o harness sem tocar em classes CSS!
    const dialog = await loader.getHarness(AmgDialogHarness);

    // O desenvolvedor expressa a intenção com elegância
    expect(await dialog.isOpen()).toBe(true);
    expect(await dialog.getContentText()).toContain('Deseja enviar este elogio?');

    // Executa a ação do usuário
    await dialog.clickCloseButton();

    expect(await dialog.isOpen()).toBe(false);
  });
});
```

O desenvolvedor sente que a ideia de teste é dele. Ele escreveu um teste legível, robusto e à prova de quebras futuras. 

Você não lhe impôs um desenho finalizado e rígido; você forneceu a moldura de co-autoria que Adolph Seltz levou ao comprador de Nova York. A biblioteca torna-se uma parceira indispensável para sempre.

---

::: quiz
? Qual foi a virada de estratégia que permitiu a Adolph Seltz vender dezenas de estampas para o estilista de Nova York?
+ Levou rascunhos inacabados e pediu a opinião e os conselhos do cliente sobre como terminá-los, fazendo-o sentir-se co-autor do projeto.
- Abaixou os preços de seus desenhos em 90% para quebrar a concorrência.
- Ameaçou abrir uma fábrica de tecidos concorrente na Filadélfia.
- Contratou detetives particulares para espionar os armários do cliente.
> Seltz compreendeu que seres humanos valorizam infinitamente mais as ideias em que participaram do que produtos prontos empurrados à força.

? Por que testes unitários baseados em `By.css('.classe-css-interna')` são considerados frágeis e perigosos?
+ Porque criam acoplamento rígido com detalhes de implementação e estilização visual, quebrando os testes a cada refatoração cosmética.
- Porque causam lentidão na conexão de internet da empresa.
- Porque o TypeScript não aceita a palavra `css` em arquivos `.spec.ts`.
- Porque exigem a instalação do Photoshop no servidor de integração contínua.
> Testes frágeis aumentam o custo de manutenção e geram desconfiança na equipe sobre a estabilidade do pacote.

? Qual é o principal papel dos Component Harnesses do `@angular/cdk/testing`?
+ Fornecer uma API oficial, estável e orientada ao domínio para automação de testes, isolando o consumidor das classes e estruturas do DOM interno.
- Substituir o framework de testes Jasmine pelo Microsoft Word.
- Gerar relatórios de cobertura em arquivos de áudio MP3.
- Cancelar os testes que demorarem mais de 2 segundos.
> Os Harnesses atuam como uma camada de abstração elegante (semelhante ao padrão Page Object) para testes unitários de componentes.

? O que o método `TestbedHarnessEnvironment.loader(fixture)` faz na suíte de testes?
+ Instancia um carregador capaz de localizar e inicializar qualquer ComponentHarness dentro da árvore de componentes do Testbed.
- Realiza o download de arquivos pesados da nuvem da Amazon.
- Recarrega a página do navegador em um loop contínuo.
- Formata o disco rígido caso haja falhas no teste.
> O loader pesquisa os nós do DOM e monta as instâncias dos harnesses correspondentes para os testes.

? O que acontece quando você refatora o HTML interno de um componente que fornece um Component Harness oficial?
+ Você atualiza os localizadores apenas dentro do arquivo do Harness na biblioteca, e todos os testes dos consumidores continuam passando normalmente.
- Todos os repositórios Git da empresa são travados por segurança.
- O Angular CLI reverte a refatoração para a versão anterior.
- É necessário demitir os analistas de qualidade da empresa.
> O Harness absorve o impacto da refatoração interna, garantindo a promessa de backward-compatibility para as suítes de testes dos clientes.
:::

---

::: cartas
Adolph Seltz :: Desenhista comercial da Filadélfia que aprendeu a vender envolvendo o cliente na concepção e acabamento de suas ideias.
A Ideia Ser Dele :: Princípio de Dale Carnegie: consulte as pessoas e permita que elas sintam que a solução partiu de sua própria inteligência.
Component Harnesses :: Padrão do Angular CDK para automação de testes desacoplada de seletores CSS e do DOM interno.
TestbedHarnessEnvironment :: Ambiente de execução que conecta testes unitários do Angular TestBed aos Harnesses de componentes.
Page Object Pattern :: Padrão arquitetural que encapsula a interação com elementos visuais por trás de métodos de domínio.
Testes Resilientes :: Testes unitários que permanecem verdes mesmo após refatorações profundas de estilo e estrutura HTML.
:::
