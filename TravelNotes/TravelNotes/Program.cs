using Microsoft.EntityFrameworkCore;
using TravelNotes.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<TravelContext>(
      options => options.UseSqlServer(builder.Configuration.GetConnectionString("TravelConnstring")));
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

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=AiRecommend}/{action=Index}/{id?}");

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
