using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.AI;
using System.Threading.Channels;

namespace AgentFrameworkExample.WebApi.Controllers
{
    [ApiController]
    [Route("api/chat")]
    public class ChatControllers(IChatService chat) : ControllerBase
    {
        [HttpPost]
        public IActionResult PostMessage(MessageRequest request)
        {
            chat.SendMessageToLlm(request.Message);
            return Ok(new { status = "Message received and sent to LLM." });
        }
    }

    public record MessageRequest(string Message);
}


public static class MessageStream
{
    public static void MapMessagesSSe(this IEndpointRouteBuilder app)
    {
        app.MapGet("/chat/stream", (ChannelReader<ServerMessage> channelReader, CancellationToken cancellationToken) =>
        {
            return Results.ServerSentEvents(channelReader.ReadAllAsync(cancellationToken),"messages");
        });
        
    }
}


public interface IChatService
{
    Task SendMessageToLlm(string message);

    bool HasPendingMessages();

    string GetNextMessage();
}

public class ChatService() : IChatService
{
    public static Queue<string> pendingMessages = new Queue<string>();

    public Task SendMessageToLlm(string message)
    {
        pendingMessages.Enqueue(message);

        return Task.CompletedTask;
    }

    public bool HasPendingMessages()
    {
        return pendingMessages.Count > 0;
    }

    public string GetNextMessage()
    {
        if (pendingMessages.Count == 0)
        {
            throw new InvalidOperationException("No pending messages.");
        }

        return pendingMessages.Dequeue();
    }
}


