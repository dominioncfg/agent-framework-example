using AgentFrameworkExamples;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ModelContextProtocol.Client;
using OpenAI;
using OpenAI.Chat;
using System.ComponentModel;

const string ModelName = "gpt-4.1-nano";
const string Instructions = "You are a helpful assistant for tourists trying to visit Madrid, no matter what you get asked you don't know about any other region or any other topics";

var host = Host.CreateDefaultBuilder()
    .ConfigureServices((_, services) =>
    {
        services.AddTransient<ITransientDependencyTool, TransientDependencyTool>();
        services.AddScoped<IScopedDependencyTool, ScopedDependencyTool>();
        services.AddSingleton<ISingletonDependencyTool, SingletonDependencyTool>();
    })
    .Build();
var configuration = host.Services.GetRequiredService<IConfiguration>();
string apiKey = configuration.GetApiKeyOrExit();
var client = new OpenAIClient(apiKey);

await using McpClient mcpClient = await McpClient.CreateAsync(new HttpClientTransport(new HttpClientTransportOptions
{
    Endpoint = new Uri("https://learn.microsoft.com/api/mcp"),
    TransportMode = HttpTransportMode.StreamableHttp
}));
IList<McpClientTool> mcpTools = await mcpClient.ListToolsAsync();

var internalTools = new List<AIFunction>
{
    AIFunctionFactory.Create(DateTimeTools.GetCurrentDateTime, "get_current_datetime", "Get the current date and time"),
    AIFunctionFactory.Create(MadridTourismTools.GetMadridWeather),
    AIFunctionFactory.Create(MadridTourismTools.GetAttractions),
};

var allTools = internalTools.Concat(mcpTools).ToList().Cast<AITool>().ToList();

var agent = client
    .GetChatClient(ModelName)
    .AsAIAgent(
        instructions: Instructions,
        tools: allTools,
        services: host.Services
    );
var session = await agent.CreateSessionAsync();

while (true)
{
    Console.Write("> ");
    string input = Console.ReadLine() ?? "";
    List<AgentResponseUpdate> updates = [];
    await foreach (AgentResponseUpdate update in agent.RunStreamingAsync(input, session))
    {
        updates.Add(update);
        Console.Write(update);
    }

    AgentResponse response = updates.ToAgentResponse();
    if (response.Usage != null)
    {
        Console.WriteLine();
        Console.WriteLine($"Tokens - In: {response.Usage.InputTokenCount} - Out: {response.Usage.OutputTokenCount}");
    }
    Console.WriteLine("==============================================");
}


public static class DateTimeTools
{
    public static string GetCurrentDateTime() => DateTime.UtcNow.ToString("o");
}

public static class MadridTourismTools
{
    [Description("Gets the current weather in Madrid")]
    public static string GetMadridWeather(IServiceProvider serviceProvider)
    {
        var staticDepenency = serviceProvider.GetRequiredService<ISingletonDependencyTool>();
        var scopedTool = serviceProvider.GetRequiredService<IScopedDependencyTool>();
        var transcientTool = serviceProvider.GetRequiredService<ITransientDependencyTool>();

        Console.WriteLine($"[MadridTourismTools] Current date and time from IStaticDependencyTool: {staticDepenency.GetCurrentDateTime()}");
        Console.WriteLine($"[MadridTourismTools] Current date and time from IScopedDependencyTool: {scopedTool.GetCurrentDateTime()}");
        Console.WriteLine($"[MadridTourismTools] Current date and time from ITransientDependencyTool: {transcientTool.GetCurrentDateTime()}");

        return "Sunny, 25°C";
    }

    [Description("Gets recommended tourist attractions in Madrid")]
    public static string GetAttractions([Description("Type of attraction")] string type = "all")
    {
        return "Prado Museum, Royal Palace, Retiro Park";
    }
}
