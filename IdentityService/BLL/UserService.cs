public class UserService : IUserService
{
    private readonly IRepository _repository;

    public UserService(IRepository repository) => _repository = repository;

    public async Task<LoginSuccessDto> Login(LoginDto login)
    {
        var pairs = (await _repository.Login(login.Username, PasswordHasher.HashPassword(login.Password))).ToArray();
        if (pairs.Length == 0) { throw new BadRequestException("Invalid credentials"); }
        return new() { Id = pairs[0].Id, Permissions = pairs.Select(pair => pair.Permission).ToArray() };
    }

    public async Task<Permission[]> GetPermissionById(int id)
    {
        var permissions = (await _repository.GetUserPermissionsById(id)).ToArray();
        if (permissions.Length == 0) { throw new NotFoundException($"User with ID {id} was not found"); }
        return permissions;
    }
}
