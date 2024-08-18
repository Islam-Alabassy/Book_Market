using DataAccess;
using DataAccess.Repository.IRepository;
using DataAccess.Repository;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Identity;
using Utilities;
using Microsoft.AspNetCore.Identity.UI.Services;
using Models.Models;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorPages();
// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<MainDbContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString(name: "DefaultConnection")));

builder.Services.AddIdentity<IdentityUser,IdentityRole>().AddEntityFrameworkStores<MainDbContext>().AddDefaultTokenProviders();
//builder.Services.Configure<IISServerOptions>(options=>
//     options.MaxRequestBodySize = 10485760); // 10 MB)
//Add Custom service to the services container
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IEmailSender, EmailSender>();
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = $"/Identity/Account/Login";
    options.LogoutPath = $"/Identity/Account/Logout";
    options.AccessDeniedPath = $"/Identity/Account/AccessDenied";
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
app.MapRazorPages();
app.MapControllerRoute(
    name: "default",
    pattern: "{area=Customer}/{controller=Home}/{action=Index}/{id?}");

app.Run();
