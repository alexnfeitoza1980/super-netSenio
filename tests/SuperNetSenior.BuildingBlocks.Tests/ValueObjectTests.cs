using SuperNetSenior.BuildingBlocks.Results;
using SuperNetSenior.BuildingBlocks.ValueObjects;

namespace SuperNetSenior.BuildingBlocks.Tests;

public class CpfTests
{
    [Theory]
    [InlineData("529.982.247-25")]
    [InlineData("52998224725")]
    [InlineData(" 111.444.777-35 ")]
    public void Aceita_cpf_valido_com_ou_sem_mascara(string raw)
    {
        var r = Cpf.Create(raw);

        Assert.True(r.IsSuccess);
        Assert.Equal(11, r.Value.Value.Length);
    }

    [Theory]
    [InlineData(null, "Cpf.Empty")]
    [InlineData("", "Cpf.Empty")]
    [InlineData("123", "Cpf.InvalidLength")]
    [InlineData("529.982.247-26", "Cpf.Invalid")]   // dígito verificador errado
    [InlineData("11111111111", "Cpf.Invalid")]      // repetidos passam na conta, mas são inválidos
    [InlineData("00000000000", "Cpf.Invalid")]
    public void Rejeita_cpf_invalido(string? raw, string codigo)
    {
        var r = Cpf.Create(raw);

        Assert.True(r.IsFailure);
        Assert.Equal(codigo, r.Error.Code);
        Assert.Equal(ErrorType.Validation, r.Error.Type);
    }

    [Fact]
    public void Formata_com_mascara()
    {
        Assert.Equal("529.982.247-25", Cpf.Create("52998224725").Value.Formatado);
    }

    [Fact]
    public void Cpfs_iguais_sao_iguais_independente_da_mascara()
    {
        Assert.Equal(Cpf.Create("529.982.247-25").Value, Cpf.Create("52998224725").Value);
    }
}

public class MoneyTests
{
    [Fact]
    public void Aceita_zero_e_positivo()
    {
        Assert.True(Money.Create(0).IsSuccess);
        Assert.Equal(10.5m, Money.Create(10.5m).Value.Amount);
    }

    [Fact]
    public void Rejeita_negativo()
    {
        var r = Money.Create(-0.01m);
        Assert.Equal("Money.Negative", r.Error.Code);
    }

    [Fact]
    public void Rejeita_mais_de_duas_casas()
    {
        var r = Money.Create(10.123m);
        Assert.Equal("Money.Precision", r.Error.Code);
    }

    [Fact]
    public void Soma_e_subtrai()
    {
        var a = Money.Create(10m).Value;
        var b = Money.Create(4m).Value;

        Assert.Equal(14m, a.Add(b).Amount);
        Assert.Equal(6m, a.Subtract(b).Value.Amount);
    }

    [Fact]
    public void Subtrair_alem_do_saldo_falha()
    {
        var r = Money.Create(1m).Value.Subtract(Money.Create(2m).Value);
        Assert.Equal("Money.Insufficient", r.Error.Code);
    }
}

public class EmailTests
{
    [Fact]
    public void Normaliza_para_minusculas_e_remove_espacos()
    {
        Assert.Equal("ana@exemplo.com", Email.Create("  Ana@Exemplo.COM ").Value.Value);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("sem-arroba")]
    [InlineData("@semusuario.com")]
    [InlineData("a@semponto")]
    [InlineData("a@@b.com")]
    [InlineData("a b@c.com")]
    [InlineData("a@.com")]
    public void Rejeita_email_invalido(string raw)
    {
        Assert.True(Email.Create(raw).IsFailure);
    }

    [Fact]
    public void Rejeita_email_longo_demais()
    {
        var longo = new string('a', Email.MaxLength) + "@x.com";
        Assert.Equal("Email.TooLong", Email.Create(longo).Error.Code);
    }
}

public class ResultTests
{
    [Fact]
    public void Sucesso_expoe_valor()
    {
        Assert.Equal(7, Result.Success(7).Value);
    }

    [Fact]
    public void Falha_nao_expoe_valor()
    {
        var r = Result.Failure<int>(Error.NotFound("x", "y"));
        Assert.Throws<InvalidOperationException>(() => r.Value);
    }

    [Fact]
    public void Falha_exige_erro()
    {
        Assert.Throws<InvalidOperationException>(() => Result.Failure(Error.None));
    }

    [Fact]
    public void Fabricas_de_erro_definem_o_tipo()
    {
        Assert.Equal(ErrorType.Conflict, Error.Conflict("a", "b").Type);
        Assert.Equal(ErrorType.Forbidden, Error.Forbidden("a", "b").Type);
        Assert.Equal(ErrorType.Unauthorized, Error.Unauthorized("a", "b").Type);
        Assert.Equal(ErrorType.NotFound, Error.NotFound("a", "b").Type);
        Assert.Equal(ErrorType.Validation, Error.Validation("a", "b").Type);
    }
}
