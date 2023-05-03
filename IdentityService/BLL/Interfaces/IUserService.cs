public interface IUserService
{
    Task<LoginSuccessDto> Login(LoginDto login);

    Task<Permission[]> GetPermissionById(int id);
}
