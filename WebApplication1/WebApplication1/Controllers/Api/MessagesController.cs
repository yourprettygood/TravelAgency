using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;

namespace WebApplication1.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class MessagesController : ControllerBase
    {
        [HttpPost]
        public IActionResult Send([FromBody] MessageRequest req)
        {
            if (string.IsNullOrWhiteSpace(req.Text))
                return BadRequest(new { ok = false, error = "Текст сообщения пуст" });

            return Ok(new { ok = true, id = Guid.NewGuid(), text = req.Text, ts = DateTime.UtcNow });
        }
    }
}
