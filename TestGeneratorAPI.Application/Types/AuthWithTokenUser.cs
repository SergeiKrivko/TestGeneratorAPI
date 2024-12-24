using System.Security.Claims;
using Microsoft.Extensions.FileSystemGlobbing;
using TestGeneratorAPI.Core.Abstractions;
using TestGeneratorAPI.Core.Enums;
using TestGeneratorAPI.Core.Models;

namespace TestGeneratorAPI.Application.Types;

public class AuthWithTokenUser : IUser
{
    public Guid Id { get; }
    public string[] Permissions { get; }

    private readonly TokenRead _token;

    private readonly Matcher? _matcher;
    private readonly Guid[] _plugins = [];

    public AuthWithTokenUser(TokenRead token, ClaimsPrincipal claimsPrincipal)
    {
        _token = token;
        Id = token.UserId;
        Permissions = token.Permissions;

        if (_token.Type == TokenType.Mask)
        {
            _matcher = new Matcher();
            _matcher.AddInclude(claimsPrincipal.Claims.Single(c => c.Type == "Mask").Value);
        }
        else if (_token.Type == TokenType.Plugins)
        {
            _plugins = claimsPrincipal.Claims.Single(c => c.Type == "Mask").Value.Split(';').Select(Guid.Parse)
                .ToArray();
        }
    }

    public bool HavePermission(TokenPermission permission) =>
        Permissions.Contains(permission.Key) && permission.TokenTypes.Contains(_token.Type);

    public bool HavePlugin(string key)
    {
        if (_token.Type == TokenType.Mask)
            return _matcher?.Match(key).HasMatches ?? false;
        return _token.Type != TokenType.Plugins;
    }

    public bool HavePlugin(PluginRead plugin)
    {
        if (_token.Type != TokenType.Admin && Id != plugin.OwnerId)
            return false;
        if (_token.Type == TokenType.Mask)
            return _matcher?.Match(plugin.Key).HasMatches ?? false;
        if (_token.Type == TokenType.Plugins)
            return _plugins.Contains(plugin.PluginId);
        return true;
    }
}