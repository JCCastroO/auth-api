using Auth.Api.View.Extensions;

var builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults();
builder.Services.ConfigureServices(builder.Configuration);

var app = builder.Build();
app.ConfigureApplication();

await app.RunAsync();