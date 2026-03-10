using SocialLibrary.API.Entities;

public class User
{
    public int Id { get; set; }

    public string UserName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string PasswordHash { get; set; }

    // Takip ilişkileri
    public ICollection<Follow> Followers { get; set; } = new List<Follow>();   // Beni takip edenler
    public ICollection<Follow> Following { get; set; } = new List<Follow>();   // Benim takip ettiklerim
    public ICollection<Rating> Ratings { get; set; } = new List<Rating>();
    public ICollection<Review> Reviews { get; set; } = new List<Review>();
    // Kullanıcının listeleri (Kütüphanem, İzlenecekler vb.)
    public ICollection<UserList> Lists { get; set; } = new List<UserList>();
}
