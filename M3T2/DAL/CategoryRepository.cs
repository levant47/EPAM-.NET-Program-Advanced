public class CategoryRepository : ICategoryRepository
{
    private readonly MySqlConnection _connection;

    public CategoryRepository(MySqlConnection connection) => _connection = connection;

    public Task<CategoryEntity?> GetById(int id) => _connection.QueryFirstOrDefaultAsync<CategoryEntity?>(@"
        SELECT *
        FROM Categories
        WHERE Id = @Id
    ", new { Id = id });

    public Task<IEnumerable<CategoryEntity>> GetAll() => _connection.QueryAsync<CategoryEntity>(@"
        SELECT *
        FROM Categories
    ");

    public Task<int> Create(CategoryCreateDto newCategory) => _connection.QueryFirstAsync<int>(@"
        INSERT INTO Categories (Name, ImageUrl, ParentCategoryId)
        VALUES (@Name, @ImageUrl, @ParentCategoryId)
        RETURNING Id
    ", newCategory);

    public Task<bool> Exists(int id) => _connection.QueryFirstAsync<bool>(@"
        SELECT EXISTS (
            SELECT 1
            FROM Categories
            WHERE Id = @Id
        )
    ", new { Id = id });

    public Task Update(int id, CategoryUpdateDto update) => _connection.ExecuteAsync(@$"
        UPDATE Categories
        SET
            Name = @Name,
            ImageUrl = @ImageUrl,
            ParentCategoryId = @ParentCategoryId
        WHERE Id = {id}
    ", update);

    public Task Delete(int id) => _connection.ExecuteAsync(@"
        DELETE FROM Categories
        WHERE Id = @Id
    ", new { Id = id });
}
