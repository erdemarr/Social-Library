namespace SocialLibrary.API.Models
{
    public class UserDto
    {
        public int Id { get; set; }
        public string UserName { get; set; } = null!;
        public string Email { get; set; } = null!;

        // Profil bilgileri eklersen buraya
        // public string? Bio { get; set; }
        // public string? AvatarUrl { get; set; }

        public int FollowersCount { get; set; }
        public int FollowingCount { get; set; }
        public bool IsFollowedByCurrentUser { get; set; }  // /me için işe yarar
    }
}
