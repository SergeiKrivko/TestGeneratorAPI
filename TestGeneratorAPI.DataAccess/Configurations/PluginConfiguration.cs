using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TestGeneratorAPI.DataAccess.Entities;

namespace TestGeneratorAPI.DataAccess.Configurations;

public class PluginConfiguration : IEntityTypeConfiguration<PluginEntity>
{
    public void Configure(EntityTypeBuilder<PluginEntity> builder)
    { 
        builder.HasKey(x => x.PluginId);

        builder.Property(x => x.PluginId)
            .ValueGeneratedOnAdd();
        
        builder.Property(b => b.Key)
            .IsRequired();
        
        builder.Property(b => b.OwnerId)
            .IsRequired();
        
        builder.Property(b => b.CreatedAt)
            .IsRequired();

        builder.Property(b => b.DeletedAt);
        
        builder.HasOne(l => l.Owner)
            .WithMany(t => t.Plugins)
            .HasForeignKey(l => l.OwnerId);
    }
}