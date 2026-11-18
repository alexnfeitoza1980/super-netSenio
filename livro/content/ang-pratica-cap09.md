---
id: ang-pratica-cap09
order: 284
kind: capitulo
num: 194
group: Angular · Parte 48 · Dados e integração
title: Formulários que ajudam a pessoa
subtitle: Validação explica como corrigir um dado sem humilhar quem está preenchendo.
minutes: 50
hue: 210
objectives:
- Escolher entre formulários template-driven, reativos e Signal Forms
- Validar campos, apresentar erros associados e preservar o estado
- Conhecer `ControlValueAccessor` para controles reutilizáveis
origem: angular/livros/01-biblioteca-angular/content/09-cap09.md
---

::: ilustracao img/ang-pratica/feedback-formulario.svg | Um erro claro é associado ao campo e permite corrigir sem perder o valor.
:::

## Três ferramentas, contextos diferentes

Formulários template-driven são convenientes para telas pequenas. Reactive Forms modelam explicitamente controles, grupos, validadores e streams de estado, sendo uma escolha madura para formulários complexos. Signal Forms, disponível nas versões atuais do Angular, organiza modelos e validação em torno de signals; confira documentação e estabilidade da versão exata antes de adotar em um produto com requisitos de suporte.

A Biblioteca Viva ainda não pede cadastro ou envio de dados. Não vamos criar um formulário só para dizer que usamos formulários. Um exercício opcional é adicionar uma sugestão de livro com `FormGroup`, campos título/autor, validação e estado de sucesso — sem enviar nada.

```ts
import { FormControl, FormGroup, Validators } from '@angular/forms';

readonly sugestao = new FormGroup({
  titulo: new FormControl('', { nonNullable: true, validators: [Validators.required, Validators.maxLength(120)] }),
  autor: new FormControl('', { nonNullable: true, validators: [Validators.required, Validators.maxLength(80)] }),
});
```

No template standalone, importe `ReactiveFormsModule` e conecte `[formGroup]="sugestao"` com `formControlName`. Mostre a mensagem depois de `touched` ou após tentativa de envio, associando-a com `aria-describedby`. Para uma caixa de busca simples, um signal pode ser suficiente; formulários são para um conjunto de valores, validação e estados de submissão.

## Validação sem culpa

Uma mensagem de campo útil identifica o problema e a correção: “Informe o título do livro” é melhor do que “Inválido”. Associe `label`, `id`, `aria-describedby` e uma indicação de erro. Não mostre erro antes de a pessoa ter uma oportunidade razoável de preencher o campo.

Validação no cliente melhora a experiência; o servidor deve validar novamente cada valor recebido. Nunca considere um formulário como controle de autorização.

## Estados do formulário

Pense em pristine/dirty, touched/untouched, valid/invalid, pending e submitted. Botão ocupado precisa de estado desabilitado e feedback. Se a operação falhar, preserve o que foi escrito e ofereça uma próxima ação. Não apague o formulário silenciosamente.

::: quiz
? A validação Angular elimina a necessidade de validar dados no servidor?
+ Não. O servidor sempre valida entradas e regras de negócio novamente.
- Sim, porque tipos TypeScript chegam intactos pela rede.
- Sim, se o campo tiver `required`.
:::

::: cartas
Quando preferir Reactive Forms? :: Quando a estrutura, validação e estado precisam ser explicitamente modelados e testados.
O que torna um erro de campo acessível? :: Rótulo associado, mensagem clara e associação programática como `aria-describedby`.
:::
