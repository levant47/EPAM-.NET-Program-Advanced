var builder = WebApplication.CreateBuilder(args);

// Add services

builder.Services.AddControllers(options => options.Filters.Add<VersioningActionFilter>());

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new() { Title = "v1", Version = "v1" });
    options.SwaggerDoc("v2", new() { Title = "v2", Version = "v2" });
    options.DocInclusionPredicate((docName, apiDescription) =>
    {
        var requirements = ApiVersionAttribute.GetFrom(apiDescription.ActionDescriptor);
        var docVersion = int.Parse(docName[1..]);
        return requirements == null || requirements.Match(docVersion);
    });
    options.OperationFilter<RemoveApiVersionRouteParameterOperationFilter>();
    options.DocumentFilter<InsertVersionNumberDocumentFilter>();
});

builder.Services.AddSingleton<ICartService, CartService>();
builder.Services.AddSingleton<IItemService, ItemService>();

builder.Services.AddSingleton<IItemRepository, ItemRepository>();

builder.Services.AddSingleton(new MongoClient(builder.Configuration["DatabaseConnection"]).GetDatabase(builder.Configuration["DatabaseName"]));

var app = builder.Build();

// Configure the request pipeline

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
    options.SwaggerEndpoint("/swagger/v2/swagger.json", "v2");
});

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
