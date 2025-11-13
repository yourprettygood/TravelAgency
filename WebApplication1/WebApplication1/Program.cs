// File: WebApplication1/WebApplication1/Program.cs
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;    // здесь будет AppDbContext
// Views и модели сами подтянутся через контроллеры

namespace WebApplication1;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // MVC с представлениями и API-контроллерами
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

        // Подключаем контроллеры (MVC + API)
        app.MapControllers();

        // Классический маршрут для MVC: по / откроется Home/Index
        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}"
        );

        app.Run();
    }
}
