public class CategoryRepository : BaseRepository<CategoryEntity>, ICategoryRepository
{
    public CategoryRepository(MySqlConnection connection) : base(connection, "Categories") { }

    public Task<int> Create(CategoryCreateDto newCategory) => base.Create(newCategory);

    public Task Update(int id, CategoryUpdateDto update) => base.Update(id, update);
}
