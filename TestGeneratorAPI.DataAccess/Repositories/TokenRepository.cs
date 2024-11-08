using Microsoft.EntityFrameworkCore;
using TestGeneratorAPI.Core.Abstractions;
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
    
    public async Task<Guid> Create(Guid id, Guid userId, string name) 
    {
        try
        {
            var entity = new TokenEntity
            {
                TokenId = id,
                UserId = userId,
                Name = name,
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
            return new TokenRead
            {
                TokenId = entity.TokenId,
                Name = entity.Name,
                UserId = entity.UserId,
                CreatedAt = entity.CreatedAt,
                DeletedAt = entity.DeletedAt,
            };
        }
        catch (Exception e)
        {
            throw new TokensRepositoryException("Failed to get token");
        }
    }
}