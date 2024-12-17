using Microsoft.EntityFrameworkCore;
using TestGeneratorAPI.Core.Abstractions;
using TestGeneratorAPI.Core.Exceptions.Repositories;
using TestGeneratorAPI.Core.Models;
using TestGeneratorAPI.DataAccess.Context;
using TestGeneratorAPI.DataAccess.Entities;

namespace TestGeneratorAPI.DataAccess.Repositories;

public class AppFilesRepository : IAppFilesRepository
{
    private readonly TestGeneratorDbContext _dbContext;

    public AppFilesRepository(TestGeneratorDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<AppFileRead> Get(Guid id)
    {
        try
        {
            var entity = await _dbContext.AppFiles
                .Where(p => p.Id == id && p.DeletedAt == null).SingleAsync();
            return Convert(entity);
        }
        catch (Exception e)
        {
            throw new AppFilesRepositoryException("Failed to get app file", e);
        }
    }

    public async Task<AppFileRead> Get(string filename, Version version, string runtime)
    {
        try
        {
            var entity = await _dbContext.AppFiles
                .Where(e => e.Version == version.ToString() && e.Runtime == runtime &&
                            e.Filename == filename && e.DeletedAt == null).LastAsync();
            return Convert(entity);
        }
        catch (Exception e)
        {
            throw new AppFilesRepositoryException($"Failed to get app file {e}", e);
        }
    }

    public async Task<AppFileRead> GetLatest(string filename, string runtime)
    {
        try
        {
            var entity = await _dbContext.AppFiles
                .Where(e => e.Runtime == runtime && e.Filename == filename && e.DeletedAt == null)
                .OrderBy(p => p.CreatedAt).LastAsync();
            return Convert(entity);
        }
        catch (Exception e)
        {
            throw new AppFilesRepositoryException($"Failed to get app file {e}", e);
        }
    }

    public async Task<List<AppFileRead>> GetAll(Version version, string runtime)
    {
        try
        {
            return await _dbContext.AppFiles.Where(e =>
                    e.Version == version.ToString() && e.Runtime == runtime && e.DeletedAt == null)
                .Select(e => Convert(e))
                .ToListAsync();
        }
        catch (Exception e)
        {
            throw new AppFilesRepositoryException("Failed to get all app files", e);
        }
    }

    public async Task<Guid> Create(Guid id, string filename, Version version, string runtime, string hash)
    {
        try
        {
            await _dbContext.AppFiles.AddAsync(new AppFileEntity
            {
                Id = id,
                Filename = filename,
                Version = version.ToString(),
                CreatedAt = DateTime.UtcNow,
                Hash = hash,
                Runtime = runtime
            });
            await _dbContext.SaveChangesAsync();
            return id;
        }
        catch (Exception e)
        {
            throw new AppFilesRepositoryException("Failed to get all app files", e);
        }
    }

    private static AppFileRead Convert(AppFileEntity entity)
    {
        return new AppFileRead
        {
            Id = entity.Id,
            Filename = entity.Filename,
            Runtime = entity.Runtime,
            Version = Version.Parse(entity.Version),
            CreatedAt = entity.CreatedAt,
            DeletedAt = entity.DeletedAt,
            Hash = entity.Hash,
        };
    }

    private static AppFileEntity Convert(AppFileRead model)
    {
        return new AppFileEntity
        {
            Id = model.Id,
            Filename = model.Filename,
            Runtime = model.Runtime,
            Version = model.Version.ToString(),
            CreatedAt = model.CreatedAt,
            DeletedAt = model.DeletedAt,
            Hash = model.Hash,
        };
    }
}