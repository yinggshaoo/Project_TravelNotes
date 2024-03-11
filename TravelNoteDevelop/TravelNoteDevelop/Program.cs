using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using TravelNoteDevelop.Data;
using TravelNoteDevelop.Utility;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("TravelNoteDevelopContextConnection") ?? throw new InvalidOperationException("Connection string 'TravelNoteDevelopContextConnection' not found.");

builder.Services.AddDbContext<TravelNoteDevelopContext>(options => options.UseSqlServer(connectionString));
builder.Services.AddControllersWithViews();
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<TravelNoteDevelopContext>().AddDefaultTokenProviders();
builder.Services.AddRazorPages();
builder.Services.AddScoped<IEmailSender, EmailSender>();
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = $"/Identity/Account/Login/";
    options.LogoutPath = $"/Identity/Account/Logout";
    options.AccessDeniedPath = $"/Identity/Account/AccessDenied";
});
var app = builder.Build();


app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
