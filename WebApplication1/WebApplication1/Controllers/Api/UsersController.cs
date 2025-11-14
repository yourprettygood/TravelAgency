using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DALtravelagency.Interfaces;
using DomainUser = DomainTravelAgency.Models.User;

namespace WebApplication1.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IAccountStorage _accounts;

        public UsersController(IAccountStorage accounts)
        {
            _accounts = accounts;
        }

        // GET: /api/users
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var list = await _accounts.GetAllAsync();
            return Ok(new { ok = true, items = list });
        }

        // GET: /api/users/5
        [HttpGet("{id:long}")]
        public async Task<IActionResult> GetById(long id)
        {
            var user = await _accounts.GetByIdAsync(id);
            if (user == null)
                return NotFound(new { ok = false, error = "Пользователь не найден" });

            return Ok(new { ok = true, item = user });
        }

        // POST: /api/users
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] DomainUser user)
        {
            // В реальном проекте тут должен быть сервис с хэшированием пароля
            var created = await _accounts.AddAsync(user);
            return Ok(new { ok = true, item = created });
        }

        // PUT: /api/users/5
        [HttpPut("{id:long}")]
        public async Task<IActionResult> Update(long id, [FromBody] DomainUser user)
        {
            if (id != user.UserId)
                return BadRequest(new { ok = false, error = "ID в URL и в модели не совпадают" });

            var existing = await _accounts.GetByIdAsync(id);
            if (existing == null)
                return NotFound(new { ok = false, error = "Пользователь не найден" });

            existing.Name = user.Name;
            existing.Email = user.Email;
            existing.Password = user.Password; // пока без хэша, это потом в сервис

            await _accounts.UpdateAsync(existing);
            return Ok(new { ok = true, item = existing });
        }

        // DELETE: /api/users/5
        [HttpDelete("{id:long}")]
        public async Task<IActionResult> Delete(long id)
        {
            var existing = await _accounts.GetByIdAsync(id);
            if (existing == null)
                return NotFound(new { ok = false, error = "Пользователь не найден" });

            await _accounts.DeleteAsync(id);
            return Ok(new { ok = true });
        }
    }
}
