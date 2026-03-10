using SocialLibrary.API.Entities;
using SocialLibrary.API.Models;

public interface IActivityService
{
    Task AddActivityAsync(int userId, ActivityType type, int? contentId, string? extraData);

    // BUNU EKLE
    Task<List<ActivityDto>> GetFeedAsync(int userId, int page, int pageSize);
}
