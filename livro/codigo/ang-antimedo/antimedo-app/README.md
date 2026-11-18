# Antimedo App — Projeto Prático em Angular 17+

Projeto completo do livro **Antimedo em Angular** (Livro 1 da coleção Aprenda Angular).

## Tecnologias e Padrões Utilizados

- **Angular 17+ Standalone Components:** Zero dependência de `NgModules`
- **Reatividade com Signals:** `signal()`, `computed()` e `effect()` para gerenciamento de estado instantâneo
- **Formulários Reativos:** Validação rigorosa com `FormBuilder` e `Validators`
- **Pipes Customizados:** Diagnóstico visual com `PipeTransform`
- **Diretivas Customizadas:** Efeitos de destaque com `@HostBinding` e `@HostListener`
- **Injeção de Dependência:** Uso da função moderna `inject()`
- **Persistência Local:** Sincronização automática com `localStorage`
- **Testes Unitários:** Suíte de testes com Jasmine

## Como Executar

Use Node.js 18 ou superior:

```bash
npm ci
npm start
npm run test:ci
npm run build
```

Acesse `http://localhost:4200`. O aplicativo guarda os dados no `localStorage` do navegador.
