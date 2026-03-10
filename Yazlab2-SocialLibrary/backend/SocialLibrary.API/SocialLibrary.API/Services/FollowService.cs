using Microsoft.EntityFrameworkCore;
using SocialLibrary.API.Data;
using SocialLibrary.API.Entities;
using SocialLibrary.API.Models;

namespace SocialLibrary.API.Services
{
    public class FollowService : IFollowService
    {
        private readonly SocialLibraryContext _context;

        public FollowService(SocialLibraryContext context)
        {
            _context = context;
        }

        public async Task<bool> FollowAsync(int followerId, int targetUserId)
        {
            if (followerId == targetUserId)
                return false; // kendini takip etme yok

            var exists = await _context.Follows
                .AnyAsync(f => f.FollowerId == followerId && f.FollowedId == targetUserId);

            if (exists)
                return false; // zaten takip ediyor

            var follow = new Follow
            {
                FollowerId = followerId,
                FollowedId = targetUserId,
                CreatedAt = DateTime.UtcNow
            };

            _context.Follows.Add(follow);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UnfollowAsync(int followerId, int targetUserId)
        {
            var follow = await _context.Follows
                .FirstOrDefaultAsync(f => f.FollowerId == followerId && f.FollowedId == targetUserId);

            if (follow == null)
                return false;

            _context.Follows.Remove(follow);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> IsFollowingAsync(int followerId, int targetUserId)
        {
            return await _context.Follows
                .AnyAsync(f => f.FollowerId == followerId && f.FollowedId == targetUserId);
        }

        public async Task<IReadOnlyList<UserDto>> GetFollowersAsync(int userId)
        {
            return await _context.Follows
                .Where(f => f.FollowedId == userId)
                .Select(f => new UserDto
                {
                    Id = f.Follower.Id,
                    UserName = f.Follower.UserName,
                    Email = f.Follower.Email
                })
                .ToListAsync();
        }

        public async Task<IReadOnlyList<UserDto>> GetFollowingAsync(int userId)
        {
            return await _context.Follows
                .Where(f => f.FollowerId == userId)
                .Select(f => new UserDto
                {
                    Id = f.Followed.Id,
                    UserName = f.Followed.UserName,
                    Email = f.Followed.Email
                })
                .ToListAsync();
        }
    }
}
