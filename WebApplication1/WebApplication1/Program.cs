// File: WebApplication1/WebApplication1/Program.cs
using Microsoft.EntityFrameworkCore;
using System;
using WebApplication1.Data;

var builder = WebApplication.CreateBuilder(args);

// Если нужен старый режим timestamp у Npgsql (часто полезно при миграциях)
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

// MVC
builder.Services.AddControllersWithViews();

// EF Core + PostgreSQL
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

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

app.MapControllers();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
