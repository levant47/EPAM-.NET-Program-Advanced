public class CategoryEntity
{
    public int Id { get; set; }

    public string Name { get; set; }

    public string? ImageUrl { get; set; }

    public int? ParentCategoryId { get; set; }

    public const int NameMaxLength = 50;
}
