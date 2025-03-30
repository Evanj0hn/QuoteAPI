using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace QuoteApi.Entities
{
    public class QuotesDbContext : IdentityDbContext<User>
    {
        public QuotesDbContext(DbContextOptions<QuotesDbContext> options) : base(options) { }

        public DbSet<Quote> Quotes => Set<Quote>();
        public DbSet<Tag> Tags => Set<Tag>();
        public DbSet<QuoteTag> QuoteTags => Set<QuoteTag>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Many-to-many linking config
            modelBuilder.Entity<QuoteTag>()
                .HasKey(qt => new { qt.QuoteId, qt.TagId });

            modelBuilder.Entity<QuoteTag>()
                .HasOne(qt => qt.Quote)
                .WithMany(q => q.QuoteTags)
                .HasForeignKey(qt => qt.QuoteId);

            modelBuilder.Entity<QuoteTag>()
                .HasOne(qt => qt.Tag)
                .WithMany(t => t.QuoteTags)
                .HasForeignKey(qt => qt.TagId);
        }
    }
}
