namespace AgentFrameworkExamples;

public interface ITransientDependencyTool
{
    string GetCurrentDateTime();
}

public class TransientDependencyTool : ITransientDependencyTool
{
    public string GetCurrentDateTime()
    {
        return DateTime.UtcNow.ToString("o");
    }
}

public interface IScopedDependencyTool
{
    string GetCurrentDateTime();
}

public class ScopedDependencyTool : IScopedDependencyTool
{
    public string GetCurrentDateTime()
    {
        return DateTime.UtcNow.ToString("o");
    }
}

public interface ISingletonDependencyTool
{
    string GetCurrentDateTime();
}
public class SingletonDependencyTool : ISingletonDependencyTool
{
    public string GetCurrentDateTime()
    {
        return DateTime.UtcNow.ToString("o");
    }
}
