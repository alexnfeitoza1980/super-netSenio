using SuperNetSenior.Banking.Domain.Entities;

namespace SuperNetSenior.Banking.Application.Abstractions;

// Repositórios só rastreiam mudanças. Quem grava é o IUnitOfWork (um único commit = uma transação).

public interface IClienteRepository
{
    Task<bool> ExisteCpfAsync(string cpf, CancellationToken cancellationToken = default);
    Task AdicionarAsync(Cliente cliente, CancellationToken cancellationToken = default);
}

public interface IUsuarioRepository
{
    Task<Usuario?> ObterPorEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<bool> ExisteEmailAsync(string email, CancellationToken cancellationToken = default);
    Task AdicionarAsync(Usuario usuario, CancellationToken cancellationToken = default);
}

public interface IContaRepository
{
    Task<Conta?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> ExisteNumeroAsync(string agencia, string numero, CancellationToken cancellationToken = default);
    Task AdicionarAsync(Conta conta, CancellationToken cancellationToken = default);
}

public interface ILancamentoRepository
{
    Task<LancamentoFinanceiro?> ObterPorChaveAsync(Guid contaId, string chaveIdempotencia, CancellationToken cancellationToken = default);
    Task AdicionarAsync(LancamentoFinanceiro lancamento, CancellationToken cancellationToken = default);
}

public interface ITransferenciaRepository
{
    Task<Transferencia?> ObterPorChaveAsync(Guid contaOrigemId, string chaveIdempotencia, CancellationToken cancellationToken = default);
    Task AdicionarAsync(Transferencia transferencia, CancellationToken cancellationToken = default);
}

public interface IPagamentoRepository
{
    Task AdicionarAsync(OperacaoPagamento operacao, CancellationToken cancellationToken = default);
}

public interface IInvestimentoRepository
{
    Task<PosicaoInvestimento?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AdicionarAsync(PosicaoInvestimento posicao, CancellationToken cancellationToken = default);
}

public interface IPasswordHasher
{
    string Hash(string senha);
    bool Verify(string senha, string hash);
}

public sealed record TokenEmitido(string Token, DateTime ExpiraEm);

public interface ITokenService
{
    TokenEmitido Gerar(Usuario usuario);
}
