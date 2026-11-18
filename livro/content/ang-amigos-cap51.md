---
id: ang-amigos-cap51
order: 372
kind: capitulo
num: 268
group: Angular · Parte 68 · Liderança Técnica e o Ecossistema Amigos UI
title: Dê ao Desenvolvedor uma Reputação a Zelar
subtitle: Empacotamento com ng-packagr, Padrão FESM2022 e Tree-Shaking Perfeito
minutes: 30
hue: 215
badge: 🤝
image: img/ang-amigos/cap51.svg
- Analisar o caso da serviçal Marie na França e o poder de atribuir uma nobre reputação às pessoas: 
- Reconhecer que bibliotecas amadoras geram pacotes inchados que sobrecarregam o bundle final da aplicação: 
- Configurar o ng-packagr para compilar bibliotecas no padrão oficial FESM2022 (Flat ECMAScript Module): 
- Estruturar pontos de entrada secundários (Secondary Entry Points) como @amigos/ui/cdk e @amigos/ui/testing: 
- Habilitar Tree-Shaking de nível cirúrgico através da marcação "sideEffects": false no package.json
origem: angular/livros/03-amigos-angular/content/51-empacotamento-ng-packagr.md
---

::: ilustracao img/ang-amigos/cap51.svg | Terminal de compilação exibindo a execução bem-sucedida de ng build @amigos/ui com ng-packagr gerando pacotes FESM2022 otimizados para Tree-Shaking.
:::

Certo ano, uma distinta senhora da sociedade de Nova York contratou uma copeira e lavadeira francesa chamada **Marie** para trabalhar em seu hotel de campo nos arredores de Paris. Marie era uma jovem desanimada, desajeitada, com os cabelos desgrenhados e roupas manchadas; parecia carregar o peso do mundo nas costas e realizava suas tarefas de limpeza com desleixo manifesto.

A patroa pensou em demiti-la de imediato. No entanto, lembrando-se de um conselho sobre a psicologia humana, resolveu fazer um experimento de liderança. Chamou Marie à sua sala particular e, em vez de recriminá-la, olhou-a nos olhos e disse com profunda gravidade:

> *"Marie, você não sabe que tesouro existe dentro de você. Falei ontem com as pessoas que a conheciam em sua aldeia natal. Elas me disseram que você sempre foi uma moça honesta, trabalhadora, de coração nobre e de extremo capricho em tudo o que faz. Fiquei muito feliz em saber disso, pois é exatamente de uma pessoa com essa reputação que eu preciso para cuidar desta casa."*

Marie ficou atônita. Ninguém jamais havia falado com ela daquela maneira; ninguém jamais havia lhe atribuído uma reputação tão honrosa. Aquelas palavras tocaram uma corda adormecida em sua alma.

A partir daquele instante, Marie transformou-se por completo. Lavou os cabelos, arrumou o vestido com esmero e passou a limpar os quartos e lustrar os talheres com um brilho que impressionou a todos os hóspedes. Ela tinha agora **uma reputação nobre a zelar**, e teria preferido morrer a decepcionar a fé que sua patroa havia depositado nela.

Dale Carnegie registra o antigo provérbio:

> *"Dê a um cão um bom nome se você quer que ele seja leal e valente. Se você quer que uma pessoa melhore em determinado aspecto, aja como se aquele traço particular já fosse uma de suas virtudes mais marcantes. Atribua-lhe uma reputação aberta e nobre para manter, e ela fará esforços prodigiosos para não desmerecer a sua confiança."*

---

## A Reputação Nobre de uma Biblioteca Angular

Traga esse princípio para a publicação e empacotamento da sua biblioteca no npm ou no registro corporativo privado da sua empresa.

Quando um desenvolvedor instala uma biblioteca desconhecida, qual é a primeira coisa que o time de arquitetura analisa? Eles analisam o **peso do pacote**, o **formato dos arquivos gerados** e o **impacto no bundle de produção**.

Se a biblioteca for amadora:
- Ela inclui código de testes dentro do pacote de produção;
- Ela exporta módulos CommonJS antigos que quebram o Tree-Shaking do Webpack/esbuild;
- Ela obriga o aplicativo a incluir dez componentes mesmo que o usuário só tenha importado um único botão;
- O bundle final incha 800 KB e a equipe técnica passa a olhar para aquele pacote com desconfiança e desprezo.

Por outro lado, quando o desenvolvedor instala a sua biblioteca e descobre que:
1. Os arquivos foram compilados com a ferramenta oficial **`ng-packagr`**;
2. Os módulos seguem o padrão **FESM2022** (Flat ECMAScript Module puro);
3. Todas as definições de tipos `.d.ts` estão perfeitamente extraídas com documentação TSDoc viva;
4. O arquivo `package.json` possui a flag `"sideEffects": false`, permitindo que o compilador do Angular elimine 100% do código não utilizado;

Esse desenvolvedor sente orgulho. Ele sabe que está utilizando uma ferramenta de classe mundial. Você concedeu a ele e ao projeto dele uma **reputação técnica nobre a zelar**.

---

## Arquitetura de Empacotamento com `ng-packagr`

O **`ng-packagr`** é a ferramenta canônica adotada pelo time do Angular e pela comunidade global para transformar projetos TypeScript em bibliotecas prontas para distribuição no formato APF (*Angular Package Format*).

Vejamos como estruturar o arquivo de configuração `ng-package.json`:

```json
// projects/amigos/ui/ng-package.json
{
  "$schema": "../../../node_modules/ng-packagr/ng-package.schema.json",
  "dest": "../../../dist/amigos/ui",
  "lib": {
    "entryFile": "src/public-api.ts",
    "cssUrl": "inline"
  }
}
```

E no `package.json` da biblioteca, declaramos com orgulho a ausência de efeitos colaterais para habilitar o **Tree-Shaking total**:

```json
// projects/amigos/ui/package.json
{
  "name": "@amigos/ui",
  "version": "1.0.0",
  "description": "Biblioteca amigável de componentes acessíveis e reativos inspirada em Dale Carnegie.",
  "keywords": ["angular", "signals", "cdk", "design-system", "a11y", "dx"],
  "peerDependencies": {
    "@angular/common": "^19.0.0",
    "@angular/core": "^19.0.0",
    "@angular/forms": "^19.0.0",
    "@angular/cdk": "^19.0.0"
  },
  "sideEffects": false
}
```

---

## Pontos de Entrada Secundários (*Secondary Entry Points*)

A marca máxima de refinamento de uma biblioteca profissional é a criação de **Pontos de Entrada Secundários**. Em vez de despejar tudo em um único arquivo gigante (`@amigos/ui`), dividimos a biblioteca em sub-módulos temáticos:
- `@amigos/ui` (Componentes Visuais Primários: Botões, Badges, Cards);
- `@amigos/ui/cdk` (Diretivas Avançadas e Utilitários de Foco e Overlays);
- `@amigos/ui/testing` (Harnesses de Testes Automatizados para o Testbed).

Para criar o ponto de entrada secundário de testes, basta criar uma pasta `testing` com seu próprio arquivo `ng-package.json`:

```json
// projects/amigos/ui/testing/ng-package.json
{
  "$schema": "../../../../node_modules/ng-packagr/ng-package.schema.json",
  "lib": {
    "entryFile": "src/index.ts"
  }
}
```

E o arquivo exportador `projects/amigos/ui/testing/src/index.ts`:

```typescript
// projects/amigos/ui/testing/src/index.ts
export * from './amg-dialog.harness';
export * from './amg-select.harness';
export * from './amg-button.harness';
```

---

## O Resultado no Terminal de Compilação

Quando rodamos a compilação:

```bash
$ ng build @amigos/ui --configuration production
```

O `ng-packagr` gera a estrutura de distribuição de elite:

```text
Building Angular Package
------------------------------------------------------------------------------
Compiling TypeScript sources through ngc
Bundling to FESM2022
Extracting typings to index.d.ts
✔ Built @amigos/ui (FESM2022: 24.2 kB, Gzip: 6.8 kB)
✔ Built @amigos/ui/testing (FESM2022: 8.4 kB)
------------------------------------------------------------------------------
Built Angular Package!
- Output folder: dist/amigos/ui
- Total compile time: 1.14s
```

Quando um desenvolvedor importa apenas o botão:

```typescript
import { AmgButtonComponent } from '@amigos/ui';
```

O bundler do Angular (esbuild) inspeciona o `"sideEffects": false`, percebe que o diálogo e a tabela não foram referenciados e **descarta todos os outros arquivos**, adicionando míseros 2 kB ao pacote final do cliente.

Você deu à sua biblioteca e ao desenvolvedor que a consome uma reputação irretocável de excelência técnica. Como Marie nos arredores de Paris, esse desenvolvedor defenderá o uso da sua biblioteca com orgulho e lealdade por toda a vida.

---

::: quiz
? Qual foi a transformação operada na serviçal Marie após sua patroa atribuir-lhe uma nobre reputação de capricho e lealdade?
+ Marie assumiu aquela virtude como sua maior honra, melhorando seu asseio e tornando-se a trabalhadora mais caprichosa da casa para zelar por seu bom nome.
- Marie pediu demissão para abrir seu próprio restaurante.
- Marie passou a trabalhar menos horas fingindo cansaço.
- Marie viajou para a Inglaterra para estudar diplomacia.
> Dale Carnegie demonstra que quando atribuímos publicamente uma virtude nobre a alguém, a pessoa se esforça de maneira prodigiosa para não decepcionar essa expectativa.

? Para que serve a ferramenta oficial `ng-packagr` no ecossistema de desenvolvimento Angular?
+ Para compilar bibliotecas TypeScript no formato padronizado Angular Package Format (APF), gerando bundles FESM e tipagens d.ts limpas.
- Para zipar arquivos em formato RAR com senha criptografada.
- Para hospedar sites gratuitamente em servidores da Universidade de Oxford.
- Para transcrever comentários de código em arquivos de áudio gravados.
> O `ng-packagr` automatiza todos os passos complexos de compilação necessários para que uma biblioteca possa ser consumida via npm.

? Qual é o benefício da flag `"sideEffects": false` no arquivo `package.json` de uma biblioteca?
+ Informa aos empacotadores (como esbuild e Webpack) que nenhum arquivo causa efeitos colaterais globais, habilitando a eliminação total de código morto (Tree-Shaking).
- Desativa a validação de tipos do compilador TypeScript.
- Impede que usuários com monitores antigos instalem o pacote.
- Desliga os alarmes de segurança do servidor de CI/CD.
> Sem o `"sideEffects": false`, os bundlers são forçados a incluir trechos de código por receio de quebrar execuções globais na importação.

? O que são *Secondary Entry Points* em uma biblioteca Angular empacotada com `ng-packagr`?
+ Sub-módulos independentes acessíveis por caminhos específicos (como `@amigos/ui/testing`), garantindo separação limpa de código de produção e de teste.
- Portas de entrada secretas para acesso de administradores do sistema.
- Rotas de fuga do prédio da empresa em caso de incêndio.
- Links alternativos para download de fotos em alta resolução.
> Os pontos de entrada secundários isolam código especializado (como utilitários de teste), impedindo que ele seja importado acidentalmente no bundle principal.

? O que caracteriza o formato de distribuição FESM2022 (Flat ECMAScript Module)?
+ Um único arquivo JavaScript achatado (*flat*) contendo código ES Modules moderno, sem wrappers desnecessários e com alta performance de carregamento.
- Um tipo de arquivo executável compatível apenas com o Windows 95.
- Uma linguagem de programação alternativa desenvolvida pela NASA.
- Um protocolo de envio de e-mails corporativos.
> O formato FESM2022 é o padrão ouro moderno para ecossistemas JavaScript, maximizando o desempenho do motor V8 e facilitando a otimização de bundles.
:::

---

::: cartas
Marie da França :: Trabalhadora doméstica que transformou seu padrão de qualidade ao receber uma nobre reputação para honrar.
Reputação a Zelar :: Princípio de Dale Carnegie: atribua uma virtude pública a alguém e a pessoa fará prodígios para merecê-la.
ng-packagr :: Ferramenta oficial recomendada pelo Angular para geração de pacotes no padrão APF (Angular Package Format).
FESM2022 :: Padrão de bundle moderno (Flat ECMAScript Module) que otimiza o empacotamento em módulos ES puros.
Tree-Shaking :: Processo de análise estática que elimina do bundle de produção todo código que não foi expressamente importado.
sideEffects: false :: Declaração crucial de package.json que garante que módulos não causam mutações globais ao serem avaliados.
:::
