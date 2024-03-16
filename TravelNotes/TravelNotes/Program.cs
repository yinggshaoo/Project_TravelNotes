using Microsoft.EntityFrameworkCore;
using TravelNotes.Models;
using Microsoft.AspNetCore.Identity;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<TravelContext>(
      options => options.UseSqlServer(builder.Configuration.GetConnectionString("TravelConnstring")));
builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<TravelContext>();
builder.Services.AddRazorPages();

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
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

//app.MapControllerRoute(
//    name: "default",
//    pattern: "{controller=PersonalPage}/{action=PersonalPage}/{id?}");
//app.MapControllerRoute(
//    name: "default",
//    pattern: "{controller=Album}/{action=Album}/{id?}");
//app.MapControllerRoute(
//    name: "default",
//    pattern: "{controller=Article}/{action=Draft}/{id?}");
//app.MapControllerRoute(
//    name: "default",
//    pattern: "{controller=Article}/{action=ArticleEdit}/{articleId=8}");
//app.MapControllerRoute(
//    name: "default",
//    pattern: "{controller=Article}/{action=ArticleView}/{articleId=4}");
//app.MapControllerRoute(
//    name: "default",
//    pattern: "{controller=Article}/{action=TestArticle}/{id?}");

app.Run();
