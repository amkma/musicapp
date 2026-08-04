namespace MusicApp.Domain.Entities;

public class Song
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public int CategoryId { get; set; }
    public Category Category { get; set; } = null!;
    public int SingerId { get; set; }
    public Singer Singer { get; set; } = null!;
}
