using gentFrameworkExample.WebApi;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenAI;
using OpenAI.Chat;

namespace AgentFrameworkExample.WebApi;

public static partial class AgentFactory
{
    public static AIAgent BuildChatClient(IConfiguration configuration, IServiceProvider serviceProvider)
    {
        var modelConfiguration = serviceProvider.GetRequiredService<ModelConfiguration>();
        string apiKey = configuration.GetApiKeyOrExit();
        var client = new OpenAIClient(apiKey);

        var internalTools = new List<AITool>
        {
            AIFunctionFactory.Create(RagTools.GetSuggestedArchitecture),
            AIFunctionFactory.Create(RagTools.CreateQuest)
        };

        var agent = client
            .GetChatClient(modelConfiguration.ModelName)
            .AsAIAgent(
             instructions: "You are a helpful assistant. You are a software architect that can help users trying to create a new app in order to choose the appropriate software architecture that is approved in our company. Answer questions based on the provided context.",
             tools: internalTools,
             services: serviceProvider
            );
        return agent;
    }
}

//var session = await agent.CreateSessionAsync();
//while (true)
//{
//    Console.Write("> ");
//    string input = Console.ReadLine() ?? "";
//    List<AgentResponseUpdate> updates = [];
//    await foreach (AgentResponseUpdate update in agent.RunStreamingAsync(input, session))
//    {
//        updates.Add(update);
//        Console.Write(update);
//    }

//    AgentResponse response = updates.ToAgentResponse();
//    if (response.Usage != null)
//    {
//        Console.WriteLine();
//        Console.WriteLine($"Tokens - In: {response.Usage.InputTokenCount} - Out: {response.Usage.OutputTokenCount}");
//    }
//    Console.WriteLine("==============================================");
//}
