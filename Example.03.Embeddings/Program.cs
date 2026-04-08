using AgentFrameworkExamples;
using Microsoft.Data.SqlTypes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenAI;

var host = Host.CreateDefaultBuilder()
    .ConfigureServices((context, services) =>
    {
        var connectionString = context.Configuration.GetConnectionString("DefaultConnection");
        services.AddDbContext<DocumentationDbContext>(options =>
            options.UseSqlServer(connectionString));
    })
    .Build();
var configuration = host.Services.GetRequiredService<IConfiguration>();
string apiKey = configuration.GetApiKeyOrExit();
var client = new OpenAIClient(apiKey);

var embeddingGenerator = client
    .GetEmbeddingClient("text-embedding-3-small")
    .AsIEmbeddingGenerator();

// Step 1: Run once to create the database schema
await Migrate(host.Services);

// Step 2: Run once to populate the database with embeddings
await IndexData(apiKey, host.Services);

// Step 3: Query the database using vector similarity search
await SimilaritySearch(apiKey, host.Services);


async Task Migrate(IServiceProvider serviceProvider)
{
    Console.WriteLine("Ensuring database is created and up to date with the latest migrations...");
    using var scope = serviceProvider.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<DocumentationDbContext>();
    await context.Database.MigrateAsync();
}

async Task IndexData(string apiKey, IServiceProvider serviceProvider)
{
    using var scope = serviceProvider.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<DocumentationDbContext>();
    

    Console.WriteLine("Adding data with embeddings to the database...");
    foreach (var article in TestData.Articles)
    {
        if (!await context.DocumentationArticles.AnyAsync(a => a.Title == article.Title))
        {
            var embedding = await embeddingGenerator.GenerateAsync(article.Content);
            article.Embedding = new SqlVector<float>(embedding.Vector);
            context.DocumentationArticles.Add(article);
            await context.SaveChangesAsync();
        }
    }
}

async Task SimilaritySearch(string apiKey, IServiceProvider serviceProvider)
{
    using var scope = serviceProvider.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<DocumentationDbContext>();

    Console.WriteLine("About what technology you are interested in?");
    var rawQuery = Console.ReadLine();
    Console.WriteLine("Querying data from the database...");

    var queryEmbedding = await embeddingGenerator.GenerateAsync(rawQuery!);
    var queryVector = new SqlVector<float>(queryEmbedding.Vector);
    var results = await context.DocumentationArticles
        .OrderBy(b => EF.Functions.VectorDistance("cosine", b.Embedding, queryVector))
        .Take(3)
        .ToListAsync();

    Console.WriteLine("Top 3 relevant articles:");
    foreach (var result in results)
    {
        Console.WriteLine("=====");
        Console.WriteLine($"Title: {result.Title}");
        Console.WriteLine($"Content: {result.Content}");
        Console.WriteLine("=====");
    }
}
