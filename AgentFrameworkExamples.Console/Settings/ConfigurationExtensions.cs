
namespace AgentFrameworkExamples;

public record ModelConfiguration
{
    public required string ModelName { get; init; }
    public string? Instructions { get; init; }

}