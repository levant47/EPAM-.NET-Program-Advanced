public class UserService : IUserService
{
    private readonly IRepository _repository;

    public UserService(IRepository repository) => _repository = repository;

    public async Task<Permission[]> Login(LoginDto login)
    {
        var permissions = (await _repository.Login(login.Username, PasswordHasher.HashPassword(login.Password))).ToArray();
        if (permissions.Length == 0) { throw new BadRequestException("Invalid credentials"); }
        return permissions;
    }
}
