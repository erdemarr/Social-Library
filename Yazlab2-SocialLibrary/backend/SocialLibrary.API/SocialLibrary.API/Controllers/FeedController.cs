using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SocialLibrary.API.Data;
using System.Security.Claims;

namespace SocialLibrary.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FeedController : ControllerBase
    {
        private readonly SocialLibraryContext _context;

        public FeedController(SocialLibraryContext context)
        {
            _context = context;
        }

        private int GetUserId()
        {
            return int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        }

        // TAKİP ETTİKLERİNİN FEEDİ
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetFeed()
        {
            int me = GetUserId();

            var followingIds = await _context.Follows
                .Where(f => f.FollowerId == me)
                .Select(f => f.FollowedId)
                .ToListAsync();

            var feed = await _context.Activities
                .Where(a => followingIds.Contains(a.UserId))
                .OrderByDescending(a => a.CreatedAt)
                .Take(50)
                .Select(a => new
                {
                    a.Id,
                    User = a.User.UserName,
                    a.Type,
                    a.ContentId,
                    a.ExtraData,
                    a.CreatedAt
                })
                .ToListAsync();

            return Ok(feed);
        }
    }
}
