public class CategoriesTests : TestsBase
{
    private CategoryService _service;

    [SetUp]
    public void SetUp() => _service = new(new CategoryRepository(_connection), new FakePermissionVerifier());

    [TearDown]
    public ValueTask TearDown() => _connection.DisposeAsync();

    [Test]
    public async Task GetAllWorks()
    {
        var setUpCategories = await SetUpCategories(3);

        var categories = (await _service.GetAll()).ToList();

        Assert.True(setUpCategories.All(setUpCategory => categories.Any(category => category.Id == setUpCategory.Id)));
    }

    [Test]
    public async Task GetByIdWorks()
    {
        var setUpCategory = await SetUpCategory();

        var retrievedCategory = await _service.GetById(setUpCategory.Id);

        Assert.NotNull(retrievedCategory);
        Assert.AreEqual(setUpCategory.Id, retrievedCategory!.Id);
        Assert.AreEqual(setUpCategory.Name, retrievedCategory.Name);
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

        var newCategory = await _service.Create(setUpCategory);

        var retrievedName = await _connection.QueryFirstOrDefaultAsync<string>(@"
            SELECT Name
            FROM Categories
            WHERE Id = @Id
        ", new { newCategory.Id });
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
        var setUpCategory = await SetUpCategory();
        var setUpUpdate = new CategoryUpdateDto { Name = "Test name (updated)" };

        await _service.Update(setUpCategory.Id, setUpUpdate);

        var retrievedName = await _connection.QueryFirstOrDefaultAsync<string>(@"
            SELECT Name
            FROM Categories
            WHERE Id = @Id
        ", new { setUpCategory.Id });
        Assert.AreEqual(setUpUpdate.Name, retrievedName);
    }

    [Test]
    public void UpdateHandlesInvalidId() => Assert.ThrowsAsync<BadRequestException>(() => _service.Update(-1, new()));

    [Test]
    public async Task UpdateValidationWorks()
    {
        var setUpCategory = await SetUpCategory();

        Assert.ThrowsAsync<BadRequestException>(() => _service.Update(setUpCategory.Id, new() { Name = "" }));
        Assert.ThrowsAsync<BadRequestException>(() => _service.Update(setUpCategory.Id, new() { Name = new('A', CategoryEntity.NameMaxLength + 1) }));
        Assert.ThrowsAsync<BadRequestException>(() => _service.Update(setUpCategory.Id, new() { Name = "Name", ParentCategoryId = -1 }));
    }

    [Test]
    public async Task DeleteWorks()
    {
        var setUpCategory = await SetUpCategory();

        await _service.Delete(setUpCategory.Id);

        var exists = await _connection.QueryFirstAsync<bool>(@"
            SELECT EXISTS (
                SELECT 1
                FROM Categories
                WHERE Id = @Id
            )
        ", new { setUpCategory.Id });
        Assert.False(exists);
    }

    [Test]
    public void DeleteHandlesInvalidId() => Assert.ThrowsAsync<BadRequestException>(() => _service.Delete(-1));

    private async Task<CategoryEntity> SetUpCategory() => (await SetUpCategories(1)).First();

    private Task<List<CategoryEntity>> SetUpCategories(int count) => DataHelper.SetUpCategories(count, _connection);
}
