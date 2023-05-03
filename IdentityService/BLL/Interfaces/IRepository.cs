public interface IRepository
{
    Task<int?> GetUserIdByCredentials(string username, string hashedPassword);

    Task<int> GetRoleCount();

    Task<int> SeedRole(string name, Permission[] permissions);

    Task SeedUser(string username, string hashedPassword, int roleId);

    Task<IEnumerable<Permission>> GetUserPermissionsById(int id);
}
