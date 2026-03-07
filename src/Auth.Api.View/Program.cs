using Auth.Api.ServiceDefaults;
using Auth.Api.View.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

builder.Services.ConfigureServices(builder.Configuration);

builder.AddServiceDefaults();
builder.AddRedisClient("redis");

var app = builder.Build();
app.ConfigureApplication();

await app.RunAsync();