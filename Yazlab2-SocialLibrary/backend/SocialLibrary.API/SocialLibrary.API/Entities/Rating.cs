using System;

namespace SocialLibrary.API.Entities
{
    public class Rating
    {
        public int Id { get; set; }

        // 1–5 arası puan, kontrolü controller’da yapacağız
        public int Score { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // FK - User
        public int UserId { get; set; }
        public User User { get; set; } = null!;

        // FK - Content
        public int ContentId { get; set; }
        public Content Content { get; set; } = null!;
    }
}
