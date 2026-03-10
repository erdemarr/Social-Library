using System;
using SocialLibrary.API.Entities;

namespace SocialLibrary.API.Models
{
    public class CreateContentModel
    {
        public ContentType Type { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public string? Author { get; set; }
        public string? Director { get; set; }
        public DateTime? ReleaseDate { get; set; }
        public string? CoverImageUrl { get; set; }
    }

    public class ContentDetailDto
    {
        public int Id { get; set; }
        public ContentType Type { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public string? Author { get; set; }
        public string? Director { get; set; }
        public DateTime? ReleaseDate { get; set; }
        public string? CoverImageUrl { get; set; }

        public double AverageRating { get; set; }
        public int RatingsCount { get; set; }

        public int? MyRating { get; set; }  // kullanıcı login ise
    }
}
