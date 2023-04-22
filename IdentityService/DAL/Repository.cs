public class Repository : IRepository
{
    private readonly SQLiteConnection _connection;

    public Repository(SQLiteConnection connection) => _connection = connection;

    public Task<IEnumerable<UserPermissionPairDto>> Login(string username, string hashedPassword) => _connection.QueryAsync<UserPermissionPairDto>("""
        SELECT Users.Id, Permissions.Permission
        FROM Users
        JOIN Permissions ON Users.RoleId = Permissions.RoleId
        WHERE Users.Username = @Username AND Users.Password = @Password
    """, new { Username = username, Password = hashedPassword });

    public Task<int> GetRoleCount() => _connection.QueryFirstAsync<int>("""
        SELECT COUNT(Id)
        FROM Roles
    """);

    public async Task<int> SeedRole(string name, Permission[] permissions)
    {
        var roleId = await _connection.QueryFirstAsync<int>("""
            INSERT INTO Roles (Name)
            VALUES (@Name)
            RETURNING Id
        """, new { Name = name });
        await _connection.ExecuteAsync($"""
            INSERT INTO Permissions (Permission, RoleId)
            VALUES {string.Join(",\n", permissions.Select(permission => $"({(int)permission}, @RoleId)"))}
        """, new { RoleId = roleId });
        return roleId;
    }

    public Task SeedUser(string username, string hashedPassword, int roleId) => _connection.ExecuteAsync("""
        INSERT INTO Users (Username, Password, RoleId)
        VALUES (@Username, @Password, @RoleId)
    """, new { Username = username, Password = hashedPassword, RoleId = roleId });

    public Task<IEnumerable<Permission>> GetUserPermissionsById(int id) => _connection.QueryAsync<Permission>("""
        SELECT Permissions.Permission
        FROM Users
        JOIN Permissions ON Users.RoleId = Permissions.RoleId
        WHERE Users.Id = @Id
    """, new { Id = id });
}
