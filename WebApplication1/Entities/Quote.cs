using System.ComponentModel.DataAnnotations;

namespace QuoteApi.Entities
{
    public class Quote
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Quote text is required.")]
        public string Text { get; set; } = string.Empty;

        public string? Author { get; set; }

        public int Likes { get; set; } = 0;

        public ICollection<QuoteTag> QuoteTags { get; set; } = new List<QuoteTag>();
    }
}
