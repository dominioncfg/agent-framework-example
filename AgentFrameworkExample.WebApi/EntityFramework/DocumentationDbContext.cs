using Microsoft.EntityFrameworkCore;

namespace AgentFrameworkExample.WebApi;

public class DocumentationDbContext : DbContext
{
    public DocumentationDbContext(DbContextOptions<DocumentationDbContext> options) : base(options)
    {
    }

    public DbSet<DocumentationArticle> DocumentationArticles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<DocumentationArticle>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(500);
            entity.Property(e => e.Content).IsRequired();
            entity.Property(e => e.Embedding).HasColumnType("vector(1536)");
        });
    }
}
