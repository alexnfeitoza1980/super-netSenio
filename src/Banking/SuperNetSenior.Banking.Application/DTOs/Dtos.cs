using SuperNetSenior.Banking.Domain.Enums;

namespace SuperNetSenior.Banking.Application.DTOs;

// ----- Acesso -----
public sealed record RegistrarUsuarioRequest(string Nome, string Email, string Cpf, string Senha);
public sealed record RegistrarUsuarioResponse(Guid UsuarioId, Guid ClienteId, string Nome, string Email);

public sealed record LoginRequest(string Email, string Senha);
public sealed record LoginResponse(string Token, DateTime ExpiraEm, Guid UsuarioId, Guid ClienteId);

// ----- Contas -----
/// <summary>A conta é sempre aberta para o cliente autenticado: o titular NÃO vem do corpo da requisição.</summary>
public sealed record AbrirContaRequest(string Numero, string Agencia = "0001");

public sealed record ContaDto(
    Guid Id, Guid ClienteId, string Numero, string Agencia, StatusConta Status, decimal Saldo, DateTime AbertaEm);

public sealed record MovimentarContaRequest(
    Guid ContaId, TipoLancamento Tipo, decimal Valor, string ChaveIdempotencia, string? Descricao = null);

public sealed record TransferirRequest(
    Guid ContaOrigemId, Guid ContaDestinoId, decimal Valor, string ChaveIdempotencia);

public sealed record TransferenciaDto(
    Guid Id, Guid ContaOrigemId, Guid ContaDestinoId, decimal Valor, string ChaveIdempotencia, DateTime CriadaEm);

// ----- Pagamentos -----
public sealed record AgendarPagamentoPixRequest(
    Guid ContaOrigemId, decimal Valor, string ChavePix, DateTime? AgendadoPara = null);

public sealed record OperacaoPagamentoDto(
    Guid Id, Guid ContaOrigemId, decimal Valor, string CodigoBarrasOuPix,
    StatusOperacao Status, DateTime AgendadoPara, DateTime SolicitadoEm);

// ----- Investimentos -----
public sealed record ResgatarInvestimentoRequest(Guid PosicaoInvestimentoId, decimal ValorResgate);

public sealed record PosicaoInvestimentoDto(
    Guid Id, Guid ContaId, string Ativo, TipoPosicao Tipo,
    decimal ValorAplicado, decimal SaldoAtual, DateTime DataAplicacao);
