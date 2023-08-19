using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Configuration;
using ProyectoFinalDesarrolloWeb.Models;
using static ProyectoFinalDesarrolloWeb.Controllers.ECommerceController;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(option => {
        option.LoginPath = "/Autenticacion/Index";
        option.AccessDeniedPath = "/Autenticacion/Mensaje";

    });


builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Admin", policy =>
        policy.RequireAuthenticatedUser()
              .RequireClaim("desTipo", "admin"));

    options.AddPolicy("Cliente", policy =>
        policy.RequireAuthenticatedUser()
              .RequireClaim("desTipo", "cliente"));
});


// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IOTimeout = TimeSpan.FromMinutes(20);
});

var app = builder.Build();

app.UseSession();

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
    pattern: "{controller=Autenticacion}/{action=Index}/{id?}");

app.Run();
