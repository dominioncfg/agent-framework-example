using Microsoft.Data.SqlTypes;using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.AI;
using System.ComponentModel;
using System.Text.Json;
using System.Threading.Channels;

namespace AgentFrameworkExample.WebApi;

public static class RagTools
{
    [Description("Look for documentation in our company related to suggest a software architecture that is approved by our governace team")]
    public static async Task<List<DocumentationArticle>> GetSuggestedArchitecture(
        DocumentationDbContext context,
        IEmbeddingGenerator<string, Embedding<float>> embeddingGenerator,
        [Description("the query about the type of app the user is trying to build")]
        string appArchitecture)
    {
        var queryEmbedding = await embeddingGenerator.GenerateAsync(appArchitecture);
        var queryVector = new SqlVector<float>(queryEmbedding.Vector);
        var results = await context.DocumentationArticles
         .OrderBy(b => EF.Functions.VectorDistance("cosine", b.Embedding, queryVector))
         .Take(3)
         .ToListAsync();

        return results;
    }


    [Description("When the users wants to create a quest to keep track of its progress to deploy a solution, then this tool is the right fit. never call this without making sure that the solution complies with ours company's governance")]
    public static void CreateQuest(ChannelWriter<ServerMessage> sseChannelWriter,
        [Description("Describe the project along with the tasks required to make it happen")]
        Quest quest)
    {
        var questJson = JsonSerializer.Serialize(quest, new
            JsonSerializerOptions()
        {
            WriteIndented = true,
        });
        Console.WriteLine("Here is your quest");
        Console.WriteLine(questJson);

        sseChannelWriter.WriteAsync(new ServerMessage("Here is your quest"), default).GetAwaiter().GetResult();
        sseChannelWriter.WriteAsync(new ServerMessage(questJson), default).GetAwaiter().GetResult();
    }

}