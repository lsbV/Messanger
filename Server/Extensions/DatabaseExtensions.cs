﻿using Azure.Storage.Blobs;
using Database.MongoDbConfigurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Core.Events;
using Server.Configurations;

namespace Server.Extensions;

public static class DatabaseExtensions
{
    public static IServiceCollection AddDatabases(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSqlDatabase(configuration);
        services.AddMongoDatabase(configuration);
        services.AddAzureBlobStorage(configuration);

        return services;
    }

    private static void AddSqlDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseSqlServer(configuration.GetConnectionString("SqlDatabase"));
        });
        using var scope = services.BuildServiceProvider().CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        context.RecreateDatabase();
    }

    private static void AddMongoDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<MongoDbConfiguration>(configuration.GetSection("MongoDbConfiguration"));
        services.AddSingleton<IMongoClient>(sp =>
        {
            var mongoDbConfiguration = sp.GetRequiredService<IOptions<MongoDbConfiguration>>().Value;
            var settings = MongoClientSettings.FromConnectionString(mongoDbConfiguration.ConnectionString);
            if (mongoDbConfiguration.ShowQueries)
            {
                var logger = sp.GetRequiredService<ILogger<MongoClient>>();
                settings.ClusterConfigurator = cb =>
                {
                    cb.Subscribe<CommandStartedEvent>(e =>
                    {
                        logger.LogInformation("MongoDB Command Started: {CommandName} - {Command}", e.CommandName, e.Command.ToJson());
                    });
                };
            }
            var client = new MongoClient(settings);
            client.ConfigureMessageEntity();
            return client;
        });
        services.AddScoped(sp =>
        {
            var settings = sp.GetRequiredService<IOptions<MongoDbConfiguration>>().Value;
            var client = sp.GetRequiredService<IMongoClient>();
            return client.GetDatabase(settings.DatabaseName);
        });
        services.AddScoped(sp =>
        {
            var settings = sp.GetRequiredService<IOptions<MongoDbConfiguration>>().Value;
            var database = sp.GetRequiredService<IMongoDatabase>();
            return database.GetCollection<Message>(settings.MessageCollectionName);
        });
    }

    public static IServiceCollection AddAzureBlobStorage(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<BlobContainerClient>(sp =>
        {
            var connectionString = configuration["AzureStorage:ConnectionString"];
            var containerName = configuration["AzureStorage:ContainerName"];
            var blobServiceClient = new BlobServiceClient(connectionString);
            if (!blobServiceClient.GetBlobContainerClient(containerName).Exists())
            {
                blobServiceClient.CreateBlobContainer(containerName);
            }
            return blobServiceClient.GetBlobContainerClient(containerName);
        });
        return services;
    }

}