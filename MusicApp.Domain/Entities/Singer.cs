using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MusicApp.Domain.Entities;

public class Singer
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public required string Name { get; set; }

    [StringLength(500)]
    public string? ImageUrl { get; set; }

    public ICollection<Song> Songs { get; set; } = new List<Song>();
}
