public interface IMessageRepository
{
    Task<IEnumerable<MessageEntity>> GetAll();

    Task<int> Create(string name, string json);

    Task Delete(int id);
}
