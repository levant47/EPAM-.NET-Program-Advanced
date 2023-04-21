public interface IRepository
{
    Task<IEnumerable<Permission>> Login(string username, string hashedPassword);

    Task<int> GetRoleCount();

    Task<int> SeedRole(string name, Permission[] permissions);

    Task SeedUser(string username, string hashedPassword, int roleId);
}
