using SocialLibrary.API.Entities;

namespace SocialLibrary.API.Models
{
    public class ActivityDto
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public string UserName { get; set; }  // User.UserName

        public ActivityType Type { get; set; }

        public int? ContentId { get; set; }   // kitap / film
        public string? ContentTitle { get; set; }

        public string? ExtraData { get; set; }     // rating değeri, reviewId, listId vs.

        public DateTime CreatedAt { get; set; }
    }
}
