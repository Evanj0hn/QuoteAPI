using System.ComponentModel.DataAnnotations;

namespace QuoteApi.Entities
{
    public class Tag
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        public ICollection<QuoteTag> QuoteTags { get; set; } = new List<QuoteTag>();
    }
}
