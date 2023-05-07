var builder = WebApplication.CreateBuilder(args);

// Add services

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition(
        "Bearer",
        new() { In = ParameterLocation.Header, Name = "Authorization", Type = SecuritySchemeType.ApiKey, Scheme = "Bearer" }
    );
    options.AddSecurityRequirement(new()
    {
        {
            new()
            {
                Reference = new() { Type = ReferenceType.SecurityScheme, Id = "Bearer" },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddJaegerTracing("Catalog Service");

builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IItemService, ItemService>();

builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IItemRepository, ItemRepository>();
builder.Services.AddScoped<IMessageRepository, MessageRepository>();

builder.Services.AddScoped<IMessagingService, MessagingService>();
builder.Services.AddSingleton<MessagingService>();

builder.Services.AddHostedService<MessagingHostedService>();

var databaseConnectionString = builder.Configuration["Database"];
builder.Services.AddScoped(_ => new MySqlConnection(databaseConnectionString));
builder.Services.AddTransient<IUnitOfWork, UnitOfWork>();

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
    catch (NotFoundException exception)
    {
        await Results.NotFound(new ProblemDetails { Title = "Not Found", Detail = exception.Message }).ExecuteAsync(context);
    }
});

app.MapControllers();

app.Run();
