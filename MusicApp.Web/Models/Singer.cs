using System.ComponentModel.DataAnnotations;
namespace MusicApp.Web.Models;

public class Singer
{
    public int Id {get; set;}

    [Required]
    [StringLength(100)]
    public required string Name {get; set;}

    [StringLength(500)]
    public string? ImageUrl{get; set;}

    public ICollection<Song> Songs {get; set;} = new List<Song>();
}