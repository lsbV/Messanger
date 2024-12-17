using System.Text.Json.Serialization;
using Server.Converters;
using Server.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddDatabases(builder.Configuration);

builder.Services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
    options.SerializerOptions.Converters.Add(new ContentDtoJsonConverter());
});




var app = builder.Build();

app.MapDefaultEndpoints();


await app.RunAsync();