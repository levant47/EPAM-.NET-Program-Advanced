var builder = WebApplication.CreateBuilder(args);

// Add services to the container

builder.Services.AddControllers();

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

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters.ValidateIssuerSigningKey = true;
        options.TokenValidationParameters.IssuerSigningKey = Jwt.Key;
        options.TokenValidationParameters.ValidateAudience = false;
        options.TokenValidationParameters.ValidateIssuer = false;
        // the default validator checks the expiry date according to local time, and we want to use UTC
        options.TokenValidationParameters.LifetimeValidator = (_before, expires, _token, _parameters) => DateTime.UtcNow < expires;
    });

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IRepository, Repository>();
builder.Services.AddScoped<ISeeder, Seeder>();

var connectionString = builder.Configuration["Sqlite"];
builder.Services.AddScoped<SQLiteConnection>(_ => new(connectionString));

builder.Services.AddSingleton(new JwtConfiguration
{
    IdentityTokenExpiresInSeconds = int.Parse(builder.Configuration["IdentityTokenExpiresInSeconds"]!),
    RefreshTokenExpiresInSeconds = int.Parse(builder.Configuration["RefreshTokenExpiresInSeconds"]!),
});

var app = builder.Build();

// Seed the database
using (var scope = app.Services.CreateScope())
{
    await scope.ServiceProvider.GetRequiredService<ISeeder>().Seed();
}

// Configure the request pipeline

app.MapControllers();

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

app.Run();
