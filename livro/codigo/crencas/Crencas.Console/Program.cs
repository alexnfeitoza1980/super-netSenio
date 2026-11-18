using Jornada.Crencas;

var caderno = new CadernoDeHipoteses();
var investigacao = caderno.Adicionar(new Hipotese(
    "Um exemplo menor ajuda a entender GroupBy.",
    OrigemDaCrenca.Desconhecida));

investigacao.Registrar(new Evidencia(
    "A consulta com três itens ficou mais fácil de acompanhar.",
    RelacaoDaEvidencia.Apoia));

investigacao.Registrar(new Evidencia(
    "Ainda não está claro como tratar uma chave ausente.",
    RelacaoDaEvidencia.Contextualiza));

Console.WriteLine($"Síntese dos registros: {investigacao.Sintetizar()}");
Console.WriteLine("A síntese descreve somente as observações registradas.");
