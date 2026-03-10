using Microsoft.EntityFrameworkCore;
using SocialLibrary.API.Data;
using SocialLibrary.API.Entities;
using SocialLibrary.API.Models;

public class ActivityService : IActivityService
{
    private readonly SocialLibraryContext _context;

    public ActivityService(SocialLibraryContext context)
    {
        _context = context;
    }

    public async Task AddActivityAsync(int userId, ActivityType type, int? contentId, string? extraData)
    {
        var activity = new Activity
        {
            UserId = userId,
            Type = type,
            ContentId = contentId,
            ExtraData = extraData,
            CreatedAt = DateTime.UtcNow
        };

        _context.Activities.Add(activity);
        await _context.SaveChangesAsync();
    }

    // >>> BURASI YENİ EKLENEN KISIM <<<
    public async Task<List<ActivityDto>> GetFeedAsync(int userId, int page, int pageSize)
    {
        // 1) Kullanıcının takip ettiklerini bul
        var followingIds = await _context.Follows
            .Where(f => f.FollowerId == userId)
            .Select(f => f.FollowedId)
            .ToListAsync();

        // 2) Kendisi de feed’de görünsün
        followingIds.Add(userId);

        // 3) Activity’leri çek
        var query = _context.Activities
            .AsNoTracking()
            .Where(a => followingIds.Contains(a.UserId))
            .OrderByDescending(a => a.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize);

        var result = await query
            .Select(a => new ActivityDto
            {
                Id = a.Id,
                UserId = a.UserId,
                UserName = a.User.UserName,        // Navigation zaten var
                Type = a.Type,
                ContentId = a.ContentId,
                ContentTitle = a.Content != null ? a.Content.Title : null,
                ExtraData = a.ExtraData,
                CreatedAt = a.CreatedAt
            })
            .ToListAsync();

        return result;
    }
}
