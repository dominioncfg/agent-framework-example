using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlTypes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.AI;

namespace AgentFrameworkExample.WebApi.Controllers;

[ApiController]
[Route("api/search")]
public class SearchControllers(DocumentationDbContext context,
    IEmbeddingGenerator<string, Embedding<float>> embeddingGenerator) : ControllerBase
{
    private const double MaxDistance = 0.90;
    private const int MaxResults = 10;

    [HttpGet]
    public async Task<IActionResult> Search([FromQuery] string query)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return BadRequest("Query is required.");
        }

        var queryEmbedding = await embeddingGenerator.GenerateAsync(query);
        var queryVector = new SqlVector<float>(queryEmbedding.Vector);

        var results = await context.DocumentationArticles
            .Select(b => new
            {
                Article = b,
                Distance = EF.Functions.VectorDistance("cosine", b.Embedding, queryVector)
            })
            .Where(x => x.Distance <= MaxDistance)
            .OrderBy(x => x.Distance)
            .Take(MaxResults)
            .ToListAsync();

        if (results.Count == 0)
        {
            results = await context.DocumentationArticles
                .Select(b => new
                {
                    Article = b,
                    Distance = EF.Functions.VectorDistance("cosine", b.Embedding, queryVector)
                })
                .OrderBy(x => x.Distance)
                .Take(MaxResults)
                .ToListAsync();
        }

        var response = results
            .Select(r => new SearchItemResponse(r.Article.Id, r.Article.Title, r.Distance))
            .ToList();

        return Ok(response);
    }
}

public record SearchItemResponse(Guid Id, string Title, double Distance);

