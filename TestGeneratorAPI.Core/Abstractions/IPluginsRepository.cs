using TestGeneratorAPI.Core.Models;

namespace TestGeneratorAPI.Core.Abstractions;

public interface IPluginsRepository
{
    Task<List<PluginRead>> GetAll();
    
    Task<List<PluginRead>> GetAll(Guid userId);
    
    Task<List<Guid>> GetByKeys(ICollection<string> keys, Guid userId);

    Task<PluginRead> Get(Guid pluginId);
    
    Task<PluginRead> Get(string key);
    
    Task<bool> Exists(string key);

    Task<Guid> Create(Guid id, Guid ownerId, string key);

    Task<Guid> Delete(Guid id);
}