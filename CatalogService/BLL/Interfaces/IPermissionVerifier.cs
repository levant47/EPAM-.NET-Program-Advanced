public interface IPermissionVerifier
{
    Task Verify(Permission permission);
}
