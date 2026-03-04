using System.ComponentModel;

namespace AgentFrameworkExample.WebApi;

[Description("A quest describe the project that the user is trying to build along with the tasks it has to do to make it happen")]
public class Quest
{
    [Description("The name of the project")]
    public string Name { get; set; } = "";

    [Description("Describes the list of steps in order to setup the app")]
    public List<QuestTask> Tasks { get; set; } = new List<QuestTask>();
}

[Description("A single operation to be done")]
public class QuestTask
{
    [Description("Describe the operation in a few words")]
    public string Name { get; set; } = "";
}
