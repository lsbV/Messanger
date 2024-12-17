using Database;
using Database.MongoDbConfigurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Core.Events;

namespace Server.Extensions
{
    public static class DatabaseExtensions
    {
        public static IServiceCollection AddDatabases(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSqlDatabase(configuration);
            services.AddMongoDatabase(configuration);

            return services;
        }

        public static void AddSqlDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("SqlDatabase"));
            });

        }

        public static void AddMongoDatabase(this IServiceCollection services, IConfiguration configuration)
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
        }

    }
}