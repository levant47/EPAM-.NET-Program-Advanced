public static class DataHelper
{
    public static async Task<List<CategoryEntity>> SetUpCategories(int count, MySqlConnection connection)
    {
        var result = new List<CategoryEntity>();
        for (var i = 0; i < count; i++)
        {
            var entity = await connection.QueryFirstAsync<CategoryEntity>(@"
                INSERT INTO Categories (Name)
                VALUES (@Name)
                RETURNING *
            ", new { Name = $"Category {Random.Shared.Next()}" });
            result.Add(entity);
        }
        return result;
    }

    public static async Task<List<ItemEntity>> SetUpItems(int categoryId, int count, MySqlConnection connection)
    {
        var result = new List<ItemEntity>();
        for (var i = 0; i < count; i++)
        {
            var entity = await connection.QueryFirstAsync<ItemEntity>(@"
                INSERT INTO Items (Name, CategoryId, Price, Amount)
                VALUES (@Name, @CategoryId, @Price, @Amount)
                RETURNING *
            ", new { Name = $"Item {Random.Shared.Next()}", CategoryId = categoryId, Price = 10, Amount = 11 });
            result.Add(entity);
        }
        return result;
    }
}
