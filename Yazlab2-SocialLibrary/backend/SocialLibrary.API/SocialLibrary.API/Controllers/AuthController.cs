using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SocialLibrary.API.Data;
using SocialLibrary.API.Entities;
using SocialLibrary.API.Models;
using SocialLibrary.API.Services;

namespace SocialLibrary.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly SocialLibraryContext _context;
        private readonly IPasswordService _passwordService;
        private readonly ITokenService _tokenService;

        public AuthController(
            SocialLibraryContext context,
            IPasswordService passwordService,
            ITokenService tokenService)
        {
            _context = context;
            _passwordService = passwordService;
            _tokenService = tokenService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if (await _context.Users.AnyAsync(u => u.Email == model.Email))
                return BadRequest("Email already taken");

            if (await _context.Users.AnyAsync(u => u.UserName == model.UserName))
                return BadRequest("Username already taken");

            var user = new User
            {
                UserName = model.UserName,
                Email = model.Email,
                PasswordHash = _passwordService.HashPassword(model.Password),
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok("Registered successfully");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginModel model)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u =>
                    u.Email == model.UserNameOrEmail ||
                    u.UserName == model.UserNameOrEmail);

            if (user == null)
                return BadRequest("User not found");

            if (!_passwordService.VerifyPassword(model.Password, user.PasswordHash))
                return BadRequest("Wrong password");

            var token = _tokenService.CreateToken(user);

            return Ok(new { token });
        }
    }
}
