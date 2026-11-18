---
id: lab-anfx-estrutura
order: 263
kind: lab
num: 30
group: Laboratório · Parte 42 · Banco ANFX: Clean Architecture na prática
title: Clean Architecture na prática
subtitle: Antes dos módulos bancários, vamos entender a estrutura que já está no projeto.
minutes: 10
hue: 304
origem: aula_guiada_mcroservicos_net_ia/portal-anfx/dist/lessons.js
---

Antes dos módulos bancários, vamos entender a estrutura que já está no projeto. O exemplo de produtos nos permite seguir uma requisição da API ao PostgreSQL sem confundir organização interna com microserviços.

::: nota Nota
**Estado atual:** há uma API executável, três bibliotecas e dois projetos de testes com métodos vazios. O exemplo de produtos não é um domínio do Banco ANFX. Ele será substituído ao longo das próximas implementações.
:::

## Camadas não são serviços

API, Application, Domain e Infrastructure fazem parte da mesma aplicação. Um futuro serviço pode usar essa organização internamente. Criar quatro projetos de código não cria quatro unidades independentes de implantação.

> **Diagrama.** Fluxo atual: requisição HTTP chega ao ProductsController, passa por ProductService, Product e ProductRepository e persiste no PostgreSQL.

- **HTTP · POST /api/Products**
- **API · ProductsController** — Recebe o contrato HTTP e devolve a resposta
- **Application · ProductService** — Orquestra criação e persistência
- **Domain · Product** — Nome obrigatório, preço não negativo
- **Infrastructure · ProductRepository** — EF Core, AppDbContext e Npgsql
- **PostgreSQL** — Conexão configurada; schema e disponibilidade precisam ser verificados

| Camada | Responsabilidade presente | Referências de projeto |
|---|---|---|
| Domain | Product, Money e IProductRepository | Nenhuma outra camada |
| Application | ProductService, DTOs e IProductService | Domain |
| Infrastructure | EF Core, PostgreSQL e repositório | Application e Domain |
| API | Controllers, Swagger e composição das dependências | Application e Infrastructure |

## Siga uma criação de produto

EXEMPLO QUE EXISTE NO CÓDIGO

```
POST /api/Products
Content-Type: application/json

{
  "name": "Exemplo da estrutura inicial",
  "price": 25.50
}
```

O controller chama `CreateAsync`. O serviço instancia `Product`, adiciona pelo repositório e salva as alterações. Em uma execução bem-sucedida com banco preparado, a API devolve `201 Created` e a localização do recurso. O exemplo foi lido no código; não é um teste de execução aprovado.

## Onde encontrar no projeto

CAMINHOS RELATIVOS À SOLUÇÃO .NET

{{files}}

## Por que essas tecnologias?

### ASP.NET Core / .NET 10

É a base já utilizada para expor HTTP e compor dependências. Facilita manter a trilha em C#. Não define sozinho o limite dos serviços nem garante resiliência.

### EF Core + Npgsql + PostgreSQL

O ORM reduz código de persistência; Npgsql integra ao PostgreSQL. O custo inclui entender consultas geradas, migrations, concorrência e transações. SQL e índices continuam relevantes.

### Contratos e injeção de dependência

O ProductService depende de IProductRepository. Isso permite substituir a persistência em verificações. Mais interfaces também trazem indireção: cada uma deve ter uma função clara.

### Swagger

Documenta e permite explorar os endpoints em Development. Ajuda a descobrir o contrato HTTP, mas não substitui testes nem explicações das regras de negócio.

## O que ainda falta verificar ou construir

- Validações coerentes com os limites do banco: o nome tem máximo de 200 caracteres no mapeamento, mas não no construtor.
- Conversão de erros de domínio em respostas HTTP adequadas: as exceções atuais não têm tratamento próprio.
- Migrations e instruções reproduzíveis de preparação do PostgreSQL.
- Testes com assertivas: os dois métodos Test1 atuais estão vazios.
- Clientes, contas e regras bancárias: ainda não existem na solução.
- Integração com IA: o nome da solução contém IA, mas ainda não há implementação.

::: nota Nota
**Sobre a validação:** a tentativa anterior de compilação no ambiente restrito falhou sem diagnóstico de compilador. Não há confirmação de build, execução da API ou integração com PostgreSQL. As informações desta aula se baseiam na leitura dos arquivos.
:::


## Arquivos para abrir

- **Composição da aplicação** — `src/aula_guiada_mcroservicos_net_ia.Api/Program.cs`
- **Entrada HTTP e resposta de criação** — `src/aula_guiada_mcroservicos_net_ia.Api/Controllers/ProductsController.cs`
- **Orquestração do caso de uso** — `src/aula_guiada_mcroservicos_net_ia.Application/Services/ProductService.cs`
- **Regras atuais da entidade** — `src/aula_guiada_mcroservicos_net_ia.Domain/Entities/Product.cs`
- **Contrato de persistência** — `src/aula_guiada_mcroservicos_net_ia.Domain/Interfaces/IProductRepository.cs`
- **Implementação do repositório** — `src/aula_guiada_mcroservicos_net_ia.Infrastructure/Repositories/ProductRepository.cs`
- **Mapeamento dos dados** — `src/aula_guiada_mcroservicos_net_ia.Infrastructure/Data/AppDbContext.cs`

::: quiz
? A solução tem quatro projetos de camadas. Isso significa que temos quatro microserviços?
- Sim, qualquer projeto .csproj é um microserviço.
+ Não. As camadas compõem uma aplicação; os serviços precisam de limites e implantação próprios.
- Sim, desde que todos usem PostgreSQL.
> A estrutura atual tem uma API executável e bibliotecas que ela utiliza. Clean Architecture organiza responsabilidades internas. A distribuição exige decisões adicionais sobre contratos, dados e operação.
:::
