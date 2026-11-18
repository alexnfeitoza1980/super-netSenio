# Atenção Plena em Angular (Livro 1 da coleção Angular com Atenção)

Livro interativo para aprender **Angular 22** do zero, vindo do C#, seguindo o roteiro de oito semanas de *Atenção Plena* (*Mindfulness*), de **Mark Williams** e **Danny Penman**. Cada semana vira um conceito do framework: detecção de mudanças, templates, rotas, RxJS, HTTP, formulários, desempenho e testes. Textos, exemplos, código e ilustrações são originais; o livro não substitui a leitura da obra.

## Abrir o livro

Abra `index.html` no navegador (funciona sem servidor). Para regenerar depois de editar `content/`, rode `python build.py` na pasta `angular/`.

## Código

`codigo/` é um workspace **Angular 22** (standalone, signals, zoneless, OnPush por padrão, Vitest):

- `src/app/capitulos/capNN/`: o código de cada capítulo, com os testes (`*.spec.ts`) ao lado;
- `src/app/labs/lab01/`: o temporizador **Pausa de um minuto** (Laboratório 1);
- `src/app/diario/`: o app **Diário de Atenção** (Laboratório 2);
- `public/api/`: JSONs servidos como se viessem de um servidor (capítulo 9).

Com o **Node.js 22.12** ou mais novo:

```bash
cd angular/livros/01-atencao-plena-angular/codigo
npm ci            # instala exatamente as versões do package-lock.json
npm start         # http://localhost:4200
npm run test:ci   # 107 testes
npm run build     # build de produção
```

Todo trecho de código do livro é lido desses arquivos na hora de gerar as páginas: se o código mudar, o livro muda junto.

## Privacidade e cuidado

O Diário guarda tudo **só no navegador** (`localStorage`) e não envia nada a lugar nenhum. Nenhum programa do livro mede, compara ou rotula pessoas. As práticas curtas são opcionais e não são terapia. Em sofrimento intenso, procure um profissional. No Brasil: CVV 188 (24 h, gratuito) e SAMU 192.
