//Demo 7 - Shopping Cart; LV

using Demo7.Models;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// enabled the session-based TempData provider

builder.Services.AddMvc().AddSessionStateTempDataProvider();

// add the session service

builder.Services.AddSession(options =>
{
    options.Cookie.Name = ".TaraStore.Session";
    options.IdleTimeout = TimeSpan.FromMinutes(20);
    options.Cookie.IsEssential = true;
});

// register TaraStoreContext class as a service
// the GetConnectionString method is used to get the Connection String Information from appsettings.jason

builder.Services.AddSqlServer<TaraStoreContext>(builder.Configuration.GetConnectionString("TaraStoreConnection"));

// the AddAuthentication and AddCookie methods add cookie authentication as a service

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.AccessDeniedPath = "/Account/Login";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(20);
        options.SlidingExpiration = true;
        options.Cookie.SameSite = SameSiteMode.Strict;
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

// add this statement to use authentication

app.UseAuthentication();

app.UseAuthorization();

// add this statement to allow the session system to automatically asscociate requests with sessions when they arrive from the client.

app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
