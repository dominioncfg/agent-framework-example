using gentFrameworkExample.WebApi;

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
app.MapControllers();
app.MapMessagesSSe();
app.Run();
