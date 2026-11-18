using Microsoft.EntityFrameworkCore;
using SuperNetSenior.Banking.Domain.Entities;
using SuperNetSenior.BuildingBlocks.Infrastructure.Messaging;
using SuperNetSenior.BuildingBlocks.ValueObjects;

namespace SuperNetSenior.Banking.Infrastructure.Persistence;

public sealed class BankingDbContext(DbContextOptions<BankingDbContext> options) : DbContext(options)
{
    public DbSet<Cliente> Clientes => Set<Cliente>();
    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Conta> Contas => Set<Conta>();
    public DbSet<LancamentoFinanceiro> Lancamentos => Set<LancamentoFinanceiro>();
    public DbSet<Transferencia> Transferencias => Set<Transferencia>();
    public DbSet<OperacaoPagamento> Pagamentos => Set<OperacaoPagamento>();
    public DbSet<PropostaSeguro> Seguros => Set<PropostaSeguro>();
    public DbSet<PosicaoInvestimento> Investimentos => Set<PosicaoInvestimento>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Cliente>(e =>
        {
            e.ToTable("clientes");
            e.HasKey(x => x.Id);
            e.Property(x => x.Nome).HasMaxLength(150).IsRequired();
            e.Property(x => x.Email).HasConversion(v => v.Value, v => Email.Create(v).Value)
                .HasMaxLength(Email.MaxLength).IsRequired();
            e.Property(x => x.Cpf).HasConversion(v => v.Value, v => Cpf.Create(v).Value)
                .HasMaxLength(11).IsRequired();
            e.HasIndex(x => x.Cpf).IsUnique();
            e.HasIndex(x => x.Email).IsUnique();
        });

        modelBuilder.Entity<Usuario>(e =>
        {
            e.ToTable("usuarios");
            e.HasKey(x => x.Id);
            e.Property(x => x.Email).HasConversion(v => v.Value, v => Email.Create(v).Value)
                .HasMaxLength(Email.MaxLength).IsRequired();
            e.Property(x => x.SenhaHash).HasMaxLength(100).IsRequired();
            e.HasIndex(x => x.Email).IsUnique();
            // FK real usuário -> cliente (o BankMore original ligava os dois pelo NOME).
            e.HasOne<Cliente>().WithMany().HasForeignKey(x => x.ClienteId).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Conta>(e =>
        {
            e.ToTable("contas");
            e.HasKey(x => x.Id);
            e.Property(x => x.Numero).HasMaxLength(20).IsRequired();
            e.Property(x => x.Agencia).HasMaxLength(10).IsRequired();
            e.Property(x => x.Saldo).HasPrecision(18, 2).IsRequired();
            e.Property(x => x.Versao).IsConcurrencyToken();
            e.Ignore(x => x.Ativa);
            e.HasIndex(x => new { x.Agencia, x.Numero }).IsUnique();
            e.HasOne<Cliente>().WithMany().HasForeignKey(x => x.ClienteId).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<LancamentoFinanceiro>(e =>
        {
            e.ToTable("lancamentos_financeiros");
            e.HasKey(x => x.Id);
            e.Property(x => x.Descricao).HasMaxLength(255).IsRequired();
            e.Property(x => x.ChaveIdempotencia).HasMaxLength(200).IsRequired();
            e.Property(x => x.Valor).HasConversion(m => m.Amount, v => Money.Create(v).Value).HasPrecision(18, 2).IsRequired();
            e.HasIndex(x => new { x.ContaId, x.ChaveIdempotencia }).IsUnique();
            e.HasOne<Conta>().WithMany().HasForeignKey(x => x.ContaId).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Transferencia>(e =>
        {
            e.ToTable("transferencias");
            e.HasKey(x => x.Id);
            e.Property(x => x.ChaveIdempotencia).HasMaxLength(200).IsRequired();
            e.Property(x => x.Valor).HasConversion(m => m.Amount, v => Money.Create(v).Value).HasPrecision(18, 2).IsRequired();
            e.HasIndex(x => new { x.ContaOrigemId, x.ChaveIdempotencia }).IsUnique();
            e.HasOne<Conta>().WithMany().HasForeignKey(x => x.ContaOrigemId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne<Conta>().WithMany().HasForeignKey(x => x.ContaDestinoId).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<OperacaoPagamento>(e =>
        {
            e.ToTable("operacoes_pagamento");
            e.HasKey(x => x.Id);
            e.Property(x => x.CodigoBarrasOuPix).HasMaxLength(150).IsRequired();
            e.Property(x => x.Valor).HasConversion(m => m.Amount, v => Money.Create(v).Value).HasPrecision(18, 2).IsRequired();
        });

        modelBuilder.Entity<PropostaSeguro>(e =>
        {
            e.ToTable("propostas_seguro");
            e.HasKey(x => x.Id);
            e.Property(x => x.TipoSeguro).HasMaxLength(50).IsRequired();
            e.Property(x => x.PremioMensal).HasConversion(m => m.Amount, v => Money.Create(v).Value).HasPrecision(18, 2).IsRequired();
            e.Property(x => x.CapitalSegurado).HasConversion(m => m.Amount, v => Money.Create(v).Value).HasPrecision(18, 2).IsRequired();
        });

        modelBuilder.Entity<PosicaoInvestimento>(e =>
        {
            e.ToTable("posicoes_investimento");
            e.HasKey(x => x.Id);
            e.Property(x => x.Ativo).HasMaxLength(100).IsRequired();
            e.Property(x => x.ValorAplicado).HasConversion(m => m.Amount, v => Money.Create(v).Value).HasPrecision(18, 2).IsRequired();
            e.Property(x => x.SaldoAtual).HasConversion(m => m.Amount, v => Money.Create(v).Value).HasPrecision(18, 2).IsRequired();
        });

        // Outbox compartilhada (BuildingBlocks).
        modelBuilder.ApplyConfiguration(new OutboxMessageConfiguration());
    }
}
