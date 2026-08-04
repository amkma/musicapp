using Microsoft.EntityFrameworkCore;
using MusicApp.Application.Categories;
using MusicApp.Application.Interfaces;
using MusicApp.Application.Seed;
using MusicApp.Application.Singers;
using MusicApp.Application.Songs;
using MusicApp.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMemoryCache();

// ponytail: Web wires DI for Infrastructure concretes + Application handlers
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<ISongRepository, SongRepository>();
builder.Services.AddScoped<ISingerRepository, SingerRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();

builder.Services.AddScoped<GetSongsHandler>();
builder.Services.AddScoped<SingerHandlers>();
builder.Services.AddScoped<CategoryHandlers>();
builder.Services.AddScoped<GeneratePerfSongsHandler>();

builder.Services.AddControllersWithViews();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();
app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
