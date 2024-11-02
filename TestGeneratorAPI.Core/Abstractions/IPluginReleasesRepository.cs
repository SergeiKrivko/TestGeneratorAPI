using TestGeneratorAPI.Core.Models;

namespace TestGeneratorAPI.Core.Abstractions;

public interface IPluginReleasesRepository
{
    Task<List<PluginReleaseRead>> GetAll(Guid pluginId);

    Task<List<PluginReleaseRead>> GetAll(Guid pluginId, string runtime);

    Task<PluginReleaseRead> Get(Guid id);

    Task<PluginReleaseRead> GetLatest(Guid pluginId, string runtime);

    Task<Guid> Create(Guid id, Guid pluginId, Guid userId, string name, string? description, Version version,
        string? runtime, string? url);

    Task<Guid> Delete(Guid id);
}