public class CategoryRepository : BaseRepository<CategoryEntity>, ICategoryRepository
{
    public CategoryRepository(MySqlConnection connection) : base(connection, "Categories") { }

    public Task<int> Create(CategoryCreateDto newCategory) => base.Create(newCategory);

    public Task Update(int id, CategoryUpdateDto update) => base.Update(id, update);

    public Task<IEnumerable<CategoryEntity>> GetByIds(IEnumerable<int> ids) => _connection.QueryAsync<CategoryEntity>($"""
        SELECT *
        FROM Categories
        WHERE Id IN ({string.Join(", ", ids)})
    """);
}
