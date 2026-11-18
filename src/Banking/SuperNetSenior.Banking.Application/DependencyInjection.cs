using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using SuperNetSenior.Banking.Application.DTOs;
using SuperNetSenior.Banking.Application.UseCases;
using SuperNetSenior.Banking.Application.Validators;

namespace SuperNetSenior.Banking.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddBankingApplication(this IServiceCollection services)
    {
        // Validadores sem estado: podem ser singleton.
        services.AddSingleton<IValidator<RegistrarUsuarioRequest>, RegistrarUsuarioRequestValidator>();
        services.AddSingleton<IValidator<LoginRequest>, LoginRequestValidator>();
        services.AddSingleton<IValidator<AbrirContaRequest>, AbrirContaRequestValidator>();
        services.AddSingleton<IValidator<MovimentarContaRequest>, MovimentarContaRequestValidator>();
        services.AddSingleton<IValidator<TransferirRequest>, TransferirRequestValidator>();
        services.AddSingleton<IValidator<AgendarPagamentoPixRequest>, AgendarPagamentoPixRequestValidator>();
        services.AddSingleton<IValidator<ResgatarInvestimentoRequest>, ResgatarInvestimentoRequestValidator>();

        services.AddScoped<IRegistrarUsuarioUseCase, RegistrarUsuarioUseCase>();
        services.AddScoped<IAutenticarUseCase, AutenticarUseCase>();
        services.AddScoped<IAbrirContaUseCase, AbrirContaUseCase>();
        services.AddScoped<IConsultarContaUseCase, ConsultarContaUseCase>();
        services.AddScoped<IMovimentarContaUseCase, MovimentarContaUseCase>();
        services.AddScoped<ITransferirUseCase, TransferirUseCase>();
        services.AddScoped<IAgendarPagamentoPixUseCase, AgendarPagamentoPixUseCase>();
        services.AddScoped<IResgatarInvestimentoUseCase, ResgatarInvestimentoUseCase>();

        return services;
    }
}
