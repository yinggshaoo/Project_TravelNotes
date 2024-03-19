using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TravelNotes.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<TravelContext>(
      options => options.UseSqlServer(builder.Configuration.GetConnectionString("TravelConnstring")));

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(option =>
{
	//未登入時會自動導到這個網址
	option.LoginPath = new PathString("/Member/fail");
	//沒有權限時會自動導到這個網址
	option.AccessDeniedPath = new PathString("/Member/Login");
	//option.ExpireTimeSpan = TimeSpan.FromSeconds(2);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Article}/{action=draft}/{id?}");
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=PersonalPage}/{action=PersonalPage}/{id?}");
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=AiRecommend}/{action=Index}/{id?}");

app.Run();
