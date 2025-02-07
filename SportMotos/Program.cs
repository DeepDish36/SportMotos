using Microsoft.EntityFrameworkCore;
using SportMotos.Models;

var builder = WebApplication.CreateBuilder(args);

// Adicionando o DbContext e lendo a conexão do appsettings.json
var connectionString = builder.Configuration.GetConnectionString("BaseDados");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));


// Add services to the container.
builder.Services.AddControllersWithViews();

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
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
