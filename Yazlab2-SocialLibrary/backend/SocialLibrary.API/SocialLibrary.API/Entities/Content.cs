using System;
using System.Collections.Generic;

namespace SocialLibrary.API.Entities
{
    public class Content
    {
        public int Id { get; set; }

        // Kitap mı film mi?
        public ContentType Type { get; set; }

        // Ortak alanlar
        public string Title { get; set; } = null!;
        public string? Description { get; set; }

        // Kitap ise Author, film ise Director doldurulabilir
        public string? Author { get; set; }      // Book için
        public string? Director { get; set; }    // Movie için

        public DateTime? ReleaseDate { get; set; }
        public string? CoverImageUrl { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // NAVIGATIONLAR
        public ICollection<Rating> Ratings { get; set; } = new List<Rating>();
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
    }
}
