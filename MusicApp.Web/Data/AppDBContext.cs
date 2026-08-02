using Microsoft.EntityFrameworkCore;
using MusicApp.Web.Models;

namespace MusicApp.Web.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Singer> Singers => Set<Singer>();
    public DbSet<Song> Songs => Set<Song>();
}