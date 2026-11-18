# O Tradutor de Empatia (`cnv-app`)

> Aplicação Angular 17+ Standalone baseada nos 4 pilares da **Comunicação Não-Violenta (CNV)** de Marshall B. Rosenberg.

## Sobre o Projeto

O **Tradutor de Empatia** recebe mensagens tensas, ríspidas ou carregadas de julgamento moralizante (comuns em e-mails corporativos, discussões de sprint ou pull requests) e as decompõe em quatro componentes essenciais:

1. **Observação Factual**: o que realmente ocorreu, despido de avaliações morais e generalizações absolutas.
2. **Sentimento Subjacente**: a vulnerabilidade ou dor real que motivou a agressão.
3. **Necessidade Não Atendida**: o valor universal legítimo que foi frustrado (clareza, pontualidade, segurança).
4. **Pedido Concreto**: solicitação afirmativa e realizável para reatar a colaboração e a paz.

## Tecnologias e Padrões Utilizados

- **Angular 17+ Standalone Components**: sem NgModules.
- **Signals**: reatividade síncrona, imutável e granular (`signal`, `computed`, `asReadonly`).
- **Reactive Forms**: validações e formulários reativos.
- **Angular CDK A11y**: regiões acessíveis com `aria-live`.
- **CSS Custom Properties**: Design Tokens com suporte a paletas acolhedoras.
- **Jasmine / Karma**: Testes de unidade automatizados com `TestBed`.

## Como Executar

```bash
npm ci
npm start
npm run test:ci
npm run build
```

Abra `http://localhost:4200`. Os testes são executados com Jasmine/Karma.
