using Microsoft.EntityFrameworkCore;
using TestGeneratorAPI.Core.Abstractions;
using TestGeneratorAPI.Core.Enums;
using TestGeneratorAPI.Core.Exceptions.Repositories;
using TestGeneratorAPI.Core.Models;
using TestGeneratorAPI.DataAccess.Context;
using TestGeneratorAPI.DataAccess.Entities;

namespace TestGeneratorAPI.DataAccess.Repositories;

public class TokensRepository : ITokensRepository
{
    private readonly TestGeneratorDbContext _dbContext;
    
    public TokensRepository(TestGeneratorDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<Guid> Create(Guid id, Guid userId, TokenType type, string name, DateTime expiresAt, string[] permissions) 
    {
        try
        {
            var entity = new TokenEntity
            {
                TokenId = id,
                UserId = userId,
                Type = type,
                Name = name,
                ExpiresAt = expiresAt,
                Permissions = permissions,
            };
            
            await _dbContext.Tokens.AddAsync(entity); 
            await _dbContext.SaveChangesAsync();

            return id;
        }
        catch (Exception ex)
        {
            throw new UserRepositoryException("Failed to create token", ex);
        }
    }

    public async Task<TokenRead> Get(Guid tokenId)
    {
        try
        {
            var entity = await _dbContext.Tokens.Where(e => e.TokenId == tokenId).SingleAsync();
            return Convert(entity);
        }
        catch (Exception e)
        {
            throw new TokensRepositoryException("Failed to get token");
        }
    }

    private static TokenRead Convert(TokenEntity entity)
    {
        return new TokenRead
        {
            TokenId = entity.TokenId,
            UserId = entity.UserId,
            Type = entity.Type,
            Name = entity.Name,
            CreatedAt = entity.CreatedAt,
            ExpiresAt = entity.ExpiresAt,
            DeletedAt = entity.DeletedAt,
            Permissions = entity.Permissions,
        };
    }

    public Task<List<TokenRead>> GetAll(Guid userId)
    {
        return _dbContext.Tokens.Where(t => t.UserId == userId && t.DeletedAt == null).Select(e => Convert(e)).ToListAsync();
    }

    public async Task<Guid> Delete(Guid tokenId)
    {
        var entity = await _dbContext.Tokens.Where(t => t.TokenId == tokenId).SingleAsync();
        entity.DeletedAt = DateTime.UtcNow;
        _dbContext.Tokens.Update(entity);
        await _dbContext.SaveChangesAsync();
        return tokenId;
    }
}