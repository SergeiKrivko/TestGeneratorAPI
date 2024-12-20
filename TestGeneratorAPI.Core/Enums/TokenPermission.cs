namespace TestGeneratorAPI.Core.Enums;

public class TokenPermission
{
    public required string Key { get; init; }
    public required string Description { get; init; }
    public TokenType[] TokenTypes { get; init; }

    public static TokenPermission UpdateUser { get; } = new TokenPermission
    {
        Key = "updateUser", Description = "Изменение данных пользователя",
        TokenTypes = [TokenType.User, TokenType.Admin]
    };

    public static TokenPermission CreatePlugin { get; } = new TokenPermission
    {
        Key = "createPlugin", Description = "Создание новых плагинов",
        TokenTypes = [TokenType.User, TokenType.Admin, TokenType.Mask]
    };

    public static TokenPermission RemovePlugin { get; } = new TokenPermission
    {
        Key = "removePlugin", Description = "Удаление существующих плагинов",
        TokenTypes = [TokenType.User, TokenType.Admin, TokenType.Mask, TokenType.Plugins]
    };

    public static TokenPermission CreateRelease { get; } = new TokenPermission
    {
        Key = "createRelease", Description = "Публикация релизов существующих плагинов",
        TokenTypes = [TokenType.User, TokenType.Admin, TokenType.Mask, TokenType.Plugins]
    };

    public static TokenPermission RemoveRelease { get; } = new TokenPermission
    {
        Key = "removeRelease", Description = "Удаление существующих релизов",
        TokenTypes = [TokenType.User, TokenType.Admin, TokenType.Mask, TokenType.Plugins]
    };

    public static TokenPermission CreateTestGeneratorRelease { get; } = new TokenPermission
    {
        Key = "createTestGeneratorRelease", Description = "Публикация релизов TestGenerator",
        TokenTypes = [TokenType.Admin]
    };

    public static TokenPermission CreateUser { get; } = new TokenPermission
    {
        Key = "createUser", Description = "Создание новых пользователей",
        TokenTypes = [TokenType.Admin]
    };
}