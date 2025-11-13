using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _db;

        public AuthController(AppDbContext db)
        {
            _db = db;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { ok = false, error = "Неверные данные" });

            var email = (dto.Email ?? "").Trim();

            var exists = await _db.Users.AnyAsync(u => u.Email == email);
            if (exists)
                return Conflict(new { ok = false, error = "Email уже зарегистрирован" });

            var user = new User
            {
                Name = (dto.Name ?? "").Trim(),
                Email = email,
                Password = dto.Password ?? "",
                CreatedAt = DateTime.UtcNow
            };

            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            await SignInAsync(user);

            return Ok(new
            {
                ok = true,
                message = "Регистрация успешна",
                user = new { user.UserId, user.Name, user.Email }
            });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { ok = false, error = "Неверные данные" });

            var email = (dto.Email ?? "").Trim();
            var pass = dto.Password ?? "";

            var user = await _db.Users
                .FirstOrDefaultAsync(u => u.Email == email && u.Password == pass);

            if (user == null)
                return Unauthorized(new { ok = false, error = "Неверный email или пароль" });

            await SignInAsync(user);

            return Ok(new
            {
                ok = true,
                message = "Вход выполнен",
                user = new { user.UserId, user.Name, user.Email }
            });
        }

        private async Task SignInAsync(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Email, user.Email)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
        }
    }
}
