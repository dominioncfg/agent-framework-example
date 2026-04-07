using AgentFrameworkExamples;
using Microsoft.Agents.AI;
using Microsoft.Data.SqlTypes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenAI;
using OpenAI.Chat;
using System.ComponentModel;
using System.Text.Json;

const string ModelName = "gpt-4.1-nano";
const string Instructions = "You are a helpful assistant. You are a software architect that can help users trying to create a new app in order to choose the appropriate software architecture that is approved in our company. Answer questions based on the provided context.";

var host = Host.CreateDefaultBuilder()
    .ConfigureServices((context, services) =>
    {
        var connectionString = context.Configuration.GetConnectionString("DefaultConnection")
            ?? "Server=localhost,1433;Database=AgentFrameworkDb;User Id=sa;Password=YourStrong@Passw0rd;TrustServerCertificate=True;";
        services.AddDbContext<DocumentationDbContext>(options =>
            options.UseSqlServer(connectionString));
    })
    .Build();
var configuration = host.Services.GetRequiredService<IConfiguration>();
string apiKey = configuration.GetApiKeyOrExit();
var client = new OpenAIClient(apiKey);

// Requires the database to be migrated and populated first.
// Run Example.03.EfCore with Migrate() and AddData() before running this example.

var internalTools = new List<AITool>
{
    AIFunctionFactory.Create(RagTools.GetSuggestedArchitecture),
    AIFunctionFactory.Create(RagTools.CreateQuest)
};

var agent = client
    .GetChatClient(ModelName)
    .AsAIAgent(
        instructions: Instructions,
        tools: internalTools,
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


public static class RagTools
{
    [Description("Look for documentation in our company related to suggest a software architecture that is approved by our governace team")]
    public static async Task<List<DocumentationArticle>> GetSuggestedArchitecture(IServiceProvider serviceProvider,
        [Description("the query about the type of app the user is trying to build")]
        string appArchitecture)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<DocumentationDbContext>();
        var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
        string apiKey = configuration.GetApiKeyOrExit();
        var client = new OpenAIClient(apiKey);
        var embeddingGenerator = client
            .GetEmbeddingClient("text-embedding-3-small")
            .AsIEmbeddingGenerator();

        var queryEmbedding = await embeddingGenerator.GenerateAsync(appArchitecture);
        var queryVector = new SqlVector<float>(queryEmbedding.Vector);
        var results = await context.DocumentationArticles
            .OrderBy(b => EF.Functions.VectorDistance("cosine", b.Embedding, queryVector))
            .Take(3)
            .ToListAsync();

        return results;
    }

    [Description("When the users wants to create a quest to keep track of its progress to deploy a solution, then this tool is the right fit. never call this without making sure that the solution complies with ours company's governance")]
    public static void CreateQuest(IServiceProvider serviceProvider,
        [Description("Describe the project along with the tasks required to make it happen")]
        Quest quest)
    {
        var questJson = JsonSerializer.Serialize(quest, new JsonSerializerOptions { WriteIndented = true });
        Console.WriteLine("Here is your quest");
        Console.WriteLine(questJson);
    }
}

[Description("A quest describe the project that the user is trying to build along with the tasks it has to do to make it happen")]
public class Quest
{
    [Description("The name of the project")]
    public string Name { get; set; }

    [Description("Describes the list of steps in order to setup the app")]
    public List<QuestTask> Tasks { get; set; }
}

[Description("A single operation to be done")]
public class QuestTask
{
    [Description("Describe the operation in a few words")]
    public string Name { get; set; }
}
