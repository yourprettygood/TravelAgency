using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;

namespace WebApplication1.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterRequest req)
        {
            if (string.IsNullOrWhiteSpace(req.Email) || string.IsNullOrWhiteSpace(req.Password))
                return BadRequest(new { ok = false, error = "Email и пароль обязательны" });
            return Ok(new { ok = true, message = "Регистрация успешна" });
        }
    /* api/auth/login */
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest req)
        {
            if (string.IsNullOrWhiteSpace(req.Email) || string.IsNullOrWhiteSpace(req.Password))
                return BadRequest(new { ok = false, error = "Email и пароль обязательны" });
            return Ok(new { ok = true, token = "demo-token", user = new { email = req.Email } });
        }
    }
}