public class MessageEntity
{
    public int Id { get; set; }

    public string Name { get; set; }

    /// <summary>JSON serialized message object</summary>
    public string Contents { get; set; }
}
