using Microsoft.EntityFrameworkCore;
using TestGeneratorAPI.Core.Abstractions;
using TestGeneratorAPI.Core.Models;
using TestGeneratorAPI.DataAccess.Context;
using TestGeneratorAPI.DataAccess.Entities;

namespace TestGeneratorAPI.DataAccess.Repositories;

public class ReleaseRepository : IReleaseRepository
{
    private readonly TestGeneratorDbContext _dbContext;
    
    public ReleaseRepository(TestGeneratorDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ReleaseRead> Get(Guid releaseId)
    {
        return Convert(await _dbContext.Releases.Where(e => e.ReleaseId == releaseId).SingleAsync());
    }

    public async Task<ReleaseRead> GetLatest()
    {
        return Convert(await _dbContext.Releases.OrderByDescending(e => e.CreatedAt).FirstAsync());
    }

    public async Task<ReleaseRead> GetLatest(string runtime)
    {
        return Convert(await _dbContext.Releases
            .Where(e => e.Runtime == runtime)
            .OrderByDescending(e => e.CreatedAt)
            .FirstAsync());
    }

    public async Task<Guid> CreateRelease(Guid id, string runtime, Version version)
    {
        await _dbContext.Releases.AddAsync(new ReleaseEntity
        {
            ReleaseId = id,
            Runtime = runtime,
            Version = version.ToString()
        });
        await _dbContext.SaveChangesAsync();
        return id;
    }

    private static ReleaseRead Convert(ReleaseEntity entity)
    {
        return new ReleaseRead
        {
            ReleaseId = entity.ReleaseId,
            Runtime = entity.Runtime,
            Version = Version.Parse(entity.Version),
            CreatedAt = entity.CreatedAt
        };
    }
}