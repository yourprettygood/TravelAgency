// File: WebApplication1/WebApplication1/Program.cs
using Microsoft.EntityFrameworkCore;
using WebApplication1.Models; // тут лежит User из AuthDtos.cs

// ---------------- НАСТРОЙКА ПРИЛОЖЕНИЯ ----------------

var builder = WebApplication.CreateBuilder(args);

// MVC + API (контроллеры с View + твои Api/AuthController, DbTestController и т.д.)
builder.Services.AddControllersWithViews();

// Регистрируем DbContext для PostgreSQL
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Для совместимости со старыми timestamp в Npgsql (по желанию)
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

// Подключаем контроллеры (и MVC, и API)
app.MapControllers();

// Классический маршрут для MVC:
// по адресу / откроется Home/Index (если у тебя есть HomeController с Index)
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
);

// НИКАКОГО Swagger и НИКАКОГО app.MapGet("/", () => "OK");

app.Run();

// ---------------- КОНТЕКСТ БД ----------------

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    // Таблица users — EF знает, как к ней обращаться
    public DbSet<User> Users => Set<User>();

    // Тут потом можно добавить остальные таблицы:
    // public DbSet<Message> Messages => Set<Message>();
    // public DbSet<Teacher> Teachers => Set<Teacher>();
    // и т.д.
}
