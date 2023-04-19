public class MessageRepository : BaseRepository<MessageEntity>, IMessageRepository
{
    public MessageRepository(MySqlConnection connection) : base(connection, "Messages") { }

    public Task<int> Create(string name, string json) => Create(new { Name = name, Contents = json });
}
