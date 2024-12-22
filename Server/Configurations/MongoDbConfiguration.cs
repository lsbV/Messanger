namespace Server.Configurations;

public class MongoDbConfiguration
{
    public string ConnectionString { get; set; } = string.Empty;
    public string DatabaseName { get; set; } = string.Empty;
    public bool ShowQueries { get; set; }
    public string MessageCollectionName { get; set; } = string.Empty;
}