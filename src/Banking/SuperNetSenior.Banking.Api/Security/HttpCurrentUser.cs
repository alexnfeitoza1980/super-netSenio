using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using SuperNetSenior.BuildingBlocks.Security;

namespace SuperNetSenior.Banking.Api.Security;

public sealed class HttpCurrentUser(IHttpContextAccessor accessor) : ICurrentUser
{
    private ClaimsPrincipal? Principal => accessor.HttpContext?.User;

    public bool EstaAutenticado => Principal?.Identity?.IsAuthenticated == true;

    // O JwtBearer por padrão converte "sub" em ClaimTypes.NameIdentifier; aceitamos os dois.
    public Guid? UsuarioId => LerGuid(JwtRegisteredClaimNames.Sub) ?? LerGuid(ClaimTypes.NameIdentifier);

    public Guid? ClienteId => LerGuid(JwtTokenService.ClaimClienteId);

    private Guid? LerGuid(string tipo) =>
        Guid.TryParse(Principal?.FindFirst(tipo)?.Value, out var id) ? id : null;
}
