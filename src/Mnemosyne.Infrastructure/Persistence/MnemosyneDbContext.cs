using Microsoft.EntityFrameworkCore;
using Mnemosyne.Domain.Entities;

namespace Mnemosyne.Infrastructure.Persistence;

public class MnemosyneDbContext : DbContext
{
    private readonly Action<ModelBuilder>? _testModelConfiguration;

    public DbSet<MemoryEntity> Memories => Set<MemoryEntity>();
    public DbSet<ProjectEntity> Projects => Set<ProjectEntity>();

    public MnemosyneDbContext(DbContextOptions<MnemosyneDbContext> options)
        : base(options)
    {
        _testModelConfiguration = null;
    }

    public MnemosyneDbContext(DbContextOptions<MnemosyneDbContext> options, Action<ModelBuilder>? testModelConfiguration)
        : base(options)
    {
        _testModelConfiguration = testModelConfiguration;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(MnemosyneDbContext).Assembly);
        _testModelConfiguration?.Invoke(modelBuilder);
    }
}
