namespace MusicApp.Domain.Entities;

public class Category
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public ICollection<Song> Songs { get; set; } = new List<Song>();
}
