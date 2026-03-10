using System;
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
    public class ListsController : ControllerBase
    {
        private readonly SocialLibraryContext _context;

        public ListsController(SocialLibraryContext context)
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

        // GET api/lists/me
        // Kendi listelerimi getir
        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> GetMyLists()
        {
            var userId = GetCurrentUserId();

            var lists = await _context.UserLists
                .Where(l => l.UserId == userId)
                .Select(l => new
                {
                    l.Id,
                    l.Name,
                    l.Description,
                    l.IsPublic,
                    ItemsCount = l.Items.Count
                })
                .ToListAsync();

            return Ok(lists);
        }

        // GET api/lists/user/5
        // Başka bir kullanıcının herkese açık listeleri
        [HttpGet("user/{userId:int}")]
        public async Task<IActionResult> GetUserPublicLists(int userId)
        {
            var lists = await _context.UserLists
                .Where(l => l.UserId == userId && l.IsPublic)
                .Select(l => new
                {
                    l.Id,
                    l.Name,
                    l.Description,
                    ItemsCount = l.Items.Count
                })
                .ToListAsync();

            return Ok(lists);
        }

        // GET api/lists/10
        // Liste detay + içerik id’leri
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetList(int id)
        {
            var list = await _context.UserLists
                .Include(l => l.Items)
                .FirstOrDefaultAsync(l => l.Id == id);

            if (list == null)
                return NotFound();

            // Eğer liste private ise ve sahibi değilsen, 403
            if (!list.IsPublic)
            {
                var userId = GetCurrentUserIdOrNull();

                if (userId == null || userId.Value != list.UserId)
                    return Forbid();
            }

            var result = new
            {
                list.Id,
                list.Name,
                list.Description,
                list.IsPublic,
                list.UserId,
                Items = list.Items.Select(li => new
                {
                    li.Id,
                    li.ContentId,
                    li.CreatedAt
                })
            };

            return Ok(result);
        }

        private int? GetCurrentUserIdOrNull()
        {
            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)
                          ?? User.FindFirst("sub");

            if (idClaim == null)
                return null;

            if (int.TryParse(idClaim.Value, out var id))
                return id;

            return null;
        }

        // POST api/lists
        // Yeni liste oluştur
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateList([FromBody] CreateListRequest request)
        {
            var userId = GetCurrentUserId();

            if (string.IsNullOrWhiteSpace(request.Name))
                return BadRequest("Liste adı boş olamaz.");

            var list = new UserList
            {
                UserId = userId,
                Name = request.Name.Trim(),
                Description = request.Description,
                IsPublic = request.IsPublic
            };

            _context.UserLists.Add(list);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetList), new { id = list.Id }, new { list.Id, list.Name });
        }

        // POST api/lists/{id}/items
        // Listeye içerik ekle
        [Authorize]
        [HttpPost("{id:int}/items")]
        public async Task<IActionResult> AddItem(int id, [FromBody] AddListItemRequest request)
        {
            var userId = GetCurrentUserId();

            var list = await _context.UserLists
                .Include(l => l.Items)
                .FirstOrDefaultAsync(l => l.Id == id);

            if (list == null)
                return NotFound("Liste bulunamadı.");

            if (list.UserId != userId)
                return Forbid();

            // İçerik var mı?
            var contentExists = await _context.Contents
                .AnyAsync(c => c.Id == request.ContentId);

            if (!contentExists)
                return BadRequest("Content bulunamadı.");

            // Zaten ekli mi?
            var alreadyExists = list.Items.Any(li => li.ContentId == request.ContentId);
            if (alreadyExists)
                return BadRequest("Bu içerik zaten listede.");

            var item = new UserListItem
            {
                UserListId = list.Id,
                ContentId = request.ContentId
            };

            _context.UserListItems.Add(item);
            await _context.SaveChangesAsync();

            return Ok(new { item.Id, item.ContentId });
        }

        // DELETE api/lists/{listId}/items/{itemId}
        [Authorize]
        [HttpDelete("{listId:int}/items/{itemId:int}")]
        public async Task<IActionResult> RemoveItem(int listId, int itemId)
        {
            var userId = GetCurrentUserId();

            var list = await _context.UserLists
                .FirstOrDefaultAsync(l => l.Id == listId);

            if (list == null)
                return NotFound("Liste bulunamadı.");

            if (list.UserId != userId)
                return Forbid();

            var item = await _context.UserListItems
                .FirstOrDefaultAsync(i => i.Id == itemId && i.UserListId == listId);

            if (item == null)
                return NotFound("Liste elemanı bulunamadı.");

            _context.UserListItems.Remove(item);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }

    public class CreateListRequest
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public bool IsPublic { get; set; } = true;
    }

    public class AddListItemRequest
    {
        public int ContentId { get; set; }
    }
}
