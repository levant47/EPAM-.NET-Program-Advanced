public class ItemsTests : TestsBase
{
    private ItemService _service;

    private int _setUpCategoryId;

    [OneTimeSetUp]
    public async Task OneTimeSetUp()
    {
        await using var connection = new MySqlConnection("server=localhost;uid=root;database=test");
        _setUpCategoryId = (await DataHelper.SetUpCategories(1, connection)).First().Id;
    }

    private class FakeMessagingService : IMessagingService { public async Task Save(BaseMessage message) { } }
    private class FakeUnitOfWork : IUnitOfWork { public Task Start() => Task.CompletedTask; public void Commit() { } }

    [SetUp]
    public void SetUp() => _service = new(
        new ItemRepository(_connection),
        new CategoryRepository(_connection),
        new FakeMessagingService(),
        new FakeUnitOfWork()
    );

    [Test]
    public async Task GetAllWorks()
    {
        var setUpItems = await SetUpItems(3);

        var items = (await _service.GetAll()).ToList();

        Assert.True(setUpItems.All(setUpItem => items.Any(item => item.Name == setUpItem.Name)));
    }

    [Test]
    public async Task GetByIdWorks()
    {
        var setUpItem = await SetUpItem();

        var retrievedItem = await _service.GetById(setUpItem.Id);

        Assert.NotNull(retrievedItem);
        Assert.AreEqual(setUpItem.Name, retrievedItem!.Name);
    }

    [Test]
    public async Task GetByIdHandlesInvalidId()
    {
        var retrievedItem = await _service.GetById(-1);

        Assert.Null(retrievedItem);
    }

    [Test]
    public async Task CreateWorks()
    {
        var setUpItem = new ItemCreateDto { Name = "My new category", CategoryId = _setUpCategoryId, Price = 10, Amount = 11 };

        var newItem = await _service.Create(setUpItem);

        var retrievedName = await _connection.QueryFirstOrDefaultAsync<string>(@"
            SELECT Name
            FROM Items
            WHERE Id = @Id
        ", new { newItem.Id });
        Assert.AreEqual(setUpItem.Name, retrievedName);
    }

    [Test]
    public void CreateValidationWorks()
    {
        Assert.ThrowsAsync<BadRequestException>(() => _service.Create(new() { Name = "" }));
        Assert.ThrowsAsync<BadRequestException>(() => _service.Create(new() { Name = new('A', ItemEntity.NameMaxLength + 1) }));
        Assert.ThrowsAsync<BadRequestException>(() => _service.Create(new() { Name = "Name", CategoryId = -1 }));
        Assert.ThrowsAsync<BadRequestException>(() => _service.Create(new() { Name = "Name", CategoryId = _setUpCategoryId, Price = 0 }));
        Assert.ThrowsAsync<BadRequestException>(() => _service.Create(new() { Name = "Name", CategoryId = _setUpCategoryId, Price = 10, Amount = 0 }));
    }

    [Test]
    public async Task UpdateWorks()
    {
        var setUpItem = await SetUpItem();
        var setUpUpdate = new ItemUpdateDto { Name = "Test name (updated)", CategoryId = _setUpCategoryId, Price = 10, Amount = 11 };

        await _service.Update(setUpItem.Id, setUpUpdate);

        var retrievedName = await _connection.QueryFirstOrDefaultAsync<string>(@"
            SELECT Name
            FROM Items
            WHERE Id = @Id
        ", new { setUpItem.Id });
        Assert.AreEqual(setUpUpdate.Name, retrievedName);
    }

    [Test]
    public void UpdateHandlesInvalidId() => Assert.ThrowsAsync<BadRequestException>(() => _service.Update(-1, new()));

    [Test]
    public async Task UpdateValidationWorks()
    {
        var setUpItem = await SetUpItem();

        Assert.ThrowsAsync<BadRequestException>(() => _service.Update(setUpItem.Id, new() { Name = "" }));
        Assert.ThrowsAsync<BadRequestException>(() => _service.Update(setUpItem.Id, new() { Name = new('A', ItemEntity.NameMaxLength + 1) }));
        Assert.ThrowsAsync<BadRequestException>(() => _service.Update(setUpItem.Id, new() { Name = "Name", CategoryId = -1 }));
        Assert.ThrowsAsync<BadRequestException>(() => _service.Update(setUpItem.Id, new() { Name = "Name", CategoryId = _setUpCategoryId, Price = 0 }));
        Assert.ThrowsAsync<BadRequestException>(() => _service.Update(setUpItem.Id, new() { Name = "Name", CategoryId = _setUpCategoryId, Price = 10, Amount = 0 }));
    }

    [Test]
    public async Task DeleteWorks()
    {
        var setUpItem = await SetUpItem();

        await _service.Delete(setUpItem.Id);

        var exists = await _connection.QueryFirstAsync<bool>(@"
            SELECT EXISTS (
                SELECT 1
                FROM Items
                WHERE Id = @Id
            )
        ", new { setUpItem.Id });
        Assert.False(exists);
    }

    [Test]
    public void DeleteHandlesInvalidId() => Assert.ThrowsAsync<BadRequestException>(() => _service.Delete(-1));

    private async Task<ItemEntity> SetUpItem() => (await SetUpItems(1)).First();

    private Task<List<ItemEntity>> SetUpItems(int count) => DataHelper.SetUpItems(_setUpCategoryId, count, _connection);
}
