using AspNetCore.Authentication.Basic;
using TestGeneratorAPI.Core.Abstractions;
using TestGeneratorAPI.Core.Exceptions.Repositories;
using TestGeneratorAPI.Core.Exceptions.Services;
using TestGeneratorAPI.Core.Models;

namespace TestGeneratorAPI.Application.Services;

public class UsersService : IUsersService, IBasicUserValidationService
{
    private readonly IUsersRepository _usersRepository;

    public UsersService(IUsersRepository usersRepository)
    {
        _usersRepository = usersRepository;
    }

    public async Task<Guid> CreateUser(UserCreate userCreate)
    {
        try
        {
            // if (await _usersRepository.ExistsByName(template.CreatorID, template.Name))
            // {
            //     throw new UserAlreadyExistsException("User with this name already exists");
            // }

            var id = Guid.NewGuid();
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(userCreate.Password);
            await _usersRepository.Create(id, userCreate.Login, passwordHash, userCreate.Name ?? userCreate.Login);
            return id;
        }
        catch (UserRepositoryException ex)
        {
            throw new UserServiceException("Failed template service whit creating template", ex);
        }
    }

    public async Task<UserRead> Get(string login)
    {
        var user = await _usersRepository.GetLastByLogin(login);
        return new UserRead
        {
            UserId = user.UserId,
            Login = user.Login,
            Name = user.Name,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt,
        };
    }

    public async Task<AuthorizedUserRead?> GetAuthorized(string login, string password)
    {
        try
        {
            var user = await _usersRepository.GetLastByLogin(login);
            if (BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            {
                return user;
            }

            return null;
        }
        catch (Exception)
        {
            return null;
        }
    }

    public async Task<bool> IsValidAsync(string username, string password)
    {
        try
        {
            var user = await _usersRepository.GetLastByLogin(username);
            return BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
        }
        catch (Exception)
        {
            return false;
        };
    }
}