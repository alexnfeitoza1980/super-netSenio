using Jornada.Habitos.Cap112;
using Jornada.Habitos.Lab12;

namespace Habitos.Testes;

public class Lab12Tests
{
    [Fact]
    public void O_exemplo_do_livro_e_um_json_valido_com_comentarios_e_virgulas_soltas()
    {
        Assert.True(Carregador.TentarCarregar(Carregador.Exemplo, out var diario, out var problema), problema);

        Assert.Equal(4, diario!.Prioridades!.Count);
        Assert.Equal(7, diario.Registros!.Count);
        Assert.Equal(Intencao.Alinhar, diario.Praticas![0].Intencao);
        Assert.Contains(DayOfWeek.Friday, diario.Praticas[1].Dias);
        Assert.Equal(new DateOnly(2026, 11, 2), diario.Registros[0].Dia);
    }

    [Fact]
    public void Json_quebrado_vira_um_aviso_claro_em_vez_de_um_susto()
    {
        Assert.False(Carregador.TentarCarregar("{ \"prioridades\": [ { \"area\": \"X\" ", out var diario, out var problema));

        Assert.Null(diario);
        Assert.Contains("confira vírgulas", problema);
    }

    [Fact]
    public void Texto_vazio_ou_nulo_nao_quebra()
    {
        Assert.False(Carregador.TentarCarregar("null", out _, out var problema));
        Assert.Contains("vazio", problema);
    }

    [Fact]
    public void Valor_de_enum_desconhecido_tambem_e_tratado_como_problema_de_leitura()
    {
        var json = """{ "praticas": [ { "nome": "x", "intencao": "Inventada", "minutos": 5, "dias": ["Monday"] } ] }""";
        Assert.False(Carregador.TentarCarregar(json, out _, out var problema));
        Assert.NotNull(problema);
    }

    [Fact]
    public void A_auditoria_junta_o_que_se_diz_o_que_se_faz_e_os_ritmos()
    {
        Carregador.TentarCarregar(Carregador.Exemplo, out var diario, out _);

        var linhas = Auditor.Auditar(diario!);

        Assert.Contains(linhas, l => l.StartsWith("Tempo registrado:"));
        Assert.Contains(linhas, l => l.StartsWith("Família:") && l.Contains("O que está no caminho?"));
        Assert.Contains(linhas, l => l.StartsWith("Redes sociais:") && l.Contains("sem estar na sua lista"));
        Assert.Contains(linhas, l => l.StartsWith("Ritmo: Alinhar recebe 75 min"));
        Assert.Contains(linhas, l => l.Contains("Nenhum ritmo cuida de: Aprender, Cuidar, Celebrar"));
        Assert.EndsWith("formar?", linhas[^1]);
    }

    [Fact]
    public void Quem_nao_preenche_nada_recebe_um_convite_e_nao_uma_cobranca()
    {
        var linhas = Auditor.Auditar(new DiarioDaSemana(null, null, null));

        Assert.Equal(2, linhas.Count);
        Assert.Contains("pule esta parte", linhas[0]);
    }

    [Fact]
    public void Nenhuma_frase_do_relatorio_julga_a_pessoa()
    {
        Carregador.TentarCarregar(Carregador.Exemplo, out var diario, out _);

        foreach (var linha in Auditor.Auditar(diario!))
        {
            var texto = linha.ToLowerInvariant();
            foreach (var palavra in new[] { "culpa", "fracasso", "preguiça", "deveria", "falhou" })
                Assert.DoesNotContain(palavra, texto);
        }
    }
}
