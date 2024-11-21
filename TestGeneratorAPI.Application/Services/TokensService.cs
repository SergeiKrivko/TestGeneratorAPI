using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Text.Json;
using TestGeneratorAPI.Core.Abstractions;
using TestGeneratorAPI.Core.Exceptions.Repositories;
using TestGeneratorAPI.Core.Exceptions.Services;
using TestGeneratorAPI.Core.Models;

namespace TestGeneratorAPI.Application.Services;

public class TokensService : ITokensService
{
    private readonly ITokensRepository _tokensRepository;
    private readonly IPluginsRepository _pluginsRepository;

    public const string Issuer = "TesteneratorAPI";
    public const string Audience = "AccessToken";

    public static string Key => Environment.GetEnvironmentVariable("TOKEN_SECRET") ?? "";

    public static SymmetricSecurityKey GetSymmetricSecurityKey() =>
        new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Key));

    public TokensService(ITokensRepository tokensRepository, IPluginsRepository pluginsRepository)
    {
        _tokensRepository = tokensRepository;
        _pluginsRepository = pluginsRepository;
    }

    public async Task<string> CreateToken(TokenCreate tokenCreate, Guid userId)
    {
        try
        {
            var id = Guid.NewGuid();

            var pluginsIds = await _pluginsRepository.GetByKeys(tokenCreate.Plugins, userId);
            if (pluginsIds.Count < tokenCreate.Plugins.Length)
                throw new TokenServiceException("Failed to create token: permission denied");

            var claims = new List<Claim>
            {
                new("Id", id.ToString()),
                new("UserId", userId.ToString()),
                new("Plugins", JsonSerializer.Serialize(pluginsIds))
            };
            // создаем JWT-токен
            var jwt = new JwtSecurityToken(
                issuer: Issuer,
                audience: Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddYears(3),
                signingCredentials: new SigningCredentials(GetSymmetricSecurityKey(),
                    SecurityAlgorithms.HmacSha256)
            );
            await _tokensRepository.Create(id, userId, tokenCreate.Name);
            return new JwtSecurityTokenHandler().WriteToken(jwt);
        }
        catch (UserRepositoryException ex)
        {
            throw new UserServiceException("Failed template service whit creating template", ex);
        }
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
        catch (Exception e)
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
}