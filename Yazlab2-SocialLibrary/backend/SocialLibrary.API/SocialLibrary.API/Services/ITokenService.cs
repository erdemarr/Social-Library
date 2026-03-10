using SocialLibrary.API.Entities;

namespace SocialLibrary.API.Services
{
    public interface ITokenService
    {
        string CreateToken(User user);
    }
}
