using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.IdentityModel.Tokens;

namespace SuperNetSenior.Banking.Tests;

/// <summary>
/// Sobe a API de verdade (pipeline HTTP completo) SEM banco: só exercita o que responde antes de tocar nos dados
/// — autenticação, autorização e validação.
/// </summary>
public class ApiPipelineTests : IClassFixture<ApiPipelineTests.Fabrica>
{
    private const string Chave = "chave-de-teste-da-api-com-mais-de-32-caracteres!";

    public sealed class Fabrica : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Testing"); // evita o EnsureCreated do ambiente Development
            builder.UseSetting("Jwt:Key", Chave);
            builder.UseSetting("Jwt:Issuer", "teste");
            builder.UseSetting("Jwt:Audience", "teste-aud");
        }
    }

    private readonly Fabrica _fabrica;

    public ApiPipelineTests(Fabrica fabrica) => _fabrica = fabrica;

    private static string Token(string chave, string issuer = "teste", string audience = "teste-aud", DateTime? expira = null)
    {
        var credenciais = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(chave)), SecurityAlgorithms.HmacSha256);
        var jwt = new JwtSecurityToken(issuer, audience,
            [new Claim("sub", Guid.NewGuid().ToString()), new Claim("cliente_id", Guid.NewGuid().ToString())],
            expires: expira ?? DateTime.UtcNow.AddMinutes(5), signingCredentials: credenciais);
        return new JwtSecurityTokenHandler().WriteToken(jwt);
    }

    private HttpClient Cliente(string? token = null)
    {
        var client = _fabrica.CreateClient();
        if (token is not null)
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return client;
    }

    [Theory]
    [InlineData("GET", "/api/contas/5b7a6f0e-0000-4000-8000-000000000001")]
    [InlineData("GET", "/api/contas/5b7a6f0e-0000-4000-8000-000000000001/saldo")]
    [InlineData("POST", "/api/contas")]
    [InlineData("POST", "/api/contas/5b7a6f0e-0000-4000-8000-000000000001/movimentos")]
    [InlineData("POST", "/api/transferencias")]
    [InlineData("POST", "/api/pagamentos/pix")]
    [InlineData("POST", "/api/investimentos/resgatar")]
    public async Task Endpoints_protegidos_exigem_token(string metodo, string url)
    {
        var request = new HttpRequestMessage(new HttpMethod(metodo), url);
        if (metodo == "POST")
            request.Content = JsonContent.Create(new { });

        var resposta = await Cliente().SendAsync(request);

        // No BankMore original, transferência, movimento e saldo respondiam sem nenhum token.
        Assert.Equal(HttpStatusCode.Unauthorized, resposta.StatusCode);
    }

    [Fact]
    public async Task Token_assinado_com_outra_chave_e_rejeitado()
    {
        var resposta = await Cliente(Token("outra-chave-totalmente-diferente-0123456789!")).GetAsync("/api/contas/5b7a6f0e-0000-4000-8000-000000000001");
        Assert.Equal(HttpStatusCode.Unauthorized, resposta.StatusCode);
    }

    [Fact]
    public async Task Token_expirado_e_rejeitado()
    {
        var expirado = Token(Chave, expira: DateTime.UtcNow.AddMinutes(-10));
        var resposta = await Cliente(expirado).GetAsync("/api/contas/5b7a6f0e-0000-4000-8000-000000000001");
        Assert.Equal(HttpStatusCode.Unauthorized, resposta.StatusCode);
    }

    [Fact]
    public async Task Token_com_audience_errada_e_rejeitado()
    {
        var resposta = await Cliente(Token(Chave, audience: "outro-sistema")).GetAsync("/api/contas/5b7a6f0e-0000-4000-8000-000000000001");
        Assert.Equal(HttpStatusCode.Unauthorized, resposta.StatusCode);
    }

    [Fact]
    public async Task Cadastro_com_cpf_invalido_retorna_400_com_codigo_do_erro()
    {
        var resposta = await Cliente().PostAsJsonAsync("/api/auth/registrar",
            new { nome = "Ana", email = "ana@exemplo.com", cpf = "111.111.111-11", senha = "senha-forte-1" });

        Assert.Equal(HttpStatusCode.BadRequest, resposta.StatusCode);
        var corpo = await resposta.Content.ReadAsStringAsync();
        Assert.Contains("Cpf.Invalid", corpo);
    }

    [Fact]
    public async Task Cadastro_com_senha_curta_retorna_400()
    {
        var resposta = await Cliente().PostAsJsonAsync("/api/auth/registrar",
            new { nome = "Ana", email = "ana@exemplo.com", cpf = "529.982.247-25", senha = "123" });

        Assert.Equal(HttpStatusCode.BadRequest, resposta.StatusCode);
    }

    [Fact]
    public async Task Login_com_dados_vazios_retorna_400()
    {
        var resposta = await Cliente().PostAsJsonAsync("/api/auth/login", new { email = "", senha = "" });
        Assert.Equal(HttpStatusCode.BadRequest, resposta.StatusCode);
    }

    [Fact]
    public async Task Login_com_email_malformado_responde_401_generico_sem_consultar_o_banco()
    {
        var resposta = await Cliente().PostAsJsonAsync("/api/auth/login", new { email = "nao-e-email", senha = "qualquer" });

        Assert.Equal(HttpStatusCode.Unauthorized, resposta.StatusCode);
        Assert.Contains("E-mail ou senha inválidos", await resposta.Content.ReadAsStringAsync());
    }

    [Fact]
    public void Sem_chave_jwt_a_aplicacao_nao_sobe()
    {
        using var semChave = new WebApplicationFactory<Program>().WithWebHostBuilder(b =>
        {
            b.UseEnvironment("Testing");
            b.UseSetting("Jwt:Key", "curta");
        });

        // Falha rápida: nunca sobe com chave fraca ou ausente.
        Assert.ThrowsAny<Exception>(() => semChave.CreateClient());
    }
}
