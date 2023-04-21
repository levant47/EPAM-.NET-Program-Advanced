public class ItemEntity : ItemSharedBase
{
    public int Id { get; set; }

    public string? Description { get; set; }

    public int CategoryId { get; set; }

    public const int NameMaxLength = 50;
}
