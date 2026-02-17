using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AgentFrameworkExamples;

public static class DependencyInjectionConfigurationExtensions
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

    public static IHost CreateHost()
    {
        return Host.CreateDefaultBuilder()
            .ConfigureAppConfiguration((context, config) =>
            {
                config.AddUserSecrets<Program>();
            })
            .ConfigureServices((context, services) =>
            {
                // Register configuration
                var configuration = context.Configuration;

                // Register DbContext
                var connectionString = configuration.GetConnectionString("DefaultConnection")
                    ?? "Server=localhost,1433;Database=AgentFrameworkDb;User Id=sa;Password=YourStrong@Passw0rd;TrustServerCertificate=True;";

                services.AddDbContext<DocumentationDbContext>(options =>
                    options.UseSqlServer(connectionString));

                // Register custom services
                services.AddTransient<ITransientDependencyTool, TransientDependencyTool>();
                services.AddScoped<IScopedDependencyTool, ScopedDependencyTool>();
                services.AddSingleton<ISingletonDependencyTool, SingletonDependencyTool>();

                // Register application services
                services.AddSingleton<ModelConfiguration>(sp => new ModelConfiguration
                {
                    ModelName = "gpt-4.1-nano",
                    Instructions = "You are a helpful assistant for tourists trying to visit Madrid, no matter what you get asked you don't know about any other region or any other topics"
                });
            })
            .Build();
    }
}