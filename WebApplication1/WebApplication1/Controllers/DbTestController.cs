// File: WebApplication1/WebApplication1/Controllers/DbTestController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text;
using DALtravelagency;             // для AppDbContext
using DomainTravelAgency.Models;   // для User, CourseProgram и т.д.



namespace WebApplication1.Controllers
{
    public class DbTestController : Controller
    {
        private readonly AppDbContext _db;

        public DbTestController(AppDbContext db)
        {
            _db = db;
        }

        // GET /DbTest
        [HttpGet("/DbTest")]
        public async Task<IActionResult> DbTest()
        {
            var sb = new StringBuilder();

            try
            {
                using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));

                // 1) Проверяем соединение
                var can = await _db.Database.CanConnectAsync(cts.Token);
                sb.AppendLine($"CanConnect={can}");

                try
                {
                    await _db.Database.ExecuteSqlRawAsync("SELECT 1", cts.Token);
                    sb.AppendLine("SELECT 1: OK");
                }
                catch (Exception ex2)
                {
                    sb.AppendLine("SELECT 1 ERROR: " + ex2.Message);
                }

                // 2) Считаем записи в основных таблицах
                // (если таблица пустая — просто будет 0, это не ошибка)
                var usersCount = await _db.Users.CountAsync(cts.Token);
                var programsCount = await _db.Programs.CountAsync(cts.Token);
                var teachersCount = await _db.Teachers.CountAsync(cts.Token);
                var enrollmentsCount = await _db.Enrollments.CountAsync(cts.Token);
                var messagesCount = await _db.Messages.CountAsync(cts.Token);

                sb.AppendLine();
                sb.AppendLine($"Users:        {usersCount}");
                sb.AppendLine($"Programs:     {programsCount}");
                sb.AppendLine($"Teachers:     {teachersCount}");
                sb.AppendLine($"Enrollments:  {enrollmentsCount}");
                sb.AppendLine($"Messages:     {messagesCount}");

                // 3) Для примера покажем один курс/одного пользователя, если есть
                var anyProgram = await _db.Programs
                    .OrderByDescending(p => p.PopularityScore)
                    .FirstOrDefaultAsync(cts.Token);

                if (anyProgram != null)
                {
                    sb.AppendLine();
                    sb.AppendLine("Sample program:");
                    sb.AppendLine($"  Id:    {anyProgram.ProgramId}");
                    sb.AppendLine($"  Title: {anyProgram.Title}");
                    sb.AppendLine($"  Diff:  {anyProgram.Difficulty}");
                }

                var anyUser = await _db.Users
                    .OrderBy(u => u.UserId)
                    .FirstOrDefaultAsync(cts.Token);

                if (anyUser != null)
                {
                    sb.AppendLine();
                    sb.AppendLine("Sample user:");
                    sb.AppendLine($"  Id:    {anyUser.UserId}");
                    sb.AppendLine($"  Name:  {anyUser.Name}");
                    sb.AppendLine($"  Email: {anyUser.Email}");
                }

                return Content(sb.ToString(), "text/plain; charset=utf-8");
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                if (ex.InnerException != null)
                    msg += " | INNER: " + ex.InnerException.Message;

                return Content("DB ERROR: " + msg, "text/plain; charset=utf-8");
            }
        }
    }
}
