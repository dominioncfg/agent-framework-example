using AgentFrameworkExamples;
using Microsoft.Agents.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenAI;
using OpenAI.Chat;

const string ModelName = "gpt-4.1-nano";
const string Instructions = "You are a helpful assistant for tourists trying to visit Madrid, no matter what you get asked you don't know about any other region or any other topics";

var host = Host.CreateDefaultBuilder().Build();
var configuration = host.Services.GetRequiredService<IConfiguration>();
string apiKey = configuration.GetApiKeyOrExit();
var client = new OpenAIClient(apiKey);

var agent = client
    .GetChatClient(ModelName)
    .AsAIAgent(Instructions);
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
