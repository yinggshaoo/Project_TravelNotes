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
	//���n�J�ɷ|�۰ʾɨ�o�Ӻ��}
	option.LoginPath = new PathString("/Member/fail");
	//�S���v���ɷ|�۰ʾɨ�o�Ӻ��}
	option.AccessDeniedPath = new PathString("/Member/NoAccess");
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

//app.MapControllerRoute(
//	name: "default",
//	pattern: "{controller=Home}/{action=Index}/{id?}");
//app.MapControllerRoute(
//	name: "default",
//	pattern: "{controller=Member}/{action=Logout}/{id?}");
//app.MapControllerRoute(
//	name: "default",
//	pattern: "{controller=Article}/{action=TestArticle}/{id?}");
//app.MapControllerRoute(
//    name: "default",
//    pattern: "{controller=AiRecommend}/{action=Index}/{id?}");

//app.MapControllerRoute(
//    name: "default",
//    pattern: "{controller=Article}/{action=draft}/{id?}");
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Backstage}/{action=Index}/{id?}");
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=PersonalPage}/{action=PersonalPage}/{id?}");


app.Run();
