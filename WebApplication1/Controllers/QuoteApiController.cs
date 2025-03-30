using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuoteApi.Entities;

namespace QuoteApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class QuoteApiController : ControllerBase
    {
        private readonly QuotesDbContext _context;

        public QuoteApiController(QuotesDbContext context)
        {
            _context = context;
        }

        // GET: api/quoteapi
        [HttpGet]
        public async Task<IActionResult> GetAllQuotes()
        {
            var quotes = await _context.Quotes
                .Include(q => q.QuoteTags)
                .ThenInclude(qt => qt.Tag)
                .ToListAsync();

            var result = quotes.Select(q => new
            {
                q.Id,
                q.Text,
                q.Author,
                q.Likes,
                Tags = q.QuoteTags.Select(qt => qt.Tag.Name)
            });

            return Ok(result);
        }

        // POST: api/quoteapi
        [HttpPost]
        public async Task<IActionResult> AddQuote([FromBody] QuoteRequest request)
        {
            var quote = new Quote
            {
                Text = request.Text,
                Author = request.Author,
                Likes = 0
            };

            foreach (var tagName in request.Tags.Distinct())
            {
                var tag = await _context.Tags.FirstOrDefaultAsync(t => t.Name == tagName);
                if (tag == null)
                {
                    tag = new Tag { Name = tagName };
                    _context.Tags.Add(tag);
                    await _context.SaveChangesAsync();
                }

                quote.QuoteTags.Add(new QuoteTag { Tag = tag });
            }

            _context.Quotes.Add(quote);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetQuoteById), new { id = quote.Id }, new
            {
                quote.Id,
                quote.Text,
                quote.Author,
                quote.Likes,
                Tags = quote.QuoteTags.Select(qt => qt.Tag.Name)
            });

        }

        // GET: api/quoteapi/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetQuoteById(int id)
        {
            var quote = await _context.Quotes
                .Include(q => q.QuoteTags)
                .ThenInclude(qt => qt.Tag)
                .FirstOrDefaultAsync(q => q.Id == id);

            if (quote == null) return NotFound();

            return Ok(new
            {
                quote.Id,
                quote.Text,
                quote.Author,
                quote.Likes,
                Tags = quote.QuoteTags.Select(qt => qt.Tag.Name)
            });
        }

        // POST: api/quoteapi/{id}/like
        [HttpPost("{id}/like")]
        public async Task<IActionResult> LikeQuote(int id)
        {
            var quote = await _context.Quotes.FindAsync(id);
            if (quote == null) return NotFound();

            quote.Likes++;
            await _context.SaveChangesAsync();
            return Ok(new { quote.Id, quote.Likes });
        }


        // GET: api/quoteapi/popular
        [HttpGet("popular")]
        public async Task<IActionResult> GetTopLikedQuotes()
        {
            var quotes = await _context.Quotes
                .OrderByDescending(q => q.Likes)
                .Take(10)
                .Select(q => new
                {
                    q.Id,
                    q.Text,
                    q.Author,
                    q.Likes
                })
                .ToListAsync();

            return Ok(quotes);
        }

        // PUT: api/quoteapi/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateQuote(int id, [FromBody] QuoteRequest request)
        {
            var quote = await _context.Quotes
                .Include(q => q.QuoteTags)
                .ThenInclude(qt => qt.Tag)
                .FirstOrDefaultAsync(q => q.Id == id);

            if (quote == null) return NotFound();

            quote.Text = request.Text;
            quote.Author = request.Author;

            // Clear existing tags
            _context.QuoteTags.RemoveRange(quote.QuoteTags);

            foreach (var tagName in request.Tags.Distinct())
            {
                var tag = await _context.Tags.FirstOrDefaultAsync(t => t.Name == tagName);
                if (tag == null)
                {
                    tag = new Tag { Name = tagName };
                    _context.Tags.Add(tag);
                    await _context.SaveChangesAsync();
                }

                quote.QuoteTags.Add(new QuoteTag { Tag = tag });
            }

            await _context.SaveChangesAsync();

            return Ok(new
            {
                quote.Id,
                quote.Text,
                quote.Author,
                quote.Likes,
                Tags = quote.QuoteTags.Select(qt => qt.Tag.Name)
            });
        }

        [HttpGet("debug")]
        public async Task<IActionResult> DebugDump()
        {
            var all = await _context.Quotes
                .Include(q => q.QuoteTags)
                .ThenInclude(qt => qt.Tag)
                .ToListAsync();

            return Ok(all.Select(q => new {
                q.Text,
                Tags = q.QuoteTags.Select(qt => qt.Tag.Name)
            }));
        }



        // GET: api/quoteapi/search?tag=funny
        [HttpGet("search")]
        public async Task<IActionResult> SearchByTag([FromQuery] string tag)
        {
            var quotes = await _context.Quotes
                .Where(q => q.QuoteTags.Any(qt => qt.Tag.Name.ToLower() == tag.ToLower()))
                .Select(q => new
                {
                    q.Id,
                    q.Text,
                    q.Author,
                    q.Likes,
                    Tags = q.QuoteTags.Select(qt => qt.Tag.Name)
                })
                .ToListAsync();

            return Ok(quotes);
        }

        // GET: api/quoteapi/tags
        [HttpGet("tags")]
        public async Task<IActionResult> GetAllTags()
        {
            var tags = await _context.Tags
                .Select(t => t.Name)
                .Distinct()
                .OrderBy(t => t)
                .ToListAsync();

            return Ok(tags);
        }



    }

    public class QuoteRequest
    {
        public string Text { get; set; } = string.Empty;
        public string? Author { get; set; }
        public List<string> Tags { get; set; } = new();
    }
}
