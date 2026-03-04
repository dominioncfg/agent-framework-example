using AgentFrameworkExample.WebApi;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);
builder
    .RegisterAgentsServices()
    .Services
    .AddOpenApi()
    .AddCors(options => options
        .AddDefaultPolicy(policy => policy
        .AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()))
    .AddControllers();
var app = builder.Build();

app.UseCors();
app.MapOpenApi();
app.MapScalarApiReference();
app.MapControllers();
app.MapMessagesSSe();
app.Run();
