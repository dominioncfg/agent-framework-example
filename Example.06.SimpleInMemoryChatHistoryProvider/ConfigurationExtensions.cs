using Microsoft.Extensions.Configuration;

namespace AgentFrameworkExamples;

public static class ConfigurationExtensions
{
    public static string GetApiKeyOrExit(this IConfiguration configuration)
    {
        string? apiKey = configuration["ApiKey"];
        if (apiKey == null)
        {
            Console.WriteLine("Please set your API key in User Secrets. Right click on the project > Manage User Secrets > Add ApiKey");
            Environment.Exit(1);
        }
        return apiKey;
    }
}
