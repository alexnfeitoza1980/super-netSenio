using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SuperNetSenior.Banking.Application.Abstractions;
using SuperNetSenior.Banking.Domain.Entities;

namespace SuperNetSenior.Banking.Api.Security;

public sealed class JwtTokenService(IOptions<JwtOptions> options) : ITokenService
{
    public const string ClaimClienteId = "cliente_id";

    public TokenEmitido Gerar(Usuario usuario)
    {
        var opcoes = options.Value;
        var expira = DateTime.UtcNow.AddMinutes(opcoes.ExpiresInMinutes);

        // Só identificadores no token: nada de CPF/e-mail (o original colocava CPF e e-mail nos claims).
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, usuario.Id.ToString()),
            new Claim(ClaimClienteId, usuario.ClienteId.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var credenciais = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(opcoes.Key)), SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(opcoes.Issuer, opcoes.Audience, claims, expires: expira, signingCredentials: credenciais);
        return new TokenEmitido(new JwtSecurityTokenHandler().WriteToken(token), expira);
    }
}
