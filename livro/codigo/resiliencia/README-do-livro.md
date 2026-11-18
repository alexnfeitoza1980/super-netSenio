# Resiliência em C# (Livro 6 da coleção Aprenda C#)

Livro interativo inspirado nos temas do texto *Resiliência: Superando suas dificuldades*, de Kaique Mathias da Silva e Wendel Juan Oliveira Reolon. As cenas, explicações, exercícios e todo o código são originais; o livro não substitui a leitura da obra nem a orientação de profissionais.

## Abrir o livro

Abra `index.html` em um navegador (funciona sem servidor). Para regenerar as páginas depois de editar `content/`, rode `python build.py` na raiz da coleção.

## Código

`codigo/` tem uma solução do Visual Studio com três projetos:

- `Resiliencia.Capitulos`: o código de cada capítulo (`Cap88` a `Cap96`), as soluções dos exercícios (`Exercicios`) e o laboratório (`Lab10`).
- `Resiliencia.Testes`: testes xUnit de tudo o que aparece no livro.
- `Resiliencia.Console`: o **Caderno de um próximo passo** (Laboratório 10).

Com o .NET 8 SDK ou superior:

```bash
dotnet test codigo/Resiliencia.sln
dotnet run --project codigo/Resiliencia.Console
```

Os trechos de código e as saídas mostrados nos capítulos vêm desses arquivos compilados e executados.

## Privacidade e cuidado

Os programas deste livro não gravam nem enviam nada: tudo fica na memória durante a sessão. Nenhum deles mede, compara ou rotula pessoas. Em sofrimento intenso ou persistente, procure apoio profissional. No Brasil: CVV 188 (24 h, gratuito) e SAMU 192.

## Créditos das fotografias

A flor na terra seca e a placa "RESILIENCE" foram fornecidas pelo leitor. As demais fotografias (amanhecer, lago, trilha, montanhas e muda) foram baixadas do Unsplash para uso local, sob a [licença Unsplash](https://unsplash.com/license).
