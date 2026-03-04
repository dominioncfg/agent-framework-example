using Microsoft.Agents.AI;
using System.Threading.Channels;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

public class ChatProcessorHostedService(IChatService chatService, AIAgent agent, ChannelWriter<ServerMessage> channelWriter) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var session = await agent.CreateSessionAsync(cancellationToken: stoppingToken);
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                if (chatService.HasPendingMessages())
                {
                    var message = chatService.GetNextMessage();

                    List<AgentResponseUpdate> updates = [];
                    await foreach (AgentResponseUpdate update in agent.RunStreamingAsync(message, session))
                    {
                        channelWriter.WriteAsync(new ServerMessage(update.Text), stoppingToken).GetAwaiter().GetResult();
                        updates.Add(update);
                    }

                    AgentResponse response = updates.ToAgentResponse();
                    if (response.Usage != null)
                    {
                        var msg = $"Message Finished - In: {response.Usage.InputTokenCount} - Out: {response.Usage.OutputTokenCount}";
                        channelWriter.WriteAsync(new ServerMessage(msg), stoppingToken).GetAwaiter().GetResult();
                    }
                }
                else
                {
                    // Wait a bit before checking again if there are no messages
                    await Task.Delay(TimeSpan.FromMilliseconds(1000), stoppingToken);
                }
            }
            catch (OperationCanceledException)
            {
                // Expected when the service is stopping
                break;
            }
            catch (Exception ex)
            {
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken); // Wait before retrying
            }
        }
    }
}


public record ServerMessage(string Message);
