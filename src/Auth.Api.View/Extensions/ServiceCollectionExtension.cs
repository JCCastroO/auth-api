using Microsoft.Data.SqlClient;
using System.Data;

namespace Auth.Api.View.Extensions;

public static class ServiceCollectionExtension
{
    public static void ConfigureServices(this IServiceCollection service, IConfiguration configuration)
    {
        service.AddSwaggerGen();

        service.ConfigureAppDependencies(configuration);
    }

    private static void ConfigureAppDependencies(this IServiceCollection service, IConfiguration configuration)
    {
        service.AddScoped<IDbConnection>(sp =>
        {
            var connectionString = configuration.GetConnectionString("Database");
            return new SqlConnection(connectionString);
        });
    }
}