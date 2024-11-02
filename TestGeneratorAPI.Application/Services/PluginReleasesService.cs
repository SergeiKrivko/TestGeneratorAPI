using TestGeneratorAPI.Core.Abstractions;
using TestGeneratorAPI.Core.Exceptions.Repositories;
using TestGeneratorAPI.Core.Exceptions.Services;
using TestGeneratorAPI.Core.Models;

namespace TestGeneratorAPI.Application.Services;

public class PluginReleasesService : IPluginReleasesService
{
    private readonly IPluginReleasesRepository _pluginReleasesRepository;
    private readonly IPluginsRepository _pluginsRepository;

    public PluginReleasesService(IPluginReleasesRepository pluginReleasesRepository,
        IPluginsRepository pluginsRepository)
    {
        _pluginReleasesRepository = pluginReleasesRepository;
        _pluginsRepository = pluginsRepository;
    }

    public async Task<Guid> CreatePluginRelease(PluginReleaseCreate pluginReleaseCreate, Guid userId)
    {
        var plugin = await _pluginsRepository.Get(pluginReleaseCreate.Key);
        var id = Guid.NewGuid();
        await _pluginReleasesRepository.Create(id, plugin.PluginId, userId, pluginReleaseCreate.Name,
            pluginReleaseCreate.Description, pluginReleaseCreate.Version, pluginReleaseCreate.Runtime, pluginReleaseCreate.Url);
        return id;
    }

    public async Task<PluginReleaseRead> GetPluginRelease(Guid pluginReleaseId)
    {
        return await _pluginReleasesRepository.Get(pluginReleaseId);
    }

    public async Task<ICollection<PluginReleaseRead>> GetPluginReleases(Guid pluginId)
    {
        return await _pluginReleasesRepository.GetAll(pluginId);
    }

    public async Task<ICollection<PluginReleaseRead>> GetPluginReleases(Guid pluginId, string runtime)
    {
        return await _pluginReleasesRepository.GetAll(pluginId, runtime);
    }

    public async Task<PluginReleaseRead> GetLatestRelease(Guid pluginId, string runtime)
    {
        return await _pluginReleasesRepository.GetLatest(pluginId, runtime);
    }

    public async Task<Guid> DeletePluginRelease(Guid pluginReleaseId)
    {
        return await _pluginReleasesRepository.Delete(pluginReleaseId);
    }
}