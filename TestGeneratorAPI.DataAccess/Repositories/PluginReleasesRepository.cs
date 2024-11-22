using Microsoft.EntityFrameworkCore;
using TestGeneratorAPI.Core.Abstractions;
using TestGeneratorAPI.Core.Exceptions.Repositories;
using TestGeneratorAPI.Core.Models;
using TestGeneratorAPI.DataAccess.Context;
using TestGeneratorAPI.DataAccess.Entities;

namespace TestGeneratorAPI.DataAccess.Repositories;

public class PluginReleasesRepository : IPluginReleasesRepository
{
    private readonly TestGeneratorDbContext _dbContext;

    public PluginReleasesRepository(TestGeneratorDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PluginReleaseRead> Get(Guid pluginId)
    {
        try
        {
            var entity = await _dbContext.PluginReleases
                .Where(p => p.PluginReleaseId == pluginId && p.DeletedAt == null).SingleAsync();
            return Convert(entity);
        }
        catch (Exception e)
        {
            throw new PluginReleasesRepositoryException("Failed to get plugin release", e);
        }
    }

    public async Task<PluginReleaseRead> GetLatest(Guid pluginId, string runtime)
    {
        try
        {
            var entity = await _dbContext.PluginReleases
                .Where(e => e.PluginId == pluginId && (e.Runtime == runtime || e.Runtime == null) &&
                            e.DeletedAt == null)
                .OrderBy(p => p.Version).LastAsync();
            return Convert(entity);
        }
        catch (Exception e)
        {
            throw new PluginReleasesRepositoryException($"Failed to get latest release of plugin {pluginId} {e}", e);
        }
    }

    public async Task<List<PluginReleaseRead>> GetAll(Guid pluginId)
    {
        try
        {
            return await _dbContext.PluginReleases.Where(e => e.PluginId == pluginId && e.DeletedAt == null)
                .Select(e => Convert(e))
                .ToListAsync();
        }
        catch (Exception e)
        {
            throw new PluginReleasesRepositoryException($"Failed to get all releases of plugin {pluginId}", e);
        }
    }

    public async Task<List<PluginReleaseRead>> GetAll(Guid pluginId, string runtime)
    {
        try
        {
            return await _dbContext.PluginReleases
                .Where(e => e.PluginId == pluginId && e.Runtime == runtime && e.DeletedAt == null)
                .Select(e => Convert(e))
                .ToListAsync();
        }
        catch (Exception e)
        {
            throw new PluginReleasesRepositoryException($"Failed to get all releases of plugin {pluginId}", e);
        }
    }

    public async Task<Guid> Create(Guid id, Guid pluginId, Guid userId, string name, string? description,
        Version version, string? runtime, string? url)
    {
        try
        {
            var entity = new PluginReleaseEntity
            {
                PluginReleaseId = id,
                PluginId = pluginId,
                PublisherId = userId,
                Name = name,
                Description = description ?? "",
                Version = version.ToString(),
                Runtime = runtime,
                Url = url,
            };
            await _dbContext.PluginReleases.AddAsync(entity);
            await _dbContext.SaveChangesAsync();
            return id;
        }
        catch (Exception e)
        {
            throw new PluginReleasesRepositoryException("Failed to create plugin release", e);
        }
    }

    public async Task<PluginReleaseRead> GetByVersion(Guid pluginId, string? runtime, Version version)
    {
        try
        {
            var entity = await _dbContext.PluginReleases
                .Where(e => e.PluginId == pluginId && (e.Runtime == runtime || e.Runtime == null) &&
                            e.DeletedAt == null && e.Version == version.ToString())
                .OrderBy(p => p.Runtime).LastAsync();
            return Convert(entity);
        }
        catch (Exception e)
        {
            throw new PluginReleasesRepositoryException($"Failed to get release '{version}' of plugin {pluginId} {e}", e);
        }
    }

    public async Task<bool> ExistsByVersion(Guid pluginId, string? runtime, Version version)
    {
        try
        {
            var entity = await _dbContext.PluginReleases
                .Where(e => e.PluginId == pluginId && (e.Runtime == runtime) &&
                            e.DeletedAt == null && e.Version == version.ToString())
                .OrderBy(p => p.Runtime).LastAsync();
            return true;
        }
        catch (Exception e)
        {
            return false;
        }
    }

    public async Task<Guid> Delete(Guid id)
    {
        try
        {
            await _dbContext.PluginReleases.Where(p => p.PluginReleaseId == id)
                .ExecuteUpdateAsync(s => s.SetProperty(p => p.DeletedAt, DateTime.UtcNow));
            await _dbContext.SaveChangesAsync();
            return id;
        }
        catch (Exception e)
        {
            throw new PluginReleasesRepositoryException("Failed to delete plugin release", e);
        }
    }

    private static PluginReleaseRead Convert(PluginReleaseEntity entity)
    {
        return new PluginReleaseRead
        {
            PluginReleaseId = entity.PluginReleaseId,
            PluginId = entity.PluginId,
            PublisherId = entity.PublisherId,
            Name = entity.Name,
            Description = entity.Description,
            Runtime = entity.Runtime,
            Version = Version.Parse(entity.Version),
            CreatedAt = entity.CreatedAt,
            DeletedAt = entity.DeletedAt,
            Url = entity.Url,
        };
    }

    private static PluginReleaseEntity Convert(PluginReleaseRead model)
    {
        return new PluginReleaseEntity
        {
            PluginReleaseId = model.PluginReleaseId,
            PluginId = model.PluginId,
            PublisherId = model.PublisherId,
            Name = model.Name,
            Description = model.Description,
            Runtime = model.Runtime,
            Version = model.Version.ToString(),
            CreatedAt = model.CreatedAt,
            DeletedAt = model.DeletedAt,
        };
    }
}