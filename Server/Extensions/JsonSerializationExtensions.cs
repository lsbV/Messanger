using System.Text.Json.Serialization;
using Server.Converters;

namespace Server.Extensions;
internal static class JsonSerializationExtensions
{
    public static IServiceCollection ConfigureJsonSerialization(this IServiceCollection services)
    {
        services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(options =>
        {
            options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
            options.SerializerOptions.Converters.Add(new ContentDtoJsonConverter());
        });
        return services;
    }
}