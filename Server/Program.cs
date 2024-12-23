using Messenger.ServiceDefaults;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddDatabases(builder.Configuration);

builder.Services.AddChatComponent();
builder.Services.ConfigureJsonSerialization();






var app = builder.Build();

app.MapDefaultEndpoints();


await app.RunAsync();