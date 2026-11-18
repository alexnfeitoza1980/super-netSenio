using SuperNetSenior.Banking.Application.DTOs;
using SuperNetSenior.Banking.Application.UseCases;
using SuperNetSenior.Banking.Application.Validators;
using SuperNetSenior.Banking.Domain.Enums;
using SuperNetSenior.Banking.Tests.Support;
using SuperNetSenior.BuildingBlocks.Results;

namespace SuperNetSenior.Banking.Tests;

public class TransferirUseCaseTests
{
    private static TransferirUseCase Criar(Mundo m) =>
        new(m.ContaRepo, m.LancamentoRepo, m.TransferenciaRepo, m.Usuario, m.UnitOfWork, new TransferirRequestValidator());

    private static (Mundo mundo, SuperNetSenior.Banking.Domain.Entities.Conta alice, SuperNetSenior.Banking.Domain.Entities.Conta bob)
        Cenario(decimal saldoAlice = 100m, bool logarComoAlice = true)
    {
        var m = new Mundo();
        var (clienteAlice, alice) = m.NovoTitular("Alice", saldoAlice);
        var (_, bob) = m.NovoTitular("Bob");
        if (logarComoAlice)
            m.LogarComo(clienteAlice);
        return (m, alice, bob);
    }

    [Fact]
    public async Task Transferencia_valida_move_o_saldo_e_grava_tudo_num_unico_commit()
    {
        var (m, alice, bob) = Cenario();

        var r = await Criar(m).ExecutarAsync(new TransferirRequest(alice.Id, bob.Id, 40m, "k1"));

        Assert.True(r.IsSuccess);
        Assert.Equal(60m, alice.Saldo);
        Assert.Equal(40m, bob.Saldo);
        Assert.Single(m.Transferencias);
        Assert.Equal(2, m.Lancamentos.Count);
        Assert.Contains(m.Lancamentos, l => l.ContaId == alice.Id && l.Tipo == TipoLancamento.Debito && l.ChaveIdempotencia == "k1:D");
        Assert.Contains(m.Lancamentos, l => l.ContaId == bob.Id && l.Tipo == TipoLancamento.Credito && l.ChaveIdempotencia == "k1:C");
        Assert.Equal(1, m.Commits); // atomicidade: um único commit
    }

    [Fact]
    public async Task Sem_autenticacao_retorna_Unauthorized()
    {
        var (m, alice, bob) = Cenario(logarComoAlice: false);

        var r = await Criar(m).ExecutarAsync(new TransferirRequest(alice.Id, bob.Id, 10m, "k1"));

        Assert.Equal(ErrorType.Unauthorized, r.Error.Type);
        Assert.Empty(m.Transferencias);
        Assert.Equal(0, m.Commits);
    }

    [Fact]
    public async Task Nao_transfere_a_partir_da_conta_de_outro_cliente()
    {
        var (m, alice, bob) = Cenario();
        m.LogarComo(m.Clientes.First(c => c.Nome == "Bob")); // Bob tenta esvaziar a conta da Alice

        var r = await Criar(m).ExecutarAsync(new TransferirRequest(alice.Id, bob.Id, 10m, "k1"));

        Assert.Equal(ErrorType.Forbidden, r.Error.Type);
        Assert.Equal(100m, alice.Saldo);
        Assert.Equal(0m, bob.Saldo);
        Assert.Equal(0, m.Commits);
    }

    [Fact]
    public async Task Saldo_insuficiente_nao_altera_nada()
    {
        var (m, alice, bob) = Cenario(saldoAlice: 30m);

        var r = await Criar(m).ExecutarAsync(new TransferirRequest(alice.Id, bob.Id, 50m, "k1"));

        Assert.Equal("Conta.SaldoInsuficiente", r.Error.Code);
        Assert.Equal(30m, alice.Saldo);
        Assert.Empty(m.Transferencias);
        Assert.Empty(m.Lancamentos);
        Assert.Equal(0, m.Commits);
    }

    [Fact]
    public async Task Destino_inativo_e_recusado_sem_mexer_no_saldo_da_origem()
    {
        var (m, alice, bob) = Cenario();
        bob.Bloquear();

        var r = await Criar(m).ExecutarAsync(new TransferirRequest(alice.Id, bob.Id, 10m, "k1"));

        Assert.Equal("Transferencia.DestinoInativo", r.Error.Code);
        Assert.Equal(100m, alice.Saldo); // o original debitava a origem antes de descobrir o problema
        Assert.Equal(1, alice.Versao); // só o crédito inicial do cenário alterou a versão
    }

    [Fact]
    public async Task Destino_inexistente_retorna_NotFound()
    {
        var (m, alice, _) = Cenario();

        var r = await Criar(m).ExecutarAsync(new TransferirRequest(alice.Id, Guid.NewGuid(), 10m, "k1"));

        Assert.Equal(ErrorType.NotFound, r.Error.Type);
    }

    [Fact]
    public async Task Origem_igual_ao_destino_e_recusada_na_validacao()
    {
        var (m, alice, _) = Cenario();

        var r = await Criar(m).ExecutarAsync(new TransferirRequest(alice.Id, alice.Id, 10m, "k1"));

        Assert.Equal(ErrorType.Validation, r.Error.Type);
        Assert.Equal(100m, alice.Saldo);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(10.555)]
    public async Task Valor_invalido_e_recusado(double valor)
    {
        var (m, alice, bob) = Cenario();

        var r = await Criar(m).ExecutarAsync(new TransferirRequest(alice.Id, bob.Id, (decimal)valor, "k1"));

        Assert.Equal(ErrorType.Validation, r.Error.Type);
        Assert.Empty(m.Transferencias);
    }

    [Fact]
    public async Task Reenvio_com_a_mesma_chave_devolve_a_mesma_transferencia_sem_duplicar()
    {
        var (m, alice, bob) = Cenario();
        var useCase = Criar(m);

        var primeira = await useCase.ExecutarAsync(new TransferirRequest(alice.Id, bob.Id, 40m, "dup"));
        var segunda = await useCase.ExecutarAsync(new TransferirRequest(alice.Id, bob.Id, 40m, "dup"));

        Assert.True(segunda.IsSuccess);
        Assert.Equal(primeira.Value.Id, segunda.Value.Id);
        Assert.Equal(60m, alice.Saldo);
        Assert.Single(m.Transferencias);
        Assert.Equal(1, m.Commits);
    }

    [Fact]
    public async Task Mesma_chave_com_outro_valor_e_conflito()
    {
        var (m, alice, bob) = Cenario();
        var useCase = Criar(m);
        await useCase.ExecutarAsync(new TransferirRequest(alice.Id, bob.Id, 40m, "k"));

        var r = await useCase.ExecutarAsync(new TransferirRequest(alice.Id, bob.Id, 99m, "k"));

        Assert.Equal(ErrorType.Conflict, r.Error.Type);
        Assert.Equal(60m, alice.Saldo);
    }

    [Fact]
    public async Task Chave_so_vale_dentro_da_conta_de_origem()
    {
        // Duas pessoas podem usar a mesma chave sem colidir.
        var (m, alice, bob) = Cenario();
        var (_, carol) = m.NovoTitular("Carol", 100m);
        var useCase = Criar(m);
        await useCase.ExecutarAsync(new TransferirRequest(alice.Id, bob.Id, 10m, "mesma"));

        m.LogarComo(m.Clientes.First(c => c.Nome == "Carol"));
        var r = await useCase.ExecutarAsync(new TransferirRequest(carol.Id, bob.Id, 10m, "mesma"));

        Assert.True(r.IsSuccess);
        Assert.Equal(2, m.Transferencias.Count);
    }
}
