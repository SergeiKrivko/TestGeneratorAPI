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
            var entity = await PrometheusRepositoryHistogram.Measure(_dbContext.AppFiles
                .Where(p => p.Id == id && p.DeletedAt == null).SingleAsync());
            return Convert(entity);
        }
        catch (Exception e)
        {
            throw new AppFilesRepositoryException("Failed to get app file", e);
        }
    }

    public async Task<List<AppFileRead>> GetAll(Guid releaseId)
    {
        try
        {
            return await PrometheusRepositoryHistogram.Measure(_dbContext.AppFiles
                .Where(e => e.ReleaseId == releaseId && e.DeletedAt == null)
                .Select(e => Convert(e))
                .ToListAsync());
        }
        catch (Exception e)
        {
            throw new AppFilesRepositoryException($"Failed to get app file {e}", e);
        }
    }

    public async Task<Guid> Create(Guid id, Guid releaseId, Guid s3Id, string filename, string hash)
    {
        try
        {
            await PrometheusRepositoryHistogram.Measure(_dbContext.AppFiles.AddAsync(new AppFileEntity
            {
                Id = id,
                Filename = filename,
                ReleaseId = releaseId,
                S3Id = s3Id,
                CreatedAt = DateTime.UtcNow,
                Hash = hash,
            }));
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
            S3Id = entity.S3Id,
            Filename = entity.Filename,
            CreatedAt = entity.CreatedAt,
            DeletedAt = entity.DeletedAt,
            Hash = entity.Hash,
        };
    }
}