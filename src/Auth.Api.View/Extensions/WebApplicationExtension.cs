using Auth.Api.ServiceDefaults;
using Auth.Api.View.Endpoints;

namespace Auth.Api.View.Extensions;

public static class WebApplicationExtension
{
    public static void ConfigureApplication(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.MapDefaultEndpoints();
        app.MapAuthEndpoints();
    }
}