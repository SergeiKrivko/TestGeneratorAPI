using Microsoft.EntityFrameworkCore;
using TestGeneratorAPI.Core.Abstractions;
using TestGeneratorAPI.Core.Exceptions.Repositories;
using TestGeneratorAPI.Core.Models;
using TestGeneratorAPI.DataAccess.Context;
using TestGeneratorAPI.DataAccess.Entities;

namespace TestGeneratorAPI.DataAccess.Repositories;

public class PluginsRepository : IPluginsRepository
{
    private readonly TestGeneratorDbContext _dbContext;

    public PluginsRepository(TestGeneratorDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PluginRead> Get(Guid pluginId)
    {
        try
        {
            var entity = await PrometheusRepositoryHistogram.Measure(_dbContext.Plugins
                .Where(p => p.PluginId == pluginId && p.DeletedAt == null).SingleAsync());
            return Convert(entity);
        }
        catch (Exception e)
        {
            throw new PluginsRepositoryException("Failed to get plugin", e);
        }
    }

    public async Task<PluginRead> Get(string key)
    {
        try
        {
            var entity = await PrometheusRepositoryHistogram.Measure(_dbContext.Plugins
                .Where(p => p.Key == key && p.DeletedAt == null).SingleAsync());
            return Convert(entity);
        }
        catch (Exception e)
        {
            throw new PluginsRepositoryException("Failed to get plugin", e);
        }
    }

    public async Task<bool> Exists(string key)
    {
        try
        {
            return await PrometheusRepositoryHistogram.Measure(_dbContext.Plugins
                .Where(p => p.Key == key && p.DeletedAt == null).AnyAsync());
        }
        catch (Exception e)
        {
            throw new PluginsRepositoryException("Failed to get plugin", e);
        }
    }

    public async Task<List<PluginRead>> GetAll()
    {
        try
        {
            return await PrometheusRepositoryHistogram.Measure(_dbContext.Plugins.Where(e => e.DeletedAt == null)
                .Select(e => Convert(e)).ToListAsync());
        }
        catch (Exception e)
        {
            throw new PluginsRepositoryException("Failed to get all plugins", e);
        }
    }

    public async Task<List<PluginRead>> GetAll(Guid userId)
    {
        try
        {
            return await PrometheusRepositoryHistogram.Measure(_dbContext.Plugins
                .Where(e => e.OwnerId == userId && e.DeletedAt == null)
                .Select(e => Convert(e)).ToListAsync());
        }
        catch (Exception e)
        {
            throw new PluginsRepositoryException($"Failed to get all plugins of user {userId}", e);
        }
    }

    public async Task<List<Guid>> GetByKeys(ICollection<string> keys, Guid userId)
    {
        try
        {
            return await PrometheusRepositoryHistogram.Measure(_dbContext.Plugins
                .Where(e => e.OwnerId == userId && e.DeletedAt == null && keys.Contains(e.Key))
                .Select(e => e.PluginId).ToListAsync());
        }
        catch (Exception e)
        {
            throw new PluginsRepositoryException($"Failed to get all plugins of user {userId}", e);
        }
    }

    public async Task<Guid> Create(Guid id, Guid ownerId, string key)
    {
        try
        {
            var entity = new PluginEntity
            {
                PluginId = id,
                OwnerId = ownerId,
                Key = key,
            };
            await PrometheusRepositoryHistogram.Measure(_dbContext.Plugins.AddAsync(entity));
            await _dbContext.SaveChangesAsync();
            return id;
        }
        catch (Exception e)
        {
            throw new PluginsRepositoryException("Failed to create plugin", e);
        }
    }

    public async Task<Guid> Delete(Guid id)
    {
        try
        {
            await PrometheusRepositoryHistogram.Measure(_dbContext.Plugins.Where(p => p.PluginId == id)
                .ExecuteUpdateAsync(s => s.SetProperty(p => p.DeletedAt, DateTime.UtcNow)));
            await _dbContext.SaveChangesAsync();
            return id;
        }
        catch (Exception e)
        {
            throw new PluginsRepositoryException("Failed to delete plugin", e);
        }
    }

    private static PluginRead Convert(PluginEntity entity)
    {
        return new PluginRead
        {
            PluginId = entity.PluginId,
            Key = entity.Key,
            OwnerId = entity.OwnerId,
            CreatedAt = entity.CreatedAt,
            DeletedAt = entity.DeletedAt,
        };
    }

    private static PluginEntity Convert(PluginRead model)
    {
        return new PluginEntity
        {
            PluginId = model.PluginId,
            Key = model.Key,
            OwnerId = model.OwnerId,
            CreatedAt = model.CreatedAt,
            DeletedAt = model.DeletedAt,
        };
    }
}