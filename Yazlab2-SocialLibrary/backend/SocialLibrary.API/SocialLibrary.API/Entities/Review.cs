using System;

namespace SocialLibrary.API.Entities
{
    public class Review
    {
        public int Id { get; set; }

        public string Text { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // FK - User
        public int UserId { get; set; }
        public User User { get; set; } = null!;

        // FK - Content
        public int ContentId { get; set; }
        public Content Content { get; set; } = null!;
    }
}
