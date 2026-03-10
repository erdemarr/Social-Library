using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SocialLibrary.API.Data;
using SocialLibrary.API.Entities;

namespace SocialLibrary.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class LibraryController : ControllerBase
    {
        private readonly SocialLibraryContext _context;

        public LibraryController(SocialLibraryContext context)
        {
            _context = context;
        }

        private int GetCurrentUserId()
        {
            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)
                          ?? User.FindFirst("sub");

            if (idClaim == null)
                throw new InvalidOperationException("User id claim not found in token.");

            return int.Parse(idClaim.Value);
        }

        public class SetLibraryStatusModel
        {
            public int ContentId { get; set; }
            public LibraryEntryType Type { get; set; }
            public LibraryEntryStatus Status { get; set; }
        }

        // POST api/library
        // Body: { "contentId": 123, "type": 1, "status": 4 } -> örn: Book + ToRead
        [HttpPost]
        public async Task<IActionResult> SetStatus([FromBody] SetLibraryStatusModel model)
        {
            var userId = GetCurrentUserId();

            var entry = await _context.LibraryEntries
                .FirstOrDefaultAsync(e =>
                    e.UserId == userId &&
                    e.ContentId == model.ContentId);

            if (entry == null)
            {
                entry = new LibraryEntry
                {
                    UserId = userId,
                    ContentId = model.ContentId,
                    Type = model.Type,
                    Status = model.Status
                };
                _context.LibraryEntries.Add(entry);
            }
            else
            {
                entry.Type = model.Type;
                entry.Status = model.Status;
            }

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // GET api/library
        // Kütüphane özetini döner: status’e göre gruplanmış
        [HttpGet]
        public async Task<IActionResult> GetMyLibrary()
        {
            var userId = GetCurrentUserId();

            var entries = await _context.LibraryEntries
                .Include(e => e.Content)
                .Where(e => e.UserId == userId)
                .ToListAsync();

            var result = new
            {
                Watched = entries
                    .Where(e => e.Status == LibraryEntryStatus.Watched)
                    .Select(e => e.Content),
                ToWatch = entries
                    .Where(e => e.Status == LibraryEntryStatus.ToWatch)
                    .Select(e => e.Content),
                Read = entries
                    .Where(e => e.Status == LibraryEntryStatus.Read)
                    .Select(e => e.Content),
                ToRead = entries
                    .Where(e => e.Status == LibraryEntryStatus.ToRead)
                    .Select(e => e.Content),
            };

            return Ok(result);
        }
    }
}
