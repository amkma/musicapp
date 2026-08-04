using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MusicApp.Domain.Entities;

public class Song
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(200)]
    public required string Title { get; set; }

    [Required]
    [ForeignKey(nameof(Category))]
    public int CategoryId { get; set; }

    public Category Category { get; set; } = null!;

    [Required]
    [ForeignKey(nameof(Singer))]
    public int SingerId { get; set; }

    public Singer Singer { get; set; } = null!;
}
