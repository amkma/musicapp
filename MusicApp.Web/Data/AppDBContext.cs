using Microsoft.EntityFrameworkCore;
using MusicApp.Web.Models;

namespace MusicApp.Web.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Singer> Singers => Set<Singer>();
    public DbSet<Song> Songs => Set<Song>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Song>()
            .HasOne(s => s.Singer)
            .WithMany(s => s.Songs)
            .HasForeignKey(s => s.SingerId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Song>()
            .HasOne(s => s.Category)
            .WithMany(c => c.Songs)
            .HasForeignKey(s => s.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
