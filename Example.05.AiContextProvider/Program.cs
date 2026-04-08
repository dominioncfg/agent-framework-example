using AgentFrameworkExamples;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenAI;
using OpenAI.Chat;

const string ModelName = "gpt-4.1-nano";

var host = Host.CreateDefaultBuilder().Build();
var configuration = host.Services.GetRequiredService<IConfiguration>();
string apiKey = configuration.GetApiKeyOrExit();
var client = new OpenAIClient(apiKey);

List<string> usersFacts = [];

ChatClientAgent memoryExtractorAgent = client
    .GetChatClient(ModelName)
    .AsAIAgent(
        instructions: "Look at the user's message and extract any memory that we do not already know (or non if there aren't any memories to store)"
    );

var agent = client
    .GetChatClient(ModelName)
    .AsAIAgent(new ChatClientAgentOptions()
    {
        ChatOptions = new ChatOptions
        {
            Instructions = "Prefix all messages with 👋👋👋"
        },
        AIContextProviders =
        [
            new MyAIContextProvider(memoryExtractorAgent, usersFacts),
        ]
    });
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
        Console.WriteLine($"Facts: {string.Join(",", usersFacts)}");
    }
    Console.WriteLine("==============================================");
}


class MyAIContextProvider : AIContextProvider
{
    private readonly ChatClientAgent memoryExtractorAgent;
    private readonly List<string> userFacts;

    public MyAIContextProvider(ChatClientAgent memoryExtractorAgent, List<string> userFacts)
    {
        this.memoryExtractorAgent = memoryExtractorAgent;
        this.userFacts = userFacts;
    }

    /* Order of an AIContextProvider when an 'RunAsync' method is being executed
     *
     * - InvokingCoreAsync --> ProvideAIContextAsync
     * - LLM Call
     * - InvokedCoreAsync --> StoreAIContextAsync
     *
     */

    // Pre LLM Call (Enrichment)
    protected override ValueTask<AIContext> ProvideAIContextAsync(InvokingContext context, CancellationToken cancellationToken = default)
    {
        /* Use this to do the following:
         * - Inject additional instructions (for this one LLM Call)
         * - Inject additional tools (for this one LLM Call)
         * - Inject additional message (that unlike the two above will become part of chat-history)
         */

        return ValueTask.FromResult(new AIContext
        {
            Instructions = "Speak like a pirate",
            Tools = [],
        });
    }

    // Post LLM Call (Leverage LLM Call Result for something)
    protected override async ValueTask StoreAIContextAsync(InvokedContext context, CancellationToken cancellationToken = default)
    {
        /* Use this to do the following:
         * - Extract information out of the response in a centralized structured manner (aka example storing memory)
         * - Deal with exceptions
         */

        Microsoft.Extensions.AI.ChatMessage lastMessageFromUser = context.RequestMessages.Last();
        List<Microsoft.Extensions.AI.ChatMessage> inputToMemoryExtractor =
        [
            new(ChatRole.Assistant, $"We know the following about the user already and should not extract that again: {string.Join(" | ", userFacts)}"),
            lastMessageFromUser
        ];

        AgentResponse<MemoryUpdate> response = await memoryExtractorAgent.RunAsync<MemoryUpdate>(inputToMemoryExtractor, cancellationToken: cancellationToken);
        
        foreach (string memoryToRemove in response.Result.MemoryToRemove ?? [])
        {
            userFacts.Remove(memoryToRemove);
        }

        userFacts.AddRange(response.Result.MemoryToAdd ?? []);
    }

    private record MemoryUpdate(List<string> MemoryToAdd, List<string> MemoryToRemove);
}
