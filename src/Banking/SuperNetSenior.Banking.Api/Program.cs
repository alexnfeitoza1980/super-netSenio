using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using SuperNetSenior.Banking.Api.Security;
using SuperNetSenior.Banking.Application;
using SuperNetSenior.Banking.Application.Abstractions;
using SuperNetSenior.Banking.Infrastructure;
using SuperNetSenior.Banking.Infrastructure.Persistence;
using SuperNetSenior.BuildingBlocks.Security;

var builder = WebApplication.CreateBuilder(args);

// ---- JWT: a chave NUNCA fica no appsettings.json versionado ----
// Em Development vem do appsettings.Development.json; em qualquer outro ambiente, de variável de ambiente (Jwt__Key) ou secret store.
var jwt = builder.Configuration.GetSection(JwtOptions.Secao).Get<JwtOptions>() ?? new JwtOptions();
if (string.IsNullOrWhiteSpace(jwt.Key) || Encoding.UTF8.GetByteCount(jwt.Key) < JwtOptions.TamanhoMinimoChave)
    throw new InvalidOperationException(
        $"Configure Jwt:Key com pelo menos {JwtOptions.TamanhoMinimoChave} caracteres (variável de ambiente Jwt__Key).");

builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection(JwtOptions.Secao));
builder.Services.AddSingleton<ITokenService, JwtTokenService>();

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUser, HttpCurrentUser>();

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwt.Issuer,
            ValidateAudience = true,
            ValidAudience = jwt.Audience,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Key)),
            ClockSkew = TimeSpan.FromSeconds(30)
        };
    });
builder.Services.AddAuthorization();

builder.Services.AddBankingApplication();
builder.Services.AddBankingInfrastructure(builder.Configuration);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "super-netSenior · Banking", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Description = "Informe apenas o token JWT."
    });
    c.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference("Bearer", document)] = []
    });
});

var app = builder.Build();

// ---- Erros inesperados viram ProblemDetails; concorrência e duplicidade viram 409 ----
app.UseExceptionHandler(erroApp => erroApp.Run(async context =>
{
    var excecao = context.Features.Get<IExceptionHandlerFeature>()?.Error;
    var (status, titulo, detalhe) = excecao switch
    {
        DbUpdateConcurrencyException => (StatusCodes.Status409Conflict, "Conflito de concorrência",
            "O recurso foi alterado por outra operação. Tente novamente."),
        DbUpdateException => (StatusCodes.Status409Conflict, "Conflito",
            "A operação conflita com um registro existente (possível duplicidade)."),
        _ => (StatusCodes.Status500InternalServerError, "Erro interno", "Ocorreu um erro inesperado.")
    };

    context.Response.StatusCode = status;
    await context.Response.WriteAsJsonAsync(new ProblemDetails { Status = status, Title = titulo, Detail = detalhe });
}));

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    // Atalho de desenvolvimento. Para produção, use migrations (dotnet ef migrations add ...).
    using var scope = app.Services.CreateScope();
    await scope.ServiceProvider.GetRequiredService<BankingDbContext>().Database.EnsureCreatedAsync();
}
else
{
    app.UseHttpsRedirection();
}

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();

public partial class Program;
