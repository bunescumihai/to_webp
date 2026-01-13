using Microsoft.EntityFrameworkCore;
using NivelAccesDate_CodeFirst.Data;
using ServiceLayer.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddControllersWithViews();

// Add database context from CodeFirst package
builder.Services.AddDbContext<ConversionsDbContext>();

// Add authentication service
builder.Services.AddScoped<AuthService>();

// Add plan service
builder.Services.AddScoped<PlanService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
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

app.MapRazorPages();

app.Run();

app.MapRazorPages();

app.Run();