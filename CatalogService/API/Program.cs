var builder = WebApplication.CreateBuilder(args);

// Add services

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IItemService, ItemService>();

builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IItemRepository, ItemRepository>();

var kafkaServer = builder.Configuration["Kafka"]!;
builder.Services.AddScoped<IMessagingService, MessagingService>(_ => new(kafkaServer));

var databaseConnectionString = builder.Configuration["Database"];
builder.Services.AddScoped(_ => new MySqlConnection(databaseConnectionString));

var app = builder.Build();

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
