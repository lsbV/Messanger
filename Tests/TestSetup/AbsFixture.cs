[assembly: AssemblyFixture(typeof(AbsFixture))]

namespace Tests.TestSetup;

// azure blob storage
public class AbsFixture
{
    public readonly BlobContainerClient ContainerClient;

    public AbsFixture()
    {
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("testsettings.json")
            .Build();
        var connectionString = configuration["AzureStorage:ConnectionString"];
        var blobServiceClient = new BlobServiceClient(connectionString);
        var containerName = configuration["AzureStorage:ContainerName"];
        if (blobServiceClient.GetBlobContainerClient(containerName).Exists())
            blobServiceClient.DeleteBlobContainer(containerName);
        blobServiceClient.CreateBlobContainer(containerName);
        ContainerClient = blobServiceClient.GetBlobContainerClient(containerName);
    }

}