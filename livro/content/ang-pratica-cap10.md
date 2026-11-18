---
id: ang-pratica-cap10
order: 285
kind: capitulo
num: 195
group: Angular · Parte 48 · Dados e integração
title: HTTP e fronteiras com serviços externos
subtitle: Uma chamada de rede é uma operação que pode demorar, falhar, ser repetida ou devolver dados inesperados.
minutes: 55
hue: 210
objectives:
- Configurar `HttpClient` e separar acesso remoto da apresentação
- Modelar loading, sucesso, vazio e falha
- Usar interceptors sem esconder erros e validar respostas externas
origem: angular/livros/01-biblioteca-angular/content/10-cap10.md
---

::: ilustracao img/ang-pratica/estados-http.svg | Uma requisição tem estados distintos: carregamento, resultados, vazio, falha ou cancelamento.
:::

## Quando o catálogo deixa de ser local

O `HttpClient` pode ler `/api/livros` e retornar um `Observable`. Configure-o com `provideHttpClient()` em `app.config.ts`, crie um serviço de dados e mantenha o componente focado na apresentação. A resposta JSON é dado externo; um tipo TypeScript não valida o conteúdo que chegou.

```ts
import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { Livro } from './livro';

@Injectable({ providedIn: 'root' })
export class LivrosApi {
  private readonly http = inject(HttpClient);

  listar(): Observable<readonly Livro[]> {
    return this.http.get<readonly Livro[]>('/api/livros');
  }
}
```

Registre `provideHttpClient()` nos providers da aplicação antes de injetar `LivrosApi`. Esse snippet demonstra o formato da integração, não acrescenta endpoint fictício ao app. Quando existir servidor, teste contrato, tipos em runtime, autenticação e erros.

Um `HttpClient.get<Livro[]>()` fornece uma expectativa ao compilador, mas não prova que o servidor enviou essa forma. Em aplicações com risco real, valide a resposta em runtime e devolva um erro de domínio compreensível.

## Quatro estados de tela

Uma tela remota precisa lidar com pelo menos: carregando, sucesso com resultados, sucesso vazio e falha. Um retry só é apropriado para erros transitórios e operações idempotentes. Não repita pagamentos, criação de conta ou qualquer comando sem uma estratégia de idempotência.

`HttpInterceptorFn` pode aplicar cabeçalhos, rastreamento e tratamento transversal. Interceptors não devem transformar qualquer erro em uma lista vazia: isso apaga a diferença entre “não há livros” e “o servidor não respondeu”.

::: passos Fluxo simples de leitura remota
Pessoa abre catálogo; a página mostra carregamento.
===
O repositório consulta a API e valida a resposta.
===
Sucesso mostra livros; vazio mostra orientação; falha mostra recuperação.
:::

## API honesta e interfaces cuidadosas

Uma mensagem útil não expõe stack trace, endereço interno ou token. Ela diz que a lista não carregou, preserva filtros e apresenta “Tentar novamente”. Para suporte, colete identificadores de diagnóstico não sensíveis e explique o que será enviado.

No protótipo local, a store é a fonte de dados. O capítulo descreve uma evolução; ela requer backend, contrato de API, autenticação e autorização próprios. Não há modo de fazer o navegador virar um banco confiável.

::: quiz
? O que representa `HttpClient.get<Livro[]>()`?
+ Uma expectativa estática para o compilador; a resposta de rede ainda precisa ser tratada como dado externo.
- Uma validação de runtime que confirma todos os campos do JSON.
- Uma garantia de que o servidor está disponível.
:::

::: cartas
Quais estados básicos a página de HTTP deve distinguir? :: Carregando, sucesso com dados, vazio e falha.
Por que não converter toda falha em lista vazia? :: Porque esconde a diferença entre ausência de dados e indisponibilidade, impedindo uma resposta útil.
:::
