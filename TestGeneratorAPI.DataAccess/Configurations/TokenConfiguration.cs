using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TestGeneratorAPI.DataAccess.Entities;

namespace TestGeneratorAPI.DataAccess.Configurations;

public class TokenConfiguration : IEntityTypeConfiguration<TokenEntity>
{
    public void Configure(EntityTypeBuilder<TokenEntity> builder)
    { 
        builder.HasKey(x => x.TokenId);

        builder.Property(x => x.TokenId)
            .IsRequired();
        
        builder.Property(b => b.UserId)
            .IsRequired();
        
        builder.Property(b => b.Permissions)
            .IsRequired();
        
        builder.Property(b => b.Type)
            .IsRequired();
        
        builder.Property(b => b.Name)
            .IsRequired();
        
        builder.Property(b => b.CreatedAt)
            .IsRequired();
        
        builder.Property(b => b.ExpiresAt)
            .IsRequired();
        
        builder.Property(b => b.DeletedAt)
            .HasDefaultValue(null);
    }
}