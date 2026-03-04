var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres")
    .WithImage("postgres:alpine3.23")
    .WithBindMount("./Script/init.sql", "/docker-entrypoint-initdb.d/init.sql")
    .WithPgAdmin();

var redis = builder.AddRedis("redis")
    .WithImage("redis:8.6.1-alpine")
    .WithRedisInsight();

builder.AddProject<Projects.Auth_Api_View>("authentication-api")
    .WithReference(postgres)
    .WaitFor(postgres)
    .WithReference(redis)
    .WaitFor(redis)
    .WithEnvironment("ConnectionStrings__Database", postgres.Resource.ConnectionStringExpression);

builder.Build().Run();