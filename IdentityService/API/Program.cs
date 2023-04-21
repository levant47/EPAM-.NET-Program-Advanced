var builder = WebApplication.CreateBuilder(args);

// Add services to the container

builder.Services.AddControllers();

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IRepository, Repository>();
builder.Services.AddScoped<ISeeder, Seeder>();

var connectionString = builder.Configuration["Sqlite"];
builder.Services.AddScoped<SQLiteConnection>(_ => new(connectionString));

var app = builder.Build();

// Seed the database
using (var scope = app.Services.CreateScope())
{
    await scope.ServiceProvider.GetRequiredService<ISeeder>().Seed();
}

// Configure the request pipeline

app.MapControllers();

app.Use(next => async context =>
{
    try { await next(context); }
    catch (BadRequestException exception)
    {
        await Results.BadRequest(new ProblemDetails { Title = "Invalid Request", Detail = exception.Message }).ExecuteAsync(context);
    }
});

app.Run();
