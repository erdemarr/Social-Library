using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SocialLibrary.API.Data;
using SocialLibrary.API.Entities;
using SocialLibrary.API.Models;
using SocialLibrary.API.Services;
using System.Security.Claims;

namespace SocialLibrary.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly SocialLibraryContext _context;
        private readonly IFollowService _followService;

        public UsersController(SocialLibraryContext context, IFollowService followService)
        {
            _context = context;
            _followService = followService;
        }

        // /api/users/me  -> mevcut kullanıcı
        [Authorize]
        [HttpGet("me")]
        public async Task<ActionResult<UserDto>> GetMe()
        {
            var userId = GetCurrentUserId();

            var user = await _context.Users
                .Where(u => u.Id == userId)
                .Select(u => new UserDto
                {
                    Id = u.Id,
                    UserName = u.UserName,
                    Email = u.Email
                })
                .FirstOrDefaultAsync();

            if (user == null)
                return NotFound();

            return Ok(user);
        }

        // ----------------------- FOLLOW -----------------------

        // Takip et
        [Authorize]
        [HttpPost("{id:int}/follow")]
        public async Task<IActionResult> Follow(int id)
        {
            var currentUserId = GetCurrentUserId();

            var success = await _followService.FollowAsync(currentUserId, id);
            if (!success)
                return BadRequest("Bu kullanıcıyı takip edemezsin (kendin olamaz veya zaten takip ediyorsun).");

            return Ok();
        }

        // Takipten çık
        [Authorize]
        [HttpDelete("{id:int}/follow")]
        public async Task<IActionResult> Unfollow(int id)
        {
            var currentUserId = GetCurrentUserId();

            var success = await _followService.UnfollowAsync(currentUserId, id);
            if (!success)
                return NotFound("Takip ilişkisi bulunamadı.");

            return Ok();
        }

        // Kullanıcının takipçileri
        [Authorize]
        [HttpGet("{id:int}/followers")]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetFollowers(int id)
        {
            var followers = await _followService.GetFollowersAsync(id);
            return Ok(followers);
        }

        // Kullanıcının takip ettikleri
        [Authorize]
        [HttpGet("{id:int}/following")]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetFollowing(int id)
        {
            var following = await _followService.GetFollowingAsync(id);
            return Ok(following);
        }

        // ------------------------------------------------------

        private int GetCurrentUserId()
        {
            var claim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (claim == null)
                throw new Exception("UserId claim bulunamadı.");

            return int.Parse(claim.Value);
        }
    }
}
