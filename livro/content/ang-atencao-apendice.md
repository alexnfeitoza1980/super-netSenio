---
id: ang-atencao-apendice
order: 336
kind: apendice
group: Angular · Parte 58 · As oito semanas
title: "De C# para Angular: um mapa" (Atenção Plena em Angular)
subtitle: O que você já sabe do .NET, e onde isso aparece (ou não) no TypeScript e no Angular.
minutes: 20
hue: 270
badge: C#
objectives:
- Ter uma tabela de consulta rápida **C# / .NET → TypeScript / Angular**
- Conhecer as armadilhas mais comuns de quem chega do C#
origem: angular/livros/01-atencao-plena-angular/content/20-apendice.md
---

## Linguagem

| C# | TypeScript | Notas |
|---|---|---|
| `int`, `long`, `double`, `decimal` | `number` (e `bigint`) | Um só tipo numérico; cuidado com dinheiro (use centavos inteiros) |
| `string`, `char` | `string` | Não há `char` |
| `bool` | `boolean` | |
| `object` / `dynamic` | `unknown` / `any` | Prefira `unknown` e confira antes de usar; evite `any` |
| `T?` | `T \| undefined` (ou `\| null`) | Com `strict`, o compilador obriga a tratar |
| `var x = ...` | `const x = ...` / `let x = ...` | `const` por padrão |
| `readonly` (campo) | `readonly` | Também existe `readonly T[]` e `Readonly<T>` |
| `record` | `interface` / `type` + objetos imutáveis | Igualdade **por referência**, não por valor |
| `enum` | união de literais (`'a' \| 'b'`) ou `enum` | Uniões são mais comuns e mais leves |
| `switch` com padrão | `switch` + uniões discriminadas + `never` | Capítulo 2 |
| `is` / `as` | `typeof`, `instanceof`, predicados `x is T` / `as T` | `as` em TS **não converte**, só afirma |
| Genéricos `<T> where T : ...` | `<T extends ...>` | |
| `Func<T, R>`, `Action<T>` | `(x: T) => R`, `(x: T) => void` | Funções são valores comuns |
| LINQ (`Where`, `Select`, `Aggregate`) | `filter`, `map`, `reduce` | Imediatos (não preguiçosos) |
| `async Task<T>` / `await` | `async (): Promise<T>` / `await` | Igual na prática |
| `IObservable<T>` (Rx.NET) | `Observable<T>` (RxJS) | Mesma família, mesmos operadores |
| `event` | `output()` (componentes) / `Subject` (serviços) | |
| Atributos `[Algo]` | Decoradores `@Algo(...)` | Decoradores são funções |
| `namespace` | módulos ES (`import` / `export`) | Um arquivo é um módulo |
| `DateTime` / `DateOnly` | `Date` (e o `Temporal`, nos navegadores novos) | `Date` tem armadilhas: veja o capítulo 12 |

## Plataforma

| .NET / ASP.NET | Angular | Capítulo |
|---|---|---|
| `Program.cs` | `main.ts` + `app.config.ts` | 1 |
| `services.AddSingleton<T>()` | `@Injectable({ providedIn: 'root' })` | 8 |
| Injeção por construtor | `inject(T)` | 8 |
| `IOptions<T>` | `InjectionToken<T>` com fábrica | 8, 9 |
| `TimeProvider` | um token como `RELOGIO` | 8, 12 |
| Razor / Blazor `.razor` | template do componente | 1, 6 |
| `@if`, `@foreach` do Razor | `@if`, `@for` | 6 |
| Rotas de controller `"{id}"` | `path: 'x/:id'` + input | 7 |
| Filtros de autorização | Guardas (só interface; segurança é no servidor) | 7 |
| `HttpClient` + `DelegatingHandler` | `HttpClient` / `httpResource` + interceptores | 9 |
| Polly | `retry({ count, delay })` com critério | 9 |
| DataAnnotations | esquema do Signal Forms | 10 |
| `INotifyPropertyChanged` | `signal` / `computed` | 3, 5 |
| xUnit `[Fact]` / `[Theory]` | Vitest `it` / `it.each` | 12 |
| Moq / NSubstitute | `vi.fn()` e `providers` no `TestBed` | 12 |
| `dotnet new` / `dotnet test` | `ng new` / `ng test` | 1, 12 |
| NuGet | npm | 1 |

## Armadilhas de quem chega do C#

::: erro As cinco mais comuns
1. **`==` em vez de `===`.** Em JavaScript, `==` converte tipos (`0 == ''` é `true`).
2. **`this` perdido.** Passar um método como callback (`onClick={this.salvar}` em outros frameworks, `setTimeout(this.tique)`) perde o `this`. Use arrow functions: `setTimeout(() => this.tique())`.
3. **Igualdade de objetos.** `{ a: 1 } === { a: 1 }` é `false`. Signals comparam por referência: crie objetos novos para avisar mudança (capítulo 4).
4. **`as` não converte.** `valor as number` não transforma texto em número; só cala o compilador. Para converter, `Number(texto)`.
5. **Números de ponto flutuante.** `0.1 + 0.2 !== 0.3`. Para dinheiro, use centavos inteiros ou uma biblioteca decimal.
:::
