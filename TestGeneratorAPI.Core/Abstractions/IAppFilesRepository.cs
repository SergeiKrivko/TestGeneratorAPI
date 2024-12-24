using TestGeneratorAPI.Core.Models;

namespace TestGeneratorAPI.Core.Abstractions;

public interface IAppFilesRepository
{
    public Task<AppFileRead> Get(Guid id);
    
    public Task<List<AppFileRead>> GetAll(Guid releaseId);

    public Task<Guid> Create(Guid id, Guid releaseId, Guid s3Id, string filename, string hash);
}