using ChatComponent.ChatOperations;

internal static class ChatComponentExtesion
{
    public static IServiceCollection AddChatComponent(this IServiceCollection services)
    {
        services.AddMediatR((a) =>
        {
            a.RegisterServicesFromAssemblies(typeof(GetChatByIdHandler).Assembly);
        });

        return services;
    }
}