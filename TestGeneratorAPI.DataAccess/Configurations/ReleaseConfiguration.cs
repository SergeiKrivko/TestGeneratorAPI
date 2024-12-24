using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TestGeneratorAPI.DataAccess.Entities;

namespace TestGeneratorAPI.DataAccess.Configurations;

public class ReleaseConfiguration : IEntityTypeConfiguration<ReleaseEntity>
{
    public void Configure(EntityTypeBuilder<ReleaseEntity> builder)
    { 
        builder.HasKey(x => x.ReleaseId);

        builder.Property(x => x.ReleaseId)
            .ValueGeneratedOnAdd();
        
        builder.Property(b => b.Runtime)
            .IsRequired();
        
        builder.Property(b => b.Version)
            .IsRequired();
        
        builder.Property(b => b.CreatedAt)
            .IsRequired();
    }
}