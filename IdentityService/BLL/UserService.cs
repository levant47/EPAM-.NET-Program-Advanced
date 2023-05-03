public class UserService : IUserService
{
    private readonly IRepository _repository;

    public UserService(IRepository repository) => _repository = repository;

    public async Task<LoginSuccessDto> Login(LoginDto login)
    {
        var id = await _repository.GetUserIdByCredentials(login.Username, PasswordHasher.HashPassword(login.Password))
            ?? throw new BadRequestException("Invalid credentials");
        return new() { Id = id, Permissions = (await _repository.GetUserPermissionsById(id)).ToArray() };
    }

    public async Task<Permission[]> GetPermissionById(int id)
    {
        var permissions = (await _repository.GetUserPermissionsById(id)).ToArray();
        if (permissions.Length == 0) { throw new NotFoundException($"User with ID {id} was not found"); }
        return permissions;
    }
}
