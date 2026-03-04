using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.AI;
using OpenAI;
using System.Threading.Channels;

namespace AgentFrameworkExample.WebApi;

public static class DependencyInjectionConfigurationExtensions
{
    const string ModelName = "gpt-4.1-nano";
    const string Instructions = "You are a helpful assistant. You are a software architect that can help users trying to create a new app in order to choose the appropriate software architecture that is approved in our company. Answer questions based on the provided context.";
    const string Description = "An agent that helps users choose the right software architecture for their app based on the provided context.";
    const string EmbeddingModel = "text-embedding-3-small";

    public static WebApplicationBuilder RegisterAgentsServices(this WebApplicationBuilder builder)
    {
        return builder
            .AddCustomAgent()
            .AddDatabase()
            .AddServerSentEventChannels()
            .AddServices();
    }

    private static WebApplicationBuilder AddCustomAgent(this WebApplicationBuilder builder)
    {
        string apiKey = builder.Configuration["ApiKey"]!;
        var openAiClient = new OpenAIClient(apiKey);

        var chatClient = openAiClient.GetChatClient(ModelName).AsIChatClient();
        builder.Services.AddChatClient(chatClient);
        var embeddingGenerator = openAiClient
         .GetEmbeddingClient(EmbeddingModel)
         .AsIEmbeddingGenerator();
        builder.Services.AddEmbeddingGenerator(embeddingGenerator);

        builder.AddAIAgent(
        name: "architect-helper",
        (sp, key) =>
        {
            var chatClient = sp.GetRequiredService<IChatClient>();

            AIAgent agent = new ChatClientAgent(chatClient, Instructions,
                name: key,
                description: Description,
                tools: [
                    AIFunctionFactory.Create(RagTools.GetSuggestedArchitecture),
                    AIFunctionFactory.Create(RagTools.CreateQuest),
                ],
                services:sp
                );
            return agent;
        })
        .WithAITool(AIFunctionFactory.Create(RagTools.GetSuggestedArchitecture))
        .WithAITool(AIFunctionFactory.Create(RagTools.CreateQuest));

        return builder;
    }

    private static WebApplicationBuilder AddDatabase(this WebApplicationBuilder builder)
    {
        var connectionString = "Server=localhost,1433;Database=AgentFrameworkDb;User Id=sa;Password=YourStrong@Passw0rd;TrustServerCertificate=True;";
        builder.Services.AddDbContext<DocumentationDbContext>(options =>
                  options.UseSqlServer(connectionString));

        return builder;
    }

    private static WebApplicationBuilder AddServerSentEventChannels(this WebApplicationBuilder builder)
    {
        var channel = Channel.CreateUnbounded<ServerMessage>();
        builder.Services.AddSingleton(channel);
        builder.Services.AddSingleton(channel.Reader);
        builder.Services.AddSingleton(channel.Writer);
        return builder;
    }

    private static WebApplicationBuilder AddServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddSingleton<IChatService, InMemoryChatService>();
        builder.Services.AddHostedService<ChatProcessorHostedService>();
        return builder;
    }
}