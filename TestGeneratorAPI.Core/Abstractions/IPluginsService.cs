using TestGeneratorAPI.Core.Models;

namespace TestGeneratorAPI.Core.Abstractions;

public interface IPluginsService
{
    Task<PluginRead> GetPlugin(Guid pluginId);
    
    Task<ICollection<PluginRead>> GetAllPlugins();
    
    Task<PluginRead> GetPluginByKey(string key);
    
    Task<Guid> CreatePlugin(PluginCreate pluginCreate, Guid userId);

    Task<Guid> DeletePlugin(Guid pluginId);
}