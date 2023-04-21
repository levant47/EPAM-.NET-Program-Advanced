public interface IUserService
{
    Task<Permission[]> Login(LoginDto login);
}
