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
    [HttpGet]
    public async Task<IActionResult> Search([FromQuery] string query)
    {
        var queryEmbedding = await embeddingGenerator.GenerateAsync(query);
        var queryVector = new SqlVector<float>(queryEmbedding.Vector);
        var results = await context.DocumentationArticles
            .OrderBy(b => EF.Functions.VectorDistance("cosine", b.Embedding, queryVector))
            .Take(3)
            .ToListAsync();

        var response = results.Select(r => new SearchItemResponse(r.Id, r.Title)).ToList();
        return Ok(response);
    }
}

public record SearchItemResponse(Guid Id, string Title);

