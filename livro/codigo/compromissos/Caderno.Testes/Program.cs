using Quatro;
using System.Text.Json;
int ok = 0, falhas = 0;
void Verificar(string nome, Action teste)
{
    try { teste(); ok++; Console.WriteLine($"OK {nome}"); }
    catch (Exception e) { falhas++; Console.WriteLine($"FALHOU {nome}: {e.Message}"); }
}
void Igual<T>(T esperado, T real)
{
    if (!EqualityComparer<T>.Default.Equals(esperado, real)) throw new Exception($"Esperado {esperado}; recebido {real}");
}
void Lanca<T>(Action acao) where T : Exception
{
    try { acao(); } catch (T) { return; }
    throw new Exception($"Esperava {typeof(T).Name}");
}
DateOnly dia = new(2026, 11, 5);
Verificar("texto vazio rejeitado", () => Lanca<ArgumentException>(() => Regras.Texto(" ")));
Verificar("texto aparado", () => Igual("Oi", Regras.Texto(" Oi ")));
Verificar("acordo começa não escolhido", () => Igual(false, new Inventario().Registrar("Responder", "Família").Escolhido));
Verificar("escolha preserva identidade", () => { var i = new Inventario(); var a = i.Registrar("Responder", "Hábito"); i.Escolher(a.Id); Igual(a.Id, i.Todos[0].Id); Igual(true, i.Todos[0].Escolhido); });
Verificar("acordo ausente", () => Lanca<KeyNotFoundException>(() => new Inventario().Escolher(Guid.NewGuid())));
Verificar("busca ignora caixa", () => { var i = new Inventario(); i.Registrar("Responder hoje", "Hábito"); Igual(1, i.Buscar("RESPONDER").Count); });
Verificar("busca exige termo", () => Lanca<ArgumentException>(() => new Inventario().Buscar(" ")));
Verificar("snapshot não muda coleção interna", () => { var i = new Inventario(); i.Registrar("A", "B"); var a = (Acordo[])i.Todos; a[0] = a[0] with { Texto = "Alterado" }; Igual("A", i.Todos[0].Texto); });
Verificar("reflexão separa campos", () => { var r = new Reflexao("Ocorreu", "Pensei", "Farei"); Igual("Pensei", r.Interpretacao); Igual("Ocorreu", r.Observacao); });
Verificar("reflexão exige passo", () => Lanca<ArgumentException>(() => new Reflexao("A", "B", "")));
Verificar("promessa aberta", () => Igual(EstadoDaPromessa.Aberta, new Promessa("A", dia).Estado));
Verificar("cumprimento no prazo", () => { var p = new Promessa("A", dia); p.Cumprir(dia); Igual(false, p.FoiCumpridaComAtraso); Igual(EstadoDaPromessa.Cumprida, p.Estado); });
Verificar("atraso observável", () => Igual(true, new Promessa("A", dia).EstaAtrasada(dia.AddDays(1))));
Verificar("cumprimento tardio", () => { var p = new Promessa("A", dia); p.Cumprir(dia.AddDays(1)); Igual(true, p.FoiCumpridaComAtraso); });
Verificar("não cumprir duas vezes", () => { var p = new Promessa("A", dia); p.Cumprir(dia); Lanca<InvalidOperationException>(() => p.Cumprir(dia)); });
Verificar("revisão guarda antes e depois", () => { var p = new Promessa("A", dia); p.Renegociar(dia.AddDays(2), dia, "Combinado"); Igual(dia, p.Revisoes[0].Antes); Igual(dia.AddDays(2), p.Prazo); });
Verificar("revisão exige motivo sem mutação", () => { var p = new Promessa("A", dia); Lanca<ArgumentException>(() => p.Renegociar(dia.AddDays(2), dia, "")); Igual(0, p.Revisoes.Count); Igual(dia, p.Prazo); });
Verificar("revisão exige data futura", () => Lanca<ArgumentException>(() => new Promessa("A", dia).Renegociar(dia.AddDays(-1), dia, "B")));
Verificar("cancelamento com motivo", () => { var p = new Promessa("A", dia); p.Cancelar("Outro acordo"); Igual(EstadoDaPromessa.Cancelada, p.Estado); Igual(false, p.EstaAtrasada(dia.AddDays(2))); });
Verificar("encerrada não renegocia", () => { var p = new Promessa("A", dia); p.Cancelar("B"); Lanca<InvalidOperationException>(() => p.Renegociar(dia.AddDays(2), dia, "B")); });
Verificar("resposta exige limite", () => Lanca<ArgumentException>(() => new RespostaEscolhida("A", "B", "C", "")));
Verificar("pergunta sem resposta é pendente", () => { var p = new Pergunta("A?"); Igual(EstadoDaPergunta.Pendente, p.Estado); Igual(true, p.Evidencia is null); });
Verificar("confirmar registra fonte", () => { var p = new Pergunta("A?"); p.Esclarecer(true, "Sim", "Conversa"); Igual(EstadoDaPergunta.Confirmada, p.Estado); Igual("Conversa", p.Evidencia!.Fonte); });
Verificar("refutar não equivale a ausência", () => { var p = new Pergunta("A?"); p.Esclarecer(false, "Não", "Conversa"); Igual(EstadoDaPergunta.Refutada, p.Estado); });
Verificar("fonte vazia mantém pendência", () => { var p = new Pergunta("A?"); Lanca<ArgumentException>(() => p.Esclarecer(true, "Sim", "")); Igual(EstadoDaPergunta.Pendente, p.Estado); });
Verificar("não reescreve evidência", () => { var p = new Pergunta("A?"); p.Esclarecer(true, "Sim", "Conversa"); Lanca<InvalidOperationException>(() => p.Esclarecer(false, "Não", "Outra")); });
Verificar("capacidade negativa rejeitada", () => Lanca<ArgumentOutOfRangeException>(() => new PlanoDoDia(-1)));
Verificar("plano soma passos", () => { var p = new PlanoDoDia(40); p.Incluir("A", 10); p.Incluir("B", 20); Igual(30, p.Planejados); });
Verificar("excesso não altera plano", () => { var p = new PlanoDoDia(10); p.Incluir("A", 10); Lanca<ArgumentOutOfRangeException>(() => p.Incluir("B", 1)); Igual(10, p.Planejados); });
Verificar("passo zero rejeitado", () => Lanca<ArgumentOutOfRangeException>(() => new PlanoDoDia(10).Incluir("A", 0)));
Verificar("recomeço preserva dias anteriores", () => { var d = new DiarioDePratica(); d.Registrar(dia, "A", "B"); d.Registrar(dia.AddDays(2), "C", "D"); Igual(2, d.DiasPraticados); });
Verificar("duas práticas mesmo dia contam um dia", () => { var d = new DiarioDePratica(); d.Registrar(dia, "A", "B"); d.Registrar(dia, "C", "D"); Igual(1, d.DiasPraticados); Igual(2, d.Registros.Count); });
Verificar("consulta só pendentes em ordem", () => { var c = new Caderno(); c.Prometer("B", dia.AddDays(1)); c.Prometer("A", dia); c.Prometer("C", dia).Cumprir(dia); Igual(2, c.Pendentes().Count); Igual("A", c.Pendentes()[0].Descricao); });
Verificar("retrato preserva revisão", () => { var c = new Caderno(); c.Prometer("A", dia).Renegociar(dia.AddDays(2), dia, "B"); Igual(1, c.Exportar().Promessas[0].Revisoes.Length); });
foreach (bool disco in new[] { false, true })
{
    string pasta = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
    Directory.CreateDirectory(pasta);
    string caminho = Path.Combine(pasta, "caderno.json");
    try
    {
        IArquivoDoCaderno arq = disco ? new ArquivoJson(caminho) : new ArquivoEmMemoria();
        Verificar($"arquivo vazio disco={disco}", () => Igual(true, arq.Ler() is null));
        Verificar($"ida e volta disco={disco}", () => { var c = new Caderno(); c.Diario.Registrar(dia, "A", "B"); c.Refletir(new("C", "D", "E")); c.Prometer("X", dia); c.Perguntar("Y?").Esclarecer(true, "Sim", "Fonte"); arq.Salvar(c.Exportar()); var r = arq.Ler()!; Igual("B", r.Praticas[0].Aprendizado); Igual("D", r.Reflexoes[0].Interpretacao); Igual("X", r.Promessas[0].Descricao); Igual("Fonte", r.Perguntas[0].Evidencia!.Fonte); });
        Verificar($"retrato independente disco={disco}", () => { var c = new Caderno(); var p = c.Prometer("A", dia); arq.Salvar(c.Exportar()); p.Cumprir(dia); Igual(EstadoDaPromessa.Aberta, arq.Ler()!.Promessas[0].Estado); });
        if (disco)
        {
            Verificar("JSON quebrado é erro", () => { File.WriteAllText(caminho, "{"); Lanca<JsonException>(() => arq.Ler()); });
            Verificar("JSON nulo é erro", () => { File.WriteAllText(caminho, "null"); Lanca<InvalidDataException>(() => arq.Ler()); });
            Verificar("versão futura é erro", () => { File.WriteAllText(caminho, "{\"Versao\":99}"); Lanca<InvalidDataException>(() => arq.Ler()); });
            Verificar("coleções faltantes é erro", () => { File.WriteAllText(caminho, "{\"Versao\":1}"); Lanca<InvalidDataException>(() => arq.Ler()); });
        }
    }
    finally { Directory.Delete(pasta, true); }
}
Console.WriteLine($"{ok} aprovados; {falhas} falhas.");
return falhas == 0 ? 0 : 1;
