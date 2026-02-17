using Microsoft.Agents.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenAI;
using OpenAI.Chat;

namespace AgentFrameworkExamples;

public static partial class Examples
{
    public static async Task BasicChatLoop(IConfiguration configuration, IServiceProvider serviceProvider)
    {
        var modelConfiguration = serviceProvider.GetRequiredService<ModelConfiguration>();
        string apiKey = configuration.GetApiKeyOrExit();
        var client = new OpenAIClient(apiKey);

        var agent = client
            .GetChatClient(modelConfiguration.ModelName)
            .AsAIAgent(modelConfiguration.Instructions);
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
    }
}
