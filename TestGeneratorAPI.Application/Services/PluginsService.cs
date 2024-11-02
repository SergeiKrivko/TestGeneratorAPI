using TestGeneratorAPI.Core.Abstractions;
using TestGeneratorAPI.Core.Exceptions.Repositories;
using TestGeneratorAPI.Core.Exceptions.Services;
using TestGeneratorAPI.Core.Models;

namespace TestGeneratorAPI.Application.Services;

public class PluginsService : IPluginsService
{
    private readonly IPluginsRepository _pluginsRepository;

    public PluginsService(IPluginsRepository pluginsRepository)
    {
        _pluginsRepository = pluginsRepository;
    }

    public async Task<Guid> CreatePlugin(PluginCreate pluginCreate, Guid userId)
    {
        if (await _pluginsRepository.Exists(pluginCreate.Key))
            throw new PluginsServiceException($"Failed to create plugin: plugin '{pluginCreate.Key}' already exists");
        var id = Guid.NewGuid();
        await _pluginsRepository.Create(id, userId, pluginCreate.Key);
        return id;
    }

    public async Task<ICollection<PluginRead>> GetAllPlugins()
    {
        return await _pluginsRepository.GetAll();
    }

    public async Task<PluginRead> GetPlugin(Guid pluginId)
    {
        return await _pluginsRepository.Get(pluginId);
    }

    public async Task<PluginRead> GetPluginByKey(string key)
    {
        return await _pluginsRepository.Get(key);
    }

    public async Task<Guid> DeletePlugin(Guid pluginId)
    {
        return await _pluginsRepository.Delete(pluginId);
    }
}