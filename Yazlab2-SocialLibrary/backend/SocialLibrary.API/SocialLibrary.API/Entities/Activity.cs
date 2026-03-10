namespace SocialLibrary.API.Entities
{
    public class Activity
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }

        public int? ContentId { get; set; }
        public Content? Content { get; set; }

        public ActivityType Type { get; set; }

        public string? ExtraData { get; set; }   // örn: rating puanı, review id, list id

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    public enum ActivityType
    {
        Rating = 1,
        Review = 2,
        LibraryAdd = 3,
        LibraryRemove = 4,
        ListAdd = 5,
        ListRemove = 6,
        Follow = 7
    }
}
