using Microsoft.EntityFrameworkCore;
using TestGeneratorAPI.Core.Abstractions;
using TestGeneratorAPI.Core.Exceptions.Repositories;
using TestGeneratorAPI.Core.Models;
using TestGeneratorAPI.DataAccess.Context;
using TestGeneratorAPI.DataAccess.Entities;

namespace TestGeneratorAPI.DataAccess.Repositories;

public class UsersRepository : IUsersRepository
{
    private readonly TestGeneratorDbContext _dbContext;
    
    public UsersRepository(TestGeneratorDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    // public async Task<List<UserRead>> Get(Guid id)
    // {
    //     try
    //     {
    //         IQueryable<UserEntity> templatesQuery;
    //         
    //         if (string.IsNullOrWhiteSpace(name))
    //         {
    //             templatesQuery = _dbContext.Users
    //                 .AsNoTracking()
    //                 .Where(t =>  t.CreatorID == creatorid);
    //         }
    //         else
    //         {
    //             templatesQuery = _dbContext.Users
    //                 .AsNoTracking()
    //                 .Where(t =>
    //                     t.Name.ToLower().Contains(name.ToLower()) && t.CreatorID == creatorid);
    //         }
    //         
    //         
    //         var templateEntities = await templatesQuery
    //             .ToListAsync();
    //     
    //         var templates = templateEntities
    //             .Select(b => UserConverter.ToUserDomain(b))
    //             .ToList();
    //     
    //         return templates;
    //     }
    //     catch (Exception ex)
    //     {
    //         throw new UserRepositoryException("Failed to get all templates", ex);
    //     }
    // }
    
    public async Task<Guid> Create(Guid id, string login, string passwordHash, string name) 
    {
        try
        {
            var entity = new UserEntity
            {
                UserId = id,
                Login = login,
                Name = name,
                PasswordHash = passwordHash,
            };
            
            await _dbContext.Users.AddAsync(entity); 
            await _dbContext.SaveChangesAsync();

            return id;
        }
        catch (Exception ex)
        {
            throw new UserRepositoryException("Failed to create template", ex);
        }
    }

    public async Task<AuthorizedUserRead> GetLastByLogin(string login)
    {
        try
        {
            var entity = await _dbContext.Users.Where(u => u.Login == login).SingleAsync();
            return new AuthorizedUserRead
            {
                UserId = entity.UserId,
                Login = entity.Login,
                Name = entity.Name,
                PasswordHash = entity.PasswordHash,
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt,
                DeletedAt = entity.DeletedAt,
            };
        }
        catch (Exception e)
        {
            throw new UserRepositoryException("Failed to create template", e);
        }
    }

    public async Task<AuthorizedUserRead> Get(Guid userId)
    {
        try
        {
            var entity = await _dbContext.Users.Where(u => u.UserId == userId).SingleAsync();
            return new AuthorizedUserRead
            {
                UserId = entity.UserId,
                Login = entity.Login,
                Name = entity.Name,
                PasswordHash = entity.PasswordHash,
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt,
                DeletedAt = entity.DeletedAt,
            };
        }
        catch (Exception e)
        {
            throw new UserRepositoryException("Failed to create template", e);
        }
    }
    
    // public async Task<User> Update(Guid templateid, Guid creatorid, string name, string text, DateTime updatedat) 
    // {
    //     try
    //     {
    //         var rowsAffected = await _dbContext.Users
    //             .Where(b => b.UserId == templateid && b.CreatorID == creatorid) 
    //             .ExecuteUpdateAsync(s => s
    //                 .SetProperty(b => b.Name, b => name) 
    //                 .SetProperty(b => b.Text, b => text)
    //                 .SetProperty(b => b.UpdatedAt, b => updatedat));
    //     
    //         if (rowsAffected == 0)
    //         {
    //             throw new EntityNotFoundException("User with the specified ID not found");
    //         }
    //
    //         var updatedUserEntity = await _dbContext.Users
    //             .AsNoTracking()
    //             .FirstOrDefaultAsync(b => b.UserId == templateid && b.CreatorID == creatorid);
    //
    //         return UserConverter.ToUserDomain(updatedUserEntity);
    //     }
    //     catch (Exception ex)
    //     {
    //         throw new UserRepositoryException("Failed to update template", ex);
    //     }
    // }
    
    // public async Task<Guid> Delete(Guid id)
    // {
    //     try
    //     {
    //         var rowsAffected = await _dbContext.Users
    //             .Where(b => b.UserId == id)
    //             .ExecuteDeleteAsync();
    //
    //         if (rowsAffected == 0)
    //         {
    //             throw new EntityNotFoundException("User with the specified ID not found");
    //         }
    //     }
    //     catch (Exception ex)
    //     {
    //         throw new UserRepositoryException("Failed to delete template", ex);
    //     }
    // }
}