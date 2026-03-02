var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres")
    .WithPgAdmin();

builder.AddProject<Projects.Auth_Api_View>("authentication-api")
    .WaitFor(postgres)
    .WithEnvironment("ConnectionStrings__Database", postgres.Resource.ConnectionStringExpression);

builder.Build().Run();