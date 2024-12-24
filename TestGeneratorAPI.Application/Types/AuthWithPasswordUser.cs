using System.Security.Claims;
using Microsoft.Extensions.FileSystemGlobbing;
using TestGeneratorAPI.Core.Abstractions;
using TestGeneratorAPI.Core.Enums;
using TestGeneratorAPI.Core.Models;

namespace TestGeneratorAPI.Application.Types;

public class AuthWithPasswordUser : IUser
{
    public Guid Id { get; }
    public string[] Permissions { get; } = TokenPermission.All.Where(t => t.TokenTypes.Contains(TokenType.User)).Select(t => t.Key)
        .ToArray();

    public AuthWithPasswordUser(Guid userId)
    {
        Id = userId;
    }

    public bool HavePermission(TokenPermission permission) =>
        Permissions.Contains(permission.Key) && permission.TokenTypes.Contains(TokenType.User);

    public bool HavePlugin(PluginRead plugin)
    {
        return plugin.OwnerId == Id;
    }

    public bool HavePlugin(string key)
    {
        return true;
    }
}