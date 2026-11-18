using SuperNetSenior.Banking.Application.DTOs;
using SuperNetSenior.Banking.Application.Events;
using SuperNetSenior.Banking.Application.UseCases;
using SuperNetSenior.Banking.Application.Validators;
using SuperNetSenior.Banking.Domain.Entities;
using SuperNetSenior.Banking.Domain.Enums;
using SuperNetSenior.Banking.Tests.Support;
using SuperNetSenior.BuildingBlocks.Results;

namespace SuperNetSenior.Banking.Tests;

public class AcessoUseCaseTests
{
    private static RegistrarUsuarioUseCase Registrar(Mundo m) =>
        new(m.ClienteRepo, m.UsuarioRepo, new FakePasswordHasher(), m.UnitOfWork, new RegistrarUsuarioRequestValidator());

    private static AutenticarUseCase Autenticar(Mundo m) =>
        new(m.UsuarioRepo, new FakePasswordHasher(), new FakeTokenService(), new LoginRequestValidator());

    private static RegistrarUsuarioRequest Pedido(string email = "ana@exemplo.com", string? cpf = null, string senha = "senha-forte-1") =>
        new("Ana Souza", email, cpf ?? Mundo.CpfValido(1), senha);

    [Fact]
    public async Task Registrar_cria_cliente_e_usuario_ligados_por_id_em_um_commit()
    {
        var m = new Mundo();

        var r = await Registrar(m).ExecutarAsync(Pedido());

        Assert.True(r.IsSuccess);
        Assert.Single(m.Clientes);
        Assert.Single(m.Usuarios);
        Assert.Equal(m.Clientes[0].Id, m.Usuarios[0].ClienteId);
        Assert.Equal("hash:senha-forte-1", m.Usuarios[0].SenhaHash); // nunca a senha em claro
        Assert.Equal(1, m.Commits);
    }

    [Fact]
    public async Task Registrar_recusa_cpf_duplicado()
    {
        var m = new Mundo();
        var cpf = Mundo.CpfValido(2);
        await Registrar(m).ExecutarAsync(Pedido("a@x.com", cpf));

        var r = await Registrar(m).ExecutarAsync(Pedido("b@x.com", cpf));

        Assert.Equal("Cliente.CpfExistente", r.Error.Code);
        Assert.Single(m.Usuarios);
    }

    [Fact]
    public async Task Registrar_recusa_email_duplicado()
    {
        var m = new Mundo();
        await Registrar(m).ExecutarAsync(Pedido("igual@x.com", Mundo.CpfValido(3)));

        var r = await Registrar(m).ExecutarAsync(Pedido("igual@x.com", Mundo.CpfValido(4)));

        Assert.Equal("Usuario.EmailExistente", r.Error.Code);
    }

    [Theory]
    [InlineData("11111111111", "senha-forte-1", ErrorType.Validation)]
    [InlineData(null, "curta", ErrorType.Validation)]
    public async Task Registrar_recusa_dados_invalidos(string? cpf, string senha, ErrorType tipo)
    {
        var m = new Mundo();

        var r = await Registrar(m).ExecutarAsync(Pedido(cpf: cpf, senha: senha));

        Assert.Equal(tipo, r.Error.Type);
        Assert.Empty(m.Clientes);
        Assert.Equal(0, m.Commits);
    }

    [Fact]
    public async Task Login_correto_devolve_token_e_cliente()
    {
        var m = new Mundo();
        await Registrar(m).ExecutarAsync(Pedido());

        var r = await Autenticar(m).ExecutarAsync(new LoginRequest("ANA@exemplo.com", "senha-forte-1"));

        Assert.True(r.IsSuccess);
        Assert.StartsWith("token-", r.Value.Token);
        Assert.Equal(m.Clientes[0].Id, r.Value.ClienteId);
    }

    [Fact]
    public async Task Login_nao_revela_se_o_usuario_existe()
    {
        var m = new Mundo();
        await Registrar(m).ExecutarAsync(Pedido());

        var senhaErrada = await Autenticar(m).ExecutarAsync(new LoginRequest("ana@exemplo.com", "errada"));
        var inexistente = await Autenticar(m).ExecutarAsync(new LoginRequest("ninguem@exemplo.com", "qualquer"));
        var emailInvalido = await Autenticar(m).ExecutarAsync(new LoginRequest("nao-e-email", "qualquer"));

        // O original dizia "Usuário não encontrado" x "Senha inválida" (enumeração de contas).
        Assert.Equal(ErrorType.Unauthorized, senhaErrada.Error.Type);
        Assert.Equal(senhaErrada.Error, inexistente.Error);
        Assert.Equal(senhaErrada.Error, emailInvalido.Error);
    }

    [Fact]
    public async Task Usuario_inativo_nao_entra()
    {
        var m = new Mundo();
        await Registrar(m).ExecutarAsync(Pedido());
        m.Usuarios[0].Inativar();

        var r = await Autenticar(m).ExecutarAsync(new LoginRequest("ana@exemplo.com", "senha-forte-1"));

        Assert.Equal("Auth.CredenciaisInvalidas", r.Error.Code);
    }
}

public class ContaUseCaseTests
{
    private static AbrirContaUseCase Abrir(Mundo m) =>
        new(m.ContaRepo, m.Usuario, m.UnitOfWork, new AbrirContaRequestValidator());

    private static MovimentarContaUseCase Movimentar(Mundo m) =>
        new(m.ContaRepo, m.LancamentoRepo, m.Usuario, m.UnitOfWork, new MovimentarContaRequestValidator());

    private static ConsultarContaUseCase Consultar(Mundo m) => new(m.ContaRepo, m.Usuario);

    [Fact]
    public async Task Abrir_conta_usa_o_cliente_autenticado_como_titular()
    {
        var m = new Mundo();
        var cliente = Cliente.Criar("Ana", "ana@x.com", Mundo.CpfValido(5)).Value;
        m.Clientes.Add(cliente);
        m.LogarComo(cliente);

        var r = await Abrir(m).ExecutarAsync(new AbrirContaRequest("777"));

        Assert.True(r.IsSuccess);
        Assert.Equal(cliente.Id, r.Value.ClienteId);
        Assert.Equal(StatusConta.Ativa, r.Value.Status);
    }

    [Fact]
    public async Task Abrir_conta_exige_autenticacao()
    {
        var r = await Abrir(new Mundo()).ExecutarAsync(new AbrirContaRequest("777"));
        Assert.Equal(ErrorType.Unauthorized, r.Error.Type);
    }

    [Fact]
    public async Task Abrir_conta_recusa_numero_repetido_na_agencia()
    {
        var m = new Mundo();
        var (cliente, conta) = m.NovoTitular("Ana");
        m.LogarComo(cliente);

        var r = await Abrir(m).ExecutarAsync(new AbrirContaRequest(conta.Numero, conta.Agencia));

        Assert.Equal(ErrorType.Conflict, r.Error.Type);
    }

    [Fact]
    public async Task Consultar_so_a_propria_conta()
    {
        var m = new Mundo();
        var (ana, contaAna) = m.NovoTitular("Ana", 50m);
        var (_, contaBob) = m.NovoTitular("Bob", 99m);
        m.LogarComo(ana);

        var propria = await Consultar(m).ExecutarAsync(contaAna.Id);
        var alheia = await Consultar(m).ExecutarAsync(contaBob.Id);
        var inexistente = await Consultar(m).ExecutarAsync(Guid.NewGuid());

        Assert.Equal(50m, propria.Value.Saldo);
        Assert.Equal(ErrorType.Forbidden, alheia.Error.Type);
        Assert.Equal(ErrorType.NotFound, inexistente.Error.Type);
    }

    [Fact]
    public async Task Deposito_e_saque_atualizam_saldo_e_extrato()
    {
        var m = new Mundo();
        var (ana, conta) = m.NovoTitular("Ana");
        m.LogarComo(ana);

        await Movimentar(m).ExecutarAsync(new MovimentarContaRequest(conta.Id, TipoLancamento.Credito, 100m, "d1"));
        var saque = await Movimentar(m).ExecutarAsync(new MovimentarContaRequest(conta.Id, TipoLancamento.Debito, 30m, "s1"));

        Assert.Equal(70m, saque.Value.Saldo);
        Assert.Equal(2, m.Lancamentos.Count);
        Assert.Equal("Depósito", m.Lancamentos[0].Descricao);
        Assert.Equal("Saque", m.Lancamentos[1].Descricao);
    }

    [Fact]
    public async Task Saque_acima_do_saldo_e_recusado()
    {
        var m = new Mundo();
        var (ana, conta) = m.NovoTitular("Ana", 10m);
        m.LogarComo(ana);

        var r = await Movimentar(m).ExecutarAsync(new MovimentarContaRequest(conta.Id, TipoLancamento.Debito, 10.01m, "s1"));

        Assert.Equal("Conta.SaldoInsuficiente", r.Error.Code);
        Assert.Equal(10m, conta.Saldo);
        Assert.Empty(m.Lancamentos);
    }

    [Fact]
    public async Task Reenvio_da_mesma_chave_nao_lanca_duas_vezes()
    {
        var m = new Mundo();
        var (ana, conta) = m.NovoTitular("Ana");
        m.LogarComo(ana);
        var pedido = new MovimentarContaRequest(conta.Id, TipoLancamento.Credito, 100m, "mesma");

        await Movimentar(m).ExecutarAsync(pedido);
        var segunda = await Movimentar(m).ExecutarAsync(pedido);

        Assert.True(segunda.IsSuccess);
        Assert.Equal(100m, conta.Saldo);
        Assert.Single(m.Lancamentos);
    }

    [Fact]
    public async Task Nao_movimenta_conta_de_terceiros()
    {
        var m = new Mundo();
        var (ana, _) = m.NovoTitular("Ana");
        var (_, contaBob) = m.NovoTitular("Bob", 100m);
        m.LogarComo(ana);

        var r = await Movimentar(m).ExecutarAsync(new MovimentarContaRequest(contaBob.Id, TipoLancamento.Debito, 50m, "roubo"));

        Assert.Equal(ErrorType.Forbidden, r.Error.Type);
        Assert.Equal(100m, contaBob.Saldo);
    }

    [Theory]
    [InlineData(TipoLancamento.Reserva)]
    [InlineData(TipoLancamento.Estorno)]
    public async Task Tipo_diferente_de_credito_ou_debito_e_recusado(TipoLancamento tipo)
    {
        var m = new Mundo();
        var (ana, conta) = m.NovoTitular("Ana", 10m);
        m.LogarComo(ana);

        var r = await Movimentar(m).ExecutarAsync(new MovimentarContaRequest(conta.Id, tipo, 5m, "k"));

        Assert.Equal(ErrorType.Validation, r.Error.Type);
    }
}

public class PagamentoEInvestimentoUseCaseTests
{
    private static AgendarPagamentoPixUseCase Pix(Mundo m) =>
        new(m.PagamentoRepo, m.ContaRepo, m.Outbox, m.Usuario, m.UnitOfWork, new AgendarPagamentoPixRequestValidator());

    private static ResgatarInvestimentoUseCase Resgate(Mundo m) =>
        new(m.InvestimentoRepo, m.ContaRepo, m.Usuario, m.UnitOfWork, new ResgatarInvestimentoRequestValidator());

    [Fact]
    public async Task Pix_grava_pagamento_e_evento_no_mesmo_commit()
    {
        var m = new Mundo();
        var (ana, conta) = m.NovoTitular("Ana", 100m);
        m.LogarComo(ana);

        var r = await Pix(m).ExecutarAsync(new AgendarPagamentoPixRequest(conta.Id, 25m, "chave@pix"));

        Assert.True(r.IsSuccess);
        Assert.Single(m.Pagamentos);
        var evento = Assert.IsType<PagamentoPixAgendadoEvent>(Assert.Single(m.Eventos));
        Assert.Equal(m.Pagamentos[0].Id, evento.OperacaoId);
        Assert.Equal(25m, evento.Valor);
        Assert.Equal(1, m.Commits);
    }

    [Fact]
    public async Task Pix_nao_sai_de_conta_de_terceiros()
    {
        var m = new Mundo();
        var (ana, _) = m.NovoTitular("Ana");
        var (_, contaBob) = m.NovoTitular("Bob", 100m);
        m.LogarComo(ana);

        var r = await Pix(m).ExecutarAsync(new AgendarPagamentoPixRequest(contaBob.Id, 25m, "chave"));

        Assert.Equal(ErrorType.Forbidden, r.Error.Type);
        Assert.Empty(m.Pagamentos);
        Assert.Empty(m.Eventos);
    }

    [Fact]
    public async Task Pix_exige_conta_ativa()
    {
        var m = new Mundo();
        var (ana, conta) = m.NovoTitular("Ana");
        conta.Bloquear();
        m.LogarComo(ana);

        var r = await Pix(m).ExecutarAsync(new AgendarPagamentoPixRequest(conta.Id, 1m, "chave"));

        Assert.Equal("Pagamento.ContaInativa", r.Error.Code);
    }

    [Fact]
    public async Task Resgate_reduz_a_posicao_do_dono()
    {
        var m = new Mundo();
        var (ana, conta) = m.NovoTitular("Ana");
        var posicao = PosicaoInvestimento.Aplicar(conta.Id, "CDB", TipoPosicao.CDB, 200m).Value;
        m.Investimentos.Add(posicao);
        m.LogarComo(ana);

        var r = await Resgate(m).ExecutarAsync(new ResgatarInvestimentoRequest(posicao.Id, 50m));

        Assert.Equal(150m, r.Value.SaldoAtual);
    }

    [Fact]
    public async Task Resgate_de_posicao_alheia_e_negado()
    {
        var m = new Mundo();
        var (ana, _) = m.NovoTitular("Ana");
        var (_, contaBob) = m.NovoTitular("Bob");
        var posicao = PosicaoInvestimento.Aplicar(contaBob.Id, "CDB", TipoPosicao.CDB, 200m).Value;
        m.Investimentos.Add(posicao);
        m.LogarComo(ana);

        var r = await Resgate(m).ExecutarAsync(new ResgatarInvestimentoRequest(posicao.Id, 50m));

        Assert.Equal(ErrorType.Forbidden, r.Error.Type);
        Assert.Equal(200m, posicao.SaldoAtual.Amount);
    }

    [Fact]
    public async Task Resgate_alem_da_posicao_e_recusado()
    {
        var m = new Mundo();
        var (ana, conta) = m.NovoTitular("Ana");
        var posicao = PosicaoInvestimento.Aplicar(conta.Id, "CDB", TipoPosicao.CDB, 100m).Value;
        m.Investimentos.Add(posicao);
        m.LogarComo(ana);

        var r = await Resgate(m).ExecutarAsync(new ResgatarInvestimentoRequest(posicao.Id, 100.01m));

        Assert.Equal("Investimento.SaldoInsuficiente", r.Error.Code);
    }

    [Fact]
    public async Task Resgate_de_posicao_inexistente()
    {
        var m = new Mundo();
        var (ana, _) = m.NovoTitular("Ana");
        m.LogarComo(ana);

        var r = await Resgate(m).ExecutarAsync(new ResgatarInvestimentoRequest(Guid.NewGuid(), 1m));

        Assert.Equal(ErrorType.NotFound, r.Error.Type);
    }
}
