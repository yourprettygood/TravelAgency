namespace WebApplication1.Models
{
    public class RegisterRequest
    {
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? Name { get; set; }
    }


    public class LoginRequest
    {
        public string? Email { get; set; }
        public string? Password { get; set; }
    }

    public class MessageRequest
    {
        public string? Text { get; set; }
    }
}