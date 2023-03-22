public class ItemsTests
{
    private MySqlConnection _connection;
    private ItemService _service;

    private int _setUpCategoryId;

    [OneTimeSetUp]
    public async Task OneTimeSetUp()
    {
        await using var connection = new MySqlConnection("server=localhost;uid=root;database=test");
        _setUpCategoryId = await connection.QueryFirstAsync<int>(@"
            INSERT INTO Categories (Name)
            VALUES ('Set-up category for item tests')
            RETURNING Id
        ");
    }

    [SetUp]
    public void SetUp()
    {
        _connection = new("server=localhost;uid=root;database=test");
        _service = new(new ItemRepository(_connection), new CategoryRepository(_connection));
    }

    [Test]
    public async Task GetAllWorks()
    {
        var setUpNames = new[] { "Item 1", "Item 2", "Item 3" };
        await _connection.ExecuteAsync(@"
            INSERT INTO Items (Name, CategoryId, Price, Amount)
            VALUES
                (@Name1, @CategoryId, @Price, @Amount),
                (@Name2, @CategoryId, @Price, @Amount),
                (@Name3, @CategoryId, @Price, @Amount)
        ", new { Name1 = setUpNames[0], Name2 = setUpNames[1], Name3 = setUpNames[2], CategoryId = _setUpCategoryId, Price = 10, Amount = 11 });

        var items = (await _service.GetAll()).ToList();

        Assert.True(setUpNames.All(name => items.Any(category => category.Name == name)));
    }

    [Test]
    public async Task GetByIdWorks()
    {
        var setUpName = "My item that I need to get by ID";
        var setUpId = await _connection.QueryFirstAsync<int>(@"
            INSERT INTO Items (Name, CategoryId, Price, Amount)
            VALUES (@Name, @CategoryId, @Price, @Amount)
            RETURNING Id
        ", new { Name = setUpName, CategoryId = _setUpCategoryId, Price = 10, Amount = 11 });

        var retrievedItem = await _service.GetById(setUpId);

        Assert.NotNull(retrievedItem);
        Assert.AreEqual(setUpName, retrievedItem!.Name);
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

        var newItemId = await _service.Create(setUpItem);

        var retrievedName = await _connection.QueryFirstOrDefaultAsync<string>(@"
            SELECT Name
            FROM Items
            WHERE Id = @Id
        ", new { Id = newItemId });
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
        var setUpId = await _connection.QueryFirstAsync<int>(@$"
            INSERT INTO Items (Name, CategoryId, Price, Amount)
            VALUES ('Test name', {_setUpCategoryId}, 10, 11)
            RETURNING Id
        ");
        var setUpUpdate = new ItemUpdateDto { Name = "Test name (updated)", CategoryId = _setUpCategoryId, Price = 10, Amount = 11 };

        await _service.Update(setUpId, setUpUpdate);

        var retrievedName = await _connection.QueryFirstOrDefaultAsync<string>(@"
            SELECT Name
            FROM Items
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
        Assert.ThrowsAsync<BadRequestException>(() => _service.Update(setUpId, new() { Name = new('A', ItemEntity.NameMaxLength + 1) }));
        Assert.ThrowsAsync<BadRequestException>(() => _service.Update(setUpId, new() { Name = "Name", CategoryId = -1 }));
        Assert.ThrowsAsync<BadRequestException>(() => _service.Update(setUpId, new() { Name = "Name", CategoryId = _setUpCategoryId, Price = 0 }));
        Assert.ThrowsAsync<BadRequestException>(() => _service.Update(setUpId, new() { Name = "Name", CategoryId = _setUpCategoryId, Price = 10, Amount = 0 }));
    }

    [Test]
    public async Task DeleteWorks()
    {
        var setUpId = await _connection.QueryFirstAsync<int>(@$"
            INSERT INTO Items (Name, CategoryId, Price, Amount)
            VALUES ('Test name', {_setUpCategoryId}, 10, 11)
            RETURNING Id
        ");

        await _service.Delete(setUpId);

        var exists = await _connection.QueryFirstAsync<bool>(@"
            SELECT EXISTS (
                SELECT 1
                FROM Items
                WHERE Id = @Id
            )
        ", new { Id = setUpId });
        Assert.False(exists);
    }

    [Test]
    public void DeleteHandlesInvalidId() => Assert.ThrowsAsync<BadRequestException>(() => _service.Delete(-1));
}
