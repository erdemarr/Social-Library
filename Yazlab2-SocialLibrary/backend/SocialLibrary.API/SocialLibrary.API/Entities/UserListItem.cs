using System;

namespace SocialLibrary.API.Entities
{
    public class UserListItem
    {
        public int Id { get; set; }

        public int UserListId { get; set; }
        public UserList UserList { get; set; } = null!;

        // Hangi içerik (Book/Movie)
        public int ContentId { get; set; }
        public Content Content { get; set; } = null!;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
