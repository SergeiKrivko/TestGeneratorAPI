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

            await PrometheusRepositoryHistogram.Measure(_dbContext.Users.AddAsync(entity));
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
            var entity =
                await PrometheusRepositoryHistogram.Measure(_dbContext.Users.Where(u => u.Login == login)
                    .SingleAsync());
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
            var entity =
                await PrometheusRepositoryHistogram.Measure(_dbContext.Users.Where(u => u.UserId == userId)
                    .SingleAsync());
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
}