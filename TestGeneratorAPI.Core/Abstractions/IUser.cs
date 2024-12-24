using TestGeneratorAPI.Core.Enums;
using TestGeneratorAPI.Core.Models;

namespace TestGeneratorAPI.Core.Abstractions;

public interface IUser
{
    public Guid Id { get; }
    public string[] Permissions { get; }

    public bool HavePermission(TokenPermission permission);

    public bool HavePlugin(PluginRead plugin);
    public bool HavePlugin(string key);
}