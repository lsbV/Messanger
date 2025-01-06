using FluentValidation.AspNetCore;
using MessageComponent.MessageOperations;
using Messenger.ServiceDefaults;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddDatabases(builder.Configuration);
builder.Services.AddAppServices(builder.Configuration);

builder.Services.AddMediatR((a) =>
    a.RegisterServicesFromAssemblies(typeof(GetChatByIdHandler).Assembly, typeof(SendMessageHandler).Assembly));
builder.Services.ConfigureJsonSerialization();

builder.Services.AddControllers();
builder.Services.AddSignalR();

builder.AddAuthenticationAndAuthorization();
builder.AddCors();

builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);
builder.Services.AddFluentValidationAutoValidation()
    .AddFluentValidationClientsideAdapters();


var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var messageCollection = scope.ServiceProvider.GetRequiredService<IMongoCollection<Message>>();
    await DatabasesSeeder.Seed(context, messageCollection, CancellationToken.None);
}

app.UseCors();
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapDefaultEndpoints();


await app.RunAsync();