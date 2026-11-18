# Você É Aquilo que Ama em C# (Livro 8 da coleção Aprenda C#)

Livro interativo **inspirado** nos temas de *Você é aquilo que ama: o poder espiritual do hábito*, de James K. A. Smith (Vida Nova, 2017). As cenas, explicações, exercícios e todo o código são originais; o livro não substitui nem resume a obra.

A obra é cristã e muitos de seus argumentos dependem da fé do autor. Aqui ela é usada como **lente de sabedoria prática** sobre desejo, hábito e formação; o que depende da fé é identificado como convicção do autor, e o leitor não precisa compartilhá-la. Nada neste livro é ciência, avaliação psicológica ou diagnóstico.

## Abrir o livro

Abra `index.html` em um navegador (funciona sem servidor). Para regenerar as páginas depois de editar `content/`, rode `python build.py` na raiz da coleção.

## Código

`codigo/` tem uma solução do Visual Studio com três projetos:

- `Habitos.Capitulos`: o código de cada capítulo (`Cap105` a `Cap115`), as soluções dos exercícios (`Exercicios`) e o laboratório (`Lab12`).
- `Habitos.Testes`: testes xUnit de tudo o que aparece no livro.
- `Habitos.Console`: a **Auditoria litúrgica** (Laboratório 12).

Com o .NET 8 SDK ou superior:

```bash
dotnet test codigo/Habitos.sln
dotnet run --project codigo/Habitos.Console
dotnet run --project codigo/Habitos.Console -- meu-diario.json
```

Os trechos de código e as saídas mostrados nos capítulos vêm desses arquivos compilados e executados.

## Privacidade e cuidado

Os programas deste livro não gravam nem enviam nada: tudo fica na memória ou no arquivo local que você informar. Nenhum deles mede, compara ou rotula pessoas. Em sofrimento intenso ou persistente, procure apoio profissional. No Brasil: CVV 188 (24 h, gratuito) e SAMU 192.
