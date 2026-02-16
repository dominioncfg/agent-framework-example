using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AgentFrameworkExamples;

public static class ConfigurationExtensions
{
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

    public static IServiceProvider BuildServiceProvider()
    {
        var services = new ServiceCollection();

        services.AddTransient<ITransientDependencyTool, TransientDependencyTool>();
        services.AddScoped<IScopedDependencyTool, ScopedDependencyTool>();
        services.AddSingleton<ISingletonDependencyTool, SingletonDependencyTool>();

        return services.BuildServiceProvider();
    }

    public static IConfiguration BuildConfiguration()
    {
        var configuration = new ConfigurationBuilder()
            .AddUserSecrets<Program>()
            .Build();
        return configuration;
    }
}