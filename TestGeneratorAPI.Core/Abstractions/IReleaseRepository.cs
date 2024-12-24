using TestGeneratorAPI.Core.Models;

namespace TestGeneratorAPI.Core.Abstractions;

public interface IReleaseRepository
{
    public Task<ReleaseRead> Get(Guid releaseId);
    public Task<ReleaseRead> GetLatest();
    public Task<ReleaseRead> GetLatest(string runtime);
    public Task<Guid> CreateRelease(Guid id, string runtime, Version version);
}