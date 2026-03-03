using AgentFrameworkExamples;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

var host = DependencyInjectionConfigurationExtensions.CreateHost();

var configuration = host.Services.GetRequiredService<IConfiguration>();
var modelConfiguration = host.Services.GetRequiredService<ModelConfiguration>();

//Basic chat loop example
//await Examples.BasicChatLoop(configuration, host.Services);

//Use Tools
//await Examples.InternalToolsExample(configuration, host.Services);

// Use EF Core with Agent Framework and vector search
//await Examples.TestEf(configuration, TestMode.Migrate, host.Services);
//await Examples.TestEf(configuration, TestMode.AddData, host.Services);
//await Examples.TestEf(configuration, TestMode.QueryData, host.Services);


//Rag Agent
//await Examples.TestEf(configuration, TestMode.Migrate, host.Services);
//await Examples.TestEf(configuration, TestMode.AddData, host.Services);
//await Examples.RagChatAgent(configuration, host.Services);


//AI Context Provider example
await Examples.AiContextProvider(configuration, host.Services);