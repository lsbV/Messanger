var builder = DistributedApplication.CreateBuilder(args);

var sql = builder.AddSqlServer("sql")
    .WithLifetime(ContainerLifetime.Persistent);

var db = sql.AddDatabase("Messenger");

builder.AddProject<Projects.Server>("server")
    .WithReference(db)
    .WaitFor(db);


await builder.Build().RunAsync();