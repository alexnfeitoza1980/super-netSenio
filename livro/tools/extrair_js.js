// Avalia um arquivo JS num contexto isolado (vm) e imprime o valor de uma expressão como JSON.
// Uso: node extrair_js.js <arquivo.js> "<expressão>"     (ex.: "pixLessons" ou "window.ANFX_LESSONS")
// Serve para ler dados de aulas que vivem dentro de arquivos JS dos sites de ensino, sem executar nada de rede.
const fs = require('fs');
const vm = require('vm');

const [, , arquivo, expressao] = process.argv;
if (!arquivo || !expressao) {
  console.error('uso: node extrair_js.js <arquivo.js> "<expressão>"');
  process.exit(2);
}

const contexto = { window: {}, console: { log() {}, warn() {}, error() {} } };
vm.createContext(contexto);
vm.runInContext(fs.readFileSync(arquivo, 'utf8') + '\n;globalThis.__saida = (' + expressao + ');', contexto, { timeout: 5000 });
process.stdout.write(JSON.stringify(contexto.__saida));
