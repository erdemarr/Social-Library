using System;

namespace SocialLibrary.API.Entities
{
    public enum LibraryEntryType
    {
        Movie = 1,
        Book = 2
    }

    public enum LibraryEntryStatus
    {
        Watched = 1,   // İzledim
        ToWatch = 2,   // İzlenecek
        Read = 3,      // Okudum
        ToRead = 4     // Okunacak
    }

    public class LibraryEntry
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public User User { get; set; } = null!;

        public int ContentId { get; set; }
        public Content Content { get; set; } = null!;

        public LibraryEntryType Type { get; set; }
        public LibraryEntryStatus Status { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
