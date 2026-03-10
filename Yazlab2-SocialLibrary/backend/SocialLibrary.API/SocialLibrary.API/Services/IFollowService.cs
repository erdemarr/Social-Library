using SocialLibrary.API.Models;

namespace SocialLibrary.API.Services
{
    public interface IFollowService
    {
        Task<bool> FollowAsync(int followerId, int targetUserId);
        Task<bool> UnfollowAsync(int followerId, int targetUserId);
        Task<bool> IsFollowingAsync(int followerId, int targetUserId);

        Task<IReadOnlyList<UserDto>> GetFollowersAsync(int userId);
        Task<IReadOnlyList<UserDto>> GetFollowingAsync(int userId);
    }
}
