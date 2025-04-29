using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using SportMotos;
using SportMotos.Models;
using SportMotos.Services;

var builder = WebApplication.CreateBuilder(args);

//Criar a autentica��o para o login
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Login/Login"; // P�gina de login
        options.LogoutPath = "/Login/Logout"; // P�gina de logout
        options.ExpireTimeSpan = TimeSpan.FromHours(1);
        options.SlidingExpiration = true; // Atualiza a expira��o a cada requisi��o
    });

// Adicionando o DbContext e lendo a conex�o do appsettings.json
var connectionString = builder.Configuration.GetConnectionString("BaseDados");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddSession();

// Add services to the container.
builder.Services.AddControllersWithViews();

// Adiciona o EmailService ao cont�iner de servi�os
builder.Services.AddScoped<IEmailService, EmailService>();

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
app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
