using Microsoft.EntityFrameworkCore;
using SuperNetSenior.Banking.Application.Abstractions;
using SuperNetSenior.Banking.Domain.Entities;
using SuperNetSenior.Banking.Infrastructure.Persistence;

namespace SuperNetSenior.Banking.Infrastructure.Repositories;

// Repositórios só rastreiam mudanças no DbContext; o commit é do IUnitOfWork.

public sealed class ClienteRepository(BankingDbContext db) : IClienteRepository
{
    public Task<bool> ExisteCpfAsync(string cpf, CancellationToken ct = default) =>
        db.Clientes.AnyAsync(c => c.Cpf == SuperNetSenior.BuildingBlocks.ValueObjects.Cpf.Create(cpf).Value, ct);

    public async Task AdicionarAsync(Cliente cliente, CancellationToken ct = default) =>
        await db.Clientes.AddAsync(cliente, ct);
}

public sealed class UsuarioRepository(BankingDbContext db) : IUsuarioRepository
{
    public Task<Usuario?> ObterPorEmailAsync(string email, CancellationToken ct = default) =>
        db.Usuarios.FirstOrDefaultAsync(u => u.Email == SuperNetSenior.BuildingBlocks.ValueObjects.Email.Create(email).Value, ct);

    public Task<bool> ExisteEmailAsync(string email, CancellationToken ct = default) =>
        db.Usuarios.AnyAsync(u => u.Email == SuperNetSenior.BuildingBlocks.ValueObjects.Email.Create(email).Value, ct);

    public async Task AdicionarAsync(Usuario usuario, CancellationToken ct = default) =>
        await db.Usuarios.AddAsync(usuario, ct);
}

public sealed class ContaRepository(BankingDbContext db) : IContaRepository
{
    public Task<Conta?> ObterPorIdAsync(Guid id, CancellationToken ct = default) =>
        db.Contas.FirstOrDefaultAsync(c => c.Id == id, ct);

    public Task<bool> ExisteNumeroAsync(string agencia, string numero, CancellationToken ct = default) =>
        db.Contas.AnyAsync(c => c.Agencia == agencia && c.Numero == numero, ct);

    public async Task AdicionarAsync(Conta conta, CancellationToken ct = default) =>
        await db.Contas.AddAsync(conta, ct);
}

public sealed class LancamentoRepository(BankingDbContext db) : ILancamentoRepository
{
    public Task<LancamentoFinanceiro?> ObterPorChaveAsync(Guid contaId, string chave, CancellationToken ct = default) =>
        db.Lancamentos.FirstOrDefaultAsync(l => l.ContaId == contaId && l.ChaveIdempotencia == chave, ct);

    public async Task AdicionarAsync(LancamentoFinanceiro lancamento, CancellationToken ct = default) =>
        await db.Lancamentos.AddAsync(lancamento, ct);
}

public sealed class TransferenciaRepository(BankingDbContext db) : ITransferenciaRepository
{
    public Task<Transferencia?> ObterPorChaveAsync(Guid contaOrigemId, string chave, CancellationToken ct = default) =>
        db.Transferencias.FirstOrDefaultAsync(t => t.ContaOrigemId == contaOrigemId && t.ChaveIdempotencia == chave, ct);

    public async Task AdicionarAsync(Transferencia transferencia, CancellationToken ct = default) =>
        await db.Transferencias.AddAsync(transferencia, ct);
}

public sealed class PagamentoRepository(BankingDbContext db) : IPagamentoRepository
{
    public async Task AdicionarAsync(OperacaoPagamento operacao, CancellationToken ct = default) =>
        await db.Pagamentos.AddAsync(operacao, ct);
}

public sealed class InvestimentoRepository(BankingDbContext db) : IInvestimentoRepository
{
    public Task<PosicaoInvestimento?> ObterPorIdAsync(Guid id, CancellationToken ct = default) =>
        db.Investimentos.FirstOrDefaultAsync(p => p.Id == id, ct);

    public async Task AdicionarAsync(PosicaoInvestimento posicao, CancellationToken ct = default) =>
        await db.Investimentos.AddAsync(posicao, ct);
}
