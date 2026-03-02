var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres")
    .WithImage("postgres:alpine3.23")
    .WithBindMount("./Script/init.sql", "/docker-entrypoint-initdb.d/init.sql")
    .WithPgAdmin();

builder.AddProject<Projects.Auth_Api_View>("authentication-api")
    .WaitFor(postgres)
    .WithEnvironment("ConnectionStrings__Database", postgres.Resource.ConnectionStringExpression);

builder.Build().Run();