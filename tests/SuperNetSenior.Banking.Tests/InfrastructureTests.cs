using System.IdentityModel.Tokens.Jwt;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SuperNetSenior.Banking.Api.Security;
using SuperNetSenior.Banking.Domain.Entities;
using SuperNetSenior.Banking.Infrastructure.Persistence;
using SuperNetSenior.Banking.Infrastructure.Security;
using SuperNetSenior.BuildingBlocks.Messaging;
using SuperNetSenior.BuildingBlocks.ValueObjects;

namespace SuperNetSenior.Banking.Tests;

/// <summary>Não precisam de PostgreSQL: o modelo EF é construído e as consultas são traduzidas sem abrir conexão.</summary>
public class MapeamentoEfTests
{
    private static BankingDbContext NovoContexto() =>
        new(new DbContextOptionsBuilder<BankingDbContext>()
            .UseNpgsql("Host=localhost;Database=teste;Username=x;Password=x")
            .Options);

    [Fact]
    public void Modelo_e_construido_sem_erros()
    {
        using var db = NovoContexto();

        foreach (var tipo in new[]
                 {
                     typeof(Cliente), typeof(Usuario), typeof(Conta), typeof(LancamentoFinanceiro),
                     typeof(Transferencia), typeof(OperacaoPagamento), typeof(PosicaoInvestimento),
                     typeof(PropostaSeguro), typeof(OutboxMessage)
                 })
        {
            Assert.NotNull(db.Model.FindEntityType(tipo));
        }
    }

    [Fact]
    public void Saldo_da_conta_tem_token_de_concorrencia()
    {
        using var db = NovoContexto();
        var versao = db.Model.FindEntityType(typeof(Conta))!.FindProperty(nameof(Conta.Versao))!;
        Assert.True(versao.IsConcurrencyToken);
    }

    [Theory]
    [InlineData(typeof(Cliente), new[] { nameof(Cliente.Cpf) })]
    [InlineData(typeof(Cliente), new[] { nameof(Cliente.Email) })]
    [InlineData(typeof(Usuario), new[] { nameof(Usuario.Email) })]
    [InlineData(typeof(Conta), new[] { nameof(Conta.Agencia), nameof(Conta.Numero) })]
    [InlineData(typeof(LancamentoFinanceiro), new[] { nameof(LancamentoFinanceiro.ContaId), nameof(LancamentoFinanceiro.ChaveIdempotencia) })]
    [InlineData(typeof(Transferencia), new[] { nameof(Transferencia.ContaOrigemId), nameof(Transferencia.ChaveIdempotencia) })]
    public void Indices_unicos_garantem_idempotencia_e_unicidade_no_banco(Type entidade, string[] propriedades)
    {
        using var db = NovoContexto();
        var indices = db.Model.FindEntityType(entidade)!.GetIndexes();

        Assert.Contains(indices, i => i.IsUnique && i.Properties.Select(p => p.Name).SequenceEqual(propriedades));
    }

    [Fact]
    public void Usuario_e_conta_tem_chave_estrangeira_para_cliente()
    {
        using var db = NovoContexto();

        foreach (var tipo in new[] { typeof(Usuario), typeof(Conta) })
        {
            var fks = db.Model.FindEntityType(tipo)!.GetForeignKeys();
            Assert.Contains(fks, fk => fk.PrincipalEntityType.ClrType == typeof(Cliente));
        }
    }

    [Fact]
    public void Consultas_com_value_objects_sao_traduzidas_para_sql()
    {
        using var db = NovoContexto();
        var cpf = Cpf.Create("52998224725").Value;
        var email = Email.Create("ana@exemplo.com").Value;

        var sqlCpf = db.Clientes.Where(c => c.Cpf == cpf).ToQueryString();
        var sqlEmail = db.Usuarios.Where(u => u.Email == email).ToQueryString();

        Assert.Contains("clientes", sqlCpf);
        Assert.Contains("52998224725", sqlCpf);
        Assert.Contains("usuarios", sqlEmail);
        Assert.Contains("ana@exemplo.com", sqlEmail);
    }
}

public class SegurancaTests
{
    private static JwtTokenService NovoServico() => new(Options.Create(new JwtOptions
    {
        Key = "chave-de-teste-com-mais-de-32-caracteres!!",
        Issuer = "teste",
        Audience = "teste-aud",
        ExpiresInMinutes = 30
    }));

    private static Usuario NovoUsuario() =>
        Usuario.Criar(Guid.NewGuid(), Email.Create("ana@exemplo.com").Value, "hash").Value;

    [Fact]
    public void Token_leva_apenas_identificadores_sem_dados_pessoais()
    {
        var usuario = NovoUsuario();

        var emitido = NovoServico().Gerar(usuario);
        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(emitido.Token);

        Assert.Equal(usuario.Id.ToString(), jwt.Subject);
        Assert.Equal(usuario.ClienteId.ToString(), jwt.Claims.Single(c => c.Type == "cliente_id").Value);
        Assert.DoesNotContain(jwt.Claims, c => c.Value.Contains("ana@exemplo.com")); // o original colocava CPF e e-mail no token
        Assert.Equal("teste", jwt.Issuer);
        Assert.Contains("teste-aud", jwt.Audiences);
        Assert.InRange(emitido.ExpiraEm, DateTime.UtcNow.AddMinutes(29), DateTime.UtcNow.AddMinutes(31));
    }

    [Fact]
    public void Cada_token_tem_jti_unico()
    {
        var usuario = NovoUsuario();
        var servico = NovoServico();

        var a = new JwtSecurityTokenHandler().ReadJwtToken(servico.Gerar(usuario).Token).Id;
        var b = new JwtSecurityTokenHandler().ReadJwtToken(servico.Gerar(usuario).Token).Id;

        Assert.NotEqual(a, b);
    }

    [Fact]
    public void Hash_de_senha_e_verificavel_e_nao_expoe_a_senha()
    {
        var hasher = new BCryptPasswordHasher();

        var hash = hasher.Hash("senha-forte-1");

        Assert.DoesNotContain("senha-forte-1", hash);
        Assert.True(hasher.Verify("senha-forte-1", hash));
        Assert.False(hasher.Verify("outra", hash));
        Assert.NotEqual(hash, hasher.Hash("senha-forte-1")); // salt aleatório embutido
    }

    [Fact]
    public void Hash_malformado_nao_estoura_excecao()
    {
        Assert.False(new BCryptPasswordHasher().Verify("x", "isto-nao-e-um-hash-bcrypt"));
    }
}
