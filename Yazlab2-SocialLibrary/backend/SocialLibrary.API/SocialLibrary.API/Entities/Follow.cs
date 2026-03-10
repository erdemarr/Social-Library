using System;

namespace SocialLibrary.API.Entities
{
    public class Follow
    {
        public int Id { get; set; }

        // Takip eden
        public int FollowerId { get; set; }
        public User Follower { get; set; } = null!;

        // Takip edilen
        public int FollowedId { get; set; }
        public User Followed { get; set; } = null!;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
