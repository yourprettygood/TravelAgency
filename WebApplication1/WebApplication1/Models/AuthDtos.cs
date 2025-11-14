// File: WebApplication1/WebApplication1/Models/AuthDtos.cs
using System.ComponentModel.DataAnnotations;
using WebApplication1.Models;

namespace WebApplication1.Models
{
    // DTO для регистрации
    public class RegisterRequest
    {
        [Required]
        public string? Email { get; set; }

        [Required]
        public string? Password { get; set; }

        [Required]
        public string? Name { get; set; }
    }

    // DTO для входа
    public class LoginRequest
    {
        [Required]
        public string? Email { get; set; }

        [Required]
        public string? Password { get; set; }
    }

    // DTO для сообщения (если используется)
    public class MessageRequest
    {
        public string? Text { get; set; }
    }
}
