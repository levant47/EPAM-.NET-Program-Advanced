public class CategoriesTests
{
    private MySqlConnection _connection;
    private CategoryService _service;

    [SetUp]
    public void SetUp()
    {
        _connection = new("server=localhost;uid=root;database=test");
        _service = new(new CategoryRepository(_connection));
    }

    [TearDown]
    public ValueTask TearDown() => _connection.DisposeAsync();

    [Test]
    public async Task GetAllWorks()
    {
        var setUpNames = new[] { "Category 1", "Category 2", "Category 3" };
        await _connection.ExecuteAsync(@"
            INSERT INTO Categories (Name)
            VALUES (@Name1), (@Name2), (@Name3)
        ", new { Name1 = setUpNames[0], Name2 = setUpNames[1], Name3 = setUpNames[2] });

        var categories = (await _service.GetAll()).ToList();

        Assert.True(setUpNames.All(name => categories.Any(category => category.Name == name)));
    }

    [Test]
    public async Task GetByIdWorks()
    {
        var setUpName = "My category that I need to get by ID";
        var setUpId = await _connection.QueryFirstAsync<int>(@"
            INSERT INTO Categories (Name)
            VALUES (@Name)
            RETURNING Id
        ", new { Name = setUpName });

        var retrievedCategory = await _service.GetById(setUpId);

        Assert.NotNull(retrievedCategory);
        Assert.AreEqual(setUpName, retrievedCategory!.Name);
    }

    [Test]
    public async Task GetByIdHandlesInvalidId()
    {
        var retrievedCategory = await _service.GetById(-1);

        Assert.Null(retrievedCategory);
    }

    [Test]
    public async Task CreateWorks()
    {
        var setUpCategory = new CategoryCreateDto { Name = "My new category" };

        var newCategoryId = await _service.Create(setUpCategory);

        var retrievedName = await _connection.QueryFirstOrDefaultAsync<string>(@"
            SELECT Name
            FROM Categories
            WHERE Id = @Id
        ", new { Id = newCategoryId });
        Assert.AreEqual(setUpCategory.Name, retrievedName);
    }

    [Test]
    public void CreateValidationWorks()
    {
        Assert.ThrowsAsync<BadRequestException>(() => _service.Create(new() { Name = "" }));
        Assert.ThrowsAsync<BadRequestException>(() => _service.Create(new() { Name = new('A', CategoryEntity.NameMaxLength + 1) }));
        Assert.ThrowsAsync<BadRequestException>(() => _service.Create(new() { Name = "Name", ParentCategoryId = -1 }));
    }

    [Test]
    public async Task UpdateWorks()
    {
        var setUpId = await _connection.QueryFirstAsync<int>(@"
            INSERT INTO Categories (Name)
            VALUES ('Test name')
            RETURNING Id
        ");
        var setUpUpdate = new CategoryUpdateDto { Name = "Test name (updated)" };

        await _service.Update(setUpId, setUpUpdate);

        var retrievedName = await _connection.QueryFirstOrDefaultAsync<string>(@"
            SELECT Name
            FROM Categories
            WHERE Id = @Id
        ", new { Id = setUpId });
        Assert.AreEqual(setUpUpdate.Name, retrievedName);
    }

    [Test]
    public void UpdateHandlesInvalidId() => Assert.ThrowsAsync<BadRequestException>(() => _service.Update(-1, new()));

    [Test]
    public async Task UpdateValidationWorks()
    {
        var setUpId = await _connection.QueryFirstAsync<int>(@"
            INSERT INTO Categories (Name)
            VALUES ('Test name')
            RETURNING Id
        ");

        Assert.ThrowsAsync<BadRequestException>(() => _service.Update(setUpId, new() { Name = "" }));
        Assert.ThrowsAsync<BadRequestException>(() => _service.Update(setUpId, new() { Name = new('A', CategoryEntity.NameMaxLength + 1) }));
        Assert.ThrowsAsync<BadRequestException>(() => _service.Update(setUpId, new() { Name = "Name", ParentCategoryId = -1 }));
    }

    [Test]
    public async Task DeleteWorks()
    {
        var setUpId = await _connection.QueryFirstAsync<int>(@"
            INSERT INTO Categories (Name)
            VALUES ('Test name')
            RETURNING Id
        ");

        await _service.Delete(setUpId);

        var exists = await _connection.QueryFirstAsync<bool>(@"
            SELECT EXISTS (
                SELECT 1
                FROM Categories
                WHERE Id = @Id
            )
        ", new { Id = setUpId });
        Assert.False(exists);
    }

    [Test]
    public void DeleteHandlesInvalidId() => Assert.ThrowsAsync<BadRequestException>(() => _service.Delete(-1));
}
