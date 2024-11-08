using TestGeneratorAPI.Core.Models;

namespace TestGeneratorAPI.Core.Abstractions;

public interface IPluginReleasesService
{
    Task<PluginReleaseRead> GetPluginRelease(Guid releaseId);
    
    Task<PluginReleaseRead> GetPluginRelease(Guid pluginId, Version version, string? runtime = null);
    
    Task<bool> PluginReleaseExists(Guid pluginId, Version version, string? runtime = null);
    
    Task<ICollection<PluginReleaseRead>> GetPluginReleases(Guid pluginId);
    
    Task<ICollection<PluginReleaseRead>> GetPluginReleases(Guid pluginId, string runtime);
    
    Task<PluginReleaseRead> GetLatestRelease(Guid pluginId, string runtime);
    
    Task<Guid> CreatePluginRelease(PluginReleaseCreate pluginCreate, Guid pluginId);

    Task<Guid> DeletePluginRelease(Guid pluginId);
}