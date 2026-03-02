using AgentFrameworkExample.WebApi;
using Microsoft.Agents.AI;
using Microsoft.EntityFrameworkCore;
using System.Threading.Channels;

namespace gentFrameworkExample.WebApi;

public static class DependencyInjectionConfigurationExtensions
{
    public static IServiceCollection RegisterAgentsServices(this IServiceCollection services)
    {
        services.AddSingleton(sp => new ModelConfiguration
        {
            ModelName = "gpt-4.1-nano",
        });

        services.AddSingleton(services =>
        {
            var configuration = services.GetRequiredService<IConfiguration>();
            return AgentFactory.BuildChatClient(configuration, services);
        });

        // Register DbContext
        var connectionString = "Server=localhost,1433;Database=AgentFrameworkDb;User Id=sa;Password=YourStrong@Passw0rd;TrustServerCertificate=True;";

        services.AddDbContext<DocumentationDbContext>(options =>
                  options.UseSqlServer(connectionString));

        services.AddSingleton<IChatService, ChatService>();
        services.AddHostedService<ChatProcessorHostedService>();
        var channel =  Channel.CreateUnbounded<ServerMessage>();

        services.AddSingleton(channel);
        services.AddSingleton(channel.Reader);
        services.AddSingleton(channel.Writer);

        return services;
    }


    public static string GetApiKeyOrExit(this IConfiguration configuration)
    {
        string apiKey = configuration["ApiKey"];

        if (apiKey == null)
        {
            Console.WriteLine("Please set your API key in User Secrets. Right click on the project > Manage User Secrets > Add ApiKey");
            Environment.Exit(1);
        }
        return apiKey;
    }
}