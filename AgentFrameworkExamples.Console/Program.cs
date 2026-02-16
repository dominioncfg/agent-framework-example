using AgentFrameworkExamples;

var configuration = ConfigurationExtensions.BuildConfiguration();
var serviceProvider = ConfigurationExtensions.BuildServiceProvider();

var modelConfiguration = new ModelConfiguration
{
    ModelName = "gpt-4.1-nano",
    Instructions = "You are a helpful assistant for tourists trying to visit Madrid, no matter what you get asked you don't know about any other region or any other topics"
};


//await Examples.BasicChatLoop(configuration, modelConfiguration);

await Examples.InternalToolsExample(configuration, serviceProvider, modelConfiguration);
