using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Mnemosyne.Domain.Entities;

namespace Mnemosyne.Infrastructure.Persistence.Configurations;

public class MemoryEntityConfiguration : IEntityTypeConfiguration<MemoryEntity>
{
    public const string TableName = "memories";
    public const string SchemaName = "mnemosyne";
    public const int EmbeddingDimension = 1536;

    public void Configure(EntityTypeBuilder<MemoryEntity> builder)
    {
        builder.ToTable(TableName, SchemaName);

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        builder.Property(e => e.Content)
            .HasColumnName("content")
            .HasColumnType("text")
            .IsRequired();

        builder.Property(e => e.Type)
            .HasColumnName("type")
            .HasConversion<string>()
            .IsRequired();

        builder.Property(e => e.Embedding)
            .HasColumnName("embedding")
            .HasColumnType($"vector({EmbeddingDimension})");

        builder.Property(e => e.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(e => e.UpdatedAt)
            .HasColumnName("updated_at")
            .IsRequired();
    }
}
