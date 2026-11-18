using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SuperNetSenior.BuildingBlocks.Messaging;

namespace SuperNetSenior.BuildingBlocks.Infrastructure.Messaging;

/// <summary>Aplique no OnModelCreating do contexto de cada módulo: <c>modelBuilder.ApplyConfiguration(new OutboxMessageConfiguration());</c></summary>
public sealed class OutboxMessageConfiguration : IEntityTypeConfiguration<OutboxMessage>
{
    public void Configure(EntityTypeBuilder<OutboxMessage> entity)
    {
        entity.ToTable("outbox_messages");
        entity.HasKey(x => x.Id);
        entity.Property(x => x.Tipo).HasMaxLength(150).IsRequired();
        entity.Property(x => x.Payload).IsRequired();
        entity.Property(x => x.CriadoEm).IsRequired();
        entity.Property(x => x.Erro).HasMaxLength(1000);

        // Busca rápida das pendentes pelo worker.
        entity.HasIndex(x => x.ProcessadoEm);
    }
}
