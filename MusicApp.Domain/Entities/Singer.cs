namespace MusicApp.Domain.Entities;

public class Singer
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public string? ImageUrl { get; set; }
    public ICollection<Song> Songs { get; set; } = new List<Song>();
}
