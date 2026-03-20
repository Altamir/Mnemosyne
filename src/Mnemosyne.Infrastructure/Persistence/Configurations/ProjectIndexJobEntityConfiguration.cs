using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Mnemosyne.Domain.Entities;

namespace Mnemosyne.Infrastructure.Persistence.Configurations;

public class ProjectIndexJobEntityConfiguration : IEntityTypeConfiguration<ProjectIndexJobEntity>
{
    public void Configure(EntityTypeBuilder<ProjectIndexJobEntity> builder)
    {
        builder.ToTable("project_index_jobs");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.ProjectId)
            .IsRequired();

        builder.Property(p => p.Status)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(p => p.TotalMemories)
            .IsRequired();

        builder.Property(p => p.ProcessedMemories)
            .IsRequired();

        builder.Property(p => p.ErrorMessage)
            .HasMaxLength(1000);

        builder.Property(p => p.CreatedAt)
            .IsRequired();

        builder.Property(p => p.UpdatedAt)
            .IsRequired();

        builder.HasIndex(p => p.ProjectId);
        builder.HasIndex(p => p.Status);
    }
}
