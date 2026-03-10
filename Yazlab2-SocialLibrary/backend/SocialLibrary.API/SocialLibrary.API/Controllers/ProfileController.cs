using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SocialLibrary.API.Data;

namespace SocialLibrary.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // BU ÖNEMLİ: Token zorunlu
    public class ProfileController : ControllerBase
    {
        private readonly SocialLibraryContext _context;

        public ProfileController(SocialLibraryContext context)
        {
            _context = context;
        }

        [HttpGet("me")]
        public async Task<IActionResult> Me()
        {
            // Token içinden kullanıcı id’sini al
            var userIdClaim =
                User.FindFirst(ClaimTypes.NameIdentifier) ??
                User.FindFirst("userId");

            if (userIdClaim == null)
                return Unauthorized("UserId claim yok, token yanlış üretilmiş.");

            var userId = int.Parse(userIdClaim.Value);

            var user = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                return NotFound("Kullanıcı bulunamadı.");

            return Ok(new
            {
                user.Id,
                user.UserName,
                user.Email
            });
        }
    }
}
