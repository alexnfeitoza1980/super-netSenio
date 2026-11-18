using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SuperNetSenior.Banking.Application.Abstractions;
using SuperNetSenior.Banking.Application.Events;
using SuperNetSenior.Banking.Infrastructure.Persistence;
using SuperNetSenior.Banking.Infrastructure.Repositories;
using SuperNetSenior.Banking.Infrastructure.Security;
using SuperNetSenior.BuildingBlocks.Infrastructure;

namespace SuperNetSenior.Banking.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddBankingInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<BankingDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("Banking")));

        services.AddScoped<IClienteRepository, ClienteRepository>();
        services.AddScoped<IUsuarioRepository, UsuarioRepository>();
        services.AddScoped<IContaRepository, ContaRepository>();
        services.AddScoped<ILancamentoRepository, LancamentoRepository>();
        services.AddScoped<ITransferenciaRepository, TransferenciaRepository>();
        services.AddScoped<IPagamentoRepository, PagamentoRepository>();
        services.AddScoped<IInvestimentoRepository, InvestimentoRepository>();

        services.AddSingleton<IPasswordHasher, BCryptPasswordHasher>();

        // Unit of Work + Outbox + worker de despacho (BuildingBlocks), com os eventos deste módulo.
        services.AddModuleMessaging<BankingDbContext>(eventos => eventos
            .Register<PagamentoPixAgendadoEvent>()
            .Register<PagamentoPixLiquidadoEvent>());

        return services;
    }
}
