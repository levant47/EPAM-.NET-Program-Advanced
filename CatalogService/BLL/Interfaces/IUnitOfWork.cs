public interface IUnitOfWork
{
    Task Start();

    void Commit();
}
