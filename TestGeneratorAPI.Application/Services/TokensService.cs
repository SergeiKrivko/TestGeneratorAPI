using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.Extensions.FileSystemGlobbing;
using TestGeneratorAPI.Application.Types;
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

    public async Task<IUser?> GetUser(ClaimsPrincipal claims)
    {
        try
        {
            if (claims.HasClaim(c => c.Type == "UserId"))
            {
                var token = await _tokensRepository.Get(Guid.Parse(claims.Claims.Single(c => c.Type == "TokenId").Value));
                return new AuthWithTokenUser(token, claims);
            }

            if (claims.Identity?.Name == null)
                return null;

            return new AuthWithPasswordUser((await _usersRepository.GetLastByLogin(claims.Identity.Name)).UserId);
        }
        catch (Exception)
        {
            return null;
        }
    }
}