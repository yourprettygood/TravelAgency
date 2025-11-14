using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using DALtravelagency.Interfaces;
using DomainTravelAgency.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;
using DomainUser = DomainTravelAgency.Models.User;

namespace WebApplication1.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAccountStorage _accounts;

        public AuthController(IAccountStorage accounts)
        {
            _accounts = accounts;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { ok = false, error = "Некорректные данные" });

            var email = (dto.Email ?? "").Trim();

            // проверяем, свободен ли email
            var isFree = await _accounts.IsEmailFreeAsync(email);
            if (!isFree)
                return BadRequest(new { ok = false, error = "Пользователь с таким email уже существует" });

            var user = new DomainUser
            {
                Name = (dto.Name ?? "").Trim(),
                Email = email,
                Password = dto.Password ?? "",   // пока без хэша
                CreatedAt = DateTime.UtcNow
            };

            var created = await _accounts.AddAsync(user);

            await SignInAsync(created);

            return Ok(new
            {
                ok = true,
                user = new
                {
                    created.UserId,
                    created.Name,
                    created.Email
                }
            });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { ok = false, error = "Некорректные данные" });

            var email = (dto.Email ?? "").Trim();
            var pass = dto.Password ?? "";

            var user = await _accounts.GetByEmailAsync(email);
            if (user == null || user.Password != pass)
                return BadRequest(new { ok = false, error = "Неверный email или пароль" });

            await SignInAsync(user);

            return Ok(new
            {
                ok = true,
                user = new
                {
                    user.UserId,
                    user.Name,
                    user.Email
                }
            });
        }

        // === ВАЖНО: этот метод ДОЛЖЕН быть внутри класса AuthController ===
        private async Task SignInAsync(DomainUser user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Name, user.Name ?? string.Empty),
                new Claim(ClaimTypes.Email, user.Email ?? string.Empty)
            };

            var claimsIdentity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme
            );

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity)
            );
        }
    }
}
