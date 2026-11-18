using SuperNetSenior.Banking.Application.Abstractions;
using SuperNetSenior.Banking.Domain.Entities;
using SuperNetSenior.BuildingBlocks.Messaging;
using SuperNetSenior.BuildingBlocks.Persistence;
using SuperNetSenior.BuildingBlocks.Security;
using SuperNetSenior.BuildingBlocks.ValueObjects;

namespace SuperNetSenior.Banking.Tests.Support;

/// <summary>Banco em memória + dublês: permite testar os use cases sem PostgreSQL.</summary>
public sealed class Mundo
{
    public List<Cliente> Clientes { get; } = new();
    public List<Usuario> Usuarios { get; } = new();
    public List<Conta> Contas { get; } = new();
    public List<LancamentoFinanceiro> Lancamentos { get; } = new();
    public List<Transferencia> Transferencias { get; } = new();
    public List<OperacaoPagamento> Pagamentos { get; } = new();
    public List<PosicaoInvestimento> Investimentos { get; } = new();
    public List<IIntegrationEvent> Eventos { get; } = new();

    public int Commits { get; set; }
    public FakeCurrentUser Usuario { get; } = new();

    public ClienteRepo ClienteRepo => new(this);
    public UsuarioRepo UsuarioRepo => new(this);
    public ContaRepo ContaRepo => new(this);
    public LancamentoRepo LancamentoRepo => new(this);
    public TransferenciaRepo TransferenciaRepo => new(this);
    public PagamentoRepo PagamentoRepo => new(this);
    public InvestimentoRepo InvestimentoRepo => new(this);
    public FakeUnitOfWork UnitOfWork => new(this);
    public FakeOutbox Outbox => new(this);

    // ----- fábricas de cenário -----
    private static int _seq = 100;

    public (Cliente cliente, Conta conta) NovoTitular(string nome, decimal saldo = 0m)
    {
        var n = Interlocked.Increment(ref _seq);
        var cliente = Cliente.Criar(nome, $"{nome.ToLowerInvariant()}{n}@exemplo.com", CpfValido(n)).Value;
        var conta = Conta.Abrir(cliente.Id, n.ToString()).Value;
        if (saldo > 0)
            conta.Creditar(saldo);

        Clientes.Add(cliente);
        Contas.Add(conta);
        return (cliente, conta);
    }

    public void LogarComo(Cliente cliente) => Usuario.Entrar(cliente.Id);

    /// <summary>Gera um CPF com dígitos verificadores corretos a partir de uma semente.</summary>
    public static string CpfValido(int semente)
    {
        var baseDigits = (semente * 7919 % 900_000_000 + 100_000_000).ToString("D9");
        var d1 = Digito(baseDigits, 10);
        var d2 = Digito(baseDigits + d1, 11);
        var cpf = baseDigits + d1 + d2;
        return Cpf.EhValido(cpf) ? cpf : CpfValido(semente + 1);
    }

    private static int Digito(string digitos, int pesoInicial)
    {
        var soma = 0;
        for (var i = 0; i < digitos.Length; i++)
            soma += (digitos[i] - '0') * (pesoInicial - i);
        var resto = soma % 11;
        return resto < 2 ? 0 : 11 - resto;
    }
}

public sealed class FakeCurrentUser : ICurrentUser
{
    public bool EstaAutenticado { get; private set; }
    public Guid? UsuarioId { get; private set; }
    public Guid? ClienteId { get; private set; }

    public void Entrar(Guid clienteId)
    {
        EstaAutenticado = true;
        UsuarioId = Guid.NewGuid();
        ClienteId = clienteId;
    }
}

public sealed class FakeUnitOfWork(Mundo m) : IUnitOfWork
{
    public Task<int> CommitAsync(CancellationToken cancellationToken = default)
    {
        m.Commits++;
        return Task.FromResult(1);
    }
}

public sealed class FakeOutbox(Mundo m) : IOutbox
{
    public void Adicionar(IIntegrationEvent evento) => m.Eventos.Add(evento);
}

public sealed class FakePasswordHasher : IPasswordHasher
{
    public string Hash(string senha) => "hash:" + senha;
    public bool Verify(string senha, string hash) => hash == "hash:" + senha;
}

public sealed class FakeTokenService : ITokenService
{
    public TokenEmitido Gerar(Usuario usuario) => new("token-" + usuario.Id, DateTime.UtcNow.AddHours(1));
}

public sealed class ClienteRepo(Mundo m) : IClienteRepository
{
    public Task<bool> ExisteCpfAsync(string cpf, CancellationToken ct = default) =>
        Task.FromResult(m.Clientes.Any(c => c.Cpf.Value == cpf));

    public Task AdicionarAsync(Cliente cliente, CancellationToken ct = default)
    {
        m.Clientes.Add(cliente);
        return Task.CompletedTask;
    }
}

public sealed class UsuarioRepo(Mundo m) : IUsuarioRepository
{
    public Task<Usuario?> ObterPorEmailAsync(string email, CancellationToken ct = default) =>
        Task.FromResult(m.Usuarios.FirstOrDefault(u => u.Email.Value == email));

    public Task<bool> ExisteEmailAsync(string email, CancellationToken ct = default) =>
        Task.FromResult(m.Usuarios.Any(u => u.Email.Value == email));

    public Task AdicionarAsync(Usuario usuario, CancellationToken ct = default)
    {
        m.Usuarios.Add(usuario);
        return Task.CompletedTask;
    }
}

public sealed class ContaRepo(Mundo m) : IContaRepository
{
    public Task<Conta?> ObterPorIdAsync(Guid id, CancellationToken ct = default) =>
        Task.FromResult(m.Contas.FirstOrDefault(c => c.Id == id));

    public Task<bool> ExisteNumeroAsync(string agencia, string numero, CancellationToken ct = default) =>
        Task.FromResult(m.Contas.Any(c => c.Agencia == agencia && c.Numero == numero));

    public Task AdicionarAsync(Conta conta, CancellationToken ct = default)
    {
        m.Contas.Add(conta);
        return Task.CompletedTask;
    }
}

public sealed class LancamentoRepo(Mundo m) : ILancamentoRepository
{
    public Task<LancamentoFinanceiro?> ObterPorChaveAsync(Guid contaId, string chave, CancellationToken ct = default) =>
        Task.FromResult(m.Lancamentos.FirstOrDefault(l => l.ContaId == contaId && l.ChaveIdempotencia == chave));

    public Task AdicionarAsync(LancamentoFinanceiro lancamento, CancellationToken ct = default)
    {
        m.Lancamentos.Add(lancamento);
        return Task.CompletedTask;
    }
}

public sealed class TransferenciaRepo(Mundo m) : ITransferenciaRepository
{
    public Task<Transferencia?> ObterPorChaveAsync(Guid origemId, string chave, CancellationToken ct = default) =>
        Task.FromResult(m.Transferencias.FirstOrDefault(t => t.ContaOrigemId == origemId && t.ChaveIdempotencia == chave));

    public Task AdicionarAsync(Transferencia transferencia, CancellationToken ct = default)
    {
        m.Transferencias.Add(transferencia);
        return Task.CompletedTask;
    }
}

public sealed class PagamentoRepo(Mundo m) : IPagamentoRepository
{
    public Task AdicionarAsync(OperacaoPagamento operacao, CancellationToken ct = default)
    {
        m.Pagamentos.Add(operacao);
        return Task.CompletedTask;
    }
}

public sealed class InvestimentoRepo(Mundo m) : IInvestimentoRepository
{
    public Task<PosicaoInvestimento?> ObterPorIdAsync(Guid id, CancellationToken ct = default) =>
        Task.FromResult(m.Investimentos.FirstOrDefault(p => p.Id == id));

    public Task AdicionarAsync(PosicaoInvestimento posicao, CancellationToken ct = default)
    {
        m.Investimentos.Add(posicao);
        return Task.CompletedTask;
    }
}
