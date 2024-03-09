using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TravelNoteDevelop.Data;
var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("TravelNoteDevelopContextConnection") ?? throw new InvalidOperationException("Connection string 'TravelNoteDevelopContextConnection' not found.");

builder.Services.AddDbContext<TravelNoteDevelopContext>(options => options.UseSqlServer(connectionString));
builder.Services.AddControllersWithViews();
builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<TravelNoteDevelopContext>();
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
