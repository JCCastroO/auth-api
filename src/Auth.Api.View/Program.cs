using Auth.Api.ServiceDefaults;
using Auth.Api.View.Extensions;

var builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults();
builder.Services.ConfigureServices(builder.Configuration);
builder.AddRedisClient("redis");

var app = builder.Build();
app.ConfigureApplication();

await app.RunAsync();