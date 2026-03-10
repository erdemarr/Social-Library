using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SocialLibrary.API.Data;
using SocialLibrary.API.Entities;
using SocialLibrary.API.Models;

namespace SocialLibrary.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ContentController : ControllerBase
    {
        private readonly SocialLibraryContext _context;

        public ContentController(SocialLibraryContext context)
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

        // POST api/content
        // Basitçe içerik eklemek için (istersen sonra sadece admin'e kısıtlarız)
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<ContentDetailDto>> CreateContent(CreateContentModel model)
        {
            var content = new Content
            {
                Type = model.Type,
                Title = model.Title,
                Description = model.Description,
                Author = model.Author,
                Director = model.Director,
                ReleaseDate = model.ReleaseDate,
                CoverImageUrl = model.CoverImageUrl,
                CreatedAt = DateTime.UtcNow
            };

            _context.Contents.Add(content);
            await _context.SaveChangesAsync();

            var dto = new ContentDetailDto
            {
                Id = content.Id,
                Type = content.Type,
                Title = content.Title,
                Description = content.Description,
                Author = content.Author,
                Director = content.Director,
                ReleaseDate = content.ReleaseDate,
                CoverImageUrl = content.CoverImageUrl,
                AverageRating = 0,
                RatingsCount = 0,
                MyRating = null
            };

            return CreatedAtAction(nameof(GetContent), new { id = content.Id }, dto);
        }

        // GET api/content
        // Tüm içerikleri listele, isteğe bağlı type ve arama filtresi
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ContentDetailDto>>> GetContents(
            [FromQuery] ContentType? type,
            [FromQuery] string? search)
        {
            var query = _context.Contents
                .Include(c => c.Ratings)
                .AsQueryable();

            if (type.HasValue)
                query = query.Where(c => c.Type == type.Value);

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(c => c.Title.Contains(search));

            var list = await query
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();

            int? currentUserId = null;
            if (User.Identity?.IsAuthenticated == true)
            {
                var idClaim = User.FindFirst(ClaimTypes.NameIdentifier) ?? User.FindFirst("sub");
                if (idClaim != null)
                    currentUserId = int.Parse(idClaim.Value);
            }

            var result = list.Select(c =>
            {
                var avg = c.Ratings.Count > 0 ? c.Ratings.Average(r => r.Score) : 0.0;
                int? myRating = null;

                if (currentUserId.HasValue)
                {
                    myRating = c.Ratings
                        .FirstOrDefault(r => r.UserId == currentUserId.Value)?.Score;
                }

                return new ContentDetailDto
                {
                    Id = c.Id,
                    Type = c.Type,
                    Title = c.Title,
                    Description = c.Description,
                    Author = c.Author,
                    Director = c.Director,
                    ReleaseDate = c.ReleaseDate,
                    CoverImageUrl = c.CoverImageUrl,
                    AverageRating = avg,
                    RatingsCount = c.Ratings.Count,
                    MyRating = myRating
                };
            });

            return Ok(result);
        }

        // GET api/content/{id}
        [HttpGet("{id:int}")]
        public async Task<ActionResult<ContentDetailDto>> GetContent(int id)
        {
            var content = await _context.Contents
                .Include(c => c.Ratings)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (content == null)
                return NotFound();

            int? currentUserId = null;
            if (User.Identity?.IsAuthenticated == true)
            {
                var idClaim = User.FindFirst(ClaimTypes.NameIdentifier) ?? User.FindFirst("sub");
                if (idClaim != null)
                    currentUserId = int.Parse(idClaim.Value);
            }

            var avg = content.Ratings.Count > 0 ? content.Ratings.Average(r => r.Score) : 0.0;
            int? myRating = null;

            if (currentUserId.HasValue)
            {
                myRating = content.Ratings
                    .FirstOrDefault(r => r.UserId == currentUserId.Value)?.Score;
            }

            var dto = new ContentDetailDto
            {
                Id = content.Id,
                Type = content.Type,
                Title = content.Title,
                Description = content.Description,
                Author = content.Author,
                Director = content.Director,
                ReleaseDate = content.ReleaseDate,
                CoverImageUrl = content.CoverImageUrl,
                AverageRating = avg,
                RatingsCount = content.Ratings.Count,
                MyRating = myRating
            };

            return Ok(dto);
        }

        // POST api/content/{id}/rate
        [Authorize]
        [HttpPost("{id:int}/rate")]
        public async Task<IActionResult> RateContent(int id, RateContentModel model)
        {
            if (model.Score < 1 || model.Score > 5)
                return BadRequest("Score 1-5 arasında olmalı.");

            var userId = GetCurrentUserId();

            var contentExists = await _context.Contents.AnyAsync(c => c.Id == id);
            if (!contentExists)
                return NotFound("Content bulunamadı.");

            // User daha önce rate etmişse güncelle, etmemişse ekle
            var rating = await _context.Ratings
                .FirstOrDefaultAsync(r => r.UserId == userId && r.ContentId == id);

            if (rating == null)
            {
                rating = new Rating
                {
                    UserId = userId,
                    ContentId = id,
                    Score = model.Score,
                    CreatedAt = DateTime.UtcNow
                };
                _context.Ratings.Add(rating);
            }
            else
            {
                rating.Score = model.Score;
            }

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // POST api/content/{id}/review
        [Authorize]
        [HttpPost("{id:int}/review")]
        public async Task<IActionResult> AddReview(int id, CreateReviewModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Text))
                return BadRequest("Review boş olamaz.");

            var userId = GetCurrentUserId();

            var contentExists = await _context.Contents.AnyAsync(c => c.Id == id);
            if (!contentExists)
                return NotFound("Content bulunamadı.");

            var review = new Review
            {
                UserId = userId,
                ContentId = id,
                Text = model.Text,
                CreatedAt = DateTime.UtcNow
            };

            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // GET api/content/{id}/reviews
        [HttpGet("{id:int}/reviews")]
        public async Task<ActionResult<IEnumerable<object>>> GetReviews(int id)
        {
            var exists = await _context.Contents.AnyAsync(c => c.Id == id);
            if (!exists)
                return NotFound("Content bulunamadı.");

            var reviews = await _context.Reviews
                .Where(r => r.ContentId == id)
                .Include(r => r.User)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

            var result = reviews.Select(r => new
            {
                r.Id,
                r.Text,
                r.CreatedAt,
                User = new
                {
                    r.User.Id,
                    r.User.UserName
                }
            });

            return Ok(result);
        }
    }
}
