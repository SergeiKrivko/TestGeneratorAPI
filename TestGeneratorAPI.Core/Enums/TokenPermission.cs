namespace TestGeneratorAPI.Core.Enums;

public class TokenPermission
{
    public required string Key { get; init; }
    public required string Description { get; init; }
    public bool AdminOnly { get; init; }

    public static TokenPermission UpdateUser { get; } = new TokenPermission
        { Key = "updateUser", Description = "Изменение данных пользователя" };

    public static TokenPermission CreatePlugin { get; } = new TokenPermission
        { Key = "createPlugin", Description = "Создание новых плагинов" };

    public static TokenPermission RemovePlugin { get; } = new TokenPermission
        { Key = "removePlugin", Description = "Удаление существующих плагинов" };

    public static TokenPermission CreateRelease { get; } = new TokenPermission
        { Key = "createRelease", Description = "Публикация релизов существующих плагинов" };

    public static TokenPermission RemoveRelease { get; } = new TokenPermission
        { Key = "removeRelease", Description = "Удаление существующих релизов" };

    public static TokenPermission CreateTestGeneratorRelease { get; } = new TokenPermission
        { Key = "createTestGeneratorRelease", Description = "Публикация релизов TestGenerator", AdminOnly = true };
}