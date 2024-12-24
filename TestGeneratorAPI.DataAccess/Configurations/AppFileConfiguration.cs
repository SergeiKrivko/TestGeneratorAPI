using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TestGeneratorAPI.DataAccess.Entities;

namespace TestGeneratorAPI.DataAccess.Configurations;

public class AppFileConfiguration : IEntityTypeConfiguration<AppFileEntity>
{
    public void Configure(EntityTypeBuilder<AppFileEntity> builder)
    { 
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedOnAdd();
        
        builder.Property(b => b.ReleaseId)
            .IsRequired();
        
        builder.Property(b => b.S3Id)
            .IsRequired();
        
        builder.Property(b => b.Filename)
            .IsRequired();
        
        builder.Property(b => b.CreatedAt)
            .IsRequired();

        builder.Property(b => b.DeletedAt);
        
        builder.Property(b => b.Hash)
            .IsRequired();
        
        builder.HasOne(l => l.Release)
            .WithMany(t => t.Files)
            .HasForeignKey(l => l.ReleaseId);
    }
}