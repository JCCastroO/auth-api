var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.Auth_Api_View>("auth-api-view");

builder.Build().Run();
