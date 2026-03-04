using System.Threading.Channels;


namespace AgentFrameworkExample.WebApi;

public static class DependencyInjectionServerSentEventExtensions
{
    public static void MapMessagesSSe(this IEndpointRouteBuilder app)
    {
        app.MapGet("/chat/stream", (ChannelReader<ServerMessage> channelReader, CancellationToken cancellationToken) =>
        {
            return Results.ServerSentEvents(channelReader.ReadAllAsync(cancellationToken),"messages");
        });
    }
}

