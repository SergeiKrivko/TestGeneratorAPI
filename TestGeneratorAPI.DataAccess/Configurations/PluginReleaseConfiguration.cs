using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TestGeneratorAPI.DataAccess.Entities;

namespace TestGeneratorAPI.DataAccess.Configurations;

public class PluginReleaseConfiguration : IEntityTypeConfiguration<PluginReleaseEntity>
{
    public void Configure(EntityTypeBuilder<PluginReleaseEntity> builder)
    { 
        builder.HasKey(x => x.PluginReleaseId);

        builder.Property(x => x.PluginReleaseId)
            .ValueGeneratedOnAdd();

        builder.Property(x => x.PluginId)
            .IsRequired();
        
        builder.Property(b => b.PublisherId)
            .IsRequired();
        
        builder.Property(b => b.Name)
            .IsRequired();
        
        builder.Property(b => b.Description)
            .IsRequired();
        
        builder.Property(b => b.Version)
            .IsRequired();

        builder.Property(b => b.Runtime);
        
        builder.Property(b => b.CreatedAt)
            .IsRequired();
        
        builder.Property(b => b.DeletedAt);
        
        builder.HasOne(l => l.Publisher)
            .WithMany(t => t.PluginReleases)
            .HasForeignKey(l => l.PublisherId);
        
        builder.HasOne(l => l.Plugin)
            .WithMany(t => t.Releases)
            .HasForeignKey(l => l.PluginId);
        
    }
}