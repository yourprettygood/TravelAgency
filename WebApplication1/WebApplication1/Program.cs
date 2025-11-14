using DALtravelagency;
using DALtravelagency.Interfaces;
using DALtravelagency.Storage;
using DomainTravelAgency.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;



namespace WebApplication1;

public class Program
{
    public static void Main(string[] args)
    {

        var builder = WebApplication.CreateBuilder(args);

        // MVC
        builder.Services.AddControllersWithViews();

        // DbContext + PostgreSQL
        builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

        // Репозиторий для аккаунтов (User)
        builder.Services.AddScoped<IAccountStorage, UserStorage>();
        // Настройка для старого поведения таймстампов в Npgsql (чтоб не ругался)
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

        // Куки-аутентификация
        builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                options.LoginPath = "/";
                options.LogoutPath = "/Account/Logout";
            });

        builder.Services.AddAuthorization();

        var app = builder.Build();

        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");

        app.Run();
    }
}
