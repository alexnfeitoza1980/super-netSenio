using SuperNetSenior.Banking.Domain.Entities;
using SuperNetSenior.Banking.Domain.Enums;
using SuperNetSenior.BuildingBlocks.Results;
using SuperNetSenior.BuildingBlocks.ValueObjects;

namespace SuperNetSenior.Banking.Tests;

public class ContaTests
{
    private static Conta NovaConta(Guid? clienteId = null) =>
        Conta.Abrir(clienteId ?? Guid.NewGuid(), "12345").Value;

    [Fact]
    public void Conta_nova_esta_ativa_com_saldo_zero()
    {
        var c = NovaConta();
        Assert.True(c.Ativa);
        Assert.Equal(0m, c.Saldo);
        Assert.Equal("0001", c.Agencia);
    }

    [Fact]
    public void Creditar_e_debitar_ajustam_saldo_e_versao()
    {
        var c = NovaConta();

        Assert.True(c.Creditar(100m).IsSuccess);
        Assert.True(c.Debitar(30m).IsSuccess);

        Assert.Equal(70m, c.Saldo);
        Assert.Equal(2, c.Versao); // token de concorrência otimista
    }

    [Fact]
    public void Debitar_alem_do_saldo_falha_sem_alterar_nada()
    {
        var c = NovaConta();
        c.Creditar(10m);
        var versao = c.Versao;

        var r = c.Debitar(10.01m);

        Assert.Equal("Conta.SaldoInsuficiente", r.Error.Code);
        Assert.Equal(10m, c.Saldo);
        Assert.Equal(versao, c.Versao);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    public void Rejeita_valor_nao_positivo(double valor)
    {
        var c = NovaConta();
        Assert.Equal("Conta.ValorInvalido", c.Creditar((decimal)valor).Error.Code);
        Assert.Equal("Conta.ValorInvalido", c.Debitar((decimal)valor).Error.Code);
    }

    [Fact]
    public void Rejeita_mais_de_duas_casas_decimais()
    {
        Assert.Equal("Conta.ValorPrecisao", NovaConta().Creditar(1.001m).Error.Code);
    }

    [Fact]
    public void Conta_bloqueada_nao_movimenta()
    {
        var c = NovaConta();
        c.Creditar(50m);
        c.Bloquear();

        Assert.Equal("Conta.Inativa", c.Creditar(1m).Error.Code);
        Assert.Equal("Conta.Inativa", c.Debitar(1m).Error.Code);
        Assert.Equal(50m, c.Saldo);
    }

    [Fact]
    public void Pertence_somente_ao_titular()
    {
        var titular = Guid.NewGuid();
        var c = NovaConta(titular);

        Assert.True(c.PertenceA(titular));
        Assert.False(c.PertenceA(Guid.NewGuid()));
        Assert.False(c.PertenceA(Guid.Empty));
    }

    [Fact]
    public void Abrir_exige_cliente_e_numero()
    {
        Assert.Equal("Conta.ClienteInvalido", Conta.Abrir(Guid.Empty, "1").Error.Code);
        Assert.Equal("Conta.NumeroObrigatorio", Conta.Abrir(Guid.NewGuid(), " ").Error.Code);
    }
}

public class ClienteEUsuarioTests
{
    [Fact]
    public void Cliente_valido_normaliza_email_e_cpf()
    {
        var r = Cliente.Criar("  Ana Souza ", "ANA@Exemplo.com", "529.982.247-25");

        Assert.True(r.IsSuccess);
        Assert.Equal("Ana Souza", r.Value.Nome);
        Assert.Equal("ana@exemplo.com", r.Value.Email.Value);
        Assert.Equal("52998224725", r.Value.Cpf.Value);
        Assert.Equal(SituacaoCadastral.Ativo, r.Value.Situacao);
    }

    [Theory]
    [InlineData("", "a@b.com", "52998224725", "Cliente.NomeVazio")]
    [InlineData("Ana", "invalido", "52998224725", "Email.Invalid")]
    [InlineData("Ana", "a@b.com", "11111111111", "Cpf.Invalid")]
    public void Cliente_invalido_retorna_o_erro_certo(string nome, string email, string cpf, string codigo)
    {
        Assert.Equal(codigo, Cliente.Criar(nome, email, cpf).Error.Code);
    }

    [Fact]
    public void Usuario_fica_ligado_ao_cliente_por_id()
    {
        var clienteId = Guid.NewGuid();
        var email = Email.Create("a@b.com").Value;

        var u = Usuario.Criar(clienteId, email, "hash").Value;

        Assert.Equal(clienteId, u.ClienteId);
        Assert.True(u.Ativo);
    }

    [Fact]
    public void Usuario_exige_cliente_e_senha()
    {
        var email = Email.Create("a@b.com").Value;
        Assert.Equal("Usuario.ClienteInvalido", Usuario.Criar(Guid.Empty, email, "h").Error.Code);
        Assert.Equal("Usuario.SenhaObrigatoria", Usuario.Criar(Guid.NewGuid(), email, "").Error.Code);
    }
}

public class OutrasEntidadesTests
{
    [Fact]
    public void Lancamento_exige_chave_de_idempotencia()
    {
        var r = LancamentoFinanceiro.Criar(Guid.NewGuid(), 10m, TipoLancamento.Credito, "x", " ");
        Assert.Equal("Lancamento.ChaveObrigatoria", r.Error.Code);
    }

    [Fact]
    public void Transferencia_rejeita_mesma_conta_e_valor_invalido()
    {
        var id = Guid.NewGuid();
        Assert.Equal("Transferencia.MesmaConta", Transferencia.Criar(id, id, 10m, "k").Error.Code);
        Assert.Equal("Transferencia.ValorInvalido", Transferencia.Criar(id, Guid.NewGuid(), 0m, "k").Error.Code);
    }

    [Fact]
    public void Pix_exige_valor_positivo()
    {
        Assert.Equal("Pagamento.ValorInvalido", OperacaoPagamento.Criar(Guid.NewGuid(), 0m, "chave").Error.Code);
        Assert.Equal(StatusOperacao.Agendado, OperacaoPagamento.Criar(Guid.NewGuid(), 5m, "chave").Value.Status);
    }

    [Fact]
    public void Investimento_resgata_ate_o_saldo()
    {
        var p = PosicaoInvestimento.Aplicar(Guid.NewGuid(), "CDB", TipoPosicao.CDB, 100m).Value;

        Assert.True(p.Resgatar(40m).IsSuccess);
        Assert.Equal(60m, p.SaldoAtual.Amount);
        Assert.Equal("Investimento.SaldoInsuficiente", p.Resgatar(60.01m).Error.Code);
        Assert.Equal(60m, p.SaldoAtual.Amount);
    }

    [Fact]
    public void Aplicacao_de_investimento_exige_valor_positivo()
    {
        Assert.Equal("Investimento.ValorInvalido",
            PosicaoInvestimento.Aplicar(Guid.NewGuid(), "CDB", TipoPosicao.CDB, 0m).Error.Code);
    }

    [Fact]
    public void Seguro_nasce_como_proposta()
    {
        var s = PropostaSeguro.Criar(Guid.NewGuid(), "Vida", 50m, 100_000m).Value;
        Assert.Equal(StatusApolice.Proposta, s.Status);
        s.EmitirApolice();
        Assert.Equal(StatusApolice.Emitida, s.Status);
    }

    [Fact]
    public void Resultado_de_falha_de_erro_tipado()
    {
        Assert.Equal(ErrorType.Validation, Transferencia.Criar(Guid.Empty, Guid.NewGuid(), 1m, "k").Error.Type);
    }
}
