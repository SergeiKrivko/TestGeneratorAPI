using Microsoft.EntityFrameworkCore;
using TestGeneratorAPI.DataAccess.Configurations;
using TestGeneratorAPI.DataAccess.Entities;

namespace TestGeneratorAPI.DataAccess.Context;

public class TestGeneratorDbContext : DbContext
{
    public TestGeneratorDbContext(DbContextOptions<TestGeneratorDbContext> options)
        : base(options)
    {
    }

    public DbSet<UserEntity> Users { get; init; }
    public DbSet<TokenEntity> Tokens { get; init; }
    public DbSet<PluginEntity> Plugins { get; init; }
    public DbSet<PluginReleaseEntity> PluginReleases { get; init; }
    public DbSet<ReleaseEntity> Releases { get; init; }
    public DbSet<AppFileEntity> AppFiles { get; init; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new TokenConfiguration());
        modelBuilder.ApplyConfiguration(new PluginConfiguration());
        modelBuilder.ApplyConfiguration(new PluginReleaseConfiguration());
        modelBuilder.ApplyConfiguration(new ReleaseConfiguration());
        modelBuilder.ApplyConfiguration(new AppFileConfiguration());
    }
}