using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.Extensions.FileSystemGlobbing;
using TestGeneratorAPI.Core.Abstractions;
using TestGeneratorAPI.Core.Enums;
using TestGeneratorAPI.Core.Exceptions.Services;
using TestGeneratorAPI.Core.Models;

namespace TestGeneratorAPI.Application.Services;

public class TokensService : ITokensService
{
    private readonly ITokensRepository _tokensRepository;
    private readonly IPluginsRepository _pluginsRepository;
    private readonly IUsersRepository _usersRepository;

    public const string Issuer = "TesteneratorAPI";
    public const string Audience = "AccessToken";

    private static string Key => Environment.GetEnvironmentVariable("TOKEN_SECRET") ?? "";

    public static SymmetricSecurityKey GetSymmetricSecurityKey() =>
        new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Key));

    public TokensService(ITokensRepository tokensRepository, IPluginsRepository pluginsRepository,
        IUsersRepository usersRepository)
    {
        _tokensRepository = tokensRepository;
        _pluginsRepository = pluginsRepository;
        _usersRepository = usersRepository;
    }

    public async Task<string> CreateToken(TokenCreate tokenCreate, Guid userId)
    {
        var id = Guid.NewGuid();

        var claims = new List<Claim>
        {
            new("TokenId", id.ToString()),
            new("UserId", userId.ToString()),
            new("Type", tokenCreate.Type.ToString()),
            new("Permissions", string.Join(';', tokenCreate.Permissions)),
        };

        switch (tokenCreate.Type)
        {
            case TokenType.Plugins:
                if (tokenCreate.Plugins == null)
                    throw new Exception("Plugins field required");
                var pluginsIds = await _pluginsRepository.GetByKeys(tokenCreate.Plugins, userId);
                if (pluginsIds.Count < tokenCreate.Plugins.Length)
                    throw new TokenServiceException("Failed to create token: permission denied");
                claims.Add(new Claim("Plugins", string.Join(';', pluginsIds)));
                break;
            case TokenType.Mask:
                if (tokenCreate.Mask == null)
                    throw new Exception("Mask field required");
                claims.Add(new Claim("Mask", tokenCreate.Mask));
                break;
            case TokenType.User:
                break;
            case TokenType.Admin:
                break;
            default:
                throw new Exception("Invalid token type");
        }

        // создаем JWT-токен
        var expires = tokenCreate.ExpiresAt ?? DateTime.UtcNow.AddYears(3);
        var jwt = new JwtSecurityToken(
            issuer: Issuer,
            audience: Audience,
            claims: claims,
            expires: expires,
            signingCredentials: new SigningCredentials(GetSymmetricSecurityKey(),
                SecurityAlgorithms.HmacSha256)
        );
        await _tokensRepository.Create(id, userId, tokenCreate.Type, tokenCreate.Name, expires,
            tokenCreate.Permissions);
        return new JwtSecurityTokenHandler().WriteToken(jwt);
    }

    public async Task<List<TokenRead>> GetTokensOfUser(Guid userId)
    {
        return await _tokensRepository.GetAll(userId);
    }

    public async Task<bool> IsAlive(Guid id)
    {
        try
        {
            var token = await _tokensRepository.Get(id);
            return token.DeletedAt == null;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<Guid> DeleteToken(Guid tokenId)
    {
        return await _tokensRepository.Delete(tokenId);
    }

    public Task<TokenRead> GetToken(Guid tokenId)
    {
        return _tokensRepository.Get(tokenId);
    }

    public async Task<bool> CheckPermissions(ClaimsPrincipal claims, TokenPermission permission, object id)
    {
        try
        {
            if (!claims.HasClaim(c => c.Type == "TokenId"))
                return true;
            var tokenId = Guid.Parse(claims.Claims.Single(c => c.Type == "TokenId").Value);
            var userId = Guid.Parse(claims.Claims.Single(c => c.Type == "UserId").Value);
            var token = await GetToken(tokenId);
            
            Console.WriteLine($"Token type = {token.Type}");

            if (token.UserId != userId || token.DeletedAt != null)
                return false;

            if (!token.Permissions.Contains(permission.Key))
                return false;

            switch (token.Type)
            {
                case TokenType.Admin:
                    return true;
                case TokenType.User:
                    return !permission.AdminOnly;
                case TokenType.Mask:
                {
                    var matcher = new Matcher();
                    matcher.AddInclude(claims.Claims.Single(c => c.Type == "Mask").Value);
                    
                    if (permission.Key == "createPlugin")
                    {
                        return matcher.Match((string)id).HasMatches;
                    }
                    if (permission.Key != "createRelease" &&
                        permission.Key != "removeRelease" && permission.Key != "removePlugin")
                        return false;
                    
                    var plugin = await _pluginsRepository.Get((Guid)id);
                    return matcher.Match(plugin.Key).HasMatches;
                }
                case TokenType.Plugins:
                {
                    if (permission.Key != "createRelease" && permission.Key != "removeRelease" &&
                        permission.Key != "removePlugin")
                        return false;
                    var plugin = await _pluginsRepository.Get((Guid)id);
                    var pluginIds = claims.Claims.Single(c => c.Type == "Plugins").Value.Split(';').Select(Guid.Parse);
                    return pluginIds.Contains(plugin.PluginId);
                }
                default:
                    return false;
            }
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<AuthorizedUserRead?> GetUser(ClaimsPrincipal claims)
    {
        try
        {
            if (claims.HasClaim(c => c.Type == "UserId"))
            {
                return await _usersRepository.Get(Guid.Parse(claims.Claims.Single(c => c.Type == "UserId").Value));
            }

            if (claims.Identity?.Name == null)
                return null;

            return await _usersRepository.GetLastByLogin(claims.Identity.Name);
        }
        catch (Exception)
        {
            return null;
        }
    }

    public async Task<AuthorizedUserRead?> GetUser(ClaimsPrincipal claims, TokenPermission permission, object id)
    {
        var user = await GetUser(claims);
        if (user == null || !await CheckPermissions(claims, permission, id))
            return null;
        return user;
    }

    public async Task<AuthorizedUserRead?> GetUser(ClaimsPrincipal claims, TokenPermission permission)
    {
        var user = await GetUser(claims);
        if (user == null || !await CheckPermissions(claims, permission, user.UserId))
            return null;
        return user;
    }
}