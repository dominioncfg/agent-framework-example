public interface IChatService
{
    Task SendMessageToLlm(string message);

    bool HasPendingMessages();

    string GetNextMessage();
}


public class InMemoryChatService() : IChatService
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