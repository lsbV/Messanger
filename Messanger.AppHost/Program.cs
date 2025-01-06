var builder = DistributedApplication.CreateBuilder(args);

var sql = builder.AddSqlServer("sql")
    .WithLifetime(ContainerLifetime.Persistent);

var mongo = builder.AddMongoDB("mongo")
    .WithDataVolume()
    .WithMongoExpress()
    .WithLifetime(ContainerLifetime.Persistent);

var blobsStorage = builder.AddAzureStorage("storage")
        .RunAsEmulator()
        .AddBlobs("blobs");

var sqlDb = sql.AddDatabase("MainDb");
var mongoDb = mongo.AddDatabase("MessagesDb");


builder.AddProject<Projects.Server>("server")
    .WithReference(sqlDb)
    .WaitFor(sqlDb)
    .WithReference(mongoDb)
    .WaitFor(mongoDb)
    .WithReference(blobsStorage)
    .WaitFor(blobsStorage);


await builder.Build().RunAsync();