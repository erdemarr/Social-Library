using System;
using System.Collections.Generic;

namespace SocialLibrary.API.Entities
{
    public class UserList
    {
        public int Id { get; set; }

        // Liste sahibi
        public int UserId { get; set; }
        public User User { get; set; } = null!;

        // Örn: "Okuma Listem", "Favori Filmlerim"
        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        // Liste herkese açık mı (profilde gözükecek mi?)
        public bool IsPublic { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // Listenin içindeki elemanlar
        public ICollection<UserListItem> Items { get; set; } = new List<UserListItem>();
    }
}
