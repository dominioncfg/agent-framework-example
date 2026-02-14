using Microsoft.Agents.AI;
using Microsoft.Extensions.Configuration;
using OpenAI;
using OpenAI.Chat;

namespace AgentFrameworkExamples;

public static partial class Examples
{
    public static async Task BasicChatLoop(IConfiguration configuration)
    {
        string apiKey = configuration.GetApiKeyOrExit();

        var client = new OpenAIClient(apiKey); //Note

        //Create Agent
        var agent = client.GetChatClient("gpt-4.1-nano").AsAIAgent();
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
