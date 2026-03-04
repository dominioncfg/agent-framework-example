using Microsoft.Data.SqlTypes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.AI;
using System.ComponentModel;
using System.Text.Json;
using System.Threading.Channels;

namespace AgentFrameworkExample.WebApi;

public static class RagTools
{
    [Description("Look for documentation in our company related to suggest a software architecture that is approved by our governace team")]
    public static async Task<List<DocumentationArticle>> GetSuggestedArchitecture(
        IServiceProvider serviceProvider,
        [Description("the query about the type of app the user is trying to build")]
        string appArchitecture)
    {
        using var scope = serviceProvider.CreateAsyncScope();
        DocumentationDbContext context = scope.ServiceProvider.GetRequiredService<DocumentationDbContext>();
        IEmbeddingGenerator<string, Embedding<float>> embeddingGenerator = serviceProvider.GetRequiredService<IEmbeddingGenerator<string, Embedding<float>>>();
        var queryEmbedding = await embeddingGenerator.GenerateAsync(appArchitecture);
        var queryVector = new SqlVector<float>(queryEmbedding.Vector);
        var results = await context.DocumentationArticles
         .OrderBy(b => EF.Functions.VectorDistance("cosine", b.Embedding, queryVector))
         .Take(3)
         .ToListAsync();

        return results;
    }


    [Description("When the users wants to create a quest to keep track of its progress to deploy a solution, then this tool is the right fit. never call this without making sure that the solution complies with ours company's governance")]
    public static async Task CreateQuest(IServiceProvider serviceProvider,
        [Description("Describe the project along with the tasks required to make it happen")]
        Quest quest)
    {
        var sseChannelWriter = serviceProvider.GetRequiredService<ChannelWriter<ServerMessage>>();
        var questJson = JsonSerializer.Serialize(quest, new
            JsonSerializerOptions()
        {
            WriteIndented = true,
        });
        Console.WriteLine("Here is your quest");
        Console.WriteLine(questJson);

        await sseChannelWriter.WriteAsync(new ServerMessage("Here is your quest"), default);
        await sseChannelWriter.WriteAsync(new ServerMessage(Environment.NewLine), default);
        await sseChannelWriter.WriteAsync(new ServerMessage(questJson), default);
        await sseChannelWriter.WriteAsync(new ServerMessage(Environment.NewLine), default);
    }

    [Description("Ping")]
    public static string Ping(IServiceProvider provider)
    {
        return "Pong";
    }

}