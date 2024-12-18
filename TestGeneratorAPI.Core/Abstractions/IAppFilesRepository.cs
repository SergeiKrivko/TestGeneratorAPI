using TestGeneratorAPI.Core.Models;

namespace TestGeneratorAPI.Core.Abstractions;

public interface IAppFilesRepository
{
    public Task<AppFileRead> Get(Guid id);

    public Task<AppFileRead> Get(string filename, Version version, string runtime);
    public Task<AppFileRead> GetLatest(string filename, string runtime);
    
    public Task<List<AppFileRead>> GetAll(Version version, string runtime);

    public Task<Guid> Create(Guid id, string filename, Version version, string runtime, string hash);

    public Task<List<AppFileRead>> GetAllLatest(string runtime);
}