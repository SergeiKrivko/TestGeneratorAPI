using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TestGeneratorAPI.DataAccess.Entities;

namespace TestGeneratorAPI.DataAccess.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<UserEntity>
{
    public void Configure(EntityTypeBuilder<UserEntity> builder)
    { 
        builder.HasKey(x => x.UserId);

        builder.Property(x => x.UserId)
            .IsRequired();
        
        builder.Property(b => b.Login)
            .IsRequired();
        
        builder.Property(b => b.PasswordHash)
            .IsRequired();
        
        builder.Property(b => b.Name)
            .IsRequired();
        
        builder.Property(b => b.CreatedAt)
            .IsRequired();
        
        builder.Property(b => b.UpdatedAt)
            .IsRequired();
        
        builder.Property(b => b.DeletedAt)
            .HasDefaultValue(null);
    }
}