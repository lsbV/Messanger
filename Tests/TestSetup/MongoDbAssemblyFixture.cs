using Microsoft.Extensions.Configuration;

[assembly: AssemblyFixture(typeof(MongoDbAssemblyFixture))]
namespace Tests.TestSetup;

public class MongoDbAssemblyFixture : IDisposable
{
    public readonly MongoClient MongoClient;
    public readonly IMongoDatabase Database;
    public readonly IMongoCollection<Message> MessageCollection;
    private readonly IConfiguration _configuration;
    private bool _disposed;

    public MongoDbAssemblyFixture()
    {
        _configuration = new ConfigurationBuilder()
            .AddJsonFile("testsettings.json")
            .Build();
        MongoClient = new MongoClient(_configuration["MongoDb:ConnectionString"]);
        MongoClient.ConfigureMessageEntity();
        Database = MongoClient.GetDatabase(_configuration["MongoDb:DatabaseName"]);
        MessageCollection = Database.GetCollection<Message>(_configuration["MongoDb:MessageCollectionName"]);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed) return;
        if (disposing)
        {
            MongoClient.DropDatabase(_configuration["MongoDb:DatabaseName"]);
        }
        _disposed = true;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}
