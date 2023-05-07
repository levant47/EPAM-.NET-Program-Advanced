using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container

builder.Services.AddOcelot();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerForOcelot(builder.Configuration);

builder.Services.AddJaegerTracing("Gateway");

var app = builder.Build();

// Configure the request pipeline

app.UseSwaggerForOcelotUI();

await app.UseOcelot();

app.Run();
