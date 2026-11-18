using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SuperNetSenior.Banking.Api.Common;
using SuperNetSenior.Banking.Application.DTOs;
using SuperNetSenior.Banking.Application.UseCases;

namespace SuperNetSenior.Banking.Api.Controllers;

/// <summary>Cadastro e login: únicos endpoints anônimos.</summary>
[ApiController]
[Route("api/auth")]
[AllowAnonymous]
public sealed class AuthController : ControllerBase
{
    [HttpPost("registrar")]
    [ProducesResponseType(typeof(RegistrarUsuarioResponse), StatusCodes.Status201Created)]
    public async Task<IActionResult> Registrar(
        [FromBody] RegistrarUsuarioRequest request, [FromServices] IRegistrarUsuarioUseCase useCase, CancellationToken ct)
    {
        var result = await useCase.ExecutarAsync(request, ct);
        return result.ToActionResult(r => new ObjectResult(r) { StatusCode = StatusCodes.Status201Created });
    }

    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Login(
        [FromBody] LoginRequest request, [FromServices] IAutenticarUseCase useCase, CancellationToken ct) =>
        (await useCase.ExecutarAsync(request, ct)).ToActionResult();
}

[ApiController]
[Route("api/contas")]
[Authorize]
public sealed class ContasController : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(ContaDto), StatusCodes.Status201Created)]
    public async Task<IActionResult> Abrir(
        [FromBody] AbrirContaRequest request, [FromServices] IAbrirContaUseCase useCase, CancellationToken ct)
    {
        var result = await useCase.ExecutarAsync(request, ct);
        return result.ToActionResult(c => CreatedAtAction(nameof(Obter), new { id = c.Id }, c));
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ContaDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> Obter(
        Guid id, [FromServices] IConsultarContaUseCase useCase, CancellationToken ct) =>
        (await useCase.ExecutarAsync(id, ct)).ToActionResult();

    [HttpGet("{id:guid}/saldo")]
    public async Task<IActionResult> Saldo(
        Guid id, [FromServices] IConsultarContaUseCase useCase, CancellationToken ct) =>
        (await useCase.ExecutarAsync(id, ct)).ToActionResult(c => Ok(new { contaId = c.Id, saldo = c.Saldo }));

    /// <summary>Depósito (Credito) ou saque (Debito). A chave de idempotência torna o reenvio seguro.</summary>
    [HttpPost("{id:guid}/movimentos")]
    [ProducesResponseType(typeof(ContaDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> Movimentar(
        Guid id, [FromBody] MovimentoBody body, [FromServices] IMovimentarContaUseCase useCase, CancellationToken ct)
    {
        var request = new MovimentarContaRequest(id, body.Tipo, body.Valor, body.ChaveIdempotencia, body.Descricao);
        return (await useCase.ExecutarAsync(request, ct)).ToActionResult();
    }

    public sealed record MovimentoBody(
        Domain.Enums.TipoLancamento Tipo, decimal Valor, string ChaveIdempotencia, string? Descricao = null);
}

[ApiController]
[Route("api/transferencias")]
[Authorize]
public sealed class TransferenciasController : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(TransferenciaDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> Transferir(
        [FromBody] TransferirRequest request, [FromServices] ITransferirUseCase useCase, CancellationToken ct) =>
        (await useCase.ExecutarAsync(request, ct)).ToActionResult();
}

[ApiController]
[Route("api/pagamentos")]
[Authorize]
public sealed class PagamentosController : ControllerBase
{
    [HttpPost("pix")]
    [ProducesResponseType(typeof(OperacaoPagamentoDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> AgendarPix(
        [FromBody] AgendarPagamentoPixRequest request, [FromServices] IAgendarPagamentoPixUseCase useCase, CancellationToken ct) =>
        (await useCase.ExecutarAsync(request, ct)).ToActionResult();
}

[ApiController]
[Route("api/investimentos")]
[Authorize]
public sealed class InvestimentosController : ControllerBase
{
    [HttpPost("resgatar")]
    [ProducesResponseType(typeof(PosicaoInvestimentoDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> Resgatar(
        [FromBody] ResgatarInvestimentoRequest request, [FromServices] IResgatarInvestimentoUseCase useCase, CancellationToken ct) =>
        (await useCase.ExecutarAsync(request, ct)).ToActionResult();
}
