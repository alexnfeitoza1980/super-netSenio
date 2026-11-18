---
id: compromissos-lab9
order: 112
kind: lab
num: 9
group: C# & Mentalidade · Oficina · O Caderno dos Quatro Compromissos
title: Laboratório 9 — Um caderno que continua amanhã
subtitle: Experimente o cenário completo e registre sua própria prática entre execuções.
minutes: 90
hue: 168
badge: L9
objectives:
- Integrar as capacidades dos quatro compromissos
- Registrar e recuperar práticas em um arquivo local
- Verificar mudanças, erros e limites do armazenamento
origem: livros/05-quatro-compromissos-csharp/content/124-lab9.md
---

::: vila Uma coisa que vale levar para amanhã
Lia percebe que não precisa provar que se tornou outra pessoa. Quer lembrar uma pergunta que fez, uma promessa que cumpriu e algo que pode praticar novamente. O projeto deixa de ser uma demonstração que termina quando a tela fecha: seu próximo registro vai encontrar os anteriores.
:::

::: ilustracao img/compromissos/jornada87.svg | Escolher, perguntar, agir e rever: as peças agora pertencem ao mesmo caderno.
:::

## Primeiro, percorra os quatro compromissos

O cenário cap87 registra um acordo escolhido, esclarece uma pergunta, cumpre uma promessa e escreve um aprendizado. A saída vem de objetos que já construímos; não é um relatório de constantes sem comportamento.

```bash
dotnet run --project codigo/compromissos/Caderno.Console -- cap87
```

Compare com cap80: lá a pergunta ainda era pendente. Compare com cap76: uma promessa mudava de estado. Agora as mesmas regras trabalham juntas. O código inteiro do cenário está em Exemplos.cs, método Cap87. As responsabilidades continuam distribuídas no núcleo.

## Uma jornada guiada pelos quatro temas

```bash
dotnet run --project codigo/compromissos/Caderno.Console -- jornada minha-jornada.json
```

O console faz perguntas, uma de cada vez. Você percebe uma regra recebida, escolhe se deseja praticá-la, registra uma promessa, separa uma mensagem de sua interpretação, formula uma pergunta e planeja um passo que caiba no tempo informado. Escreve também o que essa reflexão ensinou. A data da prática vem do computador.

A pergunta pode permanecer pendente. Não invente uma fonte para avançar: responda pendente. Uma ação pode caber em dez minutos, mas o programa não declara que isso é seu melhor absoluto. Ele apenas verifica o limite que você informou. Você continua sendo responsável pelo sentido de cada resposta.

O arquivo é criado ao final ou recebe os novos registros sem descartar os anteriores. Se uma entrada for inválida antes da gravação, o comando termina com erro e os registros anteriores não são regravados. Execute novamente com o mesmo caminho para acrescentar outra jornada. O plano de minutos é verificado durante a conversa; o arquivo guarda o passo na prática, mas não conserva um objeto PlanoDoDia completo.

### Onde essa conversa vira código

Leia `codigo/compromissos/Caderno.Console/JornadaGuiada.cs`: Pedir mostra uma pergunta e lê uma linha. As classes do núcleo recebem as respostas. Nada chama o PDF para decidir um resultado; as regras são as que construímos. No final, os retratos anterior e atual são combinados e gravados. Isso ainda é armazenamento local para um usuário, sem transação ou bloqueio de escrita.

## Agora, um caderno seu

Execute os comandos abaixo a partir da pasta do livro. Escolha um nome de arquivo novo; `novo` recusa sobrescrever um arquivo existente. Não use dados íntimos para o primeiro teste.

```bash
dotnet run --project codigo/compromissos/Caderno.Console -- novo meu-caderno.json
dotnet run --project codigo/compromissos/Caderno.Console -- praticar meu-caderno.json "Perguntei antes de supor" "A pessoa preferia receber um PDF"
dotnet run --project codigo/compromissos/Caderno.Console -- ler meu-caderno.json
```

O primeiro processo cria um retrato vazio. O segundo abre esse retrato, acrescenta uma prática com a data local e grava novamente. O terceiro lê o arquivo em outra execução. O programa informa uma promessa? Não: os cenários cap74 a cap87 usam cadernos novos e não alteram seu arquivo pessoal. O comando pessoal `praticar` registra ações e aprendizados; a jornada guiada percorre as demais entidades, mas ainda não há um menu para editar registros antigos.

### Preveja a segunda execução

Rode praticar outra vez, com uma ação e um aprendizado diferentes. O total deve passar de um para dois. Feche o terminal e abra outro no mesmo diretório. A leitura deve continuar encontrando os dois registros. Se trocar de diretório, use o caminho completo do arquivo; um caminho relativo depende de onde você executa o comando.

## O caminho dos dados

```text
Texto da pessoa → validação → Pratica → Retrato → JSON → arquivo
Arquivo → JSON → Retrato → consulta no console
```

O JSON guarda dados, não métodos. A leitura recupera um retrato para consulta e acréscimo de práticas. Não promete reconstruir todas as entidades mutáveis do domínio. Essa distinção evita ensinar que desserializar qualquer objeto automaticamente restaura suas regras.

## Verifique os comportamentos

```bash
dotnet build codigo/compromissos/Caderno.slnx -m:1
dotnet run --project codigo/compromissos/Caderno.Testes
python ferramentas/verificar.py
python ferramentas/testar-console.py
```

A suíte é um executável sem bibliotecas de teste externas. Cada caso tem nome, executa uma regra e termina com código 1 se houver falha. Ela cobre estados, validação sem mutação parcial, consultas, recomeços e contratos das duas implementações de arquivo. O verificador editorial também compara as saídas de todos os capítulos e os trechos publicados com seus arquivos-fonte.

::: exercicio A informação realmente permaneceu?
Crie outro arquivo vazio e leia-o. Depois registre duas práticas no primeiro arquivo e compare os dois totais. Tente usar novo novamente com o primeiro caminho. Por fim leia um caminho que ainda não existe.
--- resposta
Cada arquivo mantém seus próprios registros. O comando novo retorna erro e não sobrescreve o existente. O caminho ausente informa que o arquivo ainda não existe. Para observar um arquivo inválido, faça uma cópia de teste e altere o JSON nela: o programa retorna erro, sem transformar corrupção em caderno vazio.
:::

## Limites específicos deste laboratório

O arquivo é local, legível por quem tiver acesso e não foi projetado para várias pessoas gravando ao mesmo tempo. WriteAllText pode deixar dados incompletos se houver uma interrupção durante a gravação. A versão e a presença de coleções são verificadas, mas não há autenticação, criptografia ou validação completa de importações. O uso sugerido é estudo local com dados fictícios, não um serviço público.

## O que vem depois da sua avaliação

Há três evoluções possíveis: um menu para operar todas as entidades, persistência com validação completa e banco de dados, e uma API .NET que uma tela Angular poderia consumir. São próximos passos, não recursos já entregues. O critério antes de ampliar é você conseguir estudar o tema enquanto acompanha as decisões do programa.

::: quiz Reproduza e explique
? Os cenários cap74 a cap87 editam meu-caderno.json?
+ Não; cada cenário começa com dados fictícios novos
- Sim, todos usam o mesmo arquivo pessoal
- Somente depois de reiniciar o computador
> Os comandos novo, praticar e ler compõem a experiência persistente pessoal.

? O que deve acontecer ao repetir novo no mesmo arquivo?
+ Retornar erro sem sobrescrever
- Apagar o histórico
- Duplicar todas as práticas
> Preservar o arquivo existente evita perda silenciosa nesse comando.

? JSON contém os métodos das classes?
+ Não, o retrato serializado contém dados
- Sim, todos os métodos compilados
- Somente os métodos públicos
> O formato não armazena a implementação do programa.

? Como executar as verificações de comportamento deste projeto?
+ dotnet run --project codigo/compromissos/Caderno.Testes
- dotnet test sem nenhum projeto
- Abrir o PDF novamente
> Esta suíte é um executável próprio, não um adaptador xUnit.

? O que o total de práticas comprova?
+ Quantos registros existem, não o valor moral de alguém
- A felicidade da pessoa
- Que o leitor domina os compromissos
> Registros não medem transformação pessoal.

? O que falta para oferecer esse sistema a várias pessoas?
+ Rever armazenamento, validação, acesso e concorrência
- Só mudar as cores
- Nada, JSON já resolve tudo
> O escopo local não equivale a um serviço de produção.
:::

::: cartas O programa completo
Núcleo :: Regras e objetos no Caderno.Core.
Console :: Entrada de comandos e coordenação dos cenários.
Verificações :: Caderno.Testes é um executável que retorna erro se houver falhas.
Retrato :: Cópia transportável dos dados, não a restauração automática do domínio.
Persistência :: Guardar dados fora do processo para outra execução.
Cenário :: Uma experiência reproduzível com dados fictícios novos.
Prática pessoal :: Registro local usando novo, praticar e ler.
Próxima evolução :: Menu, banco e API dependem de uma etapa posterior, após avaliar esta experiência.
:::
