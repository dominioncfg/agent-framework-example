using AgentFrameworkExamples;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenAI;
using OpenAI.Chat;
using System.Text.Json.Serialization;

const string ModelName = "gpt-4.1-nano";
const string Instructions = "You are a traveling assistant";

var host = Host.CreateDefaultBuilder().Build();
var configuration = host.Services.GetRequiredService<IConfiguration>();
string apiKey = configuration.GetApiKeyOrExit();
var client = new OpenAIClient(apiKey);
var chatHistoryProvider = new SimpleInMemoryChatHistoryProvider();

var agent = client
    .GetChatClient(ModelName)
    .AsAIAgent(new ChatClientAgentOptions()
    {
        ChatOptions = new ChatOptions
        {
            Instructions = Instructions
        },
        ChatHistoryProvider = chatHistoryProvider,
    });
var session = await agent.CreateSessionAsync();

while (true)
{
    Console.Write("> ");
    string input = Console.ReadLine() ?? "";
    if (string.IsNullOrEmpty(input) || input.ToLower() == "exit")
    {
        break;
    }

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
}
Console.WriteLine("==============================================");

var messages = chatHistoryProvider.GetCurrentMessages(session);
Console.WriteLine("=====");
Console.WriteLine($"Total messages in history: {messages.Count()}");
Console.WriteLine("");

foreach (var message in messages)
{
    Console.WriteLine($"{message.Role}: {message.Text}");
}
Console.WriteLine("=====");

public sealed class SimpleInMemoryChatHistoryProvider : ChatHistoryProvider
{
    private readonly ProviderSessionState<State> _sessionState;

    public SimpleInMemoryChatHistoryProvider(
        Func<AgentSession?, State>? stateInitializer = null,
        string? stateKey = null)
    {
        this._sessionState = new ProviderSessionState<State>(
            stateInitializer ?? (_ => new State()),
            stateKey ?? this.GetType().Name);
    }

    public override string StateKey => this._sessionState.StateKey;

    protected override ValueTask<IEnumerable<Microsoft.Extensions.AI.ChatMessage>> ProvideChatHistoryAsync(InvokingContext context, CancellationToken cancellationToken = default)
    {
        var messages = this._sessionState.GetOrInitializeState(context.Session).Messages;
        Console.WriteLine($"Returning {messages.Count()} messages");
        return new(messages);
    }

    protected override ValueTask StoreChatHistoryAsync(InvokedContext context, CancellationToken cancellationToken = default)
    {
        var state = this._sessionState.GetOrInitializeState(context.Session);

        var allNewMessages = context.RequestMessages.Concat(context.ResponseMessages ?? []);
        state.Messages.AddRange(allNewMessages);

        this._sessionState.SaveState(context.Session, state);

        Console.WriteLine();
        Console.WriteLine($"Messages in store: {state.Messages.Count()}");

        return default;
    }


    public List<Microsoft.Extensions.AI.ChatMessage> GetCurrentMessages(AgentSession? session = null)
    {
        return this._sessionState.GetOrInitializeState(session).Messages;
    }

    public sealed class State
    {
        [JsonPropertyName("messages")]
        public List<Microsoft.Extensions.AI.ChatMessage> Messages { get; set; } = [];
    }
}
