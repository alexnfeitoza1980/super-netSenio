---
id: lab-sns-seguranca
title: Antes e depois: a revisão de segurança do BankMore
subtitle: Os problemas encontrados numa API bancária de estudo, como cada um foi corrigido e como os testes provam que a correção funciona.
minutes: 30
hue: 12
parte: super-netSenior · da repetição à solução única
objectives:
- Reconhecer falhas clássicas: endpoint sem autenticação, **IDOR**, operação não atômica e segredo no repositório
- Relacionar cada falha à correção e ao teste que a protege
- Usar **teste de mutação manual** para confirmar que um teste realmente protege uma regra
---

::: vila A revisão
Uma API bancária de estudo funcionava no Swagger, mas uma leitura cuidadosa do código mostrou que **qualquer pessoa, sem token**, podia consultar saldos e transferir dinheiro. Este capítulo é o "antes e depois" dessa revisão.
:::

## O que foi encontrado

| # | Problema | Por que é grave |
|---|---|---|
| 1 | Transferência, movimento e saldo **sem `[Authorize]`** | Qualquer um, sem token, transfere e consulta |
| 2 | **IDOR**: a conta de origem vinha do corpo da requisição | Um usuário logado esvazia a conta de outro |
| 3 | Transferência com **vários `SaveChanges`** soltos | Falha no meio deixa débito sem crédito |
| 4 | Transferência sem checar saldo, conta ativa ou valor | Saldo negativo e movimentos inválidos |
| 5 | Idempotência "verifica e depois grava" | Duas requisições simultâneas passam as duas |
| 6 | Chave JWT e senha do banco **no repositório** | Quem tem o código forja tokens de qualquer usuário |
| 7 | Login diferenciava "usuário não encontrado" de "senha inválida" | Enumeração de contas cadastradas |
| 8 | CPF e e-mail dentro do JWT | Dado pessoal em um token que trafega e é logado |
| 9 | Vínculo usuário → conta pelo **nome** | Dois usuários com o mesmo nome compartilham acesso |

::: cuidado IDOR (Insecure Direct Object Reference)
Autenticar não basta. Se o servidor aceita **qualquer identificador** vindo do cliente (`idContaOrigem`) sem conferir se pertence ao usuário do token, o ataque é só trocar um número.
:::

## As correções

| # | Correção |
|---|---|
| 1 | `[Authorize]` em todos os controllers, exceto cadastro e login |
| 2 | O dono é decidido no servidor: `Conta.PertenceA(clienteId)`, com o `cliente_id` do JWT |
| 3 | Repositórios só rastreiam; **um** `CommitAsync` por operação |
| 4 | O use case valida saldo, conta ativa, valor positivo (até 2 casas) e origem ≠ destino |
| 5 | Índice único `(conta, chave)` no banco |
| 6 | A chave JWT vem **só** de variável de ambiente; a API **não sobe** com chave ausente ou fraca |
| 7 | Uma única mensagem para qualquer falha de login |
| 8 | O token leva apenas `sub`, `cliente_id` e `jti` |
| 9 | FK real `Usuario → Cliente` |

```csharp Falha rápida: a API recusa chave fraca
if (string.IsNullOrWhiteSpace(jwt.Key) || Encoding.UTF8.GetByteCount(jwt.Key) < JwtOptions.TamanhoMinimoChave)
    throw new InvalidOperationException(
        $"Configure Jwt:Key com pelo menos {JwtOptions.TamanhoMinimoChave} caracteres (variável de ambiente Jwt__Key).");
```

::: regra Falhe cedo
É melhor a aplicação **não subir** com uma configuração insegura do que subir e funcionar "quase certo". Um teste automatizado sobe a API sem chave e confere que ela recusa.
:::

## Como provar que a correção funciona

Um teste verde só vale se ele ficaria **vermelho** quando a regra quebra. Uma técnica barata é o **teste de mutação manual**: quebre a regra de propósito e veja se algum teste falha.

```csharp Mutação: desligar a checagem de dono
// antes
public bool PertenceA(Guid clienteId) => clienteId != Guid.Empty && ClienteId == clienteId;

// mutante
public bool PertenceA(Guid clienteId) => true;
```

Com o mutante, **seis** testes de segurança falharam (transferir, movimentar, consultar, Pix e resgate de conta alheia, mais o teste da própria regra). Depois o código foi restaurado. Se **nenhum** teste falhasse, a regra estaria sem proteção.

Além dos testes de use case com repositórios em memória, há testes que **sobem a API de verdade** (sem banco) e conferem que todo endpoint protegido responde **401** sem token, e também com token de outra chave, expirado ou com *audience* errada.

::: exercicio Faça a sua revisão
1. Escolha uma API sua e liste os endpoints. Quais não têm autorização?
2. Para cada endpoint que recebe um **identificador** do cliente, onde o servidor confere o dono?
3. Procure no repositório: há segredos (`appsettings.json`, `docker-compose.yml`)? Há dado pessoal em tokens?
4. Escolha uma regra de segurança, quebre-a de propósito e veja qual teste falha.
:::

::: resumo
- **Autenticação ≠ autorização**: o servidor precisa conferir o **dono** de cada recurso.
- **Atomicidade**: um commit por operação de negócio.
- **Segredos** só por ambiente; a aplicação deve **falhar rápido** sem eles.
- **Prove** a proteção: teste de mutação e testes que sobem o pipeline HTTP real.
:::

::: quiz
? Um usuário autenticado consegue transferir dinheiro da conta de outro enviando o id dessa conta no corpo. Como se chama essa falha?
- SQL Injection
+ IDOR (referência direta insegura a objeto)
- Cross-Site Scripting
> O servidor aceitou um identificador do cliente sem verificar se aquele objeto pertence ao usuário autenticado.

? Para que serve o teste de mutação manual descrito no capítulo?
- Para acelerar a execução dos testes
+ Para confirmar que um teste realmente protege uma regra: se a regra é quebrada e nenhum teste falha, ela estava sem proteção
- Para gerar dados de teste automaticamente
> Quebrar a regra de propósito mostra quais testes dependem dela.

? Por que o login deve devolver a mesma mensagem para "usuário inexistente" e "senha errada"?
- Para economizar código
+ Para não revelar quais e-mails estão cadastrados (evita enumeração de contas)
- Porque o BCrypt exige
> Mensagens diferentes permitem a um atacante descobrir quais contas existem.
:::

::: cartas
IDOR :: Falha em que o servidor aceita um identificador do cliente sem checar o dono do recurso.
Teste de mutação :: Quebrar a regra de propósito para ver se algum teste falha.
Falhar cedo :: Recusar subir com configuração insegura (ex.: chave JWT ausente ou fraca).
Enumeração de contas :: Descobrir quais usuários existem pela diferença nas mensagens de erro.
:::
