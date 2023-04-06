var builder = WebApplication.CreateBuilder(args);

// Add services

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IItemService, ItemService>();

builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IItemRepository, ItemRepository>();

var databaseConnectionStringFormat = builder.Configuration["DatabaseConnectionStringFormat"]!;
var databaseName = builder.Configuration["DatabaseName"]!.Replace("'", "").Replace("\"", "").Replace("\\", ""); // just in case
builder.Services.AddScoped(_ => new MySqlConnection(string.Format(databaseConnectionStringFormat, databaseName)));

var app = builder.Build();

// create database if it doesn't exist
{
    await using var sysConnection = new MySqlConnection(string.Format(databaseConnectionStringFormat, "sys"));
    var targetDbExists = await sysConnection.QueryFirstOrDefaultAsync<bool>(@$"
        SELECT 1
        FROM information_schema.SCHEMATA
        WHERE SCHEMA_NAME = '{databaseName}'
    ");
    if (!targetDbExists)
    {
        await sysConnection.ExecuteAsync($"CREATE DATABASE {databaseName}; USE {databaseName}");

        using var scope = app.Services.CreateScope();
        await using var targetDbConnection = scope.ServiceProvider.GetRequiredService<MySqlConnection>();
        await Schema.Create(targetDbConnection);
    }
}

// Configure the request pipeline

app.UseSwagger();
app.UseSwaggerUI();

app.Use(next => async context =>
{
    try { await next(context); }
    catch (BadRequestException exception)
    {
        await Results.BadRequest(new ProblemDetails { Title = "Invalid Request", Detail = exception.Message }).ExecuteAsync(context);
    }
});

app.MapControllers();

app.Run();
