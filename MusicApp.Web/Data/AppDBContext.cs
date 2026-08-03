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

        // ponytail: seed with fixed Ids so FK relationships hold
        modelBuilder.Entity<Category>().HasData(
            new Category { Id = 1, Name = "Pop" },
            new Category { Id = 2, Name = "Rock" },
            new Category { Id = 3, Name = "Jazz" });

        modelBuilder.Entity<Singer>().HasData(
            new Singer { Id = 1, Name = "Adele",       ImageUrl = "https://via.placeholder.com/150?text=Adele" },
            new Singer { Id = 2, Name = "Ed Sheeran",  ImageUrl = "https://via.placeholder.com/150?text=Ed+Sheeran" },
            new Singer { Id = 3, Name = "Aurora",      ImageUrl = "https://via.placeholder.com/150?text=Aurora" },
            new Singer { Id = 4, Name = "Freddie King",ImageUrl = "https://via.placeholder.com/150?text=Freddie+King" });

        modelBuilder.Entity<Song>().HasData(
            new Song { Id = 1,  Title = "Hello",          CategoryId = 1, SingerId = 1 },
            new Song { Id = 2,  Title = "Rolling in Deep",CategoryId = 1, SingerId = 1 },
            new Song { Id = 3,  Title = "Shape of You",   CategoryId = 1, SingerId = 2 },
            new Song { Id = 4,  Title = "Perfect",        CategoryId = 1, SingerId = 2 },
            new Song { Id = 5,  Title = "Runaway",        CategoryId = 2, SingerId = 3 },
            new Song { Id = 6,  Title = "Running to Sea", CategoryId = 2, SingerId = 3 },
            new Song { Id = 7,  Title = "Half Light",     CategoryId = 3, SingerId = 3 },
            new Song { Id = 8,  Title = "Hide Away",      CategoryId = 2, SingerId = 4 },
            new Song { Id = 9,  Title = "You Got What",   CategoryId = 3, SingerId = 4 },
            new Song { Id = 10, Title = "Someone Like You", CategoryId = 1, SingerId = 1 });
    }
}
