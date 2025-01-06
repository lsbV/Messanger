[assembly: AssemblyFixture(typeof(MongoFixture))]
namespace Tests.TestSetup;

public class MongoFixture : IDisposable
{
    public readonly MongoClient MongoClient;
    public readonly IMongoDatabase Database;
    public readonly IMongoCollection<Message> MessageCollection;
    private bool _disposed;

    public MongoFixture()
    {
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("testsettings.json")
            .Build();
        MongoClient = new MongoClient(configuration["MongoDb:ConnectionString"]);
        MongoClient.ConfigureMessageEntity();
        Database = MongoClient.GetDatabase(configuration["MongoDb:DatabaseName"]);
        MessageCollection = Database.GetCollection<Message>(configuration["MongoDb:MessageCollectionName"]);
        MongoClient.DropDatabase(configuration["MongoDb:DatabaseName"]);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed) return;
        if (disposing)
        {
            MongoClient.Dispose();
        }
        _disposed = true;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}
